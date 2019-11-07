(function () {
    "use strict";

    var sampleTitle = "Media Extensions";

    var scenarios = [{
        url: "/html/scenario1_LocalDecoder.html",
        title: "Local Decoder"
    }, {
        url: "/html/scenario2_SchemaHandler.html",
        title: "Custom Schema Handler"
    }, {
        url: "/html/scenario3_VideoStabilizationEffect.html",
        title: "Video Stabilization Effect"
    }, {
        url: "/html/scenario4_CustomEffect.html",
        title: "Custom Effects"
    }];

    function videoOnError(error) {
        switch (error.target.error.code) {
            case error.target.error.MEDIA_ERR_ABORTED:
                SdkSample.displayError("You aborted the video playback.");
                break;
            case error.target.error.MEDIA_ERR_NETWORK:
                SdkSample.displayError("A network error caused the video download to fail part-way.");
                break;
            case error.target.error.MEDIA_ERR_DECODE:
                SdkSample.displayError("The video playback was aborted due to a corruption problem or because the video used features your browser did not support.");
                break;
            case error.target.error.MEDIA_ERR_SRC_NOT_SUPPORTED:
                SdkSample.displayError("The video could not be loaded, either because the server or network failed or because the format is not supported.");
                break;
            default:
                SdkSample.displayError("An unknown error occurred.");
                break;
        }
    }

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: new WinJS.Binding.List(scenarios),
        navigationDisabled: false,
        currentScenarioUrl: null,
        disabledOnIndex: 0,
        videoOnError: videoOnError,

        formatError: function (e, prefix) {
            if (e.number) {
                WinJS.log && WinJS.log(
                    prefix +
                    e.message +
                    ", Error code: " +
                    e.number.toString(16),
                    null,
                    "error"
                );
            } else {
                WinJS.log && WinJS.log(
                    prefix +
                    e.message,
                    null,
                    "error"
                );
            }
        },

        displayStatus: function (msg) {
            WinJS.log && WinJS.log(msg, null, "status");
        },

        displayError: function (msg) {
            WinJS.log && WinJS.log(msg, null, "error");
        },

        clearLastStatus: function () {
            WinJS.log && WinJS.log("", null, "status");
        },
    });
})();