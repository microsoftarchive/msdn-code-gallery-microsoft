//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include <CritSec.h>
#include <AsyncCB.h>
#include <linklist.h>
#include <BaseAttributes.h>
#include <StspNetwork.h>

#include "microsoft.samples.simplecommunication.h"
#include "StspStreamSink.h"
#include "StspInitializeOperation.h"
#include "StspEvent.h"

// {BCA19BBF-B335-4406-9357-F138D5B98CF3}
DEFINE_GUID(CLSID_CStspMediaSink, 
0xbca19bbf, 0xb335, 0x4406, 0x93, 0x57, 0xf1, 0x38, 0xd5, 0xb9, 0x8c, 0xf3);

namespace Stsp { 
class DECLSPEC_UUID("BCA19BBF-B335-4406-9357-F138D5B98CF3") CMediaSink
    : public Microsoft::WRL::RuntimeClass<
           Microsoft::WRL::RuntimeClassFlags< Microsoft::WRL::RuntimeClassType::WinRtClassicComMix >, 
           ABI::Microsoft::Samples::SimpleCommunication::IStspMediaSink,
           ABI::Windows::Media::IMediaExtension,
           FtmBase,
           IMFMediaSink,
           IMFClockStateSink >
    , public CBaseAttributes<>
{
    InspectableClass(RuntimeClass_Microsoft_Samples_SimpleCommunication_StspMediaSink,BaseTrust)

public:
    CMediaSink();
    ~CMediaSink();

    // IStspMediaSink
    IFACEMETHOD (Close) ();
    IFACEMETHOD (InitializeAsync) (ABI::Windows::Media::MediaProperties::IMediaEncodingProperties *audioEncodingProperties, ABI::Windows::Media::MediaProperties::IMediaEncodingProperties *videoEncodingProperties, IInitializeOperation **asyncInfo);
    IFACEMETHOD (add_IncomingConnectionEvent) ( 
        _In_opt_ ABI::Microsoft::Samples::SimpleCommunication::IIncomingConnectionHandler *handler,
        _Out_ EventRegistrationToken *cookie);
    IFACEMETHOD (remove_IncomingConnectionEvent)(EventRegistrationToken cookie);

    // IMediaExtension
    IFACEMETHOD (SetProperties) (ABI::Windows::Foundation::Collections::IPropertySet *pConfiguration) {return S_OK;}

    // IMFMediaSink methods
    IFACEMETHOD (GetCharacteristics) (DWORD *pdwCharacteristics);

    IFACEMETHOD (AddStreamSink)(
        /* [in] */ DWORD dwStreamSinkIdentifier,
        /* [in] */ IMFMediaType *pMediaType,
        /* [out] */ IMFStreamSink **ppStreamSink);

    IFACEMETHOD (RemoveStreamSink) (DWORD dwStreamSinkIdentifier);
    IFACEMETHOD (GetStreamSinkCount) (_Out_ DWORD *pcStreamSinkCount);
    IFACEMETHOD (GetStreamSinkByIndex) (DWORD dwIndex, _Outptr_ IMFStreamSink **ppStreamSink);
    IFACEMETHOD (GetStreamSinkById) (DWORD dwIdentifier, IMFStreamSink **ppStreamSink);
    IFACEMETHOD (SetPresentationClock) (IMFPresentationClock *pPresentationClock);
    IFACEMETHOD (GetPresentationClock) (IMFPresentationClock **ppPresentationClock);
    IFACEMETHOD (Shutdown) ();

    // IMFClockStateSink methods
    IFACEMETHOD (OnClockStart) (MFTIME hnsSystemTime, LONGLONG llClockStartOffset);
    IFACEMETHOD (OnClockStop) (MFTIME hnsSystemTime);
    IFACEMETHOD (OnClockPause) (MFTIME hnsSystemTime);
    IFACEMETHOD (OnClockRestart) (MFTIME hnsSystemTime);
    IFACEMETHOD (OnClockSetRate) (MFTIME hnsSystemTime, float flRate);

    LONGLONG GetStartTime() const {return _llStartTime;}

    void ReportEndOfStream();

    HRESULT _TriggerInitialization();
    HRESULT _TriggerAcceptConnection(DWORD dwConnectionId);
    HRESULT _TriggerRefuseConnection(DWORD dwConnectionId);

private:
    typedef ComPtrList<IMFStreamSink> StreamContainer;

private:
    HRESULT SendDescription();
    HRESULT FillStreamDescription(CStreamSink *pStream, StspStreamDescription *pStreamDescription, IMediaBufferWrapper **ppAttributes);

    void HandleError(HRESULT hr);

    HRESULT SetMediaStreamProperties ( 
        ABI::Windows::Media::Capture::MediaStreamType MediaStreamType,
         _In_opt_ ABI::Windows::Media::MediaProperties::IMediaEncodingProperties *mediaEncodingProperties);


    HRESULT OnIntialize(IMFAsyncResult* pAsyncResult);
    HRESULT OnAccept(IMFAsyncResult* pAsyncResult);
    HRESULT OnReceived(IMFAsyncResult* pAsyncResult);
    HRESULT OnDescriptionSent(IMFAsyncResult* pAsyncResult);
    HRESULT OnFireIncomingConnection(IMFAsyncResult* pAsyncResult);
    HRESULT OnAcceptConnection(IMFAsyncResult* pAsyncResult);
    HRESULT OnRefuseConnection(IMFAsyncResult* pAsyncResult);

    HRESULT PrepareRemoteUrl(IHostDescription *pRemoteHostDescription, HSTRING *pstrRemoteUrl);

    HRESULT CheckShutdown() const
    {
        if (_IsShutdown)
        {
            return MF_E_SHUTDOWN;
        }
        else
        {
            return S_OK;
        }
    }

private:
    long                            _cRef;                      // reference count
    CritSec                         _critSec;                   // critical section for thread safety

    bool                            _IsShutdown;                // Flag to indicate if Shutdown() method was called.
    bool                            _IsConnected;
    LONGLONG                        _llStartTime;

    ComPtr<IMFPresentationClock>    _spClock;                   // Presentation clock.
    ComPtr<INetworkChannel>         _spNetworkSender;           
    ComPtr<IMediaBufferWrapper>     _spReceiveBuffer;
    ComPtr<CInitializeOperation>    _spInitializeOperation;
    StreamContainer                 _streams;
    long                            _cStreamsEnded;
    HString                         _strRemoteUrl;

    DWORD                           _dwWaitingConnectionId;

    CEventSource<ABI::Microsoft::Samples::SimpleCommunication::IIncomingConnectionHandler> _evtStspSourceCreated;

    AsyncCallback<CMediaSink>       _OnInitializeCB;      // Callback invoked for asynchronous initialization
    AsyncCallback<CMediaSink>       _OnAcceptCB;          // Callback invoked when connection has been accepted
    AsyncCallback<CMediaSink>       _OnReceivedCB;        // Callback invoked when data was received
    AsyncCallback<CMediaSink>       _OnDesciptionSentCB;  // Callback invoked when description has been sent
    AsyncCallback<CMediaSink>       _OnFireIncomingConnectionCB;  // Callback invoked to fire incoming connection event
    AsyncCallback<CMediaSink>       _OnAcceptConnectionCB;  // Callback invoked when client code accepted connection
    AsyncCallback<CMediaSink>       _OnRefuseConnectionCB;      // Callback invoked when client code refused connection
};

} // namespace Stsp
