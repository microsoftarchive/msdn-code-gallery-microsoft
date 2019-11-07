//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="./constants.js" />
/// <dictionary target='member'>dtr</dictionary>
/// <dictionary target='member'>rts</dictionary>
/// <dictionary target='parameter'>dte</dictionary>

(function () {
    "use strict";

    var constants = SdkSample.Constants;

    WinJS.Namespace.define("UsbCdcControlAccess", {
        /// <summary> 
        /// Serial port class, wraps Windows.Devices.Usb.UsbDevice.
        /// </summary>
        UsbSerialPort: WinJS.Class.define(
            /// <summary>
            /// Constructor, called from create method.
            /// </summary>
            /// <param name="device">The UsbDevice, passed to create method.</param>
            /// <remarks>
            /// Do not call this constructor directly. Use create method instead.
            /// </remarks>
            function UsbSerialPort(device) {
                this._device = device;
            }, {
                _device: null,          // Windows.Devices.Usb.UsbDevice
                _baudRate: 9600,
                _parity: constants.parity.none,
                _dataBits: 8,
                _stopBits: 1.0,
                _dtrEnable: false,
                _rtsEnable: false,
                _readTimeout: constants.infiniteTimeout, // Milliseconds
                _cdcControl: null,      // Windows.Devices.Usb.UsbInterface
                _cdcData: null,         // Windows.Devices.Usb.UsbInterface

                /// <summary>
                /// Gets or sets the number of milliseconds before a time-out occurs when a read operation does not finish.
                /// </summary>
                /// <remarks>
                /// -1 is the default value, means InfiniteTimeout.
                /// </remarks>
                readTimeout: {
                    get: function () { return this._readTimeout; },
                    set: function (timeout) { this._readTimeout = timeout; }
                },

                /// <summary>
                /// Gets the UsbDevice, which has been passed to create/constructor method.
                /// </summary>
                usbDevice: {
                    get: function () { return this._device; }
                },

                /// <summary>
                /// This is an internal method called from create method.
                /// </summary>
                /// <remarks>
                /// The result of Promise contains success(true) or failure(false).
                /// </remarks>
                tryInitialize: function () {
                    var that = this;
                    return new WinJS.Promise(function (complete, error, progress) {
                        var device = that._device;
                        var cdcControl = null;
                        var cdcData = null;
                        var promises = [];

                        for (var i = 0; i < device.configuration.usbInterfaces.size; i++) {
                            var usbInterface = device.configuration.usbInterfaces[i];
                            if (!usbInterface) {
                                continue;
                            }

                            for (var j = 0; j < usbInterface.interfaceSettings.size; j++) {
                                var setting = usbInterface.interfaceSettings[j];
                                var index = setting.interfaceDescriptor.interfaceNumber;
                                var interfaceClass = setting.interfaceDescriptor.classCode;

                                if (interfaceClass === constants.interfaceClass.cdcControl) {
                                    cdcControl = usbInterface;
                                } else if (interfaceClass === constants.interfaceClass.cdcData) {
                                    cdcData = usbInterface;
                                } else if (interfaceClass === constants.interfaceClass.vendorSpecific) {
                                    if (cdcControl === null) {
                                        promises.push(that.getCdcControl(index, usbInterface));
                                    }

                                    if (cdcData === null) {
                                        if (usbInterface.bulkInPipes.size > 0 && usbInterface.bulkOutPipes.size > 0) {
                                            // This could be a CdcData.
                                            cdcData = usbInterface;
                                        }
                                    }
                                }
                            }
                        }

                        WinJS.Promise.join(promises).done(function (/*@type(Array)*/cdcControls) {
                            that._cdcControl = cdcControl;
                            cdcControls.forEach(function (got) {
                                if (got !== null) {
                                    that._cdcControl = got;
                                }
                            });
                            that._cdcData = cdcData;

                            if (!!that._cdcControl && !!that._cdcData) {
                                // Flush the pipes.
                                for (var bulkIn = 0; bulkIn < that._cdcData.bulkInPipes.size; bulkIn++) {
                                    that._cdcData.bulkInPipes[bulkIn].flushBuffer();
                                }
                                complete(true);
                            } else {
                                complete(false); 
                            }
                        },
                        function (e) {
                            // This device will not work as a serial port.
                            error(new WinJS.ErrorFromName(e[0].name, e[0].message));
                        });
                    });
                },

                getCdcControl: function (index, cdcInterface) {
                    var that = this;
                    return new WinJS.Promise(function (complete, error, progress) {
                        that.setLineCoding(index, constants.dteRateTestValue, 0, 0, constants.dataBitsTestValue).then(function (written) {
                            // Test GetLineCoding
                            return that.getLineCoding(index).then(function (gotBuffer) {
                                if (gotBuffer !== null && gotBuffer.length === constants.expectedResultGetLineCoding) {
                                    var reader = Windows.Storage.Streams.DataReader.fromBuffer(gotBuffer);
                                    reader.byteOrder = Windows.Storage.Streams.ByteOrder.littleEndian;
                                    var dwBPS = reader.readUInt32();
                                    if (dwBPS === constants.dteRateTestValue) {
                                        // This could be a CdcControl.
                                        complete(cdcInterface);
                                        return;
                                    }
                                }
                                complete(null);
                            },
                            function (/*@type(Error)*/e) {
                                if (e.number === -2147024865) {
                                    complete(null);
                                } else {
                                    error(e);
                                }
                            });
                        },
                        function (/*@type(Error)*/e) {
                            if (e.number === -2147024865) {
                                complete(null);
                            } else {
                                error(e);
                            }
                        });
                    });
                },

                /// <summary>
                /// Opens the serial port using the specified baud rate, parity bit, data bits, and stop bit.
                /// </summary>
                /// <param name="baudRate">The baud rate.</param>
                /// <param name="parity">One of the Parity values.</param>
                /// <param name="dataBits">The data bits value.</param>
                /// <param name="stopBits">One of the StopBits values.</param>
                open: function (baudRate, parity, dataBits, stopBits) {
                    var that = this;
                    this._baudRate  = baudRate;
                    this._parity    = parity;
                    this._dataBits  = dataBits;
                    this._stopBits  = stopBits;

                    return new WinJS.Promise(function (complete, error, progress) {
                        // Do SetLineCoding
                        that.setLineCoding(that._cdcControl.interfaceNumber, that._baudRate, that._stopBits, that._parity, that._dataBits).then(function (len) {
                            if (len !== constants.expectedResultSetLineCoding) {
                                throw new WinJS.ErrorFromName("NoInterface", "The UsbDevice is not compatible with SetLineCoding.");
                            }

                            // Do SetControlLineState
                            return that.setControlLineState(that._cdcControl.interfaceNumber);
                        }
                        ).done(function (len) {
                            complete();
                        });
                    });
                },

                /// <summary>
                /// Sends a raw CDC request.
                /// </summary>
                /// <param name="request">CDC request code.</param>
                /// <param name="value">value, corresponded with the request code.</param>
                /// <param name="buffer">data, corresponded with the request code.</param>
                /// <returns>
                /// The result of Promise contains a length of bytes actually sent.
                /// </returns>
                setControlRequest: function (request, value, buffer) {
                    var requestType = new Windows.Devices.Usb.UsbControlRequestType();
                    requestType.asByte = constants.requestType.set;
                    return this.usbControlRequestForSet(this._cdcControl.InterfaceNumber, requestType, request, value, buffer);
                },

                /// <summary>
                /// Reads a number of bytes from the SerialPort input buffer and writes those bytes into a byte array at the specified offset.
                /// </summary>
                /// <param name="array">The byte array to write the input to.</param>
                /// <param name="offset">The offset in the buffer array to begin writing.</param>
                /// <param name="count">The number of bytes to read.</param>
                /// <returns>
                /// The result of Promise contains the number of bytes read.
                /// A less length than count means timeout occurred.
                /// </returns>
                /// <remarks>
                /// WinJS.ErrorFromName("InvalidArgument"): offset plus count is greater than the length of the buffer. 
                /// Error callback function is invoked with message=="Canceled": The Promise was canceled.
                /// </remarks>
                read: function (array, offset, count)
                {
                    var that = this;
                    var errorCallBack;
                    var cancelPromise = new WinJS.Promise(function (complete, error, progress) {
                        errorCallBack = error;
                        return WinJS.Promise.timeout(constants.enoughLongTimeout).then(function () {
                            complete();
                        });
                    },
                    function () {
                        // OnCancel
                        errorCallBack(new WinJS.ErrorFromName("UserCancel", "User canneled the read operation."));
                    });

                    return new WinJS.Promise(function (complete, error, progress) {
                        that.readInternal(array, offset, offset, count, that._readTimeout, cancelPromise).then(function (readCount) {
                            complete(readCount);
                        },
                        function (errorInfo) {
                            
                        });
                    },
                    function () {
                        // OnCancel
                        cancelPromise.cancel();
                    });
                },

                /// <summary>
                /// This is an internal method called from read method.
                /// </summary>
                /// <param name="array">The byte array, passed to Read method.</param>
                /// <param name="initialOffset">offset, passed to Read method.</param>
                /// <param name="offset">The offset in the buffer array to begin writing.</param>
                /// <param name="count">The number of bytes to read.</param>
                /// <param name="timeout">Milliseconds before a time-out occurs.</param>
                /// <param name="cancelPromise">A Promise will be used by readPartial method.</param>
                /// <returns>
                /// The result of Promise contains the length of bytes read.
                /// </returns>
                readInternal: function (array, initialOffset, offset, count, timeout, cancelPromise) {
                    var that = this;
                    var start = (new Date()).getTime();

                    var promise = new WinJS.Promise(function (complete, error, progress) {
                        that.readPartial(array, offset, count, timeout, cancelPromise).then(function (readCount) {
                            if (readCount === undefined) {
                                // Maybe some error status.
                                error();
                                return;
                            } else if (readCount < 0) {
                                // Cancel the task.
                                promise.cancel();
                                return;
                            } else if (readCount < count) {
                                var newTimeout = constants.infiniteTimeout;
                                if (timeout >= 0) {
                                    newTimeout = timeout - ((new Date()).getTime() - start);
                                    if (newTimeout < 0) {
                                        // Timeout
                                        complete(offset + readCount - initialOffset);
                                        return;
                                    }
                                }
                                that.readInternal(array, initialOffset, offset + readCount, count - readCount, newTimeout, cancelPromise).then(function (readInternalCount) {
                                    complete(readInternalCount);
                                });
                                return;
                            }
                            complete(offset + readCount - initialOffset);
                        });
                    });

                    return promise;
                },

                /// <summary>
                /// This is an internal method called from readInternal method.
                /// </summary>
                /// <param name="array">The byte array, passed to Read method.</param>
                /// <param name="offset">The offset in the buffer array to begin writing.</param>
                /// <param name="count">The number of bytes to read.</param>
                /// <param name="timeout">Milliseconds before a time-out occurs.</param>
                /// <param name="cancelPromise">A Promise will be signaled by user cancellation.</param>
                /// <returns>
                /// The result of Promise contains the length of bytes read. This may be less than count.
                /// </returns>
                readPartial: function (array, offset, count, timeout, cancelPromise) {
                    // Buffer check.
                    if (array.length < (offset + count)) {
                        throw new WinJS.ErrorFromName("InvalidArgument", "offset plus count is greater than the length of the buffer.");
                    }

                    var inputStream = this._cdcData.bulkInPipes[0].inputStream;
                    var reader = new Windows.Storage.Streams.DataReader(inputStream);
                    var loadAsyncPromise = reader.loadAsync(count);
                    loadAsyncPromise.then(function (loadedCount) {
                        if (loadedCount > 0) {
                            for (var i = 0; i < loadedCount; i++) {
                                var data = reader.readByte();
                                array[i + offset] = data;
                            }
                        }
                        return loadedCount;
                    });
                    var timeoutPromise = WinJS.Promise.timeout(timeout === constants.infiniteTimeout ? (constants.enoughLongTimeout) : timeout);
                    var promises = [loadAsyncPromise, cancelPromise, timeoutPromise];
                    function cancelPromises (exclude) {
                        promises.forEach(function (promise, index) {
                            // Don't cancel cancelPromise because it may be reused again.
                            if (promise !== cancelPromise) {
                                if (index !== exclude) {
                                    promise.cancel();
                                }
                            }
                        });
                    };
                    return new WinJS.Promise(function (complete, error, progress) {
                        return WinJS.Promise.any(promises).then(function (result) {
                            cancelPromises(parseInt(result.key, 10));
                            switch (result.key) {
                                case "0": // Loaded
                                    complete(result.value.operation.getResults());
                                    break;
                                case "1": // Canceled
                                    complete(-1);
                                    break;
                                case "2": // Timeout
                                    complete(0);
                                    break;
                                default:
                                    error(new WinJS.ErrorFromName("Unexpected", "Unexpected index value of promise array."));
                                    break;
                            }
                        },
                        function (err) {
                            cancelPromises(parseInt(err.key, 10));
                            switch (err.key) {
                                case "1": // Canceled
                                    complete(-1);
                                    return;
                            }
                            error(err);
                        });
                    });
                },

                /// <summary>
                /// Writes a specified number of bytes to the serial port using data from a buffer.
                /// </summary>
                /// <param name="buffer">The byte array that contains the data to write to the port.</param>
                /// <param name="offset">The zero-based byte offset in the buffer parameter at which to begin copying bytes to the port.</param>
                /// <param name="count">The number of bytes to write.</param>
                /// <remarks>
                /// WinJS.ErrorFromName("InvalidArgument"): offset plus count is greater than the length of the buffer.
                /// </remarks>
                write: function (buffer, offset, count) {
                    // Overflow check.
                    if (buffer.length < (offset + count)) {
                        throw new WinJS.ErrorFromName("InvalidArgument", "offset plus count is greater than the length of the buffer.");
                    }
                    var outputStream = this._cdcData.bulkOutPipes[0].outputStream;
                    var writer = new Windows.Storage.Streams.DataWriter(outputStream);
                    writer.writeBuffer(buffer, offset, count);
                    return writer.storeAsync();
                },

                /// <summary>
                /// Gets or Sets a value that enables the Data Terminal Ready (DTR) signal during serial communication.
                /// </summary>
                /// <remarks>
                /// Set: Promise completes when DTR has been set to the serial port. 
                /// </remarks>
                dtrEnable: {
                    get: function () { return this._dtrEnable; },
                    set: function (value) {
                        var that = this;
                        return new WinJS.Promise(function (complete, error, progress) {
                            that._dtrEnable = value;
                            that.setControlLineState(that._cdcControl.interfaceNumber).then(function (count) {
                                complete();
                            });
                        });
                    }
                },

                /// <summary>
                /// Gets or Sets a value indicating whether the Request to Send (RTS) signal is enabled during serial communication.
                /// </summary>
                /// <remarks>
                /// Set: Promise completes when RTS has been set to the serial port. 
                /// </remarks>
                rtsEnable: {
                    get: function () { return this._rtsEnable; },
                    set: function (value) {
                        var that = this;
                        return new WinJS.Promise(function (complete, error, progress) {
                            that._rtsEnable = value;
                            that.setControlLineState(that._cdcControl.interfaceNumber).then(function (count) {
                                complete();
                            });
                        });
                    }
                },

                /// <summary>
                /// Sends a raw CDC request.
                /// </summary>
                /// <param name="index">Interface index.</param>
                /// <param name="requestType">UsbControlRequestType for CDC request.</param>
                /// <param name="request">CDC request code.</param>
                /// <param name="value">value, corresponded with the request code.</param>
                /// <param name="buffer">data, corresponded with the request code.</param>
                /// <returns>
                /// The result of Promise contains a length of 'buffer' bytes actually sent to the serial port.
                /// </returns>
                usbControlRequestForSet: function (index, requestType, request, value, buffer) {
                    var packet = new Windows.Devices.Usb.UsbSetupPacket();
                    packet.requestType = requestType;
                    packet.request = request;
                    packet.value = value;
                    packet.length = buffer !== null ? buffer.length : 0;
                    packet.index = index;

                    return this._device.sendControlOutTransferAsync(packet, buffer);
                },

                /// <summary>
                /// GET_LINE_CODING CDC request.
                /// </summary>
                /// <param name="index">Interface index.</param>
                /// <returns>
                /// The result of Promise contains a buffer of Line Coding structure.
                /// </returns>
                getLineCoding: function (index) {
                    var that = this;
                    // GetLineCoding
                    var buffer = new Windows.Storage.Streams.Buffer(constants.expectedResultGetLineCoding);
                    buffer.length = constants.expectedResultGetLineCoding;

                    var requestType = new Windows.Devices.Usb.UsbControlRequestType();
                    requestType.asByte = constants.requestType.get;

                    var packet = new Windows.Devices.Usb.UsbSetupPacket();
                    packet.requestType = requestType;
                    packet.request = constants.requestCode.getLineCoding;
                    packet.value = 0;
                    packet.length = buffer.length;
                    packet.index = index;
            
                    return that._device.sendControlInTransferAsync(packet, buffer);
                },

                /// <summary>
                /// SET_LINE_CODING CDC request.
                /// </summary>
                /// <param name="index">Interface index.</param>
                /// <param name="dteRate">Data terminal rate, in bits per second.</param>
                /// <param name="charFormat">Stop bits.</param>
                /// <param name="parityType">Parity.</param>
                /// <param name="dataBits">Data bits.</param>
                /// <returns>
                /// The result of Promise contains a length of bytes actually sent to the serial port.
                /// </returns>
                setLineCoding: function (index, dteRate, charFormat, parityType, dataBits) {
                    // SetLineCoding
                    var writer = new Windows.Storage.Streams.DataWriter();
                    writer.byteOrder = Windows.Storage.Streams.ByteOrder.littleEndian;
                    writer.writeUInt32(dteRate);
                    writer.writeByte(charFormat);
                    writer.writeByte(parityType);
                    writer.writeByte(dataBits);
                    var buffer = writer.detachBuffer();

                    var requestType = new Windows.Devices.Usb.UsbControlRequestType();
                    requestType.asByte = constants.requestType.set;

                    return this.usbControlRequestForSet(
                        index,
                        requestType,
                        constants.requestCode.setLineCoding,
                        0,
                        buffer);
                },

                /// <summary>
                /// SET_CONTROL_LINE_STATE CDC request.
                /// </summary>
                /// <param name="index">Interface index.</param>
                /// <returns>
                /// The result of Promise contains a length of bytes actually sent to the serial port. Should be zero.
                /// </returns>
                setControlLineState: function (index) {
                    // SetControlLineState

                    var requestType = new Windows.Devices.Usb.UsbControlRequestType();
                    requestType.asByte = constants.requestType.set;

                    var value = (this._rtsEnable ? 1 : 0) << 1 | (this._dtrEnable ? 1 : 0);

                    return this.usbControlRequestForSet(
                        index,
                        requestType,
                        constants.requestCode.setControlLineState,
                        value,
                        null);
                },
            },
            // StaticMembers
            {
                /// <summary>
                /// Initializes a new instance of the UsbSerialPort class.
                /// </summary>
                /// <param name="device">
                /// UsbDevice, whose functions should be compatible with CdcControl and CdcData.
                /// </param>
                /// <returns>
                /// The result of Promise contains a new UsbSerialPort instance. It contains null, if failed.
                /// </returns>
                create: function (device) {
                    return new WinJS.Promise(function (complete, error, progress) {
                        var instance = new UsbCdcControlAccess.UsbSerialPort(device);
                        instance.tryInitialize().done(function (success) {
                            if (success) {
                                complete(instance);
                            } else {
                                complete(null);
                            }
                        }
                        , function (err) {
                            error(err);
                        });
                    });
                },
            }
        )
    });
})();