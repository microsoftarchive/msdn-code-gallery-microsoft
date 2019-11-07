// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="Tracing.cs">
//   Copyright (c) 2014 Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   Use of this sample source code is subject to the terms of the Microsoft license
//   agreement under which you licensed this sample source code and is provided AS-IS.
//   If you did not accept the terms of the license agreement, you are not authorized
//   to use this sample source code. For the terms of the license, please see the
//   license agreement between you and Microsoft.<br/><br/>
//   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604.
// </summary>
// ----------------------------------------------------------------------------

namespace CameraSampleCS.Helpers
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Simple logger class.
    /// </summary>
    public static class Tracing
    {
#if DEBUG
        /// <summary>
        /// When a previous log message was written.
        /// </summary>
        private static DateTime? prevLogTime;
#endif

        /// <summary>
        /// Logs the <paramref name="message"/> given.
        /// </summary>
        /// <param name="message">Message to be logged.</param>
        public static void Trace(string message)
        {
#if DEBUG
            Tracing.Trace(message, null);
#endif
        }

        /// <summary>
        /// Adds the <paramref name="obj"/> given to the log.
        /// </summary>
        /// <param name="obj">Object, which string representation should be logged.</param>
        public static void Trace(object obj)
        {
#if DEBUG
            Tracing.Trace(obj != null ? obj.ToString() : string.Empty, null);
#endif
        }

        /// <summary>
        /// Logs the <paramref name="message"/> given.
        /// </summary>
        /// <param name="message">Message to be logged.</param>
        /// <param name="args">Message arguments.</param>
        public static void Trace(string message, params object[] args)
        {
#if DEBUG
            if (args != null && args.Length != 0)
            {
                try
                {
                    message = string.Format(CultureInfo.InvariantCulture, message, args);
                }
                catch
                {
                    // Do nothing.
                }
            }

            DateTime now = DateTime.Now;

            string deltaString = prevLogTime.HasValue
                                 ? string.Format(CultureInfo.InvariantCulture, " ({0} ms)", ((TimeSpan)(now - prevLogTime)).TotalMilliseconds)
                                 : string.Empty;

            string resultMessage = string.Format(CultureInfo.InvariantCulture, "{0:HH:mm:ss.fff}{1}: {2}", now, deltaString, message);
            Debug.WriteLine(resultMessage);

            prevLogTime = now;
#endif
        }
    }
}
