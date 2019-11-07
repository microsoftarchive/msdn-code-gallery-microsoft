//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/S3_GetStatus.html", {
        ready: function (element, options) {
            document.getElementById("GetStatus").addEventListener("click", doGetStatus, false);
            SdkSample.validateFileExistence();
        }
    });

    function doGetStatus() {

        var SampleFileProtectionStatus = null;
        if ((null === SdkSample.sampleFile)
            || (null === SdkSample.targetFile)
            || (null === SdkSample.sampleFolder)
            || (null === SdkSample.targetFolder))
        {
            WinJS.log && WinJS.log("You need to click the Setup button in the 'Protect a file or folder with an Enterprise Identity' scenario.", "sample", "error");
            return;
        }

        var outputDiv = document.getElementById("output");
        try {
            Windows.Security.EnterpriseData.FileRevocationManager.getStatusAsync(SdkSample.sampleFile).done(
                function (rslt) {
                    outputDiv.innerHTML = "The protection status of the file '" + SdkSample.sampleFilename + "' is " + rslt + "<br /><br />";
                },
                function (err) {
                    // If folder doesn't exist
                    WinJS.log && WinJS.log("Get status for RevokerSample.txt file failed " + err, "sample", "error");
                    return;
                });

            Windows.Security.EnterpriseData.FileRevocationManager.getStatusAsync(SdkSample.sampleFolder).done(
                function (rslt) {
                    outputDiv.innerHTML += "The protection status of the folder " + SdkSample.sampleFoldername + " is " + rslt + "<br /><br />";
                },
                function (err) {
                    // If folder doesn't exist
                    WinJS.log && WinJS.log("Get status for RevokerSample folder failed " + err, "sample", "error");
                    return;
                });

            Windows.Security.EnterpriseData.FileRevocationManager.getStatusAsync(SdkSample.targetFile).done(
                function (rslt) {
                    outputDiv.innerHTML += "The protection status of the file " + SdkSample.targetFilename + " is " + rslt + "<br /><br />";
                },
                function (err) {
                    // If folder doesn't exist
                    WinJS.log && WinJS.log("Get status for RevokeTarget.txt file failed " + err, "sample", "error");
                    return;
                });

            Windows.Security.EnterpriseData.FileRevocationManager.getStatusAsync(SdkSample.targetFolder).done(
                function (rslt) {
                    outputDiv.innerHTML += "The protection status of the folder " + SdkSample.targetFoldername + " is " + rslt + "<br /><br />";
                },
                function (err) {
                    // If folder doesn't exist
                    WinJS.log && WinJS.log("Get status for RevokeTarget folder failed " + err, "sample", "error");
                    return;
                });
        }
        catch( Exception )
        {
            WinJS.log && WinJS.log("Get status exception " + Exception, "sample", "error");
        }
    }
})();
