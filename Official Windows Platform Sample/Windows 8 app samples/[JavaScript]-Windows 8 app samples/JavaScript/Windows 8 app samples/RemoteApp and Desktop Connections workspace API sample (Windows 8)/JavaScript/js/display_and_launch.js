//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/display_and_launch.html", {
        ready: function (element, options) {
            document.getElementById("refreshWkspItems").addEventListener("click", refreshWorkspaceItems, false);
        }
    });

    function refreshWorkspaceItems() {
        // Initialize the ActiveX control
        var wkspActiveX = null;
        var hasEverBeenSubscribed = true;

        try {
            wkspActiveX = Microsoft.Sample.WorkspaceBrokerApi.WorkspaceActiveX.createInstance();
        } catch (e) {
            WinJS.log && WinJS.log("Error setting up the Workspace ActiveX control. Error: " + e.number + " " + e.description, "sample", "error");
            return;
        }

        try {
            wkspActiveX.InitializeWorkspaceConfiguration();
        } catch (e) {
            if (e.number === -2147024894) {
                // This is equivalent to 0x80070002 (ERROR_FILE_NOT_FOUND), and is expected if you have never been subscribed to any workspaces on this machine
                hasEverBeenSubscribed = false;
            } else {
                WinJS.log && WinJS.log("Error calling InitializeWorkspaceConfiguration: " + e.number + " " + e.description, "sample", "error");
                wkspActiveX = null;
                return;
            }
        }

        if (!wkspActiveX) {
            WinJS.log && WinJS.log("Cannot continue, Workspace ActiveX control not ready", "sample", "error");
            return;
        }
        if (!hasEverBeenSubscribed) {
            WinJS.log && WinJS.log("No resources to display, you have never been subscribed to a workspace on this machine.", "sample", "status");
            return;
        }

        WinJS.log && WinJS.log("Workspace ActiveX control ready", "sample", "status");

        // First clear out any old resources in the UI
        var itemsRegion = document.getElementById("wkspItems");
        while (itemsRegion.hasChildNodes()) { 
            itemsRegion.removeChild(itemsRegion.lastChild); 
        }

        try {
            var numWorkspaces = wkspActiveX.WorkspacesCount;

            if (numWorkspaces === 0) {
                WinJS.log && WinJS.log("No resources to display, you are currently not subscribed to any workspaces.", "sample", "status");
                return;
            }

            for (var wkspIdx = 0; wkspIdx < numWorkspaces; wkspIdx++) {
                var wkspId = wkspActiveX.WorkspaceId(wkspIdx);
                var wkspName = wkspActiveX.WorkspaceName(wkspIdx);

                var wkspNameTag = document.createElement("h2");
                wkspNameTag.textContent = wkspName;
                itemsRegion.appendChild(wkspNameTag);

                var numFolders = wkspActiveX.WorkspaceFoldersCount(wkspId);
                
                for (var folderIdx = 0; folderIdx < numFolders; folderIdx++) {
                    var folderName = wkspActiveX.WorkspaceFolderName(wkspId, folderIdx);

                    var folderNameTag = document.createElement("h3");
                    folderNameTag.textContent = folderName;
                    itemsRegion.appendChild(folderNameTag);

                    var numItems = wkspActiveX.WorkspaceFolderContentsCount(wkspId, folderName);
                    
                    if (numItems > 0) {
                        var listTag = document.createElement("ul");

                        for (var itemIdx = 0; itemIdx < numItems; itemIdx++) {
                            var listItemTag = generateWorkspaceItemElement(wkspActiveX, wkspId, folderName, itemIdx);                          
                            listTag.appendChild(listItemTag);
                        }

                        itemsRegion.appendChild(listTag);
                    }
                }
            }
        } catch (e) {
            WinJS.log && WinJS.log("Error displaying workspace resources: " + e.number + " " + e.message, "sample", "error");
            return;
        }

        WinJS.log && WinJS.log("Workspace Resource refresh complete", "sample", "status");
    }

    function generateWorkspaceItemElement(wkspActiveX, wkspId, folderName, itemIdx) {
        var itemName = wkspActiveX.WorkspaceFolderItemName(wkspId, folderName, itemIdx);
        var itemIcon = wkspActiveX.WorkspaceFolderImageData(wkspId, folderName, itemIdx);
        var itemFileType = wkspActiveX.WorkspaceFolderItemFileExtension(wkspId, folderName, itemIdx);
        var isDesktop = wkspActiveX.IsWorkspaceFolderItemRemoteDesktop(wkspId, folderName, itemIdx);

        // If desired, you can use itemIcon to display an icon image for the resources by generating an HTML tag similar to the following:
        // <img src="data:image/png;base64,contents-of-itemIcon-variable" />

        // If desired, you can distinguish between resources types (e.g. ".rdp" vs. 3rd party file types) by examining the itemFileType variable

        // If desired, you can seperate Desktop from Non-Desktop (e.g. RemoteApp) resources by filtering on the isDesktop variable

        var resourceLinkTag = document.createElement("a");
        resourceLinkTag.textContent = itemName;
        resourceLinkTag.addEventListener("click", function () {
            try {
                wkspActiveX.LaunchWorkspaceItem(wkspId, folderName, itemName, "sample activation context");
            } catch (e) {
                WinJS.log && WinJS.log("Error launching workspace item: " + e.number + " " + e.message, "sample", "error");
            }
        }, false);

        var listItemTag = document.createElement("li");
        listItemTag.appendChild(resourceLinkTag);

        return listItemTag;   
    }
})();
