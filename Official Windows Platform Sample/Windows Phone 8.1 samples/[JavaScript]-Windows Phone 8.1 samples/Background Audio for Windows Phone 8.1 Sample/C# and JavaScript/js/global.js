var currentTrack = null;
var mediaPlayer = null;
var constants = {
    'CurrentTrack' : 'trackname',
    'BackgroundTaskStarted' : 'BackgroundTaskStarted',
    'BackgroundTaskRunning' : 'BackgroundTaskRunning',
    'BackgroundTaskCancelled' : 'BackgroundTaskCancelled',
    'BackgroundTaskState' : 'backgroundtaskstate',
    'ServerStarted' : 'ServerStarted',
    'AppSuspended' : 'appsuspend',
    'AppResumed' : 'appresumed',
    'AppState' : 'appstate',
    'StartPlayback' : 'startplayback',
    'SkipNext': 'skipnext',
    'SkipPrevious' : 'skipprevious',
    'Position' : 'position',
    'Trackchanged' : 'songchanged',
    'ForegroundAppActive' : 'Active',
    'ForegroundAppSuspended' : 'Suspended'
    };

var readyforPlaybackObj = {    
    callbackQueue : [],
    isMyBackgroundTaskRunning: false,

    //
    //When the instance of background media player is accesssed the entry point in the background task is invoked
    //The entry point is the Run method in MyBackgroundTask.cs. When this is invoked it send a message to foreground-ServerStarted
    //To receive messages from background ensure that the app subscribes to messagereceivedfrombackground as soon as it starts up

    getBackgroundAudio: function () {
        try{
            mediaPlayer = Windows.Media.Playback.BackgroundMediaPlayer.current;
            mediaPlayer.autoPlay = false;
            mediaPlayer.addEventListener("currentstatechanged", currentstatechangedHandler);
        }
        catch (error) {
            log("From getBackgroundAudio: " + error.message)
            log("From getBackgroundAudio: " + error.description);

        }
    },

    /// <summary>
    /// Gets the information about background task is running or not by reading the setting saved by background task
    /// </summary>

    checkBackgroundTaskRunning: function () {
        try{
            readResetSettingsValue(constants.BackgroundTaskState);
            if (value == null) {
                if (readyforPlaybackObj.isMyBackgroundTaskRunning) {                                      
                    return true;
                }
                return false;
            }
            if (value == constants.BackgroundTaskRunning) {
                readyforPlaybackObj.isMyBackgroundTaskRunning = true;                               
                return true;
            }
            else {
                readyforPlaybackObj.isMyBackgroundTaskRunning = false;                
                return false;
            }
        }
        catch (error) {
            log("From checkbackgroundtask: " + error.message)
            log("From checkbackgroundtask: " + error.description);
        }

    },
    //
    //When server started message is received, set ServerStarted variable and checkfor
    //the status of background app. Process any callbacks queued up when user pressed play button in the meantime.
    //
    init: function () {
        if (!readyforPlaybackObj.checkBackgroundTaskRunning()) {
            readyforPlaybackObj.processCallback();
        }
    },
    //
    //If serverStarted go ahead and call the callback
    //else push the request into queue
    //
    onReady: function (callback) {
        //Another app may have started background audio
        //check for status of background audio        
        readyforPlaybackObj.getBackgroundAudio();        
        if (readyforPlaybackObj.checkBackgroundTaskRunning()) {
            callback();
        } else {
            readyforPlaybackObj.callbackQueue.push(callback);
        }
    },

    processCallback: function () {
        var callback = readyforPlaybackObj.callbackQueue.pop();
        while (callback != undefined) {            
            callback();
        }
    }
};

function getcurrentTrack() {
    readResetSettingsValue(constants.CurrentTrack);
    if (value != null) {
        currentTrack = value;
    }
    return currentTrack;
}


//
//Add event handlers required.
//
function addMediaPlayerEventHandlers() {
    Windows.Media.Playback.BackgroundMediaPlayer.onmessagereceivedfrombackground = function (e) {
        messagereceivedHandler(e);
    }
    document.getElementById("PlayButton").addEventListener("click", playbuttonClick, false);
    document.getElementById("PrevButton").addEventListener("click", playPrevSong, false);
    document.getElementById("NextButton").addEventListener("click", playNextSong, false);    
}

