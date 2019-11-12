using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15DeleteContactGroup
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            DeleteContactGroup(service);
        
            Console.WriteLine("\r\n");
            Console.WriteLine("Press any key...");
            Console.Read();
        }
        private static void DeleteContactGroup(ExchangeService service)
        {
            // Change the name "Brian Johnson" to the display name of the contact you're looking for.
            string DisplayName = "Brian Johnson";
            Contact contact = Ex15FindContactByDisplayName.FindContactByDisplayName(service, DisplayName);

            // If the contact doesn't exist, create it.
            if (contact == null)
            {
                Ex15CreateContact.CreateContact(service);
                contact = Contact.Bind(service, Ex15FindContactByDisplayName.FindContactByDisplayName(service, DisplayName).Id);
            }

            // Instantiate property definitions for the group to be deleted.
            ExtendedPropertyDefinition PidLidEmail1DisplayName = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.Address, 0x8080, MapiPropertyType.String);
            ExtendedPropertyDefinition PidLidEmail1AddressType = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.Address, 0x8082, MapiPropertyType.String);
            ExtendedPropertyDefinition PidLidEmail1Address = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.Address, 0x8083, MapiPropertyType.String);
            ExtendedPropertyDefinition PidLidEmail1OriginalDisplayName = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.Address, 0x8084, MapiPropertyType.String);
            ExtendedPropertyDefinition PidLidEmailOriginalEntryId = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.Address, 0x8085, MapiPropertyType.Binary);

            // Put your property definitions into an array so that you have a valid parameter to pass to your PropertySet constructor.
            ExtendedPropertyDefinition[] Email1PropertyGroup = new ExtendedPropertyDefinition[5]{ PidLidEmail1DisplayName, 
                                                                                                  PidLidEmail1AddressType, 
                                                                                                  PidLidEmail1Address, 
                                                                                                  PidLidEmail1OriginalDisplayName, 
                                                                                                  PidLidEmailOriginalEntryId};
            // Instantiate the property set you want to delete and bind to your contact object.
            PropertySet Email1PropertySet = new PropertySet(BasePropertySet.IdOnly, Email1PropertyGroup);
            contact = Contact.Bind(service, Ex15FindContactByDisplayName.FindContactByDisplayName(service, DisplayName).Id, Email1PropertySet);

            // Remove all the properties in the property group.
            contact.RemoveExtendedProperty(PidLidEmail1DisplayName);
            contact.RemoveExtendedProperty(PidLidEmail1AddressType);
            contact.RemoveExtendedProperty(PidLidEmail1Address);
            contact.RemoveExtendedProperty(PidLidEmail1OriginalDisplayName);
            contact.RemoveExtendedProperty(PidLidEmailOriginalEntryId);

            // Call the Update method on your contact object to send the request to the Exchange server.
            contact.Update(ConflictResolutionMode.AlwaysOverwrite);

            Console.WriteLine("Email Address deleted.");

        }
    }
}
