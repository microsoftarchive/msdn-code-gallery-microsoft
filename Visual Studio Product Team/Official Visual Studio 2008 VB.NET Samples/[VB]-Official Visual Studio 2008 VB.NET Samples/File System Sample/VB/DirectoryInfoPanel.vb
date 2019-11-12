' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.IO

Public Class DirectoryInfoPanel
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
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'DescriptionTextBox
        '
        Me.DescriptionTextBox.Text = "The DirectoryInfo class encapsulates information related to directories on the fi" & _
            "le system."
        '
        'BackgroundWorker1
        '
        Me.BackgroundWorker1.WorkerReportsProgress = False
        Me.BackgroundWorker1.WorkerSupportsCancellation = False
        '
        'DirectoryInfoPanel
        '
        Me.Name = "DirectoryInfoPanel"
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Shared panelInstance As DirectoryInfoPanel
    Friend WithEvents directoryChooser As New DirectoryChooser
    Private dirInfo As DirectoryInfo
    Private dirSize As Long

    ''' <summary>
    ''' Get a global isntance of the panel.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As DirectoryInfoPanel
        If (panelInstance Is Nothing) Then
            panelInstance = New DirectoryInfoPanel
        End If
        Return panelInstance
    End Function

    ''' <summary>
    ''' Load the panel, adding a control for the directory parameter in My.Computer.FileSystem.GetDirectoryInfo
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub GetDirectoryInfo_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitializeUserControls()
        MyBase.AddParameter("directory", directoryChooser)
    End Sub


    ''' <summary>
    ''' Initialize the controls on the panel, setting each to its default value
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeUserControls()
        MyBase.MethodNameLabel.Text = "My.Computer.FileSystem.GetDirectoryInfo("
        directoryChooser.AutoSize = True
        directoryChooser.Reset()
        SetDirectoryInfo()
    End Sub


    ''' <summary>
    ''' Get the directoryInfo object for the directory specified
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ExececuteMethodButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExececuteMethodButton.Click
        SetDirectoryInfo()
    End Sub


    ''' <summary>
    ''' Set all of the directory information on the panel
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetDirectoryInfo()
        If (Me.directoryChooser.Directory <> String.Empty) Then
            dirInfo = My.Computer.FileSystem.GetDirectoryInfo(CType(Me.directoryChooser.Directory, String))
            Me.nameLabel.Text = dirInfo.Name
            Me.locationLabel.Text = dirInfo.FullName
            Me.sizeLabel.Text = "Calculating..."
            Me.createdLabel.Text = dirInfo.CreationTime.ToString
            Me.modifiedLabel.Text = dirInfo.LastWriteTime.ToString
            Me.accessedLabel.Text = dirInfo.LastWriteTime.ToString

            'Calculating Directory Size takes a long time, so we'll do it in the background
            Me.BackgroundWorker1.RunWorkerAsync()
        Else
            Me.nameLabel.Text = String.Empty
            Me.locationLabel.Text = String.Empty
            Me.sizeLabel.Text = String.Empty
            Me.createdLabel.Text = String.Empty
            Me.modifiedLabel.Text = String.Empty
            Me.accessedLabel.Text = String.Empty
        End If
    End Sub

    ''' <summary>
    ''' Reset the panel controls to their default values
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ResetValuesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetValuesButton.Click
        InitializeUserControls()
    End Sub

    ''' <summary>
    ''' DirectoryInfo does not return the size of the directory, so we have to calculate it manually.  We do this by calculating the 
    ''' size of each file and subdirectory in the chosen directory.  Because this uses recursion, it is not the most efficient technique
    ''' for calculating directory sizes.
    ''' </summary>
    ''' <param name="dir"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetDirectorySize(ByVal dir As DirectoryInfo) As Long
        Dim directorySize As Long = 0

        'First add up the size of all files in the directory
        For Each f As FileInfo In dir.GetFiles()
            directorySize += f.Length
        Next

        'Then recursively calculate the size of each sub directory
        For Each subDir As DirectoryInfo In dir.GetDirectories()
            directorySize += GetDirectorySize(subDir)
        Next
        Return directorySize
    End Function

    ''' <summary>
    ''' Calculate the directory size in the background.  Using the background worker allows us to update the UI as the size results are calculated.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        'This method will run on a thread other than the UI thread.
        'Be sure not to manipulate any Windows Forms controls created
        'on the UI thread from this method.
        Me.dirSize = Me.GetDirectorySize(dirInfo)
    End Sub

    ''' <summary>
    ''' Update the UI with the directory size
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        Me.sizeLabel.Text = Me.dirSize.ToString
    End Sub
End Class
