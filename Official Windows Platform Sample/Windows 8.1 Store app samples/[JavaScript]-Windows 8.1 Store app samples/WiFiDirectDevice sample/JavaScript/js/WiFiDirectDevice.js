//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var WiFiDirectDeviceNS = Windows.Devices.WiFiDirect;
    var devInfoCollection;
    var streamSocket;
    var wfdDevice;    

    function socketError(errMessage) {
        WiFiDirectDeviceHelpers.displayError(errMessage);

        WiFiDirectDeviceHelpers.id("getDevicesButton").style.display = "inline";
        WiFiDirectDeviceHelpers.id("sendButton").style.display = "none";
        WiFiDirectDeviceHelpers.id("messageBox").style.display = "none";
        WiFiDirectDeviceHelpers.id("foundDevicesList").style.display = "none";

        // Clear the foundDevicesList
        var devicesList = WiFiDirectDeviceHelpers.id("foundDevicesList");

        WiFiDirectDeviceHelpers.id("foundDevicesList").style.display = "none";
        for (var i = devicesList.options.length; i > 0; i--) {
            devicesList.options[i] = null;
        }
    }

    function getDevices() {

        WiFiDirectDeviceHelpers.displayStatus("Enumerating WiFiDirect devices...");        

        // Empty the foundDevicesList
        var devicesList = WiFiDirectDeviceHelpers.id("foundDevicesList");

        for (var i = devicesList.options.length; i >= 0; i--) {
            devicesList.options[i] = null;
        }

        var deviceSelector = WiFiDirectDeviceNS.WiFiDirectDevice.getDeviceSelector();
        Windows.Devices.Enumeration.DeviceInformation.findAllAsync(deviceSelector.toString(), null).done(
            function (devInfoCollectionTemp) {
                if (devInfoCollectionTemp.length === 0) {
                    WiFiDirectDeviceHelpers.displayStatus("No WiFiDirect devices found.");
                }
                else {
                    var optionElement;
                    var textElement;
                    var deviceName;
                    var devInfo;

                    // add newly found devices into the drop down list.
                    for (i = 0; i < devInfoCollectionTemp.length; i++) {
                        devInfo = devInfoCollectionTemp[i];

                        optionElement = document.createElement("option");
                        optionElement.setAttribute("value", i + 1);
                        deviceName = devInfo.name;
                        textElement = document.createTextNode(deviceName);
                        optionElement.appendChild(textElement);
                        devicesList.appendChild(optionElement);
                    }

                    WiFiDirectDeviceHelpers.id("foundDevicesList").style.display = "inline";
                    WiFiDirectDeviceHelpers.id("connectButton").style.display = "inline";
                    WiFiDirectDeviceHelpers.displayStatus("Enumerating WiFiDirect devices completed successfully.");
                    devInfoCollection = devInfoCollectionTemp;
                }
            },
            function (err) {
                WiFiDirectDeviceHelpers.displayError(err);
            }
            );
    }

    function disconnect() {
        WiFiDirectDeviceHelpers.displayStatus("WiFiDirect device disconnected.");

        // Empty the current option list
        var devicesList = WiFiDirectDeviceHelpers.id("foundDevicesList");

        for (var i = devicesList.options.length; i >= 0; i--) {
            devicesList.options[i] = null;
        }

        WiFiDirectDeviceHelpers.id("pcIpAddress").style.display = "none";
        WiFiDirectDeviceHelpers.id("deviceIpAddress").style.display = "none";
        WiFiDirectDeviceHelpers.id("disconnectButton").style.display = "none";
        WiFiDirectDeviceHelpers.id("getDevicesButton").style.display = "inline";
        WiFiDirectDeviceHelpers.id("connectButton").style.display = "none";
        WiFiDirectDeviceHelpers.id("foundDevicesList").style.display = "none";
 
        wfdDevice.close();
    }

    function connect() {
        var chosenDevInfo = null;
        var endpointPair = null;

        var devicesList = WiFiDirectDeviceHelpers.id("foundDevicesList");

        // If nothing is selected, return
        if (devicesList.selectedIndex === -1) {
            WiFiDirectDeviceHelpers.displayStatus("Please select a device");
            return;
        }
        else {
            chosenDevInfo = devInfoCollection[devicesList.selectedIndex];
        }

        WiFiDirectDeviceHelpers.displayStatus("Connecting to " + chosenDevInfo.name + " ...");      


        // Connect to the selected WiFiDirect device 
        WiFiDirectDeviceNS.WiFiDirectDevice.fromIdAsync(chosenDevInfo.id).done(
            function (wfdDeviceTemp) {

                if (wfdDeviceTemp === null) {
                    WiFiDirectDeviceHelpers.displayStatus("Connection to " + chosenDevInfo.name + " failed.");
                    return;
                }                

                wfdDevice = wfdDeviceTemp;

                // Register for Connection status change notification
                wfdDevice.onconnectionstatuschanged = disconnectNotificationHandler;

                // Get the EndpointPair collection
                var endpointPairCollection = wfdDevice.getConnectionEndpointPairs();
                if (endpointPairCollection.length > 0) {
                    endpointPair = endpointPairCollection[0];
                }
                else {
                    WiFiDirectDeviceHelpers.displayStatus("Connection to " + chosenDevInfo.name + " failed.");
                    return;
                }

                WiFiDirectDeviceHelpers.id("pcIpAddress").innerHTML = "PC's IP Address: " + endpointPair.localHostName.toString();
                WiFiDirectDeviceHelpers.id("deviceIpAddress").innerHTML = "Device's IP Address: " + endpointPair.remoteHostName.toString();

                WiFiDirectDeviceHelpers.id("pcIpAddress").style.display = "inline";
                WiFiDirectDeviceHelpers.id("deviceIpAddress").style.display = "inline";
                WiFiDirectDeviceHelpers.id("disconnectButton").style.display = "inline";
                WiFiDirectDeviceHelpers.id("getDevicesButton").style.display = "none";
                WiFiDirectDeviceHelpers.id("connectButton").style.display = "none";
                WiFiDirectDeviceHelpers.id("foundDevicesList").style.display = "none";

                WiFiDirectDeviceHelpers.displayStatus("Connection succeeded");
            },
            function (err) {
                WiFiDirectDeviceHelpers.id("connectButton").style.display = "none";
                WiFiDirectDeviceHelpers.id("foundDevicesList").style.display = "none";
                WiFiDirectDeviceHelpers.displayError("Connect failed with " + err);
            });
    }

    function disconnectNotificationHandler(e) {

        WiFiDirectDeviceHelpers.displayStatus("WiFiDirect device disconnected");

        // Empty the foundDevicesList list
        var devicesList = WiFiDirectDeviceHelpers.id("foundDevicesList");

        for (var i = devicesList.options.length; i >= 0; i--) {
            devicesList.options[i] = null;
        }

        WiFiDirectDeviceHelpers.id("getDevicesButton").style.display = "inline";
        WiFiDirectDeviceHelpers.id("connectButton").style.display = "none";
        WiFiDirectDeviceHelpers.id("foundDevicesList").style.display = "none";
        WiFiDirectDeviceHelpers.id("pcIpAddress").style.display = "none";
        WiFiDirectDeviceHelpers.id("deviceIpAddress").style.display = "none";
        WiFiDirectDeviceHelpers.id("disconnectButton").style.display = "none";
    }

    var page = WinJS.UI.Pages.define("/html/WiFiDirectDevice.html", {
        ready: function (element, options) {
            WiFiDirectDeviceHelpers.id("getDevicesButton").addEventListener("click", getDevices, false);
            WiFiDirectDeviceHelpers.id("connectButton").addEventListener("click", connect, false);
            WiFiDirectDeviceHelpers.id("disconnectButton").addEventListener("click", disconnect, false);
        },

        unload: function () {
        }
    });

})();
