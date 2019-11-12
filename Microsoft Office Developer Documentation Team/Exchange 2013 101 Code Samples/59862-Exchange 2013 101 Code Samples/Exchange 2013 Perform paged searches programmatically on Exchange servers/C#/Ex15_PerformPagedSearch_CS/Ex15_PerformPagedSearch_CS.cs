using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_PerformPagedSearch_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            PerformPagedSearch(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Performs a search for items with "Exchange" in the item body. It returns the results in pages and then
        /// parses the results to identify which type of item is returned.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void PerformPagedSearch(ExchangeService service)
        {
            int initialOffset = 0;
            const int pageSize = 50;
            bool moreItems = true;
            ItemView view = new ItemView(pageSize, initialOffset);
            string querystring = "Body:Exchange";

            while (moreItems)
            {
                try
                {
                    FindItemsResults<Item> results = service.FindItems(WellKnownFolderName.Inbox, querystring, view);

                    foreach (Item item in results.Items)
                    {
                        switch (item.GetType().ToString())
                        {
                            case "Microsoft.Exchange.WebServices.Data.EmailMessage":
                                Console.WriteLine("Found an email with a subject of: {0}", item.Subject);
                                break;
                            case "Microsoft.Exchange.WebServices.Data.MeetingRequest":
                                Console.WriteLine("Found a meeting request with a subject of: {0}", item.Subject);
                                break;
                            case "Microsoft.Exchange.WebServices.Data.MeetingCancellation":
                                Console.WriteLine("Found a meeting cancellation with a subject of: {0}", item.Subject);
                                break;
                            default:
                                Console.WriteLine("Found an unexpected type: ", item.GetType().ToString());
                                break;
                        }
                    }

                    if (results.MoreAvailable == false)
                    {
                        moreItems = false;
                    }

                    else
                    {
                        view.Offset = view.Offset + pageSize;
                        Console.WriteLine("Page #{0}", view.Offset / pageSize);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex.Message);
                    break;
                }
            }
        }
    }
}
