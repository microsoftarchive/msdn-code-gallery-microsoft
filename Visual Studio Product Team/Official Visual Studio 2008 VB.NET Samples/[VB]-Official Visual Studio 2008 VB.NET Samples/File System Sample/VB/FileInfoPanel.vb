' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.IO

Public Class FileInfoPanel
    Inherits FileSystemSample.FileSystemInfoBasePanel

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'DescriptionTextBox
        '
        Me.DescriptionTextBox.Text = "The FileInfo class encapsulates attributes related to a file, such as its size, l" & _
            "ocation, and accessed time."
        '
        'FileInfoPanel
        '
        Me.Name = "FileInfoPanel"
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Shared panelInstance As FileInfoPanel
    Friend WithEvents fileChooser As New FileChooser
    Private fInfo As FileInfo

    ''' <summary>
    ''' Get a global instance of this panel
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As FileInfoPanel
        If (panelInstance Is Nothing) Then
            panelInstance = New FileInfoPanel
        End If
        Return panelInstance
    End Function

    ''' <summary>
    ''' Load the panel, adding a control that allows users to select a file.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub FileInfoPanel_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitializeUserControls()
        MyBase.AddParameter("file", FileChooser)
    End Sub

    ''' <summary>
    ''' Set the panel back to its original state
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeUserControls()
        MyBase.MethodNameLabel.Text = "My.Computer.FileSystem.GetFileInfo("
        FileChooser.AutoSize = True
        SetfileInfo()
    End Sub


    ''' <summary>
    ''' Populate the controls based on the user input
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ExececuteMethodButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExececuteMethodButton.Click
        SetfileInfo()
    End Sub

    ''' <summary>
    ''' Create a FileInfo for the file chosen and populate the UI with the appropriate values
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetfileInfo()
        If (Me.fileChooser.Filename <> String.Empty) Then
            fInfo = My.Computer.FileSystem.GetFileInfo(CType(Me.fileChooser.Filename, String))
            Me.nameLabel.Text = fInfo.Name
            Me.locationLabel.Text = fInfo.FullName
            Me.sizeLabel.Text = fInfo.Length.ToString
            Me.createdLabel.Text = fInfo.CreationTime.ToString
            Me.modifiedLabel.Text = fInfo.LastWriteTime.ToString
            Me.accessedLabel.Text = fInfo.LastWriteTime.ToString
            Me.readOnlyCheckBox.Checked = fInfo.IsReadOnly()
        Else
            Me.nameLabel.Text = String.Empty
            Me.locationLabel.Text = String.Empty
            Me.sizeLabel.Text = String.Empty
            Me.createdLabel.Text = String.Empty
            Me.modifiedLabel.Text = String.Empty
            Me.accessedLabel.Text = String.Empty
            Me.readOnlyCheckBox.Checked = False
        End If
    End Sub

    ''' <summary>
    ''' Reset the panel back to its original state.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ResetValuesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetValuesButton.Click
        InitializeUserControls()
    End Sub

End Class
