using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_AccessCalendarItemsInARecurringSeries_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            AccessCalendarItemsInARecurringSeries(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Finds recurring master calendar items, occurrences and exceptions for recurring calendar items,
        /// and single occurrence calendar items by using the FindItem operation.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void AccessCalendarItemsInARecurringSeries(ExchangeService service)
        {
            // Specify a view that returns up to five recurring master items.
            ItemView view = new ItemView(5);

            // Specify a calendar view for returning instances of a recurring series.
            DateTime startDate = new DateTime(2013, 6, 1);
            DateTime endDate = new DateTime(2013, 8, 1);
            CalendarView calView = new CalendarView(startDate, endDate);

            string querystring = "Subject:'Weekly Tennis Lesson'";

            try
            {
                // Find up to the first five recurring master appointments in the calendar with 'Weekly Tennis Lesson' set for the subject property.
                // This results in a FindItem operation call to EWS. This will return the recurring master
                // appointment.
                FindItemsResults<Item> masterResults = service.FindItems(WellKnownFolderName.Calendar, querystring, view);

                foreach (Item item in masterResults.Items)
                {
                    Appointment appointment = item as Appointment;

                    if (appointment.AppointmentType == AppointmentType.RecurringMaster)
                    {
                        Console.WriteLine("Appointment is the recurring master appointment.");
                    }
                    else
                    {
                        // Calendar item is not part of a recurring series.
                        // The value of appointment.AppointmentType is AppointmentType.Single.
                        Console.WriteLine("Appointment is not part of a recurring series.");
                    }
                }

                // Find all the appointments in the calendar based on the dates set in the CalendarView.
                // This results in a FindItem call to EWS. This will return the occurrences and exceptions
                // to a recurring series and will return appointments that are not part of a recurring series. This will not return 
                // recurring master items. Note that a search restriction or querystring cannot be used with a CalendarView.
                FindItemsResults<Item> instanceResults = service.FindItems(WellKnownFolderName.Calendar, calView);

                foreach (Item item in instanceResults.Items)
                {
                    Appointment appointment = item as Appointment;

                    if (appointment.AppointmentType == AppointmentType.Occurrence)
                    {
                        Console.WriteLine("Appointment is a recurring occurrence.");
                    }
                    else if (appointment.AppointmentType == AppointmentType.Exception)
                    {
                        Console.WriteLine("Appointment is an exception occurrence.");
                    }
                    else
                    {
                        // Calendar item is not part of a recurring series.
                        // The value of appointment.AppointmentType is AppointmentType.Single.
                        Console.WriteLine("Appointment is not part of a recurring series.");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

    }
}
