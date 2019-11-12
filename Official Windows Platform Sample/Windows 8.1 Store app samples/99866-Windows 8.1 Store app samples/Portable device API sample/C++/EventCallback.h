// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// EventCallback.h
// Declaration of the EventCallback class.
//

#pragma once

#include <wrl\implements.h>

namespace SDKSample
{
    namespace PortableDeviceCPP
    {
        // Delegate used by MainPage to be notified of events
        public delegate void EventReceived(Platform::String^ deviceId, Platform::String^ eventData);

        // IPortableDeviceEventCallback implementation for use with device events.
        class EventCallback : public Microsoft::WRL::RuntimeClass<
            Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::RuntimeClassType::ClassicCom>,
            IPortableDeviceEventCallback>
        {
        public:
            EventCallback(EventReceived^ eventReceived) : _eventReceived(eventReceived) {}

            ~EventCallback() {}

            IFACEMETHODIMP OnEvent(_In_ IPortableDeviceValues* eventParameters)
            {
                if (eventParameters != nullptr && _eventReceived != nullptr)
                {
                    // Extract event data. For simplicity, we'll just display the event type and some basic data.
                    GUID eventId;
                    if (SUCCEEDED(eventParameters->GetGuidValue(WPD_EVENT_PARAMETER_EVENT_ID, &eventId)))
                    {
                        Platform::String^ eventData;
                        if (eventId == WPD_EVENT_OBJECT_ADDED)
                        {
                            eventData = "Object Added";
                            PWSTR newObjectId = nullptr;
                            if (SUCCEEDED(eventParameters->GetStringValue(WPD_OBJECT_ID, &newObjectId)))
                            {
                                CoTaskMemString newObjectIdPtr(newObjectId);
                                eventData += ", Object id: " + ref new Platform::String(newObjectId);
                            }
                        }
                        else if (eventId == WPD_EVENT_OBJECT_REMOVED)
                        {
                            eventData = "Object Removed";
                            PWSTR removedObjectId = nullptr;
                            if (SUCCEEDED(eventParameters->GetStringValue(WPD_OBJECT_ID, &removedObjectId)))
                            {
                                CoTaskMemString removedObjectIdPtr(removedObjectId);
                                eventData += ", Object id: " + ref new Platform::String(removedObjectId);
                            }
                        }
                        else if (eventId == WPD_EVENT_OBJECT_UPDATED)
                        {
                            eventData = "Object Updated";
                            PWSTR updatedObjectId = nullptr;
                            if (SUCCEEDED(eventParameters->GetStringValue(WPD_OBJECT_ID, &updatedObjectId)))
                            {
                                CoTaskMemString updatedObjectIdPtr(updatedObjectId);
                                eventData += ", Object id: " + ref new Platform::String(updatedObjectId);
                            }
                        }
                        else
                        {
                            // Handle more events as needed
                            eventData = "Other event";
                        }

                        PWSTR deviceId = nullptr;
                        if (SUCCEEDED(eventParameters->GetStringValue(WPD_EVENT_PARAMETER_PNP_DEVICE_ID, &deviceId)))
                        {
                            //  Invoke the delegate
                            CoTaskMemString deviceIdPtr(deviceId);
                            _eventReceived(ref new Platform::String(deviceId), eventData);
                        }
                    }
                }
                return S_OK;
            }

        private:
            EventReceived^ _eventReceived;
        };
    }
}
