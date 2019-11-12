/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// Module:
//      Geolocation.h
//
// Description:
//      Defines the CGeolocation container class

--*/


#pragma once

class CSensorManager; //forward declaration


class CGeolocation : public CSensor
{
public:
    CGeolocation();
    ~CGeolocation();
    
    HRESULT Initialize( _In_ SensorType sensType, 
                        _In_ DWORD sensNum, 
                        _In_ LPWSTR pwszManufacturer,
                        _In_ LPWSTR pwszProduct,
                        _In_ LPWSTR pwszSerialNumber,
                        _In_ LPWSTR sensorID,
                        _In_ IWDFDevice* pWdfDevice,
                        _In_ CSensorManager* pSensorManager);

    HRESULT GetPropertyValuesForGeolocationObject(LPCWSTR           wszObjectID,
                                       IPortableDeviceKeyCollection*  pKeys,
                                       IPortableDeviceValues*         pValues);

    HRESULT UpdateGeolocationPropertyValues();
    HRESULT UpdateGeolocationDataFieldValues();

//TODO define this == 1 if the sensor must have a known unique ID for all instances on any system
#define USE_GEOLOCATION_SPECIFIC_UNIQUE_ID      1

#if USE_GEOLOCATION_SPECIFIC_UNIQUE_ID 
    virtual HRESULT SetUniqueID(_In_ IWDFDevice* pWdfDevice);
#endif

    virtual HRESULT SetState(_In_ SensorState newState, _Out_ bool* pfStateChanged);

private:

    HRESULT InitializeGeolocation(_In_ IWDFDevice* pWdfDevice);
    HRESULT AddGeolocationPropertyKeys();
    HRESULT AddGeolocationSettablePropertyKeys();
    HRESULT AddGeolocationDataFieldKeys();
    HRESULT SetGeolocationDefaultValues(_In_ IWDFDevice* pWdfDevice);

    HRESULT SetSettableGeolocationProperties();

};


const unsigned short SENSOR_GEOLOCATION_NAME[]                  = L"Geolocation";
const unsigned short SENSOR_GEOLOCATION_DESCRIPTION[]           = L"Geolocation Sensor";
const char SENSOR_GEOLOCATION_TRACE_NAME[]                      = "Geolocation";

// Default Values
const FLOAT DEFAULT_GEOLOCATION_CHANGE_SENSITIVITY              = 5.0F;
const FLOAT DEFAULT_GEOLOCATION_MAXIMUM                         = FLT_MAX;
const FLOAT DEFAULT_GEOLOCATION_MINIMUM                         = -FLT_MAX;
const FLOAT DEFAULT_GEOLOCATION_ACCURACY                        = FLT_MAX;
const FLOAT DEFAULT_GEOLOCATION_RESOLUTION                      = FLT_MAX;

const FLOAT MIN_GEOLOCATION_SENSITIVITY                         = -FLT_MAX;
const FLOAT MIN_GEOLOCATION_MAXIMUM                             = -FLT_MAX;
const FLOAT MAX_GEOLOCATION_MINIMUM                             = FLT_MAX;
const FLOAT MIN_GEOLOCATION_ACCURACY                            = -FLT_MAX;
const FLOAT MIN_GEOLOCATION_RESOLUTION                          = -FLT_MAX;

const ULONG GEOLOCATION_MIN_REPORT_INTERVAL                     = 250;
const ULONG DEFAULT_GEOLOCATION_CURRENT_REPORT_INTERVAL         = 250;

const USHORT USAGE_GEOLOCATION_SENSOR                           = (0x53);
