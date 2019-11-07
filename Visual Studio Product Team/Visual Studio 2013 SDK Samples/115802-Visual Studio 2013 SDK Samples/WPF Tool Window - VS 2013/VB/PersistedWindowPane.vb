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
Imports System.Diagnostics
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Text
Imports System.Windows.Forms
Imports System.Runtime.InteropServices
Imports System.ComponentModel.Design
Imports Microsoft.VisualStudio.Shell.Interop

Imports MsVsShell = Microsoft.VisualStudio.Shell
Imports VsConstants = Microsoft.VisualStudio.VSConstants
Imports ErrorHandler = Microsoft.VisualStudio.ErrorHandler

Namespace Microsoft.Samples.VisualStudio.IDE.ToolWindow
	''' <summary>
	''' This PersistedWindowPane demonstrates the following features:
	'''	 - Hosting a user control in a tool window
	'''	 - Persistence (visible when VS starts based on state when VS closed)
	'''	 - Tool window Toolbar
	'''	 - Selection tracking (content of the Properties window is based on 
	'''	   the selection in that window)
	''' 
	''' Tool windows are composed of a frame (provided by Visual Studio) and a
	''' pane (provided by the package implementer). The frame implements
	''' IVsWindowFrame while the pane implements IVsWindowPane.
	''' 
	''' PersistedWindowPane inherits the IVsWindowPane implementation from its
	''' base class (ToolWindowPane). PersistedWindowPane will host a .NET
	''' UserControl (PersistedWindowControl). The Package base class will
	''' get the user control by asking for the Window property on this class.
	''' </summary>
	<Guid("0577E97C-00F3-486d-BBDA-69CF53FC92BE")> _
	Friend Class PersistedWindowPane
		Inherits MsVsShell.ToolWindowPane
		' Control that will be hosted in the tool window
        Private control As PersistedWindowWPFControl = Nothing

		''' <summary>
		''' Constructor for ToolWindowPane.
		''' Initialization that depends on the package or that requires access
		''' to VS services should be done in OnToolWindowCreated.
		''' </summary>
		Public Sub New()
            MyBase.New(Nothing)

			' Set the image that will appear on the tab of the window frame
			' when docked with another window.
			' The resource ID corresponds to the one defined in Resources.resx
			' while the Index is the offset in the bitmap strip. Each image in
			' the strip is 16x16.
			Me.BitmapResourceID = 301
			Me.BitmapIndex = 3

			' Add the toolbar by specifying the Guid/MenuID pair corresponding to
			' the toolbar definition in the vsct file.
			Me.ToolBar = New CommandID(New Guid(GuidsList.guidClientCmdSet), PkgCmdId.IDM_MyToolbar)
            ' Specify that we want the toolbar at the top of the window.
			Me.ToolBarLocation = CInt(Fix(VSTWT_LOCATION.VSTWT_TOP))

            ' Creating the user control that will be displayed in the window.
            control = New PersistedWindowWPFControl()

            Me.Content = control

		End Sub

		''' <summary>
		''' This is called after our control has been created and sited.
		''' This is a good place to initialize the control with data gathered
		''' from Visual Studio services.
		''' </summary>
		Public Overrides Sub OnToolWindowCreated()
			MyBase.OnToolWindowCreated()

			Dim package As PackageToolWindow = CType(Me.Package, PackageToolWindow)

			' Set the text that will appear in the title bar of the tool window.
			' Note that because we need access to the package for localization,
			' we have to wait to do this here. If we used a constant string,
			' we could do this in the consturctor.
			Me.Caption = package.GetResourceString("@100")

            ' Add the handler for our toolbar button.
            Dim id As New CommandID(New Guid(GuidsList.guidClientCmdSet), PkgCmdId.cmdidRefreshWindowsList)
			Dim command As MsVsShell.OleMenuCommand = DefineCommandHandler(New EventHandler(AddressOf Me.RefreshList), id)

			' Get the selection tracking service and pass it to the control so that it can push the
			' active selection. Only needed if you want to display something in the Properties window.
            ' Note that this service is only available for windows (not in the global service provider).
            ' Additionally, each window has its own (so you should not be sharing one between multiple windows).
			control.TrackSelection = CType(Me.GetService(GetType(STrackSelection)), ITrackSelection)

            ' Ensure the control's handle has been created; otherwise, BeginInvoke cannot be called.
            ' Note that during runtime this should have no effect when running inside Visual Studio,
            ' as the control's handle should already be created, but unit tests can end up calling
            ' this method without the control being created.
            control.InitializeComponent()


            ' Delay initialization of the list until other tool windows have also had a chance to be
            ' initialized
            control.Dispatcher.BeginInvoke(New Action(AddressOf control.RefreshData))
		End Sub


		Public Overrides Sub OnToolBarAdded()
			MyBase.OnToolBarAdded()

			' In general it is not useful to override this method,
			' but it is useful when the tool window hosts a toolbar
			' with a drop-down (combo box) that needs to be initialized.
			' If that were the case, the initalization would happen here.
		End Sub

		''' <summary>
		''' This method is called to refresh the list of items.
        ''' This is the handler for the Refresh button on the toolbar.
		''' </summary>
		''' <param name="sender"></param>
		''' <param name="arguments"></param>
		Private Sub RefreshList(ByVal sender As Object, ByVal arguments As EventArgs)
			' Update the content of the control
			control.RefreshData()
		End Sub

		''' <summary>
		''' Define a command handler.
		''' When the user presses the button corresponding to the CommandID,
		''' then the EventHandler will be called.
		''' </summary>
		''' <param name="id">The CommandID (Guid/ID pair) as defined in the .vsct file</param>
		''' <param name="handler">Method that should be called to implement the command</param>
		''' <returns>The menu command. This can be used to set parameter such as the default visibility once the package is loaded</returns>
		Private Function DefineCommandHandler(ByVal handler As EventHandler, ByVal id As CommandID) As MsVsShell.OleMenuCommand
			' First add it to the package. This is to keep the visibility
			' of the command on the toolbar constant when the tool window does
			' not have focus. In addition, it creates the command object for us.
			Dim package As PackageToolWindow = CType(Me.Package, PackageToolWindow)
			Dim command As MsVsShell.OleMenuCommand = package.DefineCommandHandler(handler, id)
            ' Verify that the command was added.
			If command Is Nothing Then
				Return command
			End If

			' Get the OleCommandService object provided by the base window pane class; this object is the one
			' responsible for handling the collection of commands implemented by the package.
			Dim menuService As MsVsShell.OleMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), MsVsShell.OleMenuCommandService)

            If menuService IsNot Nothing Then
                ' Add the command handler.
                menuService.AddCommand(command)
            End If
			Return command
		End Function
	End Class
End Namespace
