//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/ReplaceAppointment.html", {
        ready: function (element, options) {
            document.getElementById("replaceButton").addEventListener("click", replaceAppointment, false);
        }
    });

    function replaceAppointment(e) {
        // The appointment id argument for ReplaceAppointmentAsync is typically retrieved from AddAppointmentAsync and stored in app data.
        var appointmentIdOfAppointmentToReplace = document.querySelector('#appointmentIdInput').value;

        if (!appointmentIdOfAppointmentToReplace) {
            document.querySelector('#result').innerText = "The appointment id cannot be empty";
        } else {
            // The Appointment argument for ReplaceAppointmentAsync should contain all of the Appointment's properties including those that may have changed.
            var appointment = new Windows.ApplicationModel.Appointments.Appointment();

            // Get the selection rect of the button pressed to replace this appointment
            var boundingRect = e.srcElement.getBoundingClientRect();
            var selectionRect = { x: boundingRect.left, y: boundingRect.top, width: boundingRect.width, height: boundingRect.height };

            // ReplaceAppointmentAsync returns an updated appointment id when the appointment was successfully replaced.
            // The updated id may or may not be the same as the original one retrieved from AddAppointmentAsync.
            // An optional instance start time can be provided to indicate that a specific instance on that date should be replaced
            // in the case of a recurring appointment.
            // If the appointment id returned is an empty string, that indicates that the appointment was not replaced.
            if (document.querySelector('#instanceStartDateCheckBox').checked) {
                // Replace a specific instance starting on the date provided.
                var instanceStartDate = document.querySelector('#startTimeDatePicker').winControl.current;

                Windows.ApplicationModel.Appointments.AppointmentManager.showReplaceAppointmentAsync(appointmentIdOfAppointmentToReplace, appointment, selectionRect, Windows.UI.Popups.Placement.default, instanceStartDate)
                    .done(function (updatedAppointmentId) {
                        if (updatedAppointmentId) {
                            document.querySelector('#result').innerText = "Updated Appointment Id: " + updatedAppointmentId;
                        } else {
                            document.querySelector('#result').innerText = "Appointment not replaced";
                        }
                    });
            } else {
                // Replace an appointment that occurs only once or in the case of a recurring appointment, replace the entire series.
                Windows.ApplicationModel.Appointments.AppointmentManager.showReplaceAppointmentAsync(appointmentIdOfAppointmentToReplace, appointment, selectionRect, Windows.UI.Popups.Placement.default)
                    .done(function (updatedAppointmentId) {
                        if (updatedAppointmentId) {
                            document.querySelector('#result').innerText = "Updated Appointment Id: " + updatedAppointmentId;
                        } else {
                            document.querySelector('#result').innerText = "Appointment not replaced";
                        }
                    });
            }
        }
    }
})();
