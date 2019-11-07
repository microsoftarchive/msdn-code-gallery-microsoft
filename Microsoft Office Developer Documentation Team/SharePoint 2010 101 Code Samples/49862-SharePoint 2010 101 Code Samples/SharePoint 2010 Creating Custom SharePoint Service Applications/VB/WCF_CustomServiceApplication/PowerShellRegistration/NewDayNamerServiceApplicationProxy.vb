Imports Microsoft.SharePoint.PowerShell
Imports System.Management.Automation
Imports Microsoft.SharePoint.Administration
Imports WCF_CustomServiceApplication.Client

Namespace PowerShellRegistration
    'This class registers a new PowerShell cmdlet: New-DayNamerServiceApplicationProxy
    <Cmdlet(VerbsCommon.[New], "DayNamerServiceApplicationProxy", SupportsShouldProcess:=True)> _
    Public Class NewDayNamerServiceApplicationProxy
        Inherits SPCmdlet

        Private _uri As Uri

#Region "cmdlet parameters"
        <Parameter(Mandatory:=True)> _
        <ValidateNotNullOrEmpty()> _
        Public Name As String

        <Parameter(Mandatory:=True, ParameterSetName:="Uri")> _
        <ValidateNotNullOrEmpty()> _
        Public Property Uri As String
            Get
                Return _uri.ToString()
            End Get
            Set(ByVal value As String)
                _uri = New Uri(value)
            End Set
        End Property

        <Parameter(Mandatory:=True, ParameterSetName:="ServiceApplication")> _
        <ValidateNotNullOrEmpty()> _
        Public ServiceApplication As SPServiceApplicationPipeBind
#End Region

        Protected Overrides Function RequireUserFarmAdmin() As Boolean
            Return True
        End Function

        Protected Overrides Sub InternalProcessRecord()
            'Validation checks
            'Ensure can hit farm
            Dim farm As SPFarm = SPFarm.Local
            If farm Is Nothing Then
                ThrowTerminatingError(New InvalidOperationException("SharePoint farm not found."), ErrorCategory.ResourceUnavailable, Me)
                SkipProcessCurrentRecord()
            End If

            'Ensure proxy installed
            Dim serviceProxy As DayNamerServiceProxy = farm.ServiceProxies.GetValue(Of DayNamerServiceProxy)()
            If serviceProxy Is Nothing Then
                ThrowTerminatingError(New InvalidOperationException("Day Namer Service Proxy not found."), ErrorCategory.NotInstalled, Me)
                SkipProcessCurrentRecord()
            End If

            'Ensure can hit service application
            Dim existingServiceAppProxy As DayNamerServiceApplicationProxy = serviceProxy.ApplicationProxies.GetValue(Of DayNamerServiceApplicationProxy)()
            If existingServiceAppProxy IsNot Nothing Then
                ThrowTerminatingError(New InvalidOperationException("Day Namer Service Application Proxy already exists."), ErrorCategory.ResourceExists, Me)
                SkipProcessCurrentRecord()
            End If

            Dim serviceApplicationAddress As Uri = Nothing
            If ParameterSetName = "Uri" Then
                serviceApplicationAddress = _uri
            ElseIf ParameterSetName = "ServiceApplication" Then
                'make sure can get a reference to service app
                Dim serviceApp As SPServiceApplication = ServiceApplication.Read()
                If serviceApp Is Nothing Then
                    WriteError(New InvalidOperationException("Service application not found."), ErrorCategory.ResourceExists, serviceApp)
                    SkipProcessCurrentRecord()
                End If
                'make sure can connect to service app
                Dim sharedServiceApp As ISharedServiceApplication = TryCast(serviceApp, ISharedServiceApplication)
                If sharedServiceApp Is Nothing Then
                    WriteError(New InvalidOperationException("Service application not found."), ErrorCategory.ResourceExists, serviceApp)
                    SkipProcessCurrentRecord()
                End If
                serviceApplicationAddress = sharedServiceApp.Uri
            Else
                ThrowTerminatingError(New InvalidOperationException("Invalid parameter set."), ErrorCategory.InvalidArgument, Me)
            End If

            'create the service app proxy
            If serviceApplicationAddress IsNot Nothing And ShouldProcess(Me.Name) Then
                Dim serviceAppProxy As DayNamerServiceApplicationProxy = _
                    New DayNamerServiceApplicationProxy(Me.Name, _
                        serviceProxy, _
                        serviceApplicationAddress)

                'Provisiong the service app proxy
                serviceAppProxy.Provision()

                'pass service app proxy back to the PowerShell
                WriteObject(serviceAppProxy)
            End If

        End Sub

    End Class

End Namespace

