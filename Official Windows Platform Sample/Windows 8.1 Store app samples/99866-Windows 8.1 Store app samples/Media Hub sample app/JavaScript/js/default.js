//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // Add a useful method for media elements that wraps actions that expect an
    // event in a Promise that handles errors
    HTMLMediaElement.prototype.eventAsync = function (options) {
        var that = this;
        return new WinJS.Promise(function (complete, error) {
            var handler = function () {
                that.removeEventListener(options.event, handler);
                that.removeEventListener("error", handler);

                if (that.error) {
                    error(that.error);
                } else {
                    complete();
                }
            };
            that.addEventListener(options.event, handler, false);
            that.addEventListener("error", handler, false);
            options.action();
        });
    };

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    var nav = WinJS.Navigation;

    // Handle activation
    app.addEventListener("activated", function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (app.sessionState.history) {
                nav.history = app.sessionState.history;
            }
            args.setPromise(WinJS.UI.processAll().then(function () {
                WinJS.Binding.processAll(document.body, Application.Player.UI.binding);
                var appBars = document.querySelectorAll(".win-appbar");

                // Define handlers for appbar buttons
                document.getElementById("appbarOpen").addEventListener("click", function () {
                    var picker = new Windows.Storage.Pickers.FileOpenPicker();
                    picker.fileTypeFilter.replaceAll(["*"]);
                    picker.pickSingleFileAsync().done(function (file) {
                        if (file) {
                            new Application.Data.File({ file: file }).invoke();
                        }
                    });
                }, false);

                document.getElementById("appbarPlay").addEventListener("click", function () {
                    Application.player.element[(Application.player.element.paused) ? 'play' : 'pause']();
                }, false);

                document.getElementById("appbarPlayTo").addEventListener("click", function () {
                    Windows.Media.PlayTo.PlayToManager.showPlayToUI();
                }, false);

                // The player object changes the displayClassName property when the video changes from 
                // fullscreen to non-fullscreen mode, by handling this here the app can disable appbars when
                // getting into fullscreen mode avoiding playback interference.
                Application.Player.UI.binding.bind("displayClassName", function () {
                    for (var index = 0, size = appBars.length; index < size; index++) {
                        appBars[index].winControl.disabled = Application.player.fullscreen;
                    }
                });

                // When the video thumbnail is clicked navigate to the playback controls
                Application.player.element.addEventListener("click", function () {
                    if (!Application.player.fullscreen) {
                        nav.navigate("/pages/playbackcontrols/playbackcontrols.html", { behavior: "restore" });
                    }
                }, false);

                return nav.navigate("/pages/browser/browser.html", Application.navigator.home);
            }));
        }
    });

    app.start();
})();
