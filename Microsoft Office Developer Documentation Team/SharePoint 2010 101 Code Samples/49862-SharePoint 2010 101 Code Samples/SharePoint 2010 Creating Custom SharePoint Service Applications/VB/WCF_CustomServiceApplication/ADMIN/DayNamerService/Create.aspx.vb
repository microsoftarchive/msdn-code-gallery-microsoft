Imports System
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Administration
Imports Microsoft.SharePoint.WebControls
Imports WCF_CustomServiceApplication.Server
Imports WCF_CustomServiceApplication.Client

Namespace Admin

    Partial Public Class Create
        Inherits LayoutsPageBase

        'This administration page is displayed in Central
        'Administration when someone creates an new instance
        'of the custom service application

        'page web controls
        Protected ApplicationPoolSelection As IisWebServiceApplicationPoolSection
        'Protected ServiceAppName As InputFormTextBox
        'Protected DefaultServiceApp As InputFormCheckBox

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            MyBase.OnInit(e)

            'get reference to OK button on dialog master page & wire up handler to it's OK button
            Dim masterPage As DialogMaster = DirectCast(Me.Page.Master, DialogMaster)
            AddHandler masterPage.OkButton.Click, AddressOf okToday_Click

        End Sub

        Private Sub okToday_Click(ByVal sender As Object, ByVal e As EventArgs)
            'create the service app
            SetupDayNamerServiceApp()
            'finish call
            SendResponseForPopUI()
        End Sub

        Private Sub SetupDayNamerServiceApp()
            'create a long running op..
            Using op As SPLongOperation = New SPLongOperation(Me)
                op.Begin()
                Try
                    'get reference to the installed service
                    Dim service As DayNamerService = SPFarm.Local.Services.GetValue(Of DayNamerService)()
                    Dim serviceApp As DayNamerServiceApplication = CreateServiceApplication(service)

                    'if the service instance isn't running, start it up
                    StartServiceInstances()

                    'create service app proxy
                    CreateServiceApplicationProxy(serviceApp)
                Catch ex As Exception
                    Throw New SPException("Error creating Day Namer service application.", ex)
                End Try
            End Using

        End Sub

        Private Function CreateServiceApplication(ByVal service As DayNamerService) As DayNamerServiceApplication

            'create service app
            Dim serviceApp As DayNamerServiceApplication = DayNamerServiceApplication.Create( _
                ServiceAppName.Text, _
                service, _
                ApplicationPoolSelection.GetOrCreateApplicationPool())
            serviceApp.Update()

            'start it if it isn't already started
            If serviceApp.Status <> SPObjectStatus.Online Then
                serviceApp.Status = SPObjectStatus.Online
            End If

            'configure service app endpoint
            serviceApp.AddServiceEndpoint(String.Empty, SPIisWebServiceBindingType.Http)
            serviceApp.Update(True)

            'now provision the service app
            serviceApp.Provision()
            Return serviceApp

        End Function

        Private Sub CreateServiceApplicationProxy(ByVal serviceApp As DayNamerServiceApplication)
            'get reference to the installed service proxy
            Dim serviceProxy As DayNamerServiceProxy = SPFarm.Local.ServiceProxies.GetValue(Of DayNamerServiceProxy)()
            'create service app proxy
            Dim serviceAppProxy As DayNamerServiceApplicationProxy = New DayNamerServiceApplicationProxy( _
                ServiceAppName.Text + " Proxy", _
                serviceProxy, _
                serviceApp.Uri)
            serviceAppProxy.Update(True)
            'provision service app proxy
            serviceAppProxy.Provision()
            'start it if it isn't already started
            If serviceAppProxy.Status <> SPObjectStatus.Online Then
                serviceAppProxy.Status = SPObjectStatus.Online
            End If
            serviceAppProxy.Update(True)
            'add the proxy to the default group if selected
            If DefaultServiceApp.Checked Then
                Dim defaultGroup As SPServiceApplicationProxyGroup = SPServiceApplicationProxyGroup.Default
                defaultGroup.Add(serviceAppProxy)
                defaultGroup.Update(True)
            End If
        End Sub

        Private Shared Sub StartServiceInstances()
            'loop through all service instances on the current server and see if the one for
            'this service app is running or not
            For Each serviceInstance As SPServiceInstance In SPServer.Local.ServiceInstances
                Dim dayNamerServiceInstance As DayNamerServiceInstance = TryCast(serviceInstance, DayNamerServiceInstance)
                'If this one isn't running, start it up
                If dayNamerServiceInstance IsNot Nothing Then
                    If dayNamerServiceInstance.Status <> SPObjectStatus.Online Then
                        dayNamerServiceInstance.Status = SPObjectStatus.Online
                    End If
                End If
            Next
        End Sub

        Private ReadOnly Property ApplicationId As Guid
            Get
                If MyBase.Request.QueryString IsNot Nothing Then
                    Dim s As String = MyBase.Request.QueryString("appid")
                    If String.IsNullOrEmpty(s) Then
                        Return Guid.Empty
                    End If
                    Try
                        Return New Guid(s)
                    Catch ex As FormatException
                        Throw New Exception()
                    End Try
                End If
                Return Guid.Empty
            End Get
        End Property

    End Class

End Namespace
