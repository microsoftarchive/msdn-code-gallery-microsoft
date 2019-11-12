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
using Windows.ApplicationModel.Contacts;

namespace ContactManagerSample
{
    /// <summary>
    /// A page for the 'Show contact card' scenario that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScenarioShowContactCard : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // Length limits allowed by the API
        private const uint MAX_EMAIL_ADDRESS_LENGTH = 321;
        private const uint MAX_PHONE_NUMBER_LENGTH = 50;

        public ScenarioShowContactCard()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// This is the click handler for the 'Show contact card' button.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowContactCardButton_Click(object sender, RoutedEventArgs e)
        {
            if ((this.EmailAddress.Text.Length == 0) && (this.PhoneNumber.Text.Length == 0))
            {
                this.rootPage.NotifyUser("You must enter an email address and/or phone number of the contact for the system to search and show the contact card.", NotifyType.ErrorMessage);
            }
            else
            {
                Contact contact = new Contact();
                if (this.EmailAddress.Text.Length > 0)
                {
                    if (this.EmailAddress.Text.Length <= MAX_EMAIL_ADDRESS_LENGTH)
                    {
                        ContactEmail email = new ContactEmail();
                        email.Address = this.EmailAddress.Text;
                        contact.Emails.Add(email);
                    }
                    else
                    {
                        this.rootPage.NotifyUser("The email address you entered is too long.", NotifyType.ErrorMessage);
                        return;
                    }
                }

                if (this.PhoneNumber.Text.Length > 0)
                {
                    if (this.PhoneNumber.Text.Length <= MAX_PHONE_NUMBER_LENGTH)
                    {
                        ContactPhone phone = new ContactPhone();
                        phone.Number = this.PhoneNumber.Text;
                        contact.Phones.Add(phone);
                    }
                    else
                    {
                        this.rootPage.NotifyUser("The phone number you entered is too long.", NotifyType.ErrorMessage);
                        return;
                    }
                }

                // Get the selection rect of the button pressed to show contact card.
                Rect rect = Helper.GetElementRect(sender as FrameworkElement);

                ContactManager.ShowContactCard(contact, rect, Windows.UI.Popups.Placement.Default);
                this.rootPage.NotifyUser("ContactManager.ShowContactCard() was called.", NotifyType.StatusMessage);
            }
        }
    }
}
