//// Copyright (c) Microsoft Corporation. All rights reserved
(function () {
    "use strict";
    var ContactsNS = Windows.ApplicationModel.Contacts;

    var page = WinJS.UI.Pages.define("/html/ScenarioShowContactCardDelayLoad.html", {
        ready: function (element, options) {
            document.getElementById("buttonShowContactCardDelayLoad").addEventListener("click", ShowContactCardDelayLoad, false);
        }
    });

    function downLoadContactDataAsync(contact) {
        return new WinJS.Promise(function (comp) {
            // Simulate the download latency by delaying the execution by 2 seconds.
            setTimeout(function () {
                // Add more data to the contact object.
                var workEmail = new ContactsNS.ContactEmail();
                workEmail.address = "kim@adatum.com";
                workEmail.kind = ContactsNS.ContactEmailKind.work;
                contact.emails.append(workEmail);                   

                var homePhone = new ContactsNS.ContactPhone();
                homePhone.number = "(444) 555-0001";
                homePhone.kind = ContactsNS.ContactPhoneKind.home;
                contact.phones.append(homePhone);

                var workPhone = new ContactsNS.ContactPhone();
                workPhone.number = "(245) 555-0123";
                workPhone.kind = ContactsNS.ContactPhoneKind.work;
                contact.phones.append(workPhone);

                var mobilePhone = new ContactsNS.ContactPhone();
                mobilePhone.number = "(921) 555-0187";
                mobilePhone.kind = ContactsNS.ContactPhoneKind.mobile;
                contact.phones.append(mobilePhone);

                var address = new ContactsNS.ContactAddress();
                address.streetAddress = "123 Main St";
                address.locality = "Redmond";
                address.region = "WA";
                address.country = "USA";
                address.postalCode = "23456";
                address.kind = ContactsNS.ContactAddressKind.home;
                contact.addresses.append(address);

                comp({ fullContact: contact, hasMoreData: true });
            },
            2000);
        });
    }

    function ShowContactCardDelayLoad(evt) {
        // Create contact object with small set of initial data to display.
        var contact = new ContactsNS.Contact();
        contact.firstName = "Kim";
        contact.lastName = "Abercrombie";

        var email = new ContactsNS.ContactEmail();
        email.address = "kim@contoso.com";
        contact.emails.append(email);

        // Get the selection rect of the button pressed to show contact card.
        var boundingRect = evt.srcElement.getBoundingClientRect();
        var selectionRect = { x: boundingRect.left, y: boundingRect.top, width: boundingRect.width, height: boundingRect.height };

        var delayedDataLoader = ContactsNS.ContactManager.showDelayLoadedContactCard(
            contact,
            selectionRect,
            Windows.UI.Popups.Placement.below // The contact card placement can change when it is updated with more data. For improved user experience, specify placement 
                                              // of the card so that it has space to grow and will not need to be repositioned. In this case, default placement first places 
                                              // the card above the button because the card is small, but after the card is updated with more data, the operating system moves 
                                              // the card below the button to fit the card's expanded size. Specifying that the contact card is placed below at the beginning 
                                              // avoids this repositioning.
            );
        var message = "ContactManager.showDelayLoadedContactCard() was called.\r\n";
        WinJS.log && WinJS.log(message, "sample", "status");

        // Simulate downloading more data from the network for the contact.
        message += "Downloading data ...\r\n";
        WinJS.log && WinJS.log(message, "sample", "status");
        downLoadContactDataAsync(contact).then(
            function complete(result) {
                if (result.hasMoreData) {
                    // We get more data - update the contact card with the full set of contact data.
                    delayedDataLoader.setData(result.fullContact);
                    message += "ContactCardDelayedDataLoader.setData() was called.\r\n";
                    WinJS.log && WinJS.log(message, "sample", "status");
                }
                else {
                    // No more data to show - just close the data loader to tell the contact card UI all data has been set.
                    delayedDataLoader.close();
                }
            },
            function error(e) {
                WinJS.log && WinJS.log(e.message, "sample", "error");
            });
    }
})();
