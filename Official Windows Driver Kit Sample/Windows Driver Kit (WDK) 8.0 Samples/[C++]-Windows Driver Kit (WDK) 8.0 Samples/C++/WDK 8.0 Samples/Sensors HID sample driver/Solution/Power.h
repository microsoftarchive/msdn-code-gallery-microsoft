/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Power.h
//
// Description:
//      Defines the CPower container class

--*/


#pragma once

typedef struct _POWER_DEVICE_PROPERTIES
{
    // from input report
    BOOL   fSensorStateSupported;
    BOOL   fSensorStateSelectorSupported;
    BOOL   fEventTypeSupported;
    BOOL   fEventTypeSelectorSupported;

    // supported datafields
    BOOL   fPowerSupported;

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
    BOOL   fPowerSensitivitySupported;
    FLOAT  fltPowerSensitivity;
    
    BOOL   fPowerMaximumSupported;
    FLOAT  fltPowerMaximum;
    BOOL   fPowerMinimumSupported;
    FLOAT  fltPowerMinimum;

    BOOL   fPowerAccuracySupported;
    FLOAT  fltPowerAccuracy;

    BOOL   fPowerResolutionSupported;
    FLOAT  fltPowerResolution;

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

} POWER_DEVICE_PROPERTIES, *PPOWER_DEVICE_PROPERTIES;


class CPower : public CSensor
{
public:
    CPower();
    ~CPower();
    
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

    HRESULT GetPropertyValuesForPowerObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT ProcessPowerAsyncRead( BYTE* buffer, ULONG length );
    HRESULT UpdatePowerPropertyValues( BYTE* pbSendReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported );

private:

    HRESULT InitializePower(VOID);
    HRESULT AddPowerPropertyKeys();
    HRESULT AddPowerSettablePropertyKeys();
    HRESULT AddPowerDataFieldKeys();
    HRESULT SetPowerDefaultValues();

    HRESULT SetSettablePowerProperties();

    POWER_DEVICE_PROPERTIES m_DeviceProperties;
};


const unsigned short SENSOR_POWER_NAME[]                        = L"Power";
const unsigned short SENSOR_POWER_DESCRIPTION[]                 = L"Power Sensor";
const char SENSOR_POWER_TRACE_NAME[]                            = "Power";

// Default Values
const FLOAT DEFAULT_POWER_SENSITIVITY                           = 0.05F;
const FLOAT DEFAULT_POWER_MAXIMUM                               = FLT_MAX;
const FLOAT DEFAULT_POWER_MINIMUM                               = -FLT_MAX;
const FLOAT DEFAULT_POWER_ACCURACY                              = FLT_MAX;
const FLOAT DEFAULT_POWER_RESOLUTION                            = FLT_MAX;

const FLOAT MIN_POWER_SENSITIVITY                               = -FLT_MAX;
const FLOAT MIN_POWER_MAXIMUM                                   = -FLT_MAX;
const FLOAT MAX_POWER_MINIMUM                                   = FLT_MAX;
const FLOAT MIN_POWER_ACCURACY                                  = -FLT_MAX;
const FLOAT MIN_POWER_RESOLUTION                                = -FLT_MAX;

const ULONG DEFAULT_POWER_MIN_REPORT_INTERVAL                   = 50;
const ULONG DEFAULT_POWER_CURRENT_REPORT_INTERVAL               = 100;


// For reference: Complete HID report descriptor

