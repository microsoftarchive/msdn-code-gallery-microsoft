#pragma once
class CMediaStreamSchemeHandler
    : public Microsoft::WRL::RuntimeClass<
           Microsoft::WRL::RuntimeClassFlags< Microsoft::WRL::RuntimeClassType::WinRtClassicComMix >, 
           ABI::Windows::Media::IMediaExtension,
           IMFSchemeHandler >
{
    InspectableClass(RuntimeClass_MSSWinRTExtension_MediaStreamSchemeHandler, BaseTrust);

public:
    CMediaStreamSchemeHandler(void);
    ~CMediaStreamSchemeHandler(void);

    // IMediaExtension
    IFACEMETHOD (SetProperties) (ABI::Windows::Foundation::Collections::IPropertySet *pConfiguration);

    // IMFSchemeHandler
    IFACEMETHOD (BeginCreateObject) ( 
            __in LPCWSTR pwszURL,
            __in DWORD dwFlags,
            __in IPropertyStore *pProps,
            __out_opt  IUnknown **ppIUnknownCancelCookie,
            __in IMFAsyncCallback *pCallback,
            __in IUnknown *punkState);
        
    IFACEMETHOD (EndCreateObject) ( 
            __in IMFAsyncResult *pResult,
            __out  MF_OBJECT_TYPE *pObjectType,
            __out  IUnknown **ppObject);
        
    IFACEMETHOD (CancelObjectCreation) ( 
            __in IUnknown *pIUnknownCancelCookie);

private:
    ComPtr<ABI::MSSWinRTExtension::IMediaStreamSourcePlugin> _spPlugin;
};

