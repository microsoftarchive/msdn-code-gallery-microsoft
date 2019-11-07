//// Copyright (c) Microsoft Corporation. All rights reserved

// The objects defined here demonstrate how to make sure each of the views created remains alive as long as 
// the app needs them, but only when they're being used by the app or the user. Many of the scenarios contained in this
// sample use these functions to keep track of the views available and ensure that the view is not closed while
// the scenario is attempting to show it.
//
// As you can see in scenario 1, the ApplicationViewSwitcher.TryShowAsStandaloneAsync and 
// ProjectionManager.StartProjectingAsync methods let you show one view next to another. The Consolidated event
// is fired when a view stops being visible separately from other views. Common cases where this will occur
// is when the view falls out of the list of recently used apps, or when the user performs the close gesture on the view.
// This is a good time to close the view, provided the app isn't trying to show the view at the same time. This event
// is fired on the thread of the view that becomes consolidated.
//
// Each view lives on its own thread. JavaScript threads may only communicate via postMessage and may not share memory. 
// Hence, before the secondary view closes itself (after receiving the Consolidated event), it must ask the main view
// if it's safe to close the view. These helpers provide that functionality.
//
// Additionally, the main view maintains a list of the existing secondary views and some state about them (like their
// current title). These helpers provides that functionality.

// Here's an overview of this overall flow:
// On the main view's thread: ViewManager should be initialized once for the whole app. ViewManager tracks each of the 
// secondary views for the app. As views are created using ViewManager's createNewView wrapper function, a secondary view is created,
// and a ViewLifetimeControlProxy object is created for that secondary view and added to the ViewManager's collection.
//
// On each secondary view's thread: A ViewLifetimeControl object is created. The ViewLifetimeControl object tracks when
// the secondary view is in use (perhaps because it is visible). 
//
// ViewLifetimeControl and ViewLifetimeControlProxy maintain separate reference counts. ViewLifetimeControl's reference count
// keeps track of when the secondary view thinks it's in use (because it's visible, because it's running an async operation, etc.), and
// ViewLifetimeControlProxy's reference count keeps track of when the main view is interacting with the secondary view (about to show
// it to the user, etc.) When the reference count in ViewLifetimeControl and ViewLifetimeControlProxy drop to zero, the secondary view is closed.
// Details for how the reference counts are synchronized are contained in the flows below:
//
// Actions that can occur:
// A. Main window creates a view (createNewView)
// 1. A secondary view is created using MSApp.createNewView().
// 2. A ViewLifetimeControl object is created on the secondary view's thread
// 3. A ViewLifetimeControlProxy object is created on the main view's thread, and added to ViewManager's collection.
//
// B. A secondary view becomes visible
// 1. The main view is about to show a secondary view, so it should call startViewInUse. This increments the proxy's reference count
// 2. The main view calls ApplicationViewSwitcher.switchAsync, ApplicationViewSwitcher.tryShowAsStandaloneAsync, or ProjectionManager.startProjectingAsync
// 3. The secondary view becomes visible. ViewLifetimeControl on that view's thread increments its reference count.
// 4. The async method used to show the view returns (it always returns after the view has become visible) on the main view. The main view should
//    call stopViewInUse.
//
// C. A secondary view changes from visible to consolidated (_onConsolidated)
// 1. ViewLifetimeControl's reference count is decremented.
// 2. If the reference count is not zero, then stop. Otherwise, ViewLifetimeControl asks its respective proxy on the main view's thread
//    if the main view is interacting with the secondary view.
// 3. If main view is not interacting with the secondary view (if the reference count on the proxy is 0), it removes itself from the list of active proxies
//    and posts proxyReleased to the view.
// 4. The view receives the proxyReleased message and closes itself.

