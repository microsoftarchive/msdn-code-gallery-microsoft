<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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
        Me.components = New System.ComponentModel.Container
        Dim Feels_LikeLabel As System.Windows.Forms.Label
        Dim BarometerLabel As System.Windows.Forms.Label
        Dim DewpointLabel As System.Windows.Forms.Label
        Dim HumidityLabel As System.Windows.Forms.Label
        Dim VisibilityLabel As System.Windows.Forms.Label
        Dim WindSpeedLabel As System.Windows.Forms.Label
        Dim UV_IndexLabel As System.Windows.Forms.Label
        Dim SunriseLabel1 As System.Windows.Forms.Label
        Dim SunsetLabel As System.Windows.Forms.Label
        Dim Label1 As System.Windows.Forms.Label
        Dim HighTempLabel As System.Windows.Forms.Label
        Dim LowTempLabel As System.Windows.Forms.Label
        Dim High_TempLabel As System.Windows.Forms.Label
        Dim Low_TempLabel As System.Windows.Forms.Label
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.SunsetTextBox = New System.Windows.Forms.TextBox
        Me.CurrentConditionsBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.RedmondWeatherDataSet = New WindowsApplication2.RedmondWeatherDataSet
        Me.SunriseTextBox = New System.Windows.Forms.TextBox
        Me.TemperatureTextBox = New System.Windows.Forms.TextBox
        Me.TextBox6 = New System.Windows.Forms.TextBox
        Me.TextBox2 = New System.Windows.Forms.TextBox
        Me.picConditions = New System.Windows.Forms.PictureBox
        Me.ConditionsTextBox = New System.Windows.Forms.TextBox
        Me.Feels_LikeTextBox = New System.Windows.Forms.TextBox
        Me.BarometerTextBox = New System.Windows.Forms.TextBox
        Me.DewpointTextBox = New System.Windows.Forms.TextBox
        Me.HumidityTextBox = New System.Windows.Forms.TextBox
        Me.VisibilityTextBox = New System.Windows.Forms.TextBox
        Me.WindSpeedTextBox = New System.Windows.Forms.TextBox
        Me.WindDirectionTextBox = New System.Windows.Forms.TextBox
        Me.UV_IndexTextBox = New System.Windows.Forms.TextBox
        Me.Five_DayTextBox = New System.Windows.Forms.TextBox
        Me.Five_Day_ForecastBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.TextBox1 = New System.Windows.Forms.TextBox
        Me.Panel2 = New System.Windows.Forms.Panel
        Me.DayTextBox = New System.Windows.Forms.TextBox
        Me.Panel3 = New System.Windows.Forms.Panel
        Me.TextBox3 = New System.Windows.Forms.TextBox
        Me.DataRepeater1 = New Microsoft.VisualBasic.PowerPacks.DataRepeater
        Me.picTodaysForecast = New System.Windows.Forms.PictureBox
        Me.PeriodTextBox = New System.Windows.Forms.TextBox
        Me.Todays_ForecastBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ForecastTextBox = New System.Windows.Forms.TextBox
        Me.HighTempTextBox = New System.Windows.Forms.TextBox
        Me.LowTempTextBox = New System.Windows.Forms.TextBox
        Me.DataRepeater2 = New Microsoft.VisualBasic.PowerPacks.DataRepeater
        Me.txtWeekDay = New System.Windows.Forms.TextBox
        Me.picForecast = New System.Windows.Forms.PictureBox
        Me.ForecastTextBox1 = New System.Windows.Forms.TextBox
        Me.High_TempTextBox = New System.Windows.Forms.TextBox
        Me.Low_TempTextBox = New System.Windows.Forms.TextBox
        Me.Panel4 = New System.Windows.Forms.Panel
        Me.LinkLabel1 = New System.Windows.Forms.LinkLabel
        Me.TextBox4 = New System.Windows.Forms.TextBox
        Me.CurrentConditionsTableAdapter = New WindowsApplication2.RedmondWeatherDataSetTableAdapters.CurrentConditionsTableAdapter
        Me.Todays_ForecastTableAdapter = New WindowsApplication2.RedmondWeatherDataSetTableAdapters.Todays_ForecastTableAdapter
        Me.Five_Day_ForecastTableAdapter = New WindowsApplication2.RedmondWeatherDataSetTableAdapters.Five_Day_ForecastTableAdapter
        Me.Label2 = New System.Windows.Forms.Label
        Feels_LikeLabel = New System.Windows.Forms.Label
        BarometerLabel = New System.Windows.Forms.Label
        DewpointLabel = New System.Windows.Forms.Label
        HumidityLabel = New System.Windows.Forms.Label
        VisibilityLabel = New System.Windows.Forms.Label
        WindSpeedLabel = New System.Windows.Forms.Label
        UV_IndexLabel = New System.Windows.Forms.Label
        SunriseLabel1 = New System.Windows.Forms.Label
        SunsetLabel = New System.Windows.Forms.Label
        Label1 = New System.Windows.Forms.Label
        HighTempLabel = New System.Windows.Forms.Label
        LowTempLabel = New System.Windows.Forms.Label
        High_TempLabel = New System.Windows.Forms.Label
        Low_TempLabel = New System.Windows.Forms.Label
        Me.Panel1.SuspendLayout()
        CType(Me.CurrentConditionsBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RedmondWeatherDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picConditions, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Five_Day_ForecastBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.DataRepeater1.ItemTemplate.SuspendLayout()
        Me.DataRepeater1.SuspendLayout()
        CType(Me.picTodaysForecast, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Todays_ForecastBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.DataRepeater2.ItemTemplate.SuspendLayout()
        Me.DataRepeater2.SuspendLayout()
        CType(Me.picForecast, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel4.SuspendLayout()
        Me.SuspendLayout()
        '
        'Feels_LikeLabel
        '
        Feels_LikeLabel.AutoSize = True
        Feels_LikeLabel.Location = New System.Drawing.Point(117, 34)
        Feels_LikeLabel.Name = "Feels_LikeLabel"
        Feels_LikeLabel.Size = New System.Drawing.Size(58, 13)
        Feels_LikeLabel.TabIndex = 6
        Feels_LikeLabel.Text = "Feels Like:"
        '
        'BarometerLabel
        '
        BarometerLabel.AutoSize = True
        BarometerLabel.Location = New System.Drawing.Point(11, 81)
        BarometerLabel.Name = "BarometerLabel"
        BarometerLabel.Size = New System.Drawing.Size(58, 13)
        BarometerLabel.TabIndex = 8
        BarometerLabel.Text = "Barometer:"
        BarometerLabel.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'DewpointLabel
        '
        DewpointLabel.AutoSize = True
        DewpointLabel.Location = New System.Drawing.Point(12, 107)
        DewpointLabel.Name = "DewpointLabel"
        DewpointLabel.Size = New System.Drawing.Size(55, 13)
        DewpointLabel.TabIndex = 10
        DewpointLabel.Text = "Dewpoint:"
        DewpointLabel.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'HumidityLabel
        '
        HumidityLabel.AutoSize = True
        HumidityLabel.Location = New System.Drawing.Point(17, 134)
        HumidityLabel.Name = "HumidityLabel"
        HumidityLabel.Size = New System.Drawing.Size(50, 13)
        HumidityLabel.TabIndex = 12
        HumidityLabel.Text = "Humidity:"
        HumidityLabel.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'VisibilityLabel
        '
        VisibilityLabel.AutoSize = True
        VisibilityLabel.Location = New System.Drawing.Point(21, 160)
        VisibilityLabel.Name = "VisibilityLabel"
        VisibilityLabel.Size = New System.Drawing.Size(46, 13)
        VisibilityLabel.TabIndex = 16
        VisibilityLabel.Text = "Visibility:"
        VisibilityLabel.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'WindSpeedLabel
        '
        WindSpeedLabel.AutoSize = True
        WindSpeedLabel.Location = New System.Drawing.Point(142, 82)
        WindSpeedLabel.Name = "WindSpeedLabel"
        WindSpeedLabel.Size = New System.Drawing.Size(35, 13)
        WindSpeedLabel.TabIndex = 18
        WindSpeedLabel.Text = "Wind:"
        WindSpeedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'UV_IndexLabel
        '
        UV_IndexLabel.AutoSize = True
        UV_IndexLabel.Location = New System.Drawing.Point(134, 160)
        UV_IndexLabel.Name = "UV_IndexLabel"
        UV_IndexLabel.Size = New System.Drawing.Size(54, 13)
        UV_IndexLabel.TabIndex = 26
        UV_IndexLabel.Text = "UV Index:"
        UV_IndexLabel.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'SunriseLabel1
        '
        SunriseLabel1.AutoSize = True
        SunriseLabel1.Location = New System.Drawing.Point(136, 107)
        SunriseLabel1.Name = "SunriseLabel1"
        SunriseLabel1.Size = New System.Drawing.Size(45, 13)
        SunriseLabel1.TabIndex = 28
        SunriseLabel1.Text = "Sunrise:"
        SunriseLabel1.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'SunsetLabel
        '
        SunsetLabel.AutoSize = True
        SunsetLabel.Location = New System.Drawing.Point(138, 134)
        SunsetLabel.Name = "SunsetLabel"
        SunsetLabel.Size = New System.Drawing.Size(43, 13)
        SunsetLabel.TabIndex = 29
        SunsetLabel.Text = "Sunset:"
        SunsetLabel.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label1
        '
        Label1.AutoSize = True
        Label1.Location = New System.Drawing.Point(85, 160)
        Label1.Name = "Label1"
        Label1.Size = New System.Drawing.Size(31, 13)
        Label1.TabIndex = 31
        Label1.Text = "Miles"
        '
        'HighTempLabel
        '
        HighTempLabel.AutoSize = True
        HighTempLabel.Location = New System.Drawing.Point(165, 5)
        HighTempLabel.Name = "HighTempLabel"
        HighTempLabel.Size = New System.Drawing.Size(20, 13)
        HighTempLabel.TabIndex = 4
        HighTempLabel.Text = "Hi:"
        '
        'LowTempLabel
        '
        LowTempLabel.AutoSize = True
        LowTempLabel.Location = New System.Drawing.Point(164, 22)
        LowTempLabel.Name = "LowTempLabel"
        LowTempLabel.Size = New System.Drawing.Size(22, 13)
        LowTempLabel.TabIndex = 6
        LowTempLabel.Text = "Lo:"
        '
        'High_TempLabel
        '
        High_TempLabel.AutoSize = True
        High_TempLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        High_TempLabel.Location = New System.Drawing.Point(30, 110)
        High_TempLabel.Name = "High_TempLabel"
        High_TempLabel.Size = New System.Drawing.Size(22, 15)
        High_TempLabel.TabIndex = 4
        High_TempLabel.Text = "Hi:"
        '
        'Low_TempLabel
        '
        Low_TempLabel.AutoSize = True
        Low_TempLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Low_TempLabel.Location = New System.Drawing.Point(30, 132)
        Low_TempLabel.Name = "Low_TempLabel"
        Low_TempLabel.Size = New System.Drawing.Size(24, 15)
        Low_TempLabel.TabIndex = 6
        Low_TempLabel.Text = "Lo:"
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.AliceBlue
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.SunsetTextBox)
        Me.Panel1.Controls.Add(Me.SunriseTextBox)
        Me.Panel1.Controls.Add(Me.TemperatureTextBox)
        Me.Panel1.Controls.Add(Me.TextBox6)
        Me.Panel1.Controls.Add(Me.TextBox2)
        Me.Panel1.Controls.Add(Label1)
        Me.Panel1.Controls.Add(SunsetLabel)
        Me.Panel1.Controls.Add(SunriseLabel1)
        Me.Panel1.Controls.Add(Me.picConditions)
        Me.Panel1.Controls.Add(Me.ConditionsTextBox)
        Me.Panel1.Controls.Add(Feels_LikeLabel)
        Me.Panel1.Controls.Add(Me.Feels_LikeTextBox)
        Me.Panel1.Controls.Add(BarometerLabel)
        Me.Panel1.Controls.Add(Me.BarometerTextBox)
        Me.Panel1.Controls.Add(DewpointLabel)
        Me.Panel1.Controls.Add(Me.DewpointTextBox)
        Me.Panel1.Controls.Add(HumidityLabel)
        Me.Panel1.Controls.Add(Me.HumidityTextBox)
        Me.Panel1.Controls.Add(VisibilityLabel)
        Me.Panel1.Controls.Add(Me.VisibilityTextBox)
        Me.Panel1.Controls.Add(WindSpeedLabel)
        Me.Panel1.Controls.Add(Me.WindSpeedTextBox)
        Me.Panel1.Controls.Add(Me.WindDirectionTextBox)
        Me.Panel1.Controls.Add(UV_IndexLabel)
        Me.Panel1.Controls.Add(Me.UV_IndexTextBox)
        Me.Panel1.Location = New System.Drawing.Point(16, 38)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(274, 246)
        Me.Panel1.TabIndex = 0
        '
        'SunsetTextBox
        '
        Me.SunsetTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.SunsetTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.SunsetTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.CurrentConditionsBindingSource, "Sunset", True))
        Me.SunsetTextBox.Location = New System.Drawing.Point(185, 133)
        Me.SunsetTextBox.Name = "SunsetTextBox"
        Me.SunsetTextBox.Size = New System.Drawing.Size(80, 13)
        Me.SunsetTextBox.TabIndex = 36
        '
        'CurrentConditionsBindingSource
        '
        Me.CurrentConditionsBindingSource.DataMember = "CurrentConditions"
        Me.CurrentConditionsBindingSource.DataSource = Me.RedmondWeatherDataSet
        '
        'RedmondWeatherDataSet
        '
        Me.RedmondWeatherDataSet.DataSetName = "RedmondWeatherDataSet"
        Me.RedmondWeatherDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'SunriseTextBox
        '
        Me.SunriseTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.SunriseTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.SunriseTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.CurrentConditionsBindingSource, "Sunrise", True))
        Me.SunriseTextBox.Location = New System.Drawing.Point(185, 107)
        Me.SunriseTextBox.Name = "SunriseTextBox"
        Me.SunriseTextBox.Size = New System.Drawing.Size(80, 13)
        Me.SunriseTextBox.TabIndex = 35
        '
        'TemperatureTextBox
        '
        Me.TemperatureTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.TemperatureTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TemperatureTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.CurrentConditionsBindingSource, "Temperature", True))
        Me.TemperatureTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TemperatureTextBox.Location = New System.Drawing.Point(120, 9)
        Me.TemperatureTextBox.Name = "TemperatureTextBox"
        Me.TemperatureTextBox.Size = New System.Drawing.Size(36, 22)
        Me.TemperatureTextBox.TabIndex = 34
        '
        'TextBox6
        '
        Me.TextBox6.BackColor = System.Drawing.Color.AliceBlue
        Me.TextBox6.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox6.Enabled = False
        Me.TextBox6.Location = New System.Drawing.Point(18, 186)
        Me.TextBox6.Multiline = True
        Me.TextBox6.Name = "TextBox6"
        Me.TextBox6.Size = New System.Drawing.Size(222, 38)
        Me.TextBox6.TabIndex = 33
        Me.TextBox6.Text = "Observed at Seattle, Seattle Boeing Field." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "All times shown are local to Redmond." & _
            "" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'TextBox2
        '
        Me.TextBox2.BackColor = System.Drawing.Color.AliceBlue
        Me.TextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox2.Location = New System.Drawing.Point(162, 8)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(22, 22)
        Me.TextBox2.TabIndex = 32
        Me.TextBox2.Text = "F"
        '
        'picConditions
        '
        Me.picConditions.BackColor = System.Drawing.Color.AliceBlue
        Me.picConditions.Location = New System.Drawing.Point(11, 7)
        Me.picConditions.Name = "picConditions"
        Me.picConditions.Size = New System.Drawing.Size(55, 45)
        Me.picConditions.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picConditions.TabIndex = 28
        Me.picConditions.TabStop = False
        '
        'ConditionsTextBox
        '
        Me.ConditionsTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.ConditionsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.ConditionsTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.CurrentConditionsBindingSource, "Conditions", True))
        Me.ConditionsTextBox.Location = New System.Drawing.Point(11, 55)
        Me.ConditionsTextBox.Name = "ConditionsTextBox"
        Me.ConditionsTextBox.Size = New System.Drawing.Size(58, 13)
        Me.ConditionsTextBox.TabIndex = 3
        Me.ConditionsTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Feels_LikeTextBox
        '
        Me.Feels_LikeTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.Feels_LikeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.Feels_LikeTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.CurrentConditionsBindingSource, "Feels_Like", True))
        Me.Feels_LikeTextBox.Location = New System.Drawing.Point(180, 34)
        Me.Feels_LikeTextBox.Name = "Feels_LikeTextBox"
        Me.Feels_LikeTextBox.Size = New System.Drawing.Size(21, 13)
        Me.Feels_LikeTextBox.TabIndex = 7
        '
        'BarometerTextBox
        '
        Me.BarometerTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.BarometerTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.BarometerTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.CurrentConditionsBindingSource, "Barometer", True))
        Me.BarometerTextBox.Location = New System.Drawing.Point(72, 82)
        Me.BarometerTextBox.Name = "BarometerTextBox"
        Me.BarometerTextBox.Size = New System.Drawing.Size(44, 13)
        Me.BarometerTextBox.TabIndex = 9
        '
        'DewpointTextBox
        '
        Me.DewpointTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.DewpointTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DewpointTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.CurrentConditionsBindingSource, "Dewpoint", True))
        Me.DewpointTextBox.Location = New System.Drawing.Point(72, 107)
        Me.DewpointTextBox.Name = "DewpointTextBox"
        Me.DewpointTextBox.Size = New System.Drawing.Size(22, 13)
        Me.DewpointTextBox.TabIndex = 11
        '
        'HumidityTextBox
        '
        Me.HumidityTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.HumidityTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.HumidityTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.CurrentConditionsBindingSource, "Humidity", True))
        Me.HumidityTextBox.Location = New System.Drawing.Point(72, 133)
        Me.HumidityTextBox.Name = "HumidityTextBox"
        Me.HumidityTextBox.Size = New System.Drawing.Size(22, 13)
        Me.HumidityTextBox.TabIndex = 13
        '
        'VisibilityTextBox
        '
        Me.VisibilityTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.VisibilityTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.VisibilityTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.CurrentConditionsBindingSource, "Visibility", True))
        Me.VisibilityTextBox.Location = New System.Drawing.Point(72, 160)
        Me.VisibilityTextBox.Name = "VisibilityTextBox"
        Me.VisibilityTextBox.Size = New System.Drawing.Size(22, 13)
        Me.VisibilityTextBox.TabIndex = 17
        '
        'WindSpeedTextBox
        '
        Me.WindSpeedTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.WindSpeedTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.WindSpeedTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.CurrentConditionsBindingSource, "WindSpeed", True))
        Me.WindSpeedTextBox.Location = New System.Drawing.Point(180, 81)
        Me.WindSpeedTextBox.Name = "WindSpeedTextBox"
        Me.WindSpeedTextBox.Size = New System.Drawing.Size(29, 13)
        Me.WindSpeedTextBox.TabIndex = 19
        '
        'WindDirectionTextBox
        '
        Me.WindDirectionTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.WindDirectionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.WindDirectionTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.CurrentConditionsBindingSource, "WindDirection", True))
        Me.WindDirectionTextBox.Location = New System.Drawing.Point(219, 82)
        Me.WindDirectionTextBox.Name = "WindDirectionTextBox"
        Me.WindDirectionTextBox.Size = New System.Drawing.Size(21, 13)
        Me.WindDirectionTextBox.TabIndex = 21
        '
        'UV_IndexTextBox
        '
        Me.UV_IndexTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.UV_IndexTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.UV_IndexTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.CurrentConditionsBindingSource, "UV_Index", True))
        Me.UV_IndexTextBox.Location = New System.Drawing.Point(190, 160)
        Me.UV_IndexTextBox.Name = "UV_IndexTextBox"
        Me.UV_IndexTextBox.Size = New System.Drawing.Size(21, 13)
        Me.UV_IndexTextBox.TabIndex = 27
        '
        'Five_DayTextBox
        '
        Me.Five_DayTextBox.BackColor = System.Drawing.Color.White
        Me.Five_DayTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.Five_DayTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.Five_Day_ForecastBindingSource, "Day", True))
        Me.Five_DayTextBox.Location = New System.Drawing.Point(30, 28)
        Me.Five_DayTextBox.Name = "Five_DayTextBox"
        Me.Five_DayTextBox.Size = New System.Drawing.Size(52, 13)
        Me.Five_DayTextBox.TabIndex = 1
        Me.Five_DayTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Five_Day_ForecastBindingSource
        '
        Me.Five_Day_ForecastBindingSource.DataMember = "Five_Day_Forecast"
        Me.Five_Day_ForecastBindingSource.DataSource = Me.RedmondWeatherDataSet
        '
        'TextBox1
        '
        Me.TextBox1.BackColor = System.Drawing.Color.AliceBlue
        Me.TextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox1.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox1.ForeColor = System.Drawing.Color.Black
        Me.TextBox1.Location = New System.Drawing.Point(10, 3)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ReadOnly = True
        Me.TextBox1.Size = New System.Drawing.Size(164, 15)
        Me.TextBox1.TabIndex = 1
        Me.TextBox1.Text = "Current Conditions as of:"
        Me.TextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.AliceBlue
        Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel2.Controls.Add(Me.DayTextBox)
        Me.Panel2.Controls.Add(Me.TextBox1)
        Me.Panel2.Location = New System.Drawing.Point(16, 7)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(274, 31)
        Me.Panel2.TabIndex = 2
        '
        'DayTextBox
        '
        Me.DayTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.DayTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DayTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.CurrentConditionsBindingSource, "Day", True))
        Me.DayTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DayTextBox.Location = New System.Drawing.Point(181, 3)
        Me.DayTextBox.Name = "DayTextBox"
        Me.DayTextBox.Size = New System.Drawing.Size(92, 15)
        Me.DayTextBox.TabIndex = 2
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.AliceBlue
        Me.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel3.Controls.Add(Me.TextBox3)
        Me.Panel3.Location = New System.Drawing.Point(288, 7)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(270, 31)
        Me.Panel3.TabIndex = 3
        '
        'TextBox3
        '
        Me.TextBox3.BackColor = System.Drawing.Color.AliceBlue
        Me.TextBox3.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox3.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox3.ForeColor = System.Drawing.Color.Black
        Me.TextBox3.Location = New System.Drawing.Point(11, 5)
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.ReadOnly = True
        Me.TextBox3.Size = New System.Drawing.Size(227, 15)
        Me.TextBox3.TabIndex = 1
        Me.TextBox3.Text = "Today's forecast"
        Me.TextBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'DataRepeater1
        '
        Me.DataRepeater1.BackColor = System.Drawing.Color.AliceBlue
        Me.DataRepeater1.ItemHeaderVisible = False
        '
        'DataRepeater1.ItemTemplate
        '
        Me.DataRepeater1.ItemTemplate.BackColor = System.Drawing.Color.AliceBlue
        Me.DataRepeater1.ItemTemplate.Controls.Add(Me.picTodaysForecast)
        Me.DataRepeater1.ItemTemplate.Controls.Add(Me.PeriodTextBox)
        Me.DataRepeater1.ItemTemplate.Controls.Add(Me.ForecastTextBox)
        Me.DataRepeater1.ItemTemplate.Controls.Add(HighTempLabel)
        Me.DataRepeater1.ItemTemplate.Controls.Add(Me.HighTempTextBox)
        Me.DataRepeater1.ItemTemplate.Controls.Add(LowTempLabel)
        Me.DataRepeater1.ItemTemplate.Controls.Add(Me.LowTempTextBox)
        Me.DataRepeater1.ItemTemplate.Margin = New System.Windows.Forms.Padding(0)
        Me.DataRepeater1.ItemTemplate.Size = New System.Drawing.Size(262, 60)
        Me.DataRepeater1.Location = New System.Drawing.Point(288, 38)
        Me.DataRepeater1.Name = "DataRepeater1"
        Me.DataRepeater1.Size = New System.Drawing.Size(270, 246)
        Me.DataRepeater1.TabIndex = 4
        Me.DataRepeater1.Text = "DataRepeater1"
        '
        'picTodaysForecast
        '
        Me.picTodaysForecast.BackColor = System.Drawing.Color.AliceBlue
        Me.picTodaysForecast.InitialImage = Nothing
        Me.picTodaysForecast.Location = New System.Drawing.Point(80, 6)
        Me.picTodaysForecast.Name = "picTodaysForecast"
        Me.picTodaysForecast.Size = New System.Drawing.Size(40, 30)
        Me.picTodaysForecast.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picTodaysForecast.TabIndex = 29
        Me.picTodaysForecast.TabStop = False
        '
        'PeriodTextBox
        '
        Me.PeriodTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.PeriodTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.PeriodTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.Todays_ForecastBindingSource, "Period", True))
        Me.PeriodTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PeriodTextBox.Location = New System.Drawing.Point(2, 2)
        Me.PeriodTextBox.Name = "PeriodTextBox"
        Me.PeriodTextBox.Size = New System.Drawing.Size(66, 15)
        Me.PeriodTextBox.TabIndex = 1
        '
        'Todays_ForecastBindingSource
        '
        Me.Todays_ForecastBindingSource.DataMember = "Todays_Forecast"
        Me.Todays_ForecastBindingSource.DataSource = Me.RedmondWeatherDataSet
        Me.Todays_ForecastBindingSource.Sort = "Order"
        '
        'ForecastTextBox
        '
        Me.ForecastTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.ForecastTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.ForecastTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.Todays_ForecastBindingSource, "Forecast", True))
        Me.ForecastTextBox.Location = New System.Drawing.Point(68, 38)
        Me.ForecastTextBox.Name = "ForecastTextBox"
        Me.ForecastTextBox.Size = New System.Drawing.Size(118, 13)
        Me.ForecastTextBox.TabIndex = 3
        Me.ForecastTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'HighTempTextBox
        '
        Me.HighTempTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.HighTempTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.HighTempTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.Todays_ForecastBindingSource, "HighTemp", True))
        Me.HighTempTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HighTempTextBox.Location = New System.Drawing.Point(186, 4)
        Me.HighTempTextBox.Name = "HighTempTextBox"
        Me.HighTempTextBox.Size = New System.Drawing.Size(29, 14)
        Me.HighTempTextBox.TabIndex = 5
        '
        'LowTempTextBox
        '
        Me.LowTempTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.LowTempTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.LowTempTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.Todays_ForecastBindingSource, "LowTemp", True))
        Me.LowTempTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LowTempTextBox.Location = New System.Drawing.Point(186, 21)
        Me.LowTempTextBox.Name = "LowTempTextBox"
        Me.LowTempTextBox.Size = New System.Drawing.Size(29, 14)
        Me.LowTempTextBox.TabIndex = 7
        '
        'DataRepeater2
        '
        Me.DataRepeater2.ItemHeaderVisible = False
        '
        'DataRepeater2.ItemTemplate
        '
        Me.DataRepeater2.ItemTemplate.BackColor = System.Drawing.Color.White
        Me.DataRepeater2.ItemTemplate.Controls.Add(Me.txtWeekDay)
        Me.DataRepeater2.ItemTemplate.Controls.Add(Me.picForecast)
        Me.DataRepeater2.ItemTemplate.Controls.Add(Me.Five_DayTextBox)
        Me.DataRepeater2.ItemTemplate.Controls.Add(Me.ForecastTextBox1)
        Me.DataRepeater2.ItemTemplate.Controls.Add(High_TempLabel)
        Me.DataRepeater2.ItemTemplate.Controls.Add(Me.High_TempTextBox)
        Me.DataRepeater2.ItemTemplate.Controls.Add(Low_TempLabel)
        Me.DataRepeater2.ItemTemplate.Controls.Add(Me.Low_TempTextBox)
        Me.DataRepeater2.ItemTemplate.Size = New System.Drawing.Size(127, 152)
        Me.DataRepeater2.LayoutStyle = Microsoft.VisualBasic.PowerPacks.DataRepeaterLayoutStyles.Horizontal
        Me.DataRepeater2.Location = New System.Drawing.Point(15, 318)
        Me.DataRepeater2.Name = "DataRepeater2"
        Me.DataRepeater2.Size = New System.Drawing.Size(542, 160)
        Me.DataRepeater2.TabIndex = 5
        Me.DataRepeater2.Text = "DataRepeater2"
        '
        'txtWeekDay
        '
        Me.txtWeekDay.BackColor = System.Drawing.Color.White
        Me.txtWeekDay.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtWeekDay.Location = New System.Drawing.Point(24, 4)
        Me.txtWeekDay.Name = "txtWeekDay"
        Me.txtWeekDay.Size = New System.Drawing.Size(72, 13)
        Me.txtWeekDay.TabIndex = 31
        Me.txtWeekDay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'picForecast
        '
        Me.picForecast.BackColor = System.Drawing.Color.White
        Me.picForecast.InitialImage = Nothing
        Me.picForecast.Location = New System.Drawing.Point(38, 44)
        Me.picForecast.Name = "picForecast"
        Me.picForecast.Size = New System.Drawing.Size(40, 30)
        Me.picForecast.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picForecast.TabIndex = 30
        Me.picForecast.TabStop = False
        '
        'ForecastTextBox1
        '
        Me.ForecastTextBox1.BackColor = System.Drawing.Color.White
        Me.ForecastTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.ForecastTextBox1.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.Five_Day_ForecastBindingSource, "Forecast", True))
        Me.ForecastTextBox1.Location = New System.Drawing.Point(22, 80)
        Me.ForecastTextBox1.Name = "ForecastTextBox1"
        Me.ForecastTextBox1.Size = New System.Drawing.Size(72, 13)
        Me.ForecastTextBox1.TabIndex = 3
        Me.ForecastTextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'High_TempTextBox
        '
        Me.High_TempTextBox.BackColor = System.Drawing.Color.White
        Me.High_TempTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.High_TempTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.Five_Day_ForecastBindingSource, "High_Temp", True))
        Me.High_TempTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.High_TempTextBox.Location = New System.Drawing.Point(56, 110)
        Me.High_TempTextBox.Name = "High_TempTextBox"
        Me.High_TempTextBox.Size = New System.Drawing.Size(26, 14)
        Me.High_TempTextBox.TabIndex = 5
        '
        'Low_TempTextBox
        '
        Me.Low_TempTextBox.BackColor = System.Drawing.Color.White
        Me.Low_TempTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.Low_TempTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.Five_Day_ForecastBindingSource, "Low_Temp", True))
        Me.Low_TempTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Low_TempTextBox.Location = New System.Drawing.Point(56, 132)
        Me.Low_TempTextBox.Name = "Low_TempTextBox"
        Me.Low_TempTextBox.Size = New System.Drawing.Size(26, 14)
        Me.Low_TempTextBox.TabIndex = 7
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.AliceBlue
        Me.Panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel4.Controls.Add(Me.LinkLabel1)
        Me.Panel4.Controls.Add(Me.TextBox4)
        Me.Panel4.Location = New System.Drawing.Point(15, 287)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(543, 31)
        Me.Panel4.TabIndex = 6
        '
        'LinkLabel1
        '
        Me.LinkLabel1.AutoSize = True
        Me.LinkLabel1.Location = New System.Drawing.Point(124, 10)
        Me.LinkLabel1.Name = "LinkLabel1"
        Me.LinkLabel1.Size = New System.Drawing.Size(45, 13)
        Me.LinkLabel1.TabIndex = 2
        Me.LinkLabel1.TabStop = True
        Me.LinkLabel1.Text = "(Details)"
        '
        'TextBox4
        '
        Me.TextBox4.BackColor = System.Drawing.Color.AliceBlue
        Me.TextBox4.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox4.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox4.ForeColor = System.Drawing.Color.Black
        Me.TextBox4.Location = New System.Drawing.Point(4, 8)
        Me.TextBox4.Name = "TextBox4"
        Me.TextBox4.ReadOnly = True
        Me.TextBox4.Size = New System.Drawing.Size(118, 15)
        Me.TextBox4.TabIndex = 1
        Me.TextBox4.Text = "Five-day forecast"
        '
        'CurrentConditionsTableAdapter
        '
        Me.CurrentConditionsTableAdapter.ClearBeforeFill = True
        '
        'Todays_ForecastTableAdapter
        '
        Me.Todays_ForecastTableAdapter.ClearBeforeFill = True
        '
        'Five_Day_ForecastTableAdapter
        '
        Me.Five_Day_ForecastTableAdapter.ClearBeforeFill = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label2.Location = New System.Drawing.Point(66, 227)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(109, 13)
        Me.Label2.TabIndex = 37
        Me.Label2.Text = "Powered by FORECA"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(576, 484)
        Me.Controls.Add(Me.Panel4)
        Me.Controls.Add(Me.DataRepeater2)
        Me.Controls.Add(Me.DataRepeater1)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "Form1"
        Me.Text = "Redmond, WA"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.CurrentConditionsBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RedmondWeatherDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picConditions, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Five_Day_ForecastBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.DataRepeater1.ItemTemplate.ResumeLayout(False)
        Me.DataRepeater1.ItemTemplate.PerformLayout()
        Me.DataRepeater1.ResumeLayout(False)
        CType(Me.picTodaysForecast, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Todays_ForecastBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.DataRepeater2.ItemTemplate.ResumeLayout(False)
        Me.DataRepeater2.ItemTemplate.PerformLayout()
        Me.DataRepeater2.ResumeLayout(False)
        CType(Me.picForecast, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents RedmondWeatherDataSet As WindowsApplication2.RedmondWeatherDataSet
    Friend WithEvents CurrentConditionsBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents CurrentConditionsTableAdapter As WindowsApplication2.RedmondWeatherDataSetTableAdapters.CurrentConditionsTableAdapter
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents ConditionsTextBox As System.Windows.Forms.TextBox
    Friend WithEvents Feels_LikeTextBox As System.Windows.Forms.TextBox
    Friend WithEvents BarometerTextBox As System.Windows.Forms.TextBox
    Friend WithEvents DewpointTextBox As System.Windows.Forms.TextBox
    Friend WithEvents HumidityTextBox As System.Windows.Forms.TextBox
    Friend WithEvents VisibilityTextBox As System.Windows.Forms.TextBox
    Friend WithEvents WindSpeedTextBox As System.Windows.Forms.TextBox
    Friend WithEvents WindDirectionTextBox As System.Windows.Forms.TextBox
    Friend WithEvents UV_IndexTextBox As System.Windows.Forms.TextBox
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents picConditions As System.Windows.Forms.PictureBox
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents TextBox3 As System.Windows.Forms.TextBox
    Friend WithEvents DataRepeater1 As Microsoft.VisualBasic.PowerPacks.DataRepeater
    Friend WithEvents Todays_ForecastBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents Todays_ForecastTableAdapter As WindowsApplication2.RedmondWeatherDataSetTableAdapters.Todays_ForecastTableAdapter
    Friend WithEvents PeriodTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ForecastTextBox As System.Windows.Forms.TextBox
    Friend WithEvents HighTempTextBox As System.Windows.Forms.TextBox
    Friend WithEvents LowTempTextBox As System.Windows.Forms.TextBox
    Friend WithEvents picTodaysForecast As System.Windows.Forms.PictureBox
    Friend WithEvents TextBox2 As System.Windows.Forms.TextBox
    Friend WithEvents DataRepeater2 As Microsoft.VisualBasic.PowerPacks.DataRepeater
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents TextBox4 As System.Windows.Forms.TextBox
    Friend WithEvents Five_Day_ForecastBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents Five_Day_ForecastTableAdapter As WindowsApplication2.RedmondWeatherDataSetTableAdapters.Five_Day_ForecastTableAdapter
    Friend WithEvents txtWeekDay As System.Windows.Forms.TextBox
    Friend WithEvents picForecast As System.Windows.Forms.PictureBox
    Friend WithEvents Five_DayTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ForecastTextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents High_TempTextBox As System.Windows.Forms.TextBox
    Friend WithEvents Low_TempTextBox As System.Windows.Forms.TextBox
    Friend WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel
    Friend WithEvents TextBox6 As System.Windows.Forms.TextBox
    Friend WithEvents TemperatureTextBox As System.Windows.Forms.TextBox
    Friend WithEvents DayTextBox As System.Windows.Forms.TextBox
    Friend WithEvents SunsetTextBox As System.Windows.Forms.TextBox
    Friend WithEvents SunriseTextBox As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label

End Class
