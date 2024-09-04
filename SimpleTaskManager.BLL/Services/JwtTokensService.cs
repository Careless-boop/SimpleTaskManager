using Microsoft.IdentityModel.Tokens;
using SimpleTaskManager.BLL.Configurations;
using SimpleTaskManager.BLL.Interfaces;
using SimpleTaskManager.DAL.Models;
using SimpleTaskManager.DAL.Repository.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SimpleTaskManager.BLL.Services
{
    public class JwtTokensService : IJwtTokensService
    {
        private readonly JwtTokensConfiguration _configuration;
        private readonly IRepositoryWrapper _repository;

        public JwtTokensService(JwtTokensConfiguration configuration, IRepositoryWrapper repository)
        {
            _configuration = configuration;
            _repository = repository;
        }

        public string GenerateAccessToken(User user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.NameIdentifier, user.Username!),
                new Claim(ClaimTypes.Email, user.Email!)
            };
            var expiration = DateTime.UtcNow.AddMinutes(_configuration.AccessTokenExpirationMinutes);
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration.Key));
            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken tokenTemplate = new(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: signingCredentials);

            JwtSecurityTokenHandler tokenHandler = new();
            string token = tokenHandler.WriteToken(tokenTemplate);

            return token;
        }

        public async Task<User?> GetUserFromAccessTokenAsync(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            var jwtToken = tokenHandler.ReadToken(accessToken) as JwtSecurityToken;
            var userIdString = jwtToken!
                .Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?
                .Value ??
                string.Empty;

            var userId = new Guid(userIdString);

            var user = await _repository.UserRepository.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
            {
                return null;
            }

            return user;
        }
    }
}
