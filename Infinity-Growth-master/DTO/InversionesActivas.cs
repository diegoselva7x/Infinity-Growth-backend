using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class InversionesActivas
    {
        public int IdTransaccion { get; set; }
        public int Id { get; set; }
        public string Ticker { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioCompra {  get; set; }
        public decimal PrecioVenta { get; set; }
        public bool Activo { get; set; }
        public int Id_usuario { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioTotal { get; set; }
    }
}
 