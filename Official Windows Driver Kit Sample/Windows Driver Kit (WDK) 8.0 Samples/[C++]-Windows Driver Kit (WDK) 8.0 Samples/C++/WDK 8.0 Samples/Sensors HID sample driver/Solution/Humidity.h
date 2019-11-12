/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Hygrometer.h
//
// Description:
//      Defines the CHygrometer container class

--*/


#pragma once

typedef struct _HYGROMETER_DEVICE_PROPERTIES
{
    // from input report
    BOOL   fSensorStateSupported;
    BOOL   fSensorStateSelectorSupported;
    BOOL   fEventTypeSupported;
    BOOL   fEventTypeSelectorSupported;

    // supported datafields
    BOOL   fHygrometerSupported;

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
    BOOL   fHygrometerSensitivitySupported;
    FLOAT  fltHygrometerSensitivity;
    
    BOOL   fHygrometerMaximumSupported;
    FLOAT  fltHygrometerMaximum;
    BOOL   fHygrometerMinimumSupported;
    FLOAT  fltHygrometerMinimum;

    BOOL   fHygrometerAccuracySupported;
    FLOAT  fltHygrometerAccuracy;

    BOOL   fHygrometerResolutionSupported;
    FLOAT  fltHygrometerResolution;

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

} HYGROMETER_DEVICE_PROPERTIES, *PHYGROMETER_DEVICE_PROPERTIES;


class CHygrometer : public CSensor
{
public:
    CHygrometer();
    ~CHygrometer();
    
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

    HRESULT GetPropertyValuesForHygrometerObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT ProcessHygrometerAsyncRead( BYTE* buffer, ULONG length );
    HRESULT UpdateHygrometerPropertyValues( BYTE* pbSendReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported );

private:

    HRESULT InitializeHygrometer(VOID);
    HRESULT AddHygrometerPropertyKeys();
    HRESULT AddHygrometerSettablePropertyKeys();
    HRESULT AddHygrometerDataFieldKeys();
    HRESULT SetHygrometerDefaultValues();

    HRESULT SetSettableHygrometerProperties();

    HYGROMETER_DEVICE_PROPERTIES m_DeviceProperties;
};


const unsigned short SENSOR_HYGROMETER_NAME[]                        = L"Hygrometer";
const unsigned short SENSOR_HYGROMETER_DESCRIPTION[]                 = L"Hygrometer Sensor";
const char SENSOR_HYGROMETER_TRACE_NAME[]                            = "Hygrometer";

// Default Values
const FLOAT DEFAULT_HYGROMETER_SENSITIVITY                           = 0.25F;
const FLOAT DEFAULT_HYGROMETER_MAXIMUM                               = FLT_MAX;
const FLOAT DEFAULT_HYGROMETER_MINIMUM                               = -FLT_MAX;
const FLOAT DEFAULT_HYGROMETER_ACCURACY                              = FLT_MAX;
const FLOAT DEFAULT_HYGROMETER_RESOLUTION                            = FLT_MAX;

const FLOAT MIN_HYGROMETER_SENSITIVITY                               = -FLT_MAX;
const FLOAT MIN_HYGROMETER_MAXIMUM                                   = -FLT_MAX;
const FLOAT MAX_HYGROMETER_MINIMUM                                   = FLT_MAX;
const FLOAT MIN_HYGROMETER_ACCURACY                                  = -FLT_MAX;
const FLOAT MIN_HYGROMETER_RESOLUTION                                = -FLT_MAX;

const ULONG DEFAULT_HYGROMETER_MIN_REPORT_INTERVAL                   = 50;
const ULONG DEFAULT_HYGROMETER_CURRENT_REPORT_INTERVAL               = 100;




