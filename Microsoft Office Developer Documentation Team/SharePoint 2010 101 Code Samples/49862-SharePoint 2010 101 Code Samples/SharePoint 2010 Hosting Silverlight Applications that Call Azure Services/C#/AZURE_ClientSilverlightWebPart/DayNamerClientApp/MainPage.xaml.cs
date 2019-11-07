using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.SharePoint.Client;
using DayNamerClientApp.DayNamerServiceReference;

namespace DayNamerClientApp
{
    //This is the Silverlight application. Before you run this 
    //project, remove the DayNamerServiceReference Service Reference
    //and add a service reference to wherever you deployed the 
    //WCF service. E.g. http://http://YourHostedServiceName.cloudapp.net/DayInfoService.svc
    public partial class MainPage : UserControl
    {
        public string SiteUrl { get; set; }

        public MainPage()
        {
            InitializeComponent();
        }

        private void getTodayButton_Click(object sender, RoutedEventArgs e)
        {
            //Create the client proxy object
            DayInfoClient client = new DayInfoClient();
            //Call the TodayIs method asynchronously
            client.TodayIsAsync();
            //Handle its completed event
            client.TodayIsCompleted += new EventHandler<TodayIsCompletedEventArgs>(client_TodayIsCompleted);
        }

        private void client_TodayIsCompleted(object sender, TodayIsCompletedEventArgs args)
        {
            //Display the result
            resultsLabel.Content = args.Result.ToString();
        }

    }
}
