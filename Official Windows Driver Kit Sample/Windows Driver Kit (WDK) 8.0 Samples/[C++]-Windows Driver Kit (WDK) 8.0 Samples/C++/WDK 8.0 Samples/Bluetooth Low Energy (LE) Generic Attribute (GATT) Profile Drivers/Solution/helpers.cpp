/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    Helpers.cpp

Abstract:

--*/

#include "stdafx.h"

#include "helpers.tmh"

ContextMap::ContextMap(WpdBaseDriver * pWpdBaseDriver) :
    m_cRef(1)
{
    m_WpdBaseDriver = pWpdBaseDriver;
}

ContextMap::~ContextMap()
{
    CComCritSecLock<CComAutoCriticalSection> Lock(m_CriticalSection);

    IUnknown*   pUnk            = NULL;
    POSITION    elementPosition = NULL;

    elementPosition = m_Map.GetStartPosition();
    while(elementPosition != NULL)
    {
        pUnk = m_Map.GetNextValue(elementPosition);
        if(pUnk != NULL)
        {
            pUnk->Release();
        }
    }

    m_WpdBaseDriver->DispatchClientDeparture();
}

HRESULT
GetDeviceAddressFromDevice(
    _In_ IWDFDevice * Device,
    _Out_ PBTH_ADDR pBthAddr
    )
{
    HRESULT hr = S_OK;
    IWDFUnifiedPropertyStoreFactory *PropertyStoreFactory = NULL;
    IWDFUnifiedPropertyStore *PropertyStore = NULL;
    WCHAR addressString[15];                //(0xaabbccddeeff\0)
    DEVPROPTYPE devPropType = 0;
    ULONG actualSize = 0;

    ZeroMemory(addressString, 15 * sizeof(WCHAR));

    if (NULL == Device ||
        NULL == pBthAddr) 
    {
        hr = E_INVALIDARG;
        CHECK_HR(hr, "Invalid argument");
    }    

    if (SUCCEEDED(hr))
    {
        //
        // Retrieve the unified property store factory
        //
        hr = Device->QueryInterface(IID_PPV_ARGS(&PropertyStoreFactory));
        CHECK_HR(hr, "Failed to retrieve the unified property store factory");
    }
    
    if (SUCCEEDED(hr))
    {
        WDF_PROPERTY_STORE_ROOT RootSpecifier = {0};

        RootSpecifier.LengthCb = sizeof(RootSpecifier);
        RootSpecifier.RootClass = WdfPropertyStoreRootClassHardwareKey;       
        
        //
        // Retrieved the unified property store
        //
        hr = PropertyStoreFactory->RetrieveUnifiedDevicePropertyStore(&RootSpecifier,
                                                                      &PropertyStore);
        CHECK_HR(hr, "Failed to retrieve the Unified Device ProertyStore");
    }

    if (SUCCEEDED(hr))
    {
        //
        // The Address String is in the form (0xaabbccddeeff\0) so we add the leading 0x
        //
        addressString[0] = L'0';
        addressString[1] = L'x';
        //
        // The Device Address is in the form of a string (aabbccddeeff\0), so take out the leading 0x
        //
        hr = PropertyStore->GetPropertyData(&DEVPKEY_Bluetooth_DeviceAddress,
                                            0,
                                            0,
                                            sizeof(addressString) - (2 * sizeof(WCHAR)),
                                            (addressString + 2),
                                            &actualSize,
                                            &devPropType);
        CHECK_HR(hr, "Failed to get the DEVPKEY_Bluetooth_DeviceAddress property");
    }

    if (SUCCEEDED(hr))
    {
        if (DEVPROP_TYPE_STRING != devPropType ||
            actualSize != sizeof(addressString) - (2 * sizeof(WCHAR)))
        {
            hr = E_INVALIDARG;
            CHECK_HR(hr, "Invalid property type or size");
        }
    }

    if (SUCCEEDED(hr))
    {
        if (!StrToInt64Ex(addressString, STIF_SUPPORT_HEX, reinterpret_cast<LONGLONG *>(pBthAddr)))
        {
            hr = E_UNEXPECTED;
            CHECK_HR(hr, "Failed to convert the address to a string");  
        }
    }
    

    //
    // Cleanup
    //
    if (NULL != PropertyStoreFactory)
    {
        PropertyStoreFactory->Release();
        PropertyStoreFactory = NULL;
    }

    if (NULL != PropertyStore)
    {
        PropertyStore->Release();
        PropertyStore = NULL;
    }    

    return hr;
}


