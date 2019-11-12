#include "precomp.h"
#pragma hdrstop

CSensorManagerEvents::CSensorManagerEvents(CSampleRadioManager* parent, ISensorManager* pSensorManager)
{
    m_lRefCount = 1; //ref count initialized to 1

    m_pParent = parent;

    pSensorManager->AddRef();
    m_spISensorManager.Attach(pSensorManager);
}

CSensorManagerEvents::~CSensorManagerEvents()
{
    if (nullptr != m_spISensorManager)
    {
        m_spISensorManager->SetEventSink(nullptr);
    }
}

IFACEMETHODIMP CSensorManagerEvents::QueryInterface(REFIID riid, void** ppObject)
{
    HRESULT hr = S_OK;

    *ppObject = nullptr;
    if (riid == __uuidof(ISensorManagerEvents))
    {
        *ppObject = reinterpret_cast<ISensorManagerEvents*>(this);
    }
    else if (riid == IID_IUnknown)
    {
        *ppObject = reinterpret_cast<IUnknown*>(this);
    }
    else
    {
        hr = E_NOINTERFACE;
    }

    if (SUCCEEDED(hr))
    {
        (reinterpret_cast<IUnknown*>(*ppObject))->AddRef();
    }

    return hr;
}

ULONG _stdcall CSensorManagerEvents::AddRef()
{
    m_lRefCount++;
    return m_lRefCount;
}

ULONG _stdcall CSensorManagerEvents::Release()
{
    ULONG lRet = --m_lRefCount;

    if (m_lRefCount == 0)
    {
        delete this;
    }

    return lRet;
}

HRESULT CSensorManagerEvents::Initialize()
{
    HRESULT hr = m_spISensorManager->SetEventSink(this);

    return hr;
}

HRESULT CSensorManagerEvents::OnSensorEnter(__RPC__in_opt ISensor* pSensor, SensorState state)
{
    UNREFERENCED_PARAMETER(state);
    HRESULT hr = S_OK;

    if (nullptr != pSensor)
    {
        SENSOR_ID sensorID;
        hr = pSensor->GetID(&sensorID);
        if (SUCCEEDED(hr))
        {
            if (IsEqualIID(sensorID, SENSOR_GUID))
            {
                m_pParent->SensorAdded();
            }
        }
    }
    else
    {
        hr = E_POINTER;
    }

    return hr;
}
