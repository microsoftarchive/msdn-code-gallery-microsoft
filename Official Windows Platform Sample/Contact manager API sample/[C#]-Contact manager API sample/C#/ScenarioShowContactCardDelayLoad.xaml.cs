//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;

namespace ContactManagerSample
{
    /// <summary>
    /// A page for the 'Show contact card with delay loaded data' scenario that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScenarioShowContactCardDelayLoad : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public ScenarioShowContactCardDelayLoad()
        {
            this.InitializeComponent();
        }

        private async Task<Contact> DownloadContactDataAsync(Contact contact)
        {
            // Simulate the download latency by delaying the execution by 2 seconds.
            await Task.Delay(2000);
            return await Task.Run<Contact>(() =>
                {
                    // Add more data to the contact object.
                    ContactEmail workEmail = new ContactEmail();
                    workEmail.Address = "kim@adatum.com";
                    workEmail.Kind = ContactEmailKind.Work;
                    contact.Emails.Add(workEmail);

                    ContactPhone homePhone = new ContactPhone();
                    homePhone.Number = "(444) 555-0001";
                    homePhone.Kind = ContactPhoneKind.Home;
                    contact.Phones.Add(homePhone);

                    ContactPhone workPhone = new ContactPhone();
                    workPhone.Number = "(245) 555-0123";
                    workPhone.Kind = ContactPhoneKind.Work;
                    contact.Phones.Add(workPhone);

                    ContactPhone mobilePhone = new ContactPhone();
                    mobilePhone.Number = "(921) 555-0187";
                    mobilePhone.Kind = ContactPhoneKind.Mobile;
                    contact.Phones.Add(mobilePhone);

                    ContactAddress address = new ContactAddress();
                    address.StreetAddress = "123 Main St";
                    address.Locality = "Redmond";
                    address.Region = "WA";
                    address.Country = "USA";
                    address.PostalCode = "23456";
                    address.Kind = ContactAddressKind.Home;
                    contact.Addresses.Add(address);

                    return contact;
                });
        }
        /// <summary>
        /// This is the click handler for the 'Show contact card with delayed data loader' button.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ShowContactCardDelayLoadButton_Click(object sender, RoutedEventArgs e)
        {
            // Create contact object with small set of initial data to display.
            Contact contact = new Contact();
            contact.FirstName = "Kim";
            contact.LastName = "Abercrombie";

            ContactEmail email = new ContactEmail();
            email.Address = "kim@contoso.com";
            contact.Emails.Add(email);

            // Get the selection rect of the button pressed to show contact card.
            Rect rect = Helper.GetElementRect(sender as FrameworkElement);
            using (ContactCardDelayedDataLoader dataLoader = ContactManager.ShowDelayLoadedContactCard(
                contact, 
                rect,
                Windows.UI.Popups.Placement.Below // The contact card placement can change when it is updated with more data. For improved user experience, specify placement 
                                                  // of the card so that it has space to grow and will not need to be repositioned. In this case, default placement first places 
                                                  // the card above the button because the card is small, but after the card is updated with more data, the operating system moves 
                                                  // the card below the button to fit the card's expanded size. Specifying that the contact card is placed below at the beginning 
                                                  // avoids this repositioning.
                ))
            {
                string message = "ContactManager.ShowDelayLoadedContactCard() was called.\r\n";
                this.rootPage.NotifyUser(message, NotifyType.StatusMessage);

                // Simulate downloading more data from the network for the contact.
                message += "Downloading data ...\r\n";
                this.rootPage.NotifyUser(message, NotifyType.StatusMessage);

                Contact fullContact = await DownloadContactDataAsync(contact);
                if (fullContact != null)
                {
                    // We get more data - update the contact card with the full set of contact data.
                    dataLoader.SetData(fullContact);

                    message += "ContactCardDelayedDataLoader.SetData() was called.\r\n";
                    this.rootPage.NotifyUser(message, NotifyType.StatusMessage);
                }
            }
        }
    }
}
