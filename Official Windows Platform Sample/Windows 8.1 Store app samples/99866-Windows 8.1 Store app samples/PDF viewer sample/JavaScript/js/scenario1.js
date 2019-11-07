//// Copyright (c) Microsoft Corporation. All rights reserved

var pdfLib = Windows.Data.Pdf;

var PDF_PAGE_INDEX = 0; // First Page
var ZOOM_FACTOR = 2; // 200% Zoom
var PDF_PORTION_RECT = {height:400, width:300, x:100, y:200}; // Portion of a page
var PDFFILENAME = "Assets\\Windows_7_Product_Guide.pdf";

var RENDEROPTIONS = {
    NORMAL: 0,
    ZOOM: 1,
    PORTION: 2
};

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            document.getElementById("RenderPage").addEventListener("click", renderPage_click, false);
            document.getElementById("RenderPageAtZoom").addEventListener("click", renderPageAtZoom_click, false);
            document.getElementById("RenderPortionOfPage").addEventListener("click", renderPortionPage_click, false);
        }
    });

  
    function renderPage_click() {
        renderPDFPage(PDFFILENAME,PDF_PAGE_INDEX,RENDEROPTIONS.NORMAL);
    }

    function renderPageAtZoom_click() {
        renderPDFPage(PDFFILENAME, PDF_PAGE_INDEX, RENDEROPTIONS.ZOOM);
    }
    function renderPortionPage_click() {
        renderPDFPage(PDFFILENAME, PDF_PAGE_INDEX, RENDEROPTIONS.PORTION);
    }

})();

   function renderPDFPage(pdfFileName,pageIndex, renderOptions) {
       "use strict";
             Windows.ApplicationModel.Package.current.installedLocation.getFileAsync(pdfFileName).then(function loadDocument(file) {
                // Call pdfDocument.'loadfromFileAsync' to load pdf file
                 return pdfLib.PdfDocument.loadFromFileAsync(file);
                }).then(function setPDFDoc(doc) {
                   renderPage(doc, pageIndex, renderOptions);
                });          
        };

   function renderPage(pdfDocument, pageIndex, renderOptions) {
       "use strict";
       var pageRenderOutputStream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
          
       
       // Get PDF Page
       var pdfPage = pdfDocument.getPage(pageIndex);
           
       var pdfPageRenderOptions = new Windows.Data.Pdf.PdfPageRenderOptions();
       var renderToStreamPromise;
       var pagesize = pdfPage.size;         
           
       // Call pdfPage.renderToStreamAsync
       switch (renderOptions) {
           case RENDEROPTIONS.NORMAL:
               renderToStreamPromise = pdfPage.renderToStreamAsync(pageRenderOutputStream);
               break;
           case RENDEROPTIONS.ZOOM:
               // Set pdfPageRenderOptions.'destinationwidth' or 'destinationHeight' to take into effect zoom factor
               pdfPageRenderOptions.destinationHeight = pagesize.height * ZOOM_FACTOR;
               renderToStreamPromise = pdfPage.renderToStreamAsync(pageRenderOutputStream, pdfPageRenderOptions);
               break;
           case RENDEROPTIONS.PORTION:
               // Set pdfPageRenderOptions.'sourceRect' to the rectangle containing portion to show
               pdfPageRenderOptions.sourceRect = PDF_PORTION_RECT;
               renderToStreamPromise = pdfPage.renderToStreamAsync(pageRenderOutputStream, pdfPageRenderOptions);
               break;
       };      
      
       renderToStreamPromise.then(function Flush() {
           return pageRenderOutputStream.flushAsync();
       }).then(function DisplayImage() {
           if (pageRenderOutputStream !== null) {
               // Get Stream pointer
               var blob = MSApp.createBlobFromRandomAccessStream("image/png", pageRenderOutputStream);
               var picURL = URL.createObjectURL(blob, { oneTimeOnly: true });
               scenario1ImageHolder1.src = picURL;
               pageRenderOutputStream.close();
               blob.msClose();
           };
       },
          function error() {
              if (pageRenderOutputStream !== null) {
                  pageRenderOutputStream.close();

              }
          });
   }
   

   
   


