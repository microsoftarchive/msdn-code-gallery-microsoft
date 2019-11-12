using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace CSOneDriveAccess
{
    public abstract class OAuthAccessBase
    {
        public string ClientId { get; }
        public string ClientSecret { get; }

        //when user complated the authenticate, will retrun this code
        public string AccessCode { get; protected set; }
        //use this token to request Office 365 API
        public string AccessToken { get; protected set; }
        //when accessToken had expires, can use this token to refresh accessToken
        public string RefreshToken { get; protected set; }
        
        public string UserId { get; protected set; }
        public DateTime RefreshTime { get; protected set; }
        public TimeSpan RefreshTimeSpan { get; protected set; }

        public string RedirectURI { get; set; }

        public OAuthAccessBase(string clientId, string clientSecret, string redirectURI)
        {
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
            this.RedirectURI = redirectURI;
        }

        //retrun authenticate url for redirect
        public string GetLoginUrl(string scopes)
        {
            string urlStr =
                "https://login.live.com/oauth20_authorize.srf" +
                "?client_id=" + ClientId +
                "&scope=offline_access " + scopes +
                "&response_type=code" +
                "&redirect_uri=" + RedirectURI;

            return urlStr.ToString();
        }

        //get token use code
        public async Task RedeemTokensAsync(string code)
        {
            this.AccessCode = code;

            string url = "https://login.live.com/oauth20_token.srf";
            string paramStr =
                "client_id=" + ClientId +
                "&redirect_uri=" + RedirectURI +
                "&client_secret=" + ClientSecret +
                "&code=" + AccessCode +
                "&grant_type=authorization_code";

            APIRequest request = GetRequest(url, HTTPMethod.Post, paramStr.ToString());
            string response = await request.GetResponseToStringAsync();

            JObject jo = JObject.Parse(response);

            this.RefreshToken = jo.SelectToken("refresh_token").Value<string>();
            this.AccessToken = jo.SelectToken("access_token").Value<string>();
            this.UserId = jo.SelectToken("user_id").Value<string>();
            this.RefreshTimeSpan = new TimeSpan(0, 0, Convert.ToInt32(jo.SelectToken("expires_in").Value<string>()));
            this.RefreshTime = DateTime.Now;
        }


        protected async Task<string> AuthRequestToStringAsync(string url, HTTPMethod httpMethod = HTTPMethod.Get, string data = "")
        {
            await RefreshTokenIfNeededAsync();

            APIRequest request = GetRequest(url, httpMethod, data);
            return await request.GetResponseToStringAsync();
        }

        protected async Task<byte[]> AuthRequestToBytesAsync(string url, HTTPMethod httpMethod = HTTPMethod.Get, string data = "")
        {
            await RefreshTokenIfNeededAsync();

            APIRequest request = GetRequest(url, httpMethod, data);
            return await request.GetResponseTobytesAsync();
        }

        private APIRequest GetRequest(string url, HTTPMethod httpMethod = HTTPMethod.Get, string data = "")
        {
            APIRequest apiRequest = new APIRequest(url, httpMethod, data);

            if (!string.IsNullOrEmpty(AccessToken))
            {
                apiRequest.Request.Headers.Add("Authorization", "bearer " + AccessToken);
            }

            return apiRequest;
        }

        private async Task RefreshTokenIfNeededAsync()
        {
            if (RefreshTimeSpan == null || DateTime.Now - RefreshTime > RefreshTimeSpan)
            {
                string url = "https://login.live.com/oauth20_token.srf";
                string paramStr = "client_id=" + ClientId +
                                    "&redirect_uri=" + RedirectURI +
                                    "&client_secret=" + ClientSecret +
                                    "&refresh_token=" + RefreshToken +
                                    "&grant_type=refresh_token";

                APIRequest request = GetRequest(url, HTTPMethod.Post, paramStr.ToString());
                string response = await request.GetResponseToStringAsync();

                JObject jo = JObject.Parse(response);

                this.RefreshToken = jo.SelectToken("refresh_token").Value<string>();
                this.AccessToken = jo.SelectToken("access_token").Value<string>();
                this.UserId = jo.SelectToken("user_id").Value<string>();
                this.RefreshTimeSpan = new TimeSpan(0, 0, Convert.ToInt32(jo.SelectToken("expires_in").Value<string>()));
                this.RefreshTime = DateTime.Now;
            }
        }
    }
}
