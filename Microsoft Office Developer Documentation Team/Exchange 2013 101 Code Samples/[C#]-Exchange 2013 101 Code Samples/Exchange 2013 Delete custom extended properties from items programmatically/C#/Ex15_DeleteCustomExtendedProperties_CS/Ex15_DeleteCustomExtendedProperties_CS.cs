using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_DeleteCustomExtendedProperties_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            DeleteCustomExtendedProperties(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }
        /// <summary>
        /// Delete a custom extended property on an email message. 
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void DeleteCustomExtendedProperties(ExchangeService service)
        {
            ItemView view = new ItemView(10);
            // Get the GUID for the property set.
            Guid MyPropertySetId = new Guid("{C11FF724-AA03-4555-9952-8FA248A11C3E}");

            // Create a definition for the extended property.
            ExtendedPropertyDefinition extendedPropertyDefinition = new ExtendedPropertyDefinition(MyPropertySetId, 
                                                                                                   "Expiration Date", 
                                                                                                   MapiPropertyType.String);

            // Retrieve a collection of the first 10 mail items (as set by the ItemView) in the Inbox.
            view.PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, extendedPropertyDefinition);
            FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.Inbox, view);

            Item message = null;

            // Search the email collection for the extended property.
            foreach (Item item in findResults.Items)
            {
                if (item.ExtendedProperties.Count > 0)
                {   // Retrieve only the first item with the defined extended property.
                    // Identify the item, extended property index, and break.
                    foreach (ExtendedProperty extendedProperty in item.ExtendedProperties)
                    {
                        if (extendedProperty.PropertyDefinition == extendedPropertyDefinition)
                        {
                            message = item;
                            break;
                        }
                     }
                }
            }
            
            // Remove the extended property from the message.
            message.RemoveExtendedProperty(extendedPropertyDefinition);
            
            // Update the message on the server. This results in an UpdateItem operation call to EWS.
            message.Update(ConflictResolutionMode.AlwaysOverwrite);
        }
    }
}
