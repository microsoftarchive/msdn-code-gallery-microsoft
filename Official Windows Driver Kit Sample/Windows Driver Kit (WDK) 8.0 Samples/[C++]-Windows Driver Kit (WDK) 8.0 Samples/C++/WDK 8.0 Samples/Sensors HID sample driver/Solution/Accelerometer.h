/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Accelerometer.h
//
// Description:
//      Defines the CAccelerometer container class

--*/


#pragma once

class CSensorManager; //forward declaration


typedef struct _ACCELEROMETER_DEVICE_PROPERTIES
{
    // from input report
    BOOL   fSensorStateSupported;
    BOOL   fSensorStateSelectorSupported;
    BOOL   fEventTypeSupported;
    BOOL   fEventTypeSelectorSupported;

    // supported datafields
    BOOL   fMotionIntensitySupported;
    BOOL   fAccelerometerXAxisSupported;
    BOOL   fAccelerometerYAxisSupported;
    BOOL   fAccelerometerZAxisSupported;
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
    BOOL   fAccelerometerSensitivitySupported;
    FLOAT  fltAccelerometerSensitivity;
    BOOL   fAccelerometerXSensitivitySupported;
    FLOAT  fltAccelerometerXSensitivity;
    BOOL   fAccelerometerYSensitivitySupported;
    FLOAT  fltAccelerometerYSensitivity;
    BOOL   fAccelerometerZSensitivitySupported;
    FLOAT  fltAccelerometerZSensitivity;
    
    BOOL   fAccelerometerMaximumSupported;
    FLOAT  fltAccelerometerMaximum;
    BOOL   fAccelerometerXMaximumSupported;
    FLOAT  fltAccelerometerXMaximum;
    BOOL   fAccelerometerYMaximumSupported;
    FLOAT  fltAccelerometerYMaximum;
    BOOL   fAccelerometerZMaximumSupported;
    FLOAT  fltAccelerometerZMaximum;

    BOOL   fAccelerometerMinimumSupported;
    FLOAT  fltAccelerometerMinimum;
    BOOL   fAccelerometerXMinimumSupported;
    FLOAT  fltAccelerometerXMinimum;
    BOOL   fAccelerometerYMinimumSupported;
    FLOAT  fltAccelerometerYMinimum;
    BOOL   fAccelerometerZMinimumSupported;
    FLOAT  fltAccelerometerZMinimum;

    BOOL   fAccelerometerAccuracySupported;
    FLOAT  fltAccelerometerAccuracy;
    BOOL   fAccelerometerXAccuracySupported;
    FLOAT  fltAccelerometerXAccuracy;
    BOOL   fAccelerometerYAccuracySupported;
    FLOAT  fltAccelerometerYAccuracy;
    BOOL   fAccelerometerZAccuracySupported;
    FLOAT  fltAccelerometerZAccuracy;

    BOOL   fAccelerometerResolutionSupported;
    FLOAT  fltAccelerometerResolution;
    BOOL   fAccelerometerXResolutionSupported;
    FLOAT  fltAccelerometerXResolution;
    BOOL   fAccelerometerYResolutionSupported;
    FLOAT  fltAccelerometerYResolution;
    BOOL   fAccelerometerZResolutionSupported;
    FLOAT  fltAccelerometerZResolution;

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

} ACCELEROMETER_DEVICE_PROPERTIES, *PACCELEROMETER_DEVICE_PROPERTIES;


class CAccelerometer : public CSensor
{
public:
    CAccelerometer();
    ~CAccelerometer();
    
    HRESULT Initialize( _In_ SensorType sensType,
                        _In_ ULONG sensUsage,
                        _In_ USHORT sensLinkCollection,
                        _In_ DWORD sensNum, 
                        _In_ LPWSTR pwszManufacturer,
                        _In_ LPWSTR pwszProduct,
                        _In_ LPWSTR pwszSerialNumber,
                        _In_ LPWSTR pwszSensorID,
                        _In_ CSensorManager* pSensorManager);

    HRESULT GetPropertyValuesForAccelerometerObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT ProcessAccelerometerAsyncRead( BYTE* buffer, ULONG length );
    HRESULT UpdateAccelerometerPropertyValues( BYTE* pbSendReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported );

    VOID    RaiseShakeEvent();
    BOOL    HasValidShakeEvent();

private:

    HRESULT InitializeAccelerometer(VOID);
    HRESULT AddAccelerometerPropertyKeys();
    HRESULT AddAccelerometerSettablePropertyKeys();
    HRESULT AddAccelerometerDataFieldKeys();
    HRESULT SetAccelerometerDefaultValues();

    HRESULT SetSettableAccelerometerProperties();

    ACCELEROMETER_DEVICE_PROPERTIES m_DeviceProperties;

    BOOL    m_fValidShakeEvent;                       // flag that indicates a shake event needs to be fired
};


const unsigned short SENSOR_ACCELEROMETER_NAME[]                        = L"Accelerometer";
const unsigned short SENSOR_ACCELEROMETER_DESCRIPTION[]                 = L"Accelerometer Sensor";
const char SENSOR_ACCELEROMETER_TRACE_NAME[]                            = "Accelerometer";

// Default Values
const FLOAT DEFAULT_ACCELEROMETER_SENSITIVITY                            = 0.02F;
const FLOAT DEFAULT_ACCELEROMETER_MAXIMUM                                = FLT_MAX;
const FLOAT DEFAULT_ACCELEROMETER_MINIMUM                                = -FLT_MAX;
const FLOAT DEFAULT_ACCELEROMETER_ACCURACY                               = FLT_MAX;
const FLOAT DEFAULT_ACCELEROMETER_RESOLUTION                             = FLT_MAX;

const FLOAT MIN_ACCELEROMETER_SENSITIVITY                                = -FLT_MAX;
const FLOAT MIN_ACCELEROMETER_MAXIMUM                                    = -FLT_MAX;
const FLOAT MAX_ACCELEROMETER_MINIMUM                                    = FLT_MAX;
const FLOAT MIN_ACCELEROMETER_ACCURACY                                   = -FLT_MAX;
const FLOAT MIN_ACCELEROMETER_RESOLUTION                                 = -FLT_MAX;

const ULONG DEFAULT_ACCELEROMETER_MIN_REPORT_INTERVAL                    = 16;
const ULONG DEFAULT_ACCELEROMETER_CURRENT_REPORT_INTERVAL                = 100;




