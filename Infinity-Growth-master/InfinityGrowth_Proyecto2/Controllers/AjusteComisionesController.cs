using AppLogic;
using DTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InfinityGrowth_Proyecto2.Controllers // Asegúrate que el namespace sea correcto
{
    [EnableCors("AllowAll")] // Añadido para coincidir con LoginController
    [ApiController]
    [Route("api/[controller]")]
    public class AjusteComisionesController : ControllerBase
    {
        private readonly IAjusteComisionesManager _manager;

        public AjusteComisionesController(IAjusteComisionesManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// GET: api/ajustecomisiones
        /// Obtiene la configuración actual de todas las comisiones.
        /// </summary>
        [HttpGet]
        // Ya no especificamos ProducesResponseType porque siempre devolvemos API_Response
        public API_Response GetAllComisiones()
        {
            var response = new API_Response();
            try
            {
                var comisiones = _manager.GetAllComisiones();
                response.Result = "OK";
                response.Data = comisiones;
                // Opcional: Añadir un mensaje si se quiere
                response.Message = comisiones.Any()
                    ? "Configuración de comisiones obtenida correctamente."
                    : "No hay configuraciones de comisiones definidas.";
            }
            catch (Exception ex)
            {
                response.Result = "ERROR";
                response.Message = $"Ocurrió un error al obtener la configuración: {ex.Message}";
                // Loggear el error 'ex' sería bueno aquí también
            }
            return response;
        }

        /// <summary>
        /// PUT: api/ajustecomisiones
        /// Actualiza la configuración de una o más comisiones.
        /// Espera una lista de objetos AjusteComisiones en el cuerpo de la solicitud.
        /// </summary>
        [HttpPut]
        // Ya no especificamos ProducesResponseType
        public API_Response UpdateComisiones([FromBody] List<AjusteComisiones> comisiones)
        {
            var response = new API_Response();

            // Validación básica del modelo
            if (comisiones == null || !comisiones.Any())
            {
                response.Result = "ERROR";
                response.Message = "La lista de comisiones a actualizar no puede estar vacía.";
                // Devolvemos directamente como en LoginController si la entrada es inválida
                return response;
            }

            // Opcional: Validación ModelState (si la usas)
            if (!ModelState.IsValid)
            {
                response.Result = "ERROR";
                // Podrías formatear los errores de ModelState en el mensaje
                response.Message = "Datos inválidos: " + string.Join("; ", ModelState.Values
                                                             .SelectMany(v => v.Errors)
                                                             .Select(e => e.ErrorMessage));
                return response;
            }

            try
            {
                _manager.UpdateComisiones(comisiones);
                response.Result = "OK";
                response.Message = "Configuración de comisiones actualizada correctamente.";
                // No hay datos que devolver en Data para un PUT exitoso generalmente
            }
            catch (Exception ex)
            {
                response.Result = "ERROR";
                response.Message = $"Ocurrió un error al actualizar la configuración: {ex.Message}";
                // Loggear el error 'ex'
            }
            return response;
        }
    }
}