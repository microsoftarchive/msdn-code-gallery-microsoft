using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Contact_Bindings_Sample
{
    public partial class ShowContact : PhoneApplicationPage
    {
        public ShowContact()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string[] uri = e.Uri.ToString().Split('=');

            if (uri.Length > 1)
            {
                string remoteId = Uri.UnescapeDataString(uri[2]);
                contactInformation.Text = "Displaying contact information for contact with remote ID = " + remoteId;
            }
        }
    }
}
