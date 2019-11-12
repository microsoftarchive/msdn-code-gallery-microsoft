using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_AccessPropertyById_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            AccessNamedMAPIPropertyById(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Access a named property by identifier in a default named property set. Ths sample shows how to get 
        /// the location property. 
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void AccessNamedMAPIPropertyById(ExchangeService service)
        {

            // Define the meeting location property. 
            ExtendedPropertyDefinition PidLidLocation = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.Appointment,
                                                                                        0x00008208,
                                                                                        MapiPropertyType.String);

            // Retrieve a collection of 10 items.
            ItemView view = new ItemView(10);

            // Create a search filter that filters email based on the existence of the extended property.
            SearchFilter.Exists filterPidLidLocation = new SearchFilter.Exists(PidLidLocation);

            // Create a property set that includes the extended property definition.
            view.PropertySet =
             new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, PidLidLocation);

            FindItemsResults<Item> findResults; 

            // Search the default calendar folder with the defined view and search filter. This results in a call to EWS by
            // using the FindItem operation.

            try
            {
                findResults = service.FindItems(WellKnownFolderName.Calendar, filterPidLidLocation, view);


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
                            Console.WriteLine("\tExtended Property Value: " + extendedProperty.Value);
                        }
                    }
                    else
                    {
                        Console.WriteLine("\Did not find the PidLidLocation property.");
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
