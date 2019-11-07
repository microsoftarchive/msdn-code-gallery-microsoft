using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Email
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
           AccessEmailInternetMessageHeaders(service);
           Console.WriteLine("Press or select Enter..."); 
           Console.Read();
        }

        /// <summary>
        /// Finds the latest email message in the target mailbox. This sample shows three different ways to access Internet
        /// message headers for an email message.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        static void AccessEmailInternetMessageHeaders(ExchangeService service)
        {
            // Defines the extended property that contains the entire set of Internet message headers.
            ExtendedPropertyDefinition PR_TRANSPORT_MESSAGE_HEADERS = new ExtendedPropertyDefinition(0x007D, MapiPropertyType.String);

            // Defines a property set that contains the entire set of Internet message headers.
            PropertySet propsAllInternetMessageHeaders = new PropertySet(BasePropertySet.IdOnly, PR_TRANSPORT_MESSAGE_HEADERS);

            // Defines a property set that contains the Internet message headers defined in EmailMessageSchema.InternetMessageHeaders.
            // The Internet message headers returned for this property set are a subset of what is returned for
            // PR_TRANSPORT_MESSAGE_HEADERS. The EmailMessageSchema.InternetMessageHeaders property has been deemphasized. 
            // Use either the schematized Internet message headers, or, if the headers aren't schematized, use
            // PR_TRANSPORT_MESSAGE_HEADERS.
            PropertySet propsSomeInternetMessageHeaders = new PropertySet(BasePropertySet.IdOnly, EmailMessageSchema.InternetMessageHeaders);

            // Defines a property set that contains the schematized Internet message headers. 
            PropertySet propsSchematizedInternetMessageHeaders = new PropertySet(BasePropertySet.IdOnly,
                                                                                 EmailMessageSchema.InternetMessageId, // Message-ID
                                                                                 EmailMessageSchema.ConversationIndex, // Thread-Index
                                                                                 EmailMessageSchema.ConversationTopic, // Thread-Topic
                                                                                 EmailMessageSchema.From, // From
                                                                                 EmailMessageSchema.ToRecipients, // To
                                                                                 EmailMessageSchema.CcRecipients, // Cc
                                                                                 EmailMessageSchema.ReplyTo, // Reply-To
                                                                                 EmailMessageSchema.InReplyTo, // In-Reply-To
                                                                                 EmailMessageSchema.References, // References
                                                                                 EmailMessageSchema.Subject, // Subject
                                                                                 EmailMessageSchema.DateTimeSent, // Date
                                                                                 EmailMessageSchema.Sender); // Sender

            // Return a single item.
            ItemView view = new ItemView(1);

            // Find the first email message in the Inbox. This results in a FindItem operation call to EWS.
            FindItemsResults<Item> results = service.FindItems(WellKnownFolderName.Inbox, view);

            EmailMessage msg;

            // Verify that the item is an email message.
            if (results.Items[0] is EmailMessage)
            {
                // Cast the item to an email message.
                msg = results.Items[0] as EmailMessage;

                // Load the schematized Internet message headers into the corresponding EmailMessage properties.
                // This results in a GetItem operation call to EWS.
                msg.Load(propsSchematizedInternetMessageHeaders);

                Console.WriteLine("\r\nSchematized internet message headers\r\n");
                Console.WriteLine("Message-ID: " + msg.InternetMessageId);
                Console.WriteLine("Thread-Index: " + Convert.ToBase64String(msg.ConversationIndex));
                Console.WriteLine("Thread-Topic: " + msg.ConversationTopic);
                Console.WriteLine("From: " + msg.From);
                foreach (EmailAddress address in msg.ToRecipients)
                {
                    Console.WriteLine("To: " + address.Address);
                }
                foreach (EmailAddress address in msg.CcRecipients)
                {
                    Console.WriteLine("cc: " + address.Address);
                }
                foreach (EmailAddress address in msg.ReplyTo)
                {
                    Console.WriteLine("Reply-To: " + address.Address);
                }
                Console.WriteLine("In-Reply-To: " + msg.InReplyTo);
                Console.WriteLine("References: " + msg.References);
                Console.WriteLine("Subject: " + msg.Subject);
                Console.WriteLine("Date: " + msg.DateTimeSent);
                Console.WriteLine("Sender: " + msg.Sender);

                // Load some of the Internet message headers into the EmailMessageSchema.InternetMessageHeaders
                // property. This results in a GetItem call to EWS.
                msg.Load(propsSomeInternetMessageHeaders);

                Console.WriteLine("\r\nSome internet message headers\r\n");
                foreach (InternetMessageHeader hdr in msg.InternetMessageHeaders)
                {
                    Console.WriteLine("{0}:\t\t{1}", hdr.Name, hdr.Value);
                }

                // Load all the Internet message headers into an extended property. This results in a GetItem operation call
                // to EWS.
                msg.Load(propsAllInternetMessageHeaders);

                Console.WriteLine("\r\nAll internet message headers\r\n");
                foreach (ExtendedProperty prop in msg.ExtendedProperties)
                {
                    Console.Write(prop.Value.ToString());
                }
            }
        }
    }
}
