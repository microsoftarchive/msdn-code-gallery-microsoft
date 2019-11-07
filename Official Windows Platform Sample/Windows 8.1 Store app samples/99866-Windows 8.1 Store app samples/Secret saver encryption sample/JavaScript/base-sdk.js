//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.0.6/js/base.js" />
/// <reference path="//Microsoft.WinJS.0.6/js/ui.js" />
/// <reference path="HTMLElement.js" />

var sdkSample = {};

(function () {
    var scenarios = null;
    var placeHolderImageSource = "images/placeholder-sdk.png";
    var lastError = "";
    var lastStatus = "";
    sdkSample.placeHolderImageSource = placeHolderImageSource;
    sdkSample.scenarioSelection = true;

    function selectScenario(id, disableClearStatus) {
        var output = document.querySelector("#scenario" + id + "Output");
        var input = document.querySelector("#scenario" + id + "Input");

        if (!output) {
            displayError("Sample Configuration Error: Unable to find element with id = scenario" + id + "Output");
            return;
        }

        if (!input) {
            displayError("Sample Configuration Error: Unable to find element with id = scenario" + id + "Input");
            return;
        }

        sdkSample.scenarioId = id;
        scenarios.selectedIndex = parseInt(id) - 1;
        resetInputOutput();
        if (!disableClearStatus) {
            displayError("");
        }

        input.className = "item shown";
        output.className = "item shown";
    }
    sdkSample.selectScenario = selectScenario;

    function resetInputOutput() {
        var items = document.querySelectorAll("#input .item, #output .item");
        for (var i = 0, len = items.length; i < len; i++) {
            items[i].className = "item";
        }
    }

    function displayStatus(message) {
        showMessage(message, false);
    }
    sdkSample.displayStatus = displayStatus;

    function displayError(message) {
        showMessage(message, true);
    }
    sdkSample.displayError = displayError;

    function showMessage(message, isError) {
        var statusDiv = /* @type(HTMLElement) */ document.getElementById("statusMessage");
        if (!statusDiv) {
            statusDiv = /* @type(HTMLElement) */ document.getElementById("errorMessage");
        }
        if (statusDiv) {
            statusDiv.innerText = message;
            if (isError) {
                lastError = message;
                statusDiv.style.color = "blue";
            } else {
                lastStatus = message;
                statusDiv.style.color = "green";
            }
        }
    }

    function getLastError() {
        return lastError;
    }
    sdkSample.getLastError = getLastError;
    
    function getLastStatus() {
        return lastStatus;
    }
    sdkSample.getLastStatus = getLastStatus;
    
    function clearLastError() {
        lastError = "";
    }
    sdkSample.clearLastError = clearLastError;
    
    function clearLastStatus() {
        lastStatus = "";
    }
    sdkSample.clearLastStatus = clearLastStatus;

    function applicationStateCheckpoint() {
        // The checkpoint event gives us the chance to save application state.
        // In this case, we are saving the current scenario id.
        if (sdkSample.scenarioSelection) {
            WinJS.Application.sessionState.selectedScenarioId = sdkSample.scenarioId;
        }
    }

    function applicationActivated() {
        // If there is a scenario id saved in the application's session state, let's use it,
        // otherwise, select the first scenario. WinJS makes sure that the session state
        // is loaded only when the application was previously suspended and terminated.
        if (sdkSample.scenarioSelection) {
            var lastScenarioId = WinJS.Application.sessionState.selectedScenarioId || "1";
            selectScenario(lastScenarioId, true);
        }
    }

    function initialize() {
        WinJS.Application.start();
        WinJS.Application.addEventListener("checkpoint", applicationStateCheckpoint, false);
        WinJS.Application.addEventListener("activated", applicationActivated, false);

        // Make the details container in the input panel selectable so that end-developers
        // can copy and paste the text
        var detailsContainer = document.querySelector("#input .details");
        if (detailsContainer) {
            detailsContainer.setAttribute("data-win-selectable", "true");
        }

        scenarios = /*@static_cast(HTMLSelectElement)*/document.getElementById("scenarios");

        scenarios.addEventListener("change", 
            /*@static_cast(EventListener)*/function (e) {
                selectScenario(scenarios.options[scenarios.selectedIndex].value);},
            false);
        
        selectScenario("1");

        // Add sdk sample header
        var header = document.getElementById("header");
        if (Boolean(header)) {
            var logo = document.createElement("img");
            var title = document.createElement("span");
            logo.src = "images/windows-sdk.png";
            logo.alt = "Windows Logo";
            title.innerText = "Windows SDK Samples";
            
            header.appendChild(/*@static_cast(Node)*/logo);
            header.appendChild(/*@static_cast(Node)*/title);
        }

        // Add sdk sample footer
        var footer = document.getElementById("footer");
        if (Boolean(footer)) {
            var footerLogo = document.createElement("img");
            var footerText = document.createElement("div");
            var links = document.createElement("div");
            var company = document.createElement("div");
            var companyText = document.createElement("span");
            var terms = document.createElement("a");
            var pipe = document.createElement("span");
            var trademarks = document.createElement("a");
            var privacy = document.createElement("a");

            footerLogo.src = "images/microsoft-sdk.png";
            footerLogo.alt = "Microsoft";
            company.className = "company";
            companyText.innerText = "© Microsoft. All rights reserved.";
            terms.innerText = "Terms of use";
            terms.href = "http://www.microsoft.com/About/Legal/EN/US/IntellectualProperty/Copyright/default.aspx";
            trademarks.innerText = "Trademarks";
            trademarks.href = "http://www.microsoft.com/About/Legal/EN/US/IntellectualProperty/Trademarks/EN-US.aspx";
            privacy.innerText = "Privacy Statement";
            privacy.href = "http://privacy.microsoft.com";
            links.className = "links";
            pipe.className = "pipe";

            links.appendChild(/*@static_cast(Node)*/terms);
            links.appendChild(/*@static_cast(Node)*/pipe);
            links.appendChild(/*@static_cast(Node)*/trademarks);
            links.appendChild(/*@static_cast(Node)*/pipe.cloneNode(true));
            links.appendChild(/*@static_cast(Node)*/privacy);

            company.appendChild(/*@static_cast(Node)*/companyText);
            footerText.appendChild(/*@static_cast(Node)*/company);
            footerText.appendChild(/*@static_cast(Node)*/links);

            footer.appendChild(/*@static_cast(Node)*/footerLogo);
            footer.appendChild(/*@static_cast(Node)*/footerText);
        }
    }

    document.addEventListener("DOMContentLoaded", initialize, false);
})();

// Adding sdkSample to document to enable usage by the test framework
document.sdkSample = sdkSample;

window.onerror = /* @static_cast(ErrorFunction) */ function (msg, url, line) { sdkSample.displayError("Error: " + msg + " url = " + url + " line = " + line); };
