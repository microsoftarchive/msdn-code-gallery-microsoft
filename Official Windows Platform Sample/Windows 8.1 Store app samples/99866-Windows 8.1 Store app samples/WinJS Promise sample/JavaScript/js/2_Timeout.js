//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/2_Timeout.html", {
        ready: function (element, options) {
            this.outputElement = element.querySelector(".timeoutOutput");
            this.outputElement.innerText = "";

            WinJS.Utilities.query(".timeoutStartButton", element).listen("click",
                this.runTimeout.bind(this));
        },

        runTimeout: function () {
            var that = this;
            this.output(" Creating a timeout for 5 seconds \r\n");

            // create a promise that will be completed after 5000ms
            WinJS.Promise.timeout(5000).
            then(function () {
                that.output("\r\n\r\n\r\n The promise is fullfilled after 5 seconds \n");
            });
        },

        output: function (s) {
            this.outputElement.innerText += s;
        }
    });
})();
