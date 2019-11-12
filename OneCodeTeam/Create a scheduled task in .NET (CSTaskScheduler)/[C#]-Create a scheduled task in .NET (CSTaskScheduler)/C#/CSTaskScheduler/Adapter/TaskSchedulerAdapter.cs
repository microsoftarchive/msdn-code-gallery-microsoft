/****************************** Module Header ******************************\
 * Module Name: TaskSchedulerAdapter.cs
 * Project: CSTaskScheduler
 * Copyright (c) Microsoft Corporation. 
 * 
 * Represents a class which talks to TaskScheduler Service and executes the operations.
 * 
 * This source is subject to the Microsoft Public License. 
 * See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL 
 * All other rights reserved. 
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 \****************************************************************************/

namespace CSTaskScheduler.Adapter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using Entities;
    using Exceptions;
    using TaskScheduler;
    using Utilities;

    /// <summary>
    /// Represents a class which talks to TaskScheduler Service and executes the operations
    /// </summary>
    public class TaskSchedulerAdapter : IDisposable
    {
        #region Private Fields

        /// <summary>
        /// Task scheduler Service
        /// </summary>
        private ITaskService taskService = new TaskScheduler();

        /// <summary>
        /// Indicates whether it is already disposed or not
        /// </summary>
        private bool isDisposed = false;

        #endregion

        #region Finalizer

        /// <summary>
        /// Finalizes an instance of the TaskSchedulerAdapter class
        /// </summary>
        ~TaskSchedulerAdapter()
        {
            this.Dispose(false);
        }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves a list of all scheduled tasks
        /// </summary>
        /// <returns>a List containing all scheduled tasks</returns>
        public List<ScheduledTask> RetrieveScheduledTasks()
        {
            List<ScheduledTask> tasks = null;

            try
            {
                this.ConnectTaskSchedulerService();

                tasks = new List<ScheduledTask>();

                // "\\" or @"\" is the RootFolder
                GetData(this.taskService.GetFolder(Constants.TaskPathSeparator), tasks);
            }
            catch (Exception exception)
            {
                Logger.LogError(Constants.TasksFetchError, exception);

                throw;
            }

            return tasks;
        }

        /// <summary>
        /// Creates a scheduled task
        /// </summary>
        /// <param name="task">Scheduled Task object containing all configured iformation</param>
        /// <returns>True if task gets created successfully</returns>
        /// <exception cref="TaskScheduler.Exceptions.InvalidTaskNameException">Thrown when task name has invalid/illegal characters</exception>
        public bool CreateTask(ScheduledTask task)
        {
            if (task.Name.Contains(Constants.TaskPathSeparator))
            {
                throw new InvalidTaskNameException();
            }

            try
            {
                this.ConnectTaskSchedulerService();

                ITaskDefinition definition = this.TransformToRegisteredTask(task);

                // creating this task in the root Folder
                // you may choose to create in sub-folders.
                // Create SubFolder under RootFolder, if you require
                ITaskFolder rootFolder = this.taskService.GetFolder(Constants.TaskPathSeparator);

                // 6 as flags (3rd argument) means this task can be created or updated ["CreateOrUpdate" flag]
                // Add the task to the RootFolder
                // if Name id empty or null, System will create a task with name as GUID
                rootFolder.RegisterTaskDefinition(task.Name, definition, 6, null, null, _TASK_LOGON_TYPE.TASK_LOGON_NONE, null);
            }
            catch (Exception exception)
            {
                Logger.LogError(Constants.TaskCreateError, exception);

                throw;
            }

            return true;
        }

        /// <summary>
        /// Deletes a scheduled task
        /// </summary>
        /// <param name="taskPath">Path of a task</param>
        /// <returns>True if task gets deleted successfully</returns>
        /// <exception cref="TaskScheduler.Exceptions.InvalidTaskPathException">Thrown when a task with this path does not exist</exception>
        public bool DeleteTask(string taskPath)
        {
            bool isSuccessful = false;

            if (!ValidateTaskPath(taskPath))
            {
                throw new InvalidTaskPathException();
            }

            int lastIndex = taskPath.LastIndexOf(Constants.TaskPathSeparator);

            string folderPath = taskPath.Substring(0, lastIndex);

            if (string.IsNullOrWhiteSpace(folderPath))
            {
                folderPath = Constants.TaskPathSeparator;
            }

            string taskName = taskPath.Substring(lastIndex + 1);

            try
            {
                this.ConnectTaskSchedulerService();

                ITaskFolder containingFolder = this.taskService.GetFolder(folderPath);

                containingFolder.DeleteTask(taskName, 0);

                ReleaseComObject(containingFolder);

                isSuccessful = true;
            }
            catch (FileNotFoundException exception)
            {
                Logger.LogWarning(Constants.TaskDeleteError, exception);

                throw new InvalidTaskPathException(Constants.InvalidTaskPathError, exception);
            }
            catch (Exception exception)
            {
                Logger.LogError(Constants.TaskDeleteError, exception);

                throw;
            }

            return isSuccessful;
        }

        #region IDisposable Implementation

        /// <summary>
        /// Disposes the resources
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Validates the task of a path
        /// </summary>
        /// <param name="taskPath">Path of a task</param>
        /// <returns>True if task path is valid, false otherwise</returns>
        private static bool ValidateTaskPath(string taskPath)
        {
            bool isValid = false;

            if (!string.IsNullOrWhiteSpace(taskPath))
            {
                int lastIndex = taskPath.LastIndexOf(Constants.TaskPathSeparator);

                if (!(lastIndex < -1 || lastIndex == taskPath.Length - 1))
                {
                    isValid = true;
                }
            }

            return isValid;
        }

        /// <summary>
        /// Maps the type of trigger from business entity to COM representation
        /// </summary>
        /// <param name="type">Trigger Type</param>
        /// <returns>COM represented trigger type</returns>
        private static _TASK_TRIGGER_TYPE2 MapTriggerType(TriggerType type)
        {
            _TASK_TRIGGER_TYPE2 triggerType = _TASK_TRIGGER_TYPE2.TASK_TRIGGER_TIME; // default is one time/once

            switch (type)
            {
                case TriggerType.Daily:
                    triggerType = _TASK_TRIGGER_TYPE2.TASK_TRIGGER_DAILY;
                    break;
                case TriggerType.Weekly:
                    triggerType = _TASK_TRIGGER_TYPE2.TASK_TRIGGER_WEEKLY;
                    break;
                case TriggerType.Monthly:
                    triggerType = _TASK_TRIGGER_TYPE2.TASK_TRIGGER_MONTHLY;
                    break;
                case TriggerType.Once:
                default:
                    triggerType = _TASK_TRIGGER_TYPE2.TASK_TRIGGER_TIME;
                    break;
            }

            return triggerType;
        }

        /// <summary>
        /// Maps the type of an action from business entity to COM representation
        /// </summary>
        /// <param name="type">Action Type</param>
        /// <returns>COM represented action type</returns>
        private static _TASK_ACTION_TYPE MapActionType(ActionType type)
        {
            _TASK_ACTION_TYPE actionType = _TASK_ACTION_TYPE.TASK_ACTION_EXEC; // default is one time/once

            switch (type)
            {
                case ActionType.DisplayMessage:
                    actionType = _TASK_ACTION_TYPE.TASK_ACTION_SHOW_MESSAGE;
                    break;
                case ActionType.SendEmail:
                    actionType = _TASK_ACTION_TYPE.TASK_ACTION_SEND_EMAIL;
                    break;
                case ActionType.StartProgram:
                default:
                    actionType = _TASK_ACTION_TYPE.TASK_ACTION_EXEC;
                    break;
            }

            return actionType;
        }

        /// <summary>
        /// Gets the scheduled task data recursively contained in a folder and all sub-folders
        /// </summary>
        /// <param name="folder">Parent Folder containing the tasks</param>
        /// <param name="tasks">List of Tasks to be populated</param>
        private static void GetData(ITaskFolder folder, List<ScheduledTask> tasks)
        {
            // 1 for getting Hidden Tasks as well
            foreach (IRegisteredTask task in folder.GetTasks(1))
            {
                tasks.Add(TransformToTask(task));

                ReleaseComObject(task);
                
                // 1 for getting Hidden Tasks as well
                foreach (ITaskFolder subFolder in folder.GetFolders(1))
                {
                    GetData(subFolder, tasks);
                }
            }

            // Release handle
            ReleaseComObject(folder);
        }

        /// <summary>
        /// Converts the Scheduled Task COM representation to Business Entity
        /// </summary>
        /// <param name="task">COM Scheduled Task</param>
        /// <returns>An instance of Scheduled Task</returns>
        private static ScheduledTask TransformToTask(IRegisteredTask task)
        {
            ScheduledTask scheduledTask = new ScheduledTask();
            scheduledTask.Name = task.Name;
            scheduledTask.CurrentState = task.State.ToString();
            scheduledTask.TaskPath = task.Path;
            scheduledTask.NextRunTime = task.NextRunTime;
            scheduledTask.LastRunTime = task.LastRunTime;
            scheduledTask.LastRunResults = task.LastTaskResult.ToString();
            
            ITaskDefinition definition = task.Definition;

            ITaskSettings settings = definition.Settings;
            scheduledTask.Priority = settings.Priority;

            scheduledTask.Settings = new Settings();

            scheduledTask.Settings.Enabled = settings.Enabled;
            scheduledTask.Settings.Hidden = settings.Hidden;

            IRegistrationInfo registrationInfo = definition.RegistrationInfo;
            scheduledTask.Description = registrationInfo.Description;

            scheduledTask.Author = registrationInfo.Author;

            return scheduledTask;
        }

        /// <summary>
        /// Creates triggers
        /// </summary>
        /// <param name="taskDefinition">Task Definition to which triggers should be added</param>
        /// <param name="task">Task containing trigger information</param>
        private static void ConfigureTriggers(ITaskDefinition taskDefinition, ScheduledTask task)
        {
            if (task.Triggers != null)
            {
                ITriggerCollection triggers = taskDefinition.Triggers;

                foreach (ScheduleTriggerInfo triggerInfo in task.Triggers)
                {
                    ITrigger trigger = triggers.Create(MapTriggerType(triggerInfo.Type));
                    trigger.Enabled = triggerInfo.Enabled;
                    trigger.StartBoundary = triggerInfo.StartTime.ToString(Constants.DateTimeFormatExpectedByCOM);

                    if (triggerInfo.DoesExpire)
                    {
                        trigger.EndBoundary = triggerInfo.EndTime.ToString(Constants.DateTimeFormatExpectedByCOM);
                    }
                }
            }
        }

        /// <summary>
        /// Creates Actions
        /// </summary>
        /// <param name="taskDefinition">Task Definition to which triggers should be added</param>
        /// <param name="task">Task containing trigger information</param>
        private static void ConfigureActions(ITaskDefinition taskDefinition, ScheduledTask task)
        {
            IActionCollection actions = taskDefinition.Actions;

            if (task.Actions != null)
            {
                foreach (Entities.Action actionInfo in task.Actions)
                {
                    _TASK_ACTION_TYPE actionType = MapActionType(actionInfo.Type);
                    IAction action = actions.Create(actionType);

                    switch (actionType)
                    {
                        case _TASK_ACTION_TYPE.TASK_ACTION_EXEC:
                            ConfigureExecAction(action, actionInfo);
                            break;

                        case _TASK_ACTION_TYPE.TASK_ACTION_SEND_EMAIL:
                            ConfigureEmailAction(action, actionInfo);
                            break;

                        case _TASK_ACTION_TYPE.TASK_ACTION_SHOW_MESSAGE:
                            ConfigureShowMessageAction(action, actionInfo);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Configures Executable Action
        /// </summary>
        /// <param name="action">Task Action to be configured</param>
        /// <param name="actionInfo">Object containing the information to be configured</param>
        private static void ConfigureExecAction(IAction action, Entities.Action actionInfo)
        {
            IExecAction execAction = action as IExecAction;
            StartProgramAction progAction = actionInfo as StartProgramAction;

            execAction.Path = progAction.ProgramToRun;
            execAction.Arguments = progAction.Arguments;
        }

        /// <summary>
        /// Configures Email Action
        /// </summary>
        /// <param name="action">Task Action to be configured</param>
        /// <param name="actionInfo">Object containing the information to be configured</param>
        private static void ConfigureEmailAction(IAction action, Entities.Action actionInfo)
        {
            IEmailAction mailAction = action as IEmailAction;
            SendEmailAction emailAction = actionInfo as SendEmailAction;

            mailAction.From = emailAction.From;
            mailAction.To = emailAction.To;
            mailAction.Server = emailAction.SMTPServer;
            mailAction.Subject = emailAction.Subject;
            mailAction.Body = emailAction.MessageBody;
        }

        /// <summary>
        /// Configures Show Message Action
        /// </summary>
        /// <param name="action">Task Action to be configured</param>
        /// <param name="actionInfo">Object containing the information to be configured</param>
        private static void ConfigureShowMessageAction(IAction action, Entities.Action actionInfo)
        {
            IShowMessageAction showAction = action as IShowMessageAction;
            DisplayMessageAction displayMessageAction = actionInfo as DisplayMessageAction;

            showAction.Title = displayMessageAction.DisplayTitle;
            showAction.MessageBody = displayMessageAction.DisplayMessage;
        }

        /// <summary>
        /// Converts minutes expressed as integer into COM Timespan representation
        /// </summary>
        /// <param name="minutes">No of Minutes</param>
        /// <returns>String representation of Timespan Minutes</returns>
        private static string ConvertToCOMTimeSpanRepresentation(int minutes)
        {
            TimeSpan timespan = TimeSpan.FromMinutes(minutes);

            // this is the format needed by COM
            return XmlConvert.ToString(timespan);
        }

        /// <summary>
        /// Release the COM object
        /// </summary>
        /// <param name="comObject">Object to be released</param>
        private static void ReleaseComObject(object comObject)
        {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(comObject);
        }

        /// <summary>
        /// Converts the Business Entity to COM Representation of ScheduledTask
        /// </summary>
        /// <param name="task">Scheduled task Business Entity</param>
        /// <returns>COM entity of a scheduled Task</returns>
        private ITaskDefinition TransformToRegisteredTask(ScheduledTask task)
        {
            ITaskDefinition taskDefinition = this.taskService.NewTask(0);
            taskDefinition.RegistrationInfo.Description = task.Description;
            taskDefinition.RegistrationInfo.Author = task.Author;

            taskDefinition.Settings.Enabled = task.Settings.Enabled;
            taskDefinition.Settings.Hidden = task.Settings.Hidden;
            taskDefinition.Settings.Compatibility = _TASK_COMPATIBILITY.TASK_COMPATIBILITY_V2_1;

            if (task.Settings.StopTaskWhenRunsLonger)
            {
                // this is the format needed by COM
                taskDefinition.Settings.ExecutionTimeLimit = ConvertToCOMTimeSpanRepresentation(task.Settings.ExecutionTimeLimit); 
            }

            if (task.Conditions.RunOnlyIfIdle)
            {
                taskDefinition.Settings.RunOnlyIfIdle = task.Conditions.RunOnlyIfIdle;

                // this is the format needed by COM
                taskDefinition.Settings.IdleSettings.IdleDuration = ConvertToCOMTimeSpanRepresentation(task.Conditions.IdleDurationToStartTaskWhenSystemIsIdle);
            }

            // ADD Triggers
            ConfigureTriggers(taskDefinition, task);

            // ADD Actions
            ConfigureActions(taskDefinition, task);

            return taskDefinition;
        }

        /// <summary>
        /// Connects a task Service
        /// </summary>
        private void ConnectTaskSchedulerService()
        {
            try
            {
                if (!this.taskService.Connected)
                {
                    this.taskService.Connect();
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(Constants.TaskServiceConnectionError, exception);

                throw;
            }
        }

        /// <summary>
        /// Disposes the resources
        /// </summary>
        /// <param name="disposing">True indicates a call to Dispose is made to clean asap
        /// otherwise from the finalizer(destructor) when GC is reclaiming unused memory </param>
        private void Dispose(bool disposing)
        {
            lock (this)
            {
                if (!this.isDisposed)
                {
                    ReleaseComObject(this.taskService);

                    if (disposing)
                    {
                        // dispose managed resources here
                        this.taskService = null;
                    }

                    this.isDisposed = true;
                }
            }
        }
        
        #endregion
    }
}
