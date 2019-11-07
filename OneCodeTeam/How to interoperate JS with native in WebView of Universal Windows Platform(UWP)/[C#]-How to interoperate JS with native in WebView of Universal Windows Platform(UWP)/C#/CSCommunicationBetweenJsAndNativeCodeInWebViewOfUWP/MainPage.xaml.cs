using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CSCommunicationBetweenJsAndNativeCodeInWebViewOfUWP
{

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void GoToInvokeNativeCodeAddWebAllowedObjectBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(InvokeNativeCode.InvokeNativeCodeAddWebAllowedObject));
        }

        private void GoToInvokeNativeCodeScriptNotifyBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(InvokeNativeCode.InvokeNativeCodeScriptNotify));
        }

        private void GoToInvokeJavascriptBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(InvokeJavascript.InvokeJavascript));
        }
    }
}
