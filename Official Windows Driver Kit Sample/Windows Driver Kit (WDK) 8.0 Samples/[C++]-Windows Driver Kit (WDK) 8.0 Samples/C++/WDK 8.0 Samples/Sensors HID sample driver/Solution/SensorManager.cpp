//
//    Copyright (C) Microsoft.  All rights reserved.
//
/*

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

Module Name:
    SensorManager.cpp

Abstract:
    Implements the CSensorManager container class

--*/

#include "internal.h"
#include "SensorDDI.h"
#include "SensorManager.h"


#include "Sensor.h"
#include "Accelerometer.h"
#include "AmbientLight.h"
#include "Presence.h"
#include "Compass.h"
#include "Gyrometer.h"
#include "Inclinometer.h"
#include "AtmosPressure.h"
#include "Humidity.h"
#include "Thermometer.h"
#include "Potentiometer.h"
#include "Distance.h"
#include "Switches.h"
#include "Voltage.h"
#include "Current.h"
#include "Power.h"
#include "Frequency.h"
#include "Orientation.h"
#include "Custom.h"
#include "Generic.h"
#include "Unsupported.h"
#include "Device.h"

#include "SensorManager.tmh"


/////////////////////////////////////////////////////////////////////////
//
// CSensorManager::CSensorManager
//
// Object constructor function
//
/////////////////////////////////////////////////////////////////////////
CSensorManager::CSensorManager() :
    m_spWdfDevice(NULL),
    m_spClassExtension(NULL),
    m_pSensorDDI(NULL),
    m_hSensorEvent(NULL),
    m_hSensorManagerEventingThread(NULL),
    m_fSensorManagerInitialized(FALSE),
    m_fThreadActive(FALSE),
    m_pDevice(NULL)
{

}


