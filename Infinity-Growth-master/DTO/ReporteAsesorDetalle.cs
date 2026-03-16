using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ReporteAsesorDetalle
    {
        // Resumen Financiero aun no implementado

        public decimal TotalInvertidoPorClientes { get; set; }
        public decimal TotalComisionesGeneradas { get; set; }
        public decimal GananciasAcumuladas { get; set; }
        public decimal PerdidasAcumuladas { get; set; }
        public string AccionMasRentable { get; set; }
        public string AccionMenosRentable { get; set; }
    }
}
