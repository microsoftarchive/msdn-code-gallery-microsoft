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


typedef struct _SENSOR_DEVICE_PROPERTIES
{
    // Dynamic Per-datafield properties
    BOOL   fSensorSensitivitySupported[MAX_NUM_DATA_FIELDS];
    FLOAT  fltSensorSensitivity[MAX_NUM_DATA_FIELDS];
    
    BOOL   fSensorMaximumSupported[MAX_NUM_DATA_FIELDS];
    FLOAT  fltSensorMaximum[MAX_NUM_DATA_FIELDS];

    BOOL   fSensorMinimumSupported[MAX_NUM_DATA_FIELDS];
    FLOAT  fltSensorMinimum[MAX_NUM_DATA_FIELDS];

    BOOL   fSensorAccuracySupported[MAX_NUM_DATA_FIELDS];
    FLOAT  fltSensorAccuracy[MAX_NUM_DATA_FIELDS];

    BOOL   fSensorResolutionSupported[MAX_NUM_DATA_FIELDS];
    FLOAT  fltSensorResolution[MAX_NUM_DATA_FIELDS];

} SENSOR_DEVICE_PROPERTIES, *PSENSOR_DEVICE_PROPERTIES;


class CSensor
{
protected:
    CSensorManager* m_pSensorManager;

public:
    CSensor();
    ~CSensor();
    
    HRESULT     InitializeSensor(
                                _In_ SensorType sensType, 
                                _In_ ULONG sensUsage,
                                _In_ USHORT sensLinkCollection,
                                _In_ DWORD sensNum,
                                _In_ LPWSTR pwszManufacturer,
                                _In_ LPWSTR pwszProduct,
                                _In_ LPWSTR pwszSerialNumber,
                                _In_ LPWSTR pswsSensorID);

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

    HRESULT     SetUniqueID(_In_ IWDFDevice* pWdfDevice);

    VOID        TranslateHidUnitsExp(_Inout_ LONG *UnitsExp);

    LONG        ExtractValueFromUsageUValue(_In_ ULONG LogicalMin, _In_ ULONG BitSize, _In_ ULONG UsageUValue);
    ULONG       ExtractUsageUValueFromDouble(_In_ DOUBLE dblValue, _In_ LONG UnitsExp, _In_ ULONG BitSize);
    DOUBLE      ExtractDoubleFromUsageValue(_In_ ULONG LogicalMin, _In_ ULONG UsageUValue, _In_ LONG UsageValue, _In_ LONG UnitsExp);
    LONGLONG    ExtractLongLongFromUsageValue(_In_ ULONG LogicalMin, _In_ ULONG UsageUValue, _In_ LONG UsageValue, _In_ LONG UnitsExp);

    HRESULT     TranslateHidUsageToSensorDataTypePropertyKey(
                                                            _In_ ULONG ulHidUsage, 
                                                            _Out_ PROPERTYKEY* pk,
                                                            _Out_ VARTYPE* vt);

