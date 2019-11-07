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
    class Ex15_UpdateUserConfiguration_CS
    {

        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());
       
        static void Main(string[] args)
        {
            UpdateUserConfiguration(service);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        static void UpdateUserConfiguration(ExchangeService service)
        {
           // Binds to an user configuration object. Results in a call to EWS.
           UserConfiguration usrConfig = UserConfiguration.Bind(service,
                                                                "MyCustomSettingName",
                                                                WellKnownFolderName.Inbox,
                                                                UserConfigurationProperties.Id);

           // Add dictionary property values to the local copy of the object.
           usrConfig.Dictionary.Add("Key1", 1);
           usrConfig.Dictionary.Add("Key2", 3);
           usrConfig.Dictionary.Add("Key3", 9);

           // Updates the local changes to the user configuration object and uploads the
           // changes to the server. Results in a call to EWS.
           usrConfig.Update();

           Console.WriteLine("User configuration has been updated to include dictionary property values");
        }
    }
}
