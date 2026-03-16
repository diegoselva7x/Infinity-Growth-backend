using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class UsuarioReporte
    {
        public int Id { get; set; }
        public string? Correo { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido1 { get; set; }
        public string? Apellido2 { get; set; }
        public string? TipoUsuario { get; set; }
        public string? Estado { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? Direccion { get; set; }
        public int? IdAsesor { get; set; }
        public string NombreAsesor { get; set; }

    }
}
