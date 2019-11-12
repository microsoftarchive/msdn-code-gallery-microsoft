' Copyright (c) Microsoft Corporation. All rights reserved.
Partial Public Class MySamples
    Inherits InheritForm.BaseForm

    <System.Diagnostics.DebuggerNonUserCode()> _
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

    Friend WithEvents txtMySamples As System.Windows.Forms.TextBox
    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.txtMySamples = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'lblProtected
        '
        Me.lblProtected.Size = New System.Drawing.Size(0, 0)
        Me.lblProtected.Text = ""

        'txtMySamples
        '
        Me.txtMySamples.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtMySamples.Location = New System.Drawing.Point(13, 46)
        Me.txtMySamples.Multiline = True
        Me.txtMySamples.Name = "txtMySamples"
        Me.txtMySamples.ReadOnly = True
        Me.txtMySamples.Size = New System.Drawing.Size(489, 239)
        Me.txtMySamples.TabIndex = 13
        '
        'MySamples
        '
        Me.ClientSize = New System.Drawing.Size(694, 379)
        Me.Controls.Add(Me.txtMySamples)
        Me.Name = "MySamples"
        Me.Text = "My Samples"
        Me.Controls.SetChildIndex(Me.txtMySamples, 0)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

End Class


