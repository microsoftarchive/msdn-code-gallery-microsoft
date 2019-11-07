/****************************** Module Header ******************************\
 * Module Name: Condition.cs
 * Project: CSTaskScheduler
 * Copyright (c) Microsoft Corporation. 
 * 
 * Represents a class to specify the conditions for a scheduled task.
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
    using System.ComponentModel;

    /// <summary>
    /// Represents a class to specify the conditions for a scheduled task
    /// </summary>
    [Serializable]
    public class Conditions
    {
        #region Fields

        /// <summary>
        /// Specifies an dle duration in minutes
        /// </summary>
        [DefaultValue(1)]
        private int idleDurationInMinutes = 1; // default is 1 minute
        
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to start a task when system is idle or not
        /// </summary>
        public bool RunOnlyIfIdle { get; set; }

        /// <summary>
        /// Gets or sets a time in terms of no of minutes to start a task when system is idle
        /// </summary>
        public int IdleDurationToStartTaskWhenSystemIsIdle 
        {
            get
            {
                return this.idleDurationInMinutes;
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentException(Constants.InvalidIdleMinutesSpecified);
                }

                this.idleDurationInMinutes = value;
            }
        }

        #endregion
    }
}
