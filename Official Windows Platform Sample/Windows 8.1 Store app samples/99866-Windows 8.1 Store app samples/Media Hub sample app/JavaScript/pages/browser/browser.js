//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var ui = WinJS.UI;

    // When the user is panning the list new tiles come to the view and the app needs to generate
    // their thumbnails, but to avoid firing and cancelling several of these operations while the
    // user is still interacting with the list the app waits for a few milliseconds before actually
    // starting. The value of 400 milliseconds was chossen after manually testing the scenario, the 
    // wait is long enough to avoid unnecessary operations while panning/scrolling but short enough
    // to start thumbnail generation when the users stops interacting with the view in a pleasent
    // manner. 
    var waitForUserInteractionMilliseconds = 400;

    WinJS.UI.Pages.define("/pages/browser/browser.html", {
        _listView: null,
        _updateTimeout: null,
        _stopUpdate: false,

        processed: function (element, options) {
            // Change the page title
            element.querySelector("header[role=banner] .pagetitle").textContent = options.title;

            // Initialize the list view
            this._listView = element.querySelector(".itemslist").winControl;
            this._listView.itemTemplate = element.querySelector(".itemtemplate");
            this._listView.groupHeaderTemplate = element.querySelector(".headertemplate");
            this._listView.itemDataSource = options.list.dataSource;
            this._listView.groupDataSource = (options.list.groups) ? options.list.groups.dataSource : null;
            this._listView.oniteminvoked = this._itemInvoked.bind(this);
            this._listView.onloadingstatechanged = this._loadingStateChanged.bind(this);

            element.querySelector(".browserpage .empty").style.display = (options.list && options.list.length > 0) ? "none" : "";
        },

        ready: function () {
            // Set our focus in the list view
            this._listView.element.focus();
        },

        // Called when the page needs to be unloaded
        unload: function () {
            // Disable tile updates 
            this._stopUpdate = true;

            // Dispose all tiles data when needed
            for (var index = 0, size = this._listView.itemDataSource.list.length; index < size; index++) {
                var item = this._listView.itemDataSource.list.getAt(index);
                if (item.dispose) {
                    item.dispose();
                }
            }
        },

        // reloads app resources in the page
        updateResources: function (element, e) {
            // Called by _contextChanged event handler in navigator.js when a resource 
            // qualifier (language, scale, contrast, etc.) has changed. The element 
            // passed is the root of this page. 
            //
            // Since this sample app currently doesn't have any assets with variants
            // for scale/contrast/language/etc., the lines below that do the actual 
            // work are commented out. This is provided here to model how to handle 
            // scale or other resource context changes if this app were expanded to 
            // include resources with assets for such variantions.

            // Will filter for changes to specific qualifiers.
            if (e.detail.qualifier === "Scale" || e.detail.qualifier === "Contrast") {
                // if there are string resources bound to properties using data-win-res,
                // the following line will update those properties: 

                //WinJS.Resources.processAll(element);

                // Background images from the app package with variants for scale, etc. 
                // are automatically reloaded by the platform when a resource context 
                // qualifier has changed. That is not done, however, for img elements. 
                // The following will make sure those are updated:

                //var imageElements = document.getElementsByTagName("img");
                //for (var i = 0, l = imageElements.length; i < l; i++) {
                //    var previousSource = imageElements[i].src;
                //    var uri = new Windows.Foundation.Uri(document.location, previousSource);
                //    if (uri.schemeName === "ms-appx") {
                //        imageElements[i].src = "";
                //        imageElements[i].src = previousSource;
                //    }
                //}
            }
        },

        // Loading thumbnails is expensive, so load only those that are 
        // currently in view this function is used. Wait delays the operation
        // for the given milliseconds; this is done to avoid a burst of requests
        // while the user is scrolling the page.
        updateTilesRequest: function (wait) {
            var that = this;

            // Keep a local variable of the timeout handle so when the handler triggers
            // it is known if this is the last one scheduled 
            var timeout = setTimeout(function () {

                // If need to stop or this wasn't the last request just return
                if (timeout !== that._updateTimeout || that._stopUpdate) {
                    return;
                }

                // Get the segment of tiles that will be updated
                var list = that._listView.itemDataSource.list;
                var start = that._listView.indexOfFirstVisible;
                var stop = that._listView.indexOfLastVisible;

                // Update the tiles asynchronously but sequentially
                (function updateDataAsync() {
                    if (start <= stop && start >= 0 && timeout === that._updateTimeout && !that._stopUpdate) {
                        var item = list.getAt(start);
                        if (item.updateDataAsync) {
                            item.updateDataAsync().done(function () {
                                setImmediate(updateDataAsync, ++start);
                            });
                        } else {
                            setImmediate(updateDataAsync, ++start);
                        }
                    }
                })();
            }, wait);

            // This is the last request scheduled
            that._updateTimeout = timeout;
        },

        // If the list view finishes loading and tile update has not been performed once schedule it 
        // immediately, otherwise, and if scrolling, schedule a tile update
        _loadingStateChanged: function (args) {
            if (this._listView.loadingState === "complete" && this._updateTimeout === null) {
                this.updateTilesRequest(0);
            } else if (args.detail && args.detail.scrolling) {
                this.updateTilesRequest(waitForUserInteractionMilliseconds);
            }
        },

        // Handle list view tiles invokation
        _itemInvoked: function (args) {
            var item = this._listView.itemDataSource.list.getAt(args.detail.itemIndex);
            if (item.invoke) {
                item.invoke();
            }
        }
    });
})();
