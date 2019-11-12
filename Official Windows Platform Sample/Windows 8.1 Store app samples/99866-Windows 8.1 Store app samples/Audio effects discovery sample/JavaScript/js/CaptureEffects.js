//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {

    "use strict";
    var WM = Windows.Media;
    var WME = Windows.Media.Effects;
    var WMD = Windows.Media.Devices;
    var WMDM = Windows.Media.Devices.MediaDevice;
    var WMC = Windows.Media.Capture;
    var WDE = Windows.Devices.Enumeration;
    var monitorStarted = false;
    var captureEffectsManager = null;
    var deviceList = null;

    var page = WinJS.UI.Pages.define("/html/CaptureEffects.html", {
        ready: function (element, options) {
            scenarioInitialize();
        },

        unload: function (element, options) {
            scenarioClose();
        }
    });

    function categoryTypeToString(category) {
        switch (category) {
            case WMC.MediaCategory.other:
                return "Other";
            case WMC.MediaCategory.communications:
                return "Communications";
            default:
                return "Unknown";
        }
    }

    function effectsTypeToString(effects) {
        switch (effects) {
            case WME.AudioEffectType.other:
                return "Other";
            case WME.AudioEffectType.acousticEchoCancellation:
                return "AcousticEchoCancellation";
            case WME.AudioEffectType.noiseSuppression:
                return "NoiseSuppression";
            case WME.AudioEffectType.automaticGainControl:
                return "AutomaticGainControl";
            case WME.AudioEffectType.beamForming:
                return "BeamForming";
            case WME.AudioEffectType.constantToneRemoval:
                return "ConstantToneRemoval";
            case WME.AudioEffectType.equalizer:
                return "Equalizer";
            case WME.AudioEffectType.loudnessEqualizer:
                return "LoudnessEqualizer";
            case WME.AudioEffectType.bassBoost:
                return "BassBoost";
            case WME.AudioEffectType.virtualSurround:
                return "VirtualSurround";
            case WME.AudioEffectType.virtualHeadphones:
                return "VirtualHeadphones";
            case WME.AudioEffectType.speakerFill:
                return "SpeakerFill";
            case WME.AudioEffectType.roomCorrection:
                return "RoomCorrection";
            case WME.AudioEffectType.bassManagement:
                return "BassManagement";
            case WME.AudioEffectType.environmentalEffects:
                return "EnvironmentalEffects";
            case WME.AudioEffectType.speakerProtection:
                return "SpeakerProtection";
            case WME.AudioEffectType.speakerCompensation:
                return "SpeakerCompensation";
            case WME.AudioEffectType.dynamicRangeCompression:
                return "DynamicRangeCompression";
            default:
                return "Unknown";
        }
    }

    function scenarioInitialize() {
        id("btnStartStopMonitor").addEventListener("click", startStopMonitor, false);
        id("btnStartStopMonitor").disabled = false;
        id("btnRefreshEffects").addEventListener("click", refreshList, false);
        id("btnRefreshEffects").disabled = false;
        id("btnEnumerateDevices").addEventListener("click", enumerateDevices, false);
        id("btnEnumerateDevices").disabled = false;
        var categoryListBox = id("categoriesList");
        categoryListBox.options.length = 0;
        for (var i = WMC.MediaCategory.other; i <= WMC.MediaCategory.communications; i++) {
            categoryListBox.options[categoryListBox.options.length] = new Option(categoryTypeToString(i), i);
        }

        monitorStarted = false;
        captureEffectsManager = null;
        deviceList = null;
        displayEmptyDevicesList();
        displayEmptyEffectsList();
    }

    function scenarioClose() {
        if (captureEffectsManager !== null) {
            captureEffectsManager.removeEventListener("audiocaptureeffectschanged", onCaptureEffectsChanged, true);
            captureEffectsManager = null;
        }
    }

    function failedEventHandler(e) {
        displayError("Fatal error", e.message);
    }

    function displayStatus(statusText) {
        SdkSample.displayStatus(statusText);
    }

    function displayError(error) {
        SdkSample.displayError(error);
    }

    function id(elementId) {
        return document.getElementById(elementId);
    }

    function errorHandler(err) {
        displayError(err.message);
    }

    function onCaptureEffectsChanged(sender) {
        var effectsList = captureEffectsManager.getAudioCaptureEffects();

        displayEffectsList(effectsList);
    }

    function startStopMonitor() {
        var btnStartStopMonitor = id("btnStartStopMonitor");
        var devicesListBox = id("devicesList");
        var effectsLabelBox = id("lblEffectsList");
        var effectsList;
        var categoryListBox = id("categoriesList");
        var category;

        if (monitorStarted === true) {
            if (captureEffectsManager !== null) {
                captureEffectsManager.removeEventListener("audiocaptureeffectschanged", onCaptureEffectsChanged, true);
                captureEffectsManager = null;
            }
            monitorStarted = false;
            // No longer monitoring effects changes; re-enable button for "Refresh Effects List"
            id("btnRefreshEffects").disabled = false;
            // Reset effects list to empty
            effectsLabelBox.innerHTML = "Effects";
            displayEmptyEffectsList();
            btnStartStopMonitor.innerHTML = "Start Monitoring";
        }
        else {
            category = categoryListBox.selectedIndex;

            if (devicesListBox.selectedIndex < 0) {
                captureEffectsManager = WME.AudioEffectsManager.createAudioCaptureEffectsManager(
                                        WMDM.getDefaultAudioCaptureId(Windows.Media.Devices.AudioDeviceRole.Communications),
                                        category,
                                        WM.AudioProcessing.default);
                captureEffectsManager.addEventListener("audiocaptureeffectschanged", onCaptureEffectsChanged, true);

                effectsLabelBox.innerHTML = "Effects Active on {Default Device}";
            }
            else {
                captureEffectsManager = WME.AudioEffectsManager.createAudioCaptureEffectsManager(
                                        deviceList.getAt(devicesListBox.selectedIndex).id,
                                        category,
                                        WM.AudioProcessing.default);
                captureEffectsManager.addEventListener("audiocaptureeffectschanged", onCaptureEffectsChanged, true);

                effectsLabelBox.innerHTML = "Effects Active on {" + deviceList.getAt(devicesListBox.selectedIndex).name + "}";
            }

            monitorStarted = true;
            // Started monitoring effects changes; for now disable button for "Refresh Effects List"
            id("btnRefreshEffects").disabled = true;
            btnStartStopMonitor.innerHTML = "Stop Monitoring";

            // Display current effects once
            effectsList = captureEffectsManager.getAudioCaptureEffects();
            displayEffectsList(effectsList);
        }
    }

    function refreshList() {
        var devicesListBox = id("devicesList");
        var captureEffectsManagerLocal = null;
        var effectsLabelBox = id("lblEffectsList");
        var effectsList = null;
        var categoryListBox = id("categoriesList");
        var category;

        category = categoryListBox.selectedIndex;

        if (devicesListBox.selectedIndex < 0) {
            captureEffectsManagerLocal = WME.AudioEffectsManager.createAudioCaptureEffectsManager(
                                            WMDM.getDefaultAudioCaptureId(Windows.Media.Devices.AudioDeviceRole.Communications),
                                            category,
                                            WM.AudioProcessing.default);
            effectsLabelBox.innerHTML = "Effects Active on {Default Device}";
        }
        else {
            captureEffectsManagerLocal = WME.AudioEffectsManager.createAudioCaptureEffectsManager(
                                         deviceList.getAt(devicesListBox.selectedIndex).id,
                                         category,
                                         WM.AudioProcessing.default);
            effectsLabelBox.innerHTML = "Effects Active on {" + deviceList.getAt(devicesListBox.selectedIndex).name + "}";
        }

        effectsList = captureEffectsManagerLocal.getAudioCaptureEffects();
        displayEffectsList(effectsList);
        captureEffectsManagerLocal = null;
    }

    function displayEmptyEffectsList() {
        var effectsListBox = id("effectsList");
        var new_element;

        // Clear the list box contents
        while (effectsListBox.firstChild) {
            effectsListBox.removeChild(effectsListBox.firstChild);
        }

        // Insert one element as filler
        new_element = document.createElement('li');
        new_element.innerHTML = "<li style='color:#707070'>Click \"Refresh Effects List\" or \"Start Monitoring\" to display active effects</li>";
        effectsListBox.appendChild(new_element);
    }

    function displayEmptyDevicesList() {
        var devicesListBox = id("devicesList");

        // Clear the selection list box contents
        devicesListBox.options.length = 0;

        // Insert one element as filler
        devicesListBox.options[0] = new Option("Click \"Enumerate Devices\" above to display audio devices");

        // Disable selection list box
        devicesListBox.options[0].disabled = true;
    }

    function displayEffectsList(effectsList) {
        var effectsListBox = id("effectsList");
        var new_element;
        var i;

        // Clear the list box contents
        while (effectsListBox.firstChild) {
            effectsListBox.removeChild(effectsListBox.firstChild);
        }

        if (effectsList.size > 0) {
            for (i = 0; i < effectsList.size; i++) {
                new_element = document.createElement('li');
                new_element.innerHTML = effectsTypeToString(effectsList[i].audioEffectType);
                effectsListBox.appendChild(new_element);
            }
        }
        else {
            new_element = document.createElement('li');
            new_element.innerHTML = "[No Effects]";
            effectsListBox.appendChild(new_element);
        }
    }

    function enumerateDevices() {
        var DevInfo = Windows.Devices.Enumeration.DeviceInformation;

        // Capture devcies
        DevInfo.findAllAsync(Windows.Devices.Enumeration.DeviceClass.audioCapture).then(function (devices) {
            var devicesListBox = id("devicesList");
            var i;

            devicesListBox.options.length = 0;
            deviceList = devices;

            for (i = 0; i < devices.size; i++) {
                devicesListBox.options[devicesListBox.options.length] = new Option(devices.getAt(i).name, i);
            }
            devicesListBox.isDisabled = false;
        });
    }


})();
