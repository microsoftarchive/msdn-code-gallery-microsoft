#pragma once

#include "pch.h"

namespace SDKSample
{
    namespace BluetoothGattHeartRate
    {
        public ref class HeartRateMeasurement sealed
        {
        public:
            property uint16 HeartRateValue
            {
                uint16 get() 
                {
                    return heartRateValue;
                }
                void set(uint16 value)
                {
                    heartRateValue = value;
                }
            }

            property boolean HasExpendedEnergy
            {
                boolean get()
                {
                    return hasExpendedEnergy;
                }
                void set(boolean value)
                {
                    hasExpendedEnergy = value;
                }
            }

            property uint16 ExpendedEnergy
            {
                uint16 get() 
                {
                    return expendedEnergy;
                }
                void set(uint16 value) 
                {
                    expendedEnergy = value;
                }
            }

            property Windows::Foundation::DateTime Timestamp
            {
                Windows::Foundation::DateTime get() 
                {
                    return timestamp;
                }
                void set(Windows::Foundation::DateTime value) 
                {
                    timestamp = value;
                }
            }

        public:
            Platform::String^ GetDescription()
            {
                std::wstringstream wss;
                auto timeFormatter = ref new Windows::Globalization::DateTimeFormatting::DateTimeFormatter("longtime");
                auto dateFormatter = ref new Windows::Globalization::DateTimeFormatting::DateTimeFormatter("longdate");

                wss << HeartRateValue << L" bpm @ " << timeFormatter->Format(Timestamp)->Data() << 
                    dateFormatter->Format(Timestamp)->Data();

                return ref new Platform::String(wss.str().c_str());
            }
            
        private:
            uint16 heartRateValue;
            boolean hasExpendedEnergy;
            uint16 expendedEnergy;
            Windows::Foundation::DateTime timestamp;
        };

        public delegate void ValueChangeCompletedHandler(HeartRateMeasurement^ heartRateMeasurement);
        public delegate void DeviceConnectionUpdatedHandler(boolean isConnected);

        public ref class HeartRateService sealed
        {
        public:
            event ValueChangeCompletedHandler^ ValueChangeCompleted;
            event DeviceConnectionUpdatedHandler^ DeviceConnectionUpdated;

            static property HeartRateService^ Instance 
            {
                HeartRateService^ get() 
                {
                    if (instance == nullptr)
                    {
                        instance = ref new HeartRateService();
                    }

                    return instance;
                }
            }

            property bool IsServiceInitialized 
            {
                bool get() 
                {
                    return isServiceInitialized;
                }
                void set(bool value)
                {
                    isServiceInitialized = value;
                }
            }

            property Windows::Devices::Bluetooth::GenericAttributeProfile::GattDeviceService^ Service
            {
                Windows::Devices::Bluetooth::GenericAttributeProfile::GattDeviceService^ get()
                {
                    return service;
                }
                void set(Windows::Devices::Bluetooth::GenericAttributeProfile::GattDeviceService^ value)
                {
                    service = value;
                }
            }

            property Windows::Foundation::Collections::IVectorView<HeartRateMeasurement^>^ DataPoints
            {
                Windows::Foundation::Collections::IVectorView<HeartRateMeasurement^>^ get()
                {
                    auto retval = ref new Platform::Collections::Vector<HeartRateMeasurement^>();

                    // Obtain a RAII lock from dataLock
                    concurrency::reader_writer_lock::scoped_lock_read lock(dataLock);

                    // Create a snapshot of the data values
                    for (auto it = begin(datapoints); it != end(datapoints); it++)
                    {
                        retval->Append(*it);
                    }

                    return retval->GetView();
                }
            }

        public:
            virtual ~HeartRateService();
            Windows::Foundation::IAsyncAction^ InitializeHeartRateServicesAsync(
                Windows::Devices::Enumeration::DeviceInformation^ device);

            Platform::String^ ProcessBodySensorLocationData(
                const Platform::Array<unsigned char>^ bodySensorLocationData);

        private:
            HeartRateService();
            
            void App_Resuming(Platform::Object^ sender, Platform::Object^ e);
            void App_Suspending(Platform::Object^ sender, Windows::ApplicationModel::SuspendingEventArgs^ e);

            concurrency::task<void> ConfigureServiceForNotificationsAsync();

            HeartRateMeasurement^ ProcessHeartRateMeasurementData(
                Platform::Array<unsigned char>^ heartRateMeasurementData);

            void Characteristic_ValueChanged(
                Windows::Devices::Bluetooth::GenericAttributeProfile::GattCharacteristic^ sender, 
                Windows::Devices::Bluetooth::GenericAttributeProfile::GattValueChangedEventArgs^ args);

            void StartDeviceConnectionWatcher();
            void HeartRateService::OnDeviceConnectionUpdated(
                Windows::Devices::Enumeration::Pnp::PnpObjectWatcher^ sender,
                Windows::Devices::Enumeration::Pnp::PnpObjectUpdate^ e);

            static HeartRateService^ instance;
            bool isServiceInitialized;

            Windows::Devices::Bluetooth::GenericAttributeProfile::GattDeviceService^ service;
            Windows::Devices::Bluetooth::GenericAttributeProfile::GattCharacteristic^ characteristic;
            Platform::String^ deviceContainerId;
            Platform::Collections::Vector<HeartRateMeasurement^>^ datapoints;
            Windows::Devices::Enumeration::Pnp::PnpObjectWatcher^ watcher;

            concurrency::reader_writer_lock dataLock;
        };
    };
};
