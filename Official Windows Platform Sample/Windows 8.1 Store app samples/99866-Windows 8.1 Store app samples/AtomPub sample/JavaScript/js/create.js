//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/create.html", {
        ready: function (element, options) {
            document.getElementById("buttonSubmit").addEventListener("click", createStartClick, false);

            initializeCommon();
            displayCurrentItem = null;
        }
    });

    // Create a feed entry in the current feed.
    function createStartClick() {
        clearLog();

        // Since the value of 'serviceAddressField' was provided by the user it is untrusted input. We'll validate
        // it by creating a Uri instance and catching exceptions thrown by the constructor.
        // Note that this app has both "Internet (Client)" and "Home and Work Networking" capabilities set, since
        // the user may provide URIs for servers located on the intErnet or intrAnet. If apps only communicate
        // with servers on the intErnet, only the "Internet (Client)" capability should be set. Similarly if an
        // app is only intended to communicate on the intrAnet, only the "Home and Work Networking" capability should be set.
        var serviceUri;
        try {
            serviceUri = new Windows.Foundation.Uri(document.getElementById("serviceAddressField").value.trim() + defaultServiceDocUri);
        } catch (error) {
            displayError("Error: Invalid URI");
            return;
        }

        // Refresh client in case server url or credential have changed.
        createClient();

        var title = document.getElementById("titleField").value;

        // The title cannot be an empty string or a string with white spaces only, since it is used also
        // as the resource description (Slug header).
        if (title.trim() === "") {
            displayError("Error: Post title cannot be blank");
            return;
        }

        displayStatus("Performing operation...");
        displayLog("Fetching service document: " + serviceUri.displayUri);

        findEditUri(serviceUri).then(function (resourceUri) {
            if (!resourceUri) {
                displayError("Error: Edit uri not found in service document");
                return null;
            }

            displayLog("Uploading post: " + resourceUri.displayUri);

            var item = new Windows.Web.Syndication.SyndicationItem();
            item.title = new Windows.Web.Syndication.SyndicationText(title, Windows.Web.Syndication.SyndicationTextType.text);
            var content = document.getElementById("bodyField").value;
            item.content = new Windows.Web.Syndication.SyndicationContent(content, Windows.Web.Syndication.SyndicationTextType.html);

            return client.createResourceAsync(resourceUri, item.title.text, item);
        }).done(function (result) {
            if (result) {
                displayLog("Posted at " + result.itemUri.displayUri);
                displayStatus("New post created.");
            }
        }, onError);
    }

    // Find the edit uri for the feed from the service document.
    function findEditUri(serviceUri) {
        return client.retrieveServiceDocumentAsync(serviceUri).then(function (serviceDocument) {
            for (var i in serviceDocument.workspaces) {
                var workspace = serviceDocument.workspaces[i];
                for (var j in workspace.collections) {
                    var collection = workspace.collections[j];

                    if (collection.accepts.join(";") === "application/atom+xml;type=entry") {
                        return collection.uri;
                    }
                }
            }

            return null;
        }, onError);
    }
})();
