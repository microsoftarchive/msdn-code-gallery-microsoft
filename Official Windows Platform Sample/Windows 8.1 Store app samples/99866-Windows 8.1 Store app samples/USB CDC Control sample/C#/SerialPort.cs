//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.Threading.Tasks;
using Windows.Devices.Usb;

namespace UsbCdcControl
{
    namespace UsbCdcControlAccess
    {
        /// <summary> 
        /// Serial port class, wraps Windows.Devices.Usb.UsbDevice.
        /// </summary>
        public sealed class UsbSerialPort 
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
            public static Windows.Foundation.IAsyncOperation<UsbSerialPort> CreateAsync(
                Windows.Devices.Usb.UsbDevice device
            )
            {
                return Task.Run(async () =>
                {
                    var that = new UsbSerialPort(device);
                    var success = await that.TryInitialize();
                    if (success)
                    {
                        return that;
                    }
                    else
                    {
                        return null;
                    }
                }
                ).AsAsyncOperation<UsbSerialPort>();
            }

            /// <summary>
            /// Opens the serial port using the specified baud rate, parity bit, data bits, and stop bit.
            /// </summary>
            /// <param name="baudRate">The baud rate.</param>
            /// <param name="parity">One of the Parity values.</param>
            /// <param name="dataBits">The data bits value.</param>
            /// <param name="stopBits">One of the StopBits values.</param>
            public Windows.Foundation.IAsyncAction Open(
                uint baudRate,
                Parity parity,
                int dataBits,
                StopBits stopBits
            )
            {
                return Task.Run(async () =>
                {
                    this.baudRate  = baudRate;
                    this.parity    = parity;
                    this.dataBits  = dataBits;
                    this.stopBits  = stopBits;

                    if (this.cdcControl == null)
                    {
                        return;
                    }

                    // Do SetLineCoding
                    var len = await SetLineCoding(this.cdcControl.InterfaceNumber, this.baudRate, (byte)this.stopBits, (byte)this.parity, (byte)this.dataBits);
                    if (len != Constants.ExpectedResultSetLineCoding)
                    {
                        throw new System.NotSupportedException("SetLineCoding request is not supported.");
                    }

                    // Do SetControlLineState
                    len = await SetControlLineState(this.cdcControl.InterfaceNumber);
                    
                    return;

                }).AsAsyncAction();
            }

            /// <summary>
            /// Sends a raw CDC request.
            /// </summary>
            /// <param name="request">CDC request code.</param>
            /// <param name="value">value, corresponded with the request code.</param>
            /// <param name="buffer">data, corresponded with the request code.</param>
            /// <returns>
            /// The result of IAsyncOperation contains a length of bytes actually sent.
            /// </returns>
            public Windows.Foundation.IAsyncOperation<uint> SetControlRequest(
                byte request,
                ushort value,
                Windows.Storage.Streams.IBuffer buffer
            )
            {
                return Task.Run(async () =>
                {
                    var requestType = new UsbControlRequestType();
                    requestType.AsByte = RequestType.Set;
                    return await UsbControlRequestForSet(this.cdcControl.InterfaceNumber, requestType, request, value, buffer);
                }
                ).AsAsyncOperation<uint>();
            }

            /// <summary>
            /// Reads a number of bytes from the SerialPort input buffer and writes those bytes into a byte array at the specified offset.
            /// </summary>
            /// <param name="buffer">The byte array to write the input to.</param>
            /// <param name="offset">The offset in the buffer array to begin writing.</param>
            /// <param name="count">The number of bytes to read.</param>
            /// <param name="ct">A CancellationToken in order to notify of cancellation from user.</param>
            /// <returns>
            /// The result of IAsyncOperation contains the number of bytes read.
            /// A less length than count means timeout occurred.
            /// </returns>
            /// <remarks>
            /// ArgumentException: offset plus count is greater than the length of the buffer. 
            /// OperationCanceledException: The IAsyncOperation was canceled.
            /// </remarks>
            public Windows.Foundation.IAsyncOperation<int> Read(
                Windows.Storage.Streams.IBuffer buffer, 
                uint offset, 
                uint count,
                System.Threading.CancellationToken ct
            )
            {
                return Task.Run(async () =>
                {
                    var readcount = await ReadInternal(buffer, Convert.ToInt32(offset), Convert.ToInt32(count), this.ReadTimeout, ct);
                    buffer.Length = (uint)readcount;
                    return readcount;
                },
                ct).AsAsyncOperation<int>();
            }

