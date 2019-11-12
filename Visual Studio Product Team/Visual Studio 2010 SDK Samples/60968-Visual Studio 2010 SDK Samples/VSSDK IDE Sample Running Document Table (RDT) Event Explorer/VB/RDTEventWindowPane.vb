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
Imports System.Collections
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Drawing
Imports System.Data
Imports System.Windows.Forms
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.Shell

Namespace MyCompany.RdtEventExplorer
	''' <summary>
	''' This class implements the tool window exposed by this package and hosts a user control.
	'''
	''' In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
	''' usually implemented by the package implementer.
	'''
	''' This class derives from the ToolWindowPane class provided from the MPF in order to use its 
	''' implementation of the IVsWindowPane interface.
	''' </summary>
	<Guid("4982E2D7-FCB1-4681-A744-3A7DF55A1B4D")> _
	Public Class RdtEventWindowPane
		Inherits ToolWindowPane
		' This is the user control hosted by the tool window; it is exposed to the base class 
		' using the Window property. Note that, even if this class implements IDispose, we are
		' not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
		' the object returned by the Window property.
		Private control As RdtEventControl

		''' <summary>
		''' Standard constructor for the tool window.
		''' </summary>
		Public Sub New()
			MyBase.New(Nothing)
			' Set the window title reading it from the resources.
			Me.Caption = Resources.ToolWindowTitle
			' Set the image that will appear on the tab of the window frame
            ' when docked with an other window.
			' The resource ID correspond to the one defined in the resx file
			' while the Index is the offset in the bitmap strip. Each image in
			' the strip being 16x16.
			Me.BitmapResourceID = 301
			Me.BitmapIndex = 1

			' Add the toolbar by specifying the Guid/MenuID pair corresponding to
			' the toolbar definition in the vsct file.
			Me.ToolBar = New CommandID(GuidsList.guidRdtEventExplorerCmdSet, PkgCmdIDList.IDM_MyToolbar)
            ' Specify that we want the toolbar at the top of the window.
			Me.ToolBarLocation = CInt(Fix(VSTWT_LOCATION.VSTWT_TOP))
			control = New RdtEventControl()
		End Sub
		''' <summary>
		''' This is called after our control has been created and sited.
		''' This is a good place to initialize the control with data gathered
		''' from Visual Studio services.
		''' </summary>
		Public Overrides Sub OnToolWindowCreated()
			MyBase.OnToolWindowCreated()

            ' Add the handler for our toolbar button.
            Dim id As New CommandID(GuidsList.guidRdtEventExplorerCmdSet, PkgCmdIDList.cmdidRefreshWindowsList)
			DefineCommandHandler(New EventHandler(AddressOf Me.RefreshGrid), id)

			id = New CommandID(GuidsList.guidRdtEventExplorerCmdSet, PkgCmdIDList.cmdidClearWindowsList)
			DefineCommandHandler(New EventHandler(AddressOf Me.ClearGrid), id)
			'            OleMenuCommand command = DefineCommandHandler(new EventHandler(this.RefreshGrid), id);

			' Get the selection tracking service and pass it to the control so that it can push the
			' active selection. Only needed if you want to display something in the Properties window.
            ' Note that this service is only available for windows (not in the global service provider).
            ' Additionally, each window has its own (so you should not be sharing one between multiple windows).
			control.TrackSelection = CType(Me.GetService(GetType(STrackSelection)), ITrackSelection)
		End Sub
		''' <summary>
		''' This property returns the handle to the user control that should
		''' be hosted in the Tool Window.
		''' </summary>
		Public Overrides ReadOnly Property Window() As IWin32Window
			Get
				Return CType(control, IWin32Window)
			End Get
		End Property
		''' <summary>
		''' This method is called to refresh the list of items.
        ''' This is the handler for the Refresh button on the toolbar.
		''' </summary>
		''' <param name="sender"></param>
		''' <param name="arguments"></param>
		Private Sub RefreshGrid(ByVal sender As Object, ByVal arguments As EventArgs)
			' Update the content of the control
			control.RefreshGrid()
		End Sub
		''' <summary>
		''' This method is called to clear the list of items.
        ''' This is the handler for the Clear button on the toolbar.
		''' </summary>
		''' <param name="sender"></param>
		''' <param name="arguments"></param>
		Private Sub ClearGrid(ByVal sender As Object, ByVal arguments As EventArgs)
			' Update the content of the control
			control.ClearGrid()
		End Sub

		''' <summary>
		''' Define a command handler.
		''' When the user presses the button corresponding to the CommandID,
		''' then the EventHandler will be called.
		''' </summary>
		''' <param name="id">The CommandID (Guid/ID pair) as defined in the .vsct file</param>
		''' <param name="handler">Method that should be called to implement the command</param>
		''' <returns>The menu command. This can be used to set parameter such as the default visibility once the package is loaded</returns>
		Private Function DefineCommandHandler(ByVal handler As EventHandler, ByVal id As CommandID) As OleMenuCommand
			' First add it to the package. This is to keep the visibility
			' of the command on the toolbar constant when the tool window does
			' not have focus. In addition, it creates the command object for us.
			Dim package As RdtEventExplorerPkg = CType(Me.Package, RdtEventExplorerPkg)
			Dim command As OleMenuCommand = package.DefineCommandHandler(handler, id)
            ' Verify that the command was added.
			If command Is Nothing Then
				Return command
			End If

			' Get the OleCommandService object provided by the base window pane class; this object is the one
			' responsible for handling the collection of commands implemented by the package.
			Dim menuService As OleMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), OleMenuCommandService)

			If Not Nothing Is menuService Then
                ' Add the command handler.
				menuService.AddCommand(command)
			End If
			Return command
		End Function
	End Class
End Namespace
