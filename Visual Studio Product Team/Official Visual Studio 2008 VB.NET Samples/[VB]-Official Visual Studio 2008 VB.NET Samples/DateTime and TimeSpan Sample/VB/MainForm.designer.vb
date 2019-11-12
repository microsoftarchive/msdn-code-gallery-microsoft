Partial Public Class MainForm
    Inherits System.Windows.Forms.Form

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents UtcNowLabel As System.Windows.Forms.Label
    Friend WithEvents MinValue As System.Windows.Forms.Label
    Friend WithEvents MaxValue As System.Windows.Forms.Label
    Friend WithEvents FromOADateLabel As System.Windows.Forms.Label
    Friend WithEvents IsLeapYearLabel As System.Windows.Forms.Label
    Friend WithEvents DaysInMonthLable As System.Windows.Forms.Label
    Friend WithEvents MonthDaysInMonth As System.Windows.Forms.TextBox
    Friend WithEvents FromOADate As System.Windows.Forms.TextBox
    Friend WithEvents IsLeapYear As System.Windows.Forms.TextBox
    Friend WithEvents YearDaysInMonth As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents NowLabel As System.Windows.Forms.Label
    Friend WithEvents TodayLabel As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents RefreshShared As System.Windows.Forms.Button
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage5 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage6 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage7 As System.Windows.Forms.TabPage
    Friend WithEvents AddYears As System.Windows.Forms.TextBox
    Friend WithEvents AddTicks As System.Windows.Forms.TextBox
    Friend WithEvents AddSeconds As System.Windows.Forms.TextBox
    Friend WithEvents AddMinutes As System.Windows.Forms.TextBox
    Friend WithEvents AddMilliseconds As System.Windows.Forms.TextBox
    Friend WithEvents AddHours As System.Windows.Forms.TextBox
    Friend WithEvents AddDays As System.Windows.Forms.TextBox
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents RefreshCalculation As System.Windows.Forms.Button
    Friend WithEvents AddTicksLabel As System.Windows.Forms.Label
    Friend WithEvents AddYearsLabel As System.Windows.Forms.Label
    Friend WithEvents AddSecondsLabel As System.Windows.Forms.Label
    Friend WithEvents AddMinutesLabel As System.Windows.Forms.Label
    Friend WithEvents AddMillisecondsLabel As System.Windows.Forms.Label
    Friend WithEvents AddHoursLabel As System.Windows.Forms.Label
    Friend WithEvents AddDaysLabel As System.Windows.Forms.Label
    Friend WithEvents lblNow3 As System.Windows.Forms.Label
    Friend WithEvents AddMonthsLabel As System.Windows.Forms.Label
    Friend WithEvents AddMonths As System.Windows.Forms.TextBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents RefreshProperties As System.Windows.Forms.Button
    Friend WithEvents Hour As System.Windows.Forms.Label
    Friend WithEvents DayOfYear As System.Windows.Forms.Label
    Friend WithEvents DayOfWeek As System.Windows.Forms.Label
    Friend WithEvents Day As System.Windows.Forms.Label
    Friend WithEvents DateLabel As System.Windows.Forms.Label
    Friend WithEvents NowPropertyLabel As System.Windows.Forms.Label
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents Year As System.Windows.Forms.Label
    Friend WithEvents TimeOfDay As System.Windows.Forms.Label
    Friend WithEvents Ticks As System.Windows.Forms.Label
    Friend WithEvents Second As System.Windows.Forms.Label
    Friend WithEvents Month As System.Windows.Forms.Label
    Friend WithEvents Minute As System.Windows.Forms.Label
    Friend WithEvents Millisecond As System.Windows.Forms.Label
    Friend WithEvents Label30 As System.Windows.Forms.Label
    Friend WithEvents Label29 As System.Windows.Forms.Label
    Friend WithEvents Label28 As System.Windows.Forms.Label
    Friend WithEvents Label27 As System.Windows.Forms.Label
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents ToOADate As System.Windows.Forms.Label
    Friend WithEvents ToShortDateString As System.Windows.Forms.Label
    Friend WithEvents ToShortTimeString As System.Windows.Forms.Label
    Friend WithEvents ToStringLabel As System.Windows.Forms.Label
    Friend WithEvents ToUniversalTime As System.Windows.Forms.Label
    Friend WithEvents Label40 As System.Windows.Forms.Label
    Friend WithEvents Label39 As System.Windows.Forms.Label
    Friend WithEvents Label38 As System.Windows.Forms.Label
    Friend WithEvents Label37 As System.Windows.Forms.Label
    Friend WithEvents Label36 As System.Windows.Forms.Label
    Friend WithEvents ToLongTimeString As System.Windows.Forms.Label
    Friend WithEvents ToLongDateString As System.Windows.Forms.Label
    Friend WithEvents ToLocalTime As System.Windows.Forms.Label
    Friend WithEvents ToFileTime As System.Windows.Forms.Label
    Friend WithEvents Now2Label As System.Windows.Forms.Label
    Friend WithEvents Label35 As System.Windows.Forms.Label
    Friend WithEvents Label34 As System.Windows.Forms.Label
    Friend WithEvents Label33 As System.Windows.Forms.Label
    Friend WithEvents Label32 As System.Windows.Forms.Label
    Friend WithEvents Label31 As System.Windows.Forms.Label
    Friend WithEvents RefreshConversion As System.Windows.Forms.Button
    Friend WithEvents tabTimeSpan As System.Windows.Forms.TabControl
    Friend WithEvents TabPage8 As System.Windows.Forms.TabPage
    Friend WithEvents Label41 As System.Windows.Forms.Label
    Friend WithEvents Label42 As System.Windows.Forms.Label
    Friend WithEvents TabPage9 As System.Windows.Forms.TabPage
    Friend WithEvents Label69 As System.Windows.Forms.Label
    Friend WithEvents Label68 As System.Windows.Forms.Label
    Friend WithEvents Label67 As System.Windows.Forms.Label
    Friend WithEvents Label66 As System.Windows.Forms.Label
    Friend WithEvents Label65 As System.Windows.Forms.Label
    Friend WithEvents Label63 As System.Windows.Forms.Label
    Friend WithEvents Label62 As System.Windows.Forms.Label
    Friend WithEvents Label61 As System.Windows.Forms.Label
    Friend WithEvents Label60 As System.Windows.Forms.Label
    Friend WithEvents Label59 As System.Windows.Forms.Label
    Friend WithEvents Label57 As System.Windows.Forms.Label
    Friend WithEvents Label56 As System.Windows.Forms.Label
    Friend WithEvents Label55 As System.Windows.Forms.Label
    Friend WithEvents Label54 As System.Windows.Forms.Label
    Friend WithEvents Label53 As System.Windows.Forms.Label
    Friend WithEvents Label52 As System.Windows.Forms.Label
    Friend WithEvents Label50 As System.Windows.Forms.Label
    Friend WithEvents Label49 As System.Windows.Forms.Label
    Friend WithEvents Label48 As System.Windows.Forms.Label
    Friend WithEvents Label47 As System.Windows.Forms.Label
    Friend WithEvents Label46 As System.Windows.Forms.Label
    Friend WithEvents Label45 As System.Windows.Forms.Label
    Friend WithEvents RefreshTSProperties As System.Windows.Forms.Button
    Friend WithEvents EndTime As System.Windows.Forms.TextBox
    Friend WithEvents StartTime As System.Windows.Forms.TextBox
    Friend WithEvents btnCalcParse As System.Windows.Forms.Button
    Friend WithEvents txtParse As System.Windows.Forms.TextBox
    Friend WithEvents Label43 As System.Windows.Forms.Label
    Friend WithEvents Hours As System.Windows.Forms.Label
    Friend WithEvents Seconds As System.Windows.Forms.Label
    Friend WithEvents Minutes As System.Windows.Forms.Label
    Friend WithEvents Milliseconds As System.Windows.Forms.Label
    Friend WithEvents Days As System.Windows.Forms.Label
    Friend WithEvents TotalSeconds As System.Windows.Forms.Label
    Friend WithEvents TotalMinutes As System.Windows.Forms.Label
    Friend WithEvents TotalMilliseconds As System.Windows.Forms.Label
    Friend WithEvents TotalHours As System.Windows.Forms.Label
    Friend WithEvents TotalDays As System.Windows.Forms.Label
    Friend WithEvents TimeSpanTicks As System.Windows.Forms.Label
    Friend WithEvents Label71 As System.Windows.Forms.Label
    Friend WithEvents Label70 As System.Windows.Forms.Label
    Friend WithEvents Label64 As System.Windows.Forms.Label
    Friend WithEvents Label58 As System.Windows.Forms.Label
    Friend WithEvents Label51 As System.Windows.Forms.Label
    Friend WithEvents Label44 As System.Windows.Forms.Label
    Friend WithEvents RefreshTSMethods As System.Windows.Forms.Button
    Friend WithEvents FromTicks As System.Windows.Forms.TextBox
    Friend WithEvents FromSeconds As System.Windows.Forms.TextBox
    Friend WithEvents FromMinutes As System.Windows.Forms.TextBox
    Friend WithEvents FromMilliseconds As System.Windows.Forms.TextBox
    Friend WithEvents FromDays As System.Windows.Forms.TextBox
    Friend WithEvents FromHours As System.Windows.Forms.TextBox
    Friend WithEvents FromTicksLabel As System.Windows.Forms.Label
    Friend WithEvents FromSecondsLabel As System.Windows.Forms.Label
    Friend WithEvents FromMinutesLabel As System.Windows.Forms.Label
    Friend WithEvents FromMillisecondsLabel As System.Windows.Forms.Label
    Friend WithEvents FromHoursLabel As System.Windows.Forms.Label
    Friend WithEvents FromDaysLabel As System.Windows.Forms.Label
    Friend WithEvents Zero As System.Windows.Forms.Label
    Friend WithEvents TicksPerSecond As System.Windows.Forms.Label
    Friend WithEvents TicksPerMinute As System.Windows.Forms.Label
    Friend WithEvents TicksPerMillisecond As System.Windows.Forms.Label
    Friend WithEvents TicksPerHour As System.Windows.Forms.Label
    Friend WithEvents TicksPerDay As System.Windows.Forms.Label
    Friend WithEvents MinValueTS As System.Windows.Forms.Label
    Friend WithEvents MaxValueTS As System.Windows.Forms.Label
    Friend WithEvents Label79 As System.Windows.Forms.Label
    Friend WithEvents Label78 As System.Windows.Forms.Label
    Friend WithEvents Label77 As System.Windows.Forms.Label
    Friend WithEvents Label76 As System.Windows.Forms.Label
    Friend WithEvents Label75 As System.Windows.Forms.Label
    Friend WithEvents Label74 As System.Windows.Forms.Label
    Friend WithEvents Label73 As System.Windows.Forms.Label
    Friend WithEvents Label72 As System.Windows.Forms.Label

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.TabControl1 = New System.Windows.Forms.TabControl
        Me.TabPage1 = New System.Windows.Forms.TabPage
        Me.RefreshShared = New System.Windows.Forms.Button
        Me.UtcNowLabel = New System.Windows.Forms.Label
        Me.MinValue = New System.Windows.Forms.Label
        Me.MaxValue = New System.Windows.Forms.Label
        Me.FromOADateLabel = New System.Windows.Forms.Label
        Me.IsLeapYearLabel = New System.Windows.Forms.Label
        Me.DaysInMonthLable = New System.Windows.Forms.Label
        Me.MonthDaysInMonth = New System.Windows.Forms.TextBox
        Me.FromOADate = New System.Windows.Forms.TextBox
        Me.IsLeapYear = New System.Windows.Forms.TextBox
        Me.YearDaysInMonth = New System.Windows.Forms.TextBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.NowLabel = New System.Windows.Forms.Label
        Me.TodayLabel = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.TabPage2 = New System.Windows.Forms.TabPage
        Me.RefreshCalculation = New System.Windows.Forms.Button
        Me.AddMonthsLabel = New System.Windows.Forms.Label
        Me.AddMonths = New System.Windows.Forms.TextBox
        Me.Label17 = New System.Windows.Forms.Label
        Me.Label14 = New System.Windows.Forms.Label
        Me.Label15 = New System.Windows.Forms.Label
        Me.Label16 = New System.Windows.Forms.Label
        Me.AddYears = New System.Windows.Forms.TextBox
        Me.AddTicks = New System.Windows.Forms.TextBox
        Me.AddSeconds = New System.Windows.Forms.TextBox
        Me.AddSecondsLabel = New System.Windows.Forms.Label
        Me.AddTicksLabel = New System.Windows.Forms.Label
        Me.AddYearsLabel = New System.Windows.Forms.Label
        Me.AddMinutesLabel = New System.Windows.Forms.Label
        Me.AddMillisecondsLabel = New System.Windows.Forms.Label
        Me.AddHoursLabel = New System.Windows.Forms.Label
        Me.AddDaysLabel = New System.Windows.Forms.Label
        Me.lblNow3 = New System.Windows.Forms.Label
        Me.AddDays = New System.Windows.Forms.TextBox
        Me.AddHours = New System.Windows.Forms.TextBox
        Me.AddMilliseconds = New System.Windows.Forms.TextBox
        Me.AddMinutes = New System.Windows.Forms.TextBox
        Me.Label13 = New System.Windows.Forms.Label
        Me.Label12 = New System.Windows.Forms.Label
        Me.Label11 = New System.Windows.Forms.Label
        Me.Label10 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.TabPage3 = New System.Windows.Forms.TabPage
        Me.RefreshProperties = New System.Windows.Forms.Button
        Me.Year = New System.Windows.Forms.Label
        Me.TimeOfDay = New System.Windows.Forms.Label
        Me.Ticks = New System.Windows.Forms.Label
        Me.Second = New System.Windows.Forms.Label
        Me.Month = New System.Windows.Forms.Label
        Me.Minute = New System.Windows.Forms.Label
        Me.Millisecond = New System.Windows.Forms.Label
        Me.Label30 = New System.Windows.Forms.Label
        Me.Label29 = New System.Windows.Forms.Label
        Me.Label28 = New System.Windows.Forms.Label
        Me.Label27 = New System.Windows.Forms.Label
        Me.Label26 = New System.Windows.Forms.Label
        Me.Label25 = New System.Windows.Forms.Label
        Me.Label24 = New System.Windows.Forms.Label
        Me.Hour = New System.Windows.Forms.Label
        Me.DayOfYear = New System.Windows.Forms.Label
        Me.DayOfWeek = New System.Windows.Forms.Label
        Me.Day = New System.Windows.Forms.Label
        Me.DateLabel = New System.Windows.Forms.Label
        Me.NowPropertyLabel = New System.Windows.Forms.Label
        Me.Label23 = New System.Windows.Forms.Label
        Me.Label22 = New System.Windows.Forms.Label
        Me.Label21 = New System.Windows.Forms.Label
        Me.Label20 = New System.Windows.Forms.Label
        Me.Label19 = New System.Windows.Forms.Label
        Me.Label18 = New System.Windows.Forms.Label
        Me.TabPage4 = New System.Windows.Forms.TabPage
        Me.ToOADate = New System.Windows.Forms.Label
        Me.ToShortDateString = New System.Windows.Forms.Label
        Me.ToShortTimeString = New System.Windows.Forms.Label
        Me.ToStringLabel = New System.Windows.Forms.Label
        Me.ToUniversalTime = New System.Windows.Forms.Label
        Me.Label40 = New System.Windows.Forms.Label
        Me.Label39 = New System.Windows.Forms.Label
        Me.Label38 = New System.Windows.Forms.Label
        Me.Label37 = New System.Windows.Forms.Label
        Me.Label36 = New System.Windows.Forms.Label
        Me.ToLongTimeString = New System.Windows.Forms.Label
        Me.ToLongDateString = New System.Windows.Forms.Label
        Me.ToLocalTime = New System.Windows.Forms.Label
        Me.ToFileTime = New System.Windows.Forms.Label
        Me.Now2Label = New System.Windows.Forms.Label
        Me.Label35 = New System.Windows.Forms.Label
        Me.Label34 = New System.Windows.Forms.Label
        Me.Label33 = New System.Windows.Forms.Label
        Me.Label32 = New System.Windows.Forms.Label
        Me.Label31 = New System.Windows.Forms.Label
        Me.RefreshConversion = New System.Windows.Forms.Button
        Me.TabPage5 = New System.Windows.Forms.TabPage
        Me.TotalSeconds = New System.Windows.Forms.Label
        Me.TotalMinutes = New System.Windows.Forms.Label
        Me.TotalMilliseconds = New System.Windows.Forms.Label
        Me.TotalHours = New System.Windows.Forms.Label
        Me.TotalDays = New System.Windows.Forms.Label
        Me.TimeSpanTicks = New System.Windows.Forms.Label
        Me.Seconds = New System.Windows.Forms.Label
        Me.Minutes = New System.Windows.Forms.Label
        Me.Milliseconds = New System.Windows.Forms.Label
        Me.Hours = New System.Windows.Forms.Label
        Me.Days = New System.Windows.Forms.Label
        Me.Label69 = New System.Windows.Forms.Label
        Me.Label68 = New System.Windows.Forms.Label
        Me.Label67 = New System.Windows.Forms.Label
        Me.Label66 = New System.Windows.Forms.Label
        Me.Label65 = New System.Windows.Forms.Label
        Me.Label57 = New System.Windows.Forms.Label
        Me.Label56 = New System.Windows.Forms.Label
        Me.Label55 = New System.Windows.Forms.Label
        Me.Label54 = New System.Windows.Forms.Label
        Me.Label53 = New System.Windows.Forms.Label
        Me.Label52 = New System.Windows.Forms.Label
        Me.tabTimeSpan = New System.Windows.Forms.TabControl
        Me.TabPage8 = New System.Windows.Forms.TabPage
        Me.RefreshTSProperties = New System.Windows.Forms.Button
        Me.EndTime = New System.Windows.Forms.TextBox
        Me.StartTime = New System.Windows.Forms.TextBox
        Me.Label42 = New System.Windows.Forms.Label
        Me.Label41 = New System.Windows.Forms.Label
        Me.TabPage9 = New System.Windows.Forms.TabPage
        Me.btnCalcParse = New System.Windows.Forms.Button
        Me.txtParse = New System.Windows.Forms.TextBox
        Me.Label43 = New System.Windows.Forms.Label
        Me.TabPage6 = New System.Windows.Forms.TabPage
        Me.FromTicksLabel = New System.Windows.Forms.Label
        Me.FromSecondsLabel = New System.Windows.Forms.Label
        Me.FromMinutesLabel = New System.Windows.Forms.Label
        Me.FromMillisecondsLabel = New System.Windows.Forms.Label
        Me.FromHoursLabel = New System.Windows.Forms.Label
        Me.FromDaysLabel = New System.Windows.Forms.Label
        Me.FromTicks = New System.Windows.Forms.TextBox
        Me.FromSeconds = New System.Windows.Forms.TextBox
        Me.FromMinutes = New System.Windows.Forms.TextBox
        Me.FromMilliseconds = New System.Windows.Forms.TextBox
        Me.FromDays = New System.Windows.Forms.TextBox
        Me.FromHours = New System.Windows.Forms.TextBox
        Me.Label71 = New System.Windows.Forms.Label
        Me.Label70 = New System.Windows.Forms.Label
        Me.Label64 = New System.Windows.Forms.Label
        Me.Label58 = New System.Windows.Forms.Label
        Me.Label51 = New System.Windows.Forms.Label
        Me.Label44 = New System.Windows.Forms.Label
        Me.RefreshTSMethods = New System.Windows.Forms.Button
        Me.TabPage7 = New System.Windows.Forms.TabPage
        Me.Zero = New System.Windows.Forms.Label
        Me.TicksPerSecond = New System.Windows.Forms.Label
        Me.TicksPerMinute = New System.Windows.Forms.Label
        Me.TicksPerMillisecond = New System.Windows.Forms.Label
        Me.TicksPerHour = New System.Windows.Forms.Label
        Me.TicksPerDay = New System.Windows.Forms.Label
        Me.MinValueTS = New System.Windows.Forms.Label
        Me.MaxValueTS = New System.Windows.Forms.Label
        Me.Label79 = New System.Windows.Forms.Label
        Me.Label78 = New System.Windows.Forms.Label
        Me.Label77 = New System.Windows.Forms.Label
        Me.Label76 = New System.Windows.Forms.Label
        Me.Label75 = New System.Windows.Forms.Label
        Me.Label74 = New System.Windows.Forms.Label
        Me.Label73 = New System.Windows.Forms.Label
        Me.Label72 = New System.Windows.Forms.Label
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.TabPage5.SuspendLayout()
        Me.tabTimeSpan.SuspendLayout()
        Me.TabPage8.SuspendLayout()
        Me.TabPage9.SuspendLayout()
        Me.TabPage6.SuspendLayout()
        Me.TabPage7.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Controls.Add(Me.TabPage4)
        Me.TabControl1.Controls.Add(Me.TabPage5)
        Me.TabControl1.Controls.Add(Me.TabPage6)
        Me.TabControl1.Controls.Add(Me.TabPage7)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.TabControl1.Location = New System.Drawing.Point(0, 30)
        Me.TabControl1.Multiline = True
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.ShowToolTips = True
        Me.TabControl1.Size = New System.Drawing.Size(670, 427)
        Me.TabControl1.TabIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.RefreshShared)
        Me.TabPage1.Controls.Add(Me.UtcNowLabel)
        Me.TabPage1.Controls.Add(Me.MinValue)
        Me.TabPage1.Controls.Add(Me.MaxValue)
        Me.TabPage1.Controls.Add(Me.FromOADateLabel)
        Me.TabPage1.Controls.Add(Me.IsLeapYearLabel)
        Me.TabPage1.Controls.Add(Me.DaysInMonthLable)
        Me.TabPage1.Controls.Add(Me.MonthDaysInMonth)
        Me.TabPage1.Controls.Add(Me.FromOADate)
        Me.TabPage1.Controls.Add(Me.IsLeapYear)
        Me.TabPage1.Controls.Add(Me.YearDaysInMonth)
        Me.TabPage1.Controls.Add(Me.Label6)
        Me.TabPage1.Controls.Add(Me.Label7)
        Me.TabPage1.Controls.Add(Me.Label8)
        Me.TabPage1.Controls.Add(Me.NowLabel)
        Me.TabPage1.Controls.Add(Me.TodayLabel)
        Me.TabPage1.Controls.Add(Me.Label5)
        Me.TabPage1.Controls.Add(Me.Label4)
        Me.TabPage1.Controls.Add(Me.Label3)
        Me.TabPage1.Controls.Add(Me.Label2)
        Me.TabPage1.Controls.Add(Me.Label1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 40)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(662, 383)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "DateTime Shared Members"
        '
        'RefreshShared
        '
        Me.RefreshShared.Location = New System.Drawing.Point(34, 10)
        Me.RefreshShared.Name = "RefreshShared"
        Me.RefreshShared.TabIndex = 0
        Me.RefreshShared.Text = "&Refresh"
        '
        'UtcNowLabel
        '
        Me.UtcNowLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.UtcNowLabel.Location = New System.Drawing.Point(142, 111)
        Me.UtcNowLabel.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
        Me.UtcNowLabel.Name = "UtcNowLabel"
        Me.UtcNowLabel.Size = New System.Drawing.Size(197, 27)
        Me.UtcNowLabel.TabIndex = 8
        Me.UtcNowLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'MinValue
        '
        Me.MinValue.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.MinValue.Location = New System.Drawing.Point(142, 170)
        Me.MinValue.Margin = New System.Windows.Forms.Padding(1, 1, 3, 0)
        Me.MinValue.Name = "MinValue"
        Me.MinValue.Size = New System.Drawing.Size(197, 27)
        Me.MinValue.TabIndex = 9
        Me.MinValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'MaxValue
        '
        Me.MaxValue.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.MaxValue.Location = New System.Drawing.Point(142, 140)
        Me.MaxValue.Margin = New System.Windows.Forms.Padding(1, 1, 3, 1)
        Me.MaxValue.Name = "MaxValue"
        Me.MaxValue.Size = New System.Drawing.Size(197, 27)
        Me.MaxValue.TabIndex = 10
        Me.MaxValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FromOADateLabel
        '
        Me.FromOADateLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.FromOADateLabel.Location = New System.Drawing.Point(295, 285)
        Me.FromOADateLabel.Name = "FromOADateLabel"
        Me.FromOADateLabel.Size = New System.Drawing.Size(191, 23)
        Me.FromOADateLabel.TabIndex = 20
        Me.FromOADateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'IsLeapYearLabel
        '
        Me.IsLeapYearLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.IsLeapYearLabel.Location = New System.Drawing.Point(295, 256)
        Me.IsLeapYearLabel.Margin = New System.Windows.Forms.Padding(3, 1, 3, 3)
        Me.IsLeapYearLabel.Name = "IsLeapYearLabel"
        Me.IsLeapYearLabel.Size = New System.Drawing.Size(191, 23)
        Me.IsLeapYearLabel.TabIndex = 19
        Me.IsLeapYearLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'DaysInMonthLable
        '
        Me.DaysInMonthLable.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.DaysInMonthLable.Location = New System.Drawing.Point(295, 228)
        Me.DaysInMonthLable.Margin = New System.Windows.Forms.Padding(3, 3, 3, 2)
        Me.DaysInMonthLable.Name = "DaysInMonthLable"
        Me.DaysInMonthLable.Size = New System.Drawing.Size(191, 23)
        Me.DaysInMonthLable.TabIndex = 18
        Me.DaysInMonthLable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'MonthDaysInMonth
        '
        Me.MonthDaysInMonth.Location = New System.Drawing.Point(249, 228)
        Me.MonthDaysInMonth.Name = "MonthDaysInMonth"
        Me.MonthDaysInMonth.Size = New System.Drawing.Size(39, 20)
        Me.MonthDaysInMonth.TabIndex = 2
        Me.MonthDaysInMonth.Text = "2"
        '
        'FromOADate
        '
        Me.FromOADate.Location = New System.Drawing.Point(142, 285)
        Me.FromOADate.Name = "FromOADate"
        Me.FromOADate.TabIndex = 4
        Me.FromOADate.Text = "36578.325"
        '
        'IsLeapYear
        '
        Me.IsLeapYear.Location = New System.Drawing.Point(142, 256)
        Me.IsLeapYear.Name = "IsLeapYear"
        Me.IsLeapYear.TabIndex = 3
        Me.IsLeapYear.Text = "2004"
        '
        'YearDaysInMonth
        '
        Me.YearDaysInMonth.Location = New System.Drawing.Point(142, 228)
        Me.YearDaysInMonth.Name = "YearDaysInMonth"
        Me.YearDaysInMonth.TabIndex = 1
        Me.YearDaysInMonth.Text = "2004"
        '
        'Label6
        '
        Me.Label6.Location = New System.Drawing.Point(36, 228)
        Me.Label6.Name = "Label6"
        Me.Label6.TabIndex = 11
        Me.Label6.Text = "DaysInMonth"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label7
        '
        Me.Label7.Location = New System.Drawing.Point(36, 256)
        Me.Label7.Name = "Label7"
        Me.Label7.TabIndex = 12
        Me.Label7.Text = "IsLeapYear"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label8
        '
        Me.Label8.Location = New System.Drawing.Point(36, 285)
        Me.Label8.Name = "Label8"
        Me.Label8.TabIndex = 13
        Me.Label8.Text = "FromOADate"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'NowLabel
        '
        Me.NowLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.NowLabel.Location = New System.Drawing.Point(142, 54)
        Me.NowLabel.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.NowLabel.Name = "NowLabel"
        Me.NowLabel.Size = New System.Drawing.Size(197, 27)
        Me.NowLabel.TabIndex = 6
        Me.NowLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TodayLabel
        '
        Me.TodayLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TodayLabel.Location = New System.Drawing.Point(142, 82)
        Me.TodayLabel.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.TodayLabel.Name = "TodayLabel"
        Me.TodayLabel.Size = New System.Drawing.Size(197, 27)
        Me.TodayLabel.TabIndex = 7
        Me.TodayLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label5
        '
        Me.Label5.Location = New System.Drawing.Point(35, 168)
        Me.Label5.Margin = New System.Windows.Forms.Padding(3, 3, 2, 3)
        Me.Label5.Name = "Label5"
        Me.Label5.TabIndex = 5
        Me.Label5.Text = "MaxValue"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label4
        '
        Me.Label4.Location = New System.Drawing.Point(36, 140)
        Me.Label4.Margin = New System.Windows.Forms.Padding(3, 3, 1, 3)
        Me.Label4.Name = "Label4"
        Me.Label4.TabIndex = 4
        Me.Label4.Text = "MinValue"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label3
        '
        Me.Label3.Location = New System.Drawing.Point(36, 112)
        Me.Label3.Margin = New System.Windows.Forms.Padding(3, 2, 3, 3)
        Me.Label3.Name = "Label3"
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "utcNow"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(33, 85)
        Me.Label2.Name = "Label2"
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Today"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(34, 58)
        Me.Label1.Name = "Label1"
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Now"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.RefreshCalculation)
        Me.TabPage2.Controls.Add(Me.AddMonthsLabel)
        Me.TabPage2.Controls.Add(Me.AddMonths)
        Me.TabPage2.Controls.Add(Me.Label17)
        Me.TabPage2.Controls.Add(Me.Label14)
        Me.TabPage2.Controls.Add(Me.Label15)
        Me.TabPage2.Controls.Add(Me.Label16)
        Me.TabPage2.Controls.Add(Me.AddYears)
        Me.TabPage2.Controls.Add(Me.AddTicks)
        Me.TabPage2.Controls.Add(Me.AddSeconds)
        Me.TabPage2.Controls.Add(Me.AddSecondsLabel)
        Me.TabPage2.Controls.Add(Me.AddTicksLabel)
        Me.TabPage2.Controls.Add(Me.AddYearsLabel)
        Me.TabPage2.Controls.Add(Me.AddMinutesLabel)
        Me.TabPage2.Controls.Add(Me.AddMillisecondsLabel)
        Me.TabPage2.Controls.Add(Me.AddHoursLabel)
        Me.TabPage2.Controls.Add(Me.AddDaysLabel)
        Me.TabPage2.Controls.Add(Me.lblNow3)
        Me.TabPage2.Controls.Add(Me.AddDays)
        Me.TabPage2.Controls.Add(Me.AddHours)
        Me.TabPage2.Controls.Add(Me.AddMilliseconds)
        Me.TabPage2.Controls.Add(Me.AddMinutes)
        Me.TabPage2.Controls.Add(Me.Label13)
        Me.TabPage2.Controls.Add(Me.Label12)
        Me.TabPage2.Controls.Add(Me.Label11)
        Me.TabPage2.Controls.Add(Me.Label10)
        Me.TabPage2.Controls.Add(Me.Label9)
        Me.TabPage2.Location = New System.Drawing.Point(4, 40)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(662, 383)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "DateTime Calculation Methods"
        '
        'RefreshCalculation
        '
        Me.RefreshCalculation.Location = New System.Drawing.Point(18, 10)
        Me.RefreshCalculation.Name = "RefreshCalculation"
        Me.RefreshCalculation.TabIndex = 0
        Me.RefreshCalculation.Text = "&Refresh"
        '
        'AddMonthsLabel
        '
        Me.AddMonthsLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.AddMonthsLabel.Location = New System.Drawing.Point(188, 176)
        Me.AddMonthsLabel.Margin = New System.Windows.Forms.Padding(3, 0, 3, 3)
        Me.AddMonthsLabel.Name = "AddMonthsLabel"
        Me.AddMonthsLabel.Size = New System.Drawing.Size(238, 23)
        Me.AddMonthsLabel.TabIndex = 26
        Me.AddMonthsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'AddMonths
        '
        Me.AddMonths.Location = New System.Drawing.Point(127, 178)
        Me.AddMonths.Margin = New System.Windows.Forms.Padding(3, 2, 3, 3)
        Me.AddMonths.Name = "AddMonths"
        Me.AddMonths.Size = New System.Drawing.Size(45, 20)
        Me.AddMonths.TabIndex = 5
        Me.AddMonths.Text = "3"
        '
        'Label17
        '
        Me.Label17.Location = New System.Drawing.Point(18, 176)
        Me.Label17.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
        Me.Label17.Name = "Label17"
        Me.Label17.TabIndex = 24
        Me.Label17.Text = "AddMonths"
        Me.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label14
        '
        Me.Label14.Location = New System.Drawing.Point(16, 200)
        Me.Label14.Margin = New System.Windows.Forms.Padding(3, 1, 3, 0)
        Me.Label14.Name = "Label14"
        Me.Label14.TabIndex = 6
        Me.Label14.Text = "AddSeconds"
        Me.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label15
        '
        Me.Label15.Location = New System.Drawing.Point(16, 224)
        Me.Label15.Margin = New System.Windows.Forms.Padding(3, 1, 3, 1)
        Me.Label15.Name = "Label15"
        Me.Label15.TabIndex = 7
        Me.Label15.Text = "AddTicks"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label16
        '
        Me.Label16.Location = New System.Drawing.Point(16, 248)
        Me.Label16.Margin = New System.Windows.Forms.Padding(3, 0, 3, 3)
        Me.Label16.Name = "Label16"
        Me.Label16.TabIndex = 8
        Me.Label16.Text = "AddYears"
        Me.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'AddYears
        '
        Me.AddYears.Location = New System.Drawing.Point(128, 250)
        Me.AddYears.Margin = New System.Windows.Forms.Padding(3, 2, 3, 3)
        Me.AddYears.Name = "AddYears"
        Me.AddYears.Size = New System.Drawing.Size(44, 20)
        Me.AddYears.TabIndex = 8
        Me.AddYears.Text = "3"
        '
        'AddTicks
        '
        Me.AddTicks.Location = New System.Drawing.Point(128, 226)
        Me.AddTicks.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.AddTicks.Name = "AddTicks"
        Me.AddTicks.Size = New System.Drawing.Size(44, 20)
        Me.AddTicks.TabIndex = 7
        Me.AddTicks.Text = "3"
        '
        'AddSeconds
        '
        Me.AddSeconds.Location = New System.Drawing.Point(128, 202)
        Me.AddSeconds.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.AddSeconds.Name = "AddSeconds"
        Me.AddSeconds.Size = New System.Drawing.Size(44, 20)
        Me.AddSeconds.TabIndex = 6
        Me.AddSeconds.Text = "3"
        '
        'AddSecondsLabel
        '
        Me.AddSecondsLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.AddSecondsLabel.Location = New System.Drawing.Point(188, 200)
        Me.AddSecondsLabel.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
        Me.AddSecondsLabel.Name = "AddSecondsLabel"
        Me.AddSecondsLabel.Size = New System.Drawing.Size(238, 23)
        Me.AddSecondsLabel.TabIndex = 21
        Me.AddSecondsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'AddTicksLabel
        '
        Me.AddTicksLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.AddTicksLabel.Location = New System.Drawing.Point(188, 224)
        Me.AddTicksLabel.Margin = New System.Windows.Forms.Padding(3, 1, 3, 1)
        Me.AddTicksLabel.Name = "AddTicksLabel"
        Me.AddTicksLabel.Size = New System.Drawing.Size(238, 23)
        Me.AddTicksLabel.TabIndex = 22
        Me.AddTicksLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'AddYearsLabel
        '
        Me.AddYearsLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.AddYearsLabel.Location = New System.Drawing.Point(188, 248)
        Me.AddYearsLabel.Margin = New System.Windows.Forms.Padding(3, 0, 3, 3)
        Me.AddYearsLabel.Name = "AddYearsLabel"
        Me.AddYearsLabel.Size = New System.Drawing.Size(238, 23)
        Me.AddYearsLabel.TabIndex = 23
        Me.AddYearsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'AddMinutesLabel
        '
        Me.AddMinutesLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.AddMinutesLabel.Location = New System.Drawing.Point(188, 152)
        Me.AddMinutesLabel.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.AddMinutesLabel.Name = "AddMinutesLabel"
        Me.AddMinutesLabel.Size = New System.Drawing.Size(238, 23)
        Me.AddMinutesLabel.TabIndex = 20
        Me.AddMinutesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'AddMillisecondsLabel
        '
        Me.AddMillisecondsLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.AddMillisecondsLabel.Location = New System.Drawing.Point(188, 128)
        Me.AddMillisecondsLabel.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.AddMillisecondsLabel.Name = "AddMillisecondsLabel"
        Me.AddMillisecondsLabel.Size = New System.Drawing.Size(238, 23)
        Me.AddMillisecondsLabel.TabIndex = 19
        Me.AddMillisecondsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'AddHoursLabel
        '
        Me.AddHoursLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.AddHoursLabel.Location = New System.Drawing.Point(188, 104)
        Me.AddHoursLabel.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.AddHoursLabel.Name = "AddHoursLabel"
        Me.AddHoursLabel.Size = New System.Drawing.Size(238, 23)
        Me.AddHoursLabel.TabIndex = 18
        Me.AddHoursLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'AddDaysLabel
        '
        Me.AddDaysLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.AddDaysLabel.Location = New System.Drawing.Point(188, 80)
        Me.AddDaysLabel.Margin = New System.Windows.Forms.Padding(3, 1, 3, 1)
        Me.AddDaysLabel.Name = "AddDaysLabel"
        Me.AddDaysLabel.Size = New System.Drawing.Size(238, 23)
        Me.AddDaysLabel.TabIndex = 17
        Me.AddDaysLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblNow3
        '
        Me.lblNow3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblNow3.Location = New System.Drawing.Point(188, 56)
        Me.lblNow3.Margin = New System.Windows.Forms.Padding(3, 3, 3, 2)
        Me.lblNow3.Name = "lblNow3"
        Me.lblNow3.Size = New System.Drawing.Size(238, 23)
        Me.lblNow3.TabIndex = 16
        Me.lblNow3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'AddDays
        '
        Me.AddDays.Location = New System.Drawing.Point(128, 82)
        Me.AddDays.Name = "AddDays"
        Me.AddDays.Size = New System.Drawing.Size(44, 20)
        Me.AddDays.TabIndex = 1
        Me.AddDays.Text = "3"
        '
        'AddHours
        '
        Me.AddHours.Location = New System.Drawing.Point(128, 106)
        Me.AddHours.Name = "AddHours"
        Me.AddHours.Size = New System.Drawing.Size(44, 20)
        Me.AddHours.TabIndex = 2
        Me.AddHours.Text = "3"
        '
        'AddMilliseconds
        '
        Me.AddMilliseconds.Location = New System.Drawing.Point(128, 130)
        Me.AddMilliseconds.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.AddMilliseconds.Name = "AddMilliseconds"
        Me.AddMilliseconds.Size = New System.Drawing.Size(44, 20)
        Me.AddMilliseconds.TabIndex = 3
        Me.AddMilliseconds.Text = "3"
        '
        'AddMinutes
        '
        Me.AddMinutes.Location = New System.Drawing.Point(128, 154)
        Me.AddMinutes.Margin = New System.Windows.Forms.Padding(3, 1, 3, 2)
        Me.AddMinutes.Name = "AddMinutes"
        Me.AddMinutes.Size = New System.Drawing.Size(44, 20)
        Me.AddMinutes.TabIndex = 4
        Me.AddMinutes.Text = "3"
        '
        'Label13
        '
        Me.Label13.Location = New System.Drawing.Point(16, 152)
        Me.Label13.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Label13.Name = "Label13"
        Me.Label13.TabIndex = 5
        Me.Label13.Text = "AddMinutes"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label12
        '
        Me.Label12.Location = New System.Drawing.Point(16, 128)
        Me.Label12.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Label12.Name = "Label12"
        Me.Label12.TabIndex = 4
        Me.Label12.Text = "AddMilliseconds"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label11
        '
        Me.Label11.Location = New System.Drawing.Point(16, 104)
        Me.Label11.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Label11.Name = "Label11"
        Me.Label11.TabIndex = 3
        Me.Label11.Text = "AddHours"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label10
        '
        Me.Label10.Location = New System.Drawing.Point(16, 80)
        Me.Label10.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Label10.Name = "Label10"
        Me.Label10.TabIndex = 2
        Me.Label10.Text = "AddDays"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label9
        '
        Me.Label9.Location = New System.Drawing.Point(16, 56)
        Me.Label9.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.Label9.Name = "Label9"
        Me.Label9.TabIndex = 1
        Me.Label9.Text = "Now"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.RefreshProperties)
        Me.TabPage3.Controls.Add(Me.Year)
        Me.TabPage3.Controls.Add(Me.TimeOfDay)
        Me.TabPage3.Controls.Add(Me.Ticks)
        Me.TabPage3.Controls.Add(Me.Second)
        Me.TabPage3.Controls.Add(Me.Month)
        Me.TabPage3.Controls.Add(Me.Minute)
        Me.TabPage3.Controls.Add(Me.Millisecond)
        Me.TabPage3.Controls.Add(Me.Label30)
        Me.TabPage3.Controls.Add(Me.Label29)
        Me.TabPage3.Controls.Add(Me.Label28)
        Me.TabPage3.Controls.Add(Me.Label27)
        Me.TabPage3.Controls.Add(Me.Label26)
        Me.TabPage3.Controls.Add(Me.Label25)
        Me.TabPage3.Controls.Add(Me.Label24)
        Me.TabPage3.Controls.Add(Me.Hour)
        Me.TabPage3.Controls.Add(Me.DayOfYear)
        Me.TabPage3.Controls.Add(Me.DayOfWeek)
        Me.TabPage3.Controls.Add(Me.Day)
        Me.TabPage3.Controls.Add(Me.DateLabel)
        Me.TabPage3.Controls.Add(Me.NowPropertyLabel)
        Me.TabPage3.Controls.Add(Me.Label23)
        Me.TabPage3.Controls.Add(Me.Label22)
        Me.TabPage3.Controls.Add(Me.Label21)
        Me.TabPage3.Controls.Add(Me.Label20)
        Me.TabPage3.Controls.Add(Me.Label19)
        Me.TabPage3.Controls.Add(Me.Label18)
        Me.TabPage3.Location = New System.Drawing.Point(4, 40)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Size = New System.Drawing.Size(662, 383)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "DateTime Properties"
        '
        'RefreshProperties
        '
        Me.RefreshProperties.Location = New System.Drawing.Point(34, 10)
        Me.RefreshProperties.Name = "RefreshProperties"
        Me.RefreshProperties.TabIndex = 0
        Me.RefreshProperties.Text = "&Refresh"
        '
        'Year
        '
        Me.Year.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Year.Location = New System.Drawing.Point(422, 204)
        Me.Year.Margin = New System.Windows.Forms.Padding(3, 0, 3, 3)
        Me.Year.Name = "Year"
        Me.Year.Size = New System.Drawing.Size(137, 23)
        Me.Year.TabIndex = 26
        Me.Year.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TimeOfDay
        '
        Me.TimeOfDay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TimeOfDay.Location = New System.Drawing.Point(422, 180)
        Me.TimeOfDay.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.TimeOfDay.Name = "TimeOfDay"
        Me.TimeOfDay.Size = New System.Drawing.Size(137, 23)
        Me.TimeOfDay.TabIndex = 25
        Me.TimeOfDay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Ticks
        '
        Me.Ticks.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Ticks.Location = New System.Drawing.Point(422, 156)
        Me.Ticks.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Ticks.Name = "Ticks"
        Me.Ticks.Size = New System.Drawing.Size(137, 23)
        Me.Ticks.TabIndex = 24
        Me.Ticks.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Second
        '
        Me.Second.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Second.Location = New System.Drawing.Point(422, 132)
        Me.Second.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Second.Name = "Second"
        Me.Second.Size = New System.Drawing.Size(137, 23)
        Me.Second.TabIndex = 23
        Me.Second.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Month
        '
        Me.Month.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Month.Location = New System.Drawing.Point(422, 108)
        Me.Month.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Month.Name = "Month"
        Me.Month.Size = New System.Drawing.Size(137, 23)
        Me.Month.TabIndex = 22
        Me.Month.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Minute
        '
        Me.Minute.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Minute.Location = New System.Drawing.Point(422, 84)
        Me.Minute.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Minute.Name = "Minute"
        Me.Minute.Size = New System.Drawing.Size(137, 23)
        Me.Minute.TabIndex = 21
        Me.Minute.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Millisecond
        '
        Me.Millisecond.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Millisecond.Location = New System.Drawing.Point(422, 61)
        Me.Millisecond.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.Millisecond.Name = "Millisecond"
        Me.Millisecond.Size = New System.Drawing.Size(137, 23)
        Me.Millisecond.TabIndex = 20
        Me.Millisecond.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label30
        '
        Me.Label30.Location = New System.Drawing.Point(316, 204)
        Me.Label30.Margin = New System.Windows.Forms.Padding(3, 0, 3, 3)
        Me.Label30.Name = "Label30"
        Me.Label30.TabIndex = 19
        Me.Label30.Text = "Year"
        Me.Label30.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label29
        '
        Me.Label29.Location = New System.Drawing.Point(316, 180)
        Me.Label29.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Label29.Name = "Label29"
        Me.Label29.TabIndex = 18
        Me.Label29.Text = "TimeOfDay"
        Me.Label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label28
        '
        Me.Label28.Location = New System.Drawing.Point(316, 156)
        Me.Label28.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Label28.Name = "Label28"
        Me.Label28.TabIndex = 17
        Me.Label28.Text = "Ticks"
        Me.Label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label27
        '
        Me.Label27.Location = New System.Drawing.Point(316, 132)
        Me.Label27.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Label27.Name = "Label27"
        Me.Label27.TabIndex = 16
        Me.Label27.Text = "Second"
        Me.Label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label26
        '
        Me.Label26.Location = New System.Drawing.Point(316, 108)
        Me.Label26.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Label26.Name = "Label26"
        Me.Label26.TabIndex = 15
        Me.Label26.Text = "Month"
        Me.Label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label25
        '
        Me.Label25.Location = New System.Drawing.Point(316, 84)
        Me.Label25.Margin = New System.Windows.Forms.Padding(3, 1, 3, 1)
        Me.Label25.Name = "Label25"
        Me.Label25.TabIndex = 14
        Me.Label25.Text = "Minute"
        Me.Label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label24
        '
        Me.Label24.Location = New System.Drawing.Point(316, 60)
        Me.Label24.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.Label24.Name = "Label24"
        Me.Label24.TabIndex = 13
        Me.Label24.Text = "Millisecond"
        Me.Label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Hour
        '
        Me.Hour.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Hour.Location = New System.Drawing.Point(141, 180)
        Me.Hour.Margin = New System.Windows.Forms.Padding(3, 0, 3, 3)
        Me.Hour.Name = "Hour"
        Me.Hour.Size = New System.Drawing.Size(137, 23)
        Me.Hour.TabIndex = 12
        Me.Hour.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'DayOfYear
        '
        Me.DayOfYear.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.DayOfYear.Location = New System.Drawing.Point(141, 156)
        Me.DayOfYear.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.DayOfYear.Name = "DayOfYear"
        Me.DayOfYear.Size = New System.Drawing.Size(137, 23)
        Me.DayOfYear.TabIndex = 11
        Me.DayOfYear.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'DayOfWeek
        '
        Me.DayOfWeek.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.DayOfWeek.Location = New System.Drawing.Point(141, 132)
        Me.DayOfWeek.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.DayOfWeek.Name = "DayOfWeek"
        Me.DayOfWeek.Size = New System.Drawing.Size(137, 23)
        Me.DayOfWeek.TabIndex = 10
        Me.DayOfWeek.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Day
        '
        Me.Day.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Day.Location = New System.Drawing.Point(141, 108)
        Me.Day.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Day.Name = "Day"
        Me.Day.Size = New System.Drawing.Size(137, 23)
        Me.Day.TabIndex = 9
        Me.Day.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'DateLabel
        '
        Me.DateLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.DateLabel.Location = New System.Drawing.Point(141, 84)
        Me.DateLabel.Margin = New System.Windows.Forms.Padding(3, 1, 3, 1)
        Me.DateLabel.Name = "DateLabel"
        Me.DateLabel.Size = New System.Drawing.Size(137, 23)
        Me.DateLabel.TabIndex = 8
        Me.DateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'NowPropertyLabel
        '
        Me.NowPropertyLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.NowPropertyLabel.Location = New System.Drawing.Point(141, 61)
        Me.NowPropertyLabel.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.NowPropertyLabel.Name = "NowPropertyLabel"
        Me.NowPropertyLabel.Size = New System.Drawing.Size(137, 23)
        Me.NowPropertyLabel.TabIndex = 7
        Me.NowPropertyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label23
        '
        Me.Label23.Location = New System.Drawing.Point(36, 180)
        Me.Label23.Margin = New System.Windows.Forms.Padding(3, 0, 3, 3)
        Me.Label23.Name = "Label23"
        Me.Label23.TabIndex = 6
        Me.Label23.Text = "Hour"
        Me.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label22
        '
        Me.Label22.Location = New System.Drawing.Point(36, 156)
        Me.Label22.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Label22.Name = "Label22"
        Me.Label22.TabIndex = 5
        Me.Label22.Text = "DayOfYear"
        Me.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label21
        '
        Me.Label21.Location = New System.Drawing.Point(35, 132)
        Me.Label21.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Label21.Name = "Label21"
        Me.Label21.TabIndex = 4
        Me.Label21.Text = "DayOfWeek"
        Me.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label20
        '
        Me.Label20.Location = New System.Drawing.Point(35, 108)
        Me.Label20.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Label20.Name = "Label20"
        Me.Label20.TabIndex = 3
        Me.Label20.Text = "Day"
        Me.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label19
        '
        Me.Label19.Location = New System.Drawing.Point(34, 84)
        Me.Label19.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Label19.Name = "Label19"
        Me.Label19.TabIndex = 2
        Me.Label19.Text = "Date"
        Me.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label18
        '
        Me.Label18.Location = New System.Drawing.Point(35, 60)
        Me.Label18.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.Label18.Name = "Label18"
        Me.Label18.TabIndex = 1
        Me.Label18.Text = "Now"
        Me.Label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TabPage4
        '
        Me.TabPage4.Controls.Add(Me.ToOADate)
        Me.TabPage4.Controls.Add(Me.ToShortDateString)
        Me.TabPage4.Controls.Add(Me.ToShortTimeString)
        Me.TabPage4.Controls.Add(Me.ToStringLabel)
        Me.TabPage4.Controls.Add(Me.ToUniversalTime)
        Me.TabPage4.Controls.Add(Me.Label40)
        Me.TabPage4.Controls.Add(Me.Label39)
        Me.TabPage4.Controls.Add(Me.Label38)
        Me.TabPage4.Controls.Add(Me.Label37)
        Me.TabPage4.Controls.Add(Me.Label36)
        Me.TabPage4.Controls.Add(Me.ToLongTimeString)
        Me.TabPage4.Controls.Add(Me.ToLongDateString)
        Me.TabPage4.Controls.Add(Me.ToLocalTime)
        Me.TabPage4.Controls.Add(Me.ToFileTime)
        Me.TabPage4.Controls.Add(Me.Now2Label)
        Me.TabPage4.Controls.Add(Me.Label35)
        Me.TabPage4.Controls.Add(Me.Label34)
        Me.TabPage4.Controls.Add(Me.Label33)
        Me.TabPage4.Controls.Add(Me.Label32)
        Me.TabPage4.Controls.Add(Me.Label31)
        Me.TabPage4.Controls.Add(Me.RefreshConversion)
        Me.TabPage4.Location = New System.Drawing.Point(4, 40)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Size = New System.Drawing.Size(662, 383)
        Me.TabPage4.TabIndex = 3
        Me.TabPage4.Text = "DateTime Conversion Methods"
        '
        'ToOADate
        '
        Me.ToOADate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.ToOADate.Location = New System.Drawing.Point(408, 59)
        Me.ToOADate.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.ToOADate.Name = "ToOADate"
        Me.ToOADate.Size = New System.Drawing.Size(144, 23)
        Me.ToOADate.TabIndex = 16
        Me.ToOADate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ToShortDateString
        '
        Me.ToShortDateString.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.ToShortDateString.Location = New System.Drawing.Point(408, 84)
        Me.ToShortDateString.Margin = New System.Windows.Forms.Padding(3, 1, 3, 1)
        Me.ToShortDateString.Name = "ToShortDateString"
        Me.ToShortDateString.Size = New System.Drawing.Size(144, 23)
        Me.ToShortDateString.TabIndex = 17
        Me.ToShortDateString.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ToShortTimeString
        '
        Me.ToShortTimeString.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.ToShortTimeString.Location = New System.Drawing.Point(408, 108)
        Me.ToShortTimeString.Margin = New System.Windows.Forms.Padding(1, 0, 3, 0)
        Me.ToShortTimeString.Name = "ToShortTimeString"
        Me.ToShortTimeString.Size = New System.Drawing.Size(144, 23)
        Me.ToShortTimeString.TabIndex = 18
        Me.ToShortTimeString.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ToStringLabel
        '
        Me.ToStringLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.ToStringLabel.Location = New System.Drawing.Point(408, 132)
        Me.ToStringLabel.Margin = New System.Windows.Forms.Padding(1, 1, 3, 1)
        Me.ToStringLabel.Name = "ToStringLabel"
        Me.ToStringLabel.Size = New System.Drawing.Size(144, 23)
        Me.ToStringLabel.TabIndex = 19
        Me.ToStringLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ToUniversalTime
        '
        Me.ToUniversalTime.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.ToUniversalTime.Location = New System.Drawing.Point(408, 156)
        Me.ToUniversalTime.Margin = New System.Windows.Forms.Padding(1, 0, 3, 3)
        Me.ToUniversalTime.Name = "ToUniversalTime"
        Me.ToUniversalTime.Size = New System.Drawing.Size(144, 23)
        Me.ToUniversalTime.TabIndex = 20
        Me.ToUniversalTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label40
        '
        Me.Label40.Location = New System.Drawing.Point(302, 58)
        Me.Label40.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.Label40.Name = "Label40"
        Me.Label40.TabIndex = 11
        Me.Label40.Text = "ToOADate"
        Me.Label40.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label39
        '
        Me.Label39.Location = New System.Drawing.Point(302, 82)
        Me.Label39.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
        Me.Label39.Name = "Label39"
        Me.Label39.TabIndex = 12
        Me.Label39.Text = "ToShortDateString"
        Me.Label39.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label38
        '
        Me.Label38.Location = New System.Drawing.Point(302, 106)
        Me.Label38.Margin = New System.Windows.Forms.Padding(3, 1, 3, 1)
        Me.Label38.Name = "Label38"
        Me.Label38.TabIndex = 13
        Me.Label38.Text = "ToShortTimeString"
        Me.Label38.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label37
        '
        Me.Label37.Location = New System.Drawing.Point(302, 130)
        Me.Label37.Margin = New System.Windows.Forms.Padding(3, 0, 2, 0)
        Me.Label37.Name = "Label37"
        Me.Label37.TabIndex = 14
        Me.Label37.Text = "ToString"
        Me.Label37.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label36
        '
        Me.Label36.Location = New System.Drawing.Point(302, 154)
        Me.Label36.Margin = New System.Windows.Forms.Padding(3, 1, 3, 3)
        Me.Label36.Name = "Label36"
        Me.Label36.TabIndex = 15
        Me.Label36.Text = "ToUniversalTime"
        Me.Label36.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ToLongTimeString
        '
        Me.ToLongTimeString.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.ToLongTimeString.Location = New System.Drawing.Point(139, 156)
        Me.ToLongTimeString.Margin = New System.Windows.Forms.Padding(1, 0, 3, 3)
        Me.ToLongTimeString.Name = "ToLongTimeString"
        Me.ToLongTimeString.Size = New System.Drawing.Size(144, 23)
        Me.ToLongTimeString.TabIndex = 10
        Me.ToLongTimeString.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ToLongDateString
        '
        Me.ToLongDateString.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.ToLongDateString.Location = New System.Drawing.Point(139, 132)
        Me.ToLongDateString.Margin = New System.Windows.Forms.Padding(1, 0, 3, 1)
        Me.ToLongDateString.Name = "ToLongDateString"
        Me.ToLongDateString.Size = New System.Drawing.Size(144, 23)
        Me.ToLongDateString.TabIndex = 9
        Me.ToLongDateString.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ToLocalTime
        '
        Me.ToLocalTime.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.ToLocalTime.Location = New System.Drawing.Point(139, 108)
        Me.ToLocalTime.Margin = New System.Windows.Forms.Padding(1, 0, 3, 1)
        Me.ToLocalTime.Name = "ToLocalTime"
        Me.ToLocalTime.Size = New System.Drawing.Size(144, 23)
        Me.ToLocalTime.TabIndex = 8
        Me.ToLocalTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ToFileTime
        '
        Me.ToFileTime.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.ToFileTime.Location = New System.Drawing.Point(139, 84)
        Me.ToFileTime.Margin = New System.Windows.Forms.Padding(3, 1, 3, 1)
        Me.ToFileTime.Name = "ToFileTime"
        Me.ToFileTime.Size = New System.Drawing.Size(144, 23)
        Me.ToFileTime.TabIndex = 7
        Me.ToFileTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Now2Label
        '
        Me.Now2Label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Now2Label.Location = New System.Drawing.Point(139, 59)
        Me.Now2Label.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.Now2Label.Name = "Now2Label"
        Me.Now2Label.Size = New System.Drawing.Size(144, 23)
        Me.Now2Label.TabIndex = 6
        Me.Now2Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label35
        '
        Me.Label35.Location = New System.Drawing.Point(36, 156)
        Me.Label35.Margin = New System.Windows.Forms.Padding(3, 0, 3, 3)
        Me.Label35.Name = "Label35"
        Me.Label35.TabIndex = 5
        Me.Label35.Text = "ToLongTimeString"
        Me.Label35.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label34
        '
        Me.Label34.Location = New System.Drawing.Point(36, 132)
        Me.Label34.Margin = New System.Windows.Forms.Padding(3, 0, 2, 1)
        Me.Label34.Name = "Label34"
        Me.Label34.TabIndex = 4
        Me.Label34.Text = "ToLongDateString"
        Me.Label34.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label33
        '
        Me.Label33.Location = New System.Drawing.Point(33, 108)
        Me.Label33.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Label33.Name = "Label33"
        Me.Label33.TabIndex = 3
        Me.Label33.Text = "ToLocalTime"
        Me.Label33.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label32
        '
        Me.Label32.Location = New System.Drawing.Point(33, 84)
        Me.Label32.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Label32.Name = "Label32"
        Me.Label32.TabIndex = 2
        Me.Label32.Text = "ToFileTime"
        Me.Label32.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label31
        '
        Me.Label31.Location = New System.Drawing.Point(33, 60)
        Me.Label31.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.Label31.Name = "Label31"
        Me.Label31.TabIndex = 1
        Me.Label31.Text = "Now"
        Me.Label31.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'RefreshConversion
        '
        Me.RefreshConversion.Location = New System.Drawing.Point(31, 23)
        Me.RefreshConversion.Name = "RefreshConversion"
        Me.RefreshConversion.TabIndex = 0
        Me.RefreshConversion.Text = "&Refresh"
        '
        'TabPage5
        '
        Me.TabPage5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TabPage5.Controls.Add(Me.TotalSeconds)
        Me.TabPage5.Controls.Add(Me.TotalMinutes)
        Me.TabPage5.Controls.Add(Me.TotalMilliseconds)
        Me.TabPage5.Controls.Add(Me.TotalHours)
        Me.TabPage5.Controls.Add(Me.TotalDays)
        Me.TabPage5.Controls.Add(Me.TimeSpanTicks)
        Me.TabPage5.Controls.Add(Me.Seconds)
        Me.TabPage5.Controls.Add(Me.Minutes)
        Me.TabPage5.Controls.Add(Me.Milliseconds)
        Me.TabPage5.Controls.Add(Me.Hours)
        Me.TabPage5.Controls.Add(Me.Days)
        Me.TabPage5.Controls.Add(Me.Label69)
        Me.TabPage5.Controls.Add(Me.Label68)
        Me.TabPage5.Controls.Add(Me.Label67)
        Me.TabPage5.Controls.Add(Me.Label66)
        Me.TabPage5.Controls.Add(Me.Label65)
        Me.TabPage5.Controls.Add(Me.Label57)
        Me.TabPage5.Controls.Add(Me.Label56)
        Me.TabPage5.Controls.Add(Me.Label55)
        Me.TabPage5.Controls.Add(Me.Label54)
        Me.TabPage5.Controls.Add(Me.Label53)
        Me.TabPage5.Controls.Add(Me.Label52)
        Me.TabPage5.Controls.Add(Me.tabTimeSpan)
        Me.TabPage5.Location = New System.Drawing.Point(4, 40)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Size = New System.Drawing.Size(662, 383)
        Me.TabPage5.TabIndex = 4
        Me.TabPage5.Text = "TimeSpan Properties"
        '
        'TotalSeconds
        '
        Me.TotalSeconds.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TotalSeconds.Location = New System.Drawing.Point(409, 275)
        Me.TotalSeconds.Margin = New System.Windows.Forms.Padding(1, 0, 3, 3)
        Me.TotalSeconds.Name = "TotalSeconds"
        Me.TotalSeconds.Size = New System.Drawing.Size(144, 23)
        Me.TotalSeconds.TabIndex = 55
        Me.TotalSeconds.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TotalMinutes
        '
        Me.TotalMinutes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TotalMinutes.Location = New System.Drawing.Point(409, 251)
        Me.TotalMinutes.Margin = New System.Windows.Forms.Padding(1, 0, 3, 1)
        Me.TotalMinutes.Name = "TotalMinutes"
        Me.TotalMinutes.Size = New System.Drawing.Size(144, 23)
        Me.TotalMinutes.TabIndex = 54
        Me.TotalMinutes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TotalMilliseconds
        '
        Me.TotalMilliseconds.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TotalMilliseconds.Location = New System.Drawing.Point(409, 227)
        Me.TotalMilliseconds.Margin = New System.Windows.Forms.Padding(1, 0, 3, 1)
        Me.TotalMilliseconds.Name = "TotalMilliseconds"
        Me.TotalMilliseconds.Size = New System.Drawing.Size(144, 23)
        Me.TotalMilliseconds.TabIndex = 53
        Me.TotalMilliseconds.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TotalHours
        '
        Me.TotalHours.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TotalHours.Location = New System.Drawing.Point(409, 203)
        Me.TotalHours.Margin = New System.Windows.Forms.Padding(1, 0, 3, 1)
        Me.TotalHours.Name = "TotalHours"
        Me.TotalHours.Size = New System.Drawing.Size(144, 23)
        Me.TotalHours.TabIndex = 52
        Me.TotalHours.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TotalDays
        '
        Me.TotalDays.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TotalDays.Location = New System.Drawing.Point(409, 179)
        Me.TotalDays.Margin = New System.Windows.Forms.Padding(1, 0, 3, 1)
        Me.TotalDays.Name = "TotalDays"
        Me.TotalDays.Size = New System.Drawing.Size(144, 23)
        Me.TotalDays.TabIndex = 51
        Me.TotalDays.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TimeSpanTicks
        '
        Me.TimeSpanTicks.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TimeSpanTicks.Location = New System.Drawing.Point(409, 155)
        Me.TimeSpanTicks.Margin = New System.Windows.Forms.Padding(0, 3, 3, 1)
        Me.TimeSpanTicks.Name = "TimeSpanTicks"
        Me.TimeSpanTicks.Size = New System.Drawing.Size(144, 23)
        Me.TimeSpanTicks.TabIndex = 50
        Me.TimeSpanTicks.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Seconds
        '
        Me.Seconds.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Seconds.Location = New System.Drawing.Point(128, 251)
        Me.Seconds.Margin = New System.Windows.Forms.Padding(1, 0, 3, 3)
        Me.Seconds.Name = "Seconds"
        Me.Seconds.Size = New System.Drawing.Size(144, 23)
        Me.Seconds.TabIndex = 49
        Me.Seconds.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Minutes
        '
        Me.Minutes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Minutes.Location = New System.Drawing.Point(128, 227)
        Me.Minutes.Margin = New System.Windows.Forms.Padding(1, 0, 0, 1)
        Me.Minutes.Name = "Minutes"
        Me.Minutes.Size = New System.Drawing.Size(144, 23)
        Me.Minutes.TabIndex = 48
        Me.Minutes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Milliseconds
        '
        Me.Milliseconds.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Milliseconds.Location = New System.Drawing.Point(128, 203)
        Me.Milliseconds.Margin = New System.Windows.Forms.Padding(1, 0, 3, 1)
        Me.Milliseconds.Name = "Milliseconds"
        Me.Milliseconds.Size = New System.Drawing.Size(144, 23)
        Me.Milliseconds.TabIndex = 47
        Me.Milliseconds.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Hours
        '
        Me.Hours.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Hours.Location = New System.Drawing.Point(128, 179)
        Me.Hours.Margin = New System.Windows.Forms.Padding(1, 1, 3, 1)
        Me.Hours.Name = "Hours"
        Me.Hours.Size = New System.Drawing.Size(144, 23)
        Me.Hours.TabIndex = 46
        Me.Hours.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Days
        '
        Me.Days.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Days.Location = New System.Drawing.Point(128, 155)
        Me.Days.Margin = New System.Windows.Forms.Padding(1, 3, 3, 2)
        Me.Days.Name = "Days"
        Me.Days.Size = New System.Drawing.Size(144, 23)
        Me.Days.TabIndex = 45
        Me.Days.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label69
        '
        Me.Label69.Location = New System.Drawing.Point(25, 155)
        Me.Label69.Margin = New System.Windows.Forms.Padding(3, 3, 2, 1)
        Me.Label69.Name = "Label69"
        Me.Label69.TabIndex = 27
        Me.Label69.Text = "Days"
        Me.Label69.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label68
        '
        Me.Label68.Location = New System.Drawing.Point(25, 179)
        Me.Label68.Margin = New System.Windows.Forms.Padding(3, 0, 2, 0)
        Me.Label68.Name = "Label68"
        Me.Label68.TabIndex = 28
        Me.Label68.Text = "Hours"
        Me.Label68.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label67
        '
        Me.Label67.Location = New System.Drawing.Point(25, 203)
        Me.Label67.Margin = New System.Windows.Forms.Padding(3, 1, 2, 1)
        Me.Label67.Name = "Label67"
        Me.Label67.TabIndex = 29
        Me.Label67.Text = "Milliseconds"
        Me.Label67.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label66
        '
        Me.Label66.Location = New System.Drawing.Point(25, 227)
        Me.Label66.Margin = New System.Windows.Forms.Padding(3, 0, 1, 0)
        Me.Label66.Name = "Label66"
        Me.Label66.TabIndex = 30
        Me.Label66.Text = "Minutes"
        Me.Label66.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label65
        '
        Me.Label65.Location = New System.Drawing.Point(25, 251)
        Me.Label65.Margin = New System.Windows.Forms.Padding(3, 1, 2, 1)
        Me.Label65.Name = "Label65"
        Me.Label65.TabIndex = 31
        Me.Label65.Text = "Seconds"
        Me.Label65.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label57
        '
        Me.Label57.Location = New System.Drawing.Point(306, 155)
        Me.Label57.Margin = New System.Windows.Forms.Padding(3, 3, 1, 1)
        Me.Label57.Name = "Label57"
        Me.Label57.TabIndex = 39
        Me.Label57.Text = "Ticks"
        Me.Label57.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label56
        '
        Me.Label56.Location = New System.Drawing.Point(306, 179)
        Me.Label56.Margin = New System.Windows.Forms.Padding(3, 1, 2, 1)
        Me.Label56.Name = "Label56"
        Me.Label56.TabIndex = 40
        Me.Label56.Text = "TotalDays"
        Me.Label56.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label55
        '
        Me.Label55.Location = New System.Drawing.Point(306, 203)
        Me.Label55.Margin = New System.Windows.Forms.Padding(3, 0, 2, 0)
        Me.Label55.Name = "Label55"
        Me.Label55.TabIndex = 41
        Me.Label55.Text = "TotalHours"
        Me.Label55.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label54
        '
        Me.Label54.Location = New System.Drawing.Point(306, 227)
        Me.Label54.Margin = New System.Windows.Forms.Padding(3, 1, 2, 1)
        Me.Label54.Name = "Label54"
        Me.Label54.TabIndex = 42
        Me.Label54.Text = "TotalMilliseconds"
        Me.Label54.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label53
        '
        Me.Label53.Location = New System.Drawing.Point(306, 251)
        Me.Label53.Margin = New System.Windows.Forms.Padding(3, 0, 2, 0)
        Me.Label53.Name = "Label53"
        Me.Label53.TabIndex = 43
        Me.Label53.Text = "TotalMinutes"
        Me.Label53.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label52
        '
        Me.Label52.Location = New System.Drawing.Point(306, 275)
        Me.Label52.Margin = New System.Windows.Forms.Padding(1, 1, 2, 1)
        Me.Label52.Name = "Label52"
        Me.Label52.TabIndex = 44
        Me.Label52.Text = "TotalSeconds"
        Me.Label52.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'tabTimeSpan
        '
        Me.tabTimeSpan.Controls.Add(Me.TabPage8)
        Me.tabTimeSpan.Controls.Add(Me.TabPage9)
        Me.tabTimeSpan.Location = New System.Drawing.Point(10, 10)
        Me.tabTimeSpan.Name = "tabTimeSpan"
        Me.tabTimeSpan.SelectedIndex = 0
        Me.tabTimeSpan.ShowToolTips = True
        Me.tabTimeSpan.Size = New System.Drawing.Size(476, 119)
        Me.tabTimeSpan.TabIndex = 0
        '
        'TabPage8
        '
        Me.TabPage8.Controls.Add(Me.RefreshTSProperties)
        Me.TabPage8.Controls.Add(Me.EndTime)
        Me.TabPage8.Controls.Add(Me.StartTime)
        Me.TabPage8.Controls.Add(Me.Label42)
        Me.TabPage8.Controls.Add(Me.Label41)
        Me.TabPage8.Location = New System.Drawing.Point(4, 22)
        Me.TabPage8.Name = "TabPage8"
        Me.TabPage8.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage8.Size = New System.Drawing.Size(468, 93)
        Me.TabPage8.TabIndex = 0
        Me.TabPage8.Text = "Enter Start and End Times"
        '
        'RefreshTSProperties
        '
        Me.RefreshTSProperties.Location = New System.Drawing.Point(300, 20)
        Me.RefreshTSProperties.Name = "RefreshTSProperties"
        Me.RefreshTSProperties.TabIndex = 4
        Me.RefreshTSProperties.Text = "&Refresh"
        '
        'EndTime
        '
        Me.EndTime.Location = New System.Drawing.Point(117, 46)
        Me.EndTime.Name = "EndTime"
        Me.EndTime.Size = New System.Drawing.Size(167, 20)
        Me.EndTime.TabIndex = 3
        Me.EndTime.Text = "5:25:17 PM"
        '
        'StartTime
        '
        Me.StartTime.Location = New System.Drawing.Point(117, 22)
        Me.StartTime.Name = "StartTime"
        Me.StartTime.Size = New System.Drawing.Size(167, 20)
        Me.StartTime.TabIndex = 2
        Me.StartTime.Text = "8:14:12 AM"
        '
        'Label42
        '
        Me.Label42.Location = New System.Drawing.Point(10, 46)
        Me.Label42.Margin = New System.Windows.Forms.Padding(3, 1, 3, 3)
        Me.Label42.Name = "Label42"
        Me.Label42.TabIndex = 1
        Me.Label42.Text = "Ending Time"
        Me.Label42.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label41
        '
        Me.Label41.Location = New System.Drawing.Point(10, 20)
        Me.Label41.Margin = New System.Windows.Forms.Padding(3, 3, 3, 2)
        Me.Label41.Name = "Label41"
        Me.Label41.TabIndex = 0
        Me.Label41.Text = "Starting Time"
        Me.Label41.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TabPage9
        '
        Me.TabPage9.Controls.Add(Me.btnCalcParse)
        Me.TabPage9.Controls.Add(Me.txtParse)
        Me.TabPage9.Controls.Add(Me.Label43)
        Me.TabPage9.Location = New System.Drawing.Point(4, 22)
        Me.TabPage9.Name = "TabPage9"
        Me.TabPage9.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage9.Size = New System.Drawing.Size(468, 93)
        Me.TabPage9.TabIndex = 1
        Me.TabPage9.Text = "Parse TimeSpan"
        '
        'btnCalcParse
        '
        Me.btnCalcParse.Location = New System.Drawing.Point(300, 20)
        Me.btnCalcParse.Name = "btnCalcParse"
        Me.btnCalcParse.TabIndex = 2
        Me.btnCalcParse.Text = "Refresh"
        '
        'txtParse
        '
        Me.txtParse.Location = New System.Drawing.Point(117, 20)
        Me.txtParse.Margin = New System.Windows.Forms.Padding(1, 3, 3, 3)
        Me.txtParse.Name = "txtParse"
        Me.txtParse.Size = New System.Drawing.Size(167, 20)
        Me.txtParse.TabIndex = 1
        Me.txtParse.Text = "3.14:55:26.27"
        '
        'Label43
        '
        Me.Label43.Location = New System.Drawing.Point(10, 20)
        Me.Label43.Margin = New System.Windows.Forms.Padding(3, 3, 2, 3)
        Me.Label43.Name = "Label43"
        Me.Label43.TabIndex = 0
        Me.Label43.Text = "Parse"
        Me.Label43.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TabPage6
        '
        Me.TabPage6.Controls.Add(Me.FromTicksLabel)
        Me.TabPage6.Controls.Add(Me.FromSecondsLabel)
        Me.TabPage6.Controls.Add(Me.FromMinutesLabel)
        Me.TabPage6.Controls.Add(Me.FromMillisecondsLabel)
        Me.TabPage6.Controls.Add(Me.FromHoursLabel)
        Me.TabPage6.Controls.Add(Me.FromDaysLabel)
        Me.TabPage6.Controls.Add(Me.FromTicks)
        Me.TabPage6.Controls.Add(Me.FromSeconds)
        Me.TabPage6.Controls.Add(Me.FromMinutes)
        Me.TabPage6.Controls.Add(Me.FromMilliseconds)
        Me.TabPage6.Controls.Add(Me.FromDays)
        Me.TabPage6.Controls.Add(Me.FromHours)
        Me.TabPage6.Controls.Add(Me.Label71)
        Me.TabPage6.Controls.Add(Me.Label70)
        Me.TabPage6.Controls.Add(Me.Label64)
        Me.TabPage6.Controls.Add(Me.Label58)
        Me.TabPage6.Controls.Add(Me.Label51)
        Me.TabPage6.Controls.Add(Me.Label44)
        Me.TabPage6.Controls.Add(Me.RefreshTSMethods)
        Me.TabPage6.Location = New System.Drawing.Point(4, 40)
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.Size = New System.Drawing.Size(662, 383)
        Me.TabPage6.TabIndex = 5
        Me.TabPage6.Text = "TimeSpan Methods"
        '
        'FromTicksLabel
        '
        Me.FromTicksLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.FromTicksLabel.Location = New System.Drawing.Point(245, 178)
        Me.FromTicksLabel.Name = "FromTicksLabel"
        Me.FromTicksLabel.Size = New System.Drawing.Size(144, 23)
        Me.FromTicksLabel.TabIndex = 18
        Me.FromTicksLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FromSecondsLabel
        '
        Me.FromSecondsLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.FromSecondsLabel.Location = New System.Drawing.Point(245, 154)
        Me.FromSecondsLabel.Name = "FromSecondsLabel"
        Me.FromSecondsLabel.Size = New System.Drawing.Size(144, 23)
        Me.FromSecondsLabel.TabIndex = 17
        Me.FromSecondsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FromMinutesLabel
        '
        Me.FromMinutesLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.FromMinutesLabel.Location = New System.Drawing.Point(245, 130)
        Me.FromMinutesLabel.Name = "FromMinutesLabel"
        Me.FromMinutesLabel.Size = New System.Drawing.Size(144, 23)
        Me.FromMinutesLabel.TabIndex = 16
        Me.FromMinutesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FromMillisecondsLabel
        '
        Me.FromMillisecondsLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.FromMillisecondsLabel.Location = New System.Drawing.Point(245, 106)
        Me.FromMillisecondsLabel.Name = "FromMillisecondsLabel"
        Me.FromMillisecondsLabel.Size = New System.Drawing.Size(144, 23)
        Me.FromMillisecondsLabel.TabIndex = 15
        Me.FromMillisecondsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FromHoursLabel
        '
        Me.FromHoursLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.FromHoursLabel.Location = New System.Drawing.Point(245, 82)
        Me.FromHoursLabel.Name = "FromHoursLabel"
        Me.FromHoursLabel.Size = New System.Drawing.Size(144, 23)
        Me.FromHoursLabel.TabIndex = 14
        Me.FromHoursLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FromDaysLabel
        '
        Me.FromDaysLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.FromDaysLabel.Location = New System.Drawing.Point(245, 58)
        Me.FromDaysLabel.Name = "FromDaysLabel"
        Me.FromDaysLabel.Size = New System.Drawing.Size(144, 23)
        Me.FromDaysLabel.TabIndex = 13
        Me.FromDaysLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FromTicks
        '
        Me.FromTicks.Location = New System.Drawing.Point(137, 180)
        Me.FromTicks.Name = "FromTicks"
        Me.FromTicks.TabIndex = 6
        Me.FromTicks.Text = "123456789"
        Me.FromTicks.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'FromSeconds
        '
        Me.FromSeconds.Location = New System.Drawing.Point(137, 156)
        Me.FromSeconds.Name = "FromSeconds"
        Me.FromSeconds.TabIndex = 5
        Me.FromSeconds.Text = "289"
        Me.FromSeconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'FromMinutes
        '
        Me.FromMinutes.Location = New System.Drawing.Point(137, 132)
        Me.FromMinutes.Name = "FromMinutes"
        Me.FromMinutes.TabIndex = 4
        Me.FromMinutes.Text = "128"
        Me.FromMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'FromMilliseconds
        '
        Me.FromMilliseconds.Location = New System.Drawing.Point(137, 108)
        Me.FromMilliseconds.Name = "FromMilliseconds"
        Me.FromMilliseconds.TabIndex = 3
        Me.FromMilliseconds.Text = "20098"
        Me.FromMilliseconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'FromDays
        '
        Me.FromDays.Location = New System.Drawing.Point(137, 60)
        Me.FromDays.Name = "FromDays"
        Me.FromDays.TabIndex = 1
        Me.FromDays.Text = "13.456"
        Me.FromDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'FromHours
        '
        Me.FromHours.Location = New System.Drawing.Point(137, 84)
        Me.FromHours.Name = "FromHours"
        Me.FromHours.TabIndex = 2
        Me.FromHours.Text = "47.6"
        Me.FromHours.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label71
        '
        Me.Label71.Location = New System.Drawing.Point(31, 178)
        Me.Label71.Name = "Label71"
        Me.Label71.TabIndex = 6
        Me.Label71.Text = "FromTicks"
        Me.Label71.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label70
        '
        Me.Label70.Location = New System.Drawing.Point(31, 154)
        Me.Label70.Margin = New System.Windows.Forms.Padding(3, 0, 3, 3)
        Me.Label70.Name = "Label70"
        Me.Label70.TabIndex = 5
        Me.Label70.Text = "FromSeconds"
        Me.Label70.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label64
        '
        Me.Label64.Location = New System.Drawing.Point(30, 130)
        Me.Label64.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Label64.Name = "Label64"
        Me.Label64.TabIndex = 4
        Me.Label64.Text = "FromMinutes"
        Me.Label64.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label58
        '
        Me.Label58.Location = New System.Drawing.Point(30, 106)
        Me.Label58.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Label58.Name = "Label58"
        Me.Label58.TabIndex = 3
        Me.Label58.Text = "FromMilliseconds"
        Me.Label58.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label51
        '
        Me.Label51.Location = New System.Drawing.Point(30, 82)
        Me.Label51.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.Label51.Name = "Label51"
        Me.Label51.TabIndex = 2
        Me.Label51.Text = "FromHours"
        Me.Label51.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label44
        '
        Me.Label44.Location = New System.Drawing.Point(30, 58)
        Me.Label44.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.Label44.Name = "Label44"
        Me.Label44.TabIndex = 1
        Me.Label44.Text = "FromDays"
        Me.Label44.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'RefreshTSMethods
        '
        Me.RefreshTSMethods.Location = New System.Drawing.Point(28, 14)
        Me.RefreshTSMethods.Name = "RefreshTSMethods"
        Me.RefreshTSMethods.TabIndex = 0
        Me.RefreshTSMethods.Text = "&Refresh"
        '
        'TabPage7
        '
        Me.TabPage7.Controls.Add(Me.Zero)
        Me.TabPage7.Controls.Add(Me.TicksPerSecond)
        Me.TabPage7.Controls.Add(Me.TicksPerMinute)
        Me.TabPage7.Controls.Add(Me.TicksPerMillisecond)
        Me.TabPage7.Controls.Add(Me.TicksPerHour)
        Me.TabPage7.Controls.Add(Me.TicksPerDay)
        Me.TabPage7.Controls.Add(Me.MinValueTS)
        Me.TabPage7.Controls.Add(Me.MaxValueTS)
        Me.TabPage7.Controls.Add(Me.Label79)
        Me.TabPage7.Controls.Add(Me.Label78)
        Me.TabPage7.Controls.Add(Me.Label77)
        Me.TabPage7.Controls.Add(Me.Label76)
        Me.TabPage7.Controls.Add(Me.Label75)
        Me.TabPage7.Controls.Add(Me.Label74)
        Me.TabPage7.Controls.Add(Me.Label73)
        Me.TabPage7.Controls.Add(Me.Label72)
        Me.TabPage7.Location = New System.Drawing.Point(4, 40)
        Me.TabPage7.Name = "TabPage7"
        Me.TabPage7.Size = New System.Drawing.Size(662, 383)
        Me.TabPage7.TabIndex = 6
        Me.TabPage7.Text = "TimeSpan Fields"
        '
        'Zero
        '
        Me.Zero.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Zero.Location = New System.Drawing.Point(158, 204)
        Me.Zero.Name = "Zero"
        Me.Zero.Size = New System.Drawing.Size(200, 23)
        Me.Zero.TabIndex = 15
        Me.Zero.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TicksPerSecond
        '
        Me.TicksPerSecond.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TicksPerSecond.Location = New System.Drawing.Point(158, 180)
        Me.TicksPerSecond.Name = "TicksPerSecond"
        Me.TicksPerSecond.Size = New System.Drawing.Size(200, 23)
        Me.TicksPerSecond.TabIndex = 14
        Me.TicksPerSecond.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TicksPerMinute
        '
        Me.TicksPerMinute.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TicksPerMinute.Location = New System.Drawing.Point(158, 156)
        Me.TicksPerMinute.Name = "TicksPerMinute"
        Me.TicksPerMinute.Size = New System.Drawing.Size(200, 23)
        Me.TicksPerMinute.TabIndex = 13
        Me.TicksPerMinute.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TicksPerMillisecond
        '
        Me.TicksPerMillisecond.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TicksPerMillisecond.Location = New System.Drawing.Point(158, 132)
        Me.TicksPerMillisecond.Name = "TicksPerMillisecond"
        Me.TicksPerMillisecond.Size = New System.Drawing.Size(200, 23)
        Me.TicksPerMillisecond.TabIndex = 12
        Me.TicksPerMillisecond.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TicksPerHour
        '
        Me.TicksPerHour.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TicksPerHour.Location = New System.Drawing.Point(158, 108)
        Me.TicksPerHour.Name = "TicksPerHour"
        Me.TicksPerHour.Size = New System.Drawing.Size(200, 23)
        Me.TicksPerHour.TabIndex = 11
        Me.TicksPerHour.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TicksPerDay
        '
        Me.TicksPerDay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TicksPerDay.Location = New System.Drawing.Point(158, 84)
        Me.TicksPerDay.Name = "TicksPerDay"
        Me.TicksPerDay.Size = New System.Drawing.Size(200, 23)
        Me.TicksPerDay.TabIndex = 10
        Me.TicksPerDay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'MinValueTS
        '
        Me.MinValueTS.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.MinValueTS.Location = New System.Drawing.Point(158, 60)
        Me.MinValueTS.Name = "MinValueTS"
        Me.MinValueTS.Size = New System.Drawing.Size(200, 23)
        Me.MinValueTS.TabIndex = 9
        Me.MinValueTS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'MaxValueTS
        '
        Me.MaxValueTS.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.MaxValueTS.Location = New System.Drawing.Point(158, 36)
        Me.MaxValueTS.Name = "MaxValueTS"
        Me.MaxValueTS.Size = New System.Drawing.Size(200, 23)
        Me.MaxValueTS.TabIndex = 8
        Me.MaxValueTS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label79
        '
        Me.Label79.Location = New System.Drawing.Point(45, 204)
        Me.Label79.Name = "Label79"
        Me.Label79.Size = New System.Drawing.Size(110, 23)
        Me.Label79.TabIndex = 7
        Me.Label79.Text = "Zero"
        Me.Label79.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label78
        '
        Me.Label78.Location = New System.Drawing.Point(45, 180)
        Me.Label78.Name = "Label78"
        Me.Label78.Size = New System.Drawing.Size(110, 23)
        Me.Label78.TabIndex = 6
        Me.Label78.Text = "TicksPerSecond"
        Me.Label78.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label77
        '
        Me.Label77.Location = New System.Drawing.Point(45, 156)
        Me.Label77.Name = "Label77"
        Me.Label77.Size = New System.Drawing.Size(110, 23)
        Me.Label77.TabIndex = 5
        Me.Label77.Text = "TicksPerMinute"
        Me.Label77.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label76
        '
        Me.Label76.Location = New System.Drawing.Point(45, 132)
        Me.Label76.Name = "Label76"
        Me.Label76.Size = New System.Drawing.Size(110, 23)
        Me.Label76.TabIndex = 4
        Me.Label76.Text = "TicksPerMillisecond"
        Me.Label76.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label75
        '
        Me.Label75.Location = New System.Drawing.Point(45, 108)
        Me.Label75.Name = "Label75"
        Me.Label75.Size = New System.Drawing.Size(110, 23)
        Me.Label75.TabIndex = 3
        Me.Label75.Text = "TicksPerHour"
        Me.Label75.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label74
        '
        Me.Label74.Location = New System.Drawing.Point(45, 84)
        Me.Label74.Name = "Label74"
        Me.Label74.Size = New System.Drawing.Size(110, 23)
        Me.Label74.TabIndex = 2
        Me.Label74.Text = "TicksPerDay"
        Me.Label74.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label73
        '
        Me.Label73.Location = New System.Drawing.Point(45, 60)
        Me.Label73.Name = "Label73"
        Me.Label73.Size = New System.Drawing.Size(110, 23)
        Me.Label73.TabIndex = 1
        Me.Label73.Text = "MinValue"
        Me.Label73.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label72
        '
        Me.Label72.Location = New System.Drawing.Point(45, 36)
        Me.Label72.Name = "Label72"
        Me.Label72.Size = New System.Drawing.Size(110, 23)
        Me.Label72.TabIndex = 0
        Me.Label72.Text = "MaxValue"
        Me.Label72.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(6, 2, 0, 2)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'fileToolStripMenuItem
        '
        Me.fileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.exitToolStripMenuItem})
        Me.fileToolStripMenuItem.Name = "fileToolStripMenuItem"

        Me.fileToolStripMenuItem.Text = "&File"
        '
        'exitToolStripMenuItem
        '
        Me.exitToolStripMenuItem.Name = "exitToolStripMenuItem"

        Me.exitToolStripMenuItem.Text = "E&xit"
        '
        'Form1
        '
        Me.ClientSize = New System.Drawing.Size(670, 457)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage4.ResumeLayout(False)
        Me.TabPage5.ResumeLayout(False)
        Me.tabTimeSpan.ResumeLayout(False)
        Me.TabPage8.ResumeLayout(False)
        Me.TabPage8.PerformLayout()
        Me.TabPage9.ResumeLayout(False)
        Me.TabPage9.PerformLayout()
        Me.TabPage6.ResumeLayout(False)
        Me.TabPage6.PerformLayout()
        Me.TabPage7.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
