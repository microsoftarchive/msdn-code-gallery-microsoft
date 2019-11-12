//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var renderer, dataSource;

    //Initialization for the scenario
    var page = WinJS.UI.Pages.define("/html/itemTemplatesAndDataSources.html", {
        processed: function (element, options) {
            // WinJS.UI.processAll() is automatically called by the Pages infrastructure by the time this
            // function gets called, and it has processed the div with data-win-control="WinJS.UI.FlipView"

            document.querySelector("#savekey").addEventListener("click", saveKey, false);
            document.querySelector("#renderer").addEventListener("change", updateRenderer, false);
            document.querySelector("#delay").addEventListener("change", updateDelay, false);
            document.querySelector("#virtDS").addEventListener("change", updateDatasource, false);
            document.querySelector("#bindinglist").addEventListener("change", updateDatasource, false);

            renderer = document.getElementById("itemTemplatesAndDataSources_FlipView_itemTemplate");
            loadKey();
            updateDatasource();
        }
    });

    //Will change which renderer is used for the application, based on the dropdown
    function updateRenderer() {
        var choice = document.querySelector("#renderer").value;

        switch (choice) {
            case "template":
                // Declarative template defined in the html
                renderer = document.getElementById("itemTemplatesAndDataSources_FlipView_itemTemplate");
                break;

            case "simplefunc":
                // Simple function for the renderer
                renderer = simpleRenderer;
                break;

            case "placeholder":
                //Simple rendering function that can return results before the data is available
                renderer = placeholderRenderer;
                break;

            case "recycling":
                //Evolution of the placeholder to be able to reuse elements
                renderer = recyclingPlaceholderRenderer;
                break;
        }
        updateDatasource();
    }

    //
    // Simple Renderer
    //
    // Blocks waiting for the content for the item to be updated
    //
    function simpleRenderer(itemPromise) {
        // wait for the itemPromise to complete to access the data
        return itemPromise.then(function (item) {
            // root element for the item
            var element = document.createElement("div");
            element.className = "itemTempl";
            // using innerHTML is usually faster than creating the DOM with createElement & appendChild
            element.innerHTML = "<img src='" + item.data.url + "' alt='Databound image' /><div class='content'>" + item.data.title + "</div>";
            return element;
        });
    }

    //
    // Placeholder renderer
    //
    // Does not wait for item data, it immediately returns back a placeholder for the item.
    // Then when item data is available it updates the html
    //
    function placeholderRenderer(itemPromise) {
        // create a basic template for the item which doesn't depend on the data
        var element = document.createElement("div");
        element.className = "itemTempl";
        element.innerHTML = "<div class='content'>Loading...</div>";

        // return the element as the placeholder, and a callback to update it when data is available
        return {
            element: element,
            // specifies a promise that will be completed when rendering is complete
            // itemPromise will complete when the data is available
            renderComplete: itemPromise.then(function (item) {
                // mutate the element to include the data
                element.querySelector(".content").innerText = item.data.title;
                element.insertAdjacentHTML("afterBegin", "<img src='" + item.data.url + "' alt='Databound image' />");
            })
        };
    }

    //
    // Recycling renderer with placeholders
    //
    // Extends the placeholder renderer to also support element recycling. The renderer
    // may be passed the markup from an item that is no longer on screen. The renderer
    // can then update the passed in DOM for its item, which can be quicker than
    // creating new html from scratch.
    //
    function recyclingPlaceholderRenderer(itemPromise, recycled) {
        var element, img, label;
        if (!recycled) {
            // create a basic template for the item which doesn't depend on the data
            element = document.createElement("div");
            element.className = "itemTempl";
            element.innerHTML = "<img alt='Databound image' style='visibility:hidden;'/><div class='content'>Loading...</div>";
        }
        else {
            // clean up the recycled element so that we can re-use it rather than having
            // to create the html from scratch
            element = recycled;
            label = element.querySelector(".content");
            label.innerHTML = "Loading...";
            img = element.querySelector("img");
            img.style.visibility = "hidden";
        }
        // return the element as the placeholder, and a callback to update it when data is available
        return {
            element: element,
            // specifies a promise that will be completed when rendering is complete
            // itemPromise will complete when the data is available
            renderComplete: itemPromise.then(function (item) {
                // mutate the element to include the data
                if (!label) {
                    label = element.querySelector(".content");
                    img = element.querySelector("img");
                }
                label.innerText = item.data.title;
                img.src = item.data.url;
                img.style.visibility = "visible";
            })
        };
    }

    //
    // Sample logic
    //
    // code to do the input UI for the sample
    //


    function updateDelay() {
        var delay = 50 * Math.pow(2, parseInt(document.getElementById("delay").value));
        document.getElementById("delayOutput").innerText = delay + "ms";
        updateDatasource();
        updateView();
    }

    var _list;
    function updateDatasource() {
        var devkey = document.getElementById("devkey").value;
        if (devkey !== undefined && devkey.length > 0) {
            var useVirtDS = document.getElementById("virtDS").checked;
            if (useVirtDS) {
                var delay = 50 * Math.pow(2, parseInt(document.getElementById("delay").value));
                dataSource = new bingImageSearchDataSource.datasource(devkey, "Seattle", delay);
                updateView();
            } else {
                if (!_list) {
                    bingBindingListDataSource.getDataSource(devkey, "New York").then(function (ds) {
                        _list = ds;
                        dataSource = _list;
                        updateView();
                    });
                } else {
                    dataSource = _list;
                    updateView();
                }
            }
        }
        else {
            dataSource = new WinJS.Binding.List().dataSource;
            updateView();
        }
    }

    // Save the api key in app state and toggle the control enablement
    function saveKey() {
        var devkey = document.getElementById("devkey").value;
        if (devkey.length > 0) {
            try {
                var localSettings = Windows.Storage.ApplicationData.current.localSettings;
                localSettings.values["devkey"] = devkey;
            }
            catch (err) {
                //do nothing;
            }
            toggleControls(true);
            updateDatasource();

        } else {
            toggleControls(false);
        }
    }

    // load the api key from app state
    function loadKey() {
        var devkey = "";

        try {
            var localSettings = Windows.Storage.ApplicationData.current.localSettings;
            devkey = localSettings.values["devkey"];
            if (devkey === undefined) {
                devkey = "";
            }
        }
        catch (err) {
            devkey = "";
        }
        if (devkey.length > 0) {
            document.getElementById("devkey").value = devkey;
            toggleControls(true);
        } else {
            toggleControls(false);
        }
    }

    // Toggles the enablement of the controls when we have a dev key
    function toggleControls(enabled) {
        document.getElementById("options").disabled = (!enabled);
        document.getElementById("output").disabled = (!enabled);
    }

    // Initializes the data adapter and pass to the flipview
    // Called when the search button is pressed.
    // The code for the data adapter is in js/BingImageSearchDataSource.js
    function updateView() {
        var flipviewDiv = document.getElementById("itemTemplatesAndDataSources_FlipView");
        var flipview = flipviewDiv.winControl;

        // Set the properties on the flipview to use the itemDataSource
        flipview.itemDataSource = dataSource;
        flipview.itemTemplate = renderer;

        dataSource.getCount().then(function (count) {
            if (count) {
                WinJS.log && WinJS.log("", "sample", "status");
                flipviewDiv.style.visibility = "visible";
            } else {
                WinJS.log && WinJS.log("Please enter a developer account key", "sample", "status");
                flipviewDiv.style.visibility = "hidden";
            }
        });
    }
})();
