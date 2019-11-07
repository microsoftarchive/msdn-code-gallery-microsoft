//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

var videodev = null;
var videoext = null;
var lcWrapper = null;

function activatedHandler(eventArgs) {
    if (eventArgs.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.cameraSettings) {
        videoext = eventArgs.detail.videoDeviceExtension;
        videodev = eventArgs.detail.videoDeviceController;

        if (videoext !== null) {
            initializeExtension(videoext);
        }

        if (videodev !== null) {
            initializeUi();
        }
    }
}

WinJS.Application.start();
WinJS.Application.addEventListener("activated", activatedHandler, false);

function initializeExtension(mft0) {
    lcWrapper = Wrapper.WinRTComponent();
    lcWrapper.initialize(mft0);
}

function initializeUi() {
    var bValue = 0;
    var bAuto = 0;

    if (videodev.brightness.capabilities.step !== 0) {
        bValue = videodev.brightness.tryGetValue();
        document.getElementById("slBrt").value = bValue.value;
        document.getElementById("slBrt").min = videodev.brightness.capabilities.min;
        document.getElementById("slBrt").max = videodev.brightness.capabilities.max;
        document.getElementById("slBrt").step = videodev.brightness.capabilities.step;
    } else {
        document.getElementById("slBrt").disabled = "disabled";
    }
    if (videodev.brightness.capabilities.autoModeSupported) {
        bAuto = videodev.brightness.tryGetAuto();
        initializeAuto("btBrtAuto", "slBrt", "lbBrtAutoStatus", bAuto);
    } else {
        document.getElementById("slBrtAuto").disabled = "disabled";
        document.getElementById("lbBrtAutoStatus").innerText = "Manual";
    }

    if (videodev.contrast.capabilities.step !== 0) {
        bValue = videodev.contrast.tryGetValue();
        document.getElementById("slCrt").value = bValue.value;
        document.getElementById("slCrt").min = videodev.contrast.capabilities.min;
        document.getElementById("slCrt").max = videodev.contrast.capabilities.max;
        document.getElementById("slCrt").step = videodev.contrast.capabilities.step;
    } else {
        document.getElementById("slCrt").disabled = "disabled";
    }
    if (videodev.contrast.capabilities.autoModeSupported) {
        bAuto = videodev.contrast.tryGetAuto();
        initializeAuto("btCrtAuto", "slCrt", "lbCrtAutoStatus", bAuto);
    } else {
        document.getElementById("slCrtAuto").disabled = "disabled";
        document.getElementById("lbCrtAutoStatus").innerText = "Manual";
    }

    if (videodev.focus.capabilities.step !== 0) {
        bValue = videodev.focus.tryGetValue();
        document.getElementById("slFocus").value = bValue.value;
        document.getElementById("slFocus").min = videodev.focus.capabilities.min;
        document.getElementById("slFocus").max = videodev.focus.capabilities.max;
        document.getElementById("slFocus").step = videodev.focus.capabilities.step;
    } else {
        document.getElementById("slFocus").disabled = "disabled";
    }
    if (videodev.focus.capabilities.autoModeSupported) {
        bAuto = videodev.focus.tryGetAuto();
        initializeAuto("slFocusAuto", "slFocus", "lbFocusAutoStatus", bAuto);
    } else {
        document.getElementById("slFocusAuto").disabled = "disabled";
        document.getElementById("lbFocusAutoStatus").innerText = "Manual";
    }

    if (videodev.exposure.capabilities.step !== 0) {
        bValue = videodev.exposure.tryGetValue();
        document.getElementById("slExp").value = bValue.value;
        document.getElementById("slExp").min = videodev.exposure.capabilities.min;
        document.getElementById("slExp").max = videodev.exposure.capabilities.max;
        document.getElementById("slExp").step = videodev.exposure.capabilities.step;
    } else {
        document.getElementById("slExp").disabled = "disabled";
    }
    if (videodev.exposure.capabilities.autoModeSupported) {
        bAuto = videodev.exposure.tryGetAuto();
        initializeAuto("slExpAuto", "slExp", "lbExpAutoStatus", bAuto);
    } else {
        document.getElementById("slExpAuto").disabled = "disabled";
        document.getElementById("lbExpAutoStatus").innerText = "Manual";
    }

    if (lcWrapper !== null) {
        var settings = lcWrapper.getDspSetting();

        document.getElementById("slEffect").value = settings.percentOfScreen;
        document.getElementById("slEffect").min = 0;
        document.getElementById("slEffect").max = 100;
        document.getElementById("slEffect").step = 1;
        document.getElementById("slEffectOnOff").disabled = "";

        if (!settings.isEnabled) {
            document.getElementById("slEffectOnOff").value = 0;
            document.getElementById("lbEffectOnOffStatus").innerText = "Off";
            document.getElementById("slEffect").disabled = "disabled";
        } else {
            document.getElementById("slEffectOnOff").value = 1;
            document.getElementById("lbEffectOnOffStatus").innerText = "On";
            document.getElementById("slEffect").disabled = "";
        }
    } else {
        document.getElementById("slEffectOnOff").disabled = "disabled";
        document.getElementById("slEffect").disabled = "disabled";
    }
}

