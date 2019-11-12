/*

    This file contains our navigation model. It is built on top of WinJS.Navigation.

*/

(function () {

    "use strict";

    var currentView;
    var animateOut;
    var backMax = 20;

    // Only store up to 20 items on the backstack and make sure the last one is always the 'Home' screen.
    function trimBackStack() {
        WinJS.Navigation.history.backStack = WinJS.Navigation.history.backStack.filter(function (item) {
            if (item.location !== 'noConnection') {
                return true;
            }
            return false;
        });
        var stackLength = WinJS.Navigation.history.backStack.length;
        if(stackLength > backMax) {
            var newStack = WinJS.Navigation.history.backStack.slice(length - backMax);
            newStack.unshift({ location:'home', state: false});
            WinJS.Navigation.history.backStack = newStack;
        }
    }

    function beforeNavigate(e) {
        if (WinJS.Navigation.location === 'noConnection' && !ConferenceApp.Api.isSignedIn()) {
            e.preventDefault();
        }
        // clean up any pending items.
        if (currentView) {
            currentView.checkpoint();
            currentView.pending.forEach(function(item) {
                item.cancel();
            });
            currentView.pending = [];
        }

        animateOut = WinJS.Promise.wrap(true);
        if (WinJS.Navigation.location) {
            var outgoing = $('#topBanner, #contentHost');
            animateOut = WinJS.UI.Animation.exitPage(outgoing);
        }
    }
    
    function navigated(e) {

        trimBackStack();

        animateOut.then(function () {
            renderPage(e.detail.location, e.detail.state, true);
        });
    }

    // renders the template for the location the user has navigated to.
    function renderPage(newLocation, state, animate) {
        ConferenceApp.View.beforeView();
        var view = ConferenceApp.Views[newLocation];
        currentView = view;
        view.beforeRender(state);
        updateOuterPage(view.title);
        MSApp.execUnsafeLocalFunction(function () {
            $('#contentHost').html($.template(view.template, state));
        });
        if (animate) {
            var incoming = $('#topBanner, #contentHost');
            WinJS.UI.Animation.enterPage(incoming);
        }
        view.hooks(state);
    }

    // Some views have a different title bar UX, so reset the styles here. 
    // Also disables the back button when the history stack is empty.
    function updateOuterPage(title) {
        if (WinJS.Navigation.canGoBack && ConferenceApp.Api.isSignedIn()) {
            $('#backButton').removeAttr('disabled');
        }
        else {
            $('#backButton').attr('disabled', true);
        }

        $('#pageTitle').removeClass('hideTextOverflow');
        $('#pageTitle').text(title);
        $('#outerPage').removeClass('darkTitle');

    }

    // True if a given navigation state object is considered "empty"
    function isEmptyState(state) {
        if (state === undefined) {
            return true;
        }
        else if (state === null) {
            return true;
        }
        else if (typeof (state) === 'object' && Object.keys(state).length === 0) {
            return true;
        }
        else {
            return false;
        }
    }

    // navigate, but only if to a new location/state tuple.
    function navigate(destination, state) {

        var sameState = false;
        if (state === undefined || state === null) {
            state = {};
        }
        if (WinJS.Navigation.state === state) {
            sameState = true;
        }
        else if (isEmptyState(WinJS.Navigation.state) && isEmptyState(state)) {
            sameState = true;
        }

        if (WinJS.Navigation.location !== destination || !sameState) {
            WinJS.Navigation.navigate(destination, state);
        }
    }

    // try to go back, or failing that, go to the home screen.
    function goBackOrGoHome() {
        if(WinJS.Navigation.canGoBack) {
            WinJS.Navigation.back();
        }
        else {
            navigate('home');
        }
    }

    // notify views when the window size changes so they can adapt to snap/full/fill
    function windowSizeChanged(e) {
        var viewState = Windows.UI.ViewManagement.ApplicationViewState;
        if (currentView) {
            if (Windows.UI.ViewManagement.ApplicationView.value === viewState.snapped) {
                currentView.relayout('snapped');
            }
            else {
                currentView.relayout('normal');
            }
        }
    }

    // save navigation history to the data store.
    // also let the current view save any state it needs to.
    function saveState() {
        if (currentView) {
            currentView.checkpoint();
        }
        ConferenceApp.State.local.navigationState = WinJS.Navigation.history;
    }

    // let the current view handle the share contract.
    function share() {
        if (currentView) {
            return currentView.share();
        }
        return null;
    }

    // let the current view handle the playTo contract.
    function playTo() {
        if (currentView) {
            return currentView.playTo();
        }
        return null;
    }

    // initialize the navigation handler after activation
    function init() {
        window.addEventListener('resize', windowSizeChanged);
    }

    WinJS.Navigation.addEventListener('beforenavigate', beforeNavigate);
    WinJS.Navigation.addEventListener('navigated', navigated);

    WinJS.Namespace.define("ConferenceApp.Navigation", {
        navigate: navigate,
        goBackOrGoHome: goBackOrGoHome,
        init: init,
        saveState: saveState,
        renderPage: renderPage,
        share: share,
        playTo: playTo
    });
  
})();
