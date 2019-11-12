//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="./deviceList.js" />
/// <reference path="./serialPort.js" />

function compareTo (array, buffer) {
    if (array.Length !== buffer.Length) {
        return array.Length < buffer.Length ? -1 : 1;
    }

    var writer = new Windows.Storage.Streams.DataWriter();
    writer.writeBytes(array);
    var arrayReader = Windows.Storage.Streams.DataReader.fromBuffer(writer.detachBuffer());
    var bufferReader = Windows.Storage.Streams.DataReader.fromBuffer(buffer);

    for (var index = 0; index < array.Length; index++) {
        var byteOfArray = arrayReader.readByte();
        var byteOfBuffer = bufferReader.readByte();
        if (byteOfArray !== byteOfBuffer) {
            return byteOfArray < byteOfBuffer ? -1 : 1;
        }
    }

    return 0;
};

function asciiBufferToAsciiString (array) {
    var writer = new Windows.Storage.Streams.DataWriter();
    writer.writeBytes(array);
    var reader = Windows.Storage.Streams.DataReader.fromBuffer(writer.detachBuffer());
    reader.unicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.utf8;
    try {
        return reader.readString(array.length);
    }
    catch (exception) {
        if (exception.number === -2147023783) {
            // JavaScript runtime error: No mapping for the Unicode character exists in the target multi-byte code page.
            return "";
        }
    }
};

function binaryArrayToBinaryString (array) {
    var writer = new Windows.Storage.Streams.DataWriter();
    writer.writeBytes(array);
    return binaryBufferToBinaryString(writer.detachBuffer());
}

function binaryBufferToBinaryString (buffer) {
    var string = "";
    var reader = Windows.Storage.Streams.DataReader.fromBuffer(buffer);
    for (var i = 0; i < buffer.length; i++) {
        var byte = reader.readByte();
        var byteString = byte.toString(16);
        if (byteString.length === 1) {
            byteString = "0" + byteString;
        }
        byteString += " ";
        string += byteString;
    }
    return string;
}

(function () {
    "use strict";

    WinJS.Namespace.define("UsbCdcControlAccess", {

        usbSerialPortInfo: WinJS.Class.define(
            // Constructor
            function (port, id, deviceName) {
                this._port = port;
                this._deviceId = id;
                this._name = deviceName;
            },
            // InstanceMembers
            {
                _port: null,
                _deviceId: null,
                _name: null,
                port: {get: function () { return this._port;}}, 
                deviceId: { get: function () { return this._deviceId; } },
                name: { get: function () { return this._name; } }
            },
            // StaticMembers
            {

            }
        ),

        /// <summary>
        /// This holds UsbSerialPort instance(s) between scenarios.
        /// </summary>
        usbDeviceList: WinJS.Class.derive(
            // BaseClass
            Array,
            /// <summary>
            /// Constructor.
            /// Queries all the supported devices
            /// </summary>
            function () {
                SdkSample.Constants.supportedDevices.forEach(function (supportedDevice) {
                    var deviceListForSupportedDevice = new UsbCdcControl.deviceList(Windows.Devices.Usb.UsbDevice.getDeviceSelector(supportedDevice.vid, supportedDevice.pid));
                });
            },
            // InstanceMembers
            {
                _onAddedCallback: null,
                _onRemovedCallback: null,

                port: {get: function () { return this._port;}}, 
                deviceId: { get: function () { return this._deviceId; } },
                name: { get: function () { return this._name; } },

                attachWatcher: function (onAddedCallback, onRemovedCallback) {
                    this._onAddedCallback = onAddedCallback;
                    this._onRemovedCallback = onRemovedCallback;
                    UsbCdcControl.deviceList.Instances.forEach(function (deviceList) {
                        if (!deviceList.watcherStarted) {
                            deviceList.startWatcher(this.onDeviceAdded.bind(this), this.onDeviceRemoved.bind(this));
                        }
                    }.bind(this));
                },

                detachWatcher: function () {
                    this._onAddedCallback = null;
                    this._onRemovedCallback = null;
                    UsbCdcControl.deviceList.Instances.forEach(function (deviceList) {
                        deviceList.stopWatcher();
                    });
                },

                onDeviceAdded: function (info) {
                    if (this._onAddedCallback !== null) {
                        this._onAddedCallback(info);
                    }
                },

                onDeviceRemoved: function (info) {
                    if (this._onRemovedCallback !== null) {
                        this._onRemovedCallback(info);
                    }

                    for (var i = 0; i < this.length; i++) {
                        var portInfo = this[i];
                        if (portInfo.deviceId === info.id) {
                            // Dispose the UsbDevice.
                            portInfo.port.usbDevice.close();
                            this.splice(i, 1);
                            break;
                        }
                    }
                },

                /// <summary>
                /// Call Dispose() for each UsbDevice, and call List.Clear().
                /// </summary>
                disposeAll: function () {
                    this.forEach(function (portInfo) {
                        portInfo.port.usbDevice.close();
                    });
                    while (this.length > 0) {
                        this.pop();
                    }
                }
            },
            // StaticMembers
            {
                _singleton: null,
                singleton: {
                    get: function () {
                        if (!this._singleton) {
                            this._singleton = new UsbCdcControlAccess.usbDeviceList();
                        }
                        return this._singleton;
                    }
                }
            }
        )
    });
})();