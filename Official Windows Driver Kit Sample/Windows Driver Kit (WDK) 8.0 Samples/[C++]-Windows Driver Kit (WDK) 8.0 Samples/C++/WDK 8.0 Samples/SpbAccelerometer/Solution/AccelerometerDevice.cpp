/*++

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

Copyright (c) Microsoft Corporation. All rights reserved

Module Name:

    AccelerometerDevice.cpp

Abstract:

    This module contains the implementation of the SPB accelerometer's
    accelerometer device class.

--*/


#include "Internal.h"
#include "Adxl345.h"
#include "Device.h"

#include <float.h>
#include "strsafe.h"

#include "AccelerometerDevice.h"
#include "AccelerometerDevice.tmh"

// Array of settings that describe the initial
// device configuration.
const REGISTER_SETTING g_ConfigurationSettings[] =
{
    // Standby mode
    {ADXL345_POWER_CTL, 
        ADXL345_POWER_CTL_STANDBY},
    // +-16g, 13-bit resolution
    {ADXL345_DATA_FORMAT, 
        ADXL345_DATA_FORMAT_FULL_RES |
        ADXL345_DATA_FORMAT_JUSTIFY_RIGHT |
        ADXL345_DATA_FORMAT_RANGE_16G},
    // No FIFO
    {ADXL345_FIFO_CTL, 
        ADXL345_FIFO_CTL_MODE_BYPASS},
    // Data rate set to default
    {ADXL345_BW_RATE, 
        _GetDataRateFromReportInterval(DEFAULT_ACCELEROMETER_CURRENT_REPORT_INTERVAL).RateCode},
    // Activity threshold set to
    // default change sensitivity
    {ADXL345_THRESH_ACT, 
        (BYTE)(DEFAULT_ACCELEROMETER_CHANGE_SENSITIVITY / 
        ACCELEROMETER_CHANGE_SENSITIVITY_RESOLUTION)},
    // Activity detection enabled, AC coupled
    {ADXL345_ACT_INACT_CTL, 
        ADXL345_ACT_INACT_CTL_ACT_ACDC |
        ADXL345_ACT_INACT_CTL_ACT_X |
        ADXL345_ACT_INACT_CTL_ACT_Y |
        ADXL345_ACT_INACT_CTL_ACT_Z},
    // Activity interrupt mapped to pin 1
    {ADXL345_INT_MAP, 
        ADXL345_INT_ACTIVITY},
};


