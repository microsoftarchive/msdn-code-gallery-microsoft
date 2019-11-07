using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_UseNotExistsSearchFilter_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            UseNotExistsSearchFilter(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Create a search filter for filtering items based on whether a property exists for an item.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void UseNotExistsSearchFilter(ExchangeService service)
        {
            // Create a definition for an extended property that represents a custom X-Header.
            ExtendedPropertyDefinition xExperimentalHeader = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.InternetHeaders,
                                                                                                   "X-Experimental",
                                                                                                   MapiPropertyType.String);

            // The Exists filter determines whether the a property exists on an item. 
            // This filter indicates whether the custom X-Experimental property exists
            // on an item. If the Exists condition is used, then items with the 
            // X-Experimental property are returned in the search results. The Exists
            // filter will return results even if the value of the property is null
            // or empty. 
            SearchFilter.Exists exists = new SearchFilter.Exists(xExperimentalHeader);

            // The Not filter negates the result of another filter.
            // This filter indicates that the results of the Exists filter are negated.
            SearchFilter.Not not = new SearchFilter.Not(exists);

            // Create a nonpaged view that returns items with the Subject and X-Experimental
            // properties.
            ItemView view = new ItemView(10);
            view.PropertySet = new PropertySet(EmailMessageSchema.Subject, xExperimentalHeader);

            try
            {
                // Find items where the X-Experimental property is present on the item. This results 
                // in a FindItem operation call to EWS.
                FindItemsResults<Item> results = service.FindItems(WellKnownFolderName.MsgFolderRoot, exists, view);

                // Process the results for finding items that contain the X-Experimental property.
                Console.WriteLine("Sent a request to find items where the X-Experimental property exists on the item.");
                UseNotExistsHelper(results, xExperimentalHeader);

                // Find items where the X-Experimental property is not present on the item. This results 
                // in a FindItem operation call to EWS.
                FindItemsResults<Item> results2 = service.FindItems(WellKnownFolderName.MsgFolderRoot, not, view);

                // Process the results for finding items that do not contain the X-Experimental property.
                Console.WriteLine("Sent a request to find items where the X-Experimental property does not exist on the item.");
                UseNotExistsHelper(results2, xExperimentalHeader);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
        }

        private static void UseNotExistsHelper(FindItemsResults<Item> results, ExtendedPropertyDefinition propDef)
        {
            foreach (Item item in results.Items)
            {
                Console.WriteLine("Subject: {0}", item.Subject);
                if (item.ExtendedProperties.Count > 0)
                {
                    foreach (ExtendedProperty prop in item.ExtendedProperties)
                    {
                        if (prop.PropertyDefinition.Equals(propDef))
                        {
                            Console.WriteLine("X-Experimental: {0}", prop.Value.ToString());
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Extended property does not exist on the item.");
                }
            }
        }
    }
}
