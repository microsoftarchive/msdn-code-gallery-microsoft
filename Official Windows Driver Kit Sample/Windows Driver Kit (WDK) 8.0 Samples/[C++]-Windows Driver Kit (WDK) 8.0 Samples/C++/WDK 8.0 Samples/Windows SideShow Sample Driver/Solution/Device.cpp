//-----------------------------------------------------------------------
// <copyright file="Device.cpp" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation. All rights reserved.
// </copyright>
//
// Module:
//      Device.cpp
//
// Description:
//
//-----------------------------------------------------------------------



#include "Device.h"
#include "DataManager.h"
#include "DeviceSpecific.h"

// Define defaults for Device Capabilities static members
bool     CDevice::m_fDeviceCapabilitiesAreDefined = false;
wchar_t* CDevice::m_wszDeviceFriendlyName = NULL;
wchar_t* CDevice::m_wszDeviceManufacturer = NULL;
wchar_t* CDevice::m_wszDeviceFirmwareVersion = NULL;
enum     CDevice::DeviceType CDevice::m_DeviceType = CDevice::DeviceTypeUndefined;
DWORD    CDevice::m_dwDeviceHorizontalResolution = 0;
DWORD    CDevice::m_dwDeviceVerticalResolution = 0;
enum     CDevice::ColorMode CDevice::m_DeviceColorMode = CDevice::ColorModeUndefined;
DWORD    CDevice::m_dwDeviceBitDepth = 0;

extern CDataManager* g_pDataManager;


HRESULT CDevice::SetDeviceCaps(const wchar_t* const wszDeviceFriendlyName,
                               const wchar_t* const wszDeviceManufacturer,
                               const wchar_t* const wszDeviceFirmwareVersion,
                               const enum CDevice::DeviceType deviceType,
                               const DWORD dwDeviceHorizontalResolution,
                               const DWORD dwDeviceVerticalResolution,
                               const enum CDevice::ColorMode deviceColorMode,
                               const DWORD dwDeviceBitDepth)
{
    size_t cchDeviceFriendlyName = wcslen(wszDeviceFriendlyName) + 1;
    m_wszDeviceFriendlyName = new(std::nothrow) wchar_t[cchDeviceFriendlyName];
    if (NULL == m_wszDeviceFriendlyName)
    {
        ReleaseDeviceCaps();
        return E_OUTOFMEMORY;
    }
    wcscpy_s(m_wszDeviceFriendlyName, cchDeviceFriendlyName, wszDeviceFriendlyName);

    size_t cchDeviceManufacturer = wcslen(wszDeviceManufacturer) + 1;
    m_wszDeviceManufacturer = new(std::nothrow) wchar_t[cchDeviceManufacturer];
    if (NULL == m_wszDeviceManufacturer)
    {
        ReleaseDeviceCaps();
        return E_OUTOFMEMORY;
    }
    wcscpy_s(m_wszDeviceManufacturer, cchDeviceManufacturer, wszDeviceManufacturer);

    size_t cchDeviceFirmwareVersion = wcslen(wszDeviceFirmwareVersion) + 1;
    m_wszDeviceFirmwareVersion = new(std::nothrow) wchar_t[cchDeviceFirmwareVersion];
    if (NULL == m_wszDeviceFirmwareVersion)
    {
        ReleaseDeviceCaps();
        return E_OUTOFMEMORY;
    }
    wcscpy_s(m_wszDeviceFirmwareVersion, cchDeviceFirmwareVersion, wszDeviceFirmwareVersion);

    m_DeviceType = deviceType;
    m_dwDeviceHorizontalResolution = dwDeviceHorizontalResolution;
    m_dwDeviceVerticalResolution = dwDeviceVerticalResolution;
    m_DeviceColorMode = deviceColorMode;
    m_dwDeviceBitDepth = dwDeviceBitDepth;

    m_fDeviceCapabilitiesAreDefined = true;
    return S_OK;
}


HRESULT CDevice::ReleaseDeviceCaps(void)
{
    if (NULL != m_wszDeviceFriendlyName)
    {
        delete [] m_wszDeviceFriendlyName;
        m_wszDeviceFriendlyName = NULL;
    }

    if (NULL == m_wszDeviceManufacturer)
    {
        delete [] m_wszDeviceManufacturer;
        m_wszDeviceManufacturer = NULL;
    }

    if (NULL == m_wszDeviceFirmwareVersion)
    {
        delete [] m_wszDeviceFirmwareVersion;
        m_wszDeviceFirmwareVersion = NULL;
    }

    m_DeviceType = DeviceTypeUndefined;
    m_dwDeviceHorizontalResolution = 0;
    m_dwDeviceVerticalResolution = 0;
    m_DeviceColorMode = ColorModeUndefined;
    m_dwDeviceBitDepth = 0;

    m_fDeviceCapabilitiesAreDefined = false;
    return S_OK;
}


HRESULT CDevice::SendRenderedDataToDevice(const CRenderedData RenderedData)
{
    HRESULT hr = DeviceSpecificDisplayBitmap(RenderedData);

    return hr;
}


HRESULT CDevice::HandleDeviceEvent(const CDevice::DeviceEvent DeviceEvent) // Such as a Next or Previous button
{
    HRESULT hr = E_FAIL;

    if (NULL != g_pDataManager)
    {
        hr = g_pDataManager->HandleDeviceEvent(DeviceEvent);
    }

    return hr;
}
