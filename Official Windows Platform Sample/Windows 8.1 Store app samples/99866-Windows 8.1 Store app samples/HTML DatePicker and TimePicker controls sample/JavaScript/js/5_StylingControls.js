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
    var page = WinJS.UI.Pages.define("/html/5_StylingControls.html", {
        ready: function (element, options) {
            this.divDatePicker = element.querySelector("#divStylingControls_DatePicker");
            this.divTimePicker = element.querySelector("#divStylingControls_TimePicker");

            WinJS.Utilities.query("#createStyledControls", element)
                .listen("click", this.createStyledControls.bind(this));
        },

        createStyledControls: function () {
            this.resetOutput();

            // Create controls that will be styled according to style sheets in StylingControls.html
            //
            //<style>
            //    /* Display datepicker vertically instead of the default horizontal layout by
            //       looking for descendents with a class attribute starting with win-datepicker */
            //    #divStylingControls_DatePicker *[class^="win-datepicker"] { display:block; float:none; background:lightgreen;}

            //    /* Set timepicker classes based on a media query based on width, in this example we want to
            //       conditionally layout the timepicker vertically when the display width is < 320px */
            //    @media all and (max-width:320px) {
            //        #divStylingControls_TimePicker *[class^="win-timepicker"] { display: block; float:none; background:lightgreen; }
            //    }
            //</style>

            var datepickerControl = new WinJS.UI.DatePicker(this.divDatePicker);
            var timepickerControl = new WinJS.UI.TimePicker(this.divTimePicker);

            WinJS.log && WinJS.log("Notice the datepicker control is now displayed vertically.  Once the screen width becomes < 320px, the timepicker will be displayed vertically as well.",
                "sample", "status");

        },
        resetOutput: function () {
            this.divDatePicker.innerHTML = "";
            this.divTimePicker.innerHTML = "";
        }
    });

    function doSomething() {
        WinJS.log && WinJS.log("Error message here", "sample", "error");
    }
})();
