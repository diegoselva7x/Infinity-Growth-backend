using AppLogic.Services;
using DataAccess.Crud;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;



namespace AppLogic
{
    public interface IRecuperarPasswordManager
    {
        bool UpdatePassword(string email, string newPassword);
        List<Usuarios> GetUserForReset(string email);
    }
    public class RecuperarPasswordManager : IRecuperarPasswordManager
    {
        private readonly LoginCrud _loginCrud;
        private readonly RecuperarPasswordCrud _recuperarPasswordCrud;
        private readonly OTP_Service _otpService;

        public RecuperarPasswordManager(LoginCrud loginCrud, RecuperarPasswordCrud recuperarPasswordCrud, OTP_Service otpService)
        {
            _loginCrud = loginCrud;
            _recuperarPasswordCrud = recuperarPasswordCrud;
            _otpService = otpService;
        }

        public List<Usuarios> GetUserForReset(string email) // Se obtiene el usuario para recuperar la contraseña y se genera un OTP
        {
            var usuario = _loginCrud.RetrieveByCorreo<Usuarios>(email);
            if (usuario.Count > 0)
            {
                string solicitud = "Recuperar contraseña";
                _ = _otpService.GenerateOTP(usuario[0].Id, email, solicitud);

                return usuario;
            }

            return new List<Usuarios>();
        }

        public bool UpdatePassword(string email, string newPassword)
        {
            var usuario = _loginCrud.RetrieveByCorreo<Usuarios>(email);
            if (usuario.Count > 0)
            {
                var user = usuario[0];
                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                _recuperarPasswordCrud.UpadatePassword(user.Correo, user.Password);
                return true;
            }
            return false;
        }
    }
}
