//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="./util.js" />

(function () {
    "use strict";

    WinJS.UI.Pages.define("/html/scenario4_loopback.html", {

        _opRead: null,
        serialPortInfo1: {
            get: function () {
                if (UsbCdcControlAccess.usbDeviceList.singleton.length > 0 && this.comboBoxDevices1.selectedIndex !== -1) {
                    for (var i = 0; i < UsbCdcControlAccess.usbDeviceList.singleton.length; i++) {
                        if (UsbCdcControlAccess.usbDeviceList.singleton[i].deviceId === this.comboBoxDevices1.options[this.comboBoxDevices1.selectedIndex].attributes["id"]) {
                            return UsbCdcControlAccess.usbDeviceList.singleton[i];
                        }
                    }
                }
                return null;
            }
        },
        serialPortInfo2: {
            get: function () {
                if (UsbCdcControlAccess.usbDeviceList.singleton.length > 0 && this.comboBoxDevices2.selectedIndex !== -1) {
                    for (var i = 0; i < UsbCdcControlAccess.usbDeviceList.singleton.length; i++) {
                        if (UsbCdcControlAccess.usbDeviceList.singleton[i].deviceId === this.comboBoxDevices2.options[this.comboBoxDevices2.selectedIndex].attributes["id"]) {
                            return UsbCdcControlAccess.usbDeviceList.singleton[i];
                        }
                    }
                }
                return null;
            }
        },
        _previousSelectedDeviceId1: null,
        _previousSelectedDeviceId2: null,

        // Controls
        comboBoxDevices1: { get: function () { return document.querySelector("#comboBoxDevices1"); } },
        comboBoxDevices2: { get: function () { return document.querySelector("#comboBoxDevices2"); } },
        buttonInitialize: { get: function () { return document.querySelector("#buttonInitialize"); } },
        buttonLoopBackTest: { get: function () { return document.querySelector("#buttonLoopBackTest"); } },
        buttonStopLoopBack: { get: function () { return document.querySelector("#buttonStopLoopBack"); } },

        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            if (!!WinJS.Application.sessionState.scenario4_loopback) {
                this.loadState(WinJS.Application.sessionState);
            }

            // Dispose all devices which are used in Scenario1 to 3.
            UsbCdcControlAccess.usbDeviceList.singleton.disposeAll();

            this.comboBoxDevices1.onchange = this.comboBoxDevicesChange.bind(this);
            this.comboBoxDevices2.onchange = this.comboBoxDevicesChange.bind(this);
            this.buttonInitialize.onclick = this.buttonInitializeClick.bind(this);
            this.buttonLoopBackTest.onclick = this.buttonLoopBackTestClick.bind(this);
            this.buttonStopLoopBack.onclick = this.buttonStopLoopBackClick.bind(this);
            this.buttonLoopBackTest.disabled = true;
            this.buttonStopLoopBack.disabled = true;

            UsbCdcControlAccess.usbDeviceList.singleton.attachWatcher(this.onDeviceAdded.bind(this), this.onDeviceRemoved.bind(this));
        },

        unload: function () {
            UsbCdcControlAccess.usbDeviceList.singleton.detachWatcher();

            // Cancel, if a read promise is processing.
            this.buttonStopLoopBackClick();

            // Dispose all devices (prior to going to Scenario1 to 3).
            UsbCdcControlAccess.usbDeviceList.singleton.disposeAll();

            this.saveState(WinJS.Application.sessionState);

            this._opRead = null;
            this._previousSelectedDeviceId1 = null;
            this._previousSelectedDeviceId2 = null;
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

            sessionState.scenario4_loopback.forEach(function (state) {
                if (state.id === "comboBoxDevices1") {
                    that._previousSelectedDeviceId1 = state.value;
                } else if (state.id === "comboBoxDevices2") {
                    that._previousSelectedDeviceId2 = state.value;
                } else {
                    loadTextBox(state);
                    loadComboBox(state);
                }
            });
        },

        saveState: function (sessionState) {

            function saveTextBox (textBoxId) {
                var textBox = document.querySelector("#" + textBoxId);
                sessionState.scenario4_loopback.push(
                    { id: textBox.id, value: textBox.value }
                ); 
            };

            function saveComboBox (comboBoxId) {
                var comboBox = document.querySelector("#" + comboBoxId);
                sessionState.scenario4_loopback.push(
                    { id: comboBox.id, value: comboBox.options[comboBox.selectedIndex].value }
                ); 
            };

            sessionState.scenario4_loopback = [];
            if (this.comboBoxDevices1.selectedIndex >= 0) {
                sessionState.scenario4_loopback.push(
                    { id: this.comboBoxDevices1.id, value: this.comboBoxDevices1.options[this.comboBoxDevices1.selectedIndex].attributes["id"] }
                );
            }
            if (this.comboBoxDevices2.selectedIndex >= 0) {
                sessionState.scenario4_loopback.push(
                    { id: this.comboBoxDevices2.id, value: this.comboBoxDevices2.options[this.comboBoxDevices2.selectedIndex].attributes["id"] }
                );
            }
            saveTextBox("textBoxDTERate");
            saveComboBox("comboBoxCharFormat");
            saveComboBox("comboBoxParityType");
            saveComboBox("comboBoxDataBits");
            saveComboBox("comboBoxRTS");
            saveComboBox("comboBoxDTR");
        },

        onDeviceAdded: function (info) {
            var that = this;
            var comboBoxes = [this.comboBoxDevices1, this.comboBoxDevices2];
            comboBoxes.forEach(function (/*@type(HTMLSelectElement)*/comboBoxDevices) {
                var previouslySelectedIndex = comboBoxDevices.selectedIndex;
                var opt = document.createElement("option");
                opt.text = info.name;
                opt.attributes["id"] = info.id;
                comboBoxDevices.add(opt);
                if (previouslySelectedIndex === -1) {
                    if (comboBoxDevices.id === "comboBoxDevices1") {
                        if (that._previousSelectedDeviceId1 === info.id || that._previousSelectedDeviceId1 === null) {
                            comboBoxDevices.selectedIndex = comboBoxDevices.options.length - 1;
                        }
                    } else if (comboBoxDevices.id === "comboBoxDevices2") {
                        if (that._previousSelectedDeviceId2 === info.id || that._previousSelectedDeviceId2 === null) {
                            comboBoxDevices.selectedIndex = comboBoxDevices.options.length - 1;
                        }
                    }
                }
                if (previouslySelectedIndex !== comboBoxDevices.selectedIndex) {
                    comboBoxDevices.onchange(comboBoxDevices);
                }
            });
        },

        onDeviceRemoved: function (info) {
            function selectedId(comboBoxDevices) {
                if (comboBoxDevices.selectedIndex === -1) {
                    return "";
                }
                return comboBoxDevices.options[comboBoxDevices.selectedIndex].attributes["id"];
            };

            var that = this;
            var showMessageDialog = false;
            var comboBoxes = [this.comboBoxDevices1, this.comboBoxDevices2];
            comboBoxes.forEach(function (/*@type(HTMLSelectElement)*/comboBoxDevices) {
                var options = comboBoxDevices.options;
                for (var i = 0; i < options.length; i++) {
                    if (options[i].attributes["id"] === info.id) {
                        var previouslySelectedId = selectedId(comboBoxDevices);
                        if (that.buttonInitialize.disabled && comboBoxDevices.selectedIndex === i) {
                            showMessageDialog = true;
                            if (that._opRead !== null) {
                                that.buttonStopLoopBackClick(); // Cancel read operation if possible.
                            } 
                        }
                        comboBoxDevices.remove(i);
                        if (comboBoxDevices.selectedIndex === -1 && comboBoxDevices.options.length > 0) {
                            comboBoxDevices.selectedIndex = 0;
                        }
                        if (previouslySelectedId !== selectedId(comboBoxDevices)) {
                            comboBoxDevices.onchange(comboBoxDevices);
                        }
                        return;
                    }
                }
            });

            if (showMessageDialog) {
                (new Windows.UI.Popups.MessageDialog(info.name + " has been removed.")).showAsync();
            }
        },

        read: function (port, buffer, timeout) {
            port.readTimeout = timeout;
            this._opRead = port.read(buffer, 0, buffer.length);
            return this._opRead;
        },

        comboBoxDevicesChange: function (e) {
            // Restore UI state.
            this.buttonLoopBackTest.disabled = true;
            this.buttonInitialize.disabled = false;

            this.buttonStopLoopBackClick(); // Cancel read op if possible.

            // Dispose all devices.
            UsbCdcControlAccess.usbDeviceList.singleton.disposeAll();

            // Cancel, if a read promise is processing.
            if (!!this._opRead) {
                this._opRead.cancel();
                this._opRead = null;
            }

            var comboBoxDevices = !!e.target ? e.target : e;
            if (comboBoxDevices.selectedIndex === -1) {
                return;
            }

            var comboBoxes = [ this.comboBoxDevices1, this.comboBoxDevices2 ];
            for (var i = 0; i < comboBoxes.length; i++) {
                if (comboBoxes[i].selectedIndex === -1) {
                    return;
                }
            }

            var item1 = this.comboBoxDevices1.options[this.comboBoxDevices1.selectedIndex];
            var item2 = this.comboBoxDevices2.options[this.comboBoxDevices2.selectedIndex];

            if (item1.attributes["id"] === item2.attributes["id"]) {
                // Both comboBoxes are selecting a same device.
                var selectedIndex = this.comboBoxDevices2.selectedIndex;
                for (var index = 0; index < this.comboBoxDevices2.options.length; index++) {
                    if (index !== selectedIndex) {
                        this.comboBoxDevices2.selectedIndex = index;
                        break;
                    }
                }
                if (selectedIndex === this.comboBoxDevices2.selectedIndex) {
                    this.comboBoxDevices2.selectedIndex = -1;
                    return;
                }
            }
        },

        buttonInitializeClick: function () {
            var that = this;
            if (this.comboBoxDevices1.selectedIndex === -1 || this.comboBoxDevices2.selectedIndex === -1) {
                return;
            }

            var textBoxDTERate = document.querySelector("#textBoxDTERate");
            var DTERate = parseInt(textBoxDTERate.value, 10);

            var comboBoxCharFormat = document.querySelector("#comboBoxCharFormat");
            var charFormat = parseInt(comboBoxCharFormat.options[comboBoxCharFormat.selectedIndex].value, 10);

            var comboBoxParityType = document.querySelector("#comboBoxParityType");
            var parityType = parseInt(comboBoxParityType.options[comboBoxParityType.selectedIndex].value, 10);

            var comboBoxDataBits = document.querySelector("#comboBoxDataBits");
            var dataBits = parseInt(comboBoxDataBits.options[comboBoxDataBits.selectedIndex].value, 10);

            var comboBoxRTS = document.querySelector("#comboBoxRTS");
            var RTS = parseInt(comboBoxRTS.options[comboBoxRTS.selectedIndex].value, 10);

            var comboBoxDTR = document.querySelector("#comboBoxDTR");
            var DTR = parseInt(comboBoxDTR.options[comboBoxDTR.selectedIndex].value, 10);

            var createSerialPortTasks = [];

            var comboBoxes = [ this.comboBoxDevices1, this.comboBoxDevices2 ];
            comboBoxes.forEach(function (/*@type(HTMLSelectElement)*/comboBox) {
                var id = comboBox.options[comboBox.selectedIndex].attributes["id"];
                var deviceName = comboBox.options[comboBox.selectedIndex].text;
                createSerialPortTasks.push(Windows.Devices.Usb.UsbDevice.fromIdAsync(id).then(function (usbDevice) {
                    return UsbCdcControlAccess.UsbSerialPort.create(usbDevice).then(function (port) {
                        if (!port) {
                            if (!!usbDevice) {
                                usbDevice.close();
                            }
                            // Return a dummy promise.
                            return new WinJS.Promise.timeout(0);
                        }

                        UsbCdcControlAccess.usbDeviceList.singleton.push(new UsbCdcControlAccess.usbSerialPortInfo(port, id, deviceName));

                        return port.open(DTERate, parityType, dataBits, charFormat).then(function () {
                            return port.dtrEnable = DTR !== 0;
                        }
                        ).then(function () {
                            return port.rtsEnable = RTS !== 0;
                        });
                    });
                }));
            });

            WinJS.Promise.join(createSerialPortTasks).then(function () {
                if (that.serialPortInfo1 !== null && that.serialPortInfo2 !== null) {
                    that.buttonLoopBackTest.disabled = false;
                    that.buttonInitialize.disabled = true;
                    WinJS.log("Initialized.", null, "status");
                } else {
                    var deviceNumber;

                    if (!that.serialPortInfo1) {
                        if (!that.serialPortInfo2) {
                            deviceNumber = "Both devices";
                        } else {
                            deviceNumber = "Device 1";
                        }
                    } else {
                        deviceNumber = "Device 2";
                    }

                    WinJS.log(deviceNumber + " failed to be initialized.", null, "error");

                    // Close all devices because one of the devices failed to be opened
                    UsbCdcControlAccess.usbDeviceList.singleton.disposeAll();
                }
            });
        },

        buttonLoopBackTestClick: function () {
            var that = this;

            // Unicode to ASCII.
            var textToSend = document.querySelector("#textBoxForLoopback").value;                    
            var writer = new Windows.Storage.Streams.DataWriter();
            writer.unicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.utf8;
            writer.writeString(textToSend);
            writer.writeByte(0x00); // NUL
            var buffer = writer.detachBuffer();

            var readArray = new Uint8Array(buffer.length);

            var statusMessage = "";
            this.buttonLoopBackTest.disabled = true;
            this.buttonStopLoopBack.disabled = false;
            WinJS.log("", null, "status");

            // SerialPort1 to SerialPort2
            var readPromise = this.read(this.serialPortInfo2.port, readArray, SdkSample.Constants.infiniteTimeout);
            var writePromise = this.serialPortInfo1.port.write(buffer, 0, buffer.length); 
            WinJS.Promise.join([readPromise, writePromise]).then(function (results) {
                readArray.length = results[0];
                var isSame = compareTo(readArray, buffer) === 0;
                if (isSame) {
                    statusMessage += "CDC device 2 received \"" + textToSend + "\" from CDC device 1. ";
                } else {
                    statusMessage += "Loopback failed: CDC device 1 to CDC device 2. ";
                }

                // SerialPort2 to SerialPort1
                readArray.length = buffer.length;
                readPromise = that.read(that.serialPortInfo1.port, readArray, SdkSample.Constants.infiniteTimeout);
                writePromise = that.serialPortInfo2.port.write(buffer, 0, buffer.length);
                return WinJS.Promise.join([readPromise, writePromise]);
            },
            function (errorInfo) {
                if (errorInfo.message === "Canceled") {
                    // Canceled.
                    WinJS.log("Canceled", null, "error");
                    that.buttonLoopBackTest.disabled = false;
                    that.buttonStopLoopBack.disabled = true;
                }
                writePromise.cancel();
                return null;
            }
            ).done(function (results) {
                if (!!results) {
                    readArray.length = results[0];
                    var isSame = compareTo(readArray, buffer) === 0;
                    if (isSame) {
                        statusMessage += "CDC device 1 received \"" + textToSend + "\" from CDC device 2. ";
                    } else {
                        statusMessage += "Loopback failed: CDC device 2 to CDC device 1. ";
                    }

                    that.buttonLoopBackTest.disabled = false;
                    that.buttonStopLoopBack.disabled = true;
                    WinJS.log(statusMessage, null, "status");
                }
            },
            function (errorInfo) {
                if (errorInfo.message === "Canceled") {
                    // Canceled.
                    WinJS.log("Canceled", null, "error");
                    that.buttonLoopBackTest.disabled = false;
                    that.buttonStopLoopBack.disabled = true;
                }
                writePromise.cancel();
            }
            );
        },

        buttonStopLoopBackClick: function () {
            if (!!this._opRead) {
                this.buttonStopLoopBack.disabled = true;
                this._opRead.cancel();
                this._opRead = null;
            }
        }
    });
})();
