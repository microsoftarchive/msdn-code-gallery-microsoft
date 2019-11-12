/****************************** Module Header ******************************\
 * Module Name: ScheduledTasksViewPresenter.cs
 * Project: CSTaskScheduler
 * Copyright (c) Microsoft Corporation. 
 * 
 * Presenter which handles ScheduledTasksView view.
 * 
 * This source is subject to the Microsoft Public License. 
 * See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL 
 * All other rights reserved. 
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\****************************************************************************/

namespace CSTaskScheduler.Presenters
{
    using System;
    using Adapter;
    using Contracts;

    /// <summary>
    /// Presenter which handles ScheduledTasksView view
    /// </summary>
    public class ScheduledTasksViewPresenter : IScheduledTasksViewPresenter
    {
        #region Private Fields

        /// <summary>
        /// View which this presenter handles
        /// </summary>
        private IScheduledTasksView view;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ScheduledTasksViewPresenter class
        /// </summary>
        /// <param name="view">View for which it gets the data</param>
        public ScheduledTasksViewPresenter(IScheduledTasksView view)
        {
            this.view = view;
            this.view.LoadScheduledTasks += new EventHandler(this.Load);
            this.view.DeleteScheduledTask += new EventHandler(this.DeleteScheduledTask);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the data using an adapter
        /// </summary>
        /// <param name="sender">object which instantiated this event</param>
        /// <param name="e">Arguments passed along with event</param>
        private void Load(object sender, EventArgs e)
        {
            using (TaskSchedulerAdapter adapter = new TaskSchedulerAdapter())
            {
                this.view.ScheduledTasks = adapter.RetrieveScheduledTasks();
            }
        }

        /// <summary>
        /// Deletes a scheduled task using an adapter
        /// </summary>
        /// <param name="sender">object which instantiated this event</param>
        /// <param name="e">Arguments passed along with event</param>
        /// <exception cref="ApplicationException">Thrown when a task can't be deleted</exception>
        private void DeleteScheduledTask(object sender, EventArgs e)
        {
            bool isSuccessful = false;

            using (TaskSchedulerAdapter adapter = new TaskSchedulerAdapter())
            {
                isSuccessful = adapter.DeleteTask(this.view.TaskPath);
            }

            if (!isSuccessful)
            {
                throw new ApplicationException(Constants.TaskDeleteError);
            }
        }

        #endregion
    }
}
