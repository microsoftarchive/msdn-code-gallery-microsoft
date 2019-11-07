//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include <AsyncCB.h>
#include <CritSec.h>
#include <OpQueue.h>
#include <linklist.h>
#include <BaseAttributes.h>
#include <StspNetwork.h>

namespace Stsp {

class CMediaStream;

// Possible states of the source object
enum SourceState
{
    // Invalid state, source cannot be used 
    SourceState_Invalid,
    // Opening the connection
    SourceState_Opening,
    // Streaming started
    SourceState_Starting,
    // Streaming started
    SourceState_Started,
    // Streanung stopped
    SourceState_Stopped,
    // Source is shut down
    SourceState_Shutdown,
};

// Base class representing asyncronous source operation
class CSourceOperation : public IUnknown
{
public:
    enum Type
    {
        // Start the source
        Operation_Start,
        // Stop the source
        Operation_Stop,
        // Set rate
        Operation_SetRate,
    };

public:
    CSourceOperation(Type opType);
    virtual ~CSourceOperation();
    // IUnknown
    IFACEMETHOD (QueryInterface) (REFIID riid, void** ppv);
    IFACEMETHOD_(ULONG, AddRef) ();
    IFACEMETHOD_(ULONG, Release) ();

    Type GetOperationType() const {return _opType;}
    const PROPVARIANT& GetData() const { return _data;}
    HRESULT SetData(const PROPVARIANT& varData);

private:
    long                        _cRef;                      // reference count
    Type                        _opType;
    PROPVARIANT                 _data;
};

// Start operation
class CStartOperation : public CSourceOperation
{
public:
    CStartOperation(IMFPresentationDescriptor *pPD);
    ~CStartOperation();

    IMFPresentationDescriptor *GetPresentationDescriptor() {return _spPD.Get();}

private:
    ComPtr<IMFPresentationDescriptor> _spPD;   // Presentation descriptor
};

// SetRate operation
class CSetRateOperation : public CSourceOperation
{
public:
    CSetRateOperation(BOOL fThin, float flRate);
    ~CSetRateOperation();

