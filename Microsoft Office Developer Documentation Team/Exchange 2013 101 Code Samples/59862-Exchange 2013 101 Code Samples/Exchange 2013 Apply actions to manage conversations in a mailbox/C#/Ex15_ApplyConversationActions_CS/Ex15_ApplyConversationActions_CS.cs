using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_ApplyConversationActions_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            ApplyConversationActions(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Demonstrates how to apply actions on conversations items by using the EWS Managed API.
          /// </summary>
        /// <param name="service">An ExchangeService object that has credentials and an EWS URL set on it.</param>
        static void ApplyConversationActions(ExchangeService service)
        {
           // Create the conversation view that is returned in the response. This view will return the first 
           // conversation that satisfies the search criteria.
           ConversationIndexedItemView view = new ConversationIndexedItemView(1, 0, OffsetBasePoint.Beginning);

           // Create a list of categories to apply to a conversation.
           List<string> categories = new List<string>();
           categories.Add("Customer");
           categories.Add("System Integrator");

           try
           {
               // Search the Inbox for a conversation and return a results set with the specified view.
               // This results in a call to EWS. 
               ICollection<Conversation> conversations = service.FindConversation(view, WellKnownFolderName.Inbox, "Subject:\"Contoso systems\"");

               foreach (Conversation conversation in conversations)
               {
                   // Apply categorization to all items in the conversation and process the request
                   // synchronously after enabling this rule and all item categorization has been applied. 
                   // This results in a call to EWS. These categories are not added to the master category list.
                   conversation.EnableAlwaysCategorizeItems(categories, true);

                   // Apply an always move rule to all items in the conversation and move the items
                   // to the Drafts folder. Process the request asynchronously and return the response. 
                   // immediately. This results in a call to EWS. 
                   conversation.EnableAlwaysMoveItems(WellKnownFolderName.Drafts, false);
               }
           }
           catch (Exception ex)
           {
               Console.WriteLine(ex.Message);
           }
        }
    }
}
