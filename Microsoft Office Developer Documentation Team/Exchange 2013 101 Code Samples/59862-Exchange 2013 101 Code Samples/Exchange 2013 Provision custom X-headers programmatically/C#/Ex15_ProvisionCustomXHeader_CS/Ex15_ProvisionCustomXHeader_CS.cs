using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_ProvisionCustomXHeader_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            ProvisionCustomXHeader(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Provision a custom X-header in a mailbox database in Exchange 2007, or 
        /// for a single mailbox in versions of Exchange starting with Exchange 2010, including Exchange Online.
        /// </summary>
        /// <remarks>
        /// This creates a mapping between the named property and the X-header. The
        /// first time an unmapped X-header is received in a mailbox or database, 
        /// the mapping is created and the header is not saved. Subsequent messages
        /// that contain the X-header will have the X-header saved.
        /// </remarks>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void ProvisionCustomXHeader(ExchangeService service)
        { 
             // Create a definition for an extended property that will represent a custom X-Header.
            ExtendedPropertyDefinition xExperimentalHeader = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.InternetHeaders, 
                                                                                                   "X-Experimental", 
                                                                                                   MapiPropertyType.String);
            // Create an item that is used to provision the custom X-header.
            EmailMessage item = new EmailMessage(service);
            item.SetExtendedProperty(xExperimentalHeader, "Provision X-Experimental Internent message header");

            try
            {
                item.Save(WellKnownFolderName.MsgFolderRoot);

                if (service.ServerInfo.MajorVersion == 12)
                {
                    Console.WriteLine("Provisioned the X-Experimental across the mailbox database that hosts the user's mailbox.");
                }
                else // For all versions after Exchange 2007.
                {
                    Console.WriteLine("Provisioned the X-Experimental for the user's mailbox. You will need to run this " + 
                                      "for each mailbox that needs to process this X-Header.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }

        }

    }
}
