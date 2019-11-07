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

//Lync namespaces
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;

namespace Microsoft.Lync.SDK.Samples.ConversationTranslator
{

    /// <summary>
    /// Called when a message is received from Lync.
    /// </summary>
    public delegate void MessageRecived(string message, string participantName);

    /// <summary>
    /// Called when there was an issue with the conversation.
    /// </summary>
    public delegate void MessageError(Exception ex);

    /// <summary>
    /// Called when a message is sent into the conversation.
    /// </summary>
    public delegate void MessageSent(MessageContext context);


    /// <summary>
    /// Registers for conversation and participants events and responds to those by 
    /// notifying the UI through the events. This is the main point for interactions
    /// with the Lync SDK.
    /// </summary>
    public class ConversationService
    {
        //the conversation the translator is associated with
        private Conversation conversation;

        //Self participant's IM modality for sending messages
        private InstantMessageModality myImModality;

        /// <summary>
        /// Occurs when a message is received from Lync.
        /// </summary>
        public event MessageRecived MessageRecived;

        /// <summary>
        /// Occurs when a message is sent into the conversation.
        /// </summary>
        public event MessageSent MessageSent;

        /// <summary>
        /// Occurs when there was an issue with the conversation.
        /// </summary>
        public event MessageError MessageError;

        /// <summary>
        /// Receives the conversation, callback to UI and the OC root object
        /// </summary>
        public ConversationService(Conversation conversation)
        {
            //stores the conversation object
            this.conversation = conversation;

            //gets the IM modality from the self participant in the conversation
            this.myImModality = (InstantMessageModality)conversation.SelfParticipant.Modalities[ModalityTypes.InstantMessage];            
        }

        /// <summary>
        /// Hooks into the conversation events so incoming messages are translated.
        /// </summary>
        public void Start()
        {
            //subscribes to new participants (in more people joins the conversation
            conversation.ParticipantAdded += new EventHandler<ParticipantCollectionChangedEventArgs>(conversation_ParticipantAdded);

            //registers for all existing remote participants messages
            foreach (Participant participant in conversation.Participants)
            {
                //skips the self participant (only remote messages are translated)
                //Note: Self participant also fires InstantMessageReceived
                //In this case, we're not interested in watching messages
                //sent by the self participant (actually reported as InstantMessageReceived)
                if (participant.IsSelf == false)
                {
                    SubcribeToParticipantMessages(participant);
                }
            }
        }

        /// <summary>
        /// Called by the Lync SDK when a new participant is added to the conversation.
        /// </summary>
        private void conversation_ParticipantAdded(object sender, ParticipantCollectionChangedEventArgs args)
        {
            //subscribes to messages sent only by a remote participant.
            if (args.Participant.IsSelf == false)
            {
                SubcribeToParticipantMessages(args.Participant);
            }
        }


        /// <summary>
        /// Register for InstantMessageReceived events for the specified participant.
        /// </summary>
        /// <param name="participant"></param>
        private void SubcribeToParticipantMessages(Participant participant)
        {
            //registers for IM received messages
            InstantMessageModality remoteImModality = (InstantMessageModality)participant.Modalities[ModalityTypes.InstantMessage];
            remoteImModality.InstantMessageReceived += new EventHandler<MessageSentEventArgs>(remoteImModality_InstantMessageReceived);
        }


        /// <summary>
        /// Called by the Lync SDK when a new message is received.
        /// </summary>
        private void remoteImModality_InstantMessageReceived(object sender, MessageSentEventArgs args)
        {
            try
            {
                //casts the modality
                InstantMessageModality modality = (InstantMessageModality)sender;

                //gets the participant name
                string name = (string)modality.Participant.Contact.GetContactInformation(ContactInformationType.DisplayName);

                //reads the message in its plain text format (automatically converted)
                string message = args.Text;

                //notifies the UI about the new message
                MessageRecived(message, name);
            }
            catch (Exception ex)
            {
                MessageError(ex);
            }
        }

        /// <summary>
        /// Sends a message into the conversation.
        /// </summary>
        public void SendMessage(MessageContext context)
        {
            //sends the message 
            myImModality.BeginSendMessage(context.TranslatedMessage, myImModality_OnMessageSent, context);
        }

        /// <summary>
        /// Called when a message is sent.
        /// </summary>
        public void myImModality_OnMessageSent(IAsyncResult result)
        {
            //gets context from the asyncronous context
            MessageContext context = (MessageContext)result.AsyncState;

            //notifies the UI that the message was actually sent
            MessageSent(context);
        }

        /// <summary>
        /// Sends a notification about the composing state.
        /// </summary>
        public void SendComposingNotification(bool isComposing)
        {
            //send the event 'as-is' without checking the success
            try
            {
                myImModality.BeginSetComposing(isComposing, null, null);
            }
            catch (LyncClientException e)
            {
                var eventHandler = MessageError;
                if (eventHandler != null)
                {
                    eventHandler(e);
                }
            }
            catch (SystemException e)
            {
                if (LyncModelExceptionHelper.IsLyncException(e))
                {
                    // Handle the exception thrown by the Lync Model API.
                    var eventHandler = MessageError;
                    if (eventHandler != null)
                    {
                        eventHandler(e);
                    }
                }
                else
                {
                    // Rethrow the SystemException which did not come from the Lync Model API.
                    throw;
                }
            }
        }
    }
}
