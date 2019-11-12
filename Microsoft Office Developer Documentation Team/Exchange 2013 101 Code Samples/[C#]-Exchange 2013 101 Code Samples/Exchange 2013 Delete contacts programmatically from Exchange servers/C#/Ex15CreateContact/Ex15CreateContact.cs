using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    public class Ex15CreateContact
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            CreateContact(service);
        
            Console.WriteLine("\r\n");
            Console.WriteLine("Press any key...");
            Console.Read();
        }

        public static void CreateContact(ExchangeService service)
        {
            Contact contact = new Contact(service);

            // Specify the name and how the contact should be filed.
            contact.GivenName = "Brian";
            contact.MiddleName = "David";
            contact.Surname = "Johnson";
            contact.FileAsMapping = FileAsMapping.SurnameCommaGivenName;
            contact.DisplayName = "Brian Johnson";

            // Specify the company name.
            contact.CompanyName = "Contoso";

            // Specify the business, home, and car phone numbers.
            contact.PhoneNumbers[PhoneNumberKey.BusinessPhone] = "425-555-0110";
            contact.PhoneNumbers[PhoneNumberKey.HomePhone] = "425-555-0120";
            contact.PhoneNumbers[PhoneNumberKey.CarPhone] = "425-555-0130";

            // Specify two e-mail addresses.
            contact.EmailAddresses[EmailAddressKey.EmailAddress1] = new EmailAddress("brian_1@contoso.com");
            contact.EmailAddresses[EmailAddressKey.EmailAddress2] = new EmailAddress("brian_2@contoso.com");

            // Specify two IM addresses.
            contact.ImAddresses[ImAddressKey.ImAddress1] = "brianIM1@contoso.com";
            contact.ImAddresses[ImAddressKey.ImAddress2] = "brianIM2@contoso.com";

            // Specify the home address.
            PhysicalAddressEntry paEntry1 = new PhysicalAddressEntry();
            paEntry1.Street = "123 Main Street";
            paEntry1.City = "Seattle";
            paEntry1.State = "WA";
            paEntry1.PostalCode = "11111";
            paEntry1.CountryOrRegion = "United States";
            contact.PhysicalAddresses[PhysicalAddressKey.Home] = paEntry1;

            // Specify the business address.
            PhysicalAddressEntry paEntry2 = new PhysicalAddressEntry();
            paEntry2.Street = "456 Corp Avenue";
            paEntry2.City = "Seattle";
            paEntry2.State = "WA";
            paEntry2.PostalCode = "11111";
            paEntry2.CountryOrRegion = "United States";
            contact.PhysicalAddresses[PhysicalAddressKey.Business] = paEntry2;

            contact.Save();
            Console.WriteLine("Contact created.");
        }
    }
}
