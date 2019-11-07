//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // Shortcut names
    var capture = Windows.Media.Capture;
    var storage = Windows.Storage;
    var mediaProperties = Windows.Media.MediaProperties;

    // Generates a filename friendly date, the app isn't using DateTimeFormatter since
    // needs to ensure no invalid filename characters are used
    var dateTimeFileName = function () {
        var date = new Date();
        return ("0" + date.getYear()).substr(-2)
            + ("0" + (date.getMonth() + 1)).substr(-2)
            + ("0" + date.getDate()).substr(-2) + "_"
            + ("0" + date.getHours()).substr(-2)
            + ("0" + date.getMinutes()).substr(-2)
            + ("0" + date.getSeconds()).substr(-2);
    };

    // Sound recorder states, CSS friendly
    var recorderStates = {
        uninitialized: "uninitialized",
        empty: "empty",
        recording: "recording",
        waiting: "waiting",
        unavailable: "unavailable",
        playing: "playing"
    };

    // Sound recorder properties that support binding
    var recorderBind = {
        state: null,
        timeAsText: null
    };

    var soundRecorder = WinJS.Class.mix(
        function () {
            this._initObservable();

            // Initial state and properties of the sound recorder
            this._mediaCapture = null;
            this._stream = null;
            this.state = recorderStates.uninitialized;
            this.message = "Initializing";
            this.timeAsText = Application.Data.timeAsText(0);
        },
        {
            _mediaCapture: null,
            _stream: null,
            _audioTag: null,
            _interval: null,

            // Initializes the sound recorder object
            initialize: function (audioTag) {
                var that = this;

                // Initial state
                if (that.state !== recorderStates.uninitialized) {
                    throw "Invalid operation in the current state";
                }

                that.state = recorderStates.unavailable;

                // Crate the media capture object
                that._mediaCapture = new capture.MediaCapture();

                // Initialize media capture to get audio 
                var settings = new capture.MediaCaptureInitializationSettings();
                settings.audioDeviceId = "";
                settings.streamingCaptureMode = capture.StreamingCaptureMode.audio;

                that._mediaCapture.initializeAsync(settings).done(function () {

                    // Store the given audio tag locally
                    that._audioTag = audioTag;

                    // When audio playback ends rewind and change the internal state
                    that._audioTag.onended = function () {
                        that.state = recorderStates.waiting;
                        that._audioTag.pause();
                        that._audioTag.currentTime = 0;
                    };

                    // Update the current time if audio is playing
                    that._audioTag.ontimeupdate = function () {
                        that.timeAsText = Application.Data.timeAsText(that._audioTag.currentTime);
                    };

                    // Ready to capture
                    that.state = recorderStates.empty;
                },
                function (error) {
                    that.state = recorderStates.uninitialized;
                    new Windows.UI.Popups.MessageDialog(error, "Media capture initialization error").showAsync();
                });
            },
            start: function () {
                var that = this;

                if (that.state !== recorderStates.waiting && that.state !== recorderStates.empty && that.state !== recorderStates.playing) {
                    throw "Invalid operation in the current state";
                }

                that.state = recorderStates.unavailable;

                // Pause playback of previous recordings to avoid noise
                that._audioTag.pause();

                // If a memory stream already exists close it
                if (that._stream) {
                    that._stream.close();
                }

                // Create a memory stream to store the audio data
                that._stream = new storage.Streams.InMemoryRandomAccessStream();

                // Start recording in the memory stream
                that._mediaCapture.startRecordToStreamAsync(
                    mediaProperties.MediaEncodingProfile.createWma(mediaProperties.AudioEncodingQuality.medium),
                    that._stream).done(function () {
                        // Start the timer that updates the current time
                        var time = 0;
                        that._interval = setInterval(function () {
                            that.timeAsText = Application.Data.timeAsText(++time);
                        }, 1000);
                        Application.Data.timeAsText(0);

                        // Currently recording audio
                        that.state = recorderStates.recording;
                    });
            },
            stop: function () {
                var that = this;

                if (that.state !== recorderStates.recording) {
                    throw "Invalid operation in the current state";
                }

                that.state = recorderStates.unavailable;

                // Stop the record timer and reset the current time
                clearInterval(that._interval);
                Application.Data.timeAsText(0);

                // Stop recording
                that._mediaCapture.stopRecordAsync().done(function () {
                    // Seek back to the start of the stream and load the media in the audio tag
                    that._stream.seek(0);
                    var blob = MSApp.createBlobFromRandomAccessStream("audio/x-ms-wma", that._stream);
                    that._audioTag.src = URL.createObjectURL(blob, { oneTimeOnly: true });

                    // Waiting for more input
                    that.state = recorderStates.waiting;
                    that.play();
                });
            },
            saveAsync: function () {
                var that = this;

                if (that.state !== recorderStates.waiting && that.state !== recorderStates.playing) {
                    throw "Invalid operation in the current state";
                }

                that.state = recorderStates.unavailable;

                // Create and open a new file in the music library
                var file = null;
                return storage.KnownFolders.musicLibrary.createFileAsync("WIN_" + dateTimeFileName() + ".wma", storage.NameCollisionOption.generateUniqueName).then(function (result) {
                    file = result;
                    return file.openAsync(storage.FileAccessMode.readWrite);
                }).then(function (stream) {

                    // Seek to the start of the strem and copy its data to the file's stream
                    that._stream.seek(0);
                    return storage.Streams.RandomAccessStream.copyAndCloseAsync(that._stream, stream);
                }).then(function () {

                    // Add music properties to the new file
                    return file.properties.getMusicPropertiesAsync();
                }).then(function (properties) {
                    var date = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("month day year hour minute second").format(new Date());
                    properties.title = "Audio recording";
                    properties.subtitle = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("month day year hour minute second").format(new Date());
                    return properties.savePropertiesAsync();
                }).then(function () {

                    // File was saved, wait for more input
                    that.state = recorderStates.empty;
                    return WinJS.Promise.wrap({ file: file });
                });
            },
            play: function () {
                var that = this;

                if (that.state !== recorderStates.waiting) {
                    throw "Invalid operation in the current state";
                }

                that.state = recorderStates.unavailable;

                // Start playback of the current audio changing the state when done
                that._audioTag.eventAsync({
                    action: function () {
                        that._audioTag.play();
                    },
                    event: "playing"
                }).done(function () {
                    that.state = recorderStates.playing;
                });
            },
            pause: function () {
                var that = this;

                if (that.state !== recorderStates.playing) {
                    throw "Invalid operation in the current state";
                }

                that.state = recorderStates.unavailable;

                // Pause playback of the current audio changing the state when done
                that._audioTag.eventAsync({
                    action: function () {
                        that._audioTag.pause();
                    },
                    event: "pause"
                }).done(function () {
                    that.state = recorderStates.waiting;
                });
            },
            dispose: function () {
                if (this._stream) {
                    this._stream.close();
                }
            }
        },
        recorderBind,
        WinJS.Binding.mixin,
        WinJS.Binding.expandProperties(recorderBind));

    // Simple function to get video or pictures from a webcam
    var getWebcamMediaAsync = function () {
        var captureUI = capture.CameraCaptureUI();
        var destination = null;
        var file;

        // Invoke the camera capture UI
        return captureUI.captureFileAsync(capture.CameraCaptureUIMode.photoOrVideo).then(function (result) {
            // If no file was captured fail the operation with a "cancel" reason
            file = result;
            if (!file) {
                return WinJS.Promise.wrapError("cancel");
            } else if (file.contentType.indexOf("image") === 0) {
                // If a picture was aquired get the image properties of the file and save the result in the pictures library
                destination = storage.KnownFolders.picturesLibrary;
                return file.properties.getImagePropertiesAsync();
            } else {
                // Otherwise get the video properties and save them in the videos library
                destination = storage.KnownFolders.videosLibrary;
                return file.properties.getVideoPropertiesAsync();
            }
        }).then(function (properties) {
            // Save properties on the new file
            var date = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("month day year hour minute second").format(new Date());
            properties.title = (file.contentType.indexOf("image") === 0) ? "Image capture" : "Video capture";
            if (properties.subtitle !== undefined) {
                properties.subtitle = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("month day year hour minute second").format(new Date());
            }
            return properties.savePropertiesAsync();
        }).then(function () {
            // Move it to the proper location
            return file.moveAsync(destination, "WIN_" + dateTimeFileName() + file.fileType, storage.NameCollisionOption.generateUniqueName);
        }).then(function () {
            // And finish the operation
            return WinJS.Promise.wrap({ file: file, folder: destination });
        });
    };

    WinJS.Namespace.define("Application.Capture", {
        getWebcamMediaAsync: getWebcamMediaAsync,
        SoundRecorder: soundRecorder,
        SoundRecorderStates: recorderStates
    });

})();
