//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include "GeometricMediaSource.h"

const DWORD c_dwGeometricStreamId = 1;

ref class CFrameGenerator;

class CGeometricMediaStream WrlSealed
    : public IMFMediaStream
{
public:
    static ComPtr<CGeometricMediaStream> CreateInstance(CGeometricMediaSource *pSource, GeometricShape eShape);

    // IUnknown
    IFACEMETHOD (QueryInterface) (REFIID iid, void **ppv);
    IFACEMETHOD_ (ULONG, AddRef) ();
    IFACEMETHOD_ (ULONG, Release) ();

    // IMFMediaEventGenerator
    IFACEMETHOD (BeginGetEvent) (IMFAsyncCallback *pCallback,IUnknown *punkState);
    IFACEMETHOD (EndGetEvent) (IMFAsyncResult *pResult, IMFMediaEvent **ppEvent);
    IFACEMETHOD (GetEvent) (DWORD dwFlags, IMFMediaEvent **ppEvent);
    IFACEMETHOD (QueueEvent) (MediaEventType met, REFGUID guidExtendedType, HRESULT hrStatus, const PROPVARIANT *pvValue);

    // IMFMediaStream
    IFACEMETHOD (GetMediaSource) (IMFMediaSource **ppMediaSource);
    IFACEMETHOD (GetStreamDescriptor) (IMFStreamDescriptor **ppStreamDescriptor);
    IFACEMETHOD (RequestSample) (IUnknown *pToken);

    // Other public methods
    HRESULT Start();
    HRESULT Stop();
    HRESULT SetRate(float flRate);
    void Shutdown();
    void SetDXGIDeviceManager(IMFDXGIDeviceManager *pManager);

protected:
    CGeometricMediaStream();
    CGeometricMediaStream(CGeometricMediaSource *pSource, GeometricShape eShape);
    ~CGeometricMediaStream(void);

private:
    class CSourceLock;

private:
    void Initialize();
    ComPtr<IMFMediaType> CreateMediaType();
    void DeliverSample(IUnknown *pToken);
    ComPtr<IMFSample> CreateImage(GeometricShape eShape);
    HRESULT HandleError(HRESULT hErrorCode);
    void CreateVideoSampleAllocator();

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
    SourceState                 _eSourceState;              // Flag to indicate if Shutdown() method was called.
    ComPtr<CGeometricMediaSource> _spSource;
    ComPtr<IMFMediaEventQueue>  _spEventQueue;              // Event queue
    ComPtr<IMFStreamDescriptor> _spStreamDescriptor;        // Stream descriptor
    ComPtr<IMFMediaBuffer>      _spPicture;
    LONGLONG                    _llCurrentTimestamp;
    ComPtr<IMFDXGIDeviceManager> _spDeviceManager;
    GeometricShape              _eShape;
    ComPtr<IMFMediaType>        _spMediaType;
    ComPtr<IMFVideoSampleAllocatorEx> _spAllocEx;
    CFrameGenerator^            _frameGenerator;
    float                       _flRate;
};

