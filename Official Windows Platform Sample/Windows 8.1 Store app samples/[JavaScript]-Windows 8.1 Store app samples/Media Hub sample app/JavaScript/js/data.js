//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // Binding properties of items
    var bind = {
        title: null,
        subtitle: null,
        backgroundImage: null,
        backgroundOpacity: null,
        icon: null
    };

    // Non bound properties of items
    var properties = {
        updateDataAsync: null,
        invoke: null,
        dispose: null,
        group: null
    };

    // File item declaration
    var file = {
        _updateRun: null,
        file: null,
        thumbnailURL: null,
        invoke: function () {
            var that = this;

            // Make sure data is up to date to send the proper media title
            that.updateDataAsync().done(function () {
                WinJS.Navigation.navigate(
                    "/pages/playbackcontrols/playbackcontrols.html",
                    {
                        title: that.title,
                        source: that.file,
                        behavior: "load"
                    });
            });
        },
        dispose: function () {

            // Revoke the thumbnail URL when disposing
            if (this.thumbnailURL) {
                URL.revokeObjectURL(this.thumbnailURL);
                this.thumbnailURL = null;
                this.backgroundOpacity = 0;
            }
        },
        updateDataAsync: function () {
            var that = this;

            // If already have a thumbnail URL data was already up to date
            if (that.thumbnailURL) {
                return WinJS.Promise.wrap(that);
            }

            // Get the thumbnail
            return that.file.getThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.documentsView, 250).then(
                function (thumbnail) {
                    if (thumbnail) {
                        // Create a reusable thumbnail URL and change the background properties
                        that.thumbnailURL = URL.createObjectURL(thumbnail, { singleTimeOnly: false });
                        that.backgroundImage = "url('" + that.thumbnailURL + "')";
                        that.backgroundOpacity = 1;
                    } else {
                        that.icon = "";
                    }

                    // Depending on the content type get the needed properties
                    if (that.file.contentType.indexOf("video") === 0) {
                        return that.file.properties.getVideoPropertiesAsync();
                    } else if (that.file.contentType.indexOf("audio") === 0) {
                        return that.file.properties.getMusicPropertiesAsync();
                    } else if (that.file.contentType.indexOf("image") === 0) {
                        return that.file.properties.getImagePropertiesAsync();
                    } else {
                        return that.file.properties.getDocumentPropertiesAsync();
                    }
                }).then(function (props) {

                    // Set the title and subtitle depending on what's available
                    if (props.title) {
                        that.title = props.title;
                    }

                    if (props.album || props.artist) {
                        that.subtitle = (props.album) ?
                            props.album + " - " + props.artist :
                            props.artist;
                    } else if (props.subtitle) {
                        that.subtitle = props.subtitle;
                    } else if (props.dateTaken) {
                        that.subtitle = props.dateTaken;
                    }

                    return WinJS.Promise.wrap(that);
                });
        }
    };

    var folder = {
        folder: null,
        invoke: function () {
            var that = this;
            var data = [];

            // Build a list of all files and folders inside the current folder 
            that.folder.getFoldersAsync().then(function (folders) {
                for (var index = 0, size = folders.length; index < size; index++) {
                    data.push(new Application.Data.Folder({ folder: folders[index] }));
                }
                return that.folder.getFilesAsync();
            }).done(function (files) {
                for (var index = 0, size = files.length; index < size; index++) {
                    data.push(new Application.Data.File({ file: files[index] }));
                }

                // Navigate to the folder
                WinJS.Navigation.navigate(
                    "/pages/browser/browser.html",
                    {
                        title: that.folder.displayName,
                        list: new WinJS.Binding.List(data)
                    });
            });
        }
    };

    // Standard object initialization
    var initObject = function (options) {
        this._initObservable();
        for (var option in this) {
            this[option] = options[option] || this[option] || "";
        }
        this.backgroundOpacity = (this.backgroundImage) ? 1 : 0;
    };

    WinJS.Namespace.define("Application.Data", {

        Link: WinJS.Class.mix(
            initObject,
            bind,
            properties,
            WinJS.Binding.mixin,
            WinJS.Binding.expandProperties(bind)),

        File: WinJS.Class.mix(
            function (options) {
                initObject.bind(this)(options);
                this.title = this.file.displayName;
                this.icon = this.icon || WinJS.UI.AppBarIcon.placeholder;
            },
            bind,
            properties,
            file,
            WinJS.Binding.mixin,
            WinJS.Binding.expandProperties(bind)),

        Folder: WinJS.Class.mix(
            function (options) {
                initObject.bind(this)(options);
                this.title = this.folder.displayName;
                this.icon = this.icon || WinJS.UI.AppBarIcon.folder;
            },
            bind,
            properties,
            folder,
            WinJS.Binding.mixin,
            WinJS.Binding.expandProperties(bind)),

        timeAsText: function (time) {

            // Generates a time stream from the given seconds
            var seconds = Math.round(time);
            var hours = Math.floor(seconds / 3600);
            seconds -= hours * 3600;
            var minutes = Math.floor(seconds / 60);
            seconds -= minutes * 60;
            return (((hours > 0) ? hours + ":" : "") + ("0" + minutes).substr(-2) + ":" + ("0" + seconds).substr(-2));
        }

    });
})();
