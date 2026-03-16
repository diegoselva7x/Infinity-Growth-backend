using System;

namespace DTO
{
    public class ReporteAdminDetalle
    {
        // Información por cliente (detalle)
        public int Id_usuario { get; set; }
        public string NombreAsesor { get; set; }

        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public decimal Ingresos { get; set; }
        public decimal Ganancias { get; set; }
        public decimal Perdidas { get; set; }
        public decimal Rendimiento { get; set; }
    }
}
