// BindingList datasource example
//
// This implements a datasource that will fetch images from Bing's image search feature and then
// cache them in a binding.list. It will fetch one batch, return, then fetch additional batches
// every 100ms
//
//
// This implements a datasource that will fetch images from Bing's image search feature of the azure data marketplace.
// The Azure Marketplace requires the developer to subscribe to services and get an account key. Bing offers a free 
// trial subscription which can be used with this sample. For more details see:
// https://datamarket.azure.com/dataset/5BA839F1-12CE-4CCE-BF57-A49D98D29A44

(function () {

    var _devkey, _query;
    var _list;

    // Makes the http request for data and populates the specified array/list
    function getData(offset, store) {
        var requestStr = "https://api.datamarket.azure.com/Data.ashx/Bing/Search/Image"
            + "?Query='" + _query + "'"
            + "&$format=json"
            + "&Market='en-us'"
            + "&Adult='Strict'"
            + "&$top=" + 50
            + "&$skip=" + offset;

        return WinJS.xhr({ url: requestStr, user: "foo", password: _devkey, }).then(function (request) {
            var obj = JSON.parse(request.responseText);

            if (obj.d && obj.d.results) {
                var items = obj.d.results;

                for (var i = 0, itemsLength = items.length; i < itemsLength; i++) {
                    var dataItem = items[i];
                    store.push({
                        title: dataItem.Title,
                        thumbnail: dataItem.Thumbnail.MediaUrl,
                        width: dataItem.Width,
                        height: dataItem.Height,
                        linkurl: dataItem.SourceUrl
                    });
                }
                return (offset + items.length) < 1000;
            } else {
                return false;
            };
        }, function (request) {
            if (request.status === 401) {
                WinJS.log && WinJS.log(request.statusText, "sample", "error");
            } else {
                WinJS.log && WinJS.log("Error fetching data from the service. " + request.responseText, "sample", "error");
            }
        });
    }

    // Main entry point
    //
    // Will will request a batch of data and return a promise to the binding.list datasource that will
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
                }, 200);
            };
            if (hasMoreData) { next(list.length); }
            return list.dataSource;
        });
    }

    WinJS.Namespace.define("bindingDS", {
        getDataSource: getDataSource
    });

})();
