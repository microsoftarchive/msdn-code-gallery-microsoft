/****************************** Module Header ******************************\
 * Module Name: ScheduledTaskAddPresenter.cs
 * Project: CSTaskScheduler
 * Copyright (c) Microsoft Corporation. 
 * 
 * Presenter which handles ScheduledTaskAdd view.
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
    using Exceptions;

    /// <summary>
    /// Presenter which handles ScheduledTaskAdd view
    /// </summary>
    public class ScheduledTaskAddPresenter : IScheduledTaskAddPresenter
    {
        #region Private Fields

        /// <summary>
        /// View which this presenter handles
        /// </summary>
        private IScheduledTaskAdd view;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ScheduledTaskAddPresenter class
        /// </summary>
        /// <param name="view">View for which it gets the data</param>
        public ScheduledTaskAddPresenter(IScheduledTaskAdd view)
        {
            this.view = view;
            this.view.AddScheduledTask += new EventHandler(this.AddScheduledTask);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds the Scheduled task
        /// </summary>
        /// <param name="sender">Object which raised this event</param>
        /// <param name="e">Arguments passed along with this event</param>
        /// <exception cref="NoActionSpeciifedException">Thrown when a task doesn't have at least one action configured</exception>
        /// <exception cref="ApplicationException">Thrown when a task can't be created or added</exception>
        private void AddScheduledTask(object sender, EventArgs e)
        {
            bool isSuccessful = false;

            if (this.view.Task.Actions == null || this.view.Task.Actions.Count == 0)
            {
                throw new NoActionSpeciifedException();
            }

            using (TaskSchedulerAdapter adapter = new TaskSchedulerAdapter())
            {
                isSuccessful = adapter.CreateTask(this.view.Task);
            }

            if (!isSuccessful)
            {
                throw new ApplicationException(Constants.TaskCreateError);
            }
        }

        #endregion
    }
}
