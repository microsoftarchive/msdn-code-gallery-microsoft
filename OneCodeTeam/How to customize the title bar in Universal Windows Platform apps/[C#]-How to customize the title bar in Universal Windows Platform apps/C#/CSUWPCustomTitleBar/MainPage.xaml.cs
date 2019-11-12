using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CSUWPCustomTitleBar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page , INotifyPropertyChanged
    {
        #region Bind title bar's height

        public event PropertyChangedEventHandler PropertyChanged;
        private CoreApplicationViewTitleBar titleBar = CoreApplication.GetCurrentView().TitleBar;
        public double TitleBarHeight { get { return titleBar.Height; } }

        #endregion

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            this.Unloaded += MainPage_Unloaded;
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            this.DataContext = this;
            LoadCustomTitleBar();
        }

        #region Events
        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (this.Frame != null && !e.Handled)
            {
                if (this.ContentFrame != null && this.ContentFrame.CanGoBack)
                {
                    this.ContentFrame.GoBack();
                    e.Handled = true;
                }
                else if (this.Frame.CanGoBack)
                {
                    this.Frame.GoBack();
                    e.Handled = true;
                }
            }
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            this.titleBar.LayoutMetricsChanged -= TitleBar_LayoutMetricsChanged;
            this.titleBar.IsVisibleChanged -= TitleBar_IsVisibleChanged;
            Window.Current.SizeChanged -= Current_SizeChanged;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.titleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged;
            this.titleBar.IsVisibleChanged += TitleBar_IsVisibleChanged;
            Window.Current.SizeChanged += Current_SizeChanged;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("TitleBarHeight"));
            }
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            UpdateTitleBar();
        }

        private void TitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBar();
        }

        private void TitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("TitleBarHeight"));
            }
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            UpdateBackButton();
        }

        private void Navigate_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            bool isNavBtn = btn.Name == "navBtn";
            this.mediaPlayerElement.Visibility = isNavBtn ? Visibility.Collapsed : Visibility.Visible;
            this.ContentFrame.Visibility = isNavBtn ? Visibility.Visible : Visibility.Collapsed;

            if (isNavBtn)
            {
                if (this.ContentFrame.Content == null || !this.ContentFrame.Content.GetType().Equals(typeof(Page2)))
                {
                    this.ContentFrame.Navigate(typeof(Page2));
                }
                if (this.mediaPlayerElement.MediaPlayer != null)
                {
                    this.mediaPlayerElement.MediaPlayer.Pause();
                }
            }
            else if (mediaPlayerElement.Source == null)
            {
                    Uri uri = new Uri("https://mediaplatstorage1.blob.core.windows.net/windows-universal-samples-media/elephantsdream-clip-h264_sd-aac_eng-aac_spa-aac_eng_commentary-srt_eng-srt_por-srt_swe.mkv");
                    mediaPlayerElement.Source = MediaSource.CreateFromUri(uri);
            }
            this.navBtn.IsEnabled = !isNavBtn;
            this.mediaBtn.IsEnabled = isNavBtn;
        }

        #endregion

        #region Private Methods

        private void LoadCustomTitleBar()
        {
            var titleBar = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar;
            titleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(BackgroundRect);
        }
        private void UpdateTitleBar()
        {
            if (ApplicationView.GetForCurrentView().IsFullScreenMode)
            {
                UpdateBackButton();
                //Hide title bar grid
                titleBarGrid.Visibility = titleBar.IsVisible ? Visibility.Visible : Visibility.Collapsed;
                Grid.SetRow(titleBarGrid, 1);
            }
            else
            {
                UpdateBackButton();
                //Set titlebar grid visible. Show it in the first grid row.
                titleBarGrid.Visibility = Visibility.Visible;
                Grid.SetRow(titleBarGrid, 0);
            }
        }

        private void UpdateBackButton()
        {
            if ((this.Frame != null && this.Frame.CanGoBack) || (this.ContentFrame != null && this.ContentFrame.CanGoBack) || ApplicationView.GetForCurrentView().IsFullScreenMode)
            {
                Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Visible;
                this.imgLogo.Visibility = Visibility.Collapsed;
            }
            else
            {
                Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;
                this.imgLogo.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region Page
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
                {
                    if (e.NewSize.Width < 500)
                    {
                        VisualStateManager.GoToState(this, "MinimalLayout", true);
                    }
                    else if (e.NewSize.Width < 700)
                    {
                        VisualStateManager.GoToState(this, "PortraitLayout", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, "DefaultLayout", true);
                    }
                }
        
        async private void Footer_Click(object sender, RoutedEventArgs e)
                {
                    await Windows.System.Launcher.LaunchUriAsync(new Uri((sender as HyperlinkButton).Tag.ToString()));
                }

        #endregion



    }
}
