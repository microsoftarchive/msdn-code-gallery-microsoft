//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/update-usage.html", {
        ready: function (element, options) {
            document.getElementById("scenario3Open").addEventListener("click", updateUsage, false);
        }
    });

    function updateUsage() {
        var mediaType = document.scenario3InputForm.mediaType.value === "0" ?
                            Windows.Networking.NetworkOperators.ProfileMediaType.wlan :
                            Windows.Networking.NetworkOperators.ProfileMediaType.wwan;
        var profileName = document.scenario3InputForm.profileName.value;
        var usageInMB = parseInt(document.scenario3InputForm.usageInMegabytes.value);

        if (profileName === "") {
            WinJS.log && WinJS.log("Profile name cannot be empty", "sample", "error");
            return;
        }

        if (isNaN(usageInMB)) {
            WinJS.log && WinJS.log("Usage in megabytes should be a valid number", "sample", "error");
            return;
        }

        try {
            // Get the network account ID.
            var networkAccIds = Windows.Networking.NetworkOperators.MobileBroadbandAccount.availableNetworkAccountIds;

            if (networkAccIds.size === 0) {
                WinJS.log && WinJS.log("No network account ID found", "sample", "error");
                return;
            }

            // For the sake of simplicity, assume we want to use the first account.
            // Refer to the MobileBroadbandAccount API's how to select a specific account ID.
            var accountId = networkAccIds[0];

            // Create provisioning agent for specified network account ID
            var provAgent = Windows.Networking.NetworkOperators.ProvisioningAgent.createFromNetworkAccountId(accountId);

            // Retrieve associated provisioned profile
            var provisionedProfile = provAgent.getProvisionedProfile(mediaType, profileName);

            // Set the new usage
            provisionedProfile.updateUsage({ usageInMegabytes: usageInMB, lastSyncTime: new Date() });

            // Usage has been updated successfully
            WinJS.log && WinJS.log("Usage of " + usageInMB + "MB has been set for the profile " + profileName, "sample", "status");
        }
        catch (e) {
            WinJS.log && WinJS.log("An unexpected exception occured: " + e.name + ": " + e.message, "sample", "error");
        }
    }
})();
