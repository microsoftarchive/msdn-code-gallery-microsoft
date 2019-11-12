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
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using MessageBox = System.Windows.MessageBox;

namespace StartConversation
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum CheckedRadio
        {
            GIF = 1,
            INK = 2,
            HTML = 3,
            RTF = 4,
            PlainText = 5
        }
        Microsoft.Lync.Model.LyncClient client = null;
        Microsoft.Lync.Model.Extensibility.Automation automation = null;
        string RemoteUserUri = "";
        private CheckedRadio _CheckedRadio;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                //Start the conversation
                automation = LyncClient.GetAutomation();
                client = LyncClient.GetClient();
                ConversationManager conversationManager = client.ConversationManager;
                conversationManager.ConversationAdded += new EventHandler<ConversationManagerEventArgs>(conversationManager_ConversationAdded);
            
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
            if (radioGif.IsChecked == true)
            {
                _CheckedRadio = CheckedRadio.GIF;
            }
            if (radioHtml.IsChecked == true)
            {
                _CheckedRadio = CheckedRadio.HTML;
            }
            if (radioRtf.IsChecked == true)
            {
                _CheckedRadio = CheckedRadio.RTF;
            }
            if (radioText.IsChecked == true)
            {
                _CheckedRadio = CheckedRadio.PlainText;
            }
            if (radioInk.IsChecked == true)
            {
                _CheckedRadio = CheckedRadio.INK;
            }
            Conversation conversation = client.ConversationManager.AddConversation();
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
                string messageContent = "Hello World";
                IDictionary<InstantMessageContentType, string> formattedMessage = this.GenerateFormattedMessage(messageContent);

                try
                {
                    if (imModality.CanInvoke(ModalityAction.SendInstantMessage))
                    {
                        IAsyncResult asyncResult = imModality.BeginSendMessage(
                            formattedMessage,
                            SendMessageCallback,
                            imModality);
                    }
                }
                catch(LyncClientException ex)
                {
                    txtErrors.Text = ex.Message;
                }
                
            }
        }

        private IDictionary<InstantMessageContentType, string> GenerateFormattedMessage(string message)
        {
            string formattedMessage = string.Empty;
            IDictionary<InstantMessageContentType, string> textMessage = new Dictionary<InstantMessageContentType, string>();

            switch (_CheckedRadio)
            {
                case CheckedRadio.GIF:
                    formattedMessage = "base64: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";
                    textMessage.Add(InstantMessageContentType.Gif, formattedMessage);
                    break;
                case CheckedRadio.HTML:
                    formattedMessage = "<FONT COLOR=\"#353333\" SIZE=20 FACE=\"Segoe UI\">" + message + "</FONT>";
                    textMessage.Add(InstantMessageContentType.Html, formattedMessage);
                    break;
                case CheckedRadio.INK:
                    formattedMessage = "base64:AK0CHAOAgAQdA+wBTgMESBFFZBkUMggAgBQCdoziQTMIAMAMAkrz4kEVq6rTQauq00HAquo+AABWPgpSf4L9Dfoeyzcpc2JUABEsoi2C5uWLmlzZUoALLLFlJUsqLFSxZZYsqLFAgvwZ+DQCWE2WWLLLBzzlLBZQFjcsWWVLKllioLFSyyywLAsWCwLFgAo/aoL92fu1UpKVm5bllSywAAEoiksqFhYssWXNlSxZZZZZUoCC/BX4LAAACVLLFlhYssqLKiypUoEoJRKlAAFgCl+rAYL+ABP4ADE3KFgzuUTZNllLKlBZYWCyVLGVKsFhYssWWVFS5pKlSyhKFlJUpLLKTYSpZUWWVLKAgvwF+AwACVFipQSkoAAAABKEoAEolIqWLLCyoqUllAlAAACUAA==";
                                      

                    textMessage.Add(InstantMessageContentType.Ink, formattedMessage);
                    break;
                case CheckedRadio.PlainText:
                    formattedMessage = message;
                    textMessage.Add(InstantMessageContentType.PlainText, formattedMessage);
                    break;
                case CheckedRadio.RTF:
                    System.Windows.Forms.RichTextBox richTextBox = new System.Windows.Forms.RichTextBox();
                    richTextBox.ForeColor = System.Drawing.Color.Red;
                    System.Drawing.Font font = new System.Drawing.Font(System.Drawing.FontFamily.GenericSerif, 25);
                    richTextBox.Font = font;
                    richTextBox.Text = message;
                    formattedMessage = richTextBox.Rtf.Replace(Environment.NewLine, "").Trim();
                    textMessage.Add(InstantMessageContentType.RichText, formattedMessage);
                    break;
            }

            return textMessage;
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

        private void radioGif_Click(object sender, RoutedEventArgs e)
        {
            btnStartConv.Content = "Start Conversation in GIF";
        }

        private void radioText_Click(object sender, RoutedEventArgs e)
        {
            btnStartConv.Content = "Start Conversation in plain text";

        }

        private void radioRtf_Click(object sender, RoutedEventArgs e)
        {
            btnStartConv.Content = "Start Conversation in RTF";

        }

        private void radioHtml_Click(object sender, RoutedEventArgs e)
        {
            btnStartConv.Content = "Start Conversation in HTML";

        }

        private void radioInK_Click(object sender, RoutedEventArgs e)
        {
            btnStartConv.Content = "Start Conversation in INK";

        }

    }
}
