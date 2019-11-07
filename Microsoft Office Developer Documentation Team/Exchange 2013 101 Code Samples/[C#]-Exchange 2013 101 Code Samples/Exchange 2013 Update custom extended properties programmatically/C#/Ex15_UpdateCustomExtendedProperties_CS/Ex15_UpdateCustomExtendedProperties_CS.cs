using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_UpdateCustomExtendedProperties_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            UpdateCustomExtendedProperties(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }
        /// <summary>
        /// Updates a custom  extended property on an email message. 
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void UpdateCustomExtendedProperties(ExchangeService service)
        {

            ItemView view = new ItemView(1);
            // Get the GUID for the property set.
            Guid MyPropertySetId = new Guid("{C11FF724-AA03-4555-9952-8FA248A11C3E}");

            // Create a definition for the extended property.
            ExtendedPropertyDefinition extendedPropertyDefinition = new ExtendedPropertyDefinition(MyPropertySetId, "Expiration Date", MapiPropertyType.String);
            view.PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, extendedPropertyDefinition);

            try
            {
                // Retrieve the first mail found in the Drafts folder with a subject 
                // value of 'Saved with custom ExtendedPropertyDefinition'. This results in a FindItem operation call to EWS.
                FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.Drafts, "Subject:'Saved with custom ExtendedPropertyDefinition'", view);

                // Instantiate an Item and an index counter for subsequent retrieval.
                Item message = null;
                int extendedPropertyindex = 0;
                
                // Search the email collection for the extended property.
                foreach (Item item in findResults.Items)
                {
                    if (item.ExtendedProperties.Count > 0)
                    {   
                        // Retrieve only the first item with the defined extended property.
                        // Identify the item, extended property index, and break.
                        foreach (ExtendedProperty extendedProperty in item.ExtendedProperties)
                        {
                            if (extendedProperty.PropertyDefinition == extendedPropertyDefinition)
                            {
                                message = item;
                                break;
                            }
                            extendedPropertyindex++;
                        }
                    }
                }
                
                // Update the value to one hour from now.
                message.ExtendedProperties[extendedPropertyindex].Value = DateTime.Now.AddHours(1).ToString();
                
                // Update the message with the new extended property value. This results in an UpdateItem operation call to EWS.
                message.Update(ConflictResolutionMode.AlwaysOverwrite);
                Console.WriteLine("Updated an extended property.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
