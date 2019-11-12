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
using Microsoft.Phone.Shell;
using Microsoft.SharePoint.Client;
using System.Device.Location;
using Microsoft.Phone.Tasks;
using Microsoft.SharePoint.Phone.Application;

namespace SPListAppForExportName
{
    /// <summary>
    /// ListItem Display Form
    /// </summary>
    public partial class DisplayForm : PhoneApplicationPage
    {
        DisplayItemViewModel viewModel;

        /// <summary>
        /// Constructor for Display Form
        /// </summary>
        public DisplayForm()
        {
            InitializeComponent();

            viewModel = App.MainViewModel.SelectedItemDisplayViewModelInstance;
            if (!viewModel.IsInitialized)
            {
                viewModel.InitializationCompleted += new EventHandler<InitializationCompletedEventArgs>(OnViewModelInitialization);
                viewModel.Initialize();
            }
            else
            {
                this.DataContext = viewModel;
            }
        }

        /// <summary>
        /// Code to execute when app navigates to Display Form
        /// </summary>
        /// <param name="e" />
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel.ItemDeleted += new EventHandler<ItemDeletedEventArgs>(OnItemDeleted);
        }

        /// <summary>
        /// Code to execute when app navigates away from Display Form
        /// </summary>
        /// <param name="e" />
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            viewModel.ItemDeleted -= new EventHandler<ItemDeletedEventArgs>(OnItemDeleted);
        }

        /// <summary>
        /// Code to execute when ViewModel initialization completes
        /// </summary>
        /// <param name="sender" />
        /// <param name="e" />
        private void OnViewModelInitialization(object sender, InitializationCompletedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                //If initialization has failed show error message and return
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message, e.Error.GetType().Name, MessageBoxButton.OK);
                }

                //Set Page's DataContext to current ViewModel instance
                this.DataContext = (sender as DisplayItemViewModel);
            });
        }

        /// <summary>
        /// Code to execute when user click on cancel button on the Page
        /// </summary>
        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            //Navigate to the ListView Page
            NavigationService.Navigate(new Uri("/Views/List.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Code to execute when user click on Edit button on the Page
        /// </summary>
        private void OnEditButtonClick(object sender, EventArgs e)
        {
            App.MainViewModel.SelectedItemEditViewModelInstance = new EditItemViewModel { ID = viewModel.ID, DataProvider = App.DataProvider };
            NavigationService.Navigate(new Uri("/Views/EditForm.xaml", UriKind.Relative));
        }
        /// <summary>
        /// Code to execute when user click on Delete button on the Page
        /// </summary>
        private void OnDeleteButtonClick(object sender, EventArgs e)
        {
            viewModel.DeleteItem();
        }

        private void OnItemDeleted(object sender, ItemDeletedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message, e.Error.GetType().Name, MessageBoxButton.OK);
                    return;
                }

                App.DataProvider.ClearCache();
                this.NavigationService.Navigate(new Uri("/Views/List.xaml", UriKind.Relative));
            });
        }
    }
}

