/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Inclinometer.h
//
// Description:
//      Defines the CInclinometer container class

--*/


#pragma once

class CSensorManager; //forward declaration


typedef struct _INCLINOMETER_DEVICE_PROPERTIES
{
    // from input report
    BOOL   fSensorStateSupported;
    BOOL   fSensorStateSelectorSupported;
    BOOL   fEventTypeSupported;
    BOOL   fEventTypeSelectorSupported;

    // supported datafields
    BOOL   fInclinometerXAxisSupported;
    BOOL   fInclinometerYAxisSupported;
    BOOL   fInclinometerZAxisSupported;

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
    BOOL   fInclinometerSensitivitySupported;
    FLOAT  fltInclinometerSensitivity;
    BOOL   fInclinometerXSensitivitySupported;
    FLOAT  fltInclinometerXSensitivity;
    BOOL   fInclinometerYSensitivitySupported;
    FLOAT  fltInclinometerYSensitivity;
    BOOL   fInclinometerZSensitivitySupported;
    FLOAT  fltInclinometerZSensitivity;
    
    BOOL   fInclinometerMaximumSupported;
    FLOAT  fltInclinometerMaximum;
    BOOL   fInclinometerXMaximumSupported;
    FLOAT  fltInclinometerXMaximum;
    BOOL   fInclinometerYMaximumSupported;
    FLOAT  fltInclinometerYMaximum;
    BOOL   fInclinometerZMaximumSupported;
    FLOAT  fltInclinometerZMaximum;

    BOOL   fInclinometerMinimumSupported;
    FLOAT  fltInclinometerMinimum;
    BOOL   fInclinometerXMinimumSupported;
    FLOAT  fltInclinometerXMinimum;
    BOOL   fInclinometerYMinimumSupported;
    FLOAT  fltInclinometerYMinimum;
    BOOL   fInclinometerZMinimumSupported;
    FLOAT  fltInclinometerZMinimum;

    BOOL   fInclinometerAccuracySupported;
    FLOAT  fltInclinometerAccuracy;
    BOOL   fInclinometerXAccuracySupported;
    FLOAT  fltInclinometerXAccuracy;
    BOOL   fInclinometerYAccuracySupported;
    FLOAT  fltInclinometerYAccuracy;
    BOOL   fInclinometerZAccuracySupported;
    FLOAT  fltInclinometerZAccuracy;

    BOOL   fInclinometerResolutionSupported;
    FLOAT  fltInclinometerResolution;
    BOOL   fInclinometerXResolutionSupported;
    FLOAT  fltInclinometerXResolution;
    BOOL   fInclinometerYResolutionSupported;
    FLOAT  fltInclinometerYResolution;
    BOOL   fInclinometerZResolutionSupported;
    FLOAT  fltInclinometerZResolution;

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

} INCLINOMETER_DEVICE_PROPERTIES, *PINCLINOMETER_DEVICE_PROPERTIES;


class CInclinometer : public CSensor
{
public:
    CInclinometer();
    ~CInclinometer();
    
    HRESULT Initialize( _In_ SensorType sensType,
                        _In_ ULONG sensUsage,
                        _In_ USHORT sensLinkCollection,
                        _In_ DWORD sensNum, 
                        _In_ LPWSTR pwszManufacturer,
                        _In_ LPWSTR pwszProduct,
                        _In_ LPWSTR pwszSerialNumber,
                        _In_ LPWSTR pwszSensorID,
                        _In_ CSensorManager* pSensorManager);

    HRESULT GetPropertyValuesForInclinometerObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT ProcessInclinometerAsyncRead( BYTE* buffer, ULONG length );
    HRESULT UpdateInclinometerPropertyValues( BYTE* pbSendReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported );

private:

    HRESULT InitializeInclinometer(VOID);
    HRESULT AddInclinometerPropertyKeys();
    HRESULT AddInclinometerSettablePropertyKeys();
    HRESULT AddInclinometerDataFieldKeys();
    HRESULT SetInclinometerDefaultValues();

    HRESULT SetSettableInclinometerProperties();

    INCLINOMETER_DEVICE_PROPERTIES m_DeviceProperties;
};


const unsigned short SENSOR_INCLINOMETER_NAME[]                        = L"Inclinometer";
const unsigned short SENSOR_INCLINOMETER_DESCRIPTION[]                 = L"Inclinometer Sensor";
const char SENSOR_INCLINOMETER_TRACE_NAME[]                            = "Inclinometer";

// Default Values
const FLOAT DEFAULT_INCLINOMETER_SENSITIVITY                            = 0.5F;
const FLOAT DEFAULT_INCLINOMETER_MAXIMUM                                = FLT_MAX;
const FLOAT DEFAULT_INCLINOMETER_MINIMUM                                = -FLT_MAX;
const FLOAT DEFAULT_INCLINOMETER_ACCURACY                               = FLT_MAX;
const FLOAT DEFAULT_INCLINOMETER_RESOLUTION                             = FLT_MAX;

const FLOAT MIN_INCLINOMETER_SENSITIVITY                                = -FLT_MAX;
const FLOAT MIN_INCLINOMETER_MAXIMUM                                    = -FLT_MAX;
const FLOAT MAX_INCLINOMETER_MINIMUM                                    = FLT_MAX;
const FLOAT MIN_INCLINOMETER_ACCURACY                                   = -FLT_MAX;
const FLOAT MIN_INCLINOMETER_RESOLUTION                                 = -FLT_MAX;

const ULONG DEFAULT_INCLINOMETER_MIN_REPORT_INTERVAL                    = 16;
const ULONG DEFAULT_INCLINOMETER_CURRENT_REPORT_INTERVAL                = 50;



