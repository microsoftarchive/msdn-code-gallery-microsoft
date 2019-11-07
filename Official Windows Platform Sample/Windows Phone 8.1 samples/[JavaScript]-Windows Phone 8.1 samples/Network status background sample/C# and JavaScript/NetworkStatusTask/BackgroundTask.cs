// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.Networking.Connectivity;
using System.IO;

//
// The namespace for the background tasks.
//
namespace NetworkStatusTask
{
    //
    // A background task always implements the IBackgroundTask interface.
    //
    public sealed class NetworkStatusBackgroundTask : IBackgroundTask
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        //
        // The Run method is the entry point of a background task.
        //
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("Background " + taskInstance.Task.Name + " Starting...");

            //
            // Associate a cancellation handler with the background task.
            //
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            try
            {
                NetworkStateChangeEventDetails details = taskInstance.TriggerDetails as NetworkStateChangeEventDetails;

                localSettings.Values["HasNewConnectionCost"] = details.HasNewConnectionCost ? "New Connection Cost" : null;
                localSettings.Values["HasNewDomainConnectivityLevel"] = details.HasNewDomainConnectivityLevel ? "New Domain Connectivity Level" : null;
                localSettings.Values["HasNewHostNameList"] = details.HasNewHostNameList ? "New HostName List" : "";
                localSettings.Values["HasNewInternetConnectionProfile"] = details.HasNewInternetConnectionProfile ? "New Internet Connection Profile" : null;
                localSettings.Values["HasNewNetworkConnectivityLevel"] = details.HasNewNetworkConnectivityLevel ? "New Network Connectivity Level" : null;

                ConnectionProfile profile = NetworkInformation.GetInternetConnectionProfile();
                if (profile == null)
                {
                    localSettings.Values["InternetProfile"] = "Not connected to Internet";
                    localSettings.Values["NetworkAdapterId"] = "Not connected to Internet";
                }
                else
                {
                    localSettings.Values["InternetProfile"] = profile.ProfileName;

                    var networkAdapterInfo = profile.NetworkAdapter;
                    if (networkAdapterInfo == null)
                    {
                        localSettings.Values["NetworkAdapterId"] = "Not connected to Internet";
                    }
                    else
                    {
                        localSettings.Values["NetworkAdapterId"] = networkAdapterInfo.NetworkAdapterId.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unhandled exception: " + e.ToString());
            }
        }

        //
        // Handles background task cancellation.
        //
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //
            // Indicate that the background task is canceled.
            //
            Debug.WriteLine("Background " + sender.Task.Name + " Cancel Requested...");
        }
    }
}
