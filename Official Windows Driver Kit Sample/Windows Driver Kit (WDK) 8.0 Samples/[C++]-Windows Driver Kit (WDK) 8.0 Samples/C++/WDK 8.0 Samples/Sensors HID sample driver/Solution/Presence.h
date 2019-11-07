/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Presence.h
//
// Description:
//      Defines the CPresence container class

--*/


#pragma once

typedef struct _PRESPROX_DEVICE_PROPERTIES
{
    // from input report
    BOOL   fSensorStateSupported;
    BOOL   fSensorStateSelectorSupported;
    BOOL   fEventTypeSupported;
    BOOL   fEventTypeSelectorSupported;

    // supported datafields
    BOOL   fPresenceSupported;
    BOOL   fProximityRangeSupported;
    BOOL   fProximityOutOfRangeSupported;

    BOOL   fOutOfRange;

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
    BOOL   fPresProxSensitivitySupported;
    FLOAT  fltPresProxSensitivity;
    
    BOOL   fProximityMaximumSupported;
    FLOAT  fltProximityMaximum;

    BOOL   fProximityMinimumSupported;
    FLOAT  fltProximityMinimum;

    BOOL   fProximityAccuracySupported;
    FLOAT  fltProximityAccuracy;

    BOOL   fProximityResolutionSupported;
    FLOAT  fltProximityResolution;

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

} PRESPROX_DEVICE_PROPERTIES, *PPRESPROX_DEVICE_PROPERTIES;


class CPresence : public CSensor
{
public:
    CPresence();
    ~CPresence();
    
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

    HRESULT GetPropertyValuesForPresenceObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT ProcessPresenceAsyncRead( BYTE* buffer, ULONG length );
    HRESULT UpdatePresencePropertyValues( BYTE* pbSendReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported );

private:

    HRESULT InitializePresence(VOID);
    HRESULT AddPresencePropertyKeys();
    HRESULT AddPresenceSettablePropertyKeys();
    HRESULT AddPresenceDataFieldKeys();
    HRESULT SetPresenceDefaultValues();

    HRESULT SetSettablePresenceProperties();

    PRESPROX_DEVICE_PROPERTIES m_DeviceProperties;
};


const unsigned short SENSOR_PRESENCE_NAME[]                        = L"Human Presence";
const unsigned short SENSOR_PRESENCE_DESCRIPTION[]                 = L"Human Presence Sensor";
const char SENSOR_PRESENCE_TRACE_NAME[]                            = "Human Presence";

const unsigned short SENSOR_PROXIMITY_NAME[]                       = L"Human Proximity";
const unsigned short SENSOR_PROXIMITY_DESCRIPTION[]                = L"Human Proximity Sensor";
const char SENSOR_PROXIMITY_TRACE_NAME[]                           = "Human Proximity";

// Default Values
const FLOAT DEFAULT_PRESENCE_SENSITIVITY                           = 0.0F;
const FLOAT DEFAULT_PROXIMITY_SENSITIVITY                          = 0.01F;
const FLOAT DEFAULT_PROXIMITY_MAXIMUM                              = FLT_MAX;
const FLOAT DEFAULT_PROXIMITY_MINIMUM                              = -FLT_MAX;
const FLOAT DEFAULT_PROXIMITY_ACCURACY                             = FLT_MAX;
const FLOAT DEFAULT_PROXIMITY_RESOLUTION                           = FLT_MAX;

const FLOAT MIN_PRESENCE_SENSITIVITY                               = -FLT_MAX;
const FLOAT MIN_PROXIMITY_SENSITIVITY                              = -FLT_MAX;
const FLOAT MIN_PROXIMITY_MAXIMUM                                  = -FLT_MAX;
const FLOAT MAX_PROXIMITY_MINIMUM                                  = FLT_MAX;
const FLOAT MIN_PROXIMITY_ACCURACY                                 = -FLT_MAX;
const FLOAT MIN_PROXIMITY_RESOLUTION                               = -FLT_MAX;

const ULONG DEFAULT_PRESPROX_MIN_REPORT_INTERVAL                   = 50;
const ULONG DEFAULT_PRESPROX_CURRENT_REPORT_INTERVAL               = 100;




