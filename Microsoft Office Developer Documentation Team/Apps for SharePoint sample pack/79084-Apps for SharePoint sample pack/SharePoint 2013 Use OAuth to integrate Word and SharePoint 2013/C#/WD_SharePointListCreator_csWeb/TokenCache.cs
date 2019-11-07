using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Configuration;

namespace WD_SharePointOAuth_csWeb
{
    public class TokenCache
    {
        private const string REFRESH_TOKEN_COOKIE_NAME = "RefreshToken";
        private static readonly string redirectUrl = WebConfigurationManager.AppSettings.Get("RedirectUrl");

        public static void UpdateCacheWithCode(HttpRequest request, HttpResponse response, string code, Uri targetUri)
        {
            string refreshToken = TokenHelper.GetAccessToken(code, "00000003-0000-0ff1-ce00-000000000000", targetUri.Authority, TokenHelper.GetRealmFromTargetUrl(targetUri), new Uri(redirectUrl)).RefreshToken;
            SetRefreshTokenCookie(request.Cookies, refreshToken);
            SetRefreshTokenCookie(response.Cookies, refreshToken);
        }

        internal static string GetCachedRefreshToken(HttpCookieCollection requestCookies)
        {
            return GetRefreshTokenFromCookie(requestCookies);
        }

        internal static bool IsTokenInCache(HttpCookieCollection requestCookies)
        {
            return (requestCookies[REFRESH_TOKEN_COOKIE_NAME] != null);
        }

        private static string GetRefreshTokenFromCookie(HttpCookieCollection cookies)
        {
            if (cookies[REFRESH_TOKEN_COOKIE_NAME] != null)
            {
                return cookies[REFRESH_TOKEN_COOKIE_NAME].Value;
            }
            else
            {
                return null;
            }
        }

        private static void SetRefreshTokenCookie(HttpCookieCollection cookies, string refreshToken)
        {
            if (cookies[REFRESH_TOKEN_COOKIE_NAME] != null)
            {
                cookies[REFRESH_TOKEN_COOKIE_NAME].Value = refreshToken;
            }
            else
            {
                HttpCookie cookie = new HttpCookie(REFRESH_TOKEN_COOKIE_NAME, refreshToken);
                cookie.Expires = DateTime.Now.AddDays(30);
                cookies.Add(cookie);
            }
        }
    }
}