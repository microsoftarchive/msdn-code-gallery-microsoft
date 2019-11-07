//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            var template = element.querySelector("#listView").winControl.itemTemplate;
            element.querySelector("#listView").winControl.itemTemplate = incrementalTemplate(template, myData, getNItemsAsync.bind(null, 8));
        },
        unload: function () {
            resetMyData();
        }
    });

    function incrementalTemplate(template, data, getMoreData) {
        return function (itemPromise) {
            var fetching;
            return itemPromise.then(function (item) {
                if (item.key === data.getItem(data.length - 1).key) {
                    if (!fetching) {
                        fetching = true;
                        getMoreData().then(function () {
                            fetching = false;
                        });
                    }
                }
                return template(itemPromise);
            });
        };
    }
})();
