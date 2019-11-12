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
using System.Windows;

namespace Microsoft.Lync.SDK.Samples.ConversationTranslator
{
    /// <summary>
    /// Represents the data shown in one instance message line in the history panel.
    /// </summary>
    public class MessageLine
    {
        //background color of the message line
        private string myBackGround;

        //the first line with the name and time may be visible or not
        private Visibility myFirstLineVisibility;

        public MessageLine(string participantName, DateTime messageTime, string message)
        {
            myBackGround = GetMyBackGround(participantName);
            myFirstLineVisibility = GetMyFirstLineVisibility(participantName);
            this.ParticipantName = participantName;
            this.MessageTime = messageTime;
            this.Message = message;
        }

        /// <summary>
        /// Name of the participant who sent the message
        /// </summary>
        public string ParticipantName { get; set; }

        /// <summary>
        /// Original message time.
        /// </summary>
        public DateTime MessageTime { get; set; }

        /// <summary>
        /// Message content.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Returns the appropriate background for this instance.
        /// </summary>
        public string Background 
        {
            get 
            {
                return myBackGround;
            }
        }

        /// <summary>
        /// Returns the appropriate background for this instance.
        /// </summary>
        public Visibility FirstLineVisibility
        {
            get
            {
                return myFirstLineVisibility;
            }
        }

        #region Background check

        /// <summary>
        /// Background switching for highlighting messages
        /// </summary>
        private static string currentBackGround;
        private const string NormalBackGround = "#FFFFFFFF";
        private const string HighlightedBackGround = "#FFF5F7FA";

        /// <summary>
        /// Returns the current background, alternating between normal and highlighted.
        /// </summary>
        private static string GetMyBackGround(string participantName) 
        {
            //switchs the back ground for a different participant
            if (!string.Equals(lastParticipantName, participantName))
            {
                if (currentBackGround == NormalBackGround)
                {
                    currentBackGround = HighlightedBackGround;
                }
                else
                {
                    currentBackGround = NormalBackGround;
                }
            }


            return currentBackGround;
        }

        #endregion

        #region Same participant check

        //used to verify if the same participant is sending a new message
        private static string lastParticipantName;

        /// <summary>
        /// Determines if the participant for this message is the same as for the last message.
        /// </summary>
        private Visibility GetMyFirstLineVisibility(string participantName)
        {
            if (string.Equals(lastParticipantName, participantName))
            {
                //if it's the same, no need to the first history line
                return Visibility.Collapsed;
            }
            else
            {
                //show the history line and save new participant's name
                lastParticipantName = participantName;
                return Visibility.Visible;               
            }
        }

        #endregion
    }
}
