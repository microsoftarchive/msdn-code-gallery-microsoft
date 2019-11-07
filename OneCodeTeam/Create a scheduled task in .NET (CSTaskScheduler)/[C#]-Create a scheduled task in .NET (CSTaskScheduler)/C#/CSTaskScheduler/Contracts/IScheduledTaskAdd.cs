/****************************** Module Header ******************************\
 * Module Name: IScheduledTaskAdd.cs
 * Project: CSTaskScheduler
 * Copyright (c) Microsoft Corporation. 
 * 
 * Represents a view contract for Adding a Scheduled Task.
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
    using Entities;

    /// <summary>
    /// Represents a view contract for Adding a Scheduled Task
    /// </summary>
    public interface IScheduledTaskAdd
    {
        /// <summary>
        /// Event which is raised to add a scheduled task
        /// </summary>
        event EventHandler AddScheduledTask;

        /// <summary>
        /// Gets a Task to be created or updated
        /// </summary>
        ScheduledTask Task { get; }
    }
}
