using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ReporteAdminResumen
    {
        public decimal TotalInvertidoPlataforma { get; set; }
        public decimal ComisionPlataformaTotal { get; set; }
        public decimal GananciasGeneradas { get; set; }
        public decimal PerdidasGeneradas { get; set; }
        public decimal VolumenMovidoMensual { get; set; }
        public string AccionMasRentable { get; set; }
        public string AccionMenosRentable { get; set; }
    }

}
