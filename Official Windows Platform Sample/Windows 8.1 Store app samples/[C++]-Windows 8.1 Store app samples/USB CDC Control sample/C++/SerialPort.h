//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once

#include "DeviceList.h"

namespace SDKSample
{
    namespace UsbCdcControl
    {
        /// <summary>
        /// Serial port class, wraps Windows::Devices::Usb::UsbDevice.
        /// </summary>
        public ref class UsbSerialPort sealed
        {
        public:
            /// <summary>
            /// Initializes a new instance of the UsbSerialPort class.
            /// </summary>
            /// <param name="device">
            /// UsbDevice, whose functions should be compatible with CdcControl and CdcData.
            /// </param>
            /// <returns>
            /// The result of IAsyncOperation contains a new UsbSerialPort instance. It contains nullptr, if failed.
            /// </returns>
            static Windows::Foundation::IAsyncOperation<UsbSerialPort^>^ Create(
                Windows::Devices::Usb::UsbDevice^ device
                );

            /// <summary>
            /// Opens the serial port using the specified baud rate, parity bit, data bits, and stop bit.
            /// </summary>
            /// <param name="baudRate">The baud rate.</param>
            /// <param name="parity">One of the Parity values.</param>
            /// <param name="dataBits">The data bits value.</param>
            /// <param name="stopBits">One of the StopBits values.</param>
            Windows::Foundation::IAsyncOperation<Windows::Foundation::HResult>^ Open(
                int baudRate,
                Parity parity,
                int dataBits,
                StopBits stopBits
                );

            /// <summary>
            /// Sends a raw CDC request.
            /// </summary>
            /// <param name="request">CDC request code.</param>
            /// <param name="value">value, corresponded with the request code.</param>
            /// <param name="buffer">data, corresponded with the request code.</param>
            /// <returns>
            /// The result of IAsyncOperation contains a length of bytes actually sent.
            /// </returns>
            Windows::Foundation::IAsyncOperation<unsigned int>^ SetControlRequest(
                BYTE request,
                SHORT value,
                Windows::Storage::Streams::IBuffer^ buffer
                );

            /// <summary>
            /// Reads a number of bytes from the SerialPort input buffer and writes those bytes into a byte array at the specified offset.
            /// </summary>
            /// <param name="buffer">The byte array to write the input to.</param>
            /// <param name="offset">The offset in the buffer array to begin writing.</param>
            /// <param name="count">The number of bytes to read.</param>
            /// <returns>
            /// The result of IAsyncOperation contains the number of bytes read.
            /// A less length than count means timeout occurred.
            /// </returns>
            /// <remarks>
            /// InvalidArgumentException: offset plus count is greater than the length of the buffer. 
            /// concurrency::task_canceled: The IAsyncOperation was canceled.
            /// </remarks>
            Windows::Foundation::IAsyncOperation<int>^ Read(
                Windows::Storage::Streams::IBuffer^ buffer,
                int offset,
                int count
                );

            /// <summary>
            /// Writes a specified number of bytes to the serial port using data from a buffer.
            /// </summary>
            /// <param name="buffer">The byte array that contains the data to write to the port.</param>
            /// <param name="offset">The zero-based byte offset in the buffer parameter at which to begin copying bytes to the port.</param>
            /// <param name="count">The number of bytes to write.</param>
            /// <remarks>
            /// InvalidArgumentException: offset plus count is greater than the length of the buffer.
            /// </remarks>
            Windows::Foundation::IAsyncOperation<Windows::Foundation::HResult>^ Write(
                Windows::Storage::Streams::IBuffer^ buffer,
                int offset,
                int count
                );

            /// <summary>
            /// Gets a value that enables the Data Terminal Ready (DTR) signal during serial communication.
            /// </summary>
            bool DtrEnable_get();

            /// <summary>
            /// Sets a value that enables the Data Terminal Ready (DTR) signal during serial communication.
            /// </summary>
            /// <remarks>
            /// IAsyncOperation completes when DTR has been set to the serial port. 
            /// </remarks>
            Windows::Foundation::IAsyncAction^ DtrEnable_set(bool value);

            /// <summary>
            /// Gets a value indicating whether the Request to Send (RTS) signal is enabled during serial communication.
            /// </summary>
            bool RtsEnable_get();

            /// <summary>
            /// Sets a value indicating whether the Request to Send (RTS) signal is enabled during serial communication.
            /// </summary>
            /// <remarks>
            /// IAsyncOperation completes when RTS has been set to the serial port. 
            /// </remarks>
            Windows::Foundation::IAsyncAction^ RtsEnable_set(bool value);

            /// <summary>
            /// Gets or sets the number of milliseconds before a time-out occurs when a read operation does not finish.
            /// </summary>
            /// <remarks>
            /// -1 is the default value, means InfiniteTimeout.
            /// </remarks>
            property int ReadTimeout {
                int get();
                void set(int value);
            }

            /// <summary>
            /// Gets the UsbDevice, which has been passed to Create method.
            /// </summary>
            property Windows::Devices::Usb::UsbDevice^ UsbDevice {
                Windows::Devices::Usb::UsbDevice^ get() {
                    return this->device;
                }
            }

        private:
            /// <summary>
            /// Sends a raw CDC request.
            /// </summary>
            /// <param name="index">Interface index.</param>
            /// <param name="requestType">UsbControlRequestType for CDC request.</param>
            /// <param name="request">CDC request code.</param>
            /// <param name="value">value, corresponded with the request code.</param>
            /// <param name="buffer">data, corresponded with the request code.</param>
            /// <returns>
            /// The result of Concurrency::task contains a length of bytes actually sent to the serial port.
            /// </returns>
            Concurrency::task<unsigned int> UsbControlRequestForSet(
                unsigned int index,
                Windows::Devices::Usb::UsbControlRequestType^ requestType,
                BYTE request,
                SHORT value,
                Windows::Storage::Streams::IBuffer^ buffer
                );

            /// <summary>
            /// GET_LINE_CODING CDC request.
            /// </summary>
            /// <param name="index">Interface index.</param>
            /// <returns>
            /// The result of Concurrency::task contains a buffer of Line Coding structure.
            /// </returns>
            Concurrency::task<Windows::Storage::Streams::IBuffer^> GetLineCoding(
                unsigned int index
                );

            /// <summary>
            /// SET_LINE_CODING CDC request.
            /// </summary>
            /// <param name="index">Interface index.</param>
            /// <param name="dteRate">Data terminal rate, in bits per second.</param>
            /// <param name="charFormat">Stop bits.</param>
            /// <param name="parityType">Parity.</param>
            /// <param name="dataBits">Data bits.</param>
            /// <returns>
            /// The result of Concurrency::task contains a length of bytes actually sent to the serial port.
            /// </returns>
            Concurrency::task<unsigned int> SetLineCoding(
                unsigned int index,
                DWORD dteRate,
                BYTE charFormat,
                BYTE parityType,
                BYTE dataBits
                );

            /// <summary>
            /// SET_CONTROL_LINE_STATE CDC request.
            /// </summary>
            /// <param name="index">Interface index.</param>
            /// <returns>
            /// The result of Concurrency::task contains a length of bytes actually sent to the serial port. Should be zero.
            /// </returns>
            Concurrency::task<unsigned int> SetControlLineState(
                unsigned int index
                );

            /// <summary>
            /// This is an internal method called from Read method.
            /// </summary>
            /// <param name="buffer">The byte array, passed to Read method.</param>
            /// <param name="offset">The offset in the buffer array to begin writing.</param>
            /// <param name="count">The number of bytes to read.</param>
            /// <param name="timeout">Milliseconds before a time-out occurs.</param>
            /// <param name="ct">A cancellation_token will be used by ReadPartial method.</param>
            /// <returns>
            /// The result of Concurrency::task contains the length of bytes read.
            /// </returns>
            Concurrency::task<int> ReadInternal(
                Windows::Storage::Streams::IBuffer^ buffer,
                int offset,
                int count,
                int timeout,
                Concurrency::cancellation_token ct
                );

            /// <summary>
            /// This is an internal method called from ReadInternal method.
            /// </summary>
            /// <param name="buffer">The byte array, passed to Read method.</param>
            /// <param name="offset">The offset in the buffer array to begin writing.</param>
            /// <param name="count">The number of bytes to read.</param>
            /// <param name="timeout">Milliseconds before a time-out occurs.</param>
            /// <param name="ct">A cancellation_token will be signaled by user cancellation.</param>
            /// <returns>
            /// The result of Concurrency::task contains the length of bytes read. This may be less than count.
            /// </returns>
            Concurrency::task<int> ReadPartial(
                Windows::Storage::Streams::IBuffer^ buffer,
                int offset,
                int count,
                int timeout,
                Concurrency::cancellation_token ct
                );

            /// <summary>
            /// Constructor, called from Create method.
            /// </summary>
            /// <param name="device">The UsbDevice, passed to Create method.</param>
            UsbSerialPort(
                Windows::Devices::Usb::UsbDevice^ device
                );

            /// <summary>
            /// This is an internal method called from Create method.
            /// </summary>
            /// <remarks>
            /// The result of Concurrency::task contains success(true) or failure(false).
            /// </remarks>
            Concurrency::task<bool> TryInitialize();

        private:
            static const int ReadPartialResultTimeout = -1;
            static const int ReadPartialResultCanceled = -2;
            static const int ReadPartialResultInvalidArgument = -3;

        private:
            Windows::Devices::Usb::UsbDevice^   device;
            int                                 baudRate;
            Parity                              parity;
            int                                 dataBits;
            StopBits                            stopBits;
            bool                                dtrEnable;
            bool                                rtsEnable;
            int                                 readTimeout;  // Milliseconds

            // USB interfaces
            Windows::Devices::Usb::UsbInterface^ cdcControl;
            Windows::Devices::Usb::UsbInterface^ cdcData;
        };

        ref class UsbDeviceComboBoxItem sealed : public Windows::UI::Xaml::Controls::ComboBoxItem
        {
        public:
            UsbDeviceComboBoxItem(UsbDeviceInfo^ info)
            {
                this->Id = info->Id;
                this->Content = info->Name;
            }

            UsbDeviceComboBoxItem(DeviceListEntry^ info)
            {
                this->Id = info->Id;
                this->Content = info->Name;
            }

            property Platform::String^ Id;
        };
    }
}