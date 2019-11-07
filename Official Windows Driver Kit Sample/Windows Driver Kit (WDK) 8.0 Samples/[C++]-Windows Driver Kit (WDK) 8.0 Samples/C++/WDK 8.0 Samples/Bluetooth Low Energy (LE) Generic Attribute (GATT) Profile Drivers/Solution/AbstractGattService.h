/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    HealthBloodPressureService.h
    
Abstract:

--*/

#pragma once

class AbstractGattService
{
public:
    AbstractGattService() : 
        RequestFilename(SERVICE_OBJECT_ID),
        m_pDevice(NULL),
        m_hEventSync(NULL),
        m_hrMessagePumpStartStatus(E_FAIL),
        m_hMessagePumpInitialized(NULL),
        m_hMessagePumpTerminate(NULL),
        m_hMessagePumpThread(NULL),
        m_hRadioHandle(NULL),
        m_hDevNotification(NULL)
    {
        InitializeCriticalSection(&m_EventQueueCS);
        InitializeListHead(&m_EventQueueHead);        
    }

    ~AbstractGattService();

    virtual HRESULT Initialize(
        _In_ IWDFDevice* pDevice,
        _In_ BthLEDevice * pBthLEDevice);

    // Capabilities
    HRESULT GetSupportedCommands(
        _Inout_ IPortableDeviceKeyCollection*         pCommands);

    HRESULT GetSupportedMethods(
        _Inout_ IPortableDevicePropVariantCollection* pMethods);

    BOOL IsMethodSupported(
                REFGUID Method);

    HRESULT GetSupportedMethodsByFormat(
                REFGUID                               Format, 
        _Inout_ IPortableDevicePropVariantCollection* pMethods);

    HRESULT GetMethodAttributes(
                REFGUID                               Method, 
        _Inout_ IPortableDeviceValues*                pAttributes);

    HRESULT GetMethodParameterAttributes(
                REFPROPERTYKEY                        Parameter, 
        _Inout_ IPortableDeviceValues*                pAttributes);

    HRESULT GetSupportedFormats(
        _Inout_ IPortableDevicePropVariantCollection* pFormats);

    HRESULT GetFormatAttributes(
                REFGUID                               Format, 
        _Inout_ IPortableDeviceValues*                pAttributes);

    HRESULT GetSupportedFormatProperties(
                REFGUID                               Format, 
        _Inout_ IPortableDeviceKeyCollection*         pKeys);

    HRESULT GetPropertyAttributes(
                REFGUID                               Format,
                REFPROPERTYKEY                        Property,
        _Inout_ IPortableDeviceValues*                pAttributes);

    HRESULT GetSupportedEvents(
        _Inout_ IPortableDevicePropVariantCollection* pEvents);

    HRESULT GetEventAttributes(
                REFGUID                               Event, 
        _Inout_ IPortableDeviceValues*                pAttributes);

    HRESULT GetEventParameterAttributes(
                REFPROPERTYKEY                        Parameter, 
        _Inout_ IPortableDeviceValues*                pAttributes);

    HRESULT GetInheritedServices(
                const DWORD                           dwInheritanceType, 
        _Inout_ IPortableDevicePropVariantCollection* pServices);

    LPWSTR GetRequestFilename()
    {
        return RequestFilename.GetBuffer();
    }

    virtual HRESULT OnMethodInvoke(
        _In_    REFGUID                Method,
        _In_    IPortableDeviceValues* pParams,
        _Inout_ IPortableDeviceValues* pResults);
    
protected:
    //
    // Window process
    //
    static LRESULT s_WndProc(
                    HWND Hwnd, 
                    UINT Msg, 
                    WPARAM Wparam, 
                    LPARAM Lparam);
    
    VOID WndProc(
                    HWND Hwnd, 
                    UINT Msg, 
                    WPARAM Wparam, 
                    LPARAM Lparam);  

    HRESULT StartMessagePump(void);
    
    HRESULT StopMessagePump(void);
    
    static DWORD s_MessagePumpThreadProc( 
                    LPVOID lpv );
    
    DWORD MessagePumpThreadProc(void);    
    

public:
    
    //
    // Supported Items
    //
    EventAttributeInfo * m_SupportedServiceEvents;
    ULONG                m_SupportedServiceEventCount;
    EventParameterAttributeInfo * m_ServiceEventParameters;
    ULONG                         m_ServiceEventParameterCount;
    
    MethodAttributeInfo * m_SupportedServiceMethods;
    ULONG                 m_SupportedServiceMethodCount;
    MethodParameterAttributeInfo * m_ServiceMethodParameters;
    ULONG                          m_ServiceMethodParameterCount;
        
    CAtlStringW             RequestFilename;
    CComPtr<IWDFDevice>     m_pDevice;  
    BthLEDevice *           m_pBthLEDevice;

    CRITICAL_SECTION        m_EventQueueCS;
    LIST_ENTRY              m_EventQueueHead;
    HANDLE                  m_hEventSync;

    HRESULT                 m_hrMessagePumpStartStatus;
    HANDLE                  m_hMessagePumpInitialized;
    HANDLE                  m_hMessagePumpTerminate;
    HANDLE                  m_hMessagePumpThread;
    HWND                    m_hWnd;    

    BTH_ADDR                m_BthDeviceAddress;
    HANDLE                  m_hRadioHandle;
    HDEVNOTIFY              m_hDevNotification;    
};




