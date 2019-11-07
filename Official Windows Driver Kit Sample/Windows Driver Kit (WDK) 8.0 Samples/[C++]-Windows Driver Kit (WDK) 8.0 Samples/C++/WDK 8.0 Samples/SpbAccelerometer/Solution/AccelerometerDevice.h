/*++

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

Copyright (c) Microsoft Corporation. All rights reserved

Module Name:

    AccelerometerDevice.h

Abstract:

    This module contains the type definitions for the SPB accelerometer's
    accelerometer device class.

--*/

#ifndef _ACCELEROMETERDEVICE_H_
#define _ACCELEROMETERDEVICE_H_

#pragma once

typedef struct _SIMULATED_HW_SETTINGS
{
    BOOL  fUseSimulatedHw;
    ULONG SequenceDelayInUs;
} SIMULATED_HW_SETTINGS, *PSIMULATED_HW_SETTINGS;

typedef struct _REGISTER_SETTING
{
    BYTE Register;
    BYTE Value;
} REGISTER_SETTING, *PREGISTER_SETTING;

class CAccelerometerDevice :
    //public CComObjectRootEx<CComMultiThreadModel>,
    public CComObjectRoot,
    public CComCoClass<CAccelerometerDevice, &CLSID_AccelerometerDevice>,
    public ISensorDevice
{
public:
    CAccelerometerDevice();
    virtual ~CAccelerometerDevice();

    DECLARE_NO_REGISTRY()
    DECLARE_CLASSFACTORY()
    DECLARE_NOT_AGGREGATABLE(CAccelerometerDevice)

    BEGIN_COM_MAP(CAccelerometerDevice)
        COM_INTERFACE_ENTRY(ISensorDevice)
    END_COM_MAP()

// Public methods
public:
    // Interrupt callbacks
    static WUDF_INTERRUPT_ISR      OnInterruptIsr;
    static WUDF_INTERRUPT_WORKITEM OnInterruptWorkItem;

// COM Interface methods
public:
    // ISensorDevice methods
    HRESULT STDMETHODCALLTYPE Initialize(
        _In_ IWDFDevice* pWdfDevice,
        _In_ IWDFCmResourceList* pWdfResourcesRaw,
        _In_ IWDFCmResourceList* pWdfResourcesTranslated,
        _In_ ISensorDeviceCallback* pSensorDeviceCallback
        );

    HRESULT STDMETHODCALLTYPE ConfigureHardware(
        );

    HRESULT STDMETHODCALLTYPE SetDataUpdateMode(
        _In_  DATA_UPDATE_MODE Mode
        );

    HRESULT STDMETHODCALLTYPE SetReportInterval(
        _In_  ULONG ReportInterval
        );

    HRESULT STDMETHODCALLTYPE SetChangeSensitivity(
        _In_  PROPVARIANT* pVar
        );

    HRESULT STDMETHODCALLTYPE RequestNewData(
        _In_  IPortableDeviceValues* pValues
        );

    HRESULT STDMETHODCALLTYPE GetTestProperty(
        _In_  REFPROPERTYKEY key, 
        _Out_ PROPVARIANT* pVar
        );

    HRESULT STDMETHODCALLTYPE SetTestProperty(
        _In_  REFPROPERTYKEY key, 
        _In_  PROPVARIANT* pVar
        );

//Private methods
private:

    HRESULT SetDeviceStateStandby();
    HRESULT SetDeviceStatePolling();
    HRESULT SetDeviceStateEventing();

    HRESULT AddDataFieldValue(
        _In_  REFPROPERTYKEY         key, 
        _In_  PROPVARIANT*           pVar,
        _Out_ IPortableDeviceValues* pValues
        );

    HRESULT GetSimulatedHwSettings(
        _In_  IWDFDevice* pWdfDevice,
        _Out_ PSIMULATED_HW_SETTINGS pSettings);

    HRESULT ParseResources(
        _In_  IWDFDevice* pWdfDevice,
        _In_  IWDFCmResourceList* pWdfResourcesRaw,
        _In_  IWDFCmResourceList* pWdfResourcesTranslated,
        _Out_ LARGE_INTEGER* pRequestId);

    HRESULT InitializeRequest(
        _In_  IWDFDevice* pWdfDevice,
        _In_  LARGE_INTEGER id);

    HRESULT ConnectInterrupt(
        _In_     IWDFDevice* pWdfDevice,
        _In_opt_ PCM_PARTIAL_RESOURCE_DESCRIPTOR RawResource,
        _In_opt_ PCM_PARTIAL_RESOURCE_DESCRIPTOR TranslatedResource);

    HRESULT RequestData(_In_ IPortableDeviceValues * pValues);
    HRESULT PostData(_In_ IPortableDeviceValues * pValues);

    HRESULT ReadRegister(
        _In_                            BYTE   reg,
        _Out_writes_(dataBufferLength)  BYTE*  pDataBuffer,
        _In_                            ULONG  dataBufferLength,
        _In_                            ULONG  delayInUs
        );
    
    HRESULT WriteRegister(
        _In_                           BYTE   reg,
        _In_reads_(dataBufferLength)   BYTE*  pDataBuffer,
        _In_                           ULONG  dataBufferLength
        );

// Private members
private:
    // Interface pointers
    ISensorDeviceCallback*          m_pSensorDeviceCallback;
    CComPtr<IRequest>               m_spRequest;

    // TODO: Remove these when interfaces are registered
    CComObject<CSpbRequest>*        m_pSpbRequest;

    // The request data buffer and status members
    BYTE*                           m_pDataBuffer;
    CComAutoCriticalSection         m_CriticalSection;

    // Track initialization state of sensor device
    BOOL                            m_fInitialized;
    BYTE                            m_InterruptsEnabled;

    // Test members
    SIMULATED_HW_SETTINGS           m_SimulatedHwSettings;
    ULONG                           m_TestRegister;
    ULONG                           m_TestDataSize;
};

OBJECT_ENTRY_AUTO(__uuidof(AccelerometerDevice), CAccelerometerDevice)

#endif // _ACCELEROMETERDEVICE_H_
