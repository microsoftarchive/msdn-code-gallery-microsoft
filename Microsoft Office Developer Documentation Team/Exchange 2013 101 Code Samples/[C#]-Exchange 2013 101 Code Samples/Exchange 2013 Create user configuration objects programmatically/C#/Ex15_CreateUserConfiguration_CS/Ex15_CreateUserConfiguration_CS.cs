using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;
using System.IO;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_CreateUserConfiguration_CS
    {

        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());
       
       static void Main(string[] args)
        {
            CreateUserConfiguration(service, GetXmlByteArray(), GetBinaryByteArray());

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        #region Helpers
        // Creates a byte array of XML data
        static byte[] GetXmlByteArray()
        {
           String xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?><XmlElement>Data</XmlElement>";
           System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
           return encoding.GetBytes(xml);
        }

        // Creates a byte array of binary data
        static byte[] GetBinaryByteArray()
        {
           return File.ReadAllBytes(@"..\..\myImg.jpg");

        }
        #endregion

        static void CreateUserConfiguration(ExchangeService service,
                                     byte[] xmlData,
                                     byte[] binaryData)
        {
           // Create the user configuration object.
            UserConfiguration usrConfig = new UserConfiguration(service);

           // Add user configuration data to the XmlData and BinaryData properties.
            usrConfig.XmlData = xmlData;
            usrConfig.BinaryData = binaryData;

           // Name and save the user configuration object on the Inbox folder.
           // This results in a call to EWS.
            usrConfig.Save("MyCustomSettingName", WellKnownFolderName.Inbox);

            Console.WriteLine("Custom user configuration has been created.");
        }

    }
}
