/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    BthLEDeviceContent.cpp

Abstract:

    Contains the WPD Device content implementation


--*/

#include "stdafx.h"

#include "BthLEDeviceContent.tmh"

const PropertyAttributeInfo g_SupportedDeviceProperties[] =
{
    {&WPD_OBJECT_ID,                                VT_LPWSTR, UnspecifiedForm_CanRead_CannotWrite_CannotDelete_Fast, NULL}, 
    {&WPD_OBJECT_PERSISTENT_UNIQUE_ID,              VT_LPWSTR, UnspecifiedForm_CanRead_CannotWrite_CannotDelete_Fast, NULL}, 
    {&WPD_OBJECT_NAME,                              VT_LPWSTR, UnspecifiedForm_CanRead_CannotWrite_CannotDelete_Fast, NULL}, 
    {&WPD_OBJECT_FORMAT,                            VT_CLSID,  UnspecifiedForm_CanRead_CannotWrite_CannotDelete_Fast, NULL}, 
    {&WPD_OBJECT_CONTENT_TYPE,                      VT_CLSID,  UnspecifiedForm_CanRead_CannotWrite_CannotDelete_Fast, NULL}, 
    {&WPD_OBJECT_CONTAINER_FUNCTIONAL_OBJECT_ID,    VT_LPWSTR, UnspecifiedForm_CanRead_CannotWrite_CannotDelete_Fast, NULL}, 
    {&WPD_FUNCTIONAL_OBJECT_CATEGORY,               VT_CLSID,  UnspecifiedForm_CanRead_CannotWrite_CannotDelete_Fast, NULL},
    {&WPD_DEVICE_FIRMWARE_VERSION,                  VT_LPWSTR, UnspecifiedForm_CanRead_CannotWrite_CannotDelete_Fast, NULL},
    {&WPD_DEVICE_PROTOCOL,                          VT_LPWSTR, UnspecifiedForm_CanRead_CannotWrite_CannotDelete_Fast, NULL},
    {&WPD_DEVICE_MODEL,                             VT_LPWSTR, UnspecifiedForm_CanRead_CannotWrite_CannotDelete_Fast, NULL},
    {&WPD_DEVICE_SERIAL_NUMBER,                     VT_LPWSTR, UnspecifiedForm_CanRead_CannotWrite_CannotDelete_Fast, NULL},
    {&WPD_DEVICE_MANUFACTURER,                      VT_CLSID,  UnspecifiedForm_CanRead_CannotWrite_CannotDelete_Fast, NULL},
    {&WPD_DEVICE_FRIENDLY_NAME,                     VT_CLSID,  UnspecifiedForm_CanRead_CannotWrite_CannotDelete_Fast, NULL},
    {&WPD_DEVICE_TYPE,                              VT_UI4,    UnspecifiedForm_CanRead_CannotWrite_CannotDelete_Fast, NULL},
};

