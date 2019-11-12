//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/update-cost.html", {
        ready: function (element, options) {
            document.getElementById("scenario2Open").addEventListener("click", updateCost, false);
        }
    });

    function updateCost() {
        var mediaType = document.scenario2InputForm.mediaType.value === "0" ?
                            Windows.Networking.NetworkOperators.ProfileMediaType.wlan :
                            Windows.Networking.NetworkOperators.ProfileMediaType.wwan;
        var profileName = document.scenario2InputForm.profileName.value;

        var networkCostCategory = Windows.Networking.Connectivity.NetworkCostType.unknown;

        if (profileName === "") {
            WinJS.log && WinJS.log("Profile name cannot be empty", "sample", "error");
            return;
        }

        var networkCostValue = parseInt(document.scenario2InputForm.networkCostCategory.value);
        switch (networkCostValue) {
            case 0:
                networkCostCategory = Windows.Networking.Connectivity.NetworkCostType.unknown;
                break;
            case 1:
                networkCostCategory = Windows.Networking.Connectivity.NetworkCostType.unrestricted;
                break;
            case 2:
                networkCostCategory = Windows.Networking.Connectivity.NetworkCostType.fixed;
                break;
            case 3:
                networkCostCategory = Windows.Networking.Connectivity.NetworkCostType.variable;
                break;
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

            // Set the new cost
            provisionedProfile.updateCost(networkCostCategory);

            // Cost has been updated successfully
            WinJS.log && WinJS.log("Profile " + profileName + " has been updated with the cost type " + networkCostCategory, "sample", "status");
        }
        catch (e) {
            WinJS.log && WinJS.log("An unexpected exception occured: " + e.name + ": " + e.message, "sample", "error");
        }
    }
})();
