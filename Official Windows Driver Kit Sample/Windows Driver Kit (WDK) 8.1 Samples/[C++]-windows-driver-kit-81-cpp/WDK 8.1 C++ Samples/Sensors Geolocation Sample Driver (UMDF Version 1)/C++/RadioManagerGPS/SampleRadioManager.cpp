#include "precomp.h"
#pragma hdrstop

CSampleRadioManager::CSampleRadioManager() :
    _pSensorManagerEvents(nullptr),
    _hPnPEventThread(nullptr),
    _hPnPEventThreadEvent(nullptr),
    _hPnPEventWindow(nullptr)
{
}

HRESULT CSampleRadioManager::FinalConstruct()
{
    HRESULT  hr = S_OK;

    // Create Sensor Manager to use to connect to sensor
    hr = _spSensorManager.CoCreateInstance(CLSID_SensorManager, nullptr, CLSCTX_INPROC_SERVER);
    if (SUCCEEDED(hr))
    {
        // Add the one radio that is managed if it is present
        HRESULT hrTemp = S_OK;

        {
            // Brackets for sensorComm scope
            CSensorCommunication sensorComm = CSensorCommunication();
            hrTemp = sensorComm.Initialize();
        }

        if (SUCCEEDED(hrTemp))
        {
            hr = _AddInstance(SENSOR_GUID_STRING, nullptr);
        }

        // Listen for sensor add events
        if (SUCCEEDED(hr))
        {
            // Will be deleted in _Cleanup
            _pSensorManagerEvents = new CSensorManagerEvents(this, _spSensorManager);
            if (nullptr != _pSensorManagerEvents)
            {
                hr = _pSensorManagerEvents->Initialize();
            }
            else
            {
                hr = E_OUTOFMEMORY;
            }
        }

        // Listen for sensor leave events.
        // Listen for PnP events instead of subscribing to sensor events as subscribing
        // to sensor events will create a persistent connection to the driver.
        _InitializeSensorLeaveEvents();
    }

    if (FAILED(hr))
    {
        _Cleanup();
    }

    return hr;
}

HRESULT CSampleRadioManager::_InitializeSensorLeaveEvents()
{
    HRESULT hr = S_OK;

    // Create an event so we can be notified when the thread is initialized.
    _hPnPEventThreadEvent = ::CreateEvent(nullptr, TRUE, FALSE, nullptr);
    if (nullptr != _hPnPEventThreadEvent)
    {
        _hPnPEventThread = ::CreateThread(
            nullptr,
            0,
            s_ThreadPnPEvent,
            this,
            0,
            nullptr);

        if (nullptr == _hPnPEventThread)
        {
            hr = GetLastError();
        }
        else
        {
            (void)::WaitForSingleObject(_hPnPEventThreadEvent, INFINITE);
        }

        ::CloseHandle(_hPnPEventThreadEvent);
        _hPnPEventThreadEvent = nullptr;
    }
    else
    {
        hr = GetLastError();
    }

    return hr;
}

void CSampleRadioManager::FinalRelease()
{
    _Cleanup();
}

void CSampleRadioManager::_Cleanup()
{
    POSITION p;
    while (nullptr != (p = _listRadioInstances.GetHeadPosition()))
    {
        INSTANCE_LIST_OBJ *pListObj = _listRadioInstances.GetAt(p);
        if (nullptr != pListObj)
        {
            _listRadioInstances.SetAt(p, nullptr);
            _listRadioInstances.RemoveHeadNoReturn();
            delete pListObj;
        }
    }

    if (nullptr != _pSensorManagerEvents)
    {
        delete _pSensorManagerEvents;
    }

    // Destory the window
    if (nullptr != _hPnPEventWindow)
    {
        if (0 != PostMessage(_hPnPEventWindow, WM_DESTROY, 0, 0))
        {
            _hPnPEventWindow = nullptr;

            // Wait for the destroy window to be processed
            if (nullptr != _hPnPEventThread)
            {
                (void)::WaitForSingleObject(_hPnPEventThread, INFINITE);
            }
        }
    }

    if (nullptr != _hPnPEventThread)
    {
        CloseHandle(_hPnPEventThread);
        _hPnPEventThread = nullptr;
    }
}

