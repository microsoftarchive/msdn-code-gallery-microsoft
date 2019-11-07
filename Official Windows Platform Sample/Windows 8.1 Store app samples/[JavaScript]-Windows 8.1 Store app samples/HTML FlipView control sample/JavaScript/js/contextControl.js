//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var myFlipview = null;

    var page = WinJS.UI.Pages.define("/html/contextControl.html", {
        processed: function (element, options) {
            // WinJS.UI.processAll() is automatically called by the Pages infrastructure by the time this
            // function gets called, and it has processed the div with data-win-control="WinJS.UI.FlipView"
            myFlipview = document.getElementById("contextControl_FlipView").winControl;

            // A context control needs to know the count of items in the flipper,
            // so in order to find that we call flipper.count. The object passed in
            // should contain a success expando property that contains the function
            // to be called when a count is available. An optional property, error,
            // can also be specified which will be called back for erroneous cases.
            // For code brevity this property is left out of this sample.
            myFlipview.count().done(countRetrieved);
        }
    });

    // countRetrieved()
    //
    //      Purpose:
    //              The function is passed to FlipView.count() and will be
    //              called once the count is available and when it changes.
    //              This will be the trigger to setup/change the context
    //              control.  The context control we are building is based
    //              on styling radio buttons.
    function countRetrieved(count) {

        // Step 1) Create an element to host the sub-elements in the context
        //         control.
        var contextControl = document.createElement("div");
        contextControl.className = "contextControl";

        // Step 2)  Create the event handlers for the radio buttons.  It's
        //          purpose is to handle click and translate that into
        //          flipping the FlipView to the correct page.
        var isFlipping = false;
        function radioButtonClicked(eventObject) {
            if (isFlipping) {

                // Need to set whats check back since we are mid flip.
                var currentPage = myFlipview.currentPage;
                radioButtons[currentPage].checked = true;

            } else {

                // Set the new page since we are not already flipping.
                var targetPage = eventObject.srcElement.getAttribute("value");
                myFlipview.currentPage = parseInt(targetPage, 10);

            }
        }

        // Step 3) Detect when the FlipView starts flipping to prevent
        //         subsequent clicks on the contextControl until flipping has
        //         stopped.
        myFlipview.addEventListener("pagevisibilitychanged", function (eventObject) {
            if (eventObject.detail.visible === true) {
                isFlipping = true;
            }
        }, false);

        // Step 4)  Create radio buttons for each page in the FlipView and
        //          add the event handler from step 3.
        var radioButtons = [];
        for (var i = 0; i < count; ++i) {

            // Create a Radio Button
            var radioButton = document.createElement("input");
            radioButton.setAttribute("type", "radio");

            // Set the name of the radio button group.
            radioButton.setAttribute("name", "flipperContextGroup");

            // Set the value for each button to the number of the page.
            radioButton.setAttribute("value", i);

            // Set the accessibility label
            radioButton.setAttribute("aria-label", (i + 1) + " of " + count);

            // Add the handler
            radioButton.onclick = radioButtonClicked;

            // Add the buttons to our radioButtons collection
            radioButtons.push(radioButton);

            // Add the radio buttons to the main container for the context control.
            contextControl.appendChild(radioButton);
        }


        // Step 5)  Set the currently checked item to the item the FlipView is
        //          currently on.
        if (count > 0) {
            radioButtons[myFlipview.currentPage].checked = true;
        }

        // Step 6)  The context control needs to listen to FlipView events so
        //          that it can display what is currently in view.
        myFlipview.addEventListener("pageselected", function () {

            // Now that flipping is done, clear the flag.
            isFlipping = false;

            // Need to set the current page.
            var currentPage = myFlipview.currentPage;
            radioButtons[currentPage].checked = true;
        }, false);

        // Step 7)  Finally, we need to add the control into the DOM.
        var contextContainer = document.getElementById("ContextContainer");
        contextContainer.appendChild(contextControl);
    }
})();
