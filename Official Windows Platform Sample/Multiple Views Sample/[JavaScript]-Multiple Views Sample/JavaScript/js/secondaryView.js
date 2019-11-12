//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    var ViewManagement = Windows.UI.ViewManagement;

    // This page is shown in secondary views created by this app.
    // See Scenario 1 for details on how to create a secondary view
    var thisView = new SecondaryViewsHelper.ViewLifetimeControl();
    thisView.addEventListener("initializedatareceived", function (e) {
        if (e.detail.title) {
            thisView.setTitle(e.detail.title);
        }
    }, false);
    
    thisView.initialize();

    window.addEventListener("message", function (e) {
        if (e.origin === SecondaryViewsHelper.thisDomain) {
            if (e.data.doAnimateAndSwitch) {
                animateAndSwitch();
            } else if (e.data.handleProtocolLaunch) {
                handleProtocolLaunch(e.data.uri);
            }
        }
    }, false);

    var titleBox;
    document.addEventListener("DOMContentLoaded", function () {
        titleBox = document.getElementById("titleBox");
        document.getElementById("setTitleButton").addEventListener("click", setTitle, false);
        document.getElementById("clearTitleButton").addEventListener("click", clearTitle, false);
        document.getElementById("launchProtocolButton").addEventListener("click", launchProtocol, false);
        document.getElementById("goToMainButton").addEventListener("click", goToMain, false);
        document.getElementById("hideViewButton").addEventListener("click", hideView, false);
    }, false);

    function setTitle() {
        // Set a title for the window. This title is visible
        // in system switchers
        thisView.setTitle(titleBox.value);
    }

    function clearTitle() {
        // Clear the title by setting it to blank
        titleBox.value = "";
        thisView.setTitle("");
    }

    function launchProtocol() {
        // Used with Scenario 2
        thisView.startViewInUse();
        Windows.System.Launcher.launchUriAsync(
            new Windows.Foundation.Uri("ms-multiple-view-sample://basiclaunch/")
        ).done(function () {
            thisView.stopViewInUse();
        });
    }

    function handleProtocolLaunch(uri) {
        // This code should only get executed if DisableShowingMainViewOnActivation
        // has been called. See Scenario 2 for details
        WinJS.Utilities.removeClass(document.getElementById("protocolText"), "hidden");
        document.getElementById("protocolContent").innerText = uri;
    }

    function goToMain() {
        // Switch to the main view without explicitly requesting
        // that this view be hidden
        thisView.startViewInUse();
        ViewManagement.ApplicationViewSwitcher.switchAsync(thisView.opener.viewId).done(function () {
            thisView.stopViewInUse();
        });
    }

    function hideView() {
        // Switch to main and hide this view entirely from the user
        thisView.startViewInUse();
        ViewManagement.ApplicationViewSwitcher.switchAsync(
            thisView.opener.viewId,
            thisView.viewId,
            ViewManagement.ApplicationViewSwitchingOptions.consolidateViews
        ).done(function () {
            thisView.stopViewInUse();
        });
    }

    // This continues the flow from Scenario 3
    function animateAndSwitch() {
        // Before switching, make this view match the outgoing window
        // (go to a blank background)
        document.body.style.opacity = 0;

        thisView.startViewInUse();

        // Bring this view onto screen. Since the two view are drawing
        // the same visual, the user will not be able to perceive the switch
        ViewManagement.ApplicationViewSwitcher.switchAsync(
            thisView.viewId,
            thisView.opener.viewId,
            ViewManagement.ApplicationViewSwitchingOptions.skipAnimation
        ).then(function () {
            // Now that this window is on screen, animate in its contents
            return WinJS.UI.Animation.enterPage(document.body);
        }).done(function () {
            thisView.stopViewInUse();
        });
    }

})();