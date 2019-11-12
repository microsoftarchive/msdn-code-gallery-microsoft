/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
#include "CallController.h"
#include "BackEndAudio.h"
#include "BackEndCapture.h"
#include "Server.h"

using namespace PhoneVoIPApp::BackEnd;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Phone::Media::Devices;
using namespace Windows::Phone::Networking::Voip;

void CallController::SetStatusCallback(ICallControllerStatusListener^ statusListener)
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    this->statusListener = statusListener;
}

bool CallController::InitiateOutgoingCall(Platform::String^ recepientName, Platform::String^ recepientNumber)
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    VoipPhoneCall^ outgoingCall = nullptr;

    // In this sample, we allow only one call at a time.
    if (this->activeCall != nullptr)
    {
        ::OutputDebugString(L"[CallController::InitiateOutgoingCall] => Only one active call allowed in this sample at a time\n");

        // If we receive a request to initiate an outgoing call when another call is in progress,
        // we just ignore it. 
        return false;
    }

    ::OutputDebugString(L"[CallController::InitiateOutgoingCall] => Starting outgoing call\n");

    // Start a new outgoing call.
    this->callCoordinator->RequestNewOutgoingCall(this->callInProgressPageUri, recepientName, "VoIP Chatterbox", VoipCallMedia::Audio | VoipCallMedia::Video, &outgoingCall);


    // Tell the phone service that this call is active.
    // Normally, we do this only when the remote party has accepted the call.
    outgoingCall->NotifyCallActive();

    // Store it as the active call - assume we support both audio and video
    this->SetActiveCall(outgoingCall, recepientNumber, VoipCallMedia::Audio | VoipCallMedia::Video);

    return true;
}

bool CallController::OnIncomingCallReceived(Platform::String^ contactName, Platform::String^ contactNumber, IncomingCallDialogDismissedCallback^ incomingCallDialogDismissedCallback)
{
    VoipPhoneCall^ incomingCall = nullptr;

    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    // TODO: If required, contact your cloud service here for more information about the incoming call.

    try
    {
        TimeSpan timeout;
        timeout.Duration = 90 * 10 * 1000 * 1000; // in 100ns units

        ::OutputDebugString(L"[CallController::OnIncomingCallReceived] => Will time out in 90 seconds\n");

        // Store the caller number of this incoming call
        this->incomingNumber = contactNumber;

        // Store the callback that needs to be called when the incoming call dialog has been dismissed,
        // either because the call was accepted or rejected by the user.
        this->onIncomingCallDialogDismissed = incomingCallDialogDismissedCallback;

        // Ask the Phone Service to start a new incoming call
        this->callCoordinator->RequestNewIncomingCall(
            this->callInProgressPageUri,
            contactName,
            contactNumber,
            this->defaultContactImageUri,
            this->voipServiceName,
            this->brandingImageUri,
            "",                      // Was this call forwarded/delegated to this user on behalf of someone else? At this time, we won't use this field
            this->ringtoneUri,
            VoipCallMedia::Audio | VoipCallMedia::Video,
            timeout,                // Maximum amount of time to ring for
            &incomingCall);
    }
    catch(...)
    {
        // Requesting an incoming call can fail if there is already an incoming call in progress.
        // This is rare, but possible. Treat this case like a missed call.
        ::OutputDebugString(L"[CallController::OnIncomingCallReceived] => An exception has occurred\n");
        return false;
    }

    // Register for events about this incoming call.
    incomingCall->AnswerRequested += this->acceptCallRequestedHandler;
    incomingCall->RejectRequested += this->rejectCallRequestedHandler;

    return true;
}

bool CallController::HoldCall()
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    if (this->activeCall == nullptr)
    {
        // Nothing to do - there is no call to put on hold
        return false;
    }

    ::OutputDebugString(L"[CallController::HoldCall] => Trying to put call on hold\n");

    // Change the call status before notifying that the call is held because
    // access to the camera will be removed once NotifyCallHeld is called
    this->SetCallStatus(PhoneVoIPApp::BackEnd::CallStatus::Held);

    // Hold the active call
    this->activeCall->NotifyCallHeld();

    // TODO: Contact your cloud service and let it know that the active call has been put on hold.

    return true;
}

bool CallController::ResumeCall()
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    if (this->activeCall == nullptr)
    {
        // Nothing to do - there is no call to resume
        return false;
    }

    ::OutputDebugString(L"[CallController::ResumeCall] => Trying to resume a call\n");

    // Resume the active call
    this->activeCall->NotifyCallActive();

    // TODO: Contact your cloud service and let it know that the active call has been resumed.

    // Change the call status after notifying that the call is active
    // if it is done before access to the camera will not have been granted yet
    this->SetCallStatus(PhoneVoIPApp::BackEnd::CallStatus::InProgress);

    return true;
}

