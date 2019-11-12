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
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Input;
using Windows.Media;

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

        private string _rootNamespace;

        public string RootNamespace
        {
            get { return _rootNamespace; }
            set { _rootNamespace = value; }
        }

        public bool IsFullscreen { set; get; }

        private Size _previousmediasize;

        public Size PreviousMediaSize
        {
            get { return _previousmediasize; }
            set { _previousmediasize = value; }
        }

        private Thickness _previousmediaelementmargin;
        private Brush _previousBGColor;
        private DispatcherTimer _timer;
        private bool _sliderpressed = false;

        public ObservableCollection<BindableStorageFile> MediaPlayList { set; get; }

        #endregion

        public MainPage()
        {
            InitializeComponent();

            MediaPlayList = new ObservableCollection<BindableStorageFile>();

            SetFeatureName(FEATURE_NAME);

            ScenariosList.SelectionChanged += new SelectionChangedEventHandler(ScenarioList_SelectionChanged);

            // Starting scenario is the first or based upon a previous selection
            ListBoxItem startingScenario = null;
            if (SuspensionManager.SessionState.ContainsKey("SelectedScenario"))
            {
                String selectedScenarioName = SuspensionManager.SessionState["SelectedScenario"] as string;
                startingScenario = this.FindName(selectedScenarioName) as ListBoxItem;
            }

            ScenariosList.SelectedItem = startingScenario != null ? startingScenario : Scenario1;

            Scenario3Init();

            Loaded += new RoutedEventHandler(MainPage_Loaded);
            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(MainPage_SizeChanged);
            DisplayProperties.LogicalDpiChanged += new DisplayPropertiesEventHandler(DisplayProperties_LogicalDpiChanged);

            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
        }

        #region Event Handlers

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Figure out what resolution and orientation we are in and respond appropriately
            CheckResolutionAndViewState();

            PreviousMediaSize = new Size();
            myMediaElement.MediaOpened += myMediaElement_MediaOpened;
            myMediaElement.MediaEnded += myMediaElement_MediaEnded;
            myMediaElement.MediaFailed += myMediaElement_MediaFailed;
            
            myMediaElement.CurrentStateChanged += myMediaElement_CurrentStateChanged;

            //For support Background Audio
            MediaControl.PlayPressed += MediaControl_PlayPressed;
            MediaControl.PausePressed += MediaControl_PausePressed;
            MediaControl.PlayPauseTogglePressed += MediaControl_PlayPauseTogglePressed;
            MediaControl.StopPressed += MediaControl_StopPressed;             

            timelineSlider.ValueChanged += timelineSlider_ValueChanged;

            PointerEventHandler pointerpressedhandler = new PointerEventHandler(slider_PointerEntered);
            timelineSlider.AddHandler(Control.PointerPressedEvent, pointerpressedhandler, true);

            PointerEventHandler pointerreleasedhandler = new PointerEventHandler(slider_PointerCaptureLost);
            timelineSlider.AddHandler(Control.PointerCaptureLostEvent, pointerreleasedhandler, true);
        }

        void myMediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (myPlaylist.SelectedIndex == myPlaylist.Items.Count - 1)
            {
                return;
            }
            else
            {
                if (myPlaylist.SelectedIndex + 1 < myPlaylist.Items.Count)
                {
                    myPlaylist.SelectedIndex++;
                }
            }
        }

        void MediaControl_StopPressed(object sender, object e)
        {
            myMediaElement.Stop();
        }

        async void MediaControl_PlayPauseTogglePressed(object sender, object e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (myMediaElement.CurrentState == MediaElementState.Playing)
                {
                    myMediaElement.Pause();
                }
                else
                {
                    myMediaElement.Play();
                }
            }

            );
        }
             
        void MediaControl_PlayPressed(object sender, object e)
        {
            myMediaElement.Play();
        }

        void MediaControl_PausePressed(object sender, object e)
        {
            myMediaElement.Pause();
        }

        void myMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            double absvalue = (int)Math.Round(myMediaElement.NaturalDuration.TimeSpan.TotalSeconds, MidpointRounding.AwayFromZero);
            timelineSlider.Maximum = absvalue;

            timelineSlider.StepFrequency = SliderFrequency(myMediaElement.NaturalDuration.TimeSpan);

            SetupTimer();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (myMediaElement.DefaultPlaybackRate == 0)
            {
                myMediaElement.DefaultPlaybackRate = 1.0;
                myMediaElement.PlaybackRate = 1.0;
            }

            SetupTimer();
            myMediaElement.Play();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            myMediaElement.Pause();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            myMediaElement.Stop();
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            myMediaElement.DefaultPlaybackRate = 0.0;
            myMediaElement.PlaybackRate = 2.0;
        }

        private void RewindButton_Click(object sender, RoutedEventArgs e)
        {
            myMediaElement.DefaultPlaybackRate = 0.0;
            myMediaElement.PlaybackRate = -1.0;
        }

        #endregion

        #region Marker Handling code

        private void MarkersButton_Click(object sender, RoutedEventArgs e)
        {
            TimelineMarker marker;
            marker = new TimelineMarker();

            marker.Text = "Marker Fired" + "\n";
            marker.Time = TimeSpan.FromSeconds(myMediaElement.Position.TotalSeconds + 2);
            myMediaElement.Markers.Add(marker);
            marker = new TimelineMarker();
            marker.Text = "Marker Fired" + "\n";
            marker.Time = TimeSpan.FromSeconds(myMediaElement.Position.TotalSeconds + 4);
            myMediaElement.Markers.Add(marker);
            Scenario3Text.Text += String.Format("Markers added at {0:g} and {1:g}",
                Math.Round(myMediaElement.Position.TotalSeconds + 2, 2), Math.Round(myMediaElement.Position.TotalSeconds + 4, 2)) + "\n";
        }

        void myMediaElement_MarkerReached(object sender, TimelineMarkerRoutedEventArgs e)
        {
            Scenario3Text.Text += e.Marker.Text + " - " + String.Format("{0:g}", Math.Round(e.Marker.Time.TotalSeconds, 2)) + "\r\n";
        }

        #endregion

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

        #region Template-Related Code - Do not remove

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            // Starting scenario is the first or based upon a previous selection.
            ListBoxItem startingScenario = null;
            if (SuspensionManager.SessionState.ContainsKey("SelectedScenario"))
            {
                String selectedScenarioName = SuspensionManager.SessionState["SelectedScenario"] as string;
                startingScenario = this.FindName(selectedScenarioName) as ListBoxItem;
            }

            ScenariosList.SelectedItem = startingScenario != null ? startingScenario : Scenario1;
        }

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

        #endregion

        #region File Handling Code
        async Task PickFileAsync(bool multiplieFiles)
        {

            FileOpenPicker fileopenpicker = new FileOpenPicker();

            fileopenpicker.FileTypeFilter.Add(".wmv");
            fileopenpicker.FileTypeFilter.Add(".mp4");
            fileopenpicker.FileTypeFilter.Add(".mp3");
            fileopenpicker.FileTypeFilter.Add(".wma");
            fileopenpicker.FileTypeFilter.Add(".png");
            fileopenpicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;// | PickerLocationId.DocumentsLibrary | PickerLocationId.PicturesLibrary | PickerLocationId.MusicLibrary;

            if (multiplieFiles)
            {
                var files = await fileopenpicker.PickMultipleFilesAsync();

                if (MediaPlayList.Count > 0)
                {
                    MediaPlayList.Clear();
                }

                foreach (StorageFile file in files)
                {
                    MediaPlayList.Add(new BindableStorageFile { Name = file.Name, File = file });
                }
            }
            else
            {
                var singlefileoperation = await fileopenpicker.PickSingleFileAsync();
                if (singlefileoperation != null)
                {
                    await SetMediaElementSourceAsync(singlefileoperation);
                }
            }
        }

        async Task SetMediaElementSourceAsync(StorageFile file)
        {
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            MediaControl.TrackName = file.DisplayName;
            myMediaElement.SetSource(stream, file.ContentType);
        }

        async void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ApplicationView.TryUnsnap())
            {
                VisualStateManager.GoToState(this, string.Format("Filled{0}", DisplayProperties.ResolutionScale), true);
            }

            await PickFileAsync(false);
        }

        #endregion

        #region Fullscreen
        private void FullscreenToggle()
        {
            this.IsFullscreen = !this.IsFullscreen;

            if (this.IsFullscreen)
            {
                Footer.Visibility = Visibility.Collapsed;
                Header.Visibility = Visibility.Collapsed;
                InputPanel.Visibility = Visibility.Collapsed;
                InputText.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                StatusBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                OutputStack.Visibility = Visibility.Collapsed;
                OutputText.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                transportControlStackPanel.Visibility = Visibility.Collapsed;

                _previousmediasize.Height = myMediaElement.Height;
                _previousmediasize.Width = myMediaElement.Width;
                _previousmediaelementmargin = ContentRoot.Margin;

                MediaScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                MediaScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

                _previousBGColor = ContentRoot.Background;
                ContentRoot.Margin = new Thickness();
                ContentRoot.Background = new SolidColorBrush(Colors.Black);
                LayoutRoot.Background = new SolidColorBrush(Colors.Black);

                MPContainer.Width = Window.Current.Bounds.Width;
                MPContainer.Height = Window.Current.Bounds.Height;

                myMediaElement.Width = Window.Current.Bounds.Width;
                myMediaElement.Height = double.NaN;
            }
            else
            {

                Footer.Visibility = Visibility.Visible;
                Header.Visibility = Visibility.Visible;
                InputPanel.Visibility = Visibility.Visible;
                InputText.Visibility = Windows.UI.Xaml.Visibility.Visible;

                StatusBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
                OutputStack.Visibility = Visibility.Visible;
                OutputText.Visibility = Windows.UI.Xaml.Visibility.Visible;

                transportControlStackPanel.Visibility = Visibility.Visible;

                MediaScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                MediaScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

                LayoutRoot.Background = _previousBGColor;
                ContentRoot.Background = _previousBGColor;
                ContentRoot.Margin = _previousmediaelementmargin;

                MPContainer.Height = double.NaN;
                MPContainer.Width = double.NaN;

                myMediaElement.Width = _previousmediasize.Width;
                myMediaElement.Height = _previousmediasize.Height;

            }
        }

        void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            FullscreenToggle();
        }

        void meHost_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (IsFullscreen && e.Key == Windows.System.VirtualKey.Escape)
                FullscreenToggle();
            e.Handled = true;
        }

        #endregion

        #region Scenario Specific Code

        void Scenario3Init()
        {
            MarkersButton.Click += MarkersButton_Click;
            myMediaElement.MarkerReached += new TimelineMarkerRoutedEventHandler(myMediaElement_MarkerReached);
        }

        void Scenario3Reset()
        {
            MarkersButton.Click -= new RoutedEventHandler(MarkersButton_Click);
            myMediaElement.MarkerReached -= new TimelineMarkerRoutedEventHandler(myMediaElement_MarkerReached);
        }

        void Scenario4Init()
        {
            FullscreenButton.Click += new RoutedEventHandler(FullscreenButton_Click);
            meHost.KeyDown += new Windows.UI.Xaml.Input.KeyEventHandler(meHost_KeyDown);
        }

        void Scenario4Reset()
        {
            FullscreenButton.Click -= new RoutedEventHandler(FullscreenButton_Click);
            meHost.KeyDown -= new Windows.UI.Xaml.Input.KeyEventHandler(meHost_KeyDown);
        }

        void Scenario5Init()
        {
            myPlaylist.SelectionChanged += new SelectionChangedEventHandler(myPlaylist_SelectionChanged);
            PlayListCreationButton.Click += new RoutedEventHandler(PlayListCreationButton_Click);
        }

        void Scenario5Reset()
        {
            myPlaylist.SelectionChanged -= new SelectionChangedEventHandler(myPlaylist_SelectionChanged);
            PlayListCreationButton.Click -= new RoutedEventHandler(PlayListCreationButton_Click);
        }

        async void PlayListCreationButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ApplicationView.TryUnsnap())
            {
                VisualStateManager.GoToState(this, "FullScreenLandscape", true);
            }

            myMediaElement.Stop();

            await PickFileAsync(true);

            if (MediaPlayList.Count > 0)
            {
                myPlaylist.ItemsSource = MediaPlayList;
                myPlaylist.SelectedIndex = 0;
            }
        }

        void myMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            StopTimer();
            timelineSlider.Value = 0.0;

            if (myPlaylist.SelectedIndex == myPlaylist.Items.Count - 1)
            {
                return;
            }
            else
            {
                if (myPlaylist.SelectedIndex + 1 < myPlaylist.Items.Count)
                {
                    myPlaylist.SelectedIndex++;
                }
            }
        }

        async void myPlaylist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                StorageFile selectedFile = (myPlaylist.SelectedItem as BindableStorageFile).File;
                await SetMediaElementSourceAsync(selectedFile);
            }
            catch (Exception)
            {
            }

        }

        void ScenarioList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResetAll();

            if (ScenariosList.SelectedItem != null)
            {
                NotifyUser("", NotifyType.StatusMessage);

                ListBoxItem selectedListBoxItem = ScenariosList.SelectedItem as ListBoxItem;
                SuspensionManager.SessionState["SelectedScenario"] = selectedListBoxItem.Name;

                ShowDescriptions(ScenariosList.SelectedIndex + 1);
            }

            if (ScenariosList.SelectedItem == Scenario3)
            {
                Scenario3Init();
                Scenario3Input.Visibility = Visibility.Visible;
                Scenario3Text.Visibility = Visibility.Visible;
            }
            else if (ScenariosList.SelectedItem == Scenario4)
            {
                Scenario4Init();
                Scenario3Text.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Scenario4Input.Visibility = Visibility.Visible;
                Scenario4Text.Visibility = Visibility.Visible;
            }

            else if (ScenariosList.SelectedItem == Scenario5)
            {
                Scenario5Init();
                Scenario3Text.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Scenario5Input.Visibility = Visibility.Visible;
                myPlaylist.Visibility = Visibility.Visible;
            }
        }

        public void ResetAll()
        {
            Scenario3Text.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            ShowDescriptions(-1);
            Scenario3Reset();
            Scenario4Reset();

            myPlaylist.Visibility = Visibility.Collapsed;
            Scenario5Reset();
        }

        private void ShowDescriptions(int scenariodescriptionidnumber)
        {
            for (int i = 0; i < ScenarioInputGrid.Children.Count; i++)
            {
                if (((string)ScenarioInputGrid.Children[i].GetValue(NameProperty)).Contains(scenariodescriptionidnumber.ToString()))
                {
                    ScenarioInputGrid.Children[i].Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    ScenarioInputGrid.Children[i].Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region Media Position and Slider interaction 

        void slider_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Pointer entered event fired");
            _sliderpressed = true;
        }

        void slider_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Pointer capture lost event fired");
            System.Diagnostics.Debug.WriteLine("Slider value at capture lost {0}", timelineSlider.Value);
            //myMediaElement.PlaybackRate = 1;
            myMediaElement.Position = TimeSpan.FromSeconds(timelineSlider.Value);
            _sliderpressed = false;
        }

        void timelineSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Slider old value = {0} new value = {1}.", e.OldValue, e.NewValue);
            if (!_sliderpressed)
            {
                myMediaElement.Position = TimeSpan.FromSeconds(e.NewValue);
            }
        }

        private void SetupTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(timelineSlider.StepFrequency);
            StartTimer();
        }

        private void _timer_Tick(object sender, object e)
        {
            if (!_sliderpressed)
            {
                timelineSlider.Value = myMediaElement.Position.TotalSeconds;
            }
        }

        private void StartTimer()
        {
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void StopTimer()
        {
            _timer.Stop();
            _timer.Tick -= _timer_Tick;
        }

        void myMediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (myMediaElement.CurrentState == MediaElementState.Playing)
            {
                if (_sliderpressed)
                {
                    _timer.Stop();
                }
                else
                {
                    _timer.Start();
                }
                MediaControl.IsPlaying = true;
            }

            if (myMediaElement.CurrentState == MediaElementState.Paused)
            {
                _timer.Stop();
                MediaControl.IsPlaying = false;
            }

            if (myMediaElement.CurrentState == MediaElementState.Stopped)
            {
                _timer.Stop();
                timelineSlider.Value = 0;
                MediaControl.IsPlaying = false;
            }
        }

        private double SliderFrequency(TimeSpan timevalue)
        {
            double stepfrequency = -1;

            double absvalue = (int)Math.Round(timevalue.TotalSeconds, MidpointRounding.AwayFromZero);
            stepfrequency = (int)(Math.Round(absvalue / 100));

            if (timevalue.TotalMinutes >= 10 && timevalue.TotalMinutes < 30)
            {
                stepfrequency = 10;
            }
            else if (timevalue.TotalMinutes >= 30 && timevalue.TotalMinutes < 60)
            {
                stepfrequency = 30;
            }
            else if (timevalue.TotalHours >= 1)
            {
                stepfrequency = 60;
            }

            if (stepfrequency == 0) stepfrequency += 1;

            if (stepfrequency == 1)
            {
                stepfrequency = absvalue / 100;
            }

            return stepfrequency;
        }

        #endregion
    }

    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };
}
