/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using Microsoft.Phone.Tasks;

namespace PhoneVoIPApp.UI
{
    /// <summary>
    /// The view model for the Main page
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MainViewModel()
            : base()
        {
        }

        private bool isMakeOutgoingCallButtonEnabled;
        public bool IsMakeOutgoingCallButtonEnabled
        {
            get
            {
                return this.isMakeOutgoingCallButtonEnabled;
            }

            set
            {
                if (this.isMakeOutgoingCallButtonEnabled != value)
                {
                    this.isMakeOutgoingCallButtonEnabled = value;
                    this.OnPropertyChanged("IsMakeOutgoingCallButtonEnabled");
                }
            }
        }

        private bool isViewCallStatusButtonEnabled;
        public bool IsViewCallStatusButtonEnabled
        {
            get
            {
                return this.isViewCallStatusButtonEnabled;
            }

            set
            {
                if (this.isViewCallStatusButtonEnabled != value)
                {
                    this.isViewCallStatusButtonEnabled = value;
                    this.OnPropertyChanged("IsViewCallStatusButtonEnabled");
                }
            }
        }

        private bool isPushUriAvailable;
        public bool IsPushUriAvailable
        {
            get
            {
                return this.isPushUriAvailable;
            }

            set
            {
                if (this.isPushUriAvailable != value)
                {
                    this.isPushUriAvailable = value;
                    this.OnPropertyChanged("IsPushUriAvailable");
                }
            }
        }

        /// <summary>
        /// Email the push URI for this app
        /// </summary>
        internal void EmailPushUri()
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            
            Uri pushChannelUri = ((App)App.Current).PushChannelUri;
            if (pushChannelUri != null)
            {
                emailComposeTask.Body = pushChannelUri.ToString();
            }

            emailComposeTask.Subject = this.ApplicationTitle;

            emailComposeTask.Show();
        }

        /// <summary>
        /// The cached call status has changed
        /// </summary>
        public override void OnCachedCallStatusUpdated()
        {
            base.OnCachedCallStatusUpdated();

            // The cached call status has changed
            this.UpdateButtonStates();
        }

        /// <summary>
        /// The user navigated to this page
        /// </summary>
        public override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs nee)
        {
            base.OnNavigatedTo(nee);

            // Subscribe to changes in the push URI
            App theApp = (App)App.Current;
            theApp.PushChannelUriChanged += this.PushChannelUriChanged;

            // Set the initial push channel value
            this.PushChannelUriChanged(this, theApp.PushChannelUri);
        }

        /// <summary>
        /// The user navigated away from this page
        /// </summary>
        public override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs nee)
        {
            base.OnNavigatedFrom(nee);

            // Unsubscribe to changes in the push URI
            App theApp = (App)App.Current;
            theApp.PushChannelUriChanged -= this.PushChannelUriChanged;
        }

        /// <summary>
        /// Called when the push channel URI for this app changes
        /// </summary>
        private void PushChannelUriChanged(object sender, Uri pushChannelUri)
        {
            // This call is from some background thread - dispatch to the UI thread
            this.Page.Dispatcher.BeginInvoke(() =>
            {
                this.IsPushUriAvailable = pushChannelUri != null;
            });
        }

        /// <summary>
        /// Update the states of the buttons on this page
        /// </summary>
        private void UpdateButtonStates()
        {
            bool callExists = base.CallStatus != BackEnd.CallStatus.None;

            // Outgoing call button is enabled only if there is no call already
            this.IsMakeOutgoingCallButtonEnabled = !callExists;

            // View call status button is enabled only if there is a call
            this.IsViewCallStatusButtonEnabled = callExists;
        }
    }
}
