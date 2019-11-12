/****************************** Module Header ******************************\
 * Module Name:  MainPage.cs
 * Project:      CSWindowsStoreAppSaveCollection
 * Copyright (c) Microsoft Corporation.
 * 
 * This sample demonstrate how to save collection to local storage
 *  
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System.Collections.ObjectModel;
using Windows.UI;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace CSWindowsStoreAppSaveCollection
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : CSWindowsStoreAppSaveCollection.Common.LayoutAwarePage
    {
        // The collection will be saved
        ObservableCollection<Person> itemCollection = new ObservableCollection<Person>();

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            StorageFile localFile;
            try
            {
                localFile = await ApplicationData.Current.LocalFolder.GetFileAsync("localData.xml");
            }
            catch (FileNotFoundException ex)
            {
                NotifyUser(ex.Message);
                localFile = null;
            }
            if (localFile != null)
            {
                string localData = await FileIO.ReadTextAsync(localFile);

                itemCollection = ObjectSerializer<ObservableCollection<Person>>.FromXml(localData);
            }
            gvData.ItemsSource = itemCollection;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected async override void SaveState(Dictionary<String, Object> pageState)
        {
            try
            {
                string localData = ObjectSerializer<ObservableCollection<Person>>.ToXml(itemCollection);

                if (!string.IsNullOrEmpty(localData))
                {
                    StorageFile localFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("localData.xml", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(localFile, localData);
                }
            }
            catch (Exception ex)
            {
                NotifyUser(ex.Message);
            }
        }

        #region Common methods

        async private void Footer_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri((sender as HyperlinkButton).Tag.ToString()));
        }


        public void NotifyUser(string message)
        {
            this.statusText.Text = message;
        }

        #endregion        

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (DataValidation())
            {

                tbHint.Visibility = Visibility.Collapsed;
                Person person = new Person();
                person.Name = txtName.Text;
                person.Age = Convert.ToInt32(txtAge.Text);
                itemCollection.Add(person);
                svContent.ScrollToVerticalOffset(svContent.ExtentHeight);
            }
        }

        /// <summary>
        /// Check if the information is valid
        /// </summary>
        /// <returns></returns>
        private bool DataValidation()
        {
            bool bValid = true;
            string name = txtName.Text;
            string age = txtAge.Text;
            Regex reg = new Regex("^[0-9]*$");

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(age))
            {
                bValid = false;
                tbHint.Text = "Name/Age can be empty.";
                tbHint.Visibility = Visibility.Visible;
            }
            else if (!reg.IsMatch(age) || Convert.ToInt32(age) > 120)
            {
                bValid = false;
                tbHint.Text = "Age should be a number between 0 to 120";
                tbHint.Visibility = Visibility.Visible;
            }

            return bValid;
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SecondPage));
        }
    }
}
