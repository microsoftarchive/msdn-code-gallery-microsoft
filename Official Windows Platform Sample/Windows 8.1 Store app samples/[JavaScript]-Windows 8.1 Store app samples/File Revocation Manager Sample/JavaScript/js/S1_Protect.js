//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/S1_Protect.html", {
        ready: function (element, options) {
            document.getElementById("Setup").addEventListener("click", doSetUp, false);
            document.getElementById("ProtectFile").addEventListener("click", doProtectFile, false);
            document.getElementById("ProtectFolder").addEventListener("click", doProtectFolder, false);
            SdkSample.validateFileExistence();
        }
    });

    function doSetUp() {
        try
        {
            var folderStorageQuery;
            if (null !== SdkSample.sampleFolder)
            {
                folderStorageQuery = SdkSample.sampleFolder.createItemQuery();
                folderStorageQuery.getItemCountAsync().done(
                    function(folderItems)
                    {
                        if (folderItems > 0)
                        {
                            WinJS.log && WinJS.log("You need to delete the items inside the " + SdkSample.sampleFolder.name + " folder in order to regenerate the folder.", "sample", "error");
                            return;
                        }
                    },
                    function (err) {
                        // If file doesn't exist
                        WinJS.log && WinJS.log("Get items count inside the " + SdkSample.sampleFolder.name + " failed with" + err, "sample", "error");
                        return;
                    }
                );

                SdkSample.sampleFolder.deleteAsync().done(
                    function (Success)
                    {
                        SdkSample.sampleFolder = null;
                    },
                    function (err) {
                        WinJS.log && WinJS.log("Delete " + SdkSample.sampleFolder.name +" failed " + err, "sample", "error");
                        return;
                    }
                );
            }

            if (null !== SdkSample.targetFolder)
            {
                folderStorageQuery = SdkSample.targetFolder.createItemQuery();
                folderStorageQuery.getItemCountAsync().done(
                    function(folderItems)
                    {
                        if (folderItems > 0)
                        {
                            WinJS.log && WinJS.log("You need to delete the items inside the " + SdkSample.targetFolder.name + " folder in order to regenerate the folder.", "sample", "error");
                            return;
                        }
                    },
                    function (err) {
                        // If file doesn't exist
                        WinJS.log && WinJS.log("Get items count inside the " + SdkSample.targetFolder.name + " failed with" + err, "sample", "error");
                        return;
                    }
                );

                SdkSample.targetFolder.deleteAsync().done(
                    function (Success)
                    {
                        SdkSample.targetFolder = null;
                    },
                    function (err) {
                        WinJS.log && WinJS.log("Delete " + SdkSample.targetFolder.name +" failed " + err, "sample", "error");
                        return;
                    }
                );
            }

            if (null !== SdkSample.sampleFile)
            {
                SdkSample.sampleFile.deleteAsync().done(
                    function (Success)
                    {
                        SdkSample.sampleFile = null;
                    },
                    function (err) {
                        WinJS.log && WinJS.log("Delete " + SdkSample.sampleFile.name +" failed " + err, "sample", "error");
                        return;
                    }
                );
            }

            if (null !== SdkSample.targetFile)
            {
                SdkSample.targetFile.deleteAsync().done(
                    function (Success)
                    {
                        SdkSample.targetFile = null;
                    },
                    function (err) {
                        WinJS.log && WinJS.log("Delete " + SdkSample.targetFile.name +" failed " + err, "sample", "error");
                        return;
                    }
                );
            }

            if (null !== SdkSample.pickedFolder)
            {
                SdkSample.pickedFolder = null;
            }

            Windows.Storage.AccessCache.StorageApplicationPermissions.futureAccessList.clear();

            var folderPicker = new Windows.Storage.Pickers.FolderPicker;
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.documentsLibrary;
            folderPicker.fileTypeFilter.append(".docx");
            folderPicker.fileTypeFilter.append(".xlsx");
            folderPicker.fileTypeFilter.append(".pptx");
            folderPicker.fileTypeFilter.append(".txt");

            folderPicker.pickSingleFolderAsync().done(
                function (folder)
                {
                    SdkSample.pickedFolder = folder;

                    if (null === SdkSample.pickedFolder)
                    {
                        WinJS.log && WinJS.log("Please choose a base folder in which to create the SDK Sample related files and folders by clicking the Setup button.", "sample", "error");
                        return;
                    }
                    else
                    {
                        Windows.Storage.AccessCache.StorageApplicationPermissions.futureAccessList.addOrReplace(SdkSample.pickedFolderToken, SdkSample.pickedFolder);

                        SdkSample.pickedFolder.createFolderAsync(SdkSample.sampleFoldername, Windows.Storage.CreationCollisionOption.replaceExisting).done(function (folderOne)
                        {
                            SdkSample.sampleFolder = folderOne;

                            SdkSample.pickedFolder.createFolderAsync(SdkSample.targetFoldername, Windows.Storage.CreationCollisionOption.replaceExisting).done(function (folderTwo)
                            {
                                SdkSample.targetFolder = folderTwo;

                                SdkSample.pickedFolder.createFileAsync(SdkSample.sampleFilename, Windows.Storage.CreationCollisionOption.replaceExisting).done(function (file)
                                {
                                    SdkSample.sampleFile = file;

                                    SdkSample.pickedFolder.createFileAsync(SdkSample.targetFilename, Windows.Storage.CreationCollisionOption.replaceExisting).done(function (files)
                                    {
                                        SdkSample.targetFile = files;
                                        WinJS.log && WinJS.log("The files " + SdkSample.sampleFile.name + " and " + SdkSample.targetFile.name + " were created.\n" +
                                                            "The folders " + SdkSample.sampleFolder.name + " and " + SdkSample.targetFolder.name + " were created.", "sample", "status");
                                    });
                                });
                            });
                        });
                    }
                });
        }
        catch (e)
        {
            SdkSample.notifyUserFileNotExist();
        }
    }

    function doProtectFile() {
        if (null === SdkSample.sampleFile)
        {
            WinJS.log && WinJS.log("You need to click the Setup button first.", "sample", "error");
            return;
        }

        if ("" === EnterpiseID.value)
        {
            WinJS.log && WinJS.log("Please enter an Enterpise ID that you want to use.", "sample", "error");
            return;
        }

        var outputDiv = document.getElementById("output");
        outputDiv.innerHTML = "";
        try{
            Windows.Security.EnterpriseData.FileRevocationManager.protectAsync(SdkSample.sampleFile, EnterpiseID.value).done(
                function (rslt) {
                    var protectStatus = rslt;
                    outputDiv.innerHTML += "The protection status of the file " + SdkSample.sampleFilename + " is " + protectStatus + "<br /><br />";
                },
                function (err) {

                    //
                    // NOTE: Generally you should not rely on exception handling
                    // to validate an Enterprise ID string. In real-world
                    // applications, the domain name of the enterprise might be
                    // parsed out of an email address or a URL, and may even be
                    // entered by a user. Your app-specific code to extract the
                    // Enterprise ID should validate the Enterprise ID string is an
                    // internationalized domain name before passing it to
                    // ProtectAsync.
                    //

                    if (err.number === -2147024809) { //InvalidArgumentException
                        outputDiv.innerHTML += "<span style='color:blue'>" +
                                               "Given Enterprise ID string is invalid.<br />" +
                                               "Please try again using a properly formatted Internationalized Domain Name as the Enterprise ID string." +
                                               "</span>", "sample", "error";
                    }
                    else {
                        WinJS.log && WinJS.log("Protect RevokeSample.txt file failed " + err, "sample", "error");
                    }
                    return;
                });
        }
        catch (Exception)
        {
            WinJS.log && WinJS.log("Protect RevokeSample.txt exception " + Exception, "sample", "error");
        }
    }

    function doProtectFolder() {
        if (null === SdkSample.sampleFolder) {
            WinJS.log && WinJS.log("You need to click the Setup button first.", "sample", "error");
            return;
        }

        if ("" === EnterpiseID.value) {
            WinJS.log && WinJS.log("Please enter an Enterpise ID that you want to use.", "sample", "error");
            return;
        }

        // Make sure the folder is empty before you protect it
        var FolderStorageQuery = SdkSample.sampleFolder.createItemQuery();
        FolderStorageQuery.getItemCountAsync().done(
        function (FolderItems) {
            if (FolderItems > 0) {
                WinJS.log && WinJS.log("You need to delete the items inside the " + SdkSample.sampleFolderame + " folder in order to regenerate the folder.", "sample", "error");
                return;
            }
        },
        function (err) {
            WinJS.log && WinJS.log("Get the items' count inside the RevokeSample failed with " + err, "sample", "error");
            return;
        });

        var outputDiv = document.getElementById("output");
        outputDiv.innerHTML = "";
        try {
            Windows.Security.EnterpriseData.FileRevocationManager.protectAsync(SdkSample.sampleFolder, EnterpiseID.value).done(
                function (rslt) {
                    outputDiv.innerHTML += "The protection status of the folder " + SdkSample.sampleFoldername + " is " + rslt + "<br /><br />";
                },
                function (err) {

                    //
                    // NOTE: Generally you should not rely on exception handling
                    // to validate an Enterprise ID string. In real-world
                    // applications, the domain name of the enterprise might be
                    // parsed out of an email address or a URL, and may even be
                    // entered by a user. Your app-specific code to extract the
                    // Enterprise ID should validate the Enterprise ID string is an
                    // internationalized domain name before passing it to
                    // ProtectAsync.
                    //

                    if (err.number === -2147024809) { //InvalidArgumentException
                        outputDiv.innerHTML += "<span style='color:blue'>" +
                                               "Given Enterprise ID string is invalid.<br />" +
                                               "Please try again using a properly formatted Internationalized Domain Name as the Enterprise ID string." +
                                               "</span>", "sample", "error";
                    }
                    else {
                        WinJS.log && WinJS.log("Protect RevokeSample folder failed " + err, "sample", "error");
                    }

                    return;
                });
        }
        catch (Exception) {
            WinJS.log && WinJS.log("Protect RevokeSample folder exception " + Exception, "sample", "error");
        }
    }
})();
