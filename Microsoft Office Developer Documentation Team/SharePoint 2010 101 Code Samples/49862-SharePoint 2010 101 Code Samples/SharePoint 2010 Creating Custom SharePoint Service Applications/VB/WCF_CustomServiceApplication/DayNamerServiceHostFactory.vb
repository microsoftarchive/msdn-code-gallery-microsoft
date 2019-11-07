Imports System.ServiceModel.Activation
Imports System.ServiceModel
Imports Microsoft.SharePoint


Namespace Server
    'This class enables SharePoint to create multiple service
    'hosts for the service application
    Friend NotInheritable Class DayNamerServiceHostFactory
        Inherits ServiceHostFactory

        Public Overrides Function CreateServiceHost(ByVal constructorString As String, ByVal baseAddresses() As System.Uri) As System.ServiceModel.ServiceHostBase
            Dim serviceHost As ServiceHost = New ServiceHost(GetType(DayNamerServiceApplication), baseAddresses)

            'Configure the service for claims
            serviceHost.Configure(SPServiceAuthenticationMode.Claims)

            Return serviceHost
        End Function

    End Class

End Namespace
