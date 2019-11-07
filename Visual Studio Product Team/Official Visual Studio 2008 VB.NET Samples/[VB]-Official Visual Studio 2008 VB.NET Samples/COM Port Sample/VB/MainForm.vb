' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Text

Public Class MainForm

    ' Declare necessary class variables.
    Private CommPort As New RS232()
    Private IsModemFound As Boolean = False
    Private ModemPort As Integer = 0


    ' This subroutine checks for available ports on the local machine. It does 
    '   this by attempting to open ports 1 through 4.
    Private Sub CheckForPortsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckForPortsButton.Click

        ' Check for Availability of each of the 4 Comm Ports, and
        '   place a check in the list box items that have openable ports.
        Dim i As Integer
        For i = 1 To 4
            WriteMessage("Testing COM" + i.ToString())
            If IsPortAvailable(i) Then
                ' Check the box for available ports.
                Me.PortsList.SetItemChecked(i - 1, True)
            Else
                ' Uncheck the box for unavailable ports.
                Me.PortsList.SetItemChecked(i - 1, False)
            End If
        Next
        ' Enable the Find Modems button.
        Me.CheckModemsButton.Enabled = True
    End Sub

    ' This subroutine attempts to send an AT command to any active Comm Ports.
    '   If a response is returned then a usable modem has been detected
    '   on that port.
    Private Sub CheckModemsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckModemsButton.Click
        Dim i As Integer
        For i = 0 To 3
            If Me.PortsList.GetItemChecked(i) Then
                ' Item is checked so it MIGHT be a valid port.
                ' Test for validity.
                If IsPortAvailable(i + 1) Then
                    ' Check if port responds to an AT command.
                    If IsPortAModem(i + 1) Then
                        ' Set the class variables to the last modem found.
                        Me.IsModemFound = True
                        Me.ModemPort = i + 1
                        ' Write message to the user.
                        WriteMessage("Port " + (i + 1).ToString() + _
                            " is a responsive modem.")
                    Else
                        ' Write message to the user.
                        WriteMessage("Port " + (i + 1).ToString() + _
                            " is not a responsive modem.")
                    End If
                End If
            End If
        Next
        ' If a modem was found, enable the rest of the buttons, so the user
        '   can interact with the modem.
        If Me.IsModemFound Then
            Me.SelectedModemTextbox.Text = "Using Modem on COM" + _
                Me.ModemPort.ToString()
            Me.UserCommandTextbox.Enabled = True
            Me.SendATCommandButton.Enabled = True
            Me.SendUserCommandButton.Enabled = True
        End If
    End Sub


    ' This subroutine clears the TextBox.
    Private Sub ClearButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ClearButton.Click
        Me.StatusTextbox.Clear()
    End Sub


    ' This subroutine sends an AT command to the modem, and records its response.
    '   It depends on the timer to do the reading of the response.
    Private Sub SendATCommandButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SendATCommandButton.Click

        ' Always wrap up working with Comm Ports in exception handlers.
        Try
            ' Enable the timer.
            Me.tmrReadCommPort.Enabled = True
            ' Attempt to open the port.
            CommPort.Open(ModemPort, 115200, 8, RS232.DataParity.Parity_None, _
                RS232.DataStopBit.StopBit_1, 4096)

            ' Write an AT Command to the Port.
            CommPort.Write(Encoding.ASCII.GetBytes("AT" & Chr(13)))
            ' Sleep long enough for the modem to respond and the timer to fire.
            System.Threading.Thread.Sleep(200)
            Application.DoEvents()
            CommPort.Close()

        Catch ex As Exception
            ' Warn the user.
            MessageBox.Show("Unable to communicate with Modem")
        Finally
            ' Disable the timer.
            Me.tmrReadCommPort.Enabled = False
        End Try

    End Sub


    ' This subroutine sends a user specified command to the modem, and records its 
    '   response. It depends on the timer to do the reading of the response.
    Private Sub SendUserCommandButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SendUserCommandButton.Click

        ' Always wrap up working with Comm Ports in exception handlers.
        Try
            ' Enable the timer.
            Me.tmrReadCommPort.Enabled = True
            ' Attempt to open the port.
            CommPort.Open(ModemPort, 115200, 8, RS232.DataParity.Parity_None, RS232.DataStopBit.StopBit_1, 4096)

            ' Write an user specified Command to the Port.
            CommPort.Write(Encoding.ASCII.GetBytes(Me.UserCommandTextbox.Text & Chr(13)))
            ' Sleep long enough for the modem to respond and the timer to fire.
            System.Threading.Thread.Sleep(200)
            Application.DoEvents()
            CommPort.Close()

        Catch ex As Exception
            ' Warn the user.
            MessageBox.Show("Unable to communicate with Modem")
        Finally
            ' Disable the timer.
            Me.tmrReadCommPort.Enabled = False
        End Try

    End Sub

    ' This subroutine is fired when the timer event is raised. It writes whatever
    '   is in the Comm Port buffer to the output window.
    Private Sub tmrReadCommPort_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrReadCommPort.Tick
        Try
            ' As long as there is information, read one byte at a time and 
            '   output it.
            While (CommPort.Read(1) <> -1)
                ' Write the output to the screen.
                WriteMessage(Chr(CommPort.InputStream(0)), False)
            End While
        Catch exc As Exception
            ' An exception is raised when there is no information to read.
            '   Don't do anything here, just let the exception go.
        End Try

    End Sub


    ' This function checks to see if the port is a modem, by sending 
    '   an AT command to the port. If the port responds, it is assumed to 
    '   be a modem. The function returns a Boolean.
    Private Function IsPortAModem(ByVal port As Integer) As Boolean

        ' Always wrap up working with Comm Ports in exception handlers.
        Try
            ' Attempt to open the port.
            CommPort.Open(port, 115200, 8, RS232.DataParity.Parity_None, _
                RS232.DataStopBit.StopBit_1, 4096)

            ' Write an AT Command to the Port.
            CommPort.Write(Encoding.ASCII.GetBytes("AT" & Chr(13)))
            ' Sleep long enough for the modem to respond.
            System.Threading.Thread.Sleep(200)
            Application.DoEvents()
            ' Try to get info from the Comm Port.
            Try
                ' Try to read a single byte. If you get it, then assume
                '   that the port contains a modem. Clear the buffer before 
                '   leaving.
                CommPort.Read(1)
                CommPort.ClearInputBuffer()
                CommPort.Close()
                Return True
            Catch exc As Exception
                ' Nothing to read from the Comm Port, so set to False
                CommPort.Close()
                Return False
            End Try
        Catch exc As Exception
            ' Port could not be opened or written to.
            Me.PortsList.SetItemChecked(port - 1, False)
            MsgBox("Could not open port.", MsgBoxStyle.OKOnly, Me.Text)
            Return False
        End Try


    End Function

    ' This function attempts to open the passed Comm Port. If it is
    '   available, it returns True, else it returns False. To determine
    '   availability a Try-Catch block is used.
    Private Function IsPortAvailable(ByVal ComPort As Integer) As Boolean
        Try
            CommPort.Open(ComPort, 115200, 8, RS232.DataParity.Parity_None, _
                RS232.DataStopBit.StopBit_1, 4096)
            ' If it makes it to here, then the Comm Port is available.
            CommPort.Close()
            Return True
        Catch
            ' If it gets here, then the attempt to open the Comm Port
            '   was unsuccessful.
            Return False
        End Try
    End Function

    ' This subroutine writes a message to the txtStatus TextBox.
    Private Sub WriteMessage(ByVal message As String)
        Me.StatusTextbox.Text += message + vbCrLf
    End Sub

    ' This subroutine writes a message to the txtStatus TextBox and allows
    '   the line feed to be suppressed.
    Private Sub WriteMessage(ByVal message As String, ByVal linefeed As Boolean)
        Me.StatusTextbox.Text += message
        If linefeed Then
            Me.StatusTextbox.Text += vbCrLf
        End If
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        End
    End Sub
End Class
