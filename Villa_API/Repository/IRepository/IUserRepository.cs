using Villa_API.Models;
using Villa_API.Models.Dto.Security;

namespace Villa_API.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        Task<LogInResponseDTO> LogIn(LogInRequestDTO logInRequestDTO);
        Task<LocalUser> Register(RegistrationRequestDTO registrationRequestDTO);
    }
}
