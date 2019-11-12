(function () {
    "use strict";

    // Helper variables.
    var appViewState = Windows.UI.ViewManagement.ApplicationViewState;
    var nav = WinJS.Navigation;
    var ui = WinJS.UI;
    var utils = WinJS.Utilities;

    var groupedItems;
    var listView;
    var feed;
    var page;

    ui.Pages.define("/pages/feed/feed.html", {

        // This function updates the ListView with new layouts
        initializeLayout: function (viewState) {
            /// <param name="listView" value="WinJS.UI.ListView.prototype" />

            if (viewState === appViewState.snapped) {
                listView.itemTemplate = document.querySelector("#textArticleTemplate");
                listView.layout = new ui.ListLayout();
            } else {
                listView.itemTemplate = Render.renderArticleTile;
                listView.layout = new ui.GridLayout({
                    groupHeaderPosition: "top",
                    groupInfo: {
                        enableCellSpanning: true,
                        cellWidth: 250,
                        cellHeight: 250
                    }
                });
            }
        },

        itemInvoked: function (args) {
            var article = groupedItems.getAt(args.detail.itemIndex);
            nav.navigate("/pages/article/article.html", { article: article });
        },

        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            page = this;
            listView = element.querySelector("#articlesListView").winControl;

            // Get the current feed.
            if (options && options.feed) {
                feed = options.feed;
            } else if (options && options.feedKey) {
                var feeds = Data.subFeeds;
                for (var i = 0, len = feeds.length; i < len; i++) {
                    if (feeds[i].url === options.feedKey) {
                        feed = feeds[i];
                        break;
                    }
                }
            } else if (!feed) {
                feed = Data.subFeeds[0];
            }

            element.querySelector("header[role=banner] .pagetitle").textContent = feed.title;

            groupedItems = Render.getGroupedArticleItems([feed]);

            // Setup the list view.
            listView.itemDataSource = groupedItems.dataSource;
            listView.oniteminvoked = this.itemInvoked.bind(this);
            this.initializeLayout(Windows.UI.ViewManagement.ApplicationView.value);
            listView.element.focus();

            document.getElementById("refreshContentCmd").addEventListener("click", refreshFeed, false);
        },

        // This function updates the page layout in response to viewState changes.
        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />
            /// <param name="viewState" value="Windows.UI.ViewManagement.ApplicationViewState" />
            /// <param name="lastViewState" value="Windows.UI.ViewManagement.ApplicationViewState" />

            listView = element.querySelector("#articlesListView").winControl;
            if (lastViewState !== viewState) {
                if (lastViewState === appViewState.snapped || viewState === appViewState.snapped) {
                    var handler = function (e) {
                        listView.removeEventListener("contentanimating", handler, false);
                        e.preventDefault();
                    };
                    listView.addEventListener("contentanimating", handler, false);
                    var firstVisible = listView.indexOfFirstVisible;
                    this.initializeLayout(viewState);
                    listView.indexOfFirstVisible = firstVisible;
                }
            }
        },

        unload: function () {
            document.getElementById("refreshContentCmd").removeEventListener("click", refreshFeed, false);
        }
    });

    // Refreshes the feeds displayed on the page.
    function refreshFeed() {
        feed.refreshArticlesAsync();
        groupedItems = Render.getGroupedArticleItems([feed], Constants.MAX_ITEMS);
        page.initializeLayout(Windows.UI.ViewManagement.ApplicationView.value);
    }
})();
