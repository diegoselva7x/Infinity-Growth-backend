namespace DTO
{
    public class Asesor
    {
        public int Id { get; set; }
        public string Correo { get; set; }
        public string Nombre { get; set; }
        public string Apellido1 { get; set; }
        public string Apellido2 { get; set; }
        public string TipoUsuario { get; set; }     // Ej: "Asesor"
        public string Estado { get; set; }          // Ej: "Activo"
        public DateTime FechaNacimiento { get; set; }
        public string Direccion { get; set; }
    }
}
