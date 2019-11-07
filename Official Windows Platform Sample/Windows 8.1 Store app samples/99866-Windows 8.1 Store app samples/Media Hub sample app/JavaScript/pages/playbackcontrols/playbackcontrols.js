//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var ui = WinJS.UI;
    var player = Application.player;
    var playerUI = Application.Player.UI;
    var playerBinding = playerUI.binding;

    WinJS.UI.Pages.define("/pages/playbackcontrols/playbackcontrols.html", {
        processed: function (element, options) {
            var that = this;

            player.element.style.display = "none";

            return WinJS.Binding.processAll(element, playerBinding).then(function () {

                // When loading new media additional operations need to be done
                if (options.behavior === "load") {
                    Application.Player.UI.binding.title = options.title;

                    // If the source is an image just set the source and return since no media events will fire
                    if (options.source.contentType && options.source.contentType.indexOf("image") === 0) {
                        player.src = options.source;
                        return WinJS.Promise.as();
                    }

                    // If the source is video or audio set the source waiting for canplay
                    return player.element.eventAsync({
                        action: function () {
                            player.src = options.source;
                        },
                        event: "canplay"

                    }).then(function () {

                        // As soon as media can play start playback and finalize the chain
                        player.element.play();
                        return WinJS.Promise.as();
                    }, function (error) {
                        var message = "Unknown failure";

                        if (error.code === error.MEDIA_ERR_ABORTED) {
                            message = "The process of fetching the media resource was stopped at the user's request. ";
                        } else if (error.code === error.MEDIA_ERR_NETWORK) {
                            message = "A network error occurred while fetching the media resource.";
                        } else if (error.code === error.MEDIA_ERR_DECODE) {
                            message = "An error occurred while decoding the media resource.";
                        } else if (error.code === error.MEDIA_ERR_SRC_NOT_SUPPORTED) {
                            message = "The media resource is not supported.";
                        }

                        Windows.UI.Popups.MessageDialog(message, "Playback Error").showAsync();

                        WinJS.Navigation.back();
                    });
                } else {

                    // When not loading media just finalize the chain
                    return WinJS.Promise.as();
                }
            });
        },

        ready: function (element, options) {
            // Bind the custom controls class name to be able to animate the transition
            playerBinding.bind("customControlsClassName", this._controlsStateChanged);

            // Need to handle scrubbing no matter where the pointer is located
            document.addEventListener("pointerup", this.disableSrubbing, false);
            document.addEventListener("pointermove", this.setTimeFromMouseEvent, false);

            element.querySelector(".timeContainer .clickeater").onpointerdown = Application.navigator.pageControl.enableSrubbing;

            element.querySelector(".loop").onclick = function () {
                Application.player.loop = !Application.player.loop;
            };

            element.querySelector(".zoom").onclick = function () {
                Application.player.zoom = !Application.player.zoom;
            };

            element.querySelector(".play").onclick = function () {
                Application.player.element[(Application.player.element.paused) ? 'play' : 'pause']();
            };

            element.querySelector(".stabilization").onclick = function () {
                Application.player.stabilization = !Application.player.stabilization;
            };

            element.querySelector(".stereo").onclick = function () {
                Application.player.stereo = !Application.player.stereo;
            };

            element.querySelector(".playto").onclick = function () {
                Windows.Media.PlayTo.PlayToManager.showPlayToUI();
            };

            element.querySelector(".track").onclick = function () {
                Application.player.changeAudioTrack();
            };

            player.showControls = true;
            player.fullscreen = true;
        },

        enableSrubbing: function (args) {

            // Enables scrubbing and disables CSS transitions for better performance
            var timeContainerProgress = Application.navigator.pageElement.querySelector(".timeContainer .progress");
            var timeContainerTracker = Application.navigator.pageElement.querySelector(".timeContainer .tracker");
            timeContainerProgress.style.transition = "none";
            timeContainerTracker.style.transition = "none";
            player.scrubbing = true;
        },

        disableSrubbing: function (args) {

            // Disables scrubbing and enables CSS transitions for better presentation
            if (player.scrubbing) {
                var timeContainerProgress = Application.navigator.pageElement.querySelector(".timeContainer .progress");
                var timeContainerTracker = Application.navigator.pageElement.querySelector(".timeContainer .tracker");
                timeContainerProgress.style.transition = "";
                timeContainerTracker.style.transition = "";

                // Need to update the position since no mouse move event may have fired
                Application.navigator.pageControl.setTimeFromMouseEvent(args);
                player.scrubbing = false;
            }
        },

        setTimeFromMouseEvent: function (args) {
            if (player.scrubbing) {

                // Calculates the new position based on the mouse event, if coming from the timeline click-eater use it
                // as a reference, otherwise use the whole screen
                var position;
                if (args.srcElement.className === "clickeater") {
                    position = (args.offsetX + args.srcElement.offsetLeft) / args.srcElement.parentNode.clientWidth;
                } else {
                    position = args.screenX / screen.width;
                }

                // Sets the current time of the video tag
                player.element.currentTime = Math.min(1, Math.max(0, position)) * player.element.duration;
            }
        },

        // When the player UI custom control visibiliy state changes animate our HTML elements
        _controlsStateChanged: function (value) {
            var elements = document.querySelector(".playbackcontrolspage div").children;
            var contentHost = document.getElementById("contenthost");
            if (value === playerUI.KnownClassNames.customControls.show) {
                contentHost.style.display = "";
                WinJS.UI.Animation.fadeIn(elements);
            } else {
                WinJS.UI.Animation.fadeOut(elements).done(function () {
                    contentHost.style.display = (player.showControls) ? "" : "none";
                });
            }
        },

        // Unload the playback controls
        unload: function () {

            // Switch to non fullscreen mode
            player.fullscreen = false;
            playerBinding.unbind("customControlsClassName", this._controlsStateChanged);
            document.removeEventListener("MSPointerUp", this.disableSrubbing);
            document.removeEventListener("MSPointerMove", this.setTimeFromMouseEvent);
        },

        // reloads app resources in the page
        updateResources: function (element, e) {
            // Called by _contextChanged event handler in navigator.js when a resource 
            // qualifier (language, scale, contrast, etc.) has changed. The element 
            // passed is the root of this page. 
            //
            // Since this sample app currently doesn't have any assets with variants
            // for scale/contrast/language/etc., the lines below that do the actual 
            // work are commented out. This is provided here to model how to handle 
            // scale or other resource context changes if this app were expanded to 
            // include resources with assets for such variantions.

            // Will filter for changes to specific qualifiers.
            if (e.detail.qualifier === "Scale" || e.detail.qualifier === "Contrast") {
                // if there are string resources bound to properties using data-win-res,
                // the following line will update those properties: 

                //WinJS.Resources.processAll(element);

                // Background images from the app package with variants for scale, etc. 
                // are automatically reloaded by the platform when a resource context 
                // qualifier has changed. That is not done, however, for img elements. 
                // The following will make sure those are updated:

                //var imageElements = document.getElementsByTagName("img");
                //for (var i = 0, l = imageElements.length; i < l; i++) {
                //    var previousSource = imageElements[i].src;
                //    var uri = new Windows.Foundation.Uri(document.location, previousSource);
                //    if (uri.schemeName === "ms-appx") {
                //        imageElements[i].src = "";
                //        imageElements[i].src = previousSource;
                //    }
                //}
            }
        },

        // Custom animation for this page; note that the video tag itself is not a child of the
        // page's element
        startAnimation: function () {
            player.element.style.display = "";
            WinJS.UI.Animation.fadeIn(player.element);
            WinJS.UI.Animation.enterPage(this.element);
        }

    });
})();
