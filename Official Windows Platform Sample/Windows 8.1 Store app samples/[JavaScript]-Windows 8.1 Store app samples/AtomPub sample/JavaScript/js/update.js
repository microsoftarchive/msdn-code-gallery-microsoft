//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/update.html", {
        ready: function (element, options) {
            document.getElementById("buttonStart").addEventListener("click", updateStartClick, false);
            document.getElementById("buttonPrevious").addEventListener("click", previousItemButtonClick, false);
            document.getElementById("buttonNext").addEventListener("click", nextItemButtonClick, false);
            document.getElementById("buttonSave").addEventListener("click", updateSaveClick, false);

            initializeCommon();
            displayCurrentItem = displayCurrentItemUpdate;
        }
    });

    // Retrieve and display the feed at given service address.
    function updateStartClick() {
        clearLog();

        // If we retrieve the feed via the Edit uri then we will be logged in and be
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
        displayLog("Requested resource: " + resourceUri.absoluteUri);

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
                displayLog("EditUri: " + currentItem.editUri.absoluteUri);
            }

            displayStatus("Fetching feed completed.");
        }, onError);
    }

    // Update feed entry with changes made to inputs fields.
    function updateSaveClick() {
        clearLog();

        createClient();

        var currentItem = getCurrentItem();
        if (!currentItem) {
            displayError("Error: No item currently displayed, please download a feed first.");
            return;
        }

        displayStatus("Updating item...");
        displayLog("Item location: " + currentItem.editUri.absoluteUri);

        // Update the item
        var updatedItem = new Windows.Web.Syndication.SyndicationItem();
        var title = document.getElementById("titleField").value;
        updatedItem.title = new Windows.Web.Syndication.SyndicationText(title, Windows.Web.Syndication.SyndicationTextType.text);
        var content = document.getElementById("bodyField").value;
        updatedItem.content = new Windows.Web.Syndication.SyndicationContent(content, Windows.Web.Syndication.SyndicationTextType.html);

        client.updateResourceAsync(currentItem.editUri, updatedItem).done(function () {
            displayStatus("Updating item completed.");
        }, onError);
    }

    // Display current syndication item
    function displayCurrentItemUpdate() {
        var currentItem = getCurrentItem();
        if (currentItem) {
            var title = "(no title)";
            if (currentItem.title) {
                title = currentItem.title.text;
            }
            document.getElementById("titleField").value = title;
            var value = "(no value)";
            if (currentItem.content) {
                value = currentItem.content.text;
            }
            else if (currentItem.summary) {
                value = currentItem.summary.text;
            }
            document.getElementById("bodyField").value = value;
        }
    }
})();
