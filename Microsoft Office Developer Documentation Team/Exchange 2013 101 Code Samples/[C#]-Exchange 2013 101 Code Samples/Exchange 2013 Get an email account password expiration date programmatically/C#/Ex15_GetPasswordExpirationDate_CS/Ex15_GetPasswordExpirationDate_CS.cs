using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_GetPasswordExpirationDate_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            GetPasswordExpirationDate(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Gets a user's password expiration date. 
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        static void GetPasswordExpirationDate(ExchangeService service)
        {
            // Create a default value for the GetValueOrDefault method.
            DateTime defaultValue = DateTime.Now.AddDays(-1);
            
            try
            {
                // Request your password expiration date. This results in a GetPasswordExpirationDate operation call to EWS.
                DateTime pwdExpDate = service.GetPasswordExpirationDate("user@contoso.com").GetValueOrDefault(defaultValue);

                // If no password is returned.
                if (pwdExpDate.Equals(defaultValue))
                {
                    Console.WriteLine("Password has expired.");
                }
                else
                {
                    Console.WriteLine("Your password expires on: " + pwdExpDate.ToShortDateString());
                }
            }

            catch (ServiceResponseException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        static void GetUserInformation(ExchangeService service)
        {

        }
    }
}
