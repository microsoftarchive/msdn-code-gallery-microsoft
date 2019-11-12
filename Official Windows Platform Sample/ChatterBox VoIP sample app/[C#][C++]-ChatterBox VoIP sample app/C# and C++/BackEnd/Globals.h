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

#include <roapi.h>
#include "IVideoRenderer.h"

namespace PhoneVoIPApp
{
    namespace BackEnd
    {
        // Forward declarations
        ref class CallController;
        ref class BackEndAudio;
        ref class BackEndTransport;
        ref class BackEndCapture;
        
        // A singleton container that is used to hold other global singletons and background process-wide static state.
        // Another purpose of this class is to start the out-of-process WinRT server, so that the UI process
        // managed code can instantiate WinRT objects in this process.
        public ref class Globals sealed
        {
        public:
            // Start the out-of-process WinRT server, so that the UI process can instantiate WinRT objects in this process.
            void StartServer(const Platform::Array<Platform::String^>^ outOfProcServerClassNames);

            // Do some app-specific periodic tasks, to let the remote server know that this endpoint is still alive.
            void DoPeriodicKeepAlive();

            // Get the process id of the current process
            static unsigned int GetCurrentProcessId();

            // Get the name of the event that indicates if the UI is connected to the background process or not
            static Platform::String^ GetUiDisconnectedEventName(unsigned int backgroundProcessId);

            // Get the name of the event that indicates if the background process is ready or not
            static Platform::String^ GetBackgroundProcessReadyEventName(unsigned int backgroundProcessId);

            // Get the single instance of this class
            static property Globals^ Instance
            {
                Globals^ get();
            }

            // Get the call controller singleton object
            property PhoneVoIPApp::BackEnd::CallController^ CallController
            {
                PhoneVoIPApp::BackEnd::CallController^ get();
            }

            // The singleton video renderer object.
            property IVideoRenderer^ VideoRenderer
            {
                IVideoRenderer^ get();
                void set(IVideoRenderer^ value);
            }

            // The singleton audio controller object.
            property BackEndAudio^ AudioController
            {
                BackEndAudio^ get();
            }

            // The singleton audio controller object.
            property BackEndCapture^ CaptureController
            {
                BackEndCapture^ get();
            }

            // The singleton transport object.
            property BackEndTransport^ TransportController
            {
                BackEndTransport^ get();
            }

        private:
            // Default constructor
            Globals();

            // Destructor
            ~Globals();

            // Name of the event that indicates if another instance of the VoIP background process exists or not
            static const LPCWSTR noOtherBackgroundProcessEventName;

            // Name of the event that indicates if the UI is connected to the background process or not
            static const LPCWSTR uiDisconnectedEventName;

            // Name of the event that indicates if the background process is ready or not
            static const LPCWSTR backgroundProcessReadyEventName;

            // The single instance of this class
            static Globals^ singleton;

            // Indicates if the out-of-process server has started or not
            bool started;

            // A cookie that is used to unregister remotely activatable objects in this process
            RO_REGISTRATION_COOKIE serverRegistrationCookie;

            // An event that indicates if another instance of the background process exists or not
            HANDLE noOtherBackgroundProcessEvent;

            // An event that indicates that the background process is ready
            HANDLE backgroundReadyEvent;

            // The call controller object
            PhoneVoIPApp::BackEnd::CallController^ callController;

            // The video renderer object
            PhoneVoIPApp::BackEnd::IVideoRenderer^ videoRenderer;

            // The audio capture/render controller
            BackEndAudio^ audioController;

            // The audio capture/render controller
            BackEndCapture^ captureController;

            // The data transport
            BackEndTransport^ transportController;
        };
    }
}
