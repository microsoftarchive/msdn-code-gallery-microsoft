//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="./util.js" />

(function () {
    "use strict";

    WinJS.UI.Pages.define("/html/scenario3_write.html", {

        serialPortInfo: {
            get: function () {
                if (UsbCdcControlAccess.usbDeviceList.singleton.length > 0) {
                    return UsbCdcControlAccess.usbDeviceList.singleton[0];
                }
                return null;
            }
        },
        rawBinaryDataProperty: [],

        // Controls
        buttonWriteBulkOut: { get: function () { return document.querySelector("#buttonWriteBulkOut"); } },
        buttonSendBreak: { get: function () { return document.querySelector("#buttonSendBreak"); } },
        buttonWriteBinary1: { get: function () { return document.querySelector("#buttonWriteBinary1"); } },
        buttonWriteBinary2: { get: function () { return document.querySelector("#buttonWriteBinary2"); } },
        buttonLoadBinaryData1: { get: function () { return document.querySelector("#buttonLoadBinaryData1"); } },
        buttonLoadBinaryData2: { get: function () { return document.querySelector("#buttonLoadBinaryData2"); } },
        textBlockDeviceInUse: { get: function () { return document.querySelector("#textBlockDeviceInUse"); } },

        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            if (!!WinJS.Application.sessionState.scenario3_write) {
                this.loadState(WinJS.Application.sessionState);
            }

            this.rawBinaryDataProperty = [];

            if (this.serialPortInfo !== null) {
                this.textBlockDeviceInUse.innerText = this.serialPortInfo.name;
            } else {
                this.buttonWriteBulkOut.disabled = true;
                this.buttonSendBreak.disabled = true;
                this.buttonWriteBinary1.disabled = true;
                this.buttonWriteBinary2.disabled = true;
                this.textBlockDeviceInUse.innerText = "No device selected.";
                this.textBlockDeviceInUse.style.color = "orangered";
            }

            UsbCdcControlAccess.usbDeviceList.singleton.attachWatcher(this.onDeviceAdded.bind(this), this.onDeviceRemoved.bind(this));

            // Set event handlers.
            this.buttonWriteBulkOut.onclick = this.buttonWriteBulkOutClick.bind(this);
            this.buttonSendBreak.onclick = this.buttonSendBreakClick.bind(this);
            this.buttonWriteBinary1.onclick = this.buttonWriteBinaryClick.bind(this);
            this.buttonWriteBinary2.onclick = this.buttonWriteBinaryClick.bind(this);
            this.buttonLoadBinaryData1.onclick = this.buttonLoadBinaryDataClick.bind(this);
            this.buttonLoadBinaryData2.onclick = this.buttonLoadBinaryDataClick.bind(this);
        },

        unload: function () {
            UsbCdcControlAccess.usbDeviceList.singleton.detachWatcher();

            this.saveState(WinJS.Application.sessionState);
        },

        loadState: function (sessionState) {
            var that = this;

            function loadTextBox(state) {
                var textBox = document.querySelector("#" + state.id);
                if (textBox.tagName === "INPUT" && textBox.type === "text") {
                    textBox.value = state.value;
                }
            };

            function loadComboBox(state) {
                var comboBox = document.querySelector("#" + state.id);
                if (comboBox.tagName === "SELECT") {
                    for (var i = 0; i < comboBox.options.length; i++) {
                        if (comboBox.options[i].value === state.value) {
                            comboBox.selectedIndex = i;
                            break;
                        }
                    }
                }
            };

            sessionState.scenario3_write.forEach(function (state) {
                loadTextBox(state);
                loadComboBox(state);
            });
        },

        saveState: function (sessionState) {

        },

        onDeviceAdded: function (info) {

        },

        onDeviceRemoved: function (info) {
            if (this.serialPortInfo !== null) {
                if (this.serialPortInfo.deviceId === info.id) {
                    (new Windows.UI.Popups.MessageDialog(info.name + " has been removed.")).showAsync();
                    this.buttonWriteBulkOut.disabled = true;
                    this.buttonSendBreak.disabled = true;
                    this.buttonWriteBinary1.disabled = true;
                    this.buttonWriteBinary2.disabled = true;
                    this.textBlockDeviceInUse.innerText = "No device selected.";
                    this.textBlockDeviceInUse.style.color = "orangered";
                }
            }
        },

        buttonWriteBulkOutClick: function () {
            if (this.serialPortInfo !== null) {
                var dataToWrite = document.querySelector("#textBoxDataToWrite").value;

                // Unicode to ASCII.
                var writer = new Windows.Storage.Streams.DataWriter();
                writer.unicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.utf8;
                writer.writeString(dataToWrite);
                if (document.querySelector("#checkBoxSendNullTerminateCharToBulkOut").checked) {
                    writer.writeByte(0x00); // NUL
                }
                var buffer = writer.detachBuffer();

                this.serialPortInfo.port.write(buffer, 0, buffer.length).then(function () {
                    var textArea = document.querySelector("#textBoxWriteLog");
                    var number = buffer.length;
                    var temp = textArea.value;
                    temp += "Write completed: \"" + dataToWrite.toString() + "\" (" + number.toString() + " bytes)\n";
                    textArea.value = temp;
                    // Auto scroll to the end position.
                    textArea.scrollTop = textArea.scrollHeight;

                    document.querySelector("#textBoxDataToWrite").value = "";
                });
            }
        },

        /// <summary>
        /// Generates a RS-232C break signal during a time length.
        /// </summary>
        buttonSendBreakClick: function () {
            if (this.serialPortInfo !== null) {
                var textBoxDurationOfBreak = document.querySelector("#textBoxDurationOfBreak");
                this.serialPortInfo.port.setControlRequest(SdkSample.Constants.requestCode.sendBreak, parseInt(textBoxDurationOfBreak.value), null);
            }   
        },

        buttonLoadBinaryDataClick: function (e) {
            var that = this;
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.documentsLibrary;
            picker.fileTypeFilter.append(".bin");
            picker.pickSingleFileAsync().then(function (file) {
                if (!file) {
                    return;
                }
                return file.openAsync(Windows.Storage.FileAccessMode.read);
            }
            ).then(function (stream) {
                if (!stream) {
                    return;
                }
                var reader = new Windows.Storage.Streams.DataReader(stream.getInputStreamAt(0));
                reader.loadAsync(stream.size).then(function (count) {
                    var array = new Uint8Array(count);
                    reader.readBytes(array);
                    var targetIndex = e.target.id.split("buttonLoadBinaryData")[1];
                    var textBox = document.querySelector("#textBoxBinaryData" + targetIndex);
                    textBox.value = binaryArrayToBinaryString(array);
                    that.rawBinaryDataProperty[targetIndex] = array;
                });
            });
        },

        buttonWriteBinaryClick: function (e) {
            var sender = e.target;
            var index = sender.id.split("buttonWriteBinary")[1];
            var array = this.rawBinaryDataProperty[index];
            if (!!array) {
                var writer = new Windows.Storage.Streams.DataWriter();
                writer.writeBytes(array);
                var buffer = writer.detachBuffer();
                this.serialPortInfo.port.write(buffer, 0, buffer.length).then(function () {
                    var temp = document.querySelector("#textBoxWriteLog").value;
                    temp += "Write completed: \"" + binaryArrayToBinaryString(array) + "\" (" + array.length.toString() + " bytes)\n";
                    document.querySelector("#textBoxWriteLog").value = temp;
                });
            }
        }
    });
})();
