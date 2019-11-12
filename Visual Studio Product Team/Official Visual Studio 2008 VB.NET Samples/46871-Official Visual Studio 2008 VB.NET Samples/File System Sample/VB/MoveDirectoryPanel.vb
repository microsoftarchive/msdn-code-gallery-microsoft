' Copyright (c) Microsoft Corporation. All rights reserved.
Imports Microsoft.VisualBasic.FileIO

Public Class MoveDirectoryPanel
    Inherits FileSystemSample.TaskPanelBase

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
        '
        'DescriptionTextBox
        '
        Me.DescriptionTextBox.AutoSize = False
        Me.DescriptionTextBox.Multiline = True
        Me.DescriptionTextBox.Size = New System.Drawing.Size(568, 55)
        Me.DescriptionTextBox.Text = "Moves a directory (including its head) under the target specified target.  If a d" & _
            "irectory with the same name exists under the target directory, the contents will" & _
            " be merged.  Windows shell dialogs may be enabled by setting showUI = true"
        '
        'ExececuteMethodButton
        '
        '
        'ResetValuesButton
        '
        '
        'MoveDirectoryPanel
        '
        Me.Name = "MoveDirectoryPanel"

    End Sub

#End Region

    Private Shared panelInstance As MoveDirectoryPanel
    Friend WithEvents sourceDirectoryChooser As New DirectoryChooser()
    Friend WithEvents targetDirectoryChooser As New DirectoryChooser()
    Friend WithEvents showUIComboBox As New ComboBox()
    Friend WithEvents onUserCancelComboBox As New ComboBox()

    ''' <summary>
    ''' Get a global instance of the panel
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As MoveDirectoryPanel
        If (panelInstance Is Nothing) Then
            panelInstance = New MoveDirectoryPanel()
        End If
        Return panelInstance
    End Function

    ''' <summary>
    ''' Create the panel and add a control for each parameter to My.Computer.FileSystem.MoveDirectory()
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MoveDirectoryPanel_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitializeUserControls()
        MyBase.AddParameter("sourceDirectoryName", sourceDirectoryChooser)
        MyBase.AddParameter("destinationDirectoryName", targetDirectoryChooser)
        MyBase.AddParameter("showUI", showUIComboBox)
        MyBase.AddParameter("onUserCancel", onUserCancelComboBox)
    End Sub


    ''' <summary>
    ''' Create the panel with the necessary control.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeUserControls()
        MyBase.MethodNameLabel.Text = "My.Computer.FileSystem.MoveDirectory("

        sourceDirectoryChooser.Reset()
        targetDirectoryChooser.Reset()

        showUIComboBox.Items.AddRange(New String() {"True", "False"})
        showUIComboBox.AutoSize = True
        showUIComboBox.SelectedItem = "False"

        onUserCancelComboBox.Items.AddRange(New String() {"Do Nothing", "Throw Exception"})
        onUserCancelComboBox.AutoSize = True
        onUserCancelComboBox.SelectedItem = "Throw Exception"

    End Sub

    ''' <summary>
    ''' Move the directory specified using My.Computer.FileSystem.MoveDirectory()
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ExececuteMethodButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExececuteMethodButton.Click
        Try
            My.Computer.FileSystem.MoveDirectory( _
                                    sourceDirectoryName:=Me.sourceDirectoryChooser.Directory, _
                                    destinationDirectoryName:=Me.targetDirectoryChooser.Directory, _
                                    showUI:=CType(UIOption.Parse(GetType(UIOption), CType(Me.showUIComboBox.SelectedItem, String)), UIOption), _
                                    onUserCancel:=ParseUICancelOption(Me.onUserCancelComboBox))
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Reset the panel to its original state
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ResetValuesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetValuesButton.Click
        InitializeUserControls()
    End Sub
End Class
