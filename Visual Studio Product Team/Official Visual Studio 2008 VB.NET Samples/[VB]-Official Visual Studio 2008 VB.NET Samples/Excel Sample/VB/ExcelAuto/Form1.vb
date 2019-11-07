Imports Microsoft.Office.Interop

Public Class Form1
    Private Sub cboRecurrence_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboRecurrence.SelectedIndexChanged
        '
        'This method sets the maximum 'Every' value based on the recurrence type selected
        Select Case Me.cboRecurrence.Text
            Case "Minutes" : Me.udEvery.Maximum = 1439
            Case "Hours" : Me.udEvery.Maximum = 23
            Case "Days" : Me.udEvery.Maximum = 365
        End Select
    End Sub
    Private Sub cmdLoadSpreadsheet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdLoadSpreadsheet.Click
        Dim dt As DataTable
        dt = PopulateExcelWorkbook("", True)
        Me.grdOrder.DataSource = dt
    End Sub
    Private Sub btnRemoveRecurrence_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemoveRecurrence.Click
        Shell("SCHTASKS /DELETE /TN ExcelAuto /F", AppWinStyle.Hide, True, 1000)
    End Sub
    Private Sub cmdSchedule_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSchedule.Click
        Dim strCommand As String
        Dim strTask As String
        Dim key As Microsoft.Win32.RegistryKey
        '
        'Remove any existing schedule
        Shell("SCHTASKS /DELETE /TN ExcelAuto /F", AppWinStyle.Hide, True, 1000)
        '
        'Create the command line for scheduling a task
        strTask = """""""""" & My.Application.Info.DirectoryPath & "\ExcelAutoTask.exe" & """"""""""

        strCommand = "SCHTASKS /CREATE "
        Select Case Me.cboRecurrence.Text
            Case "Minutes" : strCommand &= " /SC MINUTE"
            Case "Hours" : strCommand &= " /SC HOURLY"
            Case "Days" : strCommand &= " /SC DAILY"
        End Select
        strCommand &= " /MO " & udEvery.Value                           'The every parameter
        strCommand &= " /TN ExcelAuto"                                  'Task name
        strCommand &= " /SD " & Format(Me.datStart.Value, "short date") 'Start date
        strCommand &= " /ST " & Format(Me.datStart.Value, "HH:mm:ss")   'Start time
        strCommand &= " /RU SYSTEM"                                     'Account to run under
        strCommand &= " /TR " & strTask                                 'Command to run
        '
        'Schedule the task
        Shell(strCommand, AppWinStyle.Hide)
        '
        'Store the email address & SMTP Host to send to in the registry
        key = My.Computer.Registry.LocalMachine.CreateSubKey("SOFTWARE\ExcelAuto")
        key.SetValue("EmailAddress", Me.txtEmailAddress.Text)
        key.SetValue("SMTPHost", Me.txtSMTPHost.Text)
        MsgBox("Task scheduled")
    End Sub
    Private Sub cmdEmail_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdEmailOutlook.Click, cmdEmailSMTP.Click
        Dim key As Microsoft.Win32.RegistryKey
        Dim strSaveFilename As String
        '
        'Store the email address & SMTP Host to send to in the registry
        key = My.Computer.Registry.LocalMachine.CreateSubKey("SOFTWARE\ExcelAuto")
        key.SetValue("EmailAddress", Me.txtEmailAddress.Text)
        key.SetValue("SMTPHost", Me.txtSMTPHost.Text)
        '
        strSaveFilename = My.Computer.FileSystem.SpecialDirectories.Temp & "\Report " & Format(Now, "d-MMM-yyy HH.mm") & ".xlsx"
        PopulateExcelWorkbook(strSaveFilename, False)
        If sender Is Me.cmdEmailSMTP Then
            SendExcelMailViaSMTP(Me.txtEmailAddress.Text, "VB2005@example.com", strSaveFilename, Me.txtSMTPHost.Text, True)
        Else
            SendExcelMailViaOutlook(Me.txtEmailAddress.Text, "VB2005@example.com", strSaveFilename, Me.txtSMTPHost.Text, True)
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '
        'If an email address to email to is stored in the registry, then
        'retrieve it and show in the email address textbox
        Dim key As Microsoft.Win32.RegistryKey
        key = My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\ExcelAuto")
        If Not key Is Nothing Then
            Me.txtEmailAddress.Text = key.GetValue("EmailAddress")
            Me.txtSMTPHost.Text = key.GetValue("SMTPHost")
        End If
    End Sub
End Class
