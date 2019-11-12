//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/ShowTimeFrame.html", {
        ready: function (element, options) {
            document.getElementById("showTimeFrameButton").addEventListener("click", showTimeFrame, false);
        }
    });

    // Show the appointment provider app at the current date and time with a 1 hour duration
    function showTimeFrame() {
        var dateToShow = new Date();
        Windows.ApplicationModel.Appointments.AppointmentManager.showTimeFrameAsync(dateToShow, (60 * 60 * 1000))
            .done(function () {
                document.querySelector('#result').innerText = "The default appointments provider should have appeared on screen.";
            });
    }
})();
