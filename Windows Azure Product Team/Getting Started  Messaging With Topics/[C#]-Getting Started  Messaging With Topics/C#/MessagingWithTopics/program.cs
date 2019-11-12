//---------------------------------------------------------------------------------
// Microsoft (R)  Windows Azure SDK
// Software Development Kit
// 
// Copyright (c) Microsoft Corporation. All rights reserved.  
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
//---------------------------------------------------------------------------------

namespace Microsoft.Samples.MessagingWithTopics
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    public class program
    {
        private static TopicClient topicClient;
        private static string TopicName = "SampleTopic";

        static void Main(string[] args)
        {
            // Please see http://go.microsoft.com/fwlink/?LinkID=249089 for getting Service Bus connection string and adding to app.config

            Console.WriteLine("Creating Topic and Subscriptions");
            CreateTopic();
            Console.WriteLine("Press anykey to start sending messages ...");
            Console.ReadKey();
            SendMessages();
            Console.WriteLine("Press anykey to start receiving messages that you just sent ...");
            Console.ReadKey();
            ReceiveMessages();
            Console.WriteLine("\nEnd of scenario, press anykey to exit.");
            Console.ReadKey();      

        }


        private static void CreateTopic()
        {
            NamespaceManager namespaceManager = NamespaceManager.Create();

            Console.WriteLine("\nCreating Topic " + TopicName + "...");
            try
            {
                // Delete if exists
                if (namespaceManager.TopicExists(TopicName))
                {
                    namespaceManager.DeleteTopic(TopicName);
                }

                TopicDescription myTopic = namespaceManager.CreateTopic(TopicName);

                Console.WriteLine("Creating Subscriptions 'AuditSubscription' and 'AgentSubscription'...");
                SubscriptionDescription myAuditSubscription = namespaceManager.CreateSubscription(myTopic.Path, "AuditSubscription");
                SubscriptionDescription myAgentSubscription = namespaceManager.CreateSubscription(myTopic.Path, "AgentSubscription");
            }
            catch (MessagingException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private static void SendMessages()
        {
            topicClient = TopicClient.Create(TopicName);

            List<BrokeredMessage> messageList = new List<BrokeredMessage>();
            messageList.Add(CreateSampleMessage("1", "First message information"));
            messageList.Add(CreateSampleMessage("2", "Second message information"));
            messageList.Add(CreateSampleMessage("3", "Third message information"));

            Console.WriteLine("\nSending messages to topic...");
            

            foreach (BrokeredMessage message in messageList)
            {
                while (true)
                {
                    try
                    {
                        topicClient.Send(message);
                    }
                    catch (MessagingException e)
                    {
                        if (!e.IsTransient)
                        {
                            Console.WriteLine(e.Message);
                            throw;
                        }
                        else
                        {
                            HandleTransientErrors(e);
                        }
                    }
                    Console.WriteLine(string.Format("Message sent: Id = {0}, Body = {1}", message.MessageId, message.GetBody<string>()));
                    break;
                }
            }
            
            topicClient.Close();
        }

        private static void ReceiveMessages()
        {
            // For PeekLock mode (default) where applications require "at least once" delivery of messages 
            SubscriptionClient agentSubscriptionClient = SubscriptionClient.Create(TopicName, "AgentSubscription");
            BrokeredMessage message = null;
            while (true)
            {
                try
                {
                    //receive messages from Agent Subscription
                    message = agentSubscriptionClient.Receive(TimeSpan.FromSeconds(5));
                    if (message != null)
                    {
                        Console.WriteLine("\nReceiving message from AgentSubscription...");
                        Console.WriteLine(string.Format("Message received: Id = {0}, Body = {1}", message.MessageId, message.GetBody<string>()));
                        // Further custom message processing could go here...
                        message.Complete();
                    }
                    else
                    {
                        //no more messages in the subscription
                        break;
                    }
                }
                catch (MessagingException e)
                {
                    if (!e.IsTransient)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }
                    else
                    {
                        HandleTransientErrors(e);
                    }
                }
            }

            // For ReceiveAndDelete mode, where applications require "best effort" delivery of messages
            SubscriptionClient auditSubscriptionClient = SubscriptionClient.Create(TopicName, "AuditSubscription", ReceiveMode.ReceiveAndDelete);
            while (true)
            {
                try
                {
                    message = auditSubscriptionClient.Receive(TimeSpan.FromSeconds(5)); 
                    if (message != null)
                    {
                        Console.WriteLine("\nReceiving message from AuditSubscription...");
                        Console.WriteLine(string.Format("Message received: Id = {0}, Body = {1}", message.MessageId, message.GetBody<string>()));
                        // Further custom message processing could go here...
                     
                    }
                    else
                    {
                        //no more messages in the subscription
                        break;
                    }

                }
                catch (MessagingException e)
                {
                    if (!e.IsTransient)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }
                }
            }

            agentSubscriptionClient.Close();
            auditSubscriptionClient.Close();
        }

        private static BrokeredMessage CreateSampleMessage(string messageId, string messageBody)
        {
            BrokeredMessage message = new BrokeredMessage(messageBody);
            message.MessageId = messageId;
            return message;
        }

        private static void HandleTransientErrors(MessagingException e)
        {
            //If transient error/exception, let's back-off for 2 seconds and retry
            Console.WriteLine(e.Message);
            Console.WriteLine("Will retry sending the message in 2 seconds");
            Thread.Sleep(2000);
        }
    }
}
