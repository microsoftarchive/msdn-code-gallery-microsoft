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
	''' This DynamicWindowPane demonstrate the following features:
	'''	 - Hosting a user control in a tool window
	'''	 - Dynamic visibility control by a UI context
	'''	 - Window events
	''' 
	''' Tool windows are composed of a frame (provided by Visual Studio) and a
	''' pane (provided by the package implementer). The frame implements
	''' IVsWindowFrame while the pane implements IVsWindowPane.
	''' 
	''' DynamicWindowPane inherits the IVsWindowPane implementation from its
	''' base class (ToolWindowPane). DynamicWindowPane will host a .NET
	''' UserControl (DynamicWindowControl). The Package base class will
	''' get the user control by asking for the Window property on this class.
	''' </summary>
	<Guid("C0385ACB-E7FC-46ab-ADE5-5A9984D10733")> _
	Friend Class DynamicWindowPane
		Inherits MsVsShell.ToolWindowPane
        ' Control that will be hosted in the tool window.
        Private control As DynamicWindowWPFControl = Nothing
        ' Caching our output window pane.
		Private outputWindowPane_Renamed As IVsOutputWindowPane = Nothing

		''' <summary>
		''' Constructor for ToolWindowPane.
		''' Initialization that depends on the package or that requires access
		''' to VS services should be done in OnToolWindowCreated.
		''' </summary>
		Public Sub New()
			MyBase.New(Nothing)

			' Set the image that will appear on the tab of the window frame
			' when docked with another window.
			' The resource ID corresponds to the one defined in Resources.resx,
			' while the Index is the offset in the bitmap strip. Each image in
			' the strip is 16x16.
			Me.BitmapResourceID = 301
			Me.BitmapIndex = 1

            ' Creating the user control that will be displayed in the window.
            control = New DynamicWindowWPFControl()

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
            ' we could do this in the constructor.
            Me.Caption = package.GetResourceString("@110")

            ' Register to the window events.
            Dim windowFrameEventsHandler As New WindowStatus(Me.OutputWindowPane, CType(Me.Frame, IVsWindowFrame))
            ErrorHandler.ThrowOnFailure((CType(Me.Frame, IVsWindowFrame)).SetProperty(CInt(Fix(__VSFPROPID.VSFPROPID_ViewHelper)), CType(windowFrameEventsHandler, IVsWindowFrameNotify3)))
            ' Let our control have access to the window state.
            control.CurrentState = windowFrameEventsHandler
        End Sub

        ''' <summary>
        ''' Retrieve the pane that should be used to output information.
        ''' </summary>
        Private ReadOnly Property OutputWindowPane() As IVsOutputWindowPane
            Get
                If outputWindowPane_Renamed Is Nothing Then
                    ' First make sure the output window is visible.
                    Dim uiShell As IVsUIShell = CType(Me.GetService(GetType(SVsUIShell)), IVsUIShell)
                    ' Get the frame of the output window.
                    Dim outputWindowGuid As Guid = New Guid(GuidsList.guidOutputWindowFrame)
                    Dim outputWindowFrame As IVsWindowFrame = Nothing
                    ErrorHandler.ThrowOnFailure(uiShell.FindToolWindow(CUInt(__VSCREATETOOLWIN.CTW_fForceCreate), outputWindowGuid, outputWindowFrame))
                    ' Show the output window.
                    If outputWindowFrame IsNot Nothing Then
                        outputWindowFrame.Show()
                    End If

                    ' Get the output window service.
                    Dim outputWindow As IVsOutputWindow = CType(Me.GetService(GetType(SVsOutputWindow)), IVsOutputWindow)
                    ' The following GUID is a randomly generated one. This is to uniquely identify our output pane.
                    ' It is best to change it to something else to avoid sharing it with someone else.
                    ' If the goal is to share, then the same guid should be used, and the pane should only
                    ' be created if it does not already exist.
                    Dim paneGuid As New Guid("{E6E69B7B-C898-4b0a-AEAB-C1961BC9E54E}")
                    ' Create the pane.
                    Dim package As PackageToolWindow = CType(Me.Package, PackageToolWindow)
                    Dim paneName As String = package.GetResourceString("@111")
                    ErrorHandler.ThrowOnFailure(outputWindow.CreatePane(paneGuid, paneName, 1, 0))
                    ' Retrieve the pane.
                    ErrorHandler.ThrowOnFailure(outputWindow.GetPane(paneGuid, outputWindowPane_Renamed))
                End If

                Return outputWindowPane_Renamed
            End Get
        End Property
    End Class
End Namespace
