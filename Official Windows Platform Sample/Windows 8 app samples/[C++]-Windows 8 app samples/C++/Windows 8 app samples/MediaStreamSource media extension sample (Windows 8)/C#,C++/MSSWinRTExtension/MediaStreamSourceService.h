#pragma once
class CMediaStreamSourceStream;

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

class CMediaStreamSourceService :
    public Microsoft::WRL::RuntimeClass<Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::RuntimeClassType::WinRtClassicComMix>,
    ABI::MSSWinRTExtension::IMediaStreamSourceService,
    Microsoft::WRL::CloakedIid<IMFMediaSource>,
    Microsoft::WRL::FtmBase>
{
    InspectableClass(RuntimeClass_MSSWinRTExtension_MediaStreamSourceService, BaseTrust);

public:
    CMediaStreamSourceService(void);
    ~CMediaStreamSourceService(void);

    HRESULT RuntimeClassInitialize(
        _In_ ABI::MSSWinRTExtension::IMediaStreamSourcePlugin *pPlugin, _In_ IMFAsyncCallback *pOpenAsyncCallback, _In_ IUnknown *pOpenState);

    // IMediaStreamSourceService
    IFACEMETHOD (ErrorOccurred) (HSTRING errorCode);            
    IFACEMETHOD (ReportGetSampleCompleted) ( _In_ ABI::MSSWinRTExtension::IMediaStreamSample *pSample);            
    IFACEMETHOD (ReportOpenMediaCompleted) ( 
        _In_ ABI::Windows::Foundation::Collections::IPropertySet *pSourceAttributes,
        _In_ ABI::Windows::Foundation::Collections::IPropertySet *pAudioStreamAttributes);

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

    // Other public methods
    _Acquires_lock_(_cs) void Lock() { _cs.Lock(); }
    _Releases_lock_(_cs) void Unlock() { _cs.Unlock(); }
    HRESULT HandleError(HRESULT hrError);
    HRESULT GetSampleAsync(ABI::MSSWinRTExtension::MediaStreamType streamType);

private:
    HRESULT CompleteOpen(HRESULT hResult);
    HRESULT InitPresentationDescription();

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
    CritSec _cs; 
    ComPtr<IMFAsyncResult> _spOpenResult;
    ComPtr<ABI::MSSWinRTExtension::IMediaStreamSourcePlugin> _spPlugin;
    SourceState                 _eSourceState;
    ComPtr<IMFMediaEventQueue>  _spEventQueue;              // Event queue
    ComPtr<IMFPresentationDescriptor> _spPresentationDescriptor;
    ComPtr<CMediaStreamSourceStream> _spAudioStream;
};
