//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/retrieve.html", {
        ready: function (element, options) {
            document.getElementById("buttonStart").addEventListener("click", retieveStartClick, false);
            document.getElementById("buttonPrevious").addEventListener("click", previousItemButtonClick, false);
            document.getElementById("buttonNext").addEventListener("click", nextItemButtonClick, false);

            initializeCommon();
            displayCurrentItem = displayCurrentItemGet;
        }
    });

    // Retrieve and display the feed at given service address.
    function retieveStartClick() {
        clearLog();

        // Note that this feed is public by default and will not require authentication.
        // We will only get back a limited use feed, without information about editing.

        // Since the value of 'serviceAddressField' was provided by the user it is untrusted input. We'll validate
        // it by creating a Uri instance and catching exceptions thrown by the constructor.
        // Note that this app has both "Internet (Client)" and "Home and Work Networking" capabilities set, since
        // the user may provide URIs for servers located on the intErnet or intrAnet. If apps only communicate
        // with servers on the intErnet, only the "Internet (Client)" capability should be set. Similarly if an
        // app is only intended to communicate on the intrAnet, only the "Home and Work Networking" capability should be set.
        var resourceUri;
        try {
            resourceUri = new Windows.Foundation.Uri(document.getElementById("serviceAddressField").value.trim() + defaultFeedUri);
        } catch (error) {
            displayError("Error: Invalid URI");
            return;
        }
        
        // Refresh client in case server url or credential have changed.
        createClient();

        displayStatus("Fetching resource...");
        displayLog("Requested resource: " + resourceUri.displayUri);
        client.retrieveFeedAsync(resourceUri).done(function (feed) {
            currentFeed = feed;
            currentItemIndex = 0;

            displayStatus("Fetching resource completed.");
            displayCurrentItem();
        }, onError);
    }
})();
