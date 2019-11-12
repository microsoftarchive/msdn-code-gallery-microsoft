//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// Requires workspaceActiveX.js

(function () {
    "use strict";
    var wkspActiveX = null;

    var page = WinJS.UI.Pages.define("/html/subscribe.html", {
        ready: function (element, options) {
            document.getElementById("subscribe").addEventListener("click", doSubscribe, false);

            // Initialize the ActiveX control
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
                } else {
                    WinJS.log && WinJS.log("Error calling InitializeWorkspaceConfiguration: " + e.number + " " + e.description, "sample", "error");
                    return;
                }
            }

            // Register for workspace subsciption results
            wkspActiveX.attachEvent("OnWorkspaceSetupCompleted", onWorkspaceSetupCompletedHandler);

            WinJS.log && WinJS.log("Workspace ActiveX control ready", "sample", "status");
        }
    });

    function doSubscribe() {
        try {
            wkspActiveX.SetupWorkspace();
        } catch (e) {
            WinJS.log && WinJS.log("Error calling SetupWorkspace: " + e.number + " " + e.description, "sample", "error");
        }
    }

    function onWorkspaceSetupCompletedHandler(statusCode) {
        WinJS.log && WinJS.log("Workspace subscription succeeded, or a subscribed workspace was modified. Status code: " + statusCode, "sample", "status");
        // This tells you it is now time to update your list of workspaces, workspace folders, and workspace items (see the other scenario in this sample for details)
    }
})();
