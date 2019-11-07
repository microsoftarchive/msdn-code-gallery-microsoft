//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// --- global html variables ------
// we have only two html elements on the page: inkCanvas and statusMessage
// here we declare both for easier/faster usage below in the code
// for the same reason here we also declare a "2d" context of the inkCanvas
var inkCanvas;
var inkContext;
var statusMessage;

function initialize() {
    // obtain inkCanvas and inkContext variables from html
    // also set some default values
    inkCanvas = document.getElementById("inkCanvas");
    inkCanvas.width = window.innerWidth - 10;
    inkCanvas.height = window.innerHeight - 32;
    inkContext = inkCanvas.getContext("2d");
    inkContext.lineCap = "round";
    inkContext.lineJoin = "round";
    
    // register hooks for input processing: pointer down/update/up messages for inkCanvas element
    inkCanvas.addEventListener("pointerdown", onPointerDown, false);
    inkCanvas.addEventListener("pointermove", onPointerUpdate, false);
    inkCanvas.addEventListener("pointerup", onPointerUp, false);

    // obtain statusMessage variable from html
    // set initial value to the default (currentRecognizerId = 0) recognizer name
    statusMessage = document.getElementById("statusMessage");
    statusMessage.innerText = "Current recognizer: " + inkManager.getRecognizers()[currentRecognizerId].name;
}
document.addEventListener("DOMContentLoaded", initialize, false);

// --- global inking variables ------
// this is our one and only inking object, it enables us to cover all scenarios
// no additional inking objects are required
var inkManager = new Windows.UI.Input.Inking.InkManager();

// global current values of the recognized text, current inking color, current inking width and current recognizer id
var currentText = "";
var currentColor = Windows.UI.ColorHelper.fromArgb(255, 0, 0, 0);
var currentWidth = 2;
var currentRecognizerId = 0;
var currentPenId = -1;

// --- input processing ------
// occurs when pointer (touch, pen, mouse) first comes in contact with inkCanvas
// this is the first message for every stroke
function onPointerDown(evt) {
    // we only allow single contact at the time
    if (currentPenId === -1) {
        var current = evt.currentPoint;

        // clear selection and update the render
        if (anySelected()) {
            unselectAll();
            renderAllStrokes();
        }

        // we only consider pen and mouse (with a button set) pointer types, we ignore touch    
        if (((evt.pointerType === "pen") && (evt.button === 2)) || ((evt.pointerType === "mouse") && (evt.button === 0) && (evt.ctrlKey === true))) {
            // select mode
            statusMessage.innerText = "Select mode: draw a contour around the ink you want to select.";
            inkManager.mode = Windows.UI.Input.Inking.InkManipulationMode.selecting;

            // setup drawing attributes for live rendering
            // in selection mode we render the lasso
            inkContext.lineWidth = 0.1;
            inkContext.strokeStyle = toColorString(Windows.UI.Colors.gold);

            // start live rendering
            inkContext.beginPath();
            inkContext.moveTo(current.rawPosition.x, current.rawPosition.y);
        } else if (((evt.pointerType === "pen") && (current.properties.isEraser)) || ((evt.pointerType === "mouse") && (evt.button === 2))) {
            // erase mode
            statusMessage.innerText = "Erase mode: scribble across the ink you want to erase.";
            inkManager.mode = Windows.UI.Input.Inking.InkManipulationMode.erasing;
        } else if ((evt.pointerType === "pen") || ((evt.pointerType === "mouse") && (evt.button === 0))) {
            // ink mode
            statusMessage.innerText = "";
            inkManager.mode = Windows.UI.Input.Inking.InkManipulationMode.inking;

            // setup drawing attributes for live rendering
            // in inking mode we render the stroke
            inkContext.lineWidth = currentWidth;
            inkContext.strokeStyle = toColorString(currentColor);

            // start live rendering
            inkContext.beginPath();
            inkContext.moveTo(current.rawPosition.x, current.rawPosition.y);
        } else {
            return;
        }

        // add current point to inkManager
        inkManager.processPointerDown(current);

        // set the current pointer id based on id of the current pointer
        // we later use this pointer id to restrict input processing to this pointer only
        currentPenId = evt.pointerId;
    }
}

