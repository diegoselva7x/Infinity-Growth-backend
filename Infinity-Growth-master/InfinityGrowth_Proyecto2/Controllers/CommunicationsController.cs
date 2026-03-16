using AppLogic.Services;
using DTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApi.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class CommunicationsController : ControllerBase
    {
        private readonly IEmailService _serviceData;
        public CommunicationsController(IEmailService pServiceData)
        {
            _serviceData = pServiceData;
        }

        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmail pSendEmail)
        {
            try
            {
                var result = await _serviceData.SendEmail(pSendEmail.EmailAddres, pSendEmail.Subject, pSendEmail.PlainTextContent, pSendEmail.HtmlContent);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

