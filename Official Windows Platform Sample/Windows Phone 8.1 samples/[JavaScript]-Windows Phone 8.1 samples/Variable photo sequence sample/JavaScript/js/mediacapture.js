// Namespaces
var Capture = Windows.Media.Capture;
var MediaProperties = Windows.Media.MediaProperties;

// Constants
var RESET_SESSION = true;
var STOP_SESSION = false;

var capturePhotoSessionComplete = false;

// MediaCapture Object
var mediaCaptureMgr = new Capture.MediaCapture();

// Display object
var displayReq = new Windows.System.Display.DisplayRequest();

// MediaCapture target variables
var previewUrl = null;
var photoFile = "photo.jpg";
var photoStorage = null;
var variablePhotoSequenceOp = null;
var videoStream = null;

// HTML Elements to keep track of

var br = document.createElement('br');

// Device lists
var webcamList = null;
var microphoneList = null;


// Capture Session flags and variables
var initialized = false;
var webcamListSet = false;
var microphoneListSet = false;

var previewStarted = false;
var rotateVideoOnOrientationChange = true;
var reverseVideoRotation = false;
var requestedWebcam = "";
var requestedMicrophone = "";

// Effects list tracking

var photosCapturedCount = 0;

var noofframes = 0;
var frameIndex = 0;

var rotWidth = 0;
var rotHeight = 0;


// VPS Per Frame setting


var vpsPerFrameControlList = [
        { controlName: "Exposure", control: 'rngVPSExposure', autoControl: 'chkVPSAutoExposure' },
        { controlName: "IsoSpeed", control: 'rngVPSIsoSpeed', autoControl: 'chkVPSAutoIsoSpeed' },
        { controlName: "ExposureCompensation", control: 'rngVPSExpC', autoControl: 'chkNULL' },
        { controlName: "Focus", control: 'rngVPSFocus', autoControl: 'chkNULL' }
];


var imgCount = 0;


// Variable Photo Sequence Controller
var vpsController;
var frameControllers = new Array();
var MAX_FRAME_CONTROLLERS = 20;
var vpsStopped = false;
var imageEncodingProperties = null;

// PLM Message Handlers
Windows.UI.WebUI.WebUIApplication.addEventListener("activated", plmActivateMessageHandler, false);
Windows.UI.WebUI.WebUIApplication.addEventListener("suspending", plmSuspendingMessageHandler, false);
Windows.UI.WebUI.WebUIApplication.addEventListener("resuming", plmResumingMessageHandler, false);
Windows.Graphics.Display.DisplayInformation.getForCurrentView().addEventListener("orientationchanged", updatePreviewForRotation, false);

// Start or Stop the capture session

function onClickStartButton() {
    if (initialized) {
        refreshCaptureSession(STOP_SESSION);
    } else {
        if (mediaCaptureMgr === null) {
            mediaCaptureMgr = new Capture.MediaCapture();
        }
        initializeMediaCaptureSession();
    }
}

// Start or Stop Preview
function onClickPreview() {
    if (!previewStarted) {
        document.getElementById("btnVPS").disabled = false;
        startPreview();
    } else {
        stopPreview();
        document.getElementById("btnPreview").textContent = "Start Preview";
        document.getElementById("btnVPS").disabled = true;
        previewStarted = false;
    }
}

// Adds a VPS Frame
function onClickAddFrame() {
    logMessage('Creating frame controllers.  ');
    var sel = document.getElementById("lstFrames");
    var opt = sel.options;
    if (opt.length < MAX_FRAME_CONTROLLERS) {
        frameControllers[noofframes] = new Windows.Media.Devices.Core.FrameController();
        var frameName = "Frame " + noofframes;
        sel.add(new Option(frameName, noofframes));
        document.getElementById("lstFrames").selectedIndex = (opt.length - 1);
        setFrame();
        noofframes++;
        document.getElementById("btnVPS").disabled = false;
    } else {
        logMessage("Cannot create more than 20 frames");
    }

}

// Removes a VPS frame
function onClickRemoveFrame() {
    var sel = document.getElementById("lstFrames");
    var opt = sel.options;
    if (opt.length > 1) {
        sel.removeChild(sel[sel.selectedIndex]);
        document.getElementById("lstFrames").selectedIndex = 0;
        setFrame();
    } else {
        logMessage("Cannot remove frame");
    }
}

// Clear the Image Tags generated in the div tag
function clrImgTags() {
    var node = document.getElementById("playback");
    var child = node.firstChild;
    while (child) {
        node.removeChild(child);
        child = node.firstChild;
    }
}

function addNewImgTag(index) {
    var newtag = document.createElement('img');
    newtag.id = "imageBox" + index;
    newtag.width = 100;
    newtag.height = 100;
    newtag = document.getElementById("playback").appendChild(newtag);
}

