/*
Copyright (c) Microsoft Corporation. All rights reserved
*/

// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
	"use strict";

	var nav = WinJS.Navigation;

	WinJS.UI.Pages.define("/pages/messagePage/messagePage.html", {
		// This function is called whenever a user navigates to this page. It
		// populates the page elements with the app's data.
		ready: function (element, options) {
			var howToUseLink = element.querySelector("#howToUseLink");
			howToUseLink.addEventListener("click", this.howToUseLinkClick, false);

			var parameters = options.split("|");
			if (parameters.length > 2) {
				var contactParagraph = element.querySelector("#contactParagraph");
				contactParagraph.innerText = parameters[1];
				var msgTextParagraph = element.querySelector("#msgTextParagraph");
				msgTextParagraph.innerText = parameters[2];

				// Only give audible feedback if the commandMode key in the SpeechRecognitionResult's SemanticInterpretation
				// collection is not "text". If the app was launched by typing, rather than voice, then we should give silent
				// feedback as a courtesy.
				if (parameters[0] != "text") {
					var speechSynthesizer = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
					var feedback = "VoiceCommandsQuickstart has launched and is messaging " + contactParagraph.innerText + " with the message " + msgTextParagraph.innerText;
					speechSynthesizer.synthesizeTextToStreamAsync(feedback).then(
						// Success function.
						function (stream) {
							var feedbackAudioElement = document.querySelector("#feedbackAudioElement");
							var blob = MSApp.createBlobFromRandomAccessStream(stream.ContentType, stream);
							feedbackAudioElement.src = URL.createObjectURL(blob, { oneTimeOnly: true });
							feedbackAudioElement.play();
						}
					);
				}
			}
		},

		unload: function () {
		},

		updateLayout: function (element) {
		},

		howToUseLinkClick: function (mouseEvent) {
			nav.navigate(Application.navigator.home, nav.state);
		}
	});
})();
