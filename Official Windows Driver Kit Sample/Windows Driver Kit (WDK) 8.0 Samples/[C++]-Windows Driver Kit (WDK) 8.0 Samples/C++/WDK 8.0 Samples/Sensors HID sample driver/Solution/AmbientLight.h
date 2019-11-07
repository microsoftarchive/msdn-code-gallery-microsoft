/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      AmbientLight.h
//
// Description:
//      Defines the CAmbientLight container class

--*/


#pragma once


const ULONG AMBIENTLIGHT_MAX_RESPONSE_CURVE_XY_PAIRS = 20;


typedef struct _AMBIENTLIGHT_DEVICE_PROPERTIES
{
    // from input report
    BOOL   fSensorStateSupported;
    BOOL   fSensorStateSelectorSupported;
    BOOL   fEventTypeSupported;
    BOOL   fEventTypeSelectorSupported;

    // supported datafields
    BOOL   fAmbientLightIlluminanceSupported;
    BOOL   fAmbientLightColorTempSupported;
    BOOL   fAmbientLightChromaticitySupported;
    BOOL   fAmbientLightChromaticityXSupported;
    BOOL   fAmbientLightChromaticityYSupported;

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
    BOOL   fUsingSensitivityAbs;
    BOOL   fUsingSensitivityRelPct;
    BOOL   fAmbientLightSensitivitySupported;
    FLOAT  fltAmbientLightSensitivity;
    BOOL   fAmbientLightIlluminanceSensitivitySupported;
    FLOAT  fltAmbientLightIlluminanceSensitivity;
    BOOL   fAmbientLightColorTempSensitivitySupported;
    FLOAT  fltAmbientLightColorTempSensitivity;
    BOOL   fAmbientLightChromaticitySensitivitySupported;
    FLOAT  fltAmbientLightChromaticitySensitivity;
    BOOL   fAmbientLightChromaticityXSensitivitySupported;
    FLOAT  fltAmbientLightChromaticityXSensitivity;
    BOOL   fAmbientLightChromaticityYSensitivitySupported;
    FLOAT  fltAmbientLightChromaticityYSensitivity;
    
    BOOL   fAmbientLightMaximumSupported;
    FLOAT  fltAmbientLightMaximum;
    BOOL   fAmbientLightIlluminanceMaximumSupported;
    FLOAT  fltAmbientLightIlluminanceMaximum;
    BOOL   fAmbientLightColorTempMaximumSupported;
    FLOAT  fltAmbientLightColorTempMaximum;
    BOOL   fAmbientLightChromaticityMaximumSupported;
    FLOAT  fltAmbientLightChromaticityMaximum;
    BOOL   fAmbientLightChromaticityXMaximumSupported;
    FLOAT  fltAmbientLightChromaticityXMaximum;
    BOOL   fAmbientLightChromaticityYMaximumSupported;
    FLOAT  fltAmbientLightChromaticityYMaximum;

    BOOL   fAmbientLightMinimumSupported;
    FLOAT  fltAmbientLightMinimum;
    BOOL   fAmbientLightIlluminanceMinimumSupported;
    FLOAT  fltAmbientLightIlluminanceMinimum;
    BOOL   fAmbientLightColorTempMinimumSupported;
    FLOAT  fltAmbientLightColorTempMinimum;
    BOOL   fAmbientLightChromaticityMinimumSupported;
    FLOAT  fltAmbientLightChromaticityMinimum;
    BOOL   fAmbientLightChromaticityXMinimumSupported;
    FLOAT  fltAmbientLightChromaticityXMinimum;
    BOOL   fAmbientLightChromaticityYMinimumSupported;
    FLOAT  fltAmbientLightChromaticityYMinimum;

    BOOL   fAmbientLightAccuracySupported;
    FLOAT  fltAmbientLightAccuracy;
    BOOL   fAmbientLightIlluminanceAccuracySupported;
    FLOAT  fltAmbientLightIlluminanceAccuracy;
    BOOL   fAmbientLightColorTempAccuracySupported;
    FLOAT  fltAmbientLightColorTempAccuracy;
    BOOL   fAmbientLightChromaticityAccuracySupported;
    FLOAT  fltAmbientLightChromaticityAccuracy;
    BOOL   fAmbientLightChromaticityXAccuracySupported;
    FLOAT  fltAmbientLightChromaticityXAccuracy;
    BOOL   fAmbientLightChromaticityYAccuracySupported;
    FLOAT  fltAmbientLightChromaticityYAccuracy;

    BOOL   fAmbientLightResolutionSupported;
    FLOAT  fltAmbientLightResolution;
    BOOL   fAmbientLightIlluminanceResolutionSupported;
    FLOAT  fltAmbientLightIlluminanceResolution;
    BOOL   fAmbientLightColorTempResolutionSupported;
    FLOAT  fltAmbientLightColorTempResolution;
    BOOL   fAmbientLightChromaticityResolutionSupported;
    FLOAT  fltAmbientLightChromaticityResolution;
    BOOL   fAmbientLightChromaticityXResolutionSupported;
    FLOAT  fltAmbientLightChromaticityXResolution;
    BOOL   fAmbientLightChromaticityYResolutionSupported;
    FLOAT  fltAmbientLightChromaticityYResolution;

    //Extended properties
    BOOL   fAmbientLightResponseCurveSupported;
    ULONG  ulAmbientLightResponseCurveCount;
    DWORD  dwAmbientLightResponseCurveXYPairs[AMBIENTLIGHT_MAX_RESPONSE_CURVE_XY_PAIRS*2];

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

} AMBIENTLIGHT_DEVICE_PROPERTIES, *PAMBIENTLIGHT_DEVICE_PROPERTIES;


