//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

namespace AzureMobileSendEmail
{
    using System;
    using System.Threading.Tasks;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class FeedbackContent : UserControl
    {
        public FeedbackContent()
        {
            this.InitializeComponent();
        }

        public event EventHandler FeedbackSent;

        private enum NotifyType
        {
            ErrorMessage,
            StatusMessage,
            SuccessMessage
        }

        private async void SendFeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            await this.SendFeedback();
        }

        private async Task SendFeedback()
        {
            this.NotifyUser("Sending feedback...", NotifyType.StatusMessage);
            this.FeedbackText.IsEnabled = false;
            this.SendFeedbackButton.IsEnabled = false;

            try
            {
                var feedback = new Feedback
                {
                    Text = this.FeedbackText.Text,
                    SentDate = DateTime.Now
                };

                // Insert feedback in Windows Azure Mobile Services
                await App.MobileService.GetTable<Feedback>().InsertAsync(feedback);

                this.NotifyUser("Feedback sent successfully!", NotifyType.SuccessMessage);

                // Set the EventHandler to trigger the close of the panel
                if (this.FeedbackSent != null)
                {
                    await Task.Delay(3000);
                    this.FeedbackSent(this, null);
                }
            }
            catch (Exception ex)
            {
                this.NotifyUser("There was an error while sending the feedback:" 
                    + Environment.NewLine + Environment.NewLine + ex.Message, NotifyType.ErrorMessage);
            }          

            this.FeedbackText.IsEnabled = true;
            this.SendFeedbackButton.IsEnabled = true;
        }

        private void NotifyUser(string message, NotifyType type)
        {
            this.StatusBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
            switch (type)
            {
                case NotifyType.ErrorMessage:
                    StatusBlock.Style = AzureMobileSendEmail.App.Current.Resources["ErrorStyle"] as Style;
                    break;
                case NotifyType.StatusMessage:
                    StatusBlock.Style = AzureMobileSendEmail.App.Current.Resources["StatusStyle"] as Style;
                    break;
                case NotifyType.SuccessMessage:
                    StatusBlock.Style = AzureMobileSendEmail.App.Current.Resources["SuccessStyle"] as Style;
                    break;
            }

            StatusBlock.Text = message;
        }
    }
}