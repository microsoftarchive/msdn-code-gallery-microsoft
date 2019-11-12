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
    using Microsoft.ServiceBus.Messaging;

    class Program
    {
        #region Fields
        static string ServiceBusConnectionString;        

        const double ReceiveMessageTimeout = 20.0;
        #endregion

        static void Main(string[] args)
        {
            ParseArgs(args);

            // Receive request messages from request queue
            Console.Title = "Server";
            QueueClient requestClient = CreateQueueClient(SampleManager.RequestQueueName);
            QueueClient responseClient = CreateQueueClient(SampleManager.ResponseQueueName);
            Console.WriteLine("Ready to receive messages from {0}...", requestClient.Path);

            ReceiveMessages(requestClient, responseClient);

            Console.WriteLine("\nServer complete.");
            requestClient.Close();
            responseClient.Close();
            Console.ReadLine();
        }

        static QueueClient CreateQueueClient(string queueName)
        {
            return MessagingFactory.CreateFromConnectionString(ServiceBusConnectionString).CreateQueueClient(queueName);
        }

        static void ReceiveMessages(QueueClient requestClient, QueueClient responseClient)
        {
            // Read messages from queue until queue is empty:
            Console.WriteLine("Reading messages from queue {0}", requestClient.Path);

            BrokeredMessage request = requestClient.Receive(TimeSpan.FromSeconds(ReceiveMessageTimeout));
            while (request != null)
            {
                SampleManager.OutputMessageInfo("REQUEST: ", request);

                BrokeredMessage response = new BrokeredMessage();
                response.SessionId = request.ReplyToSessionId;
                response.MessageId = request.MessageId;
                responseClient.Send(response);
                request = requestClient.Receive(TimeSpan.FromSeconds(ReceiveMessageTimeout));
            }
        }

        static void ParseArgs(string[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("Incorrect number of arguments. args = {0}", args.ToString());
            }

            ServiceBusConnectionString = args[0];
            
        }
    }
}