            /// <summary>
            /// Writes a specified number of bytes to the serial port using data from a buffer.
            /// </summary>
            /// <param name="buffer">The byte array that contains the data to write to the port.</param>
            /// <param name="offset">The zero-based byte offset in the buffer parameter at which to begin copying bytes to the port.</param>
            /// <param name="count">The number of bytes to write.</param>
            /// <remarks>
            /// ArgumentException: offset plus count is greater than the length of the buffer.
            /// </remarks>
            public Windows.Foundation.IAsyncAction Write(
                Windows.Storage.Streams.IBuffer buffer,
                uint offset,
                uint count
            )
            {
                var outputStream = this.cdcData.BulkOutPipes[0].OutputStream;
                var writer = new Windows.Storage.Streams.DataWriter(outputStream);

                return Task.Run(async () =>
                {
                    // Overflow check.
                    if ((int)buffer.Length < (offset + count))
                    {
                        throw new System.ArgumentException("Capacity of buffer is not enough.");
                    }

                    writer.WriteBuffer(buffer, offset, count);

                    var written = await writer.StoreAsync();

                    return;
                }
                ).AsAsyncAction();
            }

            /// <summary>
            /// Gets or Sets a value that enables the Data Terminal Ready (DTR) signal during serial communication.
            /// </summary>
            private bool DtrEnable
            {
                get;
                set;
            }

            /// <summary>
            /// Sets a value that enables the Data Terminal Ready (DTR) signal during serial communication.
            /// </summary>
            /// <remarks>
            /// IAsyncOperation completes when DTR has been set to the serial port. 
            /// </remarks>
            public Windows.Foundation.IAsyncAction DtrEnable_set(bool value)
            {
                return Task.Run(async () =>
                {
                    this.DtrEnable = value;
                    var count = await SetControlLineState(this.cdcControl.InterfaceNumber);
                    return;
                }
                ).AsAsyncAction();
            }

            /// <summary>
            /// Gets or Sets a value indicating whether the Request to Send (RTS) signal is enabled during serial communication.
            /// </summary>
            private bool RtsEnable
            {
                get;
                set;
            }

            /// <summary>
            /// Sets a value indicating whether the Request to Send (RTS) signal is enabled during serial communication.
            /// </summary>
            /// <remarks>
            /// IAsyncOperation completes when RTS has been set to the serial port. 
            /// </remarks>
            public Windows.Foundation.IAsyncAction RtsEnable_set(bool value)
            {
                return Task.Run(async () =>
                {
                    this.RtsEnable = value;
                    var count = await SetControlLineState(this.cdcControl.InterfaceNumber);
                    return;
                }
                ).AsAsyncAction();
            }

            /// <summary>
            /// Gets or sets the number of milliseconds before a time-out occurs when a read operation does not finish.
            /// </summary>
            /// <remarks>
            /// -1 is the default value, means InfiniteTimeout.
            /// </remarks>
            public int ReadTimeout
            {
                get;
                set;
            }

            /// <summary>
            /// Gets the UsbDevice, which has been passed to CreateAsync method.
            /// </summary>
            public Windows.Devices.Usb.UsbDevice UsbDevice
            {
                get
                {
                    return this.device;
                }
            }

            /// <summary>
            /// Sends a raw CDC request.
            /// </summary>
            /// <param name="index">Interface index.</param>
            /// <param name="requestType">UsbControlRequestType for CDC request.</param>
            /// <param name="request">CDC request code.</param>
            /// <param name="value">value, corresponded with the request code.</param>
            /// <param name="buffer">data, corresponded with the request code.</param>
            /// <returns>
            /// The result of Task contains a length of bytes actually sent to the serial port.
            /// </returns>
            private Task<uint> UsbControlRequestForSet(
                uint index,
                Windows.Devices.Usb.UsbControlRequestType requestType,
                byte request,
                ushort value,
                Windows.Storage.Streams.IBuffer buffer
            )
            {
                return Task.Run(async () =>
                {
                    var packet = new UsbSetupPacket();
                    packet.RequestType = requestType;
                    packet.Request = request;
                    packet.Value = value;
                    packet.Length = buffer != null ? buffer.Length : 0;
                    packet.Index = index;

                    return await this.device.SendControlOutTransferAsync(packet, buffer);
                });
            }

