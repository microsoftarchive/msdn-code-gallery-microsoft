#include "precomp.h"
#pragma hdrstop

HRESULT CSampleRadioInstance_CreateInstance(
    _In_                            PCWSTR pszKeyName,
    _In_                            ISampleRadioManagerInternal *pParentManager,
    _COM_Outptr_                    ISampleRadioInstanceInternal **ppRadioInstance
    )
{
    return CSampleRadioInstance::CreateInstance(pszKeyName, pParentManager, ppRadioInstance);
}

CSampleRadioInstance::CSampleRadioInstance()
    : _pParentManager(nullptr)
{
    ZeroMemory(&_guidInstanceId, sizeof(GUID));
}

void CSampleRadioInstance::FinalRelease()
{
    _Cleanup();
}

// static
HRESULT CSampleRadioInstance::CreateInstance(
    _In_  PCWSTR pszKeyName,
    _In_  ISampleRadioManagerInternal *pParentManager,
    _COM_Outptr_ ISampleRadioInstanceInternal **ppRadioInstance
    )
{
    HRESULT hr;
    CComObject<CSampleRadioInstance> *pRadioInstance;
    CComPtr<ISampleRadioInstanceInternal> spRadioInstanceInternal;

    *ppRadioInstance = nullptr;

     hr = CComObject<CSampleRadioInstance>::CreateInstance(&pRadioInstance);
     if (SUCCEEDED(hr) && (pRadioInstance != nullptr))
     {
         hr = pRadioInstance->QueryInterface(IID_PPV_ARGS(&spRadioInstanceInternal));
     }
     if (SUCCEEDED(hr)  && (pRadioInstance != nullptr))
     {
         hr = pRadioInstance->_Init(pszKeyName, pParentManager);
     }
     if (SUCCEEDED(hr))
     {
         *ppRadioInstance = spRadioInstanceInternal.Detach();
     }

     return hr;
}

HRESULT CSampleRadioInstance::_Init(
    _In_ PCWSTR pszKeyName,
    _In_ ISampleRadioManagerInternal *pParentManager
    )
{
    HRESULT hr;

    _ATLTRY
    {
        _strInstanceId = pszKeyName;
    }
    _ATLCATCH(e)
    {
        return e;
    }

    _pParentManager = pParentManager;

    hr = CLSIDFromString(pszKeyName, &_guidInstanceId);
    if (SUCCEEDED(hr))
    {
        _ATLTRY
        {
            // TODO: Set the friendly name of the GPS
            _strInstanceName = L"GPS";
        }
        _ATLCATCH(e)
        {
            UNREFERENCED_PARAMETER(e);
            hr = E_OUTOFMEMORY;
        }
    }

    if (FAILED(hr))
    {
        _Cleanup();
    }

    return hr;
}

void CSampleRadioInstance::_Cleanup()
{
    if (nullptr != _pParentManager)
    {
        _pParentManager = nullptr;
    }
}

IFACEMETHODIMP CSampleRadioInstance::GetRadioManagerSignature(_Out_ GUID *pguidSignature)
{
    if (nullptr == pguidSignature)
    {
        return E_INVALIDARG;
    }

    *pguidSignature = __uuidof(SampleRadioManager);

    return S_OK;
}

IFACEMETHODIMP CSampleRadioInstance::GetInstanceSignature(_Out_ BSTR *pbstrID)
{
    if (nullptr == pbstrID)
    {
        return E_INVALIDARG;
    }

    HRESULT hr = S_OK;

    *pbstrID = _strInstanceId.AllocSysString();
    if (nullptr == *pbstrID)
    {
        hr = E_OUTOFMEMORY;
    }

    return hr;
}

IFACEMETHODIMP CSampleRadioInstance::GetFriendlyName(_In_ LCID /* lcid*/, _Out_ BSTR *pbstrName)
{
    if (nullptr == pbstrName)
    {
        return E_INVALIDARG;
    }

    HRESULT hr = S_OK;

    *pbstrName = _strInstanceName.AllocSysString();
    if (nullptr == *pbstrName)
    {
        hr = E_OUTOFMEMORY;
    }

    return hr;
}