    HRESULT     HandleDynamicDatafield(
                                                    _In_ PROPERTYKEY pkDatakey,
                                                    _In_ VARTYPE vtType,
                                                    _In_ LONG Usage,
                                                    _In_ USHORT ReportCount,
                                                    _In_ LONG UnitsExp,
                                                    _In_ LONG UsageValue,
                                                    _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH*2) PCHAR UsageArray);

    HRESULT     HandleDefinedDynamicDatafield(
                                                    _In_ LONG Usage,
                                                    _In_ USHORT ReportCount,
                                                    _In_ LONG UnitsExp,
                                                    _In_ LONG UsageValue,
                                                    _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH*2) PCHAR UsageArray);

    HRESULT     HandleArbitraryDynamicDatafield(
                                                    _In_ PROPERTYKEY pkDatakey,
                                                    _In_ VARTYPE vtType,
                                                    _In_ USHORT ReportCount,
                                                    _In_ LONG UnitsExp,
                                                    _In_ LONG UsageValue,
                                                    _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH*2) PCHAR UsageArray);

    HRESULT     HandleDynamicDatafieldProperty(
                                                    _In_ PROPERTYKEY pkDatakey,
                                                    _In_ LONG Usage,
                                                    _In_ LONG UnitsExp,
                                                    _In_ LONG UsageValue,
                                                    _In_ USHORT UsageDataModifier);

    HRESULT     HandleDefinedDynamicDatafieldProperty(
                                                    _In_ LONG Usage,
                                                    _In_ LONG UnitsExp,
                                                    _In_ LONG UsageValue,
                                                    _In_ USHORT UsageDataModifier);

    HRESULT     HandleArbitraryDynamicDatafieldProperty(
                                                    _In_ PROPERTYKEY pkDatakey,
                                                    _In_ LONG UnitsExp,
                                                    _In_ LONG UsageValue,
                                                    _In_ USHORT UsageDataModifier);

    HRESULT     HandleReportIntervalUpdate(
                                                    _In_ UCHAR reportID,
                                                    _In_ BOOL fReportIntervalSupported,
                                                    _Out_ ULONG* pulReportInterval,
                                                    _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport, 
                                                    _In_ ULONG ulReportSize);

    HRESULT     HandleChangeSensitivityUpdate(
                                                    _In_ UCHAR reportID, 
                                                    _In_ BOOL fWriteToDevice,
                                                    _In_ BOOL fGlobalSensitivitySupported,
                                                    _In_ BOOL fBulkSensitivitySupported,
                                                    _In_ BOOL fDatafieldSensitivitySupported,
                                                    _In_ ULONG ulBulkUsage,
                                                    _In_ BOOL fDatafieldSupported,
                                                    _In_ ULONG ulDatafieldUsage,
                                                    _In_ ULONG ulDatafieldUnits,
                                                    _In_ PROPERTYKEY pkDatafield,
                                                    _In_ DWORD dwDfIdx,
                                                    _Inout_ FLOAT* pfltGlobalSensitivity, 
                                                    _Inout_ FLOAT* pfltBulkSensitivity, 
                                                    _Inout_ FLOAT* pfltDatafieldSensitivity, 
                                                    _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport, 
                                                    _In_ ULONG uReportSize);

    HRESULT     HandleMaximumUpdate(
                                                    _In_ UCHAR reportID, 
                                                    _In_ BOOL fGlobalMaximumSupported,
                                                    _In_ BOOL fBulkMaximumSupported,
                                                    _In_ BOOL fDatafieldMaximumSupported,
                                                    _In_ ULONG ulBulkUsage,
                                                    _In_ BOOL fDatafieldSupported,
                                                    _In_ ULONG ulDatafieldUsage,
                                                    _In_ ULONG ulDatafieldUnits,
                                                    _In_ PROPERTYKEY pkDatafield,
                                                    _In_ DWORD dwDfIdx,
                                                    _Inout_ FLOAT* pfltGlobalMaximum, 
                                                    _Inout_ FLOAT* pfltBulkMaximum, 
                                                    _Inout_ FLOAT* pfltDatafieldMaximum, 
                                                    _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport, 
                                                    _In_ ULONG uReportSize);

    HRESULT     HandleMinimumUpdate(
                                                    _In_ UCHAR reportID, 
                                                    _In_ BOOL fGlobalMaximumSupported,
                                                    _In_ BOOL fBulkMaximumSupported,
                                                    _In_ BOOL fDatafieldMaximumSupported,
                                                    _In_ ULONG ulBulkUsage,
                                                    _In_ BOOL fDatafieldSupported,
                                                    _In_ ULONG ulDatafieldUsage,
                                                    _In_ ULONG ulDatafieldUnits,
                                                    _In_ PROPERTYKEY pkDatafield,
                                                    _In_ DWORD dwDfIdx,
                                                    _Inout_ FLOAT* pfltGlobalMaximum, 
                                                    _Inout_ FLOAT* pfltBulkMaximum, 
                                                    _Inout_ FLOAT* pfltDatafieldMaximum, 
                                                    _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport, 
                                                    _In_ ULONG uReportSize);

    HRESULT     HandleAccuracyUpdate(
                                                    _In_ UCHAR reportID, 
                                                    _In_ BOOL fGlobalMaximumSupported,
                                                    _In_ BOOL fBulkMaximumSupported,
                                                    _In_ BOOL fDatafieldMaximumSupported,
                                                    _In_ ULONG ulBulkUsage,
                                                    _In_ BOOL fDatafieldSupported,
                                                    _In_ ULONG ulDatafieldUsage,
                                                    _In_ ULONG ulDatafieldUnits,
                                                    _In_ PROPERTYKEY pkDatafield,
                                                    _In_ DWORD dwDfIdx,
                                                    _Inout_ FLOAT* pfltGlobalMaximum, 
                                                    _Inout_ FLOAT* pfltBulkMaximum, 
                                                    _Inout_ FLOAT* pfltDatafieldMaximum, 
                                                    _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport, 
                                                    _In_ ULONG uReportSize);

    HRESULT     HandleResolutionUpdate(
                                                    _In_ UCHAR reportID, 
                                                    _In_ BOOL fGlobalMaximumSupported,
                                                    _In_ BOOL fBulkMaximumSupported,
                                                    _In_ BOOL fDatafieldMaximumSupported,
                                                    _In_ ULONG ulBulkUsage,
                                                    _In_ BOOL fDatafieldSupported,
                                                    _In_ ULONG ulDatafieldUsage,
                                                    _In_ ULONG ulDatafieldUnits,
                                                    _In_ PROPERTYKEY pkDatafield,
                                                    _In_ DWORD dwDfIdx,
                                                    _Inout_ FLOAT* pfltGlobalMaximum, 
                                                    _Inout_ FLOAT* pfltBulkMaximum, 
                                                    _Inout_ FLOAT* pfltDatafieldMaximum, 
                                                    _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport, 
                                                    _In_ ULONG uReportSize);

    HRESULT     GetPropertyValuesForSensorObject(
                                                    _In_ LPCWSTR                        wszObjectID,
                                                    _In_ IPortableDeviceKeyCollection*  pKeys,
                                                    _In_ IPortableDeviceValues*         pValues,
                                                    _In_ LPCWSTR                        wszSensorName,
                                                    _In_ GUID                           guidSensorCategory,
                                                    _Out_ BOOL*                         pfError);

    HRESULT     HandleGetHidInputSelectors(
                                                    _Inout_ BOOL* pfSensorStateSelectorSupported,
                                                    _Inout_ USHORT* pusSensorState,
                                                    _Inout_ BOOL* pfEventTypeSelectorSupported,
                                                    _Inout_ USHORT* pusEventType,
                                                    _In_ HIDP_REPORT_TYPE ReportType, 
                                                    _In_ USAGE UsagePage, 
                                                    _In_ USHORT LinkCollection, 
                                                    _In_reads_bytes_(MAX_NUM_HID_USAGES*2) USAGE* pUsageList, 
                                                    _In_ ULONG* pNumUsages, 
                                                    _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pInputReport, 
                                                    _In_ ULONG uReportSize);

    HRESULT     HandleCommonInputValues(
                                                    _In_ ULONG idx,
                                                    _Inout_ BOOL* pfSensorStateSupported,
                                                    _Inout_ USHORT* pusSensorState,
                                                    _Inout_ BOOL* pfEventTypeSupported,
                                                    _Inout_ USHORT* pusEventType,
                                                    _In_ USAGE Usage,
                                                    _In_ LONG UsageValue,
                                                    _In_ ULONG UsageUValue,
                                                    _In_ LONG UnitsExp,
                                                    _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH*2) CHAR*  pUsageArray,
                                                    _Out_ BOOL* pfInputHandled);

    HRESULT     HandleGetHidFeatureSelectors(
                                                    _Inout_ BOOL* pfReportingStateSelectorSupported,
                                                    _Inout_ ULONG* pulReportingStateSelector,
                                                    _Inout_ BOOL* pfPowerStateSelectorSupported,
                                                    _Inout_ ULONG* pulPowerStateSelector,
                                                    _Inout_ BOOL* pfSensorStatusSelectorSupported,
                                                    _Inout_ ULONG* pulSensorStatusSelector,
                                                    _Inout_ BOOL* pfConnectionTypeSelectorSupported,
                                                    _Inout_ ULONG* pulConnectionTypeSelector,
                                                    _In_ HIDP_REPORT_TYPE ReportType, 
                                                    _In_ USAGE UsagePage, 
                                                    _In_ USHORT LinkCollection, 
                                                    _In_reads_bytes_(MAX_NUM_HID_USAGES*2) USAGE* pUsageList, 
                                                    _In_ ULONG* pNumUsages, 
                                                    _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport, 
                                                    _In_ ULONG uReportSize);

    HRESULT     HandleGetCommonFeatureValues(
                                                    _In_ ULONG idx,
                                                    _Inout_ BOOL* pfReportingStateSupported,
                                                    _Inout_ ULONG* pulReportingState,
                                                    _Inout_ BOOL* pfPowerStateSupported,
                                                    _Inout_ ULONG* pulPowerState,
                                                    _Inout_ BOOL* pfSensorStatusSupported,
                                                    _Inout_ ULONG* pulSensorStatus,
                                                    _Inout_ BOOL* pfConnectionTypeSupported,
                                                    _Inout_ ULONG* pulConnectionType,
                                                    _Inout_ BOOL* pfReportIntervalSupported,
                                                    _Inout_ ULONG* pulReportInterval,
                                                    _Inout_ BOOL* pfGlobalSensitivitySupported,
                                                    _Inout_ FLOAT* pfltGlobalSensitivity,
                                                    _Inout_ BOOL* pfGlobalMaximumSupported,
                                                    _Inout_ FLOAT* pfltGlobalMaximum,
                                                    _Inout_ BOOL* pfGlobalMinimumSupported,
                                                    _Inout_ FLOAT* pfltGlobalMinimum,
                                                    _Inout_ BOOL* pfGlobalAccuracySupported,
                                                    _Inout_ FLOAT* pfltGlobalAccuracy,
                                                    _Inout_ BOOL* pfGlobalResolutionSupported,
                                                    _Inout_ FLOAT* pfltGlobalResolution,
                                                    _Inout_ BOOL* pfMinimumReportIntervalSupported,
                                                    _Inout_ ULONG* pulMinimumReportInterval,
                                                    _Inout_ BOOL* pfFriendlyNameSupported,
                                                    _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH) PWCHAR pwszFriendlyName,
                                                    _Inout_ BOOL* pfPersistentUniqueIDSupported,
                                                    _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH) PWCHAR pwszPersistentUniqueID,
                                                    _Inout_ BOOL* pfManufacturerSupported,
                                                    _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH) PWCHAR pwszManufacturer,
                                                    _Inout_ BOOL* pfModelSupported,
                                                    _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH) PWCHAR pwszModel,
                                                    _Inout_ BOOL* pfSerialNumberSupported,
                                                    _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH) PWCHAR pwszSerialNumber,
                                                    _Inout_ BOOL* pfDescriptionSupported,
                                                    _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH) PWCHAR pwszDescription,
                                                    _In_ USAGE Usage,
                                                    _In_ LONG UsageValue,
                                                    _In_ ULONG UsageUValue,
                                                    _In_ LONG UnitsExp,
                                                    _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH*2) CHAR*  pUsageArray,
                                                    _Out_ BOOL* pfFeatureHandled);

    HRESULT     HandleSetReportingAndPowerStates(
                                                    _In_ BOOL fReportingStateSupported,
                                                    _In_ BOOL fReportingStateSelectorSupported,
                                                    _In_ BOOL fReportingState,
                                                    _In_ BOOL fPowerStateSupported,
                                                    _In_ BOOL fPowerStateSelectorSupported,
                                                    _In_ ULONG ulPowerState,
                                                    _In_ HIDP_REPORT_TYPE ReportType, 
                                                    _In_ USAGE UsagePage, 
                                                    _In_ USHORT LinkCollection, 
                                                    _In_reads_bytes_(MAX_NUM_HID_USAGES*2) USAGE* UsageList, 
                                                    _In_ ULONG* pNumUsages, 
                                                    _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport, 
                                                    _In_ ULONG uReportSize);

    VOID        TraceHIDParseError(
                                                    HRESULT hr,
                                                    SensorType sensType,
                                                    HIDP_REPORT_TYPE ReportType,
                                                    USHORT LinkCollection);

    HRESULT     InitPerDataFieldProperties(
                                                    _In_ PROPERTYKEY pkDataField);

    HRESULT     GetSensorPropertiesFromDevice(      
                                                    _In_ UCHAR reportID, 
                                                    _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport, 
                                                    _In_ UINT uReportSize);

    HRESULT     SetThenGetSensorPropertiesFromDevice(      
                                                    _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport, 
                                                    _In_ UINT uReportSize,
                                                    _In_ BOOL fExactCopyRequired,
                                                    _Out_ BOOL* pfIsExactCopy);

    VOID        ReportCommonInputReportDescriptorConditions(
                                                    _In_ BOOL fSensorStateSelectorSupported,
                                                    _In_ BOOL fEventTypeSelectorSupported,
                                                    _In_ BOOL fSensorStateSupported,
                                                    _In_ BOOL fEventTypeSupported );

    VOID        ReportCommonFeatureReportDescriptorConditions(
                                                    _In_ BOOL fFeatureReportSupported,
                                                    _In_ BOOL fReportingStateSelectorSupported,
                                                    _In_ BOOL fPowerStateSelectorSupported,
                                                    _In_ BOOL fSensorStatusSelectorSupported,
                                                    _In_ BOOL fConnectionTypeSelectorSupported,
                                                    _In_ BOOL fReportingStateSupported,
                                                    _In_ BOOL fPowerStateSupported,
                                                    _In_ BOOL fSensorStatusSupported,
                                                    _In_ BOOL fConnectionTypeSupported,
                                                    _In_ BOOL fReportIntervalSupported,
                                                    _In_ BOOL fGlobalSensitivitySupported,
                                                    _In_ BOOL fGlobalMaximumSupported,
                                                    _In_ BOOL fGlobalMinimumSupported,
                                                    _In_ BOOL fGlobalAccuracySupported,
                                                    _In_ BOOL fGlobalResolutionSupported,
                                                    _In_ BOOL fMinimumReportIntervalSupported,
                                                    _In_ BOOL fFriendlyNameSupported,
                                                    _In_ BOOL fPersistentUniqueIDSupported,
                                                    _In_ BOOL fManufacturerSupported,
                                                    _In_ BOOL fModelSupported,
                                                    _In_ BOOL fSerialNumberSupported,
                                                    _In_ BOOL fDescriptionSupported);

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


    WCHAR           m_pwszManufacturer[HID_USB_DESCRIPTOR_MAX_LENGTH];
    WCHAR           m_pwszProduct[HID_USB_DESCRIPTOR_MAX_LENGTH];
    WCHAR           m_pwszSerialNumber[HID_USB_DESCRIPTOR_MAX_LENGTH];
    WCHAR           m_SensorID[HID_USB_DESCRIPTOR_MAX_LENGTH];
    CHAR            m_SensorName[HID_USB_DESCRIPTOR_MAX_LENGTH];

    CLIENT_MAP      m_pClientMap;
    SUBSCRIBER_MAP  m_pSubscriberMap;

    FLOAT           m_fltLowestClientChangeSensitivities[MAX_NUM_DATA_FIELDS];
    ULONG           m_ulLowestClientReportInterval;

    FLOAT           m_fltDefaultChangeSensitivity;
    FLOAT           m_fltDefaultRangeMaximum;
    FLOAT           m_fltDefaultRangeMinimum;
    FLOAT           m_fltDefaultAccuracy;
    FLOAT           m_fltDefaultResolution;

    ULONG           m_ulDefaultCurrentReportInterval;
    ULONG           m_ulDefaultMinimumReportInterval;


    BOOL            m_fSensorUpdated; // flag checks to see if data report was received
    BOOL            m_fInitialDataReceived;  // flag checks to see if inital poll request was fulfilled
    BOOL            m_fReportingState; // tracks whether sensor reporting is turned on or off
    ULONG           m_ulPowerState;    // tracks in what state sensor power should be

    SENSOR_DEVICE_PROPERTIES m_DynamicDeviceProperties;
    LONG            m_DynamicDatafieldSupported[MAX_NUM_DATA_FIELDS];
    LONG            m_DynamicDatafieldUsages[MAX_NUM_DATA_FIELDS];
    BOOL            m_DynamicDatafieldSensitivitySupported[MAX_NUM_DATA_FIELDS];
    FLOAT           m_DynamicDatafieldSensitivity[MAX_NUM_DATA_FIELDS];
    BOOL            m_DynamicDatafieldMaximumSupported[MAX_NUM_DATA_FIELDS];
    FLOAT           m_DynamicDatafieldMaximum[MAX_NUM_DATA_FIELDS];
    BOOL            m_DynamicDatafieldMinimumSupported[MAX_NUM_DATA_FIELDS];
    FLOAT           m_DynamicDatafieldMinimum[MAX_NUM_DATA_FIELDS];
    BOOL            m_DynamicDatafieldAccuracySupported[MAX_NUM_DATA_FIELDS];
    FLOAT           m_DynamicDatafieldAccuracy[MAX_NUM_DATA_FIELDS];
    BOOL            m_DynamicDatafieldResolutionSupported[MAX_NUM_DATA_FIELDS];
    FLOAT           m_DynamicDatafieldResolution[MAX_NUM_DATA_FIELDS];

    CComPtr<IPortableDeviceKeyCollection>   m_spSupportedSensorDataFields;
    CComPtr<IPortableDeviceValues>          m_spSensorPropertyValues;

    // Values
    CComPtr<IPortableDeviceValues>          m_spSensorDataFieldValues;

    ULONGLONG                               m_ullInitialEventTime;
    ULONG                                   m_ulEventCount;
    ULONG                                   m_InputReportFailureCount;

    BOOL                                    m_fSensorStateChanged;
    BOOL                                    m_fSensorPropertiesPreviouslyUpdated; // flag note the properties have been updated at least once

