/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Frequency.h
//
// Description:
//      Defines the CFrequency container class

--*/


#pragma once

typedef struct _FREQUENCY_DEVICE_PROPERTIES
{
    // from input report
    BOOL   fSensorStateSupported;
    BOOL   fSensorStateSelectorSupported;
    BOOL   fEventTypeSupported;
    BOOL   fEventTypeSelectorSupported;

    // supported datafields
    BOOL   fFrequencySupported;

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
    BOOL   fFrequencySensitivitySupported;
    FLOAT  fltFrequencySensitivity;
    
    BOOL   fFrequencyMaximumSupported;
    FLOAT  fltFrequencyMaximum;
    BOOL   fFrequencyMinimumSupported;
    FLOAT  fltFrequencyMinimum;

    BOOL   fFrequencyAccuracySupported;
    FLOAT  fltFrequencyAccuracy;

    BOOL   fFrequencyResolutionSupported;
    FLOAT  fltFrequencyResolution;

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

} FREQUENCY_DEVICE_PROPERTIES, *PFREQUENCY_DEVICE_PROPERTIES;


class CFrequency : public CSensor
{
public:
    CFrequency();
    ~CFrequency();
    
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

    HRESULT GetPropertyValuesForFrequencyObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT ProcessFrequencyAsyncRead( BYTE* buffer, ULONG length );
    HRESULT UpdateFrequencyPropertyValues( BYTE* pbSendReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported );

private:

    HRESULT InitializeFrequency(VOID);
    HRESULT AddFrequencyPropertyKeys();
    HRESULT AddFrequencySettablePropertyKeys();
    HRESULT AddFrequencyDataFieldKeys();
    HRESULT SetFrequencyDefaultValues();

    HRESULT SetSettableFrequencyProperties();

    FREQUENCY_DEVICE_PROPERTIES m_DeviceProperties;
};


const unsigned short SENSOR_FREQUENCY_NAME[]                        = L"Frequency";
const unsigned short SENSOR_FREQUENCY_DESCRIPTION[]                 = L"Frequency Sensor";
const char SENSOR_FREQUENCY_TRACE_NAME[]                            = "Frequency";

// Default Values
const FLOAT DEFAULT_FREQUENCY_SENSITIVITY                           = 0.5F;
const FLOAT DEFAULT_FREQUENCY_MAXIMUM                               = FLT_MAX;
const FLOAT DEFAULT_FREQUENCY_MINIMUM                               = -FLT_MAX;
const FLOAT DEFAULT_FREQUENCY_ACCURACY                              = FLT_MAX;
const FLOAT DEFAULT_FREQUENCY_RESOLUTION                            = FLT_MAX;

const FLOAT MIN_FREQUENCY_SENSITIVITY                               = -FLT_MAX;
const FLOAT MIN_FREQUENCY_MAXIMUM                                   = -FLT_MAX;
const FLOAT MAX_FREQUENCY_MINIMUM                                   = FLT_MAX;
const FLOAT MIN_FREQUENCY_ACCURACY                                  = -FLT_MAX;
const FLOAT MIN_FREQUENCY_RESOLUTION                                = -FLT_MAX;

const ULONG DEFAULT_FREQUENCY_MIN_REPORT_INTERVAL                   = 50;
const ULONG DEFAULT_FREQUENCY_CURRENT_REPORT_INTERVAL               = 100;




