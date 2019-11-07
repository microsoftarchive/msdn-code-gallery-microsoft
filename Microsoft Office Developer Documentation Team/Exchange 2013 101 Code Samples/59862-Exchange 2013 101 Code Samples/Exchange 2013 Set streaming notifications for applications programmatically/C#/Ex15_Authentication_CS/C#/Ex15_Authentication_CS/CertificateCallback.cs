using System;
using System.Net;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    public static class CertificateCallback
    {
        static CertificateCallback()
        {
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;
        }

        public static void Initialize()
        {
        }

        // SECURITY NOTE: This certificate callback function is meant for an on-premise
        //                test installation of Exchange that uses the default self-signed
        //                certificate. We included this callback function to make it 
        //                possible to run the Exchange 101 code samples without needing
        //                to install a signed certificate.
        //
        //                In production, the Exchange server should have a valid, signed 
        //                certificate. This code should not be used on a server that has 
        //                a signed certificate, and it should not be used on a production 
        //                server.
        //                
        //                To activate the callback method, remove the code comments as
        //                instructed below.
        private static bool CertificateValidationCallBack(
             object sender,
             System.Security.Cryptography.X509Certificates.X509Certificate certificate,
             System.Security.Cryptography.X509Certificates.X509Chain chain,
             System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            /* Delete this block comment to activate the certificate callback function.

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
              if (chain != null && chain.ChainStatus != null)
              {
                foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                {
                  if ((certificate.Subject == certificate.Issuer) &&
                     (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                  {
                    // Self-signed certificates with an untrusted root are valid. 
                    continue;
                  }
                  else
                  {
                    if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                    {
                      // If there are any other errors in the certificate chain, the certificate is invalid,
                      // so the method returns false.
                      return false;
                    }
                  }
                }
              }

              // When processing reaches this line, the only errors in the certificate chain are 
              // untrusted root errors for self-signed certificates. These certificates are valid
              // for default Exchange server installations, so return true.
              return true;
            }
            else
            {
              // In all other cases, return false.
              return false;
            }

            Delete this block comment and the following line to activate the callback function */
            throw new ApplicationException("Invalid server certificate. See CertificateCallback.cs for more information");
        }
    }
}
