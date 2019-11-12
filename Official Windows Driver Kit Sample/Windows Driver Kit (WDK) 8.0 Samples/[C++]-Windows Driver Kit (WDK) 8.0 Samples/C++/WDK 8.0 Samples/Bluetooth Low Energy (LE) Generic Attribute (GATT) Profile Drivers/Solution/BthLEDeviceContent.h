/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    BthLEDeviceContent.h

Abstract:

    Contains the WPD Device content declaration

--*/

#pragma once

#define DEVICE_POWER_LEVEL_VALUE                          100

#define DEVICE_SUPPORTS_NONCONSUMABLE_VALUE               FALSE

class BthLEDeviceContent : public AbstractDeviceContent
{
public:
    BthLEDeviceContent()
    {
        ObjectID                    = WPD_DEVICE_OBJECT_ID;
        PersistentUniqueID          = WPD_DEVICE_OBJECT_ID;
        ParentID                    = L"";
        ParentPersistentUniqueID    = L"";
        ContainerFunctionalObjectID = L"";
        Name                        = WPD_DEVICE_OBJECT_ID;
        Protocol                    = DEVICE_PROTOCOL_NAME;
        FirmwareVersion             = L""; // Get from the "Device Information Service"
        Model                       = L""; // Get from the "Device Information Service"
        Manufacturer                = L""; // Get from the "Device Information service"
        FriendlyName                = DEVICE_FRIENDLY_NAME;
        SerialNumber                = L""; // Get from the "Device information service"
        PowerLevel                  = 50;
        PowerSource                 = WPD_POWER_SOURCE_BATTERY;
        DeviceType                  = WPD_DEVICE_TYPE_GENERIC;
        Format                      = WPD_OBJECT_FORMAT_UNSPECIFIED;
        ContentType                 = WPD_CONTENT_TYPE_FUNCTIONAL_OBJECT;
        FunctionalCategory          = WPD_FUNCTIONAL_CATEGORY_DEVICE;
        RequiredScope               = BLUETOOTH_GATT_SERVICE_ACCESS;
        SupportsNonConsumable       = FALSE;
    }

    virtual ~BthLEDeviceContent()
    {
    }

    BthLEDeviceContent(const AbstractDeviceContent& src)
    {
        *this = src;
    }

    virtual HRESULT InitializeContent(
        _In_ IWDFDevice* pDevice,
        _Inout_ DWORD *pdwLastObjectID,
        _In_ HANDLE hDeviceHandle,
        _In_ BthLEDevice * pBthLEDevice);

    virtual HRESULT InitializeEnumerationContext(
                ACCESS_SCOPE                Scope,
        _Out_   WpdObjectEnumeratorContext* pEnumeratorContext);

    // Property Management
    virtual HRESULT GetSupportedProperties(
        _Out_   IPortableDeviceKeyCollection*         pKeys);

    virtual HRESULT GetValue(
                REFPROPERTYKEY                        Key, 
        _Out_   IPortableDeviceValues*                pStore);

public:
    CAtlStringW  Protocol;
    CAtlStringW  FirmwareVersion;
    CAtlStringW  Model;
    CAtlStringW  FriendlyName;
    CAtlStringW  SerialNumber;
    CAtlStringW  Manufacturer;

    GUID         FunctionalCategory;
    BOOL         SupportsNonConsumable;
    DWORD        PowerLevel;
    DWORD        PowerSource;
    DWORD        DeviceType;

private:
    HANDLE       m_hDeviceHandle;
        
};

