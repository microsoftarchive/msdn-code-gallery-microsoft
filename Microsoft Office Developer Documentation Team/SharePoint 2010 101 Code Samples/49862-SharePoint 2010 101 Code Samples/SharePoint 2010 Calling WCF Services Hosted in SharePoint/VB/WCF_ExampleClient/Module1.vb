'This is the main WCF namespace
Imports System.ServiceModel

Module Module1

    Sub Main()
        'Before you run this WCF Client, ensure that the 
        'WCF_ServiceHostedInSharePoint project has been deployed

        'Before we create the client we need a binding and an endpoint
        Dim binding As BasicHttpBinding = New BasicHttpBinding()
        binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm
        Dim endPoint As EndpointAddress = New EndpointAddress("http://intranet.contoso.com/_vti_bin/AddAnnouncement.svc")
        'Create the WCF Client
        Console.WriteLine("Connecting to the WCF Announcement Adder Service hosted in SharePoint")
        Dim sharePointClient As SharePointAnnouncementAdderClient = New SharePointAnnouncementAdderClient(binding, endPoint)
        'set up impersonation so SharePoint know who the user is
        sharePointClient.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation
        'Get the list entries from the WCF service
        Console.WriteLine("Inserting the item...")
        Console.WriteLine()
        Dim sharepointResults As Boolean = sharePointClient.AddAnnouncement("Test Item", "This item was created by a WCF service hosted in SharePoint")
        If sharepointResults Then
            Console.WriteLine("The announcement was successfully created and saved")
        Else
            Console.WriteLine("The announcement was not successfully added")
        End If
        Console.WriteLine("Press any key to close the client application")
        Console.ReadKey()
    End Sub

End Module
