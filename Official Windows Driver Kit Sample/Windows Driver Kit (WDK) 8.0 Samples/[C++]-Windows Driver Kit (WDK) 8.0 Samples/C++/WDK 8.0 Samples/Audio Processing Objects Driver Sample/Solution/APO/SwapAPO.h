//
// SwapAPO.h -- Copyright (c) Microsoft Corporation. All rights reserved.
//
// Description:
//
//   Declaration of the CSwapAPO class.
//

#pragma once

#include <audioenginebaseapo.h>
#include <BaseAudioProcessingObject.h>
#include <SwapAPOInterface.h>
#include <SwapAPODll.h>

#include <commonmacros.h>

_Analysis_mode_(_Analysis_code_type_user_driver_)

#define PK_EQUAL(x, y)  ((x.fmtid == y.fmtid) && (x.pid == y.pid))


#pragma AVRT_VTABLES_BEGIN
// Swap APO class - GFX
class CSwapAPOGFX :
    public CComObjectRootEx<CComMultiThreadModel>,
    public CComCoClass<CSwapAPOGFX, &CLSID_SwapAPOGFX>,
    public CBaseAudioProcessingObject,
    public IMMNotificationClient,
    public IAudioSystemEffects,
    public IAudioSystemEffectsCustomFormats,
    public ISwapAPOGFX
{
public:
    // constructor
    CSwapAPOGFX() : CBaseAudioProcessingObject(sm_RegProperties)
    {
        m_pf32Coefficients = NULL;
    }

    virtual ~CSwapAPOGFX();    // destructor

DECLARE_REGISTRY_RESOURCEID(IDR_SWAPAPOGFX)

BEGIN_COM_MAP(CSwapAPOGFX)
    COM_INTERFACE_ENTRY(ISwapAPOGFX)
    COM_INTERFACE_ENTRY(IAudioSystemEffects)
    COM_INTERFACE_ENTRY(IAudioSystemEffectsCustomFormats)
    COM_INTERFACE_ENTRY(IMMNotificationClient)
    COM_INTERFACE_ENTRY(IAudioProcessingObjectRT)
    COM_INTERFACE_ENTRY(IAudioProcessingObject)
    COM_INTERFACE_ENTRY(IAudioProcessingObjectConfiguration)
END_COM_MAP()

DECLARE_PROTECT_FINAL_CONSTRUCT()

public:
    STDMETHOD_(void, APOProcess)(UINT32 u32NumInputConnections,
        APO_CONNECTION_PROPERTY** ppInputConnections, UINT32 u32NumOutputConnections,
        APO_CONNECTION_PROPERTY** ppOutputConnections);

    STDMETHOD(Initialize)(UINT32 cbDataSize, BYTE* pbyData);

    virtual HRESULT ValidateAndCacheConnectionInfo(
                                    UINT32 u32NumInputConnections, 
                                    APO_CONNECTION_DESCRIPTOR** ppInputConnections, 
                                    UINT32 u32NumOutputConnections, 
                                    APO_CONNECTION_DESCRIPTOR** ppOutputConnections);

    // IMMNotificationClient
    STDMETHODIMP OnDeviceStateChanged(LPCWSTR pwstrDeviceId, DWORD dwNewState) 
    { 
        UNREFERENCED_PARAMETER(pwstrDeviceId); 
        UNREFERENCED_PARAMETER(dwNewState); 
        return S_OK; 
    }
    STDMETHODIMP OnDeviceAdded(LPCWSTR pwstrDeviceId)
    { 
        UNREFERENCED_PARAMETER(pwstrDeviceId); 
        return S_OK; 
    }
    STDMETHODIMP OnDeviceRemoved(LPCWSTR pwstrDeviceId)
    { 
        UNREFERENCED_PARAMETER(pwstrDeviceId); 
        return S_OK; 
    }
    STDMETHODIMP OnDefaultDeviceChanged(EDataFlow flow, ERole role, LPCWSTR pwstrDefaultDeviceId)
    { 
        UNREFERENCED_PARAMETER(flow); 
        UNREFERENCED_PARAMETER(role); 
        UNREFERENCED_PARAMETER(pwstrDefaultDeviceId); 
        return S_OK; 
    }
    STDMETHODIMP OnPropertyValueChanged(LPCWSTR pwstrDeviceId, const PROPERTYKEY key);

    // IAudioSystemEffectsCustomFormats
    STDMETHODIMP GetFormatCount(UINT* pcFormats);
    STDMETHODIMP GetFormat(UINT nFormat, IAudioMediaType** ppFormat);
    STDMETHODIMP GetFormatRepresentation(UINT nFormat, _Outptr_ LPWSTR* ppwstrFormatRep);

public:
    LONG                                    m_fEnableSwapGFX;
    CComPtr<IPropertyStore>                 m_spAPOSystemEffectsProperties;
    CComPtr<IMMDeviceEnumerator>            m_spEnumerator;
    static const CRegAPOProperties<1>       sm_RegProperties;   // registration properties

    // Locked memory
    FLOAT32                                 *m_pf32Coefficients;

};
#pragma AVRT_VTABLES_END