bool CallController::EndCall()
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    if (this->activeCall == nullptr)
    {
        // Nothing to do - there is no call to end
        return false;
    }

    ::OutputDebugString(L"[CallController::EndCall] => Trying to end a call\n");

    // Unregister from audio endpoint changes
    this->audioRoutingManager->AudioEndpointChanged -= this->audioEndpointChangedHandlercookie;

    // TODO: Contact your cloud service and let it know that the active call has ended.

    // Change the call status
    this->SetCallStatus(PhoneVoIPApp::BackEnd::CallStatus::None);

    // End the active call.
    this->activeCall->NotifyCallEnded();
    this->activeCall = nullptr;
    
    // Reset camera choice to front facing for next call
    this->cameraLocation = PhoneVoIPApp::BackEnd::CameraLocation::Front;

    return true;
}

bool CallController::ToggleCamera()
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    if (this->activeCall == nullptr)
    {
        // Nothing to do - there is no call to end
        return false;
    }
    ::OutputDebugString(L"[CallController::ToggleCamera] => Trying to toggle the camera\n");

    Globals::Instance->CaptureController->ToggleCamera();

    return true;
}

PhoneVoIPApp::BackEnd::CallStatus CallController::CallStatus::get()
{
    // No need to lock - this get is idempotent
    return this->callStatus;
}

PhoneVoIPApp::BackEnd::CameraLocation CallController::CameraLocation::get()
{
    // No need to lock - this get is idempotent
    return this->cameraLocation;
}

PhoneVoIPApp::BackEnd::MediaOperations CallController::MediaOperations::get()
{
    // No need to lock - this get is idempotent
    return this->mediaOperations;
}

bool CallController::IsShowingVideo::get()
{
    // No need to lock - this get is idempotent
    return this->isShowingVideo;
}

void CallController::IsShowingVideo::set(bool value)
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    // Has anything changed?
    if (this->isShowingVideo == value)
        return; // No

    // Update the value
    this->isShowingVideo = value;

    // Start/stop video capture/render based on this change
    this->UpdateMediaOperations();
}

bool CallController::IsRenderingVideo::get()
{
    // No need to lock - this get is idempotent
    return this->isRenderingVideo;
}

void CallController::IsRenderingVideo::set(bool value)
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    // Has anything changed?
    if (this->isRenderingVideo == value)
        return; // No

    // Update the value
    this->isRenderingVideo = value;

    // Start/stop video capture/render based on this change
    this->UpdateMediaOperations();
}

CallAudioRoute CallController::AvailableAudioRoutes::get()
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    if (this->activeCall == nullptr)
    {
        // There is no call in progress
        return CallAudioRoute::None;
    }

    return (CallAudioRoute)(this->audioRoutingManager->AvailableAudioEndpoints);
}

CallAudioRoute CallController::AudioRoute::get()
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    if (this->activeCall == nullptr)
    {
        // There is no call in progress
        return CallAudioRoute::None;
    }

    auto audioEndpoint = this->audioRoutingManager->GetAudioEndpoint();
    switch(audioEndpoint)
    {
    case AudioRoutingEndpoint::Earpiece:
    case AudioRoutingEndpoint::WiredHeadset:
    case AudioRoutingEndpoint::WiredHeadsetSpeakerOnly:
        return CallAudioRoute::Earpiece;

    case AudioRoutingEndpoint::Default:
    case AudioRoutingEndpoint::Speakerphone:
        return CallAudioRoute::Speakerphone;

    case AudioRoutingEndpoint::Bluetooth:
    case AudioRoutingEndpoint::BluetoothWithNoiseAndEchoCancellation:
        return CallAudioRoute::Bluetooth;

    default:
        throw ref new FailureException("Unexpected audio routing endpoint");
    }

}

void CallController::AudioRoute::set(CallAudioRoute newRoute)
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    if (this->callStatus != PhoneVoIPApp::BackEnd::CallStatus::InProgress)
    {
        // There is no call in progress - do nothing
        return;
    }

    switch(newRoute)
    {
    case CallAudioRoute::Earpiece:
        this->audioRoutingManager->SetAudioEndpoint(AudioRoutingEndpoint::Earpiece);
        break;

    case CallAudioRoute::Speakerphone:
        this->audioRoutingManager->SetAudioEndpoint(AudioRoutingEndpoint::Speakerphone);
        break;

    case CallAudioRoute::Bluetooth:
        this->audioRoutingManager->SetAudioEndpoint(AudioRoutingEndpoint::Bluetooth);
        break;

    case CallAudioRoute::None:
    default:
        throw ref new FailureException("Cannot set audio route to CallAudioRoute::None");
    }
}

