//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var networkInfo = Windows.Networking.Connectivity.NetworkInformation;
    var networkCostInfo = Windows.Networking.Connectivity.NetworkCostType;
    var networkConnectivityInfo = Windows.Networking.Connectivity.NetworkConnectivityLevel;
    var networkAuthenticationInfo = Windows.Networking.Connectivity.NetworkAuthenticationType;
    var networkEncryptionInfo = Windows.Networking.Connectivity.NetworkEncryptionType;

    var page = WinJS.UI.Pages.define("/html/connection-profile-list.html", {
        ready: function (element, options) {
            document.getElementById("scenario3").addEventListener("click", displayConnectionProfileList, false);
        }
    });

    //
    //Get Profile connection cost info
    //
    function getCostType(networkCostType) {
        switch (networkCostType) {
            case networkCostInfo.unrestricted:
                return 'unrestricted';
            case networkCostInfo.fixed:
                return 'fixed';
            case networkCostInfo.variable:
                return 'variable';
            case networkCostInfo.unknown:
                return 'unknown';
            default:
                return 'error';
        }
    }

    //
    //Suggested behaviors based on Profile cost
    //
    //
    // Implementation of simple behavior cost awareness
    // Application implements three behavioral models:
    //     - High Cost network behavior: this handles an exceptional case where the network 
    //          access cost is higher than the plan normal cost
    //                         This behavioral model could include:
    //                              - prompt the user for permission to access the network
    //                               - suspend all network activities
    //     - Costed network behavior: this handles the case where the network access cost is the         
    //         normal plan cost.
    //                         The behavior model should focus on optimizing the network usage:
    //                               - select a lower resolution for a movie
    //                               - delay non critical downloads
    //                               - avoid pre-fetching of information over the network
    //     - Free network behavior: this handles the case where network access is free or         
    //          unknown.
    //                        This could translate for example in streaming a movie in HD.
    //  
    function costBasedSuggestions(connectionCost) {

        var returnString = "";

        // Check cost flags to see if connection status is outside the MNO's network
        if (connectionCost.roaming) {
            //ImplementHighCostBehavior
            returnString = "Connection is roaming outside of MNO's network, using the connection may result in additional charges. Applications may prompt the user for permission before accessing the network, thereby implementing high cost behavior in this scenario";
        }

        // Applications can implement two different behaviors based on connection cost in the following scenarios:
        // 1- the plan is variable, or is fixed and the status is either approaching datalimit 
        //      or over data limit
        // 2- the plan is either unrestricted or fixed 
        else {
            switch (connectionCost.networkCostType) {
                case networkCostInfo.variable:
                    // pay per byte
                    returnString = "Connection cost is variable, and the connection is charged based on usage, so the application can implement the costed network behavior";
                    break;

                case networkCostInfo.fixed:
                    if (connectionCost.approachingDataLimit || connectionCost.overDataLimit) {
                        //Implement High Cost Network Behavior
                        returnString = "Connection has exceeded the usage cap limit or is approaching the datalimit, and the application can implement high cost network behavior in this scenario";
                    }
                    else {
                        // pre-paid plan
                        returnString = "Application can implemement the costed network behavior";
                    }
                    break;

                case networkCostInfo.unrestricted:
                case networkCostInfo.unknown:
                    // no cost or unknown cost
                    //Implement Free network behavior
                    returnString = "Application can implement the free network behavior";
                    break;
            }
        }
        return returnString;
    }

    function getNetworkSecuritySettings(netSecuritySettings) {
        var networkSecurity = "";
        networkSecurity += "Network Security Settings: \n";
        networkSecurity += "====================\n";

        if (netSecuritySettings === null)
        {
            networkSecurity += "Network Security Settings not available\n";
            return networkSecurity;
        }

        //NetworkAuthenticationType
        switch (netSecuritySettings.networkAuthenticationType) {
            case networkAuthenticationInfo.none:
                networkSecurity += "NetworkAuthenticationType: None";
                break;
            case networkAuthenticationInfo.unknown:
                networkSecurity += "NetworkAuthenticationType: Unknown";
                break;
            case networkAuthenticationInfo.open80211:
                networkSecurity += "NetworkAuthenticationType: Open80211";
                break;
            case networkAuthenticationInfo.sharedKey80211:
                networkSecurity += "NetworkAuthenticationType: SharedKey80211";
                break;
            case networkAuthenticationInfo.wpa:
                networkSecurity += "NetworkAuthenticationType: Wpa";
                break;
            case networkAuthenticationInfo.wpaPsk:
                networkSecurity += "NetworkAuthenticationType: WpaPsk";
                break;
            case networkAuthenticationInfo.wpaNone:
                networkSecurity += "NetworkAuthenticationType: WpaNone";
                break;
            case networkAuthenticationInfo.rsna:
                networkSecurity += "NetworkAuthenticationType: Rsna";
                break;
            case networkAuthenticationInfo.rsnaPsk:
                networkSecurity += "NetworkAuthenticationType: RsnaPsk";
                break;
            default:
                networkSecurity += "NetworkAuthenticationType: Error";
                break;
        }
        networkSecurity += "\n\r";

        //NetworkEncryptionType
        switch (netSecuritySettings.networkEncryptionType) {
            case networkEncryptionInfo.none:
                networkSecurity += "NetworkEncryptionType: None";
                break;
            case networkEncryptionInfo.unknown:
                networkSecurity += "NetworkEncryptionType: Unknown";
                break;
            case networkEncryptionInfo.wep:
                networkSecurity += "NetworkEncryptionType: Wep";
                break;
            case networkEncryptionInfo.wep40:
                networkSecurity += "NetworkEncryptionType: Wep40";
                break;
            case networkEncryptionInfo.wep104:
                networkSecurity += "NetworkEncryptionType: Wep104";
                break;
            case networkEncryptionInfo.tkip:
                networkSecurity += "NetworkEncryptionType: Tkip";
                break;
            case networkEncryptionInfo.ccmp:
                networkSecurity += "NetworkEncryptionType: Ccmp";
                break;
            case networkEncryptionInfo.wpaUseGroup:
                networkSecurity += "NetworkEncryptionType: WpaUseGroup";
                break;
            case networkEncryptionInfo.rsnUseGroup:
                networkSecurity += "NetworkEncryptionType: RsnUseGroup";
                break;
            default:
                networkSecurity += "NetworkEncryptionType: Error";
                break;
        }
        networkSecurity += "\n\r";
        return networkSecurity;
    }
    //
    // Get Connection Profile Information
    //
    function getConnectionProfileInfo(connectionProfile) {

        try {
            if (connectionProfile === null) {
                return "Profile not found\n\r";
            }

            var returnString = "ProfileName: " + connectionProfile.profileName + "\n\r";

            switch (connectionProfile.getNetworkConnectivityLevel()) {
                case networkConnectivityInfo.none:
                    returnString += "Connectivity Level: None\n\r";
                    break;
                case networkConnectivityInfo.localAccess:
                    returnString += "Connectivity Level: Local Access\n\r";
                    break;
                case networkConnectivityInfo.constrainedInternetAccess:
                    returnString += "Connectivity Level: Constrained Internet Access\n\r";
                    break;
                case networkConnectivityInfo.internetAccess:
                    returnString += "Connectivity Level: Internet Access\n\r";
                    break;
            }

            //Display Connection cost info
            returnString += "Connection Cost Information:\n\r";
            returnString += "===============\n\r";
            var connectionCost = connectionProfile.getConnectionCost();

            if (connectionCost !== null)
            {
                returnString += "Cost Type: " + getCostType(connectionCost.networkCostType) + "\n\r";
                returnString += "Roaming: " + connectionCost.roaming + "\n\r";
                returnString += "Over Datalimit: " + connectionCost.overDataLimit + "\n\r";
                returnString += "Approaching Datalimit: " + connectionCost.approachingDataLimit + "\n\r";
                returnString += "Cost Based Suggestions: " + costBasedSuggestions(connectionCost) + "\n\r";
            }
            else
            {
                returnString += "Connection Cost not available\n\r";
            }

            //Display Dataplan status info
            returnString += "Dataplan Status Information:\n\r";
            returnString += "===============\n\r";
            var dataPlanStatus = connectionProfile.getDataPlanStatus();
            if (dataPlanStatus !== null)
            {
                if (dataPlanStatus.dataPlanUsage !== null) {
                    returnString += "Usage In Megabytes: " + dataPlanStatus.dataPlanUsage.megabytesUsed + "\n\r";
                    returnString += "Last Sync Time: " + dataPlanStatus.dataPlanUsage.lastSyncTime + "\n\r";
                }
                else {
                    returnString += "Dataplan Usage: " + "Not Defined" + "\n\r";
                }

                if (dataPlanStatus.inboundBitsPerSecond !== null) {
                    returnString += "Inbound Bits Per Second: " + dataPlanStatus.inboundBitsPerSecond + "\n\r";
                }
                else {
                    returnString += "Inbound Bits Per Second: " + "Not Defined" + "\n\r";
                }

                if (dataPlanStatus.outboundBitsPerSecond !== null) {
                    returnString += "Outbound Bits Per Second: " + dataPlanStatus.outboundBitsPerSecond + "\n\r";
                }
                else {
                    returnString += "Outbound Bits Per Second: " + "Not Defined" + "\n\r";
                }

                if (dataPlanStatus.dataLimitInMegabytes !== null) {
                    returnString += "Data Limit In Megabytes: " + dataPlanStatus.dataLimitInMegabytes + "\n\r";
                }
                else {
                    returnString += "Data Limit In Megabytes: " + "Not Defined" + "\n\r";
                }

                if (dataPlanStatus.nextBillingCycle !== null) {
                    returnString += "Next Billing Cycle: " + dataPlanStatus.nextBillingCycle + "\n\r";
                }
                else {
                    returnString += "Next Billing Cycle: " + "Not Defined" + "\n\r";
                }

                if (dataPlanStatus.maxTransferSizeInMegabytes !== null) {
                    returnString += "Maximum Transfer Size in Megabytes: " + dataPlanStatus.maxTransferSizeInMegabytes + "\n\r";
                }
                else {
                    returnString += "Maximum Transfer Size in Megabytes: " + "Not Defined" + "\n\r";
                }
            }
            else
            {
                returnString += "Dataplan Status not available\n\r";
            }


            //Get Network Security Settings information
            returnString += getNetworkSecuritySettings(connectionProfile.networkSecuritySettings);

        }

        catch (e) {
            WinJS.log && WinJS.log("An unexpected exception occured: " + e.name + ": " + e.message, "sample", "error");
        }

        return returnString;
    }

    //
    //Get list of connection profiles on the machine
    //
    function displayConnectionProfileList() {
        var profileList = "";
        try {
            var ConnectionProfiles = networkInfo.getConnectionProfiles();
            if (ConnectionProfiles.length !== 0) {
                for (var i = 0; i < ConnectionProfiles.length; i++) {

                    //Display Connection profile info for each profile
                    profileList += getConnectionProfileInfo(ConnectionProfiles[i]);
                    profileList += "----------------------------------------------------------------\n\r";
                }
                WinJS.log && WinJS.log(profileList, "sample", "status");
            }
            else {
                sdkSample.displayStatus("No connection profile found", "sample", "status");
            }
        }

        catch (e) {
            WinJS.log && WinJS.log("An unexpected exception occured: " + e.name + ": " + e.message, "sample", "error");
        }
    }

})();