IFACEMETHODIMP CSampleRadioManager::GetRadioInstances(_Out_ IRadioInstanceCollection **ppCollection)
{
    CAutoVectorPtr<IRadioInstance *> rgpIRadioInstance;
    HRESULT  hr = S_OK;
    DWORD    cInstance;

    if (nullptr == ppCollection)
    {
        return E_INVALIDARG;
    }

    *ppCollection = nullptr;

    CComCritSecLock<CComAutoCriticalSection> lock(_criticalSection);
    cInstance = static_cast<DWORD>(_listRadioInstances.GetCount());
    if (cInstance > 0)
    {
        if (!rgpIRadioInstance.Allocate(cInstance))
        {
            hr = E_OUTOFMEMORY;
        }
        if (SUCCEEDED(hr))
        {
            ZeroMemory(rgpIRadioInstance, sizeof(rgpIRadioInstance[0]) * cInstance);
            DWORD dwIndex = 0;

            for (POSITION p = _listRadioInstances.GetHeadPosition(); nullptr != p; _listRadioInstances.GetNext(p))
            {
                hr = (_listRadioInstances.GetAt(p))->spRadioInstance.QueryInterface(&(rgpIRadioInstance[dwIndex]));
                if (FAILED(hr))
                {
                     break;
                }
                else
                {
                    dwIndex++;
                }
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = CRadioInstanceCollection_CreateInstance(cInstance, rgpIRadioInstance, ppCollection);
    }

    for (DWORD dwIndex = 0; dwIndex < cInstance; dwIndex++)
    {
        if (nullptr != rgpIRadioInstance[dwIndex])
        {
            rgpIRadioInstance[dwIndex]->Release();
        }
    }

    return hr;
}

IFACEMETHODIMP CSampleRadioManager::OnSystemRadioStateChange(
    _In_ SYSTEM_RADIO_STATE sysRadioState,
    _In_ UINT32 uTimeoutSec)
{
    HRESULT hr = S_OK;
    CAutoPtr<SET_SYS_RADIO_JOB> spSetSysRadioJob;
    bool fRefAdded = false;

    spSetSysRadioJob.Attach(new SET_SYS_RADIO_JOB);
    if (nullptr == spSetSysRadioJob)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        spSetSysRadioJob->hr = E_FAIL;
        spSetSysRadioJob->srsTarget = sysRadioState;
        spSetSysRadioJob->pSampleRM = this;

        // Add ref to object to avoid object release before working thread return
        this->AddRef();
        fRefAdded = true;

        HANDLE hEvent = ::CreateEvent(nullptr, TRUE, FALSE, nullptr);
        if (nullptr == hEvent)
        {
            hr = HRESULT_FROM_WIN32(GetLastError());
        }
        else
        {
            spSetSysRadioJob->hEvent.Attach(hEvent);
        }
    }

    if (SUCCEEDED(hr))
    {
        if (!QueueUserWorkItem(CSampleRadioManager::s_ThreadSetSysRadio, spSetSysRadioJob, WT_EXECUTEDEFAULT))
        {
            hr = HRESULT_FROM_WIN32(GetLastError());
        }
    }

    if (SUCCEEDED(hr))
    {
        DWORD dwIgnore;
        hr = CoWaitForMultipleHandles(0,
                                      uTimeoutSec * 1000,
                                      1,
                                      reinterpret_cast<LPHANDLE>(&(spSetSysRadioJob->hEvent)),
                                      &dwIgnore);
        if (RPC_S_CALLPENDING == hr)
        {
             spSetSysRadioJob.Detach();
        }
        else
        {
            hr = spSetSysRadioJob->hr;
        }
    }

    if (fRefAdded)
    {
        this->Release();
    }

    return hr;
}

IFACEMETHODIMP CSampleRadioManager::OnInstanceRadioChange(
    _In_ BSTR bstrRadioInstanceID,
    _In_ DEVICE_RADIO_STATE radioState)
{
    return _FireEventOnInstanceRadioChange(bstrRadioInstanceID, radioState);
}

HRESULT CSampleRadioManager::_FireEventOnInstanceAdd(_In_ IRadioInstance *pRadioInstance)
{
    HRESULT hr;

    Lock();
    for (IUnknown** ppUnkSrc = m_vec.begin(); ppUnkSrc < m_vec.end(); ppUnkSrc++)
    {
        if ((nullptr != ppUnkSrc) && (nullptr != *ppUnkSrc))
        {
            CComPtr<IMediaRadioManagerNotifySink> spSink;

            hr = (*ppUnkSrc)->QueryInterface(IID_PPV_ARGS(&spSink));
            if (SUCCEEDED(hr))
            {
                spSink->OnInstanceAdd(pRadioInstance);
            }
        }
    }
    Unlock();

    return S_OK;
}

HRESULT CSampleRadioManager::_FireEventOnInstanceRemove(_In_ BSTR bstrRadioInstanceID)
{
    HRESULT hr;

    Lock();
    for (IUnknown** ppUnkSrc = m_vec.begin(); ppUnkSrc < m_vec.end(); ppUnkSrc++)
    {
        if ((nullptr != ppUnkSrc) && (nullptr != *ppUnkSrc))
        {
            CComPtr<IMediaRadioManagerNotifySink> spSink;

            hr = (*ppUnkSrc)->QueryInterface(IID_PPV_ARGS(&spSink));
            if (SUCCEEDED(hr))
            {
                spSink->OnInstanceRemove(bstrRadioInstanceID);
            }
        }
    }
    Unlock();

    return S_OK;
}

HRESULT CSampleRadioManager::_FireEventOnInstanceRadioChange(
    _In_ BSTR bstrRadioInstanceID,
    _In_ DEVICE_RADIO_STATE radioState
    )
{
    HRESULT hr;

    Lock();
    for (IUnknown** ppUnkSrc = m_vec.begin(); ppUnkSrc < m_vec.end(); ppUnkSrc++)
    {
        if ((nullptr != ppUnkSrc) && (nullptr != *ppUnkSrc))
        {
            CComPtr<IMediaRadioManagerNotifySink> spSink;

            hr = (*ppUnkSrc)->QueryInterface(IID_PPV_ARGS(&spSink));
            if (SUCCEEDED(hr))
            {
                spSink->OnInstanceRadioChange(bstrRadioInstanceID, radioState);
            }
        }
    }
    Unlock();

    return S_OK;
}


HRESULT CSampleRadioManager::_AddInstance(_In_ PCWSTR pszKeyName, _Out_opt_ IRadioInstance **ppRadioInstance)
{
    HRESULT hr = S_OK;
    CComPtr<ISampleRadioInstanceInternal> spRadioInstance;
    CAutoPtr<INSTANCE_LIST_OBJ> spInstanceObj;

    CComCritSecLock<CComAutoCriticalSection> lock(_criticalSection);

    spInstanceObj.Attach(new INSTANCE_LIST_OBJ);
    if (nullptr == spInstanceObj)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        CComPtr<ISampleRadioManagerInternal> spRMInternal = this;
        hr = CSampleRadioInstance_CreateInstance(pszKeyName, spRMInternal, &spRadioInstance);
    }

    if (SUCCEEDED(hr))
    {
        spInstanceObj->fExisting = true;
        spInstanceObj->spRadioInstance = spRadioInstance;

        _ATLTRY
        {
            spInstanceObj->strRadioInstanceID = pszKeyName;
            _listRadioInstances.AddTail(spInstanceObj);
            spInstanceObj.Detach();
        }
        _ATLCATCH(e)
        {
            hr = e;
        }

        if (SUCCEEDED(hr))
        {
            if (ppRadioInstance != nullptr)
            {
                hr = spRadioInstance->QueryInterface(IID_PPV_ARGS(ppRadioInstance));
            }
        }
    }

    return hr;
}

