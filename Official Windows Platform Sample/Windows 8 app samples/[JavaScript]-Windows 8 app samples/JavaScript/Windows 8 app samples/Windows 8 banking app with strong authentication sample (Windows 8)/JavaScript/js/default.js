// For an introduction to the Grid template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=232446
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
                var lastScenarioId = parseInt(WinJS.Application.sessionState.selectedScenarioId || "-1");
                var backStack = WinJS.Application.sessionState.backStack;
                var forwardStack = WinJS.Application.sessionState.forwardStack;
                var current = WinJS.Application.sessionState.current;

                if (backStack) {
                    WinJS.Navigation.history.backStack = backStack;
                }
                if (forwardStack) {
                    WinJS.Navigation.history.forwardStack = forwardStack;
                }
                switch (lastScenarioId) {
                    case 0:
                    case 1:
                        if (current.state) {
                            WinJS.Navigation.navigate(current.location, { item: current.state.item });
                        }
                        else {
                            WinJS.Navigation.navigate(current.location);
                        }
                        break;
                    default:
                        var homePage = document.body.getAttribute('data-homePage');
                        WinJS.Navigation.navigate(homePage);
                        break;
                }
            }
            args.setPromise(WinJS.UI.processAll());
        }
    };

    if (Windows.Storage.ApplicationData.current.localSettings.values["scenarioId"]) {
        Windows.Storage.ApplicationData.current.localSettings.values.remove("scenarioId");
    }

    // checkpoint event handler, store app session states for PLM, e.g. navigation history.
    app.onchekpoint = function (args)
    {
        // The checkpoint event gives us the chance to save application state.
        if (Windows.Storage.ApplicationData.current.localSettings.values["scenarioId"]) {
            WinJS.Application.sessionState.selectedScenarioId = Windows.Storage.ApplicationData.current.localSettings.values["scenarioId"];
        }

        WinJS.Application.sessionState.backStack = WinJS.Navigation.history.backStack;
        WinJS.Application.sessionState.forwardStack = WinJS.Navigation.history.forwardStack;
        WinJS.Application.sessionState.current = WinJS.Navigation.history.current;
        WinJS.Application.sessionState.current.initialPlaceholder = true;
    };
        
    app.start();
    
})();
