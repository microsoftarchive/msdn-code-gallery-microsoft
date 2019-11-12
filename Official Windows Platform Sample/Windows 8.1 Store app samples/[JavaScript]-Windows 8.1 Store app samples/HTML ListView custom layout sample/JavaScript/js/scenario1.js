//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            // Wire up ListView properties
            this._statusTemplate = element.querySelector(".statusTemplate").winControl;
            this._photoTemplate = element.querySelector(".photoTemplate").winControl;
            this._listView = element.querySelector(".listView").winControl;
            this._listView.itemTemplate = this._statusRenderer.bind(this);
        },

        // Conditional renderer that chooses between statusTemplate and photoTemplate
        _statusRenderer: function (itemPromise) {
            var that = this;
            return itemPromise.then(function (item) {
                
                if (item.data.type === "photo") {
                    return that._photoTemplate.renderItem(itemPromise);
                }

                return that._statusTemplate.renderItem(itemPromise);
            });
        }
    });

    WinJS.Namespace.define("SDKSample.Scenario1", {
        StatusLayout: WinJS.Class.define(function (options) {
            this._site = null;
            this._surface = null;
        }, 
        {
            // This sets up any state and CSS layout on the surface of the custom layout
            initialize: function (site) {
                this._site = site;
                this._surface = this._site.surface;

                // Add a CSS class to control the surface level layout
                WinJS.Utilities.addClass(this._surface, "statusLayout");

                return WinJS.UI.Orientation.vertical;
            },

            // Reset the layout to its initial state
            uninitialize: function () {
                WinJS.Utilities.removeClass(this._surface, "statusLayout");
                this._site = null;
                this._surface = null;
            },
        })
    });
    

})();
