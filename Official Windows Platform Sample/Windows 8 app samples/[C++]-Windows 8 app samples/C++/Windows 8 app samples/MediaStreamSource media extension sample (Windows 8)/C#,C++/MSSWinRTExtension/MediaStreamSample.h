#pragma once

class CMediaStreamSample :
    public Microsoft::WRL::RuntimeClass<Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::RuntimeClassType::WinRtClassicComMix>,
    ABI::MSSWinRTExtension::IMediaStreamSample,
    Microsoft::WRL::CloakedIid<IMFMediaBuffer>,
    Microsoft::WRL::FtmBase>
{
    InspectableClass(RuntimeClass_MSSWinRTExtension_MediaStreamSample, BaseTrust);

public:
    CMediaStreamSample(void);
    ~CMediaStreamSample(void);
    
    HRESULT RuntimeClassInitialize(ABI::Windows::Storage::Streams::IBuffer *pBuffer,
        long nOffset,
        long cCount,
        LONGLONG llTimestamp,
        LONGLONG llDuration);

    // IMediaStreamSample
    IFACEMETHOD (get_Timestamp) ( 
        _Out_ LONGLONG *pllTimestamp);

    IFACEMETHOD (get_Duration) ( 
        _Out_ LONGLONG *pllDuration);

    // IMFMediaBuffer
    IFACEMETHOD (Lock) ( 
        _Outptr_result_bytebuffer_to_(*pcbMaxLength, *pcbCurrentLength)  BYTE **ppbBuffer,
        _Out_opt_  DWORD *pcbMaxLength,
        _Out_opt_  DWORD *pcbCurrentLength);

    IFACEMETHOD (Unlock) (void);

    IFACEMETHOD (GetCurrentLength) ( 
        _Out_  DWORD *pcbCurrentLength);

    IFACEMETHOD (SetCurrentLength) ( 
        DWORD cbCurrentLength);

    IFACEMETHOD (GetMaxLength) ( 
        _Out_  DWORD *pcbMaxLength);

private:
    ComPtr<ABI::Windows::Storage::Streams::IBuffer> _spBuffer;
    LONGLONG        _llTimestamp;
    LONGLONG        _llDuration;
    long            _nOffset;
    long            _cCount;
    BYTE            *_pLockedBuffer;
};

class CMediaStreamSampleFactory : public ActivationFactory<ABI::MSSWinRTExtension::IMediaStreamSampleFactory>
{
public:
    IFACEMETHOD (ActivateInstance)(_Outptr_result_nullonfailure_ IInspectable **ppvObject);

    // IMediaStreamSampleFactory
    IFACEMETHOD (CreateSample) (ABI::Windows::Storage::Streams::IBuffer *pBuffer,//IInspectable *pBuffer,
        long nOffset,
        long cCount,
        LONGLONG llTimestamp,
        LONGLONG llDuration,
        ABI::MSSWinRTExtension::IMediaStreamSample **ppSample);
};