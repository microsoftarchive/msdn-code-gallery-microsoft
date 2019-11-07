Public Class Form1
    ' This sample application demonstrates the use of multiple DataRepeater controls to 
    ' display weather forecast data from a local database.

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Loads data into the 'RedmondWeatherDataSet.Five_Day_Forecast' table.
        Me.Five_Day_ForecastTableAdapter.Fill(Me.RedmondWeatherDataSet.Five_Day_Forecast)
        ' Loads data into the 'RedmondWeatherDataSet.Todays_Forecast' table.
        Me.Todays_ForecastTableAdapter.Fill(Me.RedmondWeatherDataSet.Todays_Forecast)
        ' Loads data into the 'RedmondWeatherDataSet.CurrentConditions' table.
        Me.CurrentConditionsTableAdapter.Fill(Me.RedmondWeatherDataSet.CurrentConditions)

        ' Set the picture for the forecast.
        Select Case Me.ConditionsTextBox.Text
            Case ForecastTypes.Cloudy
                Me.picConditions.Image = My.Resources.Cloudy
            Case ForecastTypes.Clear
                Me.picConditions.Image = My.Resources.Clear
            Case ForecastTypes.Rain
                Me.picConditions.Image = My.Resources.Rain
            Case ForecastTypes.Shower_Clear
                Me.picConditions.Image = My.Resources.Showersclear
            Case ForecastTypes.Sprinkles
                Me.picConditions.Image = My.Resources.Sprinkles
            Case ForecastTypes.Fair
                Me.picConditions.Image = My.Resources.Fair
            Case Else
                ' No image exists for the forecast condition.
                MsgBox("error " & Me.ConditionsTextBox.Text)
        End Select
        MakeTransparent(picConditions.Image)

        ' Add a degree symbol.
        Me.TemperatureTextBox.Text += Chr(176)
        Me.DewpointTextBox.Text += Chr(176)
        ' Add a percent symbol.
        Me.HumidityTextBox.Text += " %"
        ' Add supplementary text.
        Me.BarometerTextBox.Text += " in"
        Me.WindSpeedTextBox.Text += " mph"

    End Sub

    Private Sub DataRepeater1_DrawItem(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.PowerPacks.DataRepeaterItemEventArgs) Handles DataRepeater1.DrawItem
        ' The DrawItem event is used to control what is displayed in each individual
        ' item in the "Today's forecast" DataRepeater control.
        Dim pic As PictureBox = CType(e.DataRepeaterItem.Controls("picTodaysForecast"), System.Windows.Forms.PictureBox)

        ' Disable the control while drawing.
        e.DataRepeaterItem.Enabled = False

        ' Set the picture for the forecast.
        Select Case e.DataRepeaterItem.Controls("ForecastTextBox").Text
            Case ForecastTypes.Cloudy
                pic.Image = My.Resources.Cloudy
            Case ForecastTypes.Clear
                pic.Image = My.Resources.Clear
            Case ForecastTypes.Rain
                pic.Image = My.Resources.Rain
            Case ForecastTypes.Shower_Clear
                pic.Image = My.Resources.Showersclear
            Case ForecastTypes.Sprinkles
                pic.Image = My.Resources.Sprinkles
            Case ForecastTypes.Fair
                pic.Image = My.Resources.Fair
            Case Else
                pic.Image = CType(DataRepeater1.ItemTemplate.Controls("picTodaysForecast"), System.Windows.Forms.PictureBox).Image
        End Select

        MakeTransparent(pic.Image)

        ' Hide or show the labels depending on detail of data.
        If e.DataRepeaterItem.Controls("LowTempTextBox").Text.Length = 0 Then
            e.DataRepeaterItem.Controls("LowTempLabel").Visible = False
            e.DataRepeaterItem.Controls("HighTempLabel").Visible = False
        End If


        ' Add a degree symbol to the temperatures.
        If e.DataRepeaterItem.Controls("HighTempTextBox").Text.Length > 0 AndAlso e.DataRepeaterItem.Controls("HighTempTextBox").Text.LastIndexOf(Chr(176)) = -1 Then
            e.DataRepeaterItem.Controls("HighTempTextBox").Text += Chr(176)
        End If

        If e.DataRepeaterItem.Controls("LowTempTextBox").Text.Length > 0 AndAlso e.DataRepeaterItem.Controls("LowTempTextBox").Text.LastIndexOf(Chr(176)) = -1 Then
            e.DataRepeaterItem.Controls("LowTempTextBox").Text += Chr(176)
        End If

        ' Enable the control when the drawing is done.
        e.DataRepeaterItem.Enabled = True
    End Sub

    Private Sub DataRepeater2_DrawItem(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.PowerPacks.DataRepeaterItemEventArgs) Handles DataRepeater2.DrawItem
        ' The DrawItem event is used to control what is displayed in each individual
        ' item in the "Five day forecast" DataRepeater control.

        Dim pic As PictureBox = CType(e.DataRepeaterItem.Controls("picForecast"), System.Windows.Forms.PictureBox)

        'set the picture for the Forecast
        Select Case e.DataRepeaterItem.Controls("ForecastTextBox1").Text
            Case ForecastTypes.Cloudy
                pic.Image = My.Resources.Cloudy
            Case ForecastTypes.Clear
                pic.Image = My.Resources.Clear
            Case ForecastTypes.Rain
                pic.Image = My.Resources.Rain
            Case ForecastTypes.Shower_Clear
                pic.Image = My.Resources.Showersclear
            Case ForecastTypes.Snow
                pic.Image = My.Resources.Cloudy
            Case ForecastTypes.Sprinkles
                pic.Image = My.Resources.Sprinkles
            Case ForecastTypes.Fair
                pic.Image = My.Resources.Fair
            Case Else
                pic.Image = CType(DataRepeater2.ItemTemplate.Controls("picForecast"), System.Windows.Forms.PictureBox).Image
        End Select
        MakeTransparent(pic.Image)

        ' Hide or show the labels depending on detail of data.
        If e.DataRepeaterItem.Controls("Low_TempTextBox").Text.Length = 0 Then
            e.DataRepeaterItem.Controls("Low_TempLabel").Visible = False
            e.DataRepeaterItem.Controls("High_TempLabel").Visible = False
        End If

        ' Set weekday text based on Day of Week.
        Select Case Weekday(e.DataRepeaterItem.Controls("Five_DayTextBox").Text)
            Case 2
                e.DataRepeaterItem.Controls("txtWeekDay").Text = "Monday"
            Case 3
                e.DataRepeaterItem.Controls("txtWeekDay").Text = "Tuesday"
            Case 4
                e.DataRepeaterItem.Controls("txtWeekDay").Text = "Wednesday"
            Case 5
                e.DataRepeaterItem.Controls("txtWeekDay").Text = "Thursday"
            Case 6
                e.DataRepeaterItem.Controls("txtWeekDay").Text = "Friday"
            Case 7
                e.DataRepeaterItem.Controls("txtWeekDay").Text = "Saturday"
            Case 1
                e.DataRepeaterItem.Controls("txtWeekDay").Text = "Sunday"

        End Select

        ' If the date is tomorrows date change the caption.
        If CDate(e.DataRepeaterItem.Controls("Five_DayTextBox").Text) = CDate(Format(Now(), "d")).AddDays(1) Then
            e.DataRepeaterItem.Controls("txtWeekDay").Text = "Tomorrow"
        End If

        ' Add the degree symbol to the temperature fields.
        If e.DataRepeaterItem.Controls("High_TempTextBox").Text.Length > 0 AndAlso e.DataRepeaterItem.Controls("High_TempTextBox").Text.LastIndexOf(Chr(176)) = -1 Then
            e.DataRepeaterItem.Controls("High_TempTextBox").Text += Chr(176)
        End If

        If e.DataRepeaterItem.Controls("Low_TempTextBox").Text.Length > 0 AndAlso e.DataRepeaterItem.Controls("Low_TempTextBox").Text.LastIndexOf(Chr(176)) = -1 Then
            e.DataRepeaterItem.Controls("Low_TempTextBox").Text += Chr(176)
        End If
    End Sub

    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        ' Display the Extended Forecast form.
        Form2.Show()
    End Sub

    Private Sub MakeTransparent(ByVal pic As Image)
        ' Create a bitmap.
        Dim g As New System.Drawing.Bitmap(pic)
        ' Make the white areas in the bitmap transparent.
        g.MakeTransparent(System.Drawing.Color.White)
        ' Assign the image.
        pic = g
    End Sub
End Class
