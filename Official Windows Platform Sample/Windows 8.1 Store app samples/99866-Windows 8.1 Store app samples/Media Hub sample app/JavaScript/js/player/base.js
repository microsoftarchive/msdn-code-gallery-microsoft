//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var playerUI = Application.Player.UI;

    var player = WinJS.Class.define(function (element, options) {
        var that = this;

        that.element = element;

        // Create an element to be used when swapping from fullscreen to non-fullscreen modes
        that._referenceElement = document.createElement("span");
        that._referenceElement.style.display = "none";
        document.body.insertBefore(that._referenceElement, document.body.children[0]);

        // Update the player internal states certain video tag events
        that.element.addEventListener("emptied", that._updateState.bind(that));
        that.element.addEventListener("canplaythrough", that._updateState.bind(that));
        that.element.addEventListener("pause", that._updateState.bind(that));
        that.element.addEventListener("play", that._updateState.bind(that));
        that.element.addEventListener("ended", that._videoEnded.bind(that));
        that.element.addEventListener("click", that._videoClick.bind(that));
        that.element.addEventListener("durationchange", that._videoDurationChange.bind(that));

        // To improve performance only register timeupdate when controls are shown
        that._videoTimeUpdate = that._videoTimeUpdate.bind(that);
        playerUI.binding.bind("customControlsClassName", function (value) {
            if (value === playerUI.KnownClassNames.customControls.show) {
                that.element.addEventListener("timeupdate", that._videoTimeUpdate);
            } else {
                that.element.removeEventListener("timeupdate", that._videoTimeUpdate);
            }
        });


        // Handle PlayTo requests
        var playToManager = Windows.Media.PlayTo.PlayToManager.getForCurrentView();
        playToManager.addEventListener("sourcerequested", function (e) {
            e.sourceRequest.setSource(that.element.msPlayToSource);
        });

        // Handle transport controls
        that._transportControls = Windows.Media.SystemMediaTransportControls.getForCurrentView();
        that._transportControls.addEventListener("buttonpressed", function (args) {
            if (args.button === Windows.Media.SystemMediaTransportControlsButton.play) {
                that.element.play();
            } else if (args.button === Windows.Media.SystemMediaTransportControlsButton.pause) {
                that.element.pause();
            }
        }, false);

        Application.player = that;
    },
    {
        _displayRequest: new Windows.System.Display.DisplayRequest(),
        _displayRequestActive: false,

        // Enables or disables screen power state changes like the screen
        // turning off after a few minutes.
        _changeDisplayRequest: function (value) {
            try {
                if (!value && this._displayRequestActive) {
                    this._displayRequest.requestRelease();
                    this._displayRequestActive = false;
                } else if (value && !this._displayRequestActive) {
                    this._displayRequest.requestActive();
                    this._displayRequestActive = true;
                }
            } catch (error) { } // Ignore errors
        },
        _transportControls: null,
        _enabledAudioTrack: null,
        _referenceElement: null,
        _updateState: function () {

            // Update the player state depending on the state of the video tag
            if (this.element.readyState < this.element.HAVE_FUTURE_DATA && !this.sourceIsPicture) {
                playerUI.binding.stateClassName = playerUI.KnownClassNames.state.unavailable;

                // If no media is loaded enable display power state changes
                this._changeDisplayRequest(false);
            } else if (this.element.paused || this.element.ended) {
                playerUI.binding.stateClassName = playerUI.KnownClassNames.state.paused;
                this._transportControls.playbackStatus = Windows.Media.MediaPlaybackStatus.paused;

                // If media is paused or ended enable display power state changes
                this._changeDisplayRequest(false);
            } else {
                playerUI.binding.stateClassName = playerUI.KnownClassNames.state.playing;
                this._transportControls.playbackStatus = Windows.Media.MediaPlaybackStatus.playing;

                // Disable display power state changes when playing a video 
                this._changeDisplayRequest(this.sourceIsVideo ? true : false);
            }
        },
        _videoEnded: function () {
            // When ended seek back to the start
            this.element.pause();
            this.element.currentTime = 0;
        },
        _videoClick: function () {
            // If the video is clicked change the control's state
            this.showControls = !this.showControls;
        },
        _videoDurationChange: function () {
            // Update the duration state class name
            if (this.element.duration === NaN || this.element.duration === Infinity) {
                playerUI.binding.durationClassName = playerUI.KnownClassNames.duration.unknown;
            } else {
                playerUI.binding.durationClassName = playerUI.KnownClassNames.duration.known;
            }
            playerUI.binding.durationAsText = Application.Data.timeAsText(this.element.duration);
        },
        _videoTimeUpdate: function () {
            // Update the time text and percentage
            playerUI.binding.currentTimeAsText = Application.Data.timeAsText(this.element.currentTime);
            if (playerUI.binding.durationClassName === playerUI.KnownClassNames.duration.known) {
                playerUI.binding.currentTimeAsPercentage = ((this.element.currentTime / this.element.duration) * 100) + "%";
            }
        },
        element: null,
        src: {
            get: function () { return this.element.src; },

            // Sets the source of the video tag
            set: function (value) {
                var that = this;

                // Reset the background image
                that.element.style.backgroundImage = "";

                // Remove effects if any are present and add stabilization if needed
                that.element.msClearEffects();
                if (playerUI.binding.stabilizationClassName === playerUI.KnownClassNames.stabilization.enabled) {
                    that.element.msInsertVideoEffect(Windows.Media.VideoEffects.videoStabilization, false);
                }

                // Revoke the object URL if one is being used
                if (that.element.src && that.element.src.indexOf("blob") === 0) {
                    URL.revokeObjectURL(that.element.src);
                }

                // If possible get the content type and update its state
                if (typeof value === "string") {
                    playerUI.binding.sourceTypeClassName = playerUI.KnownClassNames.sourceType.unknown;
                } else {
                    if (value.contentType) {
                        if (value.contentType.indexOf("video") === 0) {
                            playerUI.binding.sourceTypeClassName = playerUI.KnownClassNames.sourceType.video;
                        } else if (value.contentType.indexOf("audio") === 0) {
                            playerUI.binding.sourceTypeClassName = playerUI.KnownClassNames.sourceType.music;
                        } else if (value.contentType.indexOf("image") === 0) {
                            playerUI.binding.sourceTypeClassName = playerUI.KnownClassNames.sourceType.picture;
                        } else {
                            playerUI.binding.sourceTypeClassName = playerUI.KnownClassNames.sourceType.unknown;
                        }
                    }
                }

                // Enable background playback 
                that.element.msAudioCategory = "BackgroundCapableMedia";

                (function () {
                    if (that.sourceIsPicture) {

                        // If the source is a picture just set the video tag background to the URL
                        that.element.removeAttribute("src");
                        that.element.load();
                        that.element.style.backgroundImage = "url('" + ((typeof value === "string") ? value : URL.createObjectURL(value, { oneTimeOnly: true })) + "')";
                        that._updateState.bind(that)();
                        return WinJS.Promise.as();
                    } else {
                        return that.element.eventAsync({
                            action: function () {
                                that.element.src = (typeof value === "string") ? value : URL.createObjectURL(value, { oneTimeOnly: false });
                            },
                            event: "loadedmetadata"
                        });
                    }
                })().done(function () {
                    if (that.sourceIsUnknown) {

                        // If the videoWidth is 0 this is most probably audio
                        playerUI.binding.sourceTypeClassName = playerUI.KnownClassNames.sourceType[
                            (that.element.videoWidth !== 0) ? "video" : "music"
                        ];
                    }

                    // Find out which audio track is enabled by default. Note that multiple tracks
                    // can be enabled at the same time but this app always enable a single one
                    if (that.sourceIsMusic || that.sourceIsVideo) {
                        that._enabledAudioTrack = null;
                        for (var index = 0, size = that.element.audioTracks.length; index < size; index++) {
                            if (that._enabledAudioTrack === null && that.element.audioTracks[index].enabled) {
                                that._enabledAudioTrack = index;
                            } else {
                                that.element.audioTracks[index].enabled = false;
                            }
                        }
                    }

                    if (that.sourceIsMusic) {
                        that._transportControls.displayUpdater.type = Windows.Media.MediaPlaybackType.music;
                        that._transportControls.displayUpdater.musicProperties.title = value.displayName || "" + value;
                    } else if (that.sourceIsVideo) {
                        that._transportControls.displayUpdater.type = Windows.Media.MediaPlaybackType.video;
                        that._transportControls.displayUpdater.videoProperties.title = value.displayName || "" + value;
                    } else if (that.sourceIsPicture) {
                        that._transportControls.displayUpdater.type = Windows.Media.MediaPlaybackType.image;
                        that._transportControls.displayUpdater.imageProperties.title = value.displayName || "" + value;
                    } else {
                        that._transportControls.displayUpdater.type = Windows.Media.MediaPlaybackType.unknown;
                    }

                    that._transportControls.isPauseEnabled = true;
                    that._transportControls.isPlayEnabled = true;
                    that._transportControls.displayUpdater.update();

                    if (value.getThumbnailAsync) {
                        that._transportControls.displayUpdater.copyFromFileAsync(that._transportControls.displayUpdater.type, value).done(function () {
                            that._transportControls.displayUpdater.update();
                        });

                        if (that.sourceIsMusic) {
                            value.getThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.singleItem, Math.min(screen.width, screen.height) / 2).done(function (thumbnail) {
                                that.element.style.backgroundImage = "url('" + URL.createObjectURL(thumbnail, { oneTimeOnly: true }) + "')";
                            });
                        }
                    }
                }, function () { }); // Ignore loading errors, the user should handle video element error events
            }
        },

        sourceIsVideo: { get: function () { return playerUI.binding.sourceTypeClassName === playerUI.KnownClassNames.sourceType.video; } },
        sourceIsMusic: { get: function () { return playerUI.binding.sourceTypeClassName === playerUI.KnownClassNames.sourceType.music; } },
        sourceIsPicture: { get: function () { return playerUI.binding.sourceTypeClassName === playerUI.KnownClassNames.sourceType.picture; } },
        sourceIsUnknown: { get: function () { return playerUI.binding.sourceTypeClassName === playerUI.KnownClassNames.sourceType.unknown; } },

        // Enables the next audio track 
        changeAudioTrack: function () {
            if (this._enabledAudioTrack !== null) {
                this.element.audioTracks[this._enabledAudioTrack].enabled = false;
                this._enabledAudioTrack = (this._enabledAudioTrack + 1) % this.element.audioTracks.length;
                this.element.audioTracks[this._enabledAudioTrack].enabled = true;
            }
        },

        // To enable fast scrubbing change the playback rate to zero
        scrubbing: {
            get: function () { return this.element.playbackRate === 0; },
            set: function (value) { this.element.playbackRate = (value) ? 0 : this.element.defaultPlaybackRate; }
        },

        // Controls zoom in the video
        zoom: {
            get: function () { return this.element.msZoom; },
            set: function (value) {
                this.element.msZoom = value;
                playerUI.binding.zoomClassName = (this.element.msZoom) ?
                    playerUI.KnownClassNames.zoom.enabled :
                    playerUI.KnownClassNames.zoom.disabled;
            }
        },

        // Controls video stabilization
        stabilization: {
            get: function () { return playerUI.binding.stabilizationClassName === playerUI.KnownClassNames.stabilization.enabled; },
            set: function (value) {
                var that = this;

                // Avoid uneeded operations if not changing the value
                if (that.stabilization === value) {
                    return;
                }

                playerUI.binding.stabilizationClassName = (value) ?
                    playerUI.KnownClassNames.stabilization.enabled :
                    playerUI.KnownClassNames.stabilization.disabled;

                // Remove all effects and add video stabilization if needed
                that.element.msClearEffects();
                if (value) {
                    that.element.msInsertVideoEffect(Windows.Media.VideoEffects.videoStabilization, false);
                }

                // Store the current playback state to restore it after the media pipeline is reloaded
                var currentTime = that.element.currentTime;
                var playing = !that.element.paused;

                // Reload the pipeline to apply effect changes
                that.element.eventAsync({
                    action: function () {
                        that.element.load();
                    },
                    event: "canplay"
                }).done(function () {

                    // Seek to the stored position and continue playback if needed
                    that.element.currentTime = currentTime;
                    if (playing) {
                        that.element.play();
                    }
                });
            }
        },

        // Controls stereo rendering
        stereo: {
            get: function () { return this.element.msStereo3DRenderMode === "stereo"; },
            set: function (value) {
                if (value && !Windows.Graphics.Display.DisplayInformation.getForCurrentView().stereoEnabled) {
                    new Windows.UI.Popups.MessageDialog("Stereo rendering is not supported by this computer", "3D video").showAsync();
                } else if (value && !this.element.msIsStereo3D) {
                    new Windows.UI.Popups.MessageDialog("The media doesn't appear to support 3D video", "3D video").showAsync();
                } else {
                    this.element.msStereo3DRenderMode = (value) ? "stereo" : "mono";
                }

                playerUI.binding.stereoClassName = (this.stereo) ?
                    playerUI.KnownClassNames.stereo.enabled :
                    playerUI.KnownClassNames.stereo.disabled;
            }
        },

        // Controls looping in the video
        loop: {
            get: function () { return this.element.loop; },
            set: function (value) {
                this.element.loop = value;
                playerUI.binding.loopClassName = (this.element.loop) ?
                    playerUI.KnownClassNames.loop.enabled :
                    playerUI.KnownClassNames.loop.disabled;
            }
        },

        // Changes the fullscreen mode
        fullscreen: {
            get: function () { return playerUI.binding.displayClassName === playerUI.KnownClassNames.display.fullscreen; },
            set: function (value) {
                if ((!value && this.element.parentNode === document.body) ||
                    (value && this.element.parentNode !== document.body)) {
                    this.element.swapNode(this._referenceElement);
                }
                playerUI.binding.displayClassName = (value) ?
                    playerUI.KnownClassNames.display.fullscreen :
                    playerUI.KnownClassNames.display.restore;
            }
        },

        // Changes custom controls visibility
        showControls: {
            get: function () { return playerUI.binding.customControlsClassName === playerUI.KnownClassNames.customControls.show; },
            set: function (value) {
                playerUI.binding.customControlsClassName = (value) ?
                    playerUI.KnownClassNames.customControls.show :
                    playerUI.KnownClassNames.customControls.hide;
            }
        }
    });

    WinJS.Namespace.define("Application.Player", {
        WinControl: player
    });
})();
