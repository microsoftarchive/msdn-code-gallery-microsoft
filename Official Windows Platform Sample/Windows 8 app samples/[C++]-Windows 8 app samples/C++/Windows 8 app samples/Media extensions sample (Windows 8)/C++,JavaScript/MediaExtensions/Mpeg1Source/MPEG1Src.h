//////////////////////////////////////////////////////////////////////////
//
// MPEG1Source.h
// Implements the MPEG-1 media source object.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//////////////////////////////////////////////////////////////////////////


#pragma once

#include <new.h>
#include <windows.h>
#include <assert.h>
#include <wrl\implements.h>

#ifndef _ASSERTE
#define _ASSERTE assert
#endif

#include <windows.media.h>

#include <mftransform.h>

#include <mfapi.h>
#include <mfobjects.h>
#include <mfidl.h>
#include <mferror.h>

//#include <amvideo.h>    // VIDEOINFOHEADER definition
//#include <dvdmedia.h>   // VIDEOINFOHEADER2
//#include <mmreg.h>      // MPEG1WAVEFORMAT

// Common sample files.
#include "linklist.h"

#include "asynccb.h"
#include "OpQueue.h"


template <class T> void SafeRelease(T **ppT)
{
    if (*ppT)
    {
        (*ppT)->Release();
        *ppT = NULL;
    }
}

void DllAddRef();
void DllRelease();


// Forward declares
class CMPEG1ByteStreamHandler;
class CMPEG1Source;
class CMPEG1Stream;
class SourceOp;

typedef ComPtrList<IMFSample>       SampleList;
typedef ComPtrList<IUnknown, true>  TokenList;    // List of tokens for IMFMediaStream::RequestSample

enum SourceState
{
    STATE_INVALID,      // Initial state. Have not started opening the stream.
    STATE_OPENING,      // BeginOpen is in progress.
    STATE_STOPPED,
    STATE_PAUSED,
    STATE_STARTED,
    STATE_SHUTDOWN
};

#include "Parse.h"          // MPEG-1 parser
#include "MPEG1Stream.h"    // MPEG-1 stream



const UINT32 MAX_STREAMS = 32;

class StreamList
{
    CMPEG1Stream*   m_streams[MAX_STREAMS];
    BYTE            m_id[MAX_STREAMS];
    UINT32          m_count;

public:
    StreamList() : m_count(0)
    {
        ZeroMemory(m_streams, sizeof(m_streams));
    }
    ~StreamList()
    {
        Clear();
    }

    UINT32 GetCount() const { return m_count; }

    void Clear()
    {
        for (UINT32 i = 0; i < MAX_STREAMS; i++)
        {
            SafeRelease(&m_streams[i]);
        }
        m_count = 0;
    }

    HRESULT AddStream(BYTE id, CMPEG1Stream *pStream)
    {
        if (GetCount() >= MAX_STREAMS)
        {
            return E_FAIL;
        }

        m_streams[m_count] = pStream;
        pStream->AddRef();

        m_id[m_count] = id;

        m_count++;

        return S_OK;
    }

    CMPEG1Stream* Find(BYTE id)
    {

        // This method can return NULL if the source did not create a
        // stream for this ID. In particular, this can happen if:
        //
        // 1) The stream type is not supported. See IsStreamTypeSupported().
        // 2) The source is still opening.
        //
        // Note: This method does not AddRef the stream object. The source
        // uses this method to access the streams. If the source hands out
        // a stream pointer (e.g. in the MENewStream event), the source
        // must AddRef the stream object.

        CMPEG1Stream* pStream = NULL;
        for (UINT32 i = 0; i < m_count; i++)
        {
            if (m_id[i] == id)
            {
                pStream = m_streams[i];
                break;
            }
        }
        return pStream;
    }

    // Accessor.
    CMPEG1Stream* operator[](DWORD index)
    {
        assert(index < m_count);
        return m_streams[index];
    }

    // Const accessor.
    CMPEG1Stream* const operator[](DWORD index) const
    {
        assert(index < m_count);
        return m_streams[index];
    }
};


// Constants

