using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleTaskManager.BLL.Configurations;
using SimpleTaskManager.BLL.DTOs;
using SimpleTaskManager.BLL.Interfaces;

namespace SimpleTaskManager.WebApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtTokensService _jwtTokensService;
        private readonly ICookiesService _cookiesService;
        private readonly JwtTokensConfiguration _jwtConfiguration;

        public UserController(
            IUserService userService,
            IJwtTokensService jwtTokensService,
            ICookiesService cookiesService,
            JwtTokensConfiguration jwtConfiguration)
        {
            _userService = userService;
            _jwtTokensService = jwtTokensService;
            _cookiesService = cookiesService;
            _jwtConfiguration = jwtConfiguration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterUserDTO registerUserDto)
        {
            var result = await _userService.RegisterUserAsync(registerUserDto);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO loginUserDto)
        {
            var user = await _userService.AuthenticateUserAsync(loginUserDto);
            if(user == null)
            {
                return Unauthorized("Invalid credentials!");
            }
            var token = _jwtTokensService.GenerateAccessToken(user);

            await _cookiesService.AppendCookiesToResponseAsync(HttpContext!.Response,
                ("accessToken", token, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddMinutes(_jwtConfiguration.AccessTokenExpirationMinutes),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                }));

            return Ok("User loged in successfully.");
        }
    }
}
