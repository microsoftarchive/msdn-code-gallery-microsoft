<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.stsMessage = New System.Windows.Forms.StatusStrip()
        Me.tsslbResult = New System.Windows.Forms.ToolStripStatusLabel()
        Me.grpNetworkAdapters = New System.Windows.Forms.GroupBox()
        Me.stsMessage.SuspendLayout()
        Me.SuspendLayout()
        '
        'stsMessage
        '
        Me.stsMessage.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsslbResult})
        resources.ApplyResources(Me.stsMessage, "stsMessage")
        Me.stsMessage.Name = "stsMessage"
        '
        'tsslbResult
        '
        Me.tsslbResult.BackColor = System.Drawing.SystemColors.Control
        Me.tsslbResult.Name = "tsslbResult"
        resources.ApplyResources(Me.tsslbResult, "tsslbResult")
        '
        'grpNetworkAdapters
        '
        Me.grpNetworkAdapters.BackColor = System.Drawing.SystemColors.ControlLightLight
        resources.ApplyResources(Me.grpNetworkAdapters, "grpNetworkAdapters")
        Me.grpNetworkAdapters.Name = "grpNetworkAdapters"
        Me.grpNetworkAdapters.TabStop = False
        '
        'EnableDisableNetworkAdapterForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.Controls.Add(Me.stsMessage)
        Me.Controls.Add(Me.grpNetworkAdapters)
        Me.MaximizeBox = False
        Me.Name = "EnableDisableNetworkAdapterForm"
        Me.stsMessage.ResumeLayout(False)
        Me.stsMessage.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents stsMessage As System.Windows.Forms.StatusStrip
    Private WithEvents tsslbResult As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents grpNetworkAdapters As System.Windows.Forms.GroupBox

End Class
