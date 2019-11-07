' Copyright (c) Microsoft Corporation. All rights reserved.
Imports Microsoft.VisualBasic.FileIO

Public Class CopyFilePanel
    Inherits FileSystemSample.TaskPanelBase

#Region " Windows Form Designer generated code "

    Private Sub New()
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
        '
        'DescriptionTextBox
        '
        Me.DescriptionTextBox.AutoSize = False
        Me.DescriptionTextBox.Location = New System.Drawing.Point(7, 20)
        Me.DescriptionTextBox.Multiline = True
        Me.DescriptionTextBox.Size = New System.Drawing.Size(568, 61)
        Me.DescriptionTextBox.Text = "My.Computer.FileSystem.CopyFile copies from one location to another.  It exposes " & _
            "functionality to rename file during the copy, show the Windows Shell dialog, and" & _
            " overwrite the existing file in the target directory, if any."
        '
        'ExececuteMethodButton
        '
        '
        'ResetValuesButton
        '
        '
        'CopyFilePanel
        '
        Me.Name = "CopyFilePanel"

    End Sub

#End Region


    Private Shared panelInstance As CopyFilePanel
    Friend WithEvents sourceFileChooser As New FileChooser()
    Friend WithEvents targetChooser As New DirectoryChooser()
    Friend WithEvents newNameTextBox As New TextBox()
    Friend WithEvents overWriteComboBox As New ComboBox()
    Friend WithEvents showUIComboBox As New ComboBox()
    Friend WithEvents onUserCancelComboBox As New ComboBox()

    ''' <summary>
    ''' Return a global instance of this panel.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As CopyFilePanel
        If (panelInstance Is Nothing) Then
            panelInstance = New CopyFilePanel()
        End If
        Return panelInstance
    End Function

    ''' <summary>
    ''' Load the panel, adding controls to the panel for each parameter to copyDirectory
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CopyFilePanel_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitializeUserControls()
        MyBase.AddParameter("sourceFileName", sourceFileChooser)
        MyBase.AddParameter("destinationFileName", targetChooser)
        MyBase.AddParameter("showUI", showUIComboBox)
        MyBase.AddParameter("onUserCancel", onUserCancelComboBox)
    End Sub

    ''' <summary>
    ''' When the user selects a filename, automatically set the newName parameter to the filename they selected.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub sourceFileChooser_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles sourceFileChooser.Leave
        If (Me.sourceFileChooser.Filename <> String.Empty) Then
            Me.newNameTextBox.Text = System.IO.Path.GetFileName(Me.sourceFileChooser.Filename)
        End If
    End Sub


    ''' <summary>
    ''' Initialize each control with all possible parameter values
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeUserControls()
        MyBase.MethodNameLabel.Text = "My.Computer.FileSystem.CopyFile("

        Me.sourceFileChooser.Reset()
        Me.targetChooser.Reset()
        Me.newNameTextBox.Text = String.Empty

        overWriteComboBox.Items.AddRange(New String() {"True", "False"})
        overWriteComboBox.AutoSize = True
        overWriteComboBox.SelectedItem = "False"

        showUIComboBox.Items.AddRange(New String() {"True", "False"})
        showUIComboBox.AutoSize = True
        showUIComboBox.SelectedItem = "False"

        onUserCancelComboBox.Items.AddRange(New String() {"Do Nothing", "Throw Exception"})
        onUserCancelComboBox.AutoSize = True
        onUserCancelComboBox.SelectedItem = "Throw Exception"
    End Sub

    ''' <summary>
    ''' Copy the file using the parameters specified.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ExececuteMethodButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExececuteMethodButton.Click
        Try
            My.Computer.FileSystem.CopyFile( _
                                    sourceFileName:=Me.sourceFileChooser.Filename, _
                                    destinationFileName:=Me.targetChooser.Directory, _
                                    showUI:=CType(UIOption.Parse(GetType(UIOption), CType(Me.showUIComboBox.SelectedItem, String)), UIOption), _
                                    onUserCancel:=ParseUICancelOption(Me.onUserCancelComboBox))
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub ResetValuesButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ResetValuesButton.Click
        InitializeUserControls()
    End Sub
End Class
