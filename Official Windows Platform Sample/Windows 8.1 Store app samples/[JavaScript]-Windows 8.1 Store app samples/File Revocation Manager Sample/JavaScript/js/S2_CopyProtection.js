//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/S2_CopyProtection.html", {
        ready: function (element, options) {
            document.getElementById("CopyProtectionToFile").addEventListener("click", doCopyProtectionToFile, false);
            document.getElementById("CopyProtectionToFolder").addEventListener("click", doCopyProtectionToFolder, false);
            SdkSample.validateFileExistence();
        }
    });

    function doCopyProtectionToFile() {
        if ((null === SdkSample.sampleFile) || (null === SdkSample.targetFile))
        {
            WinJS.log && WinJS.log("You need to click the Setup button in the 'Protect a file or folder with an Enterprise Identity' scenario.", "sample", "error");
            return;
        }

        Windows.Security.EnterpriseData.FileRevocationManager.copyProtectionAsync(SdkSample.sampleFile, SdkSample.targetFile).done(
            function (rslt)
            {
                if (!rslt)
                {
                    // Make sure the source file is protected
                    Windows.Security.EnterpriseData.FileRevocationManager.getStatusAsync( SdkSample.sampleFile ).done(
                        function( protectStatus )
                        {
                            if( Windows.Security.EnterpriseData.FileProtectionStatus.protected !== protectStatus )
                            {
                                WinJS.log && WinJS.log("The protection cannot be copied since the status of the source file " + SdkSample.sampleFilename + " is " + protectStatus + ".\n" +
                                                        "Please try again after clicking the Setup Button followed by the Protect File button in the 'Protect a file or folder with an Enterprise Identity' scenario.",
                                                        "sample", "error");
                                return;
                            }
                        },
                        function( err ){
                            WinJS.log && WinJS.log("Get status of RevokeSample.txt file failed " + err, "sample", "error");
                        });

                    // Get the target file's protection status
                    Windows.Security.EnterpriseData.FileRevocationManager.getStatusAsync( SdkSample.targetFile ).done(
                        function( protectStatus )
                        {
                            // Check the target file's protection status
                            if( Windows.Security.EnterpriseData.FileProtectionStatus.protected === protectStatus )
                            {
                                WinJS.log && WinJS.log("The protection cannot be copied since the target file " + SdkSample.targetFilename + " is already protected by another Enterprise Identity.\n" +
                                                        "Please try again after clicking the Setup Button followed by the Protect File button in the 'Protect a file or folder with an Enterprise Identity' scenario.",
                                                        "sample", "error");
                                return;
                            }
                            else
                            {
                                WinJS.log && WinJS.log("The protection cannot be copied since the status of the target file " + SdkSample.targetFilename + " is " + protectStatus + ".\n" +
                                                        "Please try again after clicking the Setup Button followed by the Protect File button in the 'Protect a file or folder with an Enterprise Identity' scenario.",
                                                        "sample", "error");
                            }
                        },
                        function( err ){
                            WinJS.log && WinJS.log("Get status of RevokeTarget.txt file failed " + err, "sample", "error");
                        });

                }
                else
                {
                    Windows.Security.EnterpriseData.FileRevocationManager.getStatusAsync( SdkSample.targetFile ).done(
                        function( protectStatus )
                        {
                            WinJS.log && WinJS.log("The protection was copied.\n" +
                                                    "The protection status of the target file " + SdkSample.targetFilename + " is " + protectStatus,
                                                    "sample", "status");
                        },
                        function (err) {
                            WinJS.log && WinJS.log("Get status of RevokeTarget.txt file failed " + err, "sample", "error");
                        });
                }
            },
            function (err){}
        );
    }

    function doCopyProtectionToFolder() {
        if ((null === SdkSample.sampleFolder) || (null === SdkSample.targetFolder)) {
            WinJS.log && WinJS.log("You need to click the Setup button in the 'Protect a file or folder with an Enterprise Identity' scenario.", "sample", "error");
            return;
        }

        // Make sure the folder is empty before you protect it
        var FolderStorageQuery = SdkSample.targetFolder.createItemQuery();
        FolderStorageQuery.getItemCountAsync().done(
        function (FolderItems) {
            if (FolderItems > 0) {
                WinJS.log && WinJS.log("You need to delete the items inside the " + RootPage.sampleFoldername + " folder in order to regenerate the folder.", "sample", "error");
                return;
            }
        },
        function (err) {
            WinJS.log && WinJS.log("Get the items' count inside the RevokeSample failed with " + err, "sample", "error");
            return;
        });

        Windows.Security.EnterpriseData.FileRevocationManager.copyProtectionAsync(SdkSample.sampleFolder, SdkSample.targetFolder).done(
           function (rslt) {
               if (!rslt) {
                   // Make sure the source file is protected
                   Windows.Security.EnterpriseData.FileRevocationManager.getStatusAsync(SdkSample.sampleFolder).done(
                       function (protectStatus) {
                           if (Windows.Security.EnterpriseData.FileProtectionStatus.protected !== protectStatus) {
                               WinJS.log && WinJS.log("The protection cannot be copied since the status of the source folder " + SdkSample.sampleFoldername + " is " + protectStatus + ".\n" +
                                                       "Please try again after clicking the Setup Button followed by the Protect folder button in the 'Protect a file or folder with an Enterprise Identity' scenario.",
                                                       "sample", "error");
                               return;
                           }
                       },
                       function (err) {
                           WinJS.log && WinJS.log("Get status of RevokeSample folder failed " + err, "sample", "error");
                       });

                   // Get the target folder's protection status
                   Windows.Security.EnterpriseData.FileRevocationManager.getStatusAsync(SdkSample.targetFolder).done(
                       function (protectStatus) {
                           // Check the target folder's protection status
                           if (Windows.Security.EnterpriseData.FileProtectionStatus.protected === protectStatus) {
                               WinJS.log && WinJS.log("The protection cannot be copied since the target folder " + SdkSample.targetFoldername + " is already protected by another Enterprise Identity.\n" +
                                                       "Please try again after clicking the Setup Button followed by the Protect folder button in the 'Protect a file or folder with an Enterprise Identity' scenario.",
                                                       "sample", "error");
                               return;
                           }
                           else {
                               WinJS.log && WinJS.log("The protection cannot be copied since the status of the target folder " + SdkSample.targetFoldername + " is " + protectStatus + ".\n" +
                                                       "Please try again after clicking the Setup Button followed by the Protect folder button in the 'Protect a file or folder with an Enterprise Identity' scenario.",
                                                       "sample", "error");
                           }
                       },
                       function (err) {
                           WinJS.log && WinJS.log("Get status of RevokeTarget folder failed " + err, "sample", "error");
                       });

               }
               else {
                   Windows.Security.EnterpriseData.FileRevocationManager.getStatusAsync(SdkSample.targetFolder).done(
                       function (protectStatus) {
                           WinJS.log && WinJS.log("The protection was copied.\n" +
                                                   "The protection status of the target folder " + SdkSample.targetFoldername + " is " + protectStatus,
                                                   "sample", "status");
                       },
                       function (err) {
                           WinJS.log && WinJS.log("Get status of RevokeTarget folder failed " + err, "sample", "error");
                       });
               }
           },
           function (err) { }
       );
    }
})();
