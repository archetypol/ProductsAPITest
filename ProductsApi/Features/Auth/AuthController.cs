using Microsoft.AspNetCore.Mvc;
using ProductsApi.Auth.Models;
using ProductsApi.Auth.Service;

namespace ProductsApi.Auth.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserAuthService _authService;

        public AuthController(UserAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userRegisteredResult = await _authService.RegisterUser(model);
            if (!userRegisteredResult.IsSuccess)
            {
                return BadRequest(userRegisteredResult.Errors);
            }

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var validUserResult = await _authService.LoginUser(model);
            if (!validUserResult.IsSuccess)
                return BadRequest("Invalid Credentials");

            var user = validUserResult.Value;
            var jwtToken = _authService.MintJWTToken(user);

            return Ok(new LoginResponse { Token = jwtToken });
        }
    }
}
