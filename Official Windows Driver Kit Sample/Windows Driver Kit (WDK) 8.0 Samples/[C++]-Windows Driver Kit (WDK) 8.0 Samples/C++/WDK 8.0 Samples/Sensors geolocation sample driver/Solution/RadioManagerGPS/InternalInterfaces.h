#pragma once

// TODO: generate a new GUID for this interface
interface __declspec(uuid("{8D23CC9E-D8B0-42F0-A3E1-64DB6C29DC72}")) ISampleRadioInstanceInternal : public IUnknown
{
    IFACEMETHOD(OnSysRadioChange)(_In_ SYSTEM_RADIO_STATE sysRadioState) = 0;
};

// TODO: generate a new GUID for this interface
interface __declspec(uuid("{4AF85E00-0374-4051-804B-CAAEA9A0B3AF}")) ISampleRadioManagerInternal : public IUnknown
{
    IFACEMETHOD(OnInstanceRadioChange)(_In_ BSTR bstrRadioInstanceID, _In_ DEVICE_RADIO_STATE radioState) = 0;
};


HRESULT CSampleRadioInstance_CreateInstance(
    _In_                            PCWSTR pszKeyName,
    _In_                            ISampleRadioManagerInternal *pParentManager,
    _COM_Outptr_                    ISampleRadioInstanceInternal **ppRadioInstance
    );

HRESULT CRadioInstanceCollection_CreateInstance(
    _In_                            DWORD cInstances,
    _In_reads_(cInstances)          IRadioInstance **rgpIRadioInstance,
    _COM_Outptr_                    IRadioInstanceCollection **ppInstanceCollection);

