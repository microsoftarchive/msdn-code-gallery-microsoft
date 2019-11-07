using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_ModifyItemsInARecurringSeries_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            ModifyItemsInARecurringSeries(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Finds a recurring master calendar item and makes a modification to one of the occurrence items.
        /// The occurrence item then becomes an exception item in the recurring series.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void ModifyItemsInARecurringSeries(ExchangeService service)
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
                            // This will load occurrence information.
                            masterItem.Load();

                            Console.WriteLine("Number of occurrences: " + masterItem.Recurrence.NumberOfOccurrences);

                            if (masterItem.Recurrence.NumberOfOccurrences > 0)
                            {
                                // Access the first occurrence in the recurring series.
                                Appointment occurrence = Appointment.BindToOccurrence(service, new ItemId(masterItem.Id.UniqueId), 1);

                                // Update the importance on the occurrence.
                                occurrence.Importance = Importance.High;

                                // This results in an UpdateItem operation call to EWS. At this point, the occurrence has 
                                // been updated on the server.
                                occurrence.Update(ConflictResolutionMode.AutoResolve);

                                // Synchronize the recurring master from the service. This results in a GetItem operation call to EWS.  
                                // Note that because the ModifiedOccurrences property is read only,
                                // you can't update the property on the recurring master on the client.
                                masterItem.Load(new PropertySet(AppointmentSchema.ModifiedOccurrences));

                                // Identify the number of modified occurrences.
                                Console.WriteLine("Number of modified occurrences: " + masterItem.ModifiedOccurrences.Count);
                            }
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
