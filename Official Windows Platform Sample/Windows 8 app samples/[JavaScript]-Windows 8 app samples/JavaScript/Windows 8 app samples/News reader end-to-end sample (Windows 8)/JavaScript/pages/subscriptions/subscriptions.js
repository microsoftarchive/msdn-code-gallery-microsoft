// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";

    // Some helper variables.
    var appView = Windows.UI.ViewManagement.ApplicationView;
    var appViewState = Windows.UI.ViewManagement.ApplicationViewState;
    var ui = WinJS.UI;

    WinJS.UI.Pages.define("/pages/subscriptions/subscriptions.html", {

        // This function updates the ListView with new layouts
        initializeLayout: function (listView, viewState) {
            /// <param name="listView" value="WinJS.UI.ListView.prototype" />

            if (viewState === appViewState.snapped) {
                listView.itemDataSource = Render.groupedFeedItems.dataSource;
                listView.groupDataSource = null;
                listView.layout = new ui.ListLayout();
            } else {
                listView.itemDataSource = Render.groupedFeedItems.dataSource;
                listView.groupDataSource = Render.groupedFeedItems.groups.dataSource;
                listView.layout = new ui.GridLayout({ groupHeaderPosition: "top" });
            }
        },

        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {

            // Disable the app bar.
            var appbar = document.getElementById("appbar").winControl;
            appbar.disabled = true;

            // Setup the zoomed in list view.
            var listViewIn = element.querySelector("#feedsListView-in").winControl;
            listViewIn.groupHeaderTemplate = element.querySelector("#categoryHeaderTemplate");
            listViewIn.itemTemplate = element.querySelector("#feedItemTemplate");
            this.initializeLayout(listViewIn, appView.value);
            listViewIn.element.focus();

            // Setup the zoomed out list view.
            var listViewOut = element.querySelector("#feedsListView-out").winControl;
            listViewOut.itemDataSource = Render.groupedFeedItems.groups.dataSource;
            listViewOut.itemTemplate = element.querySelector("#categoryItemTemplate");
            listViewOut.layout = new ui.GridLayout();

            // Set the selection mode.
            listViewIn.selectionMode = WinJS.UI.SelectionMode.multi;
            listViewIn.tapBehavior = WinJS.UI.TapBehavior.toggleSelect;
            listViewIn.selection.set(Render.selectionIndices);

            // Handle selection events.
            listViewIn.addEventListener("selectionchanged", function (evt) {
                var oldSel = Render.selectionIndices;
                var newSel = listViewIn.selection.getIndices();
                Render.selectionIndices = newSel;

                if (newSel.length > oldSel.length) {

                    // Get the newly added index.
                    var added = getDiffInArrays(newSel, oldSel);
                    var addedIndex = added[0];

                    // Subscribe to the selected feed.
                    var feed = Render.getFeedObject(addedIndex);
                    Data.subscribe(feed);

                    // Check for the subscription limit.
                    if (Data.subFeeds.length >= Constants.SUB_LIMIT) {
                        listViewIn.selectionMode = WinJS.UI.SelectionMode.none;
                        listViewIn.tapBehavior = WinJS.UI.TapBehavior.none;
                    }
                } else {

                    // Get the newly removed index.
                    var removed = getDiffInArrays(oldSel, newSel);
                    var removedIndex = removed[0];

                    // Unsubscribe from the selected feed.
                    var theFeed = Render.getFeedObject(removedIndex);
                    Data.unsubscribe(theFeed);

                    // Check for the subscription limit.
                    if (Data.subFeeds.length < Constants.SUB_LIMIT) {
                        listViewIn.selectionMode = WinJS.UI.SelectionMode.multi;
                        listViewIn.tapBehavior = WinJS.UI.TapBehavior.toggleSelect;
                    }
                }
            });

            document.getElementById("doneButton").addEventListener("click", function () {
                WinJS.Navigation.navigate("/pages/news/news.html");
            }, false);
        },

        unload: function () {

            // Re-enable the appbar.
            var appbar = document.getElementById("appbar").winControl;
            appbar.disabled = false;
        }
    });

    // Return an array with integers in 'a' but not in 'b'.
    function getDiffInArrays(a, b) {
        return a.filter(function (obj) {
            return (b.indexOf(obj) === -1);
        });
    }
})();