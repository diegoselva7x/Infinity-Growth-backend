using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
// using Microsoft.Extensions.Configuration; // No parece usarse directamente aquí
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// using System.Text.Json; // No parece usarse directamente aquí (el SDK maneja JSON)
using System.Globalization; // Necesario para CultureInfo.InvariantCulture
using AppLogic;
using AppLogic.Services;
using DTO;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using HttpClient = System.Net.Http.HttpClient;
using JsonSerializer = System.Text.Json.JsonSerializer;

// ------------------------------------------

public class PayPalService
{
    private readonly PayPalHttpClient _client;
    private readonly IOptions<PayPalOptions> _options;
    private readonly string _baseUrl; // Necesitarás inicializar esto (ej. desde IConfiguration)
    private readonly ILogger<PayPalService> _logger;
    private readonly HttpClient _httpClient;

    public PayPalService(IOptions<PayPalOptions> options, ILogger<PayPalService> logger, IConfiguration configuration) // Añadir IConfiguration
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = new HttpClient();

        // Validar opciones
        if (string.IsNullOrWhiteSpace(_options.Value.ClientId) ||
            string.IsNullOrWhiteSpace(_options.Value.ClientSecret) ||
            string.IsNullOrWhiteSpace(_options.Value.Environment))
        {
            _logger.LogError("PayPal ClientId, ClientSecret o Environment no están configurados.");
            throw new InvalidOperationException("Configuración de PayPal incompleta.");
        }

        // Configurar el cliente PayPal SDK
        PayPalEnvironment environment;
        if (_options.Value.Environment.Equals("Sandbox", StringComparison.OrdinalIgnoreCase))
        {
            environment = new SandboxEnvironment(_options.Value.ClientId, _options.Value.ClientSecret);
        }
        else if (_options.Value.Environment.Equals("Live", StringComparison.OrdinalIgnoreCase))
        {
            environment = new LiveEnvironment(_options.Value.ClientId, _options.Value.ClientSecret);
        }
        else
        {
            _logger.LogError("Valor inválido para PayPal:Environment. Usar 'Sandbox' o 'Live'. Valor recibido: {Environment}", _options.Value.Environment);
            throw new InvalidOperationException("Configuración de entorno PayPal inválida.");
        }

        _client = new PayPalHttpClient(environment);
        _logger.LogInformation("Cliente PayPal SDK inicializado para el entorno: {Environment}", _options.Value.Environment);