HRESULT CSampleRadioManager::_SetSysRadioState(_In_ SYSTEM_RADIO_STATE sysRadioState)
{
    HRESULT hr = S_OK;
    CComCritSecLock<CComAutoCriticalSection> lock(_criticalSection);
    for (POSITION p = _listRadioInstances.GetHeadPosition(); nullptr != p; _listRadioInstances.GetNext(p))
    {
        INSTANCE_LIST_OBJ *pInstanceObj = _listRadioInstances.GetAt(p);
        hr = pInstanceObj->spRadioInstance->OnSysRadioChange(sysRadioState);
        if (FAILED(hr))
        {
            break;
        }
    }
    return hr;
}

DWORD WINAPI CSampleRadioManager::s_ThreadSetSysRadio(LPVOID pThis)
{
    SET_SYS_RADIO_JOB *pJob = reinterpret_cast<SET_SYS_RADIO_JOB *>(pThis);

    pJob->hr = pJob->pSampleRM->_SetSysRadioState(pJob->srsTarget);

    SetEvent(pJob->hEvent);
    return ERROR_SUCCESS;
}

void CSampleRadioManager::SensorAdded()
{
    CComPtr<IRadioInstance> spRadioInstance;
    HRESULT hr = _AddInstance(SENSOR_GUID_STRING, &spRadioInstance);
    if (SUCCEEDED(hr))
    {
        _FireEventOnInstanceAdd(spRadioInstance);
    }
}

