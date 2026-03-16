using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using DTO;
using AppLogic;
using AppLogic.Services;


[Route("api/[controller]")]
[ApiController]
public class PayPalController : ControllerBase
{
    private readonly PayPalService _paypalService;
    private readonly ILogger<PayPalController> _logger;
    private readonly WalletManager _walletManager;
    // private readonly IWalletService _walletService; // Servicio para actualizar saldo

    public PayPalController(
        PayPalService paypalService,
        ILogger<PayPalController> logger,
        WalletManager walletManager
        // IWalletService walletService
        )
    {
        _paypalService = paypalService ?? throw new ArgumentNullException(nameof(paypalService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _walletManager = walletManager ?? throw new ArgumentNullException(nameof(walletManager));
        // _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
    }

    /// <summary>
    /// Endpoint para iniciar el proceso de fondeo: Crea una orden en PayPal.
    /// </summary>
    [HttpPost("create-order")]
    [ProducesResponseType(typeof(CreateOrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto request)
    {
        if (request == null || request.Amount <= 0)
        {
            _logger.LogWarning("Intento de crear orden PayPal con monto inválido: {Amount}", request?.Amount);
            return BadRequest(new { message = "El monto debe ser un valor positivo." });
        }

        // string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // if (string.IsNullOrEmpty(userId)) { return Unauthorized(); }

        try
        {
            _logger.LogInformation("Recibida solicitud para crear orden PayPal por {Amount} {Currency}", request.Amount, request.CurrencyCode ?? "USD");

            // --- CORRECCIÓN: Recibir objeto resultado, NO desconstruir tupla ---
            var orderCreationResult = await _paypalService.CreateOrderAsync(request.Amount, request.CurrencyCode ?? "USD");

            // --- CORRECCIÓN: Acceder a propiedades del objeto resultado ---
            // *** AJUSTA '.OrderId' y '.ApproveUrl' si las propiedades en el objeto que devuelve
            // *** tu CreateOrderAsync se llaman diferente ***
            if (orderCreationResult == null || string.IsNullOrEmpty(orderCreationResult.OrderId) || string.IsNullOrEmpty(orderCreationResult.ApproveUrl))
            {
                _logger.LogError("PayPalService.CreateOrderAsync devolvió resultado inválido o vacío.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error interno al obtener detalles de la orden de PayPal." });
            }

            _logger.LogInformation("Orden PayPal {OrderId} creada exitosamente. Enviando URL de aprobación al cliente.", orderCreationResult.OrderId);

            // --- CORRECCIÓN: Usar propiedades del objeto resultado ---
            return Ok(new CreateOrderResponseDto { OrderId = orderCreationResult.OrderId, ApproveUrl = orderCreationResult.ApproveUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error excepcional al crear orden PayPal para monto {Amount} {Currency}.", request.Amount, request.CurrencyCode ?? "USD");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado al procesar la solicitud de pago." });
        }
    }

    /// <summary>
    /// Endpoint para finalizar el fondeo: Captura una orden previamente aprobada en PayPal.
    /// </summary>
    [HttpPost("capture-order")]
    [ProducesResponseType(typeof(CaptureOrderResponseDto), StatusCodes.Status200OK)] // Éxito
    [ProducesResponseType(typeof(CaptureOrderResponseDto), StatusCodes.Status400BadRequest)] // Fallo esperado
    [ProducesResponseType(typeof(CaptureOrderResponseDto), StatusCodes.Status500InternalServerError)] // Error inesperado
    public async Task<IActionResult> CaptureOrder([FromBody] CaptureOrderRequestDto request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.OrderId))
        {
            _logger.LogWarning("Intento de capturar orden PayPal con OrderId vacío o nulo.");
            return BadRequest(new CaptureOrderResponseDto { Success = false, Message = "Se requiere el ID de la orden de PayPal." });
        }

        // string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // if (string.IsNullOrEmpty(userId)) { return Unauthorized(new CaptureOrderResponseDto { Success = false, Message = "Usuario no autorizado."}); }

        try
        {
            _logger.LogInformation("Recibida solicitud para capturar orden PayPal {OrderId}", request.OrderId);

            // --- CORRECCIÓN: Llamar y recibir PayPalCaptureResult ---
            PayPalCapture captureResult = await _paypalService.CaptureOrderAsync(request.OrderId);

            // --- CORRECCIÓN: Usar captureResult.Success ---
            if (captureResult.Success)
            {
                _logger.LogInformation("Orden PayPal {OrderId} capturada exitosamente. Monto: {Amount} {Currency}. CaptureID: {CaptureId}",
                                       request.OrderId, captureResult.Amount, captureResult.Currency, captureResult.PayPalCaptureId);

                // --- Lógica de Actualización del Wallet (Placeholder) ---
                try
                {
                    // bool updateSuccess = await _walletService.CreditUserWalletAsync(userId, captureResult.Amount, captureResult.Currency, request.OrderId, captureResult.PayPalCaptureId);
                    // if (!updateSuccess) { /* Manejar fallo crítico */ }

                    _logger.LogInformation("Placeholder: Wallet actualizado para usuario {UserId} por orden PayPal {OrderId}", "USER_ID_AQUI", request.OrderId);

                    // --- CORRECCIÓN: Devolver Ok con CaptureOrderResponseDto actualizado ---
                    return Ok(new CaptureOrderResponseDto
                    {
                        Success = true,
                        Message = "Fondeo realizado exitosamente.",
                        Amount = captureResult.Amount,
                        Currency = captureResult.Currency,
                        PayPalCaptureId = captureResult.PayPalCaptureId
                    });
                }
                catch (Exception walletEx)
                {
                    _logger.LogError(walletEx, "¡CRÍTICO! Excepción al actualizar wallet para Orden PayPal {OrderId} capturada.", request.OrderId);
                    // Devolver éxito con advertencia o error 500. Opto por éxito con advertencia.
                    return Ok(new CaptureOrderResponseDto
                    {
                        Success = true, // El pago en PayPal SÍ fue exitoso
                        Message = "Fondeo realizado, pero hubo un problema al actualizar el saldo. Contacte a soporte si no se refleja.",
                        Amount = captureResult.Amount,
                        Currency = captureResult.Currency,
                        PayPalCaptureId = captureResult.PayPalCaptureId
                    });
                }
                // --- Fin Lógica Wallet ---
            }
            else // captureResult.Success es false
            {
                // --- CORRECCIÓN: Usar captureResult.FailureMessage ---
                _logger.LogWarning("La captura de la orden PayPal {OrderId} no fue exitosa: {FailureMessage}", request.OrderId, captureResult.FailureMessage);
                return BadRequest(new CaptureOrderResponseDto { Success = false, Message = captureResult.FailureMessage ?? "No se pudo completar el pago en PayPal." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error excepcional en endpoint CaptureOrder para PayPal OrderId {OrderId}.", request.OrderId);
            return StatusCode(StatusCodes.Status500InternalServerError, new CaptureOrderResponseDto { Success = false, Message = "Ocurrió un error inesperado al confirmar el pago." });
        }
    }

    /// <summary>
    /// Endpoint para transferir fondos del wallet a una cuenta PayPal
    /// </summary>
    [HttpPost("transfer-to-paypal")]
    [ProducesResponseType(typeof(PayPalTransferResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TransferToPayPal([FromBody] PayPalTransferRequest request)
    {
        if (request == null || request.Monto <= 0 || string.IsNullOrEmpty(request.PayPalEmail))
        {
            _logger.LogWarning("Solicitud de transferencia PayPal inválida: {Request}", request);
            return BadRequest(new { message = "Datos de transferencia inválidos" });
        }

        try
        {
            // 1. Verificar que el usuario tenga saldo suficiente
            if (!_walletManager.HasSufficientBalance(request.IdUsuario, request.Monto))
            {
                _logger.LogWarning("Usuario {UserId} intenta transferir {Amount} sin saldo suficiente", 
                    request.IdUsuario, request.Monto);
                return BadRequest(new { message = "Saldo insuficiente para realizar la transferencia" });
            }

            // 2. Realizar la transferencia a PayPal
            _logger.LogInformation("Iniciando transferencia a PayPal para usuario {UserId} por {Amount}", 
                request.IdUsuario, request.Monto);
            var transferResult = await _paypalService.TransferToPayPalAsync(request);

            // 3. Si la transferencia fue exitosa, rebajar el saldo del wallet
            if (transferResult.Success)
            {
                try
                {
                    // Rebajar el saldo del wallet SOLO SI la transferencia fue exitosa
                    bool withdrawalSuccess = _walletManager.WithdrawForPayPalTransfer(
                        request.IdUsuario, 
                        request.Monto, 
                        transferResult.TransactionId);
                    
                    if (!withdrawalSuccess)
                    {
                        // Esto no debería ocurrir ya que verificamos el saldo anteriormente
                        _logger.LogError("ERROR CRÍTICO: Transferencia a PayPal exitosa pero falló al rebajar del wallet. UserId: {UserId}, Amount: {Amount}", 
                            request.IdUsuario, request.Monto);
                        
                        // En un escenario real, aquí habría que manejar este caso mediante un sistema de compensación
                        return StatusCode(StatusCodes.Status500InternalServerError, new { 
                            message = "La transferencia se realizó pero hubo un problema al actualizar su saldo. Por favor contacte a soporte." 
                        });
                    }
                    
                    _logger.LogInformation("Transferencia a PayPal completada exitosamente para usuario {UserId}. TransactionId: {TransactionId}", 
                        request.IdUsuario, transferResult.TransactionId);
                    
                    return Ok(new {
                        success = true,
                        message = "Transferencia a PayPal realizada exitosamente",
                        transactionId = transferResult.TransactionId,
                        amount = transferResult.Amount,
                        status = transferResult.Status
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al registrar el retiro en el wallet tras transferencia exitosa a PayPal. Usuario: {UserId}", request.IdUsuario);
                    
                    // IMPORTANTE: La transferencia PayPal ya se hizo pero no se registró en nuestro sistema
                    // Esto requiere atención manual y posible reconciliación
                    return StatusCode(StatusCodes.Status500InternalServerError, new { 
                        message = "La transferencia a PayPal se realizó exitosamente, pero hubo un error al actualizar su wallet. Por favor contacte a soporte técnico con este código: " + transferResult.TransactionId
                    });
                }
            }
            else
            {
                _logger.LogWarning("Falló la transferencia a PayPal para usuario {UserId}: {Message}", 
                    request.IdUsuario, transferResult.Message);
                
                return BadRequest(new {
                    success = false,
                    message = transferResult.Message
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error excepcional procesando transferencia a PayPal para usuario {UserId}: {Message}", 
                request.IdUsuario, ex.Message);
                
            // Determinar si es un error específico que podamos manejar mejor
            string errorMessage = "Ocurrió un error inesperado al procesar la transferencia";
            
            // Si es un error del procedimiento almacenado, dar un mensaje más específico
            if (ex.Message.Contains("SP_REGISTER_WALLET_TRANSACTION"))
            {
                errorMessage = "Error en el sistema: Configuración del procedimiento almacenado incorrecta. Por favor contacte a soporte técnico.";
            }
            
            return StatusCode(StatusCodes.Status500InternalServerError, new { 
                message = errorMessage 
            });
        }
    }
}

// --- DTOs (Data Transfer Objects) ---
// *** Recomendación: Mueve estas clases a sus propios archivos en AppLogic/Models ***
// *** Si lo haces, necesitarás añadir 'using InfinityGrowth_Proyecto2.AppLogic.Models;' arriba ***

public class CreateOrderRequestDto
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser positivo.")]
    public decimal Amount { get; set; }
    public string? CurrencyCode { get; set; } // Ej. "USD"
}

public class CreateOrderResponseDto
{
    public string OrderId { get; set; }
    public string ApproveUrl { get; set; }
}

public class CaptureOrderRequestDto
{
    [System.ComponentModel.DataAnnotations.Required]
    public string OrderId { get; set; }
}

// --- CORRECCIÓN: Definición de CaptureOrderResponseDto actualizada ---
public class CaptureOrderResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; }

    // Detalles adicionales en caso de éxito
    public decimal? Amount { get; set; } // El monto realmente capturado
    public string? Currency { get; set; } // La moneda
    public string? PayPalCaptureId { get; set; } // El ID específico de la transacción de captura en PayPal
}
