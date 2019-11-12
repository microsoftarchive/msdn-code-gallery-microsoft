'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports Microsoft.VisualBasic
Imports System
Imports System.ComponentModel.Design
Imports System.Diagnostics
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports System.Text
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.Shell.Interop

Namespace Microsoft.Samples.VisualStudio.MenuCommands
    ''' <summary>
    ''' This is the class that implements the package. This is the class that Visual Studio will create
    ''' when one of the commands will be selected by the user, and so it can be considered the main
    ''' entry point for the integration with the IDE.
    ''' Notice that this implementation derives from Microsoft.VisualStudio.Shell.Package that is the
    ''' basic implementation of a package provided by the Managed Package Framework (MPF).
    ''' </summary>
    <PackageRegistration(UseManagedResourcesOnly:=True), _
    InstalledProductRegistration("#110", "#112", "1.0", IconResourceID:=400), _
    ProvideMenuResource("Menus.ctmenu", 1), _
    Guid(GuidsList.guidMenuAndCommandsPkg), ComVisible(True)> _
    Public NotInheritable Class MenuCommandsPackage
        Inherits Package
#Region "Member Variables"
        Private dynamicVisibilityCommand1 As OleMenuCommand
        Private dynamicVisibilityCommand2 As OleMenuCommand
#End Region

        ''' <summary>
        ''' Default constructor of the package. This is the constructor that will be used by VS
        ''' to create an instance of your package. Inside the constructor you should do only the
        ''' more basic initializazion like setting the initial value for some member variable. But
        ''' you should never try to use any VS service because this object is not part of VS
        ''' environment yet; you should wait and perform this kind of initialization inside the
        ''' Initialize method.
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Initialization of the package; this is the place where you can put all the initialization
        ''' code that relies on services provided by Visual Studio.
        ''' </summary>
        Protected Overrides Sub Initialize()
            MyBase.Initialize()

            ' Now get the OleCommandService object provided by the MPF; this object is the one
            ' responsible for handling the collection of commands implemented by the package.
            Dim mcs As OleMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), OleMenuCommandService)
            If Not Nothing Is mcs Then
                ' Now create one object derived from MenuCommnad for each command defined in
                ' the VSCT file and add it to the command service.

                ' For each command we have to define its id that is a unique Guid/integer pair.
                Dim id As New CommandID(New Guid(GuidsList.guidMenuAndCommandsCmdSet), PkgCmdIDList.cmdidMyCommand)
                ' Now create the OleMenuCommand object for this command. The EventHandler object is the
                ' function that will be called when the user will select the command.
                Dim command As New OleMenuCommand(New EventHandler(AddressOf MenuCommandCallback), id)
                ' Add the command to the command service.
                mcs.AddCommand(command)

                ' Create the MenuCommand object for the command placed in the main toolbar.
                id = New CommandID(New Guid(GuidsList.guidMenuAndCommandsCmdSet), PkgCmdIDList.cmdidMyGraph)
                command = New OleMenuCommand(New EventHandler(AddressOf GraphCommandCallback), id)
                mcs.AddCommand(command)

                ' Create the MenuCommand object for the command placed in our toolbar.
                id = New CommandID(New Guid(GuidsList.guidMenuAndCommandsCmdSet), PkgCmdIDList.cmdidMyZoom)
                command = New OleMenuCommand(New EventHandler(AddressOf ZoomCommandCallback), id)
                mcs.AddCommand(command)

                ' Create the DynamicMenuCommand object for the command defined with the TEXTCHANGES
                ' flag.
                id = New CommandID(New Guid(GuidsList.guidMenuAndCommandsCmdSet), PkgCmdIDList.cmdidDynamicTxt)
                command = New DynamicTextCommand(id, VSPackage.ResourceManager.GetString("DynamicTextBaseText"))

                mcs.AddCommand(command)

                ' Now create two OleMenuCommand objects for the two commands with dynamic visibility
                id = New CommandID(New Guid(GuidsList.guidMenuAndCommandsCmdSet), PkgCmdIDList.cmdidDynVisibility1)
                dynamicVisibilityCommand1 = New OleMenuCommand(New EventHandler(AddressOf DynamicVisibilityCallback), id)
                mcs.AddCommand(dynamicVisibilityCommand1)

                id = New CommandID(New Guid(GuidsList.guidMenuAndCommandsCmdSet), PkgCmdIDList.cmdidDynVisibility2)
                dynamicVisibilityCommand2 = New OleMenuCommand(New EventHandler(AddressOf DynamicVisibilityCallback), id)

                ' This command is the one that is invisible by default, so we have to set its visble
                ' property to false because the default value of this property for every object derived
                ' from MenuCommand is true.
                dynamicVisibilityCommand2.Visible = False
                mcs.AddCommand(dynamicVisibilityCommand2)
            End If
        End Sub

