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
using System.Diagnostics;
using System.Collections.Generic;

using System.Collections.ObjectModel;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using Windows.Media.Capture;
using Windows.Media.Render;
using Windows.Media.Devices;
using Windows.Media.Effects;
using Windows.Devices.Enumeration;

namespace AudioEffects
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CaptureEffects : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        private AudioCaptureEffectsManager m_CaptureEffectsManager = null;
        private bool m_MonitorStarted = false;
        private DeviceInformationCollection m_DeviceInfo = null;

        // For populating the "Effects" listbox's contents
        public class EffectView
        {
            public string EffectName { get; set; }
            public string EffectNameColor { get; set; }
        }

        // For populating the "Devices" listbox's contents
        public class DeviceView
        {
            public string DeviceName { get; set; }
            public string DeviceNameColor { get; set; }
        }

        // For populating the "Category" listbox's contents
        public class CategoryView
        {
            public string CategoryName { get; set; }
        }

        public class EffectLabelView
        {
            public string EffectLabelName { get; set; }
        }

        public CaptureEffects()
        {
            this.InitializeComponent();
            ScenarioInit();
        }

        private void ScenarioInit()
        {
            m_MonitorStarted = false;
            m_DeviceInfo = null;
            m_CaptureEffectsManager = null;
            DisplayEmptyDevicesList();
            DisplayEmptyEffectsList();
            DisplayCategoriesList();
            lblEffects.DataContext = new EffectLabelView { EffectLabelName = "Effects" };
        }

        private void ScenarioClose()
        {
            if (m_CaptureEffectsManager != null)
            {
                m_CaptureEffectsManager.AudioCaptureEffectsChanged -= OnCaptureEffectsChanged;
                m_CaptureEffectsManager = null;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ScenarioClose();
        }

        private void ShowStatusMessage(String text)
        {
            rootPage.NotifyUser(text, NotifyType.StatusMessage);
        }

        private void ShowExceptionMessage(Exception ex)
        {
            rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
        }

        private async void Failed(Windows.Media.Capture.MediaCapture currentCaptureObject, MediaCaptureFailedEventArgs currentFailure)
        {
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    ShowStatusMessage("Fatal error" + currentFailure.Message);
                });
            }
            catch (Exception e)
            {
                ShowExceptionMessage(e);
            }
        }

        private void OnCaptureEffectsChanged(AudioCaptureEffectsManager sender, object evt)
        {
            var ignored = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                    IReadOnlyList<AudioEffect> effectslist = m_CaptureEffectsManager.GetAudioCaptureEffects();
                    DisplayEffectsList ( effectslist );
            });
        }

        private void StartStopMonitor(Object sender, Windows.UI.Xaml.RoutedEventArgs evt)
        {
            if ( m_MonitorStarted == true )
            {
                if (m_CaptureEffectsManager != null)
                {
                    m_CaptureEffectsManager.AudioCaptureEffectsChanged -= OnCaptureEffectsChanged;
                    m_CaptureEffectsManager = null;
                }
                // No longer monitoring effects changes; re-enable button for "Refresh Effects List"
                btnRefreshEffects.IsEnabled = true;
                // Reset effects list to empty
                lblEffects.DataContext = new EffectLabelView { EffectLabelName = "Effects" };
                DisplayEmptyEffectsList();
                m_MonitorStarted = false;
                btnStartStopMonitor.Content = "Start Monitoring";
            }
            else
            {
                int deviceIndex = DevicesList.SelectedIndex;
                MediaCategory Category;

                if ( CategoriesList.SelectedIndex < 0 )
                {
                    Category = MediaCategory.Communications;
                }
                else
                {
                    Category = (MediaCategory)CategoriesList.SelectedIndex;
                }

                if (deviceIndex < 0)
                {
                    m_CaptureEffectsManager = AudioEffectsManager.CreateAudioCaptureEffectsManager(
                       MediaDevice.GetDefaultAudioCaptureId(AudioDeviceRole.Communications),
                       Category,
                       Windows.Media.AudioProcessing.Default);
                    m_CaptureEffectsManager.AudioCaptureEffectsChanged += OnCaptureEffectsChanged;
                    lblEffects.DataContext = new EffectLabelView { EffectLabelName = "Effects Active on {Default Device}" };
                }
                else
                {
                    m_CaptureEffectsManager = AudioEffectsManager.CreateAudioCaptureEffectsManager(
                        m_DeviceInfo[deviceIndex].Id,
                        Category,
                        Windows.Media.AudioProcessing.Default);
                    m_CaptureEffectsManager.AudioCaptureEffectsChanged += OnCaptureEffectsChanged;
                    lblEffects.DataContext = new EffectLabelView { EffectLabelName = "Effects Active on  {" + m_DeviceInfo[deviceIndex].Name + "}" };
                }

                // Display current effects once
                IReadOnlyList<AudioEffect> Effectslist = m_CaptureEffectsManager.GetAudioCaptureEffects();
                DisplayEffectsList(Effectslist);

                // Start monitoring effects changes; for now disable button for "Refresh Effects List"
                btnRefreshEffects.IsEnabled = false;
                m_MonitorStarted = true;
                btnStartStopMonitor.Content = "Stop Monitoring";
            }
        }

        private void RefreshList(Object sender, Windows.UI.Xaml.RoutedEventArgs evt)
        {
            AudioCaptureEffectsManager CaptureEffectsManagerLocal;
            IReadOnlyList<AudioEffect> Effectslist;
            int deviceIndex = DevicesList.SelectedIndex;
            MediaCategory Category;

            if (CategoriesList.SelectedIndex < 0)
            {
                Category = MediaCategory.Communications;
            }
            else
            {
                Category = (MediaCategory)CategoriesList.SelectedIndex;
            }

            if (deviceIndex < 0)
            {
                CaptureEffectsManagerLocal = AudioEffectsManager.CreateAudioCaptureEffectsManager(
                    MediaDevice.GetDefaultAudioCaptureId(AudioDeviceRole.Communications),
                    Category,
                    Windows.Media.AudioProcessing.Default);
                lblEffects.DataContext = new EffectLabelView { EffectLabelName = "Effects Active on {Default Device}" };
            }
            else
            {
                CaptureEffectsManagerLocal = AudioEffectsManager.CreateAudioCaptureEffectsManager(
                    m_DeviceInfo[deviceIndex].Id,
                    Category,
                    Windows.Media.AudioProcessing.Default);
                lblEffects.DataContext = new EffectLabelView { EffectLabelName = "Effects Active on  {" + m_DeviceInfo[deviceIndex].Name + "}" };
            }

            Effectslist = CaptureEffectsManagerLocal.GetAudioCaptureEffects();
            DisplayEffectsList(Effectslist);
        }

        private void DisplayCategoriesList()
        {
            var noteList = new ObservableCollection<CategoryView>();

            for ( var i = MediaCategory.Other; i <= MediaCategory.Communications; i++ )
            {
                noteList.Add(new CategoryView { CategoryName = i.ToString() });
            }

            CategoriesList.ItemsSource = noteList;
        }

        private void DisplayEmptyEffectsList() {
            var noteList = new ObservableCollection<EffectView>();

            // Insert one element as filler
            noteList.Add(new EffectView { EffectName = "Click \"Refresh Effects List\" or \"Start Monitoring\" to display audio effects", EffectNameColor = "#707070" });

            EffectsList.ItemsSource = noteList;
            EffectsList.IsEnabled = false;
        }

        private void DisplayEmptyDevicesList() {
            var noteList = new ObservableCollection<DeviceView>();

            // Insert one element as filler
            noteList.Add(new DeviceView { DeviceName = "Click \"Enumerate Devices\" to display audio devices", DeviceNameColor = "#707070" });

            DevicesList.ItemsSource = noteList;
            DevicesList.IsEnabled = false;
        }

        private void DisplayEffectsList(IReadOnlyList<AudioEffect> effectslist)
        {
            var noteList = new ObservableCollection<EffectView>();

            if (effectslist.Count > 0)
            {
                for (int i = 0; i < effectslist.Count; i++)
                {
                    noteList.Add(new EffectView { EffectName = effectslist[i].AudioEffectType.ToString() });
                }
            }
            else
            {
                noteList.Add(new EffectView { EffectName = "[No Effects]" });
            }

            EffectsList.ItemsSource = noteList;
        }

        private async void EnumerateDevices (Object sender, Windows.UI.Xaml.RoutedEventArgs evt)
        {
            string AudioSelector = MediaDevice.GetAudioCaptureSelector();
            DeviceInformationCollection DeviceInfoCollection;

            DeviceInfoCollection = await DeviceInformation.FindAllAsync(AudioSelector);
            var noteList = new ObservableCollection<DeviceView>();

            m_DeviceInfo = DeviceInfoCollection;

            for ( int i = 0; i < DeviceInfoCollection.Count; i++ )
            {
                noteList.Add(new DeviceView { DeviceName = DeviceInfoCollection[i].Name });
            }

            DevicesList.ItemsSource = noteList;
            DevicesList.IsEnabled = true;
        }

    }

}