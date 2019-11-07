//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var audtag = null;
    var mediaControl;

    var page = WinJS.UI.Pages.define("/html/backgroundcapablemedia.html", {
        ready: function (element, options) {
            document.getElementById("button1").addEventListener("click", doSomething1, false);
        },

        unload: function() {

            // Remove the audio tag and then null it.
            // Then unload event listeners so you don't press play on another media element you switched from.
           
            if (audtag) {
                document.getElementById("MediaElement").removeChild(audtag);
            }
            audtag = null;
            if (mediaControl) {
            mediaControl.removeEventListener("playpausetogglepressed", playpause, false);
            mediaControl.removeEventListener("playpressed", play, false);
            mediaControl.removeEventListener("stoppressed", stop, false);
            mediaControl.removeEventListener("pausepressed", pause, false);
            mediaControl.removeEventListener("soundlevelchanged", soundLevelChanged, false);
            }
        }
    });

    function doSomething1() {
        var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
        openPicker.viewMode = Windows.Storage.Pickers.PickerViewMode.list;
        openPicker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.musicLibrary;
        openPicker.fileTypeFilter.replaceAll([".mp3", ".mp4", ".m4a", ".wma", ".wav"]);
        openPicker.pickSingleFileAsync().done(function (file) {
            if (file) {
                // Create the media control.

                mediaControl = Windows.Media.MediaControl;

                // Add event listeners for PBM notifications to illustrate app is
                // getting a new SoundLevel and pass the audio tag along to the function

                mediaControl.addEventListener("soundlevelchanged", soundLevelChanged, false);

                // Add event listeners for the mandatory media commands.
                // These are necessary to play streams of type 'backgroundCapableMedia'
                mediaControl.addEventListener("playpausetogglepressed", playpause, false);
                mediaControl.addEventListener("playpressed", play, false);
                mediaControl.addEventListener("stoppressed", stop, false);
                mediaControl.addEventListener("pausepressed", pause, false);

                var fileLocation = window.URL.createObjectURL(file, {oneTimeOnly: true });

                if (!audtag) {
                    audtag = document.createElement('audio');
                    audtag.setAttribute("id", "audtag");
                    audtag.setAttribute("controls", "true");
                    audtag.setAttribute("msAudioCategory", "BackgroundCapableMedia");
                    audtag.setAttribute("src", fileLocation);
                    document.getElementById("MediaElement").appendChild(audtag);
                    audtag.load();
                    WinJS.log && WinJS.log("Audio Tag Loaded", "sample", "status");
                    log(getTimeStampedMessage("test"));
                }

            } else {
                WinJS.log && WinJS.log("Audio Tag Did Not Load Properly", "sample", "error");
            }

        });

        
    }
    function playpause() {
        // Handle the Play/Pause event and print status to screen.
        WinJS.log && WinJS.log("Play/Pause Received", "sample", "status");
        if (!audtag.paused) {
            audtag.pause();
            Windows.Media.MediaControl.isPlaying = false;
        } else {
            audtag.play();
            Windows.Media.MediaControl.isPlaying = true;
        }
    }

    function play() {
        // Handle the Play event and print status to screen..
        WinJS.log && WinJS.log("Play Received", "sample", "status");
        audtag.play();
        Windows.Media.MediaControl.isPlaying = true;
    }

    function stop() {
        // Handle the Stop event and print status to screen.
        WinJS.log && WinJS.log("Stop Received (but a media element can't 'stop', so just diplaying text.", "sample", "status");
    }

    function pause() {
        // Handle the Pause event and print status to screen.
        WinJS.log && WinJS.log("Pause Received", "sample", "status");
        audtag.pause();
        Windows.Media.MediaControl.isPlaying = false;
    }

    function soundLevelChanged() {

        //Catch SoundLevel notifications and determine SoundLevel state.  If it's muted, we'll pause the player.
	//If your app is playing media you feel that a user should not miss if a VOIP call comes in, you may
	//want to consider pausing playback when your app receives a SoundLevel(Low) notification.
	//A SoundLevel(Low) means your app volume has been attenuated by the system (likely for a VOIP call).

        var soundLevel = Windows.Media.MediaControl.soundLevel;

       
        switch (soundLevel) {
           
            case Windows.Media.SoundLevel.muted:
                log(getTimeStampedMessage("App sound level is: Muted"));
                break;
            case Windows.Media.SoundLevel.low:
                log(getTimeStampedMessage("App sound level is: Low"));
                break;
            case Windows.Media.SoundLevel.full:
                log(getTimeStampedMessage("App sound level is: Full"));
                break;
        }
        
      
        appMuted();
    }

    function appMuted() {

        if (audtag) {
            if (!audtag.paused) {
                audtag.pause();
                Windows.Media.MediaControl.isPlaying = false;
                WinJS.log && WinJS.log("Audio Paused", "sample", "status");
            }
        }
    }

    function log(msg) {
        var pTag = document.createElement("p");
        pTag.innerHTML = msg;
        document.getElementById("StatusOutput").appendChild(pTag);
    }

    function getTimeStampedMessage(eventCalled) {
        var timeformat = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longtime");
        var time = timeformat.format(new Date());

        var message = eventCalled + "\t\t" + time;
        return message;
    }

})();
