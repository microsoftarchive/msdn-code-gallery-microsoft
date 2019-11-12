using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_UseBitmaskSearchFilter_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            UseBitmaskSearchFilter(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Create a bitmask search filter for filtering out items that have ever been read.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void UseBitmaskSearchFilter(ExchangeService service)
        {
            // Define the property that we will use for the bitmask filter. This definition is for the 
            // PidTagMessageFlags property.
            ExtendedPropertyDefinition PidTagMessageFlags = new ExtendedPropertyDefinition(0x0E07, MapiPropertyType.Integer);

            // The ExcludesBitmask filter determines whether a property excludes the value defined by a bitmask.
            // This filter indicates that the PidTagMessageFlags property is filtered on the mfEverRead flag, which
            // indicates whether the mail has ever been read.
            SearchFilter.ExcludesBitmask excludesBitmask = new SearchFilter.ExcludesBitmask(PidTagMessageFlags, 0x00000400);

            // Create a nonpaged view.
            ItemView view = new ItemView(10);
            view.PropertySet = new PropertySet(PidTagMessageFlags, EmailMessageSchema.Subject);

            try
            {
                // Find the first 10 items that exclude the mfEverRead flag. This results in a FindItem operation call to EWS.                
                FindItemsResults<Item> results = service.FindItems(WellKnownFolderName.Inbox, excludesBitmask, view);

                foreach (Item item in results.Items)
                {
                    foreach (ExtendedProperty prop in item.ExtendedProperties)
                    {
                        string hexValue = ((int)prop.Value).ToString("X8");
                        string hexId = ((int)prop.PropertyDefinition.Tag).ToString("X4");
                        Console.WriteLine("Item subject: {0}", item.Subject);
                        Console.WriteLine("PidTagMessageFlags({0}) value: {1}", hexId, hexValue);
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
