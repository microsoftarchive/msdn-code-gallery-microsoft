/*++

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

Copyright (c) Microsoft Corporation. All rights reserved

Module Name:

    SensorDdi.h

Abstract:

    This module contains the type definitions for the ISensorDriver
    interface which is used by the Sensor Class Extension.

--*/

#ifndef _SENSORDDI_H_
#define _SENSORDDI_H_

#pragma once

// Forward declarations
class CClientManager;
class CReportManager;


class CSensorDdi : 
    public CComObjectRoot,
    public ISensorDriver,
    public ISensorDeviceCallback
{
public:
    CSensorDdi();
    virtual ~CSensorDdi();

    DECLARE_NOT_AGGREGATABLE(CSensorDdi)

    BEGIN_COM_MAP(CSensorDdi)
        COM_INTERFACE_ENTRY(ISensorDriver)
        COM_INTERFACE_ENTRY(ISensorDeviceCallback)
    END_COM_MAP()

// Public methods
public:
    HRESULT Initialize(
        _In_ IWDFDevice* pWdfDevice,
        _In_ IWDFCmResourceList * pWdfResourcesRaw,
        _In_ IWDFCmResourceList * pWdfResourcesTranslated);
    VOID Uninitialize();
    HRESULT SetSensorClassExtension(
        _In_ ISensorClassExtension* pClassExtension);
    HRESULT Start();
    HRESULT Stop();
    HRESULT ReportIntervalExpired();

// COM Interface methods
public:
    // ISensorDriver methods
    HRESULT STDMETHODCALLTYPE OnGetSupportedSensorObjects(
        _Out_ IPortableDeviceValuesCollection** ppSensorObjectCollection
        );

    HRESULT STDMETHODCALLTYPE OnGetSupportedProperties(
        _In_  LPWSTR pwszObjectID,
        _Out_ IPortableDeviceKeyCollection** ppSupportedProperties
        );

    HRESULT STDMETHODCALLTYPE OnGetSupportedDataFields(
        _In_  LPWSTR pwszObjectID,
        _Out_ IPortableDeviceKeyCollection** ppSupportedDataFields
        );

    HRESULT STDMETHODCALLTYPE OnGetSupportedEvents(
        _In_  LPWSTR pwszObjectID,
        _Out_ GUID** ppSupportedEvents,
        _Out_ ULONG* pulEventCount
        );

    HRESULT STDMETHODCALLTYPE OnGetProperties(
        _In_  IWDFFile* pClientFile,
        _In_  LPWSTR pwszObjectID,
        _In_  IPortableDeviceKeyCollection* pProperties,
        _Out_ IPortableDeviceValues** ppPropertyValues
        );

    HRESULT STDMETHODCALLTYPE OnGetDataFields(
        _In_  IWDFFile* pClientFile,
        _In_  LPWSTR pwszObjectID,
        _In_  IPortableDeviceKeyCollection* pDataFields,
        _Out_ IPortableDeviceValues** ppDataValues
        );

    HRESULT STDMETHODCALLTYPE OnSetProperties(
        _In_  IWDFFile* pClientFile,
        _In_  LPWSTR pwszObjectID,
        _In_  IPortableDeviceValues* pPropertiesToSet,
        _Out_ IPortableDeviceValues** ppResults
        );

    HRESULT STDMETHODCALLTYPE OnClientConnect(
        _In_ IWDFFile* pClientFile,
        _In_ LPWSTR pwszObjectID
        );

    HRESULT STDMETHODCALLTYPE OnClientDisconnect(
        _In_ IWDFFile* pClientFile,
        _In_ LPWSTR pwszObjectID
        );

    HRESULT STDMETHODCALLTYPE OnClientSubscribeToEvents(
        _In_ IWDFFile* pClientFile,
        _In_ LPWSTR pwszObjectID
        );

    HRESULT STDMETHODCALLTYPE OnClientUnsubscribeFromEvents(
        _In_ IWDFFile* pClientFile,
        _In_ LPWSTR pwszObjectID
        );

    HRESULT STDMETHODCALLTYPE OnProcessWpdMessage(
        _In_ IUnknown* pUnkPortableDeviceValuesParams,
        _In_ IUnknown* pUnkPortableDeviceValuesResults
        );

    // ISensorDeviceCallback methods
    HRESULT STDMETHODCALLTYPE OnNewData(
        _In_ IPortableDeviceValues* pValues
        );

// Private methods
private:
    // Initialization
    HRESULT InitializeSensorDevice(
        _In_ IWDFDevice* pWdfDevice,
        _In_ IWDFCmResourceList * pWdfResourcesRaw,
        _In_ IWDFCmResourceList * pWdfResourcesTranslated);
    HRESULT InitializeSensorDriverInterface(_In_ IWDFDevice* pWdfDevice);
    
    LPWSTR GetSensorObjectID();
    HRESULT AddPropertyKeys();
    HRESULT AddDataFieldKeys();
    HRESULT SetDefaultPropertyValues();
    HRESULT SetUniqueID(_In_ IWDFDevice* pWdfDevice);

    // Steadystate methods
    HRESULT SetState(_In_ SensorState newState);
    BOOL HasStateChanged();
    HRESULT SetTimeStamp();
    HRESULT ApplyUpdatedProperties();
    HRESULT SetDataUpdateMode(_In_ DATA_UPDATE_MODE Mode);

    // Helper methods
    HRESULT GetProperty(
        _In_  REFPROPERTYKEY key, 
        _Out_ PROPVARIANT *pVar);

    HRESULT GetDataField(
        _In_  REFPROPERTYKEY key, 
        _Out_ PROPVARIANT *pVar);

    HRESULT GetAllDataFields(
        _Inout_ IPortableDeviceValues* pValues);

    HRESULT CopyKeys(
        _In_    IPortableDeviceKeyCollection *pSourceKeys,
        _Inout_ IPortableDeviceKeyCollection *pTargetKeys);

    BOOL IsPerDataFieldProperty(PROPERTYKEY key);
    BOOL IsTestProperty(PROPERTYKEY key);

    HRESULT PollForData();

    // Data eventing methods
    VOID RaiseDataEvent();
    HRESULT PostDataEvent(_In_ IPortableDeviceValues *pValues);


private:    
    // PropertyKeys
    CComPtr<IPortableDeviceKeyCollection>  m_spSupportedSensorProperties;
    CComPtr<IPortableDeviceKeyCollection>  m_spSupportedSensorDataFields;
    BOOL                                   m_fStateChanged;

    // Values
    CComPtr<IPortableDeviceValues>         m_spSensorPropertyValues;
    CComPtr<IPortableDeviceValues>         m_spSensorDataFieldValues;
        
    // Make calls to get/set properties thread safe
    CComAutoCriticalSection                m_CacheCriticalSection;

    // Make calls to client thread safe
    CComAutoCriticalSection                m_ClientCriticalSection;

    // Interface pointer to the class extension, sensor device, and IWDFDevice
    CComPtr<ISensorClassExtension>         m_spClassExtension;
    CComPtr<ISensorDevice>                 m_spSensorDevice;
    CComPtr<IWDFDevice2>                   m_spWdfDevice2;

    // Client and report managers
    CComObject<CClientManager>*            m_pClientManager;
    CComObject<CReportManager>*            m_pReportManager;


    // TODO: Remove this when interface is registered
    CComObject<CAccelerometerDevice>*      m_pAccelerometerDevice;

    // Track sensor object state
    BOOL                                   m_fSensorInitialized;
    DATA_UPDATE_MODE                       m_DataUpdateMode;
};

#endif // _SENSORDDI_H_