IFACEMETHODIMP CSampleRadioInstance::GetRadioState(_Out_ DEVICE_RADIO_STATE *pRadioState)
{
    if (nullptr == pRadioState)
    {
        return E_INVALIDARG;
    }

    DEVICE_RADIO_STATE radioState = DRS_RADIO_ON;

    *pRadioState = DRS_RADIO_ON;

    CSensorCommunication sensorComm = CSensorCommunication();
    HRESULT hr = sensorComm.Initialize();
    if (SUCCEEDED(hr))
    {
        sensorComm.GetRadioState(&radioState);
    }

    if (SUCCEEDED(hr))
    {
        if ((radioState > DRS_RADIO_MAX) || (radioState == DRS_RADIO_INVALID))
        {
            radioState = DRS_HW_RADIO_ON_UNCONTROLLABLE;
        }

        *pRadioState = radioState;
    }

    return hr;
}


IFACEMETHODIMP CSampleRadioInstance::SetRadioState(_In_ DEVICE_RADIO_STATE radioState, _In_ UINT32 uTimeoutSec)
{
    if ((radioState != DRS_RADIO_ON) && (radioState != DRS_SW_RADIO_OFF))
    {
        return E_INVALIDARG; // invalid input
    }

    DEVICE_RADIO_STATE drsCurrent;
    bool fRefAdded = false;
    HRESULT hr = GetRadioState(&drsCurrent);

    // fail to get current radio state or current radio state is uncontrollable
    if (FAILED(hr) || (drsCurrent & DRS_HW_RADIO_ON_UNCONTROLLABLE))
    {
        return E_FAIL;
    }

    if ((drsCurrent & DRS_SW_RADIO_OFF) == radioState)
    {
        return S_OK;    // current software radio is same as target software radio
    }

    // Keep all other bits same, only change the last bit of radio state to represent software radio change
    DEVICE_RADIO_STATE drsState = static_cast<DEVICE_RADIO_STATE>((drsCurrent & DRS_HW_RADIO_OFF) | radioState);

    CAutoPtr<SET_DEVICE_RADIO_JOB> spSetDeviceRadioJob;

    spSetDeviceRadioJob.Attach(new SET_DEVICE_RADIO_JOB);
    if (nullptr == spSetDeviceRadioJob)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        spSetDeviceRadioJob->hr = E_FAIL;
        spSetDeviceRadioJob->drsTarget = drsState;
        spSetDeviceRadioJob->pInstance = this;

        // Let working thread hold ref on object such that object won't be released before working thread return.
        this->AddRef();
        fRefAdded = true;

        HANDLE hEvent = ::CreateEvent(nullptr, TRUE, FALSE, nullptr);
        if (nullptr == hEvent)
        {
            hr = HRESULT_FROM_WIN32(GetLastError());
        }
        else
        {
            spSetDeviceRadioJob->hEvent.Attach(hEvent);
        }
    }

    if (SUCCEEDED(hr))
    {
        if (!QueueUserWorkItem(CSampleRadioInstance::s_ThreadSetRadio, spSetDeviceRadioJob, WT_EXECUTEDEFAULT))
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
                                      reinterpret_cast<LPHANDLE>(&(spSetDeviceRadioJob->hEvent)),
                                      &dwIgnore);
        if (RPC_S_CALLPENDING == hr)
        {
            spSetDeviceRadioJob.Detach();
        }
        else
        {
            hr = spSetDeviceRadioJob->hr;
        }
    }

    if (fRefAdded)
    {
        this->Release();
    }
    return hr;
}

IFACEMETHODIMP_(BOOL) CSampleRadioInstance::IsMultiComm()
{
    // Is not MultiComm
    return FALSE;
}

IFACEMETHODIMP_(BOOL) CSampleRadioInstance::IsAssociatingDevice()
{
    // Always true
    return TRUE;
}

