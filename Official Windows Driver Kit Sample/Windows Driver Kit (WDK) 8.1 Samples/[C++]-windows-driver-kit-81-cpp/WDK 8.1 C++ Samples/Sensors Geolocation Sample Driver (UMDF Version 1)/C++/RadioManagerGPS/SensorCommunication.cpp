#include "precomp.h"
#pragma hdrstop

#define MAX_DEVPATH_LENGTH 256

CSensorCommunication::CSensorCommunication()
{
    m_hDev = nullptr;
}

CSensorCommunication::~CSensorCommunication()
{
    if (nullptr != m_hDev)
    {
        CloseHandle(m_hDev);
    }
}

// Will create a connection to the device
HRESULT CSensorCommunication::Initialize()
{
    HRESULT hr = S_OK;
    WCHAR completeDeviceName[MAX_DEVPATH_LENGTH];

    if (nullptr != m_hDev)
    {
        // m_hDev should be null to start
        hr = E_FAIL;
    }

    if (SUCCEEDED(hr))
    {
        hr = GetDevicePath(
            (LPGUID) &GUID_DEVINTERFACE_GPS_RADIO_MANAGEMENT,
            completeDeviceName,
            MAX_DEVPATH_LENGTH
            );
    }

    if (SUCCEEDED(hr))
    {
        m_hDev = CreateFile(
            completeDeviceName,
            GENERIC_WRITE | GENERIC_READ,
            FILE_SHARE_WRITE | FILE_SHARE_READ,
            nullptr, // default security
            OPEN_EXISTING,
            FILE_ATTRIBUTE_NORMAL | FILE_FLAG_OVERLAPPED,
            nullptr
            );

        if (INVALID_HANDLE_VALUE == m_hDev)
        {
            hr = HRESULT_FROM_WIN32(GetLastError());
            m_hDev = nullptr;
        }
    }

    return hr;
}

// Helper function to get the device path
HRESULT CSensorCommunication::GetDevicePath(
    _In_ LPGUID interfaceGuid,
    _Out_writes_z_(cchDevicePath) PWCHAR devicePath,
    _In_ size_t cchDevicePath
    )
{
    SP_DEVICE_INTERFACE_DATA deviceInterfaceData = {};
    PSP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData = nullptr;
    ULONG length = 0;
    ULONG requiredLength = 0;
    BOOL bResult = FALSE;
    HRESULT hr = S_OK;

    SecureZeroMemory(devicePath, cchDevicePath * sizeof(WCHAR));

    HDEVINFO hardwareDeviceInfo = SetupDiGetClassDevs(
        interfaceGuid,
        nullptr,
        nullptr,
        (DIGCF_PRESENT | DIGCF_DEVICEINTERFACE)
        );

    if (INVALID_HANDLE_VALUE == hardwareDeviceInfo)
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
    }

    if (SUCCEEDED(hr))
    {
        deviceInterfaceData.cbSize = sizeof(SP_DEVICE_INTERFACE_DATA);

        bResult = SetupDiEnumDeviceInterfaces(
            hardwareDeviceInfo,
            0,
            interfaceGuid,
            0,
            &deviceInterfaceData
            );

        if (FALSE == bResult)
        {
            hr = HRESULT_FROM_WIN32(GetLastError());
        }
    }

    // Get the size of the detailed data
    if (SUCCEEDED(hr))
    {
        bResult = SetupDiGetDeviceInterfaceDetail(
            hardwareDeviceInfo,
            &deviceInterfaceData,
            nullptr,
            0,
            &requiredLength,
            nullptr
            );

        if (FALSE == bResult)
        {
            hr = HRESULT_FROM_WIN32(GetLastError());
            if (HRESULT_FROM_WIN32(ERROR_INSUFFICIENT_BUFFER) == hr)
            {
                // This is expected as this is a query for the needed buffer size
                hr = S_OK;
            }
        }
    }

    // Allocate the detailed data
    if (SUCCEEDED(hr))
    {
        deviceInterfaceDetailData = 
            (PSP_DEVICE_INTERFACE_DETAIL_DATA) LocalAlloc(LMEM_FIXED, requiredLength);

        if (nullptr == deviceInterfaceDetailData)
        {
            hr = E_OUTOFMEMORY;
        }
    }

    // Get the detailed data
    if (SUCCEEDED(hr))
    {
        deviceInterfaceDetailData->cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);

        length = requiredLength;

        bResult = SetupDiGetDeviceInterfaceDetail(
            hardwareDeviceInfo,
            &deviceInterfaceData,
            deviceInterfaceDetailData,
            length,
            &requiredLength,
            nullptr);

        if (FALSE == bResult)
        {
            hr = HRESULT_FROM_WIN32(GetLastError());
        }
    }

    // Copy the device path from the detailed data to the output
    if (SUCCEEDED(hr))
    {
        hr = StringCchCopy(
            devicePath,
            cchDevicePath,
            deviceInterfaceDetailData->DevicePath);
    }

    // Cleanup memory

    if (INVALID_HANDLE_VALUE != hardwareDeviceInfo)
    {
        SetupDiDestroyDeviceInfoList(hardwareDeviceInfo);
    }

    if (nullptr != deviceInterfaceDetailData)
    {
        LocalFree(deviceInterfaceDetailData);
    }

    return hr;
}

