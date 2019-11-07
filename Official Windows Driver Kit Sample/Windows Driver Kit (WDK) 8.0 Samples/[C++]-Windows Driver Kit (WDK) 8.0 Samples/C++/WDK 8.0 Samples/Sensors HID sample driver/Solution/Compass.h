/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Compass.h
//
// Description:
//      Defines the CCompass container class

--*/


#pragma once

typedef struct _COMPASS_DEVICE_PROPERTIES
{
    // from input report
    BOOL   fSensorStateSupported;
    BOOL   fSensorStateSelectorSupported;
    BOOL   fEventTypeSupported;
    BOOL   fEventTypeSelectorSupported;

    // supported datafields
    BOOL   fCompassXAxisSupported;
    BOOL   fCompassYAxisSupported;
    BOOL   fCompassZAxisSupported;
    BOOL   fCompassCompensatedMagneticNorthSupported;
    BOOL   fCompassCompensatedTrueNorthSupported;
    BOOL   fCompassMagneticNorthSupported;
    BOOL   fCompassTrueNorthSupported;

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
    BOOL   fCompassMagneticSensitivitySupported;
    FLOAT  fltCompassMagneticSensitivity;
    BOOL   fCompassXSensitivitySupported;
    FLOAT  fltCompassXSensitivity;
    BOOL   fCompassYSensitivitySupported;
    FLOAT  fltCompassYSensitivity;
    BOOL   fCompassZSensitivitySupported;
    FLOAT  fltCompassZSensitivity;

    BOOL   fCompassHeadingSensitivitySupported;
    FLOAT  fltCompassHeadingSensitivity;
    BOOL   fCompassCompensatedMagneticNorthSensitivitySupported;
    FLOAT  fltCompassCompensatedMagneticNorthSensitivity;
    BOOL   fCompassCompensatedTrueNorthSensitivitySupported;
    FLOAT  fltCompassCompensatedTrueNorthSensitivity;
    BOOL   fCompassMagneticNorthSensitivitySupported;
    FLOAT  fltCompassMagneticNorthSensitivity;
    BOOL   fCompassTrueNorthSensitivitySupported;
    FLOAT  fltCompassTrueNorthSensitivity;
    
    BOOL   fCompassMagneticMaximumSupported;
    FLOAT  fltCompassMagneticMaximum;
    BOOL   fCompassXMaximumSupported;
    FLOAT  fltCompassXMaximum;
    BOOL   fCompassYMaximumSupported;
    FLOAT  fltCompassYMaximum;
    BOOL   fCompassZMaximumSupported;
    FLOAT  fltCompassZMaximum;

    BOOL   fCompassHeadingMaximumSupported;
    FLOAT  fltCompassHeadingMaximum;
    BOOL   fCompassCompensatedMagneticNorthMaximumSupported;
    FLOAT  fltCompassCompensatedMagneticNorthMaximum;
    BOOL   fCompassCompensatedTrueNorthMaximumSupported;
    FLOAT  fltCompassCompensatedTrueNorthMaximum;
    BOOL   fCompassMagneticNorthMaximumSupported;
    FLOAT  fltCompassMagneticNorthMaximum;
    BOOL   fCompassTrueNorthMaximumSupported;
    FLOAT  fltCompassTrueNorthMaximum;

    BOOL   fCompassMagneticMinimumSupported;
    FLOAT  fltCompassMagneticMinimum;
    BOOL   fCompassXMinimumSupported;
    FLOAT  fltCompassXMinimum;
    BOOL   fCompassYMinimumSupported;
    FLOAT  fltCompassYMinimum;
    BOOL   fCompassZMinimumSupported;
    FLOAT  fltCompassZMinimum;

    BOOL   fCompassHeadingMinimumSupported;
    FLOAT  fltCompassHeadingMinimum;
    BOOL   fCompassCompensatedMagneticNorthMinimumSupported;
    FLOAT  fltCompassCompensatedMagneticNorthMinimum;
    BOOL   fCompassCompensatedTrueNorthMinimumSupported;
    FLOAT  fltCompassCompensatedTrueNorthMinimum;
    BOOL   fCompassMagneticNorthMinimumSupported;
    FLOAT  fltCompassMagneticNorthMinimum;
    BOOL   fCompassTrueNorthMinimumSupported;
    FLOAT  fltCompassTrueNorthMinimum;

    BOOL   fCompassMagneticAccuracySupported;
    FLOAT  fltCompassMagneticAccuracy;
    BOOL   fCompassXAccuracySupported;
    FLOAT  fltCompassXAccuracy;
    BOOL   fCompassYAccuracySupported;
    FLOAT  fltCompassYAccuracy;
    BOOL   fCompassZAccuracySupported;
    FLOAT  fltCompassZAccuracy;

    BOOL   fCompassHeadingAccuracySupported;
    FLOAT  fltCompassHeadingAccuracy;
    BOOL   fCompassCompensatedMagneticNorthAccuracySupported;
    FLOAT  fltCompassCompensatedMagneticNorthAccuracy;
    BOOL   fCompassCompensatedTrueNorthAccuracySupported;
    FLOAT  fltCompassCompensatedTrueNorthAccuracy;
    BOOL   fCompassMagneticNorthAccuracySupported;
    FLOAT  fltCompassMagneticNorthAccuracy;
    BOOL   fCompassTrueNorthAccuracySupported;
    FLOAT  fltCompassTrueNorthAccuracy;

    BOOL   fCompassMagneticResolutionSupported;
    FLOAT  fltCompassMagneticResolution;
    BOOL   fCompassXResolutionSupported;
    FLOAT  fltCompassXResolution;
    BOOL   fCompassYResolutionSupported;
    FLOAT  fltCompassYResolution;
    BOOL   fCompassZResolutionSupported;
    FLOAT  fltCompassZResolution;

    BOOL   fCompassHeadingResolutionSupported;
    FLOAT  fltCompassHeadingResolution;
    BOOL   fCompassCompensatedMagneticNorthResolutionSupported;
    FLOAT  fltCompassCompensatedMagneticNorthResolution;
    BOOL   fCompassCompensatedTrueNorthResolutionSupported;
    FLOAT  fltCompassCompensatedTrueNorthResolution;
    BOOL   fCompassMagneticNorthResolutionSupported;
    FLOAT  fltCompassMagneticNorthResolution;
    BOOL   fCompassTrueNorthResolutionSupported;
    FLOAT  fltCompassTrueNorthResolution;

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

} COMPASS_DEVICE_PROPERTIES, *PCOMPASS_DEVICE_PROPERTIES;


