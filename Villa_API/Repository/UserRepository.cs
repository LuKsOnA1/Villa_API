using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Villa_API.Data;
using Villa_API.Models.Dto.Security;
using Villa_API.Models.User;
using Villa_API.Repository.IRepository;

namespace Villa_API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private string secretKey;

        public UserRepository(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            this.secretKey = config.GetValue<string>("ApiSettings:Secret");
        }


        public bool IsUniqueUser(string username)
        {
            var user = _db.LocalUsers.FirstOrDefault(x => x.UserName == username);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<LogInResponseDTO> LogIn(LogInRequestDTO logInRequestDTO)
        {
            var user = _db.LocalUsers.FirstOrDefault(x => x.UserName.ToLower() ==  logInRequestDTO.UserName.ToLower()
            && x.Password == logInRequestDTO.Password);

            if (user == null)
            {
                return new LogInResponseDTO()
                {
                    Token = "",
                    User = null
                };
            }

            // if user was found generate JWT Token ...

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            // describing token 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
             // creating token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            LogInResponseDTO logInResponseDTO = new LogInResponseDTO
            {
                Token = tokenHandler.WriteToken(token),
                User = user
            };

            return logInResponseDTO;
        }

        public async Task<LocalUser> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            LocalUser user = new()
            {
                UserName = registrationRequestDTO.UserName,
                Password = registrationRequestDTO.Password,
                Name = registrationRequestDTO.Name,
                Role = registrationRequestDTO.Role
            };

            _db.LocalUsers.Add(user);
            await _db.SaveChangesAsync();
            user.Password = "";
            return user;
        }
    }
}
