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
using Microsoft.SharePoint.Client;
using Microsoft.Phone.Tasks;
using System.Device.Location;
using Microsoft.Phone.Shell;
using Microsoft.SharePoint.Phone.Application;

namespace MultipleSPListAppSingleProject.OrdersListViews
{
    /// <summary>
    /// ListItem Edit Form
    /// </summary>
    public partial class EditForm : PhoneApplicationPage
    {
        EditItemViewModel viewModel;

        /// <summary>
        /// Constructor for Edit Form
        /// </summary>
        public EditForm()
        {
            InitializeComponent();

            viewModel = App.OrdersListViewModel.SelectedItemEditViewModelInstance;
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
        /// Code to execute when app navigates to Edit Form
        /// </summary>
        /// <param name="e" />
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel.ItemUpdated += new EventHandler<ItemUpdatedEventArgs>(OnItemUpdated);
        }

        /// <summary>
        /// Code to execute when app navigates away from Edit Form
        /// </summary>
        /// <param name="e" />
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            viewModel.ItemUpdated -= new EventHandler<ItemUpdatedEventArgs>(OnItemUpdated);
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
                    return;
                }

                //Set Page's DataContext to current ViewModel instance
                this.DataContext = (sender as EditItemViewModel);
            });
        }

        /// <summary>
        /// Code to execute when user clicks on cancel button on the Application
        /// </summary>
        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/OrdersListViews/OrdersList.xaml", UriKind.Relative));
        }



        /// <summary>
        /// Code to execute when user clicks on Submit button on the ListEdit Form to update SharePoint ListItem
        /// </summary>
        private void OnBtnSubmitClick(object sender, EventArgs e)
        {
            viewModel.UpdateItem();
        }

        private void OnItemUpdated(object sender, ItemUpdatedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message, e.Error.GetType().Name, MessageBoxButton.OK);
                    return;
                }

                this.NavigationService.Navigate(new Uri("/OrdersListViews/OrdersList.xaml", UriKind.Relative));
            });
        }
    }
}
