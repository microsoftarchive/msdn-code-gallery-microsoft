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
    HRESULT Stop();

    void    CleanupFile(_In_ IWDFFile *pWdfFile);
    HRESULT ProcessIoControl(_In_ IWDFIoRequest*   pRequest);
 
    HRESULT StringTrim(_Inout_updates_(HID_USB_DESCRIPTOR_MAX_LENGTH) LPWSTR pwszString);
    HRESULT SetState(_In_ LPVOID pvData, _In_ SensorState state);

    inline HRESULT EnterProcessing(DWORD64 dwControlFlag);
    inline void    ExitProcessing(DWORD64 dwControlFlag);

public:
    CComObject<CSensorDDI>*     m_pSensorDDI;
    ULONG                       m_NumMappedSensors;
    WCHAR                       m_wszDeviceName[MAX_PATH];
    WCHAR                       m_wszDeviceId[MAX_PATH];
    SENSOR_LIST                 m_pSensorList;  // the object used by the driver to get sensor data
    SENSOR_ID_MAP               m_AvailableSensorsIDs;  // map of available sensors and their object IDs
    SENSOR_TYPE_MAP             m_AvailableSensorsTypes;    // map of available sensors and their sensor types
    SENSOR_USAGE_MAP            m_AvailableSensorsUsages;    // map of available sensors and their hid usages
    SENSOR_LINKS_MAP            m_AvailableSensorsLinkCollections; // map of available sensors and their link collections
    DWORD                       m_fInitializationComplete;
    PHIDP_PREPARSED_DATA        m_pPreparsedData;
    PHIDP_LINK_COLLECTION_NODE  m_pLinkCollectionNodes;
    ULONG                       m_NumLinkCollectionNodes;
    HIDP_CAPS                   m_HidCaps;
    CMyDevice*                  m_pDevice;

    CComPtr<IWDFDevice>         m_spWdfDevice;

private:

    // Private Methods

    HRESULT InitializeClassExtension();
    BOOL    IsInitialized(void) const { return m_fSensorManagerInitialized; }
   
    //Methods that call into Class Extension
    HRESULT PostDataEvent(_In_ LPWSTR objectId, IPortableDeviceValues *pValues);
    HRESULT PostShakeEvent(_In_ LPWSTR objectId, IPortableDeviceValues *pValues);
       
    // Thread Process that fires data events
    static DWORD WINAPI _SensorEventThreadProc(_In_ LPVOID pvData);

    // Helper functions to activate/deactivate thread
    VOID    Activate();
    VOID    DeActivate();
    BOOL    IsActive()                  { return m_fThreadActive; }
    HANDLE  GetSensorEventHandle(VOID)  { return m_hSensorEvent; }
    SENSOR_LIST* GetSensorListHandle(VOID) { return &m_pSensorList; }
      
    // Private Data Members
    CComPtr<ISensorClassExtension>  m_spClassExtension;


    mutable CComAutoCriticalSection m_CriticalSection;
    BOOL                            m_fSensorManagerInitialized;
    HANDLE                          m_hSensorEvent;
    HANDLE                          m_hSensorManagerEventingThread;       // Thread handle for raising data events
    BOOL                            m_fThreadActive;                    // Flag denoting that driver thread is active
};

