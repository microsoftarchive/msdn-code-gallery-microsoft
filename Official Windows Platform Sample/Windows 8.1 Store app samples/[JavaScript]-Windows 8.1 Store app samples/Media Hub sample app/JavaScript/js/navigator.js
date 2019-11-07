//// Copyright (c) Microsoft Corporation. All rights reserved

// NOTE: The navigator used in this sample application is a modified version of the one shown
// in the Navigation VS sample.
(function () {
    "use strict";

    var nav = WinJS.Navigation;

    WinJS.Namespace.define("Application", {
        PageControlNavigator: WinJS.Class.define(
            // Define the constructor function for the PageControlNavigator.
            function PageControlNavigator(element, options) {
                this._element = element || document.createElement("div");
                this._element.appendChild(this._createPageElement());

                this._eventHandlerRemover = [];
                var that = this;
                function addRemovableEventListener(e, eventName, handler, capture) {
                    e.addEventListener(eventName, handler, capture);
                    that._eventHandlerRemover.push(function () {
                        e.removeEventListener(eventName, handler);
                    });
                };

                addRemovableEventListener(nav, 'navigating', this._navigating.bind(this), false);
                addRemovableEventListener(nav, 'navigated', this._navigated.bind(this), false);

                window.onresize = this._resized.bind(this);

                // Event listener for resource context changes: If app resources get
                // localized or have variants created for different high-contrast modes
                // or different scales, then the contextChanged event will occur at
                // runtime if language or scale or high contrast is changed on the 
                // system. 
                addRemovableEventListener(WinJS.Resources, "contextchanged", this._contextChanged.bind(this), false);

                Application.navigator = this;
            }, {
                // The home screen data
                home: (function () {
                    var groups = {
                        media: { key: "0", title: "Play" },
                        author: { key: "1", title: "Create" }
                    };

                    var data = [
                        new Application.Data.Folder({ folder: Windows.Storage.KnownFolders.videosLibrary, icon: WinJS.UI.AppBarIcon.video, group: groups.media }),
                        new Application.Data.Folder({ folder: Windows.Storage.KnownFolders.musicLibrary, icon: WinJS.UI.AppBarIcon.audio, group: groups.media }),
                        new Application.Data.Folder({ folder: Windows.Storage.KnownFolders.picturesLibrary, icon: WinJS.UI.AppBarIcon.pictures, group: groups.media }),
                        new Application.Data.Folder({ folder: Windows.Storage.KnownFolders.mediaServerDevices, icon: WinJS.UI.AppBarIcon.mapdrive, group: groups.media }),
                        new Application.Data.Link({
                            title: "Sound recorder", icon: WinJS.UI.AppBarIcon.microphone, group: groups.author, invoke: function () {
                                WinJS.Navigation.navigate("/pages/soundrecorder/soundrecorder.html");
                            }
                        }),
                        new Application.Data.Link({
                            title: "Webcam capture", icon: WinJS.UI.AppBarIcon.webcam, group: groups.author, invoke: function () {
                                Application.Capture.getWebcamMediaAsync().done(function (result) {
                                    (new Application.Data.Folder({ folder: result.folder })).invoke();
                                }, function (error) {
                                    if (error !== "cancel") {
                                        Windows.UI.Popups.MessageDialog(error, "Webcam capture failed").showAsync();
                                    }
                                });
                            }
                        }),
                        new Application.Data.Link({
                            title: "Convert media", icon: WinJS.UI.AppBarIcon.copy, group: groups.author, invoke: function () {
                                WinJS.Navigation.navigate("/pages/convert/convert.html");
                            }
                        })
                    ];

                    return {
                        title: "Media Hub",
                        list: new WinJS.Binding.List(data).createGrouped(
                            function (item) { return item.group.key; },
                            function (item) { return item.group; },
                            function (left, right) { return parseInt(left) - parseInt(right); })
                    };
                })(),

                _element: null,
                _lastNavigationPromise: WinJS.Promise.as(),
                _lastViewstate: 0,

                // This is the currently loaded Page object.
                pageControl: {
                    get: function () { return this.pageElement && this.pageElement.winControl; }
                },

                // This is the root element of the current page.
                pageElement: {
                    get: function () { return this._element.firstElementChild; }
                },

                // This function disposes the page navigator and its contents.
                dispose: function () {
                    if (this._disposed) {
                        return;
                    }

                    this._disposed = true;
                    WinJS.Utilities.disposeSubTree(this._element);
                    for (var i = 0; i < this._eventHandlerRemover.length; i++) {
                        this._eventHandlerRemover[i]();
                    }
                    this._eventHandlerRemover = null;
                },

                // Calls page implementation of updateResources to reload app resources in the page
                _contextChanged: function (e) {
                    // Could test e.detail.qualifier here to filter for changes on 
                    // specific resource qualifiers (e.g., scale, language, contrast).
                    // Will pass e to the page-specific implementations of 
                    // updateResources instead to allow finer control.
                    if (this.pageControl && this.pageControl.updateResources) {
                        this.pageControl.updateResources.call(this.pageControl, this.pageElement, e);
                    }
                },

                // Creates a container for a new page to be loaded into.
                _createPageElement: function () {
                    var element = document.createElement("div");
                    element.setAttribute("dir", window.getComputedStyle(this._element, null).direction);
                    element.style.width = "100%";
                    element.style.height = "100%";
                    return element;
                },

                _startAnimation: function () {
                },

                // Animate the current page using its custom animation, if one is
                // defined, or the default enterPage animation.
                _navigated: function () {
                    if (this.pageControl && this.pageControl.startAnimation) {
                        this.pageControl.startAnimation();
                    } else {
                        WinJS.UI.Animation.enterPage(this.pageElement);
                    }
                },

                // Responds to navigation by adding new pages to the DOM.
                _navigating: function (args) {
                    var newElement = this._createPageElement();
                    var parentedComplete;
                    var parented = new WinJS.Promise(function (c) { parentedComplete = c; });

                    this._lastNavigationPromise.cancel();

                    this._lastNavigationPromise = WinJS.Promise.timeout().then(function () {
                        return WinJS.UI.Pages.render(args.detail.location, newElement, args.detail.state, parented);
                    }).then(function parentElement(control) {
                        // Dispose BackButton control
                        var backButtons = document.querySelectorAll(".win-navigation-backbutton");
                        Array.prototype.forEach.call(backButtons, function (button) {
                            if (button.winControl) {
                                button.winControl.dispose();
                            }
                        });

                        var oldElement = this.pageElement;
                        if (oldElement.winControl && oldElement.winControl.unload) {
                            oldElement.winControl.unload();
                        }
                        WinJS.Utilities.disposeSubTree(this._element);
                        this._element.appendChild(newElement);
                        this._element.removeChild(oldElement);
                        oldElement.innerText = "";
                        parentedComplete();
                    }.bind(this));

                    args.detail.setPromise(this._lastNavigationPromise);
                },

                // Responds to resize events and call the updateLayout function
                // on the currently loaded page.
                _resized: function (args) {
                    if (this.pageControl && this.pageControl.updateLayout) {
                        this.pageControl.updateLayout.call(this.pageControl, this.pageElement);
                    }
                },
            }
        )
    });
})();
