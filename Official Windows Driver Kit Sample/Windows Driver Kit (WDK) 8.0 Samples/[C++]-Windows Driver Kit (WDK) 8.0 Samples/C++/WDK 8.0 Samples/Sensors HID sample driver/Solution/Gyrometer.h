/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Gyrometer.h
//
// Description:
//      Defines the CGyrometer container class

--*/


#pragma once

class CSensorManager; //forward declaration


typedef struct _GYROMETER_DEVICE_PROPERTIES
{
    // from input report
    BOOL   fSensorStateSupported;
    BOOL   fSensorStateSelectorSupported;
    BOOL   fEventTypeSupported;
    BOOL   fEventTypeSelectorSupported;

    // supported datafields
    BOOL   fGyrometerXAxisSupported;
    BOOL   fGyrometerYAxisSupported;
    BOOL   fGyrometerZAxisSupported;

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
    BOOL   fGyrometerSensitivitySupported;
    FLOAT  fltGyrometerSensitivity;
    BOOL   fGyrometerXSensitivitySupported;
    FLOAT  fltGyrometerXSensitivity;
    BOOL   fGyrometerYSensitivitySupported;
    FLOAT  fltGyrometerYSensitivity;
    BOOL   fGyrometerZSensitivitySupported;
    FLOAT  fltGyrometerZSensitivity;
    
    BOOL   fGyrometerMaximumSupported;
    FLOAT  fltGyrometerMaximum;
    BOOL   fGyrometerXMaximumSupported;
    FLOAT  fltGyrometerXMaximum;
    BOOL   fGyrometerYMaximumSupported;
    FLOAT  fltGyrometerYMaximum;
    BOOL   fGyrometerZMaximumSupported;
    FLOAT  fltGyrometerZMaximum;

    BOOL   fGyrometerMinimumSupported;
    FLOAT  fltGyrometerMinimum;
    BOOL   fGyrometerXMinimumSupported;
    FLOAT  fltGyrometerXMinimum;
    BOOL   fGyrometerYMinimumSupported;
    FLOAT  fltGyrometerYMinimum;
    BOOL   fGyrometerZMinimumSupported;
    FLOAT  fltGyrometerZMinimum;

    BOOL   fGyrometerAccuracySupported;
    FLOAT  fltGyrometerAccuracy;
    BOOL   fGyrometerXAccuracySupported;
    FLOAT  fltGyrometerXAccuracy;
    BOOL   fGyrometerYAccuracySupported;
    FLOAT  fltGyrometerYAccuracy;
    BOOL   fGyrometerZAccuracySupported;
    FLOAT  fltGyrometerZAccuracy;

    BOOL   fGyrometerResolutionSupported;
    FLOAT  fltGyrometerResolution;
    BOOL   fGyrometerXResolutionSupported;
    FLOAT  fltGyrometerXResolution;
    BOOL   fGyrometerYResolutionSupported;
    FLOAT  fltGyrometerYResolution;
    BOOL   fGyrometerZResolutionSupported;
    FLOAT  fltGyrometerZResolution;

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

} GYROMETER_DEVICE_PROPERTIES, *PGYROMETER_DEVICE_PROPERTIES;


class CGyrometer : public CSensor
{
public:
    CGyrometer();
    ~CGyrometer();
    
    HRESULT Initialize( _In_ SensorType sensType,
                        _In_ ULONG sensUsage,
                        _In_ USHORT sensLinkCollection,
                        _In_ DWORD sensNum, 
                        _In_ LPWSTR pwszManufacturer,
                        _In_ LPWSTR pwszProduct,
                        _In_ LPWSTR pwszSerialNumber,
                        _In_ LPWSTR pwszSensorID,
                        _In_ CSensorManager* pSensorManager);

    HRESULT GetPropertyValuesForGyrometerObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT ProcessGyrometerAsyncRead( BYTE* buffer, ULONG length );
    HRESULT UpdateGyrometerPropertyValues( BYTE* pbSendReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported );

private:

    HRESULT InitializeGyrometer(VOID);
    HRESULT AddGyrometerPropertyKeys();
    HRESULT AddGyrometerSettablePropertyKeys();
    HRESULT AddGyrometerDataFieldKeys();
    HRESULT SetGyrometerDefaultValues();

    HRESULT SetSettableGyrometerProperties();

    GYROMETER_DEVICE_PROPERTIES m_DeviceProperties;
};


const unsigned short SENSOR_GYROMETER_NAME[]                        = L"Gyrometer";
const unsigned short SENSOR_GYROMETER_DESCRIPTION[]                 = L"Gyrometer Sensor";
const char SENSOR_GYROMETER_TRACE_NAME[]                            = "Gyrometer";

// Default Values
const FLOAT DEFAULT_GYROMETER_SENSITIVITY                            = 0.5F;
const FLOAT DEFAULT_GYROMETER_MAXIMUM                                = FLT_MAX;
const FLOAT DEFAULT_GYROMETER_MINIMUM                                = -FLT_MAX;
const FLOAT DEFAULT_GYROMETER_ACCURACY                               = FLT_MAX;
const FLOAT DEFAULT_GYROMETER_RESOLUTION                             = FLT_MAX;

const FLOAT MIN_GYROMETER_SENSITIVITY                                = -FLT_MAX;
const FLOAT MIN_GYROMETER_MAXIMUM                                    = -FLT_MAX;
const FLOAT MAX_GYROMETER_MINIMUM                                    = FLT_MAX;
const FLOAT MIN_GYROMETER_ACCURACY                                   = -FLT_MAX;
const FLOAT MIN_GYROMETER_RESOLUTION                                 = -FLT_MAX;

const ULONG DEFAULT_GYROMETER_MIN_REPORT_INTERVAL                    = 16;
const ULONG DEFAULT_GYROMETER_CURRENT_REPORT_INTERVAL                = 100;



