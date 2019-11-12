using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_AccessPropertyByName_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            AccessNamedMAPIPropertyByName(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Access a named property by name in a default named property set. Ths sample shows how to get 
        /// the junk email move stamp property.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void AccessNamedMAPIPropertyByName(ExchangeService service)
        {
            // Define the http://schemas.microsoft.com/exchange/junkemailmovestamp secure messaging property. 
            ExtendedPropertyDefinition PidNameExchangeJunkEmailMoveStamp = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.PublicStrings,
                                                                                        "http://schemas.microsoft.com/exchange/junkemailmovestamp",
                                                                                        MapiPropertyType.Integer);
            // Retrieve a collection of 10 items.
            ItemView view = new ItemView(10);
            
            // Create a search filter that filters email based on the existence of the extended property.
            SearchFilter.Exists exchangeJunkEmailMoveStampFilter = new SearchFilter.Exists(PidNameExchangeJunkEmailMoveStamp);

            // Create a property set that includes the extended property definition.
            view.PropertySet =
             new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, PidNameExchangeJunkEmailMoveStamp);

            // Search the Junk Email folder with the defined view and search filter. This results in a FindItem operation call to EWS.
            FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.JunkEmail, exchangeJunkEmailMoveStampFilter, view);

            // Search the item collection returned in the results for the extended property.
            foreach (Item item in findResults.Items)
            {
                Console.WriteLine(item.Subject);

                // Determine whether the item has the custom extended property set.
                if (item.ExtendedProperties.Count > 0)
                {
                    // Display the extended name and value of the extended property.
                    foreach (ExtendedProperty extendedProperty in item.ExtendedProperties)
                    {
                        Console.WriteLine("\tExtended Property Name: " + extendedProperty.PropertyDefinition.Name);
                        Console.WriteLine("\tExtended Property Value: " + extendedProperty.Value);
                    }
                }
                else
                {
                    Console.WriteLine("\tThis");
                }
            }
        }
    }
}
