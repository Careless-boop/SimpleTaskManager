using Microsoft.AspNetCore.Http;

namespace SimpleTaskManager.BLL.Interfaces
{
    public interface ICookiesService
    {
        public Task AppendCookiesToResponseAsync(HttpResponse httpResponse, params (string key, string value, CookieOptions options)[] values);
    }
}
