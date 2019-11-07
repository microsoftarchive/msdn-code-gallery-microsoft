// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/p/?linkid=232509
(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.

                // Retrieve our greetingOutput session state info, 
                // if it exists. 
                var outputValue = WinJS.Application.sessionState.greetingOutput;
                if (outputValue) {
                    var greetingOutput = document.getElementById("greetingOutput");
                    greetingOutput.innerText = outputValue;
                }
            }
            args.setPromise(WinJS.UI.processAll().then(function completed() {

                // Retrieve the div that hosts the Rating control.
                var ratingControlDiv = document.getElementById("ratingControlDiv");

                // Retrieve the actual Rating control.
                var ratingControl = ratingControlDiv.winControl;

                // Register the event handler. 
                ratingControl.addEventListener("change", ratingChanged, false);

                // Retrieve the button and register our event handler. 
                var helloButton = document.getElementById("helloButton");
                helloButton.addEventListener("click", buttonClickHandler, false);

                // Retrieve the input element and register our
                // event handler.
                var nameInput = document.getElementById("nameInput");
                nameInput.addEventListener("change", nameInputChanged);

                // Restore app data. 
                var roamingSettings = Windows.Storage.ApplicationData.current.roamingSettings;

                // Restore the user name.
                var userName =
                    Windows.Storage.ApplicationData.current.roamingSettings.values["userName"];
                if (userName) {
                    nameInput.value = userName;
                }

                // Restore the rating. 
                var greetingRating = roamingSettings.values["greetingRating"];
                if (greetingRating) {
                    ratingControl.userRating = greetingRating;
                    var ratingOutput = document.getElementById("ratingOutput");
                    ratingOutput.innerText = greetingRating;
                }

            }));

        }
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    function buttonClickHandler(eventInfo) {

        var userName = document.getElementById("nameInput").value;
        var greetingString = "Hello, " + userName + "!";
        document.getElementById("greetingOutput").innerText = greetingString;

        // Save the session data. 
        WinJS.Application.sessionState.greetingOutput = greetingString;
    }

    function ratingChanged(eventInfo) {

        var ratingOutput = document.getElementById("ratingOutput");
        ratingOutput.innerText = eventInfo.detail.tentativeRating;

        // Store the rating for multiple sessions.
        var appData = Windows.Storage.ApplicationData.current;
        var roamingSettings = appData.roamingSettings;
        roamingSettings.values["greetingRating"] = eventInfo.detail.tentativeRating;
    }

    function nameInputChanged(eventInfo) {
        var nameInput = eventInfo.srcElement;

        // Store the user's name for multiple sessions.
        var appData = Windows.Storage.ApplicationData.current;
        var roamingSettings = appData.roamingSettings;
        roamingSettings.values["userName"] = nameInput.value;
    }

    app.start();
})();

