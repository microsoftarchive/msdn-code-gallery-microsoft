//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

using Windows.Devices.Enumeration;
using Windows.Devices.Enumeration.Pnp;

namespace DeviceEnumeration
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Containers : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Containers()
        {
            this.InitializeComponent();
        }
        
        async void EnumerateDeviceContainers(object sender, RoutedEventArgs eventArgs)
        {
            FocusState focusState = EnumerateContainersButton.FocusState;
            EnumerateContainersButton.IsEnabled = false;

            string[] properties = { "System.ItemNameDisplay", "System.Devices.ModelName", "System.Devices.Connected" };
            var containers = await PnpObject.FindAllAsync(PnpObjectType.DeviceContainer, properties);

            DeviceContainersOutputList.Items.Clear();
            rootPage.NotifyUser(containers.Count + " device container(s) found", NotifyType.StatusMessage);
            foreach (PnpObject container in containers)
            {
                DeviceContainersOutputList.Items.Add(new DisplayItem(container));
            }

            EnumerateContainersButton.IsEnabled = true;
            EnumerateContainersButton.Focus(focusState);
        }

        class DisplayItem
        {
            public DisplayItem(PnpObject container)
            {
                name = (string)container.Properties["System.ItemNameDisplay"];
                if (name == null || name.Length == 0)
                {
                    name = "*Unnamed*";
                }

                id = "Id: " + container.Id;
                properties += "Property store count is: " + container.Properties.Count + "\n";
                foreach (var prop in container.Properties)
                {
                    properties += prop.Key + " := " + prop.Value + "\n";
                }
            }         
            
            public string Id { get { return id; } }
            public string Name { get { return name; } }
            public string Properties { get { return properties; } }

            readonly string id;
            readonly string name;
            readonly string properties;
        }
        
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
