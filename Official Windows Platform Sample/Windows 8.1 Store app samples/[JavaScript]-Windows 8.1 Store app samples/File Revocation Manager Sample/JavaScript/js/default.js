//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "File Revocation Manager ";

    var pickedFolder = null;
    var targetFile = null;
    var targetFolder = null;
    var sampleFile = null;
    var sampleFolder = null;
    var sampleFileStatus = null;
    var sampleFolderStatus = null;
    var targetFileStatus = null;
    var targetFolderStatus = null;
    var sampleFilename = "RevokeSample.txt";
    var sampleFoldername = "RevokeSample";
    var targetFilename = "RevokeTarget.txt";
    var targetFoldername = "RevokeTarget";
    var pickedFolderToken = "pickedFolderToken";

    var scenarios = [
        { url: "/html/S1_Protect.html", title: "Protect a file or folder with an enterprise identity" },
        { url: "/html/S2_CopyProtection.html", title: "Copy enterprise protection" },
        { url: "/html/S3_GetStatus.html", title: "Get the protection status of the files and folders" },
        { url: "/html/S4_Revoke.html", title: "Revoke an enterprise identity" },
        { url: "/html/S5_Cleanup.html", title: "Cleanup the files and folders" }
    ];

    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                // Navigate to either the first scenario or to the last running scenario
                // before suspension or termination.
                var url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
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
        eventObject.detail.setPromise(WinJS.UI.Pages.render(url, host, eventObject.detail.state).then(function () {
            WinJS.Application.sessionState.lastUrl = url;
        }));
    });

    function validateFileExistence() {
        try
        {

            if (Windows.Storage.AccessCache.StorageApplicationPermissions.futureAccessList.containsItem(SdkSample.pickedFolderToken))
            {
                SdkSample.pickedFolder = null;

                Windows.Storage.AccessCache.StorageApplicationPermissions.futureAccessList.getFolderAsync(SdkSample.pickedFolderToken).done(
                    function (folder)
                    {
                        SdkSample.pickedFolder = folder;

                        SdkSample.sampleFile = null;
                        SdkSample.targetFile = null;

                        SdkSample.sampleFolder = null;
                        SdkSample.targetFolder = null;

                        SdkSample.pickedFolder.createFileAsync(SdkSample.sampleFilename, Windows.Storage.CreationCollisionOption.openIfExists).done(
                            function (file)
                            {
                                SdkSample.sampleFile = file;
                            }
                        );

                        SdkSample.pickedFolder.createFileAsync(SdkSample.targetFilename, Windows.Storage.CreationCollisionOption.openIfExists).done(
                            function (file)
                            {
                                SdkSample.targetFile = file;
                            }
                        );

                        SdkSample.pickedFolder.createFolderAsync(SdkSample.sampleFoldername, Windows.Storage.CreationCollisionOption.openIfExists).done(
                            function (fldr)
                            {
                                SdkSample.sampleFolder = fldr;
                            }
                        );

                        SdkSample.pickedFolder.createFolderAsync(SdkSample.targetFoldername, Windows.Storage.CreationCollisionOption.openIfExists).done(
                            function (fldr)
                            {
                                SdkSample.targetFolder = fldr;
                            }
                        );
                    }
                );
            }
        }
        catch( e )
        {
            notifyUserFileNotExist();
        }

    };

    function notifyUserFileNotExist() {
        WinJS.log && WinJS.log("A file or folder used by the application does not exist.\n" +
                                "Please try again after clicking the Setup Button in the Protect a file or folder with an Enterprise Identity scenario.", "sample", "error");
    }

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios,
        validateFileExistence: validateFileExistence,
        notifyUserFileNotExist: notifyUserFileNotExist,
        sampleFile: sampleFile,
        sampleFolder: sampleFolder,
        targetFile: targetFile,
        targetFolder: targetFolder,
        sampleFilename: sampleFilename,
        sampleFoldername: sampleFoldername,
        targetFilename: targetFilename,
        targetFoldername: targetFoldername,
        sampleFileStatus: sampleFileStatus,
        sampleFolderStatus: sampleFolderStatus,
        targetFileStatus: targetFileStatus,
        targetFolderStatus: targetFolderStatus
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
