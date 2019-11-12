//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var networkInfo = Windows.Networking.Connectivity.NetworkInformation;
    var page = WinJS.UI.Pages.define("/html/profile-localusagedata.html", {
        ready: function (element, options) {
            document.getElementById("scenario2").addEventListener("click", displayLocalDataUsage, false);
        }
    });

    //
    //Get Internet Connection Profile and display local data usage for the profile for the past 1 hour
    //
    function displayLocalDataUsage() {
        try {
            var currTime = new Date();

            //Set start Time to 1 hour (3600000ms) before current time
            var startTime = new Date(currTime - 3600000);

            // get the ConnectionProfile that is currently used to connect to the Internet
            var connectionProfile = networkInfo.getInternetConnectionProfile();
            if (connectionProfile === null) {
                WinJS.log && WinJS.log("Not connected to Internet\n\r", "sample", "status");
            }
            else {
                var LocalUsage = connectionProfile.getLocalUsage(startTime, currTime);
                var lclString = "Local Data Usage: \n\r";
                lclString += "Bytes Sent: " + LocalUsage.bytesSent + "\n\r";
                lclString += "Bytes Received: " + LocalUsage.bytesReceived + "\n\r";
                WinJS.log && WinJS.log(lclString, "sample", "status");
            }
        }

        catch (e) {
            WinJS.log && WinJS.log("An unexpected exception occured: " + e.name + ": " + e.message, "sample", "error");
        }
    }
})();