            /// <summary>
            /// GET_LINE_CODING CDC request.
            /// </summary>
            /// <param name="index">Interface index.</param>
            /// <returns>
            /// The result of Task contains a buffer of Line Coding structure.
            /// </returns>
            private Task<Windows.Storage.Streams.IBuffer> GetLineCoding(
                uint index
            )
            {
                return Task.Run(async () =>
                {
                    var buffer = new Windows.Storage.Streams.Buffer(Constants.ExpectedResultGetLineCoding);
                    buffer.Length = Constants.ExpectedResultGetLineCoding;

                    var requestType = new UsbControlRequestType();
                    requestType.AsByte = RequestType.Get;

                    var packet = new UsbSetupPacket();
                    packet.RequestType = requestType;
                    packet.Request = RequestCode.GetLineCoding;
                    packet.Value = 0;
                    packet.Length = buffer.Length;
                    packet.Index = index;
            
                    return await this.device.SendControlInTransferAsync(packet, buffer);
                });
            }

            /// <summary>
            /// SET_LINE_CODING CDC request.
            /// </summary>
            /// <param name="index">Interface index.</param>
            /// <param name="dteRate">Data terminal rate, in bits per second.</param>
            /// <param name="charFormat">Stop bits.</param>
            /// <param name="parityType">Parity.</param>
            /// <param name="dataBits">Data bits.</param>
            /// <returns>
            /// The result of Task contains a length of bytes actually sent to the serial port.
            /// </returns>
           private Task<uint> SetLineCoding(
                uint index,
                uint dteRate,
                byte charFormat,
                byte parityType,
                byte dataBits
            )
            {
                // SetLineCoding
                var writer = new Windows.Storage.Streams.DataWriter();
                writer.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;
                writer.WriteUInt32(dteRate);
                writer.WriteByte(charFormat);
                writer.WriteByte(parityType);
                writer.WriteByte(dataBits);
                var buffer = writer.DetachBuffer();

                var requestType = new UsbControlRequestType();
                requestType.AsByte = RequestType.Set;

                return UsbControlRequestForSet(
                        index,
                        requestType,
                        RequestCode.SetLineCoding,
                        0,
                        buffer);
            }

            /// <summary>
            /// SET_CONTROL_LINE_STATE CDC request.
            /// </summary>
            /// <param name="index">Interface index.</param>
            /// <returns>
            /// The result of Task contains a length of bytes actually sent to the serial port. Should be zero.
            /// </returns>
            private Task<uint> SetControlLineState(
                uint index
            )
            {
                // SetControlLineState

                var requestType = new UsbControlRequestType();
                requestType.AsByte = RequestType.Set;

                var value = (this.RtsEnable ? 1 : 0) << 1 | (this.DtrEnable ? 1 : 0);

                return UsbControlRequestForSet(
                        index,
                        requestType,
                        RequestCode.SetControlLineState,
                        (ushort)value,
                        null
                        );
            }

