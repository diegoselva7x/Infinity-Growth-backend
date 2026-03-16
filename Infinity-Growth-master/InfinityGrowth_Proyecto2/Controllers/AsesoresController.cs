using AppLogic;
using DTO;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsesoresController : ControllerBase
    {
        private readonly AsesoresManager _manager;

        public AsesoresController(AsesoresManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        [Route("GetAll")]
        public ActionResult<List<Asesor>> GetAll()
        {
            try
            {
                var asesores = _manager.ObtenerTodos();
                return Ok(asesores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
