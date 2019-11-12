using System;
using System.Collections.ObjectModel;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Email
    {
        // Set up the ExchangeService object with credentials, set the EWS URL, and enable tracing.  
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
           SendBatchEmails(service);
           Console.WriteLine("Press or select Enter..."); 
           Console.ReadLine();
        }

        /// <summary>
        /// Creates and tries to send three email messages with one call to EWS. The third email message intentionally fails 
        /// to demonstrate how EWS returns errors for batch requests.
        /// </summary>
        /// <param name="service">A valid ExchangeService object with credentials and the EWS URL.</param>
        static void SendBatchEmails(ExchangeService service)
        {
           // Create three separate email messages.
           EmailMessage message1 = new EmailMessage(service);
           message1.ToRecipients.Add("user1@contoso.com");
           message1.ToRecipients.Add("user2@contoso.com");
           message1.Subject = "Status Update";
           message1.Body = "Project complete!";

           EmailMessage message2 = new EmailMessage(service);
           message2.ToRecipients.Add("user1@contoso.com");
           message2.Subject = "High priority work items";
           message2.Importance = Importance.High;
           message2.Body = "Finish estimate by EOD!";

           EmailMessage message3 = new EmailMessage(service);
           message3.BccRecipients.Add("user1@contoso.com");
           message3.BccRecipients.Add("user2contoso.com"); // Invalid email address format. 
           message3.Subject = "Surprise party!";
           message3.Body = "Don't tell anyone. It will be at 6:00 at Aisha's house. Shhh!";
           message3.Categories.Add("Personal Party");

           Collection<EmailMessage> msgs = new Collection<EmailMessage>() { message1, message2, message3 };

           try
           {
              // Send the batch of email messages. This results in a call to EWS. The response contains the results of the batched request to send email messages.
              ServiceResponseCollection<ServiceResponse> response = service.CreateItems(msgs, WellKnownFolderName.Drafts, MessageDisposition.SendOnly, null);

              // Check the response to determine whether the email messages were successfully submitted.
              if (response.OverallResult == ServiceResult.Success)
              {
                 Console.WriteLine("All email messages were successfully submitted");
                 return;
              }

              int counter = 1;

              /* If the response was not an overall success, access the errors.
               * Results are returned in the order that the action was submitted. For example, the attempt for message1
               * will be represented by the first result since it was the first one added to the collection.
               * Errors are not returned if an NDR is returned.
               */
              foreach (ServiceResponse resp in response)
              {
                 Console.WriteLine("Result (message {0}): {1}", counter, resp.Result);
                 Console.WriteLine("Error Code: {0}", resp.ErrorCode);
                 Console.WriteLine("Error Message: {0}\r\n", resp.ErrorMessage);

                 counter++;
              }
           }
           catch (Exception e)
           {
              Console.WriteLine(e.Message);
           }
        }
    }
}
