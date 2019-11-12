using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CSAppLauncher
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void RunMainPage_Click(object sender, RoutedEventArgs e)
        {
            await LaunchAppAsync("test-launchmainpage://HostMainpage/Path1?param=This is param");
        }

        private async void RunPage1_Click(object sender, RoutedEventArgs e)
        {
            await LaunchAppAsync("test-launchpage1://Page1/Path1?param1=This is param1&param1=This is param2");
        }

        private async Task LaunchAppAsync(string uriStr)
        {
            Uri uri = new Uri(uriStr);
            var promptOptions = new Windows.System.LauncherOptions();
            promptOptions.TreatAsUntrusted = false;

            bool isSuccess = await Windows.System.Launcher.LaunchUriAsync(uri, promptOptions);

            if (!isSuccess)
            {
                string msg = "Launch failed";
                await new MessageDialog(msg).ShowAsync();
            }
        }
    }
}