const DWORD INITIAL_BUFFER_SIZE = 4 * 1024; // Initial size of the read buffer. (The buffer expands dynamically.)
const DWORD READ_SIZE = 4 * 1024;           // Size of each read request.
const DWORD SAMPLE_QUEUE = 2;               // How many samples does each stream try to hold in its queue?

// Represents a request for an asynchronous operation.

class SourceOp : public IUnknown
{
public:

    enum Operation
    {
        OP_START,
        OP_PAUSE,
        OP_STOP,
        OP_SETRATE,
        OP_REQUEST_DATA,
        OP_END_OF_STREAM
    };

    static HRESULT CreateOp(Operation op, SourceOp **ppOp);
    static HRESULT CreateStartOp(IMFPresentationDescriptor *pPD, SourceOp **ppOp);
    static HRESULT CreateSetRateOp(BOOL fThin, float flRate, SourceOp **ppOp);

    // IUnknown
    STDMETHODIMP QueryInterface(REFIID iid, void** ppv);
    STDMETHODIMP_(ULONG) AddRef();
    STDMETHODIMP_(ULONG) Release();

    SourceOp(Operation op);
    virtual ~SourceOp();

    HRESULT SetData(const PROPVARIANT& var);

    Operation Op() const { return m_op; }
    const PROPVARIANT& Data() { return m_data;}

protected:
    long        m_cRef;     // Reference count.
    Operation   m_op;
    PROPVARIANT m_data;     // Data for the operation.
};

class StartOp : public SourceOp
{
public:
    StartOp(IMFPresentationDescriptor *pPD);
    ~StartOp();

    HRESULT GetPresentationDescriptor(IMFPresentationDescriptor **ppPD);

protected:
    IMFPresentationDescriptor   *m_pPD; // Presentation descriptor for Start operations.

};

class SetRateOp : public SourceOp
{
public:
    SetRateOp(BOOL fThin, float flRate);
    ~SetRateOp();

    BOOL IsThin() const {return m_fThin;}
    float GetRate() const {return m_flRate;}

private:
    BOOL m_fThin;
    float m_flRate;
};

