using System;
using System.Collections.ObjectModel;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_DeleteAnItem_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            DeleteAnItem(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Deletes a single item in a call to EWS.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void DeleteAnItem(ExchangeService service)
        {
            // Create an item to be deleted. You can delete any item type in your Exchange mailbox. 
            // You will need to save these items to your Exchange mailbox before they can be deleted.
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
                // now have item identifiers. You need the identifiers to delete the item.
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

            try
            {
                // You can delete a single item. Both of the following lines will result in a DeleteItem operation call to EWS.
                // The email message to delete must be saved on the server before you can delete it. 
                email1.Delete(DeleteMode.HardDelete);
                email2.Delete(DeleteMode.HardDelete);
            }

            catch (ServiceResponseException ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
        }
    }
}
