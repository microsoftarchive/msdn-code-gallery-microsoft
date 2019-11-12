using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace WPF_TwoWnds
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static CustomData _customData;
        public static CustomMsgBox _customMsgBox;
        public static bool _customMsgBoxIsOpen=true;
        protected override void OnStartup(StartupEventArgs e)
        {
            App._customData = new CustomData();
            App._customData.IsVisible = false;


            base.OnStartup(e);
            WndAgent wndAgent = new WndAgent();
            WndCustomer wndCustomer = new WndCustomer();

            Screen screenAgent = Screen.AllScreens[0];
            Screen screenCustomer = Screen.AllScreens[1];

            Rectangle rectAgent = screenAgent.WorkingArea;
            Rectangle rectCustomer = screenCustomer.WorkingArea;

            wndAgent.Top = rectAgent.Top;
            wndAgent.Left = rectAgent.Left;


            wndCustomer.Top = rectCustomer.Top;
            wndCustomer.Left = rectCustomer.Left;


            _customMsgBox = new CustomMsgBox();

            _customMsgBox.Top = rectAgent.Top + 100;
            _customMsgBox.Left = rectAgent.Left + 500;


            wndCustomer.SendNotifyEvent += wndAgent.wndCustomer_SendNotifyEvent;
            wndAgent.SendTypingEvent += wndCustomer.wndAgent_SendTypingEvent;
            wndAgent.SendTypingCompletedEvent += wndCustomer.wndAgent_SendTypingCompletedEvent;

            wndAgent.Show();
            wndCustomer.Show();

            wndCustomer.Owner = wndAgent;


       


        }

        
    }
}
