using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_DeleteARecurringSeries_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            DeleteARecurringSeries(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Finds a recurring master calendar item and deletes it. This deletes all occurrence 
        /// items.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void DeleteARecurringSeries(ExchangeService service)
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
                            Console.WriteLine("Found a master recurring item.");

                            // This results in a DeleteItem operation call to EWS. This deletes
                            // all occurrences from the mailbox.
                            masterItem.Delete(DeleteMode.HardDelete);

                            Console.WriteLine("Deleted the master recurring item.");
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
