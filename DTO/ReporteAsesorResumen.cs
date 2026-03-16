using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ReporteAsesorResumen
    {
        public int Id_Usuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public decimal InversionTotal { get; set; }
        public decimal Ganancias { get; set; }
        public decimal Perdidas { get; set; }
        public decimal ComisionGeneradaDelUsuario { get; set; }
        public decimal RendimientoTotal { get; set; }
    }
}
