//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // Define page.
    var page = WinJS.UI.Pages.define("/html/Scenario2_PickContacts.html", {
        ready: function (element, options) {
            document.getElementById("open").addEventListener("click", pickContacts, false);
        }
    });

    // Pick one or more contacts.
    function pickContacts() {
        // Clean scenario output
        WinJS.log && WinJS.log("", "sample", "status");

        // Create the picker
        var picker = new Windows.ApplicationModel.Contacts.ContactPicker();
        picker.desiredFieldsWithContactFieldType.append(Windows.ApplicationModel.Contacts.ContactFieldType.email);

        // Open the picker for the user to select contacts
        picker.pickContactsAsync().done(function (contacts) {
            if (contacts.length > 0) {
                // Display the selected contact names
                var output = "Selected contacts:\n";
                contacts.forEach(function (contact) {
                    output += contact.displayName + "\n";
                });
                WinJS.log && WinJS.log(output, "sample", "status");
            } else {
                // The picker was dismissed without selecting any contacts
                WinJS.log && WinJS.log("No contacts were selected", "sample", "status");
            }
        });
    }
})();
