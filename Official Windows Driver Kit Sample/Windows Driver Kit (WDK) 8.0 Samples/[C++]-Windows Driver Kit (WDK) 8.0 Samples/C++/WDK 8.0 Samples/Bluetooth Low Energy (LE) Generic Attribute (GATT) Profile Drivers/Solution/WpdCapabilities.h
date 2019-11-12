/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    WpdCapabilities.h
    
Abstract:

--*/

#pragma once

class WpdCapabilities
{
public:
    WpdCapabilities();
    virtual ~WpdCapabilities();

    HRESULT Initialize(_In_ BthLEDevice* pDevice);

    HRESULT DispatchWpdMessage(
                REFPROPERTYKEY          Command,
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetSupportedCommands(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetFunctionalCategories(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetFunctionalObjects(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    
    HRESULT OnGetSupportedContentTypes(
        IPortableDeviceValues*  pParams,
        IPortableDeviceValues*  pResults);

    HRESULT OnGetSupportedFormats(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetSupportedFormatProperties(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetFixedPropertyAttributes(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetSupportedEvents(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetEventOptions(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

private:
    BthLEDevice* m_pDevice;
};