HRESULT UpdateDeviceFriendlyName(
    _In_    IPortableDeviceClassExtension*  pPortableDeviceClassExtension,
    _In_    LPCWSTR                         wszDeviceFriendlyName)
{
    HRESULT hr = S_OK;

    // If we were passed NULL parameters we have nothing to do, return S_OK.
    if ((pPortableDeviceClassExtension == NULL) ||
        (wszDeviceFriendlyName         == NULL))
    {
        return S_OK;
    }

    CComPtr<IPortableDeviceValues>  pParams;
    CComPtr<IPortableDeviceValues>  pResults;
    CComPtr<IPortableDeviceValues>  pValues;

    // Prepare to make a call to set the device information
    if (hr == S_OK)
    {
        hr = CoCreateInstance(CLSID_PortableDeviceValues,
                              NULL,
                              CLSCTX_INPROC_SERVER,
                              IID_IPortableDeviceValues,
                              (VOID**)&pParams);
        CHECK_HR(hr, "Failed to CoCreate CLSID_PortableDeviceValues");
    }

    if (hr == S_OK)
    {
        hr = CoCreateInstance(CLSID_PortableDeviceValues,
                              NULL,
                              CLSCTX_INPROC_SERVER,
                              IID_IPortableDeviceValues,
                              (VOID**)&pResults);
        CHECK_HR(hr, "Failed to CoCreate CLSID_PortableDeviceValues for results");
    }

    if (hr == S_OK)
    {
        hr = CoCreateInstance(CLSID_PortableDeviceValues,
                              NULL,
                              CLSCTX_INPROC_SERVER,
                              IID_IPortableDeviceValues,
                              (VOID**)&pValues);
        CHECK_HR(hr, "Failed to CoCreate CLSID_PortableDeviceValues for results");
    }

    // Get the information values to update and set them in WPD_PROPERTY_CLASS_EXTENSION_DEVICE_INFORMATION_VALUES
    if (hr == S_OK)
    {
        hr = pValues->SetStringValue(WPD_DEVICE_FRIENDLY_NAME, wszDeviceFriendlyName);
        CHECK_HR(hr, ("Failed to set WPD_DEVICE_FRIENDLY_NAME"));
    }

    // Set the params
    if (hr == S_OK)
    {
        hr = pParams->SetGuidValue(WPD_PROPERTY_COMMON_COMMAND_CATEGORY, WPD_COMMAND_CLASS_EXTENSION_WRITE_DEVICE_INFORMATION.fmtid);
        CHECK_HR(hr, ("Failed to set WPD_PROPERTY_COMMON_COMMAND_CATEGORY"));
    }
    if (hr == S_OK)
    {
        hr = pParams->SetUnsignedIntegerValue(WPD_PROPERTY_COMMON_COMMAND_ID, WPD_COMMAND_CLASS_EXTENSION_WRITE_DEVICE_INFORMATION.pid);
        CHECK_HR(hr, ("Failed to set WPD_PROPERTY_COMMON_COMMAND_ID"));
    }
    if (hr == S_OK)
    {
        hr = pParams->SetIPortableDeviceValuesValue(WPD_PROPERTY_CLASS_EXTENSION_DEVICE_INFORMATION_VALUES, pValues);
        CHECK_HR(hr, ("Failed to set WPD_PROPERTY_CLASS_EXTENSION_DEVICE_INFORMATION_VALUES"));
    }

    // Make the call
    if (hr == S_OK)
    {
        hr = pPortableDeviceClassExtension->ProcessLibraryMessage(pParams, pResults);
        CHECK_HR(hr, ("Failed to process update device information message"));
    }

    // A Failed ProcessLibraryMessage operation for updating this value is not considered
    // fatal and should return S_OK.

    return S_OK;
}

