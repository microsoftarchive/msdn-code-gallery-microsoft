using System;
using System.IO;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_GetAttachments_CS
    {
        static IUserData UserData = UserDataFromConsole.GetUserData();
        static ExchangeService service = Service.ConnectToService(UserData, new TraceListener());

        [STAThread]
        static void Main(string[] args)
        {
            GetAttachments(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Demonstrates three ways to get file attachments and how to get an item attachment.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void GetAttachments(ExchangeService service)
        {
            // Return a single item.
            ItemView view = new ItemView(1);

            string querystring = "HasAttachments:true Subject:'Message with Attachments' Kind:email";

            // Find the first email message in the Inbox that has attachments. This results in a FindItem operation call to EWS.
            FindItemsResults<Item> results = service.FindItems(WellKnownFolderName.Inbox, querystring, view);

            if (results.TotalCount > 0)
            {
                EmailMessage email = results.Items[0] as EmailMessage;

                // Request all the attachments on the email message. This results in a GetItem operation call to EWS.
                email.Load(new PropertySet(EmailMessageSchema.Attachments));

                foreach (Attachment attachment in email.Attachments)
                {
                    if (attachment is FileAttachment)
                    {
                        FileAttachment fileAttachment = attachment as FileAttachment;

                        // Load the file attachment into memory. This gives you access to the attachment content, which 
                        // is a byte array that you can use to attach this file to another item. This results in a GetAttachment operation
                        // call to EWS.
                        fileAttachment.Load();
                        Console.WriteLine("Load a file attachment with a name = " + fileAttachment.Name);

                        // Load attachment contents into a file. This results in a GetAttachment operation call to EWS.
                        fileAttachment.Load("C:\\temp\\" + fileAttachment.Name);

                        // Put attachment contents into a stream.
                        using (FileStream theStream = new FileStream("C:\\temp\\Stream_" + fileAttachment.Name, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            //This results in a GetAttachment operation call to EWS.
                            fileAttachment.Load(theStream);
                        }
                    }
                    else // Attachment is an item attachment.
                    {
                        ItemAttachment itemAttachment = attachment as ItemAttachment;
                            
                        // Load the item attachment properties. This results in a GetAttachment operation call to EWS.
                        itemAttachment.Load();
                        Console.WriteLine("Loaded an item attachment with Subject = " + itemAttachment.Item.Subject);
                    }
                }
            }
        }
    }
}
