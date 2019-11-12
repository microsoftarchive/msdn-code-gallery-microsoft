using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_AccessPropertyByGUIDName_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            AccessMAPIPropertyInPropertySetByGuidAndName(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Access a property by its property set identified by its GUID and property identifier.
        /// This sample shows how to get the task due date property.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void AccessMAPIPropertyInPropertySetByGuidAndName(ExchangeService service)
        {
            // Define the normalized subject property.
            ExtendedPropertyDefinition PidLidTaskDueDate = new ExtendedPropertyDefinition(new Guid("00062003-0000-0000-C000-000000000046"),
                                                                                              0x00008105, 
                                                                                              MapiPropertyType.SystemTime);

            // Retrieve a collection of 10 items.
            ItemView view = new ItemView(10);

            // Create a search filter that filters email based on the existence of the extended property.
            SearchFilter.Exists filterPidLidTaskDueDate = new SearchFilter.Exists(PidLidTaskDueDate);

            // Create a property set that includes the extended property definition.
            view.PropertySet =
             new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, PidLidTaskDueDate);

            FindItemsResults<Item> findResults;

            try
            {
                // Search the default Tasks folder with the defined view and search filter. This results in a FindItem operation call to EWS.
                findResults = service.FindItems(WellKnownFolderName.Tasks, filterPidLidTaskDueDate, view);

                // Search the item collection returned in the results for the extended property.
                foreach (Item item in findResults.Items)
                {
                    Console.WriteLine("Item subject: {0}", item.Subject);

                    // Determine whether the item has the custom extended property set.
                    if (item.ExtendedProperties.Count > 0)
                    {
                        // Display the extended name and value of the extended property.
                        foreach (ExtendedProperty extendedProperty in item.ExtendedProperties)
                        {
                            Console.WriteLine("\Extended Property Value: " + extendedProperty.Value);
                        }
                    }
                    else
                    {
                        Console.WriteLine("\Did not find the PidLidTaskDueDate property.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }

        }
    }
}
