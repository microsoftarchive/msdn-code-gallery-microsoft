' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MainForm

    Private Sub DisplayTSProperties(ByVal ts As TimeSpan)
        ' Use instance properties of the TimeSpan type.
        ' Demonstrates:
        '  TimeSpan.Days
        '  TimeSpan.Hours
        '  TimeSpan.Milliseconds
        '  TimeSpan.Minutes
        '  TimeSpan.Seconds
        '  TimeSpan.Ticks
        '  TimeSpan.TotalDays
        '  TimeSpan.TotalHours
        '  TimeSpan.TotalMilliseconds
        '  TimeSpan.TotalMinutes
        '  TimeSpan.TotalSeconds
        Try
            Days.Text = ts.Days.ToString
            Hours.Text = ts.Hours.ToString
            Milliseconds.Text = ts.Milliseconds.ToString
            Minutes.Text = ts.Minutes.ToString
            Seconds.Text = ts.Seconds.ToString
            TimeSpanTicks.Text = ts.Ticks.ToString
            TotalDays.Text = ts.TotalDays.ToString
            TotalHours.Text = ts.TotalHours.ToString
            TotalMilliseconds.Text = ts.TotalMilliseconds.ToString
            TotalMinutes.Text = ts.TotalMinutes.ToString
            TotalSeconds.Text = ts.TotalSeconds.ToString
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Text)
        End Try
    End Sub

    Private Sub LoadCalculationMethods()
        ' Use instance methods of the DateTime type.
        ' Demonstrates:
        '  DateTime.AddDays
        '  DateTime.AddHours
        '  DateTime.AddMilliseconds
        '  DateTime.AddMinutes
        '  DateTime.AddMonths
        '  DateTime.AddSeconds
        '  DateTime.AddTicks
        '  DateTime.AddYears
        Try
            Dim dateNow As DateTime = DateTime.Now
            lblNow3.Text = dateNow.ToString

            AddDaysLabel.Text = dateNow.AddDays(CDbl(AddDays.Text)).ToString
            AddHoursLabel.Text = dateNow.AddHours(CDbl(AddHours.Text)).ToString
            AddMillisecondsLabel.Text = dateNow.AddMilliseconds(CDbl(AddMilliseconds.Text)).ToString
            AddMinutesLabel.Text = dateNow.AddMinutes(CDbl(AddMinutes.Text)).ToString
            AddMonthsLabel.Text = dateNow.AddMonths(CInt(AddMonths.Text)).ToString
            AddSecondsLabel.Text = dateNow.AddSeconds(CDbl(AddSeconds.Text)).ToString
            AddTicksLabel.Text = dateNow.AddTicks(CLng(AddTicks.Text)).ToString
            AddYearsLabel.Text = dateNow.AddYears(CInt(AddYears.Text)).ToString
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Text)
        End Try
    End Sub

    Private Sub LoadConversionMethods()
        ' Use instance methods of the DateTime type.
        ' Demonstrates:
        '  DateTime.ToFileTime
        '  DateTime.ToLocalTime
        '  DateTime.ToLongDateString
        '  DateTime.ToLongTimeString
        '  DateTime.ToOADate
        '  DateTime.ToShortDateString
        '  DateTime.ToShortTimeString
        '  DateTime.ToString
        '  DateTime.ToUniversalTime
        Try
            Dim dateNow As DateTime = DateTime.Now

            Now2Label.Text = dateNow.ToString
            ToFileTime.Text = dateNow.ToFileTime.ToString
            ToLocalTime.Text = dateNow.ToLocalTime.ToString
            ToLongDateString.Text = dateNow.ToLongDateString
            ToLongTimeString.Text = dateNow.ToLongTimeString
            ToOADate.Text = dateNow.ToOADate.ToString
            ToShortDateString.Text = dateNow.ToShortDateString
            ToShortTimeString.Text = dateNow.ToShortTimeString
            ToStringLabel.Text = dateNow.ToString
            ToUniversalTime.Text = dateNow.ToUniversalTime.ToString
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Text)
        End Try
    End Sub

    Private Sub LoadProperties()
        ' Use instance properties of the 
        ' the DateTime type.
        ' Demonstrates:
        '  DateTime.Now
        '  DateTime.Date
        '  DateTime.Day
        '  DateTime.DayOfYear
        '  DateTime.Hour
        '  DateTime.Millisecond
        '  DateTime.DayOfWeek
        '  DateTime.Minute
        '  DateTime.Month
        '  DateTime.Second
        '  DateTime.Ticks
        '  DateTime.TimeOfDay
        '  DateTime.Year
        Try
            ' No need to use an explicit constructor
            ' unless you need to specify a value
            ' at the time the instance gets created.
            Dim dateNow As DateTime = DateTime.Now

            NowPropertyLabel.Text = dateNow.ToString
            DateLabel.Text = dateNow.Date.ToString
            Day.Text = dateNow.Day.ToString
            DayOfYear.Text = dateNow.DayOfYear.ToString
            Hour.Text = dateNow.Hour.ToString
            Millisecond.Text = dateNow.Millisecond.ToString
            DayOfWeek.Text = dateNow.DayOfWeek.ToString
            Minute.Text = dateNow.Minute.ToString
            Month.Text = dateNow.Month.ToString
            Second.Text = dateNow.Second.ToString
            Ticks.Text = dateNow.Ticks.ToString
            TimeOfDay.Text = dateNow.TimeOfDay.ToString
            Year.Text = dateNow.Year.ToString
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Text)
        End Try
    End Sub

    Private Sub LoadSharedMembers()
        ' Use shared members of the DateTime type.
        ' Demonstrates these properties:
        '  DateTime.Now 
        '     Gets a DateTime that is the current local date and time on this computer.
        '  DateTime.UtcNow
        '    Gets a DateTime that is the current local date and time on this computer 
        '    expressed as the coordinated universal time (UTC).
        '  DateTime.MinValue
        '    Represents the smallest possible value of DateTime. This field is read-only.
        '  DateTime.MaxValue
        '    Represents the largest possible value of DateTime. This field is read-only.

        ' Demonstrates these methods:
        '  DateTime.FromOADate
        '    Returns a DateTime equivalent to the specified OLE Automation Date.
        '  DateTime.IsLeapYear
        '    Returns an indication whether the specified year is a leap year.
        '  DateTime.DaysInMonth
        '    Returns the number of days in the specified month of the specified year.
        Try
            NowLabel.Text = DateTime.Now.ToString
            TodayLabel.Text = DateTime.Today.ToString
            UtcNowLabel.Text = DateTime.UtcNow.ToString
            MinValue.Text = DateTime.MinValue.ToString
            MaxValue.Text = DateTime.MaxValue.ToString

            FromOADateLabel.Text = DateTime.FromOADate(CDbl(FromOADate.Text)).ToString
            IsLeapYearLabel.Text = DateTime.IsLeapYear(CInt(IsLeapYear.Text)).ToString
            DaysInMonthLable.Text = DateTime.DaysInMonth( _
             CInt(YearDaysInMonth.Text), CInt(MonthDaysInMonth.Text)).ToString
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Text)
        End Try
    End Sub

    Private Sub LoadTimeSpanFields()
        ' Use shared fields provided by TimeSpan type.
        ' Demonstrates:
        '  TimeSpan.MaxValue
        '  TimeSpan.Minvalue
        '  TimeSpan.TicksPerDay
        '  TimeSpan.TicksPerHour
        '  TimeSpan.TicksPerMillisecond
        '  TimeSpan.TicksPerMinute
        '  TimeSpan.TicksPerSecond
        '  TimeSpan.Zero
        Try
            MaxValueTS.Text = TimeSpan.MaxValue.ToString
            MinValueTS.Text = TimeSpan.MinValue.ToString
            TicksPerDay.Text = TimeSpan.TicksPerDay.ToString
            TicksPerHour.Text = TimeSpan.TicksPerHour.ToString
            TicksPerMillisecond.Text = TimeSpan.TicksPerMillisecond.ToString
            TicksPerMinute.Text = TimeSpan.TicksPerMinute.ToString
            TicksPerSecond.Text = TimeSpan.TicksPerSecond.ToString
            Zero.Text = TimeSpan.Zero.ToString
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Text)
        End Try
    End Sub

    Private Sub LoadTSMethods()
        ' Use shared methods of the TimeSpan type.
        ' Demonstrates:
        '  TimeSpan.FromDays
        '  TimeSpan.FromHours
        '  TimeSpan.FromMilliseconds
        '  TimeSpan.FromMinutes
        '  TimeSpan.FromSeconds
        '  TimeSpan.FromTicks
        Try
            FromDaysLabel.Text = TimeSpan.FromDays(CDbl(FromDays.Text)).ToString
            FromHoursLabel.Text = TimeSpan.FromHours(CDbl(FromHours.Text)).ToString
            FromMillisecondsLabel.Text = TimeSpan.FromMilliseconds(CDbl(FromMilliseconds.Text)).ToString
            FromMinutesLabel.Text = TimeSpan.FromMinutes(CDbl(FromMinutes.Text)).ToString
            FromSecondsLabel.Text = TimeSpan.FromSeconds(CDbl(FromSeconds.Text)).ToString
            FromTicksLabel.Text = TimeSpan.FromTicks(CLng(FromTicks.Text)).ToString
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Text)
        End Try
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        ' Call procedures that load values onto the form.
        LoadSharedMembers()
        LoadProperties()
        LoadConversionMethods()
        LoadCalculationMethods()
        LoadTimeSpanFields()
        LoadTSMethods()

    End Sub

    Private Sub btnRefreshShared_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshShared.Click
        LoadSharedMembers()
    End Sub

    Private Sub btnRefreshCalculation_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshCalculation.Click
        LoadCalculationMethods()
    End Sub

    Private Sub btnRefreshProperties_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshProperties.Click
        LoadProperties()
    End Sub

    Private Sub btnRefreshConversion_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshConversion.Click
        LoadConversionMethods()
    End Sub

    Private Sub btnRefreshTSProperties_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshTSProperties.Click
        Try
            ' Create a TimeSpan instance based on 
            ' DateTime values provided on the form.
            Dim ts As TimeSpan
            Dim startDate As DateTime
            Dim endDate As DateTime

            ' Parse the text from the text boxes.
            startDate = DateTime.Parse(StartTime.Text)
            endDate = DateTime.Parse(EndTime.Text)
            ts = endDate.Subtract(startDate).Duration

            ' Display the properties of the TimeSpan
            ' instance you've created.
            DisplayTSProperties(ts)
        Catch Ex As Exception
            MessageBox.Show(Ex.Message, Me.Text)
        End Try
    End Sub

    Private Sub btnCalcParse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCalcParse.Click
        ' Display TimeSpan properties given a string containing
        ' a TimeSpan value to parse.
        Try
            ' No need to use an explicit constructor
            ' unless you need to specify a value
            ' at the time the instance gets created.
            Dim ts As TimeSpan
            ts = TimeSpan.Parse(txtParse.Text)

            ' Display the properties of the TimeSpan
            ' instance you've created.
            DisplayTSProperties(ts)
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Text)
        End Try
    End Sub

    Private Sub btnRefreshTSMethods_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshTSMethods.Click
        LoadTSMethods()
    End Sub

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
