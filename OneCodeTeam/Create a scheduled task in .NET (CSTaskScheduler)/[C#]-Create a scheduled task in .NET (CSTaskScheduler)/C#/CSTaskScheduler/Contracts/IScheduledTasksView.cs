/****************************** Module Header ******************************\
 * Module Name: IScheduledTasksView.cs
 * Project: CSTaskScheduler
 * Copyright (c) Microsoft Corporation. 
 * 
 * Represents a view contract for Viewing and deleting a Scheduled Task.
 * 
 * This source is subject to the Microsoft Public License. 
 * See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL 
 * All other rights reserved. 
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\****************************************************************************/

namespace CSTaskScheduler.Contracts
{
    using System;
    using System.Collections.Generic;
    using Entities;

    /// <summary>
    /// Represents a view contract for Viewing and deleting a Scheduled Task
    /// </summary>
    public interface IScheduledTasksView
    {
        /// <summary>
        /// Event which is raised to load all scheduled tasks
        /// </summary>
        event EventHandler LoadScheduledTasks;

        /// <summary>
        /// Event which is raised to delete a scheduled task
        /// </summary>
        event EventHandler DeleteScheduledTask;

        /// <summary>
        /// Sets a List of all scheduled tasks
        /// </summary>
        List<ScheduledTask> ScheduledTasks { set; }

        /// <summary>
        /// Gets a Path of a selected Task
        /// </summary>
        string TaskPath { get; }
    }
}
