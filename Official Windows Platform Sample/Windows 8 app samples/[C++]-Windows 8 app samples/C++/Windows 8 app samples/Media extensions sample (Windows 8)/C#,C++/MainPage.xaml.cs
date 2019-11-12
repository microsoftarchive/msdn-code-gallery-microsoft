// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Media;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Documents;

using MediaExtensionsCS;

namespace SDKTemplateCS
{
    partial class MainPage : Page
    {
        #region Properties

        private Frame _scenariosFrame;

        public Frame ScenariosFrame
        {
            get { return _scenariosFrame; }
            set { _scenariosFrame = value; }
        }

        private Frame _inputFrame;

        public Frame InputFrame
        {
            get { return _inputFrame; }
            set { _inputFrame = value; }
        }

        private Frame _outputFrame;

        public Frame OutputFrame
        {
            get { return _outputFrame; }
            set { _outputFrame = value; }
        }

        private string _rootNamespace;

        public string RootNamespace
        {
            get { return _rootNamespace; }
            set { _rootNamespace = value; }
        }

        private MediaExtensionManager _extensionManager = new MediaExtensionManager();

        public MediaExtensionManager ExtensionManager
        {
            get { return _extensionManager; }
        }
        #endregion

        #region Events

        public event System.EventHandler InputFrameLoaded;
        public event System.EventHandler OutputFrameLoaded;

        #endregion

        public MainPage()
        {
            InitializeComponent();

            _scenariosFrame = ScenarioList;
            _inputFrame = ScenarioInput;
            _outputFrame = ScenarioOutput;

            SetFeatureName(FEATURE_NAME);

            Loaded += new RoutedEventHandler(MainPage_Loaded);
            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(MainPage_SizeChanged);
            DisplayProperties.LogicalDpiChanged += new DisplayPropertiesEventHandler(DisplayProperties_LogicalDpiChanged);

            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Figure out what resolution and orientation we are in and respond appropriately
            CheckResolutionAndViewState();

            // Load the ScenarioList page into the proper frame
            ScenarioList.Navigate(Type.GetType(_rootNamespace + ".ScenarioList"), this);
        }

        #region Resolution and orientation code

        void DisplayProperties_LogicalDpiChanged(object sender)
        {
            CheckResolutionAndViewState();
        }

        void CheckResolutionAndViewState()
        {
            VisualStateManager.GoToState(this, ApplicationView.Value.ToString() + DisplayProperties.ResolutionScale.ToString(), false);
        }

        void MainPage_SizeChanged(Object sender, Windows.UI.Core.WindowSizeChangedEventArgs args)
        {
            CheckResolutionAndViewState();
        }

        #endregion

        private void SetFeatureName(string str)
        {
            FeatureName.Text = str;
        }

        async void Footer_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(((HyperlinkButton)sender).Tag.ToString()));
        }

        public void NotifyUser(string strMessage, NotifyType type)
        {
            switch (type)
            {
                case NotifyType.StatusMessage:
                    StatusBlock.Style = Resources["StatusStyle"] as Style;
                    break;
                case NotifyType.ErrorMessage:
                    StatusBlock.Style = Resources["ErrorStyle"] as Style;
                    break;
            }
            StatusBlock.Text = strMessage;
        }

        public void DoNavigation(Type pageType, Frame frame)
        {
            frame.Navigate(pageType, this);
            if (pageType.Name.Contains("Input"))
            {
                // Raise InputFrameLoaded so downstream pages know that the input frame content has been loaded.
                if (InputFrameLoaded != null)
                {
                    InputFrameLoaded(this, new EventArgs());
                }
            }
            else
            {
                // Raise OutputFrameLoaded so downstream pages know that the output frame content has been loaded.
                if (OutputFrameLoaded != null)
                {
                    OutputFrameLoaded(this, new EventArgs());
                }
            }
        }

        //
        //  Open a single file picker [with fileTypeFilter].
        //  And then, call media.SetSource(picked file).
        //  If the file is successfully opened, VideoMediaOpened() will be called and call media.Play().
        //
        public async void PickSingleFileAndSet(string[] fileTypeFilter, params MediaElement[] mediaElements)
        {
            var currentState = Windows.UI.ViewManagement.ApplicationView.Value;
            if (currentState == Windows.UI.ViewManagement.ApplicationViewState.Snapped &&
                !Windows.UI.ViewManagement.ApplicationView.TryUnsnap())
            {
                // File picker cannot be used in the snapped state.
                NotifyUser("File picker cannot be used when application is in the snapped state.", NotifyType.StatusMessage);
                return;
            }

            CoreDispatcher dispatcher = Window.Current.Dispatcher;

            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            foreach (string filter in fileTypeFilter)
            {
                picker.FileTypeFilter.Add(filter);
            }
            StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                try
                {
                    var stream = await file.OpenAsync(FileAccessMode.Read);

                    for (int i = 0; i < mediaElements.Length; ++i)
                    {
                        MediaElement me = mediaElements[i];
                        me.Stop();
                        if (i + 1 < mediaElements.Length)
                        {
                            me.SetSource(stream.CloneStream(), file.ContentType);
                        }
                        else
                        {
                            me.SetSource(stream, file.ContentType);
                        }
                    }
                }
                catch (Exception ex)
                {
                    NotifyUser("Cannot open video file - error: " + ex.Message, NotifyType.ErrorMessage);
                }
            }
        }

        public void VideoOnError(Object obj, ExceptionRoutedEventArgs args)
        {
            NotifyUser("Cannot open video file - error: " + args.ErrorMessage, NotifyType.ErrorMessage);
        }
    }

    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };
}
