#pragma once
#include "MediaStreamSourceService.h"

#define AUDIO_STREAM_ID 1

class CMediaStreamSourceStream
    : public IMFMediaStream
{
public:
    static HRESULT CreateInstance(
        _In_ CMediaStreamSourceService *pSource, 
        _In_ ABI::Windows::Foundation::Collections::IPropertySet *pStreamAttributes,
        _Out_ CMediaStreamSourceStream **ppStream);

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
    HRESULT ReportSample(IMFSample *pSample);
    HRESULT Start();
    HRESULT Stop();
    HRESULT Shutdown();
  
private:
    CMediaStreamSourceStream(_In_ CMediaStreamSourceService *pSource);
    ~CMediaStreamSourceStream(void);

    HRESULT Initialize(_In_ ABI::Windows::Foundation::Collections::IPropertySet *pStreamAttributes);

    HRESULT DeliverSamples();
    HRESULT HandleError(HRESULT hrError);
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
    ComPtr<CMediaStreamSourceService> _spSource;            // Media source
    ComPtr<IMFMediaEventQueue>  _spEventQueue;              // Event queue
    ComPtr<IMFStreamDescriptor> _spStreamDescriptor;        // Stream descriptor
    ComPtrList<IMFSample>       _samples;
    ComPtrList<IUnknown, true>  _tokens;
};

