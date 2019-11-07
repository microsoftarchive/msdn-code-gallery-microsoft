using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using System.Text;

namespace CSPostJsonViaHttpClient
{
    public sealed partial class MainPage : Page
    {
        HttpClient httpClient;

        public MainPageViewModel ViewModel { get; set; } = new MainPageViewModel()
        {
            Name = "Tommy",
            Age = "23",
            StartButtonIsEnabled = true
        };

        public MainPage()
        {
            this.InitializeComponent();

            this.DataContext = ViewModel;
        }

        async private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri((sender as HyperlinkButton).Tag.ToString()));
        }

        private async void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StartButtonIsEnabled = false;

            httpClient = new HttpClient();
            try
            {
                string resourceAddress = "http://localhost:46789/WCFService.svc/GetData";

                string jsonStr = JsonConvert.SerializeObject(new { Name = ViewModel.Name, Age = ViewModel.Age });

                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage wcfResponse = await httpClient.PostAsync(resourceAddress, new StringContent(jsonStr, Encoding.UTF8, "application/json"));

                string responseText = await wcfResponse.Content.ReadAsStringAsync();
                ViewModel.ServerResult = responseText;
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
                ViewModel.StartButtonIsEnabled = true;
                if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }
            }
        }

        public void NotifyUser(string message)
        {
            this.StatusText.Text = message;
        }
    }

    public class MainPageViewModel : Common.BindableBase
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _age;
        public string Age
        {
            get { return _age; }
            set { SetProperty(ref _age, value); }
        }

        private string _serverResult;
        public string ServerResult
        {
            get { return _serverResult; }
            set { SetProperty(ref _serverResult, value); }
        }

        private bool _startButtonIsEnabled;
        public bool StartButtonIsEnabled
        {
            get { return _startButtonIsEnabled; }
            set { SetProperty(ref _startButtonIsEnabled, value); }
        }
    }
}
