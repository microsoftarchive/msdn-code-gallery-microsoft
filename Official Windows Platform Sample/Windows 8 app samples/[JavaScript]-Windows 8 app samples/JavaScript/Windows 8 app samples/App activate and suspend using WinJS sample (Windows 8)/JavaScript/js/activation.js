//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var app = WinJS.Application;

    var page = WinJS.UI.Pages.define("/html/activation.html", {
        processed: function (element, options) {
            // During an initial activation this event is called before activation completes.
            // Do any initialization work that is required for the initial UI to be complete.
            // Populate the text boxes with the previously saved values
            if (app.sessionState.person) {
                document.getElementById("firstName").value = app.sessionState.person.firstName;
                document.getElementById("lastName").value = app.sessionState.person.lastName;
            }
        },

        ready: function (element, options) {
            // During an initial activation this event is called after activation has completed.
            // Do any initialization work that is not related to getting the initial UI set up.
        },

        checkpoint: function() {
            // These will get written out on the next checkpoint event
            var person = {};
            person.firstName = document.getElementById("firstName").value;
            person.lastName = document.getElementById("lastName").value;
            app.sessionState.person = person;
        }
    });
})();
