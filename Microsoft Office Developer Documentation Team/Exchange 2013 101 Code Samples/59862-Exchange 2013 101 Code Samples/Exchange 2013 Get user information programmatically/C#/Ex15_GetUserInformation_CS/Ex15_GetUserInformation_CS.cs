using System;
using System.Text;
using Microsoft.Exchange.WebServices.Autodiscover;
using System.Net;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_GetUserInformation_CS
    {


        static void Main(string[] args)
        {
            Console.Title = "EWS Test Console";
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WindowHeight = Console.LargestWindowHeight * 9 / 10;
            Console.WindowWidth = Console.LargestWindowWidth / 2;
            Console.SetWindowPosition(0, 0);
            Console.SetBufferSize(200, 3000);

            // Create an AutodiscoverService object to provide user settings.
            AutodiscoverService autodiscover = new AutodiscoverService();

            // Get the email address and password from the console.
            IUserData userData = UserDataFromConsole.GetUserData();

            // Create credentials for the Autodiscover service.
            autodiscover.Credentials = new NetworkCredential(userData.EmailAddress, userData.Password);

            // Create an array that contains all the UserSettingName enumeration values.
            // Your application should only request the settings that it needs.
            UserSettingName[] allSettings = (UserSettingName[])Enum.GetValues(typeof(UserSettingName));


            // Get all the user setting values for the email address.
            Console.Write("Doing autodiscover lookup for " + userData.EmailAddress + "...");

            GetUserSettingsResponse response = GetUserSettings(autodiscover, userData.EmailAddress, 10, allSettings);

            Console.WriteLine(" complete.");
            Console.WriteLine();

            // Write the user setting values to the console.
            foreach (UserSettingName settingKey in response.Settings.Keys)
            {
                Console.WriteLine(string.Format("{0}: {1}", settingKey, response.Settings[settingKey]));
            }

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.ReadLine();
        }

        public static GetUserSettingsResponse GetUserSettings(
            AutodiscoverService service,
            string emailAddress,
            int maxHops,
            params UserSettingName[] settings)
        {
            Uri url = null;
            GetUserSettingsResponse response = null;

            for (int attempt = 0; attempt < maxHops; attempt++)
            {
                service.Url = url;
                service.EnableScpLookup = (attempt < 2);

                response = service.GetUserSettings(emailAddress, settings);

                if (response.ErrorCode == AutodiscoverErrorCode.RedirectAddress)
                {
                    url = new Uri(response.RedirectTarget);
                }
                else if (response.ErrorCode == AutodiscoverErrorCode.RedirectUrl)
                {
                    url = new Uri(response.RedirectTarget);
                }
                else
                {
                    return response;
                }
            }

            throw new Exception("No suitable Autodiscover endpoint was found.");
        }
    }
}
