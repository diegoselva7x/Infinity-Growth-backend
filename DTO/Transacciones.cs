using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class Transacciones
    {
        public int Id { get; set; }
        public int Usuario { get; set; }
        public int Activo { get; set; }
        public Boolean Tipo { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioTotal { get; set; }
    }
}
