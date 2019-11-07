/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Sensor.h
//
// Description:
//      Defines the CSensor container class

--*/


#pragma once


class CSensor;      //forward declaration


class CSensor
{
protected:
    CSensorManager* m_pSensorManager;

public:
    CSensor();
    ~CSensor();
    
    HRESULT     InitializeSensor(
                                _In_ SensorType sensType, 
                                _In_ DWORD sensNum,
                                _In_ LPWSTR pwszManufacturer,
                                _In_ LPWSTR pwszProduct,
                                _In_ LPWSTR pwszSerialNumber,
                                _In_ LPWSTR sensorID,
                                _In_ IWDFDevice* pWdfDevice);
    HRESULT     Uninitialize(VOID);

    HRESULT     GetSupportedProperties(IPortableDeviceKeyCollection **ppKeys);
    HRESULT     GetSupportedDataFields(IPortableDeviceKeyCollection **ppKeys);
    HRESULT     GetRequiredDataFields(IPortableDeviceKeyCollection **ppKeys);

    HRESULT     GetSettableProperties(IPortableDeviceKeyCollection **ppKeys);

    DWORD       GetSubscriberCount(void);
    SensorType  GetSensorType(void){ return m_SensorType; }
    ULONG       GetSensorNumber(void){ return m_SensorNum; }
    CComBSTR    GetSensorObjectID(void) { return m_pSensorManager->m_AvailableSensorsIDs[m_SensorNum]; }
 
    HRESULT     GetProperty(REFPROPERTYKEY key, PROPVARIANT *pValue);
    HRESULT     GetDataField(REFPROPERTYKEY key, PROPVARIANT *pValue);
    HRESULT     GetAllDataFieldValues(IPortableDeviceValues* pValues);

    HRESULT     SetProperty(REFPROPERTYKEY key, const PROPVARIANT *pValue, IPortableDeviceValues* spDfVals);
    HRESULT     SetDataField(REFPROPERTYKEY key, const PROPVARIANT *pValue);
    HRESULT     SetTimeStamp();

    HRESULT     Subscribe(_In_ IWDFFile* appID);
    HRESULT     Unsubscribe(_In_ IWDFFile* appID);

    HRESULT     RemoveProperty(REFPROPERTYKEY key);
    HRESULT     RemoveDataField(REFPROPERTYKEY key);

    BOOL        IsInitialized(VOID) { return m_fSensorInitialized; }

    VOID        RaiseDataEvent();
    BOOL        HasValidDataEvent();

    VOID        SetDataEventHandle(HANDLE hEvent) { m_hSensorEvent = hEvent; }

    virtual HRESULT SetUniqueID(_In_ IWDFDevice* pWdfDevice);


    HRESULT     InitPerDataFieldProperties(
                                                    _In_ PROPERTYKEY pkDataField);

    HRESULT     GetPropertyValuesForSensorObject(
                                                    _In_ LPCWSTR                        wszObjectID,
                                                    _In_ IPortableDeviceKeyCollection*  pKeys,
                                                    _In_ IPortableDeviceValues*         pValues,
                                                    _In_ LPCWSTR                        wszSensorName,
                                                    _In_ GUID                           guidSensorCategory,
                                                    _Out_ BOOL*                         pfError);

    FLOAT       GetRangeMaximumValue(
                                                    _In_ FLOAT fltDefaultRangeMaximum,
                                                    _In_ BOOL fSpecificMaximumSupported,
                                                    _In_ FLOAT fltSpecificMaximum,
                                                    _In_ BOOL fBulkMaximumSupported,
                                                    _In_ FLOAT fltBulkMaximum,
                                                    _In_ BOOL fGlobalMaximumSupported,
                                                    _In_ FLOAT fltGlobalMaximum);

    FLOAT       GetRangeMinimumValue(
                                                    _In_ FLOAT fltDefaultRangeMinimum,
                                                    _In_ BOOL fSpecificMinimumSupported,
                                                    _In_ FLOAT fltSpecificMinimum,
                                                    _In_ BOOL fBulkMinimumSupported,
                                                    _In_ FLOAT fltBulkMinimum,
                                                    _In_ BOOL fGlobalMinimumSupported,
                                                    _In_ FLOAT fltGlobalMinimum);

