using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{   
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Email
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());
        
        [STAThreadAttribute]
        static void Main(string[] args)
        {
           EmbedInlineImageInEmail(service);
           Console.WriteLine("Press or select Enter..."); 
           Console.Read();
        }

        /// <summary>
        /// Creates an email message and prompts the user to add an image to the email message. The image is added inline
        /// to the HTML body of the email message.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        static void EmbedInlineImageInEmail(ExchangeService service)
        {
            // Create the email message and set properties.
            EmailMessage email = new EmailMessage(service);
            email.ToRecipients.Add("user1@contoso.com");
            email.Subject = "SUBJECT: New email message with embedded image";

            // Create the HTML body of the email message. Note that the src attribute will be set
            // after an image has been selected.
            MessageBody htmlBody = new MessageBody();
            htmlBody.BodyType = BodyType.HTML;
            string htmlBodyTxt = @"<html><head></head><body><p>Here is where you will find an inline image.</p>
                                <img width=100 height=100 id=""1"" src=""cid:{0}"" alt=""image alt text"">
                                </body></html>";

           // Use OpenFileDialog to get the image.
           Stream myStream = null;
           string inputFileName = string.Empty;
           OpenFileDialog openFileDialog = new OpenFileDialog();
           openFileDialog.Filter = "*.jpg|*.jpg|*.bmp|*.bmp|All files (*.*)|*.*";
           openFileDialog.InitialDirectory = "C:";
           openFileDialog.Title = "Select a .jpg or .bmp image file";
           openFileDialog.FilterIndex = 3;

           if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog.OpenFile()) != null)
                    {
                        inputFileName = openFileDialog.SafeFileName;

                        using (myStream)
                        {
                            // Add the attachment to the email message. 
                            var fileAttach = email.Attachments.AddFileAttachment(inputFileName, myStream);
                            email.Attachments[0].IsInline = true;

                            // Format the HTML body with the file name and set the body content on the email message.
                            htmlBodyTxt = string.Format(htmlBodyTxt, inputFileName);
                            htmlBody.Text = htmlBodyTxt;
                            email.Body = htmlBody;

                            /* This results in two calls to EWS. The first is a CreateItem operation call, which creates the 
                             * email message in the Drafts folder and adds the attachment. The second is a SendItem call, which sends
                             * the email message and creates a copy in the default Sent Items folder.*/
                            email.SendAndSaveCopy();
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
}
