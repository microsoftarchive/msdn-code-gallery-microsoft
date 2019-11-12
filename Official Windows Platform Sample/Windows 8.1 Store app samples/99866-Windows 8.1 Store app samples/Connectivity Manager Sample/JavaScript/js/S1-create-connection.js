//// Copyright (c) Microsoft Corporation. All rights reserved
var connectionSession = null;

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/S1-create-connection.html", {
        ready: function (element, options) {
            document.getElementById("ConnectButton").addEventListener("click", connectButtonClick, false);
            ConnectivityNS.NetworkInformation.addEventListener("networkstatuschanged", onNetworkStatusChanged);

            if (connectionSession !== null) {
                // Ensure that we can still disconnect, even if we switch scenarios
                OutputText.textContent = "Connected to " + connectionSession.connectionProfile.profileName + "\n" + "Connectivity Level: " +
                    networkConnectivityLevelToString(connectionSession.connectionProfile.getNetworkConnectivityLevel());

                ConnectButton.textContent = "Disconnect";
            }
        },
        unload: function (element, options) {
            ConnectivityNS.NetworkInformation.removeEventListener("networkstatuschanged", onNetworkStatusChanged);
        }
    });
    var ConnectivityNS = Windows.Networking.Connectivity;
    var connectionResult = null;

    function onNetworkStatusChanged() {
        if (connectionSession !== null) {
            OutputText.textContent = "Connected to " + connectionSession.connectionProfile.profileName + "\n" + "Connectivity Level: " +
                networkConnectivityLevelToString(connectionSession.connectionProfile.getNetworkConnectivityLevel());
        }
    }

    function connectButtonClick() {
        switch (ConnectButton.textContent) {
            case "Connect":
                connect();
                break;
            case "Cancel":
                cancel();
                break;
            case "Disconnect":
                disconnect();
                break;
        }
    }

    function connectionCompletedHandler(asyncInfo) {
        connectionSession = asyncInfo;

        OutputText.textContent = "Connected to " + connectionSession.connectionProfile.profileName + "\n" + "Connectivity Level: " +
            networkConnectivityLevelToString(connectionSession.connectionProfile.getNetworkConnectivityLevel());

        // Transition button to Disconnect state
        ConnectButton.textContent = "Disconnect";

        connectionResult.operation.close();
        connectionResult = null;
    }

    function connectionErrorHandler(error) {
        OutputText.textContent = error.message;

        ConnectButton.textContent = "Connect";

        connectionResult = null;
    }
    
    function parseCellularApnAuthenticationType(input) {
        switch (input) {
            case "Chap":
                return ConnectivityNS.CellularApnAuthenticationType.chap;
            case "Mschapv2":
                return ConnectivityNS.CellularApnAuthenticationType.mschapv2;
            case "Pap":
                return ConnectivityNS.CellularApnAuthenticationType.pap;
            default:
                return ConnectivityNS.CellularApnAuthenticationType.none;
        }
    }

    function parseCompressionEnabled(input) {
        if (input === "Yes") {
            return true;
        }

        return false;
    }

    function networkConnectivityLevelToString(input) {
        switch (input) {
            case ConnectivityNS.NetworkConnectivityLevel.localAccess:
                return "Local Access";
            case ConnectivityNS.NetworkConnectivityLevel.constrainedInternetAccess:
                return "Constrained Internet Access";
            case ConnectivityNS.NetworkConnectivityLevel.internetAccess:
                return "Internet Access";
            default:
                return "None";
        }
    }

    function connect() {
        OutputText.textContent = "Attempting to connect!";

        // Fill in the CellularApnContext
        var cellularApnContext = new ConnectivityNS.CellularApnContext();
        cellularApnContext.accessPointName = AccessPointName.value;
        cellularApnContext.providerId = ProviderId.value;
        cellularApnContext.userName = UserName.value;
        cellularApnContext.password = Password.value;
        cellularApnContext.authenticationType = parseCellularApnAuthenticationType(Authentication.children[Authentication.selectedIndex].textContent);
        cellularApnContext.isCompressionEnabled = parseCompressionEnabled(Compression.children[Compression.selectedIndex].textContent);

        // Call AcquireConnectionAsync with the CellularApnContext, and set the handler
        connectionResult = ConnectivityNS.ConnectivityManager.acquireConnectionAsync(cellularApnContext);
        connectionResult.then(connectionCompletedHandler, connectionErrorHandler);

        // Transition button to Cancel state
        ConnectButton.textContent = "Cancel";
    }

    function cancel() {
        OutputText.textContent = "Connection canceled";

        if (connectionResult !== null) {
            connectionResult.operation.cancel();
            connectionResult = null;
        }

        // Transition button to Connect state
        ConnectButton.textContent = "Connect";
    }

    function disconnect() {
        OutputText.textContent = "Disconnected";

        // Disconnect
        if (connectionSession !== null) {
            connectionSession.close();
            connectionSession = null;
        }

        // Transition button to Disconnect state
        ConnectButton.textContent = "Connect";
    }
})();
