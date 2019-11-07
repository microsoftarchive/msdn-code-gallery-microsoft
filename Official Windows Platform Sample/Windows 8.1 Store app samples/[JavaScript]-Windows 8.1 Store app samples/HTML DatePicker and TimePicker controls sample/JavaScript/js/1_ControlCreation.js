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
    var page = WinJS.UI.Pages.define("/html/1_ControlCreation.html", {
        ready: function (element, options) {
            this.divControlCreation = element.querySelector("#divControlCreation");

            element.querySelector("#createImperative")
                .addEventListener("click", this.createDatePickerImperative.bind(this), false);
            element.querySelector("#createDeclarative")
                .addEventListener("click", this.createDatePickerDeclarative.bind(this), false);
        },

        //
        // This function demonstrates creating a date picker control
        // and attaching to to a DOM element through JavaScript
        //
        createDatePickerImperative: function () {
            this.resetOutput();

            // Create a JavaScript date object for date September 1, 1990.
            // Note, JavaScript months are 0 based so September is referenced by 8, not 9
            var initialDate = new Date(1990, 8, 1, 0, 0, 0, 0);

            // Create a new DatePicker control with value of initialDate inside element "myDatePickerDiv"
            var control = new WinJS.UI.DatePicker(this.divControlCreation, { current: initialDate });

            WinJS.log && WinJS.log("Imperative DatePicker with initial date: September, 1, 1990", "sample", "status");
        },

        //
        // This function demonstrates creating a data picker control
        // declaratively through HTML
        //
        createDatePickerDeclarative: function () {
            this.resetOutput();

            // Specify the control type to be created along with an options record to generate instance with given value.
            // Note: internally, date value is passed along to the JavaScript Date object constructor which does the parsing.
            this.divControlCreation.innerHTML = "<div id=\"solstice2011\" data-win-control='WinJS.UI.DatePicker' data-win-options='{current:\"June, 21, 2011\"}'></div>";

            // Activate controls inside the div and process the options records
            WinJS.UI.processAll(this.divControlCreation);

            WinJS.log && WinJS.log("Declarative DatePicker with initial date: June, 21, 2011", "sample", "status");
        },

        resetOutput: function () {
            this.divControlCreation.innerHTML = "";
        }
    });
})();