//
//To Handle state changes
//
function currentstatechangedHandler() {
    try{
        mediaPlayer = Windows.Media.Playback.BackgroundMediaPlayer.current;
        switch (mediaPlayer.currentState) {
            case Windows.Media.Playback.MediaPlayerState.playing:
                document.getElementById("PlayButton").innerHTML = "| |";      // Change to pause button            
                document.getElementById("PrevButton").disabled = false;
                document.getElementById("NextButton").disabled = false;
                break;
            case Windows.Media.Playback.MediaPlayerState.paused:
                document.getElementById("PlayButton").innerHTML = ">";
                break;
        }
    }
    catch (error) {
        log("From currentstate: " + error.message)
        log("From currentstate: " + error.description);
    }

}

//
//Remove event handlers when app gets suspended
//
function removeMediaPlayerEventHandlers() {
    try{
        mediaPlayer = Windows.Media.Playback.BackgroundMediaPlayer.current;
        Windows.Media.Playback.BackgroundMediaPlayer.removeEventListener("messagereceivedfrombackground", messagereceivedHandler);
        mediaPlayer.removeEventListener("currentstatechanged", currentstatechangedHandler);
    }
    catch (error) {
        log("From getBackgroundAudio: " + error.message)
        log("From getBackgroundAudio: " + error.description);

    }
}

//
//Handle messages received from background
//
function messagereceivedHandler(e) {
    var messageSize = e.detail.length;
    for (var i = 0; i < messageSize; i++){
        for (var key in e.detail[i].data) {
            switch (key) {
                //When server started message is received from background task a variable is set
                case constants.BackgroundTaskStarted:                    
                    readyforPlaybackObj.init();
                    break;
                case constants.Trackchanged:                                        
                    document.getElementById("txtCurrentTracktext").innerHTML = e.detail[i].data[key];
                    break;
            }
        }    
    }    
}


//
//To start playback send message to the background
//
function playbuttonClick() {
    //
    //Background process may have been cancelled due to another application using background audio
    //    
    try{                           
            readyforPlaybackObj.onReady(function(){
            if (Windows.Media.Playback.MediaPlayerState.playing == mediaPlayer.currentState) {
                mediaPlayer.pause();
            }
            else if (Windows.Media.Playback.MediaPlayerState.paused == mediaPlayer.currentState) {
                mediaPlayer.play();
            }
            else if (Windows.Media.Playback.MediaPlayerState.closed == mediaPlayer.currentState) {
                startBackgroundAudioTask();
            }
        });
    }
    catch (error) {
        log("From playbuttonclick: " + error.message)
        log("From playbuttonclick: " + error.description);
    }
}

function startBackgroundAudioTask() {
    //Send message to initiate playback    
    if (readyforPlaybackObj.checkBackgroundTaskRunning()) {
        var message = new Windows.Foundation.Collections.ValueSet();
        message.insert(constants.StartPlayback, "0");
        Windows.Media.Playback.BackgroundMediaPlayer.sendMessageToBackground(message);
    }
}

function playNextSong() {
    var message = new Windows.Foundation.Collections.ValueSet();
    message.insert(constants.SkipNext, "");
    Windows.Media.Playback.BackgroundMediaPlayer.sendMessageToBackground(message);
    // Prevent the user from repeatedly pressing the button and causing 
    // a backlong of button presses to be handled. This button is re-eneabled 
    // in the TrackReady Playstate handler.
    document.getElementById("NextButton").disabled = true; 
}

function playPrevSong()
{
    var message = new Windows.Foundation.Collections.ValueSet();
    message.insert(constants.SkipPrevious, "");
    Windows.Media.Playback.BackgroundMediaPlayer.sendMessageToBackground(message);
    // Prevent the user from repeatedly pressing the button and causing 
    // a backlong of button presses to be handled. This button is re-eneabled 
    // in the TrackReady Playstate handler.
    document.getElementById("PrevButton").disabled = true;
}

function log(message) {
    var statusDiv = document.getElementById("statusMessage");
    if (statusDiv) {
        message += '<br>';
        statusDiv.innerHTML += message;
    }
}
