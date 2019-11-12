// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";

    WinJS.UI.Pages.define("/pages/itemDetail/itemDetail.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            // TODO: Initialize the page here.
            // Display the appbar but hide the Full View button
            var appbar = document.getElementById('appbar');
            var appbarCtrl = appbar.winControl;
            appbarCtrl.hideCommands(["view"], false);

            var item = options && options.item ? options.item : Data.items.getAt(0);
            element.querySelector(".titlearea .pagetitle").textContent = item.title;
            element.querySelector("article .item-content").innerHTML = item.content;

        },

        unload: function () {
            // TODO: Respond to navigations away from this page.
        },

        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />

            // TODO: Respond to changes in viewState.
        }
    });
})();

