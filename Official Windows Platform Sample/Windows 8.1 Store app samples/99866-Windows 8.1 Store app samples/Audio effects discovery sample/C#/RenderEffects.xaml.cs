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
    public sealed partial class RenderEffects : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        private AudioRenderEffectsManager m_RenderEffectsManager = null;
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

        public RenderEffects()
        {
            this.InitializeComponent();
            ScenarioInit();
        }

        private void ScenarioInit()
        {
            m_MonitorStarted = false;
            m_DeviceInfo = null;
            m_RenderEffectsManager = null;
            DisplayEmptyDevicesList();
            DisplayEmptyEffectsList();
            DisplayCategoriesList();
            lblEffects.DataContext = new EffectLabelView { EffectLabelName = "Effects" };
        }

        private void ScenarioClose()
        {
            if (m_RenderEffectsManager != null)
            {
                m_RenderEffectsManager.AudioRenderEffectsChanged -= OnRenderEffectsChanged;
                m_RenderEffectsManager = null;
            }

            CategoriesList.LayoutUpdated -= CategoriesList_LayoutUpdated;
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

        private void OnRenderEffectsChanged(AudioRenderEffectsManager sender, object evt)
        {
            var ignored = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                IReadOnlyList<AudioEffect> effectslist = m_RenderEffectsManager.GetAudioRenderEffects();
                DisplayEffectsList(effectslist);
            });
        }

        private void StartStopMonitor(Object sender, Windows.UI.Xaml.RoutedEventArgs evt)
        {
            if ( m_MonitorStarted == true )
            {
                if (m_RenderEffectsManager != null)
                {
                    m_RenderEffectsManager.AudioRenderEffectsChanged -= OnRenderEffectsChanged;
                    m_RenderEffectsManager = null;
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
                AudioRenderCategory Category;

                if ( CategoriesList.SelectedIndex < 0 )
                {
                    Category = AudioRenderCategory.Communications;
                }
                else
                {
                    Category = (AudioRenderCategory)CategoriesList.SelectedIndex;
                }

                if (deviceIndex < 0)
                {
                    m_RenderEffectsManager = AudioEffectsManager.CreateAudioRenderEffectsManager(
                       MediaDevice.GetDefaultAudioRenderId(AudioDeviceRole.Communications),
                       Category,
                       Windows.Media.AudioProcessing.Default);
                    m_RenderEffectsManager.AudioRenderEffectsChanged += OnRenderEffectsChanged;
                    lblEffects.DataContext = new EffectLabelView { EffectLabelName = "Effects Active on {Default Device}" };
                }
                else
                {
                    m_RenderEffectsManager = AudioEffectsManager.CreateAudioRenderEffectsManager(
                        m_DeviceInfo[deviceIndex].Id,
                        Category,
                        Windows.Media.AudioProcessing.Default);
                    m_RenderEffectsManager.AudioRenderEffectsChanged += OnRenderEffectsChanged;
                    lblEffects.DataContext = new EffectLabelView { EffectLabelName = "Effects Active on  {" + m_DeviceInfo[deviceIndex].Name + "}" };
                }

                // Display current effects once
                IReadOnlyList<AudioEffect> Effectslist = m_RenderEffectsManager.GetAudioRenderEffects();
                DisplayEffectsList(Effectslist);

                // Start monitoring effects changes; for now disable button for "Refresh Effects List"
                btnRefreshEffects.IsEnabled = false;
                m_MonitorStarted = true;
                btnStartStopMonitor.Content = "Stop Monitoring";
            }
        }

        private void RefreshList(Object sender, Windows.UI.Xaml.RoutedEventArgs evt)
        {
            AudioRenderEffectsManager RenderEffectsManagerLocal;
            IReadOnlyList<AudioEffect> Effectslist;
            int deviceIndex = DevicesList.SelectedIndex;
            AudioRenderCategory Category;

            if (CategoriesList.SelectedIndex < 0)
            {
                Category = AudioRenderCategory.Communications;
            }
            else
            {
                Category = (AudioRenderCategory)CategoriesList.SelectedIndex;
            }

            if (deviceIndex < 0)
            {
                RenderEffectsManagerLocal = AudioEffectsManager.CreateAudioRenderEffectsManager(
                    MediaDevice.GetDefaultAudioRenderId(AudioDeviceRole.Communications),
                    Category,
                    Windows.Media.AudioProcessing.Default);
                lblEffects.DataContext = new EffectLabelView { EffectLabelName = "Effects Active on {Default Device}" };
            }
            else
            {
                RenderEffectsManagerLocal = AudioEffectsManager.CreateAudioRenderEffectsManager(
                    m_DeviceInfo[deviceIndex].Id,
                    Category,
                    Windows.Media.AudioProcessing.Default);
                lblEffects.DataContext = new EffectLabelView { EffectLabelName = "Effects Active on  {" + m_DeviceInfo[deviceIndex].Name + "}" };
            }

            Effectslist = RenderEffectsManagerLocal.GetAudioRenderEffects();
            DisplayEffectsList(Effectslist);
        }

        private void DisplayCategoriesList()
        {
            var noteList = new ObservableCollection<CategoryView>();

            for (var i = AudioRenderCategory.Other; i <= AudioRenderCategory.GameMedia; i++)
            {
                noteList.Add(new CategoryView { CategoryName = i.ToString() });
            }

            CategoriesList.ItemsSource = noteList;

            CategoriesList.LayoutUpdated += CategoriesList_LayoutUpdated;
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

        private void DisplayEffectsList ( IReadOnlyList<AudioEffect> effectslist )
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

        private async void EnumerateDevices(Object sender, Windows.UI.Xaml.RoutedEventArgs evt)
        {
            string AudioSelector = MediaDevice.GetAudioRenderSelector();
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

        private void CategoriesList_LayoutUpdated(object sender, object e)
        {
            // This CS sample does not include "Audio" in "Background Tasks" declaration; so disable the "BackgroundCapableMedia" category from drop-down list
            ComboBoxItem cbItem = (ComboBoxItem)CategoriesList.ContainerFromIndex((int)AudioRenderCategory.BackgroundCapableMedia);

            // Perform disabling only after ComboBoxItem has been rendered
            if (cbItem != null)
            {
                cbItem.IsEnabled = false;
            }
        }

    }

}