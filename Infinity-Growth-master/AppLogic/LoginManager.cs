using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLogic.Services;
using DataAccess.Crud;
using DTO;
using BCrypt.Net;


namespace AppLogic
{
    public interface ILoginManager
    {
        public List<Usuarios> GetUserLogin(string pCorreo, string pPassword);
        public Usuarios GetUserById(int pId);
        public bool ValidarOTP(int usuario, int otp);
    }
    public class LoginManager : ILoginManager
    {
        private readonly IEmailService _emailService;
        private readonly LoginCrud _loginCrud;
        private readonly Encrypt_Service _encryptService;
        private readonly OTP_Service _otpService;

        public LoginManager(IEmailService emailService, LoginCrud loginCrud, Encrypt_Service encryptService, OTP_Service otpService)
        {
            _emailService = emailService;
            _loginCrud = loginCrud;
            _encryptService = encryptService;
            _otpService = otpService;
        }

        public List<Usuarios> GetUserLogin(string pCorreo, string pPassword) // Se obtiene el usuario que intenta iniciar sesión y se genera un OTP
        {
            var appList = _loginCrud.RetrieveByCorreo<Usuarios>(pCorreo);

            if (appList.Count > 0)
            {
                var user = appList[0];

                if (_encryptService.VerifyPassword(pPassword, user.Password)) // Se verifica que la contraseña ingresada sea correcta   
                {
                    if (user.Estado == 0)
                    {
                        return new List<Usuarios>() { user };
                    }

                    if (user.Estado == 2)
                    {
                        return new List<Usuarios>() { user };
                    }

                    if (user.isPasswordTemp == true)
                    {
                        return new List<Usuarios> { user };
                    }
                    else
                    {
                        string solicitud = "Inicio de sesión";
                        _ = _otpService.GenerateOTP(user.Id, user.Correo, solicitud);

                        return new List<Usuarios> { user };
                    }
                }
            }

            return new List<Usuarios>();
        }

        public bool ValidarOTP(int pUsuario, int pOtp)
        {
            return _otpService.ValidateOTP(pUsuario, pOtp);
        }

        public Usuarios GetUserById(int pId)
        {
            var appList = _loginCrud.RetrieveByUserId<Usuarios>(pId);

            if (appList.Count > 0)
            {
                return appList[0];
            }
            return new Usuarios();
        }


    }
}

