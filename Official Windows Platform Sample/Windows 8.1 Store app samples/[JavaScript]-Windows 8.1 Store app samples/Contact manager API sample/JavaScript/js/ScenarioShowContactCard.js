//// Copyright (c) Microsoft Corporation. All rights reserved
(function () {
    "use strict";
    var ContactsNS = Windows.ApplicationModel.Contacts;

    var page = WinJS.UI.Pages.define("/html/ScenarioShowContactCard.html", {
        ready: function (element, options) {
            document.getElementById("buttonShowContactCard").addEventListener("click", ShowContactCard, false);
        }
    });

    // Length limits allowed by the API
    var MAX_EMAIL_ADDRESS_LENGTH = 321;
    var MAX_PHONE_NUMBER_LENGTH = 50;

    function ShowContactCard(evt) {
        var emailAddress = document.getElementById("inputEmailAddress").value;
        var phoneNumber = document.getElementById("inputPhoneNumber").value;

        if ((emailAddress.length === 0) && (phoneNumber.length === 0)) {
            WinJS.log && WinJS.log("You must enter an email address and/or phone number of the contact for the system to search and show the contact card.", "sample", "error");
        }
        else {
            // Create input contact object for calling ContactManager.showContactCard().
            var contact = new ContactsNS.Contact();
            if (emailAddress.length > 0) {
                if (emailAddress.length <= MAX_EMAIL_ADDRESS_LENGTH) {
                    var email = new ContactsNS.ContactEmail();
                    email.address = emailAddress;
                    contact.emails.append(email);
                }
                else {
                    WinJS.log && WinJS.log("The email address you entered is too long.", "sample", "error");
                    return;
                }
            }

            if (phoneNumber.length > 0) {
                if (phoneNumber.length <= MAX_PHONE_NUMBER_LENGTH) {
                    var phone = new ContactsNS.ContactPhone();
                    phone.number = phoneNumber;
                    contact.phones.append(phone);
                }
                else {
                    WinJS.log && WinJS.log("The phone number you entered is too long.", "sample", "error");
                    return;
                }
            }

            // Get the selection rect of the button pressed to show contact card.
            var boundingRect = evt.srcElement.getBoundingClientRect();
            var selectionRect = { x: boundingRect.left, y: boundingRect.top, width: boundingRect.width, height: boundingRect.height };

            ContactsNS.ContactManager.showContactCard(contact, selectionRect, Windows.UI.Popups.Placement.default);
            WinJS.log && WinJS.log("ContactManager.showContactCard() was called.", "sample", "status");
        }
    }
})();
