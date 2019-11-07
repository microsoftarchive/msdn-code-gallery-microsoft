#pragma once
#include "GeometricMediaSource.h"

const DWORD c_dwGeometricStreamId = 1;

class CFrameGenerator;

class CGeometricMediaStream 
    : public IMFMediaStream
{
public:
    static HRESULT CreateInstance(CGeometricMediaSource *pSource, GeometricShape eShape, CGeometricMediaStream **ppStream);

    // IUnknown
    IFACEMETHOD (QueryInterface) (REFIID iid, void** ppv);
    IFACEMETHOD_ (ULONG, AddRef) ();
    IFACEMETHOD_ (ULONG, Release) ();

    // IMFMediaEventGenerator
    IFACEMETHOD (BeginGetEvent) (IMFAsyncCallback* pCallback,IUnknown* punkState);
    IFACEMETHOD (EndGetEvent) (IMFAsyncResult* pResult, IMFMediaEvent** ppEvent);
    IFACEMETHOD (GetEvent) (DWORD dwFlags, IMFMediaEvent** ppEvent);
    IFACEMETHOD (QueueEvent) (MediaEventType met, REFGUID guidExtendedType, HRESULT hrStatus, const PROPVARIANT* pvValue);

    // IMFMediaStream
    IFACEMETHOD (GetMediaSource) (IMFMediaSource** ppMediaSource);
    IFACEMETHOD (GetStreamDescriptor) (IMFStreamDescriptor** ppStreamDescriptor);
    IFACEMETHOD (RequestSample) (IUnknown* pToken);

    // Other public methods
    HRESULT Start();
    HRESULT Stop();
    HRESULT SetRate(float flRate);
    HRESULT Shutdown();
    HRESULT SetDXGIDeviceManager(IMFDXGIDeviceManager *pManager);

protected:
    CGeometricMediaStream();
    CGeometricMediaStream(CGeometricMediaSource *pSource, GeometricShape eShape);
    ~CGeometricMediaStream(void);

private:
    class CSourceLock;

private:
    HRESULT Initialize();
    HRESULT CreateMediaType(IMFMediaType **ppMediaType);
    HRESULT DeliverSample(IUnknown *pToken);
    HRESULT CreateImage(GeometricShape eShape, IMFSample **spSample);
    HRESULT HandleError(HRESULT hErrorCode);
    HRESULT CreateVideoSampleAllocator();

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
    CFrameGenerator             *_pFrameGenerator;
    float                       _flRate;
};

