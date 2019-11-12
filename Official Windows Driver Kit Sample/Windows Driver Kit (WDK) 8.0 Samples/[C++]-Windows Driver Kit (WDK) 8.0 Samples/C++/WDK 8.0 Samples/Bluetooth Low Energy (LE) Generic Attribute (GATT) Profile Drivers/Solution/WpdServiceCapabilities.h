/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    WpdServiceCapabilities.h
    
Abstract:

--*/

#pragma once

class WpdServiceCapabilities
{
public:
    WpdServiceCapabilities();
    virtual ~WpdServiceCapabilities();

    HRESULT Initialize(_In_ WpdGattService* pGattService);

    HRESULT DispatchWpdMessage(
                REFPROPERTYKEY         Command,
        _In_    IPortableDeviceValues* pParams,
        _Inout_ IPortableDeviceValues* pResults);

private:
    HRESULT OnGetSupportedCommands(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetSupportedMethods(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetSupportedMethodsByFormat(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetMethodAttributes(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetMethodParameterAttributes(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetSupportedFormats(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetFormatAttributes(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetSupportedFormatProperties(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetFormatPropertyAttributes(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetSupportedEvents(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetEventAttributes(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetEventParameterAttributes(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

    HRESULT OnGetInheritedServices(
        _In_    IPortableDeviceValues*  pParams,
        _Inout_ IPortableDeviceValues*  pResults);

private:
    WpdGattService* m_pGattService;
};


