using Microsoft.AspNetCore.Mvc;
using System.Net;
using Villa_API.Models;
using Villa_API.Models.Dto.Security;
using Villa_API.Repository.IRepository;

namespace Villa_API.Controllers
{
    [Route("api/UserAuth")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly APIResponse _response;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            this._response = new();
        }


        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LogIn([FromBody] LogInRequestDTO model)
        {
            var logInResponse = await _userRepository.LogIn(model);
            if (logInResponse.User == null || string.IsNullOrEmpty(logInResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username or Password is incorrect!");
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = logInResponse;

            return Ok(_response);
        }


        
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            bool ifUserNameUnique = _userRepository.IsUniqueUser(model.UserName);
            if (!ifUserNameUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username already exists!");
                return BadRequest(_response);
            }

            var user = await _userRepository.Register(model);
            if (user == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error while registering");
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;

            return Ok(_response);
        }
    }
}
