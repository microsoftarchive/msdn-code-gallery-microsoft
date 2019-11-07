//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/RemoveAppointment.html", {
        ready: function (element, options) {
            document.getElementById("removeButton").addEventListener("click", removeAppointment, false);
        }
    });

    function removeAppointment(e) {
        // The appointment id argument for ShowRemoveAppointmentAsync is typically retrieved from AddAppointmentAsync and stored in app data.
        var appointmentId = document.querySelector('#appointmentIdInput').value;

        // The appointment id cannot be empty.
        if (!appointmentId) {
            document.querySelector('#result').innerText = "The appointment id cannot be empty";
        } else {
            // Get the selection rect of the button pressed to remove this appointment
            var boundingRect = e.srcElement.getBoundingClientRect();
            var selectionRect = { x: boundingRect.left, y: boundingRect.top, width: boundingRect.width, height: boundingRect.height };

            // ShowRemoveAppointmentAsync returns a boolean indicating whether or not the appointment related to the appointment id given was removed.
            // An optional instance start time can be provided to indicate that a specific instance on that date should be removed
            // in the case of a recurring appointment.
            if (document.querySelector('#instanceStartDateCheckBox').checked) {
                // Remove a specific instance starting on the date provided.
                var instanceStartDate = document.querySelector('#startTimeDatePicker').winControl.current;

                Windows.ApplicationModel.Appointments.AppointmentManager.showRemoveAppointmentAsync(appointmentId, selectionRect, Windows.UI.Popups.Placement.default, instanceStartDate)
                    .done(function (removed) {
                        if (removed) {
                            document.querySelector('#result').innerText = "Appointment removed";
                        } else {
                            document.querySelector('#result').innerText = "Appointment not removed";
                        }
                    });
            } else {
                // Remove an appointment that occurs only once or in the case of a recurring appointment, remove the entire series.
                Windows.ApplicationModel.Appointments.AppointmentManager.showRemoveAppointmentAsync(appointmentId, selectionRect, Windows.UI.Popups.Placement.default)
                .done(function (removed) {
                    if (removed) {
                        document.querySelector('#result').innerText = "Appointment removed";
                    } else {
                        document.querySelector('#result').innerText = "Appointment not removed";
                    }
                });
            }
        }
    }
})();
