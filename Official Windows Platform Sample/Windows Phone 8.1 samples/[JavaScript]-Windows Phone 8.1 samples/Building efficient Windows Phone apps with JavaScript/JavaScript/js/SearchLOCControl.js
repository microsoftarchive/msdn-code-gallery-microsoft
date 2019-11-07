//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
(function () {
    "use strict";

    WinJS.Namespace.define("SearchLOCControl", {
        Control: WinJS.Class.define(function (element) {
            this.element = element || document.createElement('div');
            this.element.winControl = this;

            this._disposed = false;
            WinJS.Utilities.addClass(this.element, "win-disposable");

            var htmlString = "<h3>Library of Congress Picture Search</h3>" +
               "<input type='search' id='searchQuery' placeholder='Browse pictures' />" +
                  "<br/><br/>" +
                  "<div id='searchResults' class='searchList'></div>" +
                  "<div id='searchResultsTemplate' data-win-control='WinJS.Binding.Template'>" +
                     "<div class='searchResultsItem'>" +
                        "<img src='#' data-win-bind='src: pictureThumb' />" +
                        "<div class='details'>" +
                           "<p data-win-bind='textContent: title'></p>" +
                           "<p data-win-bind='textContent: date'></p>" +
                        "</div></div></div>";

            var that = this;

            // NOTE: This is an unusual technique for accomplishing this
            // task. The code here is written specifically to be terse.
            MSApp.execUnsafeLocalFunction(function () {
                $(that.element).append(htmlString);
                WinJS.UI.processAll().done(function () {
                    that.searchQuery = $("#searchQuery")[0];
                    that.searchQuery.onchange = that.submitQuery;
                });
            });

        }, {
            submitQuery: function (evt) {
                var queryString = evt.target.value;
                var searchResultsList = $("#searchResults")[0];

                if (queryString != "") {
                    var searchResults = LOCPictures.searchPictures(queryString).
                       then(function (response) {
                           var searchList = new WinJS.Binding.List(response),
                              searchListView;

                           if (searchResultsList.winControl) {
                               searchListView = searchResultsList.winControl;
                               searchListView.itemDataSource = searchList.dataSource;
                           }
                           else {
                               searchListView = new WinJS.UI.ListView(searchResultsList, {
                                   itemDataSource: searchList.dataSource,
                                   itemTemplate: $("#searchResultsTemplate")[0],
                                   layout: { type: WinJS.UI.CellSpanningLayout }
                               });
                           }

                           WinJS.UI.process(searchListView);
                       });
                }
            },
            dispose: function () {
                this._disposed = true;

                this.searchQuery.removeEventListener("querysubmitted", this.submitQuery);

                WinJS.Utilities.disposeSubTree(this.element);

                this.searchQuery = null;
                this.element.winControl = null;
                this.element = null;
            }

        })
    })
})();