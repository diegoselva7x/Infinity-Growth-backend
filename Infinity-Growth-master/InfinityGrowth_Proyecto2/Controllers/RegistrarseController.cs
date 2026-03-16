using AppLogic;
using Azure;
using DTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace InfinityGrowth_Proyecto2.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]

    public class RegistrarseController : ControllerBase
    {

        private readonly IRegistrarseManager _registrarseManager;

        public RegistrarseController(IRegistrarseManager pRegistrarseManager)
        {
            _registrarseManager = pRegistrarseManager;
        }

        [HttpPost("CreateUser")]
        public API_Response CreateUser([FromBody] Usuarios pUsers)
        {
            API_Response response = new API_Response();
            try
            {
                if (pUsers == null)
                    throw new ArgumentNullException(nameof(pUsers));

                if (pUsers.IdAsesor <= 0)
                    throw new Exception("Debe especificar un asesor válido.");

                _registrarseManager.CreateUser(pUsers);

                response.Result = "OK";
                response.Data = pUsers;
                response.Message = "Usuario creado exitosamente.";
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