HRESULT RegisterServices(
    _In_    IPortableDeviceClassExtension*  pPortableDeviceClassExtension,
            const bool                      bUnregister)
{
    // If we were passed NULL parameters we have nothing to do, return S_OK.
    if (pPortableDeviceClassExtension == NULL)
    {
        return S_OK;
    }

    CComPtr<IPortableDeviceValues>                 pParams;
    CComPtr<IPortableDeviceValues>                 pResults;
    CComPtr<IPortableDevicePropVariantCollection>  pInterfaces;

    PROPERTYKEY commandToUse = bUnregister?
                                 WPD_COMMAND_CLASS_EXTENSION_UNREGISTER_SERVICE_INTERFACES:
                                 WPD_COMMAND_CLASS_EXTENSION_REGISTER_SERVICE_INTERFACES;

    // Prepare to make a call to register the services
    HRESULT hr = CoCreateInstance(CLSID_PortableDeviceValues,
                          NULL,
                          CLSCTX_INPROC_SERVER,
                          IID_IPortableDeviceValues,
                          (VOID**)&pParams);
    CHECK_HR(hr, "Failed to CoCreate CLSID_PortableDeviceValues");

    if (hr == S_OK)
    {
        hr = CoCreateInstance(CLSID_PortableDeviceValues,
                              NULL,
                              CLSCTX_INPROC_SERVER,
                              IID_IPortableDeviceValues,
                              (VOID**)&pResults);
        CHECK_HR(hr, "Failed to CoCreate CLSID_PortableDeviceValues for results");
    }

    if (hr == S_OK)
    {
        hr = CoCreateInstance(CLSID_PortableDevicePropVariantCollection,
                              NULL,
                              CLSCTX_INPROC_SERVER,
                              IID_IPortableDevicePropVariantCollection,
                              (VOID**)&pInterfaces);
        CHECK_HR(hr, "Failed to CoCreate CLSID_PortableDevicePropVariantCollection for interfaces");
    }

    // Get the interfaces values to register and set them in WPD_PROPERTY_CLASS_EXTENSION_SERVICE_INTERFACES
    if (hr == S_OK)
    {
        PROPVARIANT pv;
        PropVariantInit(&pv);
        pv.vt = VT_CLSID;

        pv.puuid = (CLSID*)&SERVICE_GattService;
        hr = pInterfaces->Add(&pv);
        CHECK_HR(hr, "Failed to add the Gatt Service to the list of requested interfaces");

        // Don't call PropVariantClear, since we did not allocate the memory for these GUIDs
    }

    // Set the params
    if (hr == S_OK)
    {
        hr = pParams->SetGuidValue(WPD_PROPERTY_COMMON_COMMAND_CATEGORY, commandToUse.fmtid);
        CHECK_HR(hr, ("Failed to set WPD_PROPERTY_COMMON_COMMAND_CATEGORY"));
    }
    if (hr == S_OK)
    {
        hr = pParams->SetUnsignedIntegerValue(WPD_PROPERTY_COMMON_COMMAND_ID, commandToUse.pid);
        CHECK_HR(hr, ("Failed to set WPD_PROPERTY_COMMON_COMMAND_ID"));
    }

    if (hr == S_OK)
    {
        hr = pParams->SetStringValue(WPD_PROPERTY_CLASS_EXTENSION_SERVICE_OBJECT_ID, SERVICE_OBJECT_ID);
        CHECK_HR(hr, ("Failed to set WPD_PROPERTY_CLASS_EXTENSION_SERVICE_OBJECT_ID"));
    }

    if (hr == S_OK)
    {
        hr = pParams->SetIPortableDevicePropVariantCollectionValue(WPD_PROPERTY_CLASS_EXTENSION_SERVICE_INTERFACES, pInterfaces);
        CHECK_HR(hr, ("Failed to set WPD_PROPERTY_CLASS_EXTENSION_SERVICE_INTERFACES"));
    }

    // Make the call
    if (hr == S_OK)
    {
        hr = pPortableDeviceClassExtension->ProcessLibraryMessage(pParams, pResults);
        CHECK_HR(hr, ("Failed to process update device information message"));
    }

    return hr;
}


