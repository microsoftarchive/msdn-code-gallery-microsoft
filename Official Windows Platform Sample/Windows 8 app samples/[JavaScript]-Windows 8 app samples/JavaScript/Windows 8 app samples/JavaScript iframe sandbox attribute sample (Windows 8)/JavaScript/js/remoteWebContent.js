//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/remoteWebContent.html", {
        ready: function (element, options) {
            document.getElementById("loadUrl").addEventListener("click", loadIframes, false);
            document.getElementById("reset").addEventListener("click", resetSample, false);
            document.getElementById("leftButton").addEventListener("click", SdkSample.showLeftHost, false);
            document.getElementById("rightButton").addEventListener("click", SdkSample.showRightHost, false);
            document.getElementById("allowScripts").addEventListener("change", updateTokens, false);
            document.getElementById("allowSameOrigin").addEventListener("change", updateTokens, false);
            document.getElementById("allowForms").addEventListener("change", updateTokens, false);
        }
    });

    function loadIframes() {
        // If a web site URL is not available, go to the default iframe HTML.
        if (document.getElementById("webSiteUrl").value === "") {
            updateIframes("/html/iframe.html");
        } else {
            updateIframes(document.getElementById("webSiteUrl").value);
        }
        hideHosts();
    }

    function resetSample() {
        document.getElementById("webSiteUrl").value = "";
        document.getElementById("iframe2").sandbox = "";
        updateIframes("/html/iframe.html");
        document.getElementById("iframe2header").innerText = "<iframe sandbox=\"\">";
        document.getElementById("allowScripts").checked = false;
        document.getElementById("allowSameOrigin").checked = false;
        document.getElementById("allowForms").checked = false;
        hideHosts();
    }

    function hideHosts() {
        document.getElementById("leftHost").style.visibility = "hidden";
        document.getElementById("rightHost").style.visibility = "hidden";
    }

    function updateIframes(url) {
        document.getElementById("iframe1").src = url;
        document.getElementById("iframe2").src = url;
    }

    function updateTokens() {
        var sandboxAttribute = "";
        if (document.getElementById("allowScripts").checked === true) {
            sandboxAttribute += "allow-scripts ";
        }
        if (document.getElementById("allowSameOrigin").checked === true) {
            sandboxAttribute += "allow-same-origin ";
        }
        if (document.getElementById("allowForms").checked === true) {
            sandboxAttribute += "allow-forms ";
        }
        document.getElementById("iframe2").sandbox = sandboxAttribute.trim();
        document.getElementById("iframe2header").innerText = "<iframe sandbox=\"" + sandboxAttribute.trim() + "\">";
        loadIframes();
    }
})();
