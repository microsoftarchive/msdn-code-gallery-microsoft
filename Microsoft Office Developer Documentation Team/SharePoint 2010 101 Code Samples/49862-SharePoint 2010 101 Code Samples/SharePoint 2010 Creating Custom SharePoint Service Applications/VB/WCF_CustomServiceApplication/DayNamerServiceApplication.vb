Imports System.ServiceModel
Imports Microsoft.SharePoint.Administration
Imports Microsoft.SharePoint.Utilities

Namespace Server

    'The SPIisWebServiceApplication class runs on Application servers
    'and holds references to all the endpoints. It is also persisted
    'in the Config database. Finally it's where you write code that 
    'implements the service application's functionality. Make sure the 
    'GUID below matches the one referred to in your service proxy class
    <ServiceBehavior( _
        InstanceContextMode:=InstanceContextMode.PerSession, _
        ConcurrencyMode:=ConcurrencyMode.Multiple, _
        IncludeExceptionDetailInFaults:=True)> _
    <System.Runtime.InteropServices.Guid("3CB59033-C70F-4699-BCA9-443DE4FF0D97")> _
    Public Class DayNamerServiceApplication
        Inherits SPIisWebServiceApplication
        Implements IDayNamerContract

        <Persisted()> _
        Private _settings As Integer
        Public Property Setting As Integer
            Get
                Return _settings
            End Get
            Set(ByVal value As Integer)
                _settings = value
            End Set
        End Property

#Region "Constructors"

        Public Sub New()
            MyBase.New()
        End Sub

        Private Sub New(ByVal name As String, ByVal service As DayNamerService, ByVal appPool As SPIisWebServiceApplicationPool)
            MyBase.New(name, service, appPool)
        End Sub

#End Region

        Public Shared Function Create(ByVal name As String, ByVal service As DayNamerService, ByVal appPool As SPIisWebServiceApplicationPool) As DayNamerServiceApplication
            'This method creates the custom service application

            'Validation checks
            If name Is Nothing Then
                Throw New ArgumentNullException("name")
            End If
            If service Is Nothing Then
                Throw New ArgumentNullException("service")
            End If
            If appPool Is Nothing Then
                Throw New ArgumentNullException("appPool")
            End If

            'Create the service application
            Dim serviceApplication As DayNamerServiceApplication = New DayNamerServiceApplication(name, service, appPool)
            serviceApplication.Update()

            'Register the supported endpoints
            serviceApplication.AddServiceEndpoint("http", SPIisWebServiceBindingType.Http)
            serviceApplication.AddServiceEndpoint("https", SPIisWebServiceBindingType.Https, "secure")

            Return serviceApplication

        End Function

#Region "Service Application Properties"

        Protected Overrides ReadOnly Property DefaultEndpointName As String
            Get
                Return "http"
            End Get
        End Property

        Public Overrides ReadOnly Property TypeName As String
            Get
                Return "Day Namer Service Application"
            End Get
        End Property

        Protected Overrides ReadOnly Property InstallPath As String
            Get
                Return SPUtility.GetGenericSetupPath("WebServices\DayNamer")
            End Get
        End Property

        Protected Overrides ReadOnly Property VirtualPath As String
            Get
                Return "DayNamer.svc"
            End Get
        End Property

        Public Overrides ReadOnly Property ApplicationClassId As System.Guid
            Get
                'This GUID matches the one for the class above
                Return New Guid("3CB59033-C70F-4699-BCA9-443DE4FF0D97")
            End Get
        End Property

        Public Overrides ReadOnly Property ApplicationVersion As System.Version
            Get
                Return New Version("1.0.0.0")
            End Get
        End Property

#End Region

#Region "Service Application Admin Pages"

        'These two properties point to the Manage.aspx page in the ADMIN folder
        'In fact that page is blank for this service application, but these properties are required.
        Public Overrides ReadOnly Property ManageLink As Microsoft.SharePoint.Administration.SPAdministrationLink
            Get
                Return New SPAdministrationLink("/_admin/DayNamerService/Manage.aspx")
            End Get
        End Property

        Public Overrides ReadOnly Property PropertiesLink As Microsoft.SharePoint.Administration.SPAdministrationLink
            Get
                Return New SPAdministrationLink("/_admin/DayNamerService/Manage.aspx")
            End Get
        End Property

#End Region

#Region "IDayNamerContract Implementation"

        'This is where the service application actually does something.
        'In this case, it's very simple by way of demonstration

        'This simple method returns today's name
        Public Function TodayAdd(ByVal daysToAdd As Integer) As String Implements IDayNamerContract.TodayAdd
            'Add the requested number to today
            Dim requestedDateTime As DateTime = DateTime.Today.AddDays(daysToAdd)
            Dim requestedDay As DayOfWeek = requestedDateTime.DayOfWeek
            'Return the day's name
            Return requestedDay.ToString()
        End Function


        Public Function TodayIs() As String Implements IDayNamerContract.TodayIs
            'Find out what today is
            Dim today As DayOfWeek = DateTime.Today.DayOfWeek
            'Return today's name
            Return today.ToString()
        End Function


#End Region

    End Class

End Namespace