#Region "Commands Actions"
        ''' <summary>
        ''' This function prints text on the debug ouput and on the generic pane of the 
        ''' Output window.
        ''' </summary>
        ''' <param name="text"></param>
        Private Sub OutputCommandString(ByVal text As String)
            ' Build the string to write on the debugger and Output window.
            Dim outputText As New StringBuilder()
            outputText.Append(" ================================================" & Microsoft.VisualBasic.vbLf)
            outputText.AppendFormat("  MenuAndCommands: {0}" & Microsoft.VisualBasic.Constants.vbLf, text)
            outputText.Append(" ================================================" & Microsoft.VisualBasic.Constants.vbLf + Microsoft.VisualBasic.Constants.vbLf)

            ' Now print the string on the Output window.
            Dim windowPane As IVsOutputWindowPane = TryCast(GetService(GetType(SVsGeneralOutputWindowPane)), IVsOutputWindowPane)
            If Nothing Is windowPane Then
                Debug.WriteLine("Failed to get a reference to the Output window General pane")
                Return
            End If
            If Microsoft.VisualStudio.ErrorHandler.Failed(windowPane.OutputString(outputText.ToString())) Then
                Debug.WriteLine("Failed to write on the Output window")
            End If
        End Sub

        ''' <summary>
        ''' Event handler called when the user selects the Sample command.
        ''' </summary>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Microsoft.Samples.VisualStudio.MenuCommands.MenuCommandsPackage.OutputCommandString(System.String)")> _
        Private Sub MenuCommandCallback(ByVal caller As Object, ByVal args As EventArgs)
            OutputCommandString("Sample Command Callback.")
        End Sub

        ''' <summary>
        ''' Event handler called when the user selects the Graph command.
        ''' </summary>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Microsoft.Samples.VisualStudio.MenuCommands.MenuCommandsPackage.OutputCommandString(System.String)")> _
        Private Sub GraphCommandCallback(ByVal caller As Object, ByVal args As EventArgs)
            OutputCommandString("Graph Command Callback.")
        End Sub

        ''' <summary>
        ''' Event handler called when the user selects the Zoom command.
        ''' </summary>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Microsoft.Samples.VisualStudio.MenuCommands.MenuCommandsPackage.OutputCommandString(System.String)")> _
        Private Sub ZoomCommandCallback(ByVal caller As Object, ByVal args As EventArgs)
            OutputCommandString("Zoom Command Callback.")
        End Sub

        ''' <summary>
        ''' Event handler called when the user selects one of the two menus with
        ''' dynamic visibility.
        ''' </summary>
        Private Sub DynamicVisibilityCallback(ByVal caller As Object, ByVal args As EventArgs)
            ' This callback is supposed to be called only from the two menus with dynamic visibility
            ' defined inside this package, so first we have to verify that the caller is correct.

            ' Check that the type of the caller is the expected one.
            Dim command As OleMenuCommand = TryCast(caller, OleMenuCommand)
            If Nothing Is command Then
                Return
            End If

            ' Now check the command set.
            If command.CommandID.Guid <> New Guid(GuidsList.guidMenuAndCommandsCmdSet) Then
                Return
            End If

            ' This is one of our commands. Now what we want to do is to switch the visibility status
            ' of the two menus with dynamic visibility, so that if the user clicks on one, then this 
            ' will make it invisible and the other one visible.
            If command.CommandID.ID = PkgCmdIDList.cmdidDynVisibility1 Then
                ' The user clicked on the first one; make it invisible and show the second one.
                dynamicVisibilityCommand1.Visible = False
                dynamicVisibilityCommand2.Visible = True
            ElseIf command.CommandID.ID = PkgCmdIDList.cmdidDynVisibility2 Then
                ' The user clicked on the second one; make it invisible and show the first one.
                dynamicVisibilityCommand2.Visible = False
                dynamicVisibilityCommand1.Visible = True
            End If
        End Sub
#End Region
    End Class
End Namespace
