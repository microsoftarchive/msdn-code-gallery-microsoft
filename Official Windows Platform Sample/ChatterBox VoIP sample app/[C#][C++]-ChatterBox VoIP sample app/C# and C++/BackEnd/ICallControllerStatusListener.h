/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
#pragma once

#include <windows.phone.networking.voip.h>

namespace PhoneVoIPApp
{
    namespace BackEnd
    {
        // The status of a call
        public enum class CallStatus
        {
            None = 0x00,
            InProgress,
            Held
        };

        // Where is the call audio going?
        public enum class CallAudioRoute
        {
            None            = Windows::Phone::Media::Devices::AvailableAudioRoutingEndpoints::None,
            Earpiece        = Windows::Phone::Media::Devices::AvailableAudioRoutingEndpoints::Earpiece,
            Speakerphone    = Windows::Phone::Media::Devices::AvailableAudioRoutingEndpoints::Speakerphone,
            Bluetooth       = Windows::Phone::Media::Devices::AvailableAudioRoutingEndpoints::Bluetooth
        };

        // Which camera are we using?
        public enum class CameraLocation
        {
            Front = Windows::Phone::Media::Capture::CameraSensorLocation::Front,
            Back = Windows::Phone::Media::Capture::CameraSensorLocation::Back
        };

        // Used to indicate the status of video/audio capture/render
        public enum class MediaOperations
        {
            None                = 0x00,
            VideoCapture        = 0x01,
            VideoRender         = 0x02,
            AudioCapture        = 0x04,
            AudioRender         = 0x08
        };

        // An interface that is used by the call controller to deliver status change notifications.
        // This interface is meant to be implemented in the UI process, and will be called back by
        // the agent host process using out-of-process WinRT.
        public interface class ICallControllerStatusListener
        {
            // The methods below are just for illustration purposes - add your own methods here

            // The status of a call has changed.
            void OnCallStatusChanged(CallStatus newStatus);

            // The call audio route has changed. Also called when the available audio routes have changed.
            void OnCallAudioRouteChanged(CallAudioRoute newRoute);

            // Video/audio capture/render has started/stopped
            void OnMediaOperationsChanged(MediaOperations newOperations);

            // Camera location has changed
            void OnCameraLocationChanged(CameraLocation newCameraLocation);
        };
    }
}
