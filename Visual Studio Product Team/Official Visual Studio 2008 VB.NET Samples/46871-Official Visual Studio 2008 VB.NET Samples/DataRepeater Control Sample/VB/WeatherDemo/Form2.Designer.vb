<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form2
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
        Dim Day_Forecast_DetailsLabel As System.Windows.Forms.Label
        Dim Night_Forecast_DetailsLabel As System.Windows.Forms.Label
        Dim High_TempLabel As System.Windows.Forms.Label
        Dim Low_TempLabel As System.Windows.Forms.Label
        Me.DataRepeater1 = New Microsoft.VisualBasic.PowerPacks.DataRepeater
        Me.txtWeekDay = New System.Windows.Forms.TextBox
        Me.picForecast = New System.Windows.Forms.PictureBox
        Me.DayTextBox = New System.Windows.Forms.TextBox
        Me.Detailed_TenDay_ForecastBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.RedmondWeatherDataSet = New WindowsApplication2.RedmondWeatherDataSet
        Me.ForecastTextBox = New System.Windows.Forms.TextBox
        Me.Day_Forecast_DetailsTextBox = New System.Windows.Forms.TextBox
        Me.Night_Forecast_DetailsTextBox = New System.Windows.Forms.TextBox
        Me.High_TempTextBox = New System.Windows.Forms.TextBox
        Me.Low_TempTextBox = New System.Windows.Forms.TextBox
        Me.Day_Precip_ChanceTextBox = New System.Windows.Forms.TextBox
        Me.Night_Precip_ChanceTextBox = New System.Windows.Forms.TextBox
        Me.General_Forecast_DetailsTextBox = New System.Windows.Forms.TextBox
        Me.ShapeContainer1 = New Microsoft.VisualBasic.PowerPacks.ShapeContainer
        Me.LineShape3 = New Microsoft.VisualBasic.PowerPacks.LineShape
        Me.LineShape2 = New Microsoft.VisualBasic.PowerPacks.LineShape
        Me.LineShape1 = New Microsoft.VisualBasic.PowerPacks.LineShape
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.Label1 = New System.Windows.Forms.Label
        Me.Panel2 = New System.Windows.Forms.Panel
        Me.Label2 = New System.Windows.Forms.Label
        Me.Panel3 = New System.Windows.Forms.Panel
        Me.Panel4 = New System.Windows.Forms.Panel
        Me.Label4 = New System.Windows.Forms.Label
        Me.Panel5 = New System.Windows.Forms.Panel
        Me.Label3 = New System.Windows.Forms.Label
        Me.Detailed_TenDay_ForecastTableAdapter = New WindowsApplication2.RedmondWeatherDataSetTableAdapters.Detailed_TenDay_ForecastTableAdapter
        Me.TextBox1 = New System.Windows.Forms.TextBox
        Day_Forecast_DetailsLabel = New System.Windows.Forms.Label
        Night_Forecast_DetailsLabel = New System.Windows.Forms.Label
        High_TempLabel = New System.Windows.Forms.Label
        Low_TempLabel = New System.Windows.Forms.Label
        Me.DataRepeater1.ItemTemplate.SuspendLayout()
        Me.DataRepeater1.SuspendLayout()
        CType(Me.picForecast, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Detailed_TenDay_ForecastBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RedmondWeatherDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.SuspendLayout()
        '
        'Day_Forecast_DetailsLabel
        '
        Day_Forecast_DetailsLabel.AutoSize = True
        Day_Forecast_DetailsLabel.BackColor = System.Drawing.Color.AliceBlue
        Day_Forecast_DetailsLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Day_Forecast_DetailsLabel.Location = New System.Drawing.Point(224, 6)
        Day_Forecast_DetailsLabel.Name = "Day_Forecast_DetailsLabel"
        Day_Forecast_DetailsLabel.Size = New System.Drawing.Size(33, 13)
        Day_Forecast_DetailsLabel.TabIndex = 4
        Day_Forecast_DetailsLabel.Text = "Day:"
        '
        'Night_Forecast_DetailsLabel
        '
        Night_Forecast_DetailsLabel.AutoSize = True
        Night_Forecast_DetailsLabel.BackColor = System.Drawing.Color.AliceBlue
        Night_Forecast_DetailsLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Night_Forecast_DetailsLabel.Location = New System.Drawing.Point(220, 42)
        Night_Forecast_DetailsLabel.Name = "Night_Forecast_DetailsLabel"
        Night_Forecast_DetailsLabel.Size = New System.Drawing.Size(41, 13)
        Night_Forecast_DetailsLabel.TabIndex = 6
        Night_Forecast_DetailsLabel.Text = "Night:"
        '
        'High_TempLabel
        '
        High_TempLabel.AutoSize = True
        High_TempLabel.BackColor = System.Drawing.Color.AliceBlue
        High_TempLabel.Location = New System.Drawing.Point(92, 60)
        High_TempLabel.Name = "High_TempLabel"
        High_TempLabel.Size = New System.Drawing.Size(20, 13)
        High_TempLabel.TabIndex = 8
        High_TempLabel.Text = "Hi:"
        '
        'Low_TempLabel
        '
        Low_TempLabel.AutoSize = True
        Low_TempLabel.BackColor = System.Drawing.Color.AliceBlue
        Low_TempLabel.Location = New System.Drawing.Point(148, 60)
        Low_TempLabel.Name = "Low_TempLabel"
        Low_TempLabel.Size = New System.Drawing.Size(30, 13)
        Low_TempLabel.TabIndex = 10
        Low_TempLabel.Text = "Low:"
        '
        'DataRepeater1
        '
        Me.DataRepeater1.BackColor = System.Drawing.Color.AliceBlue
        Me.DataRepeater1.ItemHeaderVisible = False
        '
        'DataRepeater1.ItemTemplate
        '
        Me.DataRepeater1.ItemTemplate.BackColor = System.Drawing.Color.White
        Me.DataRepeater1.ItemTemplate.Controls.Add(Me.txtWeekDay)
        Me.DataRepeater1.ItemTemplate.Controls.Add(Me.picForecast)
        Me.DataRepeater1.ItemTemplate.Controls.Add(Me.DayTextBox)
        Me.DataRepeater1.ItemTemplate.Controls.Add(Me.ForecastTextBox)
        Me.DataRepeater1.ItemTemplate.Controls.Add(Day_Forecast_DetailsLabel)
        Me.DataRepeater1.ItemTemplate.Controls.Add(Me.Day_Forecast_DetailsTextBox)
        Me.DataRepeater1.ItemTemplate.Controls.Add(Night_Forecast_DetailsLabel)
        Me.DataRepeater1.ItemTemplate.Controls.Add(Me.Night_Forecast_DetailsTextBox)
        Me.DataRepeater1.ItemTemplate.Controls.Add(High_TempLabel)
        Me.DataRepeater1.ItemTemplate.Controls.Add(Me.High_TempTextBox)
        Me.DataRepeater1.ItemTemplate.Controls.Add(Low_TempLabel)
        Me.DataRepeater1.ItemTemplate.Controls.Add(Me.Low_TempTextBox)
        Me.DataRepeater1.ItemTemplate.Controls.Add(Me.Day_Precip_ChanceTextBox)
        Me.DataRepeater1.ItemTemplate.Controls.Add(Me.Night_Precip_ChanceTextBox)
        Me.DataRepeater1.ItemTemplate.Controls.Add(Me.General_Forecast_DetailsTextBox)
        Me.DataRepeater1.ItemTemplate.Controls.Add(Me.ShapeContainer1)
        Me.DataRepeater1.ItemTemplate.Margin = New System.Windows.Forms.Padding(0)
        Me.DataRepeater1.ItemTemplate.Size = New System.Drawing.Size(564, 85)
        Me.DataRepeater1.Location = New System.Drawing.Point(16, 30)
        Me.DataRepeater1.Name = "DataRepeater1"
        Me.DataRepeater1.Size = New System.Drawing.Size(572, 770)
        Me.DataRepeater1.TabIndex = 0
        Me.DataRepeater1.Text = "DataRepeater1"
        '
        'txtWeekDay
        '
        Me.txtWeekDay.BackColor = System.Drawing.Color.AliceBlue
        Me.txtWeekDay.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtWeekDay.Location = New System.Drawing.Point(6, 8)
        Me.txtWeekDay.Name = "txtWeekDay"
        Me.txtWeekDay.Size = New System.Drawing.Size(72, 13)
        Me.txtWeekDay.TabIndex = 32
        Me.txtWeekDay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'picForecast
        '
        Me.picForecast.BackColor = System.Drawing.Color.AliceBlue
        Me.picForecast.Location = New System.Drawing.Point(116, 0)
        Me.picForecast.Name = "picForecast"
        Me.picForecast.Size = New System.Drawing.Size(58, 32)
        Me.picForecast.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picForecast.TabIndex = 29
        Me.picForecast.TabStop = False
        '
        'DayTextBox
        '
        Me.DayTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.DayTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DayTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.Detailed_TenDay_ForecastBindingSource, "Day", True))
        Me.DayTextBox.Location = New System.Drawing.Point(12, 32)
        Me.DayTextBox.Name = "DayTextBox"
        Me.DayTextBox.Size = New System.Drawing.Size(58, 13)
        Me.DayTextBox.TabIndex = 1
        '
        'Detailed_TenDay_ForecastBindingSource
        '
        Me.Detailed_TenDay_ForecastBindingSource.DataMember = "Detailed_TenDay_Forecast"
        Me.Detailed_TenDay_ForecastBindingSource.DataSource = Me.RedmondWeatherDataSet
        Me.Detailed_TenDay_ForecastBindingSource.Sort = "Order"
        '
        'RedmondWeatherDataSet
        '
        Me.RedmondWeatherDataSet.DataSetName = "RedmondWeatherDataSet"
        Me.RedmondWeatherDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'ForecastTextBox
        '
        Me.ForecastTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.ForecastTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.ForecastTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.Detailed_TenDay_ForecastBindingSource, "Forecast", True))
        Me.ForecastTextBox.Location = New System.Drawing.Point(108, 36)
        Me.ForecastTextBox.Name = "ForecastTextBox"
        Me.ForecastTextBox.Size = New System.Drawing.Size(76, 13)
        Me.ForecastTextBox.TabIndex = 3
        Me.ForecastTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Day_Forecast_DetailsTextBox
        '
        Me.Day_Forecast_DetailsTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.Day_Forecast_DetailsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.Day_Forecast_DetailsTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.Detailed_TenDay_ForecastBindingSource, "Day_Forecast_Details", True))
        Me.Day_Forecast_DetailsTextBox.Location = New System.Drawing.Point(266, 8)
        Me.Day_Forecast_DetailsTextBox.Multiline = True
        Me.Day_Forecast_DetailsTextBox.Name = "Day_Forecast_DetailsTextBox"
        Me.Day_Forecast_DetailsTextBox.Size = New System.Drawing.Size(228, 26)
        Me.Day_Forecast_DetailsTextBox.TabIndex = 5
        '
        'Night_Forecast_DetailsTextBox
        '
        Me.Night_Forecast_DetailsTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.Night_Forecast_DetailsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.Night_Forecast_DetailsTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.Detailed_TenDay_ForecastBindingSource, "Night_Forecast_Details", True))
        Me.Night_Forecast_DetailsTextBox.Location = New System.Drawing.Point(264, 44)
        Me.Night_Forecast_DetailsTextBox.Multiline = True
        Me.Night_Forecast_DetailsTextBox.Name = "Night_Forecast_DetailsTextBox"
        Me.Night_Forecast_DetailsTextBox.Size = New System.Drawing.Size(232, 26)
        Me.Night_Forecast_DetailsTextBox.TabIndex = 7
        '
        'High_TempTextBox
        '
        Me.High_TempTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.High_TempTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.High_TempTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.Detailed_TenDay_ForecastBindingSource, "High_Temp", True))
        Me.High_TempTextBox.Location = New System.Drawing.Point(116, 60)
        Me.High_TempTextBox.Name = "High_TempTextBox"
        Me.High_TempTextBox.Size = New System.Drawing.Size(29, 13)
        Me.High_TempTextBox.TabIndex = 9
        '
        'Low_TempTextBox
        '
        Me.Low_TempTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.Low_TempTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.Low_TempTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.Detailed_TenDay_ForecastBindingSource, "Low_Temp", True))
        Me.Low_TempTextBox.Location = New System.Drawing.Point(180, 60)
        Me.Low_TempTextBox.Name = "Low_TempTextBox"
        Me.Low_TempTextBox.Size = New System.Drawing.Size(29, 13)
        Me.Low_TempTextBox.TabIndex = 11
        '
        'Day_Precip_ChanceTextBox
        '
        Me.Day_Precip_ChanceTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.Day_Precip_ChanceTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.Day_Precip_ChanceTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.Detailed_TenDay_ForecastBindingSource, "Day_Precip_Chance", True))
        Me.Day_Precip_ChanceTextBox.Location = New System.Drawing.Point(524, 6)
        Me.Day_Precip_ChanceTextBox.Name = "Day_Precip_ChanceTextBox"
        Me.Day_Precip_ChanceTextBox.Size = New System.Drawing.Size(29, 13)
        Me.Day_Precip_ChanceTextBox.TabIndex = 13
        '
        'Night_Precip_ChanceTextBox
        '
        Me.Night_Precip_ChanceTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.Night_Precip_ChanceTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.Night_Precip_ChanceTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.Detailed_TenDay_ForecastBindingSource, "Night_Precip_Chance", True))
        Me.Night_Precip_ChanceTextBox.Location = New System.Drawing.Point(524, 46)
        Me.Night_Precip_ChanceTextBox.Name = "Night_Precip_ChanceTextBox"
        Me.Night_Precip_ChanceTextBox.Size = New System.Drawing.Size(29, 13)
        Me.Night_Precip_ChanceTextBox.TabIndex = 15
        '
        'General_Forecast_DetailsTextBox
        '
        Me.General_Forecast_DetailsTextBox.BackColor = System.Drawing.Color.AliceBlue
        Me.General_Forecast_DetailsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.General_Forecast_DetailsTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.Detailed_TenDay_ForecastBindingSource, "General_Forecast_Details", True))
        Me.General_Forecast_DetailsTextBox.Location = New System.Drawing.Point(266, 6)
        Me.General_Forecast_DetailsTextBox.Multiline = True
        Me.General_Forecast_DetailsTextBox.Name = "General_Forecast_DetailsTextBox"
        Me.General_Forecast_DetailsTextBox.Size = New System.Drawing.Size(230, 34)
        Me.General_Forecast_DetailsTextBox.TabIndex = 17
        '
        'ShapeContainer1
        '
        Me.ShapeContainer1.Location = New System.Drawing.Point(0, 0)
        Me.ShapeContainer1.Margin = New System.Windows.Forms.Padding(0)
        Me.ShapeContainer1.Name = "ShapeContainer1"
        Me.ShapeContainer1.Shapes.AddRange(New Microsoft.VisualBasic.PowerPacks.Shape() {Me.LineShape3, Me.LineShape2, Me.LineShape1})
        Me.ShapeContainer1.Size = New System.Drawing.Size(560, 80)
        Me.ShapeContainer1.TabIndex = 30
        Me.ShapeContainer1.TabStop = False
        '
        'LineShape3
        '
        Me.LineShape3.BorderColor = System.Drawing.Color.DeepSkyBlue
        Me.LineShape3.Name = "LineShape3"
        Me.LineShape3.X1 = 504
        Me.LineShape3.X2 = 504
        Me.LineShape3.Y1 = 0
        Me.LineShape3.Y2 = 85
        '
        'LineShape2
        '
        Me.LineShape2.BorderColor = System.Drawing.Color.DeepSkyBlue
        Me.LineShape2.Name = "LineShape2"
        Me.LineShape2.X1 = 215
        Me.LineShape2.X2 = 215
        Me.LineShape2.Y1 = 0
        Me.LineShape2.Y2 = 85
        '
        'LineShape1
        '
        Me.LineShape1.BorderColor = System.Drawing.Color.DeepSkyBlue
        Me.LineShape1.Name = "LineShape1"
        Me.LineShape1.X1 = 88
        Me.LineShape1.X2 = 88
        Me.LineShape1.Y1 = 0
        Me.LineShape1.Y2 = 85
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.AliceBlue
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Location = New System.Drawing.Point(16, 8)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(94, 20)
        Me.Panel1.TabIndex = 35
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.AliceBlue
        Me.Label1.Location = New System.Drawing.Point(2, 2)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(26, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Day"
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.AliceBlue
        Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel2.Controls.Add(Me.Label2)
        Me.Panel2.Controls.Add(Me.Panel3)
        Me.Panel2.Location = New System.Drawing.Point(110, 8)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(126, 20)
        Me.Panel2.TabIndex = 36
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.AliceBlue
        Me.Label2.Location = New System.Drawing.Point(4, 2)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(48, 13)
        Me.Label2.TabIndex = 37
        Me.Label2.Text = "Forecast"
        '
        'Panel3
        '
        Me.Panel3.Location = New System.Drawing.Point(124, 0)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(290, 20)
        Me.Panel3.TabIndex = 36
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.AliceBlue
        Me.Panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel4.Controls.Add(Me.Label4)
        Me.Panel4.Location = New System.Drawing.Point(518, 8)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(70, 20)
        Me.Panel4.TabIndex = 36
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.AliceBlue
        Me.Label4.Location = New System.Drawing.Point(6, 4)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(37, 13)
        Me.Label4.TabIndex = 35
        Me.Label4.Text = "Precip"
        '
        'Panel5
        '
        Me.Panel5.BackColor = System.Drawing.Color.AliceBlue
        Me.Panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel5.Controls.Add(Me.Label3)
        Me.Panel5.Location = New System.Drawing.Point(236, 8)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(286, 20)
        Me.Panel5.TabIndex = 36
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.AliceBlue
        Me.Label3.Location = New System.Drawing.Point(4, 2)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(60, 13)
        Me.Label3.TabIndex = 38
        Me.Label3.Text = "Description"
        '
        'Detailed_TenDay_ForecastTableAdapter
        '
        Me.Detailed_TenDay_ForecastTableAdapter.ClearBeforeFill = True
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(0, 10000)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(100, 20)
        Me.TextBox1.TabIndex = 0
        '
        'Form2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(595, 811)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.Panel5)
        Me.Controls.Add(Me.Panel4)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.DataRepeater1)
        Me.Name = "Form2"
        Me.Text = "Extended Weather Forecast"
        Me.DataRepeater1.ItemTemplate.ResumeLayout(False)
        Me.DataRepeater1.ItemTemplate.PerformLayout()
        Me.DataRepeater1.ResumeLayout(False)
        CType(Me.picForecast, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Detailed_TenDay_ForecastBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RedmondWeatherDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.Panel5.ResumeLayout(False)
        Me.Panel5.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents DataRepeater1 As Microsoft.VisualBasic.PowerPacks.DataRepeater
    Friend WithEvents RedmondWeatherDataSet As WindowsApplication2.RedmondWeatherDataSet
    Friend WithEvents Detailed_TenDay_ForecastBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents Detailed_TenDay_ForecastTableAdapter As WindowsApplication2.RedmondWeatherDataSetTableAdapters.Detailed_TenDay_ForecastTableAdapter
    Friend WithEvents DayTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ForecastTextBox As System.Windows.Forms.TextBox
    Friend WithEvents Day_Forecast_DetailsTextBox As System.Windows.Forms.TextBox
    Friend WithEvents Night_Forecast_DetailsTextBox As System.Windows.Forms.TextBox
    Friend WithEvents High_TempTextBox As System.Windows.Forms.TextBox
    Friend WithEvents Low_TempTextBox As System.Windows.Forms.TextBox
    Friend WithEvents Day_Precip_ChanceTextBox As System.Windows.Forms.TextBox
    Friend WithEvents Night_Precip_ChanceTextBox As System.Windows.Forms.TextBox
    Friend WithEvents General_Forecast_DetailsTextBox As System.Windows.Forms.TextBox
    Friend WithEvents picForecast As System.Windows.Forms.PictureBox
    Friend WithEvents ShapeContainer1 As Microsoft.VisualBasic.PowerPacks.ShapeContainer
    Friend WithEvents LineShape1 As Microsoft.VisualBasic.PowerPacks.LineShape
    Friend WithEvents LineShape2 As Microsoft.VisualBasic.PowerPacks.LineShape
    Friend WithEvents txtWeekDay As System.Windows.Forms.TextBox
    Friend WithEvents LineShape3 As Microsoft.VisualBasic.PowerPacks.LineShape
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
End Class
