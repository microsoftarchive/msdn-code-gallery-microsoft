Imports Microsoft.SharePoint.PowerShell
Imports System.Management.Automation
Imports Microsoft.SharePoint.Administration
Imports WCF_CustomServiceApplication.Server

Namespace PowerShellRegistration
    'This class registers a new PowerShell cmdlet: New_DayNamerServiceApplication
    'You can use this to create the service application, or use Central Administration
    'After you run this cmdlet, be sure to run the New-DayNamerServiceApplicationProxy cmdlet
    <Cmdlet(VerbsCommon.[New], "DayNamerServiceApplication", SupportsShouldProcess:=True)> _
    Public Class NewDayNamerServiceApplication
        Inherits SPCmdlet

#Region "cmdlet parameters"
        <Parameter(Mandatory:=True)> _
        <ValidateNotNullOrEmpty()> _
        Public Name As String

        <Parameter(Mandatory:=True)> _
        <ValidateNotNullOrEmpty()> _
        Public ApplicationPool As SPIisWebServiceApplicationPoolPipeBind
#End Region

        Protected Overrides Function RequireUserFarmAdmin() As Boolean
            Return True
        End Function

        Protected Overrides Sub InternalProcessRecord()

            'Validation checks
            'ensure you can hit farm
            Dim farm As SPFarm = SPFarm.Local
            If farm Is Nothing Then
                ThrowTerminatingError(New InvalidOperationException("SharePoint farm not found."), ErrorCategory.ResourceUnavailable, Me)
                SkipProcessCurrentRecord()
            End If

            'ensure you can hit local server
            Dim server As SPServer = SPServer.Local
            If server Is Nothing Then
                ThrowTerminatingError(New InvalidOperationException("SharePoint local server not found."), ErrorCategory.ResourceUnavailable, Me)
                SkipProcessCurrentRecord()
            End If

            'ensure you can hit service application
            Dim service As DayNamerService = farm.Services.GetValue(Of DayNamerService)()
            If service Is Nothing Then
                ThrowTerminatingError(New InvalidOperationException("Day Namer Service not found."), ErrorCategory.ResourceUnavailable, Me)
                SkipProcessCurrentRecord()
            End If

            'ensure you can hit app pool
            Dim appPool As SPIisWebServiceApplicationPool = Me.ApplicationPool.Read()
            If appPool Is Nothing Then
                ThrowTerminatingError(New InvalidOperationException("Application pool not found."), ErrorCategory.ResourceUnavailable, Me)
                SkipProcessCurrentRecord()
            End If

            'Check a service app doesn't already exist
            Dim existingServiceApp As DayNamerServiceApplication = service.Applications.GetValue(Of DayNamerServiceApplication)()
            If existingServiceApp IsNot Nothing Then
                WriteError(New InvalidOperationException("Day Namer Service Application already exists."), _
                    ErrorCategory.ResourceExists, _
                    existingServiceApp)
                SkipProcessCurrentRecord()
            End If

            'Create & provision the service application
            If ShouldProcess(Me.Name) Then
                Dim serviceApp As DayNamerServiceApplication = DayNamerServiceApplication.Create( _
                    Me.Name, _
                    service, _
                    appPool)

                'Provision the service application
                serviceApp.Provision()

                'Pass the new service application back to PowerShell
                WriteObject(serviceApp)
            End If

        End Sub


    End Class

End Namespace
