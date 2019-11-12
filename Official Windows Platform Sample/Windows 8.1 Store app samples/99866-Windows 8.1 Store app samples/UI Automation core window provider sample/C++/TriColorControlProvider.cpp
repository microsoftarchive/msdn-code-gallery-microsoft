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
using namespace ABI::UiaCoreWindowProvider;

// CTriColorControlProvider: provider for the tri-color control root

[uuid("CF681FC9-63AF-4708-9250-564D8348388A")]
class CTriColorControlProvider:
    public Microsoft::WRL::RuntimeClass<  
        Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::RuntimeClassType::WinRtClassicComMix>,    
        ITriColorControlProvider,
        IRawElementProviderSimple,
        IRawElementProviderFragment,
        IValueProvider,
        ISelectionProvider,
        IDisconnectableProvider>
{
    InspectableClass(RuntimeClass_UiaCoreWindowProvider_TriColorControlProvider, BaseTrust);

public:
    CTriColorControlProvider();

    // ITriColorControlProvider
    IFACEMETHODIMP Initialize(_In_ ITriColorControlInternal* pControl);

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

    // ISelectionProvider methods
    IFACEMETHODIMP GetSelection(_Outptr_result_maybenull_ SAFEARRAY * *retVal);
    IFACEMETHODIMP get_CanSelectMultiple(_Out_ BOOL *retVal);
    IFACEMETHODIMP get_IsSelectionRequired(_Out_ BOOL *retVal);

    // IValueProvider methods
    IFACEMETHODIMP SetValue(_In_ LPCWSTR val);
    IFACEMETHODIMP get_Value(_Out_ BSTR *retVal);
    IFACEMETHODIMP get_IsReadOnly(_Out_ BOOL *retVal);

    // IDisconnectableProvider methods
    IFACEMETHODIMP Disconnect();

protected:

    // Make sure we haven't been disconnected; return the right error if we have.
    HRESULT CheckDisconnected();

protected:

    // Pointer to the main UI object
    Microsoft::WRL::ComPtr<ITriColorControlInternal> m_control;

    // Has this provider been disconnected?
    BOOL m_disconnected;
};

CTriColorControlProvider::CTriColorControlProvider()
    : m_disconnected(FALSE)
{
}

//
// ITriColorControlProvider Implementation
//

IFACEMETHODIMP CTriColorControlProvider::Initialize(_In_ ITriColorControlInternal* pControl)
{
    m_control = pControl;
    return S_OK;
}

//
// IRawElementProviderSimple Implementation
//

IFACEMETHODIMP CTriColorControlProvider::get_ProviderOptions(_Out_ ProviderOptions * retVal)
{
    // ProviderOptions_UseClientCoordinates constant is new for Windows 8 and 
    // instructs UIA that this provider works in terms of client coordinates rather than screen coordinates.
    // Constant may not be available yet in the SDK, depending on when the SDK was constructed.
    *retVal = ProviderOptions_ServerSideProvider | 
              ProviderOptions_UseComThreading | 
              (ProviderOptions)0x100 /*ProviderOptions_UseClientCoordinates*/;
    return S_OK;
}

IFACEMETHODIMP CTriColorControlProvider::GetPatternProvider(_In_ PATTERNID patternId, _Outptr_result_maybenull_ IUnknown **retVal)
{
    *retVal = nullptr;
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        if (patternId == UIA_ValuePatternId || patternId == UIA_SelectionPatternId)
        {
            *retVal = static_cast<IValueProvider*>(this);
            AddRef();
        }
    }
    return hr;
}

