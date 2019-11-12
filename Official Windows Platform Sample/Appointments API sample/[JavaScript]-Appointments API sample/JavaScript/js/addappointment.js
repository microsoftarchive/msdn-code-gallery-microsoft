//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/AddAppointment.html", {
        ready: function (element, options) {
            document.getElementById("addButton").addEventListener("click", addAppointment, false);
        }
    });

    function addAppointment(e) {
        // Create an Appointment that should be added the user's appointments provider app.
        var appointment = new Windows.ApplicationModel.Appointments.Appointment();

        // Get the selection rect of the button pressed to add this appointment
        var boundingRect = e.srcElement.getBoundingClientRect();
        var selectionRect = { x: boundingRect.left, y: boundingRect.top, width: boundingRect.width, height: boundingRect.height };

        // ShowAddAppointmentAsync returns an appointment id if the appointment given was added to the user's calendar.
        // This value should be stored in app data and roamed so that the appointment can be replaced or removed in the future.
        // An empty string return value indicates that the user canceled the operation before the appointment was added.
        Windows.ApplicationModel.Appointments.AppointmentManager.showAddAppointmentAsync(appointment, selectionRect, Windows.UI.Popups.Placement.default)
            .done(function (appointmentId) {
                if (appointmentId) {
                    document.querySelector('#result').innerText = "Appointment Id: " + appointmentId;
                } else {
                    document.querySelector('#result').innerText = "Appointment not added";
                }
            });
    }
})();
