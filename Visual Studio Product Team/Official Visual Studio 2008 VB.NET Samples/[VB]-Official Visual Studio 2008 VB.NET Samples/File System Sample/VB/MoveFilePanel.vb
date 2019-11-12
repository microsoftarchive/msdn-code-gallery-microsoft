' Copyright (c) Microsoft Corporation. All rights reserved.
Imports Microsoft.VisualBasic.FileIO

Public Class MoveFilePanel
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
        Me.DescriptionTextBox.Text = "Moves a file from one directory to another, optionally showing Windows shell dial" & _
            "ogs and overwriting the target file, if any."
        '
        'MoveFilePanel
        '
        Me.Name = "MoveFilePanel"

    End Sub

#End Region


    Private Shared panelInstance As MoveFilePanel
    Friend WithEvents sourceFileChooser As New FileChooser()
    Friend WithEvents targetDirectoryChooser As New DirectoryChooser()
    Friend WithEvents showUIComboBox As New ComboBox()
    Friend WithEvents onUserCancelComboBox As New ComboBox()

    ''' <summary>
    ''' Get a global instance of this panel
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As MoveFilePanel
        If (panelInstance Is Nothing) Then
            panelInstance = New MoveFilePanel()
        End If
        Return panelInstance
    End Function

    ''' <summary>
    ''' Load the panel and add a control for each parameter to My.Computer.FileSystem.MoveFile
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MoveFilePanel_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitializeUserControls()
        MyBase.AddParameter("sourceFileName", sourceFileChooser)
        MyBase.AddParameter("destinationFileName", targetDirectoryChooser)
        MyBase.AddParameter("showUI", showUIComboBox)
        MyBase.AddParameter("onUserCancel", onUserCancelComboBox)
    End Sub

    ''' <summary>
    ''' Initialize all of the controls on the panel to their default values
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeUserControls()
        MyBase.MethodNameLabel.Text = "My.Computer.FileSystem.MoveFile("

        sourceFileChooser.Reset()

        showUIComboBox.Items.AddRange(New String() {"True", "False"})
        showUIComboBox.AutoSize = True
        showUIComboBox.SelectedItem = "False"

        onUserCancelComboBox.Items.AddRange(New String() {"Do Nothing", "Throw Exception"})
        onUserCancelComboBox.AutoSize = True
        onUserCancelComboBox.SelectedItem = "Throw Exception"

    End Sub


    ''' <summary>
    ''' Move the file using the parameters provided.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ExececuteMethodButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExececuteMethodButton.Click
        Try
            My.Computer.FileSystem.MoveFile( _
                                    sourceFileName:=Me.sourceFileChooser.Filename, _
                                    destinationFileName:=Me.targetDirectoryChooser.Directory, _
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