HRESULT CSensorCommunication::SetRadioState(_In_ DEVICE_RADIO_STATE state)
{
    return SetRadioStateHelper(state, IOCTL_GPS_RADIO_MANAGEMENT_SET_RADIO_STATE);
}

HRESULT CSensorCommunication::GetRadioState(_Out_ DEVICE_RADIO_STATE* pState)
{
    return GetRadioStateHelper(pState, IOCTL_GPS_RADIO_MANAGEMENT_GET_RADIO_STATE);
}

HRESULT CSensorCommunication::SetPreviousRadioState(_In_ DEVICE_RADIO_STATE state)
{
    return SetRadioStateHelper(state, IOCTL_GPS_RADIO_MANAGEMENT_SET_PREVIOUS_RADIO_STATE);
}

HRESULT CSensorCommunication::GetPreviousRadioState(_Out_ DEVICE_RADIO_STATE* pState)
{
    return GetRadioStateHelper(pState, IOCTL_GPS_RADIO_MANAGEMENT_GET_PREVIOUS_RADIO_STATE);
}

HRESULT CSensorCommunication::SetRadioStateHelper(
    _In_ DEVICE_RADIO_STATE state,
    _In_ DWORD dwIoControlCode
    )
{
    HRESULT hr = S_OK;

    if (nullptr == m_hDev)
    {
        hr = E_FAIL;
    }

    if (SUCCEEDED(hr))
    {
        ULONG returnLength = 0;

        BOOL bResult = DeviceIoControl(
            m_hDev,
            dwIoControlCode,
            &state,                         // Ptr to InBuffer
            sizeof(DEVICE_RADIO_STATE),     // Length of InBuffer
            nullptr,                        // Ptr to OutBuffer
            0,                              // Length of OutBuffer
            &returnLength,                  // BytesReturned
            nullptr                         // Ptr to Overlapped structure
            );

        if (FALSE == bResult)
        {
            hr = HRESULT_FROM_WIN32(GetLastError());
        }
    }

    return hr;
}

HRESULT CSensorCommunication::GetRadioStateHelper(
    _Out_ DEVICE_RADIO_STATE* pState,
    _In_ DWORD dwIoControlCode
    )
{
    HRESULT hr = S_OK;
    *pState = DRS_RADIO_INVALID;

    if (nullptr == m_hDev)
    {
        hr = E_FAIL;
    }

    if (SUCCEEDED(hr))
    {
        ULONG returnLength = 0;

        BOOL bResult = DeviceIoControl(
            m_hDev,
            dwIoControlCode,
            nullptr,                        // Ptr to InBuffer
            0,                              // Length of InBuffer
            pState,                         // Ptr to OutBuffer
            sizeof(DEVICE_RADIO_STATE),     // Length of OutBuffer
            &returnLength,                  // BytesReturned
            nullptr                         // Ptr to Overlapped structure
            );

        if (FALSE == bResult)
        {
            hr = HRESULT_FROM_WIN32(GetLastError());
        }
    }

    return hr;
}

