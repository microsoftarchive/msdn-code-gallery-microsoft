//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace Appointments
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RemoveAppointment : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public RemoveAppointment()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// Removes the appointment associated with a particular appointment id string from the defaul appointment provider app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            // The appointment id argument for ShowRemoveAppointmentAsync is typically retrieved from AddAppointmentAsync and stored in app data.
            String appointmentId = AppointmentIdTextBox.Text;

            // The appointment id cannot be null or empty.
            if (String.IsNullOrEmpty(appointmentId))
            {
                ResultTextBlock.Text = "The appointment id cannot be empty";
            }
            else
            {
                // Get the selection rect of the button pressed to remove this appointment
                var rect = GetElementRect(sender as FrameworkElement);

                // ShowRemoveAppointmentAsync returns a boolean indicating whether or not the appointment related to the appointment id given was removed.
                // An optional instance start time can be provided to indicate that a specific instance on that date should be removed
                // in the case of a recurring appointment.
                bool removed;
                if (InstanceStartDateCheckBox.IsChecked.Value)
                {
                    // Remove a specific instance starting on the date provided.
                    var instanceStartDate = InstanceStartDateDatePicker.Date;
                    removed = await Windows.ApplicationModel.Appointments.AppointmentManager.ShowRemoveAppointmentAsync(appointmentId, rect, Windows.UI.Popups.Placement.Default, instanceStartDate);
                }
                else
                {
                    // Remove an appointment that occurs only once or in the case of a recurring appointment, replace the entire series.
                    removed = await Windows.ApplicationModel.Appointments.AppointmentManager.ShowRemoveAppointmentAsync(appointmentId, rect, Windows.UI.Popups.Placement.Default);
                }

                if (removed)
                {
                    ResultTextBlock.Text = "Appointment removed";
                }
                else
                {
                    ResultTextBlock.Text = "Appointment not removed";
                }
            }
        }

        private Windows.Foundation.Rect GetElementRect(FrameworkElement element)
        {
            Windows.UI.Xaml.Media.GeneralTransform buttonTransform = element.TransformToVisual(null);
            Windows.Foundation.Point point = buttonTransform.TransformPoint(new Windows.Foundation.Point());
            return new Windows.Foundation.Rect(point, new Windows.Foundation.Size(element.ActualWidth, element.ActualHeight));
        }
    }
}