// This function takes care of starting the VPS session
function onClickTakeVPS() {

    // Add all frame controls to the controller
    clrImgTags();
    vpsController.desiredFrameControllers.clear();
    var sel = document.getElementById("lstFrames");
    for (var j = 0; j < sel.length; j++) {
        addNewImgTag(j);
        logMessage(sel.options[j].text + "=" + sel.options[j].value);
        var id = sel.options[j].value;
        vpsController.desiredFrameControllers.append(frameControllers[id]);
    }
    // Prepare VPS
    prepareVariablePhotoSequence();

}

// Change the Per Frame Exposure Value
function changeVPSExposure() {
    logMessage('Frame[' + lstFrames.selectedIndex + ']');
    logMessage('Current Exposure Value =' + frameControllers[lstFrames.selectedIndex].exposureControl.value);
    var control = document.getElementById(vpsPerFrameControlList[0].control);
    logMessage('Set Exposure Value =' + control.value);
    frameControllers[lstFrames.selectedIndex].exposureControl.value = control.value;
    var label = document.getElementById('lblExposure');
    label.value = "Exposure:" + control.value;
}

// Change the Per Frame ISoSPeed
function changeVPSIsoSpeed() {
    logMessage('Frame[' + lstFrames.selectedIndex + ']');
    logMessage('Current IsoSpeed Value =' + frameControllers[lstFrames.selectedIndex].isoSpeedControl.value);
    var control = document.getElementById(vpsPerFrameControlList[1].control);
    logMessage('Set IsoSpeed Value =' + control.value);
    frameControllers[lstFrames.selectedIndex].isoSpeedControl.value = control.value;
    var label = document.getElementById('lblIsoSpeed');
    label.value = "IsoSpeed:" + control.value;
}

// Template function to change the Auto settings
function changeVPSAuto(index) {
    logMessage('Control :' + vpsPerFrameControlList[index].controlName);
    var control = document.getElementById(vpsPerFrameControlList[index].control);
    var autoControl = document.getElementById(vpsPerFrameControlList[index].autoControl);
    if (index === 0) {
        frameControllers[lstFrames.selectedIndex].exposureControl.auto = autoControl.checked;
        var label = document.getElementById('lblExposure');
        if (autoControl.checked) {
            label.value = "Exposure: Auto";
        } else {
            control = document.getElementById(vpsPerFrameControlList[0].control);
            label.value = "Exposure:" + control.value;
        }
    } else {
        frameControllers[lstFrames.selectedIndex].isoSpeedControl.auto = autoControl.checked;
        var isoLabel = document.getElementById('lblIsoSpeed');
        if (autoControl.checked) {
            isoLabel.value = "IsoSpeed: Auto";
        } else {
            control = document.getElementById(vpsPerFrameControlList[1].control);
            isoLabel.value = "IsoSpeed:" + control.value;
        }
    }
    control.disabled = autoControl.checked;


}

// Change the Per Frame Exposure compensation
function changeVPSExpC() {
    logMessage('Frame[' + lstFrames.selectedIndex + ']');
    logMessage('Current ExpCmp Value =' + frameControllers[lstFrames.selectedIndex].exposureCompensationControl.value);
    var control = document.getElementById(vpsPerFrameControlList[2].control);
    logMessage('Set Value =' + control.value);
    frameControllers[lstFrames.selectedIndex].exposureCompensationControl.value = control.value;
    var label = document.getElementById('lblExpC');
    label.value = "ExpCompensation:" + control.value;
}

// Change the Per Frame Focus 
function changeVPSFocus() {
    logMessage('Frame[' + lstFrames.selectedIndex + ']');
    logMessage('Current Focus Control Value =' + frameControllers[lstFrames.selectedIndex].focusControl.value);
    var control = document.getElementById(vpsPerFrameControlList[3].control);
    logMessage('Set Value =' + control.value);
    frameControllers[lstFrames.selectedIndex].focusControl.value = control.value;
    var label = document.getElementById('lblFocus');
    label.value = "Focus:" + control.value;
}

// VPS Driver properties
function configureVariablePhotoSequence() {
    logMessage('\nVariablePhotoSequence Configuration:');
    logMessage('\tMaxPhotosPerSecond Supported: ' + vpsController.maxPhotosPerSecond);
    logMessage('\tPhotosPerSecondLimit Set    : ' + vpsController.photosPerSecondLimit);
}