class CCompass : public CSensor
{
public:
    CCompass();
    ~CCompass();
    
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

    HRESULT GetPropertyValuesForCompassObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT ProcessCompassAsyncRead( BYTE* buffer, ULONG length );
    HRESULT UpdateCompassPropertyValues( BYTE* pbSendReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported );

private:

    HRESULT InitializeCompass(VOID);
    HRESULT AddCompassPropertyKeys();
    HRESULT AddCompassSettablePropertyKeys();
    HRESULT AddCompassDataFieldKeys();
    HRESULT SetCompassDefaultValues();

    HRESULT SetSettableCompassProperties();

    COMPASS_DEVICE_PROPERTIES m_DeviceProperties;
};


const unsigned short SENSOR_COMPASS_NAME[]                        = L"Compass";
const unsigned short SENSOR_COMPASS_DESCRIPTION[]                 = L"Compass Sensor";
const char SENSOR_COMPASS_TRACE_NAME[]                            = "Compass";

// Default Values
const FLOAT DEFAULT_COMPASS_SENSITIVITY                            = 0.2F;
const FLOAT DEFAULT_COMPASS_MAXIMUM                                = FLT_MAX;
const FLOAT DEFAULT_COMPASS_MINIMUM                                = -FLT_MAX;
const FLOAT DEFAULT_COMPASS_ACCURACY                               = FLT_MAX;
const FLOAT DEFAULT_COMPASS_RESOLUTION                             = FLT_MAX;

const FLOAT MIN_COMPASS_SENSITIVITY                                = -FLT_MAX;
const FLOAT MIN_COMPASS_MAXIMUM                                    = -FLT_MAX;
const FLOAT MAX_COMPASS_MINIMUM                                    = FLT_MAX;
const FLOAT MIN_COMPASS_ACCURACY                                   = -FLT_MAX;
const FLOAT MIN_COMPASS_RESOLUTION                                 = -FLT_MAX;

const ULONG DEFAULT_COMPASS_MIN_REPORT_INTERVAL                    = 50;
const ULONG DEFAULT_COMPASS_CURRENT_REPORT_INTERVAL                = 100;