/////////////////////////////////////////////////////////////////////////
//
// CSensorManager::~CSensorManager
//
// Object destructor function
//
/////////////////////////////////////////////////////////////////////////
CSensorManager::~CSensorManager()
{
    SAFE_RELEASE(m_pSensorDDI);
    SAFE_RELEASE(m_pDevice);
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorManager::Initialize
//
// Merely store the device pointer.  The rest of the init will be done
// in Start().  This is because init depends on communication with the
// device.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorManager::Initialize(_In_ IWDFDevice* pWdfDevice, _In_ CMyDevice* pDevice)
{
    HRESULT hr = (FALSE == IsInitialized()) ? S_OK : E_UNEXPECTED;

    if(SUCCEEDED(hr))
    {        
        // Store the IWDF Device pointer
        m_spWdfDevice = pWdfDevice;
        m_fInitializationComplete = FALSE;
        m_NumMappedSensors = 0;
        m_pDevice = pDevice;
        m_pDevice->AddRef();
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorManager::Uninitialize
//
//
//
//
/////////////////////////////////////////////////////////////////////////
void CSensorManager::Uninitialize()
{
    // Free all the sensor objects
    for(DWORD i = 0; i < m_pSensorList.GetCount(); i++)
    {
        CSensor *pSensor = m_pSensorList.GetAt(m_pSensorList.FindIndex(i));

        if (nullptr != pSensor)
        {
            pSensor->Uninitialize();

            delete pSensor;
        }
    }

    // Clear the Sensor list and Sensor, client and subscriber maps
    m_pSensorList.RemoveAll();
    m_AvailableSensorsIDs.RemoveAll();
    m_AvailableSensorsTypes.RemoveAll();
    m_AvailableSensorsUsages.RemoveAll();
    m_AvailableSensorsLinkCollections.RemoveAll();

    // Release Sensor Class Extension and Sensor DDI
    if(NULL != m_spClassExtension)
    {
        m_spClassExtension->Uninitialize();
        m_spClassExtension.Release();
    }

    SAFE_RELEASE(m_pSensorDDI);

    m_fSensorManagerInitialized = FALSE;

    return;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorManager::InitializeClassExtension
//
//
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorManager::InitializeClassExtension()
{
    HRESULT hr = (NULL == m_spClassExtension) ? S_OK : E_UNEXPECTED;
    
    if(SUCCEEDED(hr))
    {
        // CoCreate the Sensor ClassExtension
        if(SUCCEEDED(hr))
        {
            hr =  CoCreateInstance(CLSID_SensorClassExtension,
                                    NULL,
                                    CLSCTX_INPROC_SERVER,
                                    IID_PPV_ARGS(&m_spClassExtension));
            
            if (REGDB_E_CLASSNOTREG == hr)
            {
                Trace(TRACE_LEVEL_ERROR, "Class is not registered, hr = %!HRESULT!", hr);
                hr = E_UNEXPECTED;
                m_spClassExtension = NULL;
            }
        }

        // Initialize Sensor ClassExtension
        if(SUCCEEDED(hr))
        {
            CComPtr<IUnknown> spIUnknown;
            hr = m_pSensorDDI->QueryInterface(IID_IUnknown, (void**)&spIUnknown);
            
            if(SUCCEEDED(hr))
            {
                if ( NULL != m_spClassExtension )
                {
                    if (nullptr != m_spWdfDevice)
                    {
                        hr = m_spClassExtension->Initialize(m_spWdfDevice, spIUnknown);
                    }
                    else
                    {
                        hr = E_POINTER;
                    }
                }
            }
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////////////
//
// CSensorManager::Start
//
//
//
//
//////////////////////////////////////////////////////////////////////////////////
HRESULT CSensorManager::Start()
{
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    HRESULT hr;

    // Create the sensor DDI if not done already
    if (NULL == m_pSensorDDI)
    {
        //
        // Create the SensorDDI object that implements ISensorDriver
        //
        hr = CComObject<CSensorDDI>::CreateInstance(&m_pSensorDDI);
        if ((SUCCEEDED(hr)) && (NULL != m_pSensorDDI))
        {
            m_pSensorDDI->AddRef();
        }
    }

    // Always initialize the sensor DDI on Start()
    if(NULL != m_pSensorDDI)
    {
        m_pSensorDDI->m_pSensorManager = this;
        hr = m_pSensorDDI->InitSensorDevice(m_spWdfDevice);
    }
    else
    {
        hr = E_UNEXPECTED;
        Trace(TRACE_LEVEL_ERROR, "SensorDDI pointer is NULL, hr = %!HRESULT!", hr);
    }

    // Init sensor driver the first time Start() is called.  This is done
    // here vs. in Initialize() because we need to contact the device to
    // determine its type before init.
    if (SUCCEEDED(hr) && FALSE == IsInitialized())
    {
        // Get the device's report description to determine
        // the sensor and report type
        SensorType  sensType = SensorTypeNone;

        WCHAR* tempStr = nullptr;
        WCHAR* sensorID = nullptr;
        WCHAR* deviceID = nullptr;
        WCHAR* pwszManufacturer = nullptr;
        WCHAR* pwszProduct = nullptr;
        WCHAR* pwszSerialNumber = nullptr;

        try 
        {
            tempStr = new WCHAR[HID_USB_DESCRIPTOR_MAX_LENGTH];
        }
        catch(...)
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to allocate memory for temp string, hr = %!HRESULT!", hr);

            if (nullptr != tempStr) 
            {
                delete[] tempStr;
            }
        }

        try 
        {
            sensorID = new WCHAR[HID_USB_DESCRIPTOR_MAX_LENGTH];
        }
        catch(...)
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to allocate memory for sensor ID string, hr = %!HRESULT!", hr);

            if (nullptr != sensorID) 
            {
                delete[] sensorID;
            }
        }

        try 
        {
            deviceID = new WCHAR[HID_USB_DESCRIPTOR_MAX_LENGTH];
        }
        catch(...)
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to allocate memory for device ID string, hr = %!HRESULT!", hr);

            if (nullptr != deviceID) 
            {
                delete[] deviceID;
            }
        }

        try 
        {
            pwszManufacturer = new WCHAR[HID_USB_DESCRIPTOR_MAX_LENGTH];
        }
        catch(...)
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to allocate memory for manufacturer string, hr = %!HRESULT!", hr);

            if (nullptr != pwszManufacturer) 
            {
                delete[] pwszManufacturer;
            }
        }

        try 
        {
            pwszProduct = new WCHAR[HID_USB_DESCRIPTOR_MAX_LENGTH];
        }
        catch(...)
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to allocate memory for product string, hr = %!HRESULT!", hr);

            if (nullptr != pwszProduct) 
            {
                delete[] pwszProduct;
            }
        }

        try 
        {
            pwszSerialNumber = new WCHAR[HID_USB_DESCRIPTOR_MAX_LENGTH];
        }
        catch(...)
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to allocate memory for serial number string, hr = %!HRESULT!", hr);

            if (nullptr != pwszSerialNumber) 
            {
                delete[] pwszSerialNumber;
            }
        }

        if (SUCCEEDED(hr))
        {
            int numSensors = 0;

#pragma warning(push)
#pragma warning(disable:26035)
            // the OACR warning is being disabled here because it expects a null-terminated string
            // for this particular use. However, string termination happens in the called method
            hr = m_pSensorDDI->RequestDeviceInfo(&sensType, pwszManufacturer, pwszProduct, pwszSerialNumber, deviceID);
#pragma warning(pop)

            if (SUCCEEDED(hr))
            {
                CSensor*        pSensor = NULL;
                CAccelerometer* pAccelerometer = NULL;
                CAmbientLight*  pAmbientLight = NULL;
                CPresence*      pPresence = NULL;
                CCompass*       pCompass = NULL;
                CGyrometer*     pGyrometer = NULL;
                CInclinometer*  pInclinometer = NULL;
                CBarometer*     pBarometer = NULL;
                CHygrometer*    pHygrometer = NULL;
                CThermometer*   pThermometer = NULL;
                CPotentiometer* pPotentiometer = NULL;
                CDistance*      pDistance = NULL;
                CSwitch*        pSwitch = NULL;
                CVoltage*       pVoltage = NULL;
                CCurrent*       pCurrent = NULL;
                CPower*         pPower = NULL;
                CFrequency*     pFrequency = NULL;
                COrientation*   pOrientation = NULL;
                CCustom*        pCustom = NULL;
                CGeneric*       pGeneric = NULL;
                CUnsupported*   pUnsupported = NULL;

                if (sensType == Collection)
                {
                    numSensors = (int)m_AvailableSensorsTypes.GetCount();
                }
                else
                {
                    numSensors = 1;
                }

                m_NumMappedSensors = (ULONG)numSensors;

                //Build the sensor objects from the sensor map
                if((SUCCEEDED(hr)) && (numSensors > 0))
                {
                    for (int idx = 0; idx < numSensors; idx++)
                    {
                        if (((sensType < FirstSensorType) || (sensType > LastSensorType)) && sensType != Collection)
                        {
                            hr = E_UNEXPECTED;
                            Trace(TRACE_LEVEL_ERROR, "Invalid sensor type, hr = %!HRESULT!", hr);
                        }
                        else
                        {
                            hr = StringCchCopy(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, m_wszDeviceId);

                            if (SUCCEEDED(hr))
                            {
                                hr = StringTrim(sensorID);

                                if (SUCCEEDED(hr))
                                {
                                    switch (m_AvailableSensorsTypes[idx])
                                    {

                                    case Accelerometer:
                                        hr = StringCchPrintf(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L"#%s-%i", deviceID, idx);

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = StringCchCat(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempStr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            m_AvailableSensorsIDs[idx] = sensorID;

                                            try 
                                            {
                                                pAccelerometer = new CAccelerometer();
                                            }
                                            catch(...)
                                            {
                                                hr = E_UNEXPECTED;
                                                Trace(TRACE_LEVEL_ERROR, "Failed to create new accelerometer, hr = %!HRESULT!", hr);

                                                if (nullptr != pAccelerometer) 
                                                    delete pAccelerometer;
                                            }

                                            if (NULL != pAccelerometer)
                                            {
                                                hr = pAccelerometer->Initialize(
                                                        Accelerometer,
                                                        m_AvailableSensorsUsages[idx],
                                                        m_AvailableSensorsLinkCollections[idx],
                                                        idx, 
                                                        pwszManufacturer,
                                                        pwszProduct,
                                                        pwszSerialNumber,
                                                        sensorID,
                                                        this);
                                            }
                                            else
                                            {
                                                hr = E_OUTOFMEMORY;
                                                Trace(TRACE_LEVEL_ERROR, "Unable to create accelerometer object, hr = %!HRESULT!", hr);
                                            }
                                            if(SUCCEEDED(hr))
                                            {
                                                pSensor = static_cast<CSensor*>(pAccelerometer);
                                                Trace(TRACE_LEVEL_INFORMATION, "Accelerometer sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                            }
                                        }
                                        break;

                                    case AmbientLight:
                                        hr = StringCchPrintf(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L"#%s-%i", deviceID, idx);

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = StringCchCat(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempStr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            m_AvailableSensorsIDs[idx] = sensorID;

                                            try 
                                            {
                                                pAmbientLight = new CAmbientLight();
                                            }
                                            catch(...)
                                            {
                                                hr = E_UNEXPECTED;
                                                Trace(TRACE_LEVEL_ERROR, "Failed to create new ambientlight, hr = %!HRESULT!", hr);

                                                if (nullptr != pAmbientLight) 
                                                    delete pAmbientLight;
                                            }

                                            if (NULL != pAmbientLight)
                                            {
                                                hr = pAmbientLight->Initialize(
                                                        AmbientLight, 
                                                        m_AvailableSensorsUsages[idx],
                                                        m_AvailableSensorsLinkCollections[idx],
                                                        idx, 
                                                        pwszManufacturer,
                                                        pwszProduct,
                                                        pwszSerialNumber,
                                                        sensorID,
                                                        this);
                                            }
                                            else
                                            {
                                                hr = E_OUTOFMEMORY;
                                                Trace(TRACE_LEVEL_ERROR, "Unable to create ambientlight object, hr = %!HRESULT!", hr);
                                            }
                                            if(SUCCEEDED(hr))
                                            {
                                                pSensor = static_cast<CSensor*>(pAmbientLight);
                                                Trace(TRACE_LEVEL_INFORMATION, "AmbientLight sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                            }
                                        }
                                        break;

                                    case Presence:
                                        hr = StringCchPrintf(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L"#%s-%i", deviceID, idx);

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = StringCchCat(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempStr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            m_AvailableSensorsIDs[idx] = sensorID;

                                            try 
                                            {
                                                pPresence = new CPresence();
                                            }
                                            catch(...)
                                            {
                                                hr = E_UNEXPECTED;
                                                Trace(TRACE_LEVEL_ERROR, "Failed to create new presence, hr = %!HRESULT!", hr);

                                                if (nullptr != pPresence) 
                                                    delete pPresence;
                                            }

                                            if (NULL != pPresence)
                                            {
                                                hr = pPresence->Initialize(
                                                        Presence, 
                                                        m_AvailableSensorsUsages[idx],
                                                        m_AvailableSensorsLinkCollections[idx],
                                                        idx, 
                                                        pwszManufacturer,
                                                        pwszProduct,
                                                        pwszSerialNumber,
                                                        sensorID,
                                                        this);
                                            }
                                            else
                                            {
                                                hr = E_OUTOFMEMORY;
                                                Trace(TRACE_LEVEL_ERROR, "Unable to create presence object, hr = %!HRESULT!", hr);
                                            }
                                            if(SUCCEEDED(hr))
                                            {
                                                pSensor = static_cast<CSensor*>(pPresence);
                                                Trace(TRACE_LEVEL_INFORMATION, "Presence sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                            }
                                        }
                                        break;

                                    case Compass:
                                        hr = StringCchPrintf(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L"#%s-%i", deviceID, idx);

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = StringCchCat(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempStr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            m_AvailableSensorsIDs[idx] = sensorID;

                                            try 
                                            {
                                                pCompass = new CCompass();
                                            }
                                            catch(...)
                                            {
                                                hr = E_UNEXPECTED;
                                                Trace(TRACE_LEVEL_ERROR, "Failed to create new compass, hr = %!HRESULT!", hr);

                                                if (nullptr != pCompass) 
                                                    delete pCompass;
                                            }

                                            if (NULL != pCompass)
                                            {
                                                hr = pCompass->Initialize(
                                                        Compass, 
                                                        m_AvailableSensorsUsages[idx],
                                                        m_AvailableSensorsLinkCollections[idx],
                                                        idx, 
                                                        pwszManufacturer,
                                                        pwszProduct,
                                                        pwszSerialNumber,
                                                        sensorID,
                                                        this);
                                            }
                                            else
                                            {
                                                hr = E_OUTOFMEMORY;
                                                Trace(TRACE_LEVEL_ERROR, "Unable to create compass object, hr = %!HRESULT!", hr);
                                            }
                                            if(SUCCEEDED(hr))
                                            {
                                                pSensor = static_cast<CSensor*>(pCompass);
                                                Trace(TRACE_LEVEL_INFORMATION, "Compass sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                            }
                                        }
                                        break;

                                    case Gyrometer:
                                        hr = StringCchPrintf(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L"#%s-%i", deviceID, idx);

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = StringCchCat(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempStr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            m_AvailableSensorsIDs[idx] = sensorID;

                                            try 
                                            {
                                                pGyrometer = new CGyrometer();
                                            }
                                            catch(...)
                                            {
                                                hr = E_UNEXPECTED;
                                                Trace(TRACE_LEVEL_ERROR, "Failed to create new gyrometer, hr = %!HRESULT!", hr);

                                                if (nullptr != pGyrometer) 
                                                    delete pGyrometer;
                                            }

                                            if (NULL != pGyrometer)
                                            {
                                                hr = pGyrometer->Initialize(
                                                        Gyrometer, 
                                                        m_AvailableSensorsUsages[idx],
                                                        m_AvailableSensorsLinkCollections[idx],
                                                        idx, 
                                                        pwszManufacturer,
                                                        pwszProduct,
                                                        pwszSerialNumber,
                                                        sensorID,
                                                        this);
                                            }
                                            else
                                            {
                                                hr = E_OUTOFMEMORY;
                                                Trace(TRACE_LEVEL_ERROR, "Unable to create gyrometer object, hr = %!HRESULT!", hr);
                                            }
                                            if(SUCCEEDED(hr))
                                            {
                                                pSensor = static_cast<CSensor*>(pGyrometer);
                                                Trace(TRACE_LEVEL_INFORMATION, "Gyrometer sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                            }
                                        }
                                        break;

                                    case Inclinometer:
                                        hr = StringCchPrintf(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L"#%s-%i", deviceID, idx);

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = StringCchCat(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempStr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            m_AvailableSensorsIDs[idx] = sensorID;

                                            try 
                                            {
                                                pInclinometer = new CInclinometer();
                                            }
                                            catch(...)
                                            {
                                                hr = E_UNEXPECTED;
                                                Trace(TRACE_LEVEL_ERROR, "Failed to create new inclinometer, hr = %!HRESULT!", hr);

                                                if (nullptr != pInclinometer) 
                                                    delete pInclinometer;
                                            }

                                            if (NULL != pInclinometer)
                                            {
                                                hr = pInclinometer->Initialize(
                                                        Inclinometer, 
                                                        m_AvailableSensorsUsages[idx],
                                                        m_AvailableSensorsLinkCollections[idx],
                                                        idx, 
                                                        pwszManufacturer,
                                                        pwszProduct,
                                                        pwszSerialNumber,
                                                        sensorID,
                                                        this);
                                            }
                                            else
                                            {
                                                hr = E_OUTOFMEMORY;
                                                Trace(TRACE_LEVEL_ERROR, "Unable to create Inclinometer object, hr = %!HRESULT!", hr);
                                            }
                                            if(SUCCEEDED(hr))
                                            {
                                                pSensor = static_cast<CSensor*>(pInclinometer);
                                                Trace(TRACE_LEVEL_INFORMATION, "Inclinometer sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                            }
                                        }
                                        break;

                                    case Barometer:
                                        hr = StringCchPrintf(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L"#%s-%i", deviceID, idx);

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = StringCchCat(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempStr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            m_AvailableSensorsIDs[idx] = sensorID;

                                            try 
                                            {
                                                pBarometer = new CBarometer();
                                            }
                                            catch(...)
                                            {
                                                hr = E_UNEXPECTED;
                                                Trace(TRACE_LEVEL_ERROR, "Failed to create new barometer, hr = %!HRESULT!", hr);

                                                if (nullptr != pBarometer) 
                                                    delete pBarometer;
                                            }

                                            if (NULL != pBarometer)
                                            {
                                                hr = pBarometer->Initialize(
                                                        Barometer, 
                                                        m_AvailableSensorsUsages[idx],
                                                        m_AvailableSensorsLinkCollections[idx],
                                                        idx, 
                                                        pwszManufacturer,
                                                        pwszProduct,
                                                        pwszSerialNumber,
                                                        sensorID,
                                                        this);
                                            }
                                            else
                                            {
                                                hr = E_OUTOFMEMORY;
                                                Trace(TRACE_LEVEL_ERROR, "Unable to create Barometer object, hr = %!HRESULT!", hr);
                                            }
                                            if(SUCCEEDED(hr))
                                            {
                                                pSensor = static_cast<CSensor*>(pBarometer);
                                                Trace(TRACE_LEVEL_INFORMATION, "Barometer sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                            }
                                        }
                                        break;

                                    case Hygrometer:
                                        hr = StringCchPrintf(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L"#%s-%i", deviceID, idx);

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = StringCchCat(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempStr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            m_AvailableSensorsIDs[idx] = sensorID;

                                            try 
                                            {
                                                pHygrometer = new CHygrometer();
                                            }
                                            catch(...)
                                            {
                                                hr = E_UNEXPECTED;
                                                Trace(TRACE_LEVEL_ERROR, "Failed to create new hygrometer, hr = %!HRESULT!", hr);

                                                if (nullptr != pHygrometer) 
                                                    delete pHygrometer;
                                            }

                                            if (NULL != pHygrometer)
                                            {
                                                hr = pHygrometer->Initialize(
                                                        Hygrometer, 
                                                        m_AvailableSensorsUsages[idx],
                                                        m_AvailableSensorsLinkCollections[idx],
                                                        idx, 
                                                        pwszManufacturer,
                                                        pwszProduct,
                                                        pwszSerialNumber,
                                                        sensorID,
                                                        this);
                                            }
                                            else
                                            {
                                                hr = E_OUTOFMEMORY;
                                                Trace(TRACE_LEVEL_ERROR, "Unable to create Hygrometer object, hr = %!HRESULT!", hr);
                                            }
                                            if(SUCCEEDED(hr))
                                            {
                                                pSensor = static_cast<CSensor*>(pHygrometer);
                                                Trace(TRACE_LEVEL_INFORMATION, "Hygrometer sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                            }
                                        }
                                        break;

                                    case Thermometer:
                                        hr = StringCchPrintf(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L"#%s-%i", deviceID, idx);

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = StringCchCat(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempStr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            m_AvailableSensorsIDs[idx] = sensorID;

                                            try 
                                            {
                                                pThermometer = new CThermometer();
                                            }
                                            catch(...)
                                            {
                                                hr = E_UNEXPECTED;
                                                Trace(TRACE_LEVEL_ERROR, "Failed to create new thermometer, hr = %!HRESULT!", hr);

                                                if (nullptr != pThermometer) 
                                                    delete pThermometer;
                                            }

                                            if (NULL != pThermometer)
                                            {
                                                hr = pThermometer->Initialize(
                                                        Thermometer, 
                                                        m_AvailableSensorsUsages[idx],
                                                        m_AvailableSensorsLinkCollections[idx],
                                                        idx, 
                                                        pwszManufacturer,
                                                        pwszProduct,
                                                        pwszSerialNumber,
                                                        sensorID,
                                                        this);
                                            }
                                            else
                                            {
                                                hr = E_OUTOFMEMORY;
                                                Trace(TRACE_LEVEL_ERROR, "Unable to create Thermometer object, hr = %!HRESULT!", hr);
                                            }
                                            if(SUCCEEDED(hr))
                                            {
                                                pSensor = static_cast<CSensor*>(pThermometer);
                                                Trace(TRACE_LEVEL_INFORMATION, "Thermometer sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                            }
                                        }
                                        break;

                                    case Potentiometer:
                                        hr = StringCchPrintf(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L"#%s-%i", deviceID, idx);

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = StringCchCat(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempStr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            m_AvailableSensorsIDs[idx] = sensorID;

                                            try 
                                            {
                                                pPotentiometer = new CPotentiometer();
                                            }
                                            catch(...)
                                            {
                                                hr = E_UNEXPECTED;
                                                Trace(TRACE_LEVEL_ERROR, "Failed to create new potentiometer, hr = %!HRESULT!", hr);

                                                if (nullptr != pPotentiometer) 
                                                    delete pPotentiometer;
                                            }

                                            if (NULL != pPotentiometer)
                                            {
                                                hr = pPotentiometer->Initialize(
                                                        Potentiometer, 
                                                        m_AvailableSensorsUsages[idx],
                                                        m_AvailableSensorsLinkCollections[idx],
                                                        idx, 
                                                        pwszManufacturer,
                                                        pwszProduct,
                                                        pwszSerialNumber,
                                                        sensorID,
                                                        this);
                                            }
                                            else
                                            {
                                                hr = E_OUTOFMEMORY;
                                                Trace(TRACE_LEVEL_ERROR, "Unable to create Potentiometer object, hr = %!HRESULT!", hr);
                                            }
                                            if(SUCCEEDED(hr))
                                            {
                                                pSensor = static_cast<CSensor*>(pPotentiometer);
                                                Trace(TRACE_LEVEL_INFORMATION, "Potentiometer sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                            }
                                        }
                                        break;

                                    case Distance:
                                        hr = StringCchPrintf(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L"#%s-%i", deviceID, idx);

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = StringCchCat(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempStr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            m_AvailableSensorsIDs[idx] = sensorID;

                                            try 
                                            {
                                                pDistance = new CDistance();
                                            }
                                            catch(...)
                                            {
                                                hr = E_UNEXPECTED;
                                                Trace(TRACE_LEVEL_ERROR, "Failed to create new distance, hr = %!HRESULT!", hr);

                                                if (nullptr != pDistance) 
                                                    delete pDistance;
                                            }

                                            if (NULL != pDistance)
                                            {
                                                hr = pDistance->Initialize(
                                                        Distance, 
                                                        m_AvailableSensorsUsages[idx],
                                                        m_AvailableSensorsLinkCollections[idx],
                                                        idx, 
                                                        pwszManufacturer,
                                                        pwszProduct,
                                                        pwszSerialNumber,
                                                        sensorID,
                                                        this);
                                            }
                                            else
                                            {
                                                hr = E_OUTOFMEMORY;
                                                Trace(TRACE_LEVEL_ERROR, "Unable to create Distance object, hr = %!HRESULT!", hr);
                                            }
                                            if(SUCCEEDED(hr))
                                            {
                                                pSensor = static_cast<CSensor*>(pDistance);
                                                Trace(TRACE_LEVEL_INFORMATION, "Distance sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                            }
                                        }
                                        break;

                                    case Switch:
                                        hr = StringCchPrintf(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L"#%s-%i", deviceID, idx);

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = StringCchCat(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempStr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            m_AvailableSensorsIDs[idx] = sensorID;

                                            try 
                                            {
                                                pSwitch = new CSwitch();
                                            }
                                            catch(...)
                                            {
                                                hr = E_UNEXPECTED;
                                                Trace(TRACE_LEVEL_ERROR, "Failed to create new switch, hr = %!HRESULT!", hr);

                                                if (nullptr != pSwitch) 
                                                    delete pSwitch;
                                            }

                                            if (NULL != pSwitch)
                                            {
                                                hr = pSwitch->Initialize(
                                                        Switch, 
                                                        m_AvailableSensorsUsages[idx],
                                                        m_AvailableSensorsLinkCollections[idx],
                                                        idx, 
                                                        pwszManufacturer,
                                                        pwszProduct,
                                                        pwszSerialNumber,
                                                        sensorID,
                                                        this);
                                            }
                                            else
                                            {
                                                hr = E_OUTOFMEMORY;
                                                Trace(TRACE_LEVEL_ERROR, "Unable to create Switch object, hr = %!HRESULT!", hr);
                                            }
                                            if(SUCCEEDED(hr))
                                            {
                                                pSensor = static_cast<CSensor*>(pSwitch);
                                                Trace(TRACE_LEVEL_INFORMATION, "Switch sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                            }
                                        }
                                        break;

                                    case Voltage:
                                        hr = StringCchPrintf(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L"#%s-%i", deviceID, idx);

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = StringCchCat(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempStr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            m_AvailableSensorsIDs[idx] = sensorID;

                                            try 
                                            {
                                                pVoltage = new CVoltage();
                                            }
                                            catch(...)
                                            {
                                                hr = E_UNEXPECTED;
                                                Trace(TRACE_LEVEL_ERROR, "Failed to create new voltage, hr = %!HRESULT!", hr);

                                                if (nullptr != pVoltage) 
                                                    delete pVoltage;
                                            }

                                            if (NULL != pVoltage)
                                            {
                                                hr = pVoltage->Initialize(
                                                        Voltage, 
                                                        m_AvailableSensorsUsages[idx],
                                                        m_AvailableSensorsLinkCollections[idx],
                                                        idx, 
                                                        pwszManufacturer,
                                                        pwszProduct,
                                                        pwszSerialNumber,
                                                        sensorID,
                                                        this);
                                            }
                                            else
                                            {
                                                hr = E_OUTOFMEMORY;
                                                Trace(TRACE_LEVEL_ERROR, "Unable to create Voltage object, hr = %!HRESULT!", hr);
                                            }
                                            if(SUCCEEDED(hr))
                                            {
                                                pSensor = static_cast<CSensor*>(pVoltage);
                                                Trace(TRACE_LEVEL_INFORMATION, "Voltage sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                            }
                                        }
                                        break;

                                    case Current:
                                        hr = StringCchPrintf(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L"#%s-%i", deviceID, idx);

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = StringCchCat(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempStr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            m_AvailableSensorsIDs[idx] = sensorID;

                                            try 
                                            {
                                                pCurrent = new CCurrent();
                                            }
                                            catch(...)
                                            {
                                                hr = E_UNEXPECTED;
                                                Trace(TRACE_LEVEL_ERROR, "Failed to create new current, hr = %!HRESULT!", hr);

                                                if (nullptr != pCurrent) 
                                                    delete pCurrent;
                                            }

                                            if (NULL != pCurrent)
                                            {
                                                hr = pCurrent->Initialize(
                                                        Current, 
                                                        m_AvailableSensorsUsages[idx],
                                                        m_AvailableSensorsLinkCollections[idx],
                                                        idx, 
                                                        pwszManufacturer,
                                                        pwszProduct,
                                                        pwszSerialNumber,
                                                        sensorID,
                                                        this);
                                            }
                                            else
                                            {
                                                hr = E_OUTOFMEMORY;
                                                Trace(TRACE_LEVEL_ERROR, "Unable to create Current object, hr = %!HRESULT!", hr);
                                            }
                                            if(SUCCEEDED(hr))
                                            {
                                                pSensor = static_cast<CSensor*>(pCurrent);
                                                Trace(TRACE_LEVEL_INFORMATION, "Current sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                            }
                                        }
                                        break;

                                    case Power:
                                        hr = StringCchPrintf(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L"#%s-%i", deviceID, idx);

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = StringCchCat(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempStr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            m_AvailableSensorsIDs[idx] = sensorID;

                                            try 
                                            {
                                                pPower = new CPower();
                                            }
                                            catch(...)
                                            {
                                                hr = E_UNEXPECTED;
                                                Trace(TRACE_LEVEL_ERROR, "Failed to create new power, hr = %!HRESULT!", hr);

                                                if (nullptr != pPower) 
                                                    delete pPower;
                                            }

                                            if (NULL != pPower)
                                            {
                                                hr = pPower->Initialize(
                                                        Power, 
                                                        m_AvailableSensorsUsages[idx],
                                                        m_AvailableSensorsLinkCollections[idx],
                                                        idx, 
                                                        pwszManufacturer,
                                                        pwszProduct,
                                                        pwszSerialNumber,
                                                        sensorID,
                                                        this);
                                            }
                                            else
                                            {
                                                hr = E_OUTOFMEMORY;
                                                Trace(TRACE_LEVEL_ERROR, "Unable to create Power object, hr = %!HRESULT!", hr);
                                            }
                                            if(SUCCEEDED(hr))
                                            {
                                                pSensor = static_cast<CSensor*>(pPower);
                                                Trace(TRACE_LEVEL_INFORMATION, "Power sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                            }
                                        }
                                        break;

                                    case Frequency:
                                        hr = StringCchPrintf(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L"#%s-%i", deviceID, idx);

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = StringCchCat(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempStr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            m_AvailableSensorsIDs[idx] = sensorID;

                                            try 
                                            {
                                                pFrequency = new CFrequency();
                                            }
                                            catch(...)
                                            {
                                                hr = E_UNEXPECTED;
                                                Trace(TRACE_LEVEL_ERROR, "Failed to create new frequency, hr = %!HRESULT!", hr);

                                                if (nullptr != pFrequency) 
                                                    delete pFrequency;
                                            }

                                            if (NULL != pFrequency)
                                            {
                                                hr = pFrequency->Initialize(
                                                        Frequency, 
                                                        m_AvailableSensorsUsages[idx],
                                                        m_AvailableSensorsLinkCollections[idx],
                                                        idx, 
                                                        pwszManufacturer,
                                                        pwszProduct,
                                                        pwszSerialNumber,
                                                        sensorID,
                                                        this);
                                            }
                                            else
                                            {
                                                hr = E_OUTOFMEMORY;
                                                Trace(TRACE_LEVEL_ERROR, "Unable to create Frequency object, hr = %!HRESULT!", hr);
                                            }
                                            if(SUCCEEDED(hr))
                                            {
                                                pSensor = static_cast<CSensor*>(pFrequency);
                                                Trace(TRACE_LEVEL_INFORMATION, "Frequency sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                            }
                                        }
                                        break;

                                    case Orientation:
                                        hr = StringCchPrintf(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L"#%s-%i", deviceID, idx);

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = StringCchCat(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempStr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            m_AvailableSensorsIDs[idx] = sensorID;

                                            try 
                                            {
                                                pOrientation = new COrientation();
                                            }
                                            catch(...)
                                            {
                                                hr = E_UNEXPECTED;
                                                Trace(TRACE_LEVEL_ERROR, "Failed to create new orientation, hr = %!HRESULT!", hr);

                                                if (nullptr != pOrientation) 
                                                    delete pOrientation;
                                            }

                                            if (NULL != pOrientation)
                                            {
                                                hr = pOrientation->Initialize(
                                                        Orientation, 
                                                        m_AvailableSensorsUsages[idx],
                                                        m_AvailableSensorsLinkCollections[idx],
                                                        idx, 
                                                        pwszManufacturer,
                                                        pwszProduct,
                                                        pwszSerialNumber,
                                                        sensorID,
                                                        this);
                                            }
                                            else
                                            {
                                                hr = E_OUTOFMEMORY;
                                                Trace(TRACE_LEVEL_ERROR, "Unable to create Orientation object, hr = %!HRESULT!", hr);
                                            }
                                            if(SUCCEEDED(hr))
                                            {
                                                pSensor = static_cast<CSensor*>(pOrientation);
                                                Trace(TRACE_LEVEL_INFORMATION, "Orientation sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                            }
                                        }
                                        break;

                                    case Custom:
                                        hr = StringCchPrintf(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L"#%s-%i", deviceID, idx);

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = StringCchCat(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempStr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            m_AvailableSensorsIDs[idx] = sensorID;

                                            try 
                                            {
                                                pCustom = new CCustom();
                                            }
                                            catch(...)
                                            {
                                                hr = E_UNEXPECTED;
                                                Trace(TRACE_LEVEL_ERROR, "Failed to create new custom, hr = %!HRESULT!", hr);

                                                if (nullptr != pCustom) 
                                                    delete pCustom;
                                            }

                                            if (NULL != pCustom)
                                            {
                                                hr = pCustom->Initialize(
                                                        Custom, 
                                                        m_AvailableSensorsUsages[idx],
                                                        m_AvailableSensorsLinkCollections[idx],
                                                        idx, 
                                                        pwszManufacturer,
                                                        pwszProduct,
                                                        pwszSerialNumber,
                                                        sensorID,
                                                        this);
                                            }
                                            else
                                            {
                                                hr = E_OUTOFMEMORY;
                                                Trace(TRACE_LEVEL_ERROR, "Unable to create custom sensor object, hr = %!HRESULT!", hr);
                                            }
                                            if(SUCCEEDED(hr))
                                            {
                                                pSensor = static_cast<CSensor*>(pCustom);
                                                Trace(TRACE_LEVEL_INFORMATION, "Custom sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                            }
                                        }
                                        break;

                                    case Generic:
                                        hr = StringCchPrintf(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L"#%s-%i", deviceID, idx);

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = StringCchCat(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempStr);
                                        }


                                        if (SUCCEEDED(hr))
                                        {
                                            m_AvailableSensorsIDs[idx] = sensorID;

                                            try 
                                            {
                                                pGeneric = new CGeneric();
                                            }
                                            catch(...)
                                            {
                                                hr = E_UNEXPECTED;
                                                Trace(TRACE_LEVEL_ERROR, "Failed to create new generic, hr = %!HRESULT!", hr);

                                                if (nullptr != pGeneric) 
                                                    delete pGeneric;
                                            }

                                            if (NULL != pGeneric)
                                            {
                                                hr = pGeneric->Initialize(
                                                        Generic, 
                                                        m_AvailableSensorsUsages[idx],
                                                        m_AvailableSensorsLinkCollections[idx],
                                                        idx, 
                                                        pwszManufacturer,
                                                        pwszProduct,
                                                        pwszSerialNumber,
                                                        sensorID,
                                                        this);
                                            }
                                            else
                                            {
                                                hr = E_OUTOFMEMORY;
                                                Trace(TRACE_LEVEL_ERROR, "Unable to create Generic sensor object, hr = %!HRESULT!", hr);
                                            }
                                            if(SUCCEEDED(hr))
                                            {
                                                pSensor = static_cast<CSensor*>(pGeneric);
                                                Trace(TRACE_LEVEL_INFORMATION, "Generic sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                            }
                                        }
                                        break;

                                    case Unsupported:
                                        hr = StringCchPrintf(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L"#%s-%i", deviceID, idx);

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = StringCchCat(sensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, tempStr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            m_AvailableSensorsIDs[idx] = sensorID;

                                            try 
                                            {
                                                pUnsupported = new CUnsupported();
                                            }
                                            catch(...)
                                            {
                                                hr = E_UNEXPECTED;
                                                Trace(TRACE_LEVEL_ERROR, "Failed to create new unsupported, hr = %!HRESULT!", hr);

                                                if (nullptr != pUnsupported) 
                                                    delete pUnsupported;
                                            }

                                            if (NULL != pUnsupported)
                                            {
                                                hr = pUnsupported->Initialize(
                                                        Unsupported, 
                                                        m_AvailableSensorsUsages[idx],
                                                        m_AvailableSensorsLinkCollections[idx],
                                                        idx, 
                                                        pwszManufacturer,
                                                        pwszProduct,
                                                        pwszSerialNumber,
                                                        sensorID,
                                                        this);
                                            }
                                            else
                                            {
                                                hr = E_OUTOFMEMORY;
                                                Trace(TRACE_LEVEL_ERROR, "Unable to create Unsupported sensor object, hr = %!HRESULT!", hr);
                                            }
                                            if(SUCCEEDED(hr))
                                            {
                                                pSensor = static_cast<CSensor*>(pUnsupported);
                                                Trace(TRACE_LEVEL_INFORMATION, "Unsupported sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                            }
                                        }
                                        break;

                                    default:
                                        hr = E_UNEXPECTED;
                                        Trace(TRACE_LEVEL_ERROR, "Invalid sensor type = %i, hr = %!HRESULT!", m_AvailableSensorsTypes[idx], hr);
                                        break;
                                    }
                                }
                            }
                        }
                    
                        // Set the unique persistent ID
                        if(SUCCEEDED(hr))
                        {
                            hr = pSensor->SetUniqueID(m_spWdfDevice);
                        }

                        if(SUCCEEDED(hr))
                        {
                            m_pSensorList.AddTail(pSensor);
                            pSensor->m_fReportingState = FALSE;
                        }

                        // Update the property values for each sensor
                        if (SUCCEEDED(hr))
                        {
                            hr = m_pSensorDDI->UpdateSensorPropertyValues(m_AvailableSensorsIDs[idx], FALSE);
                        }

                    } //end for idx
                }

                // Create and initialize the Sensor Class Extension
                if(SUCCEEDED(hr))
                {
                    hr = InitializeClassExtension();
                }

                if(SUCCEEDED(hr))
                {
                    m_fInitializationComplete = TRUE;
                    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Sensor Class Extension initialized");
                }
            }
        }
        else
        {
            Trace(TRACE_LEVEL_ERROR, "%!FUNC! Failed to get device info");
        }

#pragma warning(suppress: 6001) //uninitialized memory
        if (nullptr != tempStr) 
        {
            delete[] tempStr;
        }

#pragma warning(suppress: 6001) //uninitialized memory
        if (nullptr != sensorID) 
        {
            delete[] sensorID;
        }

#pragma warning(suppress: 6001) //uninitialized memory
        if (nullptr != deviceID) 
        {
            delete[] deviceID;
        }

#pragma warning(suppress: 6001) //uninitialized memory
        if (nullptr != pwszManufacturer) 
        {
            delete[] pwszManufacturer;
        }

#pragma warning(suppress: 6001) //uninitialized memory
        if (nullptr != pwszProduct) 
        {
            delete[] pwszProduct;
        }

#pragma warning(suppress: 6001) //uninitialized memory
        if (nullptr != pwszSerialNumber) 
        {
            delete[] pwszSerialNumber;
        }
    }
    
    // Update each sensor state to SENSOR_STATE_NO_DATA
    // Note: CSensorManager::_SensorEventThreadProc() will change sensor state to SENSOR_STATE_READY
    //   whenever a data field is ready.
    if (SUCCEEDED(hr))
    {
        for(DWORD i = 0; i < m_pSensorList.GetCount(); i++)
        {
            CSensor *pSensor;
            
            POSITION pos = NULL;
            pos = m_pSensorList.FindIndex(i);

            if (NULL != pos)
            {
                pSensor = m_pSensorList.GetAt(pos);

                if(nullptr != pSensor)
                {
                    SetState(pSensor, SENSOR_STATE_NO_DATA);
                }
                else
                {
                    hr = E_UNEXPECTED;
                }
            }
            else
            {
                hr = E_UNEXPECTED;
            }
        }
    }
    
    if (SUCCEEDED(hr))
    {  
        // Step 1: Create the Data Changed Event Handle
        m_hSensorEvent = ::CreateEvent(  NULL,        // No security attributes
                                        FALSE,       // Automatic-reset event object
                                        FALSE,       // Initial state is non-signaled
                                        NULL  );     // Unnamed object
     
        POSITION pos = NULL;
        CSensor* pSensor = nullptr;

        for(DWORD i = 0; i < m_pSensorList.GetCount(); i++)
        {
            pos = m_pSensorList.FindIndex(i);
            if (NULL != pos)
            {
                pSensor = m_pSensorList.GetAt(pos);
                if (nullptr != pSensor)
                {
                    pSensor->SetDataEventHandle(m_hSensorEvent);
                }
                else
                {
                    hr = E_UNEXPECTED;
                }
            }
            else
            {
                hr = E_UNEXPECTED;
            }
        }

        // Step 2: Activate & Create and start the eventing thread
        if (SUCCEEDED(hr))
        {
            Activate();

            m_hSensorManagerEventingThread = ::CreateThread(NULL,                                              // Cannot be inherited by child process
                                                         0,                                                   // Default stack size
                                                         &CSensorManager::_SensorEventThreadProc,   // Thread proc
                                                         (LPVOID)this,                                        // Thread proc argument
                                                         0,                                                   // Starting state = running
                                                         NULL);

            if (nullptr == m_hSensorManagerEventingThread)
            {
                Trace(TRACE_LEVEL_ERROR, "%!FUNC! sensor event thread failed to create");
                hr = HRESULT_FROM_WIN32(::GetLastError());
            }
            else
            {
                Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! sensor event thread successfully created");
            }

        }// No thread identifier

        if(SUCCEEDED(hr))
        {
            m_fSensorManagerInitialized = TRUE;
            Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Sensor initialization completed successfully");
        }
        else
        {
            Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Sensor initialization failed");
        }
    }


    return hr;
}

/////////////////////////////////////////////////////////////////////////////////
//
// CSensorManager::Stop
//
//
//
//
//////////////////////////////////////////////////////////////////////////////////
HRESULT CSensorManager::Stop()
{
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    HRESULT hr = S_OK;

    // Update each sensor state to SENSOR_STATE_NO_DATA
    for(DWORD i = 0; i < m_pSensorList.GetCount(); i++)
    {
        CSensor *pSensor = nullptr;
        CComBSTR objectId;
        POSITION pos = NULL;

        pos = m_pSensorList.FindIndex(i);
        if (NULL != pos)
        {
            pSensor = m_pSensorList.GetAt(pos);
        
            if(nullptr != pSensor)
            {
                SetState(pSensor, SENSOR_STATE_NO_DATA);
            }
            else
            {
                hr = E_UNEXPECTED;
            }
        }
        else
        {
            hr = E_UNEXPECTED;
        }
    }
    
    // DeInitialize the sensor device
    if(NULL != m_pSensorDDI)
    {
        hr = m_pSensorDDI->DeInitSensorDevice();
    }

    // Step 1: Stop the eventing thread and Close the handle
    if (NULL != m_hSensorManagerEventingThread)
    {
        // De-activate and close the thread
        DeActivate();
        WaitForSingleObject(m_hSensorManagerEventingThread, INFINITE);
        CloseHandle(m_hSensorManagerEventingThread);
    }
   
    // Step 2: Close the Data Change Event handle
    if (NULL != m_hSensorEvent)
    {
        CloseHandle(m_hSensorEvent);
    }

    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Exit, hr = %!HRESULT!", hr);

    return hr;

}

/////////////////////////////////////////////////////////////////////////////////
//
// CSensorManager::_SensorEventThreadProc
//
//
//
//
//////////////////////////////////////////////////////////////////////////////////
DWORD WINAPI CSensorManager::_SensorEventThreadProc(_In_ LPVOID pvData)
{
    CSensorManager* pParent = (CSensorManager*)pvData;

    if (NULL == pParent)
    {
        return 0;
    }

    // Cast the argument to the correct type.
    CSensorManager* pThis = static_cast<CSensorManager*>(pvData);

    HRESULT hr = CoInitializeEx(NULL, COINIT_MULTITHREADED);

    if (FAILED(hr))
    {
        Trace(TRACE_LEVEL_ERROR, "Failed to call CoInitialize on _SensorEventThreadProc thread, hr = %!HRESULT!", hr);
        return 0;
    }
    
    // Create the data event parameters collection if it doesn't exist
    CComPtr<IPortableDeviceValues> spDataEventParams;
    if (spDataEventParams == NULL)
    {
        hr = CoCreateInstance(CLSID_PortableDeviceValues,
                              NULL,
                              CLSCTX_INPROC_SERVER,
                              IID_PPV_ARGS(&spDataEventParams));
    }

    if (FAILED(hr))
    {
        Trace(TRACE_LEVEL_ERROR, "Failed to CoCreateInstance for Data Event Parameters, hr = %!HRESULT!", hr);
        return 0;
    }
    
    // Create the shake event parameters collection if it doesn't exist
    CComPtr<IPortableDeviceValues> spShakeEventParams;
    if (spShakeEventParams == NULL)
    {
        hr = CoCreateInstance(CLSID_PortableDeviceValues,
                              NULL,
                              CLSCTX_INPROC_SERVER,
                              IID_PPV_ARGS(&spShakeEventParams));
    }

    if (FAILED(hr))
    {
        Trace(TRACE_LEVEL_ERROR, "Failed to CoCreateInstance for Shake Event Parameters, hr = %!HRESULT!", hr);
        return 0;
    }
    else
    {
        while (pParent->IsActive() &&
               (WAIT_OBJECT_0 == WaitForSingleObject(pParent->GetSensorEventHandle(), INFINITE)))
        {    
            hr = pThis->EnterProcessing(PROCESSING_ISENSOREVENT);

            // Loop through every sensor and post an event for each one if needed
            if (S_OK == hr)
            {
                for (DWORD i = 0; i < pThis->m_pSensorList.GetCount(); i++)
                {
                    CSensor *pSensor = nullptr;
                    CComBSTR objectId = NULL;
                    POSITION pos = NULL;
                
                    pos = pThis->m_pSensorList.FindIndex(i);

                    if (NULL != pos)
                    {
                        pSensor = pThis->m_pSensorList.GetAt(pos);

                        if (nullptr != pSensor)
                        {
                            // Check if this Sensor has valid data to post
                            if ( TRUE == pSensor->HasValidDataEvent() )
                            {
                                // Initialize the event parameters
                                spDataEventParams->Clear();

                                // Populate the event type
                                hr = spDataEventParams->SetGuidValue(SENSOR_EVENT_PARAMETER_EVENT_ID, SENSOR_EVENT_DATA_UPDATED);

                                if (SUCCEEDED(hr))
                                {
                                    // Update sensor state to ready if needed
                                    hr = pParent->SetState(pSensor, SENSOR_STATE_READY);
                                }

                                // Get the All the Data Field values
                                // Populate the event parameters
                                if (SUCCEEDED(hr))
                                {
                                    hr = pSensor->GetAllDataFieldValues(spDataEventParams);
                                }

                                if ( SUCCEEDED(hr) )
                                {                            
                                    objectId = pSensor->GetSensorObjectID();
                                    pParent->PostDataEvent(objectId, spDataEventParams);
                                }
                            }

                            // Check if this is an accelerometer and if there was a shake event
                            SensorType sensType = pSensor->GetSensorType();
                            if (Accelerometer == sensType)
                            {
                                CAccelerometer* pAccelerometer = static_cast<CAccelerometer*>(pSensor);
                                if (S_OK == hr)
                                {
                                    // Initialize the event parameters
                                    spShakeEventParams->Clear();

                                    // Populate the event type
                                    hr = spShakeEventParams->SetGuidValue(SENSOR_EVENT_PARAMETER_EVENT_ID, SENSOR_EVENT_ACCELEROMETER_SHAKE);
                                }
                                if (SUCCEEDED(hr))
                                {
                                    if ( TRUE == pAccelerometer->HasValidShakeEvent() )
                                    {
                                        // Update sensor state to ready if needed
                                        hr = pParent->SetState(pSensor, SENSOR_STATE_READY);
                    
                                        // Get the All the Data Field values
                                        // Populate the event parameters
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = pSensor->GetAllDataFieldValues(spShakeEventParams);
                                        }

                                        if ( SUCCEEDED(hr) )
                                        {                            
                                            objectId = pSensor->GetSensorObjectID();

                                            if (NULL != objectId)
                                            {
                                                pParent->PostShakeEvent(objectId, spShakeEventParams);
                                            }
                                            else
                                            {
                                                hr = E_UNEXPECTED;        
                                                Trace(TRACE_LEVEL_ERROR, "Failed to get sensor objectID for %s, hr = %!HRESULT!", pSensor->m_SensorName, hr);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //failed to get pSensor
                            hr = E_UNEXPECTED;        
                            Trace(TRACE_LEVEL_ERROR, "Failed to get pSensor from SensorList, hr = %!HRESULT!", hr);
                        }
                    }
                    else
                    {
                        hr = E_UNEXPECTED;
                        Trace(TRACE_LEVEL_ERROR, "Failed to get sensor index from SensorList, hr = %!HRESULT!", hr);
                    }
                }
            }

            pThis->ExitProcessing(PROCESSING_ISENSOREVENT);
        }
    }
    
    CoUninitialize();

    return 0;
}

/////////////////////////////////////////////////////////////////////////////////
//
// CSensorManager::SetState
//
//
//
//
//////////////////////////////////////////////////////////////////////////////////
HRESULT CSensorManager::SetState(_In_ LPVOID pvData, _In_ SensorState newState)
{

    HRESULT hr = S_OK;

    CSensor* pSensor = static_cast<CSensor*>(pvData);

    if (nullptr != pSensor)
    {
        CComBSTR objectId = NULL;
        objectId = pSensor->GetSensorObjectID();

        if (NULL != objectId)
        {
            SensorState currentState;
            DWORD dwValue = 0;

            PROPVARIANT var;
            PropVariantInit(&var);

            hr = pSensor->GetProperty(SENSOR_PROPERTY_STATE, &var);

            if (SUCCEEDED(hr))
            {
                PropVariantToUInt32(var, &dwValue);
                currentState = (SensorState)dwValue;

                if ((currentState != newState) || (TRUE == pSensor->m_fSensorStateChanged))
                {
                    PropVariantClear(&var);
                    InitPropVariantFromUInt32(newState, &var);

                    hr = pSensor->SetProperty(SENSOR_PROPERTY_STATE, &var, nullptr);

                    if (SUCCEEDED(hr) && (nullptr != m_spClassExtension))
                    {
                        hr = m_spClassExtension->PostStateChange(objectId, newState);

                        if (SUCCEEDED(hr))
                        {
                            switch(newState)
                            {
                            case SENSOR_STATE_READY:
                                Trace(TRACE_LEVEL_INFORMATION, "Setting %s state, State = READY", pSensor->m_SensorName);
                                if ((FALSE == pSensor->m_fSensorPropertiesPreviouslyUpdated) || (TRUE == pSensor->m_fSensorStateChanged))
                                {
                                    m_pSensorDDI->UpdateSensorPropertyValues(pSensor->m_SensorID, FALSE);
                                    Trace(TRACE_LEVEL_INFORMATION, "Updating %s property values because of state change -> READY", pSensor->m_SensorName);
                                }
                                break;
                            case SENSOR_STATE_NOT_AVAILABLE:
                                Trace(TRACE_LEVEL_INFORMATION, "Setting %s state, State = NOT_AVAILABLE", pSensor->m_SensorName);
                                break;
                            case SENSOR_STATE_NO_DATA:
                                Trace(TRACE_LEVEL_INFORMATION, "Setting %s state, State = NO_DATA", pSensor->m_SensorName);
                                break;
                            case SENSOR_STATE_INITIALIZING:
                                Trace(TRACE_LEVEL_INFORMATION, "Setting %s state, State = INITIALIZING", pSensor->m_SensorName);
                                break;
                            case SENSOR_STATE_ACCESS_DENIED:
                                Trace(TRACE_LEVEL_INFORMATION, "Setting %s state, State = ACCESS_DENIED", pSensor->m_SensorName);
                                break;
                            case SENSOR_STATE_ERROR:
                                Trace(TRACE_LEVEL_INFORMATION, "Setting %s state, State = ERROR", pSensor->m_SensorName);
                                break;
                            default:
                                Trace(TRACE_LEVEL_INFORMATION, "Setting %s state, State = UNKNOWN", pSensor->m_SensorName);
                                break;
                            }

                            if (SENSOR_STATE_READY != newState)
                            {
                                DWORD cDatafields = 0;

                                hr = pSensor->m_spSupportedSensorDataFields->GetCount(&cDatafields);

                                if (SUCCEEDED(hr))
                                {
                                    PROPERTYKEY pkDfKey = {0};
                                    PROPVARIANT value;

                                    Trace(TRACE_LEVEL_INFORMATION, "Setting %s datafield values = VT_EMPTY", pSensor->m_SensorName);
                                    for (DWORD dwIdx = 0; dwIdx < cDatafields; dwIdx++)
                                    {
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = pSensor->m_spSupportedSensorDataFields->GetAt(dwIdx, &pkDfKey);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            PropVariantInit( &value );
                                            value.vt = VT_EMPTY;

                                            pSensor->m_spSensorDataFieldValues->SetValue(pkDfKey, &value);

                                            PropVariantClear( &value );
                                        }
                                    }
                                }

                                pSensor->m_fSensorPropertiesPreviouslyUpdated = FALSE;
                            }
                        }
                    }
                }
            }

            PropVariantClear(&var);
            pSensor->m_fSensorStateChanged = FALSE;
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to get sensor ObjectID for %s, hr = %!HRESULT!", pSensor->m_SensorName, hr);
        }

    }
    else
    {
        hr = E_UNEXPECTED;
        Trace(TRACE_LEVEL_ERROR, "pSensor == nullptr, hr = %!HRESULT!", hr);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////////////
//
// CSensorManager::PostDataEvent
//
//
//
//
//////////////////////////////////////////////////////////////////////////////////
HRESULT CSensorManager::PostDataEvent(_In_ LPWSTR objectId, IPortableDeviceValues *pValues)
{

    HRESULT hr = S_OK;

    CComPtr<IPortableDeviceValuesCollection> spEventCollection;

    if(spEventCollection == NULL)
    {
        //*--  CoCreate a collection to store the sensor object identifiers.
        hr = CoCreateInstance( CLSID_PortableDeviceValuesCollection,
                                NULL,
                                CLSCTX_INPROC_SERVER,
                                IID_PPV_ARGS(&spEventCollection));
    }

    if( SUCCEEDED(hr) )
    {
        hr = spEventCollection->Add( pValues );
        if( SUCCEEDED(hr) && (m_spClassExtension != NULL) )
        {
            hr = m_spClassExtension->PostEvent( objectId, spEventCollection );            
        }
    }


    return hr;
}

/////////////////////////////////////////////////////////////////////////////////
//
// CSensorManager::PostShakeEvent
//
//
//
//
//////////////////////////////////////////////////////////////////////////////////
HRESULT CSensorManager::PostShakeEvent(_In_ LPWSTR objectId, IPortableDeviceValues *pValues)
{
    Trace(TRACE_LEVEL_VERBOSE, "%!FUNC! Entry");

    HRESULT hr = S_OK;

    CComPtr<IPortableDeviceValuesCollection> spEventCollection;

    if(spEventCollection == NULL)
    {
        //*--  CoCreate a collection to store the sensor object identifiers.
        hr = CoCreateInstance( CLSID_PortableDeviceValuesCollection,
                                NULL,
                                CLSCTX_INPROC_SERVER,
                                IID_PPV_ARGS(&spEventCollection));
    }

    if( SUCCEEDED(hr) )
    {
        hr = spEventCollection->Add( pValues );
        if( SUCCEEDED(hr) && (m_spClassExtension != NULL) )
        {
            hr = m_spClassExtension->PostEvent( objectId, spEventCollection );            
        }
    }


    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorManager::OnCleanupFile
//
// This method is called when the file handle to the device is closed
//
// Parameters:
//      pWdfFile - pointer to a file object
//
/////////////////////////////////////////////////////////////////////////
void CSensorManager::CleanupFile(
    _In_ IWDFFile* pWdfFile
    )
{
    if (NULL != m_spClassExtension)
    {
        m_spClassExtension->CleanupFile(pWdfFile);
    }

    return;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorManager::ProcessIoControl
//
// This method is called to process a Device IO Control
//
// Parameters:
//      pRequest - [in] pointer to the request
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorManager::ProcessIoControl(
    _In_ IWDFIoRequest*   pRequest
    )
{

    HRESULT hr = S_OK;

    if (NULL != m_spClassExtension)
    {
        hr = m_spClassExtension->ProcessIoControl(pRequest);
    }

    return hr;
}

VOID CSensorManager::DeActivate()
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    m_fThreadActive = FALSE; 
    SetEvent(m_hSensorEvent);
    return;
}

VOID CSensorManager::Activate()
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    m_fThreadActive = TRUE; 

    return;
}

/*
    Trims spaces and non-printable characters from the start and end of the string
*/
HRESULT CSensorManager::StringTrim(_Inout_updates_(HID_USB_DESCRIPTOR_MAX_LENGTH) LPWSTR pwszString)
{
    HRESULT hr = S_OK;

    size_t length = 0;
    hr = StringCchLength(pwszString, HID_USB_DESCRIPTOR_MAX_LENGTH, &length);

    if (SUCCEEDED(hr))
    {
        ULONG idx = 0;
        wchar_t* str = nullptr;

        try 
        {
#pragma warning(suppress: 6014) //OACR does not recognize "delete[] original" below as deleting "str"
            str = new wchar_t[length + 1];
        }
        catch(...)
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to allocate memory for string trim, hr = %!HRESULT!", hr);

            if (nullptr != str) 
            {
                delete[] str;
            }
        }

        if (SUCCEEDED(hr) && (length > 0))
        {
            wchar_t* original = str;
            SecureZeroMemory(str, (length + 1) * sizeof(wchar_t));

            hr = StringCchCopy(str, length + 1, pwszString);
    
            if (SUCCEEDED(hr))
            {
                wchar_t *end;  
                // Trim leading space
                idx = 0;
                while(iswspace(*str) && (idx < length))
                {
                    str++;
                    idx++;
                }

                if( 0 == *str )
                {
                    hr = StringCchCopy(pwszString, HID_USB_DESCRIPTOR_MAX_LENGTH, L"");
                }
                else
                {
                    // Trim trailing space  
                    end = str + wcslen(str) - 1;
                    while(end > str && iswspace(*end))
                    {
                        end--;
                    }
                    // Write new null terminator  
                    *(end+1) = 0;  

                    hr = StringCchCopy(pwszString, HID_USB_DESCRIPTOR_MAX_LENGTH, str);
                }
 
                delete[] original;
            }
        }
        else
        {
            hr = E_OUTOFMEMORY;
            Trace(TRACE_LEVEL_ERROR, "str == nullptr, hr = %!HRESULT!", hr);
        }
    }

    return hr;
}

inline HRESULT CSensorManager::EnterProcessing(DWORD64 dwControlFlag)
{
    return m_pDevice->EnterProcessing(dwControlFlag);
}

inline void CSensorManager::ExitProcessing(DWORD64 dwControlFlag)
{
    m_pDevice->ExitProcessing(dwControlFlag);
}


