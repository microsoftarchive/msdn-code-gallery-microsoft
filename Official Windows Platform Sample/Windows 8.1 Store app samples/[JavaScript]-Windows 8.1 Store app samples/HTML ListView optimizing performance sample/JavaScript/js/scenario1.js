//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var renderer, dataSource;

    //Will change which renderer is used for the application, based on the dropdown
    function updateRenderer() {
        var choice = document.querySelector("#renderer").value;

        switch (choice) {
            case "template":
                // Declarative template defined in the html
                renderer = document.getElementById("itemTemplate");
                break;

            case "simplefunc":
                // Simple function for the renderer
                renderer = simpleRenderer;
                break;

            case "placeholder":
                //Simple rendering function that can return results before the data is available
                renderer = placeholderRenderer;
                break;

            case "multistage":
                //Renderer that will queue the fetching of images
                renderer = multistageRenderer;
                break;

            case "batched":
                thumbnailBatch = createBatch();
                renderer = batchRenderer;
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
            element.innerHTML = "<img src='" + item.data.thumbnail + "' alt='Databound image' /><div class='content'>" + item.data.title + "</div>";
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
        element.innerHTML = "<div class='content'>...</div>";

        // return the element as the placeholder, and a callback to update it when data is available
        return {
            element: element,
            // specifies a promise that will be completed when rendering is complete
            // itemPromise will complete when the data is available
            renderComplete: itemPromise.then(function (item) {
                // mutate the element to include the data
                element.querySelector(".content").innerText = item.data.title;
                element.insertAdjacentHTML("afterBegin", "<img src='" + item.data.thumbnail + "' alt='Databound image' />");
            })
        };
    }

    //
    // Multi-stage renderer
    //
    // Renderer to delay load the images. Some parts of the view may be expensive
    // to create, so the renderer can be written to update the view in stages.
    //
    // The item has an additional promise "item.ready" which indicates the time to do more expensive
    // work such as loading images, svg, or creating controls.
    //
    // To assist with queuing images, the promise has an image loader to queue downloads, that can be
    // cancelled, and a function to check if the item is on screen.
    //
    // The renderComplete promise will be cancelled if the item is scrolled far out of the view. The cancel
    // will bubble up the promise chain, cancelling work in progress and not starting new work.
    //
    function multistageRenderer(itemPromise) {
        var element, img, label; 
        // create a basic template for the item which doesn't depend on the data
        element = document.createElement("div");
        element.className = "itemTempl";
        element.innerHTML = "<img alt='Databound image' style='opacity:0;'/><div class='content'>...</div>";
        img = element.querySelector("img");
        label = element.querySelector(".content");
        label.innerHTML = "...";
       
        // return the element as the placeholder, and a callback to update it when data is available
        return {
            element: element,
            // specifies a promise that will be completed when rendering is complete
            // itemPromise will complete when the data is available
            renderComplete: itemPromise.then(function (item) {
                // mutate the element to update only the title
                if (!label) { label = element.querySelector(".content"); }
                label.innerText = item.data.title;
                // use the item.ready promise to delay the more expensive work
                return item.ready;
                // use the ability to chain promises, to enable work to be cancelled
            }).then(function (item) {
                return item.loadImage(item.data.thumbnail, img).then(function () {
                    //once loaded check if the item is visible
                    return item.isOnScreen();
                });
            }).then(function (onscreen) {
                if (!onscreen) {
                    //if the item is not visible, then don't animate its opacity
                    img.style.opacity = 1;
                } else {
                    //if the item is visible then animate the opacity of the image
                    WinJS.UI.Animation.fadeIn(img);
                }
            }).then(null, function (err) {
                //if an error occurs, and its not a cancellation, then show a placeholder
                if (err.name === "Canceled") {
                    return WinJS.Promise.wrapError(err);
                }
                img.src = "/images/tile-sdk.png";
                img.style.opacity = 1;
                return;
            })
        };
    }


    //
    // Multi-stage + batching renderer
    //
    // Extends the multistage renderer to delay inserting the images. The images are inserted
    // as a batch so that the dom layout and repaint work is minimized.
    //
    function batchRenderer(itemPromise) {
        var element, label, item, img;
        // create a basic template for the item which doesn't depend on the data
        element = document.createElement("div");
        element.className = "itemTempl";
        element.innerHTML = "<div class='content'>...</div>";
        label = element.querySelector(".content");
        label.innerHTML = "...";
       
        // return the element as the placeholder, and a callback to update it when data is available
        return {
            element: element,
            // specifies a promise that will be completed when rendering is complete
            // itemPromise will complete when the data is available
            renderComplete: itemPromise.then(function (i) {
                item = i;
                // mutate the element to update only the title
                if (!label) { label = element.querySelector(".content"); }
                label.innerText = item.data.title;
                // use the item.ready promise to delay the more expensive work
                return item.ready;
                // use the ability to chain promises, to enable work to be cancelled
            }).then(function () {
                //use the image loader to queue the loading of the image
                return item.loadImage(item.data.thumbnail);
            }).then(thumbnailBatch()
            ).then(function (newimg) {
                img = newimg;
                element.insertBefore(img, element.firstElementChild);
                return item.isOnScreen();
            }).then(function (onscreen) {
                if (!onscreen) {
                    //if the item is not visible, then don't animate its opacity
                    img.style.opacity = 1;
                } else {
                    //if the item is visible then animate the opacity of the image
                    WinJS.UI.Animation.fadeIn(img);
                }
            }).then(null, function (err) {
                //if an error occurs, and its not a cancellation, then show a placeholder
                if (err.name === "Canceled") {
                    return WinJS.Promise.wrapError(err);
                } else {
                    img = element.querySelector("img");
                    if (img) {
                        img.src = "/images/placeholder-sdk.png";
                        img.style.opacity = 1;
                    }
                    return;
                }
            })
        };
    }

    //
    // Batch based actions
    //

    function createBatch(waitPeriod) {
        var batchTimeout = WinJS.Promise.as();
        var batcheItems = [];

        function completeBatch() {
            var callbacks = batcheItems;
            batcheItems = [];

            for (var i = 0; i < callbacks.length; i++) {
                callbacks[i]();
            }
        }

        return function () {
            batchTimeout.cancel();
            batchTimeout = WinJS.Promise.timeout(waitPeriod || 64).then(completeBatch);

            var delayedPromise = new WinJS.Promise(function (c) {
                batcheItems.push(c);
            });
            return function (v) { return delayedPromise.then(function () { return v; }); };
        };
    }
    var thumbnailBatch;


    //
    // Sample logic
    //
    // code to do the input UI for the sample
    //


    function updateDelay() {
        var delay = 50 * Math.pow(2, parseInt(document.getElementById("delay").value));
        document.getElementById("delayOutput").innerText = delay + "ms";
        updateDatasource();
    }

    var rule;
    function updateSize() {
        var size = parseInt(document.getElementById("size").value);
        var sizecss = "width: " + (size * 1.4) + "px; height: " + size + "px;";
        if (!rule) {
            var sheet = document.getElementById("style1").sheet;
            sheet.addRule(".itemTempl", sizecss);
            var rules = sheet.rules;
            rule = rules[rules.length - 1];
        } else {
            rule.style.cssText = sizecss;
        }

        //force the listview to update based on the item size
        document.getElementById("listview1").winControl.forceLayout();
    }

    var _list;
    function updateDatasource() {
        var devkey = document.getElementById("devkey").value;
        if (devkey !== undefined && devkey.length > 12) {
            var useVirtDS = document.getElementById("virtDS").checked;
            if (useVirtDS) {
                var delay = 50 * Math.pow(2, parseInt(document.getElementById("delay").value));
                dataSource = new bing.datasource(devkey, "Seattle", delay);
                updateView();
            } else {
                if (!_list) {
                    bindingDS.getDataSource(devkey, "New York").done(function (ds) {
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
        if (devkey.length > 12) {
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
        if (devkey.length > 12) {
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

    // Initializes the data adapter and pass to the listview
    // Called when the search button is pressed.
    // The code for the data adapter is in js/BingImageSearchDataSource.js
    function updateView() {
        var listview = document.getElementById("listview1").winControl;

        // Set the properties on the list view to use the itemDataSource
        listview.itemDataSource = dataSource;
        listview.itemTemplate = renderer;
    }

    //Initialization for the scenario
    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            document.querySelector("#savekey").addEventListener("click", saveKey, false);
            document.querySelector("#renderer").addEventListener("change", updateRenderer, false);
            document.querySelector("#delay").addEventListener("change", updateDelay, false);
            document.querySelector("#size").addEventListener("change", updateSize, false);
            document.querySelector("#virtDS").addEventListener("change", updateDatasource, false);
            document.querySelector("#bindinglist").addEventListener("change", updateDatasource, false);

            renderer = document.getElementById("itemTemplate");
            loadKey();
            updateDatasource();
        }
    });

})();
