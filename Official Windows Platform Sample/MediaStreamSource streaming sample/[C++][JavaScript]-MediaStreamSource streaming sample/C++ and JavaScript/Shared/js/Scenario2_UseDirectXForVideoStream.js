//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var MSS = null;
    var videoDesc = null;
    var sampleGenerator = null;
    var width = 1152;
    var height = 720;
    var frameRateN = 30;
    var frameRateD = 1;


    var page = WinJS.UI.Pages.define("/html/Scenario2_UseDirectXForVideoStream.html", {
        ready: function (element, options) {
            var videoEncProps = Windows.Media.MediaProperties.VideoEncodingProperties.createUncompressed(Windows.Media.MediaProperties.MediaEncodingSubtypes.bgra8, width, height);
            videoDesc = new Windows.Media.Core.VideoStreamDescriptor(videoEncProps);
            videoDesc.encodingProperties.frameRate.numerator = frameRateN;
            videoDesc.encodingProperties.frameRate.denominator = frameRateD;
            videoDesc.encodingProperties.bitrate = frameRateN * frameRateD * width * height * 4;

            MSS = new Windows.Media.Core.MediaStreamSource(videoDesc);
            MSS.bufferTime = 250;
            MSS.addEventListener("starting", mss_starting);
            MSS.addEventListener("samplerequested", mss_samplerequested);

            sampleGenerator = new DXSurfaceGenerator.SampleGenerator();
            
            id("mediaPlayer").autoplay = false;
            id("mediaPlayer").src = URL.createObjectURL(MSS, { oneTimeOnly: true });
        }
    });

    function mss_starting(e)
    {
        sampleGenerator.initialize(MSS, videoDesc);
        e.request.setActualStartPosition(0);
    }

    function mss_samplerequested(e)
    {
        sampleGenerator.generateSample(e.request);
        //sampleGenerator.generateTestSample(e.request);
    }

    function mss_closed(e)
    {

    }

    function id(elementId) {
        return document.getElementById(elementId);
    }

    function handleError(error) {
        WinJS.log && WinJS.log("Error: " + error.message, "sample", "error");
    }

})();
