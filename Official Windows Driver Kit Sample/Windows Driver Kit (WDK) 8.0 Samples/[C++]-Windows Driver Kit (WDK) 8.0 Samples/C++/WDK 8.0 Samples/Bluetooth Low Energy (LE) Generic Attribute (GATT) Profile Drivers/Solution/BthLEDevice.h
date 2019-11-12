/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    BthLEDevice.h

Abstract:

    This class represents an abstraction of a real device.
    Driver implementors should replace this with their own
    device I/O classes/libraries.


--*/


#pragma once

class BthLEDevice
{
public:
    BthLEDevice() : 
        m_dwLastObjectID(0),
        m_DeviceHandle(NULL)
    {
    }

    ~BthLEDevice()
    {
        if (NULL != m_DeviceHandle)
        {
            CloseHandle(m_DeviceHandle);
        }

        if (NULL != m_pGattService)
        {
            delete m_pGattService;
            m_pGattService = NULL;
        }
            
    }

    HRESULT InitializeContent(_In_ IWDFDevice* pDevice, _In_ PCWSTR DeviceFileName);

    WpdGattService* GetGattService();

    ACCESS_SCOPE GetAccessScope(
        _Out_   IPortableDeviceValues*                pParams);

    HRESULT DispatchClientArrival();

    HRESULT DispatchClientDeparture();

    HRESULT DispatchClientApplicationActivated();

    HRESULT DispatchClientApplicationSuspended();

    HRESULT DispatchDeviceConnected();

    HRESULT DispatchDeviceDisconnected();

    // Device Capabilities
    // These are legacy commands that apply to the whole device, no access scope is required
    HRESULT GetSupportedCommands(
        _Out_   IPortableDeviceKeyCollection*         pCommands);

    HRESULT GetSupportedFunctionalCategories(
        _Out_   IPortableDevicePropVariantCollection* pFunctionalCategories);
   
    HRESULT GetFunctionalObjects(
                REFGUID                               guidFunctionalCategory,
        _Inout_ IPortableDevicePropVariantCollection* pFunctionalObjects);
   
    HRESULT GetSupportedContentTypes(
                REFGUID                               guidFunctionalCategory,
        _Out_   IPortableDevicePropVariantCollection* pContentTypes);
   
    HRESULT GetSupportedFormats(
                REFGUID                               guidContentType,
        _Inout_ IPortableDevicePropVariantCollection* pFormats);
    
    HRESULT GetSupportedFormatProperties(
                REFGUID                               guidObjectFormat, 
        _Inout_   IPortableDeviceKeyCollection*         pKeys);
    
    HRESULT GetFixedPropertyAttributes(
                REFGUID                               guidObjectFormat, 
                REFPROPERTYKEY                        Key, 
        _Out_   IPortableDeviceValues* pAttributes);
    
    HRESULT GetSupportedEvents(
        _Inout_   IPortableDevicePropVariantCollection* pEvents);
    
    HRESULT GetEventOptions(
        _Inout_   IPortableDeviceValues*                pOptions);

    // Enumeration
    // Depending on the access scope, the driver can display only objects within the current
    // scoped hierarchy tree
    void InitializeEnumerationContext(
                ACCESS_SCOPE                          Scope, 
        _In_    LPCWSTR                               wszParentID, 
        _Out_   WpdObjectEnumeratorContext*           pEnumContext);
  
    HRESULT FindNext(
                const DWORD                           dwNumObjectsRequested, 
        _In_    WpdObjectEnumeratorContext*           pEnumContext, 
        _Inout_ IPortableDevicePropVariantCollection* pObjectIDCollection, 
        _Out_   DWORD*                                pdwNumObjectsEnumerated);

    HRESULT GetObjectIDsByFormat(
                ACCESS_SCOPE                          Scope,
                REFGUID                               guidObjectFormat,
        _In_    LPCWSTR                               wszParentObjectID,
                const DWORD                           dwDepth,
        _Inout_ IPortableDevicePropVariantCollection* pObjectIDs);

    HRESULT GetObjectIDsFromPersistentUniqueIDs(
                ACCESS_SCOPE                          Scope,
        _In_    IPortableDevicePropVariantCollection* pPersistentIDs,
        _Inout_ IPortableDevicePropVariantCollection* pObjectIDs);

    // Property Management
    // Depending on the access scope, the driver can allow access to properties of objects within the current
    // scoped hierarchy tree
    HRESULT GetSupportedProperties(
                ACCESS_SCOPE                          Scope, 
        _In_    LPCWSTR                               wszObjectID, 
        _Inout_ IPortableDeviceKeyCollection*         pKeys);

    HRESULT GetAllPropertyValues(
                ACCESS_SCOPE                          Scope,
        _In_    LPCWSTR                               wszObjectID,
        _Inout_ IPortableDeviceValues*                pValues);

    HRESULT GetPropertyValues(
                ACCESS_SCOPE                          Scope,
        _In_    LPCWSTR                               wszObjectID,
        _In_    IPortableDeviceKeyCollection*         pKeys,
        _Inout_ IPortableDeviceValues*                pValues);

    HRESULT SetPropertyValues(
                ACCESS_SCOPE                          Scope,
        _In_    LPCWSTR                               wszObjectID,
        _In_    IPortableDeviceValues*                pValues,
        _Inout_ IPortableDeviceValues*                pResults, 
        _Inout_ IPortableDeviceValues*                pEventParams,
        _Out_   bool*                                 pbObjectChanged);

    HRESULT GetPropertyAtributes(
                ACCESS_SCOPE                          Scope,
        _In_    LPCWSTR                               wszObjectID,
                REFPROPERTYKEY                        Key,
        _Inout_ IPortableDeviceValues*                pAttributes);


private:
    HRESULT GetContent(
                ACCESS_SCOPE                          Scope,
        _In_    LPCWSTR                               wszObjectID,
        _Out_   AbstractDeviceContent**                         ppContent);

    HRESULT RemoveObjectsMarkedForDeletion();

private:

    BthLEDeviceContent   m_DeviceContent;
    WpdGattService *     m_pGattService;

    // FileName of the LE device
    CAtlStringW         m_DeviceFileName;
    HANDLE              m_DeviceHandle;

    DWORD               m_dwLastObjectID;
};

