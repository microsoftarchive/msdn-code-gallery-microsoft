//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // This is the variable that will be used to hold the data for the Table of Contents.
    var myTOCData;
    var myFlipView;

    var page = WinJS.UI.Pages.define("/html/interactiveContent.html", {
        processed: function (element, options) {
            // WinJS.UI.processAll() is automatically called by the Pages infrastructure by the time this
            // function gets called, and it has processed the div with data-win-control="WinJS.UI.FlipView"

            initialize();
        }
    });

    // initialize()
    //
    //      Purpose:
    //          To build the table of contents (TOC) we want to walk through
    //          the items in the dataSource and create links for them.  To
    //          simplify the process, the first item in the data source will
    //          duplicate the remaining items.  This makes it simple to
    //          constuct the TOC as well as template the remaining items.
    function initialize() {
        myFlipView = document.getElementById("interactiveContent_FlipView").winControl;

        // Attach Click Event Handlers to determine which item to navigate to.
        myFlipView.addEventListener("click", clickHandler, false);

        // Copy the data source and insert it as the first item in.
        var contents = { type: "contentsArray", contents: DefaultData.array };
        myTOCData = DefaultData.array.slice(0);
        myTOCData.splice(0, 0, contents);

        // Update Scenario 3 FlipView to use the custom template and the Table of
        // contents data source.
        myFlipView.itemTemplate = mytemplate;
        myFlipView.itemDataSource = new WinJS.Binding.List(myTOCData).dataSource;
    }

    //  mytemplate
    //
    //      Purpose: This function simply picks whether the Item or Table of
    //               Contents needs to be rendered and calls the appropriate
    //               function.
    function mytemplate(itemPromise) {
        return itemPromise.then(function (currentItem) {
            if (currentItem.data.type === "item") {
                return renderItem(currentItem.data);

            } else {
                return renderTableOfContents(currentItem.data, currentItem.key);
            }
        });
    }

    //  renderTableOfContents()
    //
    //      Purpose: This function is responsible for walking through an array
    //               of titles and constructing the table of contents.
    function renderTableOfContents(dataObject, index) {

        // Step 1) Create an element to hold the table of contents.
        var TableOfContents = document.createElement("div");
        TableOfContents.className = "TableOfContents";

        // Step 2) Create an element used to center the title and items.
        var TOCCenter = document.createElement("div");
        TOCCenter.className = "TOCCenter";

        var title = document.createElement("h2");
        title.className = "TOCTitle";
        title.innerText = "Table of Contents";

        TOCCenter.appendChild(title);

        // Step 3) Loop through the elements and add them to the Table.
        for (var i = 0, len = dataObject.contents.length; i < len; i++) {
            TOCCenter.appendChild(renderTOCItem(dataObject.contents[i].title, i));
        }
        TableOfContents.appendChild(TOCCenter);

        return TableOfContents;
    }

    function renderTOCItem(itemName, itemNumber) {

        // Create the container that will hold all of the sub-elements
        var itemContainer = document.createElement("button");
        itemContainer.value = itemNumber + 1;
        itemContainer.className = "itemContainer";

        // Create a div to add some color to the item
        var lightStrip = document.createElement("div");
        lightStrip.className = "lightStrip";

        // Create a simple anchor tag to make the titles interactive,
        // store the item number so that it can be retrieved onClick,
        // add set the class name for styling, and set the visible text.
        var TOCItem = document.createElement("p");
        TOCItem.className = "TOCItem";
        TOCItem.innerText = itemName;

        // Put all the pieces together
        itemContainer.appendChild(lightStrip);
        itemContainer.appendChild(TOCItem);

        return itemContainer;
    }

    // renderItem()
    //
    //      Purpose: This function takes the raw data and constructs the
    //               item that is used in the FlipView.  It creates items of
    //               the following form:
    //
    //                  <div id="Scenario3_ItemTemplate">
    //                      <div class="overlaidItemTemplate">
    //                          <img class="image" data-win-bind="src: picture" data-win-bind="alt: title" />
    //                          <div class="overlay">
    //                              <div class="ItemTitle" data-win-bind="innerText: title"></div>
    //                          </div>
    //                      </div>
    //                  </div>
    function renderItem(dataObject) {

        // Create the Item Title div
        var itemTitle = document.createElement("h2");
        itemTitle.className = "overlayElement ItemTitle";
        itemTitle.innerText = dataObject.title;

        // Create the Item Title div
        var TOC = document.createElement("a");
        TOC.className = "overlayElement win-type-xx-small returnTOC";
        TOC.innerText = "Click to return to the Table of Contents.";

        // Create the Overlay div
        var overlay = document.createElement("div");
        overlay.className = "overlayElement overlay";
        overlay.appendChild(itemTitle);
        overlay.appendChild(TOC);

        // Create the Image div
        var image = document.createElement("img");
        image.className = "image";
        image.src = dataObject.picture;
        image.alt = dataObject.title;

        // Create the overlaidItemTemplate div
        var overlaidItemTemplate = document.createElement("div");
        overlaidItemTemplate.className = "overlaidItemTemplate";
        overlaidItemTemplate.appendChild(image);
        overlaidItemTemplate.appendChild(overlay);

        return overlaidItemTemplate;
    }

    // clickHandler
    //
    //      Purpose: This event handler is responsible for trigger navigation
    //               to the item that was clicked on in the table of contents
    //               or navigating back to the table of contents when the link
    //               is clicked on a item.
    function clickHandler(evt) {

        // First check if the source of the event is an item in the table of
        // contents
        if (WinJS.Utilities.hasClass(evt.srcElement, "itemContainer")) {

            // Since we are navigating to an item, we need to retrieve the
            // number of the page and set the currentPage on the FlipView.
            myFlipView.currentPage = parseInt(evt.srcElement.value);

        } else if (WinJS.Utilities.hasClass(evt.srcElement, "returnTOC")) {

            // Since we know that we are returning to the table of contents,
            // simply set the currentPage to trigger the navigation.
            myFlipView.currentPage = 0;
        }
    }
})();