// Prepare VPS
function prepareVariablePhotoSequence() {
    try {
        // Configure Photo Sequence
        configureVariablePhotoSequence();
        photosCapturedCount = 0;
        imgCount = 0;
        var vpsImageEncodingProperties = MediaProperties.ImageEncodingProperties.createJpeg();

        photoFile = "photo.jpg";
        vpsImageEncodingProperties.subtype = "JPEG";

        logMessage('\nCalling prepareVariablePhotoSequenceCaptureAsync');
        mediaCaptureMgr.prepareVariablePhotoSequenceCaptureAsync(vpsImageEncodingProperties).done(
            prepareVariablePhotoSequenceComplete, prepareVariablePhotoSequenceError);
    }
    catch (ex) {

        logMessage('prepareVariablePhotoSequenceCaptureAsync failed: ' + ex.message);
        capturePhotoSessionComplete = true;

    }
}

function prepareVariablePhotoSequenceComplete(opPreparePhotoSequence) {
    logMessage('\nprepareVariablePhotoSequenceCaptureAsync succeeded');

    variablePhotoSequenceOp = opPreparePhotoSequence;
    variablePhotoSequenceOp.addEventListener("photocaptured", variablePhotoSequenceOp_PhotoCaptured, false);
    variablePhotoSequenceOp.addEventListener("stopped", variablePhotoSequenceOp_Stopped, false);

    startVariablePhotoSequence();
}

function prepareVariablePhotoSequenceError(prepareVPSError) {
    logMessage('prepareVariablePhotoSequenceCaptureAsync failed' + (prepareVPSError.number >>> 0).toString(16));
    capturePhotoSessionComplete = true;
}

// StartVPS
function startVariablePhotoSequence() {
    try {
        // Start recording
        logMessage('\nCalling VariablePhotoSequenceCapture.startAsync ');
        variablePhotoSequenceOp.startAsync().done(
            startVariablePhotoSequenceComplete, startVariablePhotoSequenceError);
    }
    catch (exception) {
        logMessage('VariablePhotoSequenceCapture.startAsync: ');
        capturePhotoSessionComplete = true;
    }
}

function startVariablePhotoSequenceComplete() {
    logMessage('\nVariablePhotoSequenceCapture.startAsync succeeded');

    setTimeout(stopVariablePhotoSequence, 10000);

}

function startVariablePhotoSequenceError() {
    logMessage('VariablePhotoSequenceCapture.startAsync failed');
}

function variablePhotoSequenceOp_Stopped(handler) {
    if (photosCapturedCount === MAX_FRAME_CONTROLLERS) {
        logMessage('\nSuccess: Stopped event recevied after: ' + MAX_FRAME_CONTROLLERS + ' frames!');
        vpsStopped = true;

    }
}

// Custom Scaling code for low memory devices
function customScaling(stream) {
    logMessage("Scaling image from stream");
    logMessage(stream.contentType);
    var destStream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
    var contentType = stream.contentType;

    Windows.Graphics.Imaging.BitmapDecoder.createAsync(stream).then(function (decoder) {
        logMessage("Created Decoder");
        Windows.Graphics.Imaging.BitmapEncoder.createForTranscodingAsync(destStream, decoder).then(function (encoder) {
            logMessage("Created Encoder");
            encoder.bitmapTransform.scaledWidth = 640;
            encoder.bitmapTransform.scaledHeight = 480;

            encoder.bitmapTransform.interpolationMode = Windows.Graphics.Imaging.BitmapInterpolationMode.fant;

            encoder.flushAsync().done(function () {
                logMessage("Encoded stream");
                setImage(destStream, true, contentType);
                destStream.close();
            });
        });
    });
}

// Set the image in the image tag
function setImage(stream, fMakeBlobFirst, contentType) {
    logMessage("Setting image src");

    var img = "imageBox" + imgCount;
    logMessage(img);

    var imgTag = document.getElementById(img);

    if (fMakeBlobFirst) {
        logMessage("Creating blob");
        var blob = MSApp.createBlobFromRandomAccessStream(contentType, stream);
        imgTag.src = URL.createObjectURL(blob, { oneTimeOnly: true });
    } else {
        imgTag.src = URL.createObjectURL(stream, { oneTimeOnly: true });
    }
    imgTag.onerror = function (evt) {
        logMessage("Image on error fired");
    };
    imgCount++;
}


