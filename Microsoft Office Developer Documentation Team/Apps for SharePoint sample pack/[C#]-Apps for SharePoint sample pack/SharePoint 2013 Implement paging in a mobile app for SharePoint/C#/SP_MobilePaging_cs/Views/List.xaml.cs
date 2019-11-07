using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Threading;
using Microsoft.SharePoint.Client;
using System.Collections;
using Microsoft.Phone.Shell;
using Microsoft.SharePoint.Phone.Application;
using System.Collections.ObjectModel;

namespace SP_MobilePaging_cs
{
    /// <summary>
    /// ListView Form
    /// </summary>
    public partial class ListForm : PhoneApplicationPage
    {
        /// <summary>
        /// Constructor for List Form
        /// </summary>
        public ListForm()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            App.MainViewModel.ViewDataLoaded += new EventHandler<ViewDataLoadedEventArgs>(OnViewDataLoaded);
            App.MainViewModel.InitializationCompleted += new EventHandler<InitializationCompletedEventArgs>(OnViewModelInitialization);

            this.DataContext = App.MainViewModel;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            App.MainViewModel.ViewDataLoaded -= new EventHandler<ViewDataLoadedEventArgs>(OnViewDataLoaded);
            App.MainViewModel.InitializationCompleted -= new EventHandler<InitializationCompletedEventArgs>(OnViewModelInitialization);
        }

        /// <summary>
        /// Code to execute when a Pivot Item is loaded on the page.
        /// </summary>
        private void OnPivotItemLoaded(object sender, PivotItemEventArgs e)
        {
            //Check if ListForm ViewModel is initialized. If already initialized, start loading data for the current View
            if (!App.MainViewModel.IsInitialized)
                App.MainViewModel.Initialize();
            else
                App.MainViewModel.LoadData(e.Item.Name);
        }


        /// <summary>
        /// Code to execute when Refresh Button on the application bar is clicked
        /// </summary>
        private void OnRefreshButtonClick(object sender, EventArgs e)
        {
            if (Views.SelectedItem == null)
                return;

            //Check if ListForm ViewModel is initialized. If already initialized, start refreshing data for the current View
            if (!App.MainViewModel.IsInitialized)
                App.MainViewModel.Initialize();
            else
                App.MainViewModel.RefreshData(((PivotItem)Views.SelectedItem).Name);
        }

        /// <summary>
        /// Code to execute when ViewModel initialization completes
        /// </summary>
        private void OnViewModelInitialization(object sender, InitializationCompletedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                //If initialization has failed, show error message and return
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message, e.Error.GetType().Name, MessageBoxButton.OK);
                    return;
                }

                App.MainViewModel.LoadData(((PivotItem)Views.SelectedItem).Name);
            });
        }

        /// <summary>
        /// Code to execute when loading view data is complete
        /// </summary>
        void OnViewDataLoaded(object sender, ViewDataLoadedEventArgs e)
        {
            if (e.Error != null)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(e.Error.Message, e.Error.GetType().Name, MessageBoxButton.OK);
                });

                return;
            }

            App.MainViewModel[e.ViewName] = e.ViewData;
        }


    }
}
