/****************************** Module Header ******************************\
 * Module Name:  MainPage.xaml.cs
 * Project:      CSUnvsAppJsonToWebService.WindowsPhone
 * Copyright (c) Microsoft Corporation.
 * 
 * This sample demonstrates how to use HttpClient to post Json data in 
 * universal Windows apps.
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
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace CSUnvsAppJsonToWebService
{
    
    public sealed partial class MainPage : Page
    {
        HttpClient httpClient;
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private async void Footer_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hyperlinkButton = sender as HyperlinkButton;
            if (hyperlinkButton != null)
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(hyperlinkButton.Tag.ToString()));
            }
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            // Clear text of Output textbox 
            this.OutputField.Text = string.Empty;
            this.statusText.Text = string.Empty;

            this.StartButton.IsEnabled = false;
            httpClient = new HttpClient();
            try
            {
                string resourceAddress = "http://localhost:4848/WCFService.svc/GetData";
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

        public void NotifyUser(string message)
        {
            this.statusText.Text = message;
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
    }
}
