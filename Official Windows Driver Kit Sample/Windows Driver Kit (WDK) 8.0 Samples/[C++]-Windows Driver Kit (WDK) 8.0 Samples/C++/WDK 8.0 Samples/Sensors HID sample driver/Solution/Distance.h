/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Distance.h
//
// Description:
//      Defines the CDistance container class

--*/


#pragma once

class CSensorManager; //forward declaration


typedef struct _DISTANCE_DEVICE_PROPERTIES
{
    // from input report
    BOOL   fSensorStateSupported;
    BOOL   fSensorStateSelectorSupported;
    BOOL   fEventTypeSupported;
    BOOL   fEventTypeSelectorSupported;

    // supported datafields
    BOOL   fDistanceXAxisSupported;
    BOOL   fDistanceYAxisSupported;
    BOOL   fDistanceZAxisSupported;

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
    BOOL   fDistanceSensitivitySupported;
    FLOAT  fltDistanceSensitivity;
    BOOL   fDistanceXSensitivitySupported;
    FLOAT  fltDistanceXSensitivity;
    BOOL   fDistanceYSensitivitySupported;
    FLOAT  fltDistanceYSensitivity;
    BOOL   fDistanceZSensitivitySupported;
    FLOAT  fltDistanceZSensitivity;
    
    BOOL   fDistanceMaximumSupported;
    FLOAT  fltDistanceMaximum;
    BOOL   fDistanceXMaximumSupported;
    FLOAT  fltDistanceXMaximum;
    BOOL   fDistanceYMaximumSupported;
    FLOAT  fltDistanceYMaximum;
    BOOL   fDistanceZMaximumSupported;
    FLOAT  fltDistanceZMaximum;

    BOOL   fDistanceMinimumSupported;
    FLOAT  fltDistanceMinimum;
    BOOL   fDistanceXMinimumSupported;
    FLOAT  fltDistanceXMinimum;
    BOOL   fDistanceYMinimumSupported;
    FLOAT  fltDistanceYMinimum;
    BOOL   fDistanceZMinimumSupported;
    FLOAT  fltDistanceZMinimum;

    BOOL   fDistanceAccuracySupported;
    FLOAT  fltDistanceAccuracy;
    BOOL   fDistanceXAccuracySupported;
    FLOAT  fltDistanceXAccuracy;
    BOOL   fDistanceYAccuracySupported;
    FLOAT  fltDistanceYAccuracy;
    BOOL   fDistanceZAccuracySupported;
    FLOAT  fltDistanceZAccuracy;

    BOOL   fDistanceResolutionSupported;
    FLOAT  fltDistanceResolution;
    BOOL   fDistanceXResolutionSupported;
    FLOAT  fltDistanceXResolution;
    BOOL   fDistanceYResolutionSupported;
    FLOAT  fltDistanceYResolution;
    BOOL   fDistanceZResolutionSupported;
    FLOAT  fltDistanceZResolution;

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

} DISTANCE_DEVICE_PROPERTIES, *PDISTANCE_DEVICE_PROPERTIES;


class CDistance : public CSensor
{
public:
    CDistance();
    ~CDistance();
    
    HRESULT Initialize( _In_ SensorType sensType,
                        _In_ ULONG sensUsage,
                        _In_ USHORT sensLinkCollection,
                        _In_ DWORD sensNum, 
                        _In_ LPWSTR pwszManufacturer,
                        _In_ LPWSTR pwszProduct,
                        _In_ LPWSTR pwszSerialNumber,
                        _In_ LPWSTR pwszSensorID,
                        _In_ CSensorManager* pSensorManager);

    HRESULT GetPropertyValuesForDistanceObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT ProcessDistanceAsyncRead( BYTE* buffer, ULONG length );
    HRESULT UpdateDistancePropertyValues( BYTE* pbSendReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported );

private:

    HRESULT InitializeDistance(VOID);
    HRESULT AddDistancePropertyKeys();
    HRESULT AddDistanceSettablePropertyKeys();
    HRESULT AddDistanceDataFieldKeys();
    HRESULT SetDistanceDefaultValues();

    HRESULT SetSettableDistanceProperties();

    DISTANCE_DEVICE_PROPERTIES m_DeviceProperties;
};


const unsigned short SENSOR_DISTANCE_NAME[]                        = L"Distance";
const unsigned short SENSOR_DISTANCE_DESCRIPTION[]                 = L"Distance Sensor";
const char SENSOR_DISTANCE_TRACE_NAME[]                            = "Distance";

// Default Values
const FLOAT DEFAULT_DISTANCE_SENSITIVITY                            = 0.01F;
const FLOAT DEFAULT_DISTANCE_MAXIMUM                                = FLT_MAX;
const FLOAT DEFAULT_DISTANCE_MINIMUM                                = -FLT_MAX;
const FLOAT DEFAULT_DISTANCE_ACCURACY                               = FLT_MAX;
const FLOAT DEFAULT_DISTANCE_RESOLUTION                             = FLT_MAX;

const FLOAT MIN_DISTANCE_SENSITIVITY                                = -FLT_MAX;
const FLOAT MIN_DISTANCE_MAXIMUM                                    = -FLT_MAX;
const FLOAT MAX_DISTANCE_MINIMUM                                    = FLT_MAX;
const FLOAT MIN_DISTANCE_ACCURACY                                   = -FLT_MAX;
const FLOAT MIN_DISTANCE_RESOLUTION                                 = -FLT_MAX;

const ULONG DEFAULT_DISTANCE_MIN_REPORT_INTERVAL                    = 50;
const ULONG DEFAULT_DISTANCE_CURRENT_REPORT_INTERVAL                = 100;



