using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_TwoWnds
{
    public delegate void SendTypingNotification();
    public delegate void SendTypingCompletedNotification();

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WndAgent : Window
    {
        public event SendTypingNotification SendTypingEvent;
        public event SendTypingCompletedNotification SendTypingCompletedEvent;

        public WndAgent()
        {
            InitializeComponent();
         
            this.Loaded += WndAgent_Loaded;
            this.PreviewKeyDown += WndAgent_PreviewKeyDown;

        }


        /// <summary>
        /// handle ESC key to exit the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WndAgent_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        void WndAgent_Loaded(object sender, RoutedEventArgs e)
        {
           
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowState = WindowState.Maximized;
        }


        /// <summary>
        /// when this button is clicked, indication agent has finish inputing info binded in Customer window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
          //  App._customData = new CustomData { UserName = AccountName.Text, Info = NotifyInfo.Text };
            SendTypingCompletedEvent();

            App._customData.UserName = AccountName.Text;
            App._customData.Info = NotifyInfo.Text;
            App._customData.IsVisible = true;
        }

        public void wndCustomer_SendNotifyEvent()
        {
            //throw new NotImplementedException();
           // MessageBox.Show("a");
            if (App._customMsgBoxIsOpen)
            {
                App._customMsgBox.Show();
                App._customMsgBoxIsOpen = false;
            }
         
            
        }


        /// <summary>
        /// all the input events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Input_GotFocus(object sender, RoutedEventArgs e)
        {
            SendTypingEvent();
        }

    }
}
