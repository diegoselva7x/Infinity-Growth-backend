using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ReporteClienteDetalle
    {
        public decimal TotalInvertido { get; set; }
        public decimal GananciaAcumulada { get; set; }
        public decimal PerdidaAcumulada { get; set; }
        public decimal ComisionPagada { get; set; }

        public string AccionMasRentable { get; set; }
        public string AccionMenosRentable { get; set; }
    }

}
