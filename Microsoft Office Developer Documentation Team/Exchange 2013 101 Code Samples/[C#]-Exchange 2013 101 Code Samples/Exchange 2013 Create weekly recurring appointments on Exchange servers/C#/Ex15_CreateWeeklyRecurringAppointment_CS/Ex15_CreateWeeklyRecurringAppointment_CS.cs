using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_CreateWeeklyRecurringAppointment_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            CreateWeeklyRecurringAppointment(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Creates a weekly recurring appointment. To make this a weekly recurring meeting, add attendees
        /// to the Appointment.RequiredAttendees and/or Appointment.OptionalAttendees collections.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void CreateWeeklyRecurringAppointment(ExchangeService service)
        {
            Appointment appointment = new Appointment(service);

            appointment.Subject = "Weekly Tennis Lesson";
            appointment.Body = "My one hour tennis lesson is every Saturday at 10:00 for ten weeks.";
            appointment.Start = new DateTime(2013, 6, 1, 10, 0, 0);
            appointment.End = appointment.Start.AddHours(1);
            appointment.Location = "Contoso Pavilion";
            DayOfTheWeek[] days = new DayOfTheWeek[] { DayOfTheWeek.Saturday };
            appointment.Recurrence = new Recurrence.WeeklyPattern(appointment.Start.Date, 1, days);
            appointment.Recurrence.StartDate = appointment.Start.Date;
            appointment.Recurrence.NumberOfOccurrences = 10;
            
            appointment.Save();
        }
    }
}