IFACEMETHODIMP CTriColorControlProvider::GetPropertyValue(_In_ PROPERTYID idProp, _Out_ VARIANT * retVal)
{
    retVal->vt = VT_EMPTY;
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        switch (idProp)
        {
        case UIA_ProviderDescriptionPropertyId:
            retVal->bstrVal = SysAllocString(L"CTriColorControlProvider");
            if (retVal->bstrVal != nullptr)
            {
                retVal->vt = VT_BSTR;
            }
            break;

        case UIA_NamePropertyId:
            // FUTURE: Should be localized
            retVal->bstrVal = SysAllocString(L"Ready state");
            if (retVal->bstrVal != nullptr)
            {
                retVal->vt = VT_BSTR;
            }
            break;

        case UIA_AutomationIdPropertyId:
            retVal->bstrVal = SysAllocString(L"CTriColorControlProvider0001");
            if (retVal->bstrVal != nullptr)
            {
                retVal->vt = VT_BSTR;
            }
            break;

        case UIA_IsPasswordPropertyId:
            retVal->vt = VT_BOOL;
            retVal->boolVal = VARIANT_FALSE;
            break;

        case UIA_ControlTypePropertyId:
            retVal->lVal = UIA_CustomControlTypeId;
            retVal->vt = VT_I4;
            break;

        case UIA_LocalizedControlTypePropertyId:
            // FUTURE: Should be localized
            retVal->bstrVal = SysAllocString(L"tri-color picker");
            if (retVal->bstrVal != nullptr)
            {
                retVal->vt = VT_BSTR;
            }
            break;

        case UIA_HelpTextPropertyId:
            // FUTURE: Should be localized
            retVal->bstrVal = SysAllocString(L"This is a color picker for a choice of three colors.  Use Up and Down arrows to move the selection between the colors.");
            if (retVal->bstrVal != nullptr)
            {
                retVal->vt = VT_BSTR;
            }
            break;

        case UIA_IsKeyboardFocusablePropertyId:
        case UIA_IsContentElementPropertyId:
        case UIA_IsControlElementPropertyId:
        case UIA_IsEnabledPropertyId:
            retVal->boolVal = VARIANT_TRUE;
            retVal->vt = VT_BOOL;
            break;

        case UIA_HasKeyboardFocusPropertyId:
            hr = m_control->get_HasFocus(&retVal->boolVal);
            if (SUCCEEDED(hr))
            {
                retVal->vt = VT_BOOL;
            }
            break;
        }
    }
    return hr;
}

// Respond to UIA's request for a host provider - we don't have one for this element.
IFACEMETHODIMP CTriColorControlProvider::get_HostRawElementProvider(_Outptr_result_maybenull_ IRawElementProviderSimple **retVal)
{
    *retVal = nullptr;
    return S_OK;
}

//
// IRawElementProviderFragment Implementation
//

IFACEMETHODIMP CTriColorControlProvider::Navigate(_In_ NavigateDirection direction, _Outptr_result_maybenull_ IRawElementProviderFragment ** retVal)
{
    *retVal = nullptr;
    ComPtr<IInspectable> spFragment;
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        if (direction == NavigateDirection_Parent)
        {
            hr = m_control->GetAppWindowProvider(&spFragment);
        }
        else if (direction == NavigateDirection_FirstChild)
        {
            hr = m_control->GetTriColorFragmentProvider(TriColorValue::Red, &spFragment);
        }
        else if (direction == NavigateDirection_LastChild)
        {
            hr = m_control->GetTriColorFragmentProvider(TriColorValue::Green, &spFragment);
        }
    }

    if (SUCCEEDED(hr) && spFragment != nullptr)
    {
        hr = spFragment.Get()->QueryInterface(IID_PPV_ARGS(retVal));
    }

    // For the other directions (next, previous) the default of nullptr is correct
    return hr;
}

IFACEMETHODIMP CTriColorControlProvider::GetRuntimeId(_Outptr_result_maybenull_ SAFEARRAY ** retVal)
{
    *retVal = nullptr;
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        int runtimeId[2] = { UiaAppendRuntimeId, 1 /* this is always control 1 */ };
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

IFACEMETHODIMP CTriColorControlProvider::get_BoundingRectangle(_Out_ UiaRect * retVal)
{
    ZeroMemory(retVal, sizeof(*retVal));
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        ABI::Windows::Foundation::Rect controlRect;
        hr = m_control->get_ControlRect(&controlRect);
        if (SUCCEEDED(hr))
        {
            retVal->left = controlRect.X;
            retVal->top = controlRect.Y;
            retVal->width = controlRect.Width;
            retVal->height = controlRect.Height;
        }
    }

    return hr;
}

IFACEMETHODIMP CTriColorControlProvider::GetEmbeddedFragmentRoots(_Outptr_result_maybenull_ SAFEARRAY ** retVal)
{
    *retVal = nullptr;

    // We don't have any embedded fragment roots
    return CheckDisconnected();
}

IFACEMETHODIMP CTriColorControlProvider::SetFocus()
{
    // UIA will take care of this for us.
    return CheckDisconnected();
}

