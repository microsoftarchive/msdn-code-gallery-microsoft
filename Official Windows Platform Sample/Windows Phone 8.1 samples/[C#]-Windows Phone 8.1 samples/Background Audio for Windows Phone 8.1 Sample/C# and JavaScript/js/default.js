// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=329104
(function () {
    "use strict";

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
        
    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // This application has been newly launched. Initialize
                // your application here.                
                saveSettingsValue(constants.AppState, constants.ForegroundAppActive);
                addMediaPlayerEventHandlers();                
                readyforPlaybackObj.getBackgroundAudio(); 
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
                foregroundAppResuming();            
            } 
            args.setPromise(WinJS.UI.processAll());
        }
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().        
        
        args.setPromise(WinJS.UI.processAll());
        
    };
    
    Windows.UI.WebUI.WebUIApplication.addEventListener("resuming", foregroundAppResuming);
    Windows.UI.WebUI.WebUIApplication.addEventListener("suspending", foregroundAppSuspending);
    
    app.start();
})();

//
//Method to execute when app is suspending
//
function foregroundAppSuspending() {
    var message = new Windows.Foundation.Collections.ValueSet();
    message.insert(constants.AppSuspended, "");
    Windows.Media.Playback.BackgroundMediaPlayer.sendMessageToBackground(message);

    removeMediaPlayerEventHandlers();
    saveSettingsValue(constants.AppState, constants.ForegroundAppSuspended);
}


function foregroundAppResuming() {
    //
    //App is resuming add back event handlers for app buttons
    //Get ready to receive messages from background
    //

    addMediaPlayerEventHandlers();    
    saveSettingsValue(constants.AppState, constants.ForegroundAppActive);              
            
    // Verify if the task was running before
    if (readyforPlaybackObj.checkBackgroundTaskRunning()) {
        
        //if yes, get media player instance and connect to its event handlers                                  
        //send message to background task that app is resumed
        //so it can start sending notifications
        
        var message = new Windows.Foundation.Collections.ValueSet();
        message.insert(constants.AppResumed, "");
        Windows.Media.Playback.BackgroundMediaPlayer.sendMessageToBackground(message);

        readyforPlaybackObj.getBackgroundAudio();
        
        if (mediaPlayer.currentState === Windows.Media.Playback.MediaPlayerState.playing) {            
            document.getElementById("PlayButton").innerHTML = "| |";  // Change to pause button                    
        }
        else {            
            document.getElementById("PlayButton").innerHTML = ">";     // Change to play button
        }
        document.getElementById("txtCurrentTracktext").innerHTML = getcurrentTrack();
    }
    else {        
        document.getElementById("PlayButton").innerHTML = ">";     // Change to play button
        document.getElementById("txtCurrentTracktext").innerHTML = "";
    }
}