    static VOID CALLBACK LongReportIntervalTimerCallback(
                                                    _Inout_ PTP_CALLBACK_INSTANCE pInstance,
                                                    _Inout_opt_ PVOID pContext,
                                                    _Inout_ PTP_TIMER pTimer);

    VOID        CheckLongReportIntervalTimer();
    VOID        StartLongReportIntervalTimer();
    VOID        StopLongReportIntervalTimer();
    VOID        StopIdleForLongReportIntervalTimer();


    HRESULT     HandleReportIntervalUpdate();
    HRESULT     HandleLocationDesiredAccuracyUpdate();
    HRESULT     HandleGeolocationRadioStateUpdate();
    HRESULT     HandleChangeSensitivityUpdate();
    HRESULT     HandleSetReportingAndPowerStates();

    WCHAR       m_pwszManufacturer[DESCRIPTOR_MAX_LENGTH];
    WCHAR       m_pwszProduct[DESCRIPTOR_MAX_LENGTH];
    WCHAR       m_pwszSerialNumber[DESCRIPTOR_MAX_LENGTH];
    WCHAR       m_SensorID[DESCRIPTOR_MAX_LENGTH];
    CHAR        m_SensorName[DESCRIPTOR_MAX_LENGTH];

    CLIENT_MAP      m_pClientMap;
    SUBSCRIBER_MAP  m_pSubscriberMap;

    FLOAT       m_fltLowestClientChangeSensitivities[MAX_NUM_DATA_FIELDS];
    ULONG       m_ulLowestClientReportInterval;
    ULONG       m_ulLowestClientLocationDesiredAccuracy;
    ULONG       m_ulCurrentGeolocationRadioState;
    ULONG       m_ulRequiredGeolocationRadioState;
    ULONG       m_ulPreviousGeolocationRadioState;

    FLOAT       m_fltDefaultChangeSensitivity;
    FLOAT       m_fltDefaultRangeMaximum;
    FLOAT       m_fltDefaultRangeMinimum;
    FLOAT       m_fltDefaultAccuracy;
    FLOAT       m_fltDefaultResolution;

    ULONG       m_ulDefaultCurrentReportInterval;
    ULONG       m_ulDefaultMinimumReportInterval;

    BOOL        m_fSensorUpdated; // flag checks to see if data report was received
    BOOL        m_fInitialDataReceived;  // flag checks to see if inital poll request was fulfilled
    BOOL        m_fReportingState; // tracks whether sensor reporting is turned on or off
    ULONG       m_ulPowerState;    // tracks in what state sensor power should be

    ULONGLONG   m_ullInitialEventTime;
    ULONG       m_ulEventCount;

    PTP_POOL                                m_pThreadpool;
    TP_CALLBACK_ENVIRON                     m_ThreadpoolEnvironment;
    PTP_TIMER                               m_pLongReportIntervalTimer;
    CComAutoCriticalSection                 m_CriticalSectionLongReportInterval;    // Critical section to synchronize long report interval timer
    bool                                    m_fUsingLongReportIntervalTimer;

//protected:
public:

    // PropertyKeys
    CComPtr<IPortableDeviceKeyCollection>   m_spSupportedSensorProperties;
    CComPtr<IPortableDeviceKeyCollection>   m_spSettableSensorProperties;
    CComPtr<IPortableDeviceKeyCollection>   m_spSupportedSensorDataFields;
    CComPtr<IPortableDeviceKeyCollection>   m_spRequiredDataFields;

    // Values
    CComPtr<IPortableDeviceValues>          m_spSensorPropertyValues;
    CComPtr<IPortableDeviceValues>          m_spSensorDataFieldValues;

    CComAutoCriticalSection                 m_CriticalSection; // This is used to make all calls to get/set properties thread safe
    
    BOOL                                    m_fSensorInitialized;    // flag checks if we have been initialized

    SensorType                              m_SensorType;
    ULONG                                   m_SensorNum;

    HANDLE                                  m_hSensorEvent;            // Handle to the Eventing thread proc
    BOOL                                    m_fValidDataEvent;                       // flag that indicates an event needs to be fired

    CComPtr<IWDFDevice2>                    m_spWdfDevice2;
};


