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
    public sealed partial class AppointmentProperties : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public AppointmentProperties()
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
        /// Creates an Appointment based on the input fields and validates it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            bool isAppointmentValid = true;
            var appointment = new Windows.ApplicationModel.Appointments.Appointment();

            // StartTime
            var date = StartTimeDatePicker.Date;
            var time = StartTimeTimePicker.Time;
            var timeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
            var startTime = new DateTimeOffset(date.Year, date.Month, date.Day, time.Hours, time.Minutes, 0, timeZoneOffset);
            appointment.StartTime = startTime;

            // Subject
            appointment.Subject = SubjectTextBox.Text;

            if (appointment.Subject.Length > 255)
            {
                isAppointmentValid = false;
                ResultTextBlock.Text = "The subject cannot be greater than 255 characters.";
            }

            // Location
            appointment.Location = LocationTextBox.Text;

            if (appointment.Location.Length > 32768)
            {
                isAppointmentValid = false;
                ResultTextBlock.Text = "The location cannot be greater than 32,768 characters.";
            }

            // Details
            appointment.Details = DetailsTextBox.Text;

            if (appointment.Details.Length > 1073741823)
            {
                isAppointmentValid = false;
                ResultTextBlock.Text = "The details cannot be greater than 1,073,741,823 characters.";
            }

            // Duration
            if (DurationComboBox.SelectedIndex == 0)
            {
                // 30 minute duration is selected
                appointment.Duration = TimeSpan.FromMinutes(30);
            }
            else
            {
                // 1 hour duration is selected
                appointment.Duration = TimeSpan.FromHours(1);
            }

            // All Day
            appointment.AllDay = AllDayCheckBox.IsChecked.Value;

            // Reminder
            if (ReminderCheckBox.IsChecked.Value)
            {
                switch (ReminderComboBox.SelectedIndex)
                {
                    case 0:
                        appointment.Reminder = TimeSpan.FromMinutes(15);
                        break;
                    case 1:
                        appointment.Reminder = TimeSpan.FromHours(1);
                        break;
                    case 2:
                        appointment.Reminder = TimeSpan.FromDays(1);
                        break;
                }
            }

            //Busy Status
            switch (BusyStatusComboBox.SelectedIndex)
            {
                case 0:
                    appointment.BusyStatus = Windows.ApplicationModel.Appointments.AppointmentBusyStatus.Busy;
                    break;
                case 1:
                    appointment.BusyStatus = Windows.ApplicationModel.Appointments.AppointmentBusyStatus.Tentative;
                    break;
                case 2:
                    appointment.BusyStatus = Windows.ApplicationModel.Appointments.AppointmentBusyStatus.Free;
                    break;
                case 3:
                    appointment.BusyStatus = Windows.ApplicationModel.Appointments.AppointmentBusyStatus.OutOfOffice;
                    break;
                case 4:
                    appointment.BusyStatus = Windows.ApplicationModel.Appointments.AppointmentBusyStatus.WorkingElsewhere;
                    break;
            }

            // Sensitivity
            switch (SensitivityComboBox.SelectedIndex)
            {
                case 0:
                    appointment.Sensitivity = Windows.ApplicationModel.Appointments.AppointmentSensitivity.Public;
                    break;
                case 1:
                    appointment.Sensitivity = Windows.ApplicationModel.Appointments.AppointmentSensitivity.Private;
                    break;
            }

            // Uri
            if (UriTextBox.Text.Length > 0)
            {
                try
                {
                    appointment.Uri = new System.Uri(UriTextBox.Text);
                }
                catch (Exception)
                {
                    isAppointmentValid = false;
                    ResultTextBlock.Text = "The Uri provided is invalid.";
                }
            }

            // Organizer
            // Note: Organizer can only be set if there are no invitees added to this appointment.
            if (OrganizerRadioButton.IsChecked.Value)
            {
                var organizer = new Windows.ApplicationModel.Appointments.AppointmentOrganizer();

                // Organizer Display Name
                organizer.DisplayName = OrganizerDisplayNameTextBox.Text;

                if (organizer.DisplayName.Length > 256)
                {
                    isAppointmentValid = false;
                    ResultTextBlock.Text = "The organizer display name cannot be greater than 256 characters.";
                }
                else
                {
                    // Organizer Address (e.g. Email Address)
                    organizer.Address = OrganizerAddressTextBox.Text;

                    if (organizer.Address.Length > 321)
                    {
                        isAppointmentValid = false;
                        ResultTextBlock.Text = "The organizer address cannot be greater than 321 characters.";
                    }
                    else if (organizer.Address.Length == 0)
                    {
                        isAppointmentValid = false;
                        ResultTextBlock.Text = "The organizer address must be greater than 0 characters.";
                    }
                    else
                    {
                        appointment.Organizer = organizer;
                    }
                }
            }

            // Invitees
            // Note: If the size of the Invitees list is not zero, then an Organizer cannot be set.
            if (InviteeRadioButton.IsChecked.Value)
            {
                var invitee = new Windows.ApplicationModel.Appointments.AppointmentInvitee();

                // Invitee Display Name
                invitee.DisplayName = InviteeDisplayNameTextBox.Text;

                if (invitee.DisplayName.Length > 256)
                {
                    isAppointmentValid = false;
                    ResultTextBlock.Text = "The invitee display name cannot be greater than 256 characters.";
                }
                else
                {
                    // Invitee Address (e.g. Email Address)
                    invitee.Address = InviteeAddressTextBox.Text;

                    if (invitee.Address.Length > 321)
                    {
                        isAppointmentValid = false;
                        ResultTextBlock.Text = "The invitee address cannot be greater than 321 characters.";
                    }
                    else if (invitee.Address.Length == 0)
                    {
                        isAppointmentValid = false;
                        ResultTextBlock.Text = "The invitee address must be greater than 0 characters.";
                    }
                    else
                    {
                        // Invitee Role
                        switch (RoleComboBox.SelectedIndex)
                        {
                            case 0:
                                invitee.Role = Windows.ApplicationModel.Appointments.AppointmentParticipantRole.RequiredAttendee;
                                break;
                            case 1:
                                invitee.Role = Windows.ApplicationModel.Appointments.AppointmentParticipantRole.OptionalAttendee;
                                break;
                            case 2:
                                invitee.Role = Windows.ApplicationModel.Appointments.AppointmentParticipantRole.Resource;
                                break;
                        }

                        // Invitee Response
                        switch (ResponseComboBox.SelectedIndex)
                        {
                            case 0:
                                invitee.Response = Windows.ApplicationModel.Appointments.AppointmentParticipantResponse.None;
                                break;
                            case 1:
                                invitee.Response = Windows.ApplicationModel.Appointments.AppointmentParticipantResponse.Tentative;
                                break;
                            case 2:
                                invitee.Response = Windows.ApplicationModel.Appointments.AppointmentParticipantResponse.Accepted;
                                break;
                            case 3:
                                invitee.Response = Windows.ApplicationModel.Appointments.AppointmentParticipantResponse.Declined;
                                break;
                            case 4:
                                invitee.Response = Windows.ApplicationModel.Appointments.AppointmentParticipantResponse.Unknown;
                                break;
                        }

                        appointment.Invitees.Add(invitee);
                    }
                }
            }

            if (isAppointmentValid)
            {
                ResultTextBlock.Text = "The appointment was created successfully and is valid.";
            }
        }

        /// <summary>
        /// Organizer and Invitee properties are mutually exclusive.
        /// This radio button enables the organizer properties while disabling the invitees.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrganizerRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            OrganizerStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            InviteeStackPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        /// <summary>
        /// Organizer and Invitee properties are mutually exclusive.
        /// This radio button enables the invitees properties while disabling the organizer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InviteeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            InviteeStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            OrganizerStackPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        /// <summary>
        /// Displays the combo box containing various reminder times.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReminderCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ReminderComboBox.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        /// <summary>
        /// Hides the combo box containing various reminder times.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReminderCheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            ReminderComboBox.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

    }
}