            /// <summary>
            /// This is an internal method called from Read method.
            /// </summary>
            /// <param name="buffer">The byte array, passed to Read method.</param>
            /// <param name="offset">The offset in the buffer array to begin writing.</param>
            /// <param name="count">The number of bytes to read.</param>
            /// <param name="timeout">Milliseconds before a time-out occurs.</param>
            /// <param name="ct">A cancellation_token will be used by ReadPartial method.</param>
            /// <returns>
            /// The result of Task contains the length of bytes read.
            /// </returns>
            private Task<int> ReadInternal(
                Windows.Storage.Streams.IBuffer buffer,
                int offset, 
                int count,
                int timeout,
                System.Threading.CancellationToken ct
            )
            {
                // ConfigureAwait(false) means that we'd try to execute the task in non UI thread.
                Task<int> task;
                (task = Task.Run(async () =>
                {
                    int totalReadcount = 0;

                    while (timeout == Constants.InfiniteTimeout || timeout >= 0)
                    {
                        var start = System.DateTime.Now.Ticks; // 100 nano-sec.

                        var readcount = 0;
                        try
                        {
                            readcount = await this.ReadPartial(buffer, offset, count, timeout, ct);
                        }
                        catch (TimeoutException)
                        {
                            break; // timeout
                        }
                        catch (OperationCanceledException)
                        {
                            // Cancel the task.
                            throw new OperationCanceledException("ReadInternal was canceled.");
                        }

                        totalReadcount += readcount;

                        if (readcount >= count)
                        {
                            break; // completed;
                        }

                        // Prepare for the next ReadPartial.
                        if (timeout >= 0)
                        {
                            timeout = (int)(timeout - (System.DateTime.Now.Ticks - start) / 10000);
                        }
                        offset += readcount;
                        count  -= readcount;
                    }

                    return totalReadcount;
                }
                )).ConfigureAwait(false);

                return task;
            }

            /// <summary>
            /// This is an internal method called from ReadInternal method.
            /// </summary>
            /// <param name="buffer">The byte array, passed to Read method.</param>
            /// <param name="offset">The offset in the buffer array to begin writing.</param>
            /// <param name="count">The number of bytes to read.</param>
            /// <param name="timeout">Milliseconds before a time-out occurs.</param>
            /// <param name="ct">A cancellation_token will be signaled by user cancellation.</param>
            /// <returns>
            /// The result of Task contains the length of bytes read. This may be less than count.
            /// </returns>
            private Task<int> ReadPartial(
                Windows.Storage.Streams.IBuffer buffer, 
                int offset, 
                int count,
                int timeout,
                System.Threading.CancellationToken ct
            )
            {
                // Buffer check.
                if ((int)buffer.Length < (offset + count))
                {
                    throw new ArgumentException("Capacity of buffer is not enough.");
                }

                var inputStream = this.cdcData.BulkInPipes[0].InputStream;
                var reader = new Windows.Storage.Streams.DataReader(inputStream);

                return Task.Run(async () =>
                {
                    // CancellationTokenSource to cancel tasks.
                    var cancellationTokenSource = new System.Threading.CancellationTokenSource();

                    // LoadAsync task.
                    var loadTask = reader.LoadAsync((uint)count).AsTask<uint>(cancellationTokenSource.Token);

                    // A timeout task that completes after the specified delay.
                    var timeoutTask = Task.Delay(timeout == Constants.InfiniteTimeout ? System.Threading.Timeout.Infinite : timeout, cancellationTokenSource.Token);

                    // Cancel tasks by user's cancellation.
                    bool canceledByUser = false;
                    ct.Register(()=>
                    {
                        canceledByUser = true;
                        cancellationTokenSource.Cancel();
                    });

                    // Wait tasks.
                    Task[] tasks = { loadTask, timeoutTask };
                    var signaledTask = await Task.WhenAny(tasks);

                    // Check the task status.
                    bool loadCompleted = signaledTask.Equals(loadTask) && loadTask.IsCompleted && !loadTask.IsCanceled;
                    bool isTimeout = signaledTask.Equals(timeoutTask) && timeoutTask.IsCompleted && !timeoutTask.IsCanceled;

                    // Cancel all incomplete tasks.
                    cancellationTokenSource.Cancel();

                    int loadedCount = 0;
                    if (loadCompleted)
                    {
                        loadedCount = (int)loadTask.Result;
                    }
                    else if (isTimeout)
                    {
                        // Timeout.
                        throw new System.TimeoutException("ReadPartial was timeout.");
                    }
                    else if (canceledByUser)
                    {
                        throw new OperationCanceledException("ReadPartial was canceled.");
                    }

                    if (loadedCount > 0)
                    {
                        var readBuffer = reader.ReadBuffer((uint)loadedCount);
                        System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions.CopyTo(readBuffer, 0, buffer, (uint)offset, (uint)loadedCount);
                    }

                    return loadedCount;
                });
            }

