'*************************** Module Header ******************************\
' Module Name:	MainModule.vb
' Project:		VBSMTPSendEmail
' Copyright (c) Microsoft Corporation.
' 
' VBSMTPSendEmail demonstrates sending email with attachment and embedded 
' image in the message body using SMTP server from a VB.NET program.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

#Region "Imports directives"

Imports System.Net.Mail
Imports System.Net

#End Region


Module MainModule

    Sub Main()

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' Build an email object.
        ' 

        Console.WriteLine("Build email object")
        Dim mail As New MailMessage
        mail.To.Add("anyreceiver@anydomain.com")
        mail.From = New MailAddress("anyaddress@anydomain.com")
        mail.Subject = "Test email of All-In-One Code Framework - VBSMTPSendEmail"
        mail.Body = "Welcome to <a href='http://cfx.codeplex.com'>All-In-One Code Framework</a>!"
        mail.IsBodyHtml = True

        ' Attachments
        Console.WriteLine("Add attachment")
        Dim attachedFile As String = "<attached file path>"
        mail.Attachments.Add(New Attachment(attachedFile))

        ' Embedded image in the message body
        Console.WriteLine("Embed image")
        mail.Body += "<br/><img alt="""" src=""cid:image1"">"

        Dim imgFile As String = "<image file path>"
        Dim htmlView As AlternateView = _
        AlternateView.CreateAlternateViewFromString(mail.Body, Nothing, _
                                                    "text/html")
        Dim imgLink As LinkedResource = New LinkedResource(imgFile, _
                                                           "image/jpg")
        imgLink.ContentId = "image1"
        imgLink.TransferEncoding = System.Net.Mime.TransferEncoding.Base64
        htmlView.LinkedResources.Add(imgLink)
        mail.AlternateViews.Add(htmlView)


        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' Configure the SMTP client and send the email.
        ' 

        ' Configure the SMTP client
        Dim smtp As New SmtpClient
        smtp.Host = "smtp.live.com"
        smtp.Credentials = New NetworkCredential( _
        "myaccount@live.com", "mypassword")
        smtp.EnableSsl = True

        ' Send the email
        Console.WriteLine("Sending email...")
        smtp.Send(mail)

    End Sub

End Module
