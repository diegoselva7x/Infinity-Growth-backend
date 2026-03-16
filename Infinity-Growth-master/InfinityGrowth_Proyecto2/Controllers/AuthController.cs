using AppLogic;
using AppLogic.Services;
using DTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace InfinityGrowth_Proyecto2.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILoginManager _loginManager;
        private readonly Jwt_Service _jwtService;
        private readonly IRecuperarPasswordManager _recuperarPasswordManager;

        public AuthController(ILoginManager pLoginManager, Jwt_Service jwtService, IRecuperarPasswordManager pRecuperarPasswordManager)
        {
            _loginManager = pLoginManager;
            _jwtService = jwtService;
            _recuperarPasswordManager = pRecuperarPasswordManager;
        }

        //Login
        [HttpPost("Login")]
        public API_Response Login([FromBody] LoginRequest request)
        {
            API_Response response = new API_Response();
            try
            {
                var user = _loginManager.GetUserLogin(request.Correo, request.Password);
                
                if (user.Count == 0)
                {
                    return new API_Response
                    {
                        Result = "ERROR",
                        Data = null,
                        Message = "Credenciales incorectas."
                    };
                }

                if (user[0].Estado == 0)
                {
                    response.Result = "ERROR";
                    response.Message = "Usuario inactivo, no puedes inciar sesión.";
                    return response;
                }

                if (user[0].Estado == 2)
                {
                    response.Result = "ERROR";
                    response.Message = "Usuario a la espera de un Asesor, no puedes ingresar al sistema.";
                    return response;
                }

                if (user.Count > 0 && user[0].isPasswordTemp == true)
                {
                    response.Result = "OK";
                    response.Data = new { Id = user[0].Id, isTemp = true };
                    response.Message = "La contraseña es temporal, debe cambiarla para poder continuar.";
                    return response;
                }

                return new API_Response
                {
                    Result = "OK",
                    Data = user[0].Id,
                    Message = "Usuario autenticado. Se ha enviado un OTP a su correo."
                };
            }
            catch (Exception ex)
            {
                response.Result = "ERROR";
                response.Message = ex.Message;
            }
            return response;
        }

        //validar el OTP
        [HttpPost("ValidateOTP")]
        public API_Response ValidateOTP([FromBody] OTP_Request request)
        {
            API_Response response = new API_Response();
            try
            {
                var user = _loginManager.GetUserById(request.Id_Usuario);

                if (user == null)
                {
                    return new API_Response
                    {
                        Result = "ERROR",
                        Data = null,
                        Message = "Usuario no encontrado."
                    };
                }
                if (_loginManager.ValidarOTP(user.Id, request.Otp_Code))
                {
                    var token = _jwtService.GenerateToken(user);

                    var Tokenresponse = new TokenResponseDto
                    {
                        Result = "OK",
                        Token = token,
                        Expiration = DateTime.UtcNow.AddMinutes(40),
                    };

                   return new API_Response
                   {
                       Result = "OK",
                       Data = Tokenresponse,
                       Message = "OTP válido. Usuario autenticado."
                   };
                }
                else
                {
                    response.Result = "ERROR";
                    response.Message = "OTP incorrecto o expirado.";
                }
            }
            catch (Exception ex)
            {
                response.Result = "ERROR";
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost("GetUserForReset")]
        public API_Response GetUserForReset([FromBody] RecuperarPasswordRequest request)
        {
            var user = _recuperarPasswordManager.GetUserForReset(request.Correo);
            if (user != null)
            {
                return new API_Response
                {
                    Result = "OK",
                    Data = user[0].Id,
                    Message = "Usuario encontrado. Se ha enviado un OTP a su correo."
                };
            }
            else
            {
                return new API_Response
                {
                    Result = "ERROR",
                    Data = null,
                    Message = "Usuario no encontrado."
                };
            }
        }

        //Recuperar contraseña
        [HttpPost("UpdatePassword")]
        public API_Response UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            var result = _recuperarPasswordManager.UpdatePassword(request.Correo, request.NewPassword);

            if (result)
            {
                return new API_Response
                {
                    Result = "OK",
                    Message = "Contraseña actualizada correctamente."
                };
            }
            else
            {
                return new API_Response
                {
                    Result = "ERROR",
                    Message = "Error al actualizar la contraseña."
                };
            }

        }

        //Obtener la foto de perfil
        [HttpGet("GetProfilePicture/{id}")]
        public IActionResult GetProfilePicture(int id)
        {
            var user = _loginManager.GetUserById(id);
            if (user != null)
            {
                string base64String = Convert.ToBase64String(user.ProfileFoto);
                string imageDataURL = $"data:image/png;base64,{base64String}";

                return Ok(new { ImageData = imageDataURL });
            }
            else
            {
                return NotFound("Usuario no encontrado.");
            }
        }

    }
}
