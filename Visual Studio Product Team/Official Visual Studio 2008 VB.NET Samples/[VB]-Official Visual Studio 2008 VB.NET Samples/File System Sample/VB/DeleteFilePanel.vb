' Copyright (c) Microsoft Corporation. All rights reserved.
Imports Microsoft.VisualBasic.FileIO

Public Class DeleteFilePanel
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
        Me.DescriptionTextBox.Text = "Deletes a file from the filesystem, optionally showing Windows shell dialogs and " & _
            "sending the file to the Recycle Bin."
        '
        'ExececuteMethodButton
        '
        '
        'ResetValuesButton
        '
        '
        'DeleteFilePanel
        '
        Me.Name = "DeleteFilePanel"

    End Sub

#End Region


    Private Shared panelInstance As DeleteFilePanel
    Friend WithEvents sourceFileChooser As New FileChooser()
    Friend WithEvents sendToRecycleBinComboBox As New ComboBox()
    Friend WithEvents showUIComboBox As New ComboBox()
    Friend WithEvents onUserCancelComboBox As New ComboBox()


    ''' <summary>
    ''' Return a global instance of this panel.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As DeleteFilePanel
        If (panelInstance Is Nothing) Then
            panelInstance = New DeleteFilePanel
        End If
        Return panelInstance
    End Function

    ''' <summary>
    ''' Load the panel and place a control for each paramater to My.Computer.FileSystem.DeleteFile on the panel.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DeleteFilePanel_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitializeUserControls()
        MyBase.AddParameter("file", sourceFileChooser)
        MyBase.AddParameter("recycle", sendToRecycleBinComboBox)
        MyBase.AddParameter("showUI", showUIComboBox)
        MyBase.AddParameter("onUserCancel", onUserCancelComboBox)
    End Sub

    ''' <summary>
    ''' Initialize the panel and set each control to its default value.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeUserControls()
        MyBase.MethodNameLabel.Text = "My.Computer.FileSystem.DeleteFile("
        sourceFileChooser.Reset()

        sendToRecycleBinComboBox.Items.AddRange(New String() {"True", "False"})
        sendToRecycleBinComboBox.AutoSize = True
        sendToRecycleBinComboBox.SelectedItem = "False"

        showUIComboBox.Items.AddRange(New String() {"True", "False"})
        showUIComboBox.AutoSize = True
        showUIComboBox.SelectedItem = "False"

        onUserCancelComboBox.Items.AddRange(New String() {"Do Nothing", "Throw Exception"})
        onUserCancelComboBox.AutoSize = True
        onUserCancelComboBox.SelectedItem = "Throw Exception"
    End Sub

    ''' <summary>
    ''' Delete the specified file using My.Computer.FileSystem.DeleteFile().  All input validation is deferred to DeleteFile().
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ExececuteMethodButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExececuteMethodButton.Click
        Try
            My.Computer.FileSystem.DeleteFile( _
                                    file:=Me.sourceFileChooser.Filename, _
                                    showUI:=CType(UIOption.Parse(GetType(UIOption), CType(Me.showUIComboBox.SelectedItem, String)), UIOption), _
                                    recycle:=CType(RecycleOption.Parse(GetType(RecycleOption), CType(Me.sendToRecycleBinComboBox.SelectedItem, String)), RecycleOption), _
                                    onUserCancel:=ParseUICancelOption(Me.onUserCancelComboBox))
        Catch ex As Exception

            MessageBox.Show(ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Reset the panel to its original state.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ResetValuesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetValuesButton.Click
        InitializeUserControls()
    End Sub

End Class
