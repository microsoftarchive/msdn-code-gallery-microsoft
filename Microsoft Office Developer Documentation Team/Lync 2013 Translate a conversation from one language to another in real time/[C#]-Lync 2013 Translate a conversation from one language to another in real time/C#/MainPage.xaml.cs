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
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using ConversationTranslator;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;


namespace Microsoft.Lync.SDK.Samples.ConversationTranslator
{

    /// <summary>
    /// Defines the possible UI states of the translator.
    /// </summary>
    public enum UIState 
    {
        Proof,  // user has not yet submitted the message for translation
        Send    // the translation has been received and is ready for being sent (user can make modifications)
    };

    /// <summary>
    /// Implements the UI behavior.
    /// </summary>
    public partial class MainPage : UserControl
    {
        // Service used to request translations
        private TranslationService translationService;

        //component that interacts with Lync
        private ConversationService conversationService;

        //keeps track of the current message being translated / sent
        private MessageContext messageContext;

        //the Lync conversation
        private Conversation conversation;

        //Current state of the UI
        private UIState state;

        //main view mode reference
        private MainViewModel viewModel;

        //controls the typing state
        private bool isComposing = false;

        public MainPage()
        {
            InitializeComponent();
            viewModel = App.MainViewModel;
        }

        /// <summary>
        /// Loads the necessary components and register for events.
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //gets the conversation this translator is associated with
                conversation = (Conversation)LyncClient.GetHostingConversation();

                //DEBUG: when running on the web browser, just get the first current
                //active conversation on OC
                if (conversation == null)
                {
                    //obtains the first active conversation in Lync
                    conversation = LyncClient.GetClient().ConversationManager.Conversations[0];

                    //cannot run without a conversation
                    if (this.conversation == null)
                    {
                        throw new NotSupportedException("You need one conversation to Debug the Conversation Translator");
                    }
                }

                //set the initial UI state to proof
                SetUIState(UIState.Proof);

                //creates the conversation service component and subscribes to events
                conversationService = new ConversationService(conversation);
                conversationService.MessageError += new MessageError(conversationService_MessageError);
                conversationService.MessageRecived += new MessageRecived(conversationService_MessageRecived);
                conversationService.MessageSent += new MessageSent(conversationService_MessageSent);

                //starts listening to Lync events
                conversationService.Start();


                //obtains the translation service component and subscribes to events
                translationService = new TranslationService();
                translationService.LanguagesReceived += new LanguagesReceived(translationService_LanguagesReceived);
                translationService.TranslationError += new TranslationError(translationService_TranslationError);
                translationService.TranslationReceived += new TranslationReceived(translationService_TranslationReceived);
                translationService.Start();
            }
            catch (AutomationServerException automationException)
            {
                if (automationException.Reason == AutomationServerException.FailureReason.ClientNotTrusted)
                {
                    ShowError("Please add the website from which the Conversation Translator is being downloaded to your Internet Explorer trusted site list.");
                }
                else if (automationException.Reason == AutomationServerException.FailureReason.ServerNotRunning)
                {
                    ShowError("Microsoft Lync does not appear to be running. If you opened Translator from Lync, there might be an issue with the Lync installation.");
                }
                else
                {
                    //The reason for the AutomationServerException in unknown, report a generic message
                    ShowError("There was an error when connecting to Microsoft Lync");
                }
            }
            catch (Exception)
            {
                //possibly an error in one of the services
                ShowError("There was an error when starting the Conversation Translator. Please try again later.");
            }
        }

        /// <summary>
        /// Button Proof/Send behavior.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonProofSend_Click(object sender, RoutedEventArgs e)
        {
            ProofOrSend();
        }

        /// <summary>
        /// Called when the ProofSend button is clicked or user presses Enter.
        /// </summary>
        /// <param name="message"></param>
        private void ProofOrSend()
        {
            //gets the message and clears the text box
            string message = textBoxMessage.Text;
            textBoxMessage.Text = string.Empty;

            //empty messages are not sent for translation
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            //depending on the UI state, the message will be sent for translation
            //or into the conversation
            if (state == UIState.Proof)
            {
                //sends the message for translation
                SendLocalMessageForTranslation(message);
            }
            else // Send state
            {
                //sends the message into the conversation
                DoSendMessage(message);

                //switches back to proof state
                SetUIState(UIState.Proof);
            }
        }

        /// <summary>
        /// Sends the message into the conversation.
        /// </summary>
        public void DoSendMessage(string translatedMessage)
        {
            try
            {
                //updates the translated message, in case the user corrected it
                messageContext.TranslatedMessage = translatedMessage;

                //sends the message into the conversation
                conversationService.SendMessage(messageContext);

                //notifies end of composing
                SetComposing(false);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }


        /// <summary>
        /// Translates the message assyncronously.
        /// </summary>
        /// <param name="message"></param>
        private void SendLocalMessageForTranslation(string message)
        {
            //assembles a MessageContext object
            messageContext = new MessageContext();
            messageContext.OriginalMessage = message;
            //reads the self participant's name
            try
            {
                messageContext.ParticipantName = 
                    conversation.SelfParticipant.Contact.GetContactInformation(ContactInformationType.DisplayName) as string;
            }
            catch (LyncClientException e)
            {
                ShowError(e);
                return;
            }
            catch (SystemException e)
            {
                if (LyncModelExceptionHelper.IsLyncException(e))
                {
                    // Log the exception thrown by the Lync Model API.
                    ShowError(e);
                    return;
                }
                else
                {
                    // Rethrow the SystemException which did not come from the Lync Model API.
                    throw;
                }
            }
            messageContext.SourceLanguage = viewModel.SourceLanguage.Code;
            messageContext.TargetLanguage = viewModel.TargetLanguage.Code;
            messageContext.Direction = MessageDirection.Outgoing;
            messageContext.MessageTime = DateTime.Now;

            //sends the message for translation
            translationService.Translate(messageContext);
        }

        /// <summary>
        /// Shows the translation received from the translation service.
        /// The visual representation depends on the message direction.
        /// </summary>
        /// <param name="context"></param>
        private void translationService_TranslationReceived(MessageContext context)
        {
            //incoming message: just show in the history
            if (context.Direction == MessageDirection.Incoming)
            {
                ShowIncomingTranslatedMessage(context);
            }
            else //outgoing message: move to the Send state
            {
                SetUIState(UIState.Send);
                ShowProofMessage(context.TranslatedMessage);
            }
        }

        /// <summary>
        /// Adds an incoming translated message to the message log.
        /// </summary>
        private void ShowIncomingTranslatedMessage(MessageContext context)
        {
            //writes a message log in the correct thread
            this.Dispatcher.BeginInvoke(() =>
            {
                WriteMessageToHistory(context.ParticipantName, context.MessageTime, context.TranslatedMessage);
            });
        }

        /// <summary>
        /// Writes a message to the in-memory conversation history.
        /// </summary>
        /// <param name="senderName">The name of the person who sent the message.</param>
        /// <param name="time">The time information.</param>
        /// <param name="message">The message itself.</param>
        private void WriteMessageToHistory(string senderName, DateTime time, string message)
        {
            //adds a line for the received or sent message
            MessageLine line = new MessageLine(senderName, time, message);
            viewModel.MessageHistory.Add(line);

            //manually scrows down to the last added message
            listBoxHistory.UpdateLayout();
            scrollViewerMessageLog.ScrollToVerticalOffset(listBoxHistory.ActualHeight);
        }

        /// <summary>
        /// Shows a translated outgoing message for proofing
        /// </summary>
        private void ShowProofMessage(string message)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                textBoxMessage.Text = message;
            });
        }

        /// <summary>
        /// Button cancel behavior
        /// </summary>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DoCancel();
        }

        /// <summary>
        /// When user is typing in the text area, watch for ESC or ENTER.
        /// </summary>
        private void textBoxMessage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProofOrSend();
            }
            else if (e.Key == Key.Escape)
            {
                DoCancel();
            }
            else
            {
                //sends the composing notification according to the text content
                SetComposing(string.IsNullOrWhiteSpace(textBoxMessage.Text) == false);
            }
        }

        /// <summary>
        /// Cancels the current translation.
        /// </summary>
        private void DoCancel()
        {
            //clears the text and the UI state.
            textBoxMessage.Text = string.Empty;
            SetUIState(UIState.Proof);
            textBoxMessage.Focus();

            //notifies end of composing
            SetComposing(false);
        }


        #region Conversation Service event handling (Lync-related events)

        /// <summary>
        /// Called when a message is effectivelly sent into the conversation.
        /// </summary>
        /// <param name="context"></param>
        private void conversationService_MessageSent(MessageContext context)
        {
            //posts the action into the UI thread
            this.Dispatcher.BeginInvoke(() =>
            {
                //writes the sent message into the in-memory history
                WriteMessageToHistory(context.ParticipantName, context.MessageTime, context.TranslatedMessage);
            });
        }


        /// <summary>
        /// Called when a message is received for the specified participant.
        /// </summary>
        private void conversationService_MessageRecived(string message, string participantName)
        {
            //posts the action into the UI thread
            this.Dispatcher.BeginInvoke(() =>
            {
                //creates a message context object to pass the message data across the layers
                MessageContext context = new MessageContext();
                context.MessageTime = DateTime.Now;
                context.ParticipantName = participantName;
                context.OriginalMessage = message;
                context.TargetLanguage = viewModel.SourceLanguage.Code;
                context.Direction = MessageDirection.Incoming;

                //otimization for two participants (avoids language detection)
                if (conversation.Participants.Count == 2)
                {
                    context.IsLanguageDetectionNeeded = false;
                    context.SourceLanguage = viewModel.TargetLanguage.Code;
                }

                //sends the message for translation
                translationService.Translate(context);
            });
        }

        private void conversationService_MessageError(Exception ex)
        {
            //this is an unexpected error
            ShowError(ex);
        }

        #endregion

        /// <summary>
        /// When the translation fails, print original message with an error prefix.
        /// </summary>
        private void translationService_TranslationError(Exception ex, MessageContext context)
        {
            WriteMessageToHistory(context.ParticipantName, context.MessageTime, "[Translation Failed] " + context.OriginalMessage);
        }

        /// <summary>
        /// Sets the UI state according to the value provided.
        /// </summary>
        private void SetUIState(UIState newState)
        {
            this.state = newState;
            this.Dispatcher.BeginInvoke(() =>
            {
                if (newState == UIState.Proof)
                {
                    buttonProofSend.Content = "Translate";
                    buttonCancel.IsEnabled = false;
                    buttonProofSend.Background = new SolidColorBrush(Colors.DarkGray);
                    buttonCancel.Background = new SolidColorBrush(Colors.DarkGray);
                    textBoxMessage.FontWeight = FontWeights.Normal;
                }
                else // Send state
                {
                    buttonProofSend.Content = "Send";
                    buttonCancel.IsEnabled = true;
                    buttonCancel.Background = new SolidColorBrush(Colors.Red);
                    buttonProofSend.Background = new SolidColorBrush(Colors.Green);
                    textBoxMessage.FontWeight = FontWeights.Bold;
                }
            });
        }

        /// <summary>
        /// Changes the IsTyping state and possibly send a notification to the other participants.
        /// </summary>
        private void SetComposing(bool isComposingNow)
        {
            //nothing to do if the value is the same
            if (this.isComposing == isComposingNow)
            {
                return;
            }

            //saves the new value
            isComposing = isComposingNow;
            
            //sends the notification to the conversation participants
            conversationService.SendComposingNotification(isComposing);            
        }

        /// <summary>
        /// Presents an error to the user.
        /// </summary>
        private void ShowError(string message)
        {
            textBoxMessage.Text = message;
        }

        /// <summary>
        /// Presents an exception to the user.
        /// </summary>
        private void ShowError(Exception ex)
        {
            ShowError(ex.Message + "\n" + ex.StackTrace);
        }

        /// <summary>
        /// Languages received from the translation service.
        /// </summary>
        /// <param name="languages">Set of supported languages.</param>
        private void translationService_LanguagesReceived(IList<Language> languages)
        {
            //need to swtich to the UI thread
            Dispatcher.BeginInvoke(() =>
            {
                foreach (Language language in languages)
                {
                    viewModel.Languages.Add(language);
                }

                //updates the combos, necessary given that the set of languages comes after initialization
                comboBoxMyLanguage.SelectedItem = viewModel.SourceLanguage;
                comboBoxTargetLanguage.SelectedItem = viewModel.TargetLanguage;
            });

        }
    }
}
