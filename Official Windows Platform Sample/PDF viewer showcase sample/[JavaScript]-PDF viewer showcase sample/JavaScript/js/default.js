//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

    WinJS.Namespace.define("PdfShowcase", {
        loadedFile: null,
        zoomedInViewSource: null,
        zoomedOutViewSource: null
    });

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // The following code provides mapping of items between zoomedIn and zoomedOut view
                window.zoomedInItem = window.zoomedOutItem = WinJS.UI.eventHandler(function (item) {
                    var clone = Object.create(item);
                    clone.groupIndexHint = clone.firstItemIndexHint = item.index;
                    return clone;
                });

                args.setPromise(WinJS.UI.processAll().done(function () {

                    // Initialize app bar
                    initializeAppBar();

                    // Initialize application
                    initializeApp();

                    // Initializing data share source event handler for sharing PDF file
                    var dataTransferManager = Windows.ApplicationModel.DataTransfer.DataTransferManager.getForCurrentView();
                    dataTransferManager.addEventListener("datarequested", eventHandlerShareItem);

                }));
            }
        }
    };

    app.start();
})();

function initializeApp() {

    // Loading file from assets
    Windows.ApplicationModel.Package.current.installedLocation.getFileAsync("assets\\Sample.pdf").done(function load(file) {

        // Updating details of file currently loaded
        PdfShowcase.loadedFile = file;

        // Loading PDF file and trigger load of pages
        loadPDF(file);
    });
}

function loadPDF(file) {

    // Loading PDf file from the assets
    pdfLibrary.loadPDF(file).done(function (pdfDocument) {

        if (pdfDocument !== null) {
            // Initialize ZoomedInView control of Semantic Zoom
            initializeZoomedInView(pdfDocument);

            // Initialize ZoomedOutView control of Semantic Zoom
            initializeZoomedOutView(pdfDocument);
        }
    }, function error() {
        // Password protected file, user should provide a password to open the file
    });

};

function initializeZoomedInView(pdfDocument) {

    // Virtualized Data Source takes following arguments
    //  zoomedInListView:           element for zoomed out view
    //  pdfDocument:                PDF document returned by loadPDF
    //  pdfPageRenderingOptions:    null, will be initialized by Virtualized Data Source Constructor
    //  pageToLoad:                 number of pages to load on each request to Virtualized Data source itemFromIndex method
    //  inMemoryFlag:               false, all images are kept in memory 
    //  temporary folder:           null, not required as inMemoryFlag is set to false

    if (PdfShowcase.zoomedInViewSource !== null) {

        // Unloading currently loaded PDF file
        PdfShowcase.zoomedInViewSource.unload();
    }

    var zoomedInListView = document.getElementById("zoomedInListView");

    // Initializing control
    zoomedInListView.winControl.itemDataSource = null;
    zoomedInListView.winControl.layout = new WinJS.UI.GridLayout();

    var zoomedInViewSource = new PDF.dataAdapter.dataSource(zoomedInListView, pdfDocument, null, 5, true, null);

    //  Setting data source for element
    zoomedInListView.winControl.itemDataSource = zoomedInViewSource;

    PdfShowcase.zoomedInViewSource = zoomedInViewSource;

};

function initializeZoomedOutView(pdfDocument) {

    if (PdfShowcase.zoomedOutViewSource !== null) {

        // Unloading currently loaded PDF file
        PdfShowcase.zoomedOutViewSource.unload();
    }

    // Initializing page rendering options for zoomed out view
    var pdfPageRenderingOptions = new Windows.Data.Pdf.PdfPageRenderOptions;

    // High contrast will be honored by PDF
    pdfPageRenderingOptions.isIgnoringHighContrast = false;

    // Setting the height of thumbnail to fixed value 300px. 
    // We have chosen 300px as the size of thumbnail as it gives decent preview of a PDF page on all resolutions
    pdfPageRenderingOptions.destinationHeight = 300;

    WinJS.Application.local.folder.createFolderAsync("temp", Windows.Storage.CreationCollisionOption.replaceExisting).done(function (tempFolder) {
        // Virtualized Data Source takes following arguments
        //  zoomedOutListView:          element for zoomed out view
        //  pdfDocument:                PDF document returned by loadPDF
        //  pdfPageRenderingOptions:    page rendering option with height set to 300px for Zoomed Out view
        //  pageToLoad:                 number of pages to load on each request to Virtualized Data source itemFromIndex method
        //  inMemoryFlag:               true,all the thumbnails generated will be placed on disk
        //  temporary folder:           path on disk where these images will be kept

        var zoomedOutListView = document.getElementById("zoomedOutListView");

        // Initializing control
        zoomedOutListView.winControl.itemDataSource = null;
        zoomedOutListView.winControl.layout = new WinJS.UI.GridLayout();

        var zoomedOutViewSource = new PDF.dataAdapter.dataSource(zoomedOutListView, pdfDocument, pdfPageRenderingOptions, 5, false, tempFolder);

        // Setting data source for element
        zoomedOutListView.winControl.itemDataSource = zoomedOutViewSource;

        PdfShowcase.zoomedOutViewSource = zoomedOutViewSource;
    });
};

function eventHandlerShareItem(e) {
    if (PdfShowcase.loadedFile !== null) {
        var request = e.request;
        request.data.properties.title = "Sharing File";
        request.data.properties.description = PdfShowcase.loadedFile.name;
        request.data.setStorageItems([PdfShowcase.loadedFile]);
    }
};


// This function initializes app bar and its options
function initializeAppBar() {

    // Add event listeners to handle click of Open option in command bar
    document.getElementById("open").addEventListener("click", openClick, false);
}

function openClick() {

    // Launching file picker to pick PDF file for load
    var picker = new Windows.Storage.Pickers.FileOpenPicker();
    picker.fileTypeFilter.replaceAll([".pdf"]);
    picker.pickSingleFileAsync().done(function (file) {
        if (file !== null) {
            // Checking if file is already loaded
            if (PdfShowcase.loadedFile.path !== file.path) {

                // Updating details of loaded file
                PdfShowcase.loadedFile = file;

                // Loading file selected through file picker
                loadPDF(file);
            }
        }
    });
}

window.blobUriFromStream = WinJS.Binding.initializer(function (source, sourceProp, dest, destProp) {
    if (source[sourceProp] !== null) {
        dest[destProp] = URL.createObjectURL(source[sourceProp], { oneTimeOnly: true });
    }
});
