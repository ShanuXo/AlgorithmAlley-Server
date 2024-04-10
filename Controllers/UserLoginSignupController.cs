using AlgorithmAlley.DTO;
using AlgorithmAlley.Models;
using AlgorithmAlley.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AlgorithmAlley.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginSignupController : ControllerBase
    {
        private readonly IUserServices _userService;

        public UserLoginSignupController(IUserServices userService)
        {
            _userService = userService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(UserInformationDTO userInformationDTO)
        {
            try
            {
                var response = await _userService.SignUpAsync(userInformationDTO);
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponses<string>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrorMessage = new List<string> { ex.Message }
                });
            }
        }
      
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            try
            {
                var response = await _userService.LoginAsync(loginDTO.Email, loginDTO.Password);
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponses<string>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrorMessage = new List<string> { ex.Message }
                });
            }
        }
    }
}
