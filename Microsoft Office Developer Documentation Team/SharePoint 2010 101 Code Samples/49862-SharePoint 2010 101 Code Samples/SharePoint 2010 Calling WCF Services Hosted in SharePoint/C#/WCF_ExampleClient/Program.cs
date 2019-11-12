using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//This is the main WCF namespace
using System.ServiceModel;

namespace WCF_ExampleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //Before you run this WCF Client, ensure that the 
            //WCF_ServiceHostedInSharePoint project has been deployed

            //Before we create the client we need a binding and an endpoint
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
            EndpointAddress endPoint = new EndpointAddress("http://intranet.contoso.com/_vti_bin/AddAnnouncement.svc");
            //Create the WCF Client
            Console.WriteLine("Connecting to the WCF Announcement Adder Service hosted in SharePoint");
            SharePointAnnoucementAdderClient sharePointClient = new SharePointAnnoucementAdderClient(binding, endPoint);
            //set up impersonation so SharePoint know who the user is
            sharePointClient.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            //Get the list entries from the WCF service
            Console.WriteLine("Inserting the item...");
            Console.WriteLine();
            bool sharepointResults = sharePointClient.AddAnnouncement("Test Item", "This item was created by a WCF service hosted in SharePoint");
            if (sharepointResults)
            {
                Console.WriteLine("The announcement was successfully created and saved");
            }
            else
            {
                Console.WriteLine("The announcement was not successfully added");
            }
            Console.WriteLine("Press any key to close the client application");
            Console.ReadKey();
        }
    }
}
