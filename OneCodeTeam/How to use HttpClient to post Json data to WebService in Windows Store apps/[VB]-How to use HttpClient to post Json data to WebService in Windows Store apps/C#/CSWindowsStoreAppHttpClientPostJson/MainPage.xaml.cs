/****************************** Module Header ******************************\
 * Module Name:  MainPage.xaml.cs
 * Project:      CSWindowsStoreAppHttpClientPostJson
 * Copyright (c) Microsoft Corporation.
 * 
 * This sample demonstrates how to use HttpClient to post Json data in 
 * Windows Store app.
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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace CSWindowsStoreAppHttpClientPostJson
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : CSWindowsStoreAppHttpClientPostJson.Common.LayoutAwarePage
    {
        HttpClient httpClient;
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
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Start to Call WCF service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            // Clear text of Output textbox 
            this.OutputField.Text = string.Empty;
            this.StatusBlock.Text = string.Empty;

            this.StartButton.IsEnabled = false;
            httpClient = new HttpClient();
            try
            {
                string resourceAddress = "http://localhost:44516/WCFService.svc/GetData";
                int age = Convert.ToInt32(this.Agetxt.Text);
                if (age > 120 || age < 0)
                {
                    throw new Exception("Age must be between 0 and 120");
                }
                Person p = new Person { Name = this.Nametxt.Text, Age = age };
                string postBody = JsonSerializer(p);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage wcfResponse = await httpClient.PostAsync(resourceAddress, new StringContent(postBody, Encoding.UTF8, "application/json"));
                await DisplayTextResult(wcfResponse, OutputField);
            }
            catch (HttpRequestException hre)
            {
                NotifyUser("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                NotifyUser("Request canceled.");
            }
            catch (Exception ex)
            {
                NotifyUser(ex.Message);
            }
            finally
            {
                this.StartButton.IsEnabled = true;
                if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }
            }
        }

        /// <summary>
        /// Display Result which returns from WCF Service in "OutputField" Textbox
        /// </summary>
        /// <param name="response">Http response Message</param>
        /// <param name="output">Show result control</param>
        /// <returns></returns>
        private async Task DisplayTextResult(HttpResponseMessage response, TextBox output)
        {
            string responJsonText = await response.Content.ReadAsStringAsync();
            GetJsonValue(responJsonText);
            output.Text += GetJsonValue(responJsonText);
        }

        /// <summary>
        /// Serialize Person object to Json string
        /// </summary>
        /// <param name="objectToSerialize">Person object instance</param>
        /// <returns>return Json String</returns>
        public string JsonSerializer(Person objectToSerialize)
        {
            if (objectToSerialize == null)
            {
                throw new ArgumentException("objectToSerialize must not be null");
            }
            MemoryStream ms = null;

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(objectToSerialize.GetType());
            ms = new MemoryStream();
            serializer.WriteObject(ms, objectToSerialize);
            ms.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        /// <summary>
        /// Get Result from Json String
        /// </summary>
        /// <param name="jsonString">Json string which returns from WCF Service</param>
        /// <returns>Result string</returns>
        public string GetJsonValue(string jsonString)
        {
            int ValueLength = jsonString.LastIndexOf("\"") - (jsonString.IndexOf(":") + 2);
            string value = jsonString.Substring(jsonString.IndexOf(":") + 2, ValueLength);
            return value;

        }

        #region Common methods

        /// <summary>
        /// The event handler for the click event of the link in the footer. 
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private async void FooterClick(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hyperlinkButton = sender as HyperlinkButton;
            if (hyperlinkButton != null)
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(hyperlinkButton.Tag.ToString()));
            }
        }

        public void NotifyUser(string message)
        {
            this.StatusBlock.Text = message;
        }

        #endregion
    }
}