IFACEMETHODIMP CTriColorControlProvider::get_FragmentRoot(_Outptr_result_maybenull_ IRawElementProviderFragmentRoot ** retVal)
{
    *retVal = nullptr;
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        ComPtr<IInspectable> spParentFragment;
        hr = m_control->GetAppWindowProvider(&spParentFragment);
        if (SUCCEEDED(hr))
        {
            hr = spParentFragment.Get()->QueryInterface(IID_PPV_ARGS(retVal));
        }
    }
    return hr;
}

//
// ISelectionProvider Implementation
//

IFACEMETHODIMP CTriColorControlProvider::GetSelection(_Outptr_result_maybenull_ SAFEARRAY * *retVal) 
{ 
    *retVal = nullptr;
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        TriColorValue controlValue;
        hr = m_control->get_ControlValue(&controlValue);
        if (SUCCEEDED(hr))
        {
            ComPtr<IInspectable> spFragmentProvider;
            hr = m_control->GetTriColorFragmentProvider(controlValue, &spFragmentProvider);
            if (SUCCEEDED(hr))
            {
                *retVal = SafeArrayCreateVector(VT_UNKNOWN, 0, 1);
                if (*retVal != nullptr)
                {
                    long index = 0;
                    hr = SafeArrayPutElement(*retVal, &index, spFragmentProvider.Get());
                    if (FAILED(hr))
                    {
                        SafeArrayDestroy(*retVal);
                        *retVal = nullptr;
                    }
                }
                else
                {
                    hr = E_OUTOFMEMORY;
                }
            }
        }
    }

    return hr; 
}

IFACEMETHODIMP CTriColorControlProvider::get_CanSelectMultiple(_Out_ BOOL *retVal) 
{ 
    *retVal = FALSE;
    return CheckDisconnected();
}

IFACEMETHODIMP CTriColorControlProvider::get_IsSelectionRequired(_Out_ BOOL *retVal) 
{ 
    *retVal = TRUE;
    return CheckDisconnected();
}

//
// IValueProvider Implementation
//

IFACEMETHODIMP CTriColorControlProvider::SetValue(_In_ LPCWSTR val) 
{ 
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        TriColorValue newValue = TriColorValue::Red;
        bool bFound = false;
        if (0 == _wcsicmp(val, L"red"))
        {
            newValue = TriColorValue::Red;
            bFound = true;
        }
        else if (0 == _wcsicmp(val, L"yellow"))
        {
            newValue = TriColorValue::Yellow;
            bFound = true;
        } 
        else if (0 == _wcsicmp(val, L"green"))
        {
            newValue = TriColorValue::Green;
            bFound = true;
        }

        if (bFound)
        {
            hr = m_control->put_ControlValue(newValue);
        }
        else
        {
            hr = E_INVALIDARG;
        }
    }
    return hr;
}

IFACEMETHODIMP CTriColorControlProvider::get_Value(_Out_ BSTR *retVal) 
{ 
    *retVal = nullptr;
    HRESULT hr = CheckDisconnected();
    if (SUCCEEDED(hr))
    {
        TriColorValue controlValue;
        hr = m_control->get_ControlValue(&controlValue);
        if (SUCCEEDED(hr))
        {
            hr = TriColorValueHelper::ValueToString(controlValue, retVal);
        }
    }

    return hr;
}

IFACEMETHODIMP CTriColorControlProvider::get_IsReadOnly(_Out_ BOOL *retVal) 
{ 
    *retVal = FALSE;
    return CheckDisconnected();
}

//
// IDisconnectableProvider Implementation
//

IFACEMETHODIMP CTriColorControlProvider::Disconnect()
{
    // Disconnect and drop our back reference
    m_disconnected = TRUE;
    m_control = nullptr;
    return S_OK;
}

HRESULT CTriColorControlProvider::CheckDisconnected()
{
    return (m_disconnected) ? UIA_E_ELEMENTNOTAVAILABLE : S_OK;
}

// Factory function for this class
HRESULT CTriColorControlProvider_CreateInstance(IInspectable **retVal)
{
    HRESULT hr = E_OUTOFMEMORY;
    *retVal = nullptr;
    ComPtr<CTriColorControlProvider> spProvider = Make<CTriColorControlProvider>();
    if (spProvider != nullptr)
    {
        hr = spProvider.Get()->QueryInterface(IID_PPV_ARGS(retVal));
    }
    return hr;
}

