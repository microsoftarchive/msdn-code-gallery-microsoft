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
        Me.lblExcel = New System.Windows.Forms.Label()
        Me.tbExcelName = New System.Windows.Forms.TextBox()
        Me.btnBrowser = New System.Windows.Forms.Button()
        Me.btnConvert = New System.Windows.Forms.Button()
        Me.tbXmlView = New System.Windows.Forms.TextBox()
        Me.btnSaveAs = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblExcel
        '
        resources.ApplyResources(Me.lblExcel, "lblExcel")
        Me.lblExcel.Name = "lblExcel"
        '
        'tbExcelName
        '
        resources.ApplyResources(Me.tbExcelName, "tbExcelName")
        Me.tbExcelName.Name = "tbExcelName"
        '
        'btnBrowser
        '
        resources.ApplyResources(Me.btnBrowser, "btnBrowser")
        Me.btnBrowser.Name = "btnBrowser"
        Me.btnBrowser.UseVisualStyleBackColor = True
        '
        'btnConvert
        '
        resources.ApplyResources(Me.btnConvert, "btnConvert")
        Me.btnConvert.Name = "btnConvert"
        Me.btnConvert.UseVisualStyleBackColor = True
        '
        'tbXmlView
        '
        resources.ApplyResources(Me.tbXmlView, "tbXmlView")
        Me.tbXmlView.Name = "tbXmlView"
        Me.tbXmlView.ReadOnly = True
        '
        'btnSaveAs
        '
        resources.ApplyResources(Me.btnSaveAs, "btnSaveAs")
        Me.btnSaveAs.Name = "btnSaveAs"
        Me.btnSaveAs.UseVisualStyleBackColor = True
        '
        'MainForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.btnSaveAs)
        Me.Controls.Add(Me.tbXmlView)
        Me.Controls.Add(Me.btnConvert)
        Me.Controls.Add(Me.btnBrowser)
        Me.Controls.Add(Me.tbExcelName)
        Me.Controls.Add(Me.lblExcel)
        Me.Name = "MainForm"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents lblExcel As System.Windows.Forms.Label
    Private WithEvents tbExcelName As System.Windows.Forms.TextBox
    Private WithEvents btnBrowser As System.Windows.Forms.Button
    Private WithEvents btnConvert As System.Windows.Forms.Button
    Private WithEvents tbXmlView As System.Windows.Forms.TextBox
    Private WithEvents btnSaveAs As System.Windows.Forms.Button

End Class
