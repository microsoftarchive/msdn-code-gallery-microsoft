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

namespace Microsoft.Lync.SDK.Samples.ConversationTranslator
{

    /// <summary>
    /// Describes the direction of the instant message.
    /// </summary>
    public enum MessageDirection { Incoming, Outgoing };

    /// <summary>
    /// Instant message helper model for exchanging information between 
    /// the different application layers.
    /// </summary>
    public class MessageContext
    {
        /// <summary>
        /// Direction of the message.
        /// </summary>
        public MessageDirection Direction { get; set; }

        /// <summary>
        /// Time when the message was sent / received.
        /// </summary>
        public DateTime MessageTime { get; set; }

        /// <summary>
        /// Sender of the message.
        /// </summary>
        public string ParticipantName { get; set; }

        /// <summary>
        /// Original language of the message.
        /// </summary>
        public string SourceLanguage { get; set; }

        /// <summary>
        /// Language the message will be translated to.
        /// </summary>
        public string TargetLanguage { get; set; }

        /// <summary>
        /// Specifies whether the source language is known.
        /// </summary>
        public bool IsLanguageDetectionNeeded { get; set; }

        /// <summary>
        /// Original message content.
        /// </summary>
        public string OriginalMessage { get; set; }

        /// <summary>
        /// Message translated to the target language.
        /// </summary>
        public string TranslatedMessage { get; set; }       
    }
}
