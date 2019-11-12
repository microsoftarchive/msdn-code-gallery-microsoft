//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Smart Card";

    var scenarios = [
        {
            url: "/html/S1-CreateTPMVSC.html",
            title: "Create and provision a TPM virtual smart card"
        },
        {
            url: "/html/S2-ChangePin.html",
            title: "Change smart card PIN"
        },
        {
            url: "/html/S3-ResetPin.html",
            title: "Reset smart card PIN"
        },
        {
            url: "/html/S4-ChangeAdminKey.html",
            title: "Change smart card admin key"
        },
        {
            url: "/html/S5-VerifyResponse.html",
            title: "Verify response"
        },
        {
            url: "/html/S6-DeleteTPMVSC.html",
            title: "Delete TPM virtual smart card"
        },
        {
            url: "/html/S7-ListSmartCards.html",
            title: "List all smart cards"
        }
    ];

    function activated(eventObject) {

        if (eventObject.detail.kind ===
            Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Use setPromise to indicate to the system that the splash screen 
            // must not be torn down until after processAll and navigate
            // complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                // Navigate to either the first scenario or to the last running
                // scenario before suspension or termination.
                var url = WinJS.Application.sessionState.lastUrl ||
                          scenarios[0].url;
                return WinJS.Navigation.navigate(url);
            }));
        }
    }

    WinJS.Navigation.addEventListener("navigated", function (eventObject) {
        var url = eventObject.detail.location;
        var host = document.getElementById("contentHost");

        // Call unload method on current scenario, if there is one
        host.winControl && host.winControl.unload && host.winControl.unload();
        WinJS.Utilities.empty(host);
        eventObject.detail.setPromise(WinJS.UI.Pages.render(
            url,
            host,
            eventObject.detail.state).then(
        function () {
            WinJS.Application.sessionState.lastUrl = url;
        }));
    });

    // Calculates the response by encrypting the challenge with the admin key,
    // using the Triple DES (3DES) algorithm. When the card attempts
    // authentication, it compares the response to the expected response.
    // If the resulting values are the same, the authentication is successful.
    function calculateChallengeResponse(challenge, adminkey) {
        var objAlg =
            Windows.Security.Cryptography.Core.SymmetricKeyAlgorithmProvider
            .openAlgorithm(
                Windows.Security.Cryptography.Core.SymmetricAlgorithmNames
                .tripleDesCbc);

        var symetricKey = objAlg.createSymmetricKey(adminkey);
        var buffEncrypted =
            Windows.Security.Cryptography.Core.CryptographicEngine.encrypt(
            symetricKey,
            challenge,
            null);

        return buffEncrypted;
    };

    function getSmartCard() {
        return SdkSample.reader.findAllCardsAsync().then(
        function (smartCardList) {
            return smartCardList.getAt(0);
        });
    }

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios,
        adminKey: null,
        reader: null,
        calculateChallengeResponse: calculateChallengeResponse,
        getSmartCard: getSmartCard,
        ADMIN_KEY_LENGTH_IN_BYTES: 24
    });

    // This error handler will handle all exceptions which are not caught
    // in the application.
    WinJS.Application.addEventListener(
        "error",
        function (error) {
            WinJS.log && WinJS.log("Unexpected exception occurred: " +
                error.detail.error ? error.detail.error.toString() : "Unknown",
                "sample",
                "error");
            return true; // only if error is handled
        });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();

})();
