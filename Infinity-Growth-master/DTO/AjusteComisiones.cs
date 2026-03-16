using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class AjusteComisiones
    {
       
        public int IdTipoComision { get; set; }

    
        public string Nombre { get; set; } 

      
        public string? Descripcion { get; set; } 

       
        public decimal Porcentaje { get; set; }
    }
}

