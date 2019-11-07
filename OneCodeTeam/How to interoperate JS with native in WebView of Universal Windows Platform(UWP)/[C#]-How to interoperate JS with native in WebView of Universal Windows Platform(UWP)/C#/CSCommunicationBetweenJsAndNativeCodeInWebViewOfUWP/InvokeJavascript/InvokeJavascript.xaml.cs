using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CSCommunicationBetweenJsAndNativeCodeInWebViewOfUWP.InvokeJavascript
{
    public sealed partial class InvokeJavascript : Page
    {
        public InvokeJavascript()
        {
            this.InitializeComponent();
        }

        private async void RunJsAndNoResultBtn_Click(object sender, RoutedEventArgs e)
        {
            string nowStr = string.Format("This time is from Native code:{0}", DateTime.Now.ToString());

            try
            {
                //the first param is the function name of javescript,
                //the second is the parameter of function
                await MainWebView.InvokeScriptAsync("invokeJSAndNoResult", new string[] { nowStr });
            }
            catch (Exception ex)
            {
                MessageDialog showDialog = new MessageDialog(ex.Message);
                await showDialog.ShowAsync();
            }
        }

        private async void RunJsAndResultBtn_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                //if function haven't a parameter, you can give a null or not.
                string result = await MainWebView.InvokeScriptAsync("getDateTimeFromJS", null);

                RunJsAndResultBtnResultContainer.Text = result;
            }
            catch (Exception ex)
            {
                MessageDialog showDialog = new MessageDialog(ex.Message);
                await showDialog.ShowAsync();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
