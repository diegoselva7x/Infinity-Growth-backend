using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class OTP_Codes
    {
        public int Id_otp {  get; set; }    
        public int Id_usuario { get; set; } 
        public int Codigo { get; set; }
        public DateTime FechaEnvio { get; set; }
        public DateTime FechaVencimiento{ get; set; }   
        public Boolean Activo {  get; set; }    


    }
}