Platform::String^ CallController::OtherPartyName::get()
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);
    return this->otherPartyName;
}

Platform::String^ CallController::OtherPartyNumber::get()
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);
    return this->otherPartyNumber;
}

Windows::Foundation::DateTime CallController::CallStartTime::get()
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    if (this->activeCall != nullptr)
    {
        // There is a call in progress
        return this->activeCall->StartTime;
    }
    else
    {
        // There is no call in progress
        Windows::Foundation::DateTime minValue;
        minValue.UniversalTime = 0;
        return minValue;
    }
}

CallController::CallController() :
    callInProgressPageUri(L"/CallStatusPage.xaml"),
    voipServiceName(nullptr),
    defaultContactImageUri(nullptr),
    brandingImageUri(nullptr),
    ringtoneUri(nullptr),
    statusListener(nullptr),
    callStatus(PhoneVoIPApp::BackEnd::CallStatus::None),
    otherPartyName(nullptr),
    otherPartyNumber(nullptr),
    incomingNumber(nullptr),
    mediaOperations(PhoneVoIPApp::BackEnd::MediaOperations::None),
    onIncomingCallDialogDismissed(nullptr),
    activeCall(nullptr),
    cameraLocation(PhoneVoIPApp::BackEnd::CameraLocation::Front)
{
    this->callCoordinator = VoipCallCoordinator::GetDefault();
    this->audioRoutingManager = AudioRoutingManager::GetDefault();

    // URIs required for interactions with the VoipCallCoordinator
    String^ installFolder = String::Concat(Windows::ApplicationModel::Package::Current->InstalledLocation->Path, "\\");
    this->defaultContactImageUri = ref new Uri(installFolder, "Assets\\DefaultContactImage.png");
    this->voipServiceName = ref new String(L"PhoneVoIPApp");
    this->brandingImageUri = ref new Uri(installFolder, "Assets\\ApplicationIcon.png");
    this->ringtoneUri = ref new Uri(installFolder, "Assets\\Ringtone.wma");

    // Event handler delegates - creating them once and storing them as member variables
    // avoids having to create new delegate objects for each phone call.
    this->acceptCallRequestedHandler = ref new TypedEventHandler<VoipPhoneCall^, CallAnswerEventArgs^>(this, &CallController::OnAcceptCallRequested);
    this->rejectCallRequestedHandler = ref new TypedEventHandler<VoipPhoneCall^, CallRejectEventArgs^>(this, &CallController::OnRejectCallRequested);
    this->holdCallRequestedHandler = ref new TypedEventHandler<VoipPhoneCall^, CallStateChangeEventArgs^>(this, &CallController::OnHoldCallRequested);
    this->resumeCallRequestedHandler = ref new TypedEventHandler<VoipPhoneCall^, CallStateChangeEventArgs^>(this, &CallController::OnResumeCallRequested);
    this->endCallRequestedHandler = ref new TypedEventHandler<VoipPhoneCall^, CallStateChangeEventArgs^>(this, &CallController::OnEndCallRequested);
    this->audioEndpointChangedHandler = ref new TypedEventHandler<AudioRoutingManager^, Object^>(this, &CallController::OnAudioEndpointChanged);
    this->cameraLocationChangedHandler = ref new CameraLocationChangedEventHandler(this, &CallController::OnCameraLocationChanged);
}

CallController::~CallController()
{
}

void CallController::SetCallStatus(PhoneVoIPApp::BackEnd::CallStatus newStatus)
{
    // No need to lock - private method

    if (newStatus == this->callStatus)
        return; // Nothing more to do

    // Change the call status
    this->callStatus = newStatus;

    // Update audio/video capture/render status, if required.
    this->UpdateMediaOperations();

    // If required, let the UI know.
    if (this->statusListener != nullptr)
    {
        this->statusListener->OnCallStatusChanged(this->callStatus);
    }
}

