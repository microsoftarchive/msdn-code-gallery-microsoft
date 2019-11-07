using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using System.Text;

namespace CSOneDriveAccess
{
    public abstract class O365RestSessionBase
    {
        #region property
        #region config param
        /// <summary>
        /// The auth API root address
        /// </summary>
        public string AuthRoot { get; set; } = "https://login.live.com/";
        /// <summary>
        /// when user finished the auth, office 365 will request this URI with a code.
        /// </summary>
        public string CallBackURI { get; set; }
        /// <summary>
        /// clientId of you office 365 application, you can find it in https://apps.dev.microsoft.com/
        /// </summary>
        public string ClientId { get; }
        /// <summary>
        /// Password/Public Key of you office 365 application, you can find it in https://apps.dev.microsoft.com/
        /// </summary>
        public string ClientSecret { get; }
        #endregion

        #region auth property
        /// <summary>
        /// when user complated the authenticate, will retrun this code
        /// </summary>
        public string AccessCode { get; protected set; }
        /// <summary>
        /// use this token to request Office 365 API
        /// </summary>
        public string AccessToken { get; protected set; }
        /// <summary>
        /// when accessToken had expires, can use this token to refresh accessToken
        /// </summary>
        public string RefreshToken { get; protected set; }
        /// <summary>
        /// The last time of you get token
        /// </summary>
        public DateTime RefreshTime { get; protected set; }
        /// <summary>
        /// Refresh time span
        /// </summary>
        public TimeSpan RefreshTimeSpan { get; protected set; }
        #endregion

        /// <summary>
        /// User ID from office 365, this is got by when user auth login on you office 365 application.
        /// </summary>
        public string UserId { get; protected set; }
        #endregion

        #region constructed method
        /// <summary>
        /// Init a O365RestSessionBase object
        /// </summary>
        /// <param name="clientId">clientId of you office 365 application, you can find it in https://apps.dev.microsoft.com/</param>
        /// <param name="clientSecret">Password/Public Key of you office 365 application, you can find it in https://apps.dev.microsoft.com/</param>
        /// <param name="redirectURI">Authentication callback url, you can set it in https://apps.dev.microsoft.com/</param>
        public O365RestSessionBase(string clientId, string clientSecret, string redirectURI)
        {
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
            this.CallBackURI = redirectURI;
        }
        #endregion

        #region Auth method
        /// <summary>
        /// retrun authenticate url for redirect
        /// </summary>
        /// <param name="scopes">about the scopes, see at https://dev.onedrive.com/auth/msa_oauth.htm#authentication-scopes</param>
        /// <returns></returns>
        public string GetLoginUrl(string scopes)
        {
            string urlStr = $"{AuthRoot}oauth20_authorize.srf?client_id={ClientId}&scope={scopes} offline_access&response_type=code&redirect_uri={CallBackURI}";

            return urlStr.ToString();
        }

        /// <summary>
        /// get token use code
        /// </summary>
        /// <param name="code">when user complated the authenticate, will retrun this code to you call back uri</param>
        /// <returns></returns>
        public async Task RedeemTokensAsync(string code)
        {
            this.AccessCode = code;

            string url = $"{AuthRoot}oauth20_token.srf";
            byte[] param = Encoding.UTF8.GetBytes($"client_id={ClientId}&redirect_uri={CallBackURI}&client_secret={ClientSecret}&code={AccessCode}&grant_type=authorization_code");

            ApiRequest request = new ApiRequest(url, HTTPMethod.Post, param, "application/x-www-form-urlencoded");

            string response = await request.GetResponseStringAsync();
            AnalysisAuthResult(response);
        }

        /// <summary>
        /// refresh token if needed
        /// </summary>
        /// <returns></returns>
        private async Task RefreshTokenIfNeededAsync()
        {
            if (RefreshTimeSpan == null || DateTime.Now - RefreshTime > RefreshTimeSpan)
            {
                string url = $"{AuthRoot}oauth20_token.srf";
                byte[] param = Encoding.UTF8.GetBytes($"client_id={ClientId}&redirect_uri={CallBackURI}&client_secret={ClientSecret}&refresh_token={RefreshToken}&grant_type=refresh_token");

                ApiRequest request = new ApiRequest(url, HTTPMethod.Post, param, "application/x-www-form-urlencoded");

                string response = await request.GetResponseStringAsync();

                AnalysisAuthResult(response);
            }
        }

        /// <summary>
        /// analysis auth result
        /// </summary>
        /// <param name="authResult"></param>
        private void AnalysisAuthResult(string authResult)
        {
            JObject jo = JObject.Parse(authResult);

            this.RefreshToken = jo.SelectToken("refresh_token").Value<string>();
            this.AccessToken = jo.SelectToken("access_token").Value<string>();
            this.UserId = jo.SelectToken("user_id").Value<string>();
            this.RefreshTimeSpan = new TimeSpan(0, 0, Convert.ToInt32(jo.SelectToken("expires_in").Value<string>()));
            this.RefreshTime = DateTime.Now;
        }
        #endregion

        #region api request method
        /// <summary>
        /// start a request with auth info and get response as string
        /// </summary>
        /// <param name="uri">Request uri</param>
        /// <param name="httpMethod">HttpMethod</param>
        /// <param name="data">The requst body</param>
        /// <param name="contentType">Request content type</param>
        /// <returns></returns>
        protected async Task<string> AuthRequestToStringAsync(string uri, HTTPMethod httpMethod = HTTPMethod.Get, byte[] data = null, string contentType = null)
        {
            ApiRequest request = await InitAuthRequest(uri, httpMethod, data, contentType);
            return await request.GetResponseStringAsync();
        }

        /// <summary>
        /// start a request with auth info and get response as byte[]
        /// </summary>
        /// <param name="uri">Request uri</param>
        /// <param name="httpMethod">HttpMethod</param>
        /// <param name="data">The requst body</param>
        /// <param name="contentType">Request content type</param>
        /// <returns></returns>
        protected async Task<byte[]> AuthRequestToBytesAsync(string uri, HTTPMethod httpMethod = HTTPMethod.Get, byte[] data = null, string contentType = null)
        {
            ApiRequest request = await InitAuthRequest(uri, httpMethod, data, contentType);
            return await request.GetResponseBytesAsync();
        }

        /// <summary>
        /// Init a request with auth info
        /// </summary>
        /// <param name="uri">Request uri</param>
        /// <param name="httpMethod">HttpMethod</param>
        /// <param name="data">The requst body</param>
        /// <param name="contentType">Request content type</param>
        /// <returns></returns>
        protected async Task<ApiRequest> InitAuthRequest(string uri, HTTPMethod httpMethod, byte[] data, string contentType)
        {
            await RefreshTokenIfNeededAsync();

            ApiRequest apiRequest = new ApiRequest(uri, httpMethod, data, contentType);

            if (!string.IsNullOrEmpty(AccessToken))
            {
                apiRequest.Request.Headers.Add("Authorization", $"bearer {AccessToken}");
            }

            return apiRequest;
        }
        #endregion
    }
}