            /// <summary>
            /// Constructor, called from CreateAsync method.
            /// </summary>
            /// <param name="device">The UsbDevice, passed to CreateAsync method.</param>
            private UsbSerialPort (
                Windows.Devices.Usb.UsbDevice device
            )
            {
                this.device = device;

                this.DtrEnable = false;
                this.RtsEnable = false;

                this.ReadTimeout = Constants.InfiniteTimeout;

                this.cdcControl = null;
                this.cdcData = null;
            }

            /// <summary>
            /// This is an internal method called from CreateAsync method.
            /// </summary>
            /// <remarks>
            /// The result of Task contains success(true) or failure(false).
            /// </remarks>
            private Task<bool> TryInitialize()
            {
                return Task.Run(async () =>
                {
                    var device = this.device;

                    Windows.Devices.Usb.UsbInterface cdcControl = null;
                    Windows.Devices.Usb.UsbInterface cdcData = null;

                    for (int i = 0; i < device.Configuration.UsbInterfaces.Count; i ++)
                    {
                        var interf = device.Configuration.UsbInterfaces[i];
                        if (interf == null)
                        {
                            continue;
                        }

                        for (int j = 0; j < interf.InterfaceSettings.Count; j ++)
                        {
                            var setting = interf.InterfaceSettings[j];
                            byte index = setting.InterfaceDescriptor.InterfaceNumber;
                            byte interfaceClass = setting.InterfaceDescriptor.ClassCode;

                            if (interfaceClass == InterfaceClass.CdcControl)
                            {
                                cdcControl = interf;
                            }
                            else if (interfaceClass == InterfaceClass.CdcData)
                            {
                                cdcData = interf;
                            }
                            else if (interfaceClass == InterfaceClass.VendorSpecific)
                            {
                                if (cdcControl == null)
                                {
                                    // Test SetLineCoding/GetLineCoding
                                    uint written = 0;
                                    Windows.Storage.Streams.IBuffer gotBuffer = null;
                                    try
                                    {
                                        written = await this.SetLineCoding(index, Constants.DteRateTestValue, 0, 0, Constants.DataBitsTestValue);
                                    }
                                    catch (Exception)
                                    {
                                        // This interface is not compatible with CDC ACM. Try the next interface.
                                        continue;
                                    }

                                    try
                                    {
                                        gotBuffer = await this.GetLineCoding(index);
                                    }
                                    catch (Exception)
                                    {
                                        // This interface is not compatible with CDC ACM. Try the next interface.
                                        continue;
                                    }
                                        
                                    if (gotBuffer != null && gotBuffer.Length == Constants.ExpectedResultGetLineCoding)
                                    {
                                        var lineCodingReader = Windows.Storage.Streams.DataReader.FromBuffer(gotBuffer);
                                        lineCodingReader.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;
                                        var bps = lineCodingReader.ReadUInt32();
                                        if (bps == Constants.DteRateTestValue)
                                        {
                                            // This could be a CdcControl.
                                            cdcControl = interf;
                                        }
                                    }
                                }

                                if (cdcData == null)
                                {
                                    if (interf.BulkInPipes.Count > 0 &&
                                        interf.BulkOutPipes.Count > 0)
                                    {
                                        // This could be a CdcData.
                                        cdcData = interf;
                                    }
                                }
                            }
                        }
                    }

                    this.cdcControl = cdcControl;
                    this.cdcData    = cdcData;

                    if (this.cdcControl != null && this.cdcData != null)
                    {
                        // Flush the pipes.
                        for (int IN = 0; IN < this.cdcData.BulkInPipes.Count; IN++)
                        {
                            this.cdcData.BulkInPipes[IN].FlushBuffer();
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }

            private Windows.Devices.Usb.UsbDevice   device;
            private uint                            baudRate;
            private Parity                          parity;
            private int                             dataBits;
            private StopBits                        stopBits;

            // USB interfaces
            private Windows.Devices.Usb.UsbInterface cdcControl;
            private Windows.Devices.Usb.UsbInterface cdcData;
        };
    }
}




