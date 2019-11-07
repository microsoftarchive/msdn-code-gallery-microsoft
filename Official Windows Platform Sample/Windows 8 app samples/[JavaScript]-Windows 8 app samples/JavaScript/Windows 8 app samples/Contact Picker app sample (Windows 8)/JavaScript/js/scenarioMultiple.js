//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenarioMultiple.html", {
        ready: function (element, options) {
            document.getElementById("open").addEventListener("click", pickContacts, false);
        }
    });

    function pickContacts() {
        // Clean scenario output
        WinJS.log && WinJS.log("", "sample", "status");

        // Verify that we are unsnapped or can unsnap to open the picker
        var viewState = Windows.UI.ViewManagement.ApplicationView.value;
        if (viewState === Windows.UI.ViewManagement.ApplicationViewState.snapped &&
            !Windows.UI.ViewManagement.ApplicationView.tryUnsnap()) {
            // Fail silently if we can't unsnap
            return;
        };

        // Create the picker
        var picker = new Windows.ApplicationModel.Contacts.ContactPicker();
        picker.commitButtonText = "Select";

        // Open the picker for the user to select contacts
        picker.pickMultipleContactsAsync().done(function (contacts) {
            if (contacts.length > 0) {
                // Display the selected contact names
                var output = "Selected contacts:\n";
                contacts.forEach(function (contact) {
                    output += contact.name + "\n";
                });
                WinJS.log && WinJS.log(output, "sample", "status");
            } else {
                // The picker was dismissed without selecting any contacts
                WinJS.log && WinJS.log("No contacts were selected", "sample", "status");
            }
        });
    }
})();
