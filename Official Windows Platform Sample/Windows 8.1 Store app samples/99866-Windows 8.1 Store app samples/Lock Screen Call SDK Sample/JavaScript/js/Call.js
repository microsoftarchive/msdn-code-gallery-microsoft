//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var callUI;
    var endCallButton;
    var hangUpTimeout;

    var page = WinJS.UI.Pages.define("/html/Call.html", {
        ready: function (element, options) {
            endCallButton = document.getElementById("endCallButton");

            // This sample uses the same page for both lock screen calls and normal calls.
            // The "activated" handler in default.js handles both normal call activation
            // and lock screen call activation by passing the eventObject.details object
            // as the page options. If activated from a lock screen call, the details
            // object will have a callUI member which gives us access to the lock screen.
            // The arguments member is the toast command string specified in the XML generated
            // in the xmlPayload variable in Toast.js.

            // the buildArguments function in Toast.js passes the arguments
            // in the form
            //   "<call mode> <caller identity> <simulated call duration>"

            var callArguments = options.arguments.split(" ");
            var callMode = callArguments[0];
            var callerIdentity = callArguments[1];
            var callDuration = callArguments[2];

            var imageText = "";
            switch (callerIdentity) {
                case "Dad":
                    imageText = "\ud83d\udc68";
                    break;
                case "Mom":
                    imageText = "\ud83d\udc69";
                    break;
                case "Grandpa":
                    imageText = "\ud83d\udc74";
                    break;
                case "Grandma":
                    imageText = "\ud83d\udc75";
                    break;
            }
            if (callMode === "Voice") {
                imageText = "\ud83d\udd0a";
            }

            var callTitle = callMode + " call from " + callerIdentity;

            callUI = options.callUI;
            if (callUI) {
                // Hide controls since they are not interactive on the lock screen.
                endCallButton.style.visibility = "hidden";

                // Hook up events.
                callUI.addEventListener("endrequested", onEndRequested);
                callUI.addEventListener("closed", onClosed);

                // Set the title.
                callUI.callTitle = callTitle;
            }

            var callImage = document.getElementById("callImage");

            callImage.style.fontSize = (window.innerHeight / 2) + "px";
            callImage.innerText = imageText;

            // Assign a random dark background color so that each call looks
            // slightly different.
            callImage.style.backgroundColor = "#" +
                    Math.floor(((1 + Math.random()) * 4096) & 0x1777).toString(16).substr(1);

            document.getElementById("callTitle").innerText = callTitle;

            endCallButton.addEventListener("click", onEndCall, false);

            if (callDuration > 0) {
                hangUpTimeout = setTimeout(otherPartyHangsUp, callDuration * 1000);
            }
        },
        unload: function () {
            endCallButton = null;
            callUI = null;
            clearTimeout(hangUpTimeout);
        }
    });

    function navigateToMainPage() {
        return WinJS.Navigation.navigate("/html/Toast.html");
    }

    function fadeToBlack() {
        // Fade to black by fading in a solid black surface
        var callFadeOut = document.getElementById("callFadeOut");
        callFadeOut.style.visibility = "visible";
        return WinJS.UI.Animation.fadeIn(callFadeOut);
    }

    // This function simulates the other party hanging up the call.
    function otherPartyHangsUp() {
        var exitAnimation;
        if (callUI) {
            exitAnimation = fadeToBlack();
        } else {
            exitAnimation = WinJS.Promise.wrap();
        }
        exitAnimation.done(function () {
            if (callUI) {
                callUI.dismiss();
            }
            return navigateToMainPage();
        });
    }

    function onEndCall() {
        return navigateToMainPage();
    }

    // Called when the user ends the call directly from the lock screen.
    function onEndRequested(args) {
        var deferral = args.getDeferral();
        fadeToBlack().done(function () {
            // Complete the deferral before navigating so that the lock
            // screen does not show our main page.
            deferral.complete();            
            return navigateToMainPage();
        });
    }

    // Called when the call is removed from the lock screen by whatever means.
    function onClosed() {
        // Show the "End Call" button in our app, if it still exists.
        if (endCallButton) {
            endCallButton.style.visibility = "visible";
        }
        callUI = null;
    }
})();
