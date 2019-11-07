/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Thermometer.h
//
// Description:
//      Defines the CThermometer container class

--*/


#pragma once

typedef struct _THERMOMETER_DEVICE_PROPERTIES
{
    // from input report
    BOOL   fSensorStateSupported;
    BOOL   fSensorStateSelectorSupported;
    BOOL   fEventTypeSupported;
    BOOL   fEventTypeSelectorSupported;

    // supported datafields
    BOOL   fThermometerSupported;

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
    BOOL   fThermometerSensitivitySupported;
    FLOAT  fltThermometerSensitivity;
    
    BOOL   fThermometerMaximumSupported;
    FLOAT  fltThermometerMaximum;
    BOOL   fThermometerMinimumSupported;
    FLOAT  fltThermometerMinimum;

    BOOL   fThermometerAccuracySupported;
    FLOAT  fltThermometerAccuracy;

    BOOL   fThermometerResolutionSupported;
    FLOAT  fltThermometerResolution;

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

} THERMOMETER_DEVICE_PROPERTIES, *PTHERMOMETER_DEVICE_PROPERTIES;


class CThermometer : public CSensor
{
public:
    CThermometer();
    ~CThermometer();
    
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

    HRESULT GetPropertyValuesForThermometerObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT ProcessThermometerAsyncRead( BYTE* buffer, ULONG length );
    HRESULT UpdateThermometerPropertyValues( BYTE* pbSendReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported );

private:

    HRESULT InitializeThermometer(VOID);
    HRESULT AddThermometerPropertyKeys();
    HRESULT AddThermometerSettablePropertyKeys();
    HRESULT AddThermometerDataFieldKeys();
    HRESULT SetThermometerDefaultValues();

    HRESULT SetSettableThermometerProperties();

    THERMOMETER_DEVICE_PROPERTIES m_DeviceProperties;
};


const unsigned short SENSOR_THERMOMETER_NAME[]                        = L"Thermometer";
const unsigned short SENSOR_THERMOMETER_DESCRIPTION[]                 = L"Thermometer Sensor";
const char SENSOR_THERMOMETER_TRACE_NAME[]                            = "Thermometer";

// Default Values
const FLOAT DEFAULT_THERMOMETER_SENSITIVITY                           = 0.25F;
const FLOAT DEFAULT_THERMOMETER_MAXIMUM                               = FLT_MAX;
const FLOAT DEFAULT_THERMOMETER_MINIMUM                               = -FLT_MAX;
const FLOAT DEFAULT_THERMOMETER_ACCURACY                              = FLT_MAX;
const FLOAT DEFAULT_THERMOMETER_RESOLUTION                            = FLT_MAX;

const FLOAT MIN_THERMOMETER_SENSITIVITY                               = -FLT_MAX;
const FLOAT MIN_THERMOMETER_MAXIMUM                                   = -FLT_MAX;
const FLOAT MAX_THERMOMETER_MINIMUM                                   = FLT_MAX;
const FLOAT MIN_THERMOMETER_ACCURACY                                  = -FLT_MAX;
const FLOAT MIN_THERMOMETER_RESOLUTION                                = -FLT_MAX;

const ULONG DEFAULT_THERMOMETER_MIN_REPORT_INTERVAL                   = 50;
const ULONG DEFAULT_THERMOMETER_CURRENT_REPORT_INTERVAL               = 100;





