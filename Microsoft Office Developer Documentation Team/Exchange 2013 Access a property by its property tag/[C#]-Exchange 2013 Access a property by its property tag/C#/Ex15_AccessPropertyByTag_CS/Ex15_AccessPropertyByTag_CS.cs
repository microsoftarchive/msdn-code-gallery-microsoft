using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_AccessPropertyByTag_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            AccessMAPIPropertyByPropertyTag(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }
        /// <summary>
        /// Access a property by its property tag. Ths sample shows how to get 
        /// the normalized subject property.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void AccessMAPIPropertyByPropertyTag(ExchangeService service)
        {
            // Define the normalized subject property.
            ExtendedPropertyDefinition PidTagNormalizedSubject = new ExtendedPropertyDefinition(0x0E1D, MapiPropertyType.String);

            // Retrieve a collection of 10 items.
            ItemView view = new ItemView(10);

            // Create a search filter that filters email based on the existence of the extended property.
            SearchFilter.Exists filterPidTagNormalizedSubject = new SearchFilter.Exists(PidTagNormalizedSubject);

            // Create a property set that includes the extended property definition.
            view.PropertySet =
             new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, PidTagNormalizedSubject);

            FindItemsResults<Item> findResults;

            // Search the default Inbox folder with the defined view and search filter. This results in a FindItem operation call to EWS.

            try
            {
                findResults = service.FindItems(WellKnownFolderName.Inbox, filterPidTagNormalizedSubject, view);

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
                        Console.WriteLine("\Did not find the PidTagNormalizedSubject property.");
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
