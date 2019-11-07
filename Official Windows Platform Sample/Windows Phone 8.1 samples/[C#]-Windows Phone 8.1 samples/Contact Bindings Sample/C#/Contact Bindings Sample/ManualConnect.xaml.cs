using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Windows.Phone.PersonalInformation;

namespace Contact_Bindings_Sample
{
    /// <summary>
    /// This is an optional scenario. Needs the "People_Connect_Manual" extension in WMAppManifest.xml
    /// 
    /// When the user choses to manually add a binding for this app, the new binding will
    /// be displayed in the connect pivot but our app doesn't who it belongs to.
    /// 
    /// When the user clicks on the new binding the app will be invoked with this URI:
    /// /MainPage.xaml?action=Connect_Contact&binding_id=[id]
    /// and the UriMapper in App.xaml.cs will rename it to
    /// /ManualConnect.xaml?action=Connect_Contact&binding_id=[id]
    /// 
    /// This page needs to show an UI that let's the user chose which contact information to bind to.
    /// </summary>
    public partial class ManualConnect : PhoneApplicationPage
    {
        string bindingId;
        ContactBindingManager contactBindingManager;

        public ManualConnect()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string[] uri = e.Uri.ToString().Split('=');

            if (uri.Length > 1)
            {
                bindingId = Uri.UnescapeDataString(uri[2]);
            }

            contactBindingManager = await ContactBindings.GetAppContactBindingManagerAsync();

            EntityList.ItemsSource = await contactBindingManager.GetContactBindingsAsync();

            EntityList.SelectionChanged += EntityList_SelectionChanged;
        }

        async void EntityList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EntityList.SelectedItem != null)
            {
                ContactBinding entity = EntityList.SelectedItem as ContactBinding;
                await contactBindingManager.CreateContactBindingTileAsync(bindingId, entity);

                System.Diagnostics.Debug.WriteLine(string.Format("Bound [{0}] to entity with name [{1}]", bindingId, entity.Name));

                NavigationService.Navigate(new Uri(string.Format("/MainPage.xaml?action=Show_Contact&contact_ids={0}", entity.RemoteId), UriKind.Relative));
            }
        }
    }
}
