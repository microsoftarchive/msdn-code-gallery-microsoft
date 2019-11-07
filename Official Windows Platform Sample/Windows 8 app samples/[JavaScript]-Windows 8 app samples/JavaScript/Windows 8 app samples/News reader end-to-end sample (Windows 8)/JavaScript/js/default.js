// For an introduction to the Grid template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=232446
(function () {
    "use strict";

    // Helper variables.
    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    var nav = WinJS.Navigation;
    WinJS.strictProcessing();

    app.addEventListener("activated", function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {

            if (app.sessionState.history) {
                nav.history = app.sessionState.history;
            }
            args.setPromise(WinJS.UI.processAll().then(function () {
                IO.loadStateAsync().then(function () {

                    document.getElementById("subscriptionsCmd").addEventListener("click", function () {
                        nav.navigate("/pages/subscriptions/subscriptions.html");
                    }, false);

                    if (nav.location) {
                        nav.history.current.initialPlaceholder = true;
                        return nav.navigate(nav.location, nav.state);
                    } else {
                        return nav.navigate(Application.navigator.home);
                    }
                });
            }));

        }
    });

    app.oncheckpoint = function (args) {
        // This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. If you need to 
        // complete an asynchronous operation before your application is 
        // suspended, call args.setPromise().
        app.sessionState.history = nav.history;
        args.setPromise(IO.saveStateAsync());
    };

    app.start();

    WinJS.Namespace.define("Constants", {
        SUB_LIMIT: 20,
        MAX_ITEMS: 15
    });
})();
