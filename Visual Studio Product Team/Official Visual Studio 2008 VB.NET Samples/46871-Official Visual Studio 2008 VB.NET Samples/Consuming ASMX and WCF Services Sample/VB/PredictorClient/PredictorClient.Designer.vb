<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPredictor
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPredictor))
        Me.txtQuestion = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblAnswer = New System.Windows.Forms.Label
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.btnAsk = New System.Windows.Forms.Button
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'txtQuestion
        '
        Me.txtQuestion.Location = New System.Drawing.Point(15, 33)
        Me.txtQuestion.Name = "txtQuestion"
        Me.txtQuestion.Size = New System.Drawing.Size(295, 20)
        Me.txtQuestion.TabIndex = 6
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(319, 13)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Enter your question below and click ""Ask"" to receive your answer."
        '
        'lblAnswer
        '
        Me.lblAnswer.AutoSize = True
        Me.lblAnswer.BackColor = System.Drawing.Color.Navy
        Me.lblAnswer.Font = New System.Drawing.Font("Comic Sans MS", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAnswer.ForeColor = System.Drawing.SystemColors.ControlLightLight
        Me.lblAnswer.Location = New System.Drawing.Point(139, 214)
        Me.lblAnswer.MaximumSize = New System.Drawing.Size(100, 40)
        Me.lblAnswer.MinimumSize = New System.Drawing.Size(100, 40)
        Me.lblAnswer.Name = "lblAnswer"
        Me.lblAnswer.Size = New System.Drawing.Size(100, 40)
        Me.lblAnswer.TabIndex = 8
        Me.lblAnswer.Text = "Ask Me!"
        Me.lblAnswer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.MagicEightballClient.My.Resources.Resources.predictor
        Me.PictureBox1.Location = New System.Drawing.Point(15, 60)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(376, 359)
        Me.PictureBox1.TabIndex = 7
        Me.PictureBox1.TabStop = False
        '
        'btnAsk
        '
        Me.btnAsk.Location = New System.Drawing.Point(316, 31)
        Me.btnAsk.Name = "btnAsk"
        Me.btnAsk.Size = New System.Drawing.Size(75, 23)
        Me.btnAsk.TabIndex = 4
        Me.btnAsk.Text = "Ask"
        Me.btnAsk.UseVisualStyleBackColor = True
        '
        'frmPredictor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(403, 431)
        Me.Controls.Add(Me.lblAnswer)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.txtQuestion)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnAsk)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmPredictor"
        Me.Text = "Predictor"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtQuestion As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblAnswer As System.Windows.Forms.Label
    Friend WithEvents btnAsk As System.Windows.Forms.Button

End Class
