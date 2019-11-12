/****************************** Module Header ******************************\
* Module Name:  AutoLogin.aspx.cs
* Project:      CSASPNETAutoLogin
* Copyright (c) Microsoft Corporation.
* 
* This page request the Login.aspx firstly, and get the __VIEWSTATE and __EVENTVALIDATION'fields.
* Then we can set the post data string, such as the __VIEWSTATE, __EVENTVALIDATION,
* UserName,Password and loginButton id parameters.
* We use the webrequest to post these data into the login.aspx to login this site.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Net;
using System.Web;
using System.Text;


namespace CSASPNETAutoLogin
{
    public partial class AutoLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Clear();
        }

        protected void autoLogin_Click(object sender, EventArgs e)
        {
            string url = HttpContext.Current.Request.Url.AbsoluteUri.ToString().Replace("AutoLogin", "Login");
            CookieContainer myCookieContainer = new CookieContainer();
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.CookieContainer = myCookieContainer;
            request.Method = "GET";
            request.KeepAlive = false;

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            System.IO.Stream responseStream = response.GetResponseStream();
            System.IO.StreamReader reader = new System.IO.StreamReader(responseStream, Encoding.UTF8);
            string srcString = reader.ReadToEnd();

            // get the page ViewState                
            string viewStateFlag = "id=\"__VIEWSTATE\" value=\"";
            int i = srcString.IndexOf(viewStateFlag) + viewStateFlag.Length;
            int j = srcString.IndexOf("\"", i);
            string viewState = srcString.Substring(i, j - i);

            // get page EventValidation                
            string eventValidationFlag = "id=\"__EVENTVALIDATION\" value=\"";
            i = srcString.IndexOf(eventValidationFlag) + eventValidationFlag.Length;
            j = srcString.IndexOf("\"", i);
            string eventValidation = srcString.Substring(i, j - i);

            string submitButton = "LoginButton";

            // UserName and Password
            string userName = btnUserName.Text;
            string password = btnPassword.Text;
            // Convert the text into the url encoding string
            viewState = System.Web.HttpUtility.UrlEncode(viewState);
            eventValidation = System.Web.HttpUtility.UrlEncode(eventValidation);
            submitButton = System.Web.HttpUtility.UrlEncode(submitButton);

            // Concat the string data which will be submit
            string formatString =
                     "UserName={0}&Password={1}&loginButton={2}&__VIEWSTATE={3}&__EVENTVALIDATION={4}";
            string postString =
                     string.Format(formatString, userName, password, submitButton, viewState, eventValidation);

            // Convert the submit string data into the byte array
            byte[] postData = Encoding.ASCII.GetBytes(postString);

            // Set the request parameters
            request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.Referer = url;
            request.KeepAlive = false;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.2; CIBA)";
            request.ContentType = "application/x-www-form-urlencoded";
            request.CookieContainer = myCookieContainer;
            System.Net.Cookie ck = new System.Net.Cookie("TestCookie1", "Value of test cookie");
            ck.Domain = request.RequestUri.Host;
            request.CookieContainer.Add(ck);
            request.CookieContainer.Add(response.Cookies);

            request.ContentLength = postData.Length;

            // Submit the request data
            System.IO.Stream outputStream = request.GetRequestStream();
            request.AllowAutoRedirect = true;
            outputStream.Write(postData, 0, postData.Length);
            outputStream.Close();

            
            // Get the return data
            response = request.GetResponse() as HttpWebResponse;
            responseStream = response.GetResponseStream();
            reader = new System.IO.StreamReader(responseStream, Encoding.UTF8);
            srcString = reader.ReadToEnd();
            Response.Write(srcString);
            Response.End();
        }
    }
}
