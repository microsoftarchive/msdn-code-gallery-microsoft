using System;
using System.Net;
using Microsoft.Exchange.WebServices.Data;
using System.Security;

namespace Exchange101
{
    // Define the interface contract for user data.
    interface Ex15_CTS_IUserData_CS
    {
        string EmailAddress { get; }
        string ImpersonatedEmailAddress { get; }
        bool UseDefaultCredentials { get; }
        ICredentials Credentials { get; }
        Ex15_CTS_IUserData_CS UserData { get; }
    }

    // Implement the interface contract for user data with an object
    // that gets the information from the console.
    class Ex15_CTS_UserDataFromConsole_CS : Ex15_CTS_IUserData_CS
    {
        public string EmailAddress { get; private set; }

        public string ImpersonatedEmailAddress { get; private set; }

        public bool UseDefaultCredentials { get; private set; }

        public ICredentials Credentials { get; private set; }

        public Ex15_CTS_IUserData_CS UserData { get; private set; }


        public static Ex15_CTS_IUserData_CS Get()
        {
            Ex15_CTS_UserDataFromConsole_CS userdata = new Ex15_CTS_UserDataFromConsole_CS();

            Console.Write("Use current user's credentials? (Y/N) ");
            string answer = Console.ReadLine();
            Console.WriteLine();

            userdata.UseDefaultCredentials = answer.ToUpperInvariant().StartsWith("Y");

            Console.Write("Email address? ");
            userdata.EmailAddress = Console.ReadLine();

            if (!userdata.UseDefaultCredentials)
            {
                userdata.Credentials = new NetworkCredential(userdata.EmailAddress, GetPassword());
            }

            Console.Write("Impersonate another email address? (ENTER for none) ");
            userdata.ImpersonatedEmailAddress = Console.ReadLine();

            return userdata;
        }

        private static SecureString GetPassword()
        {
            Console.Write("Password? ");
            SecureString password = new SecureString();

            while (true)
            {
                ConsoleKeyInfo userInput = Console.ReadKey(true);
                if (userInput.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (userInput.Key == ConsoleKey.Escape)
                {
                    password.Clear();
                    break;
                }
                else if (userInput.Key == ConsoleKey.Backspace)
                {
                    if (password.Length != 0)
                    {
                        password.RemoveAt(password.Length - 1);
                    }
                }
                else
                {
                    password.AppendChar(userInput.KeyChar);
                    Console.Write("*");
                }
            }

            password.MakeReadOnly();

            Console.WriteLine();

            return password;
        }
    }
}