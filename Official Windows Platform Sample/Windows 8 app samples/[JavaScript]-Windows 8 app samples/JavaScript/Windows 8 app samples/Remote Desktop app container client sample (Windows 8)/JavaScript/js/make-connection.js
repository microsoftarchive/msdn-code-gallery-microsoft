//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var myClientControlObject = null;
    var connectButton = null;
    var propertiesButton = null;
    var serverName = null;
    var desktopWidth = 1024;
    var desktopHeight = 768;

    var page = WinJS.UI.Pages.define("/html/make-connection.html", {
        ready: function (element, options) {
            connectButton = document.getElementById("connectButton");
            connectButton.addEventListener("click", handleConnectButton, false);

            propertiesButton = document.getElementById("propertiesButton");
            propertiesButton.addEventListener("click", handlePropertiesButton, false);
        }
    });

    function handleConnectButton () {
        WinJS.log && WinJS.log("", "sample", "error");

        serverName = document.getElementById("serverName").value;
        if (!serverName) {
            WinJS.log && WinJS.log("Enter PC name to connect to.", "sample", "error");
            return;
        }

        // Toggle the connect button behavior.
        var connect = (connectButton.innerText === "Connect");
        connectButton.innerText = connect ? "Disconnect" : "Connect";

        if (connect) {
            // Create the client control, set connection properties and connect.
            myClientControlObject = Microsoft.Sample.RDPClient.createClientControl(desktopWidth, desktopHeight);

            // Set connection properties.
            setConnectionProperties();
            Microsoft.Sample.RDPClient.connectToRemotePC(myClientControlObject);
        } else { 
            // Disconnect and release the client control.
            Microsoft.Sample.RDPClient.disconnectFromRemotePC(myClientControlObject);
            Microsoft.Sample.RDPClient.removeClientControl(myClientControlObject);
            myClientControlObject = null;
        }
    }

    function handlePropertiesButton () {
        WinJS.log && WinJS.log("", "sample", "error");

        // Toggle the property button behavior.
        var clientControlRegion = document.getElementById("connectionPropertiesRegion");
        var showProperties = (clientControlRegion.style.display === "none");
        
        if (showProperties) {
            showConnectionProperties();
        } else {
            hideConnectionProperties();
        }
    }

    function setConnectionProperties () {
        // Set the server to connect to.
        myClientControlObject.Settings.SetRdpProperty("Full Address", serverName);

        // Set the desktop resolution.
        myClientControlObject.Settings.SetRdpProperty("DesktopWidth", desktopWidth);
        myClientControlObject.Settings.SetRdpProperty("DesktopHeight", desktopHeight);

        // Turn ON microphone redirection.
        myClientControlObject.Settings.SetRdpProperty("AudioCaptureMode", true);

        // Specify connection experience based on bandwidth and latency.
        // 6 ==> LAN
        myClientControlObject.Settings.SetRdpProperty("connection type", 6);

        // Turn on printer redirection.
        myClientControlObject.Settings.SetRdpProperty("RedirectPrinters", false);
    }
    
    function getConnectionProperty(propertyName) {
        var propertyValue = propertyName + " = ";

        try {
            propertyValue += myClientControlObject.Settings.GetRdpProperty(propertyName);
        } catch (e) {
            propertyValue += "Error while retrieving this property.";
        }

        return propertyValue + "\n";
    }

    function showConnectionProperties () {
        if (!myClientControlObject) {
            WinJS.log && WinJS.log("RDP client control is not created.", "sample", "error");
            return;
        }

        var displayString = "Properties retrieved individually\n";
        displayString += "---------------------------------\n";

        // Get individual properties.
        displayString += getConnectionProperty("DesktopWidth");
        displayString += getConnectionProperty("DesktopHeight");

        displayString += getConnectionProperty("AudioCaptureMode");
        displayString += getConnectionProperty("AudioMode");
        displayString += getConnectionProperty("RedirectClipboard");

        displayString += getConnectionProperty("Full Address");
        displayString += getConnectionProperty("connection type");
        displayString += getConnectionProperty("Prompt For Credentials");

        // You have not set the following properties explicitly.
        // Retrieving the default value for the property.
        displayString += getConnectionProperty("Administrative Session");
        displayString += getConnectionProperty("Authentication Level");
        displayString += getConnectionProperty("UserName");
        displayString += getConnectionProperty("Domain");
        displayString += getConnectionProperty("Server Port");

        displayString += getConnectionProperty("RedirectPrinters");

        displayString += getConnectionProperty("EnableCredSspSupport");
        
        // Get all properties set by you in RDP File format.
        displayString += "\nAll properties set by you in RDP file format\n";
        displayString += "--------------------------------------------\n";

        try {
            displayString += myClientControlObject.Settings.RetrieveSettings();
        } catch (e) {
            displayString += "Error while retrieving entire settings";
        }

        // Display the properties region.
        var connectionPropertiesRegion = document.getElementById("connectionPropertiesRegion");
        connectionPropertiesRegion.style.display = "inherit";
        connectionPropertiesRegion.innerText = displayString;
    }

    function hideConnectionProperties () {
        var connectionPropertiesRegion = document.getElementById("connectionPropertiesRegion");
        connectionPropertiesRegion.style.display = "none";
    }

})();
