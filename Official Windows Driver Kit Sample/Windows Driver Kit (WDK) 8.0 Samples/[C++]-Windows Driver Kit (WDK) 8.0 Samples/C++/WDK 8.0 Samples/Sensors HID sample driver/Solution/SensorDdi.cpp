//
//    Copyright (C) Microsoft.  All rights reserved.
//
/*++
 
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

Module Name:

    SensorDDI.cpp

Abstract:

      This module implements the ISensorDriver interface which is used
      by the Sensor Class Extension.
--*/

#include "internal.h"
#include "SensorManager.h"
#include "ReadWriteRequest.h"
#include "SensorDDI.h"

#include "devpkey.h"
#include <strsafe.h>


#include "Sensor.h"
#include "Accelerometer.h"
#include "AmbientLight.h"
#include "Presence.h"
#include "Compass.h"
#include "Inclinometer.h"
#include "Gyrometer.h"
#include "AtmosPressure.h"
#include "Distance.h"
#include "Humidity.h"
#include "Potentiometer.h"
#include "Switches.h"
#include "Thermometer.h"
#include "Voltage.h"
#include "Current.h"
#include "Power.h"
#include "Frequency.h"
#include "Orientation.h"
#include "Custom.h"
#include "Generic.h"
#include "Unsupported.h"

#include "SensorDDI.tmh"


const PROPERTYKEY g_SupportedCommonProperties[] =
{
    WPD_OBJECT_ID,
    WPD_OBJECT_PERSISTENT_UNIQUE_ID,
    WPD_OBJECT_PARENT_ID,
    WPD_OBJECT_NAME,
    WPD_OBJECT_FORMAT,
    WPD_OBJECT_CONTENT_TYPE,
    WPD_OBJECT_CAN_DELETE
};

