' Copyright (c) Microsoft Corporation. All rights reserved.
Imports Microsoft.VisualBasic.FileIO

Public Class CopyDirectoryPanel
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
        Me.DescriptionTextBox.Size = New System.Drawing.Size(568, 50)
        Me.DescriptionTextBox.Text = "Copies a directory (including its head) under a target a directory.  If a directo" & _
            "ry of the same name exists under the target directory, the contents of the copie" & _
            "d directory will be merged into the existing directory.  Like other methods on M" & _
            "y.Computer.FileSystem.MoveFile optionally shows Windows shell dialogs"
        '
        'CopyDirectoryPanel
        '
        Me.Name = "CopyDirectoryPanel"

    End Sub

#End Region

    Private Shared panelInstance As CopyDirectoryPanel
    Friend WithEvents sourceChooser As New DirectoryChooser
    Friend WithEvents targetChooser As New DirectoryChooser()
    Friend WithEvents newNameTextBox As New TextBox()
    Friend WithEvents overWriteComboBox As New ComboBox()
    Friend WithEvents showUIComboBox As New ComboBox()
    Friend WithEvents onUserCancelComboBox As New ComboBox()


    ''' <summary>
    ''' Get an instance of this control
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As CopyDirectoryPanel
        If (panelInstance Is Nothing) Then
            panelInstance = New CopyDirectoryPanel()
        End If
        Return panelInstance
    End Function

    ''' <summary>
    ''' Load the panel and add a control for each parameter in My.Computer.FileSystem.CopyDirectory
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CopyDirectoryPanel_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitializeUserControls()
        MyBase.AddParameter("sourceDirectoryName", sourceChooser)
        MyBase.AddParameter("destinationDirectoryName", targetChooser)
        MyBase.AddParameter("showUI", showUIComboBox)
        MyBase.AddParameter("onUserCancel", onUserCancelComboBox)
    End Sub


    ''' <summary>
    ''' Initialize all user controls
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeUserControls()
        MyBase.MethodNameLabel.Text = "My.Computer.FileSystem.CopyDirectory("

        sourceChooser.Reset()
        targetChooser.Reset()
        Me.newNameTextBox.Text = String.Empty

        overWriteComboBox.Items.AddRange(New String() {"True", "False"})
        overWriteComboBox.AutoSize = True
        overWriteComboBox.SelectedItem = "False"

        showUIComboBox.Items.AddRange(New String() {"OnlyErrorDialogs", "AllDialogs"})
        showUIComboBox.AutoSize = True
        showUIComboBox.SelectedItem = "AllDialogs"

        onUserCancelComboBox.Items.AddRange(New String() {"Do Nothing", "Throw Exception"})
        onUserCancelComboBox.AutoSize = True
        onUserCancelComboBox.SelectedItem = "Throw Exception"
    End Sub


    ''' <summary>
    ''' Copy the directory based on the input provided.  Named parameters are used for clarity.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ExececuteMethodButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExececuteMethodButton.Click
        Try
            My.Computer.FileSystem.CopyDirectory( _
                                    sourceDirectoryName:=Me.sourceChooser.Directory, _
                                    destinationDirectoryName:=Me.targetChooser.Directory, _
                                    showUI:=CType(UIOption.Parse(GetType(UIOption), CType(Me.showUIComboBox.SelectedItem, String)), UIOption), _
                                    onUserCancel:=ParseUICancelOption(Me.onUserCancelComboBox))
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Initialize all controls to their default values
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ResetValuesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetValuesButton.Click
        InitializeUserControls()
    End Sub

    ''' <summary>
    ''' When the user selects a director, automatically populate the "newName" field using the directory selected.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub sourceChooser_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles sourceChooser.Leave
        Me.newNameTextBox.Text = My.Computer.FileSystem.GetDirectoryInfo(sourceChooser.Directory).Name
    End Sub
End Class