        // --- Obtener BaseUrl (MUY IMPORTANTE para URLs de retorno) ---
        // Asume que tienes la URL base de tu API/Web en appsettings.json
        _baseUrl = configuration["Application:BaseUrl"] ?? "https://infinitygrowth-ui-cpgzdrcrfee9cnht.eastus-01.azurewebsites.net"; // O tu puerto/URL por defecto
        if (string.IsNullOrWhiteSpace(_baseUrl))
        {
            _logger.LogWarning("Application:BaseUrl no está configurada en appsettings.json. Usando default: {DefaultBaseUrl}", _baseUrl);
        }
        _logger.LogInformation("URL base para redirecciones PayPal: {BaseUrl}", _baseUrl);
    }


    // --- MÉTODO CreateOrderAsync CORREGIDO ---
    /// <summary>
    /// Crea una orden en PayPal.
    /// </summary>
    /// <param name="amount">Monto de la orden.</param>
    /// <param name="currencyCode">Código de moneda (ej. "USD").</param>
    /// <returns>Un objeto PayPalCreateOrderResult con OrderId y ApproveUrl, o null si falla.</returns>
    public async Task<PayPalCreateOrderResult?> CreateOrderAsync(decimal amount, string currencyCode) // <-- Firma cambiada
    {
        var orderRequest = new OrderRequest()
        {
            // Intención: CAPTURE significa que el dinero se captura inmediatamente después de la aprobación.
            // AUTHORIZE solo reserva los fondos, necesitarías una llamada separada para capturar.
            CheckoutPaymentIntent = "CAPTURE",
            PurchaseUnits = new List<PurchaseUnitRequest>()
            {
                new PurchaseUnitRequest()
                {
                    AmountWithBreakdown = new AmountWithBreakdown()
                    {
                        CurrencyCode = currencyCode,
                        // Formatear a 2 decimales usando punto como separador decimal
                        Value = amount.ToString("F2", CultureInfo.InvariantCulture)
                    }
                    // Puedes añadir 'description', 'custom_id', 'invoice_id' aquí si los necesitas
                }
            },
            ApplicationContext = new ApplicationContext()
            {
                // URLs a las que PayPal redirigirá al usuario DESPUÉS de aprobar o cancelar en su sitio.
                // Deben ser URLs ABSOLUTAS y accesibles públicamente si PayPal necesita llamarlas (depende del flujo).
                // Aquí asumimos que son rutas dentro de tu frontend/backend que manejarán el resultado.
                ReturnUrl = $"{_baseUrl}/Cliente/Redirect", // Ejemplo: URL que tu frontend/backend maneja al aprobar
                CancelUrl = $"{_baseUrl}/Cliente/Redirect",  // Ejemplo: URL que tu frontend/backend maneja al cancelar
                BrandName = "Infinity Growth", // Nombre que ve el usuario en PayPal
                LandingPage = "LOGIN", // O "BILLING" - Página de PayPal que se muestra
                UserAction = "PAY_NOW" // Texto del botón final en PayPal
            }
        };

        var request = new OrdersCreateRequest();
        request.Prefer("return=representation"); // Pide a PayPal que devuelva la orden completa en la respuesta
        request.RequestBody(orderRequest);

        try
        {
            _logger.LogInformation("Enviando solicitud para crear orden PayPal por {Amount} {Currency}", amount, currencyCode);
            // Ejecutar la solicitud
            var response = await _client.Execute(request);
            var result = response.Result<Order>(); // Deserializar la respuesta JSON a un objeto Order

            _logger.LogInformation("Orden PayPal creada con ID: {OrderId}, Estado: {Status}", result.Id, result.Status);

            // Extraer los datos necesarios para el controlador/frontend
            string orderId = result.Id;

            // Buscar el link de aprobación en la respuesta
            LinkDescription? approveLink = result.Links?.FirstOrDefault(link => link.Rel.Equals("approve", StringComparison.OrdinalIgnoreCase));

            if (approveLink == null || string.IsNullOrEmpty(approveLink.Href))
            {
                _logger.LogError("Orden PayPal {OrderId} creada pero no se encontró el link 'approve'. Respuesta Links: {@Links}", orderId, result.Links);
                // Devolver null indica fallo al controlador
                return null;
            }

            string approveUrl = approveLink.Href;
            _logger.LogInformation("URL de aprobación para Orden {OrderId}: {ApproveUrl}", orderId, approveUrl);

            // Devolver el resultado exitoso
            return new PayPalCreateOrderResult
            {
                OrderId = orderId,
                ApproveUrl = approveUrl
            };
        }
        catch (HttpException httpEx) // Error específico del SDK para respuestas HTTP no exitosas
        {
            var statusCode = httpEx.StatusCode;
            var message = httpEx.Message; // Puede contener detalles en JSON
            _logger.LogError(httpEx, "Error HTTP {StatusCode} de PayPal al crear orden. Debug Info: {Message}", statusCode, message);
            // Podrías intentar parsear 'message' para obtener detalles más específicos si es necesario.
            return null; // Indicar fallo
        }
        catch (Exception ex) // Otros errores inesperados
        {
            _logger.LogError(ex, "Error inesperado al crear orden PayPal.");
            return null; // Indicar fallo
        }
    }


    // --- MÉTODO CaptureOrderAsync (como lo tenías, parece correcto) ---
    public async Task<PayPalCapture> CaptureOrderAsync(string orderId)
    {
        if (string.IsNullOrWhiteSpace(orderId))
        {
            _logger.LogWarning("Intento de capturar orden con OrderId nulo o vacío.");
            return new PayPalCapture { Success = false, FailureMessage = "Se requiere el ID de la orden de PayPal." };
        }

        var request = new OrdersCaptureRequest(orderId);
        request.RequestBody(new OrderActionRequest()); // Cuerpo vacío necesario

        try
        {
            _logger.LogInformation("Enviando solicitud para capturar orden PayPal {OrderId}", orderId);
            var response = await _client.Execute(request);
            var result = response.Result<Order>(); // Deserializa la respuesta

            if (result != null && result.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Orden PayPal {OrderId} capturada exitosamente según estado COMPLETED.", result.Id);
                Capture? captureInfo = result.PurchaseUnits?.FirstOrDefault()?.Payments?.Captures?.FirstOrDefault();

                if (captureInfo != null && captureInfo.Amount != null &&
                    decimal.TryParse(captureInfo.Amount.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal capturedAmount))
                {
                    string currency = captureInfo.Amount.CurrencyCode;
                    string captureId = captureInfo.Id;
                    _logger.LogInformation("Detalles de captura para Orden {OrderId}: Monto={Amount} {Currency}, CaptureId={CaptureId}", result.Id, capturedAmount, currency, captureId);
                    return new PayPalCapture { Success = true, Amount = capturedAmount, Currency = currency, PayPalCaptureId = captureId };
                }
                else
                {
                    _logger.LogError("Orden {OrderId} COMPLETED, pero no se encontró info de captura válida. CaptureInfo: {@CaptureInfo}", orderId, captureInfo);
                    return new PayPalCapture { Success = false, FailureMessage = "Pago completado pero hubo un error al obtener detalles de la captura." };
                }
            }
            else
            {
                string status = result?.Status ?? "Respuesta Nula";
                _logger.LogWarning("La captura de la orden PayPal {OrderId} NO resultó en estado COMPLETED. Estado recibido: {Status}", orderId, status);
                return new PayPalCapture { Success = false, FailureMessage = $"El estado final del pago no fue 'COMPLETED', fue '{status}'." };
            }
        }
        catch (HttpException httpEx)
        {
            var statusCode = httpEx.StatusCode;
            var message = httpEx.Message;
            _logger.LogError(httpEx, "Error HTTP {StatusCode} de PayPal al capturar orden {OrderId}. Debug Info: {Message}", statusCode, orderId, message);
            string detailedMessage = $"Error de PayPal ({statusCode}) al confirmar el pago.";
            // Intenta parsear mensaje de error (opcional)
            // ...
            return new PayPalCapture { Success = false, FailureMessage = detailedMessage };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al capturar orden PayPal {OrderId}.", orderId);
            return new PayPalCapture { Success = false, FailureMessage = "Ocurrió un error inesperado al confirmar el pago." };
        }
    }

    /// <summary>
    /// Transfiere fondos a una cuenta de PayPal
    /// </summary>
    /// <param name="request">Datos de la transferencia</param>
    /// <returns>Resultado de la transferencia</returns>
    public async Task<PayPalTransferResponse> TransferToPayPalAsync(PayPalTransferRequest request)
    {
        if (request == null || request.Monto <= 0 || string.IsNullOrEmpty(request.PayPalEmail))
        {
            _logger.LogWarning("Solicitud de transferencia PayPal inválida: {Request}", request);
            return new PayPalTransferResponse 
            { 
                Success = false, 
                Message = "Datos de transferencia inválidos" 
            };
        }

        try
        {
            _logger.LogInformation("Iniciando transferencia a PayPal por {Amount} USD a: {Email}", 
                request.Monto, request.PayPalEmail);

            // Obtener token de acceso de PayPal
            var accessToken = await GetPayPalAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                return new PayPalTransferResponse
                {
                    Success = false,
                    Message = "No se pudo obtener autorización de PayPal"
                };
            }

            // Crear un identificador único para esta transacción
            var payoutBatchId = $"Batch_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            var senderItemId = $"PAYOUT_{DateTime.UtcNow:yyyyMMddHHmmss}";

            // Construir el payload para la API de PayPal Payouts
            var payload = new
            {
                sender_batch_header = new
                {
                    sender_batch_id = payoutBatchId,
                    email_subject = "Transferencia de fondos de Infinity Growth",
                    email_message = request.Concepto ?? "Transferencia desde tu wallet de Infinity Growth"
                },
                items = new[]
                {
                    new
                    {
                        recipient_type = "EMAIL",
                        amount = new
                        {
                            value = request.Monto.ToString("0.00", CultureInfo.InvariantCulture),
                            currency = "USD"
                        },
                        note = request.Concepto ?? "Transferencia de fondos",
                        sender_item_id = senderItemId,
                        receiver = request.PayPalEmail
                    }
                }
            };

            // URL de la API de PayPal Payouts (siempre usamos Sandbox para este proyecto)
            string apiUrl = "https://api-m.sandbox.paypal.com/v1/payments/payouts";
            
            // Configurar la solicitud HTTP
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            // Convertir el payload a JSON
            string jsonPayload = JsonSerializer.Serialize(payload);
            httpRequest.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            
            // Enviar la solicitud a PayPal
            _logger.LogInformation("Enviando solicitud de payout a PayPal: {Payload}", jsonPayload);
            var response = await _httpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation("Respuesta de PayPal: {StatusCode} - {Response}", 
                (int)response.StatusCode, responseContent);
            
            // Analizar la respuesta de PayPal
            if (response.IsSuccessStatusCode)
            {
                // Intentar extraer el payout_batch_id y estado de la respuesta
                string batchId = ExtractPayoutBatchId(responseContent) ?? payoutBatchId;
                string status = ExtractPayoutStatus(responseContent) ?? "PROCESSING";
                
                _logger.LogInformation("Payout creado exitosamente. ID: {BatchId}, Estado: {Status}", 
                    batchId, status);
                
                return new PayPalTransferResponse
                {
                    Success = true,
                    Message = "Transferencia procesada exitosamente. Se actualizará en su cuenta PayPal en breve.",
                    TransactionId = batchId,
                    Amount = request.Monto,
                    Status = status
                };
            }
            else
            {
                string errorMessage = ExtractErrorMessage(responseContent);
                _logger.LogError("Error al procesar payout. Código: {StatusCode}, Error: {Error}", 
                    (int)response.StatusCode, errorMessage);
                
                return new PayPalTransferResponse
                {
                    Success = false,
                    Message = $"Error al procesar la transferencia: {errorMessage}"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar transferencia a PayPal para: {Email}", request.PayPalEmail);
            return new PayPalTransferResponse
            {
                Success = false,
                Message = "Error al procesar la transferencia: " + ex.Message
            };
        }
    }

    /// <summary>
    /// Obtiene un token de acceso para la API de PayPal
    /// </summary>
    private async Task<string> GetPayPalAccessTokenAsync()
    {
        try
        {
            // Siempre usamos la URL de Sandbox para este proyecto
            string tokenUrl = "https://api-m.sandbox.paypal.com/v1/oauth2/token";
            
            // Crear credenciales en formato Base64 (ClientId:ClientSecret)
            var credentials = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{_options.Value.ClientId}:{_options.Value.ClientSecret}"));
            
            // Configurar la solicitud HTTP
            using var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            
            // Añadir el form data para solicitar el token
            var formData = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            };
            request.Content = new FormUrlEncodedContent(formData);
            
            // Enviar la solicitud a PayPal
            _logger.LogInformation("Solicitando token OAuth a PayPal");
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    // Extraer el access_token de la respuesta JSON
                    using var doc = JsonDocument.Parse(content);
                    if (doc.RootElement.TryGetProperty("access_token", out var tokenElement))
                    {
                        var token = tokenElement.GetString();
                        _logger.LogInformation("Token OAuth obtenido exitosamente");
                        return token;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al parsear respuesta de token. Respuesta: {Content}", content);
                }
            }
            
            _logger.LogError("No se pudo obtener token OAuth. Código: {StatusCode}, Respuesta: {Content}",
                (int)response.StatusCode, content);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al obtener token OAuth");
            return null;
        }
    }
    
    /// <summary>
    /// Extrae el ID del payout batch de la respuesta JSON
    /// </summary>
    private string ExtractPayoutBatchId(string jsonResponse)
    {
        try
        {
            using var doc = JsonDocument.Parse(jsonResponse);
            
            // Verificar la estructura del JSON de respuesta
            if (doc.RootElement.TryGetProperty("batch_header", out var batchHeader))
            {
                if (batchHeader.TryGetProperty("payout_batch_id", out var batchIdElement))
                {
                    return batchIdElement.GetString();
                }
            }
            
            // Intentar otra estructura posible
            if (doc.RootElement.TryGetProperty("payout_batch_id", out var directBatchId))
            {
                return directBatchId.GetString();
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al extraer batch ID. Respuesta: {Response}", jsonResponse);
            return null;
        }
    }
    
    /// <summary>
    /// Extrae el estado del payout de la respuesta JSON
    /// </summary>
    private string ExtractPayoutStatus(string jsonResponse)
    {
        try
        {
            using var doc = JsonDocument.Parse(jsonResponse);
            
            // Verificar la estructura del JSON de respuesta
            if (doc.RootElement.TryGetProperty("batch_header", out var batchHeader))
            {
                if (batchHeader.TryGetProperty("batch_status", out var statusElement))
                {
                    return statusElement.GetString();
                }
            }
            
            return "PROCESSING"; // Estado por defecto si no lo encontramos
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al extraer estado del payout. Respuesta: {Response}", jsonResponse);
            return "PROCESSING";
        }
    }
    
    /// <summary>
    /// Extrae mensaje de error de la respuesta JSON
    /// </summary>
    private string ExtractErrorMessage(string jsonResponse)
    {
        try
        {
            using var doc = JsonDocument.Parse(jsonResponse);
            
            // Estructura con campo "message"
            if (doc.RootElement.TryGetProperty("message", out var messageElement))
            {
                return messageElement.GetString();
            }
            
            // Estructura con campo "error_description"
            if (doc.RootElement.TryGetProperty("error_description", out var errorDescElement))
            {
                return errorDescElement.GetString();
            }
            
            // Estructura con campo "name" y "details"
            if (doc.RootElement.TryGetProperty("name", out var nameElement))
            {
                string errorName = nameElement.GetString();
                
                if (doc.RootElement.TryGetProperty("details", out var detailsElement) && 
                    detailsElement.ValueKind == JsonValueKind.Array && 
                    detailsElement.GetArrayLength() > 0)
                {
                    var firstDetail = detailsElement[0];
                    if (firstDetail.TryGetProperty("description", out var descElement))
                    {
                        return $"{errorName}: {descElement.GetString()}";
                    }
                    
                    if (firstDetail.TryGetProperty("issue", out var issueElement))
                    {
                        return $"{errorName}: {issueElement.GetString()}";
                    }
                }
                
                return errorName;
            }
            
            return "Error desconocido de PayPal";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al extraer mensaje de error. Respuesta: {Response}", jsonResponse);
            return "No se pudo determinar el error específico";
        }
    }
} // Fin de PayPalService

