using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_CreateCustomExtendedProperties_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            CreateCustomExtendedProperties(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Creates a custom  extended property on an email message. This uses a custom property set identifier. 
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void CreateCustomExtendedProperties(ExchangeService service)
        {
            // Create the GUID for the property set.
            Guid MyPropertySetId = new Guid("{C11FF724-AA03-4555-9952-8FA248A11C3E}");

            // Create a definition for the extended property.
            ExtendedPropertyDefinition extendedPropertyDefinition = new ExtendedPropertyDefinition(MyPropertySetId, 
                                                                                                   "Expiration Date", 
                                                                                                   MapiPropertyType.String);

            // Create an email message that you will add the extended property to.
            EmailMessage message = new EmailMessage(service);
            message.Subject = "Saved with custom ExtendedPropertyDefinition.";
            message.Body = "The expiration date is contained within the extended property.";
            message.ToRecipients.Add("user@contoso.com");

            // Add the extended property to an email message object.
            message.SetExtendedProperty(extendedPropertyDefinition, DateTime.Now.AddDays(2).ToString());

            try
            {
                // Save the email message as a draft. This results in a CreateItem call to
                // EWS.
                message.Save();
                Console.WriteLine("Saved email with custom extended property.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
        }
    }
}