const PROPERTYKEY g_SupportedDeviceProperties[] =
{
    WPD_DEVICE_FIRMWARE_VERSION,
    WPD_DEVICE_POWER_LEVEL,
    WPD_DEVICE_POWER_SOURCE,
    WPD_DEVICE_PROTOCOL,
    WPD_DEVICE_MODEL,
    WPD_DEVICE_SERIAL_NUMBER,
    WPD_DEVICE_SUPPORTS_NON_CONSUMABLE,
    WPD_DEVICE_MANUFACTURER,
    WPD_DEVICE_FRIENDLY_NAME,
    WPD_DEVICE_TYPE,
    WPD_FUNCTIONAL_OBJECT_CATEGORY
};

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::CSensorDDI()
//
// Constructor.
//
/////////////////////////////////////////////////////////////////////////
CSensorDDI::CSensorDDI()
{
    m_pSensorManager = NULL;
    m_SensorEvents_Bitmap = 0;

    // Initialize HID Read/Write requests to NULL
    for( DWORD i = 0; i < PENDING_READ_COUNT; i++ )
    {
        m_pHidReadRequest[i] = NULL;
    }

    m_pSensorManager = NULL;
    m_pHidWriteRequest = NULL;
    m_pHidIoctlRequest = NULL;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::~CSensorDDI()
//
// Destructor
//
/////////////////////////////////////////////////////////////////////////
CSensorDDI::~CSensorDDI()
{
    for( DWORD i = 0; i < PENDING_READ_COUNT; i++ )
    {
        SAFE_RELEASE(m_pHidReadRequest[i]);
    }

    SAFE_RELEASE(m_pHidWriteRequest);
    SAFE_RELEASE(m_pHidIoctlRequest);

    if (nullptr != m_pSensorManager->m_pPreparsedData) {
        delete[] (BYTE*)m_pSensorManager->m_pPreparsedData;
    }

    if (nullptr != m_pSensorManager->m_pLinkCollectionNodes) {
        delete[] (BYTE*)m_pSensorManager->m_pLinkCollectionNodes;
    }
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::InitSensorDevice
//
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::InitSensorDevice(_In_ IWDFDevice* pWdfDevice)
{
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    HRESULT hr = (NULL != pWdfDevice) ? S_OK : E_UNEXPECTED;

    m_fReleasingDevice = FALSE;

    // Create the HID Read objects
    // Initialize the request with the IWDFDevice object
    // Send the pending read request that will get completed
    if(SUCCEEDED(hr))
    {
        CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

        for( DWORD i = 0; SUCCEEDED(hr) && i < PENDING_READ_COUNT; i++ )
        {
            hr = CComObject<CReadWriteRequest>::CreateInstance(&m_pHidReadRequest[i]);

            if (SUCCEEDED(hr))
            {
                if (nullptr != m_pHidReadRequest[i])
                {
                    hr = m_pHidReadRequest[i]->AddRef();

                    if (SUCCEEDED(hr))
                    {
                        hr = m_pHidReadRequest[i]->InitializeRequest(pWdfDevice, this);
                    }
                    else
                    {
                        Trace(TRACE_LEVEL_ERROR, "Failed during m_pHidReadRequest[%i]->AddRef(), hr = %!HRESULT!", i, hr);
                    }

                    if(SUCCEEDED(hr))
                    {
                        hr = m_pHidReadRequest[i]->CreateAndSendReadRequest();
                    }
                    else
                    {
                        Trace(TRACE_LEVEL_ERROR, "Failed during m_pHidReadRequest[%i]->InitializeRequest(), hr = %!HRESULT!", i, hr);
                        break;
                    }

                    if (FAILED(hr))
                    {
                        Trace(TRACE_LEVEL_ERROR, "Failed during m_pHidReadRequest[%i]->CreateAndSendRequest(), hr = %!HRESULT!", i, hr);
                    }
                }
                else
                {
                    Trace(TRACE_LEVEL_ERROR, "m_pHidReadRequest[%i] is NULL, hr = %!HRESULT!", i, hr);
                }
            }
            else
            {
                Trace(TRACE_LEVEL_ERROR, "Failed during m_pHidReadRequest[%i]->CreateInstance(), hr = %!HRESULT!", i, hr);
                break;
            }
        }
       
        // Create the HID Write objects
        // Initialize the request with the IWDFDevice object
        if (SUCCEEDED(hr))
        {
            hr = CComObject<CReadWriteRequest>::CreateInstance(&m_pHidWriteRequest);
        }
        if ((SUCCEEDED(hr)) && (NULL != m_pHidWriteRequest))
        {
            hr = m_pHidWriteRequest->AddRef();

            if (SUCCEEDED(hr))
            {
                hr = m_pHidWriteRequest->InitializeRequest(pWdfDevice, NULL);
            }
            else
            {
                Trace(TRACE_LEVEL_ERROR, "Failed during m_pHidWriteRequest->InitializeRequest(), hr = %!HRESULT!", hr);
            }
        }
        else
        {
            Trace(TRACE_LEVEL_ERROR, "Failed during m_pHidWriteRequest->CreateInstance(), hr = %!HRESULT!", hr);
        }
       
        // Create the HID Ioctl objects
        // Initialize the request with the IWDFDevice object
        if (SUCCEEDED(hr))
        {
            hr = CComObject<CReadWriteRequest>::CreateInstance(&m_pHidIoctlRequest);
        }
        if ((SUCCEEDED(hr)) && (NULL != m_pHidIoctlRequest))
        {
            hr = m_pHidIoctlRequest->AddRef();

            if (SUCCEEDED(hr))
            {
                hr = m_pHidIoctlRequest->InitializeRequest(pWdfDevice, NULL);
            }
            else
            {
                Trace(TRACE_LEVEL_ERROR, "Failed during m_pHidIoctlRequest->InitializeRequest(), hr = %!HRESULT!", hr);
            }
        }
        else
        {
            Trace(TRACE_LEVEL_ERROR, "Failed during m_pHidIoctlRequest->CreateInstance(), hr = %!HRESULT!", hr);
        }
        if (SUCCEEDED(hr))
        {
            m_fDeviceIdle = FALSE;
        }
    }

    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Exit, hr = %!HRESULT!", hr);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::DeInitSensorDevice
//
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::DeInitSensorDevice()
{
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    HRESULT hr = S_OK;

    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    if (FALSE == m_fDeviceIdle)
    {
        m_fReleasingDevice = TRUE;

        for( DWORD i = 0; i < PENDING_READ_COUNT; i++ )
        {
            hr = m_pHidReadRequest[i]->CancelAndStopPendingRequest();

            if (SUCCEEDED(hr))
            {
                m_pHidReadRequest[i]->UninitializeRequest();
            }
        
            SAFE_RELEASE(m_pHidReadRequest[i]);
        }

        if (SUCCEEDED(hr))
        {
            hr = m_pHidWriteRequest->CancelAndStopPendingRequest();
        }

        if (SUCCEEDED(hr))
        {
            hr = m_pHidWriteRequest->UninitializeRequest();
        }

        SAFE_RELEASE(m_pHidWriteRequest);

        if (SUCCEEDED(hr))
        {
            hr = m_pHidIoctlRequest->CancelAndStopPendingRequest();
        }

        if (SUCCEEDED(hr))
        {
            hr = m_pHidIoctlRequest->UninitializeRequest();
        }
        SAFE_RELEASE(m_pHidIoctlRequest);

        if (SUCCEEDED(hr))
        {
            m_fDeviceIdle = TRUE;
        }
    }

    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Exit, hr = %!HRESULT!", hr);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::UpdateSensorValues
//
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::UpdateSensorPropertyValues(_In_ LPWSTR wszObjectID, _In_ BOOL fSettableOnly)
{
    HRESULT             hr = S_OK;
    CSensor*            pSensor = NULL;  
    SensorType          sensType = SensorTypeNone;
    DWORD               sensIndex = 0;
    ULONG               maxReportSize = MAX_REPORT_SIZE;

    hr = FindSensorTypeFromObjectID((LPWSTR)wszObjectID, &sensType, &sensIndex);

    if (SUCCEEDED(hr))
    {
        if (m_pSensorManager->m_pSensorList.GetCount() > 0)
        {
            hr = GetSensorObject(sensIndex, &pSensor);
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "No sensors found, hr = %!HRESULT!", hr);
        }

        if (SUCCEEDED(hr) && (nullptr != pSensor))
        {
            BYTE* pSendReport = nullptr;

            try 
            {
                pSendReport = new BYTE[maxReportSize];
            }
            catch(...)
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Failed to allocate memory for send report, hr = %!HRESULT!", hr);

                if (nullptr != pSendReport) 
                {
                    delete[] pSendReport;
                }
            }

            if (SUCCEEDED(hr))
            {
                CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

                BOOL                fFeatureReportSupported = FALSE;
                ULONG               reportSize = 0;

                CAccelerometer*     pAccelerometer = NULL;
                CAmbientLight*      pAmbientLight = NULL;
                CPresence*          pPresence = NULL;
                CCompass*           pCompass = NULL;
                CInclinometer*      pInclinometer = NULL;
                CGyrometer*         pGyrometer = NULL;
                CBarometer*         pBarometer = NULL;
                CHygrometer*        pHygrometer = NULL;
                CThermometer*       pThermometer = NULL;
                CPotentiometer*     pPotentiometer = NULL;
                CDistance*          pDistance = NULL;
                CSwitch*            pSwitch = NULL;
                CVoltage*           pVoltage = NULL;
                CCurrent*           pCurrent = NULL;
                CPower*             pPower = NULL;
                CFrequency*         pFrequency = NULL;
                COrientation*       pOrientation = NULL;
                CCustom*            pCustom = NULL;
                CGeneric*           pGeneric = NULL;
                CUnsupported*       pUnsupported = NULL;

                switch (sensType)
                {
                case Accelerometer:
                    pAccelerometer = (CAccelerometer*) pSensor;
                    hr = pAccelerometer->UpdateAccelerometerPropertyValues(pSendReport, &reportSize, fSettableOnly, &fFeatureReportSupported);
                    break;
                case AmbientLight:
                    pAmbientLight = (CAmbientLight*) pSensor;
                    hr = pAmbientLight->UpdateAmbientLightPropertyValues(pSendReport, &reportSize, fSettableOnly, &fFeatureReportSupported);
                    break;
                case Presence:
                    pPresence = (CPresence*) pSensor;
                    hr = pPresence->UpdatePresencePropertyValues(pSendReport, &reportSize, fSettableOnly, &fFeatureReportSupported);
                    break;
                case Compass:
                    pCompass = (CCompass*) pSensor;
                    hr = pCompass->UpdateCompassPropertyValues(pSendReport, &reportSize, fSettableOnly, &fFeatureReportSupported);
                    break;
                case Inclinometer:
                    pInclinometer = (CInclinometer*) pSensor;
                    hr = pInclinometer->UpdateInclinometerPropertyValues(pSendReport, &reportSize, fSettableOnly, &fFeatureReportSupported);
                    break;
                case Gyrometer:
                    pGyrometer = (CGyrometer*) pSensor;
                    hr = pGyrometer->UpdateGyrometerPropertyValues(pSendReport, &reportSize, fSettableOnly, &fFeatureReportSupported);
                    break;
                case Barometer:
                    pBarometer = (CBarometer*) pSensor;
                    hr = pBarometer->UpdateBarometerPropertyValues(pSendReport, &reportSize, fSettableOnly, &fFeatureReportSupported);
                    break;
                case Hygrometer:
                    pHygrometer = (CHygrometer*) pSensor;
                    hr = pHygrometer->UpdateHygrometerPropertyValues(pSendReport, &reportSize, fSettableOnly, &fFeatureReportSupported);
                    break;
                case Thermometer:
                    pThermometer = (CThermometer*) pSensor;
                    hr = pThermometer->UpdateThermometerPropertyValues(pSendReport, &reportSize, fSettableOnly, &fFeatureReportSupported);
                    break;
                case Potentiometer:
                    pPotentiometer = (CPotentiometer*) pSensor;
                    hr = pPotentiometer->UpdatePotentiometerPropertyValues(pSendReport, &reportSize, fSettableOnly, &fFeatureReportSupported);
                    break;
                case Distance:
                    pDistance = (CDistance*) pSensor;
                    hr = pDistance->UpdateDistancePropertyValues(pSendReport, &reportSize, fSettableOnly, &fFeatureReportSupported);
                    break;
                case Switch:
                    pSwitch = (CSwitch*) pSensor;
                    hr = pSwitch->UpdateSwitchPropertyValues(pSendReport, &reportSize, fSettableOnly, &fFeatureReportSupported);
                    break;
                case Voltage:
                    pVoltage = (CVoltage*) pSensor;
                    hr = pVoltage->UpdateVoltagePropertyValues(pSendReport, &reportSize, fSettableOnly, &fFeatureReportSupported);
                    break;
                case Current:
                    pCurrent = (CCurrent*) pSensor;
                    hr = pCurrent->UpdateCurrentPropertyValues(pSendReport, &reportSize, fSettableOnly, &fFeatureReportSupported);
                    break;
                case Power:
                    pPower = (CPower*) pSensor;
                    hr = pPower->UpdatePowerPropertyValues(pSendReport, &reportSize, fSettableOnly, &fFeatureReportSupported);
                    break;
                case Frequency:
                    pFrequency = (CFrequency*) pSensor;
                    hr = pFrequency->UpdateFrequencyPropertyValues(pSendReport, &reportSize, fSettableOnly, &fFeatureReportSupported);
                    break;
                case Orientation:
                    pOrientation = (COrientation*) pSensor;
                    hr = pOrientation->UpdateOrientationPropertyValues(pSendReport, &reportSize, fSettableOnly, &fFeatureReportSupported);
                    break;
                case Custom:
                    pCustom = (CCustom*) pSensor;
                    hr = pCustom->UpdateCustomPropertyValues(pSendReport, &reportSize, fSettableOnly, &fFeatureReportSupported);
                    break;
                case Generic:
                    pGeneric = (CGeneric*) pSensor;
                    hr = pGeneric->UpdateGenericPropertyValues(pSendReport, &reportSize, fSettableOnly, &fFeatureReportSupported);
                    break;
                case Unsupported:
                    pUnsupported = (CUnsupported*) pSensor;
                    hr = pUnsupported->UpdateUnsupportedPropertyValues(pSendReport, &reportSize, fSettableOnly, &fFeatureReportSupported);
                    break;

                default:
                    hr = E_UNEXPECTED;
                    break;
                }

#pragma warning(suppress: 6001) //Using unitialized memory
                Trace(TRACE_LEVEL_VERBOSE, "Final Feature Report in %!FUNC! for %s, length = %02i, 0x[%02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x ...]", 
                    pSensor->m_SensorName, reportSize,
                    pSendReport[0], pSendReport[1], pSendReport[2], pSendReport[3],
                    pSendReport[4], pSendReport[5], pSendReport[6], pSendReport[7],
                    pSendReport[8], pSendReport[9], pSendReport[10], pSendReport[11],
                    pSendReport[12], pSendReport[13], pSendReport[14], pSendReport[15]);

                delete[] pSendReport;
            }
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to get pSensor, hr = %!HRESULT!", hr);
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::UpdateSensorValues
//
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::UpdateSensorDataFieldValues(_In_ LPWSTR wszObjectID)
{
    HRESULT             hr = S_OK;
    SensorType          sensType = SensorTypeNone;
    DWORD               sensIndex = 0;

    hr = FindSensorTypeFromObjectID((LPWSTR)wszObjectID, &sensType, &sensIndex);

    //This will initiate an async response from the device
    //No return values are expected in the passed buffer
    if (SUCCEEDED(hr))
    {
        hr = E_OUTOFMEMORY;

        ULONG maxReportSize = MAX_REPORT_SIZE;
        BYTE* pSendReport = new BYTE[maxReportSize];

        if (nullptr != pSendReport)
        {
            ZeroMemory(pSendReport, maxReportSize);

            ULONG sensCount = (ULONG)m_pSensorManager->m_pSensorList.GetCount();

            if (sensCount > 1)
            {
                pSendReport[0] = (BYTE)sensIndex + 1;
            }
            else
            {
                pSendReport[0] = 0;
            }

            { // begin scope

                // m_pHidIoctlRequest can be set null on another thread via
                // DeInitSensorDevice() so protect access to it

                CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

                if (nullptr != m_pHidIoctlRequest)
                {
                    hr = m_pHidIoctlRequest->CreateAndSendIoctlRequest(
                            (BYTE*)pSendReport,
                            m_pSensorManager->m_HidCaps.InputReportByteLength, 
                            IOCTL_HID_GET_INPUT_REPORT,
                            NULL);
                }

            } // end scope

            delete[] (BYTE*)pSendReport;
        }
        else
        {
            Trace(TRACE_LEVEL_ERROR, "Failed to allocate memory for pSendReport, hr = %!HRESULT!", hr);
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::RequestDeviceInfo
//
// Synchronously request the report descriptor, which contains
// the sensor type.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::RequestDeviceInfo(
            _Inout_ SensorType* pSensType,
            _Inout_updates_(HID_USB_DESCRIPTOR_MAX_LENGTH) LPWSTR pwszManufacturer,
            _Inout_updates_(HID_USB_DESCRIPTOR_MAX_LENGTH) LPWSTR pwszProduct,
            _Inout_updates_(HID_USB_DESCRIPTOR_MAX_LENGTH) LPWSTR pwszSerialNumber,
            _Inout_updates_(HID_USB_DESCRIPTOR_MAX_LENGTH) LPWSTR pwszDeviceID)
{
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    HID_COLLECTION_INFORMATION info;
    m_pSensorManager->m_pPreparsedData = NULL;
    HIDP_CAPS caps;
    HRESULT hr = E_UNEXPECTED;
    WCHAR tempID[HID_USB_DESCRIPTOR_MAX_LENGTH] = L"";

    // m_pHidIoctlRequest can be set null on another thread via
    // DeInitSensorDevice() so protect access to it

    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    if (nullptr != m_pHidIoctlRequest)
    {
        hr = m_pHidIoctlRequest->CreateAndSendIoctlRequest(
                (BYTE*)(&info), 
                sizeof(info), 
                IOCTL_HID_GET_COLLECTION_INFORMATION,
                NULL);
    
        if (SUCCEEDED(hr))
        {
            Trace(TRACE_LEVEL_CRITICAL, "Device VID     = 0x%x", info.VendorID);
            Trace(TRACE_LEVEL_CRITICAL, "Device PID     = 0x%x", info.ProductID);
            Trace(TRACE_LEVEL_CRITICAL, "Device Version = 0x%x", info.VersionNumber);

            // This try/catch is here because OACR warns if this throws an exception there is a memory leak
            // even though there is no leak
            try 
            {
                if (info.DescriptorSize > 0)
                {
                    m_pSensorManager->m_pPreparsedData = (PHIDP_PREPARSED_DATA)(new BYTE[info.DescriptorSize]);
                }
            }
            catch(...)
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Failed to allocate memory for prepared data, hr = %!HRESULT!", hr);

                if (NULL != m_pSensorManager->m_pPreparsedData)
                {
                    delete[] (BYTE*)m_pSensorManager->m_pPreparsedData;
                }
            }
        }

        if (SUCCEEDED(hr) && (NULL != m_pSensorManager->m_pPreparsedData))
        {
            ZeroMemory(m_pSensorManager->m_pPreparsedData, info.DescriptorSize);

            hr = m_pHidIoctlRequest->CreateAndSendIoctlRequest(
                    (BYTE*)m_pSensorManager->m_pPreparsedData,
                    info.DescriptorSize, 
                    IOCTL_HID_GET_COLLECTION_DESCRIPTOR,
                    NULL);
        }
        else
        {
            hr = E_POINTER;
            Trace(TRACE_LEVEL_ERROR, "Preparsed data pointer was null, hr = %!HRESULT!", hr);
        }

        if (SUCCEEDED(hr))
        {
            // Call the parser to determine the capabilites of this HID device.
            hr = HidP_GetCaps(m_pSensorManager->m_pPreparsedData, &caps);
        }

        if (SUCCEEDED(hr))
        {
            // Call the parser to determine the capabilites of this HID device.
            // Store for later use so no need to re-parse
            hr = HidP_GetCaps(m_pSensorManager->m_pPreparsedData, &m_pSensorManager->m_HidCaps);
        }

        if (SUCCEEDED(hr))
        {
            if  ((NULL != &caps) &&
                (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage))
            {
                ULONG numNodes = caps.NumberLinkCollectionNodes;
                m_pSensorManager->m_pLinkCollectionNodes = (new HIDP_LINK_COLLECTION_NODE[(caps.NumberLinkCollectionNodes)*(sizeof(HIDP_LINK_COLLECTION_NODE))]);

                if (NULL != m_pSensorManager->m_pLinkCollectionNodes)
                {
                    hr = HidP_GetLinkCollectionNodes(m_pSensorManager->m_pLinkCollectionNodes, &numNodes, m_pSensorManager->m_pPreparsedData);

                    if (SUCCEEDED(hr))
                    {
                        m_pSensorManager->m_NumLinkCollectionNodes = numNodes;
                    }
                }
                else
                {
                    hr = E_UNEXPECTED;
                    Trace(TRACE_LEVEL_ERROR, "Failed to allocation memory for LinkCollectionNodes, hr = %!HRESULT!", hr);
                }

                if (SUCCEEDED(hr) && (NULL != &caps) &&
                    (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage) &&
                    (HID_DRIVER_USAGE_SENSOR_TYPE_COLLECTION == caps.Usage))
                {
                    ULONG numSensorNodes = 0;

                    //find the number of sensors, those nodes with a parent of 0
                    //ignore the TLC node by starting with 1
                    for (DWORD dwIdx = 1; dwIdx < caps.NumberLinkCollectionNodes; dwIdx++)
                    {
                        if (0 == m_pSensorManager->m_pLinkCollectionNodes[dwIdx].Parent)
                        {
                            numSensorNodes++;
                        }
                    }

                    if ((SUCCEEDED(hr)) && (numSensorNodes >= 1))
                    {
                        *pSensType = Collection;
                        DWORD dwSensorNumber = 0;

                        Trace(TRACE_LEVEL_CRITICAL, "Sensor collection present with %i sensors", numSensorNodes);

                        //Ignore the TLC node by starting with 1
                        for (USHORT dwLinkCollectionNode = 1; dwLinkCollectionNode < caps.NumberLinkCollectionNodes; dwLinkCollectionNode++)
                        {
                            //only those nodes with a parent of 0 are sensor nodes
                            if (0 == m_pSensorManager->m_pLinkCollectionNodes[dwLinkCollectionNode].Parent)
                            {
                                DWORD dwLinkUsage = m_pSensorManager->m_pLinkCollectionNodes[dwLinkCollectionNode].LinkUsage;

                                switch (dwLinkUsage)
                                {
                                case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_ACCELEROMETER_1D:
                                case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_ACCELEROMETER_2D:
                                case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_ACCELEROMETER_3D:
                                    m_pSensorManager->m_AvailableSensorsTypes[dwSensorNumber] = Accelerometer;
                                    m_pSensorManager->m_AvailableSensorsUsages[dwSensorNumber] = dwLinkUsage;
                                    m_pSensorManager->m_AvailableSensorsLinkCollections[dwSensorNumber] = dwLinkCollectionNode;
                                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Accelerometer, Usage = 0x%x", dwSensorNumber+1, dwLinkUsage);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_TYPE_LIGHT_AMBIENTLIGHT:
                                    m_pSensorManager->m_AvailableSensorsTypes[dwSensorNumber] = AmbientLight;
                                    m_pSensorManager->m_AvailableSensorsUsages[dwSensorNumber] = dwLinkUsage;
                                    m_pSensorManager->m_AvailableSensorsLinkCollections[dwSensorNumber] = dwLinkCollectionNode;
                                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Ambient Light, Usage = 0x%x", dwSensorNumber+1, dwLinkUsage);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_TYPE_BIOMETRIC_PRESENCE:
                                case HID_DRIVER_USAGE_SENSOR_TYPE_BIOMETRIC_PROXIMITY:
                                    m_pSensorManager->m_AvailableSensorsTypes[dwSensorNumber] = Presence;
                                    m_pSensorManager->m_AvailableSensorsUsages[dwSensorNumber] = dwLinkUsage;
                                    m_pSensorManager->m_AvailableSensorsLinkCollections[dwSensorNumber] = dwLinkCollectionNode;
                                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Presence, Usage = 0x%x", dwSensorNumber+1, dwLinkUsage);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_COMPASS_1D:
                                case HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_COMPASS_3D:
                                    m_pSensorManager->m_AvailableSensorsTypes[dwSensorNumber] = Compass;
                                    m_pSensorManager->m_AvailableSensorsUsages[dwSensorNumber] = dwLinkUsage;
                                    m_pSensorManager->m_AvailableSensorsLinkCollections[dwSensorNumber] = dwLinkCollectionNode;
                                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Compass, Usage = 0x%x", dwSensorNumber+1, dwLinkUsage);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_INCLINOMETER_1D:
                                case HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_INCLINOMETER_2D:
                                case HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_INCLINOMETER_3D:
                                    m_pSensorManager->m_AvailableSensorsTypes[dwSensorNumber] = Inclinometer;
                                    m_pSensorManager->m_AvailableSensorsUsages[dwSensorNumber] = dwLinkUsage;
                                    m_pSensorManager->m_AvailableSensorsLinkCollections[dwSensorNumber] = dwLinkCollectionNode;
                                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Inclinometer, Usage = 0x%x", dwSensorNumber+1, dwLinkUsage);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_GYROMETER_1D:
                                case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_GYROMETER_2D:
                                case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_GYROMETER_3D:
                                    m_pSensorManager->m_AvailableSensorsTypes[dwSensorNumber] = Gyrometer;
                                    m_pSensorManager->m_AvailableSensorsUsages[dwSensorNumber] = dwLinkUsage;
                                    m_pSensorManager->m_AvailableSensorsLinkCollections[dwSensorNumber] = dwLinkCollectionNode;
                                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Gyrometer, Usage = 0x%x", dwSensorNumber+1, dwLinkUsage);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_TYPE_ENVIRONMENTAL_ATMOSPHERIC_PRESSURE:
                                    m_pSensorManager->m_AvailableSensorsTypes[dwSensorNumber] = Barometer;
                                    m_pSensorManager->m_AvailableSensorsUsages[dwSensorNumber] = dwLinkUsage;
                                    m_pSensorManager->m_AvailableSensorsLinkCollections[dwSensorNumber] = dwLinkCollectionNode;
                                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Barometer, Usage = 0x%x", dwSensorNumber+1, dwLinkUsage);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_TYPE_ENVIRONMENTAL_HUMIDITY:
                                    m_pSensorManager->m_AvailableSensorsTypes[dwSensorNumber] = Hygrometer;
                                    m_pSensorManager->m_AvailableSensorsUsages[dwSensorNumber] = dwLinkUsage;
                                    m_pSensorManager->m_AvailableSensorsLinkCollections[dwSensorNumber] = dwLinkCollectionNode;
                                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Hygrometer, Usage = 0x%x", dwSensorNumber+1, dwLinkUsage);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_TYPE_ENVIRONMENTAL_TEMPERATURE:
                                    m_pSensorManager->m_AvailableSensorsTypes[dwSensorNumber] = Thermometer;
                                    m_pSensorManager->m_AvailableSensorsUsages[dwSensorNumber] = dwLinkUsage;
                                    m_pSensorManager->m_AvailableSensorsLinkCollections[dwSensorNumber] = dwLinkCollectionNode;
                                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Thermometer, Usage = 0x%x", dwSensorNumber+1, dwLinkUsage);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_TYPE_ELECTRICAL_POTENTIOMETER:
                                    m_pSensorManager->m_AvailableSensorsTypes[dwSensorNumber] = Potentiometer;
                                    m_pSensorManager->m_AvailableSensorsUsages[dwSensorNumber] = dwLinkUsage;
                                    m_pSensorManager->m_AvailableSensorsLinkCollections[dwSensorNumber] = dwLinkCollectionNode;
                                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Potentiometer, Usage = 0x%x", dwSensorNumber+1, dwLinkUsage);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_DISTANCE_1D:
                                case HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_DISTANCE_2D:
                                case HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_DISTANCE_3D:
                                    m_pSensorManager->m_AvailableSensorsTypes[dwSensorNumber] = Distance;
                                    m_pSensorManager->m_AvailableSensorsUsages[dwSensorNumber] = dwLinkUsage;
                                    m_pSensorManager->m_AvailableSensorsLinkCollections[dwSensorNumber] = dwLinkCollectionNode;
                                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Distance, Usage = 0x%x", dwSensorNumber+1, dwLinkUsage);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH:
                                case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH_ARRAY:
                                case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_MULTIVAL_SWITCH:
                                case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_MOTION_DETECTOR:
                                case HID_DRIVER_USAGE_SENSOR_TYPE_BIOMETRIC_TOUCH:
                                    m_pSensorManager->m_AvailableSensorsTypes[dwSensorNumber] = Switch;
                                    m_pSensorManager->m_AvailableSensorsUsages[dwSensorNumber] = dwLinkUsage;
                                    m_pSensorManager->m_AvailableSensorsLinkCollections[dwSensorNumber] = dwLinkCollectionNode;
                                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Switch, Usage = 0x%x", dwSensorNumber+1, dwLinkUsage);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_TYPE_ELECTRICAL_VOLTAGE:
                                    m_pSensorManager->m_AvailableSensorsTypes[dwSensorNumber] = Voltage;
                                    m_pSensorManager->m_AvailableSensorsUsages[dwSensorNumber] = dwLinkUsage;
                                    m_pSensorManager->m_AvailableSensorsLinkCollections[dwSensorNumber] = dwLinkCollectionNode;
                                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Voltage, Usage = 0x%x", dwSensorNumber+1, dwLinkUsage);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_TYPE_ELECTRICAL_CURRENT:
                                    m_pSensorManager->m_AvailableSensorsTypes[dwSensorNumber] = Current;
                                    m_pSensorManager->m_AvailableSensorsUsages[dwSensorNumber] = dwLinkUsage;
                                    m_pSensorManager->m_AvailableSensorsLinkCollections[dwSensorNumber] = dwLinkCollectionNode;
                                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Current, Usage = 0x%x", dwSensorNumber+1, dwLinkUsage);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_TYPE_ELECTRICAL_POWER:
                                    m_pSensorManager->m_AvailableSensorsTypes[dwSensorNumber] = Power;
                                    m_pSensorManager->m_AvailableSensorsUsages[dwSensorNumber] = dwLinkUsage;
                                    m_pSensorManager->m_AvailableSensorsLinkCollections[dwSensorNumber] = dwLinkCollectionNode;
                                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Power, Usage = 0x%x", dwSensorNumber+1, dwLinkUsage);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_TYPE_ELECTRICAL_FREQUENCY:
                                    m_pSensorManager->m_AvailableSensorsTypes[dwSensorNumber] = Frequency;
                                    m_pSensorManager->m_AvailableSensorsUsages[dwSensorNumber] = dwLinkUsage;
                                    m_pSensorManager->m_AvailableSensorsLinkCollections[dwSensorNumber] = dwLinkCollectionNode;
                                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Frequency, Usage = 0x%x", dwSensorNumber+1, dwLinkUsage);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_DEVICE_ORIENTATION:
                                    m_pSensorManager->m_AvailableSensorsTypes[dwSensorNumber] = Orientation;
                                    m_pSensorManager->m_AvailableSensorsUsages[dwSensorNumber] = dwLinkUsage;
                                    m_pSensorManager->m_AvailableSensorsLinkCollections[dwSensorNumber] = dwLinkCollectionNode;
                                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Orientation, Usage = 0x%x", dwSensorNumber+1, dwLinkUsage);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_TYPE_OTHER_CUSTOM:
                                    m_pSensorManager->m_AvailableSensorsTypes[dwSensorNumber] = Custom;
                                    m_pSensorManager->m_AvailableSensorsUsages[dwSensorNumber] = dwLinkUsage;
                                    m_pSensorManager->m_AvailableSensorsLinkCollections[dwSensorNumber] = dwLinkCollectionNode;
                                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Custom, Usage = 0x%x", dwSensorNumber+1, dwLinkUsage);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_TYPE_OTHER_GENERIC:
                                    m_pSensorManager->m_AvailableSensorsTypes[dwSensorNumber] = Generic;
                                    m_pSensorManager->m_AvailableSensorsUsages[dwSensorNumber] = dwLinkUsage;
                                    m_pSensorManager->m_AvailableSensorsLinkCollections[dwSensorNumber] = dwLinkCollectionNode;
                                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Generic, Usage = 0x%x", dwSensorNumber+1, dwLinkUsage);
                                    break;

                                default:
                                    m_pSensorManager->m_AvailableSensorsTypes[dwSensorNumber] = Unsupported;
                                    m_pSensorManager->m_AvailableSensorsUsages[dwSensorNumber] = dwLinkUsage;
                                    m_pSensorManager->m_AvailableSensorsLinkCollections[dwSensorNumber] = dwLinkCollectionNode;
                                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Unsupported, Usage = 0x%x", dwSensorNumber+1, dwLinkUsage);
                                    break;
                                }

                                dwSensorNumber++;
                            }
                        }

                        if (m_pSensorManager->m_AvailableSensorsTypes.GetCount() != (numSensorNodes))
                        {
                            hr = E_UNEXPECTED;
                            Trace(TRACE_LEVEL_ERROR, "Invalid sensor type discovered in a collection node, hr = %!HRESULT!", hr);
                        }
                    }
                }
                else if ((NULL != &caps) &&
                    (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage) &&
                    ((HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_ACCELEROMETER_1D == caps.Usage) || (HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_ACCELEROMETER_2D == caps.Usage) || (HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_ACCELEROMETER_3D == caps.Usage))
                    )
                {
                    m_pSensorManager->m_AvailableSensorsTypes[0] = Accelerometer;
                    m_pSensorManager->m_AvailableSensorsUsages[0] = caps.Usage;
                    m_pSensorManager->m_AvailableSensorsLinkCollections[0] = 0;
                    *pSensType = Accelerometer;
                    Trace(TRACE_LEVEL_CRITICAL, "Individual sensor present - Accelerometer, Usage = 0x%x", caps.Usage);

                }

                else if ((NULL != &caps) &&
                    (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage) &&
                    (HID_DRIVER_USAGE_SENSOR_TYPE_LIGHT_AMBIENTLIGHT == caps.Usage))
                {
                    m_pSensorManager->m_AvailableSensorsTypes[0] = AmbientLight;
                    m_pSensorManager->m_AvailableSensorsUsages[0] = caps.Usage;
                    m_pSensorManager->m_AvailableSensorsLinkCollections[0] = 0;
                    *pSensType = AmbientLight;
                    Trace(TRACE_LEVEL_CRITICAL, "Individual sensor present - Ambient Light, Usage = 0x%x", caps.Usage);
                }

                else if ((NULL != &caps) &&
                    (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage) &&
                    ((HID_DRIVER_USAGE_SENSOR_TYPE_BIOMETRIC_PRESENCE == caps.Usage) || (HID_DRIVER_USAGE_SENSOR_TYPE_BIOMETRIC_PROXIMITY == caps.Usage))
                    )
                {
                    m_pSensorManager->m_AvailableSensorsTypes[0] = Presence;
                    m_pSensorManager->m_AvailableSensorsUsages[0] = caps.Usage;
                    m_pSensorManager->m_AvailableSensorsLinkCollections[0] = 0;
                    *pSensType = Presence;
                    Trace(TRACE_LEVEL_CRITICAL, "Individual sensor present - Presence, Usage = 0x%x", caps.Usage);
                }

                else if ((NULL != &caps) &&
                    (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage) &&
                    ((HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_COMPASS_1D == caps.Usage) || (HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_COMPASS_3D == caps.Usage))
                    )
                {
                    m_pSensorManager->m_AvailableSensorsTypes[0] = Compass;
                    m_pSensorManager->m_AvailableSensorsUsages[0] = caps.Usage;
                    m_pSensorManager->m_AvailableSensorsLinkCollections[0] = 0;
                    *pSensType = Compass;
                    Trace(TRACE_LEVEL_CRITICAL, "Individual sensor present - Compass, Usage = 0x%x", caps.Usage);
                }

                else if ((NULL != &caps) &&
                    (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage) &&
                    ((HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_INCLINOMETER_1D == caps.Usage) || (HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_INCLINOMETER_2D == caps.Usage) || (HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_INCLINOMETER_3D == caps.Usage))
                    )
                {
                    m_pSensorManager->m_AvailableSensorsTypes[0] = Inclinometer;
                    m_pSensorManager->m_AvailableSensorsUsages[0] = caps.Usage;
                    m_pSensorManager->m_AvailableSensorsLinkCollections[0] = 0;
                    *pSensType = Inclinometer;
                    Trace(TRACE_LEVEL_CRITICAL, "Individual sensor present - Inclinometer, Usage = 0x%x", caps.Usage);
                }

                else if ((NULL != &caps) &&
                    (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage) &&
                    ((HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_GYROMETER_1D == caps.Usage) || (HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_GYROMETER_2D == caps.Usage) || (HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_GYROMETER_3D == caps.Usage))
                    )
                {
                    m_pSensorManager->m_AvailableSensorsTypes[0] = Gyrometer;
                    m_pSensorManager->m_AvailableSensorsUsages[0] = caps.Usage;
                    m_pSensorManager->m_AvailableSensorsLinkCollections[0] = 0;
                    *pSensType = Gyrometer;
                    Trace(TRACE_LEVEL_CRITICAL, "Individual sensor present - Gyrometer, Usage = 0x%x", caps.Usage);
                }

                else if ((NULL != &caps) &&
                    (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage) &&
                    (HID_DRIVER_USAGE_SENSOR_TYPE_ENVIRONMENTAL_ATMOSPHERIC_PRESSURE == caps.Usage))
                {
                    m_pSensorManager->m_AvailableSensorsTypes[0] = Barometer;
                    m_pSensorManager->m_AvailableSensorsUsages[0] = caps.Usage;
                    m_pSensorManager->m_AvailableSensorsLinkCollections[0] = 0;
                    *pSensType = Barometer;
                    Trace(TRACE_LEVEL_CRITICAL, "Individual sensor present - Barometer, Usage = 0x%x", caps.Usage);
                }

                else if ((NULL != &caps) &&
                    (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage) &&
                    (HID_DRIVER_USAGE_SENSOR_TYPE_ENVIRONMENTAL_HUMIDITY == caps.Usage))
                {
                    m_pSensorManager->m_AvailableSensorsTypes[0] = Hygrometer;
                    m_pSensorManager->m_AvailableSensorsUsages[0] = caps.Usage;
                    m_pSensorManager->m_AvailableSensorsLinkCollections[0] = 0;
                    *pSensType = Hygrometer;
                    Trace(TRACE_LEVEL_CRITICAL, "Individual sensor present - Hygrometer, Usage = 0x%x", caps.Usage);
                }

                else if ((NULL != &caps) &&
                    (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage) &&
                    (HID_DRIVER_USAGE_SENSOR_TYPE_ENVIRONMENTAL_TEMPERATURE == caps.Usage))
                {
                    m_pSensorManager->m_AvailableSensorsTypes[0] = Thermometer;
                    m_pSensorManager->m_AvailableSensorsUsages[0] = caps.Usage;
                    m_pSensorManager->m_AvailableSensorsLinkCollections[0] = 0;
                    *pSensType = Thermometer;
                    Trace(TRACE_LEVEL_CRITICAL, "Individual sensor present - Thermometer, Usage = 0x%x", caps.Usage);
                }

                else if ((NULL != &caps) &&
                    (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage) &&
                    (HID_DRIVER_USAGE_SENSOR_TYPE_ELECTRICAL_POTENTIOMETER == caps.Usage))
                {
                    m_pSensorManager->m_AvailableSensorsTypes[0] = Potentiometer;
                    m_pSensorManager->m_AvailableSensorsUsages[0] = caps.Usage;
                    m_pSensorManager->m_AvailableSensorsLinkCollections[0] = 0;
                    *pSensType = Potentiometer;
                    Trace(TRACE_LEVEL_CRITICAL, "Individual sensor present - Potentiometer, Usage = 0x%x", caps.Usage);
                }

                else if ((NULL != &caps) &&
                    (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage) &&
                    ((HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_DISTANCE_1D == caps.Usage) || (HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_DISTANCE_2D == caps.Usage) || (HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_DISTANCE_3D == caps.Usage))
                    )
                {
                    m_pSensorManager->m_AvailableSensorsTypes[0] = Distance;
                    m_pSensorManager->m_AvailableSensorsUsages[0] = caps.Usage;
                    m_pSensorManager->m_AvailableSensorsLinkCollections[0] = 0;
                    *pSensType = Distance;
                    Trace(TRACE_LEVEL_CRITICAL, "Individual sensor present - Distance, Usage = 0x%x", caps.Usage);
                }

                else if ((NULL != &caps) &&
                    (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage) &&
                    ((HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH == caps.Usage) 
                        || (HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH_ARRAY == caps.Usage) 
                        || (HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_MULTIVAL_SWITCH == caps.Usage)
                        || (HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_MOTION_DETECTOR == caps.Usage)
                        || (HID_DRIVER_USAGE_SENSOR_TYPE_BIOMETRIC_TOUCH == caps.Usage)
                        )
                    )
                {
                    m_pSensorManager->m_AvailableSensorsTypes[0] = Switch;
                    m_pSensorManager->m_AvailableSensorsUsages[0] = caps.Usage;
                    m_pSensorManager->m_AvailableSensorsLinkCollections[0] = 0;
                    *pSensType = Switch;
                    Trace(TRACE_LEVEL_CRITICAL, "Individual sensor present - Switch, Usage = 0x%x", caps.Usage);
                }

                else if ((NULL != &caps) &&
                    (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage) &&
                    (HID_DRIVER_USAGE_SENSOR_TYPE_ELECTRICAL_VOLTAGE == caps.Usage))
                {
                    m_pSensorManager->m_AvailableSensorsTypes[0] = Voltage;
                    m_pSensorManager->m_AvailableSensorsUsages[0] = caps.Usage;
                    m_pSensorManager->m_AvailableSensorsLinkCollections[0] = 0;
                    *pSensType = Voltage;
                    Trace(TRACE_LEVEL_CRITICAL, "Individual sensor present - Voltage, Usage = 0x%x", caps.Usage);
                }

                else if ((NULL != &caps) &&
                    (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage) &&
                    (HID_DRIVER_USAGE_SENSOR_TYPE_ELECTRICAL_CURRENT == caps.Usage))
                {
                    m_pSensorManager->m_AvailableSensorsTypes[0] = Current;
                    m_pSensorManager->m_AvailableSensorsUsages[0] = caps.Usage;
                    m_pSensorManager->m_AvailableSensorsLinkCollections[0] = 0;
                    *pSensType = Current;
                    Trace(TRACE_LEVEL_CRITICAL, "Individual sensor present - Current, Usage = 0x%x", caps.Usage);
                }

                else if ((NULL != &caps) &&
                    (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage) &&
                    (HID_DRIVER_USAGE_SENSOR_TYPE_ELECTRICAL_POWER == caps.Usage))
                {
                    m_pSensorManager->m_AvailableSensorsTypes[0] = Power;
                    m_pSensorManager->m_AvailableSensorsUsages[0] = caps.Usage;
                    m_pSensorManager->m_AvailableSensorsLinkCollections[0] = 0;
                    *pSensType = Power;
                    Trace(TRACE_LEVEL_CRITICAL, "Individual sensor present - Power, Usage = 0x%x", caps.Usage);
                }

                else if ((NULL != &caps) &&
                    (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage) &&
                    (HID_DRIVER_USAGE_SENSOR_TYPE_ELECTRICAL_FREQUENCY == caps.Usage))
                {
                    m_pSensorManager->m_AvailableSensorsTypes[0] = Frequency;
                    m_pSensorManager->m_AvailableSensorsUsages[0] = caps.Usage;
                    m_pSensorManager->m_AvailableSensorsLinkCollections[0] = 0;
                    *pSensType = Frequency;
                    Trace(TRACE_LEVEL_CRITICAL, "Individual sensor present - Frequency, Usage = 0x%x", caps.Usage);
                }

                else if ((NULL != &caps) &&
                    (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage) &&
                    (HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_DEVICE_ORIENTATION == caps.Usage))
                {
                    m_pSensorManager->m_AvailableSensorsTypes[0] = Orientation;
                    m_pSensorManager->m_AvailableSensorsUsages[0] = caps.Usage;
                    m_pSensorManager->m_AvailableSensorsLinkCollections[0] = 0;
                    *pSensType = Orientation;
                    Trace(TRACE_LEVEL_CRITICAL, "Individual sensor present - Orientation, Usage = 0x%x", caps.Usage);
                }

                else if ((NULL != &caps) &&
                    (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage) &&
                    (HID_DRIVER_USAGE_SENSOR_TYPE_OTHER_CUSTOM == caps.Usage))
                {
                    m_pSensorManager->m_AvailableSensorsTypes[0] = Custom;
                    m_pSensorManager->m_AvailableSensorsUsages[0] = caps.Usage;
                    m_pSensorManager->m_AvailableSensorsLinkCollections[0] = 0;
                    *pSensType = Custom;
                    Trace(TRACE_LEVEL_CRITICAL, "Individual sensor present - Custom, Usage = 0x%x", caps.Usage);
                }

                else if ((NULL != &caps) &&
                    (HID_DRIVER_USAGE_PAGE_SENSOR == caps.UsagePage) &&
                    (HID_DRIVER_USAGE_SENSOR_TYPE_OTHER_GENERIC == caps.Usage))
                {
                    m_pSensorManager->m_AvailableSensorsTypes[0] = Generic;
                    m_pSensorManager->m_AvailableSensorsUsages[0] = caps.Usage;
                    m_pSensorManager->m_AvailableSensorsLinkCollections[0] = 0;
                    *pSensType = Generic;
                    Trace(TRACE_LEVEL_CRITICAL, "Individual sensor present - Generic, Usage = 0x%x", caps.Usage);
                }

                else
                {
                    *pSensType = Unsupported;
                    hr = E_UNEXPECTED;
                    Trace(TRACE_LEVEL_CRITICAL, "Individual sensor present - Unsupported, Usage = 0x%x", caps.Usage);
                }
            }
        }
    
        if (SUCCEEDED(hr))
        {
            hr = m_pHidIoctlRequest->CreateAndSendIoctlRequest(
                    (BYTE*)(tempID), 
                    HID_USB_DESCRIPTOR_MAX_LENGTH,
                    IOCTL_HID_GET_SERIALNUMBER_STRING,
                    NULL);

            if (SUCCEEDED(hr))
            {
                tempID[HID_USB_DESCRIPTOR_MAX_LENGTH/2] = L'\0';

                hr = StringCchCopy(pwszSerialNumber, HID_USB_DESCRIPTOR_MAX_LENGTH, tempID);
                Trace(TRACE_LEVEL_CRITICAL, "Device Serial Number = %ls", pwszSerialNumber);

            }
            else
            {
                Trace(TRACE_LEVEL_WARNING, "Failed getting device serial number. hr = %!HRESULT!", hr);

                //
                // The HID transport driver returns STATUS_INVALID_PARAMETER/ERROR_INVALID_PARAMETER when the device has
                // no serial number. We should ignore this specific error, rather than failing the sensor manager start.
                //
                if (ERROR_INVALID_PARAMETER == HRESULT_CODE(hr))
                {
                    Trace(TRACE_LEVEL_WARNING, "Device serial number is optional for USB devices. Ignore the failure.");
                    pwszSerialNumber[0] = L'\0';
                    hr = S_OK;
                }
            }

            if (SUCCEEDED(hr))
            {
                hr = StringCchPrintf(pwszDeviceID, HID_USB_DESCRIPTOR_MAX_LENGTH, L"%04X:", info.VersionNumber);
            }

            if (SUCCEEDED(hr))
            {
                hr = StringCchCat(pwszDeviceID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempID);
                Trace(TRACE_LEVEL_CRITICAL, "Device ID            = %ls", pwszDeviceID);
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = m_pHidIoctlRequest->CreateAndSendIoctlRequest(
                    (BYTE*)(tempID), 
                    HID_USB_DESCRIPTOR_MAX_LENGTH,
                    IOCTL_HID_GET_MANUFACTURER_STRING,
                    NULL);

            if (SUCCEEDED(hr))
            {
                tempID[HID_USB_DESCRIPTOR_MAX_LENGTH/2] = L'\0';

                hr = StringCchCopy(pwszManufacturer, HID_USB_DESCRIPTOR_MAX_LENGTH, tempID);
                Trace(TRACE_LEVEL_CRITICAL, "Device Manufacturer  = %ls", pwszManufacturer);
            }
            else
            {
                Trace(TRACE_LEVEL_WARNING, "Failed getting device manufacturer string. hr = %!HRESULT!", hr);

                //
                // The HID transport driver returns STATUS_INVALID_PARAMETER/ERROR_INVALID_PARAMETER when the device has
                // no manufacturer string. We should ignore this specific error, rather than failing the sensor manager start.
                //
                if (ERROR_INVALID_PARAMETER == HRESULT_CODE(hr))
                {
                    Trace(TRACE_LEVEL_WARNING, "Device manufacturer string is optional for USB devices. Ignore the failure.");
                    pwszManufacturer[0] = L'\0';
                    hr = S_OK;
                }
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = m_pHidIoctlRequest->CreateAndSendIoctlRequest(
                    (BYTE*)(tempID), 
                    HID_USB_DESCRIPTOR_MAX_LENGTH,
                    IOCTL_HID_GET_PRODUCT_STRING,
                    NULL);

            if (SUCCEEDED(hr))
            {
                tempID[HID_USB_DESCRIPTOR_MAX_LENGTH/2] = L'\0';

                hr = StringCchCopy(pwszProduct, HID_USB_DESCRIPTOR_MAX_LENGTH, tempID);
                Trace(TRACE_LEVEL_CRITICAL, "Device Product       = %ls", pwszProduct);
            }
            else
            {
                Trace(TRACE_LEVEL_WARNING, "Failed getting device product string. hr = %!HRESULT!", hr);

                //
                // The HID transport driver returns STATUS_INVALID_PARAMETER/ERROR_INVALID_PARAMETER when the device has
                // no product string. We should ignore this specific error, rather than failing the sensor manager start.
                //
                if (ERROR_INVALID_PARAMETER == HRESULT_CODE(hr))
                {
                    Trace(TRACE_LEVEL_WARNING, "Device product string is optional for USB devices. Ignore the failure.");
                    pwszProduct[0] = L'\0';
                    hr = S_OK;
                }
            }
        }

    } // if (nullptr != m_pHidIoctlRequest)

    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Exit, hr = %!HRESULT!", hr);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnGetSupportedSensorObjects
//
// Routine Description:
//
//  This method is called by Sensor Class Extension during the initialize procedure to get 
//  the list of sensor objects and their supported properties.
//  
// Arguments:
//
//  ppPortableDeviceValuesCollection - A double IPortableDeviceValuesCollection pointer
//                                     that receives the set of Sensor property values
//
// Return Value:
//
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnGetSupportedSensorObjects(
        _Out_ IPortableDeviceValuesCollection** ppPortableDeviceValuesCollection
        )
{
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        // CoCreate a collection to store the sensor object identifiers.
        hr = CoCreateInstance(CLSID_PortableDeviceValuesCollection,
                                  NULL,
                                  CLSCTX_INPROC_SERVER,
                                  IID_PPV_ARGS(ppPortableDeviceValuesCollection));

        CComBSTR objectId;
        CComPtr<IPortableDeviceKeyCollection> spKeys;
        CComPtr<IPortableDeviceValues> spValues;

        POSITION pos = m_pSensorManager->m_AvailableSensorsIDs.GetStartPosition();

        while( NULL != pos)
        {
            objectId = m_pSensorManager->m_AvailableSensorsIDs.GetNextValue(pos);


            if (hr == S_OK)
            {
                hr = OnGetSupportedProperties(objectId, &spKeys);
            }

            if (hr == S_OK)
            {
#pragma warning(suppress: 6387) //nullptr is invalid parameter - use intentional
                hr = OnGetPropertyValues( nullptr, objectId, spKeys, &spValues );
            }

            if (hr == S_OK)
            {
                hr = (*ppPortableDeviceValuesCollection)->Add(spValues);
            }

            spKeys.Release();
            spValues.Release();
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Exit, hr = %!HRESULT!", hr);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnGetSupportedProperties
//
// Routine Description:
//
//  This method is called by Sensor Class Extension to get the list of supported properties
//  for a particular Sensor.
//  
// Arguments:
//
//  ObjectID - String that represents the object whose supported property keys are being requested
//  ppKeys - An IPortableDeviceKeyCollection to be populated with supported PROPERTYKEYs
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnGetSupportedProperties(
        _In_  LPWSTR pwszObjectID,
        _Out_ IPortableDeviceKeyCollection** ppKeys
        )
{
    Trace(TRACE_LEVEL_VERBOSE, "Get Properties supported by sensor %ls", pwszObjectID);

    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        SensorType sensType = SensorTypeNone;
        DWORD sensIndex = 0;
        CSensor* pSensor = nullptr;

        hr = FindSensorTypeFromObjectID(pwszObjectID, &sensType, &sensIndex);

        if (SUCCEEDED(hr))
        {
            hr = GetSensorObject(sensIndex, &pSensor);
        }

        if (SUCCEEDED(hr))
        {
            Trace(TRACE_LEVEL_INFORMATION, "Client unk Requesting Supported Properties for %s", pSensor->m_SensorName);
        }
    }

    if (SUCCEEDED(hr))
    {
        // CoCreate a collection to store the supported property keys.
        hr = CoCreateInstance(CLSID_PortableDeviceKeyCollection,
                                  NULL,
                                  CLSCTX_INPROC_SERVER,
                                  IID_PPV_ARGS(ppKeys));

        // Add supported property keys for the specified object to the collection
        if (SUCCEEDED(hr))
        {
            hr = AddSupportedPropertyKeys(pwszObjectID, *ppKeys);
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    Trace(TRACE_LEVEL_VERBOSE, "Properties supported by sensor %ls returned, hr = %!HRESULT!", pwszObjectID, hr);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnGetSupportedDataFields
//
// Routine Description:
//
//  This method is called by Sensor Class Extension to get the list of supported data fields
//  for a particular Sensor.
//  
// Arguments:
//
//  ObjectID - String that represents the object whose supported property keys are being requested
//  ppKeys - An IPortableDeviceKeyCollection to be populated with supported PROPERTYKEYs
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnGetSupportedDataFields(
        _In_  LPWSTR wszObjectID,
        _Out_ IPortableDeviceKeyCollection** ppKeys
        )
{
    Trace(TRACE_LEVEL_VERBOSE, "Get Datafields supported by sensor %ls", wszObjectID);

    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        SensorType sensType = SensorTypeNone;
        DWORD sensIndex = 0;
        CSensor* pSensor = nullptr;

        hr = FindSensorTypeFromObjectID(wszObjectID, &sensType, &sensIndex);

        if (SUCCEEDED(hr))
        {
            hr = GetSensorObject(sensIndex, &pSensor);
        }

        if (SUCCEEDED(hr))
        {
            Trace(TRACE_LEVEL_INFORMATION, "Client unk Requesting Supported DataFields for %s", pSensor->m_SensorName);
        }
    }

    if (SUCCEEDED(hr))
    {
        // CoCreate a collection to store the supported property keys.
        hr = CoCreateInstance(CLSID_PortableDeviceKeyCollection,
                                  NULL,
                                  CLSCTX_INPROC_SERVER,
                                  IID_PPV_ARGS(ppKeys));

        // Add supported property keys for the specified object to the collection
        if (hr == S_OK && ppKeys != NULL)
        {
            hr = AddSupportedDataFieldKeys(wszObjectID, *ppKeys);
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    Trace(TRACE_LEVEL_VERBOSE, "Datafields supported by sensor %ls returned, hr = %!HRESULT!", wszObjectID, hr);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnGetSupportedEvents
//
// Routine Description:
//
//  This method is called by Sensor Class Extension to get the list of supported events
//  for a particular Sensor.
//  
// Arguments:
//
// ObjectID - String that represents the object whose supported property keys are being requested
//  ppSupportedEvents - A set of GUIDs that represent the supported events
//  pulEventCount - Count of the number of events supported
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnGetSupportedEvents(
        _In_  LPWSTR ObjectID,
        _Out_ GUID **ppSupportedEvents,
        _Out_ ULONG *pulEventCount
        )
{
    Trace(TRACE_LEVEL_INFORMATION, "Get Events supported by sensor %ls", ObjectID);

    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        SensorType sensType = SensorTypeNone;
        DWORD sensIndex = 0;

        hr = FindSensorTypeFromObjectID((LPWSTR)ObjectID, &sensType, &sensIndex);

        if (SUCCEEDED(hr))
        {
            if (Accelerometer == sensType)
            {
                //Return three GUIDs
                GUID* pBuf = (GUID*)CoTaskMemAlloc(sizeof(GUID) * 3);

                if( pBuf != NULL )
                {
                    *pBuf       = SENSOR_EVENT_DATA_UPDATED;
                    *(pBuf + 1) = SENSOR_EVENT_STATE_CHANGED;
                    *(pBuf + 2) = SENSOR_EVENT_ACCELEROMETER_SHAKE;

                    *ppSupportedEvents = pBuf;
                    *pulEventCount = 3;
                }
                else
                {
                    hr = E_OUTOFMEMORY;

                    *ppSupportedEvents = NULL;
                    *pulEventCount = 0;
                }
            }
            else
            {
                //Return two GUIDs
                GUID* pBuf = (GUID*)CoTaskMemAlloc(sizeof(GUID) * 2);

                if( pBuf != NULL )
                {
                    *pBuf       = SENSOR_EVENT_DATA_UPDATED;
                    *(pBuf + 1) = SENSOR_EVENT_STATE_CHANGED;

                    *ppSupportedEvents = pBuf;
                    *pulEventCount = 2;
                }
                else
                {
                    hr = E_OUTOFMEMORY;

                    *ppSupportedEvents = NULL;
                    *pulEventCount = 0;
                }
            }
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    Trace(TRACE_LEVEL_INFORMATION, "Events supported by sensor %ls returned, hr = %!HRESULT!", ObjectID, hr);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnGetProperties
//
// Routine Description:
//
//  This method is called by Sensor Class Extension to get Sensor property values 
//  for a particular Sensor.
//  
// Arguments:
//
//  ObjectID - String that represents the object whose supported property keys are being requested
// 
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnGetProperties(
        _In_  IWDFFile* appId,
        _In_  LPWSTR ObjectID,
        _In_  IPortableDeviceKeyCollection* pKeys,
        _Out_ IPortableDeviceValues** ppValues
        )
{
    Trace(TRACE_LEVEL_VERBOSE, "Get Properties supported by sensor %ls for Client %p", ObjectID, appId);

    UNREFERENCED_PARAMETER(appId);

    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        SensorType sensType = SensorTypeNone;
        DWORD sensIndex = 0;
        CSensor* pSensor = nullptr;

        hr = FindSensorTypeFromObjectID(ObjectID, &sensType, &sensIndex);

        if (SUCCEEDED(hr))
        {
            hr = GetSensorObject(sensIndex, &pSensor);
        }

        if (SUCCEEDED(hr))
        {
            Trace(TRACE_LEVEL_INFORMATION, "Client %p Requesting Properties for %s", appId, pSensor->m_SensorName);
        }
    }

    if (SUCCEEDED(hr))
    {
        if ((ObjectID == NULL) ||
            (pKeys       == NULL) ||
            (ppValues     == NULL))
        {
            hr = E_INVALIDARG;
        }

        if (SUCCEEDED(hr))
        {
            hr = OnGetPropertyValues( appId, ObjectID, pKeys, ppValues );
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    Trace(TRACE_LEVEL_VERBOSE, "Properties supported by sensor %ls returned for Client %p, hr = %!HRESULT!", ObjectID, appId, hr);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnGetDataFields
//
// Routine Description:
//
//  This method is called by Sensor Class Extension to get Sensor data fields
//  for a particular Sensor.
// 
//  The parameters sent to us are:
//  wszObjectID - the object whose properties are being requested.
//  pKeys - the list of property keys of the properties to request from the object
//  pValues - an IPortableDeviceValues which will contain the property values retreived from the object
//
//  The driver should:
//  Read the specified properties for the specified object and populate pValues with the
//  results.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnGetDataFields(
        _In_  IWDFFile* appId,
        _In_  LPWSTR wszObjectID,
        _In_  IPortableDeviceKeyCollection* pKeys,
        _Out_ IPortableDeviceValues** ppValues
        )
{
    UNREFERENCED_PARAMETER(appId);

    Trace(TRACE_LEVEL_VERBOSE, "Get Datafields supported by sensor %ls for Client %p", wszObjectID, appId);


    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        if ((wszObjectID == NULL) ||
            (pKeys       == NULL) ||
            (ppValues     == NULL))
        {
            hr = E_INVALIDARG;
        }

        if (SUCCEEDED(hr))
        {
            // CoCreate a collection to store the property values.
            hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                      NULL,
                                      CLSCTX_INPROC_SERVER,
                                      IID_PPV_ARGS(ppValues));

            // Read the specified properties on the specified object and add the property values to the collection.
            if (hr == S_OK)
            {
                DWORD       cKeys       = 0;
                IPortableDeviceValues*  pValues = *ppValues; //fakes this call: hr = GetDataValuesForObject(wszObjectID, pKeys, *ppValues);

                hr = pKeys->GetCount(&cKeys);

                if (hr == S_OK)
                {
                    CAtlStringW strObjectID = wszObjectID;
                    CComBSTR objectId;
            
                    POSITION posID = m_pSensorManager->m_AvailableSensorsIDs.GetStartPosition();
                    POSITION posType = m_pSensorManager->m_AvailableSensorsTypes.GetStartPosition();
                    SensorType sensType = SensorTypeNone;
                    DWORD sensIndex = 0;
                    BOOL fPollFailed = FALSE;

                    while( NULL != posID)
                    {
                        objectId = m_pSensorManager->m_AvailableSensorsIDs.GetNextValue(posID);
                        sensType = m_pSensorManager->m_AvailableSensorsTypes.GetNextValue(posType);

                        if (strObjectID.CompareNoCase(objectId) == 0)
                        {
                            CSensor* pSensor = nullptr;

                            hr = FindSensorTypeFromObjectID((LPWSTR)wszObjectID, &sensType, &sensIndex);

                            if (SUCCEEDED(hr))
                            {
                                hr = GetSensorObject(sensIndex, &pSensor);

                                if (SUCCEEDED(hr) && (nullptr != pSensor))
                                {
                                    Trace(TRACE_LEVEL_INFORMATION, "Client %p Requesting Datafield Values for %s", appId, pSensor->m_SensorName);

                                    if ((FALSE == pSensor->m_fReportingState) || (FALSE == pSensor->m_fInitialDataReceived))
                                    {
                                        {
                                            //
                                            // Entering the critical section to make it thread-safe and also sync with 
                                            // OnAsyncReadCallback(). We're using m_CriticalSectionSensorUpdate instead 
                                            // of m_CriticalSection to avoid the dead-lock between async read completion 
                                            // and sensor de-initialization.
                                            //
                                            CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSectionSensorUpdate);
                                
                                            //issue a poll to the device for this sensor and wait for updated data from sensor
                                            pSensor->m_fSensorUpdated = FALSE;
                                            //NOTE: m_nNotifyOnSensorUpdate should be the Sensor Number n = {1..k}
                                            //this is derived from sensIndex = {0..k-1} so 1 is added indicating it is
                                            //a valid sensor (anything > 0)
                                            m_nNotifyOnSensorUpdate = sensIndex+1;
                                        }

                                        const DWORD dwInitialDataMaxTries = INITIAL_DATA_POLL_MAX_RETRIES;
                                        DWORD dwInitialDataTryCount = 0;

                                        do
                                        {
                                            fPollFailed = FALSE;

                                            Trace(TRACE_LEVEL_INFORMATION, "Sending poll request to device to update Datafield Values for %s", pSensor->m_SensorName);

                                            hr = UpdateSensorDataFieldValues(m_pSensorManager->m_AvailableSensorsIDs[sensIndex]);

                                            if (FAILED(hr))
                                            {
                                                Trace(TRACE_LEVEL_ERROR, "Failed to request input report from %s, hr = %!HRESULT!", pSensor->m_SensorName, hr);
                                            }
                                            else
                                            {
                                                const DWORD dwDataPollTimeLimit = DEVICE_POLL_TIMEOUT; //mS

                                                ULONGLONG ullInitialEventTime = GetTickCount64();
                                                ULONGLONG ulCurrentEventTime = 0;
                                                ULONG ulTimePassed = 0;
                                                ULONG ulYieldCount = 0;

                                                do //spin here until dwDataPollTimeLimit has passed;
                                                {
                                                    Yield();
                                                    ulYieldCount++;

                                                    ulCurrentEventTime = GetTickCount64();
                                                    ulTimePassed = (ULONG)(ulCurrentEventTime - ullInitialEventTime); //time passed in milliseconds

                                                } while ((FALSE == pSensor->m_fSensorUpdated) && (ulTimePassed < dwDataPollTimeLimit));

                                                if (ulTimePassed > dwDataPollTimeLimit)
                                                {
                                                    fPollFailed = TRUE;
                                                    dwInitialDataTryCount++;

                                                    //do not fail - instead, fall through and get the latest value from the API
                                                    Trace(TRACE_LEVEL_WARNING, "Failed to receive poll response from %s within %i mS, failed at %i mS and %i yields, hr = %!HRESULT!", pSensor->m_SensorName, dwDataPollTimeLimit, ulTimePassed, ulYieldCount, hr);
                                                }
                                                else
                                                {
                                                    if (TRUE == pSensor->m_fSensorUpdated) 
                                                    {
                                                        pSensor->m_fInitialDataReceived = TRUE;
                                                        Trace(TRACE_LEVEL_INFORMATION, "Received polled datafield update for %s after %i mS, hr = %!HRESULT!", pSensor->m_SensorName, ulTimePassed, hr);
                                                    }
                                                }
                                            }

                                        } while ((FALSE == pSensor->m_fInitialDataReceived) && (dwInitialDataTryCount < dwInitialDataMaxTries));

                                        if (FALSE == pSensor->m_fInitialDataReceived)
                                        {
                                            hr = E_UNEXPECTED;
                                            Trace(TRACE_LEVEL_ERROR, "Failed to receive initial data from %s after %i tries, hr = %!HRESULT!", pSensor->m_SensorName, dwInitialDataTryCount, hr);
                                        }
                                    }
                                }
                                else
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Failed to get pSensor, hr = %!HRESULT!", hr);
                                }
                            }
                            else
                            {
                                hr = E_UNEXPECTED;

                                if (TRUE == wcscmp(L"DEVICE", wszObjectID))
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Device is %ws not a supported sensor, hr = %!HRESULT!", wszObjectID, hr);
                                }
                                else
                                {
                                    //WPD is calling us, so ignore trace
                                }
                            }


                            if (SUCCEEDED(hr))
                            {
                                for (DWORD dwIndex = 0; dwIndex < cKeys; dwIndex++)
                                {
                                    PROPERTYKEY Key = WPD_PROPERTY_NULL;
                                    hr = pKeys->GetAt(dwIndex, &Key);

                                    //TODO - remove and decrement the index to skip the removed key
                                    //pKeys->RemoveAt(dwIndex);

                                    if (hr == S_OK)
                                    {
                                        PROPVARIANT var;
                                        PropVariantInit( &var );

                                        // Preset the property value to 'error not supported'.  The actual value
                                        // will replace this value, if read from the device.
                                        hr = pValues->SetErrorValue(Key, HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED));
                            
                                        if (SUCCEEDED(hr))
                                        {
                                            if (nullptr != pSensor)
                                            {
                                                if (SUCCEEDED(pSensor->GetDataField(Key, &var)))
                                                {
                                                    hr = pValues->SetValue(Key, &var);
                                                }
                                                else
                                                {
                                                    Trace(TRACE_LEVEL_ERROR, "Failed to get datafield %!GUID!-%i value from %s", &Key.fmtid, Key.pid, pSensor->m_SensorName);
                                                }
                                            }
                                            else
                                            {
                                                hr = E_POINTER;
                                                Trace(TRACE_LEVEL_ERROR, "pSensor == NULL before getting datafield %!GUID!-%i value from %s, hr = %!HRESULT!", &Key.fmtid, Key.pid, pSensor->m_SensorName, hr);
                                            }
                                        }
                                        else
                                        {
                                            hr = E_UNEXPECTED;
                                            Trace(TRACE_LEVEL_ERROR, "Failed on idx = %i for SetErrorValue for %!GUID!-%i value from %s, hr = %!HRESULT!", dwIndex, &Key.fmtid, Key.pid, pSensor->m_SensorName, hr);
                                        }

                                        // Free any allocated resources
                                        PropVariantClear( &var );
                                    }
                                    else
                                    {
                                        Trace(TRACE_LEVEL_ERROR, "Failed to update datafield %!GUID!-%i value from %s, hr = %!HRESULT!", &Key.fmtid, Key.pid, pSensor->m_SensorName, hr);
                                    }
                                }

                                if (SUCCEEDED(hr))
                                {
                                    if (FALSE == pSensor->m_fReportingState)
                                    {
                                        if (FALSE == fPollFailed)
                                        {
                                            Trace(TRACE_LEVEL_INFORMATION, "Returned poll-updated datafield values for %s, hr = %!HRESULT!", pSensor->m_SensorName, hr);
                                        }
                                        else
                                        {
                                            Trace(TRACE_LEVEL_INFORMATION, "Returned current datafield values for %s, hr = %!HRESULT!", pSensor->m_SensorName, hr);
                                        }
                                    }
                                    else
                                    {
                                        Trace(TRACE_LEVEL_INFORMATION, "Returned async-updated datafield values for %s, hr = %!HRESULT!", pSensor->m_SensorName, hr);
                                    }
                                }
                                else
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Failed while updating datafield values for %s, hr = %!HRESULT!", pSensor->m_SensorName, hr);
                                }
                            }
                            else
                            {
                                if (nullptr != pSensor)
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Failed while polling datafield values for %s, hr = %!HRESULT!", pSensor->m_SensorName, hr);
                                }
                                else
                                {
                                    Trace(TRACE_LEVEL_ERROR, "pSensor was NULL while polling datafield values, hr = %!HRESULT!", hr);
                                }
                            }
                        }

                        if (sensType == SensorTypeNone)
                        {
                            hr = E_UNEXPECTED;
                            Trace(TRACE_LEVEL_ERROR, "Failed because sensType == SensorTypeNone, hr = %!HRESULT!", hr);
                        }
                    }
                }

                //*ppValues = pValues; //fakes return of this call: hr = GetDataValuesForObject(wszObjectID, pKeys, *ppValues);
            }
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    Trace(TRACE_LEVEL_VERBOSE, "Datafields supported by sensor %ls returned for Client %p, hr = %!HRESULT!", wszObjectID, appId, hr);

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnSetProperties
//
// Routine Description:
//
//  This method is called by Sensor Class Extension to set Sensor properties
//  for a particular Sensor.
//  
// Arguments:
//
//  ObjectID - String that represents the object whose supported property keys 
//             are being requested
// 
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnSetProperties(
        _In_ IWDFFile* appId,
        _In_ LPWSTR ObjectID,
        _In_ IPortableDeviceValues* pValues,
        _Out_ IPortableDeviceValues** ppResults
        )
{
    Trace(TRACE_LEVEL_VERBOSE, "Set Properties on sensor %ls for Client %p", ObjectID, appId);

    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        if ((ObjectID == NULL) ||
            (pValues       == NULL) ||
            (ppResults     == NULL))
        {
            hr = E_INVALIDARG;
        }

        // Update our cache
        if(SUCCEEDED(hr))
        {
            DWORD           cValues     = 0;
            BOOL            fHasError   = FALSE;
            SensorType      sType       = SensorTypeNone;
            DWORD           sIndex      = 0;
            CSensor*        pSensor     = nullptr;

            // CoCreate a collection to store the property set operation results.
            hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                      NULL,
                                      CLSCTX_INPROC_SERVER,
                                      IID_PPV_ARGS(ppResults));

            // Get the sensor type 
            if( hr == S_OK )
            {        
                hr = FindSensorTypeFromObjectID(ObjectID, &sType, &sIndex);
            }

            if (SUCCEEDED(hr) && (sType != SensorTypeNone))
            {
                hr = GetSensorObject(sIndex, &pSensor);
            }
            else
            {
                hr = E_UNEXPECTED;

                if (TRUE == wcscmp(L"DEVICE", ObjectID))
                {
                    Trace(TRACE_LEVEL_ERROR, "Device is %ws not a supported sensor, hr = %!HRESULT!", ObjectID, hr);
                }
                else
                {
                    //WPD is calling us, so ignore trace
                }
            }

            // Set the property values on the specified object
            if ((hr == S_OK) && (nullptr != pSensor))
            {
                Trace(TRACE_LEVEL_INFORMATION, "Client %p Setting Properties for %s", appId, pSensor->m_SensorName);

                hr = pValues->GetCount(&cValues);

                if (hr == S_OK)
                {
                    for (DWORD dwIndex = 0; dwIndex < cValues; dwIndex++)
                    {
                        PROPERTYKEY Key = WPD_PROPERTY_NULL;
                        PROPVARIANT var;

                        PropVariantInit( &var );

                        hr = pValues->GetAt(dwIndex, &Key, &var);

                        if(hr == S_OK)
                        {
                            // Check if this is a supported settable property
                            if(TRUE == IsSettablePropertyKey(sIndex, Key))
                            {

                                FLOAT clientChangeSensitivities[MAX_NUM_DATA_FIELDS] = {0};
                                ULONG clientReportInterval = 0;

                                if (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL))
                                {
                                    //set the report interval for this client in the client report interval list
                                    hr = FindClientFromAppID( pSensor, appId, &clientReportInterval, clientChangeSensitivities);

                                    if (SUCCEEDED(hr))
                                    {
                                        if (VT_UI4 == var.vt)
                                        {
                                            Trace(TRACE_LEVEL_INFORMATION, "Client %p Setting Report Interval for %s to %i", appId, pSensor->m_SensorName, var.uintVal);

                                            pSensor->m_pClientMap[appId].ulClientReportInterval = var.uintVal;

                                            hr = SelectClientReportInterval(pSensor); 
                                        }
                                        else
                                        {
                                            hr = E_INVALIDARG;
                                            Trace(TRACE_LEVEL_ERROR, "var.vt is not == VT_UI4, hr = %!HRESULT!", hr);

                                            hr = (*ppResults)->SetErrorValue(Key, hr);
                                            fHasError = TRUE;
                                        }
                                    }
                                }
                                else if (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_CHANGE_SENSITIVITY))
                                {
                                    PROPERTYKEY pkDfKey = {0};
                                    PROPERTYKEY pkDvKey = {0};
                                    DWORD cDfVals = 0;
                                    DWORD cDfKeys = 0;
                                    PROPVARIANT val;

                                    CComPtr<IPortableDeviceValues> spDfVals = nullptr;

                                    if ((VT_UNKNOWN == var.vt) && (var.punkVal != nullptr))
                                    {
                                        IUnknown* pUnknown = var.punkVal;

                                        hr = pUnknown->QueryInterface(IID_PPV_ARGS(&spDfVals));

                                        if (SUCCEEDED(hr) && (nullptr != spDfVals))
                                        {
                                            hr = spDfVals->GetCount(&cDfVals);
                                        }
                                        else
                                        {
                                            hr = E_UNEXPECTED;
                                            Trace(TRACE_LEVEL_ERROR, "spDfValsSource == nullptr or failed, hr = %!HRESULT!", hr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = pSensor->m_spSupportedSensorDataFields->GetCount(&cDfKeys);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = FindClientFromAppID( pSensor, appId, &clientReportInterval, clientChangeSensitivities);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            for (DWORD dwDvIdx = 0; dwDvIdx < cDfVals; dwDvIdx++)
                                            {
                                                DWORD dwDkIdx = 0;

                                                PropVariantInit( &val );

                                                //get the key/value from the value values
                                                hr = spDfVals->GetAt(dwDvIdx, &pkDvKey, &val);

                                                if (SUCCEEDED(hr))
                                                {
                                                    HRESULT hrError = E_UNEXPECTED;
                                                    if (V_VT(&val) == VT_R4 || V_VT(&val) == VT_R8 || V_VT(&val) == VT_UI4)
                                                    {
                                                        float fltVal;

                                                        if (V_VT(&val) == VT_UI4)
                                                        {
                                                            fltVal = static_cast<float>(V_UI4(&val));
                                                        }
                                                        else
                                                        {
                                                            fltVal = V_VT(&val) == VT_R8 ? static_cast<float>(V_R8(&val)) : V_R4(&val);
                                                        }
                                                        if (fltVal < 0.0F)
                                                        {
                                                            hr = (*ppResults)->SetErrorValue(Key, E_INVALIDARG);
                                                            fHasError = TRUE;
                                                        }
                                                        else
                                                        {
                                                            //find the value key in the supported data fields
                                                            for (dwDkIdx = 0; dwDkIdx < cDfKeys; dwDkIdx++)
                                                            {
                                                                hr = pSensor->m_spSupportedSensorDataFields->GetAt(dwDkIdx, &pkDfKey);

                                                                if (SUCCEEDED(hr))
                                                                {
                                                                    if (IsEqualPropertyKey(pkDfKey, pkDvKey))
                                                                    {
                                                                        Trace(TRACE_LEVEL_INFORMATION, "Client %p Setting datafield %!GUID!-%i Change Sensitivity for %s to %f", appId, &pkDvKey.fmtid, pkDvKey.pid, pSensor->m_SensorName, fltVal);

                                                                        pSensor->m_pClientMap[appId].fltClientChangeSensitivity[dwDkIdx] = fltVal;

                                                                        hrError = S_OK;
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else if (V_VT(&val) == VT_NULL) 
                                                    {
                                                        //find the value key in the supported data fields
                                                        for (dwDkIdx = 0; dwDkIdx < cDfKeys; dwDkIdx++)
                                                        {
                                                            hr = pSensor->m_spSupportedSensorDataFields->GetAt(dwDkIdx, &pkDfKey);

                                                            if (SUCCEEDED(hr))
                                                            {
                                                                if (IsEqualPropertyKey(pkDfKey, pkDvKey))
                                                                {
                                                                    pSensor->m_pClientMap[appId].fltClientChangeSensitivity[dwDkIdx] = CHANGE_SENSITIVITY_NOT_SET;

                                                                    hrError = S_OK;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }

                                                    if (FAILED(hrError))
                                                    {
                                                        hr = E_UNEXPECTED;
                                                        Trace(TRACE_LEVEL_ERROR, "Client %p request to set Property %!GUID!-%i on %s not found or is invalid arg, hr = %!HRESULT!", appId, &pkDfKey.fmtid, pkDfKey.pid, pSensor->m_SensorName, hr);

                                                        hr = (*ppResults)->SetErrorValue(Key, E_INVALIDARG);
                                                        fHasError = TRUE;
                                                    }
                                                }
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = SelectClientChangeSensitivity(pSensor);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        hr = E_UNEXPECTED;
                                        Trace(TRACE_LEVEL_ERROR, "var.vt is not == VT_UNKNOWN, hr = %!HRESULT!", hr);

                                        hr = (*ppResults)->SetErrorValue(Key, E_INVALIDARG);
                                        fHasError = TRUE;
                                    }
                                }
                            }
                            else
                            {
                                hr = E_UNEXPECTED;
                                Trace(TRACE_LEVEL_ERROR, "Property not found, hr = %!HRESULT!", hr);

                                hr = (*ppResults)->SetErrorValue(Key, HRESULT_FROM_WIN32(ERROR_NOT_FOUND));
                                fHasError = TRUE;
                            }
                        }

                        PropVariantClear(&var);
                
                        if( FAILED( hr ) )
                        {
                            break;
                        }
                    }
                }

                // Since we have set failures for the property set operations we must let the application
                // know by returning S_FALSE. This will instruct the application to look at the
                // property set operation results for failure values.
                if( (hr == S_OK) && (TRUE == fHasError) )
                {
                    hr = S_FALSE;
                }
            }
            else
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Failed to get pSensor, hr = %!HRESULT!", hr);
            }

            //return hr;
        }

        // If it returns S_OK, Send the values down to the device
        // S_FALSE return indicates that the property is not supported
        if( S_OK == hr )
        {
            hr = UpdateSensorPropertyValues(ObjectID, TRUE);
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    Trace(TRACE_LEVEL_VERBOSE, "Properties on sensor %ls were set for Client %p, hr = %!HRESULT!", ObjectID, appId, hr);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnClientConnect
//
// Routine Description:
//
//  This method is called by Sensor Class Extension when a client app connects
//  to a Sensor
//  
// Arguments:
//
//  ObjectID - String that represents the object whose supported property keys 
//             are being requested
// 
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnClientConnect(
        _In_ IWDFFile* pClientFile,
        _In_ LPWSTR pwszObjectID
        )
{
CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSectionConnectDisconnect);


    HRESULT     hr      = S_OK;
    SensorType  sType   = SensorTypeNone;
    ULONG       sIndex  = 0;
    CSensor*    pSensor = nullptr;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        if (TRUE == m_fDeviceIdle)
        {
            if (NULL != m_pSensorManager)
            {
                Trace(TRACE_LEVEL_WARNING, "Re-initializing sensor device, hr = %!HRESULT!", hr);
                hr = InitSensorDevice(m_pSensorManager->m_spWdfDevice);
            }
            else
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Unable to re-initialize sensor device, hr = %!HRESULT!", hr);
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = FindSensorTypeFromObjectID(pwszObjectID, &sType, &sIndex);
        }

        if (SUCCEEDED(hr))
        {
            hr = GetSensorObject(sIndex, &pSensor);
        }
        else
        {
            hr = E_UNEXPECTED;

            if (TRUE == wcscmp(L"DEVICE", pwszObjectID))
            {
                Trace(TRACE_LEVEL_ERROR, "Device is %ws not a supported sensor, hr = %!HRESULT!", pwszObjectID, hr);
            }
            else
            {
                //WPD is calling us, so ignore trace
            }
        }

        if (SUCCEEDED(hr) && (nullptr != pSensor))
        {
            Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! for Client %p to %s", pClientFile, pSensor->m_SensorName);

            BOOL fClientIsPresent = FALSE;

            hr = CheckForClient(pSensor, pClientFile, &fClientIsPresent);

            //do not allow client to connect more than one time
            //however, if a client is already connected
            //this must return S_OK so that the class extension
            //believes the request to connect has been honored
            if (SUCCEEDED(hr))
            {
                if (FALSE == fClientIsPresent)
                {
                    size_t cClients = 0;
                    CLIENT_ENTRY entry;

                    entry.ulClientReportInterval = 0; //default
                    for (DWORD idx = 0; idx < MAX_NUM_DATA_FIELDS; idx++)
                    {
                        entry.fltClientChangeSensitivity[idx] = CHANGE_SENSITIVITY_NOT_SET;
                    }

                    pSensor->m_pClientMap[pClientFile] = entry;

                    cClients = pSensor->m_pClientMap.GetCount();

                    Trace(TRACE_LEVEL_INFORMATION, "Client %p connected to %s, Client count now = %i", pClientFile, pSensor->m_SensorName, (ULONG)cClients);

                    if (0 == cClients-1)
                    {
                        DWORD cDfKeys = 0;
                        pSensor->m_spSupportedSensorDataFields->GetCount(&cDfKeys);

                        pSensor->m_ulLowestClientReportInterval = pSensor->m_ulDefaultCurrentReportInterval;

                        for (DWORD dwIdx = 0; dwIdx < MAX_NUM_DATA_FIELDS; dwIdx++)
                        {
                            pSensor->m_fltLowestClientChangeSensitivities[dwIdx] = pSensor->m_fltDefaultChangeSensitivity;
                        }
                    }

                    if (SUCCEEDED(hr))
                    {
                        if(cClients >= 1)
                        {
                            hr = FindSensorTypeFromObjectID(pwszObjectID, &sType, &sIndex);

                            if (SUCCEEDED(hr))
                            {
                                hr = SelectClientReportInterval(pSensor);

                                if (FAILED(hr))
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Failed to select client report interval, hr = %!HRESULT!", hr);
                                }
                            }
                            else
                            {
                                hr = E_UNEXPECTED;

                                if (TRUE == wcscmp(L"DEVICE", pwszObjectID))
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Device is %ws not a supported sensor, hr = %!HRESULT!", pwszObjectID, hr);
                                }
                                else
                                {
                                    //WPD is calling us, so ignore trace
                                }
                            }

                            if (SUCCEEDED(hr))
                            {
                                hr = SelectClientChangeSensitivity(pSensor);

                                if (FAILED(hr))
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Failed to update change sensitivity, hr = %!HRESULT!", hr);
                                }
                            }

                            if(SUCCEEDED(hr))
                            {
                                hr = UpdateSensorPropertyValues(pwszObjectID, TRUE);

                                if (FAILED(hr))
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Failed to update property values, hr = %!HRESULT!", hr);
                                }
                            }

                            // Poll the sensors for initial data values
                            if(SUCCEEDED(hr))
                            {
                                hr = UpdateSensorDataFieldValues(m_pSensorManager->m_AvailableSensorsIDs[sIndex]);

                                if (FAILED(hr))
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Failed to request input report, hr = %!HRESULT!", hr);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Trace(TRACE_LEVEL_ERROR, "Client is already connected, hr = %!HRESULT!", hr);
                }
            }
            else
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Failure while trying to connect client, hr = %!HRESULT!", hr);
            }
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to get pSensor, hr = %!HRESULT!", hr);
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    return hr;
}

HRESULT CSensorDDI:: OnDeviceReconnect( )
{
    Trace(TRACE_LEVEL_INFORMATION, "Reconnect all previously connected clients");
    HRESULT     hr      = S_OK;
    CSensor*    pSensor = nullptr;

    for (DWORD dwSensIdx = 0; dwSensIdx < m_pSensorManager->m_NumMappedSensors; dwSensIdx++)
    {
        if (SUCCEEDED(hr))
        {
            hr = GetSensorObject(dwSensIdx, &pSensor);
        }

        if (SUCCEEDED(hr) && (nullptr != pSensor))
        {
            size_t cClients = pSensor->m_pClientMap.GetCount();

            if(cClients >= 1)
            {
                hr = SelectClientReportInterval(pSensor);

                if (FAILED(hr))
                {
                    Trace(TRACE_LEVEL_ERROR, "Failed to select client report interval, hr = %!HRESULT!", hr);
                }
                else
                {
                    hr = SelectClientChangeSensitivity(pSensor);

                    if (FAILED(hr))
                    {
                        Trace(TRACE_LEVEL_ERROR, "Failed to select change sensitivity, hr = %!HRESULT!", hr);
                    }
                }

                if(SUCCEEDED(hr))
                {
                    hr = UpdateSensorPropertyValues(pSensor->m_SensorID, TRUE);

                    if (FAILED(hr))
                    {
                        Trace(TRACE_LEVEL_ERROR, "Failed to update property values, hr = %!HRESULT!", hr);
                    }
                }

                // Poll the sensors for initial data values
                if(SUCCEEDED(hr))
                {
                    hr = UpdateSensorDataFieldValues(m_pSensorManager->m_AvailableSensorsIDs[dwSensIdx]);

                    if (FAILED(hr))
                    {
                        Trace(TRACE_LEVEL_ERROR, "Failed to request input report, hr = %!HRESULT!", hr);
                    }
                }
            }
        }
    }

    Trace(TRACE_LEVEL_INFORMATION, "All previously-connected clients were reconnected, hr = %!HRESULT!", hr);
    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnClientDisconnect
//
// Routine Description:
//
//  This method is called by Sensor Class Extension when a client app disconnects
//  from a Sensor
//  
// Arguments:
//
//  ObjectID - String that represents the object whose supported property keys 
//             are being requested
// 
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnClientDisconnect(
        _In_ IWDFFile* pClientFile,
        _In_ LPWSTR pwszObjectID
        )
{
CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSectionConnectDisconnect);

    HRESULT     hr      = S_OK;
    SensorType  sType   = SensorTypeNone;
    ULONG       sIndex  = 0;
    CSensor*    pSensor = nullptr;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        CComPtr<IPortableDeviceValues> spReportInterval = NULL;
        hr = CoCreateInstance(CLSID_PortableDeviceValues,
                              NULL,
                              CLSCTX_INPROC_SERVER,
                              IID_PPV_ARGS(&spReportInterval));

        if (SUCCEEDED(hr))
        {
            hr = FindSensorTypeFromObjectID(pwszObjectID, &sType, &sIndex);
        }

        if (SUCCEEDED(hr))
        {
            hr = GetSensorObject(sIndex, &pSensor);
        }
        else
        {
            hr = E_UNEXPECTED;

            if (TRUE == wcscmp(L"DEVICE", pwszObjectID))
            {
                Trace(TRACE_LEVEL_ERROR, "Device is %ws not a supported sensor, hr = %!HRESULT!", pwszObjectID, hr);
            }
            else
            {
                //WPD is calling us, so ignore trace
            }
        }

        if (SUCCEEDED(hr) && (nullptr != pSensor))
        {
            Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! for Client %p from %s", pClientFile, pSensor->m_SensorName);

            BOOL fClientIsPresent = FALSE;
            size_t cClients = pSensor->m_pClientMap.GetCount();
                        
            hr = CheckForClient( pSensor, pClientFile, &fClientIsPresent);

            if (SUCCEEDED(hr) && (TRUE == fClientIsPresent))
            {
                if (SUCCEEDED(hr))
                {
                    if (cClients > 0)
                    {
                        //find this client in the list of clients and remove from the three client lists
                        hr = RemoveClient( pSensor, pClientFile );

                        if (SUCCEEDED(hr))
                        {
                            hr = CheckForClient( pSensor, pClientFile, &fClientIsPresent);

                            if (TRUE == fClientIsPresent)
                            {
                                Trace(TRACE_LEVEL_ERROR, "Failed to remove client %p from %s, hr = %!HRESULT!", pClientFile, pSensor->m_SensorName, hr);
                            }
                            else
                            {
                                Trace(TRACE_LEVEL_INFORMATION, "Client %p successfully removed from %s, Client Count now = %i, hr = %!HRESULT!", pClientFile, pSensor->m_SensorName, (DWORD)cClients-1, hr);
                            }
                        }
                        else
                        {
                            Trace(TRACE_LEVEL_ERROR, "Client %p has already been removed from %s, hr = %!HRESULT!", pClientFile, pSensor->m_SensorName, hr);
                        }

                        cClients = pSensor->m_pClientMap.GetCount();
                        
                        if (SUCCEEDED(hr))
                        {
                            hr = SelectClientReportInterval(pSensor);

                            if (FAILED(hr))
                            {
                                Trace(TRACE_LEVEL_ERROR, "Failed to select client report interval, hr = %!HRESULT!", hr);
                            }
                        }

                        if (SUCCEEDED(hr))
                        {
                            hr = SelectClientChangeSensitivity(pSensor);

                            if (FAILED(hr))
                            {
                                Trace(TRACE_LEVEL_ERROR, "Failed to select change sensitivity, hr = %!HRESULT!", hr);
                            }
                        }

                        if (SUCCEEDED(hr))
                        {
                            if (0 == cClients)
                            {
                                Trace(TRACE_LEVEL_INFORMATION, "All Clients have been removed from %s, hr = %!HRESULT!", pSensor->m_SensorName, hr);
                            }

                            hr = UpdateSensorPropertyValues(pwszObjectID, TRUE);

                            if (FAILED(hr))
                            {
                                Trace(TRACE_LEVEL_ERROR, "Failed to update property values, hr = %!HRESULT!", hr);
                            }
                        }
                    }
                    else
                    {
                        hr = E_UNEXPECTED;
                        Trace(TRACE_LEVEL_ERROR, "Attempt to reduce client count below 0 on %s, AppID %p", pSensor->m_SensorName, pClientFile);
                    }
                }

                if (SUCCEEDED(hr) && (NULL != m_pSensorManager))
                {
                    size_t cSensors = m_pSensorManager->m_pSensorList.GetCount();
                    BOOL fHasClients = FALSE;
                    cClients = 0;

                    // see if any sensor has at least one client
                    for (sIndex = 0; sIndex < cSensors; sIndex++)
                    {
                        pSensor = nullptr;

                        hr = GetSensorObject(sIndex, &pSensor);

                        if (SUCCEEDED(hr) && (nullptr != pSensor))
                        {
                            cClients = pSensor->m_pClientMap.GetCount();

                            if (0 != cClients)
                            {
                                fHasClients = TRUE;
                            }
                        }
                        else
                        {
                            Trace(TRACE_LEVEL_ERROR, "Failed to get sensor object, hr = %!HRESULT!", hr);
                        }
                    }

                    // if no clients, then allow for device disconnect
                    if (SUCCEEDED(hr) && (FALSE == fHasClients))
                    {
                        Trace(TRACE_LEVEL_WARNING, "All clients are now disconnected from all sensors");
                        Trace(TRACE_LEVEL_WARNING, "Releasing the HID device");
                        DeInitSensorDevice();
                    }
                }
            }
            else
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Attempt to remove non-existent client, AppID* %u, Client Count %u", (DWORD)pClientFile, (ULONG)cClients);
            }
        }
        else
        {
            hr = E_POINTER;
            Trace(TRACE_LEVEL_ERROR, "Sensor not found, hr = %!HRESULT!", hr);
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnClientSubscribeToEvents
//
// Routine Description:
//
//  This method is called by Sensor Class Extension when a client subscribes to
//  events by calling SetEventSink
//  
// Arguments:
//
//  ObjectID - String that represents the object whose supported property keys 
//             are being requested
// 
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnClientSubscribeToEvents(
        _In_ IWDFFile* pClientFile,
        _In_ LPWSTR pwszObjectID
        )
{
    UNREFERENCED_PARAMETER(pClientFile);
    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        // Set the Client Status : denotes the status of event subscribed clients
        CSensor* pSensor = nullptr;
        SensorType sType;
        DWORD sIndex;

        hr = FindSensorTypeFromObjectID(pwszObjectID, &sType, &sIndex);

        if (SUCCEEDED(hr))
        {
            hr = GetSensorObject(sIndex, &pSensor);
        }
        else
        {
            hr = E_UNEXPECTED;

            if (TRUE == wcscmp(L"DEVICE", pwszObjectID))
            {
                Trace(TRACE_LEVEL_ERROR, "Device is %ws not a supported sensor, hr = %!HRESULT!", pwszObjectID, hr);
            }
            else
            {
                //WPD is calling us, so ignore trace
            }
        }

        if (SUCCEEDED(hr) && (nullptr != pSensor))
        {
            Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! for Client %p to %s", pClientFile, pSensor->m_SensorName);

            hr = pSensor->Subscribe(pClientFile);

        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to get sensor object, hr = %!HRESULT!", hr);
        }

        if(SUCCEEDED(hr) && (nullptr != pSensor))
        {
            size_t cSubscriberCount = pSensor->m_pSubscriberMap.GetCount();

            Trace(TRACE_LEVEL_INFORMATION, "Client %p Subscribed to %s, Subscriber Count = %i", pClientFile, pSensor->m_SensorName, (DWORD)cSubscriberCount);

            if (cSubscriberCount == 1)
            {
                pSensor->m_ulEventCount = 0;
            }

            hr = SelectClientReportInterval(pSensor);

            if (FAILED(hr))
            {
                Trace(TRACE_LEVEL_ERROR, "Failed to select client report interval, hr = %!HRESULT!", hr);
            }
            else
            {
                hr = UpdateSensorPropertyValues(pwszObjectID, TRUE);

                if (FAILED(hr))
                {
                    Trace(TRACE_LEVEL_ERROR, "Failed to update property values, hr = %!HRESULT!", hr);
                }
            }
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to subscribe to sensor events, hr = %!HRESULT!", hr);
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnClientUnsubscribeFromEvents
//
// Routine Description:
//
//  This method is called by Sensor Class Extension when a client unsubscribes from
//  events by calling SetEventSink(NULL)
//  
// Arguments:
//
//  ObjectID - String that represents the object whose supported property keys 
//             are being requested
// 
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnClientUnsubscribeFromEvents(
        _In_ IWDFFile* pClientFile,
        _In_ LPWSTR pwszObjectID
        )
{

    UNREFERENCED_PARAMETER(pClientFile);
    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        // Set the Client Status : denotes the status of event subscribed clients
        CSensor* pSensor = nullptr;
        SensorType sType;
        DWORD sIndex;

        hr = FindSensorTypeFromObjectID(pwszObjectID, &sType, &sIndex);

        if (SUCCEEDED(hr))
        {
            hr = GetSensorObject(sIndex, &pSensor);
        }
        else
        {
            hr = E_UNEXPECTED;

            if (TRUE == wcscmp(L"DEVICE", pwszObjectID))
            {
                Trace(TRACE_LEVEL_ERROR, "Device is %ws not a supported sensor, hr = %!HRESULT!", pwszObjectID, hr);
            }
            else
            {
                //WPD is calling us, so ignore trace
            }
        }

        if (SUCCEEDED(hr) && (nullptr != pSensor))
        {
            Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! for Client %p from %s", pClientFile, pSensor->m_SensorName);

            hr = pSensor->Unsubscribe(pClientFile);

        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to get sensor object, hr = %!HRESULT!", hr);
        }

        if(SUCCEEDED(hr))
        {
            size_t cSubscriberCount = pSensor->m_pSubscriberMap.GetCount();

            Trace(TRACE_LEVEL_INFORMATION, "Subscriber %p Removed from %s, Subscriber Count = %i", pClientFile, pSensor->m_SensorName, (DWORD)cSubscriberCount);

            if (0 == cSubscriberCount)
            {
                Trace(TRACE_LEVEL_INFORMATION, "All Subscribers removed from %s", pSensor->m_SensorName);
            }

            hr = SelectClientReportInterval(pSensor);

            if (FAILED(hr))
            {
                Trace(TRACE_LEVEL_ERROR, "Failed to select client report interval, hr = %!HRESULT!", hr);
            }
            else
            {
                hr = UpdateSensorPropertyValues(pwszObjectID, TRUE);

                if (FAILED(hr))
                {
                    Trace(TRACE_LEVEL_ERROR, "Failed to update property values, hr = %!HRESULT!", hr);
                }
            }
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failure during attempt to unsubscribe from events, hr = %!HRESULT!", hr);
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnProcessWpdMessage
//
// Routine Description:
//
//  
// Arguments:
//
// 
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnProcessWpdMessage(
        _In_ IUnknown* pPortableDeviceValuesParamsUnknown,
        _In_ IUnknown* pPortableDeviceValuesResultsUnknown
        )
{
    UNREFERENCED_PARAMETER(pPortableDeviceValuesParamsUnknown);
    UNREFERENCED_PARAMETER(pPortableDeviceValuesResultsUnknown);

    HRESULT hr = S_OK;

    return hr;
}

//Asynchronous read call back function
void CSensorDDI:: OnAsyncReadCallback( BYTE* originalBuffer, ULONG originalLength )
{

    UNREFERENCED_PARAMETER(originalLength);

    if (TRUE == m_fReleasingDevice)
    {
        return;
    }

    HRESULT             hr = S_OK;
    CSensor*            pSensor = NULL;  
    SensorType          sensType = SensorTypeNone;

    CAccelerometer*     pAccelerometer = NULL;
    CAmbientLight*      pAmbientLight = NULL;
    CPresence*          pPresence = NULL;
    CCompass*           pCompass = NULL;
    CInclinometer*      pInclinometer = NULL;
    CGyrometer*         pGyrometer = NULL;
    CBarometer*         pBarometer = NULL;
    CHygrometer*        pHygrometer = NULL;
    CThermometer*       pThermometer = NULL;
    CPotentiometer*     pPotentiometer = NULL;
    CDistance*          pDistance = NULL;
    CSwitch*            pSwitch = NULL;
    CVoltage*           pVoltage = NULL;
    CCurrent*           pCurrent = NULL;
    CPower*             pPower = NULL;
    CFrequency*         pFrequency = NULL;
    COrientation*       pOrientation = NULL;
    CCustom*            pCustom = NULL;
    CGeneric*           pGeneric = NULL;
    CUnsupported*       pUnsupported = NULL;

    int                 sensNum   = -1;
    int                 sensCount = 0;


    if (nullptr != m_pSensorManager)
    {
        if (TRUE == m_pSensorManager->m_fInitializationComplete)
        {
            if (FALSE == m_pSensorManager->m_pSensorList.IsEmpty())
            {
                BYTE * buffer = originalBuffer;
                ULONG  length = originalLength;
                ULONG  count = originalLength / m_pSensorManager->m_HidCaps.InputReportByteLength;
                DWORD  idx = 0;
                DWORD  offset = 0;

                //This for loop covers the case where more than 1 input report is queued in the same buffer
                for (idx = 0; idx < count; idx++)
                {
                    if (count > 1)
                    {
                        length = m_pSensorManager->m_HidCaps.InputReportByteLength;
                        offset = idx * length;
                        buffer = originalBuffer + offset;
                        Trace(TRACE_LEVEL_INFORMATION, "Multiple Input reports are queued, processing %i of %i", idx+1, count);
                    }

                    sensCount = (int)m_pSensorManager->m_pSensorList.GetCount();
                    if ((sensCount > 1) && (originalBuffer[offset] > 0) && ((originalBuffer[offset] <= sensCount)))
                    {
                        sensNum = originalBuffer[offset]-1;
                    }
                    else
                    {
                        if (sensCount == 1)
                            sensNum = 0;
                    }

                    if ((sensNum > sensCount-1) || (sensNum < 0))
                    {
                        hr = E_UNEXPECTED;
                        Trace(TRACE_LEVEL_ERROR, "Invalid sensor number, hr = %!HRESULT!", hr);
                    }

                    if (SUCCEEDED(hr))
                    {
                        sensCount = (int)m_pSensorManager->m_pSensorList.GetCount();
                        if (sensCount > 0)
                        {
                            sensType = (SensorType)m_pSensorManager->m_AvailableSensorsTypes[(ULONG)sensNum];

                            // during initialization, a condition can occur where there is a sensNum assigned
                            // but the corresponding sensType has not yet been assigned
                            // this if screens for that
                            if ((sensType >= FirstSensorType) || (sensType <= LastSensorType))
                            {
                                // during initialization a condition can occur where there is a sensNum assigned
                                // but the correstponding Sensor object has not been created on g_pSensorList
                                // this if screens for that
                                if ((sensNum == 0) || (m_pSensorManager->m_fInitializationComplete)) // always let the base device through
                                {
                                    hr = GetSensorObject(sensNum, &pSensor);
                                }
                                if (SUCCEEDED(hr) && (NULL != pSensor) && (pSensor->IsInitialized()))
                                {
                                    Trace(TRACE_LEVEL_VERBOSE, "%!FUNC! for %s, length = %02i, 0x[%02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x ...]", 
                                        pSensor->m_SensorName, length,
                                        buffer[0], buffer[1], buffer[2], buffer[3],
                                        buffer[4], buffer[5], buffer[6], buffer[7],
                                        buffer[8], buffer[9], buffer[10], buffer[11],
                                        buffer[12], buffer[13], buffer[14], buffer[15]); 

                                    pSensor->m_ulEventCount++;

                                    switch (sensType)
                                    {
                                    case Accelerometer:
                                        pAccelerometer = (CAccelerometer*) pSensor;
                                        if (( NULL != pAccelerometer ) && (SUCCEEDED(hr)))
                                        {
                                            hr = pAccelerometer->ProcessAccelerometerAsyncRead(buffer, length);
                                        }
                                        break;

                                    case AmbientLight:
                                        pAmbientLight = (CAmbientLight*) pSensor;
                                        if (( NULL != pAmbientLight ) && (SUCCEEDED(hr)))
                                        {
                                            hr = pAmbientLight->ProcessAmbientLightAsyncRead(buffer, length);
                                        }
                                        break;

                                    case Presence:
                                        pPresence = (CPresence*) pSensor;
                                        if (( NULL != pPresence ) && (SUCCEEDED(hr)))
                                        {
                                            hr = pPresence->ProcessPresenceAsyncRead(buffer, length);
                                        }
                                        break;

                                    case Compass:
                                        pCompass = (CCompass*) pSensor;
                                        if (( NULL != pCompass ) && (SUCCEEDED(hr)))
                                        {
                                            hr = pCompass->ProcessCompassAsyncRead(buffer, length);
                                        }
                                        break;

                                    case Inclinometer:
                                        pInclinometer = (CInclinometer*) pSensor;
                                        if (( NULL != pInclinometer ) && (SUCCEEDED(hr)))
                                        {
                                            hr = pInclinometer->ProcessInclinometerAsyncRead(buffer, length);
                                        }
                                        break;

                                    case Gyrometer:
                                        pGyrometer = (CGyrometer*) pSensor;
                                        if (( NULL != pGyrometer ) && (SUCCEEDED(hr)))
                                        {
                                            hr = pGyrometer->ProcessGyrometerAsyncRead(buffer, length);
                                        }
                                        break;

                                    case Barometer:
                                        pBarometer = (CBarometer*) pSensor;
                                        if (( NULL != pBarometer ) && (SUCCEEDED(hr)))
                                        {
                                            hr = pBarometer->ProcessBarometerAsyncRead(buffer, length);
                                        }
                                        break;

                                    case Hygrometer:
                                        pHygrometer = (CHygrometer*) pSensor;
                                        if (( NULL != pHygrometer ) && (SUCCEEDED(hr)))
                                        {
                                            hr = pHygrometer->ProcessHygrometerAsyncRead(buffer, length);
                                        }
                                        break;

                                    case Thermometer:
                                        pThermometer = (CThermometer*) pSensor;
                                        if (( NULL != pThermometer ) && (SUCCEEDED(hr)))
                                        {
                                            hr = pThermometer->ProcessThermometerAsyncRead(buffer, length);
                                        }
                                        break;

                                    case Potentiometer:
                                        pPotentiometer = (CPotentiometer*) pSensor;
                                        if (( NULL != pPotentiometer ) && (SUCCEEDED(hr)))
                                        {
                                            hr = pPotentiometer->ProcessPotentiometerAsyncRead(buffer, length);
                                        }
                                        break;

                                    case Distance:
                                        pDistance = (CDistance*) pSensor;
                                        if (( NULL != pDistance ) && (SUCCEEDED(hr)))
                                        {
                                            hr = pDistance->ProcessDistanceAsyncRead(buffer, length);
                                        }
                                        break;

                                    case Switch:
                                        pSwitch = (CSwitch*) pSensor;
                                        if (( NULL != pSwitch ) && (SUCCEEDED(hr)))
                                        {
                                            hr = pSwitch->ProcessSwitchAsyncRead(buffer, length);
                                        }
                                        break;

                                    case Voltage:
                                        pVoltage = (CVoltage*) pSensor;
                                        if (( NULL != pVoltage ) && (SUCCEEDED(hr)))
                                        {
                                            hr = pVoltage->ProcessVoltageAsyncRead(buffer, length);
                                        }
                                        break;

                                    case Current:
                                        pCurrent = (CCurrent*) pSensor;
                                        if (( NULL != pCurrent ) && (SUCCEEDED(hr)))
                                        {
                                            hr = pCurrent->ProcessCurrentAsyncRead(buffer, length);
                                        }
                                        break;

                                    case Power:
                                        pPower = (CPower*) pSensor;
                                        if (( NULL != pPower ) && (SUCCEEDED(hr)))
                                        {
                                            hr = pPower->ProcessPowerAsyncRead(buffer, length);
                                        }
                                        break;

                                    case Frequency:
                                        pFrequency = (CFrequency*) pSensor;
                                        if (( NULL != pFrequency ) && (SUCCEEDED(hr)))
                                        {
                                            hr = pFrequency->ProcessFrequencyAsyncRead(buffer, length);
                                        }
                                        break;

                                    case Orientation:
                                        pOrientation = (COrientation*) pSensor;
                                        if (( NULL != pOrientation ) && (SUCCEEDED(hr)))
                                        {
                                            hr = pOrientation->ProcessOrientationAsyncRead(buffer, length);
                                        }
                                        break;

                                    case Custom:
                                        pCustom = (CCustom*) pSensor;
                                        if (( NULL != pCustom ) && (SUCCEEDED(hr)))
                                        {
                                            hr = pCustom->ProcessCustomAsyncRead(buffer, length);
                                        }
                                        break;

                                    case Generic:
                                        pGeneric = (CGeneric*) pSensor;
                                        if (( NULL != pGeneric ) && (SUCCEEDED(hr)))
                                        {
                                            hr = pGeneric->ProcessGenericAsyncRead(buffer, length);
                                        }
                                        break;

                                    case Unsupported:
                                        pUnsupported = (CUnsupported*) pSensor;
                                        if (( NULL != pUnsupported ) && (SUCCEEDED(hr)))
                                        {
                                            hr = pUnsupported->ProcessUnsupportedAsyncRead(buffer, length);
                                        }
                                        break;

                                    default:
                                        hr = E_UNEXPECTED;
                                        Trace(TRACE_LEVEL_ERROR, "Failed to find corresponding sensor for AsyncRead processing, hr = %!HRESULT!", hr);
                                        break;
                                    }

                                    if (SUCCEEDED(hr))
                                    {
                                        //
                                        // Entering the critical section to make it thread-safe and also sync with OnGetDataFields().
                                        // We're using m_CriticalSectionSensorUpdate instead of m_CriticalSection to avoid 
                                        // the dead-lock between async read completion and sensor de-initialization.
                                        //
                                        CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSectionSensorUpdate);

                                        pSensor->m_fInitialDataReceived = TRUE;

                                        if (sensNum == m_nNotifyOnSensorUpdate-1)
                                        {
                                            pSensor->m_fSensorUpdated = TRUE;
                                            m_nNotifyOnSensorUpdate = SensorTypeNone; 
                                        }
                                    }

                                }
                                else
                                {
                                    hr = E_UNEXPECTED;
                                    Trace(TRACE_LEVEL_ERROR, "Sensor object not initialized in AsyncRead processing, hr = %!HRESULT!", hr);
                                }
                            }
                            else
                            {
                                hr = E_UNEXPECTED;
                                Trace(TRACE_LEVEL_ERROR, "Undefined sensor type in AsyncRead processing, hr = %!HRESULT!", hr);
                            }
                        }
                    }
                }
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        //to prevent OACR complaint as this method returns void
    }


    return;
}
/////////////////////////////////////////////////////////////////////////
//
//  This method is called to populate property values for the object specified.
//
//  The parameters sent to us are:
//  wszObjectID - the object whose properties are being requested.
//  pKeys - the list of property keys of the properties to request from the object
//  pValues - an IPortableDeviceValues which will contain the property values retreived from the object
//
//  The driver should:
//  Read the specified properties for the specified object and populate pValues with the
//  results.
//
//  The driver should:
//  - Return all requested property values in WPD_PROPERTY_OBJECT_PROPERTIES_PROPERTY_VALUES.  If any property read failed, the corresponding value should be
//      set to type VT_ERROR with the 'scode' member holding the HRESULT reason for the failure.
//  - S_OK should be returned if all properties were read successfully.
//  - S_FALSE should be returned if any property read failed.
//  - Any error return indicates that the driver did not fill in any results, and the caller will
//      not attempt to unpack any property values.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::OnGetPropertyValues(
        _In_ IWDFFile* appId,
        _In_ LPWSTR  wszObjectID,
        _In_ IPortableDeviceKeyCollection* pKeys,
        _Out_ IPortableDeviceValues** ppValues )
{
    HRESULT hr          = S_OK;


    // CoCreate a collection to store the property values.
    hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                NULL,
                                CLSCTX_INPROC_SERVER,
                                IID_PPV_ARGS(ppValues));

    // Read the specified properties on the specified object and add the property values to the collection.
    if (hr == S_OK && ppValues != NULL)
    {
        DWORD       cKeys       = 0;
        BOOL        fError      = FALSE;
        IPortableDeviceValues*  pValues = *ppValues; //Fakes this call: hr = GetPropertyValuesForObject(wszObjectID, pKeys, *ppValues);

        CSensor*            pSensor = NULL;  
        SensorType          sensType = SensorTypeNone;
        DWORD               sensIndex = 0;

        CAccelerometer*     pAccelerometer = NULL;
        CAmbientLight*      pAmbientLight = NULL;
        CPresence*          pPresence = NULL;
        CCompass*           pCompass = NULL;
        CInclinometer*      pInclinometer = NULL;
        CGyrometer*         pGyrometer = NULL;
        CBarometer*         pBarometer = NULL;
        CHygrometer*        pHygrometer = NULL;
        CThermometer*       pThermometer = NULL;
        CPotentiometer*     pPotentiometer = NULL;
        CDistance*          pDistance = NULL;
        CSwitch*            pSwitch = NULL;
        CVoltage*           pVoltage = NULL;
        CCurrent*           pCurrent = NULL;
        CPower*             pPower = NULL;
        CFrequency*         pFrequency = NULL;
        COrientation*       pOrientation = NULL;
        CCustom*            pCustom = NULL;
        CGeneric*           pGeneric = NULL;
        CUnsupported*       pUnsupported = NULL;

        if ((wszObjectID == NULL) ||
            (pKeys       == NULL) ||
            (pValues     == NULL))
        {
            hr = E_INVALIDARG;
        }

        if (SUCCEEDED(hr))
        {
            hr = pKeys->GetCount(&cKeys);

            if (hr == S_OK)
            {
                hr = FindSensorTypeFromObjectID((LPWSTR)wszObjectID, &sensType, &sensIndex);

                if (SUCCEEDED(hr))
                {
                    hr = GetSensorObject(sensIndex, &pSensor);

                    if ((NULL != pSensor) && (SUCCEEDED(hr)))
                    {
                        //depending on where OnGetPropertyValues() is being called from, appId may not be valid
                        if (nullptr != appId)
                        {
                            Trace(TRACE_LEVEL_INFORMATION, "Client %p Requesting Property Values for %s", appId, pSensor->m_SensorName);
                        }
                        else
                        {
                            Trace(TRACE_LEVEL_VERBOSE, "Sensor %ls is being asked for Property Values for %s", wszObjectID, pSensor->m_SensorName);
                        }

                        switch (sensType)
                        {
                        case Accelerometer:
                            pAccelerometer = (CAccelerometer*) pSensor;
                            hr = pAccelerometer->GetPropertyValuesForAccelerometerObject(wszObjectID, pKeys, pValues);
                            break;
                        case AmbientLight:
                            pAmbientLight = (CAmbientLight*) pSensor;
                            hr = pAmbientLight->GetPropertyValuesForAmbientLightObject(wszObjectID, pKeys, pValues);
                            break;
                        case Presence:
                            pPresence = (CPresence*) pSensor;
                            hr = pPresence->GetPropertyValuesForPresenceObject(wszObjectID, pKeys, pValues);
                            break;
                        case Compass:
                            pCompass = (CCompass*) pSensor;
                            hr = pCompass->GetPropertyValuesForCompassObject(wszObjectID, pKeys, pValues);
                            break;
                        case Inclinometer:
                            pInclinometer = (CInclinometer*) pSensor;
                            hr = pInclinometer->GetPropertyValuesForInclinometerObject(wszObjectID, pKeys, pValues);
                            break;
                        case Gyrometer:
                            pGyrometer = (CGyrometer*) pSensor;
                            hr = pGyrometer->GetPropertyValuesForGyrometerObject(wszObjectID, pKeys, pValues);
                            break;
                        case Barometer:
                            pBarometer = (CBarometer*) pSensor;
                            hr = pBarometer->GetPropertyValuesForBarometerObject(wszObjectID, pKeys, pValues);
                            break;
                        case Hygrometer:
                            pHygrometer = (CHygrometer*) pSensor;
                            hr = pHygrometer->GetPropertyValuesForHygrometerObject(wszObjectID, pKeys, pValues);
                            break;
                        case Thermometer:
                            pThermometer = (CThermometer*) pSensor;
                            hr = pThermometer->GetPropertyValuesForThermometerObject(wszObjectID, pKeys, pValues);
                            break;
                        case Potentiometer:
                            pPotentiometer = (CPotentiometer*) pSensor;
                            hr = pPotentiometer->GetPropertyValuesForPotentiometerObject(wszObjectID, pKeys, pValues);
                            break;
                        case Distance:
                            pDistance = (CDistance*) pSensor;
                            hr = pDistance->GetPropertyValuesForDistanceObject(wszObjectID, pKeys, pValues);
                            break;
                        case Switch:
                            pSwitch = (CSwitch*) pSensor;
                            hr = pSwitch->GetPropertyValuesForSwitchObject(wszObjectID, pKeys, pValues);
                            break;
                        case Voltage:
                            pVoltage = (CVoltage*) pSensor;
                            hr = pVoltage->GetPropertyValuesForVoltageObject(wszObjectID, pKeys, pValues);
                            break;
                        case Current:
                            pCurrent = (CCurrent*) pSensor;
                            hr = pCurrent->GetPropertyValuesForCurrentObject(wszObjectID, pKeys, pValues);
                            break;
                        case Power:
                            pPower = (CPower*) pSensor;
                            hr = pPower->GetPropertyValuesForPowerObject(wszObjectID, pKeys, pValues);
                            break;
                        case Frequency:
                            pFrequency = (CFrequency*) pSensor;
                            hr = pFrequency->GetPropertyValuesForFrequencyObject(wszObjectID, pKeys, pValues);
                            break;
                        case Orientation:
                            pOrientation = (COrientation*) pSensor;
                            hr = pOrientation->GetPropertyValuesForOrientationObject(wszObjectID, pKeys, pValues);
                            break;
                        case Custom:
                            pCustom = (CCustom*) pSensor;
                            hr = pCustom->GetPropertyValuesForCustomObject(wszObjectID, pKeys, pValues);
                            break;
                        case Generic:
                            pGeneric = (CGeneric*) pSensor;
                            hr = pGeneric->GetPropertyValuesForGenericObject(wszObjectID, pKeys, pValues);
                            break;
                        case Unsupported:
                            pUnsupported = (CUnsupported*) pSensor;
                            hr = pUnsupported->GetPropertyValuesForUnsupportedObject(wszObjectID, pKeys, pValues);
                            break;

                        default:
                            break;
                        }
                    }
                    else
                    {
                        hr = E_UNEXPECTED;
                        Trace(TRACE_LEVEL_ERROR, "Failed to get sensor object, hr = %!HRESULT!", hr);
                    }
                }
                else
                {
                    hr = E_UNEXPECTED;

                    if (TRUE == wcscmp(L"DEVICE", wszObjectID))
                    {
                        Trace(TRACE_LEVEL_ERROR, "Device is %ws not a supported sensor, hr = %!HRESULT!", wszObjectID, hr);
                    }
                    else
                    {
                        //WPD is calling us, so ignore trace
                    }
                }

            }
        }

        hr = (FALSE == fError) ? hr : S_FALSE;

        *ppValues = pValues; //Fakes return from this call: hr = GetPropertyValuesForObject(wszObjectID, pKeys, *ppValues);

    }


    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to copy PROPERTYKEYs from one collection to the other.
//
//  The parameters sent to us are:
//  pSourceKeys - The source collection
//  pTargetKeys - The target collection
//
//  The driver should:
//  Copy PROPERTYKEYs from the source collection to the target.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::CopyPropertyKeys(
    _In_ IPortableDeviceKeyCollection *pSourceKeys,
    _In_ IPortableDeviceKeyCollection *pTargetKeys)
{
    HRESULT hr = S_OK;
    DWORD cKeys = 0;
    PROPERTYKEY key = {0};

    if (NULL != pSourceKeys && NULL != pTargetKeys)
    {
        hr = pSourceKeys->GetCount(&cKeys);
        
        if (SUCCEEDED(hr))
        {
            for (DWORD dwIndex = 0; dwIndex < cKeys; ++dwIndex)
            {
                hr = pSourceKeys->GetAt(dwIndex, &key);
                
                if (SUCCEEDED(hr))
                {
                    hr = pTargetKeys->Add(key);
                }
            }
        }
    }
    else
    {
        hr = E_POINTER;
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to determine the client index and
//  the client-desired settings for settable properties.
//
//  The parameters sent to us are:
//
//  The driver should:
//  Return the client index, the client change sensitivity and the client report interval.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::FindClientFromAppID(_In_ CSensor* pSensor,
                               _In_ IWDFFile* appID, 
                               _Out_ ULONG* pClientReportInterval, 
                               _Inout_updates_(MAX_NUM_DATA_FIELDS) FLOAT* pClientChangeSensitivities)
{
    HRESULT     hr = E_POINTER;

    if ( (nullptr != pSensor) &&
         (nullptr != pClientReportInterval) &&
         (nullptr != pClientChangeSensitivities) 
         )
    {
        DWORD cDfKeys = 0; 

        hr = pSensor->m_spSupportedSensorDataFields->GetCount(&cDfKeys);

        if (SUCCEEDED(hr))
        {
            hr = HRESULT_FROM_WIN32(ERROR_NOT_FOUND);

            if (cDfKeys > 0)
            {
                CLIENT_ENTRY entry;

                if (pSensor->m_pClientMap.Lookup(appID, entry) == FALSE)
                {
                    Trace(TRACE_LEVEL_ERROR, "Failed to find client %p in %s, hr = %!HRESULT!", appID, pSensor->m_SensorName, hr);
                }
                else
                {
                    *pClientReportInterval = pSensor->m_pClientMap[appID].ulClientReportInterval;
                    for (DWORD dwDataField = 0; dwDataField < MAX_NUM_DATA_FIELDS; dwDataField++)
                    {
                        pClientChangeSensitivities[dwDataField] = pSensor->m_pClientMap[appID].fltClientChangeSensitivity[dwDataField];
                    }

                    hr = S_OK;
                }
            }
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to check for a connected client.
//
//  The parameters sent to us are:
//
//  The driver should:
//  Return a BOOL indicating whether this client is connected.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::CheckForClient(_In_ CSensor* pSensor, _In_ IWDFFile* appID, _Out_ BOOL* pfClientPresent )
{
    HRESULT hr = S_OK;
    
    *pfClientPresent = FALSE;

    if (nullptr != pSensor)
    {
        CLIENT_ENTRY entry; 

        if (pSensor->m_pClientMap.Lookup(appID, entry) == FALSE)
        {
            //Trace(TRACE_LEVEL_INFORMATION, "Client %p was not found in the %s client list, %!HRESULT!", appID, pSensor->m_SensorName, hr);
        }
        else
        {
            *pfClientPresent = TRUE;
            //Trace(TRACE_LEVEL_INFORMATION, "Client %p was found in the %s client list, %!HRESULT!", appID, pSensor->m_SensorName, hr);
        }
    }
    else
    {
        hr = E_POINTER;
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to remove a connected client.
//
//  The parameters sent to us are:
//
//  The driver should:
//  Return the client index.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::RemoveClient(_In_ CSensor* pSensor, _In_ IWDFFile* appID )
{
    HRESULT hr = S_OK;
    
    if (nullptr != pSensor)
    {

        if (pSensor->m_pClientMap.RemoveKey(appID) == FALSE)
        {
            //Entry is not in client list or could not be removed
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Client %p not found in the %s client list, hr = %!HRESULT!", appID, pSensor->m_SensorName, hr);
        }
    }
    else
    {
        hr = E_POINTER;
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to check for a subscribed client.
//
//  The parameters sent to us are:
//
//  The driver should:
//  Return a BOOL indicating whether this client is subscribed.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::CheckForSubscriber(_In_ CSensor* pSensor, _In_ IWDFFile* appID, _Out_ BOOL* pfClientIsSubscribed )
{
    HRESULT hr = S_OK;
    
    *pfClientIsSubscribed = FALSE;

    if (nullptr != pSensor)
    {
        SUBSCRIBER_ENTRY entry; 

        if (pSensor->m_pSubscriberMap.Lookup(appID, entry) == FALSE)
        {
            //Trace(TRACE_LEVEL_INFORMATION, "Client %p was not found in the %s subscriber list, %!HRESULT!", appID, pSensor->m_SensorName, hr);
        }
        else
        {
            *pfClientIsSubscribed = pSensor->m_pSubscriberMap[appID].fSubscribed;
            //Trace(TRACE_LEVEL_INFORMATION, "Client %p was found in the %s subscriber list, %!HRESULT!", appID, pSensor->m_SensorName, hr);
        }
    }
    else
    {
        hr = E_POINTER;
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to remove a subscribed client.
//
//  The parameters sent to us are:
//
//  The driver should:
//  Return the client index
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::RemoveSubscriber(_In_ CSensor* pSensor, _In_ IWDFFile* appID )
{
    HRESULT hr = S_OK;
    
    if (nullptr != pSensor)
    {

        if (pSensor->m_pSubscriberMap.RemoveKey(appID) == FALSE)
        {
            //Entry is not in client list or could not be removed
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Client %p not found in the %s subscriber list, hr = %!HRESULT!", appID, pSensor->m_SensorName, hr);
        }
    }
    else
    {
        hr = E_POINTER;
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to determine the sensor type and
//  index from the object ID.
//
//  The parameters sent to us are:
//  Key - A PROPERTYKEY
//
//  The driver should:
//  Return the sensor type and the sensor index.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::FindSensorTypeFromObjectID(_In_ LPWSTR pwszObjectID, 
                                   _Out_ SensorType* pSensorType,
                                   _Out_ DWORD* pSensorIdx)
{
    HRESULT hr = E_POINTER;
    CAtlStringW strObjectID = pwszObjectID;
    CComBSTR objectId;

    if ( (nullptr != pSensorType) &&
         (nullptr != pSensorIdx) )
    {
        if (nullptr != m_pSensorManager)
        {
            DWORD cSensorIDs = (DWORD)m_pSensorManager->m_AvailableSensorsIDs.GetCount();
            DWORD cSensorTypes = (DWORD)m_pSensorManager->m_AvailableSensorsTypes.GetCount();

            if ((cSensorIDs < 1) || (cSensorTypes < 1))
            {
                hr = E_UNEXPECTED;
            }
            else
            {
                hr = HRESULT_FROM_WIN32(ERROR_NOT_FOUND);

                POSITION posID = m_pSensorManager->m_AvailableSensorsIDs.GetStartPosition();
                POSITION posType = m_pSensorManager->m_AvailableSensorsTypes.GetStartPosition();
                SensorType sensType = SensorTypeNone;
                DWORD idx = 0;

                while(NULL != posID)
                {
                    objectId = m_pSensorManager->m_AvailableSensorsIDs.GetNextValue(posID);
                    sensType = m_pSensorManager->m_AvailableSensorsTypes.GetNextValue(posType);

                    if (strObjectID.CompareNoCase(objectId) == 0)
                    {
                        *pSensorType = sensType;
                        *pSensorIdx = idx;
                        hr = S_OK;

                        break;
                    }

                    idx++;
                }

                if (SUCCEEDED(hr))
                {
                    if (sensType == SensorTypeNone)
                    {
                        hr = E_UNEXPECTED;
                    }
                }
            }
        }
        else
        {
            hr = E_UNEXPECTED;
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to determine if the given PROPERTYKEY is a settable
//  property of the SENSOR object.
//
//  The parameters sent to us are:
//  Key - A PROPERTYKEY
//
//  The driver should:
//  Return TRUE if the PROPERTYKEY is supported; FALSE otherwise.
//
/////////////////////////////////////////////////////////////////////////
BOOL CSensorDDI::IsSettablePropertyKey(_In_ DWORD sIndex, _In_ PROPERTYKEY Key)
{
    HRESULT hr = S_OK;
    CSensor*   pSensor = nullptr;

    CComPtr<IPortableDeviceKeyCollection> spSensorProperties;
    
    BOOL fResult = FALSE;

    hr = GetSensorObject(sIndex, &pSensor);

    if (SUCCEEDED(hr) && (nullptr != pSensor))
    {
        pSensor->GetSettableProperties(&spSensorProperties);
    }
    else
    {
        hr = E_UNEXPECTED;
        Trace(TRACE_LEVEL_ERROR, "Failed to get sensor object, hr = %!HRESULT!", hr);
    }

    if (SUCCEEDED(hr))
    {
        fResult = FindPropertyKey(Key, spSensorProperties);
    }

    return fResult;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to determine if the given PROPERTYKEY can be
//  found in the given collection
//
//  The parameters sent to us are:
//  Key - A PROPERTYKEY
//  pKeys - The collection to search in.
//
//  The driver should:
//  Return TRUE if the PROPERTYKEY is found; FALSE otherwise.
//
/////////////////////////////////////////////////////////////////////////
BOOL CSensorDDI::FindPropertyKey(_In_ PROPERTYKEY Key, _In_ IPortableDeviceKeyCollection *pKeys)
{
    HRESULT hr = S_OK;

    DWORD cKeys = 0;
    PROPERTYKEY tempKey;
    BOOL fResult = FALSE;

    if (NULL != pKeys)
    {
        hr = pKeys->GetCount(&cKeys);
        
        if (SUCCEEDED(hr))
        {
            for (DWORD dwIndex = 0; dwIndex < cKeys; ++dwIndex)
            {
                hr = pKeys->GetAt(dwIndex, &tempKey);
                
                if (SUCCEEDED(hr))
                {
                    if (IsEqualPropertyKey(tempKey, Key))
                    {
                        fResult = TRUE;
                        break;
                    }
                }
            }
        }
    }

    return fResult;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to populate common PROPERTYKEYs found on ALL objects.
//
//  The parameters sent to us are:
//  pKeys - An IPortableDeviceKeyCollection to be populated with PROPERTYKEYs
//
//  The driver should:
//  Add PROPERTYKEYs pertaining to the ALL objects.
//
/////////////////////////////////////////////////////////////////////////
VOID CSensorDDI::AddCommonPropertyKeys(
    _In_ IPortableDeviceKeyCollection* pKeys)
{
    if (pKeys != NULL)
    {
        for (DWORD dwIndex = 0; dwIndex < ARRAYSIZE(g_SupportedCommonProperties); dwIndex++)
        {
            pKeys->Add(g_SupportedCommonProperties[dwIndex] );
        }
    }
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to populate common PROPERTYKEYs found on the DEVICE object.
//
//  The parameters sent to us are:
//  pKeys - An IPortableDeviceKeyCollection to be populated with PROPERTYKEYs
//
//  The driver should:
//  Add PROPERTYKEYs pertaining to the DEVICE object.
//
/////////////////////////////////////////////////////////////////////////
VOID CSensorDDI::AddDevicePropertyKeys(
    _In_ IPortableDeviceKeyCollection* pKeys)
{
    if (pKeys != NULL)
    {
        for (DWORD dwIndex = 0; dwIndex < ARRAYSIZE(g_SupportedDeviceProperties); dwIndex++)
        {
            pKeys->Add(g_SupportedDeviceProperties[dwIndex] );
        }
    }
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to populate common PROPERTYKEYs found on sensor objects.
//
//  The parameters sent to us are:
//  pKeys - An IPortableDeviceKeyCollection to be populated with PROPERTYKEYs
//
//  The driver should:
//  Add PROPERTYKEYs pertaining to the sensor objects.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::AddSensorPropertyKeys(
    _In_ SensorType sensType,
    _In_ DWORD sensIndex,
    _In_ IPortableDeviceKeyCollection* pKeys)
{
    UNREFERENCED_PARAMETER(sensType);

    HRESULT hr = S_OK;

    CComPtr<IPortableDeviceKeyCollection> spSensorProperties;
    
    if (NULL != pKeys)
    {
        // Add the supported properties
        CSensor* pSensor = nullptr;

        hr = GetSensorObject(sensIndex, &pSensor);

        if (SUCCEEDED(hr) && (nullptr != pSensor))
        {
            hr = pSensor->GetSupportedProperties(&spSensorProperties);
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to get sensor object, hr = %!HRESULT!", hr);
        }

        if (SUCCEEDED(hr))
        {
            hr = CopyPropertyKeys(spSensorProperties, pKeys);
        }
    }
    else
    {
        hr = E_UNEXPECTED;
        Trace(TRACE_LEVEL_ERROR, "pKeys or pSensorManager is null, hr = %!HRESULT!", hr);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to populate common PROPERTYKEYs found on sensor objects.
//
//  The parameters sent to us are:
//  pKeys - An IPortableDeviceKeyCollection to be populated with PROPERTYKEYs
//
//  The driver should:
//  Add PROPERTYKEYs pertaining to the sensor objects.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::AddSensorDataFieldKeys(
       _In_ SensorType sensType,
       _In_ DWORD sensIndex,
       _In_ IPortableDeviceKeyCollection* pKeys)
{    
    UNREFERENCED_PARAMETER(sensType);

    HRESULT hr = S_OK;

    CComPtr<IPortableDeviceKeyCollection> spSensorDataFields;

    if (NULL != pKeys)
    {
        // Add the supported datafields
        CSensor* pSensor = nullptr;

        hr = GetSensorObject(sensIndex, &pSensor);

        if (SUCCEEDED(hr) && (nullptr != pSensor))
        {
            hr = pSensor->GetSupportedDataFields(&spSensorDataFields);
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to get sensor object, hr = %!HRESULT!", hr);
        }
        
        if (SUCCEEDED(hr))
        {
            hr = CopyPropertyKeys(spSensorDataFields, pKeys);
        }
    }
    else
    {
        hr = E_UNEXPECTED;
        Trace(TRACE_LEVEL_ERROR, "pKeys or pSensorManager is null, hr = %!HRESULT!", hr);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to populate supported PROPERTYKEYs found on objects.
//
//  The parameters sent to us are:
//  wszObjectID - the object whose supported property keys are being requested
//  pKeys - An IPortableDeviceKeyCollection to be populated with supported PROPERTYKEYs
//
//  The driver should:
//  Add PROPERTYKEYs pertaining to the specified object.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::AddSupportedPropertyKeys(
    _In_ LPWSTR                        wszObjectID,
    _In_ IPortableDeviceKeyCollection*  pKeys)
{
    HRESULT hr = S_OK;
    CAtlStringW strObjectID = wszObjectID;
  
    // Add Common PROPERTYKEYs for ALL WPD objects
    AddCommonPropertyKeys(pKeys);

    if (strObjectID.CompareNoCase(WPD_DEVICE_OBJECT_ID) == 0)
    {
        // Add the PROPERTYKEYs for the 'DEVICE' object
        AddDevicePropertyKeys(pKeys);
    }

    // Add the PROPERTYKEYs for the 'SENSOR' object
    SensorType sType;
    DWORD sIndex;

    hr = FindSensorTypeFromObjectID(wszObjectID, &sType, &sIndex);

    if(SUCCEEDED(hr))
    {
        hr = AddSensorPropertyKeys(sType, sIndex, pKeys);
    }
    else
    {
        hr = E_UNEXPECTED;

        if (TRUE == wcscmp(L"DEVICE", wszObjectID))
        {
            Trace(TRACE_LEVEL_ERROR, "Device is %ws not a supported sensor, hr = %!HRESULT!", wszObjectID, hr);
        }
        else
        {
            //WPD is calling us, so ignore trace
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to populate supported PROPERTYKEYs found on data objects.
//
//  The parameters sent to us are:
//  wszObjectID - the object whose supported property keys are being requested
//  pKeys - An IPortableDeviceKeyCollection to be populated with supported PROPERTYKEYs
//
//  The driver should:
//  Add PROPERTYKEYs pertaining to the specified object.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::AddSupportedDataFieldKeys(
    _In_  LPWSTR                        wszObjectID,
    _In_ IPortableDeviceKeyCollection*  pKeys)
{
    HRESULT     hr          = S_OK;
    CAtlStringW strObjectID = wszObjectID;

    // Add the DATAFIELD keys for the 'SENSOR' object
    SensorType sType;
    DWORD sIndex;

    hr = FindSensorTypeFromObjectID(wszObjectID, &sType, &sIndex);

    if(SUCCEEDED(hr))
    {
        hr = AddSensorDataFieldKeys(sType, sIndex, pKeys);
    }
    else
    {
        hr = E_UNEXPECTED;

        if (TRUE == wcscmp(L"DEVICE", wszObjectID))
        {
            Trace(TRACE_LEVEL_ERROR, "Device is %ws not a supported sensor, hr = %!HRESULT!", wszObjectID, hr);
        }
        else
        {
            //WPD is calling us, so ignore trace
        }
    }
    
    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to update choose a client Report Interval.
//
//  The parameters sent to us are:
//  wszObjectID - the object whose supported property keys are being requested
//  pValues - An IPortableDeviceValues to populated the Report Interval PROPERTYKEYs
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::SelectClientReportInterval(
        _In_ CSensor* pSensor)
{
    HRESULT hr = S_OK;

    if (pSensor == NULL)
    {
        hr = E_INVALIDARG;
        return hr;
    }

    if (SUCCEEDED(hr))
    {
        ULONG shortestReportInterval = UINT_MAX;
        ULONG clientReportInterval = 0;
        size_t cClients = pSensor->m_pClientMap.GetCount();
        IWDFFile* pClient = nullptr;
        POSITION posClient = nullptr;
        CLIENT_ENTRY entry;
        BOOL fReportIntervalChosen = FALSE;

        if (cClients == 0)
        {
            pSensor->m_fReportingState = FALSE;
            pSensor->m_ulPowerState = SENSOR_POWER_STATE_POWER_OFF;

            pSensor->m_ulLowestClientReportInterval = pSensor->m_ulDefaultCurrentReportInterval;
        }
        else
        {
            posClient = pSensor->m_pClientMap.GetStartPosition();

            //find the lowest value in the client report interval list
            while (posClient != NULL)
            {
                pClient = pSensor->m_pClientMap.GetKeyAt(posClient);
                pSensor->m_pClientMap.Lookup(pClient, entry);
                clientReportInterval = pSensor->m_pClientMap[pClient].ulClientReportInterval;
                if (0 == clientReportInterval)
                {
                    //exclude this client from report interval calculations;
                }
                else if ((clientReportInterval > 0) && (clientReportInterval < shortestReportInterval))
                {
                    fReportIntervalChosen = TRUE;
                    shortestReportInterval = clientReportInterval;
                }

                pSensor->m_pClientMap.GetNextKey(posClient);
            }

            //if no clients have specified a report interval, then use the default
            if (UINT_MAX == shortestReportInterval)
            {
                shortestReportInterval = pSensor->m_ulDefaultCurrentReportInterval;
            }

            //range check to be sure is not less than Min Report Interval
            if (shortestReportInterval < pSensor->m_ulDefaultMinimumReportInterval)
            {
                shortestReportInterval = pSensor->m_ulDefaultMinimumReportInterval;
            }

            pSensor->m_ulLowestClientReportInterval = shortestReportInterval;

            if ((TRUE == fReportIntervalChosen) || (pSensor->GetSubscriberCount() > 0))
            {
                pSensor->m_fReportingState = TRUE;
                pSensor->m_ulPowerState = SENSOR_POWER_STATE_FULL_POWER;
            }
            else
            {
                pSensor->m_fReportingState = FALSE;
                pSensor->m_ulPowerState = SENSOR_POWER_STATE_LOW_POWER;
            }
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to choose a client Change Sensitivity.
//
//  The parameters sent to us are:
//  wszObjectID - the object whose supported property keys are being requested
//  pValues - An IPortableDeviceValues to populated the ChangeSensitivity PROPERTYKEYs
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::SelectClientChangeSensitivity(
        _In_ CSensor* pSensor)
{
    HRESULT hr = S_OK;

    if (pSensor == NULL)
    {
        hr = E_INVALIDARG;
        return hr;
    }

    // Update our cache
    if(SUCCEEDED(hr))
    {
        CComPtr<IPortableDeviceValues> spDfVals = nullptr;
        FLOAT clientChangeSensitivity = 0;
        FLOAT lowestChangeSensitivity = FLT_MAX;
        size_t cClients = pSensor->m_pClientMap.GetCount();
        IWDFFile* pClient = nullptr;
        POSITION posClient = nullptr;
        CLIENT_ENTRY entry;

        PROPERTYKEY pkDfKey;
        DWORD cDfKeys = 0;

        // CoCreate a collection to store the property set operation results.
        hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                    NULL,
                                    CLSCTX_INPROC_SERVER,
                                    IID_PPV_ARGS(&spDfVals));

        if (SUCCEEDED(hr))
        {
            hr = pSensor->m_spSupportedSensorDataFields->GetCount(&cDfKeys);
        }

        if (SUCCEEDED(hr))
        {
            for (ULONG uDfIdx = 1; uDfIdx < cDfKeys; uDfIdx++) //ignore the timestamp, the first datafield
            {
                pSensor->m_spSupportedSensorDataFields->GetAt(uDfIdx, &pkDfKey);

                if (SUCCEEDED(hr))
                {
                    if (cClients == 0)
                    {
                        pSensor->m_fltLowestClientChangeSensitivities[uDfIdx] = pSensor->m_fltDefaultChangeSensitivity;
                    }
                    else
                    {
                        lowestChangeSensitivity = FLT_MAX;

                        posClient = pSensor->m_pClientMap.GetStartPosition();

                        //find the lowest value in the client change sensitivity list
                        while (posClient != NULL)
                        {
                            pClient = pSensor->m_pClientMap.GetKeyAt(posClient);
                            pSensor->m_pClientMap.Lookup(pClient, entry);
                        
                            if (cClients > 0)
                            {
                                clientChangeSensitivity = pSensor->m_pClientMap[pClient].fltClientChangeSensitivity[uDfIdx];

                                if ( clientChangeSensitivity < 0.0F )
                                {
                                    //exclude this client from change sensitivity calculations
                                }
                                else if (( clientChangeSensitivity >= 0.0F) && ( clientChangeSensitivity < lowestChangeSensitivity ))
                                {
                                    lowestChangeSensitivity = clientChangeSensitivity;
                                }
                            }

                            pSensor->m_pClientMap.GetNextKey(posClient);
                        }

                        //if no clients have a specified change sensitivity, then use the default
                        if (FLT_MAX == lowestChangeSensitivity)
                        {
                            lowestChangeSensitivity = pSensor->m_fltDefaultChangeSensitivity;
                        }

                        //range check to be sure is not less than 0.0F
                        if (lowestChangeSensitivity < 0.0F)
                        {
                            lowestChangeSensitivity = pSensor->m_fltDefaultChangeSensitivity;
                        }

                        pSensor->m_fltLowestClientChangeSensitivities[uDfIdx] = lowestChangeSensitivity;
                    }
                }
            }
        }
    }

    return hr;
}

HRESULT CSensorDDI::GetSensorObject(
        _In_ ULONG ulIndex, 
        _Out_ CSensor** ppSensor)
{
    HRESULT hr = S_OK;

    if (SUCCEEDED(hr))
    {
        if (nullptr != m_pSensorManager)
        {
            POSITION pos = NULL;

            pos = m_pSensorManager->m_pSensorList.FindIndex(ulIndex);

            if (NULL != pos)
            {
                *ppSensor = m_pSensorManager->m_pSensorList.GetAt(pos);

                if (nullptr == *ppSensor)
                {
                    hr = E_UNEXPECTED;
                    Trace(TRACE_LEVEL_ERROR, "Failed to get sensor, hr = %!HRESULT!", hr);
                }
            }
            else
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Failed to find sensor position, hr = %!HRESULT!", hr);
            }
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "pSensorManger is null, hr = %!HRESULT!", hr);
        }
    }

    return hr;
}

HRESULT CSensorDDI::GetFeatureReport(
        _In_ UCHAR reportID, 
        _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport,
        _In_ UINT uReportSize,
        _In_ LPSTR sensorName
        )
{
    HRESULT hr = E_UNEXPECTED;
    BOOL fHasContent = FALSE;
    const DWORD dwMaxTries = FEATURE_REPORT_MAX_RETRIES;
    DWORD dwRetryCount = 0;

    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    if (nullptr != m_pHidIoctlRequest)
    {
        if (uReportSize <= MAX_REPORT_SIZE)
        {
            if (nullptr != pFeatureReport)
            {
                do
                {
                    ZeroMemory(pFeatureReport, uReportSize);

                    pFeatureReport[0] = reportID;

                    hr = m_pHidIoctlRequest->CreateAndSendIoctlRequest(
                            (BYTE*)pFeatureReport,
                            uReportSize, 
                            IOCTL_HID_GET_FEATURE,
                            NULL);

                    if (SUCCEEDED(hr))
                    {
                        for (DWORD idx = 0; idx < uReportSize; idx++)
                        {
                            if (pFeatureReport[idx] != 0)
                            {
                                fHasContent = TRUE;
                                break;
                            }
                        }

                        if (TRUE == fHasContent)
                        {
                            if (pFeatureReport[0] != reportID)
                            {
                                hr = E_UNEXPECTED;
                                Trace(TRACE_LEVEL_ERROR, "Feature Report for %s Report ID does not match expected reportID so retrying, retry count = %i, hr = %!HRESULT!", sensorName, dwRetryCount, hr);
                            }
                        }
                        else
                        {
                            Trace(TRACE_LEVEL_ERROR, "Feature Report for %s is empty so retrying, retry count = %i, hr = %!HRESULT!", sensorName, dwRetryCount, hr);
                        }
                    }
                    else
                    {
                        Trace(TRACE_LEVEL_ERROR, "Getting Feature Report from %s using m_pHidIoctlRequest failed so retrying, retry count = %i, hr = %!HRESULT!", sensorName, dwRetryCount, hr);
                    }

                    dwRetryCount++;

                } while ((dwRetryCount < dwMaxTries) && (FALSE == fHasContent));

                if ((dwRetryCount > dwMaxTries) && (FALSE == fHasContent))
                {
                    hr = E_UNEXPECTED;
                    Trace(TRACE_LEVEL_ERROR, "Getting Feature Report from %s failed after %i tries, hr = %!HRESULT!", sensorName, dwRetryCount, hr);
                }
            }
            else
            {
                hr = E_POINTER;
                Trace(TRACE_LEVEL_ERROR, "pFeatureReport for %s is NULL, hr = %!HRESULT!", sensorName, hr);
            }
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Feature Report size from %s is too large = %i, hr = %!HRESULT!", sensorName, uReportSize, hr);
        }
    }
    else
    {
        hr = E_POINTER;
        Trace(TRACE_LEVEL_ERROR, "m_pHidIoctlRequest is NULL, hr = %!HRESULT!", hr);
    }

    return hr;
}

inline HRESULT CSensorDDI::EnterProcessing(DWORD64 dwControlFlag)
{
    return m_pSensorManager->EnterProcessing(dwControlFlag);
}

inline void CSensorDDI::ExitProcessing(DWORD64 dwControlFlag)
{
    m_pSensorManager->ExitProcessing(dwControlFlag);
}

