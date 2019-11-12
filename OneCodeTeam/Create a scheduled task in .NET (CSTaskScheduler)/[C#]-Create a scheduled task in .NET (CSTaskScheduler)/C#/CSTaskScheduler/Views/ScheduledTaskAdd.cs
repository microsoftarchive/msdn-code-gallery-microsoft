/****************************** Module Header ******************************\
 * Module Name: ScheduledTaskAdd.cs
 * Project: CSTaskScheduler
 * Copyright (c) Microsoft Corporation. 
 * 
 * Form to add a Scheduled Task.
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
    using System.Security.Principal;
    using System.Windows.Forms;
    using Contracts;
    using Entities;
    using Presenters;
    using Utilities;

    /// <summary>
    /// Form to add a Scheduled Task
    /// </summary>
    public partial class ScheduledTaskAdd : Form, IScheduledTaskAdd
    {
        #region Private Fields

        /// <summary>
        /// Presenter which handles ScheduledTaskAdd view
        /// </summary>
        private IScheduledTaskAddPresenter presenter;

        /// <summary>
        /// New Task to be created in the system
        /// </summary>
        private ScheduledTask newTask;

        /// <summary>
        /// Specifies whether parent form should be refreshed or not
        /// </summary>
        private bool signalParentToRefresh = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ScheduledTaskAdd class
        /// </summary>
        public ScheduledTaskAdd()
        {
            this.InitializeComponent();

            this.Initialize();

            this.AttachEventHandlers();

            this.Reset();
        }

        #endregion

        #region IScheduledTaskAdd Implementation

        /// <summary>
        /// Event which is raised to add a scheduled task
        /// </summary>
        public event EventHandler AddScheduledTask;

        /// <summary>
        /// Gets a Task to be created or updated
        /// </summary>
        public ScheduledTask Task
        {
            get 
            {
                this.PrepareScheduledTask();

                return this.newTask;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the Click event of a Browse button to select a file
        /// </summary>
        /// <param name="sender">Object which raised this event</param>
        /// <param name="e">Arguments passed along with this event</param>
        private void ChooseFile_Click(object sender, EventArgs e)
        {
            if (this.FileDialog.ShowDialog().CompareTo(DialogResult.OK) == 0)
            {
                this.SelectedFileData.Text = this.FileDialog.FileName;
            }
        }

        /// <summary>
        /// Handles the Click event of a Cancel button to go back to ScheduledTasksView
        /// </summary>
        /// <param name="sender">Object which raised this event</param>
        /// <param name="e">Arguments passed along with this event</param>
        private void CancelAction_Click(object sender, EventArgs e)
        {
            // go back to ScheduledTasksView screen
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of a Create button
        /// </summary>
        /// <param name="sender">Object which raised this event</param>
        /// <param name="e">Arguments passed along with this event</param>
        private void CreateAction_Click(object sender, EventArgs e)
        {
            Control senderControl = sender as Control;

            senderControl.Enabled = false;

            try
            {
                if (this.AddScheduledTask != null)
                {
                    this.AddScheduledTask(sender, e);
                }

                MessageBox.Show(string.Format("Task: {0} Created successfully.", this.newTask.Name));

                this.Reset();

                this.signalParentToRefresh = true;
            }
            catch (Exception ex)
            {
                Logger.LogError(Constants.TaskCreateError, ex);

                string errorMessage = Constants.TaskCreateError.Equals(ex.Message) ? Constants.TaskCreateError :
                    string.Format("{0} - [Error Details: {1}]", Constants.TaskCreateError, ex.Message);

                this.SetError(this.ScheduledTaskMetadataTabControl, errorMessage);
            }

            senderControl.Enabled = true;
        }

        /// <summary>
        /// Handles the event of a StopOption Checkbox
        /// </summary>
        /// <param name="sender">Object which raised this event</param>
        /// <param name="e">Arguments passed along with this event</param>
        private void StopOption_CheckedChanged(object sender, EventArgs e)
        {
            this.StopTaskAfterMinutesData.Enabled = this.StopOption.Checked;
        }

        /// <summary>
        /// Handles the event of a IdleOption Checkbox
        /// </summary>
        /// <param name="sender">Object which raised this event</param>
        /// <param name="e">Arguments passed along with this event</param>
        private void IdleOption_CheckedChanged(object sender, EventArgs e)
        {
            this.MinutesData.Enabled = this.IdleOption.Checked;
        }

        /// <summary>
        /// Handles the event of a ExpireOption Checkbox
        /// </summary>
        /// <param name="sender">Object which raised this event</param>
        /// <param name="e">Arguments passed along with this event</param>
        private void ExpireOption_CheckedChanged(object sender, EventArgs e)
        {
            this.EndDateTimeData.Enabled = this.ExpireOption.Checked;
        }

        /// <summary>
        /// Gets fired when this form is about to close
        /// </summary>
        /// <param name="sender">Sender who raised this event</param>
        /// <param name="e">Arguments passed along with this event</param>
        private void ScheduledTaskAdd_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Inform a parent to refresh a list only if a new task had been created in the session
            if (this.signalParentToRefresh)
            {
                this.DialogResult = DialogResult.OK;
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
                this.presenter = new ScheduledTaskAddPresenter(this);
            }
        }

        /// <summary>
        /// Resets all the controls' data
        /// </summary>
        private void Reset()
        {
            this.ErrorProvider.Clear();

            // General tab
            this.NameData.Text = string.Empty;
            this.DescriptionData.Text = string.Empty;
            this.IsHiddenOption.Checked = false;
            this.RunUserLoggedOnOrNotOption.Checked = false;
            this.RunUserLoggedOnOption.Checked = true;
            
            // default, credentials of a user running this application
            this.SecurityUserNameText.Text = WindowsIdentity.GetCurrent().Name;

            // Trigger tab
            this.StartDateTimeData.Value = DateTime.Now;
            this.ExpireOption.Checked = true;
            this.EndDateTimeData.Value = DateTime.Now;

            // Action tab
            this.SelectedFileData.Text = string.Empty;
            this.ArgumentsData.Text = string.Empty;

            // Condition tab
            this.IdleOption.Checked = false;
            this.MinutesData.Enabled = false;
            this.MinutesData.Value = this.MinutesData.Minimum;

            // Settings tab
            this.StopOption.Checked = false;
            this.StopTaskAfterMinutesData.Enabled = false;
            this.StopTaskAfterMinutesData.Value = this.StopTaskAfterMinutesData.Minimum;
        }

        /// <summary>
        /// Attaches the event handlers
        /// </summary>
        private void AttachEventHandlers()
        {
            this.CreateAction.Click += new EventHandler(this.CreateAction_Click);
            this.CancelAction.Click += new EventHandler(this.CancelAction_Click);
            this.ChooseFile.Click += new EventHandler(this.ChooseFile_Click);

            this.IdleOption.CheckedChanged += new EventHandler(this.IdleOption_CheckedChanged);
            this.StopOption.CheckedChanged += new EventHandler(this.StopOption_CheckedChanged);
            this.ExpireOption.CheckedChanged += new EventHandler(this.ExpireOption_CheckedChanged);

            this.FormClosing += new FormClosingEventHandler(this.ScheduledTaskAdd_FormClosing);
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

        /// <summary>
        /// Prepares a scheduled task to be created/updated in the system
        /// </summary>
        private void PrepareScheduledTask()
        {
            this.newTask = new ScheduledTask();

            this.newTask.Name = this.NameData.Text;
            this.newTask.TaskPath = this.LocationData.Text;
            this.newTask.Description = this.DescriptionData.Text;
            this.newTask.Author = this.SecurityUserNameText.Text;

            // General Data
            this.PrepareSettings();

            this.PrepareScheduledTaskTriggerData();
            this.PrepareScheduledTaskActionData();

            // Conditions data
            this.PrepareConditions();
        }

        /// <summary>
        /// Configures general settings on a new task
        /// </summary>
        private void PrepareSettings()
        {
            Settings settings = new Settings();
            settings.Hidden = this.IsHiddenOption.Checked;
            settings.Enabled = true;
            settings.UserAccount = this.SecurityUserNameText.Text;
            settings.RunOnlyWhenUserIsLoggedOn = !this.RunUserLoggedOnOrNotOption.Checked;

            // Settings data 
            if (this.StopOption.Checked)
            {
                settings.StopTaskWhenRunsLonger = true;
                settings.ExecutionTimeLimit = Convert.ToInt32(this.StopTaskAfterMinutesData.Value);
            }

            this.newTask.Settings = settings;
        }

        /// <summary>
        /// Prepares the trigger data
        /// </summary>
        private void PrepareScheduledTaskTriggerData()
        {
            // Trigger Data
            this.newTask.Triggers = new List<ScheduleTriggerInfo>();
            ScheduleTriggerInfo trigger = new OneTimeTrigger();
            trigger.StartTime = this.StartDateTimeData.Value;

            if (this.ExpireOption.Checked)
            {
                trigger.DoesExpire = true;
                trigger.EndTime = this.EndDateTimeData.Value;
            }

            trigger.Enabled = this.EnableOption.Checked;

            this.newTask.Triggers.Add(trigger);
        }

        /// <summary>
        /// Prepares the Action Data
        /// </summary>
        private void PrepareScheduledTaskActionData()
        {
            // Trigger Data
            if (!string.IsNullOrWhiteSpace(this.SelectedFileData.Text))
            {
                this.newTask.Actions = new List<Entities.Action>();
                Entities.Action action = new StartProgramAction(this.SelectedFileData.Text, this.ArgumentsData.Text);

                this.newTask.Actions.Add(action);
            }
        }

        /// <summary>
        /// Prepares the conditions for a new task
        /// </summary>
        private void PrepareConditions()
        {
            Conditions conditions = new Conditions();

            if (this.IdleOption.Checked)
            {
                conditions.RunOnlyIfIdle = true;
                conditions.IdleDurationToStartTaskWhenSystemIsIdle = Convert.ToInt32(this.MinutesData.Value);
            }

            this.newTask.Conditions = conditions;
        }

        #endregion
    }
}
