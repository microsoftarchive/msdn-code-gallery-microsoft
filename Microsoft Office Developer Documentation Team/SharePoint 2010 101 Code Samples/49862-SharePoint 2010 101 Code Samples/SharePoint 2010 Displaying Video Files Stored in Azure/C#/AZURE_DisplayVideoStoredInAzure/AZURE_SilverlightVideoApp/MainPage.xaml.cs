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

namespace AZURE_SilverlightVideoApp
{
    public partial class MainPage : UserControl
    {
        public string SiteUrl { get; set; }

        public MainPage()
        {
            InitializeComponent();
            //Display the video's Source Uri
            txtLocation.Text = mediaVideo.Source.ToString();
        }

    }
}
