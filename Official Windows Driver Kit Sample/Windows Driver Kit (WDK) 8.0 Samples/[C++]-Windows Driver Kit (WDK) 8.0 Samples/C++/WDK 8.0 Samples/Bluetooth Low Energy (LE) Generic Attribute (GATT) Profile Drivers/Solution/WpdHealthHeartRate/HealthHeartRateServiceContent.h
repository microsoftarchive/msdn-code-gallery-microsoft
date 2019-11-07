/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    HealthHeartRateServiceContent.h

Abstract:

    Contains Health HeartRate Service Declaration.

--*/


#pragma once

class HealthHeartRateServiceContent : public AbstractDeviceContent
{
public:
    HealthHeartRateServiceContent()
    {
        ObjectID                    = SERVICE_OBJECT_ID;
        PersistentUniqueID          = SERVICE_PERSISTENT_UNIQUE_ID;
        ParentID                    = WPD_DEVICE_OBJECT_ID;
        Name                        = SERVICE_OBJECT_NAME_VALUE;
        ContentType                 = WPD_CONTENT_TYPE_UNSPECIFIED;
        Format                      = WPD_OBJECT_FORMAT_UNSPECIFIED;
        ParentPersistentUniqueID    = WPD_DEVICE_OBJECT_ID;
        ContainerFunctionalObjectID = WPD_DEVICE_OBJECT_ID;
        Version                     = SERVICE_VERSION;
        FunctionalCategory          = SERVICE_HealthHeartRateService;
        HumanReadableName           = SERVICE_HUMAN_READABLE_NAME;
        RequiredScope               = BLUETOOTH_GATT_SERVICE_ACCESS;

        CComCritSecLock<CComAutoCriticalSection> Lock(m_CCCDSetCS);
        m_bCCCDSet                  = FALSE;
    }

    HealthHeartRateServiceContent(const AbstractDeviceContent& src) : m_ValueChangeEventReg(NULL)
    {
        *this = src;
    }

    virtual ~HealthHeartRateServiceContent();

    virtual HRESULT InitializeContent(
        _In_ IWDFDevice* pDevice,
        _Inout_ DWORD *pdwLastObjectID,
        _In_ HANDLE hDeviceHandle,
        _In_ BthLEDevice * pBthLEDevice);

    virtual HRESULT CreatePropertiesOnlyObject(
        _In_    IPortableDeviceValues* pObjectProperties,
        _Inout_ DWORD*                 pdwLastObjectID,
        _Inout_ AbstractDeviceContent**          ppNewObject);

    virtual HRESULT GetSupportedProperties(
        _Inout_ IPortableDeviceKeyCollection *pKeys);

    virtual HRESULT GetPropertyAttributes(
                REFPROPERTYKEY         Key,
        _Inout_ IPortableDeviceValues* pAttributes);

    virtual HRESULT GetValue(
                REFPROPERTYKEY         Key,
        _Inout_ IPortableDeviceValues* pStore);

    virtual HRESULT DispatchClientArrival();

    virtual HRESULT DispatchClientDeparture();

    virtual HRESULT DispatchDeviceConnected();

    virtual HRESULT DispatchDeviceDisconnected();

    static VOID WINAPI s_HeartRateMeasurementEvent(
        _In_ BTH_LE_GATT_EVENT_TYPE EventType,
        _In_ PVOID EventOutParameter,
        _In_ PVOID Context
        );

    HRESULT SetCCCD();

public:
    CAtlStringW                 Version;
    CAtlStringW                 HumanReadableName;
    GUID                        FunctionalCategory;

private:
    HANDLE                      m_hDeviceHandle;

    BTH_LE_GATT_CHARACTERISTIC  m_HeartRateMeasurementCharacteristic;
    BLUETOOTH_GATT_EVENT_HANDLE m_ValueChangeEventReg;
    CComPtr<IWpdSerializer>     m_pWpdSerializer;
    CComPtr<IWDFDevice>         m_pDevice;
    BthLEDevice *               m_pBthLEDevice;

    BOOLEAN                     m_bCCCDSet;
    CComAutoCriticalSection     m_CCCDSetCS;
};

