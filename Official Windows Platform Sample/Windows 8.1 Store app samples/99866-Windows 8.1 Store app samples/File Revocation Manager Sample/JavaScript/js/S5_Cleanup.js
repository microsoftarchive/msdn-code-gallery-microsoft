//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/S5_Cleanup.html", {
        ready: function (element, options) {
            document.getElementById("Cleanup").addEventListener("click", doCleanup, false);
            SdkSample.validateFileExistence();
        }
    });

    function doCleanup() {
        try
        {
            var FolderStorageQuery;
            if (null !== SdkSample.sampleFolder) {
                FolderStorageQuery = SdkSample.sampleFolder.createItemQuery();
                FolderStorageQuery.getItemCountAsync().done(
                function (FolderItems) {
                    if (FolderItems > 0) {
                        WinJS.log && WinJS.log("You need to delete the items inside the " + RootPage.sampleFile.Name + " folder in order to regenerate the folder.", "sample", "error");
                        return;
                    }
                    else {
                        SdkSample.sampleFolder.deleteAsync().done( function () {
                            SdkSample.sampleFolder = null;
                        },
                        function (error) {
                            WinJS.log && WinJS.log( "Delete RevokeSample folder failed " + error, "sample", "error");
                        });
                    }
                },
                function (err) {
                    // If file doesn't exist
                    WinJS.log && WinJS.log("Get the items' count inside the RevokeSample failed with" + err, "sample", "error");
                });
            }

            if (null !== SdkSample.targetFolder) {
                FolderStorageQuery = SdkSample.targetFolder.createItemQuery();
                FolderStorageQuery.getItemCountAsync().done(
                function (FolderItems) {
                    if (FolderItems > 0) {
                        WinJS.log && WinJS.log("You need to delete the items inside the " + RootPage.sampleFile.Name + " folder in order to regenerate the folder.", "sample", "error");
                        return;
                    }
                    else {
                        SdkSample.targetFolder.deleteAsync().done( function () {
                            SdkSample.targetFolder = null;
                        },
                        function (error) {
                            WinJS.log && WinJS.log("Delete RevokeTarget folder failed " + error, "sample", "error");
                        });
                    }
                },
                function (err) {
                    // If file doesn't exist
                    WinJS.log && WinJS.log("Get the items' count inside the RevokeSample failed with " + err, "sample", "error");
                    return;
                });
            }

            if (null !== SdkSample.sampleFile)
            {
                SdkSample.sampleFile.deleteAsync().done( function () {
                    SdkSample.sampleFile = null;
                    },
                    function (error) {
                        WinJS.log && WinJS.log("Delete RevokeSample.txt file failed " + error, "sample", "error");
                    });
            }

            if (null !== SdkSample.targetFile)
            {
                SdkSample.targetFile.deleteAsync().done(function () {
                    SdkSample.TargetFile = null;
                },
                    function (error) {
                        WinJS.log && WinJS.log("Delete RevokeTarget.txt file failed " + error, "sample", "error");
                    });
            }

            if (null !== SdkSample.pickedFolder) {
                SdkSample.pickedFolder = null;
            }

            Windows.Storage.AccessCache.StorageApplicationPermissions.futureAccessList.clear();

            WinJS.log && WinJS.log("The files '" + SdkSample.sampleFilename + "' and " + SdkSample.targetFilename + " were deleted.\n" +
                                    "The folders '" + SdkSample.sampleFoldername + "' and '" + SdkSample.targetFoldername + "' were deleted.",
                                     "sample", "status" );
        }
        catch (e) {
            SdkSample.notifyUserFileNotExist();
        }
    }
})();
