using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class InformacionPortafolio
    {
        public int idActivo { get; set; }
        public string Nombre { get; set; }
        public string Ticker { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioTotal { get; set; }

        public decimal GananciaMonetaria { get; set; }
        public decimal GananciaPorcentual { get; set; }
    }
}
