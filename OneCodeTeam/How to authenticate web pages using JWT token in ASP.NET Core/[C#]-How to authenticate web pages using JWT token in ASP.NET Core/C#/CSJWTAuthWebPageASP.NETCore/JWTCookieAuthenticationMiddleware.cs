using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace CSJWTAuthWebPageASP.NETCore
{
    public class JWTCookieAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public static string LoginPagePath;

        public JWTCookieAuthenticationMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Cookies["Authorization"];
            if (!string.IsNullOrWhiteSpace(token))
            {
                context.Request.Headers["Authorization"] = token;
            }

            await _next(context);
            if (context.Response.StatusCode == 401)
            {
                if (context.Request.IsAjaxRequest())
                {
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(
                        new { authenticated = false, tokenExpired = true }
                    ));
                }
                else
                {
                    context.Response.Redirect(LoginPagePath);
                }
            }
        }
    }

    public static class JWTCookieAuthMiddlewareExtensions
    {
        public static IApplicationBuilder EnableJwtCookieAuthentication(this IApplicationBuilder app, string loginPagePath)
        {
            JWTCookieAuthenticationMiddleware.LoginPagePath = loginPagePath;

            return app.UseMiddleware<JWTCookieAuthenticationMiddleware>();
        }
    }
}
