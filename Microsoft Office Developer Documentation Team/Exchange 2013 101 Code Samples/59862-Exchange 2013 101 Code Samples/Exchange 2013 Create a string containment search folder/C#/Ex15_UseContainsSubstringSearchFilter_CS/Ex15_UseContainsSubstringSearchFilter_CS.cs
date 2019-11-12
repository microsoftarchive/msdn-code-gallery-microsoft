using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_UseContainsSubstringSearchFilter_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            UseContainsSubstringSearchFilter(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Create a search filter for filtering items based on a property that contains a specified string.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void UseContainsSubstringSearchFilter(ExchangeService service)
        {
            // The ContainsSubstring filter determines whether a string exists in a string value property.
            // This filter instance filters on the Subject property, where the subject contains the string "Tennis Lesson".
            SearchFilter.ContainsSubstring containsSubstring = new SearchFilter.ContainsSubstring(EmailMessageSchema.Subject, 
                                                                                                  "Tennis Lesson", 
                                                                                                  ContainmentMode.ExactPhrase, 
                                                                                                  ComparisonMode.Exact);
            
            // Create a nonpaged view.
            ItemView view = new ItemView(10);
            view.PropertySet = new PropertySet(EmailMessageSchema.Subject);

            try
            {
                FindItemsResults<Item> results = service.FindItems(WellKnownFolderName.Inbox, containsSubstring, view);

                foreach (Item item in results.Items)
                {
                    Console.WriteLine("Subject: {0}", item.Subject);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
        }
    }
}
