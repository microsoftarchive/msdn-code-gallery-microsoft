//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/6_Cancellation.html", {
        ready: function (element, options) {
            this.outputDiv = element.querySelector(".cancellationOutput");
            this.clearOutput();

            WinJS.Utilities.query(".cancelButton", element)
                .listen("click", this.cancelPromise.bind(this));
        },

        cancelPromise: function () {
            var that = this;

            this.clearOutput();
            this.output(" Creating a promise that will take 5 seconds to complete");

            var p = WinJS.Promise.timeout(5000).
                    then(function () {
                            // this will never be called because promise is explictly cancelled by calling
                            // p.cancel(); below before the promise can complete (before the 5000ms timeout)
                            that.output("\r\n\r\n The promise will never be completed. This message will never get printed");
                        },
                        function (e) {
                            that.output("\r\n\r\n Error handler is called with parameter desciption = '" + e.description + "' error");
                        }
                    );

            that.output("\r\n\r\n Cancelling the promise before it completes.");

            // this statement cancels the promise before it can complete, before the 5000ms timeout
            p.cancel();
        },

        clearOutput: function () {
            this.outputDiv.innerText = "";
        },

        output: function (s) {
            this.outputDiv.innerText += s;
        }
    });
})();
