#pragma once

class CBuffer :
    public Microsoft::WRL::RuntimeClass<Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::RuntimeClassType::WinRtClassicComMix>,
    ABI::Windows::Storage::Streams::IBuffer,
    Microsoft::WRL::CloakedIid<Windows::Storage::Streams::IBufferByteAccess>,
    Microsoft::WRL::CloakedIid<IMarshal>>
{
    InspectableClass(L"MSSWinRTExtension.Buffer", BaseTrust);

public:
    CBuffer(void);
    ~CBuffer(void);

    HRESULT RuntimeClassInitialize(
        UINT cbCapacity);

    // IBuffer
    IFACEMETHOD (get_Capacity) (_Out_ UINT *pcbCapacity);
    IFACEMETHOD (get_Length) (_Out_ UINT *pcbLength);
    IFACEMETHOD (put_Length) (UINT cbLength);

    // IBufferByteAccess
    IFACEMETHOD (Buffer) (_Out_ BYTE **ppBuffer);

    // IMarshal
    IFACEMETHOD (GetUnmarshalClass) (REFIID riid, _In_opt_ void *pv, DWORD dwDestContext, 
        _Reserved_ void *pvDestContext, DWORD mshlflags, _Out_ CLSID *pclsid);
    IFACEMETHOD (GetMarshalSizeMax) (REFIID riid, _In_opt_ void *pv, DWORD dwDestContext,
        _Reserved_ void *pvDestContext, DWORD mshlflags, _Out_ DWORD *pcbSize);
    IFACEMETHOD (MarshalInterface) (_In_ IStream *pStm, REFIID riid, _In_opt_ void *pv, DWORD dwDestContext,
        _Reserved_ void *pvDestContext, DWORD mshlflags);
    IFACEMETHOD (UnmarshalInterface) (_In_ IStream *, REFIID, _Deref_out_ void **);
    IFACEMETHOD (ReleaseMarshalData) (_In_ IStream *);
    IFACEMETHOD (DisconnectObject) (DWORD);

private:
    HRESULT CheckMarshal();

private:
    ComPtr<IMarshal>        _spBufferMarshal;
    BYTE                    *_pbBuffer;
    UINT                    _cbLength;
    UINT                    _cbCapacity;
};