// for mouse messages occurs when pointer (mouse) contact is moved
// for touch and pen messages occurs at steady rate (say 100 messages per second) regardless of whether the contact moved
function onPointerUpdate(evt) {
    // we only allow one contact at the time, currentPenId is set in pointer down event handler
    if (evt.pointerId === currentPenId) {
        var current = evt.currentPoint;

        // add current point to inkManager
        var update = inkManager.processPointerUpdate(current); // the return value is a dirty rect if we are in erasing mode.
        if (inkManager.mode === Windows.UI.Input.Inking.InkManipulationMode.erasing) {
            // if the dirty rect is not empty then some strokes have been erased and we need to update the render.
            if (update.height > 0 || update.width > 0) {
                renderAllStrokes();
            }
        } else {
            // live rendering is done here
            inkContext.lineTo(current.rawPosition.x, current.rawPosition.y);
            inkContext.stroke();
        }
    }
}

// occurs when pointer (touch, pen, mouse) leaves inkCanvas
// this is the last message for every stroke
function onPointerUp(evt) {
    if (evt.pointerId === currentPenId) {
        // reset current pointer id
        currentPenId = -1;

        // add current point to inkManager (stroke is ended with this point)
        inkManager.processPointerUp(evt.currentPoint);

        // done with live rendering
        inkContext.closePath();

        // render all strokes using bezier curves (off-line rendering)
        renderAllStrokes();

        statusMessage.innerText = "";

        if (inkManager.mode === Windows.UI.Input.Inking.InkManipulationMode.inking) {
            // as a curiosity - we change the color and the width of the next stroke here
            changeDrawingAttributes();
        }
    }
}

// --- scenarios ------
// returns true if some strokes are selected, false otherwise
function anySelected() {
    var strokes = inkManager.getStrokes();
    var size = strokes.length;

    for(var i = 0; i < size; i++) {
        if (strokes[i].selected) {
            return true;
        }
    }
    return false;
}

// selects all strokes in inkManager
function selectAll() {
    var strokes = inkManager.getStrokes();
    var size = strokes.length;

    for (var i = 0; i < size; i++) {
        strokes[i].selected = 1;
    }
}

// unselects all strokes in inkManager
// this is a faster way of doing it
// alternative is to loop for all strokes and set each selected property to 0 (false)
function unselectAll() {
    var point = { x: 0, y: 0 };
    inkManager.selectWithLine(point, point);
}

// copy strokes to clipboard
// in case some strokes are selected - copy just the selected strokes
// if no strokes are selected - copy all strokes to clipboard
function copyToClipboard() {
    if (anySelected()) {
        inkManager.copySelectedToClipboard();
    } else {
        selectAll();
        inkManager.copySelectedToClipboard();
        unselectAll();
    }
}

// copy current recognition text to clipboard as text only
// currentText is set during recognition (recognize() function call)
function copyTextToClipboard() {
    var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
    dataPackage.setText(currentText);
    Windows.ApplicationModel.DataTransfer.Clipboard.setContent(dataPackage);
}

// checks if there are strokes currently placed on clipboard
// pastes from clipboard if there are
function pasteFromClipboard() {
    if (inkManager.canPasteFromClipboard()) {
        var point = { x: 100, y: 100 };
        unselectAll();
        inkManager.pasteFromClipboard(point);
        renderAllStrokes();
    }
}

