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
Imports Microsoft.Win32
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.Shell.Interop

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider
	''' <summary>
	''' Summary description for SccProviderToolWindow.
	''' </summary>
	<Guid("B0BAC05D-bbbb-42f2-8085-723ca3712763")> _
	Public Class SccProviderToolWindow
		Inherits ToolWindowPane
		Private control As SccProviderToolWindowControl

        Public Sub New()

            MyBase.New(Nothing)
            ' Set the window title.
            Me.Caption = Resources.ResourceManager.GetString("ToolWindowCaption")

            ' Set the CommandID for the window ToolBar.
            Me.ToolBar = New System.ComponentModel.Design.CommandID(GuidList.guidSccProviderCmdSet, CommandId.imnuToolWindowToolbarMenu)

            ' Set the icon for the frame.
            ' Bitmap strip resource ID.
            Me.BitmapResourceID = CommandId.ibmpToolWindowsImages
            ' Index in the bitmap strip.
            Me.BitmapIndex = CommandId.iconSccProviderToolWindow

            control = New SccProviderToolWindowControl()
        End Sub

		Public Overrides ReadOnly Property Window() As IWin32Window
			Get
				Return CType(control, IWin32Window)
			End Get
		End Property

		''' <include file='doc\WindowPane.uex' path='docs/doc[@for="WindowPane.Dispose1"]' />
		''' <devdoc>
		'''     Called when this tool window pane is being disposed.
		''' </devdoc>
		Overrides Protected Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
                If control IsNot Nothing Then
                    Try
                        If TypeOf control Is IDisposable Then
                            control.Dispose()
                        End If
                    Catch e As Exception
                        System.Diagnostics.Debug.Fail(String.Format("Failed to dispose {0} controls." & Microsoft.VisualBasic.Constants.vbLf & "{1}", Me.GetType().FullName, e.Message))
                    End Try
                    control = Nothing
                End If

				Dim windowFrame As IVsWindowFrame = CType(Me.Frame, IVsWindowFrame)
                If windowFrame IsNot Nothing Then
                    ' Note: don't check for the return code here.
                    windowFrame.CloseFrame(CUInt(__FRAMECLOSE.FRAMECLOSE_SaveIfDirty))
                End If
			End If
			MyBase.Dispose(disposing)
		End Sub

		''' <summary>
		''' This function is only used to "do something noticeable" when the toolbar button is clicked.
		''' It is called from the package.
		''' A typical tool window may not need this function.
		''' 
        ''' The current behavior change the background color of the control.
		''' </summary>
		Public Sub ToolWindowToolbarCommand()
			If Me.control.BackColor = Color.Coral Then
				Me.control.BackColor = Color.White
			Else
				Me.control.BackColor = Color.Coral
			End If
		End Sub
	End Class
End Namespace
