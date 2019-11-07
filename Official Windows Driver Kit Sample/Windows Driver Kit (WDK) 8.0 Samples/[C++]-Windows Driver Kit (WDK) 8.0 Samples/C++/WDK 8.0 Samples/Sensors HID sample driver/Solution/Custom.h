/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Custom.h
//
// Description:
//      Defines the CCustom container class

--*/


#pragma once

typedef struct _CUSTOM_DEVICE_PROPERTIES
{
    // from input report
    BOOL   fSensorStateSupported;
    BOOL   fSensorStateSelectorSupported;
    BOOL   fEventTypeSupported;
    BOOL   fEventTypeSelectorSupported;

    // supported datafields
    BOOL   fCustomUsageSupported;
    BOOL   fCustomBooleanArraySupported;
    BOOL   fCustomValue1Supported;
    BOOL   fCustomValue2Supported;
    BOOL   fCustomValue3Supported;
    BOOL   fCustomValue4Supported;
    BOOL   fCustomValue5Supported;
    BOOL   fCustomValue6Supported;
    BOOL   fCustomValue7Supported;
    BOOL   fCustomValue8Supported;
    BOOL   fCustomValue9Supported;
    BOOL   fCustomValue10Supported;
    BOOL   fCustomValue11Supported;
    BOOL   fCustomValue12Supported;
    BOOL   fCustomValue13Supported;
    BOOL   fCustomValue14Supported;
    BOOL   fCustomValue15Supported;
    BOOL   fCustomValue16Supported;
    BOOL   fCustomValue17Supported;
    BOOL   fCustomValue18Supported;
    BOOL   fCustomValue19Supported;
    BOOL   fCustomValue20Supported;
    BOOL   fCustomValue21Supported;
    BOOL   fCustomValue22Supported;
    BOOL   fCustomValue23Supported;
    BOOL   fCustomValue24Supported;
    BOOL   fCustomValue25Supported;
    BOOL   fCustomValue26Supported;
    BOOL   fCustomValue27Supported;
    BOOL   fCustomValue28Supported;

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
    BOOL   fCustomSensitivitySupported;
    FLOAT  fltCustomSensitivity;
    BOOL   fCustomValue1SensitivitySupported;
    FLOAT  fltCustomValue1Sensitivity;
    BOOL   fCustomValue2SensitivitySupported;
    FLOAT  fltCustomValue2Sensitivity;
    BOOL   fCustomValue3SensitivitySupported;
    FLOAT  fltCustomValue3Sensitivity;
    BOOL   fCustomValue4SensitivitySupported;
    FLOAT  fltCustomValue4Sensitivity;
    BOOL   fCustomValue5SensitivitySupported;
    FLOAT  fltCustomValue5Sensitivity;
    BOOL   fCustomValue6SensitivitySupported;
    FLOAT  fltCustomValue6Sensitivity;
    BOOL   fCustomValue7SensitivitySupported;
    FLOAT  fltCustomValue7Sensitivity;
    BOOL   fCustomValue8SensitivitySupported;
    FLOAT  fltCustomValue8Sensitivity;
    BOOL   fCustomValue9SensitivitySupported;
    FLOAT  fltCustomValue9Sensitivity;
    BOOL   fCustomValue10SensitivitySupported;
    FLOAT  fltCustomValue10Sensitivity;
    BOOL   fCustomValue11SensitivitySupported;
    FLOAT  fltCustomValue11Sensitivity;
    BOOL   fCustomValue12SensitivitySupported;
    FLOAT  fltCustomValue12Sensitivity;
    BOOL   fCustomValue13SensitivitySupported;
    FLOAT  fltCustomValue13Sensitivity;
    BOOL   fCustomValue14SensitivitySupported;
    FLOAT  fltCustomValue14Sensitivity;
    BOOL   fCustomValue15SensitivitySupported;
    FLOAT  fltCustomValue15Sensitivity;
    BOOL   fCustomValue16SensitivitySupported;
    FLOAT  fltCustomValue16Sensitivity;
    BOOL   fCustomValue17SensitivitySupported;
    FLOAT  fltCustomValue17Sensitivity;
    BOOL   fCustomValue18SensitivitySupported;
    FLOAT  fltCustomValue18Sensitivity;
    BOOL   fCustomValue19SensitivitySupported;
    FLOAT  fltCustomValue19Sensitivity;
    BOOL   fCustomValue20SensitivitySupported;
    FLOAT  fltCustomValue20Sensitivity;
    BOOL   fCustomValue21SensitivitySupported;
    FLOAT  fltCustomValue21Sensitivity;
    BOOL   fCustomValue22SensitivitySupported;
    FLOAT  fltCustomValue22Sensitivity;
    BOOL   fCustomValue23SensitivitySupported;
    FLOAT  fltCustomValue23Sensitivity;
    BOOL   fCustomValue24SensitivitySupported;
    FLOAT  fltCustomValue24Sensitivity;
    BOOL   fCustomValue25SensitivitySupported;
    FLOAT  fltCustomValue25Sensitivity;
    BOOL   fCustomValue26SensitivitySupported;
    FLOAT  fltCustomValue26Sensitivity;
    BOOL   fCustomValue27SensitivitySupported;
    FLOAT  fltCustomValue27Sensitivity;
    BOOL   fCustomValue28SensitivitySupported;
    FLOAT  fltCustomValue28Sensitivity;
    
    BOOL   fCustomMaximumSupported;
    FLOAT  fltCustomMaximum;
    BOOL   fCustomValue1MaximumSupported;
    FLOAT  fltCustomValue1Maximum;
    BOOL   fCustomValue2MaximumSupported;
    FLOAT  fltCustomValue2Maximum;
    BOOL   fCustomValue3MaximumSupported;
    FLOAT  fltCustomValue3Maximum;
    BOOL   fCustomValue4MaximumSupported;
    FLOAT  fltCustomValue4Maximum;
    BOOL   fCustomValue5MaximumSupported;
    FLOAT  fltCustomValue5Maximum;
    BOOL   fCustomValue6MaximumSupported;
    FLOAT  fltCustomValue6Maximum;
    BOOL   fCustomValue7MaximumSupported;
    FLOAT  fltCustomValue7Maximum;
    BOOL   fCustomValue8MaximumSupported;
    FLOAT  fltCustomValue8Maximum;
    BOOL   fCustomValue9MaximumSupported;
    FLOAT  fltCustomValue9Maximum;
    BOOL   fCustomValue10MaximumSupported;
    FLOAT  fltCustomValue10Maximum;
    BOOL   fCustomValue11MaximumSupported;
    FLOAT  fltCustomValue11Maximum;
    BOOL   fCustomValue12MaximumSupported;
    FLOAT  fltCustomValue12Maximum;
    BOOL   fCustomValue13MaximumSupported;
    FLOAT  fltCustomValue13Maximum;
    BOOL   fCustomValue14MaximumSupported;
    FLOAT  fltCustomValue14Maximum;
    BOOL   fCustomValue15MaximumSupported;
    FLOAT  fltCustomValue15Maximum;
    BOOL   fCustomValue16MaximumSupported;
    FLOAT  fltCustomValue16Maximum;
    BOOL   fCustomValue17MaximumSupported;
    FLOAT  fltCustomValue17Maximum;
    BOOL   fCustomValue18MaximumSupported;
    FLOAT  fltCustomValue18Maximum;
    BOOL   fCustomValue19MaximumSupported;
    FLOAT  fltCustomValue19Maximum;
    BOOL   fCustomValue20MaximumSupported;
    FLOAT  fltCustomValue20Maximum;
    BOOL   fCustomValue21MaximumSupported;
    FLOAT  fltCustomValue21Maximum;
    BOOL   fCustomValue22MaximumSupported;
    FLOAT  fltCustomValue22Maximum;
    BOOL   fCustomValue23MaximumSupported;
    FLOAT  fltCustomValue23Maximum;
    BOOL   fCustomValue24MaximumSupported;
    FLOAT  fltCustomValue24Maximum;
    BOOL   fCustomValue25MaximumSupported;
    FLOAT  fltCustomValue25Maximum;
    BOOL   fCustomValue26MaximumSupported;
    FLOAT  fltCustomValue26Maximum;
    BOOL   fCustomValue27MaximumSupported;
    FLOAT  fltCustomValue27Maximum;
    BOOL   fCustomValue28MaximumSupported;
    FLOAT  fltCustomValue28Maximum;

    BOOL   fCustomMinimumSupported;
    FLOAT  fltCustomMinimum;
    BOOL   fCustomValue1MinimumSupported;
    FLOAT  fltCustomValue1Minimum;
    BOOL   fCustomValue2MinimumSupported;
    FLOAT  fltCustomValue2Minimum;
    BOOL   fCustomValue3MinimumSupported;
    FLOAT  fltCustomValue3Minimum;
    BOOL   fCustomValue4MinimumSupported;
    FLOAT  fltCustomValue4Minimum;
    BOOL   fCustomValue5MinimumSupported;
    FLOAT  fltCustomValue5Minimum;
    BOOL   fCustomValue6MinimumSupported;
    FLOAT  fltCustomValue6Minimum;
    BOOL   fCustomValue7MinimumSupported;
    FLOAT  fltCustomValue7Minimum;
    BOOL   fCustomValue8MinimumSupported;
    FLOAT  fltCustomValue8Minimum;
    BOOL   fCustomValue9MinimumSupported;
    FLOAT  fltCustomValue9Minimum;
    BOOL   fCustomValue10MinimumSupported;
    FLOAT  fltCustomValue10Minimum;
    BOOL   fCustomValue11MinimumSupported;
    FLOAT  fltCustomValue11Minimum;
    BOOL   fCustomValue12MinimumSupported;
    FLOAT  fltCustomValue12Minimum;
    BOOL   fCustomValue13MinimumSupported;
    FLOAT  fltCustomValue13Minimum;
    BOOL   fCustomValue14MinimumSupported;
    FLOAT  fltCustomValue14Minimum;
    BOOL   fCustomValue15MinimumSupported;
    FLOAT  fltCustomValue15Minimum;
    BOOL   fCustomValue16MinimumSupported;
    FLOAT  fltCustomValue16Minimum;
    BOOL   fCustomValue17MinimumSupported;
    FLOAT  fltCustomValue17Minimum;
    BOOL   fCustomValue18MinimumSupported;
    FLOAT  fltCustomValue18Minimum;
    BOOL   fCustomValue19MinimumSupported;
    FLOAT  fltCustomValue19Minimum;
    BOOL   fCustomValue20MinimumSupported;
    FLOAT  fltCustomValue20Minimum;
    BOOL   fCustomValue21MinimumSupported;
    FLOAT  fltCustomValue21Minimum;
    BOOL   fCustomValue22MinimumSupported;
    FLOAT  fltCustomValue22Minimum;
    BOOL   fCustomValue23MinimumSupported;
    FLOAT  fltCustomValue23Minimum;
    BOOL   fCustomValue24MinimumSupported;
    FLOAT  fltCustomValue24Minimum;
    BOOL   fCustomValue25MinimumSupported;
    FLOAT  fltCustomValue25Minimum;
    BOOL   fCustomValue26MinimumSupported;
    FLOAT  fltCustomValue26Minimum;
    BOOL   fCustomValue27MinimumSupported;
    FLOAT  fltCustomValue27Minimum;
    BOOL   fCustomValue28MinimumSupported;
    FLOAT  fltCustomValue28Minimum;

    BOOL   fCustomAccuracySupported;
    FLOAT  fltCustomAccuracy;
    BOOL   fCustomValue1AccuracySupported;
    FLOAT  fltCustomValue1Accuracy;
    BOOL   fCustomValue2AccuracySupported;
    FLOAT  fltCustomValue2Accuracy;
    BOOL   fCustomValue3AccuracySupported;
    FLOAT  fltCustomValue3Accuracy;
    BOOL   fCustomValue4AccuracySupported;
    FLOAT  fltCustomValue4Accuracy;
    BOOL   fCustomValue5AccuracySupported;
    FLOAT  fltCustomValue5Accuracy;
    BOOL   fCustomValue6AccuracySupported;
    FLOAT  fltCustomValue6Accuracy;
    BOOL   fCustomValue7AccuracySupported;
    FLOAT  fltCustomValue7Accuracy;
    BOOL   fCustomValue8AccuracySupported;
    FLOAT  fltCustomValue8Accuracy;
    BOOL   fCustomValue9AccuracySupported;
    FLOAT  fltCustomValue9Accuracy;
    BOOL   fCustomValue10AccuracySupported;
    FLOAT  fltCustomValue10Accuracy;
    BOOL   fCustomValue11AccuracySupported;
    FLOAT  fltCustomValue11Accuracy;
    BOOL   fCustomValue12AccuracySupported;
    FLOAT  fltCustomValue12Accuracy;
    BOOL   fCustomValue13AccuracySupported;
    FLOAT  fltCustomValue13Accuracy;
    BOOL   fCustomValue14AccuracySupported;
    FLOAT  fltCustomValue14Accuracy;
    BOOL   fCustomValue15AccuracySupported;
    FLOAT  fltCustomValue15Accuracy;
    BOOL   fCustomValue16AccuracySupported;
    FLOAT  fltCustomValue16Accuracy;
    BOOL   fCustomValue17AccuracySupported;
    FLOAT  fltCustomValue17Accuracy;
    BOOL   fCustomValue18AccuracySupported;
    FLOAT  fltCustomValue18Accuracy;
    BOOL   fCustomValue19AccuracySupported;
    FLOAT  fltCustomValue19Accuracy;
    BOOL   fCustomValue20AccuracySupported;
    FLOAT  fltCustomValue20Accuracy;
    BOOL   fCustomValue21AccuracySupported;
    FLOAT  fltCustomValue21Accuracy;
    BOOL   fCustomValue22AccuracySupported;
    FLOAT  fltCustomValue22Accuracy;
    BOOL   fCustomValue23AccuracySupported;
    FLOAT  fltCustomValue23Accuracy;
    BOOL   fCustomValue24AccuracySupported;
    FLOAT  fltCustomValue24Accuracy;
    BOOL   fCustomValue25AccuracySupported;
    FLOAT  fltCustomValue25Accuracy;
    BOOL   fCustomValue26AccuracySupported;
    FLOAT  fltCustomValue26Accuracy;
    BOOL   fCustomValue27AccuracySupported;
    FLOAT  fltCustomValue27Accuracy;
    BOOL   fCustomValue28AccuracySupported;
    FLOAT  fltCustomValue28Accuracy;

    BOOL   fCustomResolutionSupported;
    FLOAT  fltCustomResolution;
    BOOL   fCustomValue1ResolutionSupported;
    FLOAT  fltCustomValue1Resolution;
    BOOL   fCustomValue2ResolutionSupported;
    FLOAT  fltCustomValue2Resolution;
    BOOL   fCustomValue3ResolutionSupported;
    FLOAT  fltCustomValue3Resolution;
    BOOL   fCustomValue4ResolutionSupported;
    FLOAT  fltCustomValue4Resolution;
    BOOL   fCustomValue5ResolutionSupported;
    FLOAT  fltCustomValue5Resolution;
    BOOL   fCustomValue6ResolutionSupported;
    FLOAT  fltCustomValue6Resolution;
    BOOL   fCustomValue7ResolutionSupported;
    FLOAT  fltCustomValue7Resolution;
    BOOL   fCustomValue8ResolutionSupported;
    FLOAT  fltCustomValue8Resolution;
    BOOL   fCustomValue9ResolutionSupported;
    FLOAT  fltCustomValue9Resolution;
    BOOL   fCustomValue10ResolutionSupported;
    FLOAT  fltCustomValue10Resolution;
    BOOL   fCustomValue11ResolutionSupported;
    FLOAT  fltCustomValue11Resolution;
    BOOL   fCustomValue12ResolutionSupported;
    FLOAT  fltCustomValue12Resolution;
    BOOL   fCustomValue13ResolutionSupported;
    FLOAT  fltCustomValue13Resolution;
    BOOL   fCustomValue14ResolutionSupported;
    FLOAT  fltCustomValue14Resolution;
    BOOL   fCustomValue15ResolutionSupported;
    FLOAT  fltCustomValue15Resolution;
    BOOL   fCustomValue16ResolutionSupported;
    FLOAT  fltCustomValue16Resolution;
    BOOL   fCustomValue17ResolutionSupported;
    FLOAT  fltCustomValue17Resolution;
    BOOL   fCustomValue18ResolutionSupported;
    FLOAT  fltCustomValue18Resolution;
    BOOL   fCustomValue19ResolutionSupported;
    FLOAT  fltCustomValue19Resolution;
    BOOL   fCustomValue20ResolutionSupported;
    FLOAT  fltCustomValue20Resolution;
    BOOL   fCustomValue21ResolutionSupported;
    FLOAT  fltCustomValue21Resolution;
    BOOL   fCustomValue22ResolutionSupported;
    FLOAT  fltCustomValue22Resolution;
    BOOL   fCustomValue23ResolutionSupported;
    FLOAT  fltCustomValue23Resolution;
    BOOL   fCustomValue24ResolutionSupported;
    FLOAT  fltCustomValue24Resolution;
    BOOL   fCustomValue25ResolutionSupported;
    FLOAT  fltCustomValue25Resolution;
    BOOL   fCustomValue26ResolutionSupported;
    FLOAT  fltCustomValue26Resolution;
    BOOL   fCustomValue27ResolutionSupported;
    FLOAT  fltCustomValue27Resolution;
    BOOL   fCustomValue28ResolutionSupported;
    FLOAT  fltCustomValue28Resolution;

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

} CUSTOM_DEVICE_PROPERTIES, *PCUSTOM_DEVICE_PROPERTIES;

