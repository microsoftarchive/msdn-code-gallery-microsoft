' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MainForm

    ''' <summary>
    ''' Hide the main form, as a program running as a tray icon doesn't typically 
    ''' have a visible form.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnTray_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTray.Click
        Me.Hide()
        NotifyIcon1.Visible = True
        NotifyIcon1.Text = "System Information"
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Make sure that the tray icon dosen't appear until the user clicks Start.
        ' Otherwise the tray icon will be visible at the same time as the main form.
        NotifyIcon1.Visible = False
    End Sub

    ''' <summary>
    ''' Grab the operating system information.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub osMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles osMenuItem.Click
        MsgBox(My.Computer.Info.OSFullName & vbCrLf & "Version " & My.Computer.Info.OSVersion)
    End Sub

    ''' <summary>
    ''' Displays the current Date.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub currentDateMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles currentDateMenuItem.Click
        MsgBox("Today's date is " & My.Computer.Clock.LocalTime.ToLongDateString & ".")
    End Sub

    ''' <summary>
    ''' Using the TimeZone class display the name of the user's current timezone.  
    ''' The(Time Zone) class can also be used to determine if the user's location 
    ''' is currently using daylight savings time as well as the time that daylight 
    ''' savings time is active for a given time zone.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub timeZoneMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles timeZoneMenuItem.Click
        If (TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now)) Then
            MsgBox("The current time zone is: " & TimeZone.CurrentTimeZone.DaylightName & ".")
        Else
            MsgBox("The current time zone is: " & TimeZone.CurrentTimeZone.StandardName & ".")
        End If
    End Sub


    ''' <summary>
    ''' Grab the current .NET Framework Version.  Outputs the Build, Major, 
    ''' Minor information. This information can also be accessed individually 
    ''' by properties.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub frameworkMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles frameworkMenuItem.Click
        MsgBox("Framework Version: " & Environment.Version.ToString() & ".")
    End Sub

    ''' <summary>
    ''' Displays the amount of time that the machine has been up since last being rebooted.
    ''' The time retrieved from TickCount is in Milliseconds.  This is converted to minutes.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub restartMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles restartMenuItem.Click
        Dim timeSinceLastRebootMinutes As Double = My.Computer.Clock.TickCount / 1000 / 60
        MsgBox("It has been " & CInt(timeSinceLastRebootMinutes).ToString() & " minutes since your last reboot.")
    End Sub

    ''' <summary>
    ''' When the user double-clicks the tray icon, display the main form again.
    ''' Also, make the tray icon disappear while the form is visible.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ntfSystemInfo_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles NotifyIcon1.DoubleClick
        NotifyIcon1.Visible = False
        Me.Show()
    End Sub

    Protected Sub Shutdown()
        ' It's a good idea to make the system tray icon invisible before ending
        ' the application, otherwise, it can linger in the tray when the application
        ' is no longer running.
        NotifyIcon1.Visible = False
        Application.Exit()
    End Sub

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Shutdown()
    End Sub

    Private Sub exitMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitMenuItem.Click
        Shutdown()
    End Sub
End Class
