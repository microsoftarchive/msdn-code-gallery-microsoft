/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System.Windows;

namespace PhoneVoIPApp.UI
{
    /// <summary>
    /// The view model for the Outgoing Call page
    /// </summary>
    public class OutgoingCallViewModel : BaseViewModel
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public OutgoingCallViewModel()
            : base()
        {
            this.RecipientName = "Larry Zhang";
            this.RecipientNumber = "1-555-555-4321";
        }

        private string recipientName;
        public string RecipientName
        {
            get
            {
                return this.recipientName;
            }

            set
            {
                if (value == null)
                    value = string.Empty;

                this.recipientName = value;
                this.OnPropertyChanged("RecipientName");

                // Update the dial button state whenever the recipient name changes
                this.OnRecipientNameChanged(this.recipientName);
            }
        }

        private string recipientNumber;
        public string RecipientNumber
        {
            get
            {
                return this.recipientNumber;
            }

            set
            {
                if (value == null)
                    value = string.Empty;

                this.recipientNumber = value;
                this.OnPropertyChanged("RecipientNumber");
            }
        }

        private bool isDialButtonEnabled;
        public bool IsDialButtonEnabled
        {
            get
            {
                return this.isDialButtonEnabled;
            }

            set
            {
                if (this.isDialButtonEnabled != value)
                {
                    this.isDialButtonEnabled = value;
                    this.OnPropertyChanged("IsDialButtonEnabled");
                }
            }
        }

        private Visibility disabledReasonTextVisibility;
        public Visibility DisabledReasonTextVisibility
        {
            get
            {
                return this.disabledReasonTextVisibility;
            }

            set
            {
                if (this.disabledReasonTextVisibility != value)
                {
                    this.disabledReasonTextVisibility = value;
                    this.OnPropertyChanged("DisabledReasonTextVisibility");
                }
            }
        }

        /// <summary>
        /// The cached call status has changed
        /// </summary>
        public override void OnCachedCallStatusUpdated()
        {
            base.OnCachedCallStatusUpdated();

            // Update the dial button state if required
            this.UpdateDialButtonState(this.recipientName);
        }

        /// <summary>
        /// The text in the recipient name text box has changed
        /// </summary>
        public void OnRecipientNameChanged(string recipientName)
        {
            this.UpdateDialButtonState(recipientName);
        }

        /// <summary>
        /// Initiate an outoing call
        /// </summary>
        public void MakeOutgoingCall()
        {
            // Ignore the return value - it can be 'false' if a call is already in progress
            BackgroundProcessController.Instance.CallController.InitiateOutgoingCall(this.RecipientName, this.recipientNumber);

            // Disable the button, so the user doesn't press it again and again.
            // The button will get re-enabled if required when the call status changes.
            this.IsDialButtonEnabled = false;
        }

        /// <summary>
        /// Update the state of the 'dial' button
        /// </summary>
        private void UpdateDialButtonState(string recipientName)
        {
            // The dial button is enabled only if there is a non-empty recipient name, and there is no other call in progress.
            this.IsDialButtonEnabled = !string.IsNullOrEmpty(recipientName) && (base.CallStatus == BackEnd.CallStatus.None);

            // If there is already a call in progress, show the disabled reason
            this.DisabledReasonTextVisibility = (base.CallStatus != BackEnd.CallStatus.None) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
