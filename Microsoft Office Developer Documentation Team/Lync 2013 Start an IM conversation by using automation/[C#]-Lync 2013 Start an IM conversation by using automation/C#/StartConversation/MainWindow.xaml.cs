/*=====================================================================
  This file is part of the Microsoft Unified Communications Code Samples.

  Copyright (C) 2012 Microsoft Corporation.  All rights reserved.

This source code is intended only as a supplement to Microsoft
Development Tools and/or on-line documentation.  See these other
materials for detailed information regarding Microsoft code samples.

THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.
=====================================================================*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Documents;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Lync.Model.Extensibility;
using MessageBox = System.Windows.MessageBox;

namespace StartConversation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Microsoft.Lync.Model.LyncClient client = null;
        Microsoft.Lync.Model.Extensibility.Automation automation = null;
        string RemoteUserUri = "";

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                //Start the conversation
                automation = LyncClient.GetAutomation();
                client = LyncClient.GetClient();
            }
            catch (LyncClientException lyncClientException)
            {
                MessageBox.Show("Failed to connect to Lync.");
                Console.Out.WriteLine(lyncClientException);
            }
            catch (SystemException systemException)
            {
                if (IsLyncException(systemException))
                {
                    // Log the exception thrown by the Lync Model API.
                    MessageBox.Show("Failed to connect to Lync.");
                    Console.WriteLine("Error: " + systemException);
                }
                else
                {
                    // Rethrow the SystemException which did not come from the Lync Model API.
                    throw;
                }
            }
        }

        /// <summary>
        /// Start the conversation with the specified participant using sip address
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartConv_Click(object sender, RoutedEventArgs e)
        {
            TextRange tr = new TextRange(rtbParticipants.Document.ContentStart, rtbParticipants.Document.ContentEnd);
            if (String.IsNullOrEmpty(tr.Text.Trim()))
            {
                txtErrors.Text = "No participants specified!";
                return;
            }
            String[] participants = tr.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            RemoteUserUri = participants[0];
            ConversationManager conversationManager = client.ConversationManager;
            conversationManager.ConversationAdded += new EventHandler<ConversationManagerEventArgs>(conversationManager_ConversationAdded);
            Conversation conversation = conversationManager.AddConversation();
        }

        /*
        /// <summary>
        /// Start the conversation with the specified participant using sip address
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartConv_Click(object sender, RoutedEventArgs e)
        {
            TextRange tr = new TextRange(rtbParticipants.Document.ContentStart, rtbParticipants.Document.ContentEnd);
            if (String.IsNullOrEmpty(tr.Text.Trim()))
            {
                txtErrors.Text = "No participants specified!";
                return;
            }
            String[] participants = tr.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            KeyValuePair<AutomationModalitySettings, object>[] contextData = new KeyValuePair<AutomationModalitySettings, object>[]
                {
                    new KeyValuePair<AutomationModalitySettings, object>(AutomationModalitySettings.SendFirstInstantMessageImmediately, true),
                    new KeyValuePair<AutomationModalitySettings, object>(AutomationModalitySettings.FirstInstantMessage, "Hello World"),
                    new KeyValuePair<AutomationModalitySettings, object>(AutomationModalitySettings.Subject, "Welcome To Lync Conversation Window"),
                };

            IAsyncResult ar = automation.BeginStartConversation(AutomationModalities.InstantMessage, participants, contextData, null, null);
            automation.EndStartConversation(ar);
        }
        */


        /// <summary>
        /// Async callback method invoked by InstantMessageModality instance when SendMessage completes
        /// </summary>
        /// <param name="_asyncOperation">IAsyncResult The operation result</param>
        /// 
        private void SendMessageCallback(IAsyncResult ar)
        {
            InstantMessageModality imModality = (InstantMessageModality)ar.AsyncState;

            try
            {
                imModality.EndSendMessage(ar);
            }
            catch (LyncClientException lce)
            {
                MessageBox.Show("Lync Client Exception on EndSendMessage " + lce.Message);
            }

        }


        void conversationManager_ConversationAdded(object sender, ConversationManagerEventArgs e)
        {
            e.Conversation.ParticipantAdded += new EventHandler<ParticipantCollectionChangedEventArgs>(Conversation_ParticipantAdded);
            e.Conversation.AddParticipant(client.ContactManager.GetContactByUri(RemoteUserUri));
        }

        void Conversation_ParticipantAdded(object sender, ParticipantCollectionChangedEventArgs e)
        {
            // add event handlers for modalities of participants other than self participant:
            if (e.Participant.IsSelf == false)
            {
                if (((Conversation)sender).Modalities.ContainsKey(ModalityTypes.InstantMessage))
                {
                    ((InstantMessageModality)e.Participant.Modalities[ModalityTypes.InstantMessage]).InstantMessageReceived += new EventHandler<MessageSentEventArgs>(ConversationTest_InstantMessageReceived);
                    ((InstantMessageModality)e.Participant.Modalities[ModalityTypes.InstantMessage]).IsTypingChanged += new EventHandler<IsTypingChangedEventArgs>(ConversationTest_IsTypingChanged);
                }
                Conversation conversation = (Conversation)sender;

                InstantMessageModality imModality = (InstantMessageModality)conversation.Modalities[ModalityTypes.InstantMessage];

                IDictionary<InstantMessageContentType, string> textMessage = new Dictionary<InstantMessageContentType, string>();
                textMessage.Add(InstantMessageContentType.PlainText, "Hello World");

                if (imModality.CanInvoke(ModalityAction.SendInstantMessage))
                {
                    IAsyncResult asyncResult = imModality.BeginSendMessage(
                        textMessage,
                        SendMessageCallback,
                        imModality);
                }
            }
        }

        void ConversationTest_IsTypingChanged(object sender, IsTypingChangedEventArgs e)
        {

        }

        void ConversationTest_InstantMessageReceived(object sender, MessageSentEventArgs e)
        {

        }

        /// <summary>
        /// Identify if a particular SystemException is one of the exceptions which may be thrown
        /// by the Lync Model API.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private bool IsLyncException(SystemException ex)
        {
            return
                ex is NotImplementedException ||
                ex is ArgumentException ||
                ex is NullReferenceException ||
                ex is NotSupportedException ||
                ex is ArgumentOutOfRangeException ||
                ex is IndexOutOfRangeException ||
                ex is InvalidOperationException ||
                ex is TypeLoadException ||
                ex is TypeInitializationException ||
                ex is InvalidComObjectException ||
                ex is InvalidCastException;
        }

    }
}
