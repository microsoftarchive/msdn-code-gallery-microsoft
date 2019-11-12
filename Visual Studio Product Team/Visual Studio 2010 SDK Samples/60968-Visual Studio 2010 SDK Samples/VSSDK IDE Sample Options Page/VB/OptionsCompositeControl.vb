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
Imports System.IO
Imports System.Drawing
Imports System.Windows.Forms

Namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage
	''' <summary>
	''' This class implements UI for the Custom options page.
	''' It uses OptionsPageCustom object as a data objects.
	''' </summary>
	Public Class OptionsCompositeControl
		Inherits System.Windows.Forms.UserControl
		#Region "Fields"

		Private pictureBox As PictureBox
		Private openImageFileDialog As OpenFileDialog
		Private WithEvents buttonChooseImage As Button
		Private WithEvents buttonClearImage As Button
		Private customOptionsPage As OptionsPageCustom

		''' <summary> 
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.Container = Nothing

		#End Region

		#Region "Constructors"

		''' <summary>
		''' Explicitly defined default constructor.
		''' Initializes new instance of OptionsCompositeControl class.
		''' </summary>
		Public Sub New()
			' This call is required by the Windows.Forms Form Designer.
			InitializeComponent()
		End Sub

		#End Region

		#Region "Methods"

		#Region "IDisposable implementation"
		''' <summary> 
		''' Clean up any resources being used.
		''' </summary>
		Protected Overrides Overloads Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
                If openImageFileDialog IsNot Nothing Then
                    openImageFileDialog.Dispose()
                    openImageFileDialog = Nothing
                End If
                If components IsNot Nothing Then
                    components.Dispose()
                End If
				GC.SuppressFinalize(Me)
			End If
			MyBase.Dispose(disposing)
		End Sub
		#End Region

		#Region "Component Designer generated code"
		''' <summary> 
		''' Required method for Designer support - do not modify 
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
			Me.pictureBox = New System.Windows.Forms.PictureBox()
			Me.openImageFileDialog = New System.Windows.Forms.OpenFileDialog()
			Me.buttonChooseImage = New System.Windows.Forms.Button()
			Me.buttonClearImage = New System.Windows.Forms.Button()
			CType(Me.pictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.SuspendLayout()
			' 
			' pictureBox
			' 
			Me.pictureBox.Location = New System.Drawing.Point(16, 16)
			Me.pictureBox.Name = "pictureBox"
			Me.pictureBox.Size = New System.Drawing.Size(264, 120)
			Me.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
			Me.pictureBox.TabIndex = 0
			Me.pictureBox.TabStop = False
			' 
			' buttonChooseImage
			' 
			Me.buttonChooseImage.Location = New System.Drawing.Point(16, 152)
			Me.buttonChooseImage.Name = "buttonChooseImage"
			Me.buttonChooseImage.Size = New System.Drawing.Size(112, 23)
            Me.buttonChooseImage.TabIndex = 1            
            Me.buttonChooseImage.Text = Resources.ChooseImageButtonText
            AddHandler Me.buttonChooseImage.Click, New System.EventHandler(AddressOf Me.OnChooseImage)
			' 
			' buttonClearImage
			' 
			Me.buttonClearImage.Location = New System.Drawing.Point(160, 152)
			Me.buttonClearImage.Name = "buttonClearImage"
			Me.buttonClearImage.Size = New System.Drawing.Size(96, 23)
			Me.buttonClearImage.TabIndex = 2
            Me.buttonClearImage.Text = Resources.ButtonClearImageText
            AddHandler Me.buttonClearImage.Click, New System.EventHandler(AddressOf Me.OnClearImage)
			' 
			' OptionsCompositeControl
			' 
			Me.AllowDrop = True
			Me.Controls.Add(Me.buttonClearImage)
			Me.Controls.Add(Me.buttonChooseImage)
			Me.Controls.Add(Me.pictureBox)
			Me.Name = "OptionsCompositeControl"
			Me.Size = New System.Drawing.Size(292, 195)
			CType(Me.pictureBox, System.ComponentModel.ISupportInitialize).EndInit()
			Me.ResumeLayout(False)

		End Sub
		#End Region
		''' <summary>
		''' Handles the ChooseImage event. 
		''' </summary>
		''' <param name="sender">The reference to contained object.</param>
		''' <param name="e">The event arguments.</param>
        Private Sub OnChooseImage(ByVal sender As Object, ByVal e As System.EventArgs)
            openImageFileDialog = New OpenFileDialog()

            If (openImageFileDialog IsNot Nothing) AndAlso (System.Windows.Forms.DialogResult.OK = openImageFileDialog.ShowDialog()) Then
                If customOptionsPage IsNot Nothing Then
                    customOptionsPage.CustomBitmap = openImageFileDialog.FileName
                End If
                RefreshImage()
            End If
        End Sub
		''' <summary>
		''' Handles the ClearImage event. 
		''' </summary>
		''' <param name="sender">The reference to contained object.</param>
		''' <param name="e">The event arguments.</param>
        Private Sub OnClearImage(ByVal sender As Object, ByVal e As System.EventArgs)
            If customOptionsPage IsNot Nothing Then
                customOptionsPage.CustomBitmap = Nothing
            End If
            RefreshImage()
        End Sub
		''' <summary>
		''' Refresh PictureBox Image data.
		''' </summary>
		''' <remarks>Image was reloaded from the file, specified by CustomBitmap (full path to the file).</remarks>
		Private Sub RefreshImage()
            If customOptionsPage Is Nothing Then
                Return
            End If

			Dim fileName As String = customOptionsPage.CustomBitmap
            If fileName IsNot Nothing AndAlso fileName.Length <> 0 Then
                ' Avoid to use Image.FromFile() method for loading image to exclude file locks.
                Using lStream As New FileStream(fileName, FileMode.Open, FileAccess.Read)
                    pictureBox.Image = Image.FromStream(lStream)
                End Using
            Else
                pictureBox.Image = Nothing
            End If
		End Sub
		''' <summary>
		''' Gets or Sets the reference to the underlying OptionsPage object.
		''' </summary>
		Public Property OptionsPage() As OptionsPageCustom
			Get
				Return customOptionsPage
			End Get
			Set(ByVal value As OptionsPageCustom)
				customOptionsPage = value
				RefreshImage()
			End Set
		End Property
		#End Region
	End Class
End Namespace