// CMPEG1Source: The media source object.
class CMPEG1Source : 
    public OpQueue<CMPEG1Source, SourceOp>, 
    public IMFMediaSource,
	public IMFGetService,
    public IMFRateControl
{
public:
    static HRESULT CreateInstance(CMPEG1Source **ppSource);

    // IUnknown
    STDMETHODIMP QueryInterface(REFIID iid, void** ppv);
    STDMETHODIMP_(ULONG) AddRef();
    STDMETHODIMP_(ULONG) Release();

    // IMFMediaEventGenerator
    STDMETHODIMP BeginGetEvent(IMFAsyncCallback* pCallback,IUnknown* punkState);
    STDMETHODIMP EndGetEvent(IMFAsyncResult* pResult, IMFMediaEvent** ppEvent);
    STDMETHODIMP GetEvent(DWORD dwFlags, IMFMediaEvent** ppEvent);
    STDMETHODIMP QueueEvent(MediaEventType met, REFGUID guidExtendedType, HRESULT hrStatus, const PROPVARIANT* pvValue);

    // IMFMediaSource
    STDMETHODIMP CreatePresentationDescriptor(IMFPresentationDescriptor** ppPresentationDescriptor);
    STDMETHODIMP GetCharacteristics(DWORD* pdwCharacteristics);
    STDMETHODIMP Pause();
    STDMETHODIMP Shutdown();
    STDMETHODIMP Start(
        IMFPresentationDescriptor* pPresentationDescriptor,
        const GUID* pguidTimeFormat,
        const PROPVARIANT* pvarStartPosition
    );
    STDMETHODIMP Stop();

	// IMFGetService
    IFACEMETHOD (GetService) ( _In_ REFGUID guidService, _In_ REFIID riid, _Out_opt_ LPVOID *ppvObject);

    // IMFRateControl
    IFACEMETHOD (SetRate) (BOOL fThin, float flRate);        
    IFACEMETHOD (GetRate) (_Inout_opt_ BOOL *pfThin, _Inout_opt_ float *pflRate);

    // Called by the byte stream handler.
    HRESULT BeginOpen(IMFByteStream *pStream, IMFAsyncCallback *pCB, IUnknown *pUnkState);
    HRESULT EndOpen(IMFAsyncResult *pResult);

    // Queues an asynchronous operation, specify by op-type.
    // (This method is public because the streams call it.)
    HRESULT QueueAsyncOperation(SourceOp::Operation OpType);

    // Lock/Unlock:
    // Holds and releases the source's critical section. Called by the streams.
	_Acquires_lock_(m_critSec)
    void    Lock() { EnterCriticalSection(&m_critSec); }

	_Releases_lock_(m_critSec)
	void    Unlock() { LeaveCriticalSection(&m_critSec); }

    // Callbacks
    HRESULT OnByteStreamRead(IMFAsyncResult *pResult);  // Async callback for RequestData

private:

    CMPEG1Source(HRESULT& hr);
    ~CMPEG1Source();

    // CheckShutdown: Returns MF_E_SHUTDOWN if the source was shut down.
    HRESULT CheckShutdown() const
    {
        return ( m_state == STATE_SHUTDOWN ? MF_E_SHUTDOWN : S_OK );
    }

    HRESULT     CompleteOpen(HRESULT hrStatus);

    HRESULT     IsInitialized() const;
    BOOL        IsStreamTypeSupported(StreamType type) const;
    BOOL        IsStreamActive(const MPEG1PacketHeader& packetHdr);
    BOOL        StreamsNeedData() const;

    HRESULT     DoStart(StartOp *pOp);
    HRESULT     DoStop(SourceOp *pOp);
    HRESULT     DoPause(SourceOp *pOp);
    HRESULT     DoSetRate(SourceOp *pOp);
    HRESULT     OnStreamRequestSample(SourceOp *pOp);
    HRESULT     OnEndOfStream(SourceOp *pOp);

    HRESULT     InitPresentationDescriptor();
    HRESULT     SelectStreams(IMFPresentationDescriptor *pPD, const PROPVARIANT varStart);

    HRESULT     RequestData(DWORD cbRequest);
    HRESULT     ParseData();
    HRESULT     ReadPayload(DWORD *pcbAte, DWORD *pcbNextRequest);
    HRESULT     DeliverPayload();
    HRESULT     EndOfMPEGStream();

    HRESULT     CreateStream(const MPEG1PacketHeader& packetHdr);

    HRESULT     ValidatePresentationDescriptor(IMFPresentationDescriptor *pPD);

    // Handler for async errors.
    void        StreamingError(HRESULT hr);

    HRESULT     BeginAsyncOp(SourceOp *pOp);
    HRESULT     CompleteAsyncOp(SourceOp *pOp);
    HRESULT     DispatchOperation(SourceOp *pOp);
    HRESULT     ValidateOperation(SourceOp *pOp);

    BOOL        IsRateSupported(float flRate, float *pflAdjustedRate);

private:
    long                        m_cRef;                     // reference count

    CRITICAL_SECTION            m_critSec;                  // critical section for thread safety
    SourceState                 m_state;                    // Current state (running, stopped, paused)

    Buffer                      m_ReadBuffer;
    Parser                      *m_pParser;

    IMFMediaEventQueue          *m_pEventQueue;             // Event generator helper
    IMFPresentationDescriptor   *m_pPresentationDescriptor; // Presentation descriptor.
    IMFAsyncResult              *m_pBeginOpenResult;        // Result object for async BeginOpen operation.
    IMFByteStream               *m_pByteStream;

    MPEG1SystemHeader           *m_pHeader;                 // Release with CoTaskMemFree

    StreamList                  m_streams;                  // Array of streams.

    DWORD                       m_cPendingEOS;              // Pending EOS notifications.
    ULONG                       m_cRestartCounter;          // Counter for sample requests.

    SourceOp                    *m_pCurrentOp;
    SourceOp                    *m_pSampleRequest;

    // Async callback helper.
    AsyncCallback<CMPEG1Source>  m_OnByteStreamRead;

    float                       m_flRate;
};


