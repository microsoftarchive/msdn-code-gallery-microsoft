/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Voltage.h
//
// Description:
//      Defines the CVoltage container class

--*/


#pragma once

typedef struct _VOLTAGE_DEVICE_PROPERTIES
{
    // from input report
    BOOL   fSensorStateSupported;
    BOOL   fSensorStateSelectorSupported;
    BOOL   fEventTypeSupported;
    BOOL   fEventTypeSelectorSupported;

    // supported datafields
    BOOL   fVoltageSupported;

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
    BOOL   fVoltageSensitivitySupported;
    FLOAT  fltVoltageSensitivity;
    
    BOOL   fVoltageMaximumSupported;
    FLOAT  fltVoltageMaximum;
    BOOL   fVoltageMinimumSupported;
    FLOAT  fltVoltageMinimum;

    BOOL   fVoltageAccuracySupported;
    FLOAT  fltVoltageAccuracy;

    BOOL   fVoltageResolutionSupported;
    FLOAT  fltVoltageResolution;

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

} VOLTAGE_DEVICE_PROPERTIES, *PVOLTAGE_DEVICE_PROPERTIES;


class CVoltage : public CSensor
{
public:
    CVoltage();
    ~CVoltage();
    
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

    HRESULT GetPropertyValuesForVoltageObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT ProcessVoltageAsyncRead( BYTE* buffer, ULONG length );
    HRESULT UpdateVoltagePropertyValues( BYTE* pbSendReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported );

private:

    HRESULT InitializeVoltage(VOID);
    HRESULT AddVoltagePropertyKeys();
    HRESULT AddVoltageSettablePropertyKeys();
    HRESULT AddVoltageDataFieldKeys();
    HRESULT SetVoltageDefaultValues();

    HRESULT SetSettableVoltageProperties();

    VOLTAGE_DEVICE_PROPERTIES m_DeviceProperties;
};


const unsigned short SENSOR_VOLTAGE_NAME[]                        = L"Voltage";
const unsigned short SENSOR_VOLTAGE_DESCRIPTION[]                 = L"Voltage Sensor";
const char SENSOR_VOLTAGE_TRACE_NAME[]                            = "Voltage";

// Default Values
const FLOAT DEFAULT_VOLTAGE_SENSITIVITY                           = 0.02F;
const FLOAT DEFAULT_VOLTAGE_MAXIMUM                               = FLT_MAX;
const FLOAT DEFAULT_VOLTAGE_MINIMUM                               = -FLT_MAX;
const FLOAT DEFAULT_VOLTAGE_ACCURACY                              = FLT_MAX;
const FLOAT DEFAULT_VOLTAGE_RESOLUTION                            = FLT_MAX;

const FLOAT MIN_VOLTAGE_SENSITIVITY                               = -FLT_MAX;
const FLOAT MIN_VOLTAGE_MAXIMUM                                   = -FLT_MAX;
const FLOAT MAX_VOLTAGE_MINIMUM                                   = FLT_MAX;
const FLOAT MIN_VOLTAGE_ACCURACY                                  = -FLT_MAX;
const FLOAT MIN_VOLTAGE_RESOLUTION                                = -FLT_MAX;

const ULONG DEFAULT_VOLTAGE_MIN_REPORT_INTERVAL                   = 50;
const ULONG DEFAULT_VOLTAGE_CURRENT_REPORT_INTERVAL               = 100;