// Photo Captured event handler this event gets called when VPS generates each frame
function variablePhotoSequenceOp_PhotoCaptured(capturedFrame) {
    ++photosCapturedCount;
    logMessage("PhotoCaptures " + photosCapturedCount);
    try {
        Windows.Storage.KnownFolders.picturesLibrary.createFileAsync(photoFile, Windows.Storage.CreationCollisionOption.generateUniqueName).done(
        function (newPhoto) {
            try {
                var photoStorage1 = newPhoto;
                newPhoto.openAsync(Windows.Storage.FileAccessMode.readWrite).done(
                   function (newPhotoStream) {
                       Windows.Storage.Streams.RandomAccessStream.copyAndCloseAsync(capturedFrame.frame, newPhotoStream).done(
                           function () {
                               var fApplyScaling = 1;

                               if (fApplyScaling) {
                                   customScaling(capturedFrame.frame);
                               } else {
                                   setImage(capturedFrame.frame, false, "");
                               }
                           },
                           function (cError) {
                               logMessage("copyAndCloseAsync threw exception " + (cError.number >>> 0).toString(16));
                           });

                   },
                   function (pError) {
                       logMessage('openAsync failed' + (pError.number >>> 0).toString(16));
                   });
            }
            catch (exception) {
                logMessage('variablePhotoSequenceOp_PhotoCaptured: writing to file failed: ' + exception.message);
            }
        },
        function (fError) {
            logMessage('createFileAsync failed' + (fError.number >>> 0).toString(16));
        }
    // Create the file that will be used to store the picture
    );
    }
    catch (cException) {
        logMessage('photoSequenceOp_PhotoCaptured: createFileAsync failed: ' + cException.message);
        capturePhotoSessionComplete = true;
    }
}

function stopVariablePhotoSequence() {

    if (!vpsStopped) {
        logMessage('\nError: Stopped event is still not received!');
    }

    try {
        logMessage('\nCalling VariablePhotoSequenceCapture.stopAsync');
        variablePhotoSequenceOp.stopAsync().done(
            stopVariablePhotoSequenceComplete, stopVariablePhotoSqeuenceError);
    }
    catch (exception) {
        logMessage('VariablePhotoSequenceCapture.stopAsync failed: ' + exception.message);
        capturePhotoSessionComplete = true;
    }
}

function stopVariablePhotoSequenceComplete() {

    logMessage('\nVariablePhotoSequenceCapture.stopAsync succeeded.');

    finishVariablePhotoSequence();
}

function stopVariablePhotoSqeuenceError() {
    logMessage('VariablePhotoSequenceCapture.stopAsync failed' + (error.number >>> 0).toString(16));
}

function finishVariablePhotoSequence() {
    try {
        logMessage('\nCalling VariablePhotoSequenceCapture.finishAsync');
        variablePhotoSequenceOp.finishAsync().done(
            finishVariablePhotoSequenceComplete, finishVariablePhotoSqeuenceError);
    }
    catch (exception) {
        logMessage('VariablePhotoSequenceCapture.finishAsync failed: ' + exception.message);
        capturePhotoSessionComplete = true;
    }
}

function finishVariablePhotoSequenceComplete() {
    variablePhotoSequenceOp = null;
    logMessage('\n*** VariablePhotoSequenceCapture.finishAsync succeeded.  Photo Capture Session Complete ***\n');

    capturePhotoSessionComplete = true;

}

function finishVariablePhotoSqeuenceError() {
    logMessage('VariablePhotoSequenceCapture.finishAsync failed');
    capturePhotoSessionComplete = true;
}

// **************************************************//
// MediaCapture Core API functionality
// **************************************************//
function initializeMediaCaptureSession() {

    if (mediaCaptureMgr === null) {
        mediaCaptureMgr = new Capture.MediaCapture();
    }

    mediaCaptureMgr.addEventListener("failed", errorEventHandler, false);
    var initSettings = new Capture.MediaCaptureInitializationSettings();
    initSettings.audioDeviceId = requestedMicrophone;
    initSettings.videoDeviceId = requestedWebcam;
    initSettings.streamingCaptureMode = Capture.StreamingCaptureMode.audioAndVideo;
    initSettings.photoCaptureSource = Capture.PhotoCaptureSource.auto;
    logMessage('MediaCapture object Created.  ');

    try {
        mediaCaptureMgr.initializeAsync(initSettings).
            done(initializeComplete, initializeError);

    } catch (ex) {
        logMessage('An exception occurred trying to initialize device: ' + ex.message);
        mediaCaptureMgr = null;
        window.CollectGarbage();
    }
}

