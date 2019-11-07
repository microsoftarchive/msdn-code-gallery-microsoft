using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using System.Net;

namespace Exchange101
{
    static class Ex15_CTS_Service_CS
    {
        static Ex15_CTS_Service_CS()
        {
            CertificateCallback.Initialize();
        }

        // The following is a basic redirection validation callback method. It 
        // inspects the redirection URL and only allows the Service object to 
        // follow the redirection link if the URL is using HTTPS. 
        //
        // This redirection URL validation callback provides sufficient security
        // for development and testing of your application. However, it may not
        // provide sufficient security for your deployed application. You should
        // always make sure that the URL validation callback method that you use
        // meets the security requirements of your organization.
        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            bool result = false;

            Uri redirectionUri = new Uri(redirectionUrl);

            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }

            return result;
        }

        public static ITraceListener TraceListener { get; set; }
        public static ExchangeVersion ServerVersion { get { return ExchangeVersion.Exchange2010_SP2; } }

        // This method connects to EWS services for an email address using the default credentials
        // for the signed-in user. It creates a set of default credentials and passes them to the
        // internal method for creating an ExchangeService object.
        public static ExchangeService ConnectToService(string emailAddress)
        {
            return InternalConnectToService(emailAddress, CredentialCache.DefaultCredentials, string.Empty);
        }

        // This method connects to EWS services impersonating an email address for another account using the 
        // default credentials for the signed-in user. It creates a set of default credentials and passes them to the
        // internal method for creating an ExchangeService object. The user's email address must have
        // impersonation rights on the Exchange server for the impersonated email account.
        public static ExchangeService ConnectToService(string emailAddress, string impersonatedUserEmail)
        {
            return InternalConnectToService(emailAddress, CredentialCache.DefaultCredentials, impersonatedUserEmail);
        }

        // This method connects to EWS services for an email address using credentials entered for 
        // an email address. 
        public static ExchangeService ConnectToService(string emailAddress, ICredentials credentials)
        {
            return InternalConnectToService(emailAddress, credentials, string.Empty);
        }

        // This method connets to EWS services for an email address using credentaials entered by the user
        // and impersonating an second email address. The first email address must have impersonation rights
        // on the Exchage server for the impersonated account.
        public static ExchangeService ConnectToService(string emailAddress, ICredentials credentials, string impersonatedUserEmail)
        {
            return InternalConnectToService(emailAddress, credentials, impersonatedUserEmail);
        }

        // This private method creates a new ExchangeService object using the email addresess and credentails passed from the
        // public methods. 
        private static ExchangeService InternalConnectToService(string emailAddress, ICredentials credentials, string impersonatedUserEmail)
        {
            Console.Write("   Calling Autodiscover service to get Exchange endpoint...");
            ExchangeService service = new ExchangeService(ServerVersion);

            if (TraceListener != null)
            {
                service.TraceListener = TraceListener;
                service.TraceFlags = TraceFlags.All;
                service.TraceEnabled = true;
            }

            service.Credentials = (NetworkCredential)credentials;

            if (!string.IsNullOrEmpty(impersonatedUserEmail))
            {
                ImpersonatedUserId impersonatedUserId =
                  new ImpersonatedUserId(ConnectingIdType.SmtpAddress, impersonatedUserEmail);

                service.ImpersonatedUserId = impersonatedUserId;
            }

            service.AutodiscoverUrl(emailAddress, RedirectionUrlValidationCallback);
            Console.WriteLine("complete.");
            Console.WriteLine(string.Format("  EWS URL: {0}", service.Url));

            return service;
        }
    }
}
