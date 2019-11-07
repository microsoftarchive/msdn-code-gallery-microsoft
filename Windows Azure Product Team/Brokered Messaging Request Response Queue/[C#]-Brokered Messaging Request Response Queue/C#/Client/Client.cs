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
        static string ReplyToSessionId;

        const double ResponseMessageTimeout = 20.0;
        #endregion

        static void Main(string[] args)
        {
            ParseArgs(args);
            Console.Title = "Client";

            // Send request messages to request queue
            QueueClient requestClient = CreateQueueClient(SampleManager.RequestQueueName);
            QueueClient responseClient = CreateQueueClient(SampleManager.ResponseQueueName);
            Console.WriteLine("Preparing to send request messages to {0}...", requestClient.Path);

            SendMessages(requestClient, responseClient);

            // All messages sent
            Console.WriteLine("\nClient complete.");
            requestClient.Close();
            responseClient.Close();
            Console.ReadLine();
        }

        static void SendMessages(QueueClient requestClient, QueueClient responseClient)
        {
            // Send request messages to queue:
            Console.WriteLine("Sending request messages to queue {0}", requestClient.Path);
            Console.WriteLine("Receiving response messages on queue {0}", responseClient.Path);

            MessageSession receiver = responseClient.AcceptMessageSession(ReplyToSessionId);
            for (int i = 0; i < SampleManager.NumMessages; ++i)
            {
                // send request message
                BrokeredMessage message = CreateRequestMessage(i);
                requestClient.Send(message);
                SampleManager.OutputMessageInfo("REQUEST: ", message);

                // receive response message
                BrokeredMessage receivedMessage = receiver.Receive(TimeSpan.FromSeconds(ResponseMessageTimeout));
                if (receivedMessage != null)
                {
                    SampleManager.OutputMessageInfo("RESPONSE: ", message);
                    receivedMessage.Complete();
                }
                else
                {
                    Console.WriteLine("ERROR: Response timed out.");
                }
            }
            receiver.Close();

            Console.WriteLine();
        }

        // Create the runtime entities (queue client)
        static QueueClient CreateQueueClient(string queueName)
        {
            return MessagingFactory.CreateFromConnectionString(ServiceBusConnectionString).CreateQueueClient(queueName);
        }

        static BrokeredMessage CreateRequestMessage(int i)
        {
            BrokeredMessage message = new BrokeredMessage();
            message.ReplyToSessionId = ReplyToSessionId;
            message.MessageId = i.ToString();
            return message;
        }

        static void ParseArgs(string[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("Incorrect number of arguments. args = {0}", args.ToString());
            }

            ServiceBusConnectionString = args[0];            
            ReplyToSessionId = args[1];
        }
    }
}