// Setup the UI controls after initialization completes
function initializeComplete() {
    initialized = true;
    logMessage('Device Initialized');

    document.getElementById("btnStartDevice").textContent = "Stop Device";
    setVideoCaptureDeviceList();
    setAudioCaptureDeviceList();

    // Enclosure Info
    var enclosureLocation = webcamList[0].enclosureLocation;
    if (enclosureLocation) {
        if (enclosureLocation.panel === Windows.Devices.Enumeration.Panel.back) {
            logMessage("Rear Facing (away from user) Camera Initialized");
            rotateVideoOnOrientationChange = true;
            reverseVideoRotation = false;
        } else if (enclosureLocation.panel === Windows.Devices.Enumeration.Panel.front) {
            logMessage("Front Facing (towards user) Camera Initialized");
            rotateVideoOnOrientationChange = true;
            reverseVideoRotation = true;
        } else {
            rotateVideoOnOrientationChange = false;
        }
    }
    else {
        logMessage("No PLD location info present for this webcam");
        rotateVideoOnOrientationChange = false;
    }

    // Photo Capture Source
    var photoSource = mediaCaptureMgr.mediaCaptureSettings.photoCaptureSource;
    if (photoSource === Capture.PhotoCaptureSource.videoPreview) {
        logMessage("Photo Capture Source is the VideoPreview Stream");
    } else if (photoSource === Capture.PhotoCaptureSource.photo) {
        logMessage("Photo Capture Source is the Photo Stream");
    } else if (photoSource === Capture.PhotoCaptureSource.auto) {
        logMessage("Photo Capture Source is Auto (Error)");
    } else {
        logMessage("***Something very wrong happened.  Check the Photo Capture Source***");
    }
    // Start Preview
    startPreview();
    vpsController = mediaCaptureMgr.videoDeviceController.variablePhotoSequenceController;
    var selFrames = document.getElementById("lstFrames");
    var j = 0;
    for(j = 0;j<selFrames.options.length;j++){
        selFrames.options[j] = null;
    }
    
    if (!vpsController.supported) {
        document.getElementById("btnVPS").disabled = true;
        document.getElementById("lstFrames").disabled = true;
        document.getElementById("btnVPSAddFrame").disabled = true;
        document.getElementById("btnVPSRemoveFrame").disabled = true;
    } else {

        document.getElementById("lstFrames").disabled = false;
        document.getElementById("btnVPSAddFrame").disabled = false;
        document.getElementById("btnVPSRemoveFrame").disabled = false;
    }
    document.getElementById("btnPreview").disabled = false;
    rotWidth = previewTag.width;
    rotHeight = previewTag.height;
    setupVPSDeviceControls();
}

// Per Frame Settings Display page
function setFrame() {
    try {
        document.getElementById("divVPSPerFrameFlyout").style.visibility = 'visible';
        document.getElementById("divVPSPerFrameFlyout").style.display = 'block';

        var sel = document.getElementById("lstFrames");

        logMessage('Frame[' + sel.options[lstFrames.selectedIndex].value + ']');

        logMessage('Current Exposure Value =' + frameControllers[lstFrames.selectedIndex].exposureControl.value);
        var control = document.getElementById(vpsPerFrameControlList[0].control);
        var autoControl = document.getElementById(vpsPerFrameControlList[0].autoControl);
        control.value = frameControllers[lstFrames.selectedIndex].exposureControl.value;
        autoControl.checked = frameControllers[lstFrames.selectedIndex].exposureControl.auto;
        var label = document.getElementById('lblExposure');
        if (!autoControl.checked) {
            control.disabled = false;
            
            label.value = "Exposure:" + control.value;
        } else {
            control.disabled = true;

            label.value = "Exposure: Auto";
        }

        logMessage('Current IsoSpeed Value =' + frameControllers[lstFrames.selectedIndex].isoSpeedControl.value);
        autoControl = document.getElementById(vpsPerFrameControlList[1].autoControl);
        control = document.getElementById(vpsPerFrameControlList[1].control);
        control.value = frameControllers[lstFrames.selectedIndex].isoSpeedControl.value;
        autoControl.checked = frameControllers[lstFrames.selectedIndex].isoSpeedControl.auto;
        var isoLabel = document.getElementById('lblIsoSpeed');
        if (!autoControl.checked) {
            control.disabled = false;
            
            isoLabel.value = "IsoSpeed:" + control.value;
        } else {
            control.disabled = true;
            isoLabel.value = "IsoSpeed: Auto";
        }

        logMessage('Current ExpCmp Value =' + frameControllers[lstFrames.selectedIndex].exposureCompensationControl.value);
        control = document.getElementById(vpsPerFrameControlList[2].control);
        control.value = frameControllers[lstFrames.selectedIndex].exposureCompensationControl.value;
        var expLabel = document.getElementById('lblExpC');
        expLabel.value = "ExpCompensation:" + control.value;

        logMessage('Current Focus Control Value =' + frameControllers[lstFrames.selectedIndex].focusControl.value);
        control = document.getElementById(vpsPerFrameControlList[3].control);
        control.value = frameControllers[lstFrames.selectedIndex].focusControl.value;
        var focusLabel = document.getElementById('lblFocus');
        focusLabel.value = "Focus:" + control.value;
    } catch (setFrameError) {
        logMessage('The following error occurred in set frame: ' + (setFrameError.number >>> 0).toString(16));
    }
}

