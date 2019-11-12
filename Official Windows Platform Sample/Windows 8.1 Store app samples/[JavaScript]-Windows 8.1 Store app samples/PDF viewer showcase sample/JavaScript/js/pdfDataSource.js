//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    WinJS.Namespace.define("PDF", {
        dataAdapter: WinJS.Class.define(function (element, pdfDocument, pdfPageRenderingOptions, pagesToLoad, inMemoryFlag, tempFolder) {
            if ((pdfDocument === null) || (element === null)) {
                throw "Invalid data";
            }
            this._element = element;
            this._pdfDocument = pdfDocument;
            this._pdfPageRenderingOptions = pdfPageRenderingOptions;
            this._pagesToLoad = pagesToLoad;
            this._inMemoryFlag = inMemoryFlag;
            this._tempFolder = tempFolder;

            // Initialize this data source
            this._initialize();
        },
// Private members
{
    _dataArray: null,               // This object stores the URL's for the pages rendered using PDF API's
    _pdfDocument: null,             // Object returned by loadPDF 
    _tempFolder: null,              // Temporary location for storing images on disk
    _pageCount: 0,                  // Number of pages in a given PDF file
    _pagesToLoad: 0,                // Number of pages to load on each request to Virtualized Data Source

    // Private member functions
    _initialize: function () {

        this._dataArray = [];

        // Array to maintain asynchronous requests created
        this._promiseArray = [];

        // Setting page count
        this._pageCount = this._pdfDocument.pageCount;

        // Set Item dimensions
        this._setItemDimensions();

        // Initialize data source with placeholder objects
        this._initializeDataArray();
    },

    // This method initializes the pdfPageRendering options if not passed
    _setItemDimensions: function () {

        // Get page dimensions of the first page in the PDF file
        // pdfRenderingOptions will be initialized using these dimensions and will be used for rendering
        // of all pages in a PDF file
        var pdfPage = this._pdfDocument.getPage(0);

        if (this._pdfPageRenderingOptions === null) {

            this._pdfPageRenderingOptions = new Windows.Data.Pdf.PdfPageRenderOptions;

            // High contrast will be honored by PDF API
            this._pdfPageRenderingOptions.isIgnoringHighContrast = false;

            var element = document.getElementById("pdfitemmainviewdiv");
            if (window.innerHeight > window.innerWidth) {
                if (element) {
                    element.style.width = window.innerWidth.toString() + "px";
                    element.style.height = Math.floor((window.innerWidth / pdfPage.size.width) * pdfPage.size.height).toString() + "px";
                }
                this._pdfPageRenderingOptions.destinationWidth = window.innerWidth;
                this._pdfPageRenderingOptions.destinationHeight = Math.floor((window.innerWidth / pdfPage.size.width) * pdfPage.size.height);
            } else {
                if (element) {
                    element.style.height = window.innerHeight.toString() + "px";
                    element.style.width = Math.floor((window.innerHeight / pdfPage.size.height) * pdfPage.size.width).toString() + "px";
                }
                this._pdfPageRenderingOptions.destinationWidth = Math.floor((window.innerHeight / pdfPage.size.height) * pdfPage.size.width);
                this._pdfPageRenderingOptions.destinationHeight = window.innerHeight;
            }
        } else {

            var elementSZ = document.getElementById("pdfSZViewTemplate");
            if (elementSZ) {
                elementSZ.style.width = Math.floor(((this._pdfPageRenderingOptions.destinationHeight / pdfPage.size.height) * pdfPage.size.width)).toString() + "px";
                elementSZ.style.height = (this._pdfPageRenderingOptions.destinationHeight).toString() + "px";
            }
        }
    },

    // This method initializes the _dataArray
    _initializeDataArray: function () {
        var that = this;
        this._dataArray = [];
        for (var count = 0, len = that._pageCount; count < len; count++) {
            this._dataArray.push({ pageIndex: count, imageSrc: "" });
        }
    },

    // This method invokes PDF API's to get the required pages from the PDF file
    //   startIndex:    start page index
    //   endIndex:      end page index
    //   It returns a promise which is completed when loadPages is completed
    _loadPages: function (startIndex, endIndex) {
        var that = this;
        startIndex = Math.max(0, startIndex);
        endIndex = Math.min(this._pageCount, endIndex);

        var promise = this.loadPages(startIndex, endIndex, this._pdfDocument, this._pdfPageRenderingOptions, this._inMemoryFlag, this._tempFolder).then(function (pageDataArray) {
            for (var i = 0, len = pageDataArray.length; i < len; i++) {
                var index = pageDataArray[i].pageIndex;
                that._dataArray[index].imageSrc = pageDataArray[i].imageSrc;
            }
        });
 
        that._promiseArray.push(promise);

        return promise;
    },

    // This method is invoked to unload currently loaded PDF file
    unload: function () {

        // Cancelling all promises
        for (var i = 0, len = this._promiseArray.length; i < len; i++) {

            this._promiseArray[i].cancel();
        }

        this._promiseArray = null;

        this._dataArray = null;

    },

    // Called to get a count of the items
    // The value of the count can be updated later in the response to itemsFromIndex
    getCount: function () {
        var that = this;
        return WinJS.Promise.wrap(that._pageCount);
    },

    // Called by the virtualized data source to fetch items
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
        if (requestIndex >= that._pageCount) {
            return WinJS.Promise.wrapError(new WinJS.ErrorFromName(WinJS.UI.FetchError.doesNotExist));
        }
        var results = [];

        var fetchIndex = Math.max(requestIndex - countBefore, 0);
        var lastFetchIndex = Math.min(requestIndex + that._pagesToLoad, that._pageCount);

        // Return the promise which is completed when all the requested pages are loaded
        return this._loadPages(fetchIndex, lastFetchIndex + 1).then(function () {
            // Data adapter results needs an array of items of the shape:
            // items =[{ key: key1, data : { field1: value, field2: value, ... }}, { key: key2, data : {...}}, ...];
            // Form the array of results objects
            for (var i = fetchIndex, itemsLength = lastFetchIndex ; i < itemsLength; i++) {
                var dataItem = that._dataArray[i];
                results.push({
                    key: i.toString(),
                    data: {
                        imageSrc: dataItem.imageSrc,
                    }
                });

                // If this is zoomed in view, remove the reference to the stream so that it will get collected
                // by GC. This is not applicable for thumb nail view as we are cacheing the entries for thumbnail view
                // on disc
                if (that._inMemoryFlag) {
                    that._dataArray[i].imageSrc = "";
                }
            }

            return WinJS.Promise.wrap({
                items: results, // The array of items.
                offset: requestIndex - fetchIndex, // The index of the requested item in the items array.
                totalCount: that._pageCount // The total number of records. This is equal to total number of pages in a PDF file
            });
        });
    },

    // setNotificationHandler: not implemented
    // itemsFromStart: not implemented
    // itemsFromEnd: not implemented
    // itemsFromKey: not implemented
    // itemsFromDescription: not implemented
}),

    });
    WinJS.Namespace.define("PDF.dataAdapter", {
        dataSource: WinJS.Class.derive(WinJS.UI.VirtualizedDataSource, function (element, pdfDocument, pdfPageRenderingOptions, pagesToLoad, inMemoryFlag, tempFolder) {
            this._dataAdapter = new PDF.dataAdapter(element, pdfDocument, pdfPageRenderingOptions, pagesToLoad, inMemoryFlag, tempFolder);
            this._baseDataSourceConstructor(this._dataAdapter, { cacheSize: 10 });
        }, {
            unload: function () {
                this._dataAdapter.unload();
            }
        })
    });

    // Event mixin to access PDF library functions in Virtualized Data Source Class
    WinJS.Class.mix(PDF.dataAdapter, pdfLibrary);
}());