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

namespace MultipleSPListAppSingleProject
{
    /// <summary>
    /// ListView Form
    /// </summary>
    public partial class ListForm : PhoneApplicationPage
    {
        /// <summary>
        /// Contructor for List Form
        /// </summary>
        public ListForm()
        {
            InitializeComponent();
            this.DataContext = App.PrimaryViewModel;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            App.PrimaryViewModel.ViewDataLoaded += new EventHandler<ViewDataLoadedEventArgs>(OnViewDataLoaded);
            App.PrimaryViewModel.InitializationCompleted += new EventHandler<InitializationCompletedEventArgs>(OnViewModelInitialization);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            App.PrimaryViewModel.ViewDataLoaded -= new EventHandler<ViewDataLoadedEventArgs>(OnViewDataLoaded);
            App.PrimaryViewModel.InitializationCompleted -= new EventHandler<InitializationCompletedEventArgs>(OnViewModelInitialization);
        }

        /// <summary>
        /// Code to execute when a Pivot Item is loaded on the page.
        /// </summary>
        private void OnPivotItemLoaded(object sender, PivotItemEventArgs e)
        {
            if (!App.PrimaryViewModel.IsInitialized)
            {
                //Initialize ViewModel and Load Data for PivotItem upon initialization
                App.PrimaryViewModel.Initialize();
            }
            else
            {
                //Load Data for the currently loaded PivotItem
                App.PrimaryViewModel.LoadData(e.Item.Name);
            }
        }

        /// <summary>
        /// Code to execute when New Button on the application bar is clicked
        /// </summary>
        private void OnNewButtonClick(object sender, EventArgs e)
        {
            //Instantiate a new instance of NewItemViewModel and go to NewForm.
            App.PrimaryViewModel.CreateItemViewModelInstance = new NewItemViewModel { DataProvider = App.PrimaryDataProvider };
            NavigationService.Navigate(new Uri("/Views/NewForm.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Code to execute when Refresh Button on the application bar is clicked
        /// </summary>
        private void OnRefreshButtonClick(object sender, EventArgs e)
        {
            if (Views.SelectedItem == null)
                return;

            if (!App.PrimaryViewModel.IsInitialized)
            {
                //Initialize ViewModel and Load Data for PivotItem upon completion
                App.PrimaryViewModel.Initialize();
            }
            else
            {   //Refresh Data for the currently loaded PivotItem
                App.PrimaryViewModel.RefreshData(((PivotItem)Views.SelectedItem).Name);
            }
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

                App.PrimaryViewModel.LoadData(((PivotItem)Views.SelectedItem).Name);
                this.DataContext = (sender as ListViewModel);
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

            App.PrimaryViewModel[e.ViewName] = (ObservableCollection<DisplayItemViewModel>)e.ViewData;
        }


        /// <summary>
        /// Code to execute when selection of item in the ListBox changes
        /// </summary>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //If no selection is made, return
            if ((sender as ListBox).SelectedIndex == -1)
                return;

            //Set selected Item in MainViewModel
            App.PrimaryViewModel.SelectedItemDisplayViewModelInstance = (sender as ListBox).SelectedItem as DisplayItemViewModel;

            //Navigate to DisplayForm and reset the selection to none.
            NavigationService.Navigate(new Uri("/Views/DisplayForm.xaml", UriKind.Relative));
            (sender as ListBox).SelectedIndex = -1;
        }

        private void OnSecondaryListButtonClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/OrdersListViews/OrdersList.xaml", UriKind.Relative));
        }
    }
}
