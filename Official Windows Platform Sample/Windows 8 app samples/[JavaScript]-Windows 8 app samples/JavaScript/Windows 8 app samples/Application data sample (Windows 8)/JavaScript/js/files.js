//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/files.html", {
        ready: function (element, options) {
            document.getElementById("filesIncrement").addEventListener("click", filesIncrement, false);
            filesDisplayOutput();
        }
    });

    var roamingFolder = Windows.Storage.ApplicationData.current.roamingFolder;
    var filename = "sampleFile.txt";
    var counter = 0;

    // Guidance for Local, Roaming, and Temporary files.
    //
    // Files are ideal for storing large data-sets, databases, or data that is
    // in a common file-format.
    //
    // Files can exist in either the Local, Roaming, or Temporary folders.
    //
    // Roaming files will be synchronized across machines on which the user has
    // singed in with a connected account.  Roaming of files is not instant; the
    // system weighs several factors when determining when to send the data.  Usage
    // of roaming data should be kept below the quota (available via the 
    // RoamingStorageQuota property), or else roaming of data will be suspended.
    // Files cannot be roamed while an application is writing to them, so be sure
    // to close your application's file objects when they are no longer needed.
    //
    // Local files are not synchronized and remain on the machine on which they
    // were originally written.
    //
    // Temporary files are subject to deletion when not in use.  The system 
    // considers factors such as available disk capacity and the age of a file when
    // determining when or whether to delete a temporary file.

    // This sample illustrates reading and writing a file in the Roaming folder, though a
    // Local or Temporary file could be used just as easily.

    function filesIncrement() {
        counter = counter + 1;

        roamingFolder.createFileAsync(filename, Windows.Storage.CreationCollisionOption.replaceExisting)
            .then(function (file) {
                return Windows.Storage.FileIO.writeTextAsync(file, counter);
            }).done(function () {
                filesDisplayOutput();
            });
    }

    function filesReadCounter() {
        roamingFolder.getFileAsync(filename)
            .then(function (file) {
                return Windows.Storage.FileIO.readTextAsync(file);
            }).done(function (text) {
                counter = parseInt(text);
                document.getElementById("filesOutput").innerText = "Counter: " + counter;
            }, function () {
                // getFileAsync or readTextAsync failed.
                document.getElementById("filesOutput").innerText = "Counter: <not found>";
            });
    }

    function filesDisplayOutput() {
        filesReadCounter();
    }
})();
