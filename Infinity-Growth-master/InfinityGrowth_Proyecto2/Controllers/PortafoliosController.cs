using AppLogic;
using DTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace InfinityGrowth_Proyecto2.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class PortafoliosController : ControllerBase
    {
        private readonly IPortafoliosManager _portafoliosManager;

        public PortafoliosController(IPortafoliosManager portafoliosManager)
        {
            _portafoliosManager = portafoliosManager;
        }

      /*  [HttpGet("GetPortafoliosByUserId/{userId}")]
        public API_Response GetPortafoliosByUserId(int userId)
        {
            API_Response response = new API_Response();
            try
            {
                List<Portafolios> portafoliosList = _portafoliosManager.ReatrivePortafolioByUserId(userId);

                response.Result = "OK";
                response.Data = portafoliosList;
                response.Message = "Portafolios obtenidos correctamente.";
            }
            catch (System.Exception ex)
            {
                response.Result = "ERROR";
                response.Message = ex.Message;
            }

            return response;
        }*/
    }
}
