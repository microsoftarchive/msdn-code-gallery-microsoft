//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/AppointmentProperties.html", {
        ready: function (element, options) {
            document.getElementById("createButton").addEventListener("click", createAppointment, false);
            document.getElementById("organizerRadioButton").addEventListener("click", organizerRadioButtonSelected, false);
            document.getElementById("inviteeRadioButton").addEventListener("click", inviteeRadioButtonSelected, false);
        }
    });

    function createAppointment() {
        var isAppointmentValid = true;
        var appointment = new Windows.ApplicationModel.Appointments.Appointment();

        // StartTime
        var startTime = document.querySelector('#startDatePicker').winControl.current;
        var time = document.querySelector('#startTimePicker').winControl.current;
        startTime.setMinutes(time.getMinutes());
        startTime.setHours(time.getHours());
        appointment.startTime = startTime;

        // Subject
        appointment.subject = document.querySelector('#subjectInput').value;

        if (appointment.subject.length > 255) {
            isAppointmentValid = false;
            document.querySelector('#result').innerText = "The subject cannot be greater than 255 characters.";
        }

        // Location
        appointment.location = document.querySelector('#locationInput').value;;

        if (appointment.location.length > 32768) {
            isAppointmentValid = false;
            document.querySelector('#result').innerText = "The location cannot be greater than 32,768 characters.";
        }

        // Details
        appointment.details = document.querySelector('#detailsInput').value;

        if (appointment.details.length > 1073741823) {
            isAppointmentValid = false;
            document.querySelector('#result').innerText = "The details cannot be greater than 1,073,741,823 characters.";
        }

        // Duration
        if (document.querySelector('#durationSelect').selectedIndex === 0) {
            // 30 minute duration is selected
            appointment.duration = (30 * 60 * 1000);
        } else {
            // 1 hour duration is selected
            appointment.duration = (60 * 60 * 1000);
        }

        // All Day
        appointment.allDay = (document.querySelector('#allDayCheckBox').checked);

        // Reminder
        if (document.querySelector('#reminderCheckBox').checked) {
            switch (document.querySelector('#reminderSelect').selectedIndex) {
                case 0:
                    appointment.reminder = (15 * 60 * 1000);
                    break;
                case 1:
                    appointment.reminder = (60 * 60 * 1000);
                    break;
                case 2:
                    appointment.reminder = (24 * 60 * 60 * 1000);
                    break;
            }
        }

        //Busy Status
        switch (document.querySelector('#busyStatusSelect').selectedIndex) {
            case 0:
                appointment.busyStatus = Windows.ApplicationModel.Appointments.AppointmentBusyStatus.busy;
                break;
            case 1:
                appointment.busyStatus = Windows.ApplicationModel.Appointments.AppointmentBusyStatus.tentative;
                break;
            case 2:
                appointment.busyStatus = Windows.ApplicationModel.Appointments.AppointmentBusyStatus.free;
                break;
            case 3:
                appointment.busyStatus = Windows.ApplicationModel.Appointments.AppointmentBusyStatus.outOfOffice;
                break;
            case 4:
                appointment.busyStatus = Windows.ApplicationModel.Appointments.AppointmentBusyStatus.workingElsewhere;
                break;
        }

        // Sensitivity
        switch (document.querySelector('#sensitivitySelect').selectedIndex) {
            case 0:
                appointment.sensitivity = Windows.ApplicationModel.Appointments.AppointmentSensitivity.public;
                break;
            case 1:
                appointment.sensitivity = Windows.ApplicationModel.Appointments.AppointmentSensitivity.private;
                break;
        }

        // Uri
        var uriValue = document.querySelector('#uriInput').value;
        if (uriValue.length > 0) {
            try {
                appointment.uri = new Windows.Foundation.Uri(uriValue);
            }
            catch (e) {
                isAppointmentValid = false;
                document.querySelector('#result').innerText = "The Uri provided is invalid.";
            }
        }

        // Organizer
        // Note: Organizer can only be set if there are no invitees added to this appointment.
        if (document.querySelector('#organizerRadioButton').checked) {
            var organizer = new Windows.ApplicationModel.Appointments.AppointmentOrganizer();

            // Organizer Display Name
            organizer.displayName = document.querySelector('#organizerDisplayNameInput').value;

            if (organizer.displayName.length > 256) {
                isAppointmentValid = false;
                document.querySelector('#result').innerText = "The organizer display name cannot be greater than 256 characters.";
            } else {
                // Organizer Address (e.g. Email Address)
                organizer.address = document.querySelector('#organizerAddressInput').value;

                if (organizer.address.length > 321) {
                    isAppointmentValid = false;
                    document.querySelector('#result').innerText = "The organizer address cannot be greater than 321 characters.";
                } else if (organizer.address.length === 0) {
                    isAppointmentValid = false;
                    document.querySelector('#result').innerText = "The organizer address must be greater than 0 characters.";
                } else {
                    appointment.organizer = organizer;
                }
            }
        }

        // Invitees
        // Note: If the size of the Invitees list is not zero, then an Organizer cannot be set.
        if (document.querySelector('#inviteeRadioButton').checked) {
            var invitee = new Windows.ApplicationModel.Appointments.AppointmentInvitee();

            // Invitee Display Name
            invitee.displayName = document.querySelector('#inviteeDisplayNameInput').value;

            if (invitee.displayName.length > 256) {
                isAppointmentValid = false;
                document.querySelector('#result').innerText = "The invitee display name cannot be greater than 256 characters.";
            } else {
                // Invitee Address (e.g. Email Address)
                invitee.address = document.querySelector('#inviteAddressInput').value;

                if (invitee.address.length > 321) {
                    isAppointmentValid = false;
                    document.querySelector('#result').innerText = "The invitee address cannot be greater than 321 characters.";
                } else if (invitee.address.length === 0) {
                    isAppointmentValid = false;
                    document.querySelector('#result').innerText = "The invitee address must be greater than 0 characters.";
                } else {
                    // Invitee Role
                    switch (document.querySelector('#inviteeRoleSelect').selectedIndex) {
                        case 0:
                            invitee.role = Windows.ApplicationModel.Appointments.AppointmentParticipantRole.requiredAttendee;
                            break;
                        case 1:
                            invitee.role = Windows.ApplicationModel.Appointments.AppointmentParticipantRole.optionalAttendee;
                            break;
                        case 2:
                            invitee.role = Windows.ApplicationModel.Appointments.AppointmentParticipantRole.resource;
                            break;
                    }

                    // Invitee Response
                    switch (document.querySelector('#inviteeResponseSelect').selectedIndex) {
                        case 0:
                            invitee.response = Windows.ApplicationModel.Appointments.AppointmentParticipantResponse.none;
                            break;
                        case 1:
                            invitee.response = Windows.ApplicationModel.Appointments.AppointmentParticipantResponse.tentative;
                            break;
                        case 2:
                            invitee.response = Windows.ApplicationModel.Appointments.AppointmentParticipantResponse.accepted;
                            break;
                        case 3:
                            invitee.response = Windows.ApplicationModel.Appointments.AppointmentParticipantResponse.declined;
                            break;
                        case 4:
                            invitee.response = Windows.ApplicationModel.Appointments.AppointmentParticipantResponse.unknown;
                            break;
                    }

                    appointment.invitees.append(invitee);
                }
            }
        }

        if (isAppointmentValid) {
            document.querySelector('#result').innerText = "The appointment was created successfully and is valid.";
        }
    }

    function organizerRadioButtonSelected() {
        document.querySelector('#inviteeDiv').style.display = "none";
        document.querySelector('#organizerDiv').style.display = "";
    }

    function inviteeRadioButtonSelected() {
        document.querySelector('#organizerDiv').style.display = "none";
        document.querySelector('#inviteeDiv').style.display = "";
    }
})();
