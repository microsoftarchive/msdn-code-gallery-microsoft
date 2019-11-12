//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var activation = Windows.ApplicationModel.Activation;
    var activationKinds = activation.ActivationKind;
    var phoneContactStore = Windows.Phone.PersonalInformation;

    // Handle launch and continuation activation kinds.
    // Create data store and add contacts to the store.
    function activated(eventObject) {
        var activationKind = eventObject.detail.kind;
        var activatedEventArgs = eventObject.detail.detail;

        // Handle launch and continuation activation kinds
        switch (activationKind) {
            case activationKinds.launch:
            case activationKinds.pickFileContinuation:
            case activationKinds.pickSaveFileContinuation:
            case activationKinds.pickFolderContinuation:
            case activationKinds.webAuthenticationBrokerContinuation:
                var p = WinJS.UI.processAll().then(function () {
                        // Navigate to either the first scenario or to the last running scenario
                        // before suspension or termination.
                        var url = WinJS.Application.sessionState.lastUrl || "/pages/home/home.html";
                        var initialState = { activationKind: activationKind, activatedEventArgs: activatedEventArgs };
                        WinJS.Navigation.history.current.initialPlaceholder = true;
                        return WinJS.Navigation.navigate(url, initialState);
                });

                // Calling done on a promise chain allows unhandled exceptions to propagate.
                p.done();

                // Use setPromise to indicate to the system that the splash screen must not be torn down
                // until after processAll and navigate complete asynchronously.
                eventObject.setPromise(p);

                Windows.Phone.PersonalInformation.ContactStore.createOrOpenAsync(
                    Windows.Phone.PersonalInformation.ContactStoreSystemAccessMode.ReadWrite,
                    Windows.Phone.PersonalInformation.ContactStoreApplicationAccessMode.ReadOnly).then(function (contactStore) {
                        SdkSample.sampleContacts.forEach(function (sampleContact) {
                            var contact = Windows.Phone.PersonalInformation.StoredContact(contactStore);
                            contact.getPropertiesAsync().then(function (propsz) {
                                if (sampleContact.firstName) {
                                    propsz.insert(Windows.Phone.PersonalInformation.KnownContactProperties.givenName, sampleContact.firstName);
                                }

                                if (sampleContact.lastName) {
                                    propsz.insert(Windows.Phone.PersonalInformation.KnownContactProperties.familyName, sampleContact.lastName);
                                }

                                if (sampleContact.personalEmail) {
                                    propsz.insert(Windows.Phone.PersonalInformation.KnownContactProperties.email, sampleContact.personalEmail);
                                }
                            }).then(function () {
                                contact.saveAsync();
                            });
                        });
                    });
                break;

            default:
                break;
        }
    }

    function navigating(eventObject) {
        var url = eventObject.detail.location;
        var host = document.getElementById("contentHost");
        var isNavigatingBack = eventObject.detail.delta < 0;
        var animationType = WinJS.UI.PageNavigationAnimation.turnstile;
        var animations = WinJS.UI.Animation.createPageNavigationAnimations(animationType, animationType, isNavigatingBack);
        WinJS.Application.sessionState.lastUrl = url;

        var p = animations.exit(host.children).then(function () {
                // Call unload method on current scenario, if there is one
                host.winControl && host.winControl.unload && host.winControl.unload();
                WinJS.Utilities.disposeSubTree(host);
                WinJS.Utilities.empty(host);
                return WinJS.UI.Pages.render(url, host, eventObject.detail.state);
        }).then(function () {
            animations.entrance(host.children);
           });
        p.done();
        eventObject.detail.setPromise(p);
    }

    WinJS.Navigation.addEventListener("navigating", navigating);
    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();