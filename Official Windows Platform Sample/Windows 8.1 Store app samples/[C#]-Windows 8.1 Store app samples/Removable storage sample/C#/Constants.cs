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
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public const string FEATURE_NAME = "Removable Storage";

        public List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "List removable storage devices", ClassType = typeof(Microsoft.Samples.Devices.Portable.RemovableStorageSample.S1_ListStorages) },
            new Scenario() { Title = "Send file to storage device", ClassType = typeof(Microsoft.Samples.Devices.Portable.RemovableStorageSample.S2_SendToStorage) },
            new Scenario() { Title = "Get image file from storage device", ClassType = typeof(Microsoft.Samples.Devices.Portable.RemovableStorageSample.S3_GetFromStorage) },
            new Scenario() { Title = "Get image file from camera or camera memory (Autoplay)", ClassType = typeof(Microsoft.Samples.Devices.Portable.RemovableStorageSample.S4_Autoplay) },
        };

        public const int autoplayScenarioIndex = 3;

        // Contains the list of Windows.Storage.StorageItem's provided when this application is activated to handle
        // the supported file types specified in the manifest (here, these will be image files).
        public IReadOnlyList<IStorageItem> FileActivationFiles { get; set; }

        // Contains the storage folder (representing a file-system removable storage) when this application is activated by Content Autoplay
        public StorageFolder AutoplayFileSystemDeviceFolder { get; set; }

        // Contains the device identifier (representing a non-file system removable storage) provided when this application
        // is activated by Device Autoplay
        public string AutoplayNonFileSystemDeviceId { get; set; }

        // Selects and loads the Autoplay scenario
        public void LoadAutoplayScenario()
        {
            LoadScenario(scenarios[MainPage.autoplayScenarioIndex].ClassType);
            Scenarios.SelectedIndex = MainPage.autoplayScenarioIndex;
        }
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

    public partial class App : Application
    {
        /// <summary>
        /// Invoked when the application is launched to open a specific file or to access
        /// specific content. This is the entry point for Content Autoplay when camera
        /// memory is attached to the PC.
        /// </summary>
        /// <param name="args">Details about the file activation request.</param>
        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            Frame rootFrame = null;
            if (Window.Current.Content == null)
            {
                rootFrame = new Frame();
                rootFrame.Navigate(typeof(MainPage));
                Window.Current.Content = rootFrame;
            }
            else
            {
                rootFrame = Window.Current.Content as Frame;
            }
            Window.Current.Activate();
            MainPage mainPage = (MainPage)rootFrame.Content as MainPage;

            // Clear any device id so we always use the latest connected device
            mainPage.AutoplayNonFileSystemDeviceId = null;

            if (args.Verb == "storageDevice")
            {
                // Launched from Autoplay for content. This will return a single storage folder
                // representing that file system device.
                mainPage.AutoplayFileSystemDeviceFolder = args.Files[0] as StorageFolder;
                mainPage.FileActivationFiles = null;
            }
            else
            {
                // Launched to handle a file type.  This will return a list of image files that the user
                // requests for this application to handle.
                mainPage.FileActivationFiles = args.Files;
                mainPage.AutoplayFileSystemDeviceFolder = null;
            }

            // Select the Autoplay scenario
            mainPage.LoadAutoplayScenario();
        }

        /// <summary>
        /// Invoked when the application is activated.
        /// This is the entry point for Device Autoplay when a device is attached to the PC.
        /// Other activation kinds (such as search and protocol activation) may also be handled here.
        /// </summary>
        /// <param name="args">Details about the activation request.</param>
        protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.Device)
            {
                Frame rootFrame = null;
                if (Window.Current.Content == null)
                {
                    rootFrame = new Frame();
                    rootFrame.Navigate(typeof(MainPage));
                    Window.Current.Content = rootFrame;
                }
                else
                {
                    rootFrame = Window.Current.Content as Frame;
                }
                Window.Current.Activate();
                MainPage mainPage = (MainPage)rootFrame.Content as MainPage;

                // Launched from Autoplay for device, receiving the device information identifier.
                DeviceActivatedEventArgs deviceArgs = args as DeviceActivatedEventArgs;
                mainPage.AutoplayNonFileSystemDeviceId = deviceArgs.DeviceInformationId;

                // Clear any saved drive or file so we always use the latest connected device
                mainPage.AutoplayFileSystemDeviceFolder = null;
                mainPage.FileActivationFiles = null;

                // Select the Autoplay scenario
                mainPage.LoadAutoplayScenario();
            }
        }
    }
}
