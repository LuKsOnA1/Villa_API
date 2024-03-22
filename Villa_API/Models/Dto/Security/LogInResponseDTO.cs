namespace Villa_API.Models.Dto.Security
{
    public class LogInResponseDTO
    {
        public LocalUser User { get; set; }
        public string Token { get; set; }
    }
}
