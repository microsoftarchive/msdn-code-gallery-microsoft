Module MainModule

    Sub Main()
        Dim strSaveFilename, strTo, strFrom, strSMTPHost As String
        Try
            '
            'Get Email + SMTP values from the registry
            Dim key As Microsoft.Win32.RegistryKey
            key = My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\ExcelAuto")
            If Not key Is Nothing Then
                strTo = key.GetValue("EmailAddress")
                strSMTPHost = key.GetValue("SMTPHost")
                strFrom = "VB2005@example.com"
                '
                'Get temporary filename to use for saving the Excel file
                strSaveFilename = My.Computer.FileSystem.SpecialDirectories.Temp & "\Report " & Format(Now, "d-MMM-yyy HH.mm") & ".xlsx"
                PopulateExcelWorkbook(strSaveFilename, False)
                'Send mail
                SendExcelMailViaSMTP(strTo, strFrom, strSaveFilename, strSMTPHost, True)
                System.Diagnostics.EventLog.WriteEntry(My.Application.Info.AssemblyName, "Successfully sent mail", EventLogEntryType.Information)
            End If
        Catch ex As Exception
            System.Diagnostics.EventLog.WriteEntry(My.Application.Info.AssemblyName, "Exception occurred: " & ex.ToString, EventLogEntryType.Error)
        End Try

    End Sub

End Module
