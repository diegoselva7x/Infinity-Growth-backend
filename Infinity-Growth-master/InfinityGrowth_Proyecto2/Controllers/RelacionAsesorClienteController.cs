using AppLogic;
using DTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace InfinityGrowth_Proyecto2.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class RelacionAsesorClienteController : ControllerBase
    {
        private readonly RelacionAsesorClienteManager _manager;

        public RelacionAsesorClienteController(RelacionAsesorClienteManager manager)
        {
            _manager = manager;
        }

        [HttpPost("Asignar")]
        public API_Response AsignarAsesor([FromBody] AsignarAsesor model)
        {
            API_Response response = new API_Response();

            try
            {
                var resultado = _manager.AsignarAsesorACliente(model.Id_Asesor, model.Id_Cliente);

                response.Result = resultado.Contains("ya tiene asignado") ? "ERROR" : "OK";
                response.Message = resultado;
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
