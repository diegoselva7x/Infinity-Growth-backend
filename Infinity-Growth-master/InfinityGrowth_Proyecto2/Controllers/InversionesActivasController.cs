using AppLogic;
using Azure;
using DTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Globalization;

namespace InfinityGrowth_Proyecto2.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]

    public class InversionesActivasController : ControllerBase
    {

        private readonly IInversionesActivasManager _inversionesManager;

        private readonly IPriceManager _priceManager;

        public InversionesActivasController(IInversionesActivasManager pInversionesManager, IPriceManager pPriceManger)
        {
            _inversionesManager = pInversionesManager;
            _priceManager = pPriceManger;
        }

        [HttpPost("CreateInversion")]
        public async Task<API_Response> CreateInversion([FromBody] InversionesActivas pInversion)
        {
            API_Response response = new API_Response();
            try
            {
                // obtener el precio actual desde el manager del priceManager 
                var precioActual = await _priceManager.ObtenerPrecioAsync(pInversion.Ticker);
                if (precioActual == null)
                {
                    response.Result = "ERROR";
                    response.Message = "No se pudo obtener el precio actual.";
                    return response;
                }
                else
                {
                    // Convert precioActual to money before assigning it to PrecioCompra
                    pInversion.PrecioCompra = decimal.Parse(precioActual, CultureInfo.InvariantCulture);


                }
                var mensaje = await _inversionesManager.CreateInversion(pInversion);
                response.Result = "OK";
                response.Data = pInversion;

                response.Message = mensaje;
            }
            catch (System.Exception ex)
            {
                response.Result = "ERROR";
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet("GetInversionesActivasByUserId/{userId}")]
        public async Task<API_Response> GetInversionesActivasByUserId(int userId)
        {
            API_Response response = new API_Response();

            try
            {
                var inversiones = _inversionesManager.GetInversionesByUserId(userId);

                foreach (var inversion in inversiones)
                {
                    // Validación de datos base
                    if (inversion.Cantidad <= 0 || inversion.PrecioTotal <= 0)
                    {
                        inversion.GananciaMonetaria = 0;
                        inversion.GananciaPorcentual = 0;
                        continue;
                    }

                    // Obtener precio actual
                    var precioActualStr = await _priceManager.ObtenerPrecioAsync(inversion.Ticker);

                    if (string.IsNullOrWhiteSpace(precioActualStr))
                    {
                        inversion.GananciaMonetaria = 0;
                        inversion.GananciaPorcentual = 0;
                        continue;
                    }

                    // Parseo de precio
                    if (decimal.TryParse(precioActualStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precioActual))
                    {
                        decimal precioUnitarioCompra = inversion.PrecioTotal / inversion.Cantidad;
                        decimal gananciaMonetaria = (precioActual - precioUnitarioCompra) * inversion.Cantidad;
                        decimal gananciaPorcentual = ((precioActual - precioUnitarioCompra) / precioUnitarioCompra) * 100;

                        //  Si la ganancia es muy pequeña, deja más decimales, sino redondea a 2
                        inversion.GananciaMonetaria = Math.Round(gananciaMonetaria, Math.Abs(gananciaMonetaria) < 1 ? 4 : 2);
                        inversion.GananciaPorcentual = Math.Round(gananciaPorcentual, 4);

                        inversion.GananciaPorcentual = Math.Abs(gananciaPorcentual) < 1
                            ? Math.Round(gananciaPorcentual, 6)
                            : Math.Round(gananciaPorcentual, 5);
                    }
                    else
                    {
                        inversion.GananciaMonetaria = 0;
                        inversion.GananciaPorcentual = 0;
                    }
                }

                response.Result = "OK";
                response.Data = inversiones;
                response.Message = "Inversiones activas obtenidas correctamente.";
            }
            catch (Exception ex)
            {
                response.Result = "ERROR";
                response.Message = ex.Message;
            }

            return response;
        }

        [HttpPost("VentaAccion")]
        public async Task<API_Response> VentaAccion([FromBody] VenderAccion pInversion)
        {
            API_Response response = new API_Response();
            try
            {
                // Obtener el precio actual desde el manager del priceManager 
                var precioActual = await _priceManager.ObtenerPrecioAsync(pInversion.Ticker);
                if (precioActual == null)
                {
                    response.Result = "ERROR";
                    response.Message = "No se pudo obtener el precio actual.";
                    return response;
                }
                else
                {
                    pInversion.PrecioVenta = decimal.Parse(precioActual, CultureInfo.InvariantCulture);
                }

                var mensaje = _inversionesManager.SellInversion(pInversion);


                response.Result = "OK";
                response.Data = pInversion;
                response.Message = mensaje;
            }
            catch (System.Exception ex)
            {
                response.Result = "ERROR";
                response.Message = ex.Message;
            }
            return response;
        }

    }
}