/****************************** Module Header ******************************\
 * Module Name: Settings.cs
 * Project: CSTaskScheduler
 * Copyright (c) Microsoft Corporation. 
 * 
 * Represents a class to maintain settings of a task.
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
    /// Represents a class to maintain settings of a task
    /// </summary>
    [Serializable]
    public class Settings
    {
        #region Fields

        /// <summary>
        /// Specifies time in minutes, a task can run
        /// </summary>
        [DefaultValue(60)]
        private int taskRunTime = 60; // default is 60 minutes
        
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether a task should be stopped if it runs longer for specified minutes
        /// </summary>
        public bool StopTaskWhenRunsLonger { get; set; }

        /// <summary>
        /// Gets or sets the time in minutes, task has been running, after which task should be stopped
        /// </summary>
        public int ExecutionTimeLimit
        {
            get
            {
                return this.taskRunTime;
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentException(Constants.InvalidTaskRunningMinutesSpecified);
                }

                this.taskRunTime = value;
            }
        }

        /// <summary>
        /// Gets or sets a User Account (doaminName\userName) on which task will be running
        /// </summary>
        public string UserAccount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether task should be run only when specified user is logged on or not
        /// </summary>
        public bool RunOnlyWhenUserIsLoggedOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a task is enabled or disabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a task is hidden or not
        /// </summary>
        public bool Hidden { get; set; }

        #endregion
    }
}
