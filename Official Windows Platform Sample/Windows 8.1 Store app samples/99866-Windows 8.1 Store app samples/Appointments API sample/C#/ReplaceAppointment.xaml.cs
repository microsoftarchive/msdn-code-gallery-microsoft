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
    public sealed partial class ReplaceAppointment : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public ReplaceAppointment()
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
        /// Replace an appointment on the user's calendar using the default appointments provider app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Replace_Click(object sender, RoutedEventArgs e)
        {
            // The appointment id argument for ReplaceAppointmentAsync is typically retrieved from AddAppointmentAsync and stored in app data.
            String appointmentIdOfAppointmentToReplace = AppointmentIdTextBox.Text;

            if (String.IsNullOrEmpty(appointmentIdOfAppointmentToReplace))
            {
                ResultTextBlock.Text = "The appointment id cannot be empty";
            }
            else
            {
                // The Appointment argument for ReplaceAppointmentAsync should contain all of the Appointment's properties including those that may have changed.
                var appointment = new Windows.ApplicationModel.Appointments.Appointment();

                // Get the selection rect of the button pressed to replace this appointment
                var rect = GetElementRect(sender as FrameworkElement);

                // ReplaceAppointmentAsync returns an updated appointment id when the appointment was successfully replaced.
                // The updated id may or may not be the same as the original one retrieved from AddAppointmentAsync.
                // An optional instance start time can be provided to indicate that a specific instance on that date should be replaced
                // in the case of a recurring appointment.
                // If the appointment id returned is an empty string, that indicates that the appointment was not replaced.
                String updatedAppointmentId;
                if (InstanceStartDateCheckBox.IsChecked.Value)
                {
                    // Replace a specific instance starting on the date provided.
                    var instanceStartDate = InstanceStartDateDatePicker.Date;
                    updatedAppointmentId = await Windows.ApplicationModel.Appointments.AppointmentManager.ShowReplaceAppointmentAsync(appointmentIdOfAppointmentToReplace, appointment, rect, Windows.UI.Popups.Placement.Default, instanceStartDate);
                }
                else
                {
                    // Replace an appointment that occurs only once or in the case of a recurring appointment, replace the entire series.
                    updatedAppointmentId = await Windows.ApplicationModel.Appointments.AppointmentManager.ShowReplaceAppointmentAsync(appointmentIdOfAppointmentToReplace, appointment, rect, Windows.UI.Popups.Placement.Default);
                }

                if (updatedAppointmentId != String.Empty)
                {
                    ResultTextBlock.Text = "Updated Appointment Id: " + updatedAppointmentId;
                }
                else
                {
                    ResultTextBlock.Text = "Appointment not replaced.";
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