HRESULT AddStringValueToPropVariantCollection(
    _Out_   IPortableDevicePropVariantCollection* pCollection,
    _In_    LPCWSTR                               wszValue)
{
    HRESULT hr = S_OK;

    if ((pCollection == NULL) ||
        (wszValue    == NULL))
    {
        hr = E_INVALIDARG;
        return hr;
    }

    PROPVARIANT pv = {0};
    PropVariantInit(&pv);

    pv.vt      = VT_LPWSTR;
    pv.pwszVal = (LPWSTR)wszValue;

    // The wszValue will be copied into the collection, keeping the ownership
    // of the string belonging to the caller.
    // Don't call PropVariantClear, since we did not allocate the memory for these string values

    hr = pCollection->Add(&pv);

    return hr;
}

HRESULT GetClientContextMap(
    _In_        IPortableDeviceValues*  pParams,
    _Outptr_ ContextMap**            ppContextMap)
{
    HRESULT hr = S_OK;

    if(ppContextMap == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, ("Cannot have NULL parameter"));
        return hr;
    }

    hr = pParams->GetIUnknownValue(PRIVATE_SAMPLE_DRIVER_CLIENT_CONTEXT_MAP, (IUnknown**) ppContextMap);
    CHECK_HR(hr, "Failed to get PRIVATE_SAMPLE_DRIVER_CLIENT_CONTEXT_MAP");

    return hr;
}

_Success_(return == S_OK)
HRESULT GetClientContext(
    _In_        IPortableDeviceValues*  pParams,
    _In_        LPCWSTR                 pszContextKey,
    _Outptr_    IUnknown**              ppContext)
{
    HRESULT      hr             = S_OK;
    ContextMap*  pContextMap    = NULL;

    if(ppContext == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, ("Cannot have NULL parameter"));
        return hr;
    }
    *ppContext = NULL;

    hr = GetClientContextMap(pParams, &pContextMap);
    CHECK_HR(hr, ("Failed to get the client context map"));

    if ((hr == S_OK) && (pContextMap != NULL))
    {
        *ppContext = pContextMap->GetContext(pszContextKey);
        if(*ppContext == NULL)
        {
            hr = HRESULT_FROM_WIN32(ERROR_NOT_FOUND);
            CHECK_HR(hr, "Failed to find context %ws for this client", pszContextKey);
        }
    }

    SAFE_RELEASE(pContextMap);

    return hr;
}

