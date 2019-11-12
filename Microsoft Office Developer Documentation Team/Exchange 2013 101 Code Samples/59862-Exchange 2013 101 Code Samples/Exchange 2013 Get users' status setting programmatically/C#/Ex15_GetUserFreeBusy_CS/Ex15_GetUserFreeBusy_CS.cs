using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_GetUserFreeBusy_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());
        
        static void Main(string[] args)
        {
            // How to get User Free/Busy information
            GetUserFreeBusy(service);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        private static void GetUserFreeBusy(ExchangeService service)
        {
            // Create a list of attendees
            List<AttendeeInfo> attendees = new List<AttendeeInfo>();

            attendees.Add(new AttendeeInfo()
            {
                // Use the email address supplied from the console for the organizer's email address
                SmtpAddress = UserDataFromConsole.GetUserData().EmailAddress,
                AttendeeType = MeetingAttendeeType.Organizer
            });

            attendees.Add(new AttendeeInfo()
            {
                // Change karim@fabrikam.com to the email address of your prospective attendee
                SmtpAddress = "karim@fabrikam.com",
                AttendeeType = MeetingAttendeeType.Required
            });

            // Specify availability options
            AvailabilityOptions availabilityOptions = new AvailabilityOptions();
            availabilityOptions.MeetingDuration = 30;
            availabilityOptions.RequestedFreeBusyView = FreeBusyViewType.FreeBusy;

            // Return a set of of free/busy times
            GetUserAvailabilityResults freeBusyResults = service.GetUserAvailability(attendees,
                                                                                 new TimeWindow(DateTime.Now, DateTime.Now.AddDays(1)),
                                                                                     AvailabilityData.FreeBusy,
                                                                                     availabilityOptions);
            // Display available meeting times
            Console.WriteLine("Availability for {0} and {1}", attendees[0].SmtpAddress, attendees[1].SmtpAddress);
            Console.WriteLine();

            foreach (AttendeeAvailability availability in freeBusyResults.AttendeesAvailability)
            {
                Console.WriteLine(availability.Result);
                Console.WriteLine();
                foreach (CalendarEvent calendarItem in availability.CalendarEvents)
                {
                    Console.WriteLine("Free/busy status: " + calendarItem.FreeBusyStatus);
                    Console.WriteLine("Start time: " + calendarItem.StartTime);
                    Console.WriteLine("End time: " + calendarItem.EndTime);
                    Console.WriteLine();
                }
            }
        }
    }
}
