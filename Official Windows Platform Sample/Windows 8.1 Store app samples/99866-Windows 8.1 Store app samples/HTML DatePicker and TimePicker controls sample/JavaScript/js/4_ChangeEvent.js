//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />
/// <reference path="//Microsoft.WinJS.2.0/js/ui.js" />

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/4_ChangeEvent.html", {
        ready: function (element, options) {
            this.divDatePicker = element.querySelector("#divDatePicker");
            this.divTimePicker = element.querySelector("#divTimePicker");

            WinJS.Utilities.query("#createDateTimePickers", element)
                .listen("click", this.createDateTimePickers.bind(this));
        },

        createDateTimePickers: function () {
            this.resetOutput();

            // Create controls
            var datepickerControl = new WinJS.UI.DatePicker(this.divDatePicker);
            var timepickerControl = new WinJS.UI.TimePicker(this.divTimePicker);

            // Connect event listeners
            datepickerControl.element.addEventListener("change", function () {
                WinJS.log && WinJS.log("datepicker changed, new value is:\n" +
                    datepickerControl.current.toLocaleDateString(),
                    "sample", "status");
            });

            timepickerControl.element.addEventListener("change", function () {
                WinJS.log && WinJS.log("timepicker changed, new value is:\n" +
                timepickerControl.current.toLocaleTimeString(),
                "sample", "status");
            });

            WinJS.log && WinJS.log("Change the dates and times using the control and see that an alert fires in response to changes invoked through the UI.",
                "sample", "status");

        },
        resetOutput: function () {
            this.divDatePicker.innerHTML = "";
            this.divTimePicker.innerHTML = "";
        }
    });
})();
