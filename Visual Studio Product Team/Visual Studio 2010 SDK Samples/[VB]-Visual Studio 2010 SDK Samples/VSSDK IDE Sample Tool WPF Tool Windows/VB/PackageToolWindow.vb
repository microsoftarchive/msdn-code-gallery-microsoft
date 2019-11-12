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
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio.Shell.Interop

Imports MsVsShell = Microsoft.VisualStudio.Shell
Imports ErrorHandler = Microsoft.VisualStudio.ErrorHandler

Namespace Microsoft.Samples.VisualStudio.IDE.ToolWindow
	''' <summary>
	''' The Package class is responsible for the following:
	'''		- Attributes to enable registration of the components
	'''		- Enable the creation of our tool windows
	'''		- Respond to our commands
	''' 
	''' The following attributes are covered in other samples:
	'''		PackageRegistration:   Reference.Package
	'''		ProvideMenuResource:   Reference.MenuAndCommands
	''' 
	''' Our initialize method defines the command handlers for the commands that
	''' we provide under View|Other Windows to show our tool windows
	''' 
	''' The first new attribute we are using is ProvideToolWindow. That attribute
	''' is used to advertise that our package provides a tool window. In addition
	''' it can specify optional parameters to describe the default start location
	''' of the tool window. For example, the PersistedWindowPane will start tabbed
	''' with Solution Explorer. The default position is only used the very first
	''' time a tool window with a specific Guid is shown for a user. After that,
	''' the position is persisted based on the last known position of the window.
	''' When trying different default start positions, you may find it useful to
	''' delete *.prf from:
	'''		"%USERPROFILE%\Application Data\Microsoft\VisualStudio\10.0Exp\"
	''' as this is where the positions of the tool windows are persisted.
	''' 
	''' To get the Guid corresponding to the Solution Explorer window, we ran this
	''' sample, made sure the Solution Explorer was visible, selected it in the
	''' Persisted Tool Window and looked at the properties in the Properties
	''' window. You can do the same for any window.
	''' 
	''' The DynamicWindowPane makes use of a different set of optional properties.
	''' First it specifies a default position and size (again note that this only
	''' affects the very first time the window is displayed). Then it specifies the
	''' Transient flag which means it will not be persisted when Visual Studio is
	''' closed and reopened.
	''' 
	''' The second new attribute is ProvideToolWindowVisibility. This attribute
	''' is used to specify that a tool window visibility should be controled
	''' by a UI Context. For a list of predefined UI Context, look in vsshell.idl
	''' and search for "UICONTEXT_". Since we are using the UICONTEXT_SolutionExists,
	''' this means that it is possible to cause the window to be displayed simply by
	''' creating a solution/project.
	''' </summary>
	<MsVsShell.ProvideToolWindow(GetType(PersistedWindowPane), Style := MsVsShell.VsDockStyle.Tabbed, Window := "3ae79031-e1bc-11d0-8f78-00a0c9110057"), MsVsShell.ProvideToolWindow(GetType(DynamicWindowPane), PositionX:=250, PositionY:=250, Width:=160, Height:=180, Transient:=True), MsVsShell.ProvideToolWindowVisibility(GetType(DynamicWindowPane), "f1536ef8-92ec-443c-9ed7-fdadf150da82"), MsVsShell.ProvideMenuResource(1000, 1), MsVsShell.PackageRegistration(UseManagedResourcesOnly := True), Guid(GuidsList.guidClientPkg)> _
	Public Class PackageToolWindow
		Inherits MsVsShell.Package
        ' Cache the Menu Command Service since we will use it multiple times.
		Private menuService As MsVsShell.OleMenuCommandService

		''' <summary>
		''' Package contructor.
		''' While we could have used the default constructor, adding the Trace makes it
		''' possible to verify that the package was created without having to set a break
		''' point in the debugger.
		''' </summary>
		Public Sub New()
			Trace.WriteLine(String.Format(CultureInfo.CurrentCulture, "Entering constructor for class {0}.", Me.GetType().Name))
		End Sub

		''' <summary>
		''' Initialization of the package; this is the place where you can put all the initilaization
		''' code that rely on services provided by VisualStudio.
		''' </summary>
		Protected Overrides Sub Initialize()
			Trace.WriteLine(String.Format(CultureInfo.CurrentCulture, "Entering Initialize for class {0}.", Me.GetType().Name))
			MyBase.Initialize()

			' Create one object derived from MenuCommand for each command defined in
			' the VSCT file and add it to the command service.

			' Each command is uniquely identified by a Guid/integer pair.
            Dim id As New CommandID(New Guid(GuidsList.guidClientCmdSet), PkgCmdId.cmdidPersistedWindow)
            ' Add the handler for the persisted window with selection tracking.
			DefineCommandHandler(New EventHandler(AddressOf Me.ShowPersistedWindow), id)

            ' Add the handler for the tool window with dynamic visibility and events.
			id = New CommandID(New Guid(GuidsList.guidClientCmdSet), PkgCmdId.cmdidUiEventsWindow)
			DefineCommandHandler(New EventHandler(AddressOf Me.ShowDynamicWindow), id)

		End Sub

		''' <summary>
		''' Define a command handler.
		''' When the user press the button corresponding to the CommandID
		''' the EventHandler will be called.
		''' </summary>
		''' <param name="id">The CommandID (Guid/ID pair) as defined in the .vsct file</param>
		''' <param name="handler">Method that should be called to implement the command</param>
		''' <returns>The menu command. This can be used to set parameter such as the default visibility once the package is loaded</returns>
		Friend Function DefineCommandHandler(ByVal handler As EventHandler, ByVal id As CommandID) As MsVsShell.OleMenuCommand
            ' If the package is zombied, we don't want to add commands.
			If Me.Zombied Then
				Return Nothing
			End If

            ' Make sure we have the service.
			If menuService Is Nothing Then
				' Get the OleCommandService object provided by the MPF; this object is the one
				' responsible for handling the collection of commands implemented by the package.
				menuService = TryCast(GetService(GetType(IMenuCommandService)), MsVsShell.OleMenuCommandService)
			End If
			Dim command As MsVsShell.OleMenuCommand = Nothing
            If menuService IsNot Nothing Then
                ' Add the command handler.
                command = New MsVsShell.OleMenuCommand(handler, id)
                menuService.AddCommand(command)
            End If
			Return command
		End Function

		''' <summary>
		''' This method loads a localized string based on the specified resource.
		''' </summary>
		''' <param name="resourceName">Resource to load</param>
		''' <returns>String loaded for the specified resource</returns>
		Friend Function GetResourceString(ByVal resourceName As String) As String
            Dim resourceValue As String = ""
			Dim resourceManager As IVsResourceManager = CType(GetService(GetType(SVsResourceManager)), IVsResourceManager)
			If resourceManager Is Nothing Then
				Throw New InvalidOperationException("Could not get SVsResourceManager service. Make sure the package is Sited before calling this method")
			End If
			Dim packageGuid As Guid = Me.GetType().GUID
			Dim hr As Integer = resourceManager.LoadResourceString(packageGuid, -1, resourceName, resourceValue)
			Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr)
			Return resourceValue
		End Function

		''' <summary>
		''' Event handler for our menu item.
		''' This results in the tool window being shown.
		''' </summary>
		''' <param name="sender"></param>
		''' <param name="arguments"></param>
		Private Sub ShowPersistedWindow(ByVal sender As Object, ByVal arguments As EventArgs)
            ' Get the 1 (index 0) and only instance of our tool window (if it does not already exist it will get created).
			Dim pane As MsVsShell.ToolWindowPane = Me.FindToolWindow(GetType(PersistedWindowPane), 0, True)
			If pane Is Nothing Then
				Throw New COMException(Me.GetResourceString("@101"))
			End If
			Dim frame As IVsWindowFrame = TryCast(pane.Frame, IVsWindowFrame)
			If frame Is Nothing Then
				Throw New COMException(Me.GetResourceString("@102"))
			End If
            ' Bring the tool window to the front and give it focus.
			ErrorHandler.ThrowOnFailure(frame.Show())
		End Sub

		''' <summary>
		''' Event handler for our menu item.
		''' This result in the tool window being shown.
		''' </summary>
		''' <param name="sender"></param>
		''' <param name="arguments"></param>
		Private Sub ShowDynamicWindow(ByVal sender As Object, ByVal arguments As EventArgs)
            ' Get the one (index 0) and only instance of our tool window (if it does not already exist it will get created).
			Dim pane As MsVsShell.ToolWindowPane = Me.FindToolWindow(GetType(DynamicWindowPane), 0, True)
			If pane Is Nothing Then
				Throw New COMException(Me.GetResourceString("@101"))
			End If
			Dim frame As IVsWindowFrame = TryCast(pane.Frame, IVsWindowFrame)
			If frame Is Nothing Then
				Throw New COMException(Me.GetResourceString("@102"))
			End If
            ' Bring the tool window to the front and give it focus.
			ErrorHandler.ThrowOnFailure(frame.Show())
		End Sub
	End Class
End Namespace
