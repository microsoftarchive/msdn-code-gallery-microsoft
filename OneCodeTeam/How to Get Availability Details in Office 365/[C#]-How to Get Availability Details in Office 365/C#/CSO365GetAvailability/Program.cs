/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      CSO365GetAvailability
Copyright (c) Microsoft Corporation.

Currently, Outlook Web App (OWA) allows you to check the availability by using 
Schedule Assistant. But you may want to have a list of events to track the 
availability of meeting rooms. In this application, we will demonstrate how 
to availability details in Office 365.
1. You need input the email addresses and the duration what you want to get the 
availability details.
2. The application will check the addresses and the date.
3. At last the application will show the result of the availability details.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Exchange.WebServices.Data;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace CSO365GetAvailability
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback =
CallbackMethods.CertificateValidationCallBack;
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010_SP2);

            // Get the information of the account.
            UserInfo user = new UserInfo();
            service.Credentials = new WebCredentials(user.Account, user.Pwd);

            // Set the url of server.
            if (!AutodiscoverUrl(service, user))
            {
                return;
            }
            Console.WriteLine();

            GetAvailabilityResults(service, user.Account);
            Console.WriteLine();

            Console.WriteLine("Press any key to exit......");
            Console.ReadKey();
        }

        /// <summary>
        /// Get the availability information basing the emailaddresses and the date
        /// </summary>
        static void GetAvailabilityResults(ExchangeService service, String currentAddress)
        {
            do
            {
                Console.WriteLine("Please input the user identity you want to get the " +
                    "availability details:");
                String inputInfo = Console.ReadLine();

                Console.WriteLine("Please input the start date:");
                String startDate = Console.ReadLine();
                Console.WriteLine("Please input the end date:");
                String endDate = Console.ReadLine();

                if (!String.IsNullOrWhiteSpace(inputInfo))
                {
                    // You can input the "EXIT" to exit.
                    if (inputInfo.ToUpper().CompareTo("EXIT") == 0)
                    {
                        return;
                    }

                    String[] identities = inputInfo.Split(',');

                    List<String> emailAddresses = new List<String>();
                    foreach (String identity in identities)
                    {
                        NameResolutionCollection nameResolutions =
                            service.ResolveName(identity, ResolveNameSearchLocation.DirectoryOnly, true);
                        if (nameResolutions.Count != 1)
                        {
                            Console.WriteLine("{0} is invalid user identity.", identity);
                        }
                        else
                        {
                            String emailAddress = nameResolutions[0].Mailbox.Address;
                            emailAddresses.Add(emailAddress);
                        }
                    }
                    if (emailAddresses.Count > 0)
                    {
                        GetAvailabilityDetails(service, startDate, endDate, emailAddresses.ToArray());
                    }
                }
                else
                {
                    // We can also directly press Enter to get the availability details of the 
                    // login account.
                    GetAvailabilityDetails(service, startDate, endDate, currentAddress);
                }
                Console.WriteLine();
            } while (true);
        }

        /// <summary>
        /// Get the availability details of the accounts
        /// </summary>
        static void GetAvailabilityDetails(ExchangeService service, String startDate,
            String endDate, params String[] emailAddresses)
        {
            // If the date is invaild, we will set today as the start date.
            DateTime startMeetingDate;
            startMeetingDate =
                DateTime.TryParse(startDate, out startMeetingDate) ? startMeetingDate : DateTime.Now;
            // If the date is invaild, we will set two days after the start date as the end date.
            DateTime endMeetingDate;
            endMeetingDate =
                DateTime.TryParse(endDate, out endMeetingDate) && endMeetingDate >= startMeetingDate ?
                endMeetingDate : startMeetingDate.AddDays(2);

            List<AttendeeInfo> attendees = new List<AttendeeInfo>();
            foreach (String emailAddress in emailAddresses)
            {
                AttendeeInfo attendee = new AttendeeInfo(emailAddress);
                attendees.Add(attendee);
            }

            TimeWindow timeWindow = new TimeWindow(startMeetingDate, endMeetingDate);
            AvailabilityOptions availabilityOptions = new AvailabilityOptions();
            availabilityOptions.MeetingDuration = 60;

            GetUserAvailabilityResults userAvailabilityResults = service.GetUserAvailability(attendees,
                timeWindow, AvailabilityData.FreeBusyAndSuggestions, availabilityOptions);
            Console.WriteLine("{0,-15}{1,-21}{2,-11}{3,-14}{4,-10}{5,-9}", "FreeBusyStatus",
                "StartTime", "EndTime", "Subject", "Location", "IsMeeting");
            foreach (AttendeeAvailability userAvailabilityResult in
                userAvailabilityResults.AttendeesAvailability)
            {
                if (userAvailabilityResult.ErrorCode.CompareTo(ServiceError.NoError) == 0)
                {
                    foreach (CalendarEvent calendarEvent in userAvailabilityResult.CalendarEvents)
                    {
                        Console.WriteLine("{0,-15}{1,-21}{2,-11}{3,-14}{4,-10}{5,-9}",
                            calendarEvent.FreeBusyStatus,
                            calendarEvent.StartTime.ToShortDateString() + " " +
                            calendarEvent.StartTime.ToShortTimeString(),
                            calendarEvent.EndTime.ToShortTimeString(),
                            calendarEvent.Details.Subject,
                            calendarEvent.Details.Location,
                            calendarEvent.Details.IsMeeting);
                    }
                }
            }
        }


        private static Boolean AutodiscoverUrl(ExchangeService service, UserInfo user)
        {
            Boolean isSuccess = false;

            try
            {
                Console.WriteLine("Connecting the Exchange Online......");
                service.AutodiscoverUrl(user.Account, CallbackMethods.RedirectionUrlValidationCallback);
                Console.WriteLine();
                Console.WriteLine("It's success to connect the Exchange Online.");

                isSuccess = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("There's an error.");
                Console.WriteLine(e.Message);
            }

            return isSuccess;
        }
    }
}
