/*
Copyright (c) Microsoft Corporation. All rights reserved
*/

// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
	"use strict";

	var nav = WinJS.Navigation;

	WinJS.UI.Pages.define("/pages/playAMoviePage/playAMoviePage.html", {
		// This function is called whenever a user navigates to this page. It
		// populates the page elements with the app's data.
		ready: function (element, options) {
			var howToUseLink = element.querySelector("#howToUseLink");
			howToUseLink.addEventListener("click", this.howToUseLinkClick, false);

			var parameters = options.split("|");
			if (parameters.length > 1) {
				var movieSearchParagraph = element.querySelector("#movieSearchParagraph");
				movieSearchParagraph.innerText = parameters[1];
				var feedbackParagraph = element.querySelector("#feedbackParagraph");
				feedbackParagraph.innerText = "VoiceCommandsQuickstart has launched and is now playing the movie \"" + movieSearchParagraph.innerText + "\".";

				// Only give audible feedback if the commandMode key in the SpeechRecognitionResult's SemanticInterpretation
				// collection is not "text". If the app was launched by typing, rather than voice, then we should give silent
				// feedback as a courtesy.
				if (parameters[0] != "text") {
					var speechSynthesizer = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
					var feedbackParagraph = element.querySelector("#feedbackParagraph");
					speechSynthesizer.synthesizeTextToStreamAsync(feedbackParagraph.innerText).then(
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
