Imports System.Security.Permissions
Partial Public Class MainForm
    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso _session IsNot Nothing Then
            _session.Dispose()
        End If

        If disposing AndAlso (components IsNot Nothing) Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.pnlState = New System.Windows.Forms.Panel()
        Me.chkEnableTimer = New System.Windows.Forms.CheckBox()
        Me.lbState = New System.Windows.Forms.Label()
        Me.pnlList = New System.Windows.Forms.Panel()
        Me.lstRecord = New System.Windows.Forms.ListBox()
        Me.pnlState.SuspendLayout()
        Me.pnlList.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlState
        '
        Me.pnlState.Controls.Add(Me.chkEnableTimer)
        Me.pnlState.Controls.Add(Me.lbState)
        Me.pnlState.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlState.Location = New System.Drawing.Point(0, 0)
        Me.pnlState.Name = "pnlState"
        Me.pnlState.Size = New System.Drawing.Size(717, 29)
        Me.pnlState.TabIndex = 0
        '
        'chkEnableTimer
        '
        Me.chkEnableTimer.AutoSize = True
        Me.chkEnableTimer.Location = New System.Drawing.Point(410, 6)
        Me.chkEnableTimer.Name = "chkEnableTimer"
        Me.chkEnableTimer.Size = New System.Drawing.Size(304, 17)
        Me.chkEnableTimer.TabIndex = 2
        Me.chkEnableTimer.Text = "Enable a timer to detect the session state every 5 seconds "
        Me.chkEnableTimer.UseVisualStyleBackColor = True
        '
        'lbState
        '
        Me.lbState.AutoSize = True
        Me.lbState.Location = New System.Drawing.Point(13, 8)
        Me.lbState.Name = "lbState"
        Me.lbState.Size = New System.Drawing.Size(69, 13)
        Me.lbState.TabIndex = 0
        Me.lbState.Text = "Current State"
        '
        'pnlList
        '
        Me.pnlList.Controls.Add(Me.lstRecord)
        Me.pnlList.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlList.Location = New System.Drawing.Point(0, 29)
        Me.pnlList.Name = "pnlList"
        Me.pnlList.Size = New System.Drawing.Size(717, 149)
        Me.pnlList.TabIndex = 1
        '
        'lstRecord
        '
        Me.lstRecord.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstRecord.FormattingEnabled = True
        Me.lstRecord.Location = New System.Drawing.Point(0, 0)
        Me.lstRecord.Name = "lstRecord"
        Me.lstRecord.Size = New System.Drawing.Size(717, 149)
        Me.lstRecord.TabIndex = 0
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(717, 178)
        Me.Controls.Add(Me.pnlList)
        Me.Controls.Add(Me.pnlState)
        Me.Name = "MainForm"
        Me.Text = "DetectWindowsSessionState"
        Me.pnlState.ResumeLayout(False)
        Me.pnlState.PerformLayout()
        Me.pnlList.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private pnlState As Panel
    Private lbState As Label
    Private pnlList As Panel
    Private lstRecord As ListBox
    Private WithEvents chkEnableTimer As System.Windows.Forms.CheckBox
End Class

