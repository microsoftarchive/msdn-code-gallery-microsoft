//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var transcoding = Windows.Media.Transcoding;
    var storage = Windows.Storage;
    var mediaProperties = Windows.Media.MediaProperties;
    var notifications = Windows.UI.Notifications;
    var thumbnailSize = 400;

    // States of the media transcoder object
    var states = {
        empty: "media-transcoder-empty",
        unavailable: "media-transcoder-unavailable",
        audioSource: "media-transcoder-source-audio",
        videoSource: "media-transcoder-source-video",
        transcoding: "media-transcoder-transcoding"
    };

    // Bindable attributes
    var bind = {
        sourceTitle: null,
        sourceFileName: null,
        sourceBackgroundImage: null,
        state: null,
        progress: null,
        destinationFileName: null
    };

    // Transcoder object definition
    var Transcoder = WinJS.Class.mix(
        function () {
            this._initObservable();
            this._clean();
        },
        {
            _operation: null,
            _source: null,
            _thumbnailUrl: null,
            _clean: function () {

                // Resets the state of the transcoder
                this._operation = null;
                this._source = null;

                // Revoke object URLs when needed
                if (this._thumbnailUrl) {
                    URL.revokeObjectURL(this._thumbnailUrl);
                    this._thumbnailUrl = null;
                }

                this.progress = 0;
                this.state = states.empty;
                this.sourceTitle = null;
                this.sourceFileName = null;
                this.destinationFileName = null;
                this.sourceBackgroundImage = "none";
            },
            source: {
                get: function () { return this._source; },
                set: function (file) {
                    var that = this;

                    // Verify this operation can be performed in the current state
                    if (that.state === states.unavailable || that.state === states.transcoding) {
                        throw "Invalid operation in the current state";
                    }

                    that.state = states.unavailable;

                    // Clean all the object and set the source file
                    that._clean();
                    that._source = file;

                    that._source.getThumbnailAsync(storage.FileProperties.ThumbnailMode.documentsView, thumbnailSize).then(function (thumbnail) {

                        // Generate the source thumbnail
                        that._thumbnailUrl = URL.createObjectURL(thumbnail, { oneTimeOnly: false });
                        that.sourceBackgroundImage = "url('" + that._thumbnailUrl + "')";
                        return that._source.properties.getDocumentPropertiesAsync();
                    }).done(function (properties) {

                        // Extract the file title
                        that.sourceTitle = properties.title || that._source.displayName;
                        that.sourceFileName = that._source.name;

                        // Set the appropiate state
                        if (that.source.contentType.indexOf("audio") === 0) {
                            that.state = states.audioSource;
                        } else {
                            that.state = states.videoSource;
                        }
                    });
                }
            },
            startTranscodeAsync: function (file, container, profile) {
                var that = this;

                // Verify this operation can be performed in the current state
                if (that.state !== states.audioSource && that.state !== states.videoSource) {
                    return WinJS.Promise.wrapError("Invalid operation in the current state");
                }

                // Create the media transcoder object
                var transcoder = new transcoding.MediaTranscoder();
                that.state = states.unavailable;
                that.destinationFileName = file.name;

                // Prepare the transcode operation using the appropiate profile
                return transcoder.prepareFileTranscodeAsync(
                    that._source,
                    file,
                    mediaProperties.MediaEncodingProfile["create" + container](
                        mediaProperties[(that._source.contentType.indexOf("video") === 0) ?
                        "VideoEncodingQuality" : "AudioEncodingQuality"][profile])
                ).then(function (operation) {

                    // Fail the operation if transcoding is not possible
                    if (!operation.canTranscode) {
                        that._clean();
                        if (operation.failureReason === transcoding.TranscodeFailureReason.codecNotFound) {
                            return WinJS.Promise.wrapError("The codec was not found.");
                        } else if (operation.failureReason === transcoding.TranscodeFailureReason.invalidProfile) {
                            return WinJS.Promise.wrapError("Profile is invalid.");
                        } else {
                            return WinJS.Promise.wrapError("Reason unknown.");
                        }
                    }

                    // Change the state and keep a reference to the operation
                    that._operation = operation;
                    that.state = states.transcoding;

                    // Start transcoding
                    operation.transcodeAsync().done(function () {

                        // When finished clean the internal variables and state
                        Windows.UI.Popups.MessageDialog(that.destinationFileName + " was saved successfully", "Convert media").showAsync();
                        that._clean();
                    },
                    function (error) {

                        // In case the error also clean the internal variables and state
                        Windows.UI.Popups.MessageDialog(error, "Convert media error").showAsync();
                        that._clean();
                    },
                    function (progress) {

                        // Update the operation progress
                        that.progress = progress;
                    });

                    return operation;
                });
            }
        },
        bind,
        WinJS.Binding.mixin,
        WinJS.Binding.expandProperties(bind));

    WinJS.Namespace.define("Application.Transcoding", {
        transcoder: new Transcoder(),
        TranscoderStates: states
    });

})();
