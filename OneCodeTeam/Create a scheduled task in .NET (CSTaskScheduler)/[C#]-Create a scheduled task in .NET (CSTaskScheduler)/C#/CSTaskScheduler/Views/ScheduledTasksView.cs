/****************************** Module Header ******************************\
 * Module Name: ScheduledTasksView.cs
 * Project: CSTaskScheduler
 * Copyright (c) Microsoft Corporation. 
 * 
 * Form to view all Scheduled Task and delete a scheduled task.
 * 
 * This source is subject to the Microsoft Public License. 
 * See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL 
 * All other rights reserved. 
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\****************************************************************************/

namespace CSTaskScheduler.Views
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Forms;
    using Contracts;
    using Entities;
    using Presenters;
    using Utilities;

    /// <summary>
    /// Form to view all Scheduled Task and delete a scheduled task
    /// </summary>
    public partial class ScheduledTasksView : Form, IScheduledTasksView
    {
        #region Private Fields

        /// <summary>
        /// Worker thread to fetch a list of scheduled tasks
        /// </summary>
        private BackgroundWorker worker;

        /// <summary>
        /// Presenter which handles ScheduledTasksView view
        /// </summary>
        private IScheduledTasksViewPresenter presenter;
        
        /// <summary>
        /// List of all scheduled tasks
        /// </summary>
        private List<ScheduledTask> scheduledTasks;
        
        /// <summary>
        /// Index of a current row
        /// </summary>
        private int currentRowIndex;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ScheduledTasksView class
        /// </summary>
        public ScheduledTasksView()
        {
            InitializeComponent();
            
            this.AttachEventHandlers();
        }

        #endregion

        #region IScheduledTasksView Implementation

        /// <summary>
        /// Event which is raised to load all scheduled tasks
        /// </summary>
        public event EventHandler LoadScheduledTasks;

        /// <summary>
        /// Event which is raised to delete a scheduled task
        /// </summary>
        public event EventHandler DeleteScheduledTask;

        /// <summary>
        /// Gets or sets a List of all scheduled tasks
        /// </summary>
        public List<ScheduledTask> ScheduledTasks
        {
            get
            {
                return this.scheduledTasks;
            }

            set
            {
                this.scheduledTasks = value;
            }
        }

        /// <summary>
        /// Gets a Path of a selected Task
        /// </summary>
        public string TaskPath
        {
            get
            {
                return this.ScheduledTasksViewer["TaskPath", this.currentRowIndex].Value.ToString();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the events to get the fresh list of all scheduled tasks
        /// </summary>
        /// <param name="sender">Object which raised this event</param>
        /// <param name="e">Arguments passed along with this event</param>
        private void RefreshScheduledTasks(object sender, EventArgs e)
        {
            this.Initialize();
            this.worker.RunWorkerAsync();
        }

        /// <summary>
        /// Handles the event raised when a background worker is executed
        /// </summary>
        /// <param name="sender">Object which raised this event</param>
        /// <param name="e">Arguments passed along with this event</param>
        private void RetrieveScheduledTasks(object sender, DoWorkEventArgs e)
        {
            if (this.LoadScheduledTasks != null)
            {
                this.LoadScheduledTasks(sender, e);
            }
        }

        /// <summary>
        /// Handles the Click event of a Create button
        /// </summary>
        /// <param name="sender">Object which raised this event</param>
        /// <param name="e">Arguments passed along with this event</param>
        private void ScheduledTasksLoadComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                this.BindData();
            }
            else if (e.Error != null)
            {
                string errorMessage = Constants.TasksFetchError.Equals(e.Error.Message) ? Constants.TasksFetchError :
                    string.Format("{0} - [Error Details: {1}]", Constants.TasksFetchError, e.Error.Message);
                
                this.SetError(this.ScheduledTasksViewer, errorMessage);
            }
        }

        /// <summary>
        /// Handles the Click event to delete a selected scheduled task
        /// </summary>
        /// <param name="sender">Object which raised this event</param>
        /// <param name="e">Arguments passed along with this event</param>
        private void Delete(object sender, EventArgs e)
        {
            try
            {
                if (this.DeleteScheduledTask != null)
                {
                    this.DeleteScheduledTask(sender, e);

                    this.RefreshScheduledTasks(sender, e);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(Constants.TaskDeleteError, ex);

                string errorMessage = string.Concat(Constants.TaskDeleteError, this.TaskPath);

                errorMessage = Constants.TasksFetchError.Equals(ex.Message) ? errorMessage :
                    string.Format("{0} - [Error Details: {1}]", errorMessage, ex.Message);

                this.SetError(this.ScheduledTasksViewer, errorMessage);
            }
        }

        /// <summary>
        /// Handles the Cell mouse down event to show a Context menu for each row
        /// </summary>
        /// <param name="sender">Object which raised this event</param>
        /// <param name="e">Arguments passed along with this event</param>
        private void ScheduledTasksViewer_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            this.currentRowIndex = e.RowIndex;
        }

        /// <summary>
        /// Handles the Click event of a Create button to switch to Create Scrren
        /// </summary>
        /// <param name="sender">Object which raised this event</param>
        /// <param name="e">Arguments passed along with this event</param>
        private void CreateTaskAction_Click(object sender, EventArgs e)
        {
            using (ScheduledTaskAdd addForm = new ScheduledTaskAdd())
            {
                DialogResult result = addForm.ShowDialog();

                if (result.Equals(DialogResult.OK))
                {
                    // refresh the tasks list
                    this.RefreshScheduledTasks(sender, e);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes a form and it's controls
        /// </summary>
        private void Initialize()
        {
            if (this.presenter == null)
            {
                this.presenter = new ScheduledTasksViewPresenter(this);
            }

            if (this.worker != null)
            {
                if (!this.worker.CancellationPending)
                {
                    this.worker.CancelAsync();
                }

                this.worker.Dispose();
            }

            this.worker = new BackgroundWorker();
            this.worker.WorkerSupportsCancellation = true;
            this.worker.DoWork += new DoWorkEventHandler(this.RetrieveScheduledTasks);
            this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.ScheduledTasksLoadComplete);

            this.ToolStripProgressBar.ProgressBar.Show();

            this.ErrorProvider.Clear();
        }

        /// <summary>
        /// Attaches the event handlers
        /// </summary>
        private void AttachEventHandlers()
        {
            this.Load += new EventHandler(this.RefreshScheduledTasks);
            this.RefreshScheduledTasksAction.Click += new EventHandler(this.RefreshScheduledTasks);
            this.DeleteToolStripMenuItem.Click += new EventHandler(this.Delete);
            this.CreateTaskAction.Click += new EventHandler(this.CreateTaskAction_Click);
            this.ScheduledTasksViewer.CellMouseDown += new DataGridViewCellMouseEventHandler(this.ScheduledTasksViewer_CellMouseDown);
        }

        /// <summary>
        /// Binds the data to the controls
        /// </summary>
        private void BindData()
        {
            this.ScheduledTasksViewer.DataSource = this.ScheduledTasks;

            this.ToolStripProgressBar.ProgressBar.Hide();
        }

        /// <summary>
        /// Sets error description to be shown on screen
        /// </summary>
        /// <param name="control">Control for which error has to be set</param>
        /// <param name="errorMessage">Error Message</param>
        private void SetError(Control control, string errorMessage)
        {
            this.ErrorProvider.SetError(control, errorMessage);
            this.ErrorProvider.SetIconAlignment(control, ErrorIconAlignment.BottomRight);
        }

        #endregion
    }
}

