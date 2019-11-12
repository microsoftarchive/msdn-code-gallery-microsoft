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

namespace SPListAppUICustomization
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

            viewModel = App.MainViewModel.SelectedItemEditViewModelInstance;
            if (!viewModel.IsInitialized)
            {
                viewModel.InitializationCompleted += new EventHandler<InitializationCompletedEventArgs>(OnViewModelInitialization);
                viewModel.Initialize();
            }
            else
            {
                this.DataContext = viewModel;
            }

            // Adding handler for Loaded event.
            this.Loaded += new RoutedEventHandler(EditForm_Loaded);
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
            NavigationService.Navigate(new Uri("/Views/List.xaml", UriKind.Relative));
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

                this.NavigationService.Navigate(new Uri("/Views/List.xaml", UriKind.Relative));
            });
        }

        private void EditForm_Loaded(object sender, RoutedEventArgs e)
        {
            // Register EditFieldValueConverter SET function on Choice fields.
            Converter.RegisterEditFieldValueConverter(FieldType.Choice, (string fieldName, object fieldValue, ListItem item, ConversionContext context) =>
            {
                string otherCategoryValue = string.Empty;
                if (this.rbOtherCategory.IsChecked == true)
                {
                    otherCategoryValue = this.txtOtherCategory.Text.Trim();
                    if (string.IsNullOrWhiteSpace(otherCategoryValue))
                    {
                        otherCategoryValue = "(Unspecified)";
                    }
                }

                ContosoConverter.SetConvertedChoiceEditFieldValue(fieldName, fieldValue, item, context, otherCategoryValue);
            });

            // Adding RadioButton event handlers here because the
            // txtOtherCategory TextBox will be loaded and available at this point.
            this.rbOtherCategory.Checked += new RoutedEventHandler(rbOtherCategory_Checked);
            this.rbOtherCategory.Unchecked += new RoutedEventHandler(rbOtherCategory_Unchecked);
        }

        private void rbOtherCategory_Checked(object sender, RoutedEventArgs e)
        {
            this.txtOtherCategory.Visibility = System.Windows.Visibility.Visible;
            this.txtOtherCategory.Focus();
        }

        private void rbOtherCategory_Unchecked(object sender, RoutedEventArgs e)
        {
            this.txtOtherCategory.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
