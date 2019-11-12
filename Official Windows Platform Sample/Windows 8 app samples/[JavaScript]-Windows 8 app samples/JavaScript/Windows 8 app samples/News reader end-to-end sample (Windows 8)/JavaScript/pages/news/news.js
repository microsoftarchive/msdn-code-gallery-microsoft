(function () {
    "use strict";

    var appView = Windows.UI.ViewManagement.ApplicationView;
    var appViewState = Windows.UI.ViewManagement.ApplicationViewState;
    var nav = WinJS.Navigation;
    var ui = WinJS.UI;
    var utils = WinJS.Utilities;

    var groupedItems;
    var listViewIn;
    var semanticZoom;
    var page;

    ui.Pages.define("/pages/news/news.html", {

        // This function updates the ListView with new layouts
        initializeLayout: function (viewState) {
            /// <param name="listView" value="WinJS.UI.ListView.prototype" />

            if (viewState === appViewState.snapped) {
                semanticZoom.locked = true;
                listViewIn.itemDataSource = groupedItems.groups.dataSource;
                listViewIn.groupDataSource = null;
                listViewIn.itemTemplate = document.querySelector("#feedTileTemplate");
                listViewIn.layout = new ui.ListLayout();
            } else {
                semanticZoom.locked = false;
                listViewIn.itemDataSource = groupedItems.dataSource;
                listViewIn.groupDataSource = groupedItems.groups.dataSource;
                listViewIn.itemTemplate = Render.renderArticleTile;
                listViewIn.layout = new ui.GridLayout({
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
            if (appView.value === appViewState.snapped) {
                // If the page is snapped, the user invoked a group.
                var feed = groupedItems.groups.getAt(args.detail.itemIndex);
                nav.navigate("/pages/feed/feed.html", { feedKey: feed.key });
            } else {
                // If the page is not snapped, the user invoked an item.
                var article = groupedItems.getAt(args.detail.itemIndex);
                nav.navigate("/pages/article/article.html", { article: article });
            }
        },

        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {

            page = this;

            // Get the grouped articles.
            groupedItems = Render.getGroupedArticleItems(Data.subFeeds, Constants.MAX_ITEMS);

            // Force the semantic zoom layout.
            semanticZoom = element.querySelector("#articlesListViewArea").winControl;
            semanticZoom.forceLayout();

            // Setup the zoomed-in list view.
            listViewIn = element.querySelector("#articlesListView-in").winControl;
            listViewIn.groupHeaderTemplate = element.querySelector("#feedHeaderTemplate");
            listViewIn.oniteminvoked = this.itemInvoked.bind(this);
            this.initializeLayout(appView.value);
            listViewIn.element.focus();

            // Setup the zoomed-out list view.
            var listViewOut = element.querySelector("#articlesListView-out").winControl;
            listViewOut.itemDataSource = groupedItems.groups.dataSource;
            listViewOut.itemTemplate = element.querySelector("#feedTileTemplate");
            listViewOut.layout = new ui.GridLayout();

            document.getElementById("refreshContentCmd").addEventListener("click", refreshFeeds, false);
        },

        // This function updates the page layout in response to viewState changes.
        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />
            /// <param name="viewState" value="Windows.UI.ViewManagement.ApplicationViewState" />
            /// <param name="lastViewState" value="Windows.UI.ViewManagement.ApplicationViewState" />

            listViewIn = element.querySelector("#articlesListView-in").winControl;
            semanticZoom = element.querySelector("#articlesListViewArea").winControl;
            this.initializeLayout(viewState);
            if (lastViewState !== viewState) {
                if (lastViewState === appViewState.snapped || viewState === appViewState.snapped) {
                    var handler = function (e) {
                        listViewIn.removeEventListener("contentanimating", handler, false);
                        e.preventDefault();
                    };
                    listViewIn.addEventListener("contentanimating", handler, false);
                    this.initializeLayout(viewState);
                }
            }
        },

        unload: function () {
            document.getElementById("refreshContentCmd").removeEventListener("click", refreshFeeds, false);
        }
    });

    // Refreshes the feeds displayed on the page.
    function refreshFeeds() {

        Data.subFeeds.forEach(function (feed) {
            feed.refreshArticlesAsync();
        });

        groupedItems = Render.getGroupedArticleItems(Data.subFeeds, Constants.MAX_ITEMS);
        page.initializeLayout(appView.value);
    }
})();