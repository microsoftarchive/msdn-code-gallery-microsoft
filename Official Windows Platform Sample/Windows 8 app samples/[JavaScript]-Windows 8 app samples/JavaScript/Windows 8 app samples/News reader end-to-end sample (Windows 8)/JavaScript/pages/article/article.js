(function () {
    "use strict";

    // Helper variables.
    var ui = WinJS.UI;
    var utils = WinJS.Utilities;
    var appViewState = Windows.UI.ViewManagement.ApplicationViewState;

    ui.Pages.define("/pages/article/article.html", {

        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {

            var appbar = document.getElementById("appbar").winControl;
            appbar.showOnlyCommands(["subscriptionsCmd"]);

            var article = options.article;
            element.querySelector(".titlearea .pagetitle").textContent = article.title;
            element.querySelector("#article #content").innerHTML = article.body;
            element.querySelector("#articleArea").focus();
        },

        unload: function () {
            var appbar = document.getElementById("appbar").winControl;
            appbar.showOnlyCommands(["refreshContentCmd", "subscriptionsCmd"]);
        }
    });
})();