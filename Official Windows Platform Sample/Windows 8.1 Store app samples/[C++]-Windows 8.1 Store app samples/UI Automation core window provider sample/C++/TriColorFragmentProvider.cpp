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
#include "TriColorValue.h"

using namespace ABI::Windows::Foundation;
using namespace Microsoft::WRL;
using namespace ABI::Windows::UI;
using namespace ABI::Windows::UI::Core;
using namespace ABI::Windows::System;

// CTriColorFragmentProvider: provider for the tri-color control fragments

[uuid("E4D3F978-3027-4EFA-A52A-6E8FC3BCAC70")]
class CTriColorFragmentProvider:
    public Microsoft::WRL::RuntimeClass<  
        Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::RuntimeClassType::WinRtClassicComMix>,
        ITriColorFragmentProvider,
        IRawElementProviderSimple,
        IRawElementProviderFragment,
        ISelectionItemProvider,
        IDisconnectableProvider>
{
    InspectableClass(RuntimeClass_UiaCoreWindowProvider_TriColorFragmentProvider, BaseTrust);

public:
    CTriColorFragmentProvider();

    // ITriColorFragmentProvider
    IFACEMETHODIMP Initialize(_In_ ITriColorControlInternal* pControl, TriColorValue value);

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

    // ISelectionItemProvider
    STDMETHODIMP Select();
    STDMETHODIMP AddToSelection();
    STDMETHODIMP RemoveFromSelection();
    STDMETHODIMP get_IsSelected(_Out_ BOOL *retVal);
    STDMETHODIMP get_SelectionContainer(_Outptr_result_maybenull_ IRawElementProviderSimple **retVal);

    // IDisconnectableProvider methods
    IFACEMETHODIMP Disconnect();

protected:

    // Make sure we haven't been disconnected; return the right error if we have.
    HRESULT CheckDisconnected();

protected:

    // Pointer to the main UI object
    Microsoft::WRL::ComPtr<ITriColorControlInternal> m_control;

    // The color value of this provider
    TriColorValue m_value;

    // Has this provider been disconnected?
    BOOL m_disconnected;
};

CTriColorFragmentProvider::CTriColorFragmentProvider()
    : m_disconnected(FALSE)
{
}

//
// ITriColorFragmentProvider
//
IFACEMETHODIMP CTriColorFragmentProvider::Initialize(_In_ ITriColorControlInternal* pControl, TriColorValue value)
{
    m_control = pControl;    
    m_value = value;
    return S_OK;
}

//
// IRawElementProviderSimple Implementation
//

IFACEMETHODIMP CTriColorFragmentProvider::get_ProviderOptions(_Out_ ProviderOptions * retVal)
{
    // ProviderOptions_UseClientCoordinates constant is new for Windows 8 and 
    // instructs UIA that this provider works in terms of client coordinates rather than screen coordinates.
    // Constant may not be available yet in the SDK, depending on when the SDK was constructed.
    *retVal = ProviderOptions_ServerSideProvider | 
              ProviderOptions_UseComThreading | 
              (ProviderOptions)0x100 /*ProviderOptions_UseClientCoordinates*/;
    return S_OK;
}

IFACEMETHODIMP CTriColorFragmentProvider::GetPatternProvider(_In_ PATTERNID patternId, _Outptr_result_maybenull_ IUnknown **retVal)
{
    *retVal = nullptr;
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        if (patternId == UIA_SelectionItemPatternId)
        {
            *retVal = static_cast<ISelectionItemProvider*>(this);
            AddRef();
        }
    }
    return hr;
}

