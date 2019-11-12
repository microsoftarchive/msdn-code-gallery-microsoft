'**************************** Module Header ******************************\
' Module Name:  MainModule.vb
' Project:      VBO365GetAvailability
' Copyright (c) Microsoft Corporation.
' 
' Currently, Outlook Web App (OWA) allows you to check the availability by using 
' Schedule Assistant. But you may want to have a list of events to track the 
' availability of meeting rooms. In this application, we will demonstrate how 
' to availability details in Office 365.
' 1. You need input the email addresses and the duration what you want to get the 
' availability details.
' 2. The application will check the addresses and the date.
' 3. At last the application will show the result of the availability details.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports Microsoft.Exchange.WebServices.Data
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates

Namespace VBO365GetAvailability
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

            GetAvailabilityResults(service, user.Account)
            Console.WriteLine()

            Console.WriteLine("Press any key to exit......")
            Console.ReadKey()
        End Sub

        ''' <summary>
        ''' Get the availability information basing the emailaddresses and the date
        ''' </summary>
        Private Shared Sub GetAvailabilityResults(ByVal service As ExchangeService,
                                                  ByVal currentAddress As String)
            Do
                Console.WriteLine("Please input the user identity you want to get the " & "availability details:")
                Dim inputInfo As String = Console.ReadLine()

                Console.WriteLine("Please input the start date:")
                Dim startDate As String = Console.ReadLine()
                Console.WriteLine("Please input the end date:")
                Dim endDate As String = Console.ReadLine()

                If Not String.IsNullOrWhiteSpace(inputInfo) Then
                    ' You can input the "EXIT" to exit.
                    If inputInfo.ToUpper().CompareTo("EXIT") = 0 Then
                        Return
                    End If

                    Dim identities() As String = inputInfo.Split(","c)

                    Dim emailAddresses As New List(Of String)()
                    For Each identity As String In identities
                        Dim nameResolutions As NameResolutionCollection =
                            service.ResolveName(identity, ResolveNameSearchLocation.DirectoryOnly, True)
                        If nameResolutions.Count <> 1 Then
                            Console.WriteLine("{0} is invalid user identity.", identity)
                        Else
                            Dim emailAddress As String = nameResolutions(0).Mailbox.Address
                            emailAddresses.Add(emailAddress)
                        End If
                    Next identity
                    If emailAddresses.Count > 0 Then
                        GetAvailabilityDetails(service, startDate, endDate, emailAddresses.ToArray())
                    End If
                Else
                    ' We can also directly press Enter to get the availability details of the 
                    ' login account.
                    GetAvailabilityDetails(service, startDate, endDate, currentAddress)
                End If
                Console.WriteLine()
            Loop While True
        End Sub

        ''' <summary>
        ''' Get the availability details of the accounts
        ''' </summary>
        Private Shared Sub GetAvailabilityDetails(ByVal service As ExchangeService,
                                                  ByVal startDate As String, ByVal endDate As String,
                                                  ByVal ParamArray emailAddresses() As String)
            ' If the date is invaild, we will set today as the start date.
            Dim startMeetingDate As Date
            startMeetingDate = If(Date.TryParse(startDate, startMeetingDate), startMeetingDate, Date.Now)
            ' If the date is invaild, we will set two days after the start date as the end date.
            Dim endMeetingDate As Date
            endMeetingDate = If(Date.TryParse(endDate, endMeetingDate) AndAlso
                                endMeetingDate >= startMeetingDate, endMeetingDate,
                                startMeetingDate.AddDays(2))

            Dim attendees As New List(Of AttendeeInfo)()
            For Each emailAddress As String In emailAddresses
                Dim attendee As New AttendeeInfo(emailAddress)
                attendees.Add(attendee)
            Next emailAddress

            Dim timeWindow As New TimeWindow(startMeetingDate, endMeetingDate)
            Dim availabilityOptions As New AvailabilityOptions()
            availabilityOptions.MeetingDuration = 60

            Dim userAvailabilityResults As GetUserAvailabilityResults =
                service.GetUserAvailability(attendees, timeWindow,
                                            AvailabilityData.FreeBusyAndSuggestions, availabilityOptions)
            Console.WriteLine("{0,-15}{1,-21}{2,-11}{3,-14}{4,-10}{5,-9}",
                            "FreeBusyStatus", "StartTime", "EndTime", "Subject", "Location", "IsMeeting")
            For Each userAvailabilityResult As AttendeeAvailability In
                userAvailabilityResults.AttendeesAvailability
                If userAvailabilityResult.ErrorCode.CompareTo(ServiceError.NoError) = 0 Then
                    For Each calendarEvent As CalendarEvent In userAvailabilityResult.CalendarEvents
                        Console.WriteLine("{0,-15}{1,-21}{2,-11}{3,-14}{4,-10}{5,-9}",
                                          calendarEvent.FreeBusyStatus,
                                          calendarEvent.StartTime.ToShortDateString() & " " &
                                          calendarEvent.StartTime.ToShortTimeString(),
                                          calendarEvent.EndTime.ToShortTimeString(),
                                          calendarEvent.Details.Subject,
                                          calendarEvent.Details.Location,
                                          calendarEvent.Details.IsMeeting)
                    Next calendarEvent
                End If
            Next userAvailabilityResult
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
