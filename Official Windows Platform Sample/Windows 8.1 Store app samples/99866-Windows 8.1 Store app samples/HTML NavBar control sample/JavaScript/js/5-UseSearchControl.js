//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/5-UseSearchControl.html", {
        ready: function (element, options) {
            document.body.querySelector('#useSearch').addEventListener('invoked', this.navbarInvoked.bind(this));
        },

        navbarInvoked: function (ev) {
            var navbarCommand = ev.detail.navbarCommand;
            WinJS.log && WinJS.log(navbarCommand.label + " NavBarCommand invoked", "sample", "status");
            document.querySelector('select').focus();
        }
    });
})();
