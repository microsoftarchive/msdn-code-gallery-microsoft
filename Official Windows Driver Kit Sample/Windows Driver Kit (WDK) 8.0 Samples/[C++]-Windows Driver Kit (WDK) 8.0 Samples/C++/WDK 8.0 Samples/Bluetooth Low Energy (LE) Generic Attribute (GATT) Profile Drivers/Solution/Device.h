/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    Device.h

Abstract:

--*/

#pragma once

class ATL_NO_VTABLE CDevice :
    public CComObjectRootEx<CComMultiThreadModel>,
    public IPnpCallbackHardware
{
public:
    CDevice() :
        m_pWpdBaseDriver(NULL)
    {
    }

    DECLARE_NOT_AGGREGATABLE(CDevice)

    BEGIN_COM_MAP(CDevice)
        COM_INTERFACE_ENTRY(IPnpCallbackHardware)
    END_COM_MAP()

public:
    static HRESULT
    CreateInstance(
        _Outptr_ IUnknown**              ppUnkwn,
        _In_        IWDFDeviceInitialize*   pDeviceInit,
        _In_        WpdBaseDriver*          pWpdBaseDriver)
    {
        HRESULT hr = S_OK;
        CComObject< CDevice> *pMyDevice = NULL;

        if (NULL == ppUnkwn)
        {
            hr = E_POINTER;
        }
        else if (NULL == pDeviceInit || NULL == pWpdBaseDriver)
        {
            hr = E_INVALIDARG;
        }
        else
        {
            *ppUnkwn = NULL;
        }

        if ( SUCCEEDED( hr ) )
        {
            //
            // Set device properties.
            //
            pDeviceInit->SetLockingConstraint(None);
            hr = CComObject<CDevice>::CreateInstance( &pMyDevice );
        }

        if ( SUCCEEDED (hr) && NULL == pMyDevice )
        {
            hr = E_UNEXPECTED;
        }

        if( SUCCEEDED (hr) )
        {
            pMyDevice->AddRef();
            hr = pMyDevice->QueryInterface( __uuidof(IUnknown),(void **) ppUnkwn);
            if (hr == S_OK)
            {
                pMyDevice->m_pWpdBaseDriver = pWpdBaseDriver;
            }
            pMyDevice->Release();
            pMyDevice = NULL;
        }

        return hr;
    }

    // IPnpCallbackHardware
    //
    STDMETHOD_(HRESULT, OnPrepareHardware)(_In_ IWDFDevice* pDevice);
    STDMETHOD_(HRESULT, OnReleaseHardware)(_In_ IWDFDevice* pDevice);

private:

    _Success_(return == S_OK)
    HRESULT GetDeviceFriendlyName(
       _Outptr_               LPWSTR*   pwszDeviceFriendlyName);

    HRESULT GetSupportedContentTypes(
    _Outptr_result_maybenull_ IPortableDevicePropVariantCollection** ppContentTypes);

private:

    WpdBaseDriver*                         m_pWpdBaseDriver;
    CComPtr<IPortableDeviceClassExtension> m_pPortableDeviceClassExtension;
};


