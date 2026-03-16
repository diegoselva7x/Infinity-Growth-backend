using AppLogic;
using DTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using DataAccess.Crud;

namespace InfinityGrowth_Proyecto2.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReporteController : ControllerBase
    {
        private readonly IReporteManager _reporteManager;
        private readonly IInversionesActivasManager _inversionesManager;
        private readonly IPriceManager _priceManager;
        private readonly RelacionAsesorClienteCrud _relacionCrud;
        private readonly UsuariosCrud _usuariosCrud;


        public ReporteController(
            IReporteManager reporteManager,
            IInversionesActivasManager inversionesManager,
            IPriceManager priceManager,
            RelacionAsesorClienteCrud relacionCrud,
            UsuariosCrud usuariosCrud)
        {
            _reporteManager = reporteManager;
            _inversionesManager = inversionesManager;
            _priceManager = priceManager;
            _relacionCrud = relacionCrud;
            _usuariosCrud = usuariosCrud;
        }

        // 🔹 CLIENTE - Transacciones
        [HttpGet("ObtenerTransaccionesPorUsuario/{id}")]
        public API_Response ObtenerTransaccionesPorUsuario(int id)
        {
            API_Response response = new API_Response();
            var reporte = _reporteManager.ObtenerTransaccionesPorUsuario(id);

            response.Result = (reporte == null || reporte.Count == 0) ? "ERROR" : "OK";
            response.Data = reporte;
            response.Message = response.Result == "OK" ? "Transacciones cargadas correctamente." : "No se encontraron transacciones para este cliente.";
            return response;
        }

        // 🔹 CLIENTE - Detalle
        [HttpGet("ObtenerClienteDetalle/{id}")]
        public async Task<API_Response> ObtenerClienteDetalle(int id)
        {
            API_Response response = new API_Response();
            try
            {
                var detalle = _reporteManager.ObtenerClienteDetalle(id);

                if (detalle == null)
                    return new API_Response { Result = "ERROR", Message = "No se encontró resumen financiero para el cliente." };

                var inversiones = _inversionesManager.GetInversionesByUserId(id);
                string accionMasRentable = "No disponible";
                string accionMenosRentable = "No disponible";
                decimal mayorGanancia = decimal.MinValue;
                decimal mayorPerdida = decimal.MaxValue;

                foreach (var inversion in inversiones)
                {
                    if (inversion.Cantidad <= 0 || inversion.PrecioTotal <= 0) continue;

                    var precioActualStr = await _priceManager.ObtenerPrecioAsync(inversion.Ticker);
                    if (!decimal.TryParse(precioActualStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precioActual)) continue;

                    decimal precioUnitarioCompra = inversion.PrecioTotal / inversion.Cantidad;
                    decimal gananciaPorcentual = ((precioActual - precioUnitarioCompra) / precioUnitarioCompra) * 100;

                    if (gananciaPorcentual > mayorGanancia)
                    {
                        mayorGanancia = gananciaPorcentual;
                        accionMasRentable = inversion.Nombre;
                    }

                    if (gananciaPorcentual < mayorPerdida)
                    {
                        mayorPerdida = gananciaPorcentual;
                        accionMenosRentable = inversion.Nombre;
                    }
                }

                detalle.AccionMasRentable = accionMasRentable;
                detalle.AccionMenosRentable = accionMenosRentable;

                response.Result = "OK";
                response.Data = detalle;
                response.Message = "Resumen financiero del cliente cargado correctamente.";
            }
            catch (Exception ex)
            {
                response.Result = "ERROR";
                response.Message = ex.Message;
            }

            return response;
        }

        // 🔹 ASESOR - Reporte de Clientes
        [HttpGet("ObtenerReporteAsesor/{id}")]
        public API_Response ObtenerReporteAsesor(int id)
        {
            var reporte = _reporteManager.ObtenerReporteAsesor(id);
            return reporte == null || reporte.Count == 0
                ? new API_Response { Result = "ERROR", Message = "No se encontraron registros para este asesor." }
                : new API_Response { Result = "OK", Data = reporte, Message = "Reporte del asesor generado correctamente." };
        }

        // 🔹 ASESOR - Detalle
        [HttpGet("ObtenerAsesorDetalle/{id}")]
        public async Task<API_Response> ObtenerAsesorDetalle(int id)
        {
            API_Response response = new API_Response();
            try
            {
                var detalle = _reporteManager.ObtenerAsesorDetalle(id);
                if (detalle == null)
                    return new API_Response { Result = "ERROR", Message = "No se encontró resumen financiero para el asesor." };

                var clientes = _relacionCrud.GetClientesByAsesor(id);
                string accionMasRentable = "No disponible";
                string accionMenosRentable = "No disponible";
                decimal mayorGanancia = decimal.MinValue;
                decimal mayorPerdida = decimal.MaxValue;

                foreach (var idCliente in clientes)
                {
                    var inversiones = _inversionesManager.GetInversionesByUserId(idCliente);

                    foreach (var inversion in inversiones)
                    {
                        if (inversion.Cantidad <= 0 || inversion.PrecioTotal <= 0) continue;

                        var precioActualStr = await _priceManager.ObtenerPrecioAsync(inversion.Ticker);
                        if (!decimal.TryParse(precioActualStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precioActual)) continue;

                        decimal precioUnitarioCompra = inversion.PrecioTotal / inversion.Cantidad;
                        decimal gananciaPorcentual = ((precioActual - precioUnitarioCompra) / precioUnitarioCompra) * 100;

                        if (gananciaPorcentual > mayorGanancia)
                        {
                            mayorGanancia = gananciaPorcentual;
                            accionMasRentable = inversion.Nombre;
                        }

                        if (gananciaPorcentual < mayorPerdida)
                        {
                            mayorPerdida = gananciaPorcentual;
                            accionMenosRentable = inversion.Nombre;
                        }
                    }
                }

                detalle.AccionMasRentable = accionMasRentable;
                detalle.AccionMenosRentable = accionMenosRentable;

                response.Result = "OK";
                response.Data = detalle;
                response.Message = "Resumen financiero del asesor cargado correctamente.";
            }
            catch (Exception ex)
            {
                response.Result = "ERROR";
                response.Message = ex.Message;
            }

            return response;
        }

        // 🔹 ADMIN - Resumen Global
        [HttpGet("ObtenerResumenAdmin")]
        public async Task<API_Response> ObtenerResumenAdmin()
        {
            API_Response response = new API_Response();

            try
            {
                var resumen = _reporteManager.ObtenerResumenFinancieroAdmin();
                if (resumen == null || resumen.Count == 0)
                    return new API_Response { Result = "ERROR", Message = "No hay datos de resumen financiero global." };

                var todosLosClientes = _usuariosCrud.ObtenerIdsClientes();
                string accionMasRentable = "No disponible";
                string accionMenosRentable = "No disponible";
                decimal mayorGanancia = decimal.MinValue;
                decimal mayorPerdida = decimal.MaxValue;

                foreach (var clienteId in todosLosClientes)
                {
                    var inversiones = _inversionesManager.GetInversionesByUserId(clienteId);

                    foreach (var inversion in inversiones)
                    {
                        if (inversion.Cantidad <= 0 || inversion.PrecioTotal <= 0) continue;

                        var precioActualStr = await _priceManager.ObtenerPrecioAsync(inversion.Ticker);
                        if (!decimal.TryParse(precioActualStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precioActual)) continue;

                        decimal precioUnitarioCompra = inversion.PrecioTotal / inversion.Cantidad;
                        decimal gananciaPorcentual = ((precioActual - precioUnitarioCompra) / precioUnitarioCompra) * 100;

                        if (gananciaPorcentual > mayorGanancia)
                        {
                            mayorGanancia = gananciaPorcentual;
                            accionMasRentable = inversion.Nombre;
                        }

                        if (gananciaPorcentual < mayorPerdida)
                        {
                            mayorPerdida = gananciaPorcentual;
                            accionMenosRentable = inversion.Nombre;
                        }
                    }
                }

                resumen[0].AccionMasRentable = accionMasRentable;
                resumen[0].AccionMenosRentable = accionMenosRentable;

                response.Result = "OK";
                response.Data = resumen;
                response.Message = "Resumen financiero cargado correctamente.";
            }
            catch (Exception ex)
            {
                response.Result = "ERROR";
                response.Message = ex.Message;
            }

            return response;
        }

        // 🔹 ADMIN - Detalle por Cliente
        [HttpGet("ObtenerDetalleClientesAdmin")]
        public async Task<API_Response> ObtenerDetalleClientesAdmin()
        {
            API_Response response = new API_Response();

            try
            {
                var detalle = _reporteManager.ObtenerDetalleClientesAdmin();

                if (detalle == null || detalle.Count == 0)
                {
                    response.Result = "ERROR";
                    response.Data = null;
                    response.Message = "No hay registros de clientes.";
                    return response;
                }

                // Enriquecer cada cliente con datos dinámicos de rentabilidad
                foreach (var cliente in detalle)
                {
                    var inversiones = _inversionesManager.GetInversionesByUserId(cliente.Id_usuario);

                    decimal totalGanancias = 0;
                    decimal totalPerdidas = 0;

                    foreach (var inversion in inversiones)
                    {
                        if (inversion.Cantidad <= 0 || inversion.PrecioTotal <= 0)
                            continue;

                        var precioActualStr = await _priceManager.ObtenerPrecioAsync(inversion.Ticker);
                        if (!decimal.TryParse(precioActualStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precioActual))
                            continue;

                        decimal precioUnitarioCompra = inversion.PrecioTotal / inversion.Cantidad;
                        decimal diferencia = (precioActual - precioUnitarioCompra) * inversion.Cantidad;

                        if (diferencia >= 0)
                            totalGanancias += diferencia;
                        else
                            totalPerdidas += Math.Abs(diferencia);
                    }

                    cliente.Ganancias = Math.Round(totalGanancias, 2);
                    cliente.Perdidas = Math.Round(totalPerdidas, 2);

                    decimal baseTotal = cliente.Ingresos;
                    cliente.Rendimiento = baseTotal > 0
                        ? Math.Round(((cliente.Ganancias - cliente.Perdidas) / baseTotal) * 100, 2)
                        : 0;
                }

                response.Result = "OK";
                response.Data = detalle;
                response.Message = "Detalle de clientes cargado correctamente.";
            }
            catch (Exception ex)
            {
                response.Result = "ERROR";
                response.Message = ex.Message;
            }

            return response;
        }
    }
}
