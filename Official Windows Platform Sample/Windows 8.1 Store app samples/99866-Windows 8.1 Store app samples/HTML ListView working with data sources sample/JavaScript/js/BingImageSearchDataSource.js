// Bing image search data source example
//
// This implements a datasource that will fetch images from Bing's image search feature of the azure data marketplace.
// The Azure Marketplace requires the developer to subscribe to services and get an account key. Bing offers a free 
// trial subscription which can be used with this sample. For more details see:
// https://datamarket.azure.com/dataset/5BA839F1-12CE-4CCE-BF57-A49D98D29A44

(function () {

    // Definition of the data adapter
    var bingImageSearchDataAdapter = WinJS.Class.define(
        function (devkey, query) {

            // Constructor
            this._minPageSize = 50;
            this._maxPageSize = 50;
            this._maxCount = 1000;
            this._devkey = devkey;
            this._query = query;
        },

        // Data Adapter interface methods
        // These define the contract between the virtualized datasource and the data adapter.
        // These methods will be called by virtualized datasource to fetch items, count etc.
        {
            // This example only implements the itemsFromIndex and count methods

            // Called to get a count of the items
            // The value of the count can be updated later in the response to itemsFromIndex
            getCount: function () {
                var that = this;

                // As the bing API does not return a count at this time, and the queries invariably return
                // large datasets, we'll check the results and then assume its maxcount if the result set is full

                // Build up a URL to request 50 items so we can get the count if less than that
                var requestStr = "https://api.datamarket.azure.com/Data.ashx/Bing/Search/Image"

                // Common request fields (required)
                + "?Query='" + that._query + "'"
                + "&$format=json"
                + "&Market='en-us'"
                + "&Adult='Strict'"
                + "&$top=50"
                + "&$skip=0";

                //Return the promise from making an XMLHttpRequest to the server
                // The bing API authenticates using any username and the developer key as the password.
                return WinJS.xhr({ url: requestStr, user: "foo", password: that._devkey, }).then(

                    //Callback for success
                    function (request) {
                        var obj = JSON.parse(request.responseText);

                        // Verify if the service has returned images
                        if (obj.d && obj.d.results) {
                            var count = obj.d.results.length < 50 ? obj.d.results.length : that._maxCount;
                            if (count === 0) { WinJS.log && WinJS.log("The search returned 0 results.", "sample", "error"); }
                            return count;
                        } else {
                            WinJS.log && WinJS.log("Error fetching results from the Bing API", "sample", "error");
                            return 0;
                        }
                    },
                    // Called if the XHR fails
                     function (request) {
                         if (request && request.name === "Canceled") {
                             return WinJS.Promise.wrapError(request);
                         } else {
                             if (request.status === 401) {
                                 WinJS.log && WinJS.log(request.statusText, "sample", "error");
                             } else {
                                 WinJS.log && WinJS.log("Error fetching data from the service. " + request.responseText, "sample", "error");
                             }
                             
                             return 0;
                         }
                     });
            },

            // Called by the virtualized datasource to fetch items
            // It will request a specific item and optionally ask for a number of items on either side of the requested item. 
            // The implementation should return the specific item and, in addition, can choose to return a range of items on either 
            // side of the requested index. The number of extra items returned by the implementation can be more or less than the number requested.
            //
            // Must return back an object containing fields:
            //   items: The array of items of the form items=[{ key: key1, data : { field1: value, field2: value, ... }}, { key: key2, data : {...}}, ...];
            //   offset: The offset into the array for the requested item
            //   totalCount: (optional) update the value of the count
            itemsFromIndex: function (requestIndex, countBefore, countAfter) {
                var that = this;
                if (requestIndex >= that._maxCount) {
                    return WinJS.Promise.wrapError(new WinJS.ErrorFromName(WinJS.UI.FetchError.doesNotExist));
                }

                var fetchSize, fetchIndex;
                // See which side of the requestIndex is the overlap
                if (countBefore > countAfter) {
                    //Limit the overlap
                    countAfter = Math.min(countAfter, 10);
                    //Bound the request size based on the minimum and maximum sizes
                    var fetchBefore = Math.max(Math.min(countBefore, that._maxPageSize - (countAfter + 1)), that._minPageSize - (countAfter + 1));
                    fetchSize = fetchBefore + countAfter + 1;
                    fetchIndex = requestIndex - fetchBefore;
                } else {
                    countBefore = Math.min(countBefore, 10);
                    var fetchAfter = Math.max(Math.min(countAfter, that._maxPageSize - (countBefore + 1)), that._minPageSize - (countBefore + 1));
                    fetchSize = countBefore + fetchAfter + 1;
                    fetchIndex = requestIndex - countBefore;
                }

                // Build up a URL for the request
                var requestStr = "https://api.datamarket.azure.com/Data.ashx/Bing/Search/Image"

                // Common request fields (required)
                + "?Query='" + that._query + "'"
                + "&$format=json"
                + "&Market='en-us'"
                + "&Adult='Strict'"
                + "&$top=" + fetchSize
                + "&$skip=" + fetchIndex;

                // Return the promise from making an XMLHttpRequest to the server
                // The bing API authenticates using any username and the developer key as the password.
                return WinJS.xhr({ url: requestStr, user: "foo", password: that._devkey }).then(

                    //Callback for success
                    function (request) {
                        var results = [], count;

                        // Use the JSON parser on the results, safer than eval
                        var obj = JSON.parse(request.responseText);

                        // Verify if the service has returned images
                        if (obj.d && obj.d.results) {
                            var items = obj.d.results;

                            // Data adapter results needs an array of items of the shape:
                            // items =[{ key: key1, data : { field1: value, field2: value, ... }}, { key: key2, data : {...}}, ...];
                            // Form the array of results objects
                            for (var i = 0, itemsLength = items.length; i < itemsLength; i++) {
                                var dataItem = items[i];
                                results.push({
                                    key: (fetchIndex + i).toString(),
                                    data: {
                                        title: dataItem.Title,
                                        thumbnail: dataItem.Thumbnail.MediaUrl,
                                        width: dataItem.Width,
                                        height: dataItem.Height,
                                        linkurl: dataItem.SourceUrl,
                                        url: dataItem.MediaUrl
                                    }
                                });
                            }

                            WinJS.log && WinJS.log("", "sample", "status");
                            return {
                                items: results, // The array of items
                                offset: requestIndex - fetchIndex, // The offset into the array for the requested item
                            };

                        } else {
                            WinJS.log && WinJS.log(request.statusText, "sample", "error");
                            return WinJS.Promise.wrapError(new WinJS.ErrorFromName(WinJS.UI.FetchError.doesNotExist));
                        }
                    },

                    //Called on an error from the XHR Object
                    function (request) {
                        if (request.status === 401) {
                            WinJS.log && WinJS.log(request.statusText, "sample", "error");
                        } else {
                            WinJS.log && WinJS.log("Error fetching data from the service. " + request.responseText, "sample", "error");
                        }
                        return WinJS.Promise.wrapError(new WinJS.ErrorFromName(WinJS.UI.FetchError.noResponse));
                    });
            }

            // setNotificationHandler: not implemented
            // itemsFromStart: not implemented
            // itemsFromEnd: not implemented
            // itemsFromKey: not implemented
            // itemsFromDescription: not implemented
        });

    WinJS.Namespace.define("bingImageSearchDataSource", {
        datasource: WinJS.Class.derive(WinJS.UI.VirtualizedDataSource, function (devkey, query) {
            this._baseDataSourceConstructor(new bingImageSearchDataAdapter(devkey, query));
        })
    });
})();
