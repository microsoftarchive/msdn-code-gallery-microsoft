//// Copyright (c) Microsoft Corporation. All rights reserved

// PDF Helper Library
// This library contains functions implemented using API's in the Windows.Data.Pdf namespace
// Methods in this library are invoked by both Virtualized data source and app initialize for loading pdf file

pdfLibrary = {

    // This method loads the pdf file
    //  Arguments:
    //      fileName:   path of the file to be loaded
    //  Return Values:
    //      pdfDocument: initialized pdf document object
    loadPDF: function (file) {
        "use strict";
        return file.openReadAsync().then(function (stream) {
            return Windows.Data.Pdf.PdfDocument.loadFromStreamAsync(stream);
        });
    },
    // This method loads pdf pages in the given range
    //  Arguments:
    //      startIndex:                 start index from the range of pages from the pdf file to be loaded
    //      endIndex:                   end index from the range of pages from the pdf file to be loaded
    //      pdfDocument:                pdf document object
    //      pdfPageRenderingOptions:    rendering options to be specified for a page    
    //      inMemory:                   boolean, whether to load the page in memory or on disk    
    //      tempFolder:                 path where images corresponding to pdf pages are kept on disk. This option is used
    //                                  only when inMemory flag is false
    //  Return Values:
    //      pageIndex:                  Array of index of the rendered page                          
    //      imageSrc:                   Array of Object URL of the image corresponding to pageIndex  array

    loadPages: function (startIndex, endIndex, pdfDocument, pdfPageRenderingOptions, inMemoryFlag, tempFolder) {
        var promiseArray = [];
        for (var count = startIndex; count < endIndex; count++) {
            //In memory flag will be false in case of Thumbnail view (zoomed out view). In thumbnail view
            // images are generated in temporary folder and hence to avoid their regeneration we are checking whether
            // the view is thumbnail view and if the image is not yet generated then only generate the image
            var promise;

            if (inMemoryFlag || this._dataArray[count].imageSrc === "") {
                promise = pdfLibrary.loadPage(count, pdfDocument, pdfPageRenderingOptions, inMemoryFlag, tempFolder).then(function (pageData) {
                    return pageData;
                });
            } else {
                // Else pass the already generated image data
                promise = WinJS.Promise.wrap(this._dataArray[count]);
            }

            promiseArray.push(promise);
        }
        return WinJS.Promise.join(promiseArray);
    },

    // This method is invoked internally by loadPages to load individual page
    //  Arguments:
    //      pageIndex:                  index of page from the pdf file to be rendered
    //      pdfDocument:                pdf document object
    //      pdfPageRenderingOptions:    rendering options to be specified for a page    
    //      inMemory:                   boolean, whether to load the page in memory or on disk    
    //      tempFolder:                 path where images corresponding to pdf pages are kept on disk. This option is used
    //                                  only when inMemory flag is false
    //  Return Values:
    //      pageIndex:                  index of the rendered page
    //      imageSrc:                   Object URL of the image corresponding to pageIndex


    loadPage: function (pageIndex, pdfDocument, pdfPageRenderingOptions, inMemoryFlag, tempFolder) {
        var filePointer = null;

        var promise = null;
        if (inMemoryFlag) {
            promise = WinJS.Promise.wrap(new Windows.Storage.Streams.InMemoryRandomAccessStream());
        } else {
            // Creating file on disk to store the rendered image for a page on disk
            // This image will be stored in the temporary folder provided during VDS init
            var filename = this.randomFileName() + ".png";
            var file = null;
            promise = tempFolder.createFileAsync(filename,
            Windows.Storage.CreationCollisionOption.replaceExisting).then(function (filePtr) {
                filePointer = filePtr;
                return filePointer.openAsync(Windows.Storage.FileAccessMode.readWrite);
            }, function (error) {
                // Error while opening a file
                filePointer = null;
            }, function (error) {
                // Error while creating a file
            });
        }
        return promise.then(function (imageStream) {
            var pdfPage = pdfDocument.getPage(pageIndex);
            return pdfPage.renderToStreamAsync(imageStream, pdfPageRenderingOptions).then(function () {
                return imageStream.flushAsync();
            })
            .then(function closeStream() {
                var picURL = null;
                if (inMemoryFlag) {
                    var renderStream = Windows.Storage.Streams.RandomAccessStreamReference.createFromStream(imageStream);
                    return renderStream.openReadAsync().then(function (stream) {

                        // Cleaning the objects
                        imageStream.close();
                        pdfPage.close();

                        return { pageIndex: pageIndex, imageSrc: stream };

                    });
                } else {
                    // Cleaning the objects
                    imageStream.close();
                    pdfPage.close();

                    return { pageIndex: pageIndex, imageSrc: filePointer };
                }
            });
        });
    },

    // Utility functions to create random string used as a file name
    randomString: function () {
        return Math.floor((1 + Math.random()) * 0x10000)
                   .toString(16)
                   .substring(1);
    },

    randomFileName: function () {
        return this.randomString() + this.randomString() + '-' + this.randomString() + '-' + this.randomString() + '-' +
                this.randomString() + '-' + this.randomString() + this.randomString() + this.randomString();
}
};