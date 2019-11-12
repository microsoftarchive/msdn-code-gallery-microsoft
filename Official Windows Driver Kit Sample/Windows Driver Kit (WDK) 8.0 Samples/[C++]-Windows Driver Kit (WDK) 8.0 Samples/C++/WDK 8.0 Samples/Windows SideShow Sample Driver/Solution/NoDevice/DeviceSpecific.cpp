//-----------------------------------------------------------------------
// <copyright file="DeviceSpecific.cpp" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation. All rights reserved.
// </copyright>
//
// Module:
//      DeviceSpecific.cpp
//
// Description:
//      Implements the device specific code for the driver.
//
//      This file contains device specific information, for example, how
//      to send data to the XYZ device as opposed to the ABC device.
//
//      This sample implementation doesn't do anything - it only contains
//      the stubs to be implemented later
//
//-----------------------------------------------------------------------



#include <windows.h>
#include "DeviceSpecific.h"
#include "Device.h"
#include "Renderer.h"
#include "RenderedData.h"


extern bool g_fDeviceIsValid;


HRESULT DeviceSpecificDisplayBitmap(const CRenderedData RenderedData)
{
    // Send a bitmap to your device here.
    // The bitmap is in buffer RenderedData.pbBitmapData of size (in bytes) RenderedData.cbBitmapData
    // For Example: SendBitmap(RenderedData.pbBitmapData, RenderedData.cbBitmapData);
    (RenderedData);

    return S_OK;
}


HRESULT DeviceDisplayInitialization(void)
{
    // Optionally check if the device has already been initialized
    // If it has, don't initialize a second time
    // Make sure to set this variable to true after initializing.
    if (true == g_fDeviceIsValid)
    {
        return E_FAIL;
    }

    // Initialize your device here.

    // Set the device capabilities
    CDevice::SetDeviceCaps(L"No Device (Sample)", // wszDeviceFriendlyName
                           L"Microsoft", // wszDeviceManufacturer
                           L"1.0.3.1", // wszDeviceFirmwareVersion
                           CDevice::DeviceTypeBitmap, // deviceType
                           320, // dwDeviceHorizontalResolution
                           240, // dwDeviceVerticalResolution
                           CDevice::ColorModeMonochrome, // deviceColorMode
                           1); // dwDeviceBitDepth

    // Set the renderer capabilities
    CRendererBase::SetRendererCaps(L"No Device (Sample)", // wszDefaultBackgroundTitle
                                   L"Windows SideShow Driver\nEnable gadgets in the control panel", // wszDefaultBackgroundBody
                                   L"Tahoma", // wszFontName
                                   11, // wFontSizeInPixels
                                   13, // wLineHeightInPixels
                                   3, // wNumberOfLinesDisplayedOnscreen
                                   13, // wScrollAmountInPixels
                                   3, // wTextOffsetFromLeft
                                   3, // wTextOffsetFromRight
                                   3, // wTextOffsetFromTop
                                   3); // wTextOffsetFromBottom

    return S_OK;
}


HRESULT DeviceButtonsInitialization(void)
{
    // Initialize button callbacks here.
    // This might be a thread, or registering a callback function.
    // Start Read Thread

    return S_OK;
}


HRESULT DeviceDisplayShutdown(void)
{
    // Optionally check if the device has already been shutdown
    // If it has, don't shutdown a second time
    // Make sure to set this variable to false after shutting down.
    if (false == g_fDeviceIsValid)
    {
        return E_FAIL;
    }

    // Shutdown the device here.

    CRendererBase::ReleaseRendererCaps();
    CDevice::ReleaseDeviceCaps();

    return S_OK;
}


HRESULT DeviceButtonsShutdown(void)
{
    // Shutdown button callbacks here.
    // This might be killing the thread or unregistering the callback

    return S_OK;
}
