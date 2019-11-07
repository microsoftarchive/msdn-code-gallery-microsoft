//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var mediaControls;
    var currentSongIndex = 0;
    var playlist = [];
    var playlistCount;
    var nextRegistered = false;
    var previousRegistered = false;
    var thumbnailName = null;

    function song() {
        this.URLObject;
        this.file;
    }


    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            document.getElementById("button1").addEventListener("click", doSomething1, false);

            // Create new media control
            mediaControls = Windows.Media.MediaControl;

            // Add an event listener to listen for common media commands
            //
            // When using the background audio category the Play and Pause events are
            // not optional.  Play and Pause must be registered to use the background
            // category.
            mediaControls.addEventListener("playpausetogglepressed", playPause, false);
            mediaControls.addEventListener("playpressed", play, false);
            mediaControls.addEventListener("stoppressed", stop, false);
            mediaControls.addEventListener("pausepressed", pause, false);
            mediaControls.addEventListener("fastforwardpressed", fastForward, false);
            mediaControls.addEventListener("rewindpressed", rewind, false);
            mediaControls.addEventListener("channeluppressed", channelUp, false);
            mediaControls.addEventListener("channeldownpressed", channelDown, false);
            mediaControls.addEventListener("recordpressed", record, false);

            // Audio tag specific event listeners.
            id("audiotag").addEventListener("playing", playing, false);
            id("audiotag").addEventListener("pause", paused, false);
            id("audiotag").addEventListener("ended", songEnded, false);

            mediaControls.isPlaying = false;

            
            

        }
    });

    function id(elementId) {
        return document.getElementById(elementId);
    }


    function createPlaylist(files) {
        var file;
        if (files.size > 0) {
            playlistCount = files.size;

            // Reset the event handlers
            //
            if (nextRegistered) {
                mediaControls.removeEventListener("nexttrackpressed", nextTrack, false);
                nextRegistered = false;
            }
            if (previousRegistered) {
                mediaControls.removeEventListener("previoustrackpressed", previousTrack, false);
                previousRegistered = false;
            }

            // Set the current track back to 0 reset playlist
            //
            currentSongIndex = 0;
            playlist = [];

            if (playlistCount > 1) {
                if (!nextRegistered) {
                    mediaControls.addEventListener("nexttrackpressed", nextTrack, false);
                    nextRegistered = true;
                }
            }

            for (var fileIndex = 0; fileIndex < files.size; fileIndex++) {
                var tempSong = new song;

                file = files[fileIndex];
                tempSong.URLObject = URL.createObjectURL(file, {oneTimeOnly: false}); 
                tempSong.file = file;
                playlist[fileIndex] = tempSong;
            }
        }

    }

    function setCurrentPlaying(index) {
        id("audiotag").src = playlist[index].URLObject;
        if (mediaControls.isPlaying === true) {
            id("audiotag").play();
        }
        setMetaData(index);

    }
    function setMetaData(songIndex) {
        try {
            var thumbnailMode = Windows.Storage.FileProperties.ThumbnailMode.musicView;
            var thumbnailOptions = Windows.Storage.FileProperties.ThumbnailOptions.ResizeThumbnail;
            var file = playlist[songIndex].file;

            // This will get the thumbnail from the file, save it to a local temporary location and give that URL to the MTC UI
            file.getThumbnailAsync(thumbnailMode, 96, thumbnailOptions).done(function (thumbnail) {
                if (thumbnail) {
                    var inputStream = thumbnail.getInputStreamAt(0);
                    var reader = new Windows.Storage.Streams.DataReader(inputStream);
                    var size = thumbnail.size;

                    // Save the thumbnail to a local temporary location
                    if (size > 0) {
                        reader.loadAsync(size).done(function () {
                            var buffer = new Array(size);
                            reader.readBytes(buffer);

                            // Close the file streams
                            thumbnail.close();
                            reader.close();
                           

                            var tempFolder = Windows.Storage.ApplicationData.current.temporaryFolder;

                            // Use ping-pong buffers in case the Media Control UI is still reading the thumbnail during a track change
                            if (thumbnailName === null || thumbnailName === "thumbnail2.jpg") {
                                thumbnailName = "thumbnail.jpg";
                            } else {
                                thumbnailName = "thumbnail2.jpg";
                            }

                            tempFolder.createFileAsync(thumbnailName, Windows.Storage.CreationCollisionOption.replaceExisting).done(function (imageFile) {
                                Windows.Storage.FileIO.writeBytesAsync(imageFile, buffer).done(function () {                                   
                                    playlist[songIndex].file.properties.getMusicPropertiesAsync().done(function (musicProperties) {
                                        setAlbumArt(thumbnailName, musicProperties);
                                    });
                                });
                            });
                        });
                    } else {
                        WinJS.log("Thumbnail is empty", "sample", "error");
                        thumbnail.close();
                        reader.close();
                        inputStream.close();
                        
                    }                    
                } else {
                    WinJS.log(getTimeStampedMessage("Song Art Work not available for " + storageFile.path), "sample", "error");
                    mediaControls.albumArt = "";
                }
            });
        } catch (ex) {
            WinJS.log("Error in GetThumbnail(): " + ex.description, "sample", "error");
        }

        
    }



    function setAlbumArt(fileName, musicProperties) {
        // it is a good idea to put the setting of the track name and uri in a try/catch in case song names have 
        // more that 127 characters.  
        try {
            var uri = new Windows.Foundation.Uri("ms-appdata:///temp/" + fileName);

            mediaControls.albumArt = uri;
            mediaControls.trackName = musicProperties.title;
            mediaControls.artistName = musicProperties.artist;
           

        } catch (ex) {
            WinJS.log("Error in setAlbumArt(): " + ex, "sample", "error");            
        }
    }

    function doSomething1() {
        var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
        openPicker.fileTypeFilter.replaceAll([".mp3", ".mp4", ".wma", ".m4a"]);
        openPicker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.musicLibrary;

        openPicker.pickMultipleFilesAsync().done(function (files) {
            if (files.length > 0) {
                createPlaylist(files);
                setCurrentPlaying(currentSongIndex);
            }
        });        
        
    }

    function getTimeStampedMessage(eventCalled) {
        var timeformat = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longtime");
        var time = timeformat.format(new Date());
        var message = eventCalled + "\t\t" + time;

        return message;

    }

    function playing() {
        //  When the audio tag changes to playing update the isPlaying property
        //  This will help the OS display the correct Play/Pause button in the UI
        mediaControls.isPlaying = true;
    }

    function paused() {
        //  When the audio tag changes to paused update the isPlaying property
        //  This will help the OS display the correct Play/Pause button in the UI
        mediaControls.isPlaying = false;
    }

    function songEnded() {
        // At the end of the song move to the next track
        nextTrack();
    }

    function playPause() {
        // Handle the Play/Pause event.  We will just display something on the screen.
        if (mediaControls.isPlaying === false) {
            WinJS.log(getTimeStampedMessage("Play/Pause - Play Received"), "sample", "status");
            id("audiotag").play();
        }
        else {
            WinJS.log(getTimeStampedMessage("Play/Pause - Pause Received"), "sample", "status");            
            id("audiotag").pause();

        }
    }

    function play() {
        // Handle the Play event.  We will just display something on the screen.
        WinJS.log(getTimeStampedMessage("Play Received"), "sample", "status");
        id("audiotag").play();
    }

    function stop() {
        // Handle the stop event.
        WinJS.log(getTimeStampedMessage("Stop Received"), "sample", "status");
        
        id("audiotag").pause();
        id("audiotag").currentTime = 0;
    }

    function pause() {
        // Handle the Pause event.
        WinJS.log(getTimeStampedMessage("Pause Received"), "sample", "status");        
        id("audiotag").pause();
    }

    function nextTrack() {
        // Handle the Next Track event.
        WinJS.log(getTimeStampedMessage("Next Track Received"), "sample", "status");        
        if (currentSongIndex < (playlistCount - 1)) {
            currentSongIndex++;
            setCurrentPlaying(currentSongIndex);
            if (currentSongIndex > 0) {
                if (!previousRegistered) {
                    // add the previous track listener if not at the beginning of the playlist
                    mediaControls.addEventListener("previoustrackpressed", previousTrack, false);
                    previousRegistered = true;
                }
            }
            if (currentSongIndex === (playlistCount - 1)) {
                if (nextRegistered) {
                    // remove the nexttrack registration if at the end of the playlist
                    mediaControls.removeEventListener("nexttrackpressed", nextTrack, false);
                    nextRegistered = false;
                }
            }
        }


    }

    function previousTrack() {
        // Handle the Previous Track event.
        WinJS.log(getTimeStampedMessage("Prev Track Received"), "sample", "status");        
        if (currentSongIndex > 0) {
            if (currentSongIndex === (playlistCount - 1)) {
                if (!nextRegistered) {
                    // Add the next track listener if the playlist is not at the end.
                    mediaControls.addEventListener("nexttrackpressed", nextTrack, false);
                    nextRegistered = true;
                }
            }
            currentSongIndex--;

            if (currentSongIndex === 0) {
                if (previousRegistered) {
                    // Remove the previous track registration if the playlist is at the beginning
                    mediaControls.removeEventListener("previoustrackpressed", previousTrack, false);
                    previousRegistered = false;
                }

            }

            setCurrentPlaying(currentSongIndex);
        }
    }

    function fastForward() {
        // Handle the fastforward event.  The fastforward event is a repeating event.
        // If the user holds down the button more events will be fired. We will just
        // display something on the screen.
        WinJS.log(getTimeStampedMessage("Fast Forward Received"), "sample", "status");        
    }

    function rewind() {
        // Handle the rewind event.  The rewind event is a repeating event.
        // If the user holds down the button more events will be fired. We will just
        // display something on the screen.
        WinJS.log(getTimeStampedMessage("Rewind Received"), "sample", "status");        
    }

    function channelUp() {
        // Handle the channelup event.  The channelup event is a repeating event.
        // If the user holds down the button more events will be fired. We will just
        // display something on the screen.
        WinJS.log(getTimeStampedMessage("Channel Up Received"), "sample", "status");        
    }

    function channelDown() {
        // Handle the channeldown event.  The channeldown event is a repeating event.
        // If the user holds down the button more events will be fired. We will just
        // display something on the screen.
        WinJS.log(getTimeStampedMessage("Channel Down Received"), "sample", "status");        
    }

    function record() {
        // Handle the Previous Track event.  We will just display something on the screen.
        WinJS.log(getTimeStampedMessage("Record Received"), "sample", "status");
    }




})();
