using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_CreateAttachments_CS
    {
        static IUserData UserData = UserDataFromConsole.GetUserData();
        static ExchangeService service = Service.ConnectToService(UserData, new TraceListener());

        [STAThread]
        static void Main(string[] args)
        {
            CreateAttachments(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }
        /// <summary>
        /// Demonstrates three ways to create file attachments and two different ways to create copies of an
        /// item and add it to an email message.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void CreateAttachments(ExchangeService service)
        {
            // Create an email message.
            EmailMessage email = new EmailMessage(service);
            email.Subject = "Message with Attachments";
            email.Body = "This message contains three file attachments and two item attachments.";
            email.ToRecipients.Add("user@contoso.com");

            // This must be set on the item attachments so that they can be consumed in Outlook.
            ExtendedPropertyDefinition PR_MESSAGE_FLAGS_msgflag_read = new ExtendedPropertyDefinition(3591, MapiPropertyType.Integer);

            // Use OpenFileDialog to get the file that will be used to create file attachments.
            string filename;
            string fullfilename;
            Stream myStream = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.InitialDirectory = "C:";
            openFileDialog.Title = "Select an attachment";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog.OpenFile()) != null)
                    {
                        filename = openFileDialog.SafeFileName;
                        fullfilename = openFileDialog.FileName;

                        using (myStream)
                        {
                            #region Add file attachments
                            // Add the file attachment to the email message by using the file name and stream. 
                            var fileAttach = email.Attachments.AddFileAttachment(filename, myStream);
                            
                            // Add the file attachment to the email message by using the full file name. 
                            var fileAttach2 = email.Attachments.AddFileAttachment(fullfilename);

                            // Add the file attachment to the email message by using the full file name and the file name.
                            var fileAttach3 = email.Attachments.AddFileAttachment(filename, fullfilename);

                            // A fourth option is to add a file attachment by using the the file
                            // name and byte array for the file.
                            #endregion

                            #region Add item attachments.
                            // Return a single item.
                            ItemView view = new ItemView(1);

                            // Find the first email message in the Inbox. You will "attach" this email message to the email message you are drafting.
                            // This results in a FindItem operation call to EWS.
                            FindItemsResults<Item> results = service.FindItems(WellKnownFolderName.Inbox, view);

                            // Verify that the item is an email message.
                            if (results.Items[0] is EmailMessage)
                            {
                                EmailMessage msg = results.Items[0] as EmailMessage;
                                System.Collections.ObjectModel.Collection<PropertyDefinitionBase> currentProps = msg.GetLoadedPropertyDefinitions();
                                currentProps.Add(EmailMessageSchema.Body);
                                currentProps.Add(EmailMessageSchema.ToRecipients);
                                currentProps.Add(EmailMessageSchema.CcRecipients);

                                msg.Load(new PropertySet(currentProps));

                                // Attach the email message found from search to the new email message. In reality, you can't attach
                                // existing items, you have to make a copy of the item property by property. The Item type does not
                                // implement ICloneable.
                                ItemAttachment<EmailMessage> itemAttachment = email.Attachments.AddItemAttachment<EmailMessage>();
                                itemAttachment.Name = "Item attachment based on copied property values";
                                itemAttachment.Item.Subject = msg.Subject;
                                itemAttachment.Item.Body = msg.Body;
                                foreach (EmailAddress address in msg.ToRecipients)
                                {
                                    itemAttachment.Item.ToRecipients.Add(address);
                                }
                                foreach (EmailAddress address in msg.CcRecipients)
                                {
                                    itemAttachment.Item.ToRecipients.Add(address);
                                }
                                // The Sent flag should be set.
                                itemAttachment.Item.SetExtendedProperty(PR_MESSAGE_FLAGS_msgflag_read, 1);

                                // Unfortunately, not all properties are settable, like DateTimeReceived. If the item is an email message,
                                // you can use the MIME content, which provides a decent copy of the email message without any of the 
                                // Exchange store properties. You can add store properties to the item as well, as long 
                                // as they are writeable.
                                msg.Load(new PropertySet(EmailMessageSchema.MimeContent));
                                ItemAttachment<EmailMessage> itemAttachment2 = email.Attachments.AddItemAttachment<EmailMessage>();
                                itemAttachment2.Name = "Item attachment based on copied MIME content";
                                itemAttachment2.Item.MimeContent = msg.MimeContent;
                                // The Sent flag should be set.
                                itemAttachment2.Item.SetExtendedProperty(PR_MESSAGE_FLAGS_msgflag_read, 1);
                            }
                            #endregion

                            /* This results in two calls to EWS. The first call is the CreateItem operation, which creates the 
                            * email message in the Drafts folder and adds the attachment. The second call is a SendItem call, which sends
                            * the email message and creates a copy in the default Sent Items folder.*/
                            email.SendAndSaveCopy();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }
    }
}