class CCustom : public CSensor
{
public:
    CCustom();
    ~CCustom();
    
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

    HRESULT GetPropertyValuesForCustomObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT ProcessCustomAsyncRead( BYTE* buffer, ULONG length );
    HRESULT UpdateCustomPropertyValues( BYTE* pbSendReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported );

private:

    HRESULT InitializeCustom(VOID);
    HRESULT AddCustomPropertyKeys();
    HRESULT AddCustomSettablePropertyKeys();
    HRESULT AddCustomDataFieldKeys();
    HRESULT SetCustomDefaultValues();

    HRESULT SetSettableCustomProperties();

    CUSTOM_DEVICE_PROPERTIES m_DeviceProperties;
};


const unsigned short SENSOR_CUSTOM_NAME[]                        = L"Custom";
const unsigned short SENSOR_CUSTOM_DESCRIPTION[]                 = L"Custom Sensor";
const char SENSOR_CUSTOM_TRACE_NAME[]                            = "Custom";

// Default Values
const FLOAT DEFAULT_CUSTOM_SENSITIVITY                           = 0.05F;
const FLOAT DEFAULT_CUSTOM_MAXIMUM                               = FLT_MAX;
const FLOAT DEFAULT_CUSTOM_MINIMUM                               = -FLT_MAX;
const FLOAT DEFAULT_CUSTOM_ACCURACY                              = FLT_MAX;
const FLOAT DEFAULT_CUSTOM_RESOLUTION                            = FLT_MAX;

const FLOAT MIN_CUSTOM_SENSITIVITY                               = -FLT_MAX;
const FLOAT MIN_CUSTOM_MAXIMUM                                   = -FLT_MAX;
const FLOAT MAX_CUSTOM_MINIMUM                                   = FLT_MAX;
const FLOAT MIN_CUSTOM_ACCURACY                                  = -FLT_MAX;
const FLOAT MIN_CUSTOM_RESOLUTION                                = -FLT_MAX;

const ULONG DEFAULT_CUSTOM_MIN_REPORT_INTERVAL                   = 50;
const ULONG DEFAULT_CUSTOM_CURRENT_REPORT_INTERVAL               = 100;



