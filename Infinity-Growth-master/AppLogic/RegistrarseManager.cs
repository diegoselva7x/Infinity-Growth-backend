using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLogic.Services;
using Azure.Communication.Email;
using DataAccess.Crud;
using DTO;
using static System.Net.WebRequestMethods;

namespace AppLogic
{
    public interface IRegistrarseManager
    {
        Task<string> CreateUser(Usuarios dto);
        Task SendEmail(string emailAddress, string password);
    }
    public class RegistrarseManager : IRegistrarseManager
    {
        private readonly PasswordGenerator_Service _passwordGeneratorService;
        private readonly Encrypt_Service _encryptService;
        private readonly IEmailService _emailService;
        private readonly LoginCrud _loginCrud;
        private readonly RegistrarseCrud _registrarseCrud;

        public RegistrarseManager(IEmailService emailService, LoginCrud loginCrud, RegistrarseCrud registrarseCrud)
        {
            _passwordGeneratorService = new PasswordGenerator_Service();
            _encryptService = new Encrypt_Service();
            _emailService = emailService;
            _loginCrud = loginCrud;
            _registrarseCrud = registrarseCrud;
        }

        public async Task<string> CreateUser(Usuarios dto)
        {
            try
            {
                var verificarEmail = _loginCrud.RetrieveByCorreo<Usuarios>(dto.Correo);
                if (verificarEmail.Count > 0)
                {
                    throw new Exception("El correo ya se encuentra registrado.");
                }

                // Generar contraseña temporal
                var password = _passwordGeneratorService.GeneratePassword(8);
                dto.Password = _encryptService.HashPassword(password);
                dto.isPasswordTemp = true;

                // Crear usuario con rol de cliente y estado activo
                dto.TipoUsuario = 3;
                dto.Estado = 1;

                _registrarseCrud.Create(dto);
                var resultId = _loginCrud.RetrieveByCorreo<Usuarios>(dto.Correo);

                if (resultId.Count == 0)
                {
                    throw new Exception("No se pudo registrar el usuario.");
                }

                await SendEmail(dto.Correo, password);

                return $"Usuario registrado de manera correcta: {resultId}";
            }
            catch (Exception ex)
            {
                return "Ocurrió un error inesperado. Intente nuevamente. " + ex.Message;
            }
        }

        public async Task SendEmail(string emailAddress, string password)
        {
            string subject = "Registro de acceso Infinity Growth";
            string plainTextContent = "Registro de acceso Infinity Growth";
            string htmlContent = $@"
                                    <html>
                                      <body style=""margin:0; font-family: 'Segoe UI', sans-serif; background-color: #18191C; padding: 20px;"">
                                        <div style=""max-width: 600px; margin: auto; background-color: #23262E; padding: 30px; border-radius: 12px; color: #FFFFFF;"">
                                          <div style=""text-align: right; margin-bottom: 20px;"">
                                            <img src=""https://i.postimg.cc/prrMnWZz/logo-png-dark.png"" alt=""Logo"" style=""height: 40px;"">
                                          </div>
                                          <h2 style=""color: #FFFFFF; margin-bottom: 16px;"">Bienvenido a Infinity Growth</h2>
                                          <p style=""font-size: 15px; color: #CCCCCC;"">
                                            Le informamos que hemos registrado su usuario para el ingreso a nuestra plataforma.
                                          </p>
                                          <p style=""font-size: 15px; color: #CCCCCC;"">Sus credenciales de acceso son las siguientes:</p>
                                          <ul style=""font-size: 15px; color: #FFFFFF; line-height: 1.6;"">
                                            <li><strong>Usuario:</strong> {emailAddress}</li>
                                            <li><strong>Contraseña temporal:</strong> {password}</li>
                                          </ul>
                                          <p style=""font-size: 15px; color: #CCCCCC;"">
                                            Le recomendamos cambiar su contraseña al realizar su primer ingreso.
                                          </p>
                                          <div style=""margin-top: 25px;"">
                                            <a href=""{{loginLink}}"" style=""display: inline-block; background-color: #97AE4D; color: #15161A; text-align: center; padding: 10px 15px; border: none; border-radius: 12px; cursor: pointer; font-weight: 600; transition: all 0.3s; text-decoration: none;"">
                                              Ingresar a la plataforma
                                            </a>
                                          </div>
                                        </div>
                                      </body>
                                    </html>";
            await _emailService.SendEmail(emailAddress, subject, plainTextContent, htmlContent);
        }
    }
}