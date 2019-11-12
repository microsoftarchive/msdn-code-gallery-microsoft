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
    class Ex15_GetUserConfiguration_CS
    {

       static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());
       
       static void Main(string[] args)
        {
            GetUserConfiguration(service);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        static void GetUserConfiguration(ExchangeService service)
        {
           // Binds to an user configuration object. Results in a call to EWS.
           UserConfiguration usrConfig = UserConfiguration.Bind(service,
                                                                "MyCustomSettingName",
                                                                WellKnownFolderName.Inbox,
                                                                UserConfigurationProperties.All);
           // Display the returned property values.
           Console.WriteLine("User Config Identifier: " + usrConfig.ItemId.UniqueId);
           Console.WriteLine("XmlData: " + Encoding.UTF8.GetString(usrConfig.XmlData));
           Console.WriteLine("BinaryData: " + Encoding.UTF8.GetString(usrConfig.BinaryData));
        }
    }
}
