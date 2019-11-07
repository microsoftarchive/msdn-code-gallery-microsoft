using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_UseAnEqualitySearchFilter_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            UseAnEqualitySearchFilter(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Create a search filter for filtering items based on equality comparisons of property values.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void UseAnEqualitySearchFilter(ExchangeService service)
        {
            // The IsGreaterThan filter determines whether the value of a property is greater than a specific value.  
            // This filter instance filters on the DateTimeReceived property, where the value is greater than a month ago.
            SearchFilter.IsGreaterThan isGreaterThan =  new SearchFilter.IsGreaterThan(EmailMessageSchema.DateTimeReceived, DateTime.Now.AddMonths(-1));

            // The IsGreaterThanOrEqualTo filter determines whether the value of a property is greater than or equal to a specific value.  
            // This filter instance filters on the DateTimeCreated property, where the value is greater than or equal to a week ago.
            SearchFilter.IsGreaterThanOrEqualTo isGreaterThanOrEqualTo = new SearchFilter.IsGreaterThanOrEqualTo(EmailMessageSchema.DateTimeCreated, DateTime.Now.AddDays(-7));

            // The IsLessThan filter determines whether the value of a property is less than a specific value.  
            // This filter instance filters on the DateTimeReceived property, where the value is less than the time an hour ago.
            SearchFilter.IsLessThan isLessThan = new SearchFilter.IsLessThan(EmailMessageSchema.DateTimeReceived, DateTime.Now.AddHours(-1));

            // The IsLessThanOrEqualTo filter determines whether the value of a property is less than or equal to a specific value.  
            // This filter instance filters on the DateTimeCreated property, where the value is less than or equal to the time two days ago.
            SearchFilter.IsLessThanOrEqualTo isLessThanOrEqualTo = new SearchFilter.IsLessThanOrEqualTo(EmailMessageSchema.DateTimeCreated, DateTime.Now.AddDays(-2));

            // The IsEqualTo filter determines whether the value of a property is equal to a specific value.  
            // This filter instance filters on the Importance property where it is set to Normal.
            SearchFilter.IsEqualTo isEqualTo = new SearchFilter.IsEqualTo(EmailMessageSchema.Importance, Importance.Normal);

            // The IsNotEqualTo filter determines whether the value of a property is not equal to a specific value.  
            // This filter instance filters on the IsRead property, where it is not set to true.
            SearchFilter.IsNotEqualTo isNotEqualTo = new SearchFilter.IsNotEqualTo(EmailMessageSchema.IsRead, true);

            // Create a search filter collection that will filter based on an item's Importance and IsRead flag. 
            // Both conditions must pass for an item to be returned in a result set.
            SearchFilter.SearchFilterCollection secondLevelSearchFilterCollection1 = new SearchFilter.SearchFilterCollection(LogicalOperator.And, 
                                                                                                                             isEqualTo, 
                                                                                                                             isNotEqualTo);

            // Create a search filter collection that will filter based on an item's DateTimeCreated and DateTimeReceived properties.
            // All four conditions must pass for an item to be returned in a result set.
            SearchFilter.SearchFilterCollection secondLevelSearchFilterCollection2 = new SearchFilter.SearchFilterCollection(LogicalOperator.And,
                                                                                                                             isGreaterThan,
                                                                                                                             isGreaterThanOrEqualTo,
                                                                                                                             isLessThan,
                                                                                                                             isLessThanOrEqualTo);

            // The SearchFilterCollection contains a collection of search filter collections. Items must pass the search conditions
            // of either collection for an item to be returned in a result set.
            SearchFilter.SearchFilterCollection firstLevelSearchFilterCollection = new SearchFilter.SearchFilterCollection(LogicalOperator.Or,
                                                                                                                           secondLevelSearchFilterCollection1,
                                                                                                                           secondLevelSearchFilterCollection2);


            // Create a nonpaged view and add properties to the results set.
            ItemView view = new ItemView(10);
            view.PropertySet = new PropertySet(EmailMessageSchema.Subject, 
                                               EmailMessageSchema.DateTimeCreated, 
                                               EmailMessageSchema.DateTimeReceived,
                                               EmailMessageSchema.Importance,
                                               EmailMessageSchema.IsRead);

            try
            {
                // Search the Inbox based on the ItemView and the SearchFilterCollection. This results in a FindItem operation call
                // to EWS. 
                FindItemsResults<Item> results = service.FindItems(WellKnownFolderName.Inbox, 
                                                                   firstLevelSearchFilterCollection, 
                                                                   view);

                foreach (Item item in results.Items)
                {
                    Console.WriteLine("\r\nSubject:\t\t{0}", item.Subject);
                    Console.WriteLine("DateTimeCreated:\t{0}", item.DateTimeCreated.ToShortDateString());
                    Console.WriteLine("DateTimeReceived:\t{0}", item.DateTimeReceived.ToShortDateString());
                    Console.WriteLine("Importance:\t\t{0}", item.Importance.ToString());
                    Console.WriteLine("IsRead:\t\t\t{0}", (item as EmailMessage).IsRead.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
        }

    }
}
