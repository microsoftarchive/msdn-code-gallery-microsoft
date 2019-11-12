using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Messaging;
using System.Windows.Shapes;

namespace SilverlightMessageProvider
{
    /// <summary>
    /// This Silverlight application sends a message using Silverlight's local message
    /// sender class. The InitParameters collection is used to store the ID of the 
    /// Silverlight application to send a message to. 
    /// </summary>
    public partial class MainPage : UserControl
    {
        internal LocalMessageSender mySender;

        public MainPage()
        {
            InitializeComponent();
        }

        private void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            //Send the message
            mySender.SendAsync(textBoxMessage.Text);
        }
    }
}
