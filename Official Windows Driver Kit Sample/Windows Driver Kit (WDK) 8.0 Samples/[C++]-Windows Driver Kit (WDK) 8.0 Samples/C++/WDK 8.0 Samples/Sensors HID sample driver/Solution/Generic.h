/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Generic.h
//
// Description:
//      Defines the CGeneric container class

--*/


#pragma once

typedef struct _GENERIC_DEVICE_PROPERTIES
{
    BOOL   fSensorCategorySupported;
    BOOL   fSensorTypeSupported;

    // from input report
    BOOL   fSensorStateSupported;
    BOOL   fSensorStateSelectorSupported;
    BOOL   fEventTypeSupported;
    BOOL   fEventTypeSelectorSupported;

    // supported datafields
    BOOL   fGenericSupported;

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
    BOOL   fGenericSensitivitySupported;
    FLOAT  fltGenericSensitivity;
    
    BOOL   fGenericMaximumSupported;
    FLOAT  fltGenericMaximum;
    BOOL   fGenericMinimumSupported;
    FLOAT  fltGenericMinimum;

    BOOL   fGenericAccuracySupported;
    FLOAT  fltGenericAccuracy;

    BOOL   fGenericResolutionSupported;
    FLOAT  fltGenericResolution;

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

} GENERIC_DEVICE_PROPERTIES, *PGENERIC_DEVICE_PROPERTIES;


class CGeneric : public CSensor
{
public:
    CGeneric();
    ~CGeneric();
    
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

    HRESULT GetPropertyValuesForGenericObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT ProcessGenericAsyncRead( BYTE* buffer, ULONG length );
    HRESULT UpdateGenericPropertyValues( BYTE* pbSendReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported );

private:

    HRESULT InitializeGeneric(VOID);
    HRESULT AddGenericPropertyKeys();
    HRESULT AddGenericSettablePropertyKeys();
    HRESULT AddGenericDataFieldKeys();
    HRESULT SetGenericDefaultValues();

    HRESULT SetSettableGenericProperties();

    GENERIC_DEVICE_PROPERTIES m_DeviceProperties;
};

const unsigned short SENSOR_GENERIC_NAME[]                        = L"Generic";
const unsigned short SENSOR_GENERIC_DESCRIPTION[]                 = L"Generic Sensor";
const char SENSOR_GENERIC_TRACE_NAME[]                            = "Generic";

// Default Values
const FLOAT DEFAULT_GENERIC_SENSITIVITY                           = 0.05F;
const FLOAT DEFAULT_GENERIC_MAXIMUM                               = FLT_MAX;
const FLOAT DEFAULT_GENERIC_MINIMUM                               = -FLT_MAX;
const FLOAT DEFAULT_GENERIC_ACCURACY                              = FLT_MAX;
const FLOAT DEFAULT_GENERIC_RESOLUTION                            = FLT_MAX;

const FLOAT MIN_GENERIC_SENSITIVITY                               = -FLT_MAX;
const FLOAT MIN_GENERIC_MAXIMUM                                   = -FLT_MAX;
const FLOAT MAX_GENERIC_MINIMUM                                   = FLT_MAX;
const FLOAT MIN_GENERIC_ACCURACY                                  = -FLT_MAX;
const FLOAT MIN_GENERIC_RESOLUTION                                = -FLT_MAX;

const ULONG DEFAULT_GENERIC_MIN_REPORT_INTERVAL                   = 16;
const ULONG DEFAULT_GENERIC_CURRENT_REPORT_INTERVAL               = 100;


