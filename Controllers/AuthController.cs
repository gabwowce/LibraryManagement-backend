using LibraryManagement.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Models;


namespace LibraryManagement.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;

        public AuthController(AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LibraryManagement.Models.LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Invalid request");
            }

            var (token, member) = _authenticationService.Authenticate(request.Username, request.Password);
            if (token == null)
            {
                return Unauthorized();
            }

            var userDto = new MemberDto
            {
                Name = member.Name,
                Surname = member.Surname,
                Role = member.Role
            };

            return Ok(new { Token = token, User = userDto });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logged out successfully" });
        }

    }
}