function initializeError(initError) {
    logMessage('Initialization failed with the error: ' + (initError.number >>> 0).toString(16));
}

// Start Preview
function startPreview() {
    try {
        var previewVidTag = document.getElementById("previewTag");

        if (previewUrl === null) {
            previewUrl = URL.createObjectURL(mediaCaptureMgr);
            logMessage('URL creation succeeded. ');
            previewVidTag.src = previewUrl;
        }

        previewVidTag.play();
        document.getElementById("btnPreview").textContent = "Stop Preview";
        previewStarted = true;
        logMessage('Start Preview Succeeded. ');
        updatePreviewForRotation();
        displayReq.requestActive();

    } catch (startPreviewError) {
        logMessage('The following error occurred attempting to start Preview: ' + (startPreviewError.number >>> 0).toString(16));
    }
}

function stopPreview() {
    try {
        document.getElementById('previewTag').pause();
        document.getElementById("btnPreview").textContent = "Start Preview";
        previewStarted = false;
        logMessage('Stop Preview Succeeded. ');

    } catch (stopPreviewError) {
        logMessage('The following error occurred attempting to start Preview: ' + (stopPreviewError.number >>> 0).toString(16));
    }
}

// **************************************************//
// VPS Controls
// **************************************************//

function getVPSPerFrameControl(controlName) {
    if (controlName === "Exposure") {
        return vpsController.frameCapabilities.exposure;
    } else if (controlName === "ExposureCompensation") {
        return vpsController.frameCapabilities.exposureCompensation;
    } else if (controlName === "Focus") {
        return vpsController.frameCapabilities.focus;
    } else if (controlName === "IsoSpeed") {
        return vpsController.frameCapabilities.isoSpeed;
    }
    else {
        return null;
    }
}

//Setup VPS UI controls
function setupVPSPerFrameControl(deviceControl, rangeControl, autoControl) {
    if (deviceControl.supported) {

        rangeControl.max = deviceControl.max;
        rangeControl.min = deviceControl.min;
        rangeControl.step = deviceControl.step;

        var controlCurrentValue = deviceControl.Value;
        rangeControl.value = controlCurrentValue;
        if (autoControl !== "chkNULL") {
            autoControl.disabled = false;
            autoControl.checked = deviceControl.auto;
            if (deviceControl.auto) {
                rangeControl.disabled = true;
            } else {
                rangeControl.disabled = false;
            }

        } else {
            rangeControl.disabled = false;
        }
    }
    else {
        rangeControl.disabled = true;

    }
}

function setupVPSDeviceControls() {
    var numControls = vpsPerFrameControlList.length;
    var index = 0;
    for (index = 0; index < numControls; index++) {
        var deviceControl = getVPSPerFrameControl(vpsPerFrameControlList[index].controlName);
        logMessage('Control :' + vpsPerFrameControlList[index].controlName);
        var control = document.getElementById(vpsPerFrameControlList[index].control);
        var autoControl = null;
        if (vpsPerFrameControlList[index].autoControl !== "chkNULL") {
            autoControl = document.getElementById(vpsPerFrameControlList[index].autoControl);
        } else {
            autoControl = "chkNULL";
        }
        setupVPSPerFrameControl(deviceControl, control, autoControl);
        document.getElementById("lstFrames").selectedIndex = -1;

    }
    
}

// **************************************************//
// WWA UI Controls setup
// **************************************************//
function setVideoCaptureDeviceList() {
    // Webcams
    if (!webcamListSet) {
        var videoDeviceSelectorString = Windows.Media.Devices.MediaDevice.getVideoCaptureSelector();
        var propertiesToEnumerate = new Array();
        propertiesToEnumerate.push("{a45c254e-df1c-4efd-8020-67d146a850e0} 14");
        try {
            return Windows.Devices.Enumeration.DeviceInformation.findAllAsync(videoDeviceSelectorString, propertiesToEnumerate)
                .done(populateWebcamList, enumWebcamsError);
        }
        catch (ex) {
            logMessage('Windows.Devices.Enumeration.DeviceInformation.findAllAsync triggered an exception: ' + ex.message);
        }
    }
}

function populateWebcamList(enumeratedWebcams) {
    webcamList = enumeratedWebcams;
    webcamListSet = true;
}

function enumWebcamsError(ex) {
    logMessage('An exception occurred trying to Enumerate all webcams: ' + ex.message);
}

