//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var myfile = "myfile.text";
    var app = WinJS.Application;

    var page = WinJS.UI.Pages.define("/html/lifetime.html", {
        processed: function (element, eventObject) {
            // During an initial activation this event is called before activation completes.
            // Do any initialization work that is required for the initial UI to be complete.
            if (eventObject) {
                var activatedReason = eventObject.previousExecutionState;
                if (activatedReason && activatedReason === Windows.ApplicationModel.Activation.ApplicationExecutionState.terminated) {
                    // Populate the text box with the previously saved value
                    return app.local.readText(myfile, "default").then(function (str) {
                        document.getElementById("userText").value = str;
                    }, function () {
                        document.getElementById("userText").value = 'undefined';
                    });
                }
            }

            return WinJS.Promise.wrap(null);
        },

        ready: function (element, state) {
            // During an initial activation this event is called after activation has completed.
            // Do any initialization work that is not related to getting the initial UI set up.
        },

        suspending: function (eventObject) {
            // If there is going to be some asynchronous operation done during suspension then
            // the app can signal the need to handle suspension of the application asynchronously.
            // To do so the app can use the getDeferral method.
            var suspendingDeferral = eventObject.suspendingOperation.getDeferral();

            // This is only for advanced scenarios when using a file is necessary to persist data.
            app.local.writeText(myfile, document.getElementById("userText").value).done(function () {
                // After the asynchronous operation is done the app must call complete on the deferral object
                // as follows else the app would get terminated.
                suspendingDeferral.complete();
            });
        }
    });
})();
