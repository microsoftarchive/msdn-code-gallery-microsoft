'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************


Imports Microsoft.VisualBasic
Imports System
Imports System.ComponentModel.Design
Imports System.Diagnostics

Imports Microsoft.VisualStudio.Shell
Imports Microsoft.Samples.VisualStudio.Services.Interfaces

Namespace Microsoft.Samples.VisualStudio.Services
	''' <summary>
	''' This is the second package created by this sample and is the client of the services exposed 
	''' by the ServicesPackage package.
	''' It will define three menu entries under the Tools menu and each command will try to use
	''' a different service.
	''' </summary>
    <PackageRegistration(UseManagedResourcesOnly:=True), _
        InstalledProductRegistration("#110", "#111", "1.0", IconResourceID:=400), _
        ProvideMenuResource("Menus.ctmenu", 1), _
        System.Runtime.InteropServices.Guid(GuidsList.guidClientPkg)> _
 Public Class ClientPackage
        Inherits Package
        ''' <summary>
        ''' This method is called by the base class during SetSite. At this point the service provider
        ''' for the package is set and all the services are available.
        ''' </summary>
        Protected Overloads Overrides Sub Initialize()
            ' Call the base implementation to finish the initialization of the package.
            MyBase.Initialize()

            ' Get the OleMenuCommandService object to add the MenuCommand that will handle the command
            ' defined by this package.
            Dim mcs As OleMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), OleMenuCommandService)
            If mcs Is Nothing Then
                ' If for some reason we can not get the OleMenuCommandService, then we can not add the handler
                ' for the command, so we can exit now.
                Debug.WriteLine("Can not get the OleMenuCommandService from the base class.")
                Return
            End If

            ' Define the command and add it to the command service.
            Dim id As New CommandID(New Guid(GuidsList.guidClientCmdSet), ClientPkgCmdIDList.cmdidClientGetGlobalService)
            Dim command As New MenuCommand(New EventHandler(AddressOf GetGlobalServiceCallback), id)
            mcs.AddCommand(command)

            ' Add the command that will try to get the local server and that is expected to fail.
            id = New CommandID(New Guid(GuidsList.guidClientCmdSet), ClientPkgCmdIDList.cmdidClientGetLocalService)
            command = New MenuCommand(New EventHandler(AddressOf GetLocalServiceCallback), id)
            mcs.AddCommand(command)

            ' Add the command that will call the local service using the global one.
            id = New CommandID(New Guid(GuidsList.guidClientCmdSet), ClientPkgCmdIDList.cmdidClientGetLocalUsingGlobal)
            command = New MenuCommand(New EventHandler(AddressOf GetLocalUsingGlobalCallback), id)
            mcs.AddCommand(command)
        End Sub

        ''' <summary>
        ''' This function is the event handler for the command defined by this package and is the
        ''' consumer of the service exposed by the ServicesPackage package.
        ''' </summary>
        Private Sub GetGlobalServiceCallback(ByVal sender As Object, ByVal args As EventArgs)
            ' Get the service exposed by the other package. This the expected sequence of queries:
            ' GetService will query the service provider implemented by the base class of this
            ' package for SMyGlobalService; this service will be not found (it is not exposed by this
            ' package), so the base class will forward the request to the service provider used during 
            ' SetSite; this is the global service provider and it will find the service because
            ' ServicesPackage has proffered it using the proffer service.
            Dim service As IMyGlobalService = TryCast(GetService(GetType(SMyGlobalService)), IMyGlobalService)
            If service Is Nothing Then
                ' If the service is not available we can exit now.
                Debug.WriteLine("Can not get the global service.")
                Return
            End If
            ' Call the function exposed by the global service. This function will write a message
            ' on the output window and on the debug output so that it will be possible to verify
            ' that it executed.
            service.GlobalServiceFunction()
        End Sub

        ''' <summary>
        ''' This is the function that will try to get the local service exposed by the Services
        ''' package. This function is expected to fail because this package has no access to the
        ''' service provider implemented by the other package.
        ''' </summary>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Microsoft.Samples.VisualStudio.Services.HelperFunctions.WriteOnOutputWindow(System.IServiceProvider,System.String)")> _
        Private Sub GetLocalServiceCallback(ByVal sender As Object, ByVal args As EventArgs)
            ' Try to get the service. Notice that GetService will use the service provider
            ' implemented by the base class and, in case of service not found, it will query
            ' the service provider used during SetSite to get the service. Because the
            ' service provider implemented by ServicesPackage is not inside this chain of
            ' providers, this query must fail.
            Dim service As IMyLocalService = TryCast(GetService(GetType(SMyLocalService)), IMyLocalService)
            If service IsNot Nothing Then
                ' Something strange happened, write a message on the debug output and exit.
                Debug.WriteLine("GetService for the local service succeeded, but it should fail.")
                Return
            End If

            ' Now write on the output window that the call failed and the test succeeded.
            Dim outputText As String = " ======================================" & Constants.vbLf & Constants.vbTab & "GetLocalServiceCallback test succeeded." & Constants.vbLf & " ======================================" & Constants.vbLf
            ' Write a message on the debug output.
            HelperFunctions.WriteOnOutputWindow(Me, outputText)
        End Sub

        ''' <summary>
        ''' This function will call the method of the global service that will get a reference and
        ''' call a method of the local one.
        ''' </summary>
        Private Sub GetLocalUsingGlobalCallback(ByVal sender As Object, ByVal args As EventArgs)
            ' Get a reference to the global service.
            Dim service As IMyGlobalService = TryCast(GetService(GetType(SMyGlobalService)), IMyGlobalService)
            If service Is Nothing Then
                ' The previous call failed, but we expected it to succeed.
                ' Write a message on the debug output and exit.
                Debug.WriteLine("Can not get the global service.")
                Return
            End If
            ' Now call the method that will cause the call in the local service.
            service.CallLocalService()
        End Sub
    End Class
End Namespace
