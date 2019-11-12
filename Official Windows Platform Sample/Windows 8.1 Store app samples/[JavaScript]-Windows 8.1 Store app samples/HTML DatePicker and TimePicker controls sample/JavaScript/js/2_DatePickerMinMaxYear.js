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
    var page = WinJS.UI.Pages.define("/html/2_DatePickerMinMaxYear.html", {
        ready: function (element, options) {
            this.divMinMax = element.querySelector("#divMinMax");

            WinJS.Utilities.query("#minYear", element)
                .listen("click", this.minYear.bind(this));
            WinJS.Utilities.query("#minMaxYear", element)
                .listen("click", this.minMaxYear.bind(this));
        },

        minYear: function () {
            this.resetOutput();

            // Create a new DatePicker control with minimum selectable year of 2000 inside element "divMinMax"
            var control = new WinJS.UI.DatePicker(this.divMinMax, { minYear: 2000 });

            WinJS.log && WinJS.log("DatePicker control with minYear=2000", "sample", "status");

        },

        minMaxYear: function () {
            this.resetOutput();

            // Specify the control type to be created along with an options record to generate instance with given value.
            // Note: internally, date value is passed along to the JavaScript Date object constructor which does the parsing.
            this.divMinMax.innerHTML = "<div id=\"minMaxYear\" data-win-control='WinJS.UI.DatePicker' data-win-options='{minYear:2000, maxYear:2010}'></div>";

            // Activate controls inside the div and process options records
            WinJS.UI.processAll(this.divMinMax);

            WinJS.log && WinJS.log("DatePicker control with minYear=2000 and maxYear=2010.  Note if present date is outside of a specified min/max year property, the default date (now) will be snap to the specified min/max property value.", "sample", "status");
        },

        resetOutput: function () {
            
            this.divMinMax.innerHTML = "";
        }
    });
})();
