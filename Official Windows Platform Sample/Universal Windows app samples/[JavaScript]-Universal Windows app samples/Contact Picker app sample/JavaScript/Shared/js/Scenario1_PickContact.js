//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // Define page.
    var page = WinJS.UI.Pages.define("/html/Scenario1_PickContact.html", {
        ready: function (element, options) {
            document.getElementById("open").addEventListener("click", pickContact, false);
        }
    });

    // Pick one contact.
    function pickContact() {
        // Clean scenario output
        WinJS.log && WinJS.log("", "sample", "status");

        // Create the picker
        var picker = new Windows.ApplicationModel.Contacts.ContactPicker();
        picker.desiredFieldsWithContactFieldType.append(Windows.ApplicationModel.Contacts.ContactFieldType.email);

        // Open the picker for the user to select a contact
        picker.pickContactAsync().done(function (contact) {
            if (contact !== null) {
                var output = "Selected contact:\n" + contact.displayName;
                WinJS.log && WinJS.log(output, "sample", "status");
            } else {
                // The picker was dismissed without selecting a contact
                WinJS.log && WinJS.log("No contact was selected", "sample", "status");
            }
        });
    }
})();
