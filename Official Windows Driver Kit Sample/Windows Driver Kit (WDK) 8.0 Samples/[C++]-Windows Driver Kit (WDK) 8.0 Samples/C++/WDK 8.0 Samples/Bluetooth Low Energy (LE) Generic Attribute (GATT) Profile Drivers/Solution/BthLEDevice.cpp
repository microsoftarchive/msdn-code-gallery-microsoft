/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    BthLEDevice.cpp

Abstract:


--*/

#include "stdafx.h"

#include "BthLEDevice.tmh"

const PROPERTYKEY* g_SupportedCommands[] =
{
    // WPD_CATEGORY_OBJECT_ENUMERATION
    &WPD_COMMAND_OBJECT_ENUMERATION_START_FIND,
    &WPD_COMMAND_OBJECT_ENUMERATION_FIND_NEXT,
    &WPD_COMMAND_OBJECT_ENUMERATION_END_FIND,

    // WPD_CATEGORY_OBJECT_PROPERTIES
    &WPD_COMMAND_OBJECT_PROPERTIES_GET_SUPPORTED,
    &WPD_COMMAND_OBJECT_PROPERTIES_GET,
    &WPD_COMMAND_OBJECT_PROPERTIES_GET_ALL,

    // WPD_CATEGORY_CAPABILITIES
    &WPD_COMMAND_CAPABILITIES_GET_SUPPORTED_COMMANDS,
    &WPD_COMMAND_CAPABILITIES_GET_SUPPORTED_FUNCTIONAL_CATEGORIES,
    &WPD_COMMAND_CAPABILITIES_GET_FUNCTIONAL_OBJECTS,
    &WPD_COMMAND_CAPABILITIES_GET_SUPPORTED_FORMATS,
    &WPD_COMMAND_CAPABILITIES_GET_SUPPORTED_FORMAT_PROPERTIES,
    &WPD_COMMAND_CAPABILITIES_GET_FIXED_PROPERTY_ATTRIBUTES,

    // WPD_CATEGORY_COMMON
    &WPD_COMMAND_COMMON_GET_OBJECT_IDS_FROM_PERSISTENT_UNIQUE_IDS,
};


const GUID* g_SupportedFunctionalCategories[] =
{
    &WPD_FUNCTIONAL_CATEGORY_DEVICE,
    &SERVICE_GattService,
};

const PROPERTYKEY* g_SupportedCommonProperties[] =
{
    &WPD_OBJECT_ID,
    &WPD_OBJECT_PERSISTENT_UNIQUE_ID,
    &WPD_OBJECT_PARENT_ID,
    &WPD_OBJECT_NAME,
    &WPD_OBJECT_FORMAT,
    &WPD_OBJECT_CONTENT_TYPE,
    &WPD_OBJECT_CAN_DELETE,
    &WPD_OBJECT_CONTAINER_FUNCTIONAL_OBJECT_ID,
};


