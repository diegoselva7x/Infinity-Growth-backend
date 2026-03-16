using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ReporteClienteResumen
    {
        public DateTime Fecha { get; set; }
        public string Nombre { get; set; }
        public bool TipoTransaccion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioTotal { get; set; }
        public int Id_Usuario { get; set; }

        
    }
}
