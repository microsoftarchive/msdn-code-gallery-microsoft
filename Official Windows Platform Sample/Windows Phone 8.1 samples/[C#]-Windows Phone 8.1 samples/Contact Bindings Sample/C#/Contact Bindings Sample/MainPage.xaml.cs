using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Contact_Bindings_Sample.Resources;
using Microsoft.Phone.Tasks;
using Windows.Phone.PersonalInformation;
using System.Diagnostics;
using System.Threading.Tasks;
using Shared_Library;

namespace Contact_Bindings_Sample
{
    public partial class MainPage : PhoneApplicationPage
    {
        private SaveContactTask saveContactTask;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

            saveContactTask = new SaveContactTask();
            saveContactTask.Completed += new EventHandler<SaveContactResult>(saveContactTask_Completed);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            uriNavigationText.Text = "Navigation URI:\n" + e.Uri.ToString();
            base.OnNavigatedTo(e);
        }

        void saveContactTask_Completed(object sender, SaveContactResult e)
        {
            switch (e.TaskResult)
            {
                ////Logic for when the contact was saved successfully
                //case TaskResult.OK:
                //    MessageBox.Show("Contact saved.");
                //    break;

                ////Logic for when the task was cancelled by the user
                //case TaskResult.Cancel:
                //    MessageBox.Show("Save cancelled.");
                //    break;

                //Logic for when the contact could not be saved
                case TaskResult.None:
                    MessageBox.Show("Contact could not be saved. Error=" + e.Error.Message);
                    break;
            }
        }

        private void Add_Contact1_Click(object sender, RoutedEventArgs e)
        {
            saveContactTask.FirstName = "Terry";
            saveContactTask.LastName = "Adams";
            saveContactTask.MobilePhone = "2065550001";
            saveContactTask.PersonalEmail = "terryadams@example.com";

            saveContactTask.Show();
        }

        private void Add_Contact2_Click(object sender, RoutedEventArgs e)
        {
            saveContactTask.FirstName = "Mark";
            saveContactTask.LastName = "Hanson";
            saveContactTask.MobilePhone = "2065550002";
            saveContactTask.PersonalEmail = "markhanson@example.com";

            saveContactTask.Show();
        }

        private void Add_Contact3_Click(object sender, RoutedEventArgs e)
        {
            saveContactTask.FirstName = "David";
            saveContactTask.LastName = "Pelton";
            saveContactTask.MobilePhone = "2065550003";
            saveContactTask.PersonalEmail = "davidpelton@example.com";

            saveContactTask.Show();
        }

        async private void Login_Click(object sender, RoutedEventArgs e)
        {
            //// Do sign-in logic here and store your authentication token using the AppSettings class
            App.MyAppSettings.AuthTokenSetting = ServerApi.GetAuthTokenFromWebService();
            App.MyAppSettings.UserIdSetting = ServerApi.GetUserIdFromWebService();

            // log existing bindings
            {
                ContactBindingManager manager = await ContactBindings.GetAppContactBindingManagerAsync();
                foreach (ContactBinding binding in await manager.GetContactBindingsAsync())
                {
                    Logger.Log("MainPage", "existing binding = " + binding.RemoteId);
                }
            }
            
            // Add contact bindings for every contact information that we get from the web service
            // This bindings will be automatically linked to existent phone contacts.
            // The auto-linking is based on name, email or phone numbers match.
            await CreateContactBindingsAsync();

            // log new bindings
            {
                ContactBindingManager manager = await ContactBindings.GetAppContactBindingManagerAsync();
                foreach (ContactBinding binding in await manager.GetContactBindingsAsync())
                {
                    Logger.Log("MainPage", "binding = " + binding.RemoteId);
                }
            }

            MessageBox.Show("Done!");
        }

        async private void Logout_Click(object sender, RoutedEventArgs e)
        {
            ContactBindingManager bindingManager = await ContactBindings.GetAppContactBindingManagerAsync();
            await bindingManager.DeleteAllContactBindingsAsync();

            // Do sign-out logic here and clear the authentication token using the AppSettings class
            App.MyAppSettings.AuthTokenSetting = "";
            App.MyAppSettings.UserIdSetting = "";

            MessageBox.Show("Done!");
        }

        private async Task CreateContactBindingsAsync()
        {
            ContactBindingManager bindingManager = await ContactBindings.GetAppContactBindingManagerAsync();

            // Simulate call to web service
            List<ServerApi.ContactBinding> bindings = ServerApi.GetContactsFromWebServiceAsync();

            foreach(ServerApi.ContactBinding binding in bindings)
            {
                ContactBinding myBinding = bindingManager.CreateContactBinding(binding.RemoteId);

                // This information is not displayed on the Contact Page in the People Hub app, but 
                // is used to automatically link the contact binding with existent phone contacts.
                // Add as much information as possible for the ContactBinding to increase the 
                // chances to find a matching Contact on the phone.
                myBinding.FirstName = binding.GivenName;
                myBinding.LastName = binding.FamilyName;
                myBinding.EmailAddress1 = binding.Email;
                myBinding.Name = binding.CodeName;                

                // Don't crash if one binding fails, log the error and continue saving
                try
                {
                    await bindingManager.SaveContactBindingAsync(myBinding);
                }
                catch (Exception e)
                {
                    Logger.Log("MainPage", "Binding (" + binding.RemoteId + ") failed to save. " + e.Message);
                }
            }

        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}
