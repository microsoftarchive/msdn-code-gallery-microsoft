/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace PhoneVoIPApp.UI
{
    /// <summary>
    /// The base class of all the UI pages in this applicaiton
    /// </summary>
    public class BasePage : PhoneApplicationPage
    {
        protected BasePage()
            : this(new BaseViewModel())
        {
        }

        protected BasePage(BaseViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.ViewModel.Page = this;
            this.Loaded += BasePage_Loaded;
        }

        /// <summary>
        /// The view model for this page
        /// </summary>
        protected readonly BaseViewModel ViewModel;

        /// <summary>
        /// Called when this page has been loaded
        /// </summary>
        protected virtual void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = ViewModel;
        }

        /// <summary>
        ///  Override OnNavigatedTo to register for events in the view model
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs nee)
        {
            base.OnNavigatedTo(nee);

            // Let the view model know
            this.ViewModel.OnNavigatedTo(nee);
        }

        /// <summary>
        /// Override OnNavigatedFrom to unregister for events in the view model
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs nee)
        {
            base.OnNavigatedFrom(nee);

            // Let the view model know
            this.ViewModel.OnNavigatedFrom(nee);
        }
    }
}
