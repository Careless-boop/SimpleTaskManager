using Microsoft.AspNetCore.Http;
using SimpleTaskManager.BLL.Interfaces;

namespace SimpleTaskManager.BLL.Services
{
    public class CookiesService : ICookiesService
    {
        public async Task AppendCookiesToResponseAsync(HttpResponse httpResponse, params (string key, string value, CookieOptions options)[] values)
        {
            await Task.Run(() =>
            {
                foreach (var cookie in values)
                {
                    httpResponse.Cookies.Append(cookie.key, cookie.value, cookie.options);
                }
            });
        }
    }
}
