/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Unsupported.h
//
// Description:
//      Defines the CUnsupported container class

--*/


#pragma once

typedef struct _UNSUPPORTED_DEVICE_PROPERTIES
{
    // from input report
    BOOL   fSensorStateSupported;
    BOOL   fSensorStateSelectorSupported;
    BOOL   fEventTypeSupported;
    BOOL   fEventTypeSelectorSupported;

    // supported datafields
    BOOL   fUnsupportedSupported;

    // from feature report
    BOOL   fReportingStateSupported;
    ULONG  ulReportingState;
    BOOL   fReportingStateSelectorSupported;
    ULONG  ulReportingStateSelector;
    BOOL   fPowerStateSupported;
    ULONG  ulPowerState;
    BOOL   fPowerStateSelectorSupported;
    ULONG  ulPowerStateSelector;
    BOOL   fSensorStatusSupported;
    ULONG  ulSensorStatus;
    BOOL   fSensorStatusSelectorSupported;
    ULONG  ulSensorStatusSelector;
    BOOL   fReportIntervalSupported;
    ULONG  ulReportInterval;
    BOOL   fGlobalSensitivitySupported;
    FLOAT  fltGlobalSensitivity;
    BOOL   fGlobalMaximumSupported;
    FLOAT  fltGlobalMaximum;
    BOOL   fGlobalMinimumSupported;
    FLOAT  fltGlobalMinimum;
    BOOL   fGlobalAccuracySupported;
    FLOAT  fltGlobalAccuracy;
    BOOL   fGlobalResolutionSupported;
    FLOAT  fltGlobalResolution;

    // Per-datafield properties
    BOOL   fUnsupportedSensitivitySupported;
    FLOAT  fltUnsupportedSensitivity;
    
    BOOL   fUnsupportedMaximumSupported;
    FLOAT  fltUnsupportedMaximum;
    BOOL   fUnsupportedMinimumSupported;
    FLOAT  fltUnsupportedMinimum;

    BOOL   fUnsupportedAccuracySupported;
    FLOAT  fltUnsupportedAccuracy;

    BOOL   fUnsupportedResolutionSupported;
    FLOAT  fltUnsupportedResolution;

    //Extended properties
    BOOL   fConnectionTypeSupported;
    ULONG  ulConnectionType;
    BOOL   fConnectionTypeSelectorSupported;
    ULONG  ulConnectionTypeSelector;
    BOOL   fMinimumReportIntervalSupported;
    ULONG  ulMinimumReportInterval;
    BOOL   fFriendlyNameSupported;
    WCHAR  wszFriendlyName[HID_FEATURE_REPORT_STRING_MAX_LENGTH];
    BOOL   fPersistentUniqueIDSupported;
    WCHAR  wszPersistentUniqueID[HID_FEATURE_REPORT_STRING_MAX_LENGTH];
    BOOL   fManufacturerSupported;
    WCHAR  wszManufacturer[HID_FEATURE_REPORT_STRING_MAX_LENGTH];
    BOOL   fModelSupported;
    WCHAR  wszModel[HID_FEATURE_REPORT_STRING_MAX_LENGTH];
    BOOL   fSerialNumberSupported;
    WCHAR  wszSerialNumber[HID_FEATURE_REPORT_STRING_MAX_LENGTH];
    BOOL   fDescriptionSupported;
    WCHAR  wszDescription[HID_FEATURE_REPORT_STRING_MAX_LENGTH];

} UNSUPPORTED_DEVICE_PROPERTIES, *PUNSUPPORTED_DEVICE_PROPERTIES;


class CUnsupported : public CSensor
{
public:
    CUnsupported();
    ~CUnsupported();
    
    HRESULT Initialize( _In_ SensorType sensType, 
                        _In_ ULONG sensUsage,
                        _In_ USHORT sensLinkCollection,
                        _In_ DWORD sensNum, 
                        _In_ LPWSTR pwszManufacturer,
                        _In_ LPWSTR pwszProduct,
                        _In_ LPWSTR pwszSerialNumber,
                        _In_ LPWSTR pwszSensorID,
                        _In_ CSensorManager* pSensorManager
                        );

    HRESULT GetPropertyValuesForUnsupportedObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT ProcessUnsupportedAsyncRead( BYTE* buffer, ULONG length );
    HRESULT UpdateUnsupportedPropertyValues( BYTE* pbSendReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported );

private:

    HRESULT InitializeUnsupported(VOID);
    HRESULT AddUnsupportedPropertyKeys();
    HRESULT AddUnsupportedSettablePropertyKeys();
    HRESULT AddUnsupportedDataFieldKeys();
    HRESULT SetUnsupportedDefaultValues();

    HRESULT SetSettableUnsupportedProperties();

    UNSUPPORTED_DEVICE_PROPERTIES m_DeviceProperties;
};


const unsigned short SENSOR_UNSUPPORTED_NAME[]                        = L"Unsupported";
const unsigned short SENSOR_UNSUPPORTED_DESCRIPTION[]                 = L"Unsupported Sensor";
const char SENSOR_UNSUPPORTED_TRACE_NAME[]                            = "Unsupported";

// Default Values
const FLOAT DEFAULT_UNSUPPORTED_SENSITIVITY                           = FLT_MAX;
const FLOAT DEFAULT_UNSUPPORTED_MAXIMUM                               = FLT_MAX;
const FLOAT DEFAULT_UNSUPPORTED_MINIMUM                               = -FLT_MAX;
const FLOAT DEFAULT_UNSUPPORTED_ACCURACY                              = FLT_MAX;
const FLOAT DEFAULT_UNSUPPORTED_RESOLUTION                            = FLT_MAX;

const ULONG DEFAULT_UNSUPPORTED_MIN_REPORT_INTERVAL                   = 1000;
const ULONG DEFAULT_UNSUPPORTED_CURRENT_REPORT_INTERVAL               = 1000;

const ULONG DEFAULT_UNSUPPORTED_HID_USAGE                             =0xFFFF;

