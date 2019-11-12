
'***************************** Module Header ******************************\
'* Module Name:    MainModule.vb
'* Project:        VBEmailScheduler
'* Copyright (c) Microsoft Corporation
'*
'* The project shows you how to send email at particular date and time.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\****************************************************************************


Imports System.Net.Configuration
Imports System.Configuration
Imports System.Net.Mail

Module MainModule

    Sub Main()
        Dim emailSender = New EmailSender()
        Dim config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
        Dim mailSettings = TryCast(config.GetSectionGroup("system.net/mailSettings"), MailSettingsSectionGroup)

        ' Create a Mail Message
        Dim mailMessage = New MailMessage()

        ' Receiver’s E-Mail address. 
        mailMessage.[To].Add("onecode@microsoft.com")

        ' Subject of Email
        mailMessage.Subject = "Send Email OneCode Sample"

        mailMessage.SubjectEncoding = System.Text.Encoding.UTF8
        mailMessage.Body = "This is a test mail comes from OneCode Team."
        ' Message Body
        mailMessage.BodyEncoding = System.Text.Encoding.UTF8
        mailMessage.IsBodyHtml = True
        mailMessage.Priority = MailPriority.Normal
        ' Email priority
        mailMessage.From = New MailAddress(mailSettings.Smtp.From, "OneCode", System.Text.Encoding.UTF8)

        Dim mailMessages = New List(Of MailMessage)()
        mailMessages.Add(mailMessage)

        ' Send email in Synchronous manner
        emailSender.SendEmail(mailMessages)

        ' Send Email Asynchronously
        emailSender.SendEmailAsync(mailMessages)

        ' Remove this line, after adding VBEmailScheduler.exe to Windows Task Scheduler
        Console.ReadLine()
    End Sub

End Module
