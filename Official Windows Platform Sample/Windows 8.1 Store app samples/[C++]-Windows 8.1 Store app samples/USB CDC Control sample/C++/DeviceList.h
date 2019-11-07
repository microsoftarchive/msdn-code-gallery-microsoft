//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once

namespace SDKSample {

    namespace UsbCdcControl {

        private ref class DeviceListEntry
        {
            Windows::Devices::Enumeration::DeviceInformation^ device;

        internal:

            property Platform::String^ InstanceId {
                Platform::String^ get(void) {
                    return safe_cast<Platform::String^>(this->device->Properties->Lookup("System.Devices.DeviceInstanceId"));
                }
            }

            property Platform::String^ Id {
                Platform::String^ get(void) {
                    return this->device->Id;
                }
            }

            property Platform::String^ Name {
                Platform::String^ get(void) {
                    return this->device->Name;
                }
            }

            property bool Matched;

            DeviceListEntry(Windows::Devices::Enumeration::DeviceInformation^ DeviceInterface) {
                this->device = DeviceInterface;
                Matched = true;
            }
        };

        public ref class UsbDeviceInfo sealed
        {
        internal:
            UsbDeviceInfo(Platform::String^ id, Platform::String^ name)
            {
                Id = id;
                Name = name;
            }

            UsbDeviceInfo(DeviceListEntry^ info)
            {
                Id = info->Id;
                Name = info->Name;
            }

        public:
            property Platform::String^ Name;
            property Platform::String^ Id;
        };

        public ref class DeviceList sealed
        {
        public:
            typedef Windows::Foundation::EventHandler<UsbDeviceInfo^> DeviceAddedHandler;
            typedef Windows::Foundation::EventHandler<UsbDeviceInfo^> DeviceRemovedHandler;

            property bool WatcherStarted {
                bool get(void) {
                    return this->watcherStarted;
                }
                void set(bool value) {
                    this->watcherStarted = value;
                }
            };

            void StartWatcher();

            void StopWatcher();

            event DeviceAddedHandler^ DeviceAdded;
            event DeviceRemovedHandler^ DeviceRemoved;

        internal:
            static property Windows::Foundation::Collections::IVectorView<DeviceList^>^ Instances {
                Windows::Foundation::Collections::IVectorView<DeviceList^>^ get();
            };

            property Windows::Foundation::Collections::IVectorView<DeviceListEntry^>^ Devices {
                Windows::Foundation::Collections::IVectorView<DeviceListEntry^>^ get(void){
                    return this->list->GetView();
                }
            };

            DeviceList(Platform::String^ deviceSelector);

        private:
            static Platform::Collections::Vector<DeviceList^>^ instances;
            Windows::Devices::Enumeration::DeviceWatcher^ watcher;
            Platform::Collections::Vector<DeviceListEntry^>^ list;
            const Platform::String^ deviceSelector;

            bool watcherSuspended;
            bool watcherStarted;

            DeviceListEntry^ FindDevice(Platform::String^ Id);

            void InitDeviceWatcher();

            void SuspendDeviceWatcher(Object^ Sender, Windows::ApplicationModel::SuspendingEventArgs^ Args);
            void ResumeDeviceWatcher(Object^ Sender, Object^ Args);

            void OnAdded(Windows::Devices::Enumeration::DeviceWatcher^ Sender, Windows::Devices::Enumeration::DeviceInformation^ DevInformation);
            void OnRemoved(Windows::Devices::Enumeration::DeviceWatcher^ Sender, Windows::Devices::Enumeration::DeviceInformationUpdate^ DevInformation);
            void OnEnumerationComplete(Windows::Devices::Enumeration::DeviceWatcher^ Sender, Object^ Args);
        };

    }
}