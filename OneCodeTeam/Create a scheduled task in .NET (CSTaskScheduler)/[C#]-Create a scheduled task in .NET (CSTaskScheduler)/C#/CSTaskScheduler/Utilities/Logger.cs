/****************************** Module Header ******************************\
 * Module Name: Logger.cs
 * Project: CSTaskScheduler
 * Copyright (c) Microsoft Corporation. 
 * 
 * Log helper class to log the messaged into event logs.
 * 
 * This source is subject to the Microsoft Public License. 
 * See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL 
 * All other rights reserved. 
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\****************************************************************************/

namespace CSTaskScheduler.Utilities
{
    using System;
    using System.Configuration;
    using System.Diagnostics;

    /// <summary>
    /// Log helper class to log the messaged into event logs
    /// </summary>
    public class Logger
    {
        #region Constants

        /// <summary>
        /// Specifies an app settings key to read configuration value
        /// </summary>
        private const string IsLoggingEnabledAppSettingsKey = "isLoggingEnabled";

        /// <summary>
        /// Specifies a name of the event source
        /// </summary>
        private const string SourceName = "TaskScheduler";

        #endregion

        #region Private Fields

        /// <summary>
        /// Indicates whether logging is enabled or not
        /// </summary>
        private static bool isLoggingEnabled = CheckIfLoggingEnabled();

        #endregion

        #region Public Methods

        /// <summary>
        /// Logs the error in Event Log
        /// </summary>
        /// <param name="message">Error Message to be logged</param>
        public static void LogError(string message)
        {
            LogError(message, null);
        }

        /// <summary>
        /// Logs the error in Event Log
        /// </summary>
        /// <param name="message">Error Message to be logged</param>
        /// <param name="details">Exception details to be logged</param>
        public static void LogError(string message, Exception details)
        {
            if (isLoggingEnabled)
            {
                Log(EventLogEntryType.Error, message, details);
            }
        }

        /// <summary>
        /// Logs the warning in Event Log
        /// </summary>
        /// <param name="message">Warning Message to be logged</param>
        public static void LogWarning(string message)
        {
            LogWarning(message, null);
        }

        /// <summary>
        /// Logs the warning in Event Log
        /// </summary>
        /// <param name="message">Warning Message to be logged</param>
        /// <param name="details">Exception details to be logged</param>
        public static void LogWarning(string message, Exception details)
        {
            if (isLoggingEnabled)
            {
                Log(EventLogEntryType.Warning, message, details);
            }
        }

        /// <summary>
        /// Logs the information in Event Log
        /// </summary>
        /// <param name="message">Information Message to be logged</param>
        public static void LogInformation(string message)
        {
            LogInformation(message, null);
        }

        /// <summary>
        /// Logs the information in Event Log
        /// </summary>
        /// <param name="message">Information Message to be logged</param>
        /// <param name="details">Exception details to be logged</param>
        public static void LogInformation(string message, Exception details)
        {
            if (isLoggingEnabled)
            {
                Log(EventLogEntryType.Information, message, details);
            }
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Logs the message into event logs
        /// </summary>
        /// <param name="type">Type of a log entry</param>
        /// <param name="message">Message to be logged</param>
        /// <param name="details">Exception details, if any</param>
        private static void Log(EventLogEntryType type, string message, Exception details)
        {
            message = string.IsNullOrWhiteSpace(message) ? string.Empty : message;

            message += details != null ? "\n" + details.ToString() : string.Empty;

            EventLog.WriteEntry(SourceName, message, type);
        }

        /// <summary>
        /// Checks if logging is enabled for a system
        /// </summary>
        /// <returns>True if configured, false otherwise.</returns>
        private static bool CheckIfLoggingEnabled()
        {
            bool isEnabled = false;

            string configuredValue = ConfigurationManager.AppSettings[IsLoggingEnabledAppSettingsKey];

            if (!string.IsNullOrWhiteSpace(configuredValue))
            {
                configuredValue = configuredValue.ToLower();

                switch (configuredValue)
                {
                    case "1":
                    case "true":
                    case "yes":
                        isEnabled = true;
                        break;
                }
            }

            return isEnabled;
        }
        
        #endregion
    }
}
