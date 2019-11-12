#pragma once

class CSensorCommunication
{
public:
    // Constructor and destructor
    CSensorCommunication();
    virtual ~CSensorCommunication();

    HRESULT Initialize();
    HRESULT SetRadioState(_In_ DEVICE_RADIO_STATE state);
    HRESULT GetRadioState(_Out_ DEVICE_RADIO_STATE* pState);
    HRESULT SetPreviousRadioState(_In_ DEVICE_RADIO_STATE state);
    HRESULT GetPreviousRadioState(_Out_ DEVICE_RADIO_STATE* pState);

private:
    HRESULT GetDevicePath(
        _In_ LPGUID interfaceGuid,
        _Out_writes_z_(bufLen) PWCHAR devicePath,
        _In_ size_t bufLen
        );

    HRESULT SetRadioStateHelper(
        _In_ DEVICE_RADIO_STATE state,
        _In_ DWORD dwIoControlCode
        );
    HRESULT GetRadioStateHelper(
        _Out_ DEVICE_RADIO_STATE* pState,
        _In_ DWORD dwIoControlCode
        );

    HANDLE m_hDev;
};
