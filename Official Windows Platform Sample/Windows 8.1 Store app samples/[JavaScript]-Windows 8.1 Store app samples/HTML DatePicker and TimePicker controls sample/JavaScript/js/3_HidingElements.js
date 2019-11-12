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
    var page = WinJS.UI.Pages.define("/html/3_HidingElements.html", {
        ready: function (element, options) {
            this.divHidingElements = element.querySelector("#divHidingElements");

            WinJS.Utilities.query("#monthOnly", element)
                .listen("click", this.monthOnly.bind(this));
            WinJS.Utilities.query("#hideAMPM", element)
                .listen("click", this.hideAMPM.bind(this));
        },

        monthOnly: function () {
            this.resetOutput();

            // Create a new DatePicker control that only displays the month element
            var control = new WinJS.UI.DatePicker(this.divHidingElements);

            // Hide the date, year elements, leaving only the month element visible
            control.element.querySelector(".win-datepicker-year").style.display = "none";
            control.element.querySelector(".win-datepicker-date").style.display = "none";

            WinJS.log && WinJS.log("DatePicker control only showing the month element", "sample", "status");
        },

        hideAMPM: function () {
            this.resetOutput();

            // Note: hiding the AMPM element declaratively is done through a style sheet on the HTML page:
            //    <style>
            //        #hideAMPM *.win-timepicker-ampm { display:none; }
            //    </style>

            this.divHidingElements.innerHTML = "<div id='hideAMPM' data-win-control='WinJS.UI.TimePicker'></div>";

            // Activate controls inside the div and process options records
            WinJS.UI.processAll(this.divHidingElements);

            WinJS.log && WinJS.log("TimePicker control without the ampm field", "sample", "status");
        },

        resetOutput: function () {
            this.divHidingElements.innerHTML = "";
        }
    });
})();
