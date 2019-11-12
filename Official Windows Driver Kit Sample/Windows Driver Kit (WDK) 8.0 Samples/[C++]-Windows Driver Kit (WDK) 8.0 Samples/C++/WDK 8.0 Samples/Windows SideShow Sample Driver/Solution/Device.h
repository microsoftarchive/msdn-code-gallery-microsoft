//-----------------------------------------------------------------------
// <copyright file="Device.h" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation. All rights reserved.
// </copyright>
//
// Module:
//      Device.h
//
// Description:
//      Device Class
//
//-----------------------------------------------------------------------


#pragma once


#include <windows.h>
#include "RenderedData.h"


// Singleton object
class CDevice
{
public:
    enum DeviceType
    {
        DeviceTypeUndefined,
        DeviceTypeBitmap,
        DeviceTypeText
    };

    enum ColorMode
    {
        ColorModeUndefined,
        ColorModeColor, // For color bitmap displays
        ColorModeGrayscale, // For grayscale bitmap displays
        ColorModeMonochrome // For monochrome bitmap displays
    };

    enum DeviceEvent
    {
        Undefined,
        ButtonNext,
        ButtonPrevious,
        RenderAgain
    };

    CDevice(void){}
    ~CDevice(void){}

    // Sets the device capabilities
    static HRESULT SetDeviceCaps(const wchar_t* const wszDeviceFriendlyName, // The friendly name of the device as seen in the SideShow Control Panel
                                 const wchar_t* const wszDeviceManufacturer, // The device manufacturer name as seen in device manager's details tab
                                 const wchar_t* const wszDeviceFirmwareVersion, // The driver version as seen in device manager's driver tab
                                 const enum DeviceType deviceType, // If type is text, then resolution is in characters e.g. 16x2 characters
                                 const DWORD dwDeviceHorizontalResolution, // The horizontal resolution of the device
                                 const DWORD dwDeviceVerticalResolution, // The vertical resolution of the device
                                 const enum ColorMode deviceColorMode, // The color mode of the device
                                 const DWORD dwDeviceBitDepth); // The bit depth of the device

    // Releases memory used by DeviceCaps and resets all values to 0 / NULL
    static HRESULT ReleaseDeviceCaps(void);

    // DeviceCaps Accessors
    static bool     AreDeviceCapalitiesDefined(void) {return m_fDeviceCapabilitiesAreDefined;}
    static wchar_t* GetDeviceFriendlyName(void) {return m_wszDeviceFriendlyName;}
    static wchar_t* GetDeviceManufacturer(void) {return m_wszDeviceManufacturer;}
    static wchar_t* GetDeviceFirmwareVersion(void) {return m_wszDeviceFirmwareVersion;}
    static enum DeviceType GetDeviceType(void) {return m_DeviceType;}
    static DWORD    GetDeviceHorizontalResolution(void) {return m_dwDeviceHorizontalResolution;}
    static DWORD    GetDeviceVerticalResolution(void) {return m_dwDeviceVerticalResolution;}
    static enum ColorMode GetDeviceColorMode(void) {return m_DeviceColorMode;}
    static DWORD    GetDeviceBitDepth(void) {return m_dwDeviceBitDepth;}

    HRESULT SendRenderedDataToDevice(const CRenderedData RenderedData);
    HRESULT HandleDeviceEvent(const CDevice::DeviceEvent DeviceEvent); // Such as a Next or Previous button

private:
    // Device Capabilities Members
    static bool     m_fDeviceCapabilitiesAreDefined;
    static wchar_t* m_wszDeviceFriendlyName;
    static wchar_t* m_wszDeviceManufacturer;
    static wchar_t* m_wszDeviceFirmwareVersion;
    static enum DeviceType m_DeviceType;
    static DWORD    m_dwDeviceHorizontalResolution;
    static DWORD    m_dwDeviceVerticalResolution;
    static enum ColorMode m_DeviceColorMode;
    static DWORD    m_dwDeviceBitDepth;
};
