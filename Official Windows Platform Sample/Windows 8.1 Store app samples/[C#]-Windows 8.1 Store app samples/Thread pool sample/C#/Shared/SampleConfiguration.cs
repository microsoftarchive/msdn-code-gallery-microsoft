//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************


using System;
using Windows.UI.Xaml.Controls;using System.Collections.Generic;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.UI.Core;

namespace SDKTemplate
{
    public partial class MainPage : Page
    {
        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "ThreadPool";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Thread pool delay timer", ClassType = typeof(ThreadPool.DelayTimer) },
            new Scenario() { Title = "Thread pool periodic timer", ClassType = typeof(ThreadPool.PeriodicTimer) },
            new Scenario() { Title = "Thread pool work item", ClassType = typeof(ThreadPool.WorkItem) }
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

namespace ThreadPool
{
    public enum Status
    {
        Unregistered = 0,
        Started = 1,
        Canceled = 2,
        Completed = 3
    }

    class ThreadPoolSample
    {
        public static ThreadPoolTimer DelayTimer;
        public static DelayTimer DelayTimerScenario;
        public static ulong DelayTimerMilliseconds = 1000;
        public static string DelayTimerInfo = "";
        public static Status DelayTimerStatus = Status.Unregistered;

        public static ThreadPoolTimer PeriodicTimer;
        public static PeriodicTimer PeriodicTimerScenario;
        public static long PeriodicTimerCount = 0;
        public static ulong PeriodicTimerMilliseconds = 500;
        public static string PeriodicTimerInfo = "";
        public static Status PeriodicTimerStatus = Status.Unregistered;

        public static IAsyncAction ThreadPoolWorkItem;
        public static WorkItem WorkItemScenaioro;
        public static WorkItemPriority WorkItemPriority = WorkItemPriority.Normal;
        public static Status WorkItemStatus;
        public static int WorkItemSelectedIndex = 1;

        public static ulong ValidateTimeValue(string newString, ulong oldValue)
        {
            ulong result;
            if (ulong.TryParse(newString, out result))
            {
                return result;
            }
            else
            {
                return oldValue;
            }
        }
    }
}
