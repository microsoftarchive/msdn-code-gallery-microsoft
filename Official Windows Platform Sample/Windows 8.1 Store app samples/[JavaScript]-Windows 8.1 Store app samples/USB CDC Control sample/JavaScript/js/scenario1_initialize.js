//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="./util.js" />

(function () {
    "use strict";

    WinJS.UI.Pages.define("/html/scenario1_initialize.html", {

        _previousSelectedDeviceId: null,
        serialPortInfo: {
            get: function () {
                if (UsbCdcControlAccess.usbDeviceList.singleton.length > 0) {
                    return UsbCdcControlAccess.usbDeviceList.singleton[0];
                }
                return null;
            }
        },

        // Controls
        comboBoxDevices: { get: function () { return document.querySelector("#comboBoxDevices"); } },
        buttonDeviceSelect: { get: function () { return document.querySelector("#buttonDeviceSelect"); } },
        buttonDeviceDeselect: { get: function () { return document.querySelector("#buttonDeviceDeselect"); } },
        buttonInitialize: { get: function () { return document.querySelector("#buttonInitialize"); } },

        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            var that = this;
            if (!!WinJS.Application.sessionState.scenario1_initialize) {
                this.loadState(WinJS.Application.sessionState);
            }

            if (UsbCdcControlAccess.usbDeviceList.singleton.length > 0) {
                this.comboBoxDevices.disabled = true;
                this.buttonDeviceSelect.disabled = true;
                this.buttonDeviceDeselect.disabled = false;
            } else {
                this.buttonInitialize.disabled = true;
                this.buttonDeviceDeselect.disabled = true;
            }
            UsbCdcControlAccess.usbDeviceList.singleton.attachWatcher(this.onDeviceAdded.bind(this), this.onDeviceRemoved.bind(this));

            this.buttonDeviceSelect.onclick = this.buttonDeviceSelectClick.bind(this);
            this.buttonDeviceDeselect.onclick = this.buttonDeviceDeselectClick.bind(this);
            this.buttonInitialize.onclick = this.buttonInitializeClick.bind(this);
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

            sessionState.scenario1_initialize.forEach(function (state) {
                if (state.id === that.comboBoxDevices.id) {
                    that._previousSelectedDeviceId = state.value;
                } else {
                    loadTextBox(state);
                    loadComboBox(state);
                }
            });
        },

        saveState: function (sessionState) {

            function saveTextBox (textBoxId) {
                var textBox = document.querySelector("#" + textBoxId);
                sessionState.scenario1_initialize.push(
                    { id: textBox.id, value: textBox.value }
                ); 
            };

            function saveComboBox (comboBoxId) {
                var comboBox = document.querySelector("#" + comboBoxId);
                sessionState.scenario1_initialize.push(
                    { id: comboBox.id, value: comboBox.options[comboBox.selectedIndex].value }
                ); 
            };

            sessionState.scenario1_initialize = [];
            if (this.comboBoxDevices.selectedIndex >= 0) {
                sessionState.scenario1_initialize.push(
                    { id: this.comboBoxDevices.id, value: this.comboBoxDevices.options[this.comboBoxDevices.selectedIndex].attributes["id"] }
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
            var selectedIndex = this.comboBoxDevices.selectedIndex;
            var opt = document.createElement("option");
            opt.text = info.name;
            opt.attributes["id"] = info.id;
            this.comboBoxDevices.add(opt);
            if (this.serialPortInfo !== null && this.serialPortInfo.deviceId === info.id) {
                this.comboBoxDevices.selectedIndex = this.comboBoxDevices.options.length - 1;
            } else if (selectedIndex === -1) {
                if (this._previousSelectedDeviceId === info.id || this._previousSelectedDeviceId === null) {
                    this.comboBoxDevices.selectedIndex = this.comboBoxDevices.options.length - 1;
                }
            }
        },

        onDeviceRemoved: function (info) {
            var options = this.comboBoxDevices.options;
            for (var i = 0; i < options.length; i++) {
                if (options[i].attributes["id"] === info.id) {
                    var disabled = this.comboBoxDevices.disabled;
                    this.comboBoxDevices.remove(i);
                    if (this.serialPortInfo !== null && this.serialPortInfo.deviceId === info.id) {
                        (new Windows.UI.Popups.MessageDialog(info.name + " has been removed.")).showAsync();
                        this.buttonDeviceDeselectClick();
                        if (this.comboBoxDevices.options.length > 0) {
                            this.comboBoxDevices.selectedIndex = 0;
                        }
                    } else {
                        if (this.comboBoxDevices.selectedIndex === -1) {
                            this.comboBoxDevices.selectedIndex = 0;
                        }
                        this.comboBoxDevices.disabled = disabled;
                    }
                    return;
                }
            }
        },

        buttonDeviceSelectClick: function () {
            var that = this;
            // No device selected.
            if (this.comboBoxDevices.selectedIndex === -1) {
                return;
            }

            var deviceId = this.comboBoxDevices.options[this.comboBoxDevices.selectedIndex].attributes["id"];
            var deviceName = this.comboBoxDevices.options[this.comboBoxDevices.selectedIndex].text;

            Windows.Devices.Usb.UsbDevice.fromIdAsync(deviceId).done(function (usbDevice) {
                return UsbCdcControlAccess.UsbSerialPort.create(usbDevice).then(function (serialPort) {
                    if (serialPort === null) {
                        if (!!usbDevice) {
                            usbDevice.close();
                        }
                        WinJS.log(deviceName + " is not compatible with CDC ACM.", null, "error");
                        return;
                    }

                    UsbCdcControlAccess.usbDeviceList.singleton.push(new UsbCdcControlAccess.usbSerialPortInfo(serialPort, deviceId, deviceName));

                    that.comboBoxDevices.disabled = true;
                    that.buttonDeviceSelect.disabled = true;
                    that.buttonInitialize.disabled = false;
                    that.buttonDeviceDeselect.disabled = false;
                    WinJS.log("", null, "status");
                });
            });
        },

        buttonDeviceDeselectClick: function () {
            var that = this;
            var previousSelectedDeviceId = null;
            if (this.serialPortInfo !== null) {
                previousSelectedDeviceId = this.serialPortInfo.deviceId;
            }
            this.comboBoxDevices.disabled = false;
            UsbCdcControlAccess.usbDeviceList.singleton.disposeAll();
            UsbCdcControl.deviceList.Instances.forEach(function (deviceList) {
                deviceList.Devices.forEach(function (info) {
                    var foundIndex = -1;
                    for (var i = 0; i < that.comboBoxDevices.options.length; i++) {
                        if (that.comboBoxDevices.options[i].attributes["id"] === info.id) {
                            foundIndex = i;
                            break;
                        }
                    }
                    if (foundIndex === -1) {
                        this.onDeviceAdded(info.id, info.name);
                        foundIndex = 0;
                    }

                    if (previousSelectedDeviceId === null || previousSelectedDeviceId === info.id) {
                        that.comboBoxDevices.selectedIndex = foundIndex;
                    }
                });
            });
            this.buttonDeviceSelect.disabled = false;
            this.buttonInitialize.disabled = true;
            this.buttonDeviceDeselect.disabled = true;
        },

        buttonInitializeClick: function () {
            if (this.serialPortInfo === null) {
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

            var serialPort = this.serialPortInfo.port;

            serialPort.open(DTERate, parityType, dataBits, charFormat).then(function () {
                return serialPort.dtrEnable = DTR !== 0;
            }
            ).then(function () {
                serialPort.rtsEnable = RTS !== 0;
            }
            ).done(function () {
                WinJS.log("Initialized.", null, "status");
            });
        }
    });
})();