void CallController::SetActiveCall(VoipPhoneCall^ call, Platform::String^ number, VoipCallMedia callMedia)
{
    // No need to lock - private method

    // The specified call is now active.
    // For an incoming call, this means that the local party has accepted the call.
    // For an outoing call, this means that the remote party has accepted the call.
    this->activeCall = call;

    // Listen to state changes of the active call.
    call->HoldRequested += this->holdCallRequestedHandler;
    call->ResumeRequested += this->resumeCallRequestedHandler;
    call->EndRequested += this->endCallRequestedHandler;

    // Register for audio endpoint changes
    this->audioEndpointChangedHandlercookie = this->audioRoutingManager->AudioEndpointChanged += this->audioEndpointChangedHandler;

    // Store information about the other party in the call
    this->otherPartyName = this->activeCall->ContactName;
    this->otherPartyNumber = number;

    // Change the call status
    this->SetCallStatus(PhoneVoIPApp::BackEnd::CallStatus::InProgress);

    // Figure out if video/audio capture/render are all allowed
    PhoneVoIPApp::BackEnd::MediaOperations newOperations = PhoneVoIPApp::BackEnd::MediaOperations::None;
    if ((callMedia & VoipCallMedia::Audio) != VoipCallMedia::None)
    {
        // Enable both audio capture and render by default
        newOperations = PhoneVoIPApp::BackEnd::MediaOperations::AudioCapture | PhoneVoIPApp::BackEnd::MediaOperations::AudioRender;
    }
    if ((callMedia & VoipCallMedia::Video) != VoipCallMedia::None)
    {
        // Enable both video capture and render by default
        newOperations = newOperations | PhoneVoIPApp::BackEnd::MediaOperations::VideoCapture | PhoneVoIPApp::BackEnd::MediaOperations::VideoRender;

    }
    this->SetMediaOperations(newOperations);
}

void CallController::OnAcceptCallRequested(VoipPhoneCall^ sender, CallAnswerEventArgs^ args)
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    ::OutputDebugString(L"[CallController::OnAcceptCallRequested] => Incoming call accepted\n");

    // The local user has accepted an incoming call.
    VoipPhoneCall^ incomingCall = (VoipPhoneCall^)sender;

    // If there is was a call already in progress, end it
    // As of now, we support only one call at a time in this application
    this->EndCall();

    // Reset camera choice to front facing for next call
    this->cameraLocation = PhoneVoIPApp::BackEnd::CameraLocation::Front;

    // The incoming call is the new active call.
    incomingCall->NotifyCallActive();

    // Let the incoming call agent know that incoming call processing is now complete
    if (this->onIncomingCallDialogDismissed != nullptr)
        this->onIncomingCallDialogDismissed();

    // Store it as the active call.
    this->SetActiveCall(incomingCall, this->incomingNumber, args->AcceptedMedia);

}

void CallController::OnRejectCallRequested(VoipPhoneCall^ sender, CallRejectEventArgs^ args)
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    ::OutputDebugString(L"[CallController::OnRejectCallRequested] => Incoming call rejected\n");

    // The local user has rejected an incoming call.
    VoipPhoneCall^ incomingCall = (VoipPhoneCall^)sender;

    // End it.
    incomingCall->NotifyCallEnded();

    // TODO: Contact your cloud service and let it know that the incoming call was rejected.

    // Finally, let the incoming call agent know that incoming call processing is now complete
    this->onIncomingCallDialogDismissed();
}

void CallController::OnHoldCallRequested(VoipPhoneCall^ sender, CallStateChangeEventArgs^ args)
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    // A request to put the active call on hold has been received.
    VoipPhoneCall^ callToPutOnHold = (VoipPhoneCall^)sender;

    // Sanity test.
    if (callToPutOnHold != this->activeCall)
    {
        throw ref new Platform::FailureException(L"Something is wrong. The call to put on hold is not the active call");
    }

    // Put the active call on hold.
    this->HoldCall();
}

void CallController::OnResumeCallRequested(VoipPhoneCall^ sender, CallStateChangeEventArgs^ args)
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    // A request to resumed the active call has been received.
    VoipPhoneCall^ callToResume = (VoipPhoneCall^)sender;

    // Sanity test.
    if (callToResume != this->activeCall)
    {
        throw ref new Platform::FailureException(L"Something is wrong. The call to resume is not the active call");
    }

    // Resume the active call
    this->ResumeCall();
}

void CallController::OnEndCallRequested(VoipPhoneCall^ sender, CallStateChangeEventArgs^ args)
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    // A request to end the active call has been received.
    VoipPhoneCall^ callToEnd = (VoipPhoneCall^)sender;

    // Sanity test.
    if (callToEnd != this->activeCall)
    {
        throw ref new Platform::FailureException(L"Something is wrong. The call to end is not the active call");
    }

    // End the active call
    this->EndCall();
}

void CallController::OnAudioEndpointChanged(AudioRoutingManager^ sender, Object^ args)
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    // If required, let the UI know.
    if ((this->activeCall != nullptr) && (this->statusListener != nullptr))
    {
        this->statusListener->OnCallAudioRouteChanged(this->AudioRoute);
    }
}

