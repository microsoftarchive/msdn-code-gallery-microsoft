//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // ContactPickerUI to select one or more contacts.
    var contactPickerUI;

    // Scenarios
    var scenarios = [
        { url: "/html/ContactPickerPage.html", title: "Pick contact(s)" },
    ];

    // Define page.
    var page = WinJS.UI.Pages.define("/html/ContactPickerPage.html", {
        ready: function (element, options) {
            contactPickerUI.addEventListener("contactremoved", onContactRemoved, false);
        },

        unload: function () {
            contactPickerUI.removeEventListener("contactremoved", onContactRemoved, false);
        }
    });

    // Activates ContactPickerPage.
    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.contactPicker) {
            SdkSample.sampleContacts.forEach(createContactUI);
            contactPickerUI = eventObject.detail.contactPickerUI;
            WinJS.Utilities.query("#featureLabel")[0].textContent = ContactPicker.sampleTitle;
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                var url = scenarios[0].url;
                return WinJS.Navigation.navigate(url, contactPickerUI);
            }));
        }
    }

    // Call unload method on current scenario, if there is one.
    WinJS.Navigation.addEventListener("navigated", function (eventObject) {
        var url = eventObject.detail.location;
        var host = document.getElementById("contentHost");
        if (host.winControl && host.winControl.unload) {
            host.winControl.unload();
        }
        WinJS.Utilities.empty(host);
        eventObject.detail.setPromise(WinJS.UI.Pages.render(url, host, {
            navigationState: eventObject.detail.state, contactPickerUI: contactPickerUI
        }).then(function () { }));
    });

    // Define ContactPicker namespace.
    WinJS.Namespace.define("ContactPicker", {
        sampleTitle: SdkSample.sampleTitle,
        scenarios: scenarios
    });

    // Define event listener on activated.
    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();

    // Add contact to the basket.
    function addContactToBasket(sampleContact) {
        // Apply the sample data onto the contact object
        var contact = new Windows.ApplicationModel.Contacts.Contact();
        contact.firstName = sampleContact.firstName;
        contact.lastName = sampleContact.lastName;
        contact.id = sampleContact.id;

        // Add a personal and  work email address to the contact emails vector
        if (sampleContact.personalEmail) {
            var personalEmail = new Windows.ApplicationModel.Contacts.ContactEmail();
            personalEmail.address = sampleContact.personalEmail;
            personalEmail.kind = Windows.ApplicationModel.Contacts.ContactEmailKind.personal;
            contact.emails.append(personalEmail);
        }

        if (sampleContact.workEmail) {
            var workEmail = new Windows.ApplicationModel.Contacts.ContactEmail();
            workEmail.address = sampleContact.workEmail;
            workEmail.kind = Windows.ApplicationModel.Contacts.ContactEmailKind.work;
            contact.emails.append(workEmail);
        }

        // Add a home, work, and mobile phone number to the contact phones vector
        if (sampleContact.homePhone) {
            var homePhone = new Windows.ApplicationModel.Contacts.ContactPhone();
            homePhone.number = sampleContact.homePhone;
            homePhone.kind = Windows.ApplicationModel.Contacts.ContactPhoneKind.home;
            contact.phones.append(homePhone);
        }

        if (sampleContact.workPhone) {
            var workPhone = new Windows.ApplicationModel.Contacts.ContactPhone();
            workPhone.number = sampleContact.workPhone;
            workPhone.kind = Windows.ApplicationModel.Contacts.ContactPhoneKind.work;
            contact.phones.append(workPhone);
        }

        if (sampleContact.mobilePhone) {
            var mobilePhone = new Windows.ApplicationModel.Contacts.ContactPhone();
            mobilePhone.number = sampleContact.mobilePhone;
            mobilePhone.kind = Windows.ApplicationModel.Contacts.ContactPhoneKind.mobile;
            contact.phones.append(mobilePhone);
        }

        // Add the contact to the basket
        var statusMessage = document.getElementById("statusMessage");
        switch (contactPickerUI.addContact(contact)) {
            case Windows.ApplicationModel.Contacts.Provider.AddContactResult.added:
                // Notify user that the contact was added to the basket
                statusMessage.innerText = contact.displayName + " was added to the basket";
                break;
            case Windows.ApplicationModel.Contacts.Provider.AddContactResult.alreadyAdded:
                // Notify the user that the contact is already in the basket
                statusMessage.innerText = contact.displayName + " is already in the basket";
                break;
            case Windows.ApplicationModel.Contacts.Provider.AddContactResult.unavailable:
            default:
                // Notify the user that the basket is not currently available
                statusMessage.innerText = contact.displayName + " could not be added to the basket";
                break;
        }
    }

    // Remove contact from the basket
    function removeContactFromBasket(sampleContact) {
        if (contactPickerUI.containsContact(sampleContact.id)) {
            contactPickerUI.removeContact(sampleContact.id);
            document.getElementById("statusMessage").innerText = sampleContact.displayName + " was removed from the basket";
        }
    }

    // Display message when contact is removed from the basket.
    function onContactRemoved(e) {
        var contactElement = document.getElementById(e.id);
        var sampleContact = SdkSample.sampleContacts[contactElement.value];
        contactElement.checked = false;
        document.getElementById("statusMessage").innerText += "\n" + sampleContact.displayName + " was removed from the basket";
    }

    // Add a UI checkbox for a contact so that it can added or removed from the basket.
    function createContactUI(sampleContact, index) {        
        var element = document.createElement("div");
        document.getElementById("contactList").appendChild(element);
        element.innerHTML = "<div class='contact'><label>" +
                            "<input id='" + sampleContact.id + "' value='" + index + "' type='checkbox' />" +
                            sampleContact.displayName + "</label></div>";

        element.firstElementChild.addEventListener("change", function (ev) {
            if (ev.target.checked) {
                addContactToBasket(sampleContact);
            } else {
                removeContactFromBasket(sampleContact);
            }
        }, false);
    }
})();
