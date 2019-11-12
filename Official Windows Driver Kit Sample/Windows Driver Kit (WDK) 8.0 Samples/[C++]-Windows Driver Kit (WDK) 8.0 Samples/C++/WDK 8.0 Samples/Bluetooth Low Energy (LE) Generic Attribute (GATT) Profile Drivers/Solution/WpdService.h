/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    WpdService.h
    
Abstract:

--*/

#pragma once

class WpdService
{
public:
    WpdService();
    virtual ~WpdService();

    HRESULT Initialize(_In_ IWDFDevice* pDevice, _In_ BthLEDevice* pBthLEDevice);

    HRESULT DispatchWpdMessage(
                REFPROPERTYKEY         Command,
        _In_    IPortableDeviceValues* pParams,
        _Inout_ IPortableDeviceValues* pResults);

private:
    HRESULT OnGetServiceObjectID(
        _In_    LPCWSTR                pszRequestFilename,
        _In_    IPortableDeviceValues* pParams,
        _Inout_ IPortableDeviceValues* pResults);

    HRESULT CheckRequestFilename(
        _In_    LPCWSTR                 pszRequestFilename);

private:
    WpdServiceMethods       m_ServiceMethods;    
    WpdServiceCapabilities  m_ServiceCapabilities;
    WpdGattService*    m_pGattService;
};


