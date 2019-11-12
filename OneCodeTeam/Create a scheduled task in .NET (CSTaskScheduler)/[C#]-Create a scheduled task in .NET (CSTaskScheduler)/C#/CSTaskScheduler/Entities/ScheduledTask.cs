/****************************** Module Header ******************************\
 * Module Name: ScheduledTask.cs
 * Project: CSTaskScheduler
 * Copyright (c) Microsoft Corporation. 
 * 
 * Represents a scheduled task.
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
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Represents a scheduled task
    /// </summary>
    public class ScheduledTask
    {
        #region Public Properties
        
        /// <summary>
        /// Gets or sets the name of a Scheduled Task
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the task description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the author of a Scheduled Task
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the Current State of a task
        /// </summary>
        public string CurrentState { get; set; }

        /// <summary>
        /// Gets or sets the path
        /// </summary>
        public string TaskPath { get; set; }
        
        /// <summary>
        /// Gets or sets the next date n time details when this task will run
        /// </summary>
        public DateTime NextRunTime { get; set; }
        
        /// <summary>
        /// Gets or sets the date n time details when task was last ran
        /// </summary>
        public DateTime LastRunTime { get; set; }
        
        /// <summary>
        /// Gets or sets the results when task was last ran
        /// </summary>
        public string LastRunResults { get; set; }
        
        /// <summary>
        /// Gets or sets the Priority of a task
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets a List of all triggers which can trigger the execution of this task
        /// </summary>
        [Browsable(false)]
        public List<ScheduleTriggerInfo> Triggers { get; set; }

        /// <summary>
        /// Gets or sets a List of all actions to be taken when a task is made to execute
        /// </summary>
        [Browsable(false)]
        public List<Action> Actions { get; set; }

        /// <summary>
        /// Gets or sets Pre Conditions for a task to run
        /// </summary>
        [Browsable(false)]
        public Conditions Conditions { get; set; }

        /// <summary>
        /// Gets or sets the Settings of a task
        /// </summary>
        [Browsable(false)]
        public Settings Settings { get; set; }

        #endregion
    }
}
