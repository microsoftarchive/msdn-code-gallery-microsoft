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
using System.Windows.Shapes;

namespace WPF_TwoWnds
{
    public delegate void SendNotification();
    /// <summary>
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class WndCustomer : Window
    {
        public event SendNotification SendNotifyEvent;

        public static readonly RoutedEvent TapEvent = EventManager.RegisterRoutedEvent(
        "Tap", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WndCustomer));

        // Provide CLR accessors for the event 
        public event RoutedEventHandler Tap
        {
            add { AddHandler(TapEvent, value); }
            remove { RemoveHandler(TapEvent, value); }
        }

        // This method raises the Tap event 
        void RaiseTapEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(WndCustomer.TapEvent);
            RaiseEvent(newEventArgs);
        }




        public WndCustomer()
        {
            InitializeComponent();
            this.Loaded += WndCustomer_Loaded;
            this.PreviewKeyDown += WndCustomer_PreviewKeyDown;
            this.DataContext = App._customData;
        }

        void WndCustomer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        void WndCustomer_Loaded(object sender, RoutedEventArgs e)
        {
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowState = WindowState.Maximized;
        }

        /// <summary>
        /// cx's action, press Ok button indication he agreed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            //RaiseTapEvent();
            SendNotifyEvent();

        }

        public void wndAgent_SendTypingEvent()
        {            
            this.IsEnabled = false;
        }

        public void wndAgent_SendTypingCompletedEvent()
        {
            this.IsEnabled = true;
        }

        
    }
}
