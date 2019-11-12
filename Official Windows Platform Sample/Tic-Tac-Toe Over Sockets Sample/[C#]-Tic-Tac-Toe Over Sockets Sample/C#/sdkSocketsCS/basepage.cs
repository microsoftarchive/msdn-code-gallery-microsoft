/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Windows;
using Microsoft.Phone.Controls;

namespace sdkSocketsCS
{
    /// <summary>
    /// Base page used by other pages in this application to share implementation of
    /// settings loading. Also exposes dependency properties for all settings that can be 
    /// used in data binding by all pages.
    /// </summary>
    public class basepage : PhoneApplicationPage
    {
        bool _isNewPageInstance = false;

        public basepage()
        {
            _isNewPageInstance = true;
           
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // If _isNewPageInstance is true, the page constuctor has been called, so
            // state may need to be restored
            if (_isNewPageInstance)
            {

                (Application.Current as App).ApplicationDataObjectChanged += new EventHandler(basepage_ApplicationDataObjectChanged);

                // if the application member variable is not empty,
                // set the page's data object from the application member variable.
                if ((Application.Current as App).HostName != null)
                {
                    _updateDependencyProperties();
                }
                else
                {
                    (Application.Current as App).GetDataAsync();

                }
            }

            // Set _isNewPageInstance to false. If the user navigates back to this page
            // and it has remained in memory, this value will continue to be false.
            _isNewPageInstance = false;
        }

        void basepage_ApplicationDataObjectChanged(object sender, EventArgs e)
        {
            // Update the settings DependencyProperties.
            // Note: this approach is brute-force since only one event is 
            // used to signal a change in the settings, so all settings have to be
            // updated. An alternative would be to have a changed event per setting
            // and only update those settings that have truely changed.
            if (System.Windows.Deployment.Current.Dispatcher.CheckAccess())
            {
                _updateDependencyProperties();
            }
            else
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    _updateDependencyProperties();

                });
            }
           
        }

        private void _updateDependencyProperties()
        {
            ServerName = (Application.Current as App).HostName;
            PortNumber = (Application.Current as App).PortNumber;
            PlayAsX = (Application.Current as App).PlayAsX;
            PlayAsO = !(Application.Current as App).PlayAsX;
        }

        public static readonly DependencyProperty ServerNameProperty = DependencyProperty.RegisterAttached("ServerName", typeof(string), typeof(string), new PropertyMetadata(string.Empty));
        public string ServerName
        {
            get { return (string)GetValue(ServerNameProperty); }
            set { SetValue(ServerNameProperty, value); }
        }

        public static readonly DependencyProperty PortNumberProperty = DependencyProperty.RegisterAttached("PortNumber", typeof(int), typeof(int), new PropertyMetadata(0));
        public int PortNumber
        {
            get { return (int)GetValue(PortNumberProperty); }
            set { SetValue(PortNumberProperty, value); }
        }

        public static readonly DependencyProperty PlayAsXProperty = DependencyProperty.RegisterAttached("PlayAsX", typeof(bool), typeof(bool), new PropertyMetadata(true));
        public bool PlayAsX
        {
            get { return (bool)GetValue(PlayAsXProperty); }
            set { SetValue(PlayAsXProperty, value); }
        }


        // This property is not actually stored in the application. It is faked here using the PlayAsXproperty
        // The reason for this that this always the opposite of the PlayAsX value, so there is no need to store it
        public static readonly DependencyProperty PlayAsOProperty = DependencyProperty.RegisterAttached("PlayAsO", typeof(bool), typeof(bool), new PropertyMetadata(true));
        public bool PlayAsO
        {
            get { return !(bool)GetValue(PlayAsXProperty); }
            set { SetValue(PlayAsOProperty, value); }
        }

    }
}
