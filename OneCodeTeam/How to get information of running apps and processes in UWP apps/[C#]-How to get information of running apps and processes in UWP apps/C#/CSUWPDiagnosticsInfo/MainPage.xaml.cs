using CSUWPDiagnosticsInfo.DataModel;
using CSUWPDiagnosticsInfo.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.System.Diagnostics;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CSUWPDiagnosticsInfo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainViewModel mainViewModel = new MainViewModel();
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = mainViewModel;
            lvApp.SelectionChanged += LvApp_SelectionChanged;
           
        }

        /// <summary>
        /// Show full information in flyout when item selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LvApp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvApp.SelectedIndex > -1)
            {
                ListViewItem container = lvApp.ContainerFromItem(lvApp.SelectedItem) as ListViewItem;
                Grid root = container.ContentTemplateRoot as Grid;
                FlyoutBase.ShowAttachedFlyout(root);
            }
        }

        /// <summary>
        /// Load latest data after pivot selection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot pivot = sender as Pivot;
            PivotItem selected = pivot.SelectedItem as PivotItem;
            if (selected.Equals(pivotItemApps))
            {
                LoadAppInfo();
            }
            else
            {
                LoadProcesses();
            }
        }

        /// <summary>
        /// request for app diagnostic info
        /// </summary>
        private async void LoadAppInfo()
        {
            mainViewModel.AppInfoList.Clear();
            IList<AppDiagnosticInfo> list = await AppDiagnosticInfo.RequestInfoAsync();            
            list.ToList().ForEach(o => mainViewModel.AppInfoList.Add( new AppInfoModel(o.AppInfo)));
        }

        /// <summary>
        /// request for process info
        /// </summary>
        private void LoadProcesses()
        {
            mainViewModel.ProcessList.Clear();
            List<ProcessDiagnosticInfo> processList = ProcessDiagnosticInfo.GetForProcesses().ToList();
            processList.ForEach(o => mainViewModel.ProcessList.Add(new ProcessInfoModel(o)));
        }

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
    }
}