// load an ink file using picker
function load() {  
    var openPicker = Windows.Storage.Pickers.FileOpenPicker();
    openPicker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.picturesLibrary;
    openPicker.fileTypeFilter.append(".gif");

    // closure variable, visible to all promises in the following chain
    var loadStream = null;
    openPicker.pickSingleFileAsync().done(
        function (file) {
            if (null !== file) {
                file.openAsync(Windows.Storage.FileAccessMode.read).then(
                    function (stream) {
                        loadStream = stream;
                        return inkManager.loadAsync(loadStream); // since we return the promise, it will be executed before the following .done
                    }
                ).done(
                    function () {
                        // done loading, print status message
                        var strokes = inkManager.getStrokes().length;
                        if (strokes === 0) {
                            statusMessage.innerText = "File does not contain any strokes!";
                        } else {
                            statusMessage.innerText = "Ink file loaded with " + strokes.toString() + " strokes!";
                        }

                        // update the canvas, render all strokes
                        renderAllStrokes();

                        loadStream.close();
                    },
                    function (e) {
                        statusMessage.innerText = "Load failed. Make sure you tried to open a file that can be read by the InkManager.";

                        // if the error occurred after the stream was opened, close the stream
                        if (loadStream) {
                            loadStream.close();
                        }
                    }
                );
            }
        }
    );
}

// save a current content of the inkManager to a file using picker
function save() {
    // NOTE: make sure that the inkManager has some strokes to save before calling inkManager.saveAsync
    if (inkManager.getStrokes().size > 0) {
        var savePicker = Windows.Storage.Pickers.FileSavePicker();
        savePicker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.picturesLibrary;
        savePicker.fileTypeChoices.insert("GIF with embedded ISF", [".gif"]);
        savePicker.defaultFileExtension = ".gif";

        // closure variable, visible to all promises in the following chain
        var saveStream = null;

        savePicker.pickSaveFileAsync().done(
            function (file) {
                if (null !== file) {
                    file.openAsync(Windows.Storage.FileAccessMode.readWrite).then(
                        function (stream) {
                            saveStream = stream;
                            return inkManager.saveAsync(saveStream); // since we return the promise, it will be executed before the following .then
                        }
                    ).done(
                        function () {
                            statusMessage.innerText = "Ink file saved!";
                            saveStream.close();
                        },
                        function (e) {
                            statusMessage.innerText = "Save " + e.toString();

                            // if the error occurred after the stream was opened, close the stream
                            if (saveStream) {
                                saveStream.close();
                            }
                        }
                    );
                }
            }
        );
    } else {
        statusMessage.innerText = "The InkManager doesn't contain any strokes to save";
    }
}

function recognize() {
    // NOTE: check that we have some ink to recognize before calling RecognizerContainer::RecognizeAsync()
    if (inkManager.getStrokes().size > 0) {
        // we select recognition target based on selected strokes
        // if there are some selected strokes we perform handwriting recognition only on those
        // otherwise we do it on all strokes
        var recognitionTarget;
        if (anySelected()) {
            recognitionTarget = Windows.UI.Input.Inking.InkRecognitionTarget.selected;
        } else {
            recognitionTarget = Windows.UI.Input.Inking.InkRecognitionTarget.all;
        }

        // the following call to recognizeAsync may fail for various reasons,
        // most notably if another recognition is in progress
        try {
            // handwriting recognition call is made here, we use current default handwriting recognizer
            inkManager.recognizeAsync(recognitionTarget).done(
                function (recognitionResults) {
                    // after we are done we can update the recognition results stored by the inkManager
                    inkManager.updateRecognitionResults(recognitionResults);

                    // update recognition text based on new recognition results
                    currentText = "";

                    // get updated recognition text
                    recognitionResults.forEach(function (recognitionResult) {
                        currentText += recognitionResult.getTextCandidates()[0] + " ";
                    });

                    // update recognition text box based on new recognition results
                    statusMessage.innerText = "Recognized text: " + currentText;
                },
                function (e) {
                    statusMessage.innerText = "Recognize error " + e.number.toString();
                }
            );
        } catch (e) {
            statusMessage.innerText = "Recognize error " + e.number.toString();
        }
    }
}

// change the default recognizer
function changeRecognizer() {
    currentRecognizerId++;
    if (currentRecognizerId >= inkManager.getRecognizers().length) {
        currentRecognizerId = 0;
    }

    inkManager.setDefaultRecognizer(inkManager.getRecognizers()[currentRecognizerId]);

    statusMessage.innerText = "Current recognizer: " + inkManager.getRecognizers()[currentRecognizerId].name;
}

