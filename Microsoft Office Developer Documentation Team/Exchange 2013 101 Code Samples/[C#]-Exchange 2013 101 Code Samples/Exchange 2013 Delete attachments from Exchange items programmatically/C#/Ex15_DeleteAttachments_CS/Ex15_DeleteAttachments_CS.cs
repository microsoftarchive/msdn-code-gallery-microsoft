using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Attachments
    {
        static IUserData UserData = UserDataFromConsole.GetUserData();
        static ExchangeService service = Service.ConnectToService(UserData, new TraceListener());

        static void Main(string[] args)
        {
            DeleteAttachments(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }
  
        /// <summary>
        /// Demonstrates three ways to delete attachments from an item.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void DeleteAttachments(ExchangeService service)
        {
            // Return a single item.
            ItemView view = new ItemView(1);

            string querystring = "HasAttachments:true Subject:'Message with Attachments' Kind:email";

            // Find the first email message in the Inbox that has attachments and a subject that contains 'Message with Attachments'. 
            // This results in a FindItem call to EWS.
            FindItemsResults<Item> results = service.FindItems(WellKnownFolderName.Inbox, querystring, view);

            if (results.TotalCount > 0)
            {
                EmailMessage email = results.Items[0] as EmailMessage;

                // Get all the attachments on the email message. This results in a GetAttachment call to EWS.
                email.Load(new PropertySet(EmailMessageSchema.Attachments));

                // Remove attachment by index position.
                if (email.Attachments.Count > 0)
                {
                    email.Attachments.RemoveAt(0);
                }

                // Remove attachments by file name.
                foreach (Attachment attachment in email.Attachments)
                {
                    if (attachment.Name.ToUpper() == "THIRDATTACHMENT.JPG")
                    {
                        email.Attachments.Remove(attachment);
                        break;
                    }
                }

                // Remove all attachments from an item. 
                email.Attachments.Clear();

                // Save the updated message. This results in a DeleteAttachment operation call to EWS.
                // If any other properties are updated in addition to the attachments,
                // an UpdateItem operation call to EWS will occur.
                email.Update(ConflictResolutionMode.AlwaysOverwrite);
            }
        }
    }
}
