//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;using System;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace SDKTemplate
{

    public partial class MainPage : Page
    {
        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "Background Task";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Background task", ClassType = typeof(BackgroundTask.SampleBackgroundTask) },
            new Scenario() { Title = "Background task with a condition", ClassType = typeof(BackgroundTask.SampleBackgroundTaskWithCondition) },
            new Scenario() { Title = "Servicing complete task", ClassType = typeof(BackgroundTask.ServicingCompleteTask) },
            new Scenario() { Title = "Background task with time trigger", ClassType = typeof(BackgroundTask.TimeTriggeredTask) }
        };
    }

    public class Scenario
    {
        public string Title { get; set; }

        public Type ClassType { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}

namespace BackgroundTask
{
    class BackgroundTaskSample
    {
        public const string SampleBackgroundTaskEntryPoint = "Tasks.SampleBackgroundTask";
        public const string SampleBackgroundTaskName = "SampleBackgroundTask";
        public static string SampleBackgroundTaskProgress = "";
        public static bool SampleBackgroundTaskRegistered = false;

        public const string SampleBackgroundTaskWithConditionName = "SampleBackgroundTaskWithCondition";
        public static string SampleBackgroundTaskWithConditionProgress = "";
        public static bool SampleBackgroundTaskWithConditionRegistered = false;

        public const string ServicingCompleteTaskEntryPoint = "Tasks.ServicingComplete";
        public const string ServicingCompleteTaskName = "ServicingCompleteTask";
        public static string ServicingCompleteTaskProgress = "";
        public static bool ServicingCompleteTaskRegistered = false;

        public const string TimeTriggeredTaskName = "TimeTriggeredTask";
        public static string TimeTriggeredTaskProgress = "";
        public static bool TimeTriggeredTaskRegistered = false;

        /// <summary>
        /// Register a background task with the specified taskEntryPoint, name, trigger,
        /// and condition (optional).
        /// </summary>
        /// <param name="taskEntryPoint">Task entry point for the background task.</param>
        /// <param name="name">A name for the background task.</param>
        /// <param name="trigger">The trigger for the background task.</param>
        /// <param name="condition">An optional conditional event that must be true for the task to fire.</param>
        public static async Task<BackgroundTaskRegistration> RegisterBackgroundTask(String taskEntryPoint, String name, IBackgroundTrigger trigger, IBackgroundCondition condition)
        {
            if (TaskRequiresBackgroundAccess(name))
            {
                await BackgroundExecutionManager.RequestAccessAsync();
            }

            var builder = new BackgroundTaskBuilder();

            builder.Name = name;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(trigger);

            if (condition != null)
            {
                builder.AddCondition(condition);

                //
                // If the condition changes while the background task is executing then it will
                // be canceled.
                //
                builder.CancelOnConditionLoss = true;
            }

            BackgroundTaskRegistration task = builder.Register();

            UpdateBackgroundTaskStatus(name, true);

            //
            // Remove previous completion status from local settings.
            //
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values.Remove(name);

            return task;
        }

        /// <summary>
        /// Unregister background tasks with specified name.
        /// </summary>
        /// <param name="name">Name of the background task to unregister.</param>
        public static void UnregisterBackgroundTasks(string name)
        {
            //
            // Loop through all background tasks and unregister any with SampleBackgroundTaskName or
            // SampleBackgroundTaskWithConditionName.
            //
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == name)
                {
                    cur.Value.Unregister(true);
                }
            }

            UpdateBackgroundTaskStatus(name, false);
        }

        /// <summary>
        /// Store the registration status of a background task with a given name.
        /// </summary>
        /// <param name="name">Name of background task to store registration status for.</param>
        /// <param name="registered">TRUE if registered, FALSE if unregistered.</param>
        public static void UpdateBackgroundTaskStatus(String name, bool registered)
        {
            switch (name)
            {
                case SampleBackgroundTaskName:
                    SampleBackgroundTaskRegistered = registered;
                    break;
                case SampleBackgroundTaskWithConditionName:
                    SampleBackgroundTaskWithConditionRegistered = registered;
                    break;
                case ServicingCompleteTaskName:
                    ServicingCompleteTaskRegistered = registered;
                    break;
                case TimeTriggeredTaskName:
                    TimeTriggeredTaskRegistered = registered;
                    break;
            }
        }

        /// <summary>
        /// Get the registration / completion status of the background task with
        /// given name.
        /// </summary>
        /// <param name="name">Name of background task to retreive registration status.</param>
        public static String GetBackgroundTaskStatus(String name)
        {
            var registered = false;
            switch (name)
            {
                case SampleBackgroundTaskName:
                    registered = SampleBackgroundTaskRegistered;
                    break;
                case SampleBackgroundTaskWithConditionName:
                    registered = SampleBackgroundTaskWithConditionRegistered;
                    break;
                case ServicingCompleteTaskName:
                    registered = ServicingCompleteTaskRegistered;
                    break;
                case TimeTriggeredTaskName:
                    registered = TimeTriggeredTaskRegistered;
                    break;
            }

            var status = registered ? "Registered" : "Unregistered";

            var settings = ApplicationData.Current.LocalSettings;
            if (settings.Values.ContainsKey(name))
            {
                status += " - " + settings.Values[name].ToString();
            }

            return status;
        }

        /// <summary>
        /// Determine if task with given name requires background access.
        /// </summary>
        /// <param name="name">Name of background task to query background access requirement.</param>
        public static bool TaskRequiresBackgroundAccess(String name)
        {
#if WINDOWS_PHONE_APP
            return true;
#else
            if (name == TimeTriggeredTaskName)
            {
                return true;
            }
            else
            {
                return false;
            }
#endif
        }
    }
}
