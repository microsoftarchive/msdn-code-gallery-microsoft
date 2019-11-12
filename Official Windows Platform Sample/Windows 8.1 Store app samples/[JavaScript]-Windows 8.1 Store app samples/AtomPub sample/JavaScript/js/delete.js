//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/delete.html", {
        ready: function (element, options) {
            document.getElementById("buttonStart").addEventListener("click", deleteStartClick, false);
            document.getElementById("buttonPrevious").addEventListener("click", previousItemButtonClick, false);
            document.getElementById("buttonNext").addEventListener("click", nextItemButtonClick, false);
            document.getElementById("buttonDelete").addEventListener("click", deleteDeleteClick, false);

            initializeCommon();
            displayCurrentItem = displayCurrentItemGet;
        }
    });

    // Download the feed at the given service address.
    function deleteStartClick() {
        clearLog();

        // If we retrieve the feed via the Edit uri then we will be logged in and will be
        // able to modify/delete the resource.

        // Since the value of 'serviceAddressField' was provided by the user it is untrusted input. We'll validate
        // it by creating a Uri instance and catching exceptions thrown by the constructor.
        // Note that this app has both "Internet (Client)" and "Home and Work Networking" capabilities set, since
        // the user may provide URIs for servers located on the intErnet or intrAnet. If apps only communicate
        // with servers on the intErnet, only the "Internet (Client)" capability should be set. Similarly if an
        // app is only intended to communicate on the intrAnet, only the "Home and Work Networking" capability should be set.
        var resourceUri;
        try {
            resourceUri = new Windows.Foundation.Uri(document.getElementById("serviceAddressField").value.trim() + defaultEditUri);
        } catch (error) {
            displayError("Error: Invalid URI");
            return;
        }

        // Refresh client in case server url or credential have changed.
        createClient();

        displayStatus("Fetching feed...");
        displayLog("Requested resource: " + resourceUri.displayUri);

        client.retrieveFeedAsync(resourceUri).done(function (feed) {
            currentFeed = feed;
            currentItemIndex = 0;

            displayLog("Got feed");
            var title = "(no title)";
            if (currentFeed.title) {
                title = currentFeed.title.text;
            }
            displayLog("Title: " + title);

            var currentItem = getCurrentItem();
            if (currentItem) {
                displayCurrentItem();
                displayLog("EditUri: " + currentItem.editUri.displayUri);
            }

            displayStatus("Fetching feed completed.");
        }, onError);
    }

    // Delete the current entry from the feed.
    function deleteDeleteClick() {
        clearLog();

        // Refresh client in case server url or credential have changed.
        createClient();

        var currentItem = getCurrentItem();
        if (!currentItem) {
            displayError("Error: No item currently displayed, please download a feed first.");
            return;
        }

        displayStatus("Deleting item...");
        displayLog("Item location: " + currentItem.editUri.displayUri);
        client.deleteResourceItemAsync(currentItem).done(function () {
            displayStatus("Deleting item completed.");

            // Our feed is now out of date.  Re-fetch the feed before deleting something else.
            currentFeed = null;
            document.getElementById("titleField").innerHTML = "&nbsp;";
            document.getElementById("webView").innerHTML = "&nbsp;";
        }, onError);
    }
})();
