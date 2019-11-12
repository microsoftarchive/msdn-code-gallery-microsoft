
Partial Friend Class MainForm
    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.lbStartpoint = New System.Windows.Forms.Label()
        Me.tbStartpoint = New System.Windows.Forms.TextBox()
        Me.lbBeginMillisecs = New System.Windows.Forms.Label()
        Me.btnSetBeginEndPoints = New System.Windows.Forms.Button()
        Me.lbEndpoint = New System.Windows.Forms.Label()
        Me.tbEndpoint = New System.Windows.Forms.TextBox()
        Me.lbEndMillisecs = New System.Windows.Forms.Label()
        Me.btnExtract = New System.Windows.Forms.Button()
        Me.cmbOutputAudioType = New System.Windows.Forms.ComboBox()
        Me.lbOutputFileType = New System.Windows.Forms.Label()
        Me.lbOutputDirectory = New System.Windows.Forms.Label()
        Me.tbOutputDirectory = New System.Windows.Forms.TextBox()
        Me.btnChooseOutputDirectory = New System.Windows.Forms.Button()
        Me.player = New AxWMPLib.AxWindowsMediaPlayer()
        Me.btnChooseSourceFile = New System.Windows.Forms.Button()
        Me.outputFolderBrowserDialog = New System.Windows.Forms.FolderBrowserDialog()
        Me.openAudioFileDialog = New System.Windows.Forms.OpenFileDialog()
        CType(Me.player, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lbStartpoint
        '
        Me.lbStartpoint.AutoSize = True
        Me.lbStartpoint.Location = New System.Drawing.Point(8, 128)
        Me.lbStartpoint.Name = "lbStartpoint"
        Me.lbStartpoint.Size = New System.Drawing.Size(55, 13)
        Me.lbStartpoint.TabIndex = 10
        Me.lbStartpoint.Text = "Startpoint:"
        '
        'tbStartpoint
        '
        Me.tbStartpoint.Enabled = False
        Me.tbStartpoint.Location = New System.Drawing.Point(68, 125)
        Me.tbStartpoint.Name = "tbStartpoint"
        Me.tbStartpoint.ReadOnly = True
        Me.tbStartpoint.Size = New System.Drawing.Size(100, 20)
        Me.tbStartpoint.TabIndex = 11
        '
        'lbBeginMillisecs
        '
        Me.lbBeginMillisecs.AutoSize = True
        Me.lbBeginMillisecs.Location = New System.Drawing.Point(174, 128)
        Me.lbBeginMillisecs.Name = "lbBeginMillisecs"
        Me.lbBeginMillisecs.Size = New System.Drawing.Size(63, 13)
        Me.lbBeginMillisecs.TabIndex = 12
        Me.lbBeginMillisecs.Text = "milliseconds"
        '
        'btnSetBeginEndPoints
        '
        Me.btnSetBeginEndPoints.Location = New System.Drawing.Point(12, 94)
        Me.btnSetBeginEndPoints.Name = "btnSetBeginEndPoints"
        Me.btnSetBeginEndPoints.Size = New System.Drawing.Size(129, 23)
        Me.btnSetBeginEndPoints.TabIndex = 13
        Me.btnSetBeginEndPoints.Tag = "SetStartPoint"
        Me.btnSetBeginEndPoints.Text = "Set Start Point"
        Me.btnSetBeginEndPoints.UseVisualStyleBackColor = True
        '
        'lbEndpoint
        '
        Me.lbEndpoint.AutoSize = True
        Me.lbEndpoint.Location = New System.Drawing.Point(8, 155)
        Me.lbEndpoint.Name = "lbEndpoint"
        Me.lbEndpoint.Size = New System.Drawing.Size(52, 13)
        Me.lbEndpoint.TabIndex = 14
        Me.lbEndpoint.Text = "Endpoint:"
        '
        'tbEndpoint
        '
        Me.tbEndpoint.Enabled = False
        Me.tbEndpoint.Location = New System.Drawing.Point(68, 152)
        Me.tbEndpoint.Name = "tbEndpoint"
        Me.tbEndpoint.ReadOnly = True
        Me.tbEndpoint.Size = New System.Drawing.Size(100, 20)
        Me.tbEndpoint.TabIndex = 15
        '
        'lbEndMillisecs
        '
        Me.lbEndMillisecs.AutoSize = True
        Me.lbEndMillisecs.Location = New System.Drawing.Point(174, 155)
        Me.lbEndMillisecs.Name = "lbEndMillisecs"
        Me.lbEndMillisecs.Size = New System.Drawing.Size(63, 13)
        Me.lbEndMillisecs.TabIndex = 16
        Me.lbEndMillisecs.Text = "milliseconds"
        '
        'btnExtract
        '
        Me.btnExtract.Location = New System.Drawing.Point(12, 244)
        Me.btnExtract.Name = "btnExtract"
        Me.btnExtract.Size = New System.Drawing.Size(75, 23)
        Me.btnExtract.TabIndex = 17
        Me.btnExtract.Text = "Extract"
        Me.btnExtract.UseVisualStyleBackColor = True
        '
        'cmbOutputAudioType
        '
        Me.cmbOutputAudioType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbOutputAudioType.FormattingEnabled = True
        Me.cmbOutputAudioType.Location = New System.Drawing.Point(142, 184)
        Me.cmbOutputAudioType.Name = "cmbOutputAudioType"
        Me.cmbOutputAudioType.Size = New System.Drawing.Size(121, 21)
        Me.cmbOutputAudioType.TabIndex = 22
        '
        'lbOutputFileType
        '
        Me.lbOutputFileType.AutoSize = True
        Me.lbOutputFileType.Location = New System.Drawing.Point(9, 187)
        Me.lbOutputFileType.Name = "lbOutputFileType"
        Me.lbOutputFileType.Size = New System.Drawing.Size(88, 13)
        Me.lbOutputFileType.TabIndex = 23
        Me.lbOutputFileType.Text = "Output File Type:"
        '
        'lbOutputDirectory
        '
        Me.lbOutputDirectory.AutoSize = True
        Me.lbOutputDirectory.Location = New System.Drawing.Point(9, 213)
        Me.lbOutputDirectory.Name = "lbOutputDirectory"
        Me.lbOutputDirectory.Size = New System.Drawing.Size(87, 13)
        Me.lbOutputDirectory.TabIndex = 24
        Me.lbOutputDirectory.Text = "Output Directory:"
        '
        'tbOutputDirectory
        '
        Me.tbOutputDirectory.Location = New System.Drawing.Point(142, 213)
        Me.tbOutputDirectory.Name = "tbOutputDirectory"
        Me.tbOutputDirectory.ReadOnly = True
        Me.tbOutputDirectory.Size = New System.Drawing.Size(340, 20)
        Me.tbOutputDirectory.TabIndex = 25
        '
        'btnChooseOutputDirectory
        '
        Me.btnChooseOutputDirectory.Location = New System.Drawing.Point(488, 211)
        Me.btnChooseOutputDirectory.Name = "btnChooseOutputDirectory"
        Me.btnChooseOutputDirectory.Size = New System.Drawing.Size(77, 23)
        Me.btnChooseOutputDirectory.TabIndex = 26
        Me.btnChooseOutputDirectory.Text = "Browse..."
        Me.btnChooseOutputDirectory.UseVisualStyleBackColor = True
        '
        'player
        '
        Me.player.Enabled = True
        Me.player.Location = New System.Drawing.Point(12, 41)
        Me.player.Name = "player"
        Me.player.OcxState = CType(resources.GetObject("player.OcxState"), System.Windows.Forms.AxHost.State)
        Me.player.Size = New System.Drawing.Size(612, 45)
        Me.player.TabIndex = 28
        '
        'btnChooseSourceFile
        '
        Me.btnChooseSourceFile.Location = New System.Drawing.Point(11, 12)
        Me.btnChooseSourceFile.Name = "btnChooseSourceFile"
        Me.btnChooseSourceFile.Size = New System.Drawing.Size(613, 23)
        Me.btnChooseSourceFile.TabIndex = 27
        Me.btnChooseSourceFile.Text = "Choose Audio File..."
        Me.btnChooseSourceFile.UseVisualStyleBackColor = True
        '
        'openAudioFileDialog
        '
        Me.openAudioFileDialog.Filter = "Audio Files (*.wav;*.mp3;*.mp4)|*.wav;*.mp3;*.mp4"
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(636, 279)
        Me.Controls.Add(Me.player)
        Me.Controls.Add(Me.btnChooseSourceFile)
        Me.Controls.Add(Me.btnChooseOutputDirectory)
        Me.Controls.Add(Me.tbOutputDirectory)
        Me.Controls.Add(Me.lbOutputDirectory)
        Me.Controls.Add(Me.lbOutputFileType)
        Me.Controls.Add(Me.cmbOutputAudioType)
        Me.Controls.Add(Me.btnExtract)
        Me.Controls.Add(Me.lbEndMillisecs)
        Me.Controls.Add(Me.tbEndpoint)
        Me.Controls.Add(Me.lbEndpoint)
        Me.Controls.Add(Me.btnSetBeginEndPoints)
        Me.Controls.Add(Me.lbBeginMillisecs)
        Me.Controls.Add(Me.tbStartpoint)
        Me.Controls.Add(Me.lbStartpoint)
        Me.Name = "MainForm"
        Me.Text = "VBExtractAudioFile"
        CType(Me.player, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private lbStartpoint As Label
    Private tbStartpoint As TextBox
    Private lbBeginMillisecs As Label
    Private WithEvents btnSetBeginEndPoints As Button
    Private lbEndpoint As Label
    Private tbEndpoint As TextBox
    Private lbEndMillisecs As Label
    Private WithEvents btnExtract As Button
    Private cmbOutputAudioType As ComboBox
    Private lbOutputFileType As Label
    Private lbOutputDirectory As Label
    Private tbOutputDirectory As TextBox
    Private WithEvents btnChooseOutputDirectory As Button
    Private player As AxWMPLib.AxWindowsMediaPlayer
    Private WithEvents btnChooseSourceFile As Button
    Private outputFolderBrowserDialog As FolderBrowserDialog
    Private openAudioFileDialog As OpenFileDialog
End Class


