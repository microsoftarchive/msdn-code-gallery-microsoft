/*
Copyright (c) Microsoft Corporation. All rights reserved
*/

// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
	"use strict";

	WinJS.UI.Pages.define("/pages/home/home.html", {
		// This function is called whenever a user navigates to this page. It
		// populates the page elements with the app's data.
		ready: function (element, options) {
			var addItemButton = element.querySelector("#addItemButton");
			addItemButton.addEventListener("click", this.addItemButtonClick, false);
		},

		unload: function () {
		},

		updateLayout: function (element) {
		},

		addItemButtonClick: function(mouseEvent) {
			var installedCommandSets = Windows.Media.SpeechRecognition.VoiceCommandManager.installedCommandSets;
			if (installedCommandSets.hasKey("commandSet_en-us")) {
				var commandSetEnUs = installedCommandSets.lookup("commandSet_en-us");
				commandSetEnUs.setPhraseListAsync("newspaperSection", ["national news", "world news", "sports", "entertainment", "weather"]).done(
					// Success function.
					function (result) {
						var addItemButton = document.querySelector("#addItemButton");
						addItemButton.disabled = true;
					},
					// Error function.
					function (err) {
						WinJS.log && WinJS.log("Update phrase list failed.");
					}
				);
			}
		}
	});
})();
