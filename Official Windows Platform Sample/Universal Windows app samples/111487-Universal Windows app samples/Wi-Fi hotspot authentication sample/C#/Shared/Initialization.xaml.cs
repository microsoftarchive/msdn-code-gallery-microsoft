//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Networking.NetworkOperators;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using HotspotAuthenticationTask;

namespace HotspotAuthenticationApp
{
    public sealed partial class Initialization : Page
    {
        // A pointer back to the main page.  This is needed to call methods in MainPage such as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Initialization()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

#if WINDOWS_PHONE_APP
            //For windows phone we wont be doing Native WISPr authentication because it is not supported
            ConfigStore.UseNativeWISPr = false;
#else
            ConfigStore.UseNativeWISPr = true;
#endif
            // Configure background task handler to perform authentication as default
            ConfigStore.AuthenticateThroughBackgroundTask = true;

            // Setup completion handler
            var isTaskRegistered = ScenarioCommon.Instance.RegisteredCompletionHandlerForBackgroundTask();

            // Initialize button state
            UpdateButtonState(isTaskRegistered);
        }

        /// <summary>
        /// This is the click handler for the 'Provision' button to provision the embedded XML file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ProvisionButton_Click(object sender, RoutedEventArgs args)
        {
            ProvisionButton.IsEnabled = false;

            try
            {
                // Open the installation folder
                var installLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;

                // Access the provisioning file
                var provisioningFile = await installLocation.GetFileAsync("ProvisioningData.xml");

                // Load with XML parser
                var xmlDocument = await XmlDocument.LoadFromFileAsync(provisioningFile);

                // Get raw XML
                var provisioningXml = xmlDocument.GetXml();

                // Create ProvisiongAgent Object
                var provisioningAgent = new ProvisioningAgent();

                // Create ProvisionFromXmlDocumentResults Object
                var result = await provisioningAgent.ProvisionFromXmlDocumentAsync(provisioningXml);

                if (result.AllElementsProvisioned)
                {
                    rootPage.NotifyUser("Provisioning was successful", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("Provisioning result: " + result.ProvisionResultsXml, NotifyType.StatusMessage);
                }
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }

            ProvisionButton.IsEnabled = true;
        }

        /// <summary>
        /// This is the click handler for the 'Register' button to registers a background task for
        /// the NetworkOperatorHotspotAuthentication event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegisterButton_Click(object sender, RoutedEventArgs args)
        {
            try
            {

#if WINDOWS_PHONE_APP
                // For windows phone, we need to call RequestAccessAsync always to enable Background Task to be launched even when 
                // screen is locked
                BackgroundAccessStatus status = BackgroundExecutionManager.RequestAccessAsync().AsTask().GetAwaiter().GetResult();

                if (status == BackgroundAccessStatus.Denied)
                {
                    rootPage.NotifyUser("Access denied while Requesting Async Access", NotifyType.ErrorMessage);                    
                    return;
                }
#endif
                // Create a new background task builder.
                var taskBuilder = new BackgroundTaskBuilder();

                // Create a new NetworkOperatorHotspotAuthentication trigger.
                var trigger = new NetworkOperatorHotspotAuthenticationTrigger();

                // Associate the NetworkOperatorHotspotAuthentication trigger with the background task builder.
                taskBuilder.SetTrigger(trigger);

                // Specify the background task to run when the trigger fires.
                taskBuilder.TaskEntryPoint = ScenarioCommon.BackgroundTaskEntryPoint;

                // Name the background task.
                taskBuilder.Name = ScenarioCommon.BackgroundTaskName;

                // Register the background task.
                var task = taskBuilder.Register();

                // Associate progress and completed event handlers with the new background task.
                task.Completed += new BackgroundTaskCompletedEventHandler(ScenarioCommon.Instance.OnBackgroundTaskCompleted);

                UpdateButtonState(true);
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// This is the click handler for the 'Unregister' button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnregisterButton_Click(object sender, RoutedEventArgs args)
        {
            UnregisterBackgroundTask();
            UpdateButtonState(false);
        }

        /// <summary>
        /// Unregister background task
        /// </summary>
        /// <param name="name"></param>
        private void UnregisterBackgroundTask()
        {
            // Loop through all background tasks and unregister any.
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == ScenarioCommon.BackgroundTaskName)
                {
                    cur.Value.Unregister(true);
                }
            }
        }

        /// <summary>
        /// Update button state
        /// </summary>
        /// <param name="registered">True if background task is registered</param>
        private void UpdateButtonState(bool registered)
        {
            if (registered)
            {
                RegisterButton.IsEnabled = false;
                UnregisterButton.IsEnabled = true;
            }
            else
            {
                RegisterButton.IsEnabled = true;
                UnregisterButton.IsEnabled = false;
            }
        }
    }
}
