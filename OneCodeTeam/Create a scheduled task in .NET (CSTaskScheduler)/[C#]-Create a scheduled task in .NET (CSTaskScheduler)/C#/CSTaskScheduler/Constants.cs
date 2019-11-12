/****************************** Module Header ******************************\
 * Module Name: Constants.cs
 * Project: CSTaskScheduler
 * Copyright (c) Microsoft Corporation. 
 * 
 * Constants used in the application.
 * 
 * This source is subject to the Microsoft Public License. 
 * See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL 
 * All other rights reserved. 
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\****************************************************************************/

namespace CSTaskScheduler
{
    using System;

    /// <summary>
    /// Constants used in the application
    /// </summary>
    public class Constants
    {
        #region Error Messages

        /// <summary>
        /// Error message for a task service connection failure
        /// </summary>
        public const string TaskServiceConnectionError = "Unable to connect the scheduled task service";

        /// <summary>
        /// Error message when tasks could not be fetched
        /// </summary>
        public const string TasksFetchError = "Unable to retrieve the scheduled tasks";

        /// <summary>
        /// Error message when a task can't be deleted
        /// </summary>
        public const string TaskDeleteError = "Unable to delete the scheduled task: ";

        /// <summary>
        /// Error message when a task can't be created
        /// </summary>
        public const string TaskCreateError = "Unable to create or update a scheduled task";

        /// <summary>
        /// Error message for invalid task path
        /// </summary>
        public const string InvalidTaskPathError = "Task Path is invalid";

        /// <summary>
        /// Error message for a task having illegal characters in it's name
        /// </summary>
        public const string InvalidTaskNameError = "Task Name is invalid. It contains invalid characters : '\' ";

        /// <summary>
        /// Error message when Execute Program action is configured in a new task to be created
        /// </summary>
        public const string ProgramPathInActionNotSpecified = "Program/script to run as an action is not specified.";
        
        /// <summary>
        /// Error message when sender email address is not specified
        /// </summary>
        public const string SenderEmailAddressInActionNotSpecified = "Email address of a sender is not specified.";

        /// <summary>
        /// Error message when receiver email address is not specified
        /// </summary>
        public const string ReceiverEmailAddressInActionNotSpecified = "Email address of a receiver is not specified.";

        /// <summary>
        /// Error message when SMTP server address is not specified
        /// </summary>
        public const string SMTPServerAddressInActionNotSpecified = "SMTP Server is not specified.";

        /// <summary>
        /// Error message when message for ShowMessageAction is not specified
        /// </summary>
        public const string MessageInActionNotSpecified = "Display Message is not specified.";

        /// <summary>
        /// Error message when idle duration is specified wrongly
        /// </summary>
        public const string InvalidIdleMinutesSpecified = "Idle minutes specified for running a task is invalid.";

        /// <summary>
        /// Error message when Execution Time Limit of a task is specified wrongly
        /// </summary>
        public const string InvalidTaskRunningMinutesSpecified = "Minutes specified for a task to run is invalid.";

        /// <summary>
        /// Error message when no action is configured in a new task to be created
        /// </summary>
        public const string NoActionSpecifiedError = "Atleast one action should be specified for a task.";

        /// <summary>
        /// Represents a task path separator as well as root folder location
        /// </summary>
        public const string TaskPathSeparator = "\\";

        /// <summary>
        /// Specifies the Date Time format to be shown on UI
        /// </summary>
        public const string DateTimeFormat = "dd-MM-yyyy h:mm:ss";

        /// <summary>
        /// Specifies a date time format expected by COM
        /// </summary>
        public const string DateTimeFormatExpectedByCOM = "yyyy-MM-ddThh:mm:ss";

        #endregion
    }
}