// --- CLASES DE CONFIGURACIÓN Y RESULTADO ---

// Opciones de configuración (leídas desde appsettings.json)
// Debe estar registrada en Startup.cs / Program.cs: services.Configure<PayPalOptions>(Configuration.GetSection("PayPal"))
public class PayPalOptions
{
    public string ClientId { get; set; } = string.Empty; // Inicializar para evitar warnings de nullabilidad
    public string ClientSecret { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty; // "Sandbox" o "Live"
}

// Clase para devolver el resultado de la creación de la orden
// ** Recomendación: Mover a un archivo separado en AppLogic/Models **
public class PayPalCreateOrderResult
{
    public string OrderId { get; set; } = string.Empty;
    public string ApproveUrl { get; set; } = string.Empty;
    // Podrías añadir: public bool Success { get; set; } = true; // Si manejas fallos devolviendo esto en lugar de null
}

// Clase para devolver el resultado de la captura de la orden (ya la tenías)
// ** Recomendación: Mover a un archivo separado en AppLogic/Models **
public class PayPalCaptureResult
{
    public bool Success { get; set; }
    public string? FailureMessage { get; set; } // Mensaje en caso de Success = false
    public decimal Amount { get; set; } // Monto capturado (solo si Success = true)
    public string? Currency { get; set; } // Moneda (solo si Success = true)
    public string? PayPalCaptureId { get; set; } // ID de la transacción de captura (solo si Success = true)
}