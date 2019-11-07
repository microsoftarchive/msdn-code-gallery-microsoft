(function () {
    "use strict";

    WinJS.UI.Pages.define("/pages/home/home.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            // TODO: Initialize the page here.

            WinJS.Utilities.query("a").listen("click", this.linkClickEventHandler, false);

            // Retrieve the div that hosts the Toggle control.
            var toggleControlDiv = document.getElementById("toggleControlDiv");

            // Retrieve the actual Toggle control.
            var toggleControl = toggleControlDiv.winControl;

            // Register the event handler. 
            toggleControl.addEventListener("change", this.toggleChanged, false);

            // Retrieve the button and register our event handler. 
            var helloButton = document.getElementById("helloButton");
            helloButton.addEventListener("click", this.buttonClickHandler, false);

            // Retrieve the input element and register our
            // event handler.
            var nameInput = document.getElementById("nameInput");
            nameInput.addEventListener("change", this.nameInputChanged);

            // Restore app data. 
            var roamingSettings = Windows.Storage.ApplicationData.current.roamingSettings;

            // Restore the Toggle selection.
            var toggleSelection = roamingSettings.values["toggleSelection"];
            if (toggleSelection) {
                toggleControl.checked = toggleSelection;

                // Apply the properties of the toggle selection
                var greetingOutput = document.getElementById("greetingOutput");
                if (toggleControl.checked == true) {
                    greetingOutput.setAttribute("class", "toggle-on");
                }
                else {
                    greetingOutput.removeAttribute("class", "toggle-on");
                }
            }

            // If the app was terminated last time it ran, restore the greeting
            // and name output
            if (
                WinJS.Application.sessionState.previousExecutionState
                === Windows.ApplicationModel.Activation.ApplicationExecutionState.terminated) {
                var outputValue = WinJS.Application.sessionState.greetingOutput;
                if (outputValue) {
                    var greetingOutput = document.getElementById("greetingOutput");
                    greetingOutput.innerText = outputValue;
                }
                var inputValue = WinJS.Application.sessionState.nameInput;
                if (inputValue) {
                    var nameInput = document.getElementById("nameInput");
                    nameInput.value = inputValue;
                }
            }
        },

        buttonClickHandler: function (eventInfo) {
            // Get the user's name input
            var userName = document.getElementById("nameInput").value;

            // Create the greeting string and set the greeting output to it
            var greetingString = "Hello, " + userName + "!";
            document.getElementById("greetingOutput").innerText = greetingString;

            // Save the session data. 
            WinJS.Application.sessionState.greetingOutput = greetingString;

            // Save the session data. 
            WinJS.Application.sessionState.greetingOutput = greetingString;
        },

        toggleChanged: function (eventInfo) {
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

            // Store the toggle selection for multiple sessions.
            var appData = Windows.Storage.ApplicationData.current;
            var roamingSettings = appData.roamingSettings;
            roamingSettings.values["toggleSelection"] = toggleControl.checked;
        },

        nameInputChanged: function (eventInfo) {
            var nameInput = eventInfo.srcElement;

            // Store the user's name input
            WinJS.Application.sessionState.nameInput = nameInput.value;
        },

        linkClickEventHandler: function (eventInfo) {
            eventInfo.preventDefault();
            var link = eventInfo.target;
            WinJS.Navigation.navigate(link.href);
        }

    });
})(); 
