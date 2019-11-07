//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved.

#include "pch.h"

#include <windows.system.h>
#include <windows.ui.core.h>

#include <UIAutomationCore.h>
#include <UIAutomationCoreApi.h>

#include "InternalInterfaces_h.h"

using namespace ABI::Windows::Foundation;
using namespace Microsoft::WRL;
using namespace ABI::Windows::UI;
using namespace ABI::Windows::UI::Core;
using namespace ABI::Windows::System;
using namespace ABI::UiaCoreWindowProvider;

// CAppWindowProvider: provider for the main application window

[uuid("1F3AB6DE-000A-4C93-9BA4-A629E399A463")]
class CAppWindowProvider:
    public Microsoft::WRL::RuntimeClass<  
        Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::RuntimeClassType::WinRtClassicComMix>,    
        IAppWindowProvider,
        IRawElementProviderSimple,
        IRawElementProviderFragment,
        IRawElementProviderFragmentRoot,
        IRawElementProviderAdviseEvents,
        IDisconnectableProvider>
{
    InspectableClass(RuntimeClass_UiaCoreWindowProvider_AppWindowProvider, BaseTrust);

public:
    CAppWindowProvider();

    // IAppWindowProvider methods
    IFACEMETHODIMP Initialize(_In_ ITriColorControlInternal* pControl, _In_ IInspectable* pWindow);

    // IRawElementProviderSimple methods
    IFACEMETHODIMP get_ProviderOptions(_Out_ ProviderOptions * retVal);
    IFACEMETHODIMP GetPatternProvider(_In_ PATTERNID iid, _Outptr_result_maybenull_ IUnknown * * retVal );
    IFACEMETHODIMP GetPropertyValue(_In_ PROPERTYID idProp, _Out_ VARIANT * retVal );
    IFACEMETHODIMP get_HostRawElementProvider(_Outptr_result_maybenull_ IRawElementProviderSimple ** retVal );

    // IRawElementProviderFragment methods
    IFACEMETHODIMP Navigate(_In_ NavigateDirection direction, _Outptr_result_maybenull_ IRawElementProviderFragment ** retVal );
    IFACEMETHODIMP GetRuntimeId(_Outptr_result_maybenull_ SAFEARRAY ** retVal );
    IFACEMETHODIMP get_BoundingRectangle(_Out_ UiaRect * retVal );
    IFACEMETHODIMP GetEmbeddedFragmentRoots(_Outptr_result_maybenull_ SAFEARRAY ** retVal );
    IFACEMETHODIMP SetFocus();
    IFACEMETHODIMP get_FragmentRoot(_Outptr_result_maybenull_ IRawElementProviderFragmentRoot * * retVal);

    // IRawElementProviderFragmentRoot methods
    IFACEMETHODIMP ElementProviderFromPoint(_In_ double x, _In_ double y, _Outptr_result_maybenull_ IRawElementProviderFragment ** retVal);
    IFACEMETHODIMP GetFocus(_Outptr_result_maybenull_ IRawElementProviderFragment ** retVal );

    // IRawElementProviderAdviseEvents methods
    IFACEMETHODIMP AdviseEventAdded(_In_ EVENTID eventId, _In_ SAFEARRAY * propertyIDs);
    IFACEMETHODIMP AdviseEventRemoved(_In_ EVENTID eventId, _In_ SAFEARRAY * propertyIDs);

    // IDisconnectableProvider methods
    IFACEMETHODIMP Disconnect();

protected:

    // Make sure we haven't been disconnected; return the right error if we have.
    HRESULT CheckDisconnected();

protected:

    // Pointer to the window this represents
    Microsoft::WRL::ComPtr<ABI::Windows::UI::Core::ICoreWindow> m_window;

    // Pointer to the main UI object
    Microsoft::WRL::ComPtr<ITriColorControlInternal> m_control;

    // Has this provider been disconnected?
    BOOL m_disconnected;
};

CAppWindowProvider::CAppWindowProvider()
    : m_disconnected(FALSE)
{
}

//
// IAppWindowProvider Implementation
//

