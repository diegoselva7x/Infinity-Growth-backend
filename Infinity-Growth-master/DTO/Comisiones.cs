using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class Comisiones
    {
        public int Id { get; set; }
        public int IdTransaccion {  get; set; }
        public int Usuario {  get; set; }
        public decimal Monto { get; set; }
        public Boolean Estado { get; set; }
        public DateTime Fecha { get; set; }
    }
}
