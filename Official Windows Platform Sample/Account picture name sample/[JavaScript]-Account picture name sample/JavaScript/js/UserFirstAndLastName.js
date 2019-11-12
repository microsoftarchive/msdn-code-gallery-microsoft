//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/UserFirstAndLastName.html", {
        ready: function (element, options) {
            document.getElementById("getFirstName").addEventListener("click", getFirstName, false);
            document.getElementById("getLastName").addEventListener("click", getLastName, false);
        }
    });

    function getFirstName() {
        Windows.System.UserProfile.UserInformation.getFirstNameAsync().done(function (result) {
            if (result) {
                WinJS.log && WinJS.log("First Name = " + result, "sample", "status");
            } else {
                WinJS.log && WinJS.log("No First Name was returned.", "sample", "status");
            }
        });
    }

    function getLastName() {
        Windows.System.UserProfile.UserInformation.getLastNameAsync().done(function (result) {
            if (result) {
                WinJS.log && WinJS.log("Last Name = " + result, "sample", "status");
            } else {
                WinJS.log && WinJS.log("No Last Name was returned.", "sample", "status");
            }
        });
    }
})();
