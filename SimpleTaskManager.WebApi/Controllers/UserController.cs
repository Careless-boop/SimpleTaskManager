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
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService,
            IJwtTokensService jwtTokensService,
            ICookiesService cookiesService,
            JwtTokensConfiguration jwtConfiguration,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _jwtTokensService = jwtTokensService;
            _cookiesService = cookiesService;
            _jwtConfiguration = jwtConfiguration;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterUserDTO registerUserDto)
        {
            var result = await _userService.RegisterUserAsync(registerUserDto);
            if (!result.Success)
            {
                _logger.LogWarning($"User registration failed.\n\t" +
                    $"Email:{registerUserDto.Email}\n\t" +
                    $"Username:{registerUserDto.Username}\n\t" +
                    $"Message:{result.Message}");
                return BadRequest(result.Message);
            }

            _logger.LogInformation($"User registrated.\n\t" +
                    $"Email:{registerUserDto.Email}\n\t" +
                    $"Username:{registerUserDto.Username}\n\t");

            return Ok(result.Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO loginUserDto)
        {
            var user = await _userService.AuthenticateUserAsync(loginUserDto);
            if(user == null)
            {
                _logger.LogWarning($"User login failed.\n\t" +
                    $"Login:{loginUserDto.Login}\n\t" +
                    $"Message: Invalid credentials.");
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

            _logger.LogInformation($"User loged in.\n\t" +
                    $"Login: {loginUserDto.Login}\n\t");

            return Ok("User loged in successfully.");
        }
    }
}