IFACEMETHODIMP CSampleRadioInstance::OnSysRadioChange(_In_ SYSTEM_RADIO_STATE sysRadioState)
{
    if ((sysRadioState != SRS_RADIO_ENABLED) && (sysRadioState != SRS_RADIO_DISABLED))
    {
        return E_INVALIDARG;
    }

    DEVICE_RADIO_STATE drsCurrent;
    DEVICE_RADIO_STATE drsTarget = DRS_RADIO_ON;
    DEVICE_RADIO_STATE drsPrevious = DRS_RADIO_ON;
    bool               fSetRadioState = false;

    HRESULT hr = GetRadioState(&drsCurrent);

    // fail to get current radio state or current radio state is uncontrollable, ignore this call
    if (FAILED(hr) || (drsCurrent & DRS_HW_RADIO_ON_UNCONTROLLABLE))
    {
        return S_OK;
    }

    if (SRS_RADIO_ENABLED == sysRadioState)
    {
        // If device current software radio is already on during system radio enabled,
        // we ignore the saved previous radio state and do nothing.
        // radio state will be updated only when current state is off during system radio enabling.
        if (DRS_RADIO_ON != (drsCurrent & DRS_SW_RADIO_OFF))
        {
            // System radio enable, need get previous radio state and set to previous radio state.
            // If fail to get previous radio state, assume previous radio state is software radio on.
            DEVICE_RADIO_STATE drsPreviousTemp = DRS_RADIO_ON;
            CSensorCommunication sensorComm = CSensorCommunication();
            hr = sensorComm.Initialize();
            if (SUCCEEDED(hr))
            {
                sensorComm.GetPreviousRadioState(&drsPreviousTemp);
            }

            if (SUCCEEDED(hr))
            {
                if ((DRS_RADIO_ON == drsPreviousTemp) || (DRS_SW_RADIO_OFF == drsPreviousTemp))
                {
                    drsPrevious = static_cast<DEVICE_RADIO_STATE>(drsPreviousTemp);
                }
            }

            if ((drsCurrent & DRS_SW_RADIO_OFF) != drsPrevious)
            {
                drsTarget = static_cast<DEVICE_RADIO_STATE>((drsCurrent & DRS_HW_RADIO_OFF) | drsPrevious);
                fSetRadioState = true;
            }
        }
    }
    else
    {
        CSensorCommunication sensorComm = CSensorCommunication();
        hr = sensorComm.Initialize();
        if (SUCCEEDED(hr))
        {
            sensorComm.SetPreviousRadioState(drsCurrent);
        }

        if ((drsCurrent & DRS_SW_RADIO_OFF) != DRS_SW_RADIO_OFF)
        {
            drsTarget = static_cast<DEVICE_RADIO_STATE>((drsCurrent & DRS_HW_RADIO_OFF) | DRS_SW_RADIO_OFF);
            fSetRadioState = true;
        }
    }

    if (fSetRadioState)
    {
        hr = _SetRadioState(drsTarget);
    }

    return hr;
}

HRESULT CSampleRadioInstance::_SetRadioState(_In_ DEVICE_RADIO_STATE radioState)
{
    CSensorCommunication sensorComm = CSensorCommunication();
    HRESULT hr = sensorComm.Initialize();
    if (SUCCEEDED(hr))
    {
        hr = sensorComm.SetRadioState(radioState);

        if (SUCCEEDED(hr))
        {
            CComBSTR sbstrID = _strInstanceId.AllocSysString();
            if (nullptr != sbstrID)
            {
                _pParentManager->OnInstanceRadioChange(sbstrID, radioState);
            }
        }
    }

    return hr;
}

DWORD WINAPI CSampleRadioInstance::s_ThreadSetRadio(LPVOID pThis)
{
    SET_DEVICE_RADIO_JOB *pJob = reinterpret_cast<SET_DEVICE_RADIO_JOB *>(pThis);

    pJob->hr = pJob->pInstance->_SetRadioState(pJob->drsTarget);

    SetEvent(pJob->hEvent);
    return ERROR_SUCCESS;
}
