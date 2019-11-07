using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CSOneDriveAccess
{
    public class OAuthAccess : OAuthAccessBase
    {
        public OAuthAccess(string clientId, string clientSecret, string redirectURI) : base(clientId, clientSecret, redirectURI)
        {
        }

        public async Task<Dictionary<string, string>> GetAccountInfoAsync()
        {
            string response = await AuthRequestToStringAsync("https://apis.live.net/v5.0/me?suppress_response_codes=true&suppress_redirects=true");

            JObject jo = JObject.Parse(response);

            return jo.ToObject<Dictionary<string, string>>();
        }

        public async Task<byte[]> GetAccountPicture()
        {
            byte[] response = await AuthRequestToBytesAsync("https://apis.live.net/v5.0/me/picture");

            return response;
        }
    }


}