void CSampleRadioManager::SensorRemoved()
{
    // Remove deleted instance from list
    POSITION p = _listRadioInstances.GetHeadPosition();
    while (nullptr != p)
    {
        INSTANCE_LIST_OBJ *pRadioInstanceObj = _listRadioInstances.GetAt(p);
        if (pRadioInstanceObj != nullptr && 0 == pRadioInstanceObj->strRadioInstanceID.Compare(SENSOR_GUID_STRING))
        {
            POSITION pTmp = p;
            _listRadioInstances.GetPrev(pTmp);

            _listRadioInstances.RemoveAt(p);
            CComBSTR bstrTmp = pRadioInstanceObj->strRadioInstanceID.AllocSysString();
            if (nullptr != bstrTmp)
            {
               _FireEventOnInstanceRemove(bstrTmp);
            }
            delete pRadioInstanceObj;
            p = pTmp;
        }

        if (nullptr != p)
        {
            _listRadioInstances.GetNext(p);
        }
        else
        {
            p = _listRadioInstances.GetHeadPosition();
        }
    }
}

DWORD WINAPI CSampleRadioManager::s_ThreadPnPEvent(LPVOID pThat)
{
    // This thread creates a window that will listen for PnP Events
    CSampleRadioManager* pThis = reinterpret_cast<CSampleRadioManager *>(pThat);

    CComBSTR bstrClassName = CComBSTR("RadioManger_");
    (void)bstrClassName.Append(SENSOR_GUID_STRING);

    // Register and create the window.
    WNDCLASSEXW wndclass    = {0};
    wndclass.cbSize         = sizeof(wndclass);
    wndclass.lpfnWndProc    = _PnPEventWndProc;
    wndclass.lpszClassName  = bstrClassName;
    wndclass.hInstance      = g_hInstance;

    (void)::RegisterClassEx(&wndclass);

    pThis->_hPnPEventWindow = ::CreateWindowEx(
        0,
        bstrClassName,
        L"",
        0, 0, 0, 0, 0,
        nullptr,
        nullptr,
        g_hInstance,
        (LPVOID)(pThis)
        );

    if (nullptr != pThis->_hPnPEventWindow)
    {
        // Register for PnP events
        DEV_BROADCAST_DEVICEINTERFACE devBroadcastInterface = {0};

        devBroadcastInterface.dbcc_size       = sizeof(DEV_BROADCAST_DEVICEINTERFACE);
        devBroadcastInterface.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE;
        devBroadcastInterface.dbcc_name[0]    = 0;
        devBroadcastInterface.dbcc_classguid  = SENSOR_TYPE_LOCATION_GPS;

        HDEVNOTIFY hdevNotify = ::RegisterDeviceNotification(
            pThis->_hPnPEventWindow,
            &devBroadcastInterface,
            DEVICE_NOTIFY_WINDOW_HANDLE);

        if (nullptr != hdevNotify)
        {
            // Signal the event so that the main thread knows the HWND is set.
            (void)::SetEvent(pThis->_hPnPEventThreadEvent);

            MSG msg;
            while (::GetMessage(&msg, nullptr, 0, 0))
            {
                ::TranslateMessage(&msg);
                ::DispatchMessage(&msg);
            }

            ::UnregisterDeviceNotification(hdevNotify);
        }
    }

    ::UnregisterClass(bstrClassName, g_hInstance);

    return 0;
}

