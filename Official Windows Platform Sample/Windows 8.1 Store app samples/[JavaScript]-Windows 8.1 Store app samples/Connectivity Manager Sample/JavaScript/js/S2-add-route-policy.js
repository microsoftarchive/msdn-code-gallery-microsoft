//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/S2-add-route-policy.html", {
        ready: function (element, options) {
            document.getElementById("AddRoutePolicy").addEventListener("click", addRoutePolicyClick, false);
            document.getElementById("RemoveRoutePolicy").addEventListener("click", removeRoutePolicyClick, false);
        }
    });

    var NetworkingNS = Windows.Networking;
    var ConnectivityNS = Windows.Networking.Connectivity;

    function addRoutePolicyClick() {
        if (connectionSession === null) {
            OutputText.textContent = "Please establish a connection using the \"Create Connection\" scenario first.";

            return;
        }

        try {
            var hostName = new NetworkingNS.HostName(HostName.value);
            var domainNameType = parseDomainNameType(DomainNameType.children[DomainNameType.selectedIndex].textContent);
            var routePolicy = new ConnectivityNS.RoutePolicy(connectionSession.connectionProfile, hostName, domainNameType);

            ConnectivityNS.ConnectivityManager.addHttpRoutePolicy(routePolicy);

            OutputText.textContent = "Added Route Policy\nTraffic to " + routePolicy.hostName.toString() +
                " will be routed through " + routePolicy.connectionProfile.profileName;
        } catch (ex) {
            OutputText.textContent = "Failed to add Route Policy with HostName = \"" + HostName.value + "\"\n" +
                    ex.toString();
        }
    }

    function removeRoutePolicyClick() {
        if (connectionSession === null) {
            OutputText.textContent = "Please establish a connection using the \"Create Connection\" scenario first.";
            
            return;
        }

        try {
            var hostName = new NetworkingNS.HostName(HostName.value);
            var domainNameType = parseDomainNameType(DomainNameType.children[DomainNameType.selectedIndex].textContent);
            var routePolicy = new ConnectivityNS.RoutePolicy(connectionSession.connectionProfile, hostName, domainNameType);
            
            ConnectivityNS.ConnectivityManager.removeHttpRoutePolicy(routePolicy);

            OutputText.textContent = "Removed Route Policy\nTraffic to " + routePolicy.hostName.toString() +
                " will no longer be routed through " + routePolicy.connectionProfile.profileName;
        } catch (ex) {
            OutputText.textContent= "Failed to remove Route Policy with HostName = \"" + HostName.value + "\"\n" +
                    ex.toString();
        }
    }

    function parseDomainNameType(input) {
        switch (input) {
            case "Suffix":
                return Windows.Networking.DomainNameType.suffix;
            default:
                return Windows.Networking.DomainNameType.fullyQualified;
        }
    }
})();
