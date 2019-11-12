using System;
using System.Collections.Generic;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_FindConversation_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            FindConversation(service);
            Console.WriteLine("Press or select Enter...");
            Console.ReadLine();
        }

        /// <summary>
        /// Demonstrates how to search for conversations by using the EWS Managed API.
        /// </summary>
        /// <param name="service"></param>
        static void FindConversation(ExchangeService service)
        {
            const int pageSize = 5;
            int offset = 0;

            // Create the view of conversations returned in the response. This view will return the first five
            // conversations starting with an offset of 0 from the beginning of the results set.
            ConversationIndexedItemView view = new ConversationIndexedItemView(pageSize, offset, OffsetBasePoint.Beginning);

            try
            {

                // Search the Inbox for conversations and return a results set based on the specified view.
                // This is a content index search based on the Subject and Sent properties. 
                // This results in a call to EWS. 
                ICollection<Conversation> conversations = service.FindConversation(view,
                                                                                    WellKnownFolderName.Inbox,
                                                                                    "Subject:Exchange Sent:01/01/2013..04/22/2013");

                // Examine properties on each conversation returned in the response.
                foreach (Conversation conversation in conversations)
                {
                    Console.WriteLine("Conversation Topic: " + conversation.Topic);
                    Console.WriteLine("Last Delivered: " + conversation.LastDeliveryTime.ToString());

                    // Identify each unique recipient of items in the conversation.
                    foreach (string GlUniqRec in conversation.GlobalUniqueRecipients)
                    {
                        Console.WriteLine("Global Unique Recipient: " + GlUniqRec);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
