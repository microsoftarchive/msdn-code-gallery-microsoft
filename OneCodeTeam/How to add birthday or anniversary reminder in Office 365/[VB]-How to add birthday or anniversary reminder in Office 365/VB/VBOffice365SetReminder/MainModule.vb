'**************************** Module Header ******************************\
' Module Name:  Program.vb
' Project:      VBOffice365SetReminder
' Copyright (c) Microsoft Corporation.
' 
' In this sample, we will demonstrate how to add birthday or anniversary 
' reminder for the contacts.
' We can search a contact folder to find the contacts that have birthdays or
' anniversaries, and then create the reminders for them. Additionally, we can 
' import a comma-separated values(CSV) file for creating the reminders.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports Microsoft.Exchange.WebServices.Data

Namespace VBOffice365SetReminder
    Friend Class MainModule
        Shared Sub Main(ByVal args() As String)
            ServicePointManager.ServerCertificateValidationCallback =
                AddressOf CallbackMethods.CertificateValidationCallBack
            Dim service As New ExchangeService(ExchangeVersion.Exchange2010_SP2)

            ' Get the information of the account.
            Dim user As New UserInfo()
            service.Credentials = New WebCredentials(user.Account, user.Pwd)

            ' Set the url of server.
            If Not AutodiscoverUrl(service, user) Then
                Return
            End If
            Console.WriteLine()

            Dim contactsFolder As Folder = ReminderHelper.GetContactsFolder(service, "\")
            Console.WriteLine("It's success to get the contact folder.")
            Console.WriteLine()

            Dim contactList As List(Of Contact) = ReminderHelper.GetContactsByBirthday(contactsFolder, Nothing)
            Console.WriteLine("Get the contacts that have the Birthday.")
            ' The user can specify any TimeZone to set the reminder.
            ReminderHelper.SetReminder(service, contactList,TimeZoneInfo.Local)
            Console.WriteLine()

            contactList = ReminderHelper.GetContactsByAnniversary(contactsFolder, Nothing)
            Console.WriteLine("Get the contacts that have the Anniversary.")
            ' The user can specify any TimeZone to set the reminder.
            ReminderHelper.SetReminder(service, contactList, TimeZoneInfo.Local)
            Console.WriteLine()

            Dim filePath As String = "contacts365.csv"

            ' We first import the CSV file and get the contacts.
            Dim contactListFromCSV As List(Of Contact) = ReminderHelper.ImportContactsFromCSV(service, filePath)
            Console.WriteLine("It's success to import the CSV file that has the contacts.")
            ' Then we set the reminders for the contacts.
            ' The user can specify any TimeZone to set the reminder.
            ReminderHelper.SetReminder(service, contactListFromCSV, TimeZoneInfo.Local)
            Console.WriteLine()

            Console.WriteLine("Press any key to exit......")
            Console.ReadKey()
        End Sub

        Private Shared Function AutodiscoverUrl(ByVal service As ExchangeService, ByVal user As UserInfo) As Boolean
            Dim isSuccess As Boolean = False

            Try
                Console.WriteLine("Connecting the Exchange Online......")
                service.AutodiscoverUrl(user.Account, AddressOf CallbackMethods.RedirectionUrlValidationCallback)
                Console.WriteLine()
                Console.WriteLine("It's success to connect the Exchange Online.")

                isSuccess = True
            Catch e As Exception
                Console.WriteLine("There's an error.")
                Console.WriteLine(e.Message)
            End Try

            Return isSuccess
        End Function
    End Class
End Namespace