IFACEMETHODIMP CAppWindowProvider::Initialize(_In_ ITriColorControlInternal* pControl, _In_ IInspectable* pWindow)
{
    m_control = pControl;
    return pWindow->QueryInterface(IID_PPV_ARGS(&m_window)); 
}

//
// IRawElementProviderSimple Implementation
//

IFACEMETHODIMP CAppWindowProvider::get_ProviderOptions(_Out_ ProviderOptions * pRetVal)
{
    *pRetVal = ProviderOptions_ServerSideProvider | 
               ProviderOptions_UseComThreading | 
               ProviderOptions_UseClientCoordinates;
    return S_OK;
}

IFACEMETHODIMP CAppWindowProvider::GetPatternProvider(_In_ PATTERNID /*patternId*/, _Outptr_result_maybenull_ IUnknown **ppRetVal)
{
    *ppRetVal = nullptr;
    return CheckDisconnected();
}

IFACEMETHODIMP CAppWindowProvider::GetPropertyValue(_In_ PROPERTYID idProp, _Out_ VARIANT * pRetVal)
{
    pRetVal->vt = VT_EMPTY;
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        switch (idProp)
        {
        case UIA_ProviderDescriptionPropertyId:
            pRetVal->bstrVal = SysAllocString(L"CAppWindowProvider");
            if (pRetVal->bstrVal != nullptr)
            {
                pRetVal->vt = VT_BSTR;
            }
            break;

        // case UIA_NamePropertyId:
        // The Name property is deliberately not set.
        // Names for Metro-style applications are set from their manifests.  
        // Setting a name here would be confusing and redundant.

        case UIA_AutomationIdPropertyId:
            pRetVal->bstrVal = SysAllocString(L"CAppWindowProvider0001");
            if (pRetVal->bstrVal != nullptr)
            {
                pRetVal->vt = VT_BSTR;
            }
            break;

        case UIA_IsPasswordPropertyId:
            pRetVal->vt = VT_BOOL;
            pRetVal->boolVal = VARIANT_FALSE;
            break;

        case UIA_ControlTypePropertyId:
            pRetVal->vt = VT_I4;
            pRetVal->lVal = UIA_WindowControlTypeId;
            break;

        case UIA_HasKeyboardFocusPropertyId:
            hr = m_control->get_HasFocus(&pRetVal->boolVal);
            if (SUCCEEDED(hr))
            {
                pRetVal->vt = VT_BOOL;
            }
            break;
        }
    }
    return hr;
}

// Respond to UIA's request for a host provider.
// We can get a host provider from ICoreWindow, but we need to QI it for IRawElementProviderSimple
// before we can return it.
IFACEMETHODIMP CAppWindowProvider::get_HostRawElementProvider(_Outptr_result_maybenull_ IRawElementProviderSimple **ppRetVal)
{
    *ppRetVal = nullptr;
    if (m_window != nullptr)
    {
        IInspectable *pHostAsInspectable = nullptr;
        if (SUCCEEDED(m_window->get_AutomationHostProvider(&pHostAsInspectable)))
        {
            pHostAsInspectable->QueryInterface(ppRetVal);
            pHostAsInspectable->Release();
        }
    }
    return S_OK;
}

//
// IRawElementProviderFragment Implementation
//

IFACEMETHODIMP CAppWindowProvider::Navigate(_In_ NavigateDirection direction, _Outptr_result_maybenull_ IRawElementProviderFragment ** retVal)
{
    *retVal = nullptr;
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        if (direction == NavigateDirection_FirstChild || direction == NavigateDirection_LastChild)
        {
            ComPtr<IInspectable> spControlProvider;
            hr = m_control->GetTriColorControlProvider(&spControlProvider);
            if (SUCCEEDED(hr))
            {
                hr = spControlProvider.Get()->QueryInterface(IID_PPV_ARGS(retVal));
            }  
        }
    }

    // For the other directions (parent, next, previous) the default of nullptr is correct
    return hr;
}

IFACEMETHODIMP CAppWindowProvider::GetRuntimeId(_Outptr_result_maybenull_ SAFEARRAY ** retVal)
{
    *retVal = nullptr;
    
    // UIA implements this method on behalf of the root window
    return CheckDisconnected();
}

