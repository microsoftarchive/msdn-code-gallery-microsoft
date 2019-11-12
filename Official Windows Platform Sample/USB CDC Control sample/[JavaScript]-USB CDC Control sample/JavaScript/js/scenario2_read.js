//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="./util.js" />

(function () {
    "use strict";

    WinJS.UI.Pages.define("/html/scenario2_read.html", {

        _opRead: null,
        serialPortInfo: {
            get: function () {
                if (UsbCdcControlAccess.usbDeviceList.singleton.length > 0) {
                    return UsbCdcControlAccess.usbDeviceList.singleton[0];
                }
                return null;
            }
        },

        // Controls
        buttonReadBulkIn: { get: function () { return document.querySelector("#buttonReadBulkIn"); } },
        buttonWatchBulkIn: { get: function () { return document.querySelector("#buttonWatchBulkIn"); } },
        buttonStopWatching: { get: function () { return document.querySelector("#buttonStopWatching"); } },
        textBlockDeviceInUse: { get: function () { return document.querySelector("#textBlockDeviceInUse"); } },
        radioButtonAscii: { get: function () { return document.querySelector("#radioButtonAscii"); } },
        radioButtonBinary: { get: function () { return document.querySelector("#radioButtonBinary"); } },

        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            if (!!WinJS.Application.sessionState.scenario2_read) {
                this.loadState(WinJS.Application.sessionState);
            }

            if (this.serialPortInfo !== null) {
                this.textBlockDeviceInUse.innerText = this.serialPortInfo.name;
            } else {
                this.buttonReadBulkIn.disabled = true;
                this.buttonWatchBulkIn.disabled = true;
                this.buttonStopWatching.disabled = true;
                this.textBlockDeviceInUse.innerText = "No device selected.";
                this.textBlockDeviceInUse.style.color = "orangered";
            }

            UsbCdcControlAccess.usbDeviceList.singleton.attachWatcher(this.onDeviceAdded.bind(this), this.onDeviceRemoved.bind(this));

            this.buttonReadBulkIn.onclick = this.buttonReadBulkInClick.bind(this);
            this.buttonWatchBulkIn.onclick = this.buttonWatchBulkInClick.bind(this);
            this.buttonStopWatching.onclick = this.buttonStopWatchingClick.bind(this);
            this.radioButtonAscii.onclick = this.radioButtonDataFormatClick.bind(this);
            this.radioButtonBinary.onclick = this.radioButtonDataFormatClick.bind(this);
        },

        unload: function () {
            UsbCdcControlAccess.usbDeviceList.singleton.detachWatcher();

            // Stop read.
            this.buttonStopWatchingClick();

            this.saveState(WinJS.Application.sessionState);

            this._opRead = null;
            this._serialport = null;
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

            sessionState.scenario2_read.forEach(function (state) {
                loadTextBox(state);
                loadComboBox(state);
            });
        },

        saveState: function (sessionState) {

            function saveTextBox (textBoxId) {
                var textBox = document.querySelector("#" + textBoxId);
                sessionState.scenario2_read.push(
                    { id: textBox.id, value: textBox.value }
                ); 
            };

            sessionState.scenario2_read = [];
            saveTextBox("textBoxBytesToRead");
            saveTextBox("textBoxReadTimeout");
        },

        onDeviceAdded: function (info) {

        },

        onDeviceRemoved: function (info) {
            if (this.serialPortInfo !== null && this.serialPortInfo.deviceId === info.id) {
                (new Windows.UI.Popups.MessageDialog(info.name + " has been removed.")).showAsync();
                if (this._opRead !== null) {
                    this.buttonStopWatchingClick(); // Cancel read operation if possible.
                }
                this.buttonReadBulkIn.disabled = true;
                this.buttonWatchBulkIn.disabled = true;
                this.buttonStopWatching.disabled = true;
                this.textBlockDeviceInUse.innerText = "No device selected.";
                this.textBlockDeviceInUse.style.color = "orangered";
            }
        },

        _read: function (buffer, timeout) {
            this.serialPortInfo.port.readTimeout = timeout;
            this._opRead = this.serialPortInfo.port.read(buffer, 0, buffer.length);
            return this._opRead;
        },

        _readByteOneByOne: function () {
            var that = this;
            var array = new Uint8Array(1);

            this._read(array, -1).then(function (count) {
                that._opRead = null;

                if (count > 0) {
                    that.loggingBulkIn(array);
                }
                that._readByteOneByOne();
            }
            ,
            function (errorInfo) {
                if (errorInfo.message === "Canceled") {
                    // Cancel.
                    WinJS.log("Canceled", null, "error");
                }
            });
        },

        loggingBulkIn: function (array) {
            var textArea = document.querySelector("#textBoxReadBulkInLogger");
            var isAscii = this.radioButtonAscii.checked;

            var temp = textArea.value;
            temp += isAscii ? asciiBufferToAsciiString(array) : binaryArrayToBinaryString(array);
            textArea.value = temp;

            // Auto scroll to the end position.
            textArea.scrollTop = textArea.scrollHeight;
        },

        buttonReadBulkInClick: function () {
            var that = this;

            if (this.buttonReadBulkIn.innerText === "Read") {

                var bytesToRead = parseInt(document.querySelector("#textBoxBytesToRead").value, 10);
                if (bytesToRead > 0) {
                    // UI status.
                    this.buttonReadBulkIn.innerText = "Stop Read";
                    this.buttonWatchBulkIn.disabled = true;
                    WinJS.log("Reading", null, "status");

                    var array = new Uint8Array(bytesToRead);
                    var timeout = parseInt(document.querySelector("#textBoxReadTimeout").value);
                    this._read(array, timeout).then(function (count) {
                        array = array.subarray(0, count);
                        that.buttonReadBulkIn.innerText = "Read";
                        that.buttonWatchBulkIn.disabled = false;
                        that._opRead = null;

                        if (count < bytesToRead) {
                            // This would be timeout.
                            WinJS.log("Timeout: read " + count.toString() + " byte(s)", null, "error");
                        } else {
                            WinJS.log("Completed", null, "status");
                        }

                        if (count > 0) {
                            that.loggingBulkIn(array);
                        }
                    },
                    function (errorInfo) {
                        if (errorInfo.message === "Canceled") {
                            // Cancel.
                            WinJS.log("Canceled", null, "error");
                        }
                    });
                }
            } else {
                that.buttonStopWatchingClick();
                this.buttonReadBulkIn.innerText = "Read";
            }
        },

        buttonWatchBulkInClick: function () {
            this.buttonReadBulkIn.disabled = true;
            this.buttonWatchBulkIn.disabled = true;
            this.buttonStopWatching.disabled = false;
            WinJS.log("", null, "status");

            this._readByteOneByOne();
        },

        buttonStopWatchingClick: function () {
            if (!!this._opRead) {
                this._opRead.cancel();
                this._opRead = null;
            }
            this.buttonReadBulkIn.disabled = false;
            this.buttonWatchBulkIn.disabled = false;
            this.buttonStopWatching.disabled = true;
        },

        radioButtonDataFormatClick: function (e) {
            var sender = e.target;
            var textBoxReadBulkInLogger = document.querySelector("#textBoxReadBulkInLogger");
            var writer = new Windows.Storage.Streams.DataWriter();
            var reader = null;

            if (sender.id === this.radioButtonAscii.id) {
                var binary = textBoxReadBulkInLogger.value;
                var byteArray = binary.split(" ");

                // Binary to Unicode.
                var stringLength = 0;
                writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.utf8;
                byteArray.forEach(function (oneByte) {
                    if (oneByte.length >= 2) {
                        writer.writeByte(parseInt("0x" + oneByte));
                        stringLength++;
                    }
                });
                
                reader = Windows.Storage.Streams.DataReader.fromBuffer(writer.detachBuffer());
                textBoxReadBulkInLogger.value = reader.readString(stringLength);
            } else if (sender.id === this.radioButtonBinary.id) {
                var ascii = textBoxReadBulkInLogger.value;

                // Unicode to ASCII.
                writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                writer.writeString(ascii);
                
                var charArray = new Uint8Array(writer.unstoredBufferLength);
                reader = Windows.Storage.Streams.DataReader.fromBuffer(writer.detachBuffer());
                for (var i = 0; i < ascii.length; i++) {
                    charArray[i] = reader.readByte();
                }

                textBoxReadBulkInLogger.value = binaryArrayToBinaryString(charArray);
            }

            // Auto scroll to the end position.
            textBoxReadBulkInLogger.scrollTop = textBoxReadBulkInLogger.scrollHeight;
        }
    });
})();
