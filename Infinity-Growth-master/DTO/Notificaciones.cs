using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class Notificaciones
    {
        public int Id { get; set; }
        public int Usuario {  get; set; }
        public DateTime Fecha { get; set; }
        public string Asunto { get; set; }
        public string Texto { get; set; }
        public string EmailOrigen {  get; set; }
        public string EmailDestino { get; set; }
    }
}