HRESULT BthLEDeviceContent::InitializeContent(
    _In_ IWDFDevice* pDevice,
    _Inout_ DWORD *pdwLastObjectID,
    _In_ HANDLE hDeviceHandle,
    _In_ BthLEDevice * pBthLEDevice)
{
    HRESULT hr = S_OK;
    
    if (pdwLastObjectID == NULL ||
        hDeviceHandle == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    m_hDeviceHandle = hDeviceHandle;

    // Add top level object: Gatt Service
#pragma prefast(suppress: __WARNING_MEMORY_LEAK, "pGattService does not leak memory because ~AbstractDeviceContent() will release it")
    WpdGattServiceContent* pGattService = new WpdGattServiceContent();
    if (pGattService)
    {
        hr = pGattService->InitializeContent(pDevice, pdwLastObjectID, m_hDeviceHandle, pBthLEDevice);
        if (hr == S_OK)
        {
            CComCritSecLock<CComAutoCriticalSection> Lock(m_ChildrenCS);
            m_Children.Add(pGattService);
        }
        else
        {
            hr = E_OUTOFMEMORY;
            CHECK_HR(hr, "Failed to allocate GATT service content");
        }
    }

    return hr;
}

HRESULT BthLEDeviceContent::InitializeEnumerationContext(
            ACCESS_SCOPE                Scope,
    _Out_   WpdObjectEnumeratorContext* pEnumeratorContext)
{
    HRESULT hr = S_OK;

    if (pEnumeratorContext == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }    
    
    // Initialize the enumeration context
    if (Scope == BLUETOOTH_GATT_SERVICE_ACCESS)
    {
        // scoped by Gatt service, so only the Gatt service is visible
        pEnumeratorContext->m_TotalChildren = 1;
    }
    else
    {
        // default device wide enumeration, all children are visible
        CComCritSecLock<CComAutoCriticalSection> Lock(m_ChildrenCS);
        pEnumeratorContext->m_TotalChildren = static_cast<DWORD>(m_Children.GetCount());
    }

    return hr;
}

HRESULT BthLEDeviceContent::GetSupportedProperties(
    _Out_   IPortableDeviceKeyCollection* pKeys)
{
    HRESULT hr = S_OK;
    if (pKeys == NULL)
    {
        hr = E_INVALIDARG;
        return hr;
    }

    // Add the PROPERTYKEYs for the 'DEVICE' object
    for (DWORD dwIndex = 0; dwIndex < ARRAYSIZE(g_SupportedDeviceProperties); dwIndex++)
    {
        hr = pKeys->Add(*g_SupportedDeviceProperties[dwIndex].pKey);
        CHECK_HR(hr, "Failed to add device property");
    }

    return hr;
}

HRESULT BthLEDeviceContent::GetValue(
            REFPROPERTYKEY          Key,
    _Out_   IPortableDeviceValues*  pStore)
{
    HRESULT hr = S_OK;

    if (pStore == NULL)
    {
        hr = E_INVALIDARG;
        return hr;
    }

    // Set DEVICE object properties
    if (IsEqualPropertyKey(Key, WPD_DEVICE_FIRMWARE_VERSION))
    {
        hr = pStore->SetStringValue(WPD_DEVICE_FIRMWARE_VERSION, FirmwareVersion);
        CHECK_HR(hr, "Failed to set WPD_DEVICE_FIRMWARE_VERSION");
    }
    else if (IsEqualPropertyKey(Key, WPD_DEVICE_PROTOCOL))
    {
        hr = pStore->SetStringValue(WPD_DEVICE_PROTOCOL, Protocol);
        CHECK_HR(hr, "Failed to set WPD_DEVICE_PROTOCOL");
    }
    else if (IsEqualPropertyKey(Key, WPD_DEVICE_MODEL))
    {
        hr = pStore->SetStringValue(WPD_DEVICE_MODEL, Model);
        CHECK_HR(hr, "Failed to set WPD_DEVICE_MODEL");
    }
    else if (IsEqualPropertyKey(Key, WPD_DEVICE_SERIAL_NUMBER))
    {
        hr = pStore->SetStringValue(WPD_DEVICE_SERIAL_NUMBER, SerialNumber);
        CHECK_HR(hr, "Failed to set WPD_DEVICE_SERIAL_NUMBER");
    }
    else if (IsEqualPropertyKey(Key, WPD_DEVICE_MANUFACTURER))
    {
        hr = pStore->SetStringValue(WPD_DEVICE_MANUFACTURER, Manufacturer);
        CHECK_HR(hr, "Failed to set WPD_DEVICE_MANUFACTURER");
    }
    else if (IsEqualPropertyKey(Key, WPD_DEVICE_FRIENDLY_NAME))
    {
        hr = pStore->SetStringValue(WPD_DEVICE_FRIENDLY_NAME, FriendlyName);
        CHECK_HR(hr, "Failed to set WPD_DEVICE_FRIENDLY_NAME");
    }
    else if (IsEqualPropertyKey(Key, WPD_DEVICE_TYPE))
    {
        hr = pStore->SetUnsignedIntegerValue(WPD_DEVICE_TYPE, DeviceType);
        CHECK_HR(hr, "Failed to set WPD_DEVICE_TYPE");
    }
    
    // Set general properties for DEVICE
    else if (IsEqualPropertyKey(Key, WPD_OBJECT_ID))
    {
        hr = pStore->SetStringValue(WPD_OBJECT_ID, ObjectID);
        CHECK_HR(hr, "Failed to set WPD_OBJECT_ID");
    }
    else if (IsEqualPropertyKey(Key, WPD_OBJECT_NAME))
    {
        hr = pStore->SetStringValue(WPD_OBJECT_NAME, Name);
        CHECK_HR(hr, "Failed to set WPD_OBJECT_NAME");
    }
    else if (IsEqualPropertyKey(Key, WPD_OBJECT_PERSISTENT_UNIQUE_ID))
    {
        hr = pStore->SetStringValue(WPD_OBJECT_PERSISTENT_UNIQUE_ID, PersistentUniqueID);
        CHECK_HR(hr, "Failed to set WPD_OBJECT_PERSISTENT_UNIQUE_ID");
    }
    else if (IsEqualPropertyKey(Key, WPD_OBJECT_FORMAT))
    {
        hr = pStore->SetGuidValue(WPD_OBJECT_FORMAT, Format);
        CHECK_HR(hr, "Failed to set WPD_OBJECT_FORMAT");
    }
    else if (IsEqualPropertyKey(Key, WPD_OBJECT_CONTENT_TYPE))
    {
        hr = pStore->SetGuidValue(WPD_OBJECT_CONTENT_TYPE, ContentType);
        CHECK_HR(hr, "Failed to set WPD_OBJECT_CONTENT_TYPE");
    }
    else if (IsEqualPropertyKey(Key, WPD_FUNCTIONAL_OBJECT_CATEGORY))
    {
        hr = pStore->SetGuidValue(WPD_FUNCTIONAL_OBJECT_CATEGORY, FunctionalCategory);
        CHECK_HR(hr, "Failed to set WPD_FUNCTIONAL_OBJECT_CATEGORY");
    }
    else if (IsEqualPropertyKey(Key, WPD_OBJECT_CONTAINER_FUNCTIONAL_OBJECT_ID))
    {
        hr = pStore->SetStringValue(WPD_OBJECT_CONTAINER_FUNCTIONAL_OBJECT_ID, ContainerFunctionalObjectID);
        CHECK_HR(hr, "Failed to set WPD_OBJECT_CONTAINER_FUNCTIONAL_OBJECT_ID");
    }
    else
    {
        hr = HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED);
        CHECK_HR(hr, "Property %ws.%d is not supported", CComBSTR(Key.fmtid), Key.pid);
    }

    return hr;
}


