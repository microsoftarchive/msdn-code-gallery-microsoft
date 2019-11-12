// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/p/?linkid=232509
(function () {
    "use strict";

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
            }
            args.setPromise(WinJS.UI.processAll());

            // Retrieve the div that hosts the Toggle control.
            var toggleControlDiv = document.getElementById("toggleControlDiv");

            // Retrieve the actual Toggle control.
            var toggleControl = toggleControlDiv.winControl;

            // Register the event handler. 
            toggleControl.addEventListener("change", toggleChanged);

            // Retrieve the button and register our event handler. 
            var helloButton = document.getElementById("helloButton");
            helloButton.addEventListener("click", buttonClickHandler, false);
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
        // Get the user's name input
        var userName = document.getElementById("nameInput").value;

        // Create the greeting string and set the greeting output to it
        var greetingString = "Hello, " + userName + "!";
        document.getElementById("greetingOutput").innerText = greetingString;
    }

    function toggleChanged(eventInfo) {
        // Get the toggle control
        var toggleControl = document.getElementById("toggleControlDiv").winControl;

        // Get the greeting output
        var greetingOutput = document.getElementById("greetingOutput");

        // Set the CSS class for the greeting output based on the toggle's state
        if (toggleControl.checked == true) {
            greetingOutput.setAttribute("class", "toggle-on");
        }
        else {
            greetingOutput.removeAttribute("class", "toggle-on");
        }
    }

    app.start();
})();
