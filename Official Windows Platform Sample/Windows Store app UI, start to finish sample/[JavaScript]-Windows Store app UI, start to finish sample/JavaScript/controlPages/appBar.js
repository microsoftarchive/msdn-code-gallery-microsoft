/*
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
*/

// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";

    WinJS.UI.Pages.define("/controlPages/appBar.html", {

        processed: function (element) {

            return WinJS.Resources.processAll(element);
        },

        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            // TODO: Initialize the page here.

            // Bind the data item that describes this control. 
            WinJS.Binding.processAll(element, options.item);

            var commandsAppBar = element.querySelector("#commandsAppBar").winControl;
            commandsAppBar.getCommandById("cmdAdd").addEventListener("click", doClickAdd, false);
            commandsAppBar.getCommandById("cmdRemove").addEventListener("click", doClickRemove, false);
            commandsAppBar.getCommandById("cmdDelete").addEventListener("click", doClickDelete, false);
            commandsAppBar.getCommandById("cmdCamera").addEventListener("click", doClickCamera, false);


        },

        unload: function () {
            // TODO: Respond to navigations away from this page.
        },

        updateLayout: function (element) {

            // TODO: Respond to changes in layout.
        }
    });

    function doClickAdd(eventInfo){
        document.getElementById("outputDiv").innerText =  "Add button pressed."; 
    }

    function doClickRemove(eventInfo) {
        document.getElementById("outputDiv").innerText = "Remove button pressed.";
    }

    function doClickDelete(eventInfo) {
        document.getElementById("outputDiv").innerText = "Delete button pressed.";
    }
    function doClickCamera(eventInfo) {
        document.getElementById("outputDiv").innerText = "Camera button pressed.";
    }

    function showAppBar(eventInfo) {
        var appBar = document.getElementById("commandsAppBar").winControl;
        if (appBar.hidden)
            appBar.show();
        else
            appBar.hide();
    }

    WinJS.Namespace.define("AppBarExamples",
    { showAppBar: showAppBar });


})();
