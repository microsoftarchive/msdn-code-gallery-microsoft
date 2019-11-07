/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Potentiometer.h
//
// Description:
//      Defines the CPotentiometer container class

--*/


#pragma once

typedef struct _POTENTIOMETER_DEVICE_PROPERTIES
{
    // from input report
    BOOL   fSensorStateSupported;
    BOOL   fSensorStateSelectorSupported;
    BOOL   fEventTypeSupported;
    BOOL   fEventTypeSelectorSupported;

    // supported datafields
    BOOL   fPotentiometerSupported;

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
    BOOL   fPotentiometerSensitivitySupported;
    FLOAT  fltPotentiometerSensitivity;
    
    BOOL   fPotentiometerMaximumSupported;
    FLOAT  fltPotentiometerMaximum;
    BOOL   fPotentiometerMinimumSupported;
    FLOAT  fltPotentiometerMinimum;

    BOOL   fPotentiometerAccuracySupported;
    FLOAT  fltPotentiometerAccuracy;

    BOOL   fPotentiometerResolutionSupported;
    FLOAT  fltPotentiometerResolution;

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

} POTENTIOMETER_DEVICE_PROPERTIES, *PPOTENTIOMETER_DEVICE_PROPERTIES;


class CPotentiometer : public CSensor
{
public:
    CPotentiometer();
    ~CPotentiometer();
    
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

    HRESULT GetPropertyValuesForPotentiometerObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT ProcessPotentiometerAsyncRead( BYTE* buffer, ULONG length );
    HRESULT UpdatePotentiometerPropertyValues( BYTE* pbSendReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported );

private:

    HRESULT InitializePotentiometer(VOID);
    HRESULT AddPotentiometerPropertyKeys();
    HRESULT AddPotentiometerSettablePropertyKeys();
    HRESULT AddPotentiometerDataFieldKeys();
    HRESULT SetPotentiometerDefaultValues();

    HRESULT SetSettablePotentiometerProperties();

    POTENTIOMETER_DEVICE_PROPERTIES m_DeviceProperties;
};


const unsigned short SENSOR_POTENTIOMETER_NAME[]                        = L"Potentiometer";
const unsigned short SENSOR_POTENTIOMETER_DESCRIPTION[]                 = L"Potentiometer Sensor";
const char SENSOR_POTENTIOMETER_TRACE_NAME[]                            = "Potentiometer";

// Default Values
const FLOAT DEFAULT_POTENTIOMETER_SENSITIVITY                           = 0.25F;
const FLOAT DEFAULT_POTENTIOMETER_MAXIMUM                               = FLT_MAX;
const FLOAT DEFAULT_POTENTIOMETER_MINIMUM                               = -FLT_MAX;
const FLOAT DEFAULT_POTENTIOMETER_ACCURACY                              = FLT_MAX;
const FLOAT DEFAULT_POTENTIOMETER_RESOLUTION                            = FLT_MAX;

const FLOAT MIN_POTENTIOMETER_SENSITIVITY                               = -FLT_MAX;
const FLOAT MIN_POTENTIOMETER_MAXIMUM                                   = -FLT_MAX;
const FLOAT MAX_POTENTIOMETER_MINIMUM                                   = FLT_MAX;
const FLOAT MIN_POTENTIOMETER_ACCURACY                                  = -FLT_MAX;
const FLOAT MIN_POTENTIOMETER_RESOLUTION                                = -FLT_MAX;

const ULONG DEFAULT_POTENTIOMETER_MIN_REPORT_INTERVAL                   = 50;
const ULONG DEFAULT_POTENTIOMETER_CURRENT_REPORT_INTERVAL               = 100;





