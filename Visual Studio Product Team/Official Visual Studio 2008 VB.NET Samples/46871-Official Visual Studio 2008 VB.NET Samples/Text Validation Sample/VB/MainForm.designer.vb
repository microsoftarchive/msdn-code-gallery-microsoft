Partial Public Class MainForm
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
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.txtInvalidControls = New System.Windows.Forms.TextBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.btnValidate = New System.Windows.Forms.Button
        Me.mskZipCode = New System.Windows.Forms.MaskedTextBox
        Me.mskIPAddress = New System.Windows.Forms.MaskedTextBox
        Me.mskSocialSecurity = New System.Windows.Forms.MaskedTextBox
        Me.mskPhoneNumber = New System.Windows.Forms.MaskedTextBox
        Me.lblIPAddress = New System.Windows.Forms.Label
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.rexZipCode = New ValidateText.ZipCodeTextBox
        Me.rexEmail = New ValidateText.EMailTextBox
        Me.rexIPAddress = New ValidateText.IPAddressTextBox
        Me.rexSocialSecurity = New ValidateText.SsnTextbox
        Me.rexPhoneNumber = New ValidateText.PhoneTextBox
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(20, 86)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(101, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "5 Digit US Zip Code"
        '
        'Label2
        '
        Me.Label2.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(20, 122)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(73, 13)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Email Address"
        '
        'Label3
        '
        Me.Label3.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(20, 158)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(58, 13)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "IP Address"
        '
        'Label4
        '
        Me.Label4.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(20, 194)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(117, 13)
        Me.Label4.TabIndex = 13
        Me.Label4.Text = "Social Security Number"
        '
        'Label5
        '
        Me.Label5.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(20, 230)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(96, 13)
        Me.Label5.TabIndex = 16
        Me.Label5.Text = "US Phone Number"
        '
        'txtInvalidControls
        '
        Me.txtInvalidControls.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.txtInvalidControls.Location = New System.Drawing.Point(20, 327)
        Me.txtInvalidControls.Multiline = True
        Me.txtInvalidControls.Name = "txtInvalidControls"
        Me.txtInvalidControls.ReadOnly = True
        Me.txtInvalidControls.Size = New System.Drawing.Size(528, 197)
        Me.txtInvalidControls.TabIndex = 19
        '
        'Label6
        '
        Me.Label6.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(176, 56)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(133, 13)
        Me.Label6.TabIndex = 0
        Me.Label6.Text = "Using Regular Expressions"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label7
        '
        Me.Label7.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(417, 56)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(127, 13)
        Me.Label7.TabIndex = 1
        Me.Label7.Text = "Using Masked Textboxes"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnValidate
        '
        Me.btnValidate.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btnValidate.Location = New System.Drawing.Point(346, 273)
        Me.btnValidate.Name = "btnValidate"
        Me.btnValidate.Size = New System.Drawing.Size(75, 31)
        Me.btnValidate.TabIndex = 20
        Me.btnValidate.Text = "Validate"
        '
        'mskZipCode
        '
        Me.mskZipCode.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.mskZipCode.CutCopyMaskFormat = System.Windows.Forms.MaskFormat.IncludeLiterals
        Me.mskZipCode.HidePromptOnLeave = False
        Me.mskZipCode.Location = New System.Drawing.Point(504, 85)
        Me.mskZipCode.Mask = "00000"
        Me.mskZipCode.Name = "mskZipCode"
        Me.mskZipCode.Size = New System.Drawing.Size(45, 20)
        Me.mskZipCode.TabIndex = 4
        Me.mskZipCode.TextMaskFormat = System.Windows.Forms.MaskFormat.IncludeLiterals
        '
        'mskIPAddress
        '
        Me.mskIPAddress.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.mskIPAddress.CutCopyMaskFormat = System.Windows.Forms.MaskFormat.IncludeLiterals
        Me.mskIPAddress.HidePromptOnLeave = False
        Me.mskIPAddress.Location = New System.Drawing.Point(463, 157)
        Me.mskIPAddress.Mask = "990.990.990.990"
        Me.mskIPAddress.Name = "mskIPAddress"
        Me.mskIPAddress.Size = New System.Drawing.Size(85, 20)
        Me.mskIPAddress.TabIndex = 11
        Me.mskIPAddress.TextMaskFormat = System.Windows.Forms.MaskFormat.IncludeLiterals
        '
        'mskSocialSecurity
        '
        Me.mskSocialSecurity.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.mskSocialSecurity.CutCopyMaskFormat = System.Windows.Forms.MaskFormat.IncludeLiterals
        Me.mskSocialSecurity.HidePromptOnLeave = False
        Me.mskSocialSecurity.Location = New System.Drawing.Point(463, 193)
        Me.mskSocialSecurity.Mask = "000-00-0000"
        Me.mskSocialSecurity.Name = "mskSocialSecurity"
        Me.mskSocialSecurity.Size = New System.Drawing.Size(85, 20)
        Me.mskSocialSecurity.TabIndex = 15
        Me.mskSocialSecurity.TextMaskFormat = System.Windows.Forms.MaskFormat.IncludeLiterals
        '
        'mskPhoneNumber
        '
        Me.mskPhoneNumber.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.mskPhoneNumber.CutCopyMaskFormat = System.Windows.Forms.MaskFormat.IncludeLiterals
        Me.mskPhoneNumber.HidePromptOnLeave = False
        Me.mskPhoneNumber.Location = New System.Drawing.Point(463, 230)
        Me.mskPhoneNumber.Mask = "(999)000-0000"
        Me.mskPhoneNumber.Name = "mskPhoneNumber"
        Me.mskPhoneNumber.Size = New System.Drawing.Size(85, 20)
        Me.mskPhoneNumber.TabIndex = 18
        Me.mskPhoneNumber.TextMaskFormat = System.Windows.Forms.MaskFormat.IncludeLiterals
        '
        'lblIPAddress
        '
        Me.lblIPAddress.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lblIPAddress.AutoSize = True
        Me.lblIPAddress.Location = New System.Drawing.Point(569, 160)
        Me.lblIPAddress.Name = "lblIPAddress"
        Me.lblIPAddress.Size = New System.Drawing.Size(39, 13)
        Me.lblIPAddress.TabIndex = 12
        Me.lblIPAddress.Text = "Label9"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(732, 24)
        Me.MenuStrip1.TabIndex = 21
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'fileToolStripMenuItem
        '
        Me.fileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.exitToolStripMenuItem})
        Me.fileToolStripMenuItem.Name = "fileToolStripMenuItem"
        Me.fileToolStripMenuItem.Size = New System.Drawing.Size(40, 20)
        Me.fileToolStripMenuItem.Text = "&File"
        '
        'exitToolStripMenuItem
        '
        Me.exitToolStripMenuItem.Name = "exitToolStripMenuItem"
        Me.exitToolStripMenuItem.Size = New System.Drawing.Size(109, 22)
        Me.exitToolStripMenuItem.Text = "E&xit"
        '
        'rexZipCode
        '
        Me.rexZipCode.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.rexZipCode.ErrorColor = System.Drawing.Color.Red
        Me.rexZipCode.ErrorMessage = "The zip code must be in the form of 99999"
        Me.rexZipCode.Location = New System.Drawing.Point(272, 86)
        Me.rexZipCode.Name = "rexZipCode"
        Me.rexZipCode.Size = New System.Drawing.Size(45, 20)
        Me.rexZipCode.TabIndex = 3
        Me.rexZipCode.ValidationExpression = "^\d{5}$"
        '
        'rexEmail
        '
        Me.rexEmail.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.rexEmail.ErrorColor = System.Drawing.Color.Red
        Me.rexEmail.ErrorMessage = "The e-mail address must be in the form of abc@microsoft.com"
        Me.rexEmail.Location = New System.Drawing.Point(115, 121)
        Me.rexEmail.Name = "rexEmail"
        Me.rexEmail.Size = New System.Drawing.Size(202, 20)
        Me.rexEmail.TabIndex = 6
        Me.rexEmail.ValidationExpression = "^([a-zA-Z0-9_\-])([a-zA-Z0-9_\-\.]*)@(\[((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0" & _
            "-9]|[0-9])\.){3}|((([a-zA-Z0-9\-]+)\.)+))([a-zA-Z]{2,}|(25[0-5]|2[0-4][0-9]|1[0-" & _
            "9][0-9]|[1-9][0-9]|[0-9])\])$"
        '
        'rexIPAddress
        '
        Me.rexIPAddress.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.rexIPAddress.ErrorColor = System.Drawing.Color.Red
        Me.rexIPAddress.ErrorMessage = "The IP address must be in the form of 111.111.111.111"
        Me.rexIPAddress.Location = New System.Drawing.Point(232, 157)
        Me.rexIPAddress.Name = "rexIPAddress"
        Me.rexIPAddress.Size = New System.Drawing.Size(85, 20)
        Me.rexIPAddress.TabIndex = 10
        Me.rexIPAddress.ValidationExpression = "^((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}(25[0-5]|2[0-4][0-9]|1[" & _
            "0-9][0-9]|[1-9][0-9]|[0-9])$"
        '
        'rexSocialSecurity
        '
        Me.rexSocialSecurity.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.rexSocialSecurity.ErrorColor = System.Drawing.Color.Red
        Me.rexSocialSecurity.ErrorMessage = "The social security number must be in the form of 555-55-5555"
        Me.rexSocialSecurity.Location = New System.Drawing.Point(232, 193)
        Me.rexSocialSecurity.Name = "rexSocialSecurity"
        Me.rexSocialSecurity.Size = New System.Drawing.Size(85, 20)
        Me.rexSocialSecurity.TabIndex = 14
        Me.rexSocialSecurity.ValidationExpression = "^\d{3}-\d{2}-\d{4}$"
        '
        'rexPhoneNumber
        '
        Me.rexPhoneNumber.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.rexPhoneNumber.ErrorColor = System.Drawing.Color.Red
        Me.rexPhoneNumber.ErrorMessage = "The phone number must be in the form of (555) 555-1212 or 555-555-1212."
        Me.rexPhoneNumber.Location = New System.Drawing.Point(232, 230)
        Me.rexPhoneNumber.Name = "rexPhoneNumber"
        Me.rexPhoneNumber.Size = New System.Drawing.Size(85, 20)
        Me.rexPhoneNumber.TabIndex = 17
        Me.rexPhoneNumber.ValidationExpression = "^((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}$"
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(732, 546)
        Me.Controls.Add(Me.lblIPAddress)
        Me.Controls.Add(Me.mskPhoneNumber)
        Me.Controls.Add(Me.mskSocialSecurity)
        Me.Controls.Add(Me.mskIPAddress)
        Me.Controls.Add(Me.mskZipCode)
        Me.Controls.Add(Me.btnValidate)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.rexZipCode)
        Me.Controls.Add(Me.rexEmail)
        Me.Controls.Add(Me.rexIPAddress)
        Me.Controls.Add(Me.rexSocialSecurity)
        Me.Controls.Add(Me.rexPhoneNumber)
        Me.Controls.Add(Me.txtInvalidControls)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MaximizeBox = False
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Text Validation Sample"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtInvalidControls As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents btnValidate As System.Windows.Forms.Button
    Friend WithEvents mskZipCode As System.Windows.Forms.MaskedTextBox
    Friend WithEvents mskIPAddress As System.Windows.Forms.MaskedTextBox
    Friend WithEvents mskSocialSecurity As System.Windows.Forms.MaskedTextBox
    Friend WithEvents mskPhoneNumber As System.Windows.Forms.MaskedTextBox
    Friend WithEvents lblIPAddress As System.Windows.Forms.Label
    Friend WithEvents rexZipCode As ValidateText.ZipCodeTextBox
    Friend WithEvents rexEmail As ValidateText.EMailTextBox
    Friend WithEvents rexIPAddress As ValidateText.IPAddressTextBox
    Friend WithEvents rexSocialSecurity As ValidateText.SsnTextbox
    Friend WithEvents rexPhoneNumber As ValidateText.PhoneTextBox
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
