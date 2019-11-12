/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Switch.h
//
// Description:
//      Defines the CSwitch container class

--*/


#pragma once

typedef struct _SWITCH_DEVICE_PROPERTIES
{
    // from input report
    BOOL   fSensorStateSupported;
    BOOL   fSensorStateSelectorSupported;
    BOOL   fEventTypeSupported;
    BOOL   fEventTypeSelectorSupported;

    // supported datafields
    BOOL   fSwitchSupported;
    BOOL   fSwitchArraySupported;
    BOOL   fSwitchMultivalSupported;
    BOOL   fSwitchMotionSupported;
    BOOL   fSwitchTouchSupported;

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
    BOOL   fSwitchSensitivitySupported;
    FLOAT  fltSwitchSensitivity;
    
    BOOL   fSwitchMaximumSupported;
    FLOAT  fltSwitchMaximum;
    BOOL   fSwitchMinimumSupported;
    FLOAT  fltSwitchMinimum;

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

} SWITCH_DEVICE_PROPERTIES, *PSWITCH_DEVICE_PROPERTIES;


class CSwitch : public CSensor
{
public:
    CSwitch();
    ~CSwitch();
    
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

    HRESULT GetPropertyValuesForSwitchObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT ProcessSwitchAsyncRead( BYTE* buffer, ULONG length );
    HRESULT UpdateSwitchPropertyValues( BYTE* pbSendReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported );

private:

    HRESULT InitializeSwitch(VOID);
    HRESULT AddSwitchPropertyKeys();
    HRESULT AddSwitchSettablePropertyKeys();
    HRESULT AddSwitchDataFieldKeys();
    HRESULT SetSwitchDefaultValues();

    HRESULT SetSettableSwitchProperties();

    SWITCH_DEVICE_PROPERTIES m_DeviceProperties;
};


const unsigned short SENSOR_SWITCH_NAME[]                       = L"Switch";
const unsigned short SENSOR_SWITCH_DESCRIPTION[]                = L"Switch Sensor";
const char SENSOR_SWITCH_TRACE_NAME[]                           = "Switch";

const unsigned short SENSOR_SWITCH_MULTIVAL_NAME[]              = L"Multi-value Switch";
const unsigned short SENSOR_SWITCH_MULTIVAL_DESCRIPTION[]       = L"Multi-value Switch Sensor";
const char SENSOR_SWITCH_MULTIVAL_TRACE_NAME[]                  = "Multi-value Switch";

const unsigned short SENSOR_SWITCH_ARRAY_NAME[]                 = L"Switch Array";
const unsigned short SENSOR_SWITCH_ARRAY_DESCRIPTION[]          = L"Switch Array Sensor";
const char SENSOR_SWITCH_ARRAY_TRACE_NAME[]                     = "Switch Array";

const unsigned short SENSOR_TOUCH_NAME[]                        = L"Human Touch";
const unsigned short SENSOR_TOUCH_DESCRIPTION[]                 = L"Human Touch Sensor";
const char SENSOR_TOUCH_TRACE_NAME[]                            = "Human Touch";

const unsigned short SENSOR_MOTION_NAME[]                       = L"Motion Detector";
const unsigned short SENSOR_MOTION_DESCRIPTION[]                = L"Motion Detector Sensor";
const char SENSOR_MOTION_TRACE_NAME[]                           = "Motion Detector";

// Default Values
const FLOAT DEFAULT_SWITCH_SENSITIVITY                          = 0.0F;
const FLOAT DEFAULT_SWITCH_MAXIMUM                              = FLT_MAX; //position max
const FLOAT DEFAULT_SWITCH_MINIMUM                              = -FLT_MAX;   //position min
const FLOAT DEFAULT_SWITCH_ACCURACY                             = FLT_MAX;
const FLOAT DEFAULT_SWITCH_RESOLUTION                           = FLT_MAX;

const FLOAT MIN_SWITCH_SENSITIVITY                              = -FLT_MAX;
const FLOAT MIN_SWITCH_MAXIMUM                                  = -FLT_MAX;
const FLOAT MAX_SWITCH_MINIMUM                                  = FLT_MAX;
const FLOAT MIN_SWITCH_ACCURACY                                 = -FLT_MAX;
const FLOAT MIN_SWITCH_RESOLUTION                               = -FLT_MAX;

const ULONG DEFAULT_SWITCH_MIN_REPORT_INTERVAL                  = 20;
const ULONG DEFAULT_SWITCH_CURRENT_REPORT_INTERVAL              = 100;




