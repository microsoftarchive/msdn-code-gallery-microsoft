using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_UpdateARecurringSeries_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            UpdateARecurringSeries(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Finds a recurring master calendar item and makes a modification to the recurring master item,
        /// which results in changes to all the occurrences.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void UpdateARecurringSeries(ExchangeService service)
        {
            // Specify a view that returns one recurring master item.
            ItemView view = new ItemView(1);

            string querystring = "Subject:'Weekly Tennis Lesson'";

            try
            {
                // Find the first appointment in the calendar with 'Weekly Tennis Lesson' set for the subject property.
                // This results in a FindItem operation call to EWS. This will return either a recurring master
                // appointment or a nonrecurring single appointment.
                FindItemsResults<Item> masterResults = service.FindItems(WellKnownFolderName.Calendar, querystring, view);

                foreach (Item item in masterResults.Items)
                {
                    if (item is Appointment)
                    {
                        Appointment masterItem = item as Appointment;
                        if (masterItem.AppointmentType == AppointmentType.RecurringMaster)
                        {
                            // Update the subject and move the meeting ahead one hour.
                            masterItem.Subject = masterItem.Subject + " (Updated time)";
                            masterItem.Start = masterItem.Start.AddHours(1);
                            masterItem.End = masterItem.Start.AddHours(1);

                            // This results in an UpdateItem operation call to EWS. At this point, the recurring 
                            // master has been updated on the server. All the occurrences are updated with this call.
                            masterItem.Update(ConflictResolutionMode.AutoResolve);

                            Console.WriteLine("Updated the recurring master item:\r\n");
                            Console.WriteLine("New subject: " + masterItem.Subject);
                            Console.WriteLine("New start time: " + masterItem.Start.ToShortTimeString());
                            Console.WriteLine("New end time: " + masterItem.End.ToShortTimeString());
                        }
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
