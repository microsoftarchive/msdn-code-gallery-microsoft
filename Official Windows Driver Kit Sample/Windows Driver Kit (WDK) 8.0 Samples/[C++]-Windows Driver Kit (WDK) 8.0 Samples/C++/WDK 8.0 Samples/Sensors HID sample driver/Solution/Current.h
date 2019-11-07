/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Current.h
//
// Description:
//      Defines the CCurrent container class

--*/


#pragma once

typedef struct CURRENT_DEVICE_PROPERTIES
{
    // from input report
    BOOL   fSensorStateSupported;
    BOOL   fSensorStateSelectorSupported;
    BOOL   fEventTypeSupported;
    BOOL   fEventTypeSelectorSupported;

    // supported datafields
    BOOL   fCurrentSupported;

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
    BOOL   fCurrentSensitivitySupported;
    FLOAT  fltCurrentSensitivity;
    
    BOOL   fCurrentMaximumSupported;
    FLOAT  fltCurrentMaximum;
    BOOL   fCurrentMinimumSupported;
    FLOAT  fltCurrentMinimum;

    BOOL   fCurrentAccuracySupported;
    FLOAT  fltCurrentAccuracy;

    BOOL   fCurrentResolutionSupported;
    FLOAT  fltCurrentResolution;

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

} CURRENT_DEVICE_PROPERTIES, *PCURRENT_DEVICE_PROPERTIES;


class CCurrent : public CSensor
{
public:
    CCurrent();
    ~CCurrent();
    
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

    HRESULT GetPropertyValuesForCurrentObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT ProcessCurrentAsyncRead( BYTE* buffer, ULONG length );
    HRESULT UpdateCurrentPropertyValues( BYTE* pbSendReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported );

private:

    HRESULT InitializeCurrent(VOID);
    HRESULT AddCurrentPropertyKeys();
    HRESULT AddCurrentSettablePropertyKeys();
    HRESULT AddCurrentDataFieldKeys();
    HRESULT SetCurrentDefaultValues();

    HRESULT SetSettableCurrentProperties();

    CURRENT_DEVICE_PROPERTIES m_DeviceProperties;
};


const unsigned short SENSOR_CURRENT_NAME[]                        = L"Current";
const unsigned short SENSOR_CURRENT_DESCRIPTION[]                 = L"Current Sensor";
const char SENSOR_CURRENT_TRACE_NAME[]                            = "Current";

// Default Values
const FLOAT DEFAULT_CURRENT_SENSITIVITY                           = 0.02F;
const FLOAT DEFAULT_CURRENT_MAXIMUM                               = FLT_MAX;
const FLOAT DEFAULT_CURRENT_MINIMUM                               = -FLT_MAX;
const FLOAT DEFAULT_CURRENT_ACCURACY                              = FLT_MAX;
const FLOAT DEFAULT_CURRENT_RESOLUTION                            = FLT_MAX;

const FLOAT MIN_CURRENT_SENSITIVITY                               = -FLT_MAX;
const FLOAT MIN_CURRENT_MAXIMUM                                   = -FLT_MAX;
const FLOAT MAX_CURRENT_MINIMUM                                   = FLT_MAX;
const FLOAT MIN_CURRENT_ACCURACY                                  = -FLT_MAX;
const FLOAT MIN_CURRENT_RESOLUTION                                = -FLT_MAX;

const ULONG DEFAULT_CURRENT_MIN_REPORT_INTERVAL                   = 50;
const ULONG DEFAULT_CURRENT_CURRENT_REPORT_INTERVAL               = 100;




