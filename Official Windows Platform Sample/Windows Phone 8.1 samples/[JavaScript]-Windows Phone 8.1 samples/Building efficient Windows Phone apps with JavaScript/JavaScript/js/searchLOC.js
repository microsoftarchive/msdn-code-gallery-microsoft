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

    var baseUrl = "http://loc.gov/pictures/"
    var httpClient = new Windows.Web.Http.HttpClient();

    function searchPictures(query) {
        var url = baseUrl + "search/?q=" + query + "&fo=json";
        var queryURL = encodeURI(url);
        return httpClient.getStringAsync(new Windows.Foundation.Uri(queryURL)).
           then(function (response) {
               return JSON.parse(response).results.map(function (result) {
                   return new SearchResult(result);
               });
           });
    }

    function getCollections() {
        var url = baseUrl + "?fo=json";
        return httpClient.getStringAsync(new Windows.Foundation.Uri(url)).
              then(function (response) {
                  return JSON.parse(response).featured.
                     map(function (collection) {
                         return new Collection(collection);
                     });
              });
    }

    function getCollection(collection) {
        var url = baseUrl + "search/?co=" + collection.code + "&fo=json";
        var queryUrl = encodeURI(url);
        return httpClient.getStringAsync(new Windows.Foundation.Uri(queryUrl)).
           then(function (response) {
               collection.pictures = JSON.parse(response).
                  results.map(function (picture) {
                      return new SearchResult(picture);
                  });
               return collection;
           });
    }

    function Collection(info) {
        this.title = info.title;
        this.featuredThumb = info.thumb_featured;
        this.code = info.code;
        this.pictures = [];
    }

    function SearchResult(data) {
        this.pictureThumb = data.image.thumb;
        this.title = data.title;
        this.date = data.created_published_date;
    }

    WinJS.Namespace.define("LOCPictures", {
        Collection: Collection,
        searchPictures: searchPictures,
        getCollections: getCollections,
        getCollection: getCollection
    });
})();