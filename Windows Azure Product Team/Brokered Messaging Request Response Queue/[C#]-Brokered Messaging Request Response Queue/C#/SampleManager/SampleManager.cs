//---------------------------------------------------------------------------------
// Copyright (c) 2011, Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//---------------------------------------------------------------------------------

namespace Microsoft.ServiceBus.Samples.RequestResponse
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;


    public class SampleManager
    {
        #region Fields

        static string serviceBusConnectionString;

        static NamespaceManager namespaceClient;

        static List<Process> serverProcs = new List<Process>();
        static List<Process> clientProcs = new List<Process>();
        static int numClients = 4;
        static int numServers = 1;
        static int numMessages = 10;

        static bool displayVertical = true;

        static string requestQueueName = "RequestQueue";
        static string responseQueueName = "ResponseQueue";

        static ConsoleColor[] colors = new ConsoleColor[] { 
            ConsoleColor.Red, 
            ConsoleColor.Green, 
            ConsoleColor.Yellow, 
            ConsoleColor.Cyan,
            ConsoleColor.Magenta,
            ConsoleColor.Blue,             
            ConsoleColor.White};

        // constants for imported Win32 functions
        private static IntPtr HWND_TOP = new IntPtr(0);
        #endregion

        #region Imports
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        #endregion

        static void Main(string[] args)
        {
            // Setup:
            ParseArgs(args);
            GetUserCredentials();
            CreateNamespaceClient();

            // Create queues:
            Console.WriteLine("\nCreating Queues...");
            QueueDescription requestQueue = CreateQueue(false);
            Console.WriteLine("Created {0}, Queue.RequiresSession = false", requestQueue.Path);
            QueueDescription responseQueue = CreateQueue(true);
            Console.WriteLine("Created {0}, Queue.RequiresSession = true", responseQueue.Path);

            // Start clients and servers:
            Console.WriteLine("\nLaunching clients and servers...");
            StartClients();
            StartServers();

            Console.WriteLine("\nPress [Enter] to exit.");
            Console.ReadLine();

            // Cleanup:
            namespaceClient.DeleteQueue(requestQueue.Path);
            namespaceClient.DeleteQueue(responseQueue.Path);
            StopClients();
            StopServers();
        }

        #region HelperFunctions
        static void GetUserCredentials()
        {
            Console.Write("Please provide a connection string to Service Bus (/? for help):\n ");
            serviceBusConnectionString = Console.ReadLine();

            if ((String.Compare(serviceBusConnectionString, "/?") == 0) || (serviceBusConnectionString.Length == 0))
            {
                Console.Write("To connect to the Service Bus cloud service, go to the Windows Azure portal and select 'View Connection String'.\n");
                Console.Write("To connect to the Service Bus for Windows Server, use the get-sbClientConfiguration PowerShell cmdlet.\n\n");
                Console.Write("A Service Bus connection string has the following format: \nEndpoint=sb://<namespace>.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=<key>");

                serviceBusConnectionString = Console.ReadLine();
                Environment.Exit(0);
            }
        }

        // Create the management entities (queue)
        static void CreateNamespaceClient()
        {            
            namespaceClient =  NamespaceManager.CreateFromConnectionString(serviceBusConnectionString);
        }

        static QueueDescription CreateQueue(bool session)
        {
            string queueName = (session ? responseQueueName : requestQueueName);
            QueueDescription queueDescription = new QueueDescription(queueName) { RequiresSession = session };

            // Try deleting the queue before creation. Ignore exception if queue does not exist.
            try
            {
                namespaceClient.DeleteQueue(queueName);
            }
            catch (MessagingEntityNotFoundException)
            {
            }

            return namespaceClient.CreateQueue(queueDescription);
        }

        static void StartClients()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "RequestResponseSampleClient.exe";
            for (int i = 0; i < numClients; ++i)
            {
                startInfo.Arguments = CreateArgs(i.ToString());
                Process process = Process.Start(startInfo);
                clientProcs.Add(process);
            }
            Thread.Sleep(500);
            ArrangeWindows();
        }

        static void StopClients()
        {
            foreach (Process proc in clientProcs)
            {
                proc.CloseMainWindow();
            }
        }

        static void StartServers()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "RequestResponseSampleServer.exe";
            startInfo.Arguments = CreateArgs();
            for (int i = 0; i < numServers; ++i)
            {
                Process process = Process.Start(startInfo);
                serverProcs.Add(process);
            }
            Thread.Sleep(500);
            ArrangeWindows();
        }

        static void StopServers()
        {
            foreach (Process proc in serverProcs)
            {
                proc.CloseMainWindow();
            }
        }

        static string CreateArgs(string responseToSessionId = null)
        {
            string args = serviceBusConnectionString;
            if (responseToSessionId != null)
            {
                args += " " + responseToSessionId;
            }
            return args;
        }

        static void ArrangeWindows()
        {
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

            int maxHeight = screenHeight / 3;
            int maxWidth = screenWidth / 2;


            int senderWidth = screenWidth / (numClients + 1);
            int senderHeight = maxHeight;
            int managerWidth = senderWidth;
            int managerHeight = senderHeight;
            int receiverWidth = screenWidth / (numServers);
            int receiverHeight = screenHeight / 2;
            if (displayVertical)
            {
                senderWidth = screenWidth / 3;
                senderHeight = Math.Min(maxHeight, screenHeight / (numClients + 1));
                managerWidth = maxWidth;
                managerHeight = senderHeight;
                receiverWidth = screenWidth / 3;
                receiverHeight = Math.Min(maxHeight, screenHeight / (numServers));
            }

            Console.Title = "Manager";
            IntPtr mainHandle = Process.GetCurrentProcess().MainWindowHandle;
            SetWindowPos(mainHandle, HWND_TOP, 0, 0, managerWidth, managerHeight, 0);

            for (int i = 0; i < clientProcs.Count; ++i)
            {
                IntPtr handle = clientProcs[i].MainWindowHandle;
                if (displayVertical)
                {
                    SetWindowPos(handle, HWND_TOP, 0, senderHeight * (i + 1), senderWidth, senderHeight, 0);
                }
                else
                {
                    SetWindowPos(handle, HWND_TOP, senderWidth * (i + 1), 0, senderWidth, senderHeight, 0);
                }
            }

            for (int i = 0; i < serverProcs.Count; ++i)
            {
                IntPtr handle = serverProcs[i].MainWindowHandle;
                if (displayVertical)
                {
                    SetWindowPos(handle, HWND_TOP, screenWidth - receiverWidth, receiverHeight * i, receiverWidth, receiverHeight, 0);
                }
                else
                {
                    SetWindowPos(handle, HWND_TOP, receiverWidth * i, screenHeight / 2, receiverWidth, receiverHeight, 0);
                }
            }
        }

        static void ParseArgs(string[] args)
        {
            if (args.Length > 0)
            {
                Int32.TryParse(args[0], out numClients);
            }
            if (args.Length > 1)
            {
                Int32.TryParse(args[1], out numServers);
            }
            if (args.Length > 2)
            {
                Int32.TryParse(args[2], out numMessages);
            }
            if (args.Length > 3)
            {
                Boolean.TryParse(args[3], out displayVertical);
            }
        }
        #endregion

        #region PublicHelpers
        // Public helper functions and accessors

        public static String ResponseQueueName
        {
            get { return responseQueueName; }
            set { responseQueueName = value; }
        }

        public static String RequestQueueName
        {
            get { return requestQueueName; }
            set { requestQueueName = value; }
        }

        public static int NumMessages
        {
            get { return numMessages; }
            set { numMessages = value; }
        }

        public static void OutputMessageInfo(string action, BrokeredMessage message, string additionalText = "")
        {
            string id;
            if (message.SessionId == null)
            {
                id = message.ReplyToSessionId;
            }
            else
            {
                id = message.SessionId;
            }

            Console.ForegroundColor = colors[int.Parse(message.MessageId) % colors.Length];

            Console.WriteLine("{0}{1} - Client {2}. {3}", action, message.MessageId, id, additionalText);
            Console.ResetColor();
        }
        #endregion
    }
}
