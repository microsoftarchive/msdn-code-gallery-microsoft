
'***************************** Module Header ******************************\
'* Module Name:    EmailSender.vb
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

Imports System.Collections.Generic
Imports System.Configuration
Imports System.Net.Configuration
Imports System.Net.Mail

Public Class EmailSender
    ''' <summary>
    ''' Send Email to a list of recipients. 
    ''' </summary>
    ''' <param name="recipientList">A List of MailMessage object, that contains the list of Email Message</param>
    ''' <returns>Returns True, if e-mail is sent successfully otherwise false</returns>
    Public Function SendEmail(recipientList As List(Of MailMessage)) As Boolean
        Dim smtpClient As SmtpClient = Nothing
        Dim config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
        Dim mailSettings = TryCast(config.GetSectionGroup("system.net/mailSettings"), MailSettingsSectionGroup)
        Dim status As Boolean = False
        Try
            ' SMTP settings are defined in app.config file
            smtpClient = New SmtpClient()
            For Each mailMessage As MailMessage In recipientList
                smtpClient.Send(mailMessage)
            Next

            status = True
        Catch e As Exception
            Dim errorMessage As String = String.Empty
            While e IsNot Nothing
                errorMessage += e.ToString()
                e = e.InnerException
            End While

            status = False
        Finally
            If smtpClient IsNot Nothing Then
                smtpClient.Dispose()
            End If
        End Try
        Return status
    End Function

    ''' <summary>
    ''' Asynchronously send Email to a list of recipients. 
    ''' </summary>
    ''' <param name="recipientList">A List of MailMessage object, that contains the list of Email Message</param>
    Public Sub SendEmailAsync(recipientList As List(Of MailMessage))
        Dim smtpClient As SmtpClient = Nothing
        Dim config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
        Dim mailSettings = TryCast(config.GetSectionGroup("system.net/mailSettings"), MailSettingsSectionGroup)
        Dim userState As Object = Nothing
        Try
            ' SMTP settings are defined in app.config file
            smtpClient = New SmtpClient()
            For Each mailMessage As MailMessage In recipientList
                userState = mailMessage
                AddHandler smtpClient.SendCompleted, AddressOf smtpClient_SendCompleted
                smtpClient.SendAsync(mailMessage, userState)
            Next
        Catch e As Exception
            Dim errorMessage As String = String.Empty
            While e IsNot Nothing
                errorMessage += e.ToString()
                e = e.InnerException
            End While
        Finally
            If smtpClient IsNot Nothing Then
                smtpClient.Dispose()
            End If
        End Try
    End Sub

    Private Sub smtpClient_SendCompleted(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs)
        ' Get the Original MailMessage object
        Dim mailMessage As MailMessage = DirectCast(e.UserState, MailMessage)
        Dim subject As String = mailMessage.Subject

        ' Write custom logging code here. Currently it is showing error on console.
        If e.Cancelled Then
            Console.WriteLine("Send canceled for [{0}] with subject [{1}] at [{2}].", mailMessage.[To], subject, DateTime.Now.ToString())
        End If
        If e.[Error] IsNot Nothing Then
            Console.WriteLine("An error {1} occurred when sending mail [{0}] to [{2}] at [{3}] ", subject, e.[Error].ToString(), mailMessage.[To], DateTime.Now.ToString())
        Else
            Console.WriteLine("Message [{0}] is sent to [{1}] at [{2}].", subject, mailMessage.[To], DateTime.Now.ToString())
        End If
    End Sub
End Class

