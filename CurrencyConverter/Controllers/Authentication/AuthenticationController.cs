using CurrencyConverter.API.JwtAuthentication;
using CurrencyConverter.API.JwtAuthentication.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace CurrencyConverter.API.Controllers.Authentication
{
    [ExcludeFromCodeCoverage]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthenticationController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }
        // Add Login method
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            // Add logic to authenticate user
            if (login.Username == "admin" && login.Password == "admin")
            {
                var token = _tokenService.GenerateAccessToken("1", ["Admin"]);
                return Ok(new { Token = token });
            }
            if (login.Username == "user" && login.Password == "user")
            {
                var token = _tokenService.GenerateAccessToken("2", ["User"]);
                return Ok(new { Token = token });
            }
            if (login.Username == "viewer" && login.Password == "viewer")
            {
                var token = _tokenService.GenerateAccessToken("3", ["Viewer"]);
                return Ok(new { Token = token });
            }
            return Unauthorized();
        }
    }
}