class CAmbientLight : public CSensor
{
public:
    CAmbientLight();
    ~CAmbientLight();
    
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

    HRESULT GetPropertyValuesForAmbientLightObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT ProcessAmbientLightAsyncRead( BYTE* buffer, ULONG length );
    HRESULT UpdateAmbientLightPropertyValues( BYTE* pbSendReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported );

private:

    HRESULT InitializeAmbientLight(VOID);
    HRESULT AddAmbientLightPropertyKeys();
    HRESULT AddAmbientLightSettablePropertyKeys();
    HRESULT AddAmbientLightDataFieldKeys();
    HRESULT SetAmbientLightDefaultValues();

    HRESULT SetSettableAmbientLightProperties();
    
    AMBIENTLIGHT_DEVICE_PROPERTIES m_DeviceProperties;

    FLOAT m_fltDefaultColorTempSensitivity;
    FLOAT m_fltDefaultChromaticitySensitivity;
    FLOAT m_fltDefaultColorTempMaximum;
    FLOAT m_fltDefaultChromaticityMaximum;
    FLOAT m_fltDefaultColorTempMinimum;
    FLOAT m_fltDefaultChromaticityMinimum;
    FLOAT m_fltDefaultColorTempAccuracy;
    FLOAT m_fltDefaultChromaticityAccuracy;
    FLOAT m_fltDefaultColorTempResolution;
    FLOAT m_fltDefaultChromaticityResolution;
};


const unsigned short SENSOR_AMBIENTLIGHT_NAME[]                 = L"Ambient Light";
const unsigned short SENSOR_AMBIENTLIGHT_DESCRIPTION[]          = L"Ambient Light Sensor";
const char SENSOR_AMBIENTLIGHT_TRACE_NAME[]                     = "Ambient Light";

// Default Values
const FLOAT DEFAULT_AMBIENTLIGHT_ILLUMINANCE_SENSITIVITY               = 1.0F;
const FLOAT DEFAULT_AMBIENTLIGHT_ILLUMINANCE_MAXIMUM                   = FLT_MAX;
const FLOAT DEFAULT_AMBIENTLIGHT_ILLUMINANCE_MINIMUM                   = -FLT_MAX;
const FLOAT DEFAULT_AMBIENTLIGHT_ILLUMINANCE_ACCURACY                  = FLT_MAX;
const FLOAT DEFAULT_AMBIENTLIGHT_ILLUMINANCE_RESOLUTION                = FLT_MAX;

const FLOAT DEFAULT_AMBIENTLIGHT_COLORTEMP_SENSITIVITY                 = 0.2F;
const FLOAT DEFAULT_AMBIENTLIGHT_COLORTEMP_MAXIMUM                     = FLT_MAX;
const FLOAT DEFAULT_AMBIENTLIGHT_COLORTEMP_MINIMUM                     = -FLT_MAX;
const FLOAT DEFAULT_AMBIENTLIGHT_COLORTEMP_ACCURACY                    = FLT_MAX;
const FLOAT DEFAULT_AMBIENTLIGHT_COLORTEMP_RESOLUTION                  = FLT_MAX;

const FLOAT DEFAULT_AMBIENTLIGHT_CHROMATICITY_SENSITIVITY              = 0.2F;
const FLOAT DEFAULT_AMBIENTLIGHT_CHROMATICITY_MAXIMUM                  = 1.0F;
const FLOAT DEFAULT_AMBIENTLIGHT_CHROMATICITY_MINIMUM                  = 0.0F;
const FLOAT DEFAULT_AMBIENTLIGHT_CHROMATICITY_ACCURACY                 = 0.0F;
const FLOAT DEFAULT_AMBIENTLIGHT_CHROMATICITY_RESOLUTION               = 0.0F;

const FLOAT MIN_AMBIENTLIGHT_SENSITIVITY                               = -FLT_MAX;
const FLOAT MIN_AMBIENTLIGHT_MAXIMUM                                   = -FLT_MAX;
const FLOAT MAX_AMBIENTLIGHT_MINIMUM                                   = FLT_MAX;
const FLOAT MIN_AMBIENTLIGHT_ACCURACY                                  = -FLT_MAX;
const FLOAT MIN_AMBIENTLIGHT_RESOLUTION                                = -FLT_MAX;

const ULONG DEFAULT_AMBIENTLIGHT_MIN_REPORT_INTERVAL                   = 50;
const ULONG DEFAULT_AMBIENTLIGHT_CURRENT_REPORT_INTERVAL               = 100;

// These are the default values for the Light Response Curve used by the ALS service
const DWORD DEFAULT_RESPONSE_CURVE_X_Y_PAIRS[AMBIENTLIGHT_MAX_RESPONSE_CURVE_XY_PAIRS*2] = {
    4, 35,
    40, 50,
    300, 75,
    800, 100,
    1275, 175,
    1800, 200,
    0, 0, 
    0, 0, 
    0, 0, 
    0, 0, 
    0, 0, 
    0, 0, 
    0, 0, 
    0, 0, 
    0, 0, 
    0, 0, 
    0, 0, 
    0, 0, 
    0, 0, 
    0, 0, 
};