protected:

    // PropertyKeys
    CComPtr<IPortableDeviceKeyCollection>   m_spSupportedSensorProperties;
    CComPtr<IPortableDeviceKeyCollection>   m_spSettableSensorProperties;
    CComPtr<IPortableDeviceKeyCollection>   m_spRequiredDataFields;

    CComAutoCriticalSection                 m_CriticalSection; // This is used to make all calls to get/set properties thread safe
    
    BOOL                                    m_fSensorInitialized;    // flag checks if we have been initialized

    SensorType                              m_SensorType;
    ULONG                                   m_SensorNum;
    ULONG                                   m_SensorUsage;
    BOOL                                    m_fFeatureReportSupported;
    BOOL                                    m_fHidUsagePropertySupported;

    BOOL                                    m_fCapsNodesInitialized;

    PHIDP_VALUE_CAPS                        m_InputValueCapsNodes;
    PHIDP_VALUE_CAPS                        m_OutputValueCapsNodes;
    PHIDP_VALUE_CAPS                        m_FeatureValueCapsNodes;

    PHIDP_BUTTON_CAPS                       m_InputButtonCapsNodes;
    PHIDP_BUTTON_CAPS                       m_OutputButtonCapsNodes;
    PHIDP_BUTTON_CAPS                       m_FeatureButtonCapsNodes;

    UCHAR                                   m_StartingInputReportID;
    UCHAR                                   m_StartingOutputReportID;
    UCHAR                                   m_StartingFeatureReportID;

    HANDLE                                  m_hSensorEvent;            // Handle to the Eventing thread proc
    BOOL                                    m_fValidDataEvent;         // flag that indicates an event needs to be fired

    ULONG                                   m_EventType;
    ULONG                                   m_SensorState;

    SYSTEMTIME                              m_st;

    USHORT                                  m_SensorLinkCollection;

    BOOL                                    m_fInitialPropertiesReceived;

    BOOL                                    m_fWarnedOnUseOfFeatureConnectionType;
    BOOL                                    m_fWarnedOnTypeOfFeatureConnectionType;
    BOOL                                    m_fWarnedOnUseOfFeatureReportingState;
    BOOL                                    m_fWarnedOnUseOfFeaturePowerState;
    BOOL                                    m_fWarnedOnUseOfFeatureSensorState;
    BOOL                                    m_fWarnedOnUseOfInputSensorState;
    BOOL                                    m_fWarnedOnUseOfInputEventType;

    BOOL                                    m_fInformedCommonInputReportConditions;
    BOOL                                    m_fInformedCommonFeatureReportConditions;

};


