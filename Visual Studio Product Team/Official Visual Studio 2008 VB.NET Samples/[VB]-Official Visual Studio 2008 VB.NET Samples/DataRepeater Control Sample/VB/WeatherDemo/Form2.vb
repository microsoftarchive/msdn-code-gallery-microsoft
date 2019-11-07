Public Class Form2
    ' This form displays an extended ten day forecast.
    Private Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Loads data into the 'RedmondWeatherDataSet.Detailed_TenDay_Forecast' table.
        Me.Detailed_TenDay_ForecastTableAdapter.Fill(Me.RedmondWeatherDataSet.Detailed_TenDay_Forecast)
    End Sub

    Private Sub DataRepeater1_DrawItem(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.PowerPacks.DataRepeaterItemEventArgs) Handles DataRepeater1.DrawItem
        ' The DrawItem event is used to control what is displayed in each individual
        ' item in the "Extended weather forecast" DataRepeater control.
        Dim pic As PictureBox = CType(e.DataRepeaterItem.Controls("picForecast"), System.Windows.Forms.PictureBox)

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
            Case ForecastTypes.AM_Rain
                pic.Image = My.Resources.AMRain
            Case ForecastTypes.Fair
                pic.Image = My.Resources.Fair
            Case Else
                pic.Image = CType(DataRepeater1.ItemTemplate.Controls("picForecast"), System.Windows.Forms.PictureBox).Image
        End Select

        MakeTransparent(pic.Image)

        ' Alternate the back color.
        If (e.DataRepeaterItem.ItemIndex Mod 2) <> 0 Then
            e.DataRepeaterItem.BackColor = Color.AliceBlue
        Else
            e.DataRepeaterItem.BackColor = DataRepeater1.ItemTemplate.BackColor
            For Each c As Control In e.DataRepeaterItem.Controls
                c.BackColor = DataRepeater1.ItemTemplate.BackColor
            Next
        End If

        ' Hide or show the labels depending on detail of data.
        If e.DataRepeaterItem.Controls("Low_TempTextBox").Text.Length = 0 Then
            e.DataRepeaterItem.Controls("Low_TempLabel").Visible = False
            e.DataRepeaterItem.Controls("High_TempLabel").Visible = False
        End If

        ' Set weekday text based on Day of Week
        Select Case Weekday(e.DataRepeaterItem.Controls("DayTextBox").Text)
            Case 1
                e.DataRepeaterItem.Controls("txtWeekDay").Text = "Sunday"
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
            Case Else
                e.DataRepeaterItem.Controls("txtWeekDay").Text = ""
        End Select

        ' Set the weekday to tomorrow when necessary.
        If CDate(e.DataRepeaterItem.Controls("DayTextBox").Text) = CDate(Format(Now(), "d")).AddDays(1) Then
            e.DataRepeaterItem.Controls("txtWeekDay").Text = "Tomorrow"
        End If

        ' Hide the detail forecast for all but today and tomorrow.
        If e.DataRepeaterItem.Controls("Day_Forecast_DetailsTextBox").Text.Length = 0 Then
            e.DataRepeaterItem.Controls("Day_Forecast_DetailsTextBox").Visible = False
            e.DataRepeaterItem.Controls("Day_Forecast_DetailsLabel").Visible = False

            e.DataRepeaterItem.Controls("Night_Forecast_DetailsTextBox").Visible = False
            e.DataRepeaterItem.Controls("Night_Forecast_DetailsLabel").Visible = False

            e.DataRepeaterItem.Controls("General_Forecast_DetailsTextBox").Visible = True
        End If

        ' Add a degree symbol to the temperatures.
        If e.DataRepeaterItem.Controls("High_TempTextBox").Text.Length > 0 AndAlso e.DataRepeaterItem.Controls("High_TempTextBox").Text.LastIndexOf(Chr(176)) = -1 Then
            e.DataRepeaterItem.Controls("High_TempTextBox").Text += Chr(176)
        End If

        If e.DataRepeaterItem.Controls("Low_TempTextBox").Text.Length > 0 AndAlso e.DataRepeaterItem.Controls("Low_TempTextBox").Text.LastIndexOf(Chr(176)) = -1 Then
            e.DataRepeaterItem.Controls("Low_TempTextBox").Text += Chr(176)
        End If
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