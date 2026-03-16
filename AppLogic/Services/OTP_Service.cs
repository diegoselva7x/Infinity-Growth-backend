using DataAccess.Crud;
using DTO;
using System.Security.Cryptography;

namespace AppLogic.Services
{
    public class OTP_Service 
    {
        private readonly OTPCrud _crudOtp;
        private readonly IEmailService _emailService;

        public OTP_Service(OTPCrud crudOtp, IEmailService emailService)
        {
            _crudOtp = crudOtp;
            _emailService = emailService;
        }

        private static readonly int OTPExpirationTime = 5;

        public async Task GenerateOTP(int id, string correo, string solicitud)
        {
            int otp = 0;
            int maxAttempts = 10;
            int attempts = 0;

            while (attempts < maxAttempts) // Se genera un OTP único para el usuario    
            {
                otp = RandomNumberGenerator.GetInt32(100000, 999999);
                if (!SearchOTP(otp))
                {
                    break;
                }
                attempts++;
            }

            if (attempts == maxAttempts)
            {
                throw new Exception("No se pudo generar un OTP único después de varios intentos.");
            }

            var otpCode = new OTP_Codes
            {
                Id_usuario = id,
                Codigo = otp,
                FechaEnvio = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddMinutes(OTPExpirationTime),
                Activo = true
            };
            _crudOtp.Create(otpCode);

            await SendEmail(correo, otp, solicitud);
        }


        public bool SearchOTP(int pOtp)
        {
            var otpCode = pOtp;
            return _crudOtp.SearchOTP(otpCode);
        }

        public bool ValidateOTP(int pIdUsuario, int pOtp)
        {
            var otpCode = new OTP_Codes
            {
                Id_usuario = pIdUsuario,
                Codigo = pOtp
            };
            return _crudOtp.ValidateOTP(otpCode);

        }

        public async Task SendEmail(string emailAddress, int otp, string solicitud)
        {
            string subject = "Código de verificación";
            string plainTextContent = $"Su código de verificación es: {otp}";
            string htmlContent = $@"
                                    <html>
                                      <body style=""margin:0; font-family: 'Segoe UI', sans-serif; background-color: #18191C; padding: 20px;"">
                                        <div style=""max-width: 600px; margin: auto; background-color: #23262E; padding: 30px; border-radius: 12px; color: #FFFFFF;"">
                                          <div style=""text-align: right; margin-bottom: 20px;"">
                                            <img src=""https://i.postimg.cc/prrMnWZz/logo-png-dark.png"" alt=""Logo"" style=""height: 40px;"">
                                          </div>
                                          <h2 style=""color: #FFFFFF;"">Verificación de seguridad</h2>
                                          <p style=""font-size: 15px; color: #CCCCCC;"">
                                            Hemos recibido su solicitud de <strong>{solicitud}</strong>. A continuación encontrará el código OTP para completar el proceso:
                                          </p>
                                          <div style=""display: inline-block; background-color: #97AE4D; color: #15161A; text-align: center; padding: 10px 15px; border-radius: 12px; font-weight: 600; font-size: 24px; margin: 20px 0;"">
                                            {otp}
                                          </div>
                                          <p style=""font-size: 14px; color: #999999;"">Este código es válido por tiempo limitado. No lo comparta con nadie.</p>
                                        </div>
                                      </body>
                                    </html>
";
            await _emailService.SendEmail(emailAddress, subject, plainTextContent, htmlContent);

        }

    }
}