HRESULT BthLEDevice::InitializeContent(_In_ IWDFDevice* pDevice, _In_ PCWSTR DeviceFileName)
{
    HRESULT hr = S_OK;

    //
    // Create a GattService
    //
    m_pGattService = new WpdGattService();
    if (NULL == m_pGattService)
    {
        hr = E_OUTOFMEMORY;
        CHECK_HR(hr, "Couldn't allocate GATT Service");
    }

    if (SUCCEEDED(hr))
    {
        hr = m_pGattService->Initialize(pDevice, this);
        CHECK_HR(hr, "Couldn't initialize GATT Service");
    }
    

    if (SUCCEEDED(hr))
    {
        //
        // Open a handle to the Bluetooth LE Service
        //
        m_DeviceHandle = CreateFileW(DeviceFileName 
                                    , GENERIC_READ | GENERIC_WRITE
                                    , FILE_SHARE_READ | FILE_SHARE_WRITE
                                    , NULL
                                    , OPEN_EXISTING
                                    , 0
                                    , NULL
                                    );

        if (NULL == m_DeviceHandle ||
            INVALID_HANDLE_VALUE == m_DeviceHandle)
        {
            hr = HRESULT_FROM_WIN32(GetLastError());
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = m_DeviceContent.InitializeContent(pDevice, &m_dwLastObjectID, m_DeviceHandle, this);
        CHECK_HR(hr, "Failed to initialize device content");
    }

    if (SUCCEEDED(hr))
    {
        m_DeviceFileName = DeviceFileName;
    }
    
    return hr;
}

WpdGattService * BthLEDevice::GetGattService()
{
    return m_pGattService;
}

ACCESS_SCOPE BthLEDevice::GetAccessScope(
    _Out_   IPortableDeviceValues* pParams)
{
    ACCESS_SCOPE Scope      = FULL_DEVICE_ACCESS;
    LPWSTR       pszFileName = NULL;

    // For simplicity, our request filename is the same as the the service object ID
    if (pParams && (pParams->GetStringValue(PRIVATE_SAMPLE_DRIVER_REQUEST_FILENAME, &pszFileName) == S_OK))
    {
        CAtlStringW strRequestFilename = pszFileName;
        // For simplicity, our request filename is the same as the the service object ID
        // Case-insensitive comparison is required
        if (strRequestFilename.CompareNoCase(m_pGattService->GetRequestFilename()) == 0)
        {
            Scope = BLUETOOTH_GATT_SERVICE_ACCESS;
        }
    }

    CoTaskMemFree(pszFileName);
    return Scope;
}

HRESULT BthLEDevice::DispatchClientArrival()
{
    return m_DeviceContent.DispatchClientArrival();
}

HRESULT BthLEDevice::DispatchClientDeparture()
{
    return m_DeviceContent.DispatchClientArrival();
}

HRESULT BthLEDevice::DispatchClientApplicationActivated()
{
    return m_DeviceContent.DispatchClientApplicationActivated();
}

HRESULT BthLEDevice::DispatchClientApplicationSuspended()
{
    return m_DeviceContent.DispatchClientApplicationSuspended();
}

HRESULT BthLEDevice::DispatchDeviceConnected()
{
    return m_DeviceContent.DispatchDeviceConnected();
}

HRESULT BthLEDevice::DispatchDeviceDisconnected()
{
    return m_DeviceContent.DispatchDeviceDisconnected();
}


HRESULT BthLEDevice::GetSupportedCommands(
    _Out_   IPortableDeviceKeyCollection* pCommands)
{
    HRESULT hr = S_OK;

    if(pCommands == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    for (DWORD dwIndex = 0; dwIndex < ARRAYSIZE(g_SupportedCommands); dwIndex++)
    {
        PROPERTYKEY key = *(g_SupportedCommands[dwIndex]);
        hr = pCommands->Add(key);
        CHECK_HR(hr, "Failed to add supported command at index %d", dwIndex);
        if (FAILED(hr))
        {
            break;
        }
    }
    return hr;
}

HRESULT BthLEDevice::GetSupportedFunctionalCategories(
    _Out_   IPortableDevicePropVariantCollection* pFunctionalCategories)
{
    HRESULT hr = S_OK;

    if(pFunctionalCategories == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    // Device-wide command
    for (DWORD dwIndex = 0; dwIndex < ARRAYSIZE(g_SupportedFunctionalCategories); dwIndex++)
    {
        PROPVARIANT pv = {0};
        PropVariantInit(&pv);
        // Don't call PropVariantClear, since we did not allocate the memory for these GUIDs

        pv.vt    = VT_CLSID;
        pv.puuid = (CLSID*)g_SupportedFunctionalCategories[dwIndex];

        hr = pFunctionalCategories->Add(&pv);
        CHECK_HR(hr, "Failed to add supported functional category at index %d", dwIndex);
        if (FAILED(hr))
        {
            break;
        }
    }

    return hr;
}

HRESULT BthLEDevice::GetFunctionalObjects(
            REFGUID                               guidFunctionalCategory,
    _Inout_ IPortableDevicePropVariantCollection* pFunctionalObjects)
{
    HRESULT     hr = S_OK;
    PROPVARIANT pv = {0};

    if(pFunctionalObjects == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    PropVariantInit(&pv);
    // Don't call PropVariantClear, since we did not allocate the memory for these object identifiers

    // Add WPD_DEVICE_OBJECT_ID to the functional object identifiers collection
    if ((guidFunctionalCategory  == WPD_FUNCTIONAL_CATEGORY_DEVICE) ||
        (guidFunctionalCategory  == WPD_FUNCTIONAL_CATEGORY_ALL))
    {
        pv.vt       = VT_LPWSTR;
        pv.pwszVal  = WPD_DEVICE_OBJECT_ID;
        hr = pFunctionalObjects->Add(&pv);
        CHECK_HR(hr, "Failed to add device object ID");
    }

    // Add SERVICE_OBJECT_ID to the functional object identifiers collection
    if (hr == S_OK)
    {
        if ((guidFunctionalCategory  == SERVICE_GattService) ||
            (guidFunctionalCategory  == WPD_FUNCTIONAL_CATEGORY_ALL))
        {
            pv.vt       = VT_LPWSTR;
            pv.pwszVal  = SERVICE_OBJECT_ID;
            hr = pFunctionalObjects->Add(&pv);
            CHECK_HR(hr, "Failed to add the Gatt ervice object ID");
        }
    }

    return hr;
}

HRESULT BthLEDevice::GetSupportedFormats(
            REFGUID                               guidContentType,
    _Inout_ IPortableDevicePropVariantCollection* pFormats)
{

    UNREFERENCED_PARAMETER(guidContentType);
    UNREFERENCED_PARAMETER(pFormats);

    return S_OK;
}

HRESULT BthLEDevice::GetSupportedFormatProperties(
            REFGUID                       guidObjectFormat,
    _Inout_   IPortableDeviceKeyCollection* pKeys)
{
    HRESULT hr = S_OK;

    if(pKeys == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    if (guidObjectFormat == WPD_OBJECT_FORMAT_ALL)
    {
        for (DWORD dwIndex = 0; dwIndex < ARRAYSIZE(g_SupportedCommonProperties); dwIndex++)
        {
            PROPERTYKEY key = *g_SupportedCommonProperties[dwIndex];
            hr = pKeys->Add(key);
            CHECK_HR(hr, "Failed to add common property");
        }
    }

    return hr;
}

HRESULT BthLEDevice::GetFixedPropertyAttributes(
            REFGUID                guidObjectFormat,
            REFPROPERTYKEY         Key,
    _Out_   IPortableDeviceValues* pAttributes)
{
    UNREFERENCED_PARAMETER(guidObjectFormat);
    UNREFERENCED_PARAMETER(Key);

    HRESULT hr = S_OK;

    if(pAttributes == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    //
    // Since ALL of our properties have the same attributes, we are ignoring the
    // passed in guidObjectFormat and Key parameters.  These parameters allow you to
    // customize fixed property attributes for properties for specific formats.
    //

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
        hr = pAttributes->SetBoolValue(WPD_PROPERTY_ATTRIBUTE_CAN_WRITE, FALSE);
        CHECK_HR(hr, "Failed to set WPD_PROPERTY_ATTRIBUTE_CAN_WRITE");
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

    return hr;
}

HRESULT BthLEDevice::GetSupportedEvents(
    _Inout_   IPortableDevicePropVariantCollection* pEvents)
{
    UNREFERENCED_PARAMETER(pEvents);

    HRESULT hr = S_OK;

    if(pEvents == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }
    return hr;
}

HRESULT BthLEDevice::GetEventOptions(
    _Inout_   IPortableDeviceValues* pOptions)
{
    UNREFERENCED_PARAMETER(pOptions);

    HRESULT hr = S_OK;

    if(pOptions == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }
    return hr;
}

void BthLEDevice::InitializeEnumerationContext(
            ACCESS_SCOPE                Scope,
    _In_    LPCWSTR                     wszParentID,
    _Out_   WpdObjectEnumeratorContext* pEnumContext)
{
    if (pEnumContext == NULL)
    {
        return;
    }

    pEnumContext->m_Scope = Scope;
    pEnumContext->m_strParentObjectID = wszParentID;

    if (pEnumContext->m_strParentObjectID.GetLength() == 0)
    {
        // Clients passing an 'empty' string for the parent are asking for the
        // 'DEVICE' object.  We should return 1 child in this case.
        pEnumContext->m_TotalChildren = 1;
    }
    else
    {
        AbstractDeviceContent* pContent = NULL;
        HRESULT hr = GetContent(Scope, wszParentID, &pContent);
        if (hr == S_OK)
        {
            hr = pContent->InitializeEnumerationContext(Scope, pEnumContext);
            CHECK_HR(hr, "Failed to initialize enuemration context for '%ws'", wszParentID);
        }

        if (hr != S_OK)
        {
            // Invalid, or non-existing objects contain no children.
            pEnumContext->m_TotalChildren = 0;
        }
    }
}

HRESULT BthLEDevice::FindNext(
            const DWORD                           dwNumObjectsRequested,
    _In_    WpdObjectEnumeratorContext*           pEnumContext,
    _Inout_ IPortableDevicePropVariantCollection* pObjectIDCollection,
    _Out_   DWORD*                                pdwNumObjectsEnumerated)
{
    HRESULT hr                   = S_OK;
    DWORD   NumObjectsEnumerated = 0;

    if ((pEnumContext        == NULL) ||
        (pObjectIDCollection == NULL))
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    if (NULL != pdwNumObjectsEnumerated) 
    {
        *pdwNumObjectsEnumerated = 0;
    }
        

    // If the enumeration context reports that their are more objects to return, then continue, if not,
    // return an empty results set.
    if (pEnumContext->HasMoreChildrenToEnumerate())
    {
        if (pEnumContext->m_strParentObjectID.CompareNoCase(L"") == 0)
        {
            // We are being asked for the device
            hr = AddStringValueToPropVariantCollection(pObjectIDCollection, m_DeviceContent.ObjectID);
            CHECK_HR(hr, "Failed to add 'DEVICE' object ID to enumeration collection");

            // Update the the number of children we are returning for this enumeration call
            NumObjectsEnumerated++;
        }
        else
        {
            AbstractDeviceContent* pContent = NULL;
            HRESULT hrGet = GetContent(pEnumContext->m_Scope, pEnumContext->m_strParentObjectID, &pContent);
            CHECK_HR(hrGet, "Failed to get content '%ws'", pEnumContext->m_strParentObjectID);

            if (hrGet == S_OK)
            {
                DWORD dwStartIndex = pEnumContext->m_ChildrenEnumerated;
                for (DWORD i=0; i<dwNumObjectsRequested; i++)
                {
                    AbstractDeviceContent* pChild = NULL;
                    if (pContent->FindNext(pEnumContext->m_Scope, dwStartIndex, &pChild))
                    {
                        hr = AddStringValueToPropVariantCollection(pObjectIDCollection, pChild->ObjectID);
                        CHECK_HR(hr, "Failed to add object [%ws]", pChild->ObjectID);

                        if (hr == S_OK)
                        {
                            // Update the the number of children we are returning for this enumeration call
                            dwStartIndex++;
                            NumObjectsEnumerated++;
                        }
                    }
                    else
                    {
                        // no more children
                        break;
                    }
                }
            }
        }
    }

    if (hr == S_OK && pdwNumObjectsEnumerated)
    {
        *pdwNumObjectsEnumerated = NumObjectsEnumerated;
    }

    return hr;
}

HRESULT BthLEDevice::GetObjectIDsByFormat(
            ACCESS_SCOPE                          Scope,
            REFGUID                               guidObjectFormat,
    _In_    LPCWSTR                               wszParentObjectID,
            const DWORD                           dwDepth,
    _Inout_ IPortableDevicePropVariantCollection* pObjectIDs)
{
    HRESULT      hr       = S_OK;
    AbstractDeviceContent* pContent = NULL;

    if(pObjectIDs == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, ("Cannot have NULL parameter"));
        return hr;
    }

    hr = GetContent(Scope, wszParentObjectID, &pContent);
    CHECK_HR(hr, "Failed to get content '%ws'", wszParentObjectID);

    if (hr == S_OK)
    {
        hr = pContent->GetObjectIDsByFormat(Scope, guidObjectFormat, dwDepth, pObjectIDs);
        CHECK_HR(hr, "Failed to get object IDs by format");
    }

    return hr;
}

HRESULT BthLEDevice::GetObjectIDsFromPersistentUniqueIDs(
            ACCESS_SCOPE                          Scope,
    _In_    IPortableDevicePropVariantCollection* pPersistentIDs,
    _Inout_ IPortableDevicePropVariantCollection* pObjectIDs)
{
    HRESULT hr      = S_OK;
    DWORD   dwCount = 0;

    if ((pPersistentIDs == NULL) ||
        (pObjectIDs     == NULL))
    {
        hr = E_POINTER;
        CHECK_HR(hr, ("Cannot have NULL parameter"));
        return hr;
    }

    // Iterate through the persistent ID list and add the equivalent object ID for each element.
    hr = pPersistentIDs->GetCount(&dwCount);
    CHECK_HR(hr, "Failed to get count from persistent ID collection");

    if (hr == S_OK)
    {
        PROPVARIANT pvPersistentID = {0};

        for(DWORD dwIndex = 0; dwIndex < dwCount; dwIndex++)
        {
            PropVariantInit(&pvPersistentID);

            hr = pPersistentIDs->GetAt(dwIndex, &pvPersistentID);
            CHECK_HR(hr, "Failed to get persistent ID at index %d", dwIndex);

            if (hr == S_OK)
            {
                hr = m_DeviceContent.GetObjectIDByPersistentID(Scope, pvPersistentID.pwszVal, pObjectIDs);
                CHECK_HR(hr, "Failed to get object ID from persistent unique ID '%ws'", pvPersistentID.pwszVal);
            }

            if (hr == HRESULT_FROM_WIN32(ERROR_NOT_FOUND))
            {
                PROPVARIANT pvEmptyObjectID = {0};
                pvEmptyObjectID.vt = VT_LPWSTR;
                pvEmptyObjectID.pwszVal = L"";

                // Insert empty string when object cannot be found
                hr = pObjectIDs->Add(&pvEmptyObjectID);
                CHECK_HR(hr, "Failed to set empty string for persistent unique ID '%ws' when object cannot be found", pvPersistentID.pwszVal);
            }

            PropVariantClear(&pvPersistentID);

            if(FAILED(hr))
            {
                break;
            }
        }
    }

    return hr;
}

HRESULT BthLEDevice::GetSupportedProperties(
            ACCESS_SCOPE                  Scope,
    _In_    LPCWSTR                       wszObjectID,
    _Inout_ IPortableDeviceKeyCollection* pKeys)
{
    HRESULT      hr       = S_OK;
    AbstractDeviceContent* pContent = NULL;

    if ((wszObjectID == NULL) ||
        (pKeys       == NULL))
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    hr = GetContent(Scope, wszObjectID, &pContent);
    CHECK_HR(hr, "Failed to get content '%ws'", wszObjectID);

    if (hr == S_OK)
    {
        hr = pContent->GetSupportedProperties(pKeys);
        CHECK_HR(hr, "Failed to get supported properties for '%ws'", wszObjectID);
    }

    return hr;
}

HRESULT BthLEDevice::GetAllPropertyValues(
            ACCESS_SCOPE                   Scope,
    _In_    LPCWSTR                        wszObjectID,
    _Inout_ IPortableDeviceValues*         pValues)
{
    HRESULT      hr       = S_OK;
    AbstractDeviceContent* pContent = NULL;

    if ((wszObjectID == NULL) ||
        (pValues     == NULL))
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    hr = GetContent(Scope, wszObjectID, &pContent);
    CHECK_HR(hr, "Failed to get content '%ws'", wszObjectID);

    if (hr == S_OK)
    {
        hr = pContent->GetAllValues(pValues);
        CHECK_HR(hr, "Failed to get all property values for '%ws'", wszObjectID);
    }
    return hr;
}


HRESULT BthLEDevice::GetPropertyValues(
            ACCESS_SCOPE                   Scope,
    _In_    LPCWSTR                        wszObjectID,
    _In_    IPortableDeviceKeyCollection*  pKeys,
    _Inout_ IPortableDeviceValues*         pValues)
{
    HRESULT      hrReturn = S_OK;
    HRESULT      hr       = S_OK;
    DWORD        cKeys    = 0;
    AbstractDeviceContent* pContent = NULL;

    if ((wszObjectID == NULL) ||
        (pKeys       == NULL) ||
        (pValues     == NULL))
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    hr = GetContent(Scope, wszObjectID, &pContent);
    CHECK_HR(hr, "Failed to get content '%ws'", wszObjectID);

    if (hr == S_OK)
    {
        hr = pKeys->GetCount(&cKeys);
        CHECK_HR(hr, "Failed to number of PROPERTYKEYs in collection");
    }

    if (hr == S_OK)
    {
        for (DWORD dwIndex = 0; dwIndex < cKeys; dwIndex++)
        {
            PROPERTYKEY Key = WPD_PROPERTY_NULL;
            hr = pKeys->GetAt(dwIndex, &Key);
            CHECK_HR(hr, "Failed to get PROPERTYKEY at index %d in collection", dwIndex);

            if (hr == S_OK)
            {
                hr = pContent->GetValue(Key, pValues);
                CHECK_HR(hr, "Failed to get property at index %d", dwIndex);
                if (FAILED(hr))
                {
                    // Mark the property as failed by setting the error value
                    // hrReturn is marked as S_FALSE indicating that at least one property has failed.
                    hr = pValues->SetErrorValue(Key, hr);
                    hrReturn = S_FALSE;
                }
            }
        }
    }

    if (FAILED(hr))
    {
        // A general error has occurred (rather than failure to set one or more properties)
        hrReturn = hr;
    }

    return hrReturn;
}

HRESULT BthLEDevice::SetPropertyValues(
            ACCESS_SCOPE           Scope,
    _In_    LPCWSTR                wszObjectID,
    _In_    IPortableDeviceValues* pValues,
    _Inout_ IPortableDeviceValues* pResults,
    _Inout_ IPortableDeviceValues* pEventParams,
    _Out_   bool*                  pbObjectChanged)
{
    HRESULT      hr             = S_OK;
    AbstractDeviceContent* pContent       = NULL;

    if ((wszObjectID     == NULL) ||
        (pValues         == NULL) ||
        (pResults        == NULL) ||
        (pEventParams    == NULL) ||
        (pbObjectChanged == NULL))
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }
    *pbObjectChanged = FALSE;

    hr = GetContent(Scope, wszObjectID, &pContent);
    CHECK_HR(hr, "Failed to get content '%ws'", wszObjectID);

    if (hr == S_OK)
    {
        hr = pContent->WriteValues(pValues, pResults, pbObjectChanged);
        CHECK_HR(hr, "Failed to write value for '%ws'", wszObjectID);

        if (SUCCEEDED(hr) && (*pbObjectChanged))  // hr can be S_OK or S_FALSE (if one or more property writes failed)
        {
            HRESULT hrEvent = pEventParams->SetGuidValue(WPD_EVENT_PARAMETER_EVENT_ID, WPD_EVENT_OBJECT_UPDATED);
            CHECK_HR(hrEvent, "Failed to add WPD_EVENT_PARAMETER_EVENT_ID");

            if (hrEvent == S_OK)
            {
                hrEvent = pEventParams->SetStringValue(WPD_OBJECT_PERSISTENT_UNIQUE_ID, pContent->PersistentUniqueID);
                CHECK_HR(hrEvent, "Failed to add WPD_OBJECT_PERSISTENT_UNIQUE_ID");
            }

            if (hrEvent == S_OK)
            {
                hrEvent = pEventParams->SetStringValue(WPD_EVENT_PARAMETER_OBJECT_PARENT_PERSISTENT_UNIQUE_ID, pContent->ParentPersistentUniqueID);
                CHECK_HR(hrEvent, "Failed to add WPD_EVENT_PARAMETER_OBJECT_PARENT_PERSISTENT_UNIQUE_ID");
            }

            if (hrEvent == S_OK)
            {
                // Adding this event parameter will allow WPD to scope this event to the container functional object
                hrEvent = pEventParams->SetStringValue(WPD_OBJECT_CONTAINER_FUNCTIONAL_OBJECT_ID, pContent->ContainerFunctionalObjectID);
                CHECK_HR(hrEvent, "Failed to add WPD_OBJECT_CONTAINER_FUNCTIONAL_OBJECT_ID");
            }
        }

    }

    return hr;
}

HRESULT BthLEDevice::GetPropertyAtributes(
            ACCESS_SCOPE           Scope,
    _In_    LPCWSTR                wszObjectID,
            REFPROPERTYKEY         Key,
    _Inout_ IPortableDeviceValues* pAttributes)
{
    HRESULT      hr       = S_OK;
    AbstractDeviceContent* pContent = NULL;

    if ((wszObjectID == NULL) ||
        (pAttributes == NULL))
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    hr = GetContent(Scope, wszObjectID, &pContent);
    CHECK_HR(hr, "Failed to get content '%ws'", wszObjectID);

    if (hr == S_OK)
    {
        hr = pContent->GetPropertyAttributes(Key, pAttributes);
        CHECK_HR(hr, "Failed to get property attributes for '%ws'", wszObjectID);
    }

    return hr;
}

HRESULT BthLEDevice::GetContent(
            ACCESS_SCOPE   Scope,
    _In_    LPCWSTR        wszObjectID,
    _Out_   AbstractDeviceContent**  ppContent)
{
    HRESULT hr = S_OK;

    if (ppContent == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
    }

    *ppContent = NULL;

    hr = m_DeviceContent.GetContent(Scope, wszObjectID, ppContent);
    CHECK_HR(hr, "Failed to get content '%ws'", wszObjectID);

    return hr;
}

