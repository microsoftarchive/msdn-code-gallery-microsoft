//+++++++++++++++DISCLAIMER+++++++++++++++++++++++++++++
//--------------------------------------------------------------------------------- 
//The sample scripts are not supported under any Microsoft standard support program or service. The sample scripts are provided AS IS without warranty  
//of any kind. Microsoft further disclaims all implied warranties including, without limitation, any implied warranties of merchantability or of fitness for 
//a particular purpose. The entire risk arising out of the use or performance of the sample scripts and documentation remains with you. In no event shall 
//Microsoft, its authors, or anyone else involved in the creation, production, or delivery of the scripts be liable for any damages whatsoever (including, 
//without limitation, damages for loss of business profits, business interruption, loss of business information, or other pecuniary loss) arising out of the use 
//of or inability to use the sample scripts or documentation, even if Microsoft has been advised of the possibility of such damages 
//---------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace CSharpSystemNetSSLValidation
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Initializes a new WebRequest instance for the specified URI 

            var http = (HttpWebRequest)WebRequest.Create("https://www.azure.com");
            var response = http.GetResponse();

            //Gets the stream that is used to read the body of the response from the server.

            var stream = response.GetResponseStream();
            var sr = new StreamReader(stream);
            var content = sr.ReadToEnd();
            Response.Write(content);
        }

        //Verifies the remote Secure Sockets Layer (SSL) certificate used for authentication.

        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}