IFACEMETHODIMP CAppWindowProvider::get_BoundingRectangle(_Out_ UiaRect * retVal)
{
    ZeroMemory(retVal, sizeof(*retVal));

    // UIA implements this method on behalf of the root window
    return CheckDisconnected();
}

IFACEMETHODIMP CAppWindowProvider::GetEmbeddedFragmentRoots(_Outptr_result_maybenull_ SAFEARRAY ** retVal)
{
    *retVal = nullptr;

    // We don't have any embedded fragment roots
    return CheckDisconnected();
}

IFACEMETHODIMP CAppWindowProvider::SetFocus()
{
    // UIA implements this method on behalf of the root window
    return CheckDisconnected();
}

IFACEMETHODIMP CAppWindowProvider::get_FragmentRoot(_Outptr_result_maybenull_ IRawElementProviderFragmentRoot ** retVal)
{
    *retVal = nullptr;
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        return QueryInterface(IID_PPV_ARGS(retVal));
    }
    return hr;
}

//
// IRawElementProviderFragmentRoot Implementation
//

IFACEMETHODIMP CAppWindowProvider::ElementProviderFromPoint(_In_ double x, _In_ double y, _Outptr_result_maybenull_ IRawElementProviderFragment ** retVal)
{
    *retVal = nullptr;
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        ABI::Windows::Foundation::Point pt = { static_cast<float>(x), static_cast<float>(y) };

        // Did we hit a color region?
        TriColorValue value;
        if (SUCCEEDED(m_control->ValueFromPoint(pt, &value)))
        {
            // Return the color region
            ComPtr<IInspectable> spFragmentProvider;
            hr = m_control->GetTriColorFragmentProvider(value, &spFragmentProvider);
            if (SUCCEEDED(hr))
            {
                hr = spFragmentProvider.Get()->QueryInterface(IID_PPV_ARGS(retVal));
            }  
        }
        // Otherwise, we just hit the app window itself, and we don't need to do anything for that;
        // nullptr is the correct response.
    }

    return hr;
}

IFACEMETHODIMP CAppWindowProvider::GetFocus(_Outptr_result_maybenull_ IRawElementProviderFragment ** retVal)
{
    *retVal = nullptr;
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        // If we are being asked the question at all, then this window must already have focus.
        // In that case, the focused provider is always the control.
        ComPtr<IInspectable> spControlProvider;
        hr = m_control->GetTriColorControlProvider(&spControlProvider);
        if (SUCCEEDED(hr))
        {
            hr = spControlProvider.Get()->QueryInterface(IID_PPV_ARGS(retVal));
        }  
    }

    return hr;
}

//
// IRawElementProviderAdviseEvents implementation
//

// Method exists simply to tell UIA that we want to receive event registrations
IFACEMETHODIMP CAppWindowProvider::AdviseEventAdded(_In_ EVENTID /*eventId*/, _In_ SAFEARRAY * /*propertyIDs*/)
{
    return S_OK;
}

// Method exists simply to tell UIA that we want to receive event registrations
IFACEMETHODIMP CAppWindowProvider::AdviseEventRemoved(_In_ EVENTID /*eventId*/, _In_ SAFEARRAY * /*propertyIDs*/)
{
    return S_OK;
}

//
// IDisconnectableProvider Implementation
//

IFACEMETHODIMP CAppWindowProvider::Disconnect()
{
    // Disconnect and drop our back reference
    m_disconnected = TRUE;
    m_window = nullptr;
    return S_OK;
}

HRESULT CAppWindowProvider::CheckDisconnected()
{
    return (m_disconnected) ? UIA_E_ELEMENTNOTAVAILABLE : S_OK;
}

// Factory function for this class
HRESULT CAppWindowProvider_CreateInstance(IInspectable **retVal)
{
    HRESULT hr = E_OUTOFMEMORY;
    *retVal = nullptr;
    ComPtr<CAppWindowProvider> spProvider = Make<CAppWindowProvider>();
    if (spProvider != nullptr)
    {
        hr = spProvider.Get()->QueryInterface(IID_PPV_ARGS(retVal));
    }
    return hr;
}
