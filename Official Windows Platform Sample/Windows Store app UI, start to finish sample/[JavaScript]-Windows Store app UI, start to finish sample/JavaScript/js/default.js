/*
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
*/
// For an introduction to the Hub template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=286574
(function () {
    "use strict";

    var activation = Windows.ApplicationModel.Activation;
    var app = WinJS.Application;
    var nav = WinJS.Navigation;
    var sched = WinJS.Utilities.Scheduler;
    var ui = WinJS.UI;
    var appBar;
    var cmdStylesheet;

    app.addEventListener("activated", function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }

            nav.history = app.sessionState.history || {};
            nav.history.current.initialPlaceholder = true;

            // Clear out the current settings handler.
            app.onsettings = null;
            // Populate settings pane and tie commands to settings flyouts.
            // See Adding app settings at http://go.microsoft.com/fwlink/?LinkID=288799.
            // Privacy statement must be included in Settings and on the app's description page.
            // When you submit your app to the Store, use the Privacy policy field in the Description step.
            // Note: The Permissions pane is system-controlled and can't be modified. 
            app.onsettings = function (e) {
                // Get the settings labels.
                var resSettingsHelpLabel = WinJS.Resources.getString('/help/settingsHelpLabel');
                var resSettingsPrivacyLabel = WinJS.Resources.getString('/privacy/settingsPrivacyLabel');
                e.detail.applicationcommands = {
                    "help": { title: resSettingsHelpLabel.value, href: "/settings/help.html" },
                    "privacy": { title: resSettingsPrivacyLabel.value, href: "/settings/privacy.html" }
                };
                WinJS.UI.SettingsFlyout.populateSettings(e);
            }

            // Optimize the load of the application and while the splash screen is shown, execute high priority scheduled work.
            ui.disableAnimations();
            var p = ui.processAll().then(function () {
                return nav.navigate(nav.location || Application.navigator.home, nav.state);
            }).then(function () {
                return sched.requestDrain(sched.Priority.aboveNormal + 1);
            }).then(function () {
                ui.enableAnimations();
                appBar = document.getElementById("appbar").winControl;

                // Switches between the light and dark style sheets.
                cmdStylesheet = appBar.getCommandById("cmdStylesheet");
                cmdStylesheet.addEventListener('click', doCmdStylesheet);
                initStyleSheetToggle();

                var helpButton = appBar.getCommandById("helpButton");
                helpButton.addEventListener("click", showHelp, false);


            });

            args.setPromise(p);
        }
    });

    // When your app is launched, this event occurs after both the loaded and activated events.
    // See WinJS.Application.onready at http://go.microsoft.com/fwlink/?LinkID=299192.
    app.onready = function (args) {
        // Show disclaimer regardless of how the app has been activated,
        // unless disclaimer already accepted.
        var disclaimer = Windows.Storage.ApplicationData.current.roamingSettings.values["disclaimer"];
        // If no disclaimer response, show disclaimer.
        if (!disclaimer) {
            // Get disclaimer resources.
            // See Globalizing your app at http://go.microsoft.com/fwlink/?LinkId=258266.
            var resDisclaimerTitle = WinJS.Resources.getString('disclaimerTitle');
            var resDisclaimer = WinJS.Resources.getString('disclaimer');
            var resDisclaimerButton = WinJS.Resources.getString('disclaimerButton');
            // Create a disclaimer message dialog and set its content.
            // A message dialog can support up to three commands. 
            // If no commands are specified, a close command is provided by default.
            // If specifying your own commmands, set the command that will be invoked by default.
            // For example, msg.defaultCommandIndex = 1;
            // Note: Message dialogs should be used sparingly, and only for messages or 
            // questions critical to the continued use of your app.
            // See Adding message dialogs at http://go.microsoft.com/fwlink/?LinkID=275116.
            var msg = new Windows.UI.Popups.MessageDialog(resDisclaimer.value, resDisclaimerTitle.value);
            // Handler for disclaimer.
            // For this example, we use the disclaimer to demonstrate roaming app data.
            msg.commands.append(new Windows.UI.Popups.UICommand(resDisclaimerButton.value, handleDisclaimer));

            // Show the message dialog
            msg.showAsync();
        }
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. If you need to 
        // complete an asynchronous operation before your application is 
        // suspended, call args.setPromise().
        app.sessionState.history = nav.history;
    };

    // Store the disclaimer response.
    // For this example, we use the disclaimer to demonstrate roaming app data.
    // See Roaming application data at http://go.microsoft.com/fwlink/?LinkID=313894.
    function handleDisclaimer(eventInfo) {
        var appData = Windows.Storage.ApplicationData.current;
        var roamingSettings = appData.roamingSettings;
        roamingSettings.values["disclaimer"] = true;
    }

    // Show the settings pane help page from app bar.
    function showHelp(eventInfo) {
        eventInfo.preventDefault();
        WinJS.UI.SettingsFlyout.showSettings("help", "/settings/help.html");

        // Dismiss the nav and app bars. Light dismiss doesn't execute.
        appBar.hide();
    }

    app.start();

    function doCmdStylesheet(eventInfo) {
        var selected = cmdStylesheet.selected;

        if (selected) {
            switchStyleDark();
        }
        else {
            switchStyleLight();
        }

        // Reload the hub page when the stylesheet changes. 
        if (WinJS.Navigation.location === "/pages/hub/hub.html") {
            WinJS.Navigation.navigate("/pages/hub/hub.html");
        }
    }

    WinJS.Utilities.markSupportedForProcessing(doCmdStylesheet);
    function switchStyleDark() {
        var uiLightString = "ui-light.css";
        var uiDarkString = "ui-dark.css";
        for (var i = 0; i < document.styleSheets.length; i++) {
            if (document.styleSheets[i].href &&
                document.styleSheets[i].href.lastIndexOf(uiDarkString) === (document.styleSheets[i].href.length - uiDarkString.length)) {
                document.styleSheets[i].disabled = false;
            }
            if (document.styleSheets[i].href &&
                document.styleSheets[i].href.lastIndexOf(uiLightString) === (document.styleSheets[i].href.length - uiLightString.length)) {
                document.styleSheets[i].disabled = true;
            }
        }

        // switch the class on body tag as a master selector for dark/light versions of styling examples
        WinJS.Utilities.removeClass(document.body, "ui-light");
        WinJS.Utilities.addClass(document.body, "ui-dark");
    }

    function switchStyleLight() {
        var uiLightString = "ui-light.css";
        var uiDarkString = "ui-dark.css";
        for (var i = 0; i < document.styleSheets.length; i++) {
            if (document.styleSheets[i].href &&
                document.styleSheets[i].href.lastIndexOf(uiDarkString) === (document.styleSheets[i].href.length - uiDarkString.length)) {
                document.styleSheets[i].disabled = true;
            }
            if (document.styleSheets[i].href &&
                document.styleSheets[i].href.lastIndexOf(uiLightString) === (document.styleSheets[i].href.length - uiLightString.length)) {
                document.styleSheets[i].disabled = false;
            }
        }

        WinJS.Utilities.removeClass(document.body, "ui-dark");
        WinJS.Utilities.addClass(document.body, "ui-light");
    }

    function initStyleSheetToggle() {
        var uiLightString = "ui-light.css";
        var uiDarkString = "ui-dark.css";
        for (var i = 0; i < document.styleSheets.length; i++) {
            if (document.styleSheets[i].href &&
                document.styleSheets[i].href.lastIndexOf(uiDarkString) === (document.styleSheets[i].href.length - uiDarkString.length) &&
                document.styleSheets[i].disabled === false) {
                cmdStylesheet.selected = true;


            }
            else if (document.styleSheets[i].href &&
                document.styleSheets[i].href.lastIndexOf(uiLightString) === (document.styleSheets[i].href.length - uiLightString.length) &&
                document.styleSheets[i].disabled === false) {
                cmdStylesheet.selected = false;

            }
        }
    }


    function navBarItemInvoked(eventInfo) {
        WinJS.Navigation.navigate("/pages/item/item.html", { item: eventInfo.detail.data });
    }

    function searchHandler(args) {
        WinJS.Navigation.navigate('/pages/search/searchResults.html', args.detail);
    }

    function suggestionsRequestedHandler(args) {

         

        var queryText = args.detail.queryText,
        query = queryText.toLocaleLowerCase(),
        suggestionCollection = args.detail.searchSuggestionCollection;
        if (queryText.length > 0) {
             
            ControlsData.controlsList.forEach(
                function (element, index, array) {
                    if (element.title.substr(0, query.length).toLocaleLowerCase() === query) {
                        suggestionCollection.appendQuerySuggestion(element.title);
                    }
                    else if (element.controlName.substr(0, query.length).toLocaleLowerCase() === query)
                    {
                        suggestionCollection.appendQuerySuggestion(element.controlName);
                    }
                });

            args.detail.linguisticDetails.queryTextAlternatives.forEach(
                function (element, index, array) {
                    if (element.substr(0, query.length).toLocaleLowerCase() === query) {
                        suggestionCollection.appendQuerySuggestion(element);
                    }

                });
        }

    }

    WinJS.Namespace.define("SampleNavigation",
    {
        navBarItemInvoked: WinJS.UI.eventHandler(navBarItemInvoked),
        searchHandler: WinJS.UI.eventHandler(searchHandler),
        suggestionsRequestedHandler: WinJS.UI.eventHandler(suggestionsRequestedHandler)
    }
    );

})();




