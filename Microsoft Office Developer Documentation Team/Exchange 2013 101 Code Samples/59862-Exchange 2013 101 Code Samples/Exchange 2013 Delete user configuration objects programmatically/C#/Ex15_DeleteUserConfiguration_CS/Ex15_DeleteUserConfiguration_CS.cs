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
    class Ex15_DeleteUserConfiguration_CS
    {

        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());
       
        static void Main(string[] args)
        {
            DeleteUserConfiguration(service);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        static void DeleteUserConfiguration(ExchangeService service)
        {
           // Binds to an user configuration object. Results in a call to EWS.
           UserConfiguration usrConfig = UserConfiguration.Bind(service,
                                                                "MyCustomSettingName",
                                                                WellKnownFolderName.Inbox,
                                                                UserConfigurationProperties.Id);
           // Deletes the user configuration object.
           // Results in a call to EWS.
           usrConfig.Delete();

           Console.WriteLine("The user configuration settings have been deleted.");
        }
    }
}
