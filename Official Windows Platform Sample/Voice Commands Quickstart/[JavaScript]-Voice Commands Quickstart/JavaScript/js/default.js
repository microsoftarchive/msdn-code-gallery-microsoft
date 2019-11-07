/*
Copyright (c) Microsoft Corporation. All rights reserved
*/

// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=329104
(function () {
	"use strict";

	var app = WinJS.Application;
	var activation = Windows.ApplicationModel.Activation;
	var nav = WinJS.Navigation;
	var sched = WinJS.Utilities.Scheduler;
	var ui = WinJS.UI;

	app.onactivated = function (args) {
		if (args.detail.kind === activation.ActivationKind.launch) {
			WinJS.Utilities.startLog();
			if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
				// The application has been newly launched, so install the voice commands.
				var vcdFilePath = new Windows.Foundation.Uri("ms-appx:///QuickstartCommands.xml");
				Windows.Storage.StorageFile.getFileFromApplicationUriAsync(vcdFilePath).then(
                    // Success function.
                    function (vcdStorageFile) {
                    	Windows.Media.SpeechRecognition.VoiceCommandManager.installCommandSetsFromStorageFileAsync(vcdStorageFile).then(
							function () {
								WinJS.log && WinJS.log("Installing the voice commands succeeded.");
							},
							function (err) {
								WinJS.log && WinJS.log("Installing the voice commands failed.");
							});
                    },
                    // Error function.
                    function (err) {
                    	WinJS.log && WinJS.log("File access failed.");
                    });

			} else {
				// The application has been reactivated from suspension.
			}

			nav.history = app.sessionState.history || {};
			nav.history.current.initialPlaceholder = true;

			// Optimize the load of the application and while the splash screen is shown, execute high priority scheduled work.
			ui.disableAnimations();
			var p = ui.processAll().then(function () {
				return nav.navigate(nav.location || Application.navigator.home, nav.state);
			}).then(function () {
				return sched.requestDrain(sched.Priority.aboveNormal + 1);
			}).then(function () {
				ui.enableAnimations();
			});

			args.setPromise(p);
		}
		else if (args.detail.kind === activation.ActivationKind.voiceCommand) {
			// This application has been activated with a voice command.
			if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
			} else {
				// The application has been reactivated from suspension.
			}

			nav.history = app.sessionState.history || {};
			nav.history.current.initialPlaceholder = true;

			// Optimize the load of the application and while the splash screen is shown, execute high priority scheduled work.
			ui.disableAnimations();
			var p = ui.processAll().then(function () {
				var speechRecognitionResult = args.detail.result;

				// The commandMode is either "voice" or "text", and it indicates how the voice command was entered by the user.
				// We should respect "text" mode by providing feedback in a silent form.
				var commandMode = semanticInterpretation("commandMode", speechRecognitionResult);

				// Get the name of the voice command, the actual text spoken, and the value of Command/Navigate@Target.
				var voiceCommandName = speechRecognitionResult.rulePath[0].toString();
				var textSpoken = speechRecognitionResult.text;
				var navigationTarget = semanticInterpretation("NavigationTarget", speechRecognitionResult);

				var navigateToPage = Application.navigator.home;
				var navigationParameterString = "";

				switch (voiceCommandName) {
					case "showASection":
					case "goToASection":
						var newspaperSection = semanticInterpretation("newspaperSection", speechRecognitionResult);
						navigateToPage = "/pages/showASectionPage/showASectionPage.html";
						navigationParameterString = commandMode + "|" + newspaperSection;
						break;

					case "message":
					case "text":
						var contact = semanticInterpretation("contact", speechRecognitionResult);
						var msgText = semanticInterpretation("msgText", speechRecognitionResult);
						navigateToPage = "/pages/messagePage/messagePage.html";
						navigationParameterString = commandMode + "|" + contact + "|" + msgText;
						break;

					case "playAMovie":
						var movieSearch = semanticInterpretation("movieSearch", speechRecognitionResult);
						navigateToPage = "/pages/playAMoviePage/playAMoviePage.html";
						navigationParameterString = commandMode + "|" + movieSearch;
						break;

					default:
						// There is no match for the voice command name.
						break;
				}
				return nav.navigate(navigateToPage, navigationParameterString);
			}).then(function () {
				return sched.requestDrain(sched.Priority.aboveNormal + 1);
			}).then(function () {
				ui.enableAnimations();
			});

			args.setPromise(p);
		}
	};

	app.oncheckpoint = function (args) {
		// The app is about to be suspended. Save any state
		// that needs to persist across suspensions via the
		// WinJS.Application.sessionState object, which is automatically
		// saved and restored across suspension.
		app.sessionState.history = nav.history;
	};

	function semanticInterpretation(key, speechRecognitionResult) {
		if (speechRecognitionResult.semanticInterpretation.properties.hasKey(key)) {
			return speechRecognitionResult.semanticInterpretation.properties[key][0];
		}
		else {
			return "unknown";
		}
	}

	app.start();
})();
