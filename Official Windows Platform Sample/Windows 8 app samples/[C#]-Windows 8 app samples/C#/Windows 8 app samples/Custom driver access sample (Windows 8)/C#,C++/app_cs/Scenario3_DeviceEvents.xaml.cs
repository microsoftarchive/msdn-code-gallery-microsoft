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

namespace CustomDeviceAccess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeviceEvents : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // True when change events are registered.  Used to unregister them when we leave the page.
        bool switchChangedEventsRegistered = false;

        // saved copy of the switch state, used to highlight which entries have changed
        bool[] previousSwitchValues = null;

        public DeviceEvents()
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
            ClearSwitchStateTable();
            UpdateRegisterButton();
            DeviceList.Current.DeviceClosing += Current_DeviceClosing;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (switchChangedEventsRegistered)
            {
                RegisterForSwitchStateChangedEvent(false);
            }
            DeviceList.Current.DeviceClosing -= Current_DeviceClosing;
        }

        private void deviceEventsGet_Click_1(object sender, RoutedEventArgs e)
        {
            bool[] switchStateArray;

            if (DeviceList.Current.Fx2Device == null)
            {
                rootPage.NotifyUser("Fx2 device not connected or accessible", NotifyType.ErrorMessage);
                return;
            }

            try
            {
                switchStateArray = DeviceList.Current.Fx2Device.SwitchState;
            }
            catch (Exception exception)
            {
                rootPage.NotifyUser(exception.ToString(), NotifyType.ErrorMessage);
                return;
            }

            UpdateSwitchStateTable(switchStateArray);
        }


        private void deviceEventsRegister_Click_1(object sender, RoutedEventArgs e)
        {
            if (DeviceList.Current.Fx2Device == null)
            {
                rootPage.NotifyUser("Fx2 device not connected or accessible", NotifyType.ErrorMessage);
                return;
            }

            RegisterForSwitchStateChangedEvent(!switchChangedEventsRegistered);
        }

        void Current_DeviceClosing(object sender, EventArgs e)
        {
            if (switchChangedEventsRegistered)
            {
                RegisterForSwitchStateChangedEvent(false);
            }
        }


        private void RegisterForSwitchStateChangedEvent(bool register)
        {
            if (register)
            {
                DeviceList.Current.Fx2Device.SwitchStateChanged += OnSwitchStateChangedEvent;
            }
            else
            {
                DeviceList.Current.Fx2Device.SwitchStateChanged -= OnSwitchStateChangedEvent;
            }

            switchChangedEventsRegistered = register;
            UpdateRegisterButton();
            ClearSwitchStateTable();
        }

        private void UpdateRegisterButton()
        {
            if (switchChangedEventsRegistered)
            {
                deviceEventsRegister.Content = "Unregister From Switch State Change Event";
            }
            else
            {
                deviceEventsRegister.Content = "Register For Switch State Change Event";
            }
        }

        private void OnSwitchStateChangedEvent(Samples.Devices.Fx2.Fx2Device sender, Samples.Devices.Fx2.SwitchStateChangedEventArgs eventArgs)
        {
            bool[] switchState;
            try
            {
                switchState = eventArgs.SwitchState;
            }
            catch (Exception e)
            {
                rootPage.NotifyUser("Error accessing Fx2 device:\n" + e.Message, NotifyType.ErrorMessage);
                ClearSwitchStateTable();
                return;
            }

            UpdateSwitchStateTable(switchState);
        }

        private void ClearSwitchStateTable()
        {
            deviceEventsSwitches.Inlines.Clear();
            previousSwitchValues = null;
        }

        private void UpdateSwitchStateTable(bool[] switchStateArray)
        {
            var output = deviceEventsSwitches;

            DeviceList.CreateBooleanTable(
                output.Inlines,
                switchStateArray,
                previousSwitchValues,
                "Switch Number",
                "Switch State",
                "off",
                "on"
                );
            previousSwitchValues = switchStateArray;
        }

    }
}
