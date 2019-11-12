// For an introduction to the Navigation template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232506
(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    var nav = WinJS.Navigation;


    app.addEventListener("activated", function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }

            // Save the previous execution state. 
            WinJS.Application.sessionState.previousExecutionState =
                args.detail.previousExecutionState;

            if (app.sessionState.history) {
                nav.history = app.sessionState.history;
            }
            args.setPromise(WinJS.UI.processAll().then(function () {

                // Retrieve the app bar.
                var appbar = document.getElementById("appbar").winControl;

                // Attach event handlers to the command buttons.
                var homeButton = appbar.getCommandById("homeButton");
                homeButton.addEventListener("click", goToHomePage, false);
                var page2Button = appbar.getCommandById("page2Button");
                page2Button.addEventListener("click", goToPage2, false);

                if (nav.location) {
                    nav.history.current.initialPlaceholder = true;
                    return nav.navigate(nav.location, nav.state);
                } else {
                    return nav.navigate(Application.navigator.home);
                }

            }));
        }
    });


    function goToHomePage(eventInfo) {
        WinJS.Navigation.navigate("/pages/home/home.html");
    }

    function goToPage2(eventInfo) {
        WinJS.Navigation.navigate("/pages/page2/page2.html");
    }

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. If you need to 
        // complete an asynchronous operation before your application is 
        // suspended, call args.setPromise().
        app.sessionState.history = nav.history;
    };

    app.start();
})();
