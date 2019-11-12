/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    AbstractGattService.cpp
    
Abstract:

--*/

#include "stdafx.h"

#include "AbstractGattService.tmh"

#define GATT_SERVICE_CLASSNAME L"AbstractGattServiceWindow"

// Supported commands for this service
const PROPERTYKEY* g_ServiceSupportedCommands[] =
{

    // WPD_CATEGORY_OBJECT_PROPERTIES
    &WPD_COMMAND_OBJECT_PROPERTIES_GET_SUPPORTED,
    &WPD_COMMAND_OBJECT_PROPERTIES_GET,
    &WPD_COMMAND_OBJECT_PROPERTIES_GET_ALL,
    &WPD_COMMAND_OBJECT_PROPERTIES_SET,

    // WPD_CATEGORY_SERVICE_COMMON
    &WPD_COMMAND_SERVICE_COMMON_GET_SERVICE_OBJECT_ID,

    // WPD_CATEGORY_SERVICE_CAPABILITIES
    &WPD_COMMAND_SERVICE_CAPABILITIES_GET_SUPPORTED_COMMANDS,
    &WPD_COMMAND_SERVICE_CAPABILITIES_GET_COMMAND_OPTIONS,
    &WPD_COMMAND_SERVICE_CAPABILITIES_GET_SUPPORTED_EVENTS,
    &WPD_COMMAND_SERVICE_CAPABILITIES_GET_EVENT_ATTRIBUTES,
    &WPD_COMMAND_SERVICE_CAPABILITIES_GET_EVENT_PARAMETER_ATTRIBUTES,
    &WPD_COMMAND_SERVICE_CAPABILITIES_GET_SUPPORTED_METHODS,
    &WPD_COMMAND_SERVICE_CAPABILITIES_GET_SUPPORTED_METHODS_BY_FORMAT,
    &WPD_COMMAND_SERVICE_CAPABILITIES_GET_METHOD_ATTRIBUTES,
    &WPD_COMMAND_SERVICE_CAPABILITIES_GET_METHOD_PARAMETER_ATTRIBUTES,
    &WPD_COMMAND_SERVICE_CAPABILITIES_GET_SUPPORTED_FORMATS,
    &WPD_COMMAND_SERVICE_CAPABILITIES_GET_FORMAT_ATTRIBUTES,
    &WPD_COMMAND_SERVICE_CAPABILITIES_GET_SUPPORTED_FORMAT_PROPERTIES,
    &WPD_COMMAND_SERVICE_CAPABILITIES_GET_FORMAT_PROPERTY_ATTRIBUTES,    
};

// Supported Methods
MethodAttributeInfo g_GenericServiceMethods[] = 
{
    {&METHOD_AppActivated, NAME_METHOD_AppActivated, WPD_COMMAND_ACCESS_READWRITE},
    {&METHOD_AppSuspended, NAME_METHOD_AppSuspended, WPD_COMMAND_ACCESS_READWRITE},
};

AbstractGattService::~AbstractGattService()
{
    StopMessagePump();

    if (NULL != m_hRadioHandle)
    {
        CloseHandle(m_hRadioHandle);
        m_hRadioHandle = NULL;
    }

    if (NULL != m_hEventSync)
    {
        CloseHandle(m_hEventSync);
        m_hEventSync = NULL;
    }

    DeleteCriticalSection(&m_EventQueueCS);
}

HRESULT AbstractGattService::Initialize(
    _In_ IWDFDevice* pDevice,
    _In_ BthLEDevice * pBthLEDevice)
{
    HRESULT hr = S_OK;

    m_hEventSync = CreateEvent(NULL, TRUE, FALSE, NULL);
    if (NULL == m_hEventSync)
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
        CHECK_HR(hr, "Failed to create the queue sync event");
    }

    if (SUCCEEDED(hr))
    {
        hr = GetDeviceAddressFromDevice(pDevice, &m_BthDeviceAddress);
    }

    if (SUCCEEDED(hr))
    {
        m_pDevice = pDevice;
        m_pBthLEDevice = pBthLEDevice;

        HBLUETOOTH_RADIO_FIND hRadioFind = NULL;
        BLUETOOTH_FIND_RADIO_PARAMS frp = {sizeof(BLUETOOTH_FIND_RADIO_PARAMS)};

        hRadioFind = BluetoothFindFirstRadio(&frp, &m_hRadioHandle);
        if (NULL != hRadioFind) {
            BluetoothFindRadioClose(hRadioFind);
            hRadioFind = NULL;
        }

        if (NULL == m_hRadioHandle)
        {
            hr = E_FAIL;
            CHECK_HR(hr, "Failed to open a BT radio handle");
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = StartMessagePump();
    }

    return hr;
}


