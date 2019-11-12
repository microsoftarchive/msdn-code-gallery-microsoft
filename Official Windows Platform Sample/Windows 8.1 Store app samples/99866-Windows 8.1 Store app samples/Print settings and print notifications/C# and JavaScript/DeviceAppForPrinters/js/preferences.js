//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var printHelper;
    var printerExtensionContext;
    var configuration;

    var page = WinJS.UI.Pages.define("/html/preferences.html", {

        // During an initial activation this event is called before the system splash screen is torn down.
        processed: function (element, args) {

            configuration = args;

            if (!configuration || !configuration.printerExtensionContext) {
                WinJS.log && WinJS.log("Configuration argument is invalid", "sample", "error");
                return;
            }

            // The printerExtensionContext argument contains information about the currently selected printer.
            // This object's fields and methods can't be accessed from Javascript, so the sample contains a supporting
            // WinRT library written in C# that is able to interact with this object
            printerExtensionContext = configuration.printerExtensionContext;

            // PrinterHelperClass is the sample WinRT C# library that allows us to examine the received printerExtensionContext,
            // retrieve the current print ticket, call into other .NET libraries to perform print-specific functionality, etc.
            printHelper = new Microsoft.Samples.Printers.Extensions.PrintHelperClass(printerExtensionContext);

            displaySettings(false);
        },

        // During an initial activation this event is called after the system splash screen is torn down.
        // Do any initialization work that is not related to getting the initial UI set up.
        ready: function (element, options) {

            if (!configuration) {
                WinJS.log && WinJS.log("Configuration argument is null", "sample", "error");
                return;
            }

            // Add an event listener for saverequested (the back button of the flyout is pressed).
            configuration.addEventListener("saverequested", onSaveRequested, false);
        }
    });

    function displaySettings(refreshed) {
        configuration.addEventListener("saverequested", onSaveRequested, false);

        var doc = document.getElementById("printOptions");
        if (doc.hasChildNodes()) {
            doc.removeChild(doc.firstChild);
        }

        var outputDiv = document.createElement("div");

        // Generate drop-down select controls for some common printing features.
        // The features in this sample were chosen because they're available on a wide range
        // of printer drivers.
        var features = ["PageOrientation", "PageOutputColor", "PageMediaSize", "PageMediaType"];

        for (var i = 0; i < features.length; i++) {
            var feature = features[i];

            // Check whether the currently selected printer's capabilities include this feature.
            if (!printHelper.featureExists(feature)) {
                continue;
            }

            // Get the selected option for this feature in the current context's print ticket.
            var selectedOption = printHelper.getSelectedOptionIndex(feature);

            var indexList = printHelper.getOptionInfo(feature, "Index");
            var displayNameList = printHelper.getOptionInfo(feature, "DisplayName");
            var constrainedList = new Array(indexList.length);
            if (refreshed) {
                constrainedList = printHelper.getOptionConstraints(feature);
            }

            // Generate a select control for the current feature.
            var select = generateSelect(feature, indexList, displayNameList, selectedOption, constrainedList);

            // Get the display name for the current feature.
            var featureName = printHelper.getFeatureDisplayName(feature);

            // Create a container div and a header div for the select control.
            var selectDiv = document.createElement("div");
            var selectHeaderDiv = document.createElement("div");
            selectHeaderDiv.textContent = featureName;

            selectDiv.appendChild(selectHeaderDiv);
            selectDiv.appendChild(select);

            outputDiv.appendChild(selectDiv);
        }
        try {
            document.getElementById("printOptions").appendChild(outputDiv);
        } catch (exception) {
            WinJS.log && WinJS.log(exception.message, "sample", "error");
        }

        document.getElementById("printOptions").style.display = "";
        document.getElementById("waitPanel").style.display = "none";
    }

    function onSaveRequested(eventArguments, sender) {

        if (!printHelper || !printerExtensionContext || !eventArguments) {
            WinJS.log && WinJS.log("onSaveRequested: eventArguments, printHelper, and context cannot be null", "sample", "error");
            return;
        }

        // Get the request object, which has the save method that allows saving updated print settings.
        var request = eventArguments.request;

        if (!request) {
            WinJS.log && WinJS.log("onSaveRequested: request cannot be null", "sample", "error");
            return;
        }

        var deferral = request.getDeferral();

        document.getElementById("printOptions").style.display = "none";
        document.getElementById("waitPanel").style.display = "";

        // Go through all the feature select elements, look up the selected
        // option name, and update the context
        // for each feature
        var selects = document.getElementById("printOptions").getElementsByTagName("select");
        for (var i = 0; i < selects.length; i++) {
            if (selects[i].id === "scenarios" || selects[i].id === "printerSelect") {
                // Skip the scenario selector that's part of the standard sample template
                // Also skip the hidden printer selector from launch from tile scenario
                continue;
            }

            var index = selects[i].selectedIndex;
            var optionIndex = selects[i].options[index].value;
            var feature = selects[i].id;

            // Set the feature's selected option in the context's print ticket.
            // The printerExtensionContext object is updated with each iteration of this loop
            printHelper.setFeatureOption(feature, optionIndex);
        }

        // This save request will throw an exception if ticket validation fails.
        // When the exception is thrown, the app flyout will remain.
        // If you want the flyout to remain regardless of outcome, you can call
        // request.cancel(). This should be used sparingly, however, as it could
        // disrupt the entire the print flow and will force the user to 
        // light dismiss to restart the entire experience.
        printHelper.saveTicketAsync(request, printerExtensionContext).done(function () {
            if (configuration) {
                configuration.removeEventListener("saverequested", onSaveRequested);
            }
            deferral.complete();
        }, function (e) {
            WinJS.log && WinJS.log("Failed to save the ticket", "sample", "error");
            displaySettings(true);
            deferral.complete();
        });
    }
    
    function generateSelect(id, optionIds, optionNames, selectedOption, isConstrained) {
        // Generate a drop-down select control for a feature using the specified options.
        var select = document.createElement("select");
        select.id = id;
        select.style.width = "200px";

        for (var i = 0; i < optionIds.length; i++) {
            // Create the option
            var option = document.createElement("option");
            option.text = optionNames[i];
            option.value = optionIds[i];
            option.style.color = isConstrained[i] ? "Red" : "Black";

            select.appendChild(option);
            if (selectedOption === option.value) {
                // Select the currently selected option
                option.selected = true;
                select.style.color = option.style.color;
            }
        }
        return select;
    }
})();