LRESULT WINAPI CSampleRadioManager::_PnPEventWndProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    // Window Proc that receives PnP Events.  Used to detect when a sensor leaves.
    LRESULT lRes = 0;
    CSampleRadioManager* pThis = nullptr;

    if (WM_CREATE == uMsg)
    {
        // On first run, give the window access to the parent this pointer
        SetLastError(ERROR_SUCCESS); // Required to detect if return value of 0 from SetWindowLongPtr is null or error
        pThis = reinterpret_cast<CSampleRadioManager*>(((LPCREATESTRUCT)lParam)->lpCreateParams);
        if (0 == SetWindowLongPtr(hWnd, GWLP_USERDATA, (LONG_PTR)pThis))
        {
            // Return value of 0 could be that pervious pointer was null
            HRESULT hr = HRESULT_FROM_WIN32(GetLastError());
            if (FAILED(hr))
            {
                lRes = -1;
            }
        }
    }
    else if (uMsg == WM_DESTROY)
    {
        ::SetWindowLongPtr(hWnd, GWLP_USERDATA, (LONG_PTR)NULL);
        PostQuitMessage(0);
    }
    else if (uMsg == WM_DEVICECHANGE)
    {
        // Determine if this device event is the sensor leaving the system
        pThis = reinterpret_cast<CSampleRadioManager*>(GetWindowLongPtr(hWnd, GWLP_USERDATA));
        if (nullptr != pThis && NULL != lParam && DBT_DEVICEREMOVECOMPLETE == wParam)
        {
            PDEV_BROADCAST_DEVICEINTERFACE pDevIF = reinterpret_cast<PDEV_BROADCAST_DEVICEINTERFACE>(lParam);

            if ((DBT_DEVTYP_DEVICEINTERFACE == pDevIF->dbcc_devicetype) &&
                (IsEqualGUID(SENSOR_TYPE_LOCATION_GPS, pDevIF->dbcc_classguid)))
            {
                // Make sure this is the correct sensor by comparing Sensor ID in device path
                // Device path is contained in pDevIF->dbcc_name
                // Check to see if sensor id matches
                WCHAR* pRefString = wcsrchr(pDevIF->dbcc_name, L'\\');
                if (nullptr != pRefString)
                {
                    pRefString++; // skip past backslash
                    if (0 == _wcsicmp(SENSOR_GUID_STRING, pRefString))
                    {
                        pThis->SensorRemoved();
                    }
                }
            }
        }
    }

    if (0 == lRes)
    {
        lRes = DefWindowProc(hWnd, uMsg, wParam, lParam);
    }

    return lRes;
}