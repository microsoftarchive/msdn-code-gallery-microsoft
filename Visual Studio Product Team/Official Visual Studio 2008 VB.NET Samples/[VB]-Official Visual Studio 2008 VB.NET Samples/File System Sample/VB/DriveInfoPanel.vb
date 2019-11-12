' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.IO

Public Class DriveInfoPanel
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
    Protected Friend WithEvents sizeLabel As System.Windows.Forms.Label
    Protected Friend WithEvents nameLabel As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Protected Friend WithEvents volumeLabelLabel As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Protected Friend WithEvents rootDirectoryLabel As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Protected Friend WithEvents freeSpaceLabel As System.Windows.Forms.Label

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.sizeLabel = New System.Windows.Forms.Label
        Me.nameLabel = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.volumeLabelLabel = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.rootDirectoryLabel = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.freeSpaceLabel = New System.Windows.Forms.Label
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'DescriptionTextBox
        '
        Me.DescriptionTextBox.Text = "Retrieves information related to drives on the computer."
        '
        'ExececuteMethodButton
        '
        '
        'ResetValuesButton
        '
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.freeSpaceLabel)
        Me.GroupBox2.Controls.Add(Me.Label5)
        Me.GroupBox2.Controls.Add(Me.rootDirectoryLabel)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.volumeLabelLabel)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.Label3)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Controls.Add(Me.nameLabel)
        Me.GroupBox2.Controls.Add(Me.sizeLabel)
        Me.GroupBox2.Controls.SetChildIndex(Me.EndParenLabel, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.ExececuteMethodButton, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.ResetValuesButton, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.sizeLabel, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.nameLabel, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.Label1, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.Label3, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.Label2, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.volumeLabelLabel, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.Label4, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.rootDirectoryLabel, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.Label5, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.freeSpaceLabel, 0)
        '
        'sizeLabel
        '
        Me.sizeLabel.AutoSize = True
        Me.sizeLabel.Location = New System.Drawing.Point(300, 211)
        Me.sizeLabel.Name = "sizeLabel"
        Me.sizeLabel.Size = New System.Drawing.Size(38, 14)
        Me.sizeLabel.TabIndex = 22
        Me.sizeLabel.Text = "<size>"
        '
        'nameLabel
        '
        Me.nameLabel.AutoSize = True
        Me.nameLabel.Location = New System.Drawing.Point(300, 190)
        Me.nameLabel.Name = "nameLabel"
        Me.nameLabel.Size = New System.Drawing.Size(46, 14)
        Me.nameLabel.TabIndex = 24
        Me.nameLabel.Text = "<name>"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(172, 211)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(26, 14)
        Me.Label3.TabIndex = 26
        Me.Label3.Text = "Size "
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(172, 190)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(38, 14)
        Me.Label1.TabIndex = 25
        Me.Label1.Text = "Name: "
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(172, 253)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(77, 14)
        Me.Label2.TabIndex = 27
        Me.Label2.Text = "Volume Label:"
        '
        'volumeLabelLabel
        '
        Me.volumeLabelLabel.AutoSize = True
        Me.volumeLabelLabel.Location = New System.Drawing.Point(300, 253)
        Me.volumeLabelLabel.Name = "volumeLabelLabel"
        Me.volumeLabelLabel.Size = New System.Drawing.Size(82, 14)
        Me.volumeLabelLabel.TabIndex = 28
        Me.volumeLabelLabel.Text = "<volumeLabel>"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(172, 274)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(80, 14)
        Me.Label4.TabIndex = 29
        Me.Label4.Text = "Root Directory:"
        '
        'rootDirectoryLabel
        '
        Me.rootDirectoryLabel.AutoSize = True
        Me.rootDirectoryLabel.Location = New System.Drawing.Point(300, 274)
        Me.rootDirectoryLabel.Name = "rootDirectoryLabel"
        Me.rootDirectoryLabel.Size = New System.Drawing.Size(82, 14)
        Me.rootDirectoryLabel.TabIndex = 30
        Me.rootDirectoryLabel.Text = "<rootDirectory>"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(172, 232)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(66, 14)
        Me.Label5.TabIndex = 31
        Me.Label5.Text = "Free Space: "
        '
        'freeSpaceLabel
        '
        Me.freeSpaceLabel.AutoSize = True
        Me.freeSpaceLabel.Location = New System.Drawing.Point(300, 232)
        Me.freeSpaceLabel.Name = "freeSpaceLabel"
        Me.freeSpaceLabel.Size = New System.Drawing.Size(69, 14)
        Me.freeSpaceLabel.TabIndex = 32
        Me.freeSpaceLabel.Text = "<freeSpace>"
        '
        'DirectoryInfoPanel
        '
        Me.Name = "DirectoryInfoPanel"
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Shared panelInstance As DriveInfoPanel
    Friend WithEvents drivesComboBox As New ComboBox()

    ''' <summary>
    ''' Get a global instance of this panel
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As DriveInfoPanel
        If (panelInstance Is Nothing) Then
            panelInstance = New DriveInfoPanel
        End If
        Return panelInstance
    End Function

    ''' <summary>
    ''' Load the panel, adding a control that allows users to select from a list of available drives.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub GetDriveInfo_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitializeUserControls()
        MyBase.AddParameter("drive", drivesComboBox)
    End Sub


    ''' <summary>
    ''' Set the controls to their default values
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeUserControls()
        MyBase.MethodNameLabel.Text = "My.Computer.FileSystem.GetDriveInfo("

        Me.drivesComboBox.Items.Clear()

        For Each drive As DriveInfo In My.Computer.FileSystem.Drives
            drivesComboBox.Items.Add(drive.Name)
        Next
        drivesComboBox.AutoSize = True
        drivesComboBox.SelectedIndex = 0
        Me.sizeLabel.Text = String.Empty
        Me.volumeLabelLabel.Text = String.Empty
        Me.rootDirectoryLabel.Text = String.Empty
        Me.nameLabel.Text = String.Empty
        Me.freeSpaceLabel.Text = String.Empty
    End Sub

    ''' <summary>
    ''' Display drive information for the chosen drive
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ExececuteMethodButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExececuteMethodButton.Click
        SetDriveInfo()
    End Sub


    ''' <summary>
    ''' Set the value of each control equal to 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetDriveInfo()
        Dim drive As DriveInfo = My.Computer.FileSystem.GetDriveInfo(CType(Me.drivesComboBox.SelectedItem, String))
        Me.nameLabel.Text = drive.Name
        Me.rootDirectoryLabel.Text = drive.RootDirectory.Name

        'These properties require that the drive is ready.
        If (drive.IsReady) Then
            Me.sizeLabel.Text = CType(drive.TotalSize, String)
            Me.freeSpaceLabel.Text = CType(drive.TotalFreeSpace, String)
            Me.volumeLabelLabel.Text = drive.VolumeLabel
        End If
    End Sub

    ''' <summary>
    ''' Reset the controls to their original value
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ResetValuesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetValuesButton.Click
        InitializeUserControls()
    End Sub

    ''' <summary>
    ''' The drive selected has changed, so update the UI
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub drivesComboBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        SetDriveInfo()
    End Sub
End Class