function randomColor() {
    return (Math.floor(Math.random() * 256)) & 0xf0;
}

// change drawing attributes for the next stroke
// we use random color and width each time
function changeDrawingAttributes() {
    currentColor = Windows.UI.ColorHelper.fromArgb(255, randomColor(), randomColor(), randomColor());
    currentWidth = Math.random() * 8 + 1;

    var drawingAttributes = new Windows.UI.Input.Inking.InkDrawingAttributes();
    drawingAttributes.color = currentColor;
    drawingAttributes.size = { width: currentWidth, height: currentWidth };

    inkManager.setDefaultDrawingAttributes(drawingAttributes);
}

// delete selected or all strokes
function deleteStrokes() {
    if (anySelected()) {
        inkManager.deleteSelected();
    } else {
        selectAll();
        inkManager.deleteSelected();
        unselectAll();
    }

    renderAllStrokes();
}

// keyboard event handler
function keydown(evt) {
    if (evt.ctrlKey === true) {     // Ctrl+
        // prevent default manipulation of ctrl+key events, otherwise we won't be notified events like ctrl+a, ctrl+c
        evt.preventDefault();

        // since we prevent default manipulation of ctrl+key events, we receive " keydown" event twice:
        // first time is for ctrl, second time is for key and evt.keyCode is the ascii of the key
        if (evt.keyCode === 65) {           // A
            selectAll();
            renderAllStrokes();
        } else if (evt.keyCode === 67) {    // C
            copyToClipboard();
        } else if (evt.keyCode === 68) {    // D
            changeDrawingAttributes();
        } else if (evt.keyCode === 79) {    // O
            load();
        } else if (evt.keyCode === 82) {    // R
            changeRecognizer();
        } else if (evt.keyCode === 83) {    // S
            save();
        } else if (evt.keyCode === 84) {    // T
            copyTextToClipboard();
        } else if (evt.keyCode === 86) {    // V
            pasteFromClipboard();
        }
    } else if (evt.keyCode === 8) { // backspace
        statusMessage.innerText = "";
        deleteStrokes();
    } else if (evt.keyCode === 32) { // space
        recognize();
    }
}
document.addEventListener("keydown", keydown, false);

// --- rendering ------
function byteHex(num) {
    var hex = num.toString(16);
    if (hex.length === 1) {
        hex = "0" + hex;
    }
    return hex;
}

// convert Windows.UI.Color to color string
function toColorString(color) {
    return "#" + byteHex(color.r) + byteHex(color.g) + byteHex(color.b);
}

function renderStroke(stroke, color, width) {
    inkContext.strokeStyle = color;
    inkContext.lineWidth = width;
    inkContext.beginPath();

    // render all rendering segments for the current stroke
    var first = true;
    stroke.getRenderingSegments().forEach(
        function (segment) {
            if (first) {
                inkContext.moveTo(segment.position.x, segment.position.y);
                first = false;
            } else {
                inkContext.bezierCurveTo(segment.bezierControlPoint1.x, segment.bezierControlPoint1.y,
                                         segment.bezierControlPoint2.x, segment.bezierControlPoint2.y,
                                         segment.position.x, segment.position.y);
            }
        }
    );

    inkContext.stroke();
    inkContext.closePath();
}

// clear the entire canvas and render all strokes using bezier curves
function renderAllStrokes() {
    inkContext.clearRect(0, 0, inkCanvas.width, inkCanvas.height);

    inkManager.getStrokes().forEach(
        function (stroke) {
            var color = toColorString(stroke.drawingAttributes.color);
            var width = stroke.drawingAttributes.size.width;
            if (stroke.selected) {
                renderStroke(stroke, color, width + 2);
                renderStroke(stroke, toColorString(Windows.UI.Colors.white), width);
            }
            else {
                renderStroke(stroke, color, width);
            }
        }
    );
}
