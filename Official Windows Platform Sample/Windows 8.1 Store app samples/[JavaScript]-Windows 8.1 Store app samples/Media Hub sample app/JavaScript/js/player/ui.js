//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // Class names for all playback binding properties
    var classNameValues = {
        state: {
            unavailable: "media-state-unavailable",
            paused: "media-state-paused",
            playing: "media-state-playing"
        },
        display: {
            restore: "media-display-restore",
            fullscreen: "media-display-fullscreen"
        },
        duration: {
            known: "media-duration-known",
            unknown: "media-duration-unknown"
        },
        sourceType: {
            video: "media-source-video",
            music: "media-source-music",
            picture: "media-source-picture",
            unknown: "media-source-unknown"
        },
        customControls: {
            show: "media-controls-show",
            hide: "media-controls-hide"
        },
        stabilization: {
            enabled: "media-stabilization-enabled",
            disabled: "media-stabilization-disabled"
        },
        stereo: {
            enabled: "media-stereo-enabled",
            disabled: "media-stereo-disabled"
        },
        loop: {
            enabled: "media-loop-enabled",
            disabled: "media-loop-disabled"
        },
        zoom: {
            enabled: "media-zoom-enabled",
            disabled: "media-zoom-disabled"
        }
    };

    // The list of class names that support binding
    var classNames = {
        displayClassName: null,
        stateClassName: null,
        stabilizationClassName: null,
        stereoClassName: null,
        loopClassName: null,
        zoomClassName: null,
        durationClassName: null,
        sourceTypeClassName: null,
        customControlsClassName: null
    };

    // Dynamic properties that support binding
    var active = {
        title: null,
        durationAsText: null,
        currentTimeAsPercentage: null,
        currentTimeAsText: null,
        fullClassName: null
    };

    // The main object for UI binding
    var ClassNamesSource = WinJS.Class.mix(
        function () {
            this._initObservable();

            // Initial state
            this.displayClassName = classNameValues.display.restore;
            this.stateClassName = classNameValues.state.unavailable;
            this.stabilizationClassName = classNameValues.stabilization.disabled;
            this.stereoClassName = classNameValues.stereo.disabled;
            this.loopClassName = classNameValues.loop.disabled;
            this.zoomClassName = classNameValues.zoom.disabled;
            this.durationClassName = classNameValues.duration.unknown;
            this.sourceTypeClassName = classNameValues.sourceType.unknown;
            this.customControlsClassName = classNameValues.customControls.show;

            this.currentTimeAsPercentage = "0%";
            this.durationAsText = "";
            this.currentTimeAsText = "";
            this.title = "";

            // When a class name chages update the full class name property
            for (var property in classNames) {
                this.bind(property, this._updateFullClassName.bind(this));
            }
        },
        classNames,
        {
            // The full class name property includes all class name properties in a single string
            _updateFullClassName: function () {
                var classname = "";
                for (var property in classNames) {
                    classname = classname + " " + this[property];
                }
                this.fullClassName = classname.substr(1);
            }
        },
        WinJS.Binding.mixin,
        WinJS.Binding.expandProperties(classNames),
        WinJS.Binding.expandProperties(active));

    WinJS.Namespace.define("Application.Player.UI", {
        binding: new ClassNamesSource(),
        KnownClassNames: classNameValues
    });
})();
