Partial Public Class Sites
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()> _
    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

    End Sub

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lvwWebSites = New System.Windows.Forms.ListView
        Me.ColumnHeader1 = New System.Windows.Forms.ColumnHeader("")
        Me.ColumnHeader2 = New System.Windows.Forms.ColumnHeader("")
        Me.btnAddSite = New System.Windows.Forms.Button
        Me.btnRemoveSite = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.txtWebSite = New System.Windows.Forms.TextBox
        Me.txtURL = New System.Windows.Forms.TextBox
        Me.btnOK = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'lvwWebSites
        '
        Me.lvwWebSites.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2})
        Me.lvwWebSites.FullRowSelect = True
        Me.lvwWebSites.GridLines = True
        Me.lvwWebSites.Location = New System.Drawing.Point(13, 137)
        Me.lvwWebSites.Name = "lvwWebSites"
        Me.lvwWebSites.Size = New System.Drawing.Size(441, 302)
        Me.lvwWebSites.TabIndex = 0
        Me.lvwWebSites.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Web Site"
        Me.ColumnHeader1.Width = 171
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "URL"
        Me.ColumnHeader2.Width = 408
        '
        'btnAddSite
        '
        Me.btnAddSite.Location = New System.Drawing.Point(326, 57)
        Me.btnAddSite.Name = "btnAddSite"
        Me.btnAddSite.Size = New System.Drawing.Size(128, 36)
        Me.btnAddSite.TabIndex = 1
        Me.btnAddSite.Text = "Add New Site"
        '
        'btnRemoveSite
        '
        Me.btnRemoveSite.Location = New System.Drawing.Point(472, 137)
        Me.btnRemoveSite.Name = "btnRemoveSite"
        Me.btnRemoveSite.Size = New System.Drawing.Size(128, 36)
        Me.btnRemoveSite.TabIndex = 2
        Me.btnRemoveSite.Text = "Remove This Site"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(19, 27)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(50, 14)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Web Site"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(42, 57)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(27, 14)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "URL"
        '
        'txtWebSite
        '
        Me.txtWebSite.Location = New System.Drawing.Point(76, 24)
        Me.txtWebSite.Name = "txtWebSite"
        Me.txtWebSite.Size = New System.Drawing.Size(234, 20)
        Me.txtWebSite.TabIndex = 5
        '
        'txtURL
        '
        Me.txtURL.Location = New System.Drawing.Point(76, 54)
        Me.txtURL.Multiline = True
        Me.txtURL.Name = "txtURL"
        Me.txtURL.Size = New System.Drawing.Size(234, 70)
        Me.txtURL.TabIndex = 6
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(525, 14)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 35)
        Me.btnOK.TabIndex = 7
        Me.btnOK.Text = "OK"
        '
        'frmSites
        '
        Me.ClientSize = New System.Drawing.Size(612, 501)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.txtURL)
        Me.Controls.Add(Me.txtWebSite)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnRemoveSite)
        Me.Controls.Add(Me.btnAddSite)
        Me.Controls.Add(Me.lvwWebSites)
        Me.Name = "frmSites"
        Me.Text = "Add and Remove Web Sites"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lvwWebSites As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents btnAddSite As System.Windows.Forms.Button
    Friend WithEvents btnRemoveSite As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtWebSite As System.Windows.Forms.TextBox
    Friend WithEvents txtURL As System.Windows.Forms.TextBox
    Friend WithEvents btnOK As System.Windows.Forms.Button
End Class