    BOOL IsThin() const {return _fThin;}
    float GetRate() const {return _flRate;}

private:
    BOOL _fThin;
    float _flRate;
};

class CMediaSource : 
    public OpQueue<CMediaSource, CSourceOperation>, 
    public IMFMediaSource,
	public IMFGetService,
    public IMFRateControl,
    public CBaseAttributes<>
{
public:
    static HRESULT CreateInstance(CMediaSource **ppNetSource);

    // IUnknown
    IFACEMETHOD (QueryInterface) (REFIID riid, void** ppv);
    IFACEMETHOD_(ULONG, AddRef) ();
    IFACEMETHOD_(ULONG, Release) ();

    // IMFMediaEventGenerator
    IFACEMETHOD (BeginGetEvent) (IMFAsyncCallback* pCallback,IUnknown* punkState);
    IFACEMETHOD (EndGetEvent) (IMFAsyncResult* pResult, IMFMediaEvent** ppEvent);
    IFACEMETHOD (GetEvent) (DWORD dwFlags, IMFMediaEvent** ppEvent);
    IFACEMETHOD (QueueEvent) (MediaEventType met, REFGUID guidExtendedType, HRESULT hrStatus, const PROPVARIANT* pvValue);

    // IMFMediaSource
    IFACEMETHOD (CreatePresentationDescriptor) (IMFPresentationDescriptor** ppPresentationDescriptor);
    IFACEMETHOD (GetCharacteristics) (DWORD* pdwCharacteristics);
    IFACEMETHOD (Pause) ();
    IFACEMETHOD (Shutdown) ();
    IFACEMETHOD (Start) (
        IMFPresentationDescriptor* pPresentationDescriptor,
        const GUID* pguidTimeFormat,
        const PROPVARIANT* pvarStartPosition
    );
    IFACEMETHOD (Stop)();

	// IMFGetService
    IFACEMETHOD (GetService) ( _In_ REFGUID guidService, _In_ REFIID riid, _Out_opt_ LPVOID *ppvObject);

    // IMFRateControl
    IFACEMETHOD (SetRate) (BOOL fThin, float flRate);        
    IFACEMETHOD (GetRate) (_Inout_opt_ BOOL *pfThin, _Inout_opt_ float *pflRate);

    // OpQueue
    __override HRESULT DispatchOperation(_In_ CSourceOperation *pOp);
    __override HRESULT ValidateOperation(_In_ CSourceOperation *pOp);

    // Called by the byte stream handler.
    HRESULT BeginOpen(LPCWSTR pszUrl, IMFAsyncCallback *pCB, IUnknown *pUnkState);
    HRESULT EndOpen(IMFAsyncResult *pResult);

    // Other public methods
    _Acquires_lock_(_critSec)
    HRESULT Lock();
    _Releases_lock_(_critSec)
    HRESULT Unlock();

protected:
    CMediaSource(void);
    ~CMediaSource(void);

private:
    typedef ComPtrList<IMFMediaStream> StreamContainer;

private:
    HRESULT Initialize();
    
    void HandleError(HRESULT hResult);
    HRESULT GetStreamById(DWORD dwId, CMediaStream **ppStream);
    HRESULT ParseServerUrl(LPCWSTR pszUrl);
    HRESULT CompleteOpen(HRESULT hResult);
    HRESULT SendRequest(StspOperation eOperation, IMFAsyncCallback *pCallback);
    HRESULT SendDescribeRequest();
    HRESULT SendStartRequest();
    HRESULT Receive();
    HRESULT ParseCurrentBuffer();
    HRESULT ProcessPacket(StspOperationHeader *pOpHeader, IBufferPacket *pPacket);
    HRESULT ProcessServerDescription(IBufferPacket *pPacket);
    HRESULT ProcessServerSample(IBufferPacket *pPacket);
    HRESULT AddStream(StspStreamDescription *pStreamDesc);
    HRESULT InitPresentationDescription();
    HRESULT ValidatePresentationDescriptor(IMFPresentationDescriptor *pPD);
    HRESULT SelectStreams(IMFPresentationDescriptor *pPD);

    HRESULT DoStart(CStartOperation *pOp);
    HRESULT DoStop(CSourceOperation *pOp);
    HRESULT DoSetRate(CSetRateOperation *pOp);

    HRESULT OnConnected(IMFAsyncResult *pResult);
    HRESULT OnDescriptionRequestSent(IMFAsyncResult *pResult);
    HRESULT OnStartRequestSent(IMFAsyncResult *pResult);
    HRESULT OnDataReceived(IMFAsyncResult *pResult);

    BOOL IsRateSupported(float flRate, float *pflAdjustedRate);

    HRESULT CheckShutdown() const
    {
        if (_eSourceState == SourceState_Shutdown)
        {
            return MF_E_SHUTDOWN;
        }
        else
        {
            return S_OK;
        }
    }

private:
    long                        _cRef;                      // reference count
    CritSec                     _critSec;                   // critical section for thread safety
    SourceState                 _eSourceState;              // Flag to indicate if Shutdown() method was called.
    ComPtr<IMFMediaEventQueue>  _spEventQueue;              // Event queue
    ComPtr<INetworkChannel>     _spNetworkSender;           // Network sender
    HString                     _strServerAddress;          // Address of a server
    WORD                        _serverPort;                // Port of a server
    ComPtr<IMFAsyncResult>      _spOpenResult;              // Asynchronous result of open operation
    
    ComPtr<IBufferPacket>       _spCurrentReceivePacket;    // Receive packet
    ComPtr<IMediaBufferWrapper> _spCurrentReceiveBuffer;   // Current buffer 
    StspOperationHeader         _CurrentReceivedOperationHeader; // Header of the operation currently being received from the server.

    ComPtr<IMFPresentationDescriptor> _spPresentationDescriptor;

    StreamContainer             _streams;                   // Collection of streams associated with the source

    // Asynchronous callbacks
    AsyncCallback<CMediaSource> _OnConnectedCB;         
    AsyncCallback<CMediaSource> _OnDescriptionRequestSentCB;
    AsyncCallback<CMediaSource> _OnStartRequestSentCB;
    AsyncCallback<CMediaSource> _OnDataReceivedCB;

    float                       _flRate;
};

} // namespace Stsp