HRESULT AbstractGattService::GetSupportedCommands(
    _Inout_ IPortableDeviceKeyCollection* pCommands)
{
    HRESULT hr = S_OK;

    if(pCommands == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    for (DWORD dwIndex = 0; dwIndex < ARRAYSIZE(g_ServiceSupportedCommands); dwIndex++)
    {
        PROPERTYKEY key = *(g_ServiceSupportedCommands[dwIndex]);
        hr = pCommands->Add(key);
        CHECK_HR(hr, "Failed to add supported command at index %d", dwIndex);
        if (FAILED(hr))
        {
            break;
        }
    }
    return hr;
}


HRESULT AbstractGattService::GetSupportedMethods(
    _Inout_  IPortableDevicePropVariantCollection* pMethods)
{
    HRESULT hr = S_OK;

    if (pMethods == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    PROPVARIANT pv;
    pv.vt = VT_CLSID;

    // Add the generic supported methods
    for (DWORD dwIndex = 0; dwIndex < ARRAYSIZE(g_GenericServiceMethods); dwIndex++)
    {
        if (NULL != g_GenericServiceMethods[dwIndex].pMethodGuid)
        {
            pv.puuid = (CLSID*)g_GenericServiceMethods[dwIndex].pMethodGuid;

            hr = pMethods->Add(&pv);
            CHECK_HR(hr, "Failed to add Method to the collection");
        }        
    }    

    // Add the service specific methods
    for (DWORD i=0; i<m_SupportedServiceMethodCount; i++)
    {
        if (NULL != m_SupportedServiceMethods[i].pMethodGuid)
        {
            pv.puuid = (CLSID*)m_SupportedServiceMethods[i].pMethodGuid;

            hr = pMethods->Add(&pv);
            CHECK_HR(hr, "Failed to add Method to the collection");
        }
    }

    return hr;

}

BOOL AbstractGattService::IsMethodSupported(
    REFGUID Method)
{
    // Add the generic supported methods
    for (DWORD dwIndex = 0; dwIndex < ARRAYSIZE(g_GenericServiceMethods); dwIndex++)
    {
        if (NULL != g_GenericServiceMethods[dwIndex].pMethodGuid &&
            Method == *g_GenericServiceMethods[dwIndex].pMethodGuid)
        {
            return TRUE;
        }        
    }
    
    // Add the supported methods to the collection.
    for (DWORD dwIndex = 0; dwIndex < m_SupportedServiceMethodCount; dwIndex++)
    {
        if (NULL != m_SupportedServiceMethods[dwIndex].pMethodGuid &&
            Method == *m_SupportedServiceMethods[dwIndex].pMethodGuid)
        {
            return TRUE;
        }
    }

    return FALSE;

}

HRESULT AbstractGattService::GetSupportedMethodsByFormat(
            REFGUID                               Format,
    _Inout_ IPortableDevicePropVariantCollection* pMethods)
{
    HRESULT hr = S_OK;

    if (pMethods == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    //
    // We do not support any formats
    //
    hr = HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED);
    CHECK_HR(hr, "Format %ws is not supported", CComBSTR(Format));

    return hr;
}


HRESULT AbstractGattService::GetMethodAttributes(
            REFGUID                Method,
    _Inout_ IPortableDeviceValues* pAttributes)
{
    HRESULT hr = S_OK;
    CComPtr<IPortableDeviceKeyCollection> pParameters;

    if (pAttributes == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    // CoCreate a collection for specifying the method parameters.
    hr = CoCreateInstance(CLSID_PortableDeviceKeyCollection,
                          NULL,
                          CLSCTX_INPROC_SERVER,
                          IID_IPortableDeviceKeyCollection,
                          (VOID**) &pParameters);
    CHECK_HR(hr, "Failed to CoCreate CLSID_PortableDeviceKeyCollection");

    // Loop through the generic methods
    for (DWORD dwIndex = 0; dwIndex < ARRAYSIZE(g_GenericServiceMethods); dwIndex++)
    {
        if (NULL != g_GenericServiceMethods[dwIndex].pMethodGuid &&
            Method == *g_GenericServiceMethods[dwIndex].pMethodGuid)
        {
            hr = pAttributes->SetStringValue(WPD_METHOD_ATTRIBUTE_NAME, g_GenericServiceMethods[dwIndex].wszName);
            CHECK_HR(hr, "Failed to set WPD_METHOD_ATTRIBUTE_NAME");
            if (FAILED(hr)) 
            {
                break;
            }

            hr = pAttributes->SetUnsignedIntegerValue(WPD_METHOD_ATTRIBUTE_ACCESS, g_GenericServiceMethods[dwIndex].ulMethodAttributeAccess);
            CHECK_HR(hr, "Failed to set WPD_METHOD_ATTRIBUTE_ACCESS");  
            if (FAILED(hr)) 
            {
                break;
            }

            // The generic methods do not have any parameters, insert an empty collection
            hr = pAttributes->SetIPortableDeviceKeyCollectionValue(WPD_METHOD_ATTRIBUTE_PARAMETERS, pParameters);
            CHECK_HR(hr, "Failed to set WPD_METHOD_ATTRIBUTE_PARAMETERS");
            if (FAILED(hr))
            {
                break;
            }

            // No associated formats
            hr = pAttributes->SetGuidValue(WPD_METHOD_ATTRIBUTE_ASSOCIATED_FORMAT, GUID_NULL);
            CHECK_HR(hr, "Failed to set WPD_METHOD_ATTRIBUTE_ASSOCIATED_FORMAT");      
            if (FAILED(hr)) 
            {
                break;
            }            
        }
    }        

    // Loop through the service specific methods
    for (DWORD dwIndex = 0; dwIndex < m_SupportedServiceMethodCount; dwIndex++)
    {
        if (NULL != m_SupportedServiceMethods[dwIndex].pMethodGuid &&
            Method == *m_SupportedServiceMethods[dwIndex].pMethodGuid)
        {
            hr = pAttributes->SetStringValue(WPD_METHOD_ATTRIBUTE_NAME, m_SupportedServiceMethods[dwIndex].wszName);
            CHECK_HR(hr, "Failed to set WPD_METHOD_ATTRIBUTE_NAME");
            if (FAILED(hr)) 
            {
                break;
            }

            hr = pAttributes->SetUnsignedIntegerValue(WPD_METHOD_ATTRIBUTE_ACCESS, m_SupportedServiceMethods[dwIndex].ulMethodAttributeAccess);
            CHECK_HR(hr, "Failed to set WPD_METHOD_ATTRIBUTE_ACCESS");  
            if (FAILED(hr)) 
            {
                break;
            }

            hr = SetMethodParameters(Method, &m_ServiceMethodParameters[0], m_ServiceMethodParameterCount, pParameters);
            CHECK_HR(hr, "Failed to set method parameters");
            if (hr == S_OK)
            {
                hr = pAttributes->SetIPortableDeviceKeyCollectionValue(WPD_METHOD_ATTRIBUTE_PARAMETERS, pParameters);
                CHECK_HR(hr, "Failed to set WPD_METHOD_ATTRIBUTE_PARAMETERS");
            }         

            // No associated formats
            hr = pAttributes->SetGuidValue(WPD_METHOD_ATTRIBUTE_ASSOCIATED_FORMAT, GUID_NULL);
            CHECK_HR(hr, "Failed to set WPD_METHOD_ATTRIBUTE_ASSOCIATED_FORMAT");      
            if (FAILED(hr)) 
            {
                break;
            }            
        }
    }    

    return hr;
}

HRESULT AbstractGattService::GetMethodParameterAttributes(
            REFPROPERTYKEY         Parameter,
    _Inout_ IPortableDeviceValues* pAttributes)
{
    HRESULT hr = S_OK;

    if (pAttributes == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    hr = SetMethodParameterAttributes(Parameter, &m_ServiceMethodParameters[0], m_ServiceMethodParameterCount, pAttributes);
    CHECK_HR(hr, "Failed to set method parameter attributes");

    return hr;

}

HRESULT AbstractGattService::GetSupportedFormats(
    _Inout_ IPortableDevicePropVariantCollection* pFormats)
{
    UNREFERENCED_PARAMETER(pFormats);

    return S_OK;
}

HRESULT AbstractGattService::GetFormatAttributes(
            REFGUID                Format,
    _Inout_ IPortableDeviceValues* pAttributes)
{
    UNREFERENCED_PARAMETER(Format);
    UNREFERENCED_PARAMETER(pAttributes);
    
    return S_OK;
}

HRESULT AbstractGattService::GetSupportedFormatProperties(
            REFGUID                       Format,
    _Inout_ IPortableDeviceKeyCollection* pKeys)
{
    UNREFERENCED_PARAMETER(Format);
    UNREFERENCED_PARAMETER(pKeys);

    return S_OK;
}

HRESULT AbstractGattService::GetPropertyAttributes(
            REFGUID                Format,
            REFPROPERTYKEY         Property,
    _Inout_ IPortableDeviceValues* pAttributes)
{
    UNREFERENCED_PARAMETER(Format);
    UNREFERENCED_PARAMETER(Property);
    UNREFERENCED_PARAMETER(pAttributes);

    return S_OK;
}

HRESULT AbstractGattService::GetSupportedEvents(
    _Inout_ IPortableDevicePropVariantCollection* pEvents)
{
    HRESULT hr = S_OK;

    if (pEvents == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    PROPVARIANT pv;
    pv.vt = VT_CLSID;

    for (DWORD i=0; i<m_SupportedServiceEventCount; i++)
    {
        if (NULL != m_SupportedServiceEvents[i].pEventGuid)
        {
            pv.puuid = (CLSID*)m_SupportedServiceEvents[i].pEventGuid;  // Assignment, don't PropVariantClear this

            hr = pEvents->Add(&pv);
            CHECK_HR(hr, "Failed to add event to the collection");
        }
    }

    return hr;
}

HRESULT AbstractGattService::GetEventAttributes(
            REFGUID                Event,
    _Inout_ IPortableDeviceValues* pAttributes)
{
    HRESULT hr = S_OK;
    CComPtr<IPortableDeviceValues> pEventOptions;
    CComPtr<IPortableDeviceKeyCollection> pEventParameters;

    if (pAttributes == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    // CoCreate a collection to store the event options.
    hr = CoCreateInstance(CLSID_PortableDeviceValues,
                          NULL,
                          CLSCTX_INPROC_SERVER,
                          IID_IPortableDeviceValues,
                          (VOID**) &pEventOptions);
    CHECK_HR(hr, "Failed to CoCreate CLSID_PortableDeviceValues");
    if (hr == S_OK)
    {
        hr = pEventOptions->SetBoolValue(WPD_EVENT_OPTION_IS_BROADCAST_EVENT, TRUE);
        CHECK_HR(hr, "Failed to set WPD_EVENT_OPTION_IS_BROADCAST_EVENT");
    }

    // Loop through the supported events for this service to find a match
    hr = HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED);
    for (DWORD i=0; i<m_SupportedServiceEventCount; i++)
    {
        if (NULL != m_SupportedServiceEvents[i].pEventGuid &&
            Event == *m_SupportedServiceEvents[i].pEventGuid)
        {
            // Set the event options.
            hr = pAttributes->SetIPortableDeviceValuesValue(WPD_EVENT_ATTRIBUTE_OPTIONS, pEventOptions);
            CHECK_HR(hr, "Failed to set WPD_EVENT_ATTRIBUTE_OPTIONS");

            // Set the event name.
            if (hr == S_OK)
            {
                hr = pAttributes->SetStringValue(WPD_EVENT_ATTRIBUTE_NAME, m_SupportedServiceEvents[i].wszName);
                CHECK_HR(hr, "Failed to set WPD_EVENT_ATTRIBUTE_NAME");
            }

            // Set the event parameters.
            if (hr == S_OK)
            {
                hr = CoCreateInstance(CLSID_PortableDeviceKeyCollection,
                                      NULL,
                                      CLSCTX_INPROC_SERVER,
                                      IID_IPortableDeviceKeyCollection,
                                      (VOID**) &pEventParameters);
                CHECK_HR(hr, "Failed to CoCreate CLSID_PortableDeviceValues");
            }

            if (hr == S_OK)
            {
                hr = SetEventParameters(Event, &m_ServiceEventParameters[0], m_ServiceEventParameterCount, pEventParameters);
                CHECK_HR(hr, "Failed to set event parameters");

                if (hr == S_OK)
                {
                    hr = pAttributes->SetIPortableDeviceKeyCollectionValue(WPD_EVENT_ATTRIBUTE_PARAMETERS, pEventParameters);
                    CHECK_HR(hr, "Failed to set WPD_METHOD_ATTRIBUTE_PARAMETERS");
                }
            }
            break;
        }
    }

    return hr;
}

//
// Public
//
HRESULT AbstractGattService::GetEventParameterAttributes(
            REFPROPERTYKEY         Parameter,
    _Inout_ IPortableDeviceValues* pAttributes)
{
    HRESULT hr = S_OK;

    if (pAttributes == NULL)
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter");
        return hr;
    }

    hr = SetEventParameterAttributes(Parameter, &m_ServiceEventParameters[0], m_ServiceEventParameterCount, pAttributes);
    CHECK_HR(hr, "Failed to set event parameter attributes");

    return hr;
}

HRESULT AbstractGattService::GetInheritedServices(
            const DWORD                           dwInheritanceType,
    _Inout_ IPortableDevicePropVariantCollection* pServices)
{

    UNREFERENCED_PARAMETER(dwInheritanceType);
    UNREFERENCED_PARAMETER(pServices);

    return S_OK;
}

HRESULT AbstractGattService::OnMethodInvoke(
    _In_    REFGUID                Method,
    _In_    IPortableDeviceValues* pParams,
    _Inout_ IPortableDeviceValues* pResults)
{

    HRESULT hr = S_OK;

    UNREFERENCED_PARAMETER(Method);
    UNREFERENCED_PARAMETER(pParams);
    UNREFERENCED_PARAMETER(pResults);

    UNREFERENCED_PARAMETER(pParams);
    
    if (IsEqualGUID(METHOD_AppActivated, Method))
    {   
        m_pBthLEDevice->DispatchClientApplicationActivated();
    } 
    else if (IsEqualGUID(METHOD_AppSuspended, Method)) 
    {    
        m_pBthLEDevice->DispatchClientApplicationSuspended();    
    } 
    else 
    {
        hr = HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED);
        CHECK_HR(hr, "Unknown method %ws received",CComBSTR(Method));        
    }

    return hr;

}

HRESULT 
AbstractGattService::StartMessagePump()
{
    HRESULT hr = S_OK;
    DEV_BROADCAST_HANDLE dbh = {0};

    //
    // Start a new thread to implement the message pump
    //
    m_hMessagePumpInitialized = CreateEvent(NULL, FALSE, FALSE, NULL);
    if (NULL == m_hMessagePumpInitialized)
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
        CHECK_HR(hr, "Failed to create the message pump initialized event");
    }

    if (SUCCEEDED(hr))
    {
        //
        // Create the terminate event
        //
        m_hMessagePumpTerminate = CreateEvent(NULL, FALSE, FALSE, NULL);
        if (NULL == m_hMessagePumpTerminate)
        {
            hr = HRESULT_FROM_WIN32(GetLastError());
            CloseHandle(m_hMessagePumpInitialized);
            m_hMessagePumpInitialized = NULL;
            CHECK_HR(hr, "Failed to create the message pump terminate event");
        }
    }


    if (SUCCEEDED(hr))
    {
        //
        // Create the thread and wait for it to be initialized
        //
        m_hMessagePumpThread = CreateThread(NULL,
                                        0,
                                        s_MessagePumpThreadProc,
                                        (LPVOID)this,
                                        0,
                                        NULL);
        if (NULL == m_hMessagePumpThread)
        {
            
            hr = HRESULT_FROM_WIN32(GetLastError());
            
            CloseHandle(m_hMessagePumpInitialized);
            m_hMessagePumpInitialized = NULL;
            
            CloseHandle(m_hMessagePumpTerminate);
            m_hMessagePumpTerminate = NULL;
            CHECK_HR(hr, "Failed to create the message pump thread");
        }
    }

    if (SUCCEEDED(hr))
    {
        WaitForSingleObject(m_hMessagePumpInitialized, INFINITE);
        hr = m_hrMessagePumpStartStatus;
        CHECK_HR(hr, "Failed to start the message pump");
    }


    //
    // Register for device change notifications
    //
    if (SUCCEEDED(hr))
    {
        dbh.dbch_size = sizeof(dbh);
        dbh.dbch_devicetype = DBT_DEVTYP_HANDLE;
        dbh.dbch_handle = m_hRadioHandle;
        m_hDevNotification = RegisterDeviceNotification(m_hWnd, &dbh, DEVICE_NOTIFY_WINDOW_HANDLE);
        if (NULL == m_hDevNotification)
        {
            hr = HRESULT_FROM_WIN32(GetLastError());
            CHECK_HR(hr, "Failed to register for device notifications");
        }        
    }

    return hr;
}

HRESULT 
AbstractGattService::StopMessagePump()
{
    HRESULT hr = S_OK;

    //
    // Unregister for device change notifications
    //
    if (NULL != m_hDevNotification)
    {
        UnregisterDeviceNotification(m_hDevNotification);
        m_hDevNotification = NULL;
    }
    
    if (NULL != m_hMessagePumpThread) 
    {

        //
        // Set the terminate event such that the message pump exists
        //
        SetEvent(m_hMessagePumpTerminate);

        //
        // Wait until the message pump thread exists
        //
        WaitForSingleObject(m_hMessagePumpThread, INFINITE);
        
        CloseHandle(m_hMessagePumpThread);
        m_hMessagePumpThread = NULL;
        
        CloseHandle(m_hMessagePumpTerminate);
        m_hMessagePumpTerminate = NULL;
        
        CloseHandle(m_hMessagePumpInitialized);
        m_hMessagePumpInitialized = NULL;
    }

    // destroy our hiden window
    if (NULL != m_hWnd) 
    {
        DestroyWindow(m_hWnd);
        m_hWnd = NULL;
    }

    UnregisterClass(GATT_SERVICE_CLASSNAME, g_hInstance);
    
    return hr;
}


DWORD WINAPI 
AbstractGattService::s_MessagePumpThreadProc(
        LPVOID lpv)
{
    AbstractGattService * pThis = (AbstractGattService *)lpv;
    return pThis->MessagePumpThreadProc();
}

DWORD
AbstractGattService::MessagePumpThreadProc(
        )
{
    DWORD hr = S_OK;
    DWORD dwRet;
    WNDCLASSEX wce;
    
    wce.cbSize = sizeof(WNDCLASSEX);
    wce.style = 0;
    wce.lpfnWndProc = (WNDPROC) s_WndProc;
    wce.cbClsExtra = 0;
    wce.cbWndExtra = 0;
    wce.hInstance = g_hInstance;
    wce.hIcon = NULL;
    wce.hIconSm = NULL;
    wce.hCursor = NULL;
    wce.hbrBackground = NULL;
    wce.lpszMenuName = NULL;
    wce.lpszClassName = GATT_SERVICE_CLASSNAME;

    if (!RegisterClassEx(&wce)) 
    {
        dwRet = GetLastError();
        
        if(ERROR_CLASS_ALREADY_EXISTS != dwRet)
        {
            m_hrMessagePumpStartStatus = HRESULT_FROM_WIN32(dwRet);
            hr = m_hrMessagePumpStartStatus;
            SetEvent(m_hMessagePumpInitialized);
            CHECK_HR(hr, "Failed to register the Window class");
        }
    }

    if (SUCCEEDED(hr))
    {
        m_hWnd = CreateWindow(GATT_SERVICE_CLASSNAME,
                                NULL,
                                WS_OVERLAPPEDWINDOW,
                                0,
                                0,
                                0,
                                0,
                                HWND_MESSAGE, // Message only window, skips UI initialization
                                (HMENU) NULL,
                                g_hInstance,
                                (LPVOID)this);

        if(NULL == m_hWnd)
        {
            hr = HRESULT_FROM_WIN32(GetLastError());
            m_hrMessagePumpStartStatus = hr;
            SetEvent(m_hMessagePumpInitialized);
            CHECK_HR(hr, "Failed to create the window");
        }
    }

    if (SUCCEEDED(hr))
    {
        //
        // Now notify that we are initialized and start the message pump
        //
        m_hrMessagePumpStartStatus = S_OK;
        SetEvent(m_hMessagePumpInitialized);

        DWORD dwWaitVal = (DWORD)-1;

        //
        // Message pump
        //
        do 
        {
            MSG msg;

            while (PeekMessage( &msg, NULL, 0, 0, PM_REMOVE )) 
            {
                TranslateMessage( &msg );
                DispatchMessage( &msg );
            }

            // No timeout.  We keep processing messages, until the wait handle is signaled.
            dwWaitVal = MsgWaitForMultipleObjects( 1, &m_hMessagePumpTerminate, FALSE, INFINITE, QS_ALLINPUT );
            
        } 
        while (WAIT_OBJECT_0 != dwWaitVal);
    }

    return hr;
}

//
// Static redirector 
//
LRESULT
CALLBACK
AbstractGattService::s_WndProc(
    HWND Hwnd,
    UINT Msg,
    WPARAM Wparam,
    LPARAM Lparam
    )
{
    if(WM_CREATE == Msg)
    {

        //
        // save off the instance pointer
        //
        if (NULL != Lparam)
        {
            void * pthis = reinterpret_cast<CREATESTRUCT *>(Lparam)->lpCreateParams;
            SetWindowLongPtr(Hwnd,
                             GWLP_USERDATA,
                             reinterpret_cast<LONG_PTR>(pthis));
        }
    }
    else
    {
        //
        // grab the saved instance pointer and foward the call
        //
        AbstractGattService * pthis = (AbstractGattService *)GetWindowLongPtr(Hwnd, GWLP_USERDATA);
        if(NULL != pthis)
        {
            pthis->WndProc(Hwnd, Msg, Wparam, Lparam);
        }
    }
    
    return DefWindowProc(Hwnd, Msg, Wparam, Lparam);
}

VOID 
AbstractGattService::WndProc(
        HWND Hwnd, 
        UINT Msg, 
        WPARAM Wparam, 
        LPARAM Lparam
        )
{

    UNREFERENCED_PARAMETER(Hwnd);

    switch (Msg)
    {

    case WM_DEVICECHANGE:

        ULONG eventCode = (ULONG)Wparam;
        PDEV_BROADCAST_HANDLE pHdrHandle = (PDEV_BROADCAST_HANDLE)Lparam;

        switch (eventCode)
        {
        case DBT_CUSTOMEVENT:
            if (GUID_BLUETOOTH_HCI_EVENT == pHdrHandle->dbch_eventguid) 
            {

                PBTH_HCI_EVENT_INFO pCxnEvent = (PBTH_HCI_EVENT_INFO)pHdrHandle->dbch_data;

                if (HCI_CONNECTION_TYPE_LE == pCxnEvent->connectionType &&
                    m_BthDeviceAddress == pCxnEvent->bthAddress) 
                {
                    if (pCxnEvent->connected) 
                    {
                        m_pBthLEDevice->DispatchDeviceConnected();
                    } 
                    else 
                    {
                        m_pBthLEDevice->DispatchDeviceDisconnected();                  
                    }
                }

            }
            break;
        }

        break;
    }        
}





