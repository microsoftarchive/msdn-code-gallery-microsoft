Imports Microsoft.SharePoint.Administration

Namespace Client

    'This class runs on the Web-Front End servers and 
    'connects to the WCF service on the application servers
    'This is why web parts etc. don't need to know where
    'the service application actually runs. Make sure that
    'the SupportedServiceApplication GUID matches the one
    'in the DayNamerServiceApplication class.
    <System.Runtime.InteropServices.Guid("5FB8E4AA-9C30-43B0-93C3-CEFEA5A18110")> _
    <SupportedServiceApplication( _
        "3CB59033-C70F-4699-BCA9-443DE4FF0D97", _
        "1.0.0.0", _
        GetType(DayNamerServiceApplicationProxy))> _
    Public Class DayNamerServiceProxy
        Inherits SPIisWebServiceProxy
        Implements IServiceProxyAdministration

#Region "Constructors"

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal farm As SPFarm)
            MyBase.New(farm)
        End Sub

#End Region

        Public Function CreateProxy(ByVal serviceApplicationProxyType As Type, _
                                    ByVal name As String, _
                                    ByVal serviceApplicationUri As Uri, _
                                    ByVal provisioningContext As SPServiceProvisioningContext) _
                                As SPServiceApplicationProxy _
                                Implements IServiceProxyAdministration.CreateProxy

            If Not serviceApplicationProxyType Is GetType(DayNamerServiceApplicationProxy) Then
                Throw New NotSupportedException()
            End If

            Return New DayNamerServiceApplicationProxy(name, Me, serviceApplicationUri)

        End Function

        Public Function GetProxyTypeDescription(ByVal serviceApplicationProxyType As Type) As SPPersistedTypeDescription _
            Implements IServiceProxyAdministration.GetProxyTypeDescription

            Return New SPPersistedTypeDescription("Day Namer Service Proxy", "Custom service application proxy providing simple day names.")

        End Function

        Public Function GetProxyTypes() As Type() Implements IServiceProxyAdministration.GetProxyTypes
            Dim returnType As Type() = {GetType(DayNamerServiceApplicationProxy)}
            Return returnType
        End Function

        Public Overrides Function GetCreateProxyLink(ByVal serviceApplicationProxyType As System.Type) As SPAdministrationLink _
            Implements IServiceProxyAdministration.GetCreateProxyLink

            Return MyBase.GetCreateProxyLink(serviceApplicationProxyType)
        End Function

    End Class

End Namespace