(function () {
    // Alias declaration
    var ViewManagement = Windows.UI.ViewManagement;

    // Each view lives on its own thread. JavaScript threads may only communicate via postMessage 
    // and may not share memory. But, the secondary view needs to know if the main view is trying
    // to show it so that it doesn't accidentally close itself while the view is being shown.
    // This predefined set of messages is used to communicate that state. Each of these messages is only relevant
    // to the code presented to the sample (in other words, the message are meaningless to the platform.)
    var messageTypePrefix = "SecondaryViewHelper_"
    var messageTypes = {
        // This message is posted from the secondary view to the main view to see if it's OK for the secondary view
        // to close itself
        queryProxyReadyForRelease: messageTypePrefix + "queryProxyReadyForRelease",

        // The main view returns with this message when it's no longer interacting with the secondary view.
        proxyReadyForRelease: messageTypePrefix + "proxyReadyForRelease",

        // The main view posts this message to the secondary view when it has removed the secondary view from its
        // tracking collection. The secondary view should close itself at this point
        proxyReleased: messageTypePrefix + "proxyReleased",

        // The secondary view postss this message to the main view whenever the title is changed on the secondary view.
        // This is an example of information about the contents of the secondary view that the main view may be
        // interested in.
        titleUpdated: messageTypePrefix + "titleUpdated",

        // When the main view creates a secondary view, the main view posts the message to the secondary view along
        // with any information necessary to set up the secondary view.
        initialize: messageTypePrefix + "initialize"
    };

    // A placeholder title used when the user clears the title of the view
    var emptyTitle = "<title cleared>";
    
    WinJS.Namespace.define("SecondaryViewsHelper", {
        // Store the app's domain. It's necessary to validate the origin of messages received
        // from postMessage. See http://go.microsoft.com/fwlink/?LinkID=247104 for more details
        thisDomain: document.location.protocol + "//" + document.location.host,

        // The ViewManager should be used on the main view (and only one should be created.)
        // It's used to keep track of the list of open secondary views, and to create new secondary views.
        ViewManager: WinJS.Class.define(function ViewManager_ctor() {
            this._viewReleasedWrapper = this._viewReleased.bind(this);
            window.addEventListener("message", this._handleMessage.bind(this), false);
        },
        {
            // Receives messages from the secondary views about when the secondary view is consolidated
            // (since it has gone unused by the user), when its contents or title change, etc.
            // Each message is handled by a ViewLifetimControlProxy.
            _handleMessage: function ViewManager_handleMessage(e) {
                if (e.origin === SecondaryViewsHelper.thisDomain && e.data.type) {
                    var i = this.findViewIndexByViewId(e.data.viewId);
                    if (i !== null) {
                        this.secondaryViews.getItem(i).data._handleMessage(e);
                    }
                }
            },

            // Fires when a particular secondary view is closed. Used to clean the view
            // out of the ViewManager's collection.
            _viewReleased: function ViewManager_viewReleased(e) {
                e.target.removeEventListener("released", this._viewReleasedWrapper, false);
                var i = this.findViewIndexByViewId(e.target.viewId);
                if (i !== null) {
                    this.secondaryViews.splice(i, 1);
                }
            },

            // The list of open secondary views. A WinJS.Binding.List is not strictly necesssary, but is used so the 
            // list of views can be easily shown in this sample.
            secondaryViews: new WinJS.Binding.List([]),

            // Each view has a unique Id, found using the ApplicationView.Id property or
            // ApplicationView.GetApplicationViewIdForCoreWindow method. This id is used in all of the ApplicationViewSwitcher
            // and ProjectionManager APIs. This function searches the list of views the ViewManager knows about for a
            // view with the matching id.
            findViewIndexByViewId: function ViewManager_findViewIndexByViewId(viewId) {
                for (var i = 0, len = this.secondaryViews.length; i < len; i++) {
                    var value = this.secondaryViews.getItem(i).data;
                    if (viewId === value.viewId) {
                        return i;
                    }
                }
                return null;
            },

            // This function creates a new secondary view and adds that view to the list of secondary views
            // ViewManager tracks
            createNewView: function ViewManager_createNewView(page, initData) {
                // When creating a new view, you must specify which page you would like loaded in that view.
                // The page must be contained inside your package.
                if (!page) {
                    throw "Must specify a URL of a page from your app to show in the new view";
                }

                // Create a new view. The specified page will be loaded inside the view, but the view
                // will not be shown yet. You'll need to use the ApplicationViewSwitcher or ProjectionManager
                // APIs to show the view
                var newView = MSApp.createNewView(page);

                // The secondary view may need to load a particular document or piece of content. You can
                // tell the secondary view what to load by posting a message to it.
                newView.postMessage({
                    type: messageTypes.initialize,
                    initData: initData || {} 
                }, SecondaryViewsHelper.thisDomain);

                // The main view needs to keep track of the secondary views, and prevent the view from closing
                // while it's trying to show that view. The ViewLifetimeControlProxy class encapsulates
                // this logic. When a new view is created, a ViewLifetimeControlProxy must be created as well.
                var newProxy = new SecondaryViewsHelper.ViewLifetimeControlProxy(newView);
                newProxy.addEventListener("released", this._viewReleasedWrapper, false);
                this.secondaryViews.push(newProxy);
                return newProxy;
            }
        }),

        // This class is used by the main view to keep track of the state of each of the secondary views.
        // Recall that, each view lives on its own thread. JavaScript threads may only communicate via postMessage
        // and may not share memory. Before a secondary view closes itself (after receiving "Consolidated"), it should ask the
        // main view if the main view was about to interact with the secondary view. If so, then the view should delay closing
        // itself. This object encapsulates the main view's portion of this interaction.
        ViewLifetimeControlProxy: WinJS.Class.mix(WinJS.Class.define(function ViewLifetimeControlProxy_ctor(appView) {
            // There should be one instantiation of this class for each secondary view
            this.appView = appView;
            this.viewId = appView.viewId;
            this.title = "";
        },
        {
            // This class uses reference counts to make sure the secondary views isn't closed prematurely.
            // Whenever the main view is about to interact with the secondary view, it should take a reference
            // by calling "StartViewInUse" on this object. When finished interacting, it should release the reference
            // by calling "StopViewInUse". You can see examples of this throughout the sample, especially in
            // scenario 1.
            _refCount: 0,

            // A wrapper around postMessage for sending messages to the secondary view for this particular object
            _alertView: function ViewLifetimeControlProxy_alertView(type, data) {
                if (!type) {
                    throw "Must specify a type of message to send to the proxy";
                }
                data = data || {};
                data.type = type;

                this.appView.postMessage(data, SecondaryViewsHelper.thisDomain);
            },

            // Receives messages from the secondary view for this particular object.
            // The messages are validated and routed by the ViewManager object living on the main view's thread
            _handleMessage: function ViewLifetimeControlProxy_handleMessage(e) {
                // Origin of the message has already been validated by ViewManager._handleMessage
                var data = e.data;
                switch (data.type) {
                    case messageTypes.queryProxyReadyForRelease:

                        // The secondary view may be ready to be closed (e.g., it has been consolidated).
                        // Make sure the main view is done with it, and, if so, get rid of this wrapper.
                        if (this._refCount === 0) {
                            this.dispatchEvent("released");
                            this._alertView(messageTypes.proxyReleased);
                        }
                        break;
                    case messageTypes.titleUpdated:

                        // For purposes of this sample, the collection of views
                        // is bound to a UI collection. This property is available for binding.
                        var oldValue = this.title;
                        this.title = data.title;
                        this.notify("title", this.title, oldValue);
                        break;
                }
            },

            // Stores the MSAppView object, which is used to communicate with the secondary view
            appView: null,

            // Signals that the secondary view is being interacted with by the main view,
            // so it shouldn't be closed even if it becomes "consolidated" (goes unused by the user)
            startViewInUse: function ViewLifetimeControlProxy_startViewInUse() {
                this._refCount++;
            },

            // Signals that the secondary view is no longer being interacted with by this view.
            stopViewInUse: function ViewLifetimeControlProxy_stopViewInUse() {
                this._refCount--;

                if (this._refCount === 0) {
                    this._alertView(messageTypes.proxyReadyForRelease);
                }
            }
        }), WinJS.Utilities.eventMixin, WinJS.Binding.observableMixin),

        // This class is used by the secondary view to automatically keep track of when it should close itself
        // Recall that, each view lives on its own thread. JavaScript threads may only communicate via postMessage
        // and may not share memory. Before a secondary view closes itself (after receiving "Consolidated"), it should ask the
        // main view if the main view was about to interact with the secondary view. If so, then the view should delay closing
        // itself. This object encapsulates the secondary view's portion of that interaction
        ViewLifetimeControl: WinJS.Class.mix(WinJS.Class.define(function () {
            this.opener = MSApp.getViewOpener();
            this._handleMessageWrapper = this._handleMessage.bind(this);
            this._onConsolidatedWrapper = this._onConsolidated.bind(this)
            this._onVisibilityChangeWrapper = this._onVisibilityChange.bind(this);
            this._finalizeReleaseWrapper = this._finalizeRelease.bind(this);
            this.viewId = ViewManagement.ApplicationView.getForCurrentView().id;
        }, 
        {
            // This class uses reference counts to make sure the secondary views isn't closed prematurely.
            // Whenever the secondary view starts an asynchronous operation, it should take a reference
            // by calling "StartViewInUse" on this object. When the async operation completes, it should release the reference
            // by calling "StopViewInUse". You can see examples of this throughout the sample.
            _refCount: 0,

            // Keeps track of if the main view has removed the secondary view from its collection.
            // This is set to true right before the view closes itself.
            _proxyReleased: false,

            // Keeps track of if the consolidated event has fired yet. A view is consolidated with other views
            // when there's no way for the user to get to it (it's not in the list of recently used apps, cannot be
            // launched from Start, etc.) A view stops being consolidated when it's visible--at that point
            // the user can interact with it, move it on or off screen, etc. 
            _consolidated: true,

            // A wrapper around postMessage for communicating with the main view about changes to the state
            // of the secondary view
            _alertProxy: function ViewLifetimeControlProxy_alertProxy(type, data) {
                if (!type) {
                    throw "Must specify a type of message to send to the proxy";
                }
                data = data || {};
                data.type = type;
                data.viewId = this.viewId;

                this.opener.postMessage(data, SecondaryViewsHelper.thisDomain);
            },

            // Used to process messages posted by the main view to this secondary view
            _handleMessage: function ViewLifetimeControlProxy_handleMessage(e) {
                if (e.origin === SecondaryViewsHelper.thisDomain && e.data.type) {
                    var data = e.data;
                    switch (data.type) {

                        // The main view has removed the secondary view from its collection.
                        // At this point, the secondary view should be ready to close itself.
                        case messageTypes.proxyReleased:
                            this._proxyReleased = true;
                            setImmediate(this._finalizeReleaseWrapper);
                            break;

                        // The main view has finished operations with this view. If the secondary view
                        // is ready to close, it should tell the main view so.
                        case messageTypes.proxyReadyForRelease:
                            if (this._refCount === 0) {
                                this._alertProxy(messageTypes.queryProxyReadyForRelease);
                            }
                            break;

                            // Data sent to get this view working (depends on the context where the
                            // view is launched.)
                        case messageTypes.initialize:
                            this.dispatchEvent("initializedatareceived", e.data.initData);
                            break;
                    }
                }
            },

            // Handler for the consolidated event, which fires when the view no longer accessible to the user (e.g.
            // it falls out of the list of recently used apps). It's a good idea to close consolidated views,
            // provided the main view isn't trying to show it.
            _onConsolidated: function ViewLifetimeControlProxy_onConsolidated() {
                this._setConsolidated(true);
            },

            // Handler for this view's visibility change events.
            // If a view becomes visible, the user is engaging with it, so it's a good idea not to close it
            _onVisibilityChange: function ViewLifetimeControlProxy_onVisibilityChange() {
                if (!document.hidden) {
                    this._setConsolidated(false);
                }
            },

            // Used to track if a view is consolidated or not. Recall that reference counting is used to
            // manage the lifetime of the secondary view in this sample.
            _setConsolidated: function ViewLifetimeControlProxy_setConsolidated(value) {
                if (this._consolidated !== value) {
                    this._consolidated = value;
                    
                    // Being visible is treated as taking a single reference on the view (which is released when 
                    // the view becomes consolidated).
                    if (value) {
                        this.stopViewInUse();
                    } else {
                        this.startViewInUse();
                    }
                }
            },

            // Called when a view has been "consolidated" (no longer accessible to the user) 
            // and no other view is trying to interact with it. This should only be called after the main view has
            // removed the view from its collection (stopViewInUse takes care of notifying the main view). At the end of this,
            // the view is closed. 
            _finalizeRelease: function ViewLifetimeControlProxy_finalizeRelease() {
                if (this._refCount === 0) {
                    // Release event registrations from this object
                    window.removeEventListener("message", this._handleMessageWrapper, false);
                    ViewManagement.ApplicationView.getForCurrentView().removeEventListener("consolidated", this._onConsolidatedWrapper, false);
                    document.removeEventListener("visibilitychange", this._onVisibilityChangeWrapper, false);

                    // Fire a custom event to give the rest of the view a chance to clean up. You can register for this event.
                    this.dispatchEvent("released");

                    window.close();
                }
            },

            // Sets the title on the view which is visible in the list of recently used apps and, in this sample,
            // the list of secondary views shown in the main view
            setTitle: function ViewLifetimeControlProxy_setTitle(value) {
                ViewManagement.ApplicationView.getForCurrentView().title = value;

                // Setting the title on ApplicationView to blank will clear the title in
                // the system switcher. It would be good to still have a title in the app's UI.
                if (!value) {
                    value = emptyTitle; 
                }
                this._alertProxy(messageTypes.titleUpdated, { title: value });
            },

            // Add all event listeners. This allows you to attach events to this object
            // before it begins processing messages.
            initialize: function ViewLifetimeControlProxy_initialize() {
                window.addEventListener("message", this._handleMessageWrapper, false);

                // This class will automatically tell the view when its time to close
                // or stay alive in a few cases.
                //
                // Views that are consolidated are no longer accessible to the user,
                // so it's a good idea to close them.
                ViewManagement.ApplicationView.getForCurrentView().addEventListener("consolidated", this._onConsolidatedWrapper, false);

                // On the other hand, if a view becomes visible, the user is engaging
                // with it, so it's a good idea not to close it
                document.addEventListener("visibilitychange", this._onVisibilityChangeWrapper, false);
            },

            // Signals that there are outstanding async operations, so don't close this view.
            startViewInUse: function ViewLifetimeControlProxy_startViewInUse() {
                this._refCount++;
            },

            // Should come after any call to StartViewInUse.
            // Signals that this view has finished async operations in it.
            stopViewInUse: function ViewLifetimeControlProxy_stopViewInUse() {
                this._refCount--;

                if (this._refCount === 0) {
                    if (this._proxyReleased) {

                        // If no other view is interacting with this view, and
                        // the view isn't accessible to the user, it's appropriate
                        // to close it.
                        //
                        // Before actually closing the view, make sure there are no
                        // other important events waiting in the queue.
                        setImmediate(this._finalizeReleaseWrapper);
                    } else {

                        // Check if the main view is not interacting with this view.
                        this._alertProxy(messageTypes.queryProxyReadyForRelease);
                    }
                }
            }
        }), WinJS.Utilities.eventMixin)
    });
})();