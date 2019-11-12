/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    WpdObjectProperties.h
    
Abstract:

--*/

#pragma once

class WpdObjectProperties
{
public:
    WpdObjectProperties();
    virtual ~WpdObjectProperties();

    HRESULT Initialize(_In_ BthLEDevice* pDevice);

    HRESULT DispatchWpdMessage(
                 REFPROPERTYKEY         Command,
        _In_     IPortableDeviceValues* pParams,
        _Inout_  IPortableDeviceValues* pResults);

    HRESULT OnGetSupportedProperties(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetPropertyValues(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetAllPropertyValues(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnSetPropertyValues(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetPropertyAttributes(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnDeleteProperties(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

private:
    BthLEDevice* m_pDevice;
};

