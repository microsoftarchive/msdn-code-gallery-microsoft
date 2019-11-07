/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Barometer.h
//
// Description:
//      Defines the CBarometer container class

--*/


#pragma once

typedef struct _BAROMETER_DEVICE_PROPERTIES
{
    // from input report
    BOOL   fSensorStateSupported;
    BOOL   fSensorStateSelectorSupported;
    BOOL   fEventTypeSupported;
    BOOL   fEventTypeSelectorSupported;

    // supported datafields
    BOOL   fBarometerSupported;

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
    BOOL   fBarometerSensitivitySupported;
    FLOAT  fltBarometerSensitivity;
    
    BOOL   fBarometerMaximumSupported;
    FLOAT  fltBarometerMaximum;
    BOOL   fBarometerMinimumSupported;
    FLOAT  fltBarometerMinimum;

    BOOL   fBarometerAccuracySupported;
    FLOAT  fltBarometerAccuracy;

    BOOL   fBarometerResolutionSupported;
    FLOAT  fltBarometerResolution;

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

} BAROMETER_DEVICE_PROPERTIES, *PBAROMETER_DEVICE_PROPERTIES;


class CBarometer : public CSensor
{
public:
    CBarometer();
    ~CBarometer();
    
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

    HRESULT GetPropertyValuesForBarometerObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT ProcessBarometerAsyncRead( BYTE* buffer, ULONG length );
    HRESULT UpdateBarometerPropertyValues( BYTE* pbSendReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported );

private:

    HRESULT InitializeBarometer(VOID);
    HRESULT AddBarometerPropertyKeys();
    HRESULT AddBarometerSettablePropertyKeys();
    HRESULT AddBarometerDataFieldKeys();
    HRESULT SetBarometerDefaultValues();

    HRESULT SetSettableBarometerProperties();

    BAROMETER_DEVICE_PROPERTIES m_DeviceProperties;
};


const unsigned short SENSOR_BAROMETER_NAME[]                        = L"Barometer";
const unsigned short SENSOR_BAROMETER_DESCRIPTION[]                 = L"Barometer Sensor";
const char SENSOR_BAROMETER_TRACE_NAME[]                            = "Barometer";

// Default Values
const FLOAT DEFAULT_BAROMETER_SENSITIVITY                           = 0.00001F;
const FLOAT DEFAULT_BAROMETER_MAXIMUM                               = FLT_MAX;
const FLOAT DEFAULT_BAROMETER_MINIMUM                               = -FLT_MAX;
const FLOAT DEFAULT_BAROMETER_ACCURACY                              = FLT_MAX;
const FLOAT DEFAULT_BAROMETER_RESOLUTION                            = FLT_MAX;

const FLOAT MIN_BAROMETER_SENSITIVITY                               = -FLT_MAX;
const FLOAT MIN_BAROMETER_MAXIMUM                                   = -FLT_MAX;
const FLOAT MAX_BAROMETER_MINIMUM                                   = FLT_MAX;
const FLOAT MIN_BAROMETER_ACCURACY                                  = -FLT_MAX;
const FLOAT MIN_BAROMETER_RESOLUTION                                = -FLT_MAX;

const ULONG DEFAULT_BAROMETER_MIN_REPORT_INTERVAL                   = 50;
const ULONG DEFAULT_BAROMETER_CURRENT_REPORT_INTERVAL               = 100;




