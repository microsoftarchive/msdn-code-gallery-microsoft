/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Orientation.h
//
// Description:
//      Defines the COrientation container class

--*/


#pragma once

class CSensorManager; //forward declaration


typedef struct _ORIENTATION_DEVICE_PROPERTIES
{
    // from input report
    BOOL   fSensorStateSupported;
    BOOL   fSensorStateSelectorSupported;
    BOOL   fEventTypeSupported;
    BOOL   fEventTypeSelectorSupported;

    // supported datafields
    BOOL   fOrientationRotationSupported;
    BOOL   fOrientationQuaternionSupported;

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
    BOOL   fOrientationSensitivitySupported;
    FLOAT  fltOrientationSensitivity;
    BOOL   fOrientationRotationSensitivitySupported;
    FLOAT  fltOrientationRotationSensitivity;
    BOOL   fOrientationQuaternionSensitivitySupported;
    FLOAT  fltOrientationQuaternionSensitivity;
    
    BOOL   fOrientationMaximumSupported;
    FLOAT  fltOrientationMaximum;
    BOOL   fOrientationRotationMaximumSupported;
    FLOAT  fltOrientationRotationMaximum;
    BOOL   fOrientationQuaternionMaximumSupported;
    FLOAT  fltOrientationQuaternionMaximum;

    BOOL   fOrientationMinimumSupported;
    FLOAT  fltOrientationMinimum;
    BOOL   fOrientationRotationMinimumSupported;
    FLOAT  fltOrientationRotationMinimum;
    BOOL   fOrientationQuaternionMinimumSupported;
    FLOAT  fltOrientationQuaternionMinimum;

    BOOL   fOrientationAccuracySupported;
    FLOAT  fltOrientationAccuracy;
    BOOL   fOrientationRotationAccuracySupported;
    FLOAT  fltOrientationRotationAccuracy;
    BOOL   fOrientationQuaternionAccuracySupported;
    FLOAT  fltOrientationQuaternionAccuracy;

    BOOL   fOrientationResolutionSupported;
    FLOAT  fltOrientationResolution;
    BOOL   fOrientationRotationResolutionSupported;
    FLOAT  fltOrientationRotationResolution;
    BOOL   fOrientationQuaternionResolutionSupported;
    FLOAT  fltOrientationQuaternionResolution;

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

} ORIENTATION_DEVICE_PROPERTIES, *PORIENTATION_DEVICE_PROPERTIES;


class COrientation : public CSensor
{
public:
    COrientation();
    ~COrientation();
    
    HRESULT Initialize( _In_ SensorType sensType,
                        _In_ ULONG sensUsage,
                        _In_ USHORT sensLinkCollection,
                        _In_ DWORD sensNum, 
                        _In_ LPWSTR pwszManufacturer,
                        _In_ LPWSTR pwszProduct,
                        _In_ LPWSTR pwszSerialNumber,
                        _In_ LPWSTR pwszSensorID,
                        _In_ CSensorManager* pSensorManager);

    HRESULT GetPropertyValuesForOrientationObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT ProcessOrientationAsyncRead( BYTE* buffer, ULONG length );
    HRESULT UpdateOrientationPropertyValues( BYTE* pbSendReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported );

private:

    HRESULT InitializeOrientation(VOID);
    HRESULT AddOrientationPropertyKeys();
    HRESULT AddOrientationSettablePropertyKeys();
    HRESULT AddOrientationDataFieldKeys();
    HRESULT SetOrientationDefaultValues();

    HRESULT SetSettableOrientationProperties();

    ORIENTATION_DEVICE_PROPERTIES m_DeviceProperties;
};


const unsigned short SENSOR_ORIENTATION_NAME[]                        = L"Orientation";
const unsigned short SENSOR_ORIENTATION_DESCRIPTION[]                 = L"Orientation Sensor";
const char SENSOR_ORIENTATION_TRACE_NAME[]                            = "Orientation";

const unsigned short MAX_ROTATION_MATRIX_DIMENSION                    = 3;
const unsigned short MAX_ORIENTATION_ARRAY_ELEMENTS                   = MAX_ROTATION_MATRIX_DIMENSION*MAX_ROTATION_MATRIX_DIMENSION;
const unsigned short MAX_QUATERNION_ARRAY_ELEMENTS                    = 4;

// Default Values
const FLOAT DEFAULT_ORIENTATION_SENSITIVITY                            = 0.2F;
const FLOAT DEFAULT_ORIENTATION_MAXIMUM                                = FLT_MAX;
const FLOAT DEFAULT_ORIENTATION_MINIMUM                                = -FLT_MAX;
const FLOAT DEFAULT_ORIENTATION_ACCURACY                               = FLT_MAX;
const FLOAT DEFAULT_ORIENTATION_RESOLUTION                             = FLT_MAX;

const FLOAT MIN_ORIENTATION_SENSITIVITY                                = -FLT_MAX;
const FLOAT MIN_ORIENTATION_MAXIMUM                                    = -FLT_MAX;
const FLOAT MAX_ORIENTATION_MINIMUM                                    = FLT_MAX;
const FLOAT MIN_ORIENTATION_ACCURACY                                   = -FLT_MAX;
const FLOAT MIN_ORIENTATION_RESOLUTION                                 = -FLT_MAX;

const ULONG DEFAULT_ORIENTATION_MIN_REPORT_INTERVAL                    = 16;
const ULONG DEFAULT_ORIENTATION_CURRENT_REPORT_INTERVAL                = 50;



