using System;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json.Linq;

namespace CSCommunicationBetweenJsAndNativeCodeInWebViewOfUWP.InvokeNativeCode
{
    public sealed partial class InvokeNativeCodeScriptNotify : Page
    {
        public InvokeNativeCodeScriptNotify()
        {
            this.InitializeComponent();
        }


        //Because this is using webView event, event is no result,so you can't use this way in the scene that get result.
        //but when you operator UI, this way is very esay.
        private void MainWebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            //the notify value just a string,you can provide a format json,xml,or just a string
            //in this sample I provide a string that is a json format,and I use it to simulation method route
            JObject jo = JObject.Parse(e.Value);

            string method = jo.SelectToken("method").Value<string>();

            switch (method)
            {
                case "GetNowTime":
                    GetNowTime();
                    break;
                case "SetValueToNative":
                    string param = jo.SelectToken("param").Value<string>();
                    SetValueToNative(param);
                    break;
            }
        }

        private void GetNowTime()
        {
            MessageContainer.Text = DateTime.Now.ToString();
        }

        private void SetValueToNative(string param)
        {
            MessageContainer.Text = param;
        }

        private void Back_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
