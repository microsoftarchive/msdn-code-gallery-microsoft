#pragma once

class CSampleRadioManager;

class CSensorManagerEvents :
    public ISensorManagerEvents
{
public:
    // These three methods are for IUnknown
    STDMETHOD(QueryInterface)(REFIID riid, void** ppObject );
    ULONG _stdcall AddRef();
    ULONG _stdcall Release();

    // Constructor and destructor
    CSensorManagerEvents(CSampleRadioManager* parent, ISensorManager* pSensorManager);
    virtual ~CSensorManagerEvents();

    // Called by parent
    HRESULT Initialize();

    // ISensorManagerEvents interface
    IFACEMETHOD(OnSensorEnter)(__RPC__in_opt ISensor* pSensor, SensorState state);

private:
    // Member variable to implement IUnknown reference count
    LONG m_lRefCount;

    CSampleRadioManager*    m_pParent;
    CComPtr<ISensorManager> m_spISensorManager;      // Global to keep reference for life of class
};
