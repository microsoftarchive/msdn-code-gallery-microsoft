//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/1-CreateNavBar.html", {
        ready: function (element, options) {
            document.body.querySelector('#createNavBar').addEventListener('invoked', this.navbarInvoked.bind(this));
            document.getElementById("switchStyle").addEventListener("click", switchStyle, false);
        },

        navbarInvoked: function (ev) {
            var navbarCommand = ev.detail.navbarCommand;
            WinJS.log && WinJS.log(navbarCommand.label + " NavBarCommand invoked", "sample", "status");
            document.querySelector('select').focus();
        }
    });

    function switchStyle() {
        var linkEl = document.querySelector('link');
        if (linkEl.getAttribute('href') === "//Microsoft.WinJS.2.0/css/ui-light.css") {
            linkEl.setAttribute('href', "//Microsoft.WinJS.2.0/css/ui-dark.css");
            document.getElementById("switchStyle").innerText = "Switch to light stylesheet";
            document.body.style.backgroundColor = "rgb(20,20,20)";
        } else {
            linkEl.setAttribute('href', "//Microsoft.WinJS.2.0/css/ui-light.css");
            document.getElementById("switchStyle").innerText = "Switch to dark stylesheet";
            document.body.style.backgroundColor = "rgb(240,240,240)";
        }
    }
})();
