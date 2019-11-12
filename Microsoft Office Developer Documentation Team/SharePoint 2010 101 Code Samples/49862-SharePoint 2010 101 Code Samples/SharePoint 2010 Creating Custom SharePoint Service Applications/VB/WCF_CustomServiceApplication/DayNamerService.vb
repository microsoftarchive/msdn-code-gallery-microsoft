Imports Microsoft.SharePoint.Administration
Imports WCF_CustomServiceApplication.Client

Namespace Server
    'This class creates a link to create a new Day Namer service
    'application in the Manage Service Applications page ribbon
    'in Central Administration. It also creates the service application
    'and its proxy.

    <System.Runtime.InteropServices.Guid("32C35AAD-7B0C-49D9-8626-03186D18CB8E")> _
    Public Class DayNamerService
        Inherits SPIisWebService
        Implements IServiceAdministration

#Region "Constructors"

        Public Sub New()

        End Sub

        Public Sub New(ByVal farm As SPFarm)
            MyBase.New(farm)
        End Sub

#End Region

        'This method creates the service application
        Public Function CreateApplication(ByVal name As String, _
                                          ByVal serviceApplicationType As Type, _
                                          ByVal provisioningContext As SPServiceProvisioningContext) _
                                      As SPServiceApplication _
                                      Implements IServiceAdministration.CreateApplication

            'Validation checks
            If Not serviceApplicationType Is GetType(DayNamerServiceApplication) Then
                Throw New NotSupportedException()
            End If
            If provisioningContext Is Nothing Then
                Throw New ArgumentNullException("provisioningContext")
            End If

            'If the service doesn't already exist, create it
            Dim serviceApp As DayNamerServiceApplication = TryCast(Me.Farm.GetObject(name, Me.Id, serviceApplicationType), DayNamerServiceApplication)
            If serviceApp Is Nothing Then
                serviceApp = DayNamerServiceApplication.Create(name, Me, provisioningContext.IisWebServiceApplicationPool)
            End If

            Return serviceApp

        End Function

        Public Function CreateProxy(ByVal name As String, _
                                    ByVal serviceApplication As SPServiceApplication, _
                                    ByVal provisioningContext As SPServiceProvisioningContext) _
                                As SPServiceApplicationProxy _
                                Implements IServiceAdministration.CreateProxy

            'Validation checks
            If Not serviceApplication.GetType() Is GetType(DayNamerServiceApplication) Then
                Throw New NotSupportedException()
            End If
            If serviceApplication Is Nothing Then
                Throw New ArgumentNullException("serviceApplication")
            End If

            'Verify the service proxy exists
            Dim serviceProxy As DayNamerServiceProxy = DirectCast(Me.Farm.GetObject(name, Me.Farm.Id, GetType(DayNamerServiceProxy)), DayNamerServiceProxy)
            If serviceProxy Is Nothing Then
                Throw New InvalidOperationException("DayNamerServiceProxy does not exist in the farm.")
            End If

            'If the app proxy doesn't exist, create it
            Dim applicationProxy As DayNamerServiceApplicationProxy = serviceProxy.ApplicationProxies.GetValue(Of DayNamerServiceApplicationProxy)(name)
            If applicationProxy Is Nothing Then
                Dim serviceAppAddress As Uri = DirectCast(serviceApplication, DayNamerServiceApplication).Uri
                applicationProxy = New DayNamerServiceApplicationProxy(name, serviceProxy, serviceAppAddress)
            End If

            Return applicationProxy

        End Function

        'This method returns a description of the service application
        Public Function GetApplicationTypeDescription(ByVal serviceApplicationType As Type) _
            As SPPersistedTypeDescription _
            Implements IServiceAdministration.GetApplicationTypeDescription

            If Not serviceApplicationType Is GetType(DayNamerServiceApplication) Then
                Throw New NotSupportedException()
            End If
            Return New SPPersistedTypeDescription("Day Namer Service", "Custom service application providing simple date lookups.")
        End Function

        Public Function GetApplicationTypes() _
            As Type() _
            Implements IServiceAdministration.GetApplicationTypes

            Dim returnType As Type() = {GetType(DayNamerServiceApplication)}
            Return returnType

        End Function

        Public Overrides Function GetCreateApplicationLink(ByVal serviceApplicationType As Type) _
            As SPAdministrationLink _
            Implements IServiceAdministration.GetCreateApplicationLink

            'Make sure this links to your own Create.aspx page. 
            Return New SPAdministrationLink("/_admin/DayNamerService/Create.aspx")

        End Function

        Public Overrides Function GetCreateApplicationOptions(ByVal serviceApplicationType As System.Type) _
            As SPCreateApplicationOptions _
            Implements IServiceAdministration.GetCreateApplicationOptions

            Return MyBase.GetCreateApplicationOptions(serviceApplicationType)

        End Function

    End Class

End Namespace
