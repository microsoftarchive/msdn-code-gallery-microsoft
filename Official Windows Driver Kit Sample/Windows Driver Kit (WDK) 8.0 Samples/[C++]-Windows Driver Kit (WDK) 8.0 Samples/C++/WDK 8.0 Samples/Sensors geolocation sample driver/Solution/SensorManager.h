/*++
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


Module Name:
    SensorManager.h

Abstract:
    Defines the CSensorManager container class

--*/

#pragma once


class CSensorDDI;   //forward declaration
class CSensor;      //forward declaration
class CMyDevice;    //forward declaration

class CSensorManager :
    public CComObjectRoot
{
public:
    CSensorManager();
    virtual ~CSensorManager();

    DECLARE_NOT_AGGREGATABLE(CSensorManager)

    BEGIN_COM_MAP(CSensorManager)
        
    END_COM_MAP()

    HRESULT Initialize(_In_ IWDFDevice* pWdfDevice, _In_ CMyDevice* pDevice);
    void    Uninitialize();
    
    HRESULT Start();
    HRESULT Stop(_In_ WDF_POWER_DEVICE_STATE newState);

    void    CleanupFile(_In_ IWDFFile *pWdfFile);
    HRESULT ProcessIoControl(_In_ IWDFIoRequest*   pRequest);
    HRESULT ProcessIoControlRadioManagement(_In_ IWDFIoRequest* pRequest, _In_ ULONG ControlCode);
    HRESULT SetState(_In_ LPVOID pvData, _In_ SensorState state, _Out_ bool* pfStateChanged);

    HRESULT FindSensorTypeFromObjectID(
        _In_ LPWSTR pwszObjectID, 
        _Out_ SensorType* pSensorType, 
        _Out_ DWORD* pSensorIdx
        );

    inline HRESULT EnterProcessing(DWORD64 dwControlFlag);
    inline void    ExitProcessing(DWORD64 dwControlFlag);

public:
    CComObject<CSensorDDI>*     m_pSensorDDI;
    ULONG                       m_NumMappedSensors;
    WCHAR                       m_wszDeviceName[MAX_PATH];
    SENSOR_LIST                 m_pSensorList;  // the object used by the driver to get sensor data
    SENSOR_ID_MAP               m_AvailableSensorsIDs;  // map of available sensors and their object IDs
    SENSOR_TYPE_MAP             m_AvailableSensorsTypes;    // map of available sensors and their sensor types
    DWORD                       m_fInitializationComplete;
    CMyDevice*                  m_pDevice;
    bool                        m_fDeviceActive;

public:
    //Methods that call into Class Extension
    HRESULT PostDataEvent(_In_ LPWSTR objectId, IPortableDeviceValues *pValues);
    HRESULT PostStateChange(_In_ LPWSTR objectId, _In_ SensorState newState);

private:

    // Private Methods

    HRESULT InitializeClassExtension();
    BOOL    IsInitialized(void) const { return m_fSensorManagerInitialized; }
   
    //Methods that call into Class Extension
    //HRESULT PostDataEvent(_In_ LPWSTR objectId, IPortableDeviceValues *pValues);
       
    // Thread Process that fires data events
    static DWORD WINAPI _SensorEventThreadProc(_In_ LPVOID pvData);

    // Helper functions to activate/deactivate thread
    VOID    Activate();
    VOID    DeActivate();
    BOOL    IsActive()                  { return m_fThreadActive; }
    HANDLE  GetSensorEventHandle(VOID)  { return m_hSensorEvent; }
    SENSOR_LIST* GetSensorListHandle(VOID) { return &m_pSensorList; }
      
    // Private Data Members
    CComPtr<IWDFDevice>             m_spWdfDevice;
    CComPtr<ISensorClassExtension>  m_spClassExtension;


    mutable CComAutoCriticalSection m_CriticalSection;
    BOOL                            m_fSensorManagerInitialized;
    HANDLE                          m_hSensorEvent;
    HANDLE                          m_hSensorManagerEventingThread;       // Thread handle for raising data events
    BOOL                            m_fThreadActive;                    // Flag denoting that driver thread is active
    BOOL                            m_fDeviceStopped;                   // Flag denoting that CSensorManager->Stop() was called
};

