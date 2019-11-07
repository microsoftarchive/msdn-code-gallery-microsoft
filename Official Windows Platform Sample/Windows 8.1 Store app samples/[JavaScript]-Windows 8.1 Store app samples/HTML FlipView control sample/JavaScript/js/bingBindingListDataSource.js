// BindingList datasource example
//
// This implements a datasource that will fetch images from Bing's image search feature of the azure data
// marketplace, and then cache them in a Binding.List. It will fetch one batch, return, then fetch additional
// batches every 1000ms
//
// The Azure Marketplace requires the developer to subscribe to services and get an account key. Bing offers a free 
// trial subscription which can be used with this sample. For more details see:
// https://datamarket.azure.com/dataset/5BA839F1-12CE-4CCE-BF57-A49D98D29A44

(function () {

    var _devkey, _query;
    var _maxCount = 100;

    // Makes the http request for data and populates the specified array/list
    function getData(offset, store) {
        
        var requestStr = "https://api.datamarket.azure.com/Data.ashx/Bing/Search/Image"

        // Common request fields (required)
        + "?Query='" + _query + "'"
        + "&$format=json"
        + "&Market='en-us'"
        + "&Adult='Strict'"
        + "&$top=10"
        + "&$skip=" + offset;

        return WinJS.xhr({ url: requestStr, user: "foo", password: _devkey, headers: { DataServiceVersion: 2.0 } }).then(
            function (request) {
                var obj = JSON.parse(request.responseText);

                // Verify if the service has returned images
                if (obj.d && obj.d.results) {
                    var items = obj.d.results;

                    for (var i = 0, itemsLength = items.length; i < itemsLength; i++) {
                        var dataItem = items[i];
                        store.push({
                            title: dataItem.Title,
                            thumbnail: dataItem.Thumbnail.Url,
                            width: dataItem.Width,
                            height: dataItem.Height,
                            linkurl: dataItem.Url,
                            url: dataItem.MediaUrl
                        });
                    }
                    return (store.length < _maxCount);
                } else {
                    return false;
                };
            },
            function (request) {
                if (request.status === 401) {
                    WinJS.log && WinJS.log(request.statusText, "sample", "error");
                } else {
                    WinJS.log && WinJS.log(request.statusText + ". " + request.responseText, "sample", "error");
                }
                return false;
            }
        );
    }

    // Main entry point
    //
    // This function will request a batch of data and return a promise to the Binding.List datasource that will
    // complete when the first batch is done. It will then fetch the remaining records over time.
    //
    function getDataSource(devkey, query) {
        _devkey = devkey;
        _query = query;
        var list = new WinJS.Binding.List();

        return getData(0, list).then(function (hasMoreData) {
            var next = function (offset) {
                setTimeout(function () {
                    getData(offset, list).then(function (more) {
                        if (more) {
                            next(list.length);
                        }
                    });
                }, 1000);
            };
            if (hasMoreData) { next(list.length); }
            return list.dataSource;
        });
    }

    WinJS.Namespace.define("bingBindingListDataSource", {
        getDataSource: getDataSource
    });

})();
