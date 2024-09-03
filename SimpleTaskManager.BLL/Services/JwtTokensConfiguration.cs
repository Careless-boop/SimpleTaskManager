namespace SimpleTaskManager.BLL.Services
{
    public class JwtTokensConfiguration
    {
        public double AccessTokenExpirationMinutes { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
