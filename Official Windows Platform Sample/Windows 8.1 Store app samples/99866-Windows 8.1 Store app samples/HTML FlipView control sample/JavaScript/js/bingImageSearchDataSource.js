// Bing image search data source example
//
// This implements a datasource that will fetch images from Bing's image search feature of the azure data marketplace.
// The Azure Marketplace requires the developer to subscribe to services and get an account key. Bing offers a free 
// trial subscription which can be used with this sample. For more details see:
// https://datamarket.azure.com/dataset/5BA839F1-12CE-4CCE-BF57-A49D98D29A44

(function () {

    //This should not be included/used in real apps, its there to illustrate differences between renderers
    function delayPromise(p, delay) {
        var t = new WinJS.Promise.timeout(delay);
        return WinJS.Promise.join([p, t]).then(function () { return p; });
    }

    // Definition of the data adapter
    var bingImageSearchDataAdapter = WinJS.Class.define(
        function (devkey, query, delay) {

            // Constructor
            this._minPageSize = 1;
            this._maxPageSize = 10;
            this._maxCount = 100;
            this._devkey = devkey;
            this._query = query;
            this._delay = delay;
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

                // Build up a URL to request 1 item so we can get the count
                var requestStr = "https://api.datamarket.azure.com/Data.ashx/Bing/Search/Image"

                // Common request fields (required)
                + "?Query='" + that._query + "'"
                + "&$format=json"
                + "&Market='en-us'"
                + "&Adult='Strict'"
                + "&$top=10"
                + "&$skip=0";

                //Return the promise from making an XMLHttpRequest to the server
                return delayPromise(WinJS.xhr({ url: requestStr, user: "foo", password: that._devkey, headers: { DataServiceVersion: 2.0 } }), that._delay).then(

                    //Callback for success
                    function (request) {
                        // This sample is always searching for Seattle, and we only request up to 100 items
                        return 100;
                    },
                    function (request) {
                        var xhrResult = request[0];
                        if (xhrResult.status === 401) {
                            WinJS.log && WinJS.log(xhrResult.statusText, "sample", "error");
                        } else {
                            WinJS.log && WinJS.log(xhrResult.statusText + ". " + xhrResult.responseText, "sample", "error");
                        }
                        return 0;
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

                //Return the promise from making an XMLHttpRequest to the server
                return delayPromise(WinJS.xhr({ url: requestStr, user: "foo", password: that._devkey, headers: { DataServiceVersion: 2.0 } }), that._delay).then(

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
                                        thumbnail: dataItem.Thumbnail.Url,
                                        width: dataItem.Width,
                                        height: dataItem.Height,
                                        linkurl: dataItem.Url,
                                        url: dataItem.MediaUrl
                                    }
                                });
                            }

                            return {
                                items: results, // The array of items
                                offset: requestIndex - fetchIndex, // The offset into the array for the requested item
                                totalCount: that._maxCount,
                            };
                        } else {
                            return WinJS.Promise.wrapError(new WinJS.ErrorFromName(WinJS.UI.FetchError.doesNotExist));
                        }
                    },

                    //Called on an error from the XHR Object
                    function (request) {
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
        datasource: WinJS.Class.derive(WinJS.UI.VirtualizedDataSource, function (devkey, query, delay) {
            this._baseDataSourceConstructor(new bingImageSearchDataAdapter(devkey, query, delay));
        })
    });
})();
