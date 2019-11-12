using System;
using System.Collections.Generic;
using System.Net;
using System.Security;

namespace Microsoft.Exchange.Samples.Autodiscover
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure 
    // that the code meets the coding requirements of your organization. 
    class Program
    {
        static void Main(string[] args)
        {
            // Parse the command line.
            CommandLineArgs arguments = new CommandLineArgs(args);

            if (!arguments.IsValid)
            {
                // Print help and exit.
                PrintUsage();
                return;
            }

            // Output to console is copied to AutodiscoverSample.log.
            Tracing.InitalizeLog(".\\AutodiscoverSample.log");

            Tracing.WriteLine("Performing {0} Autodiscover for {1}.",
                arguments.UseSoapAutodiscover ? "SOAP" : "POX",
                arguments.EmailAddress);

            if (!string.IsNullOrEmpty(arguments.AuthenticateAsUser))
            {
                Tracing.WriteLine("Authenticating as " + arguments.AuthenticateAsUser);
            }

            SecureString password = GetPasswordFromConsole();

            if (password.Length == 0)
                return;

            NetworkCredential userCredentials = new NetworkCredential(string.IsNullOrEmpty(arguments.AuthenticateAsUser) ? 
                arguments.EmailAddress : arguments.AuthenticateAsUser, password);

            // Create the request.
            AutodiscoverRequest autodiscoverRequest = new AutodiscoverRequest(arguments.EmailAddress,
                userCredentials);

            // Start the Autodiscover process.
            Dictionary<string, string> userSettings = autodiscoverRequest.DoAutodiscover(arguments.UseSoapAutodiscover);

            // If the process succeeded, print out the returned settings.
            if (userSettings != null)
            {
                Tracing.WriteLine("Settings:");
                foreach (KeyValuePair<string, string> setting in userSettings)
                {
                    Tracing.WriteLine("  {0}: {1}", setting.Key, setting.Value);
                }
            }
            else
            {
                Tracing.WriteLine("No settings returned.");
            }

            Tracing.WriteLine("Finished.");

            Tracing.FinalizeLog();

            Console.WriteLine("\nHit any key to exit...");
            Console.ReadKey();
        }

        // GetPasswordFromConsole
        //   This function prompts for a password and masks the user's
        //   input as the password is typed.
        //
        // Parameters:
        //   None.
        //
        // Returns:
        //   A SecureString object that contains the password entered by the user.
        //
        private static SecureString GetPasswordFromConsole()
        {
            SecureString password = new SecureString();
            bool readingPassword = true;

            Console.Write("Enter password: ");

            while (readingPassword)
            {
                ConsoleKeyInfo userInput = Console.ReadKey(true);

                switch (userInput.Key)
                {
                    case (ConsoleKey.Enter):
                        readingPassword = false;
                        break;
                    case (ConsoleKey.Escape):
                        password.Clear();
                        readingPassword = false;
                        break;
                    case (ConsoleKey.Backspace):
                        if (password.Length > 0)
                        {
                            password.RemoveAt(password.Length - 1);
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            Console.Write(" ");
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        }
                        break;
                    default:
                        if (userInput.KeyChar != 0)
                        {
                            password.AppendChar(userInput.KeyChar);
                            Console.Write("*");
                        }
                        break;
                }
            }
            Console.WriteLine();

            password.MakeReadOnly();
            return password;
        }

        // PrintUsage
        //   This function prints the help for the program.
        //
        // Parameters:
        //   None.
        //
        // Returns:
        //   None.
        //
        private static void PrintUsage()
        {
            Console.WriteLine("USAGE:");
            Console.WriteLine("  AutodiscoverSample.exe emailAddress [-skipSOAP] [-auth authEmailAddress]");
            Console.WriteLine("\nARGUMENTS:");
            Console.WriteLine("  emailAddress (required): Email address to send to Autodiscover");
            Console.WriteLine("  -skipSOAP (optional): If present, the application will not send a SOAP Autodiscover request. Instead, it will send an XML-based request (POX).");
            Console.WriteLine("  -auth authEmailAddress (optional): If present, the authEmailAddress will be used to authenticate the connection instead of emailAddress.");
            Console.WriteLine("\nHit any key to exit...");
            Console.ReadKey();
        }
    }
}