IFACEMETHODIMP CTriColorFragmentProvider::GetPropertyValue(_In_ PROPERTYID idProp, _Out_ VARIANT * retVal)
{
    retVal->vt = VT_EMPTY;
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        switch (idProp)
        {
        case UIA_ProviderDescriptionPropertyId:
            retVal->bstrVal = SysAllocString(L"CTriColorCtrl Fragment Provider");
            if (retVal->bstrVal != nullptr)
            {
                retVal->vt = VT_BSTR;
            }
            break;

        case UIA_NamePropertyId:
            // FUTURE: Should be localized
            hr = TriColorValueHelper::ValueToString(m_value, &retVal->bstrVal);
            if (SUCCEEDED(hr))
            {
                retVal->vt = VT_BSTR;
            }
            break;

        case UIA_AutomationIdPropertyId:
            hr = TriColorValueHelper::ValueToString(m_value, &retVal->bstrVal);
            if (SUCCEEDED(hr))
            {
                retVal->vt = VT_BSTR;
            }
            break;

        case UIA_ControlTypePropertyId:
            retVal->lVal = UIA_CustomControlTypeId;
            retVal->vt = VT_I4;
            break;

        case UIA_LocalizedControlTypePropertyId:
            // FUTURE: Should be localized
            retVal->bstrVal = SysAllocString(L"tri-color item");
            if (retVal->bstrVal != nullptr)
            {
                retVal->vt = VT_BSTR;
            }
            break;

        case UIA_IsKeyboardFocusablePropertyId:
        case UIA_IsContentElementPropertyId:
        case UIA_HasKeyboardFocusPropertyId:
            retVal->boolVal = VARIANT_FALSE;
            retVal->vt = VT_BOOL;
            break;

        case UIA_IsControlElementPropertyId:
        case UIA_IsEnabledPropertyId:
            retVal->boolVal = VARIANT_TRUE;
            retVal->vt = VT_BOOL;
            break;
        }
    }
    return hr;
}

// Respond to UIA's request for a host provider - we don't have one for this element.
IFACEMETHODIMP CTriColorFragmentProvider::get_HostRawElementProvider(_Outptr_result_maybenull_ IRawElementProviderSimple **retVal)
{
    *retVal = nullptr;
    return S_OK;
}

//
// IRawElementProviderFragment Implementation
//

IFACEMETHODIMP CTriColorFragmentProvider::Navigate(_In_ NavigateDirection direction, _Outptr_result_maybenull_ IRawElementProviderFragment ** retVal)
{
    *retVal = nullptr;
    ComPtr<IInspectable> spFragment;
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        if (direction == NavigateDirection_Parent)
        {
            hr = m_control->GetTriColorControlProvider(&spFragment);
        }
        else if (direction == NavigateDirection_NextSibling)
        {
            if (!TriColorValueHelper::IsLast(m_value))
            {
                hr = m_control->GetTriColorFragmentProvider(TriColorValueHelper::NextValue(m_value), &spFragment);
            }
        }
        else if (direction == NavigateDirection_PreviousSibling)
        {
            if (!TriColorValueHelper::IsFirst(m_value))
            {
                hr = m_control->GetTriColorFragmentProvider(TriColorValueHelper::PreviousValue(m_value), &spFragment);
            }
        }
    }

    if (SUCCEEDED(hr) && spFragment != nullptr)
    {
        hr = spFragment.Get()->QueryInterface(IID_PPV_ARGS(retVal));
    }

    // For the other directions (first child, last child) the default of nullptr is correct
    return hr;
}

// Return the runtime ID for this fragment.
// Our runtime ID is the UiaAppendRuntimeId constant, which will be resolved into our window ID,
// combined with a unique ID for our control (which is just #1, for this sample)
// combined with a unique ID for this fragment (which is our value turned into an integer)
IFACEMETHODIMP CTriColorFragmentProvider::GetRuntimeId(_Outptr_result_maybenull_ SAFEARRAY ** retVal)
{
    *retVal = nullptr;
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        int runtimeId[3] = { UiaAppendRuntimeId, 1 /* control ID */, static_cast<int>(m_value) };
        *retVal = SafeArrayCreateVector(VT_I4, 0, ARRAYSIZE(runtimeId));
        if (*retVal != nullptr)
        {
            for (long index = 0; index < ARRAYSIZE(runtimeId); ++index)
            {
               SafeArrayPutElement(*retVal, &index, &runtimeId[index]);
            }
        }
        else
        {
            hr = E_OUTOFMEMORY;
        }
    }

    return hr;
}

IFACEMETHODIMP CTriColorFragmentProvider::get_BoundingRectangle(_Out_ UiaRect * retVal)
{
    ZeroMemory(retVal, sizeof(*retVal));
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        ABI::Windows::Foundation::Rect valueRect;
        hr = m_control->RectFromValue(m_value, &valueRect);
        if (SUCCEEDED(hr))
        {
            retVal->left = valueRect.X;
            retVal->top = valueRect.Y;
            retVal->width = valueRect.Width;
            retVal->height = valueRect.Height;
        }
    }

    return hr;
}

