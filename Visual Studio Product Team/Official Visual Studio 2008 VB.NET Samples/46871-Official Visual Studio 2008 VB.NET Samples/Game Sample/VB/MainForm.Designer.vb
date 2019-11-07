' Copyright (c) Microsoft Corporation. All rights reserved.
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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.tenthousands = New System.Windows.Forms.PictureBox
        Me.MenuItem9 = New System.Windows.Forms.MenuItem
        Me.thousands = New System.Windows.Forms.PictureBox
        Me.hundreds = New System.Windows.Forms.PictureBox
        Me.ones = New System.Windows.Forms.PictureBox
        Me.MenuItem8 = New System.Windows.Forms.MenuItem
        Me.numbers = New System.Windows.Forms.ImageList(Me.components)
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.tens = New System.Windows.Forms.PictureBox
        Me.options = New System.Windows.Forms.PictureBox
        Me.newGame = New System.Windows.Forms.PictureBox
        Me.exitGame = New System.Windows.Forms.PictureBox
        Me.MenuItem7 = New System.Windows.Forms.MenuItem
        Me.MenuItem1 = New System.Windows.Forms.MenuItem
        Me.newGameMenu = New System.Windows.Forms.MenuItem
        Me.pauseMenu = New System.Windows.Forms.MenuItem
        Me.restartMenu = New System.Windows.Forms.MenuItem
        Me.optionsMenu = New System.Windows.Forms.MenuItem
        Me.exitMenu = New System.Windows.Forms.MenuItem
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.MainMenu1 = New System.Windows.Forms.MainMenu(Me.components)
        Me.MenuItem4 = New System.Windows.Forms.MenuItem
        Me.MenuItem5 = New System.Windows.Forms.MenuItem
        Me.MenuItem6 = New System.Windows.Forms.MenuItem
        CType(Me.tenthousands, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.thousands, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.hundreds, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        CType(Me.tens, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.options, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.newGame, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.exitGame, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tenthousands
        '
        Me.tenthousands.Location = New System.Drawing.Point(16, 16)
        Me.tenthousands.Name = "tenthousands"
        Me.tenthousands.Size = New System.Drawing.Size(26, 67)
        Me.tenthousands.TabIndex = 3
        Me.tenthousands.TabStop = False
        '
        'MenuItem9
        '
        Me.MenuItem9.Index = 4
        Me.MenuItem9.Text = "&About this game"
        '
        'thousands
        '
        Me.thousands.Location = New System.Drawing.Point(40, 16)
        Me.thousands.Name = "thousands"
        Me.thousands.Size = New System.Drawing.Size(24, 67)
        Me.thousands.TabIndex = 4
        Me.thousands.TabStop = False
        '
        'hundreds
        '
        Me.hundreds.Location = New System.Drawing.Point(64, 16)
        Me.hundreds.Name = "hundreds"
        Me.hundreds.Size = New System.Drawing.Size(24, 67)
        Me.hundreds.TabIndex = 5
        Me.hundreds.TabStop = False
        '
        'ones
        '
        Me.ones.Location = New System.Drawing.Point(112, 16)
        Me.ones.Name = "ones"
        Me.ones.Size = New System.Drawing.Size(24, 67)
        Me.ones.TabIndex = 7
        Me.ones.TabStop = False
        '
        'MenuItem8
        '
        Me.MenuItem8.Index = 3
        Me.MenuItem8.Text = "-"
        '
        'numbers
        '
        Me.numbers.ImageStream = CType(resources.GetObject("numbers.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.numbers.Images.SetKeyName(0, "")
        Me.numbers.Images.SetKeyName(1, "")
        Me.numbers.Images.SetKeyName(2, "")
        Me.numbers.Images.SetKeyName(3, "")
        Me.numbers.Images.SetKeyName(4, "")
        Me.numbers.Images.SetKeyName(5, "")
        Me.numbers.Images.SetKeyName(6, "")
        Me.numbers.Images.SetKeyName(7, "")
        Me.numbers.Images.SetKeyName(8, "")
        Me.numbers.Images.SetKeyName(9, "")
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.tenthousands)
        Me.Panel1.Controls.Add(Me.thousands)
        Me.Panel1.Controls.Add(Me.hundreds)
        Me.Panel1.Controls.Add(Me.ones)
        Me.Panel1.Controls.Add(Me.tens)
        Me.Panel1.Location = New System.Drawing.Point(335, 52)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(144, 96)
        Me.Panel1.TabIndex = 12
        '
        'tens
        '
        Me.tens.Location = New System.Drawing.Point(88, 16)
        Me.tens.Name = "tens"
        Me.tens.Size = New System.Drawing.Size(24, 67)
        Me.tens.TabIndex = 6
        Me.tens.TabStop = False
        '
        'options
        '
        Me.options.Image = CType(resources.GetObject("options.Image"), System.Drawing.Image)
        Me.options.Location = New System.Drawing.Point(327, 276)
        Me.options.Name = "options"
        Me.options.Size = New System.Drawing.Size(128, 50)
        Me.options.TabIndex = 15
        Me.options.TabStop = False
        '
        'newGame
        '
        Me.newGame.Image = CType(resources.GetObject("newGame.Image"), System.Drawing.Image)
        Me.newGame.Location = New System.Drawing.Point(327, 164)
        Me.newGame.Name = "newGame"
        Me.newGame.Size = New System.Drawing.Size(100, 50)
        Me.newGame.TabIndex = 14
        Me.newGame.TabStop = False
        '
        'exitGame
        '
        Me.exitGame.Image = CType(resources.GetObject("exitGame.Image"), System.Drawing.Image)
        Me.exitGame.Location = New System.Drawing.Point(327, 220)
        Me.exitGame.Name = "exitGame"
        Me.exitGame.Size = New System.Drawing.Size(100, 50)
        Me.exitGame.TabIndex = 13
        Me.exitGame.TabStop = False
        '
        'MenuItem7
        '
        Me.MenuItem7.Index = 2
        Me.MenuItem7.Text = "&Search"
        '
        'MenuItem1
        '
        Me.MenuItem1.Index = 0
        Me.MenuItem1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.newGameMenu, Me.pauseMenu, Me.restartMenu, Me.optionsMenu, Me.exitMenu})
        Me.MenuItem1.Text = "&Game"
        '
        'newGameMenu
        '
        Me.newGameMenu.Index = 0
        Me.newGameMenu.Shortcut = System.Windows.Forms.Shortcut.F2
        Me.newGameMenu.Text = "&New Game"
        '
        'pauseMenu
        '
        Me.pauseMenu.Index = 1
        Me.pauseMenu.Shortcut = System.Windows.Forms.Shortcut.F4
        Me.pauseMenu.Text = "&Pause"
        '
        'restartMenu
        '
        Me.restartMenu.Index = 2
        Me.restartMenu.Shortcut = System.Windows.Forms.Shortcut.F5
        Me.restartMenu.Text = "&Restart"
        Me.restartMenu.Visible = False
        '
        'optionsMenu
        '
        Me.optionsMenu.Index = 3
        Me.optionsMenu.Text = "&Options..."
        '
        'exitMenu
        '
        Me.exitMenu.Index = 4
        Me.exitMenu.Text = "E&xit"
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.Black
        Me.PictureBox1.Location = New System.Drawing.Point(7, 4)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(300, 375)
        Me.PictureBox1.TabIndex = 11
        Me.PictureBox1.TabStop = False
        '
        'Timer1
        '
        Me.Timer1.Interval = 7500
        '
        'MainMenu1
        '
        Me.MainMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItem1, Me.MenuItem4})
        '
        'MenuItem4
        '
        Me.MenuItem4.Index = 1
        Me.MenuItem4.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItem5, Me.MenuItem6, Me.MenuItem7, Me.MenuItem8, Me.MenuItem9})
        Me.MenuItem4.Text = "&Help"
        '
        'MenuItem5
        '
        Me.MenuItem5.Index = 0
        Me.MenuItem5.Text = "&Contents"
        '
        'MenuItem6
        '
        Me.MenuItem6.Index = 1
        Me.MenuItem6.Text = "&Index"
        '
        'MainForm
        '
        Me.BackColor = System.Drawing.Color.Cyan
        Me.ClientSize = New System.Drawing.Size(486, 382)
        Me.Controls.Add(Me.exitGame)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.options)
        Me.Controls.Add(Me.newGame)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Menu = Me.MainMenu1
        Me.Name = "MainForm"
        Me.Text = "Game Application Sample"
        CType(Me.tenthousands, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.thousands, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.hundreds, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        CType(Me.tens, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.options, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.newGame, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.exitGame, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tenthousands As System.Windows.Forms.PictureBox
    Friend WithEvents MenuItem9 As System.Windows.Forms.MenuItem
    Friend WithEvents thousands As System.Windows.Forms.PictureBox
    Friend WithEvents hundreds As System.Windows.Forms.PictureBox
    Friend WithEvents ones As System.Windows.Forms.PictureBox
    Friend WithEvents MenuItem8 As System.Windows.Forms.MenuItem
    Friend WithEvents numbers As System.Windows.Forms.ImageList
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents tens As System.Windows.Forms.PictureBox
    Friend WithEvents options As System.Windows.Forms.PictureBox
    Friend WithEvents newGame As System.Windows.Forms.PictureBox
    Friend WithEvents exitGame As System.Windows.Forms.PictureBox
    Friend WithEvents MenuItem7 As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
    Friend WithEvents newGameMenu As System.Windows.Forms.MenuItem
    Friend WithEvents pauseMenu As System.Windows.Forms.MenuItem
    Friend WithEvents restartMenu As System.Windows.Forms.MenuItem
    Friend WithEvents optionsMenu As System.Windows.Forms.MenuItem
    Friend WithEvents exitMenu As System.Windows.Forms.MenuItem
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents MainMenu1 As System.Windows.Forms.MainMenu
    Friend WithEvents MenuItem4 As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem5 As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem6 As System.Windows.Forms.MenuItem

End Class
