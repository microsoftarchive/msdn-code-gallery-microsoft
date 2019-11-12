//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenarioSingle.html", {
        ready: function (element, options) {
            document.getElementById("open").addEventListener("click", pickContact, false);
        }
    });

    function pickContact() {
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

        // Open the picker for the user to select a contact
        picker.pickSingleContactAsync().done(function (contact) {
            if (contact !== null) {
                // Create UI to display the contact information for the selected contact
                var contactElement = document.createElement("div");
                contactElement.className = "contact";

                // Display the name
                contactElement.appendChild(createTextElement("h3", contact.name));

                // Add the different types of contact data
                appendFields("Emails:", contact.emails, contactElement);
                appendFields("Phone Numbers:", contact.phoneNumbers, contactElement);
                appendFields("Addresses:", contact.locations, contactElement);

                // Add the contact element to the page
                document.getElementById("output").appendChild(contactElement);
            } else {
                // The picker was dismissed without selecting a contact
                WinJS.log && WinJS.log("No contact was selected", "sample", "status");
            }
        });
    }

    function appendFields(title, fields, container) {
        // Creates UI for a list of contact fields of the same type, e.g. emails or phones
        fields.forEach(function (field) {
            if (field.value) {
                // Append the title once we have a non-empty contact field
                if (title) {
                    container.appendChild(createTextElement("h4", title));
                    title = "";
                }

                // Display the category next to the field value
                switch (field.category) {
                    case Windows.ApplicationModel.Contacts.ContactFieldCategory.home:
                        container.appendChild(createTextElement("div", field.value + " (home)"));
                        break;
                    case Windows.ApplicationModel.Contacts.ContactFieldCategory.work:
                        container.appendChild(createTextElement("div", field.value + " (work)"));
                        break;
                    case Windows.ApplicationModel.Contacts.ContactFieldCategory.mobile:
                        container.appendChild(createTextElement("div", field.value + " (mobile)"));
                        break;
                    case Windows.ApplicationModel.Contacts.ContactFieldCategory.other:
                        container.appendChild(createTextElement("div", field.value + " (other)"));
                        break;
                    case Windows.ApplicationModel.Contacts.ContactFieldCategory.none:
                    default:
                        container.appendChild(createTextElement("div", field.value));
                        break;
                }
            }
        });
    }

    function createTextElement(tag, text) {
        var element = document.createElement(tag);
        element.className = "singleLineText";
        element.innerText = text;
        return element;
    }

})();
