using System;
using System.Collections.ObjectModel;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_CopyManyItems_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            CopyManyItems(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Copies two items between folders in a batched call to EWS.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void CopyManyItems(ExchangeService service)
        {
            // Create two items to copy copy. You can copy any item type in your Exchange mailbox. 
            // You will need to save these items to your Exchange mailbox before they can be copied.
            EmailMessage email1 = new EmailMessage(service);
            email1.Subject = "Draft email one";
            email1.Body = new MessageBody(BodyType.Text, "Draft body of the mail.");

            EmailMessage email2 = new EmailMessage(service);
            email2.Subject = "Draft email two";
            email1.Body = new MessageBody(BodyType.Text, "Draft body of the mail.");

            Collection<EmailMessage> messages = new Collection<EmailMessage>();
            messages.Add(email1);
            messages.Add(email2);

            try
            {
                // This results in a CreateItem call to EWS. The items are created on the server.
                // The response contains the item identifiers of the newly created items. The items on the client
                // now have item identifiers, which are needed to make a copy.
                ServiceResponseCollection<ServiceResponse> responses = service.CreateItems(messages, WellKnownFolderName.Drafts, MessageDisposition.SaveOnly, null);

                if (responses.OverallResult == ServiceResult.Success)
                {
                    Console.WriteLine("Successfully created items to be copied.");
                }
                else
                {
                    throw new Exception("The batch creation of the email message draft items was not successful.");
                }


            }
            catch (ServiceResponseException ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }

            // Get item identifiers of the items to be copied in a batch operation.
            Collection<ItemId> itemIds = new Collection<ItemId>();
            foreach (EmailMessage email in messages)
            {
                itemIds.Add(email.Id);
            }

            try
            {
                // You can create copies of items in a batch request. This will result in a CopyItem call to EWS.
                // Unlike the EmailMessage.Copy method, the batch request takes a collection of item identifiers 
                // that identify the items that will be copied. This example makes a copy of the items in the DeletedItems folder.
                ServiceResponseCollection<MoveCopyItemResponse> responses = service.CopyItems(itemIds, WellKnownFolderName.DeletedItems);

                if (responses.OverallResult == ServiceResult.Success)
                {
                    Console.WriteLine("Successfully created copies of the items.");
                }
                else
                {
                    throw new Exception("The batch copy of the email message items was not successful.");
                }
            }
            catch (ServiceResponseException ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
        }

    }
}
