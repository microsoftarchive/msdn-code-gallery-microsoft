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
#include "BackEndCapture.h"
#include <windows.phone.networking.voip.h>
#include "ICallControllerStatusListener.h"
#include "ApiLock.h"

namespace PhoneVoIPApp
{
    namespace BackEnd
    {
        // Forward declaration
        ref class Globals;

        // A method that is called back when the incoming call dialog has been dismissed.
        // This callback is used to complete the incoming call agent.
        public delegate void IncomingCallDialogDismissedCallback();

        // A class that provides methods and properties related to VoIP calls.
        // It wraps Windows.Phone.Networking.Voip.VoipCallCoordinator, and provides app-specific call functionality.
        public ref class CallController sealed
        {
        public:
            // The public methods below are just for illustration purposes - add your own methods here

            // Provide an inteface that can be used to get call controller status change notifications
            void SetStatusCallback(ICallControllerStatusListener^ statusListener);

            // Initiate an outgoing call. Called by the UI process.
            // Returns true if the outgoing call processing was started, false otherwise.
            bool InitiateOutgoingCall(Platform::String^ recepientName, Platform::String^ recepientNumber);

            // Start processing an incoming call. Called by managed code in this process (the VoIP agent host process).
            // Returns true if the incoming call processing was started, false otherwise.
            bool OnIncomingCallReceived(Platform::String^ contactName, Platform::String^ contactNumber, IncomingCallDialogDismissedCallback^ incomingCallDialogDismissedCallback);

            // Hold a call. Called by the UI process.
            bool HoldCall(); 

            // Resume a call. Called by the UI process.
            bool ResumeCall(); 

            // End a call. Called by the UI process.
            bool EndCall();

            // Toggle the camera location. Called by the UI process.
            bool ToggleCamera();

            // Get the call state
            property PhoneVoIPApp::BackEnd::CallStatus CallStatus
            {
                PhoneVoIPApp::BackEnd::CallStatus get();
            };

            // Get or set the media operations that are allowed for a call.
            property PhoneVoIPApp::BackEnd::MediaOperations MediaOperations
            {
                PhoneVoIPApp::BackEnd::MediaOperations get();
            }

            // Indicates if video is currently being displayed or not
            property bool IsShowingVideo
            {
                bool get();
                void set(bool value);
            }
            
            // Indicates whether the video is being rendered or not.
            property bool IsRenderingVideo
            {
                bool get();
                void set(bool value);
            }

            // Get the current camera location
            property PhoneVoIPApp::BackEnd::CameraLocation CameraLocation
            {
                PhoneVoIPApp::BackEnd::CameraLocation get();
            }

            // Get the possible routes for audio
            property CallAudioRoute AvailableAudioRoutes
            {
                CallAudioRoute get();
            }

            // Get or set the current route for audio
            property CallAudioRoute AudioRoute
            {
                CallAudioRoute get();
                void set(CallAudioRoute newRoute);
            }

            // Get the name of the other party in the most recent call.
            // Can return nullptr if there hasn't been a call yet.
            property Platform::String^ OtherPartyName
            {
                Platform::String^ get();
            }

            // Get the name of the other party in the most recent call.
            // Can return nullptr if there hasn't been a call yet.
            property Platform::String^ OtherPartyNumber
            {
                Platform::String^ get();
            }

            // Get call start time
            property Windows::Foundation::DateTime CallStartTime
            {
                Windows::Foundation::DateTime get();
            }

        private:
            // Only the server can create an instance of this object
            friend ref class PhoneVoIPApp::BackEnd::Globals;

            // Constructor and destructor
            CallController();
            ~CallController();

            // Set the call status
            void SetCallStatus(PhoneVoIPApp::BackEnd::CallStatus newStatus);

            // Indicates that a call is now active 
            void SetActiveCall(Windows::Phone::Networking::Voip::VoipPhoneCall^ call, Platform::String^ number, Windows::Phone::Networking::Voip::VoipCallMedia callMedia);

            // Called by the VoipCallCoordinator when the user accepts an incoming call.
            void OnAcceptCallRequested(Windows::Phone::Networking::Voip::VoipPhoneCall^ sender, Windows::Phone::Networking::Voip::CallAnswerEventArgs^ args);

            // Called by the VoipCallCoordinator when the user rejects an incoming call.
            void OnRejectCallRequested(Windows::Phone::Networking::Voip::VoipPhoneCall^ sender, Windows::Phone::Networking::Voip::CallRejectEventArgs^ args);

            // Called by the VoipCallCoordinator when a call is to be put on hold.
            void OnHoldCallRequested(Windows::Phone::Networking::Voip::VoipPhoneCall^ sender, Windows::Phone::Networking::Voip::CallStateChangeEventArgs^ args);

            // Called by the VoipCallCoordinator when a call that was previously put on hold is to be resumed.
            void OnResumeCallRequested(Windows::Phone::Networking::Voip::VoipPhoneCall^ sender, Windows::Phone::Networking::Voip::CallStateChangeEventArgs^ args);

            // Called by the VoipCallCoordinator when a call is to be ended.
            void OnEndCallRequested(Windows::Phone::Networking::Voip::VoipPhoneCall^ sender, Windows::Phone::Networking::Voip::CallStateChangeEventArgs^ args);

            // Called by the AudioRoutingManager when call audio routing changes.
            void OnAudioEndpointChanged(Windows::Phone::Media::Devices::AudioRoutingManager^ sender, Platform::Object^ args);

            // Called by the BackEndCapture when the camera is toggled
            void OnCameraLocationChanged(PhoneVoIPApp::BackEnd::CameraLocation newCameraLocation);

            // Set a value that indicates if video/audio capture/render is enabled for a call or not.
            void SetMediaOperations(PhoneVoIPApp::BackEnd::MediaOperations value);

            // Start/stop video/audio capture/playback based on the current state.
            void UpdateMediaOperations();

            // The relative URI to the call-in-progress page
            Platform::String ^callInProgressPageUri;

            // The name of this service provider
            Platform::String^ voipServiceName;

            // The URI to the default contact image
            Windows::Foundation::Uri^ defaultContactImageUri;

            // The URI to the branding image
            Windows::Foundation::Uri^ brandingImageUri;

            // The URI to the ringtone file
            Windows::Foundation::Uri^ ringtoneUri;

            // Interface used to deliver status callbacks
            ICallControllerStatusListener^ statusListener;

            // A VoIP call that is in progress
            Windows::Phone::Networking::Voip::VoipPhoneCall^ activeCall;

            // The status of a call, if any
            PhoneVoIPApp::BackEnd::CallStatus callStatus;

            // The name of the other party, if any
            Platform::String^ otherPartyName;

            // The number of the other party, if any
            Platform::String^ otherPartyNumber;

            // The number of the caller for the latest incoming call
            Platform::String^ incomingNumber;

            // Indicates if video/audio capture/render is enabled for a call or not.
            PhoneVoIPApp::BackEnd::MediaOperations mediaOperations;

            PhoneVoIPApp::BackEnd::CameraLocation cameraLocation;

            // Indicates if video is currently being displayed or not.
            bool isShowingVideo;

            bool isRenderingVideo;

            // The method to be called when the incoming call dialog box is dismissed
            IncomingCallDialogDismissedCallback^ onIncomingCallDialogDismissed;

            // The VoIP call coordinator
            Windows::Phone::Networking::Voip::VoipCallCoordinator^ callCoordinator;

            // The phone audio routing manager
            Windows::Phone::Media::Devices::AudioRoutingManager^ audioRoutingManager;

            // Phone call related event handlers
            Windows::Foundation::TypedEventHandler<Windows::Phone::Networking::Voip::VoipPhoneCall^, Windows::Phone::Networking::Voip::CallAnswerEventArgs^>^ acceptCallRequestedHandler;
            Windows::Foundation::TypedEventHandler<Windows::Phone::Networking::Voip::VoipPhoneCall^, Windows::Phone::Networking::Voip::CallRejectEventArgs^>^ rejectCallRequestedHandler;
            Windows::Foundation::TypedEventHandler<Windows::Phone::Networking::Voip::VoipPhoneCall^, Windows::Phone::Networking::Voip::CallStateChangeEventArgs^>^ holdCallRequestedHandler;
            Windows::Foundation::TypedEventHandler<Windows::Phone::Networking::Voip::VoipPhoneCall^, Windows::Phone::Networking::Voip::CallStateChangeEventArgs^>^ resumeCallRequestedHandler;
            Windows::Foundation::TypedEventHandler<Windows::Phone::Networking::Voip::VoipPhoneCall^, Windows::Phone::Networking::Voip::CallStateChangeEventArgs^>^ endCallRequestedHandler;

            // Audio related event handlers
            Windows::Foundation::TypedEventHandler<Windows::Phone::Media::Devices::AudioRoutingManager^, Platform::Object^>^ audioEndpointChangedHandler;

            // A cookie used to un-register the audio endpoint changed handler
            Windows::Foundation::EventRegistrationToken audioEndpointChangedHandlercookie;

            // Camera location related event handlers
            CameraLocationChangedEventHandler^ cameraLocationChangedHandler;
        };
    }
}
