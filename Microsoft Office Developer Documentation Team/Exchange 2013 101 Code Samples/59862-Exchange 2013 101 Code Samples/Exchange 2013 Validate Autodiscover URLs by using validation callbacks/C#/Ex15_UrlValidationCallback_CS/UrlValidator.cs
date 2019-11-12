using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exchange101
{
    public class UrlValidator
    {
        // Url validation callback for Exchange Online email account.
        // Before you use this code, make sure that the URL validation
        // is correct for your site.
        public bool ValidateUrl(string redirectionUrl)
        {
            Console.Write(string.Format("   Validating URL: {0}",redirectionUrl));
            bool result = false;

            Uri redirectionUri = new Uri(redirectionUrl);

            // Validate the contents of the redirection URL. 
            // First, make sure that th eredirection URL is using HTTPS to encrypt the authentication credentials. 
            bool isHttps = redirectionUri.Scheme.ToUpper() == "HTTPS"; 

            // Next, make sure that the redirect is going through the Outlook.com redirector.
            bool isCorrectHost = redirectionUri.Host == "autodiscover-s.outlook.com";

            // Finally, make sure that the redirect is to the Autodiscover service.
            bool isCorrectPath = redirectionUri.AbsolutePath == "/autodiscover/autodiscover.xml";

            result = isHttps && isCorrectHost && isCorrectPath;

            if (result)
            {
                Console.WriteLine(" Valid");
            }
            else
            {
                Console.WriteLine(" Invalid");
            }

            return result;
        }
    }
}