HRESULT GetClientEventCookie(
    _In_            IPortableDeviceValues*  pParams,
    _Outptr_result_maybenull_ LPWSTR*                 ppszEventCookie)
{
    HRESULT        hr               = S_OK;
    LPWSTR         pszClientContext = NULL;
    ClientContext* pClientContext   = NULL;

    if ((pParams         == NULL) ||
        (ppszEventCookie == NULL))
    {
        return E_POINTER;
    }

    *ppszEventCookie = NULL;

    hr = pParams->GetStringValue(WPD_PROPERTY_COMMON_CLIENT_INFORMATION_CONTEXT, &pszClientContext);
    CHECK_HR(hr, "Missing value for WPD_PROPERTY_COMMON_CLIENT_INFORMATION_CONTEXT");

    if (hr == S_OK)
    {
        // Get the client context for this request.
        hr = GetClientContext(pParams, pszClientContext, (IUnknown**)&pClientContext);
        CHECK_HR(hr, "Failed to get the client context");
    }

    if ((hr == S_OK) && (pClientContext->EventCookie.GetLength() > 0))
    {
        // Get the event cookie only if it has been set
        *ppszEventCookie = AtlAllocTaskWideString(pClientContext->EventCookie);
        if (*ppszEventCookie == NULL)
        {
            hr = E_OUTOFMEMORY;
            CHECK_HR(hr, "Failed to allocate the client event cookie");
        }
    }

    // We're done with the context
    SAFE_RELEASE(pClientContext);

    CoTaskMemFree(pszClientContext);
    pszClientContext = NULL;

    return hr;
}


HRESULT PostWpdEvent(
    _In_    IPortableDeviceValues*  pCommandParams,
    _In_    IPortableDeviceValues*  pEventParams)
{
    HRESULT hr             = S_OK;
    BYTE*   pBuffer        = NULL;
    DWORD   cbBuffer       = 0;
    LPWSTR  pszEventCookie = NULL;

    CComPtr<IWDFDevice>     pDevice;
    CComPtr<IWpdSerializer> pSerializer;

    // Get the WUDF Device Object
    hr = pCommandParams->GetIUnknownValue(PRIVATE_SAMPLE_DRIVER_WUDF_DEVICE_OBJECT, (IUnknown**) &pDevice);
    CHECK_HR(hr, "Failed to get PRIVATE_SAMPLE_DRIVER_WUDF_DEVICE_OBJECT");

    // Get the WpdSerializer Object
    if (hr == S_OK)
    {
        hr = pCommandParams->GetIUnknownValue(PRIVATE_SAMPLE_DRIVER_WPD_SERIALIZER_OBJECT, (IUnknown**) &pSerializer);
        CHECK_HR(hr, "Failed to get PRIVATE_SAMPLE_DRIVER_WPD_SERIALIZER_OBJECT");
    }

    if (hr == S_OK)
    {
        // Set the client event cookie if available.  This is benign, as some clients may not provide a cookie.
        HRESULT hrEventCookie = GetClientEventCookie(pCommandParams, &pszEventCookie);
        if ((hrEventCookie == S_OK) && (pszEventCookie != NULL))
        {
            hrEventCookie = pEventParams->SetStringValue(WPD_CLIENT_EVENT_COOKIE, pszEventCookie);
            CHECK_HR(hrEventCookie, "Failed to set WPD_CLIENT_EVENT_COOKIE (error ignored)");
        }
    }

    if (hr == S_OK)
    {
        // Create a buffer with the serialized parameters
        hr = pSerializer->GetBufferFromIPortableDeviceValues(pEventParams, &pBuffer, &cbBuffer);
        CHECK_HR(hr, "Failed to get buffer from IPortableDeviceValues");
    }

    if (hr == S_OK && NULL == pBuffer)
    {
        hr = E_FAIL;
        CHECK_HR(hr, "pBuffer is NULL");
    }

    // Send the event
    if (hr == S_OK)
    {
        hr = pDevice->PostEvent(WPD_EVENT_NOTIFICATION, WdfEventBroadcast, pBuffer, cbBuffer);
        CHECK_HR(hr, "Failed to post WPD (broadcast) event");
    }

    // Free the memory
    CoTaskMemFree(pBuffer);
    pBuffer = NULL;

    CoTaskMemFree(pszEventCookie);
    pszEventCookie = NULL;

    return hr;
}

