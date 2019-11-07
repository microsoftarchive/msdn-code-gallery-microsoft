using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Management.Automation;
using System.Security;
using System.Management.Automation.Runspaces;

using System.Net;

namespace Exchange101
{
    class Ex15_GetUserInformation_CS
    {
        static void Main(string[] args)
        {
            // Get the username and password for an administrative
            // user on the remote Exchange server.
            Console.Write("Enter user name: ");
            string userName = Console.ReadLine();

            SecureString securePassword = new SecureString();
            Console.Write("Enter password: ");

            while (true)
            {
                ConsoleKeyInfo userInput = Console.ReadKey(true);
                if (userInput.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (userInput.Key == ConsoleKey.Escape)
                {
                    return;
                }
                else if (userInput.Key == ConsoleKey.Backspace)
                {
                    if (securePassword.Length != 0)
                    {
                        securePassword.RemoveAt(securePassword.Length - 1);
                    }
                }
                else
                {
                    securePassword.AppendChar(userInput.KeyChar);
                    Console.Write("*");
                }
            }

            Console.WriteLine();

            securePassword.MakeReadOnly();

            // Make a PowerShell credential to connect to the remote server.
            PSCredential credential = new PSCredential(userName, securePassword);


            // Configure the connection to the remote server. Change the URI
            // to the name of your server installation.
            WSManConnectionInfo connectionInfo = new WSManConnectionInfo(
                new Uri("http://contoso.com/powershell"),
                "http://schemas.microsoft.com/powershell/Microsoft.Exchange",
                credential);


            // Set the authentication method.
            connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Kerberos;

            // Create a remote runspace on the server and run the Get-User cmdlet.
            using (Runspace runspace = RunspaceFactory.CreateRunspace(connectionInfo))
            {
                try
                {
                    runspace.Open();

                    Collection<PSObject> result = GetUserInformation(5, runspace);
                    DisplayUserInformationOnConsole(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine("Processing complete.");
            Console.ReadLine();
        }

        // This method displays the results returned from the GetUserInformationMethod
        // on the console. It iterates through the properties contained in each of the
        // PSObjects and displays the property name and value.
        private static void DisplayUserInformationOnConsole(Collection<PSObject> result)
        {
            foreach(PSObject powerShellObject in result)
            {
                foreach(PSPropertyInfo propertyInfo in powerShellObject.Properties)
                {
                    Console.WriteLine(string.Format("{0}: {1}", propertyInfo.Name, propertyInfo.Value));
                }

                Console.WriteLine();
                Console.WriteLine();
            }
        }

        // This method uses the specified runspace to run the PowerShell "Get-User" cmdlet
        // and returns the result as a collection of PSObject instances. The method will
        // return no more than count mailbox users. To simplify the code for this example,
        // the method does not filter or otherwise limit the mailbox users that are returned.
        private static Collection<PSObject> GetUserInformation(int count, Runspace runspace)
        {
            using (PowerShell powershell = PowerShell.Create())
            {
                powershell.AddCommand("Get-User");
                powershell.AddParameter("ResultSize", count);

                powershell.Runspace = runspace;

                return powershell.Invoke();
            }
        }
    }
}
