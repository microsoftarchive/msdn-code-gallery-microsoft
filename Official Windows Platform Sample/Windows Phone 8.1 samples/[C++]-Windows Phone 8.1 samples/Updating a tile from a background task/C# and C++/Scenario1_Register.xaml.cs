// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace CustomTileFromBackground
{
    /// <summary>
    /// This scenario registers a XamlRenderingBackgroundTask that, for illustration purposes, will be invoked whenever the 
    /// user changes the TimeZone setting on the phone. In your scenarios, select a trigger type to best suit your needs.
    /// For more information on the trigger types you can use to invoke a background task, see http://msdn.microsoft.com/library/windows/apps/windows.applicationmodel.background.systemtriggertype.aspx
    /// </summary>
    public sealed partial class Scenario1 : Page
    {
        private MainPage rootPage;

        public Scenario1()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;

            BackgroundExecutionManager.RemoveAccess();
            var statusOperation = await BackgroundExecutionManager.RequestAccessAsync();
            switch (statusOperation)
            {
                case BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity:
                    break;

                case BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity:
                    break;

                case BackgroundAccessStatus.Denied:
                    rootPage.NotifyUser("Background access was Denied! Expected status: Allow*RealTimeConnectivity.",NotifyType.ErrorMessage);
                    break;

                case BackgroundAccessStatus.Unspecified:
                    rootPage.NotifyUser("Background access was unspecified! Expected status: Allow*RealTimeConnectivity.", NotifyType.ErrorMessage);
                    break;

                default:
                    rootPage.NotifyUser("Background access was default! Expected status: Allow*RealTimeConnectivity.", NotifyType.ErrorMessage);
                    break;
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Unregister all background tasks associated with this app. 
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                Debug.WriteLine(String.Format("name={0}, ID={1}", task.Value.Name, task.Value.TaskId));
                task.Value.Unregister(true);
            }

            // Register the background task.
            // To keep the memory footprint of the background task as low as possible, is has been implemented in a C++ Windows Runtime Component for Windows Phone.  
            // The memory footprint will be higher if written in C# and will cause out of memory exception on low-cost-tier devices which will terminate the background task.
            BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
            builder.Name = "AppTileUpdater";
            builder.TaskEntryPoint = "BackgroundTaskCX.AppTileUpdater";

            // The trigger used in this sample is the TimeZoneChange trigger. This is for illustration purposes.
            // In a real scenario, choose the trigger that meets your needs. 
            // Note: There are two ways to start the background task for testing purposes:
            // 1. Change the time zone setting so that the system time changes - this will cause a TimeZoneChange to fire
            // 2. Find the background task "AppTileUpdater" in the LifecycleEvents drop-down on the main toolbar and tap it.
            
            builder.SetTrigger(new SystemTrigger(SystemTriggerType.TimeZoneChange, false));
            var registration = builder.Register();

            rootPage.NotifyUser("The background task has been registered.\nTo test this scenario, change the timezone in the Settings of the phone.\nEach timezone change that causes the system time to change will cause the background task to fire.", NotifyType.StatusMessage);
        }

    }
}
