//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// Requires workspaceActiveX.js

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/rdpActivation.html", {
        ready: function (element, options) {
            var applicationData = Windows.Storage.ApplicationData.current;
            var localSettings = applicationData.localSettings;
            var activationArgsWrapper = localSettings.values["activationArguments"];
            
            if (!activationArgsWrapper) {
                WinJS.log && WinJS.log("It looks like you did not arrive here by launching a workspace resource, so there is nothing to display.", "sample", "status");
            } else {
                var rdpFileContents = "";
                var activationContext = "";

                try {
                    var xmlParseSettings = new Windows.Data.Xml.Dom.XmlLoadSettings();
                    xmlParseSettings.maxElementDepth = 5;
                    xmlParseSettings.prohibitDtd = true;
                    xmlParseSettings.resolveExternals = false;
                    xmlParseSettings.validateOnParse = true;

                    var parsedArgumentsXml = new Windows.Data.Xml.Dom.XmlDocument();
                    parsedArgumentsXml.loadXml(activationArgsWrapper["val"], xmlParseSettings);

                    rdpFileContents = parsedArgumentsXml.selectSingleNode("/RdpClientParameters/RdpFileContents/text()").data;
                    activationContext = parsedArgumentsXml.selectSingleNode("/RdpClientParameters/Context/text()").data;
                } catch (e) {
                    WinJS.log && WinJS.log("Error parsing the activation arguments: " + e.number + " " + e.message, "sample", "error");
                    return;
                }

                var contextHeader = document.createElement("h3");
                contextHeader.textContent = "Activation Context";

                var contextTag = document.createElement("p");
                contextTag.textContent = activationContext;

                var contextRegion = document.getElementById("contextContainer");
                contextRegion.appendChild(contextHeader);
                contextRegion.appendChild(contextTag);

                var rdpContentsHeader = document.createElement("h3");
                rdpContentsHeader.textContent = "RDP File Contents";

                var rdpContentsTag = document.createElement("p");
                rdpContentsTag.textContent = rdpFileContents;

                var rdpContentsRegion = document.getElementById("rdpFileContentsContainer");
                rdpContentsRegion.appendChild(rdpContentsHeader);
                rdpContentsRegion.appendChild(rdpContentsTag);

                WinJS.log && WinJS.log("Successfully parsed the RDP activation arguments.", "sample", "status");
                localSettings.values.remove("activationArguments");
            }
        }
    });

})();
