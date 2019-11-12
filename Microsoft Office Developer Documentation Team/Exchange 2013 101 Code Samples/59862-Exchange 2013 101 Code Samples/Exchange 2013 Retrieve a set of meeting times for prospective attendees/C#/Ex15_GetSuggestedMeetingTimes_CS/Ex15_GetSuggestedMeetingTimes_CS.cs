using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_GetSuggestedMeetingTimes_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());
        
        static void Main(string[] args)
        {
            GetSuggestedMeetingTimes(service);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press any key...");
            Console.Read();
        }

        private static void GetSuggestedMeetingTimes(ExchangeService service)
        {
            // Create a list of attendees.
            List<AttendeeInfo> attendees = new List<AttendeeInfo>();

            attendees.Add(new AttendeeInfo()
            {
                // Change greg@fabrikam.com to your email address.
                SmtpAddress = UserDataFromConsole.GetUserData().EmailAddress,
                AttendeeType = MeetingAttendeeType.Organizer
            });

            attendees.Add(new AttendeeInfo()
            {
                // Change karim@fabrikam.com to your email address.
                SmtpAddress = "karim@fabrikam.com",
                AttendeeType = MeetingAttendeeType.Required
            });

            // Specify suggested meeting time options.
            AvailabilityOptions meetingOptions = new AvailabilityOptions();
            meetingOptions.MeetingDuration = 60;
            meetingOptions.MaximumNonWorkHoursSuggestionsPerDay = 0;
            meetingOptions.GoodSuggestionThreshold = 49;
            meetingOptions.MinimumSuggestionQuality = SuggestionQuality.Good;
            meetingOptions.DetailedSuggestionsWindow = new TimeWindow(DateTime.Now.AddDays(4), DateTime.Now.AddDays(5));

            // Return a set of of suggested meeting times.
            GetUserAvailabilityResults results = service.GetUserAvailability(attendees,
                                                                                 new TimeWindow(DateTime.Now, DateTime.Now.AddDays(2)),
                                                                                     AvailabilityData.Suggestions,
                                                                                     meetingOptions);
            // Display available meeting times.
            Console.WriteLine("Availability for {0} and {1}", attendees[0].SmtpAddress, attendees[1].SmtpAddress);
            Console.WriteLine();

            foreach (Suggestion suggestion in results.Suggestions)
            {
                Console.WriteLine(suggestion.Date);
                Console.WriteLine();
                foreach (TimeSuggestion timeSuggestion in suggestion.TimeSuggestions)
                {
                    Console.WriteLine("Suggested meeting time:" + timeSuggestion.MeetingTime);
                    Console.WriteLine();
                }
            }
        }
    }
}
