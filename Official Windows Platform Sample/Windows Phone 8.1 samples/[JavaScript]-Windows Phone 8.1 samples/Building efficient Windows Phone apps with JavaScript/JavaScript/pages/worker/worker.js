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
    WinJS.UI.Pages.define("/pages/worker/worker.html", {
        ready: function (element, options) {

            performance.mark("navigated to Worker");

            var getBaseballCards = new Worker('/js/LOC-worker.js'),
                baseballCards = new LOCPictures.Collection({
                    title: "Baseball cards",
                    thumbFeatured: null,
                    code: "bbc"
                });

            getBaseballCards.onmessage = function (message) {
                createCollection(message.data);
                getBaseballCards.terminate();
            }
            getBaseballCards.postMessage(baseballCards);
        }
    });
    function createCollection(info) {
        var collection = new WinJS.Binding.List(info.pictures),
            collectionElement = $("#collection")[0],
            collectionList = new WinJS.UI.ListView(collectionElement, {
                itemDataSource: collection.dataSource,
                itemTemplate: $('#searchResultsTemplate')[0],
                layout: { type: WinJS.UI.GridLayout }
            });
    }
})();

