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
Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel
Imports Microsoft.VisualStudio.Shell
Imports System.Runtime.InteropServices

Namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage
	''' <summary>
	''' Extends a standard dialog functionality for implementing ToolsOptions pages, 
	''' with support for the Visual Studio automation model, Windows Forms, and state 
	''' persistence through the Visual Studio settings mechanism.
	''' </summary>
	<Guid(GuidStrings.GuidPageCustom)> _
	Public Class OptionsPageCustom
		Inherits DialogPage
		#Region "Fields"
		' The path to the image file which must be shown.
		Private selectedImagePath As String = String.Empty
		#End Region ' Fields

		#Region "Properties"
		''' <summary>
		''' Gets the window an instance of DialogPage that it uses as its user interface.
		''' </summary>
		''' <devdoc>
		''' The window this dialog page will use for its UI.
		''' This window handle must be constant, so if you are
		''' returning a Windows Forms control you must make sure
		''' it does not recreate its handle.  If the window object
		''' implements IComponent it will be sited by the 
		''' dialog page so it can get access to global services.
		''' </devdoc>
		<Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
		Protected Overrides ReadOnly Property Window() As IWin32Window
			Get
                Dim optionsControl As New OptionsCompositeControl()
				optionsControl.Location = New Point(0, 0)
				optionsControl.OptionsPage = Me
				Return optionsControl
			End Get
		End Property
		''' <summary>
		''' Gets or sets the path to the image file.
		''' </summary>
		''' <remarks>The property that needs to be persisted.</remarks>
		<DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
		Public Property CustomBitmap() As String
			Get
				Return selectedImagePath
			End Get
			Set(ByVal value As String)
				selectedImagePath = value
			End Set
		End Property
		#End Region ' Properties
	End Class
End Namespace
