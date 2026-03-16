using System.ComponentModel.DataAnnotations.Schema;

namespace DTO
{
    public class Usuarios
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public int TipoUsuario { get; set; } // 1: Administrador, 2: Empleado, 3: Cliente
        public int Estado {  get; set; }
        public string Nombre { get; set; }
        public string Apellido1 { get; set; }
        public string Apellido2 { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Correo {  get; set; }
        public string Direccion {  get; set; }
        public byte[] ProfileFoto { get; set; }
        public bool isPasswordTemp { get; set; }

        [NotMapped] // Evita que esta propiedad se mapee en la base de datos
        public string ProfileFotoBase64
        {
            get => ProfileFoto != null ? Convert.ToBase64String(ProfileFoto) : null; // Convierte byte[] a Base64
            set => ProfileFoto = !string.IsNullOrEmpty(value) ? Convert.FromBase64String(value) : null; // Convierte Base64 a byte[]
        }

        [NotMapped]
        public int IdAsesor { get; set; }
    }
}