/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::CAccelerometerDevice()
//
//  Constructor
//
/////////////////////////////////////////////////////////////////////////
CAccelerometerDevice::CAccelerometerDevice() :
    m_pSensorDeviceCallback(nullptr),
    m_spRequest(nullptr),
    m_pSpbRequest(nullptr),
    m_pDataBuffer(nullptr),
    m_fInitialized(FALSE),
    m_InterruptsEnabled(0),
    m_TestRegister(0),
    m_TestDataSize(0)
{

}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::~CAccelerometerDevice()
//
//  Destructor
//
/////////////////////////////////////////////////////////////////////////
CAccelerometerDevice::~CAccelerometerDevice()
{
    // Stop the device from measuring data
    // if it isn't already
    SetDeviceStateStandby();

    SAFE_RELEASE(m_pSpbRequest);

    if (m_pDataBuffer != nullptr)
    {
        delete[] m_pDataBuffer;
        m_pDataBuffer = nullptr;
    }
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::Initialize
//
//  This method is used to initialize the accelerometer device object
//  and its child objects.
//
//  Parameters:
//      pDevice - pointer to a device object
//      pWdfResourcesRaw - pointer the raw resource list
//      pWdfResourcesTranslated - pointer to the translated resource list
//      pSensorDeviceCallback - pointer to the sensor device callback
//          interface
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAccelerometerDevice::Initialize(
    _In_ IWDFDevice* pWdfDevice,
    _In_ IWDFCmResourceList* pWdfResourcesRaw,
    _In_ IWDFCmResourceList* pWdfResourcesTranslated,
    _In_ ISensorDeviceCallback* pSensorDeviceCallback
    )
{
    FuncEntry();

    HRESULT hr = S_OK;
    LARGE_INTEGER requestId;

    requestId.QuadPart = 0;

    if (m_fInitialized == FALSE)
    {
        if ((pWdfDevice == nullptr) ||
            (pSensorDeviceCallback == nullptr))
        {
            hr = E_INVALIDARG;
        }
        else
        {
            // Save weak reference to callback
            m_pSensorDeviceCallback = pSensorDeviceCallback;
        }

        if (SUCCEEDED(hr))
        {
            // Get the simulated hardware settings
            hr = GetSimulatedHwSettings(
                pWdfDevice,
                &m_SimulatedHwSettings);
        }

        if (SUCCEEDED(hr))
        {
            // Parse the driver's resources to get the
            // resource hub connection IDs.
            hr = ParseResources(
                pWdfDevice,
                pWdfResourcesRaw, 
                pWdfResourcesTranslated,
                &requestId);
        }

        if (SUCCEEDED(hr))
        {
            // Create and initialize the request object
            hr = InitializeRequest(pWdfDevice, requestId);
        }

        if (SUCCEEDED(hr))
        {
            // Mark the sensor device as initialized
            m_fInitialized = TRUE;
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::GetSimulatedHwSettings()
//
//  This method is used to retrieve the simulated hw settings from
//  the registry.
//
//  Parameters:
//      pSettings - pointer to a simulate hw settings structure
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAccelerometerDevice::GetSimulatedHwSettings(
    _In_  IWDFDevice* pWdfDevice,
    _Out_ PSIMULATED_HW_SETTINGS pSettings
    )
{
    FuncEntry();

    HRESULT hr = S_OK;

    if ((pWdfDevice == nullptr) ||
        (pSettings == nullptr))

    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        CComPtr<IWDFPropertyStoreFactory> pPropStoreFactory;
        CComPtr<IWDFNamedPropertyStore2> pPropStore;
        WDF_PROPERTY_STORE_ROOT RootSpecifier;

        // Get the factory interface
        hr = pWdfDevice->QueryInterface(IID_PPV_ARGS(&pPropStoreFactory));

        if (SUCCEEDED(hr))
        {
            // Initialize the WDF_PROPERTY_STORE_ROOT strucutre to the
            // \Device Parameters subkey under the device's hardware key
            RtlZeroMemory(&RootSpecifier, sizeof(WDF_PROPERTY_STORE_ROOT));
            RootSpecifier.LengthCb = sizeof(WDF_PROPERTY_STORE_ROOT);
            RootSpecifier.RootClass = WdfPropertyStoreRootClassHardwareKey;
            RootSpecifier.Qualifier.HardwareKey.ServiceName =
                WDF_PROPERTY_STORE_HARDWARE_KEY_ROOT;

            // Get the property store interface for the device's hardware key
            hr = pPropStoreFactory->RetrieveDevicePropertyStore(
                &RootSpecifier,
                WdfPropertyStoreNormal,
                KEY_QUERY_VALUE,
                nullptr,
                &pPropStore,
                nullptr);
        }

        if (SUCCEEDED(hr))
        {
            LPCWSTR key = L"SimulatedHwSequenceDelay";
            PROPVARIANT var;

            PropVariantInit(&var);

            // Get the SimulatedHwSequenceDelay value
            HRESULT hrTemp = pPropStore->GetNamedValue(key, &var);

            if (SUCCEEDED(hrTemp) && (var.vt == VT_UI4) && (var.ulVal > 0))
            {
                // Value found, use simulated hardware
                Trace(
                    TRACE_LEVEL_INFORMATION,
                    "Using simulated hardware with a sequence "
                    "delay of %lu",
                    var.ulVal);

                pSettings->fUseSimulatedHw = TRUE;
                pSettings->SequenceDelayInUs = var.ulVal;
            }
            else
            {
                // Value not found, don't use simulated hardware
                Trace(
                    TRACE_LEVEL_INFORMATION,
                    "Not using simulated hardware");

                pSettings->fUseSimulatedHw = FALSE;
                pSettings->SequenceDelayInUs = 0;
            }

        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::ParseResources
//
//  This method is used to parse the device's resources and save the
//  the important ones.
//
//  Parameters:
//      pWdfDevice - pointer to the device object
//      pWdfResourcesRaw - pointer the raw resource list
//      pWdfResourcesTranslated - pointer to the translated resource list
//      pRequestId - pointer to the obtained request's resource hub
//          connection id
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAccelerometerDevice::ParseResources(
    _In_  IWDFDevice* pWdfDevice,
    _In_  IWDFCmResourceList* pWdfResourcesRaw,
    _In_  IWDFCmResourceList* pWdfResourcesTranslated,
    _Out_ LARGE_INTEGER* pRequestId
    )
{
    FuncEntry();

    HRESULT hr = S_OK;

    PCM_PARTIAL_RESOURCE_DESCRIPTOR pDescriptor;
    PCM_PARTIAL_RESOURCE_DESCRIPTOR pDescriptorRaw;
    
    ULONG resourceCount;
    BOOLEAN fRequestFound = FALSE;
    BOOLEAN fInterruptFound = FALSE;
    UCHAR connectionClass;
    UCHAR connectionType;

    UNREFERENCED_PARAMETER(pWdfResourcesRaw);

    if (pWdfResourcesTranslated == nullptr)
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        resourceCount = pWdfResourcesTranslated->GetCount();

        // Loop through the resources and save the relevant ones
        for (ULONG i = 0; i < resourceCount; i++)
        {
            pDescriptor = pWdfResourcesTranslated->GetDescriptor(i);
            pDescriptorRaw = pWdfResourcesRaw->GetDescriptor(i);

            if ((pDescriptor == nullptr) ||
                (pDescriptorRaw == nullptr))
            {
                hr = E_POINTER;
                break;
            }

            switch (pDescriptor->Type)
            {
                case CmResourceTypeConnection:

                    // Check against the expected connection types
                    connectionClass = pDescriptor->u.Connection.Class;
                    connectionType = pDescriptor->u.Connection.Type;

                    if ((connectionClass == CM_RESOURCE_CONNECTION_CLASS_SERIAL) &&
                        (connectionType == CM_RESOURCE_CONNECTION_TYPE_SERIAL_I2C))
                    {
                        if (fRequestFound == FALSE)
                        {
                            // Save the request id
                            pRequestId->LowPart =
                                pDescriptor->u.Connection.IdLowPart;
                            pRequestId->HighPart =
                                pDescriptor->u.Connection.IdHighPart;

                            fRequestFound = TRUE;
                        }
                        else
                        {
                            hr = HRESULT_FROM_WIN32(ERROR_INVALID_PARAMETER);
                            Trace(
                                TRACE_LEVEL_ERROR,
                                "Duplicate resource found - %!HRESULT!",
                                hr);
                        }
                    }

                    break;

                case CmResourceTypeInterrupt:
                    
                    if (fInterruptFound == FALSE)
                    {
                        hr = ConnectInterrupt(
                            pWdfDevice,
                            pDescriptorRaw,
                            pDescriptor);

                        if (SUCCEEDED(hr))
                        {
                            fInterruptFound = TRUE;
                        }
                    }
                    else
                    {
                        Trace(
                            TRACE_LEVEL_WARNING,
                            "Duplicate interrupt resource found, ignoring");
                    }

                    break;

                default:

                    // Ignore all other descriptors
                    break;
            }

            if (FAILED(hr))
            {
                break;
            }
        }

        if (SUCCEEDED(hr) &&
            ((fRequestFound == FALSE) || (fInterruptFound == FALSE)))
        {
            hr = HRESULT_FROM_WIN32(ERROR_RESOURCE_NOT_PRESENT);
            Trace(
                TRACE_LEVEL_ERROR,
                "Failed to find required resource - %!HRESULT!",
                hr);
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::InitializeRequest
//
//  This method is used to initialize the request object.
//
//  Parameters:
//      pWdfDevice - pointer to the device object
//      id - the resource hub connection id
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAccelerometerDevice::InitializeRequest(
    _In_  IWDFDevice* pWdfDevice,
    _In_  LARGE_INTEGER id
    )
{
    FuncEntry();

    HRESULT hr = S_OK;

    if (pWdfDevice == nullptr)
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        // Create the request object
        hr = CComObject<CSpbRequest>::CreateInstance(
            &m_pSpbRequest);
        
        if (SUCCEEDED(hr))
        {
            m_pSpbRequest->AddRef();

            hr = m_pSpbRequest->QueryInterface(
                IID_PPV_ARGS(&m_spRequest));
        }

        // TODO: CoCreateInstance rather than calling
        //       CreateInstance on the class and querying
        //       the required interface.

        //// Create the request object
        //hr = CoCreateInstance(
        //    __uuidof(SpbRequest), // CLSID_SpbRequest
        //    nullptr,
        //    CLSCTX_INPROC_SERVER,
        //    IID_PPV_ARGS(&m_spRequest));

        if (FAILED(hr))
        {
            Trace(
                TRACE_LEVEL_ERROR,
                "Failed to create the request object, %!HRESULT!", 
                hr);
        }
        
        if (SUCCEEDED(hr))
        {
            WCHAR DevicePathBuffer[RESOURCE_HUB_PATH_CHARS] = {0};
            UNICODE_STRING DevicePath;

            DevicePath.Buffer = DevicePathBuffer;
            DevicePath.Length = 0;
            DevicePath.MaximumLength = RESOURCE_HUB_PATH_SIZE;

            // Create the device path using the well known
            // resource hub path and the connection ID
            hr = HRESULT_FROM_NT( RESOURCE_HUB_CREATE_PATH_FROM_ID(
                &DevicePath,
                id.LowPart,
                id.HighPart));

            // Initialize the request object
            if (SUCCEEDED(hr))
            {
                // NULL-terminate the buffer
                DevicePathBuffer[RESOURCE_HUB_PATH_CHARS - 1] = L'\0';

                hr = m_spRequest->Initialize(
                    pWdfDevice,
                    DevicePathBuffer);
            }
        }

        if (SUCCEEDED(hr))
        {
            // Create the data buffer
            m_pDataBuffer = new BYTE[ADXL345_DATA_REPORT_SIZE_BYTES];

            if (m_pDataBuffer == nullptr)
            {
                hr = E_OUTOFMEMORY;
            }
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::ConnectInterrupt
//
//  This method is used to connect the data notification interrupt.
//
//  Parameters:
//      pWdfDevice - pointer to the device object
//      RawResource - raw resource decriptor
//      TranslatedResource - translated resource descriptor
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAccelerometerDevice::ConnectInterrupt(
    _In_     IWDFDevice* pWdfDevice,
    _In_opt_ PCM_PARTIAL_RESOURCE_DESCRIPTOR RawResource,
    _In_opt_ PCM_PARTIAL_RESOURCE_DESCRIPTOR TranslatedResource
    )
{
    FuncEntry();
    
    CComPtr<IWDFDevice3> pIWDFDevice3;
    CComPtr<IWDFInterrupt> spInterrupt;
    HRESULT hr = S_OK;

    if (pWdfDevice == nullptr)
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        hr = pWdfDevice->QueryInterface(IID_PPV_ARGS(&pIWDFDevice3));
    }

    if (SUCCEEDED(hr))
    {
        // Create interrupt
        WUDF_INTERRUPT_CONFIG config;
        WUDF_INTERRUPT_CONFIG_INIT(
            &config,
            CAccelerometerDevice::OnInterruptIsr,
            CAccelerometerDevice::OnInterruptWorkItem);
        
        config.InterruptRaw = RawResource;
        config.InterruptTranslated = TranslatedResource;

        hr = pIWDFDevice3->CreateInterrupt(&config, &spInterrupt);

        if (FAILED(hr))
        {
            Trace(
                TRACE_LEVEL_ERROR,
                "Failed to create interrupt object, %!HRESULT!",
                hr);
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = spInterrupt->AssignContext(nullptr, (void*)this);

        if (FAILED(hr))
        {
            Trace(
                TRACE_LEVEL_ERROR,
                "Failed to assign interrupt context, %!HRESULT!",
                hr);
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::ConfigureHardware
//
//  This method is used to place device in standy mode and configure it
//  for operation.
//
//  Parameters:
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAccelerometerDevice::ConfigureHardware()
{
    FuncEntry();

    HRESULT hr = (m_fInitialized == TRUE) ? S_OK : E_UNEXPECTED;

    if (SUCCEEDED(hr))
    {
        // Allocate the data buffers
        BYTE* pWriteBuffer = new BYTE[1];
        BYTE* pReadBuffer = new BYTE[1];

        if ((pWriteBuffer == nullptr) ||
            (pReadBuffer == nullptr))
        {
            hr = E_OUTOFMEMORY;
        }

        {
            // Synchronize access to device
            CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

            // Loop through configuration values and set registers
            for (DWORD i = 0; i < ARRAY_SIZE(g_ConfigurationSettings); i++)
            {
                if (SUCCEEDED(hr))
                {
                    REGISTER_SETTING setting = g_ConfigurationSettings[i];

                    // Write the configuration value to the register
                    pWriteBuffer[0] = setting.Value;
                    hr = WriteRegister(setting.Register, pWriteBuffer, 1);

                    if (SUCCEEDED(hr))
                    {
                        // Confirm register value
                        hr = ReadRegister(
                            setting.Register, 
                            pReadBuffer, 
                            1,
                            m_SimulatedHwSettings.SequenceDelayInUs);


                        if (SUCCEEDED(hr) &&
                            (pReadBuffer[0] != pWriteBuffer[0]))
                        {
                            // The register value is incorrect. Record the error
                            // and break out of the loop.
                            Trace(
                                TRACE_LEVEL_ERROR,
                                "Unexpected value at register 0x%02x: "
                                "expected 0x%02x, got 0x%02x",
                                setting.Register,
                                pWriteBuffer[0],
                                pReadBuffer[0]);

                            hr = E_UNEXPECTED;

                            break;
                        }
                    }
                }
            }
        }

        if (SUCCEEDED(hr))
        {
            Trace(
                TRACE_LEVEL_INFORMATION,
                "Accelerometer device configured");
        }

        // Delete the buffer allocations
        if (pWriteBuffer != nullptr)
        {
            delete[] pWriteBuffer;
        }

        if (pReadBuffer != nullptr)
        {
            delete[] pReadBuffer;
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::SetDataUpdateMode
//
//  This method is used to set the data update mode.
//
//  Parameters:
//      Mode - new data update mode
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAccelerometerDevice::SetDataUpdateMode(
    _In_  DATA_UPDATE_MODE Mode
    )
{
    FuncEntry();

    HRESULT hr = (m_fInitialized == TRUE) ? S_OK : E_UNEXPECTED;

    if (SUCCEEDED(hr))
    {
        switch (Mode)
        {
            case DataUpdateModeOff:
                hr = SetDeviceStateStandby();
                break;

            case DataUpdateModePolling:
                hr = SetDeviceStatePolling();
                break;

            case DataUpdateModeEventing:
                hr = SetDeviceStateEventing();
                break;

            default:
                hr = E_INVALIDARG;
                break;
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::SetDeviceStateStandby
//
//  This method is used to place the device in standby mode.
//
//  Parameters:
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAccelerometerDevice::SetDeviceStateStandby()
{
    FuncEntry();

    HRESULT hr = S_OK;

    // Allocate the data buffer
    BYTE* pBuffer = new BYTE[1];

    if (pBuffer == nullptr)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        // Synchronize access to device
        CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

        // Disable interrupts
        pBuffer[0] = 0;
        hr = WriteRegister(ADXL345_INT_ENABLE, pBuffer, 1);

        if (FAILED(hr))
        {
            Trace(
                TRACE_LEVEL_ERROR,
                "Failed to disable interrupts, %!HRESULT!", 
                hr);
        }
        else
        {
            m_InterruptsEnabled = pBuffer[0];
        }
            
        // Clear any stale interrupts
        if (SUCCEEDED(hr))
        {
            hr = ReadRegister(
                ADXL345_INT_SOURCE, 
                pBuffer, 
                1, 
                m_SimulatedHwSettings.SequenceDelayInUs);

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to read interrupt source register, %!HRESULT!", 
                    hr);
            }
        }
                
        // Place device in standy mode
        {
            pBuffer[0] = ADXL345_POWER_CTL_STANDBY;
            hr = WriteRegister(ADXL345_POWER_CTL, pBuffer, 1);

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to place device in standby mode, %!HRESULT!", 
                    hr);
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        Trace(
            TRACE_LEVEL_INFORMATION,
            "Device in standby mode");
    }
    else
    {
        Trace(
            TRACE_LEVEL_WARNING,
            "Unexpected failure while stopping accelerometer device, "
            "%!HRESULT!", 
            hr);
    }
        
    // Delete the buffer allocation
    if (pBuffer != nullptr)
    {
        delete[] pBuffer;
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::SetDeviceStatePolling
//
//  This method is used to place the device in measurement mode without
//  eventing.
//
//  Parameters:
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAccelerometerDevice::SetDeviceStatePolling()
{
    FuncEntry();

    HRESULT hr = S_OK;

    // Allocate the data buffer
    BYTE* pBuffer = new BYTE[1];

    if (pBuffer == nullptr)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        // Synchronize access to device
        CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

        // Disable interrupts
        pBuffer[0] = 0;
        hr = WriteRegister(ADXL345_INT_ENABLE, pBuffer, 1);

        if (FAILED(hr))
        {
            Trace(
                TRACE_LEVEL_ERROR,
                "Failed to disable interrupts, %!HRESULT!", 
                hr);
        }
        else
        {
            m_InterruptsEnabled = pBuffer[0];
        }
            
        // Place device in measurement mode
        if (SUCCEEDED(hr))
        {
            pBuffer[0] = ADXL345_POWER_CTL_MEASURE;
            hr = WriteRegister(ADXL345_POWER_CTL, pBuffer, 1);

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to enable measurement mode, %!HRESULT!", 
                    hr);
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        Trace(
            TRACE_LEVEL_INFORMATION,
            "Device in measurement mode (polling)");
    }
        
    // Delete the buffer allocation
    if (pBuffer != nullptr)
    {
        delete[] pBuffer;
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::SetDeviceStateEventing
//
//  This method is used to place the device in measurement mode with
//  eventing.
//
//  Parameters:
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAccelerometerDevice::SetDeviceStateEventing()
{
    FuncEntry();

    HRESULT hr = S_OK;

    // Allocate the data buffer
    BYTE* pBuffer = new BYTE[1];

    if (pBuffer == nullptr)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        // Synchronize access to device
        CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

        pBuffer[0] = ADXL345_POWER_CTL_MEASURE;
        hr = WriteRegister(ADXL345_POWER_CTL, pBuffer, 1);

        if (FAILED(hr))
        {
            Trace(
                TRACE_LEVEL_ERROR,
                "Failed to enable measurement mode, %!HRESULT!", 
                hr);
        }

        // Enable activity detection interrupt
        if (SUCCEEDED(hr))
        {
            pBuffer[0] = ADXL345_INT_ACTIVITY;
            hr = WriteRegister(ADXL345_INT_ENABLE, pBuffer, 1);

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to enable activity interrupt, %!HRESULT!", 
                    hr);
            }
            else
            {
                m_InterruptsEnabled = pBuffer[0];
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        Trace(
            TRACE_LEVEL_INFORMATION,
            "Device in measurement mode (eventing)");
    }
        
    // Delete the buffer allocation
    if (pBuffer != nullptr)
    {
        delete[] pBuffer;
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::SetReportInterval
//
//  This method is used to set the report interval of the device.
//
//  Parameters:
//      ReportInterval - desired report interval
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAccelerometerDevice::SetReportInterval(
    _In_  ULONG ReportInterval
    )
{
    FuncEntry();

    HRESULT hr = (m_fInitialized == TRUE) ? S_OK : E_UNEXPECTED;

    if (SUCCEEDED(hr))
    {
        DATA_RATE newDataRate;

        // The accelerometer only supports a subset of data rates.
        // Pick the rate that is just less than the desired report
        // interval.
        newDataRate = _GetDataRateFromReportInterval(ReportInterval);

        // Allocate the data buffer
        BYTE* pWriteBuffer = new BYTE[1];

        if (pWriteBuffer == nullptr)
        {
            hr = E_OUTOFMEMORY;
        }

        if (SUCCEEDED(hr))
        {
            // Synchronize access to device
            CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

            // Disable interrupts while data rate is modified
            pWriteBuffer[0] = 0;
            hr = WriteRegister(ADXL345_INT_ENABLE, pWriteBuffer, 1);

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to disable interrupts, %!HRESULT!", 
                    hr);
            }

            if (SUCCEEDED(hr))
            {
                // Update the data rate.
                pWriteBuffer[0] = newDataRate.RateCode;
                hr = WriteRegister(ADXL345_BW_RATE, pWriteBuffer, 1);

                if (FAILED(hr))
                {
                    Trace(
                        TRACE_LEVEL_ERROR,
                        "Failed to update data rate, %!HRESULT!", 
                        hr);
                }
                else
                {
                    Trace(
                        TRACE_LEVEL_INFORMATION,
                        "Data rate interval set to %d ms",
                        newDataRate.DataRateInterval);
                }
            }

            if (SUCCEEDED(hr))
            {
                // Reenable interrupts
                pWriteBuffer[0] = ADXL345_INT_ACTIVITY;
                hr = WriteRegister(ADXL345_INT_ENABLE, pWriteBuffer, 1);
                
                if (FAILED(hr))
                {
                    Trace(
                        TRACE_LEVEL_ERROR,
                        "Failed to enable activity interrupt, %!HRESULT!", 
                        hr);
                }
            }
        }

        // Delete the buffer allocation
        if (pWriteBuffer != nullptr)
        {
            delete[] pWriteBuffer;
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::SetChangeSensitivity
//
//  This method is used to set the change sensitivity of the device.
//
//  Parameters:
//      pVar - value containing change sensitivities
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAccelerometerDevice::SetChangeSensitivity(
    _In_  PROPVARIANT* pVar
    )
{
    FuncEntry();

    HRESULT hr = (m_fInitialized == TRUE) ? S_OK : E_UNEXPECTED;
   
    BYTE newThreshold = 0x00;

    if (SUCCEEDED(hr))
    {
        if (pVar == nullptr)
        {
            hr = E_INVALIDARG;
        }
    }

    if (SUCCEEDED(hr))
    {
        // Change sensitivity is a per data field
        // property and is stored as an IPortableDeviceValues
        if (pVar->vt != VT_UNKNOWN)
        {
            hr = E_INVALIDARG;
        }

        if (SUCCEEDED(hr))
        {
            CComPtr<IPortableDeviceValues> spPerDataFieldValues;

            spPerDataFieldValues =
                static_cast<IPortableDeviceValues*>(pVar->punkVal);

            DWORD count = 0;

            if (SUCCEEDED(hr))
            {
                // Get the count of change sensitivity values.
                hr = spPerDataFieldValues->GetCount(&count);
            }

            if (SUCCEEDED(hr))
            {
                // The accelerometer only supports a single value, so
                // pick the smallest, i.e. most sensitive.        
                DOUBLE minChangeSensitivity = DBL_MAX;

                for (DWORD i = 0; i < count; i++)
                {
                    PROPERTYKEY key;
                    PROPVARIANT var;

                    PropVariantInit(&var);

                    hr = spPerDataFieldValues->GetAt(i, &key, &var);

                    if (SUCCEEDED(hr))
                    {
                        if ((var.vt == VT_R8) &&
                            (var.dblVal < minChangeSensitivity))
                        {
                            minChangeSensitivity = var.dblVal;
                        }
                    }

                    PropVariantClear(&var);
                }

                if (SUCCEEDED(hr))
                {
                    // The threshold can only be set in increments, so
                    // round down to a more sensitive setting
                    newThreshold = (BYTE)(minChangeSensitivity /
                        ACCELEROMETER_CHANGE_SENSITIVITY_RESOLUTION);
                }
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        // Allocate the data buffer
        BYTE* pWriteBuffer = new BYTE[1];

        if (pWriteBuffer == nullptr)
        {
            hr = E_OUTOFMEMORY;
        }

        if (SUCCEEDED(hr))
        {
            // Synchronize access to device
            CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

            // Disable interrupts while activity
            // threshold is modified
            pWriteBuffer[0] = 0;
            hr = WriteRegister(ADXL345_INT_ENABLE, pWriteBuffer, 1);

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to disable interrupts, %!HRESULT!", 
                    hr);
            }

            if (SUCCEEDED(hr))
            {
                // Update the activity detection threshold.
                pWriteBuffer[0] = newThreshold;
                hr = WriteRegister(ADXL345_THRESH_ACT, pWriteBuffer, 1);

                if (FAILED(hr))
                {
                    Trace(
                        TRACE_LEVEL_ERROR,
                        "Failed to update activity threshold, %!HRESULT!", 
                        hr);
                }
                else
                {
                    Trace(
                        TRACE_LEVEL_INFORMATION,
                        "Activity threshold set to 0x%02x",
                        newThreshold);
                }
            }

            if (SUCCEEDED(hr))
            {
                // Reenable interrupts
                pWriteBuffer[0] = ADXL345_INT_ACTIVITY;
                hr = WriteRegister(ADXL345_INT_ENABLE, pWriteBuffer, 1);
                
                if (FAILED(hr))
                {
                    Trace(
                        TRACE_LEVEL_ERROR,
                        "Failed to enable activity interrupt, %!HRESULT!", 
                        hr);
                }
            }
        }

        // Delete the buffer allocation
        if (pWriteBuffer != nullptr)
        {
            delete[] pWriteBuffer;
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::RequestNewData
//
//  This method is used to synchronously request new data from the device.
//
//  Parameters:
//      ppValues - an IPortableDeviceValues pointer that receives new data
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAccelerometerDevice::RequestNewData(
    _In_ IPortableDeviceValues* pValues
    )
{
    FuncEntry();

    HRESULT hr = (m_fInitialized == TRUE) ? S_OK : E_UNEXPECTED;

    if (SUCCEEDED(hr) && (pValues == nullptr))
    {
        hr = E_INVALIDARG;
    }
    
    if (SUCCEEDED(hr))
    {
        hr = RequestData(pValues);
    
        if (FAILED(hr))
        {
            Trace(
                TRACE_LEVEL_ERROR,
                "Failed to request data synchronously, "
                "%!HRESULT!",
                hr);
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::GetTestProperty
//
//  This method is used to get one of the device's test properties.
//
//  Parameters:
//      key - the test property key
//      pVar - location for the value of the test property key
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAccelerometerDevice::GetTestProperty(
        _In_  REFPROPERTYKEY key, 
        _Out_ PROPVARIANT* pVar)
{
    FuncEntry();

    HRESULT hr = (m_fInitialized == TRUE) ? S_OK : E_UNEXPECTED;

    if (SUCCEEDED(hr) && (pVar == nullptr))
    {
        hr = E_INVALIDARG;
    }
    
    if (SUCCEEDED(hr))
    {
        if(IsEqualPropertyKey(key,
            SENSOR_PROPERTY_TEST_REGISTER))
        {
            InitPropVariantFromUInt32(m_TestRegister, pVar);
        }
        else if(IsEqualPropertyKey(key,
            SENSOR_PROPERTY_TEST_DATA_SIZE))
        {
            InitPropVariantFromUInt32(m_TestDataSize, pVar);
        }
        else if(IsEqualPropertyKey(key,
            SENSOR_PROPERTY_TEST_DATA))
        {
            BYTE* pDataBuffer = (BYTE*)CoTaskMemAlloc(m_TestDataSize);

            if (pDataBuffer == nullptr)
            {
                hr = E_OUTOFMEMORY;
            }

            if (SUCCEEDED(hr))
            {
                // Synchronize access to device
                CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

                hr = ReadRegister(
                    (BYTE)m_TestRegister,
                    pDataBuffer,
                    m_TestDataSize,
                    m_SimulatedHwSettings.SequenceDelayInUs);

                if (SUCCEEDED(hr))
                {
                    pVar->vt = (VT_VECTOR | VT_UI1);
                    pVar->caub.cElems = m_TestDataSize;                
                    pVar->caub.pElems = pDataBuffer;
                }
            }
        }
        else
        {
            hr = HRESULT_FROM_WIN32(ERROR_NOT_FOUND);
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::SetTestProperty
//
//  This method is used to set one of the device's test properties.
//
//  Parameters:
//      key - the test property key
//      pVar - pointer to the key value
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAccelerometerDevice::SetTestProperty(
        _In_  REFPROPERTYKEY key, 
        _In_  PROPVARIANT* pVar)
{
    FuncEntry();

    HRESULT hr = (m_fInitialized == TRUE) ? S_OK : E_UNEXPECTED;

    if (SUCCEEDED(hr) && (pVar == nullptr))
    {
        hr = E_INVALIDARG;
    }
    
    if (SUCCEEDED(hr))
    {
        // Synchronize access to device
        CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

        if(IsEqualPropertyKey(key,
            SENSOR_PROPERTY_TEST_REGISTER))
        {
            m_TestRegister = pVar->ulVal;
        }
        else if(IsEqualPropertyKey(key,
            SENSOR_PROPERTY_TEST_DATA_SIZE))
        {
            m_TestDataSize = pVar->ulVal;
        }
        else if(IsEqualPropertyKey(key,
            SENSOR_PROPERTY_TEST_DATA))
        {
            hr = WriteRegister(
                (BYTE)m_TestRegister,
                pVar->caub.pElems,
                m_TestDataSize);
        }
        else
        {
            hr = HRESULT_FROM_WIN32(ERROR_NOT_FOUND);
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::AddDataFieldValue
//
//  This method is used to validate each data field value and add it to
//  the specified IPortableDeviceValues instance.
//
//  Parameters:
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAccelerometerDevice::AddDataFieldValue(
    _In_  REFPROPERTYKEY         key, 
    _In_  PROPVARIANT*           pVar,
    _Out_ IPortableDeviceValues* pValues
    )
{
    FuncEntry();

    HRESULT hr = S_OK;

    if (pValues == nullptr)
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        if (IsEqualPropertyKey(key, SENSOR_DATA_TYPE_ACCELERATION_X_G) ||
            IsEqualPropertyKey(key, SENSOR_DATA_TYPE_ACCELERATION_Y_G) ||
            IsEqualPropertyKey(key, SENSOR_DATA_TYPE_ACCELERATION_Z_G))
        {
            if (pVar->vt != VT_R8)
            {
                hr = E_INVALIDARG;
            }

            if (SUCCEEDED(hr))
            {
                if ((pVar->dblVal < ACCELEROMETER_MIN_ACCELERATION_G) ||
                    (pVar->dblVal > ACCELEROMETER_MAX_ACCELERATION_G))
                {
                    hr = E_INVALIDARG;
                }
            }
        }
    }

    if (SUCCEEDED(hr))
    {

        hr = pValues->SetValue(key, pVar);
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::OnInterruptIsr
//
//  This method is called when an interrupt occurs.  It determines if the
//  driver owns the interrupt and queues a work item to defer processing
//  of the data.
//
//  Parameters:
//      pInterrupt - pointer to the interrupt object
//      MessageID - interrupt message ID
//      Reserved - 
//
//  Return Values:
//      TRUE if interrupt recognized, else FALSE.
//
/////////////////////////////////////////////////////////////////////////
BOOLEAN
CAccelerometerDevice::OnInterruptIsr(
    _In_ IWDFInterrupt* pInterrupt,
    _In_ ULONG MessageID,
    _In_ ULONG Reserved
    )
{
    FuncEntry();

    UNREFERENCED_PARAMETER(MessageID);
    UNREFERENCED_PARAMETER(Reserved);

    CComPtr<IWDFDevice> pWdfDevice = nullptr;
    CAccelerometerDevice* pAccelerometerDevice;
    BOOLEAN interruptRecognized = FALSE;
    HRESULT hr;

    hr = pInterrupt->RetrieveContext((void**)&pAccelerometerDevice);

    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR,
            "Failed to retrieve device context, "
            "reporting device failure, %!HRESULT!",
            hr);
        
        pWdfDevice = pInterrupt->GetDevice();
        pWdfDevice->SetPnpState(WdfPnpStateFailed, WdfTrue);
        pWdfDevice->CommitPnpState();
    }

    if (SUCCEEDED(hr) && (pAccelerometerDevice != nullptr))
    {
        BYTE interruptSource = 0;
        BYTE validInterrupts = 0;;

        {
            // Synchronize access to device
            CComCritSecLock<CComAutoCriticalSection> scopeLock(
                pAccelerometerDevice->m_CriticalSection);

            // Read the interrupt source register to 
            // check for relevant interrupt. Doing so clears
            // the interrupt.
            hr = pAccelerometerDevice->ReadRegister(
                ADXL345_INT_SOURCE,
                &interruptSource,
                sizeof(interruptSource),
                0);

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to read INT_SOURCE register, %!HRESULT!",
                    hr);
            }

            if (SUCCEEDED(hr))
            {
                // Throw away any interrupts that are not enabled.
                validInterrupts = interruptSource & 
                        pAccelerometerDevice->m_InterruptsEnabled;

                if ((interruptSource > 0) && (validInterrupts == 0))
                {
                    Trace(
                        TRACE_LEVEL_INFORMATION,
                        "Interrupt detected with INT_SOURCE=0x%x but "
                        "INT_ENABLE=0x%x, treating as unrecognized",
                        interruptSource,
                        pAccelerometerDevice->m_InterruptsEnabled);
                }
            }
        }

        if (SUCCEEDED(hr))
        {
            // Confirm that an activity interrupt was fired
            if ((validInterrupts & ADXL345_INT_ACTIVITY) > 0)
            {
                interruptRecognized = TRUE;
        
                //
                // NOTE:
                // It is best practice when handling interrupts to quickly
                // service the interrupt in the ISR and then queue a work item
                // to retrieve and process the data.
                //

                BOOLEAN workItemQueued = pInterrupt->QueueWorkItemForIsr();

                Trace(
                    TRACE_LEVEL_VERBOSE,
                    "Work item %s queued for interrupt",
                    workItemQueued ? "" : "already ");
            }
        }
    }

    FuncExit();

    return interruptRecognized;
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::OnInterruptWorkitem
//
//  This method is called on behalf of an interrupt to defer processing.  
//  It retrieves latest data and posts it.
//
//  Parameters:
//      pInterrupt - pointer to the interrupt object
//      AssociatedObject - pointer to the associated object
//
//  Return Values:
//      None.
//
/////////////////////////////////////////////////////////////////////////
VOID
CAccelerometerDevice::OnInterruptWorkItem(
    _In_ IWDFInterrupt* pInterrupt,
    _In_ IWDFObject* AssociatedObject
    )
{
    FuncEntry();

    UNREFERENCED_PARAMETER(AssociatedObject);

    CComPtr<IWDFDevice> pWdfDevice = nullptr;
    CAccelerometerDevice* pAccelerometerDevice;
    HRESULT hr;

    hr = pInterrupt->RetrieveContext((void**)&pAccelerometerDevice);

    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR,
            "Failed to retrieve device context, "
            "reporting device failure, %!HRESULT!",
            hr);
        
        pWdfDevice = pInterrupt->GetDevice();
        pWdfDevice->SetPnpState(WdfPnpStateFailed, WdfTrue);
        pWdfDevice->CommitPnpState();
    }

    if (SUCCEEDED(hr) && (pAccelerometerDevice != nullptr))
    {
        CComPtr<IPortableDeviceValues> spValues = nullptr;

        // Create an IPortableDeviceValues to hold the data
        hr = CoCreateInstance(
            CLSID_PortableDeviceValues,
            nullptr,
            CLSCTX_INPROC_SERVER,
            IID_PPV_ARGS(&spValues));
                
        if (SUCCEEDED(hr))
        {
            hr = pAccelerometerDevice->RequestData(spValues);
    
            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to request data in interrupt work item, "
                    "%!HRESULT!",
                    hr);
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = pAccelerometerDevice->PostData(spValues);
    
            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to post new data to the DDI, "
                    "%!HRESULT!",
                    hr);
            }
        }
    }

    FuncExit();
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::RequestData
//
//  This method is used to request data from the device.
//
//  Parameters:
//      pValues - an IPortableDeviceValues collection to place the list of 
//          new data field values
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAccelerometerDevice::RequestData(
    _In_ IPortableDeviceValues * pValues
    )
{
    FuncEntry();

    HRESULT hr = m_fInitialized ? S_OK : E_UNEXPECTED;

    if (SUCCEEDED(hr))
    {
        // Synchronize access to device
        CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

        // Read the data registers asynchronously
        hr = ReadRegister(
            ADXL345_DATA_X0,
            m_pDataBuffer,
            ADXL345_DATA_REPORT_SIZE_BYTES,
            0);

        if (FAILED(hr))
        {
            Trace(
                TRACE_LEVEL_ERROR,
                "Failed to read new data from device, %!HRESULT!",
                hr);
        }

        if (SUCCEEDED(hr))
        {
            // Get the data values as doubles
            SHORT xRaw, yRaw, zRaw;
            DOUBLE xAccel, yAccel, zAccel;
            const DOUBLE scaleFactor = 1/256.0F;

            xRaw = (SHORT)((m_pDataBuffer[1] << 8) | m_pDataBuffer[0]);
            yRaw = (SHORT)((m_pDataBuffer[3] << 8) | m_pDataBuffer[2]);
            zRaw = (SHORT)((m_pDataBuffer[5] << 8) | m_pDataBuffer[4]);

            xAccel = (DOUBLE)xRaw * scaleFactor;
            yAccel = (DOUBLE)yRaw * scaleFactor;
            zAccel = (DOUBLE)zRaw * scaleFactor;

            // Verify each accelerometer data value and 
            // add it to the list
            if (SUCCEEDED(hr))
            {
                PROPVARIANT var;
                PropVariantInit(&var);
                var.vt = VT_R8;

                var.dblVal = xAccel;
                hr = AddDataFieldValue(
                    SENSOR_DATA_TYPE_ACCELERATION_X_G,
                    &var,
                    pValues);

                if (SUCCEEDED(hr))
                {
                    var.dblVal = yAccel;
                    hr = AddDataFieldValue(
                        SENSOR_DATA_TYPE_ACCELERATION_Y_G,
                        &var,
                        pValues);
                }

                if (SUCCEEDED(hr))
                {
                    var.dblVal = zAccel;
                    hr = AddDataFieldValue(
                        SENSOR_DATA_TYPE_ACCELERATION_Z_G,
                        &var,
                        pValues);
                }

                PropVariantClear(&var);
            }
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::PostData
//
//  This method is used to post new data to the DDI.
//  to the DDI.
//
//  Parameters:
//      pValues - an IPortableDeviceValues collection to place the list of 
//          new data field values
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAccelerometerDevice::PostData(
    _In_ IPortableDeviceValues * pValues
    )
{
    FuncEntry();

    HRESULT hr = m_fInitialized ? S_OK : E_UNEXPECTED;

    // Post the 
    if (SUCCEEDED(hr))
    {
        m_pSensorDeviceCallback->OnNewData(pValues);
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::ReadRegister
//
//  This method is used to read a buffer of data from the device's register
//  interface.  The first byte is read from the specified register, the second
//  from the next, etc.
//
//  Parameters:
//      reg - the first register to be read from
//      pDataBuffer - pointer to the output buffer
//      dataBufferLength - size of the output buffer
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAccelerometerDevice::ReadRegister(
    _In_                            BYTE   reg,
    _Out_writes_(dataBufferLength)  BYTE*  pDataBuffer,
    _In_                            ULONG  dataBufferLength,
    _In_                            ULONG  delayInUs
    )
{
    FuncEntry();

    HRESULT hr = (m_fInitialized == TRUE) ? S_OK : E_UNEXPECTED;

    if (SUCCEEDED(hr) && pDataBuffer == nullptr)
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        Trace(
            TRACE_LEVEL_VERBOSE,
            "Read %lu bytes from register 0x%02x",
            dataBufferLength,
            reg);

        // Execute the write-read sequence
        hr = m_spRequest->CreateAndSendWriteReadSequence(
            &reg,
            1,
            pDataBuffer,
            dataBufferLength,
            delayInUs);
    }

    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR,
            "Failed to read from register 0x%02x, %!HRESULT!",
            reg,
            hr);
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CAccelerometerDevice::WriteRegister
//
//  This method is used to write a buffer of data to the device's register
//  interface.  The first byte is written to the specified register, the 
//  second from the next, etc.
//
//  Parameters:
//      reg - the first register to be written to
//      pDataBuffer - pointer to the output buffer
//      dataBufferLength - size of the output buffer
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAccelerometerDevice::WriteRegister(
    _In_                           BYTE   reg,
    _In_reads_(dataBufferLength)   BYTE*  pDataBuffer,
    _In_                           ULONG  dataBufferLength
    )
{
    FuncEntry();

    HRESULT hr = (m_fInitialized == TRUE) ? S_OK : E_UNEXPECTED;

    if (SUCCEEDED(hr) && pDataBuffer == nullptr)
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {   
        // A write-write sequence is implemented with
        // a single write request. Allocate a buffer to
        // hold the register and data.
        ULONG bufferLength = dataBufferLength + 1;
        BYTE* pBuffer = new BYTE[bufferLength];

        if (pBuffer == nullptr)
        {
            hr = E_OUTOFMEMORY; 
        }

        if (SUCCEEDED(hr))
        {
            // Fill the buffer
            memcpy(pBuffer, &reg, 1);
            memcpy((pBuffer + 1), pDataBuffer, dataBufferLength);
            
            Trace(
                TRACE_LEVEL_VERBOSE,
                "Write %lu bytes to register 0x%02x",
                dataBufferLength,
                reg);
            
            // Execute the write
            hr = m_spRequest->CreateAndSendWrite(
                pBuffer,
                bufferLength);
        }
           
        // Destroy the allocated buffer
        if (pBuffer != nullptr)
        {
            delete[] pBuffer;
        }

    }

    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR,
            "Failed to write to register 0x%02x, %!HRESULT!",
            reg,
            hr);
    }

    FuncExit();

    return hr;
}