function setBrtValue(newValue) {
    videodev.brightness.trySetValue(newValue);
}

function setCrtValue(newValue) {
    videodev.contrast.trySetValue(newValue);
}

function setFocusValue(newValue) {
    videodev.focus.trySetValue(newValue);
}

function setExposureValue(newValue) {
    videodev.exposure.trySetValue(newValue);
}

function setEffectValue(newValue) {
    lcWrapper.updateDsp(newValue);
}

function initializeAuto(autoName, sliderName, autoStatus, value) {
    if (value) {
        document.getElementById(autoName).value = 1;
        document.getElementById(sliderName).disabled = "disabled";
        document.getElementById(autoStatus).innerText = "Auto";
    } else {
        document.getElementById(autoName).value = 0;
        document.getElementById(sliderName).disabled = "";
        document.getElementById(autoStatus).innerText = "Manual";
    }
}

function onBrtAutoSliderChange(newValue) {
    if (newValue === "0") {
        videodev.brightness.trySetAuto(false);
        document.getElementById("lbBrtAutoStatus").innerText = "Manual";
        document.getElementById("slBrt").disabled = "";
    } else {
        videodev.brightness.trySetAuto(true);
        document.getElementById("lbBrtAutoStatus").innerText = "Auto";
        document.getElementById("slBrt").disabled = "disabled";
    }
}

function onCrtAutoSliderChange(newValue) {
    if (newValue === "0") {
        videodev.contrast.trySetAuto(false);
        document.getElementById("lbCrtAutoStatus").innerText = "Manual";
        document.getElementById("slCrt").disabled = "";
    } else {
        videodev.contrast.trySetAuto(true);
        document.getElementById("lbCrtAutoStatus").innerText = "Auto";
        document.getElementById("slCrt").disabled = "disabled";
    }
}

function onFocusAutoSliderChange(newValue) {
    if (newValue === "0") {
        videodev.focus.trySetAuto(false);
        document.getElementById("lbFocusAutoStatus").innerText = "Manual";
        document.getElementById("slFocus").disabled = "";
    } else {
        videodev.focus.trySetAuto(true);
        document.getElementById("lbFocusAutoStatus").innerText = "Auto";
        document.getElementById("slFocus").disabled = "disabled";
    }
}

function onExpAutoSliderChange(newValue) {
    if (newValue === "0") {
        videodev.exposure.trySetAuto(false);
        document.getElementById("lbExpAutoStatus").innerText = "Manual";
        document.getElementById("slExp").disabled = "";
    } else {
        videodev.exposure.trySetAuto(true);
        document.getElementById("lbExpAutoStatus").innerText = "Auto";
        document.getElementById("slExp").disabled = "disabled";
    }
}

function onEffectOnOffSliderChange(newValue) {
    if (newValue === "0") {
        lcWrapper.disable();
        document.getElementById("lbEffectOnOffStatus").innerText = "Off";
        document.getElementById("slEffect").disabled = "disabled";
    } else {
        lcWrapper.enable();
        document.getElementById("lbEffectOnOffStatus").innerText = "On";
        document.getElementById("slEffect").disabled = "";
    }
}
