#pragma once
#include "AsyncCB.h"
#include "GeometricSource.h"

class CGeometricSchemeHandler
    : public Microsoft::WRL::RuntimeClass<
    Microsoft::WRL::RuntimeClassFlags< Microsoft::WRL::RuntimeClassType::WinRtClassicComMix >, 
    ABI::Windows::Media::IMediaExtension,
    IMFSchemeHandler >
{
    InspectableClass(RuntimeClass_GeometricSource_GeometricSchemeHandler, BaseTrust)

public:
    CGeometricSchemeHandler(void);
    ~CGeometricSchemeHandler(void);

    // IMediaExtension
    IFACEMETHOD (SetProperties) (ABI::Windows::Foundation::Collections::IPropertySet *pConfiguration);

    // IMFSchemeHandler
    IFACEMETHOD (BeginCreateObject) ( 
        _In_ LPCWSTR pwszURL,
        _In_ DWORD dwFlags,
        _In_ IPropertyStore *pProps,
        _Out_opt_  IUnknown **ppIUnknownCancelCookie,
        _In_ IMFAsyncCallback *pCallback,
        _In_ IUnknown *punkState);

    IFACEMETHOD (EndCreateObject) ( 
        _In_ IMFAsyncResult *pResult,
        _Out_  MF_OBJECT_TYPE *pObjectType,
        _Out_  IUnknown **ppObject);

    IFACEMETHOD (CancelObjectCreation) ( 
        _In_ IUnknown *pIUnknownCancelCookie);

private:
    HRESULT OnSourceOpen(_In_ IMFAsyncResult *pResult);

private:
    AsyncCallback<CGeometricSchemeHandler> _OnSourceOpenCB;
};

