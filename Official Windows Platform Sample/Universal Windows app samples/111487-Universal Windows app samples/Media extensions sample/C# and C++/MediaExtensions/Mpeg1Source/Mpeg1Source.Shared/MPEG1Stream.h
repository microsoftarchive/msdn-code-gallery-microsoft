//////////////////////////////////////////////////////////////////////////
//
// MPEG1Stream.h
// Implements the stream object (IMFMediaStream) for the MPEG-1 source.
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

class CMPEG1Source;

// The media stream object.
class CMPEG1Stream WrlSealed: public IMFMediaStream
{
public:

    CMPEG1Stream(CMPEG1Source *pSource, IMFStreamDescriptor *pSD);
    ~CMPEG1Stream();
    void Initialize();

    // IUnknown
    STDMETHODIMP QueryInterface(REFIID iid, void **ppv);
    STDMETHODIMP_(ULONG) AddRef();
    STDMETHODIMP_(ULONG) Release();

    // IMFMediaEventGenerator
    STDMETHODIMP BeginGetEvent(IMFAsyncCallback *pCallback,IUnknown *punkState);
    STDMETHODIMP EndGetEvent(IMFAsyncResult *pResult, IMFMediaEvent **ppEvent);
    STDMETHODIMP GetEvent(DWORD dwFlags, IMFMediaEvent **ppEvent);
    STDMETHODIMP QueueEvent(MediaEventType met, REFGUID guidExtendedType, HRESULT hrStatus, const PROPVARIANT *pvValue);

    // IMFMediaStream
    STDMETHODIMP GetMediaSource(IMFMediaSource **ppMediaSource);
    STDMETHODIMP GetStreamDescriptor(IMFStreamDescriptor **ppStreamDescriptor);
    STDMETHODIMP RequestSample(IUnknown *pToken);

    // Other methods (called by source)
    void     Activate(bool fActive);
    void     Start(const PROPVARIANT &varStart);
    void     Pause();
    void     Stop();
    void     SetRate(float flRate);
    void     EndOfStream();
    void     Shutdown();

    bool      IsActive() const { return m_fActive; }
    bool      NeedsData();

    void   DeliverPayload(IMFSample *pSample);

    // Callbacks
    HRESULT     OnDispatchSamples(IMFAsyncResult *pResult);

private:

    // SourceLock class:
    // Small helper class to lock and unlock the source.
    class SourceLock
    {
    private:
        CMPEG1Source *m_pSource;
    public:
		_Acquires_lock_(m_pSource)
        SourceLock(CMPEG1Source *pSource);

		_Releases_lock_(m_pSource)
        ~SourceLock();
    };

private:

    HRESULT CheckShutdown() const
    {
        return ( m_state == STATE_SHUTDOWN ? MF_E_SHUTDOWN : S_OK );
    }
    void DispatchSamples() throw();


private:
    long                m_cRef;                 // reference count

    ComPtr<CMPEG1Source> m_spSource;            // Parent media source
    ComPtr<IMFStreamDescriptor> m_spStreamDescriptor;
    ComPtr<IMFMediaEventQueue> m_spEventQueue;  // Event generator helper

    SourceState         m_state;                // Current state (running, stopped, paused)
    bool                m_fActive;              // Is the stream active?
    bool                m_fEOS;                 // Did the source reach the end of the stream?

    SampleList          m_Samples;              // Samples waiting to be delivered.
    TokenList           m_Requests;             // Sample requests, waiting to be dispatched.

    float               m_flRate;
};


