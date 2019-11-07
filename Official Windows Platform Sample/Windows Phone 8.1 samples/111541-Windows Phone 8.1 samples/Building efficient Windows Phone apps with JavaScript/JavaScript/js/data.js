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

    function getFeaturedCollection(collection) {
        return LOCPictures.getCollection(collection);
    }

    var data = LOCPictures.getCollections().
    then(function (message) {
        var data = message;
        var dataList = new WinJS.Binding.List(data);

        var collectionTasks = [];
        for (var i = 0; i < 6; i++) {
            collectionTasks.push(getFeaturedCollection(data[i]));
        }

        return WinJS.Promise.join(collectionTasks).then(function () {
            return dataList;
        });
    });

    WinJS.Namespace.define("Data", {
        featuredCollections: data
    });
})();