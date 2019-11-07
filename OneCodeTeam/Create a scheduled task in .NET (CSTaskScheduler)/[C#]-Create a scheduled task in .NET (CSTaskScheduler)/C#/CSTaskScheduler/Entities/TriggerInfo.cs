/****************************** Module Header ******************************\
 * Module Name: TriggerInfo.cs
 * Project: CSTaskScheduler
 * Copyright (c) Microsoft Corporation. 
 * 
 * TriggerInfo.
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
    /// Type of a trigger
    /// </summary>
    [Serializable]
    public enum TriggerType
    {
        /// <summary>
        /// executed only once in a lifetime
        /// </summary>
        Once,

        /// <summary>
        /// executed daily
        /// </summary>
        Daily,

        /// <summary>
        /// executed weekly
        /// </summary>
        Weekly,

        /// <summary>
        /// executed monthly
        /// </summary>
        Monthly
    }

    /// <summary>
    /// Base class representing the trigger based on schedule
    /// </summary>
    [Serializable]
    public abstract class ScheduleTriggerInfo
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a date and time when this trigger will come into an effect
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this trigger ever expires
        /// </summary>
        public bool DoesExpire { get; set; }

        /// <summary>
        /// Gets or sets a date and time when this trigger will expire
        /// </summary>
        public DateTime EndTime { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether a trigger is enabled or not
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets a Type of a trigger
        /// </summary>
        public abstract TriggerType Type { get; }

        #endregion
    }

    /// <summary>
    /// Represents a trigger which executes only once
    /// </summary>
    [Serializable]
    public sealed class OneTimeTrigger : ScheduleTriggerInfo
    {
        #region Public Properties

        /// <summary>
        /// Gets a Type of a trigger
        /// </summary>
        public override TriggerType Type
        {
            get
            {
                return TriggerType.Daily;
            }
        }

        #endregion
    }

    // You can Create Classes for Daily, Weekly n Monthly Triggers as well. For demonstration purposes, i created only for a one time trigger
}
