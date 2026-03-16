namespace DTO
{
    public class UpdatePasswordRequest
    {
        public string Correo { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
