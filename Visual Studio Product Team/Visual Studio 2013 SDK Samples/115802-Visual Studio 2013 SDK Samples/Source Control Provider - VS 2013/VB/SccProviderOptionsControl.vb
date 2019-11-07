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
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.Collections
Imports System.ComponentModel
Imports System.Data
Imports System.Windows.Forms
Imports System.Drawing
Imports Microsoft.VisualStudio.Shell.Interop

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider
	''' <summary>
	''' Summary description for SccProviderOptionsControl.
	''' </summary>
	Public Class SccProviderOptionsControl
		Inherits System.Windows.Forms.UserControl
		Private label1 As Label

		''' <summary> 
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.Container = Nothing
        ' The parent page, use to persist data.
		Private _customPage As SccProviderOptions

		Public Sub New()
			' This call is required by the Windows.Forms Form Designer.
			InitializeComponent()

            ' TODO: Add any initialization after the InitializeComponent call.

		End Sub

		''' <summary> 
		''' Clean up any resources being used.
		''' </summary>
		Protected Overrides Overloads Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
                If components IsNot Nothing Then
                    components.Dispose()
                End If
				GC.SuppressFinalize(Me)
			End If
			MyBase.Dispose(disposing)
		End Sub

		#Region "Component Designer generated code"
		''' <summary> 
		''' Required method for Designer support - do not modify 
		''' the contents of this method with the code editor.
		''' </summary>
        Private Sub InitializeComponent()
            Me.label1 = New System.Windows.Forms.Label()
            Me.SuspendLayout()
                ' 
                ' label1
                ' 
            Me.label1.AutoSize = True
            Me.label1.Location = New System.Drawing.Point(13, 28)
            Me.label1.Name = "label1"
            Me.label1.Size = New System.Drawing.Size(217, 13)
            Me.label1.TabIndex = 2
            Me.label1.Text = "Sample source control provider options page"
                ' 
                ' SccProviderOptionsControl
                ' 
            Me.AllowDrop = True
            Me.Controls.Add(Me.label1)
            Me.Name = "SccProviderOptionsControl"
            Me.Size = New System.Drawing.Size(292, 195)
            Me.ResumeLayout(False)
            Me.PerformLayout()
        End Sub
		#End Region

		Public WriteOnly Property OptionsPage() As SccProviderOptions
			Set(ByVal value As SccProviderOptions)
				_customPage = value
			End Set
		End Property
	End Class

End Namespace