#pragma AVRT_VTABLES_BEGIN
// Swap APO class - LFX
class CSwapAPOLFX :
    public CComObjectRootEx<CComMultiThreadModel>,
    public CComCoClass<CSwapAPOLFX, &CLSID_SwapAPOLFX>,
    public CBaseAudioProcessingObject,
    public IMMNotificationClient,
    public IAudioSystemEffects,
    public ISwapAPOLFX
{
public:
    // constructor
    CSwapAPOLFX() : CBaseAudioProcessingObject(sm_RegProperties)
    {
    }

    virtual ~CSwapAPOLFX();    // destructor

DECLARE_REGISTRY_RESOURCEID(IDR_SWAPAPOLFX)

BEGIN_COM_MAP(CSwapAPOLFX)
    COM_INTERFACE_ENTRY(ISwapAPOLFX)
    COM_INTERFACE_ENTRY(IAudioSystemEffects)
    COM_INTERFACE_ENTRY(IMMNotificationClient)
    COM_INTERFACE_ENTRY(IAudioProcessingObjectRT)
    COM_INTERFACE_ENTRY(IAudioProcessingObject)
    COM_INTERFACE_ENTRY(IAudioProcessingObjectConfiguration)
END_COM_MAP()

DECLARE_PROTECT_FINAL_CONSTRUCT()

public:
    STDMETHOD_(void, APOProcess)(UINT32 u32NumInputConnections,
        APO_CONNECTION_PROPERTY** ppInputConnections, UINT32 u32NumOutputConnections,
        APO_CONNECTION_PROPERTY** ppOutputConnections);

    STDMETHOD(Initialize)(UINT32 cbDataSize, BYTE* pbyData);

    // IMMNotificationClient
    STDMETHODIMP OnDeviceStateChanged(LPCWSTR pwstrDeviceId, DWORD dwNewState) 
    { 
        UNREFERENCED_PARAMETER(pwstrDeviceId); 
        UNREFERENCED_PARAMETER(dwNewState); 
        return S_OK; 
    }
    STDMETHODIMP OnDeviceAdded(LPCWSTR pwstrDeviceId)
    { 
        UNREFERENCED_PARAMETER(pwstrDeviceId); 
        return S_OK; 
    }
    STDMETHODIMP OnDeviceRemoved(LPCWSTR pwstrDeviceId)
    { 
        UNREFERENCED_PARAMETER(pwstrDeviceId); 
        return S_OK; 
    }
    STDMETHODIMP OnDefaultDeviceChanged(EDataFlow flow, ERole role, LPCWSTR pwstrDefaultDeviceId)
    { 
        UNREFERENCED_PARAMETER(flow); 
        UNREFERENCED_PARAMETER(role); 
        UNREFERENCED_PARAMETER(pwstrDefaultDeviceId); 
        return S_OK; 
    }
    STDMETHODIMP OnPropertyValueChanged(LPCWSTR pwstrDeviceId, const PROPERTYKEY key);

public:
    LONG                                    m_fEnableSwapLFX;
    CComPtr<IPropertyStore>                 m_spAPOSystemEffectsProperties;
    CComPtr<IMMDeviceEnumerator>            m_spEnumerator;
    static const CRegAPOProperties<1>       sm_RegProperties;   // registration properties

};
#pragma AVRT_VTABLES_END

OBJECT_ENTRY_AUTO(__uuidof(SwapAPOGFX), CSwapAPOGFX)
OBJECT_ENTRY_AUTO(__uuidof(SwapAPOLFX), CSwapAPOLFX)

//
//   Declaration of the ProcessSwap routine.
//
void ProcessSwap(
    FLOAT32 *pf32InputFrames,
    FLOAT32 *pf32OutputFrames,
    UINT32   u32ValidFrameCount,
    UINT32   u32SamplesPerFrame);

//
//   Declaration of the ProcessSwapScale routine.
//
void ProcessSwapScale(
    FLOAT32 *pf32InputFrames,
    FLOAT32 *pf32OutputFrames,
    UINT32   u32ValidFrameCount,
    UINT32   u32SamplesPerFrame,
    FLOAT32  *pf32Coefficients );


