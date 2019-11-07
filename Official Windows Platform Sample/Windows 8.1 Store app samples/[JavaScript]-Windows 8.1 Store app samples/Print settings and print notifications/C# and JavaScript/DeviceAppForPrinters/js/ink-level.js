//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/ink-level.html", {

        // During an initial activation this event is called after the system splash screen is torn down.
        // Do any initialization work that is not related to getting the initial UI set up.
        ready: function (element, options) {
            document.getElementById("enumPrinters").addEventListener("click", onEnumeratePrinters, false);

            if (options && options.arguments) {
                WinJS.log && WinJS.log("Toast Context (eventArguments.arguments): " + options.arguments, "sample", "status");
            }

            var toastDiv = document.getElementById("toastOutput");

            // Get printer name
            toastDiv.appendChild(displayPrinterNameFromBackgroundTaskTriggerDetails());
            // Get asyncUI xml
            toastDiv.appendChild(displayAsyncUIxmlFromBackgroundTaskTriggerDetails());

            // Clearing the live tile status
            Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().clear();
            Windows.UI.Notifications.BadgeUpdateManager.createBadgeUpdaterForApplication().clear();

        },
        unload: function (element, options) {
            // When this page is unloaded, unsubscribe from the oninklevelreceived event.
            if (printHelper !== null) {
                printHelper.removeEventListener("oninklevelreceived", onInkLevelReceived);
            }
        }
    });

    var associatedPrinterList;

    var keyPrinterName = "BA5857FA-DE2C-4A4A-BEF2-49D8B4130A39";
    var keyAsyncUIXML = "55DCA47A-BEE9-43EB-A7C8-92ECA2FA0685";
    var settings = Windows.Storage.ApplicationData.current.localSettings;

    function displayPrinterNameFromBackgroundTaskTriggerDetails() {

        var outputDiv = document.createElement("div");
        var printerName = settings.values[keyPrinterName];
        if (printerName) {
            outputDiv.textContent = ("Printer name from background task triggerDetails: " + printerName);
        }
        return outputDiv;
    }

    function displayAsyncUIxmlFromBackgroundTaskTriggerDetails() {

        var outputDiv = document.createElement("div");
        var asyncUIXML = settings.values[keyAsyncUIXML];
        if (asyncUIXML) {
            outputDiv.textContent = ("AsyncUI xml from background task triggerDetails: " + asyncUIXML);
        }
        return outputDiv;
    }

    function onEnumeratePrinters() {
        /// <summary>
        ///     Sets up the UI for what happens when the user clicks on "Enumerate Associated Printers" button.
        /// </summary>

        // UI manipulation to display the printers in a selection box for users to select.
        document.getElementById("bidiOutput").innerText = "";
        var printerSelect = document.getElementById("printerSelect");
        var getInkButton = document.getElementById("queryInk");

        // Displaying the selection box and button to query for ink
        printerSelect.length = 0;
        associatedPrinterList = new Array();

        // Actual enumeration of the printers
        enumeratePrinters();

        // Adding a listener for when the user selected a printer and queries for ink status
        getInkButton.addEventListener("click", onGetInk, false);
    }

    function enumeratePrinters() {
        /// <summary>
        ///     Enumerate printers associated with the app through the following steps.
        ///
        ///     1. Searching through all devices interfaces for printers.
        ///     2. Getting the container for each printer device.
        ///     3. Checking for association by comparing each container's PackageFamilyName property
        /// </summary>

        // This is the GUID for printers
        var printerInterfaceClass = "{0ecef634-6ef0-472a-8085-5ad023ecbccd}";
        var selector = "System.Devices.InterfaceClassGuid:=\"" + printerInterfaceClass + "\"";

        // By default, findAllAsync does not return the containerId for the device it queries.
        // We have to add it as an additonal property to retrieve.
        var containerIdField = "System.Devices.ContainerId";
        var propertiesToRetrieve = [containerIdField];

        // Asynchronously find all printer devices.
        Windows.Devices.Enumeration.DeviceInformation.findAllAsync(selector, propertiesToRetrieve)
            .done(function (devInfoCollection) {

                // For each printer device returned, check if it is associated.
                devInfoCollection.forEach(function (deviceInfo) {
                    findAssociatedPrinters(deviceInfo.name, deviceInfo.id, deviceInfo.properties[containerIdField]);
                });

            }, function (e) {
                // Display an error message if any errors is encountered by findAllAsync().
                WinJS.log && WinJS.log("Error in obtaining device information: " + e.message, "sample", "error");
            });
    }


    function findAssociatedPrinters(displayName, interfaceId, containerId) {
        /// <summary>
        ///     Check if a printer is associated with the current application, if it is, add its interfaceId to a list of associated interfaceIds
        ///
        ///     For each different app, it will have a different correctPackageFamilyName.
        ///     Look in the Visual Studio packagemanifest editor to see what it is for your app.
        /// </summary>
        /// <param name="displayName" type="String">
        ///     The friendly display name of the printer.
        /// </param>
        /// <param name="interfaceId" type="String">
        ///     The interfaceId of this printer.
        /// </param>
        /// <param name="containerId" type="String">
        ///     The containerId of this printer.
        /// </param>

        // Specifically telling createFromIdAsync to retrieve the AppPackageFamilyName.
        var packageFamilyName = "System.Devices.AppPackageFamilyName";
        var containerPropertiesToGet = new Array();
        containerPropertiesToGet.push(packageFamilyName);

        // createFromIdAsync needs braces on the containerId string.
        var containerIdwithBraces = "{" + containerId + "}";

        // Asynchoronously getting the container information of the printer.
        Windows.Devices.Enumeration.Pnp.PnpObject.createFromIdAsync(Windows.Devices.Enumeration.Pnp.PnpObjectType.deviceContainer, containerIdwithBraces, containerPropertiesToGet)
            .done(function (containerInfo) {

                // A printer could be associated with other device apps, only the ones with package family name
                // matching this app's is associated with this app. The correct string is different for each app
                // and can be found in the Visual Studio packagemanifest editor
                var appPackageFamilyName = "Microsoft.SDKSamples.DeviceAppForPrinters.JS_8wekyb3d8bbwe";
                var prop = containerInfo.properties;

                // If the packageFamilyName of the printer container matches the one for this app, the printer is associated with this app.
                var packageFamilyNameList = prop[packageFamilyName];
                if (packageFamilyNameList !== null) {
                    for (var j = 0; j < packageFamilyNameList.length; j++) {
                        if (packageFamilyNameList[j] === appPackageFamilyName) {
                            associatedPrinterList.push(interfaceId);
                            addToSelect(displayName);
                        }
                    }
                }
            }, function (e) {
                WinJS.log && WinJS.log("Error in obtaining container information: " + e.message, "sample", "error");
            });
    }

    function addToSelect(displayName) {
        /// <summary>
        ///     Adds the friendly printer name to the selection box to allow the user to select it.
        /// </summary>
        /// <param name="displayName" type="String">
        ///     The friendly name of the printer.
        /// </param>

        // Adds the string to the select box.
        var printerSelect = document.getElementById("printerSelect");
        var printerOption = document.createElement("option");
        printerOption.text = displayName;
        printerOption.value = printerSelect.length;
        printerSelect.appendChild(printerOption);

        // Display a message to the user that printer(s) have been found.
        WinJS.log && WinJS.log("Printer(s) found", "sample", "status");
    }

    function onGetInk() {
        /// <summary>
        ///     Sends a bidi request that retrieves ink level.
        /// </summary>

        var inkDiv = document.getElementById("bidiOutput");
        inkDiv.innerHTML = "";

        // Get the user selected printer's interfaceId.
        var selectedIndex = document.getElementById("printerSelect").selectedIndex;
        if (selectedIndex < associatedPrinterList.length) {
            var output = document.getElementById("scenario1Output");
            var interfaceId = associatedPrinterList[selectedIndex];
            try {
                // Remove any existing event handlers for oninklevelreceived, if any
                if (printHelper !== null) {
                    printHelper.removeEventListener("oninklevelreceived", onInkLevelReceived);
                    printHelper = null;
                }

                // Use the PrintHelperClass to retrieve the data.
                var printerExtensionContext = Windows.Devices.Printers.Extensions.PrintExtensionContext.fromDeviceId(interfaceId);
                printHelper = new Microsoft.Samples.Printers.Extensions.PrintHelperClass(printerExtensionContext);

                // Add the event handler for the oninklevelreceived event and send the Bidi query
                printHelper.addEventListener("oninklevelreceived", onInkLevelReceived);
                printHelper.sendInkLevelQuery();
            } catch (exception) {
                WinJS.log && WinJS.log("Error retrieving PrinterExtensionContext from InterfaceId", "sample", "error");
                inkDiv.textContent = exception.message;
            }

        } else {
            WinJS.log && WinJS.log("Error getting ink level: selected printer out of bounds", "sample", "error");
        }
    }

    function onInkLevelReceived(e) {
        /// <summary>
        ///     This event handler is called when ink level data is available.
        /// </summary>

        var inkDiv = document.getElementById("bidiOutput");
        inkDiv.innerHTML = "";

        WinJS.log && WinJS.log("Ink level query successful", "sample", "status");
        inkDiv.textContent = e;
    }

    // Represents the current instance of the PrintHelper class.
    var printHelper = null;
})();
