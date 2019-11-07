' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Net.Mail
Imports System.Text
Imports System.ServiceProcess

Public Class MainForm
    Inherits System.Windows.Forms.Form

    Dim arlAttachments As ArrayList


    ''' <summary>
    ''' Handles the Browse button click event. Uses an OpenFileDialog to allow the 
    ''' user to find an attachment to send, which is then added to an arraylist of
    ''' MailAttachment objects.
    ''' </summary>
    Private Sub Browse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Browse.Click
        With odlgAttachment
            .InitialDirectory = "C:\"
            .Filter = "All Files (*.*)|*.*|HTML Files (*.htm;*.html)|*.htm|Microsoft Mail Documents (*.msg)|*.msg|Word Documents (*.doc)|*.doc|Excel Files(*.xl*)|*.xl*|Excel Worksheets (*.xls)|*.xls|Excel Charts (*.xlc)|*.xlc|PowerPoint Presentations (*.ppt)|*.ppt|Text Files (*.txt)|*.txt"
            .FilterIndex = 1

            ' The OpenFileDialog control only has an Open button, not an OK button.
            ' However, there is no DialogResult.Open enum so use DialogResult.OK.
            If .ShowDialog() = Windows.Forms.DialogResult.OK Then
                If IsNothing(arlAttachments) Then
                    arlAttachments = New ArrayList()

                    ' Clear the "(No Attachments)" default text in the ListView
                    Attachments.Items.Clear()
                End If
                arlAttachments.Add(New Attachment(.FileName))

                ' You only want to show the file name. The OpenFileDialog.FileName
                ' property contains the full path. So Split the path and reverse it
                ' to grab the first string in the array, which is just the FileName.
                Dim strFileName() As String = .FileName.Split(New Char() {CChar("\")})
                System.Array.Reverse(strFileName)
                Attachments.Items.Add(strFileName(0))
            End If
        End With
    End Sub

    ''' <summary>
    ''' Handles the Send button click event. This routine checks for valid email
    ''' addresses, builds the body of a message using StringBuilder, creates a 
    ''' mail message, and then attempts to send it.
    ''' </summary>
    Private Sub Send_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Send.Click

        If ToAddress.Text = "" Or From.Text = "" Then
            MsgBox("You must enter both To and From email addresses.")
            Exit Sub
        End If

        ' Use the StringBuilder class instead of traditional string concatenation.
        ' It is optimized for building strings because it is capable of modifying
        ' the underlying buffer instead of having to make a copy of the string for 
        ' each concatenation.
        Dim sb As New StringBuilder()

        ' Build the email message body.
        sb.Append("The following email was sent to you from the Send Mail " & _
            "sample application:")
        sb.Append(vbCrLf)
        sb.Append(vbCrLf)
        sb.Append("SUBJECT: ")
        sb.Append(Trim(Subject.Text))
        sb.Append(vbCrLf)
        sb.Append(vbCrLf)
        sb.Append("MESSAGE: ")
        sb.Append(Trim(Body.Text))
        sb.Append(vbCrLf)

        ' Creating a mail message is as simple as instantiating a class and 
        ' setting a few properties.
        Dim mailMsg As New MailMessage(From.Text.Trim, ToAddress.Text.Trim)
        With mailMsg
            If Not String.IsNullOrEmpty(CC.Text) Then
                .CC.Add(New MailAddress(CC.Text.Trim))
            End If

            If Not String.IsNullOrEmpty(BCC.Text) Then
                .Bcc.Add(New MailAddress(BCC.Text.Trim))
            End If

            .Subject = Subject.Text.Trim
            .Body = sb.ToString

            If Not IsNothing(arlAttachments) Then
                Dim mailAttachment As Attachment
                For Each mailAttachment In arlAttachments
                    .Attachments.Add(mailAttachment)
                Next
            End If
        End With

        ' Set the SmtpServer name. This can be any of the following depending on
        ' your local security settings:

        ' a) Local IP Address (assuming your local machine's SMTP server has the 
        ' right to send messages through a local firewall (if present).

        ' b) 127.0.0.1 the loopback of the local machine.

        ' c) "smarthost" or the name or the IP address of the exchange server you 
        ' utilize for messaging. This is usually what is needed if you are behind
        ' a corporate firewall.

        ' Use structured error handling to attempt to send the email message and 
        ' provide feedback to the user about the success or failure of their 
        ' attempt.
        Try
            Dim client As New SmtpClient("smarthost")
            client.Send(mailMsg)
            Attachments.Items.Clear()
            Attachments.Items.Add("(No Attachments)")

            MessageBox.Show("Your email has been successfully sent!", _
                "Email Send Status", MessageBoxButtons.OK, _
                MessageBoxIcon.Information)
        Catch exp As Exception
            MessageBox.Show("The following problem occurred when attempting to " & _
                "send your email: " & exp.Message, _
                Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Handles the form Load event. Checks to make sure that the SMTP Service is 
    ''' both installed and running.
    ''' </summary>
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, MyBase.Load, MyBase.Load, MyBase.Load
        ' Ensure the SMTP Service is installed.
        Dim services() As ServiceController = ServiceController.GetServices
        Dim service As ServiceController = Nothing
        Dim blnHasSmtpService As Boolean = False

        ' Loop through all the services on the machine and find the SMTP Service.
        For Each service In services
            If service.ServiceName.ToLower = "smtpsvc" Then
                blnHasSmtpService = True
                Exit For
            End If
        Next

        If Not blnHasSmtpService Then
            MessageBox.Show("You do not have SMTP Service installed on this " & _
                "machine. Please check the Readme file for information on how " & _
                "to install SMTP Service.", Me.Text, _
                MessageBoxButtons.OK, MessageBoxIcon.Information)
            service.Stop()
        End If

        ' Ensure the SMTP Service is running. If not, start it.
        If Not service.Status = ServiceControllerStatus.Running Then
            Try
                service.Start()
            Catch
                MsgBox("There was an error when attempting " & _
                    "to start SMTP Service. Please consult the Readme " & _
                    "file for more information.")
            End Try
        End If

        ' Fill the Priority ComboBox with the MailPriority values
        With Priority
            .Items.AddRange(New String() {"Normal", "Low", "High"})
            .SelectedIndex = 0
        End With
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
