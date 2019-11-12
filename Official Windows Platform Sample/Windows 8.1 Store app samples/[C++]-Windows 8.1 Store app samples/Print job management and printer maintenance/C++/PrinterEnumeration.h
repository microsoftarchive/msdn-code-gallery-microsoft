//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// PrinterEnumeration.h
// Declaration of the PrinterEnumeration class.
//
#pragma once

#include "pch.h"

namespace SDKSample
{
    namespace DeviceAppForPrinters2
    {
        /// <summary>
        /// Represents one printer queue object, and exposes its properties
        /// in a fashion suitable to data binding.
        /// </summary>
        [Windows::UI::Xaml::Data::Bindable]
        public ref class PrinterInfo sealed
        {
            // Public constructor and methods.
        public:
            PrinterInfo(Platform::String^ name, Platform::String^ deviceId)
            {
                this->name = name;
                this->deviceId = deviceId;
            }

            /// <summary>
            /// Returns the name of the print queue.
            /// </summary>
            property Platform::String^ Name
            {
                Platform::String^ get()
                {
                    return name;
                }
            }

            /// <summary>
            /// Returns the device id for the print queue.
            /// </summary>
            property Platform::String^ DeviceId
            {
                Platform::String^ get()
                {
                    return deviceId;
                }
            }

            // Private members.
        private:
            Platform::String^ name;
            Platform::String^ deviceId;
        };

        /// <summary>
        /// Exposes functionality used to enumerate printers associated with a Windows Store Device App (WSDA).
        /// </summary>
        ref class PrinterEnumeration sealed
        {
            // Public constructors and methods
        internal:
            /// <summary>
            /// Enumerates printers associated with the package family name provided at construction time.
            /// </summary>
            static Concurrency::task<Windows::Foundation::Collections::IVector<PrinterInfo^>^> EnumeratePrintersAsync(Platform::String^ packageFamilyName);

            // Private methods
        private:
            /// <summary>
            /// Exposes functionality used to enumerate printers associated with a Windows Store Device App (WSDA).
            /// </summary>
            /// <param name="packageFamilyName">
            /// Package family name of the Windows Store Device App.
            /// </param>
            PrinterEnumeration(Platform::String^ packageFamilyName)
            {
                this->packageFamilyName = packageFamilyName;
                printQueueCollection = ref new Platform::Collections::Vector<PrinterInfo^>();
            }

            ~PrinterEnumeration() { }

            /// <summary>
            /// Retrieves the list of printers associated with the Windows Store Device App, from the supplied
			/// collection of printer device interfaces (in the DeviceInformationCollection object).
            /// </summary>
            Concurrency::task<Windows::Foundation::Collections::IVector<PrinterInfo^>^> EnumerateAssociatedPrintersAsync(
                Windows::Devices::Enumeration::DeviceInformationCollection^ printerDeviceInterfaceCollection);

            /// <summary>
            /// Determines if the device container is registered with the Windows Store Device App.
            /// </summary>
            bool IsContainerRegisteredWithDeviceApp(Windows::Devices::Enumeration::Pnp::PnpObject^ deviceContainer);

            // Private members
        private:
            Platform::String^ packageFamilyName;
            Platform::Collections::Vector<PrinterInfo^>^ printQueueCollection;
        };


    }
} // namespace SDKSample