function setAudioCaptureDeviceList() {
    // Webcams
    if (!microphoneListSet) {
        var audioDeviceSelectorString = Windows.Media.Devices.MediaDevice.getAudioCaptureSelector();
        var propertiesToEnumerate = new Array();
        propertiesToEnumerate.push("{a45c254e-df1c-4efd-8020-67d146a850e0} 14");

        Windows.Devices.Enumeration.DeviceInformation.findAllAsync(audioDeviceSelectorString, propertiesToEnumerate)
            .done(populateMicrophoneList, enumMicrophonesError);
    }
}

function populateMicrophoneList(enumeratedMicrophones) {
    microphoneList = enumeratedMicrophones;
    microphoneListSet = true;
}

function enumMicrophonesError(exMsg) {
    logMessage('An exception occurred trying to Enumerate all Microphones: ' + exMsg.message);
}



// **************************************************//
// Capture Session Methods
// **************************************************//
function refreshCaptureSession(doReset) {

        shutdownCaptureSession(doReset);

}

function shutdownCaptureSession(doReset) {
    if (variablePhotoSequenceOp !== null) {
        finishVariablePhotoSequence();
    }
    clrImgTags();
    document.getElementById('previewTag').pause();
    document.getElementById('previewTag').src = null;
    previewUrl = null;
    document.getElementById('btnPreview').textContent = "Start Preview";
    previewStarted = false;
    resetMediaCaptureSettingsLists();
    mediaCaptureMgr = null;

    window.CollectGarbage();

    if (document.msVisibilityState === "visible" && initialized && previewStarted) {
        displayReq.requestRelease();
    }

    initialized = false;
    logMessage('Stopping Device');
    document.getElementById("btnStartDevice").textContent = "Start Device";
    document.getElementById('btnPreview').disabled = true;
    document.getElementById("btnVPS").disabled = true;
    document.getElementById("lstFrames").disabled = true;
    document.getElementById("btnVPSAddFrame").disabled = true;
    document.getElementById("btnVPSRemoveFrame").disabled = true;
    document.getElementById("divVPSPerFrameFlyout").style.display = 'none';

    if (doReset) {
        initializeMediaCaptureSession();
    }
}

function resetMediaCaptureSettingsLists() {
    webcamListSet = false;
    microphoneListSet = false;
}




// **************************************************//
// Event Handling
// **************************************************//
function plmActivateMessageHandler() {
    logMessage('App has been activated');
    setVideoCaptureDeviceList();
    setAudioCaptureDeviceList();
    document.getElementById("divVPSPerFrameFlyout").style.display = 'none';
}

function plmSuspendingMessageHandler(eventArgs) {
    logMessage('App has been suspended');
    refreshCaptureSession(STOP_SESSION);
}

function plmResumingMessageHandler() {
    logMessage('App has been resumed');
    initializeMediaCaptureSession();
}

function updatePreviewForRotation() {
    if (!mediaCaptureMgr) {
        return;
    }
      try {
          var rotDegree;
          var videoEncodingProperties = mediaCaptureMgr.videoDeviceController.getMediaStreamProperties(Windows.Media.Capture.MediaStreamType.VideoPreview);

          var previewMirroring = mediaCaptureMgr.getPreviewMirroring();
          var counterclockwiseRotation = (previewMirroring && !reverseVideoRotation) ||
                                        (!previewMirroring && reverseVideoRotation);

          //Set the video subtype
          var rotGUID = "{0xC380465D, 0x2271, 0x428C, {0x9B, 0x83, 0xEC, 0xEA, 0x3B, 0x4A, 0x85, 0xC1}}";


          if (rotateVideoOnOrientationChange) {
               // Lookup up the rotation degrees.  
                rotDegree = videoPreviewRotationLookup(Windows.Graphics.Display.DisplayInformation.getForCurrentView().currentOrientation, counterclockwiseRotation);

                // Rotate the preview video
                 videoEncodingProperties.properties.insert(rotGUID, rotDegree);
                 mediaCaptureMgr.setEncodingPropertiesAsync(Windows.Media.Capture.MediaStreamType.VideoPreview, videoEncodingProperties, null).done(
                 function () {
                    if (rotDegree === 90 || rotDegree === 270) {
                        previewTag.Height = rotWidth;
                        previewTag.Width = rotHeight;
                    } else {
                        previewTag.height = rotHeight;
                        previewTag.width = rotWidth;
                    }
                    });
             } else {
                 mediaCaptureMgr.setPreviewRotation(Windows.Media.Capture.VideoRotation.none);
             }
        }
        catch (ex) {
            logMessage('Rotation Handler triggered an exception: ' + ex.message);
        }
}





