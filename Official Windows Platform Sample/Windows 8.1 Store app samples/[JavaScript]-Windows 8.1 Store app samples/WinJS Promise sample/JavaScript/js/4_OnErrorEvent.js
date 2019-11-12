//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/4_OnErrorEvent.html", {
        ready: function (element, options) {
            this.outputDiv = element.querySelector(".onErrorOutput");
            this.outputDiv.innerText = "";

            WinJS.Utilities.query(".onErrorErrorButton", element)
                .listen("click", this.fireAnError.bind(this));
            WinJS.Utilities.query(".onErrorExceptionButton", element)
                .listen("click", this.fireAnException.bind(this));
        },

        fireAnError: function () {
            // register the error handler
            WinJS.Promise.onerror = this.errorHandler.bind(this);

            // Generate a promise in an error state
            WinJS.Promise.wrapError("Error inside a promise")
                .then(null,
                // We still need to handle the error, or the app will get killed
                // due to the unhandled exception
                function () { });
        },

        fireAnException: function () {
            // register the error handler
            WinJS.Promise.onerror = this.errorHandler.bind(this);

            var p = WinJS.Promise.as()
                .then(function () {
                    // throw an exception from this Promise
                    throw "Exception inside a promise";
                }).then(null,
                    // We still need to handle the error, or the app will get killed
                    // due to the unhandled exception
                    function () { });
        },

        errorHandler: function (evt) {
            // we'll examine the detail property of the event object to determine which
            // errors come from a Promise object.
            var detail = evt.detail;

            this.clearOutput();
            this.output(" Properties of the argument of the error handler");

            // sort through the detail contents to identify which one's are from promises
            for (var i in detail) {
                if (i !== "promise") {
                    this.output("\r\n\r\n Property '" + i + "' : " + detail[i]);
                } else {
                    this.output("\r\n\r\n Property '" + i + "' : promise object");
                }
            }

            // remove the onerror event handler
            WinJS.Promise.onerror = null;
        },
        clearOutput: function () {
            this.outputDiv.innerText = "";
        },

        output: function (s) {
            this.outputDiv.innerText += s;
        }
    });
})();