HRESULT AddPropertyAttributesByType(
    _In_    const DevicePropertyAttributesType type,
    _Inout_ IPortableDeviceValues*             pAttributes)
{
    HRESULT hr = S_OK;
    if (pAttributes == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    if (type == UnspecifiedForm_CanRead_CanWrite_CannotDelete_Fast)
    {
        hr = pAttributes->SetBoolValue(WPD_PROPERTY_ATTRIBUTE_CAN_WRITE, TRUE);
        CHECK_HR(hr, "Failed to set WPD_PROPERTY_ATTRIBUTE_CAN_WRITE");

        if (hr == S_OK)
        {
            hr = pAttributes->SetBoolValue(WPD_PROPERTY_ATTRIBUTE_CAN_DELETE, FALSE);
            CHECK_HR(hr, "Failed to set WPD_PROPERTY_ATTRIBUTE_CAN_DELETE");
        }

        if (hr == S_OK)
        {
            hr = pAttributes->SetBoolValue(WPD_PROPERTY_ATTRIBUTE_CAN_READ, TRUE);
            CHECK_HR(hr, "Failed to set WPD_PROPERTY_ATTRIBUTE_CAN_READ");
        }

        if (hr == S_OK)
        {
            hr = pAttributes->SetBoolValue(WPD_PROPERTY_ATTRIBUTE_FAST_PROPERTY, TRUE);
            CHECK_HR(hr, "Failed to set WPD_PROPERTY_ATTRIBUTE_FAST_PROPERTY");
        }

        if (hr == S_OK)
        {
            hr = pAttributes->SetUnsignedIntegerValue(WPD_PROPERTY_ATTRIBUTE_FORM, WPD_PROPERTY_ATTRIBUTE_FORM_UNSPECIFIED);
            CHECK_HR(hr, "Failed to set WPD_PROPERTY_ATTRIBUTE_FORM");
        }
        else
        {
            hr = pAttributes->SetBoolValue(WPD_PROPERTY_ATTRIBUTE_CAN_WRITE, FALSE);
            CHECK_HR(hr, "Failed to set WPD_PROPERTY_ATTRIBUTE_CAN_WRITE");
        }
    }
    else
    {
        hr = pAttributes->SetBoolValue(WPD_PROPERTY_ATTRIBUTE_CAN_WRITE, FALSE);
        CHECK_HR(hr, "Failed to set WPD_PROPERTY_ATTRIBUTE_CAN_WRITE");

        if (hr == S_OK)
        {
            hr = pAttributes->SetBoolValue(WPD_PROPERTY_ATTRIBUTE_CAN_DELETE, FALSE);
            CHECK_HR(hr, "Failed to set WPD_PROPERTY_ATTRIBUTE_CAN_DELETE");
        }

        if (hr == S_OK)
        {
            hr = pAttributes->SetBoolValue(WPD_PROPERTY_ATTRIBUTE_CAN_READ, TRUE);
            CHECK_HR(hr, "Failed to set WPD_PROPERTY_ATTRIBUTE_CAN_READ");
        }

        if (hr == S_OK)
        {
            hr = pAttributes->SetBoolValue(WPD_PROPERTY_ATTRIBUTE_FAST_PROPERTY, TRUE);
            CHECK_HR(hr, "Failed to set WPD_PROPERTY_ATTRIBUTE_FAST_PROPERTY");
        }

        if (hr == S_OK)
        {
            hr = pAttributes->SetUnsignedIntegerValue(WPD_PROPERTY_ATTRIBUTE_FORM, WPD_PROPERTY_ATTRIBUTE_FORM_UNSPECIFIED);
            CHECK_HR(hr, "Failed to set WPD_PROPERTY_ATTRIBUTE_FORM");
        }

    }

    return hr;
}

#define WPD_PROPERTY_ATTRIBUTE_MAX_SIZE_VALUE 1024
HRESULT SetPropertyAttributes(
                                REFPROPERTYKEY                  Key,
    _In_reads_(cAttributeInfo)  const PropertyAttributeInfo*    AttributeInfo,
    _In_                        DWORD                           cAttributeInfo,
    _Inout_                     IPortableDeviceValues*          pAttributes)
{
    HRESULT hr = HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED);

    if (pAttributes == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    for (DWORD dwIndex=0; dwIndex<cAttributeInfo; dwIndex++)
    {
        if (IsEqualPropertyKey(Key, *(AttributeInfo[dwIndex].pKey)))
        {
            // Set vartype
            hr = pAttributes->SetUnsignedIntegerValue(WPD_PROPERTY_ATTRIBUTE_VARTYPE, AttributeInfo[dwIndex].Vartype);
            CHECK_HR(hr, "Failed to set WPD_PROPERTY_ATTRIBUTE_VARTYPE");

            // Set name
            if (hr == S_OK && AttributeInfo[dwIndex].wszName != NULL)
            {
                hr = pAttributes->SetStringValue(WPD_PROPERTY_ATTRIBUTE_NAME, AttributeInfo[dwIndex].wszName);
                CHECK_HR(hr, "Failed to set WPD_PROPERTY_ATTRIBUTE_NAME");
            }

            // Set max size for string properties
            if (hr == S_OK && AttributeInfo[dwIndex].Vartype == VT_LPWSTR)
            {
                hr = pAttributes->SetUnsignedLargeIntegerValue(WPD_PROPERTY_ATTRIBUTE_MAX_SIZE, WPD_PROPERTY_ATTRIBUTE_MAX_SIZE_VALUE);
                CHECK_HR(hr, "Failed to set WPD_PROPERTY_ATTRIBUTE_MAX_SIZE");
            }

            // Set access attributes
            if (hr == S_OK)
            {
                hr = AddPropertyAttributesByType(AttributeInfo[dwIndex].AttributesType, pAttributes);
                CHECK_HR(hr, "Failed to set common property attributes");
            }

            break;
        }
    }

    return hr;
}

HRESULT SetMethodParameters(
                                REFGUID Method,
    _In_reads_(cAttributeInfo)  const MethodParameterAttributeInfo*  AttributeInfo,
    _In_                        DWORD                               cAttributeInfo,
    _Inout_                     IPortableDeviceKeyCollection*       pParameters)
{
    HRESULT hr = S_OK;

    if (NULL == pParameters)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    for (DWORD dwIndex=0; dwIndex < cAttributeInfo; dwIndex++)
    {
        if (*AttributeInfo[dwIndex].pMethodGuid == Method)
        {
            hr = pParameters->Add(*AttributeInfo[dwIndex].pParameter);
            CHECK_HR(hr, "Failed to add event parameter to collection");
        }
    }

    return hr;
}


HRESULT SetMethodParameterAttributes(
                                REFPROPERTYKEY                      Parameter,
    _In_reads_(cAttributeInfo)  const MethodParameterAttributeInfo* AttributeInfo,
    _In_                        DWORD                               cAttributeInfo,
    _Inout_                     IPortableDeviceValues*              pAttributes)
{
    HRESULT hr = HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED);

    if (pAttributes == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    for (DWORD dwIndex=0; dwIndex<cAttributeInfo; dwIndex++)
    {
        if (IsEqualPropertyKey(Parameter, *AttributeInfo[dwIndex].pParameter))
        {
            // Set vartype
            hr = pAttributes->SetUnsignedIntegerValue(WPD_PARAMETER_ATTRIBUTE_VARTYPE, AttributeInfo[dwIndex].Vartype);
            CHECK_HR(hr, "Failed to set WPD_PARAMETER_ATTRIBUTE_VARTYPE");

            // Set form
            if (hr == S_OK)
            {
                hr = pAttributes->SetUnsignedIntegerValue(WPD_PARAMETER_ATTRIBUTE_FORM, (DWORD)AttributeInfo[dwIndex].Form);
                CHECK_HR(hr, "Failed to set WPD_PARAMETER_ATTRIBUTE_FORM");
            }

            // Set order
            if (hr == S_OK)
            {
                hr = pAttributes->SetUnsignedIntegerValue(WPD_PARAMETER_ATTRIBUTE_ORDER, AttributeInfo[dwIndex].Order);
                CHECK_HR(hr, "Failed to set WPD_PARAMETER_ATTRIBUTE_ORDER");
            }

            // Set usage
            if (hr == S_OK)
            {
                hr = pAttributes->SetUnsignedIntegerValue(WPD_PARAMETER_ATTRIBUTE_USAGE, AttributeInfo[dwIndex].UsageType);
                CHECK_HR(hr, "Failed to set WPD_PARAMETER_ATTRIBUTE_USAGE");
            }

            // Set name
            if (hr == S_OK)
            {
                hr = pAttributes->SetStringValue(WPD_PARAMETER_ATTRIBUTE_NAME, AttributeInfo[dwIndex].wszName);
                CHECK_HR(hr, "Failed to set WPD_PARAMETER_ATTRIBUTE_NAME");
            }

            break;
        }
    }

    return hr;
}

HRESULT SetEventParameterAttributes(
                                REFPROPERTYKEY                      Parameter,
    _In_reads_(cAttributeInfo)  const EventParameterAttributeInfo*  AttributeInfo,
    _In_                        DWORD                               cAttributeInfo,
    _Inout_                     IPortableDeviceValues*              pAttributes)
{
    HRESULT hr = HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED);

    if (pAttributes == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    for (DWORD dwIndex=0; dwIndex<cAttributeInfo; dwIndex++)
    {
        if (IsEqualPropertyKey(Parameter, *AttributeInfo[dwIndex].pParameter))
        {
            // Set vartype
            hr = pAttributes->SetUnsignedIntegerValue(WPD_PARAMETER_ATTRIBUTE_VARTYPE, AttributeInfo[dwIndex].Vartype);
            CHECK_HR(hr, "Failed to set WPD_PARAMETER_ATTRIBUTE_VARTYPE");
            break;
        }
    }

    return hr;

}

HRESULT SetEventParameters(
                                REFGUID Event,
    _In_reads_(cAttributeInfo)  const EventParameterAttributeInfo*  AttributeInfo,
    _In_                        DWORD                               cAttributeInfo,
    _Inout_                     IPortableDeviceKeyCollection*       pParameters)
{
    HRESULT hr = HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED);

    if (pParameters == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    for (DWORD dwIndex=0; dwIndex<cAttributeInfo; dwIndex++)
    {
        GUID guidEvent = *AttributeInfo[dwIndex].pEventGuid;
        PROPERTYKEY param = *AttributeInfo[dwIndex].pParameter;

        if (guidEvent == Event)
        {
            hr = pParameters->Add(param);
            CHECK_HR(hr, "Failed to add event parameter to collection");
        }
    }

    return hr;
}

VOID ConvertFileTimeToUlonglong(
    _In_                        FILETIME * fTime,
    _Out_                       ULONGLONG * pResult)
{
    //large int from fTime
    ULARGE_INTEGER inftime;
    inftime.LowPart = fTime->dwLowDateTime;
    inftime.HighPart = fTime->dwHighDateTime;    
    
    //Large int init to the first second of jan 1 1970
    SYSTEMTIME jan1970 = { 1970, 1, 4,1,0,0,0,0};
    FILETIME ftjan1970;
    SystemTimeToFileTime(&jan1970, &ftjan1970);
    ULARGE_INTEGER largejan1970;
    largejan1970.LowPart = ftjan1970.dwLowDateTime;
    largejan1970.HighPart = ftjan1970.dwHighDateTime;
    
    //shift from 1601 to 1970 and convert from 100 nanosecond intervals to seconds
    *pResult = (inftime.QuadPart - largejan1970.QuadPart) / 10000000;            
}