function updateRecordForRotation() {
    if (!mediaCaptureMgr) {
        return;
    }
    var counterclockwiseRotation = reverseVideoRotation;

    if (rotateVideoOnOrientationChange) {
        mediaCaptureMgr.setRecordRotation(videoRotationLookup(Windows.Graphics.Display.DisplayInformation.getForCurrentView().currentOrientation, counterclockwiseRotation));
    } else {
        mediaCaptureMgr.setRecordRotation(Windows.Media.Capture.VideoRotation.none);
    }
}

function getCurrentPhotoRotation() {
    var counterclockwiseRotation = reverseVideoRotation;

    if (rotateVideoOnOrientationChange) {
        return photoRotationLookup(Windows.Graphics.Display.DisplayInformation.getForCurrentView().currentOrientation, counterclockwiseRotation);
    } else {
        return Windows.Storage.FileProperties.PhotoOrientation.normal;
    }
}

function photoRotationLookup(displayOrientation, counterclockwise) {
    switch (displayOrientation) {
        case Windows.Graphics.Display.DisplayOrientations.landscape:
            return Windows.Storage.FileProperties.PhotoOrientation.normal;

        case Windows.Graphics.Display.DisplayOrientations.portrait:
            return (counterclockwise) ? Windows.Storage.FileProperties.PhotoOrientation.rotate90 :
                Windows.Storage.FileProperties.PhotoOrientation.rotate270;

        case Windows.Graphics.Display.DisplayOrientations.landscapeFlipped:
            return Windows.Storage.FileProperties.PhotoOrientation.rotate180;

        case Windows.Graphics.Display.DisplayOrientations.portraitFlipped:
            return (counterclockwise) ? Windows.Storage.FileProperties.PhotoOrientation.rotate270 :
                Windows.Storage.FileProperties.PhotoOrientation.rotate90;

        default:
            return Windows.Storage.FileProperties.PhotoOrientation.unspecified;
    }
}

function videoRotationLookup(displayOrientation, counterclockwise) {
    switch (displayOrientation) {
        case Windows.Graphics.Display.DisplayOrientations.landscape:
            return Windows.Media.Capture.VideoRotation.none;

        case Windows.Graphics.Display.DisplayOrientations.portrait:
            return (counterclockwise) ? Windows.Media.Capture.VideoRotation.clockwise270Degrees :
                Windows.Media.Capture.VideoRotation.clockwise90Degrees;

        case Windows.Graphics.Display.DisplayOrientations.landscapeFlipped:
            return Windows.Media.Capture.VideoRotation.clockwise180Degrees;

        case Windows.Graphics.Display.DisplayOrientations.portraitFlipped:
            return (counterclockwise) ? Windows.Media.Capture.VideoRotation.clockwise90Degrees :
                Windows.Media.Capture.VideoRotation.clockwise270Degrees;

        default:
            return Windows.Media.Capture.VideoRotation.none;
    }
}

function videoPreviewRotationLookup(displayOrientation, counterclockwise) {
    var degreesToRotate;

    switch (displayOrientation) {
        case Windows.Graphics.Display.DisplayOrientations.landscape:
            degreesToRotate = 0;
            break;
        case Windows.Graphics.Display.DisplayOrientations.portrait:
            if (counterclockwise) {
                degreesToRotate = 270;
            } else {
                degreesToRotate = 90;
            }
            break;
        case Windows.Graphics.Display.DisplayOrientations.landscapeFlipped:
            degreesToRotate = 180;
            break;
        case Windows.Graphics.Display.DisplayOrientations.portraitFlipped:
            if (counterclockwise) {
                degreesToRotate = 90;
            } else {
                degreesToRotate = 270;
            }
            break;
        default:
            degreesToRotate = 0;
            break;
    }

    return degreesToRotate;
}

function failedEventHandler(failedError) {
    logMessage("FATAL ERROR: " + (failedError.code >>> 0).toString(16));
}

function errorEventHandler(eventError) {
    var errorString = (eventError.code >>> 0).toString(16);
    if (errorString.toLocaleLowerCase() === "c00d3ea3") {
        refreshCaptureSession(STOP_SESSION);
        logMessage("The Camera has been grabbed by another application.  Click 'Start Device' to reclaim");
    } else if (errorString.toLocaleLowerCase() === "c00d36b4") {
        refreshCaptureSession(STOP_SESSION);
        logMessage("MediaCapture Error Occurred: MF_E_INVALIDMEDIATYPE.  Click 'Start Device' to resume capture session");
    } else {
        refreshCaptureSession(STOP_SESSION);
        logMessage("MediaCapture Error Occurred: " + errorString + ".  Click 'Start Device' to resume capture session");
    }
}

//**************************************************//
// Logging
//**************************************************//
function logMessage(message) {
    document.getElementById('log').innerHTML += message;
    document.getElementById('log').appendChild(br);
    var t = document.getElementById('log');
    t.scrollTop = t.scrollHeight;
}