void CallController::OnCameraLocationChanged(PhoneVoIPApp::BackEnd::CameraLocation newCameraLocation)
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    if(this->cameraLocation == newCameraLocation || this->activeCall == nullptr)
    {
        // nothing to do
        return;
    }

    this->cameraLocation = newCameraLocation;

    // If required, let the UI know.
    if ((this->statusListener != nullptr))
    {
        this->statusListener->OnCameraLocationChanged(this->cameraLocation);
    }

}

void CallController::SetMediaOperations(PhoneVoIPApp::BackEnd::MediaOperations value)
{
    // No need to lock - private method

    // Has anything changed?
    if (this->mediaOperations == value)
        return; // No

    // Update the value
    this->mediaOperations = value;

    // Start/stop video/audio capture based on this change
    this->UpdateMediaOperations();
}

void CallController::UpdateMediaOperations()
{
    // No need to lock - private method

    bool captureAudio = false, captureVideo = false, renderAudio = false, renderVideo = false;

    if (this->callStatus == PhoneVoIPApp::BackEnd::CallStatus::InProgress)
    {
        // A call is in progress

        // Start audio capture/render, if enabled
        captureAudio = ((this->mediaOperations & PhoneVoIPApp::BackEnd::MediaOperations::AudioCapture) != PhoneVoIPApp::BackEnd::MediaOperations::None);
        renderAudio = ((this->mediaOperations & PhoneVoIPApp::BackEnd::MediaOperations::AudioRender) != PhoneVoIPApp::BackEnd::MediaOperations::None);

        // Start video capture/render if enabled *and* the UI is showing video.
        if (this->isShowingVideo)
        {
            if(isRenderingVideo)
            {
                renderVideo = ((this->mediaOperations & PhoneVoIPApp::BackEnd::MediaOperations::VideoRender) != PhoneVoIPApp::BackEnd::MediaOperations::None);
            }

            // Does this phone have a camera?
            auto availableCameras = Windows::Phone::Media::Capture::AudioVideoCaptureDevice::AvailableSensorLocations;
            bool isCameraPresent = ((availableCameras != nullptr) && (availableCameras->Size > 0));

            // Start capture only if there is a camera present
            if (isCameraPresent)
            {
                captureVideo = ((this->mediaOperations & PhoneVoIPApp::BackEnd::MediaOperations::VideoCapture) != PhoneVoIPApp::BackEnd::MediaOperations::None);
            }
        }
    }
    // else: call is not in progress - all capture/rendering should stop

    // What are the new media operations?
    PhoneVoIPApp::BackEnd::MediaOperations newOperations = PhoneVoIPApp::BackEnd::MediaOperations::None;

    // Start/stop audio capture and render
    if (captureAudio || renderAudio)
    {
        ::OutputDebugString(L"[CallController::UpdateMediaOperations] => Starting audio\n");
        newOperations = newOperations | (PhoneVoIPApp::BackEnd::MediaOperations::AudioCapture | PhoneVoIPApp::BackEnd::MediaOperations::AudioRender);
        Globals::Instance->AudioController->Start();
    }
    else
    {
        ::OutputDebugString(L"[CallController::UpdateMediaOperations] => Stopping audio\n");
        Globals::Instance->AudioController->Stop();
    }

    // Start/stop video render
    if (Globals::Instance->VideoRenderer != nullptr)
    {
        if (renderVideo)
        {
            ::OutputDebugString(L"[CallController::UpdateMediaOperations] => Starting video render\n");
            newOperations = newOperations | PhoneVoIPApp::BackEnd::MediaOperations::VideoRender;
            Globals::Instance->VideoRenderer->Start();
        }
        else
        {
            ::OutputDebugString(L"[CallController::UpdateMediaOperations] => Stopping video render\n");
            Globals::Instance->VideoRenderer->Stop();
        }

        if (captureVideo)
        {
            ::OutputDebugString(L"[CallController::UpdateMediaOperations] => Starting video capture\n");
            newOperations = newOperations | PhoneVoIPApp::BackEnd::MediaOperations::VideoCapture;
            Globals::Instance->CaptureController->Start(cameraLocation);
            Globals::Instance->CaptureController->CameraLocationChanged += cameraLocationChangedHandler;
        }
        else
        {
            ::OutputDebugString(L"[CallController::UpdateMediaOperations] => Stopping video capture\n");
            Globals::Instance->CaptureController->Stop();
        }
    }

    // Let the listener know that the allowed media operation state has changed
    if (this->statusListener != nullptr)
    {
        this->statusListener->OnMediaOperationsChanged(newOperations);
    }
}
