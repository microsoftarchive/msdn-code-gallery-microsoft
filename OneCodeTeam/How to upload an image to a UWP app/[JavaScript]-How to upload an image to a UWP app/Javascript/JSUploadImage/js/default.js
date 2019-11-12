/****************************** Module Header ******************************\
 * Module Name:  default.js
 * Project:      JSUploadImage
 * Copyright (c) Microsoft Corporation.
 * 
 * An application that selects a local image and uploads onto the app
 *  
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

(function () {
	"use strict";

	var app = WinJS.Application;
	var activation = Windows.ApplicationModel.Activation;

	app.onactivated = function (args) {
		if (args.detail.kind === activation.ActivationKind.launch) {
			if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
				// TODO: This application has been newly launched. Initialize your application here.
			} else {
				// TODO: This application was suspended and then terminated.
				// To create a smooth user experience, restore application state here so that it looks like the app never stopped running.
			}
			args.setPromise(WinJS.UI.processAll());

		    //retrieve button and register event handler
			var uploadButton = document.getElementById("uploadButton");
			uploadButton.addEventListener("click", buttonClickHandler, false)
		}
	};

	app.oncheckpoint = function (args) {
		// TODO: This application is about to be suspended. Save any state that needs to persist across suspensions here.
		// You might use the WinJS.Application.sessionState object, which is automatically saved and restored across suspension.
		// If you need to complete an asynchronous operation before your application is suspended, call args.setPromise().
	};

	function buttonClickHandler(eventInfo) {
	    // Verify that we are currently not snapped, or that we can unsnap to open the picker
	    var currentState = Windows.UI.ViewManagement.ApplicationView.value;
	    if (currentState === Windows.UI.ViewManagement.ApplicationViewState.snapped &&
            !Windows.UI.ViewManagement.ApplicationView.tryUnsnap()) {
	        // Fail silently if we can't unsnap
	        return;
	    }

	    // Create the picker object and set options
	    var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
	    openPicker.viewMode = Windows.Storage.Pickers.PickerViewMode.thumbnail;
	    openPicker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.picturesLibrary;
	    // Users expect to have a filtered view of their folders
	    openPicker.fileTypeFilter.replaceAll([".png", ".jpg", ".jpeg"]);

	    // Open the picker for the user to pick a file
	    openPicker.pickSingleFileAsync().then(function (file) {
	        var url = URL.createObjectURL(file, { oneTimeOnly: true });
	        uploadedImage.setAttribute("src", url);
	    });
	}

	app.start();
})();
