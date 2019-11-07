//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario6.html", {
        ready: function (element, options) {
            document.getElementById("addToList").addEventListener("click", addToList, false);
            document.getElementById("showList").addEventListener("click", showList, false);
            document.getElementById("openFromList").addEventListener("click", openFromList, false);
            //to test if "sample.dat" is created
            SdkSample.validateFileExistence();
        }
    });

    // Adds 'sample.dat' to the persistence list
    function addToList() {
        if (SdkSample.sampleFile !== null) {
            var outputDiv = document.getElementById("output");
            var MRUradio = document.getElementById("MRUradio");
            var FALradio = document.getElementById("FALradio");
            if (MRUradio.checked) {
                SdkSample.mruToken = Windows.Storage.AccessCache.StorageApplicationPermissions.mostRecentlyUsedList.add(SdkSample.sampleFile, SdkSample.sampleFile.name);
                outputDiv.innerHTML = "The file '" + SdkSample.sampleFile.name + "' was added to the MRU list and a token was stored.";
            } else if (FALradio.checked) {
                SdkSample.falToken = Windows.Storage.AccessCache.StorageApplicationPermissions.futureAccessList.add(SdkSample.sampleFile, SdkSample.sampleFile.name);
                outputDiv.innerHTML = "The file '" + SdkSample.sampleFile.name + "' was added to the FAL list and a token was stored.";
            }
        }
    }

    // Shows the contents of the persistence list
    function showList() {
        if (SdkSample.sampleFile !== null) {
            var outputDiv = document.getElementById("output");
            var MRUradio = document.getElementById("MRUradio");
            var FALradio = document.getElementById("FALradio");
            if (MRUradio.checked) {
                var mruEntries = Windows.Storage.AccessCache.StorageApplicationPermissions.mostRecentlyUsedList.entries;
                if (mruEntries.size > 0) {
                    var mruOutputText = "The MRU list contains the following item(s):<br /><br />";
                    mruEntries.forEach(function (entry) {
                        mruOutputText += entry.metadata + "<br />"; // Application previously chose to store sampleFile.name in this field
                    });

                    outputDiv.innerHTML = mruOutputText;
                } else {
                    outputDiv.innerHTML = "The MRU list is empty, please select 'Most Recently Used' and click 'Add to List' to add a file to the MRU list.";
                }
            } else if (FALradio.checked) {
                var falEntries = Windows.Storage.AccessCache.StorageApplicationPermissions.futureAccessList.entries;
                if (falEntries.size > 0) {
                    var falOutputText = "The FAL list contains the following item(s):<br /><br />";
                    falEntries.forEach(function (entry) {
                        falOutputText += entry.metadata + "<br />"; // Application previously chose to store sampleFile.name in this field
                    });

                    outputDiv.innerHTML = falOutputText;
                } else {
                    outputDiv.innerHTML = "The FAL list is empty, please select 'Future Access List' and click 'Add to List' to add a file to the FAL list.";
                }
            }
        }
    }

    // Opens and reads 'sample.dat' from the persistence list
    function openFromList() {
        if (SdkSample.sampleFile !== null) {
            var outputDiv = document.getElementById("output");
            var MRUradio = document.getElementById("MRUradio");
            var FALradio = document.getElementById("FALradio");
            if (MRUradio.checked) {
                if (SdkSample.mruToken !== null) {
                    // Open the 'sample.dat' via the token that was stored when adding this file into the MRU list
                    Windows.Storage.AccessCache.StorageApplicationPermissions.mostRecentlyUsedList.getFileAsync(SdkSample.mruToken).then(function (file) {
                        // Read the file
                        Windows.Storage.FileIO.readTextAsync(file).done(function (fileContent) {
                            outputDiv.innerHTML = "The file '" + file.name + "' was opened by a stored token from the MRU list, it contains the following text:<br /><br />" + fileContent;
                        },
                        function (error) {
                            WinJS.log && WinJS.log(error, "sample", "error");
                        });
                    },
                    function (error) {
                        WinJS.log && WinJS.log(error, "sample", "error");
                    });
                } else {
                    outputDiv.innerHTML = "The MRU list is empty, please select 'Most Recently Used' list and click 'Add to List' to add a file to the MRU list.";
                }
            } else if (FALradio.checked) {
                if (SdkSample.falToken !== null) {
                    // Open the 'sample.dat' via the token that was stored when adding this file into the FAL list
                    Windows.Storage.AccessCache.StorageApplicationPermissions.futureAccessList.getFileAsync(SdkSample.falToken).then(function (file) {
                        // Read the file
                        Windows.Storage.FileIO.readTextAsync(file).done(function (fileContent) {
                            outputDiv.innerHTML = "The file '" + file.name + "' was opened by a stored token from the FAL list, it contains the following text:<br /><br />" + fileContent;
                        },
                        function (error) {
                            WinJS.log && WinJS.log(error, "sample", "error");
                        });
                    },
                    function (error) {
                        WinJS.log && WinJS.log(error, "sample", "error");
                    });
                } else {
                    outputDiv.innerHTML = "The FAL list is empty, please select 'Future Access List' list and click 'Add to List' to add a file to the FAL list.";
                }
            }
        }
    }
})();