IFACEMETHODIMP CTriColorFragmentProvider::GetEmbeddedFragmentRoots(_Outptr_result_maybenull_ SAFEARRAY ** retVal)
{
    *retVal = nullptr;

    // We don't have any embedded fragment roots
    return CheckDisconnected();
}

IFACEMETHODIMP CTriColorFragmentProvider::SetFocus()
{
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        // This is not focusable.
        hr = UIA_E_INVALIDOPERATION;
    }
    return hr;
}

IFACEMETHODIMP CTriColorFragmentProvider::get_FragmentRoot(_Outptr_result_maybenull_ IRawElementProviderFragmentRoot ** retVal)
{
    *retVal = nullptr;
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        ComPtr<IInspectable> spFragmentRoot;
        hr = m_control->GetAppWindowProvider(&spFragmentRoot);
        if (SUCCEEDED(hr))
        {
            hr = spFragmentRoot.Get()->QueryInterface(IID_PPV_ARGS(retVal));
        }
    }
    return hr;
}

//
// ISelectionItemProvider Implementation
//

STDMETHODIMP CTriColorFragmentProvider::Select()
{ 
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        hr = m_control->put_ControlValue(m_value);
    }
    return hr;
}

STDMETHODIMP CTriColorFragmentProvider::AddToSelection()
{ 
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        // Is this fragment already selected?
        TriColorValue controlValue;
        hr = m_control->get_ControlValue(&controlValue);
        if (SUCCEEDED(hr))
        {
            if (controlValue != m_value)
            {
                // Cannot do multiple selection
                hr = UIA_E_INVALIDOPERATION;
            }
            else
            {
                // It's already selected - operation succeeded.
                hr = S_OK;
            }
        }
    }
    return hr;
}

STDMETHODIMP CTriColorFragmentProvider::RemoveFromSelection()
{ 
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        // Is this fragment already selected?
        TriColorValue controlValue;
        hr = m_control->get_ControlValue(&controlValue);
        if (SUCCEEDED(hr))
        {
            if (controlValue == m_value)
            {
                // Cannot do multiple selection
                hr = UIA_E_INVALIDOPERATION;
            }
            else
            {
                // It's already unselected - operation succeeded.
                hr = S_OK;
            }
        }
    }
    return hr;
}

STDMETHODIMP CTriColorFragmentProvider::get_IsSelected(_Out_ BOOL *retVal)
{
    *retVal = FALSE;
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        // Is this fragment already selected?
        TriColorValue controlValue;
        hr = m_control->get_ControlValue(&controlValue);
        if (SUCCEEDED(hr))
        {
            *retVal = (controlValue == m_value);
        }
    }
    return hr;
}

STDMETHODIMP CTriColorFragmentProvider::get_SelectionContainer(_Outptr_result_maybenull_ IRawElementProviderSimple **retVal)
{
    *retVal = nullptr;
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        ComPtr<IInspectable> spParentFragment;
        hr = m_control->GetTriColorControlProvider(&spParentFragment);
        if (SUCCEEDED(hr))
        {
            hr = spParentFragment.Get()->QueryInterface(IID_PPV_ARGS(retVal));
        }
    }
    return hr;
}

//
// IDisconnectableProvider Implementation
//

IFACEMETHODIMP CTriColorFragmentProvider::Disconnect()
{
    // Disconnect and drop our back reference
    m_disconnected = TRUE;
    m_control = nullptr;
    return S_OK;
}

HRESULT CTriColorFragmentProvider::CheckDisconnected()
{
    return (m_disconnected) ? UIA_E_ELEMENTNOTAVAILABLE : S_OK;
}

// Factory function for this class
HRESULT CTriColorFragmentProvider_CreateInstance(IInspectable **retVal)
{
    HRESULT hr = E_OUTOFMEMORY;
    *retVal = nullptr;
    ComPtr<CTriColorFragmentProvider> spProvider = Make<CTriColorFragmentProvider>();
    if (spProvider != nullptr)
    {
        hr = spProvider.Get()->QueryInterface(IID_PPV_ARGS(retVal));
    }
    return hr;
}
