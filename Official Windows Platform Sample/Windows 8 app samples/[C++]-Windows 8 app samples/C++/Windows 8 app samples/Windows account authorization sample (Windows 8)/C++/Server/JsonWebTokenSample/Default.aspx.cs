using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicrosoftAccount.Server.JsonWebTokenSample;

public partial class _Default : System.Web.UI.Page
{
    // sample class to store per UserId information
    class _ClientInfo
    {
        public string userId;
        public DateTime lastModified;
        public int numberOfRequests;

        public _ClientInfo(string id)
        {
            userId = id;
            lastModified = DateTime.Now;
            numberOfRequests = 1;
        }

        public void Hit()
        {
            lastModified = DateTime.Now;
            numberOfRequests++;
        }

        public string Serialize()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("userId", userId);
            dict.Add("lastModified", lastModified.ToString());
            dict.Add("numberOfRequests", numberOfRequests);
            return serializer.Serialize((object)dict);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get the AppSettings section.
        NameValueCollection appSettings = ConfigurationManager.AppSettings;
        
        try
        {
            if (Request.QueryString["access_token"] != null)
            {
                _ClientInfo clientInfo = null;
                Dictionary<int, string> keyIdsKeys = new Dictionary<int, string>();

                // NOTE: these keys should be kept protected with DPAPI 
                // and not exposed in clear text within the web code.
                keyIdsKeys.Add(0, appSettings["key_0"] );

                JsonWebToken token = new JsonWebToken(Request.QueryString["access_token"]);

                // Validate signature and ensure that the token is issued 
                // to this client and this target
                token.Validate(keyIdsKeys, appSettings["appId"], appSettings["audience"]);

                Application.Lock();
                if (Application[token.Claims.UserId] != null)
                {
                    clientInfo = (_ClientInfo)Application[token.Claims.UserId];
                    clientInfo.Hit();
                }
                else
                {
                    clientInfo = new _ClientInfo(token.Claims.UserId);
                    Application[token.Claims.UserId] = clientInfo;
                }

                Application.UnLock();

                Response.Write(clientInfo.Serialize());
             }
            else
            {
                throw new Exception(string.Format("Could not find access_token in QueryString"));
            }
        }
        catch (Exception)
        {
            Response.Status = "401 Unauthorized";
            Response.Write("Access denied.");
        }
    }
}