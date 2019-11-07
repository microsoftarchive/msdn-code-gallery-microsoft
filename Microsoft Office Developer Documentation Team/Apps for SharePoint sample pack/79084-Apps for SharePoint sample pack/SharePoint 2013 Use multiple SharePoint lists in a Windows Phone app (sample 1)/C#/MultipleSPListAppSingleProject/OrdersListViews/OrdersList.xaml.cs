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

namespace MultipleSPListAppSingleProject.OrdersListViews
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
            this.DataContext = App.OrdersListViewModel;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            App.OrdersListViewModel.ViewDataLoaded += new EventHandler<ViewDataLoadedEventArgs>(OnViewDataLoaded);
            App.OrdersListViewModel.InitializationCompleted += new EventHandler<InitializationCompletedEventArgs>(OnViewModelInitialization);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            App.OrdersListViewModel.ViewDataLoaded -= new EventHandler<ViewDataLoadedEventArgs>(OnViewDataLoaded);
            App.OrdersListViewModel.InitializationCompleted -= new EventHandler<InitializationCompletedEventArgs>(OnViewModelInitialization);
        }

        /// <summary>
        /// Code to execute when a Pivot Item is loaded on the page.
        /// </summary>
        private void OnPivotItemLoaded(object sender, PivotItemEventArgs e)
        {
            if (!App.OrdersListViewModel.IsInitialized)
            {
                //Initialize ViewModel and Load Data for PivotItem upon initialization
                App.OrdersListViewModel.Initialize();
            }
            else
            {
                //Load Data for the currently loaded PivotItem
                App.OrdersListViewModel.LoadData(e.Item.Name);
            }
        }

        /// <summary>
        /// Code to execute when New Button on the application bar is clicked
        /// </summary>
        private void OnNewButtonClick(object sender, EventArgs e)
        {
            //Instantiate a new instance of NewItemViewModel and go to NewForm.
            App.OrdersListViewModel.CreateItemViewModelInstance = new NewItemViewModel { DataProvider = App.OrdersListDataProvider };
            NavigationService.Navigate(new Uri("/OrdersListViews/OrdersListNewForm.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Code to execute when Refresh Button on the application bar is clicked
        /// </summary>
        private void OnRefreshButtonClick(object sender, EventArgs e)
        {
            if (Views.SelectedItem == null)
                return;

            if (!App.OrdersListViewModel.IsInitialized)
            {
                //Initialize ViewModel and Load Data for PivotItem upon completion
                App.OrdersListViewModel.Initialize();
            }
            else
            {   //Refresh Data for the currently loaded PivotItem
                App.OrdersListViewModel.RefreshData(((PivotItem)Views.SelectedItem).Name);
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

                App.OrdersListViewModel.LoadData(((PivotItem)Views.SelectedItem).Name);
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

            App.OrdersListViewModel[e.ViewName] = (ObservableCollection<DisplayItemViewModel>)e.ViewData;
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
            App.OrdersListViewModel.SelectedItemDisplayViewModelInstance = (sender as ListBox).SelectedItem as DisplayItemViewModel;

            //Navigate to DisplayForm and reset the selection to none.
            NavigationService.Navigate(new Uri("/OrdersListViews/OrdersListDisplayForm.xaml", UriKind.Relative));
            (sender as ListBox).SelectedIndex = -1;
        }

        private void OnPrimaryListButtonClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/List.xaml", UriKind.Relative));
        }
    }
}
