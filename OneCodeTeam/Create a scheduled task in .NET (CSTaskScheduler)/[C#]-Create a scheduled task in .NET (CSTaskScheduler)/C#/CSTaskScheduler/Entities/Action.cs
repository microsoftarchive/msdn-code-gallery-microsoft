/****************************** Module Header ******************************\
 * Module Name: Action.cs
 * Project: CSTaskScheduler
 * Copyright (c) Microsoft Corporation. 
 * 
 * Represents an action.
 * 
 * This source is subject to the Microsoft Public License. 
 * See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL 
 * All other rights reserved. 
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 \****************************************************************************/

namespace CSTaskScheduler.Entities
{
    using System;

    /// <summary>
    /// Type of action to be performed
    /// </summary>
    [Serializable]
    public enum ActionType
    {
        /// <summary>
        /// Start a program\script
        /// </summary>
        StartProgram,

        /// <summary>
        /// Send an email
        /// </summary>
        SendEmail,

        /// <summary>
        /// Display a message on screen
        /// </summary>
        DisplayMessage
    }

    /// <summary>
    /// Represents an action
    /// </summary>
    [Serializable]
    public abstract class Action
    {
        #region Public Properties
        /// <summary>
        /// Gets a Type of an Action
        /// </summary>
        public abstract ActionType Type { get; }

        /// <summary>
        /// Gets or sets an Unique identifier of an action
        /// </summary>
        public virtual string Id { get; set; }

        #endregion
    }

    /// <summary>
    /// Represents an action to start a program or a script
    /// </summary>
    [Serializable]
    public sealed class StartProgramAction : Action
    {
        #region Fields
        
        /// <summary>
        /// Path to a program
        /// </summary>
        private string programPath;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the StartProgramAction class
        /// </summary>
        /// <param name="program">Program/script to be run as an action</param>
        public StartProgramAction(string program)
        {
            this.ProgramToRun = program;
        }

        /// <summary>
        /// Initializes a new instance of the StartProgramAction class
        /// </summary>
        /// <param name="program">Program/script to be run as an action</param>
        /// <param name="arguments">Optional Arguments to be passed to a program/script before executing</param>
        public StartProgramAction(string program, string arguments)
            : this(program)
        {
            this.Arguments = arguments;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the program along with path to run as an action
        /// </summary>
        public string ProgramToRun
        {
            get
            {
                return this.programPath;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(Constants.ProgramPathInActionNotSpecified);
                }

                this.programPath = value.Trim();
            }
        }

        /// <summary>
        /// Gets or sets the Optional arguments to be passed to a program/script before start
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// Gets a Type of an Action
        /// </summary>
        public override ActionType Type
        {
            get
            {
                return ActionType.StartProgram;
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents an action to send an email
    /// </summary>
    [Serializable]
    public sealed class SendEmailAction : Action
    {
        #region Fields
        
        /// <summary>
        /// Sender email address
        /// </summary>
        private string senderEmailAddress;

        /// <summary>
        /// Receiver email address
        /// </summary>
        private string receiverEmailAddress;

        /// <summary>
        /// SMTP server address
        /// </summary>
        private string smtpServerAddress;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the SendEmailAction class
        /// </summary>
        /// <param name="from">Sender email address</param>
        /// <param name="to">Receiver email address</param>
        /// <param name="smtpServer">Smtp Server</param>
        public SendEmailAction(string from, string to, string smtpServer)
        {
            this.From = from;
            this.To = to;
            this.SMTPServer = smtpServer;
        }

        /// <summary>
        /// Initializes a new instance of the SendEmailAction class
        /// </summary>
        /// <param name="from">Sender email address</param>
        /// <param name="to">Receiver email address</param>
        /// <param name="smtpServer">Smtp Server</param>
        /// <param name="subject">Subject of an email</param>
        /// <param name="contents">Body of an email</param>
        public SendEmailAction(string from, string to, string smtpServer, string subject, string contents)
            : this(from, to, smtpServer)
        {
            this.Subject = subject;

            this.MessageBody = contents;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the email address of a receiver
        /// </summary>
        public string From
        {
            get
            {
                return this.senderEmailAddress;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(Constants.SenderEmailAddressInActionNotSpecified);
                }

                this.senderEmailAddress = value.Trim();
            }
        }

        /// <summary>
        /// Gets or sets the email address of a sender
        /// </summary>
        public string To
        {
            get
            {
                return this.receiverEmailAddress;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(Constants.ReceiverEmailAddressInActionNotSpecified);
                }

                this.receiverEmailAddress = value.Trim();
            }
        }

        /// <summary>
        /// Gets or sets the SMTP server address
        /// </summary>
        public string SMTPServer
        {
            get
            {
                return this.smtpServerAddress;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(Constants.SMTPServerAddressInActionNotSpecified);
                }

                this.smtpServerAddress = value.Trim();
            }
        }

        /// <summary>
        /// Gets or sets the Subject of an email
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the Contents of an email 
        /// </summary>
        public string MessageBody { get; set; }

        /// <summary>
        /// Gets a Type of an Action
        /// </summary>
        public override ActionType Type
        {
            get
            {
                return ActionType.SendEmail;
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents an action to display a message
    /// </summary>
    [Serializable]
    public sealed class DisplayMessageAction : Action
    {
        #region Fields

        /// <summary>
        /// Message to be displayed
        /// </summary>
        private string message;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the DisplayMessageAction class
        /// </summary>
        /// <param name="message">Message to be displayed</param>
        public DisplayMessageAction(string message)
        {
            this.DisplayMessage = message;
        }

        /// <summary>
        /// Initializes a new instance of the DisplayMessageAction class
        /// </summary>
        /// <param name="message">Message to be displayed</param>
        /// <param name="title">Title of a message</param>
        public DisplayMessageAction(string message, string title)
            : this(message)
        {
            this.DisplayTitle = title;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the message to be displayed
        /// </summary>
        public string DisplayMessage
        {
            get
            {
                return this.message;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(Constants.MessageInActionNotSpecified);
                }

                this.message = value;
            }
        }

        /// <summary>
        /// Gets or sets the title of a message
        /// </summary>
        public string DisplayTitle { get; set; }

        /// <summary>
        /// Gets a Type of an Action
        /// </summary>
        public override ActionType Type
        {
            get
            {
                return ActionType.DisplayMessage;
            }
        }

        #endregion
    }
}
