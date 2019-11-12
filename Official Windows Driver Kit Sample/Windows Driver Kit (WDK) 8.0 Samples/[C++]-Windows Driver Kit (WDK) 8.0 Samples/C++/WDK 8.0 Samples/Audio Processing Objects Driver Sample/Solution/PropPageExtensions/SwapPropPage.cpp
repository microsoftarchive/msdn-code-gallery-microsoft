//**@@@*@@@****************************************************
//
// Microsoft Windows
// Copyright (C) Microsoft Corporation. All rights reserved.
//
//**@@@*@@@****************************************************

//
// FileName:    SwapPropPage.cpp
//
// Abstract:    Implementation of the CSwapPropPage class
//
// ----------------------------------------------------------------------------


#include "stdafx.h"
#include <initguid.h>   // DEFINE_GUID
#include <mmdeviceapi.h>
#include <audioenginebaseapo.h>
#include "SwapPropPage.h"
#include <functiondiscoverykeys.h>
#include <CustomPropKeys.h>
#include "cplext_i.c"

_Analysis_mode_(_Analysis_code_type_user_driver_)

// ----------------------------------------------------------------------------
// Function:
//      CSwapPropPage::CSwapPropPage
//
// Description:
//      CSwapPropPage constructor
// ----------------------------------------------------------------------------
CSwapPropPage::CSwapPropPage()
:   m_pAudioFXExtParams(NULL)
{
}


// ----------------------------------------------------------------------------
// Function:
//      CSwapPropPage::~CSwapPropPage
//
// Description:
//      CSwapPropPage destructor
// ----------------------------------------------------------------------------
CSwapPropPage::~CSwapPropPage()
{
    SAFE_RELEASE(m_pAudioFXExtParams->pFxProperties);
    SAFE_DELETE(m_pAudioFXExtParams);
}


// ----------------------------------------------------------------------------
// Function:
//      CSwapPropPage::GetDeviceFriendlyName
//
// Description:
//      Retrieves the endpoint's friendly name
//
// Parameters:
//      ppNameOut - [out] The friendly name of the endpoint
//
// Return values:
//      S_OK if successful
// ----------------------------------------------------------------------------
HRESULT CSwapPropPage::GetDeviceFriendlyName
(
    _Outptr_result_maybenull_ LPWSTR* ppNameOut
)
{
    CComPtr<IMMDeviceEnumerator>    spEnumerator;
    CComPtr<IPropertyStore>         spProperties;
    CComPtr<IMMDevice>              spMMDevice;
    HRESULT                         hr = S_OK;
    PROPVARIANT                     var;

    IF_TRUE_ACTION_JUMP((ppNameOut == NULL), hr = E_POINTER, Exit);

    *ppNameOut = NULL;

    // Create device enumerator and get IMMDevice from the device ID
    hr = spEnumerator.CoCreateInstance(__uuidof(MMDeviceEnumerator));
    IF_FAILED_JUMP(hr, Exit);

    hr = spEnumerator->GetDevice(m_pAudioFXExtParams->pwstrEndpointID, &spMMDevice);
    IF_FAILED_JUMP(hr, Exit);

    // Open the PropertyStore for read access
    hr = spMMDevice->OpenPropertyStore(STGM_READ, &spProperties);
    IF_FAILED_JUMP(hr, Exit);

    PropVariantInit(&var);

    // Retrieve the friendly name of the endpoint
    hr = spProperties->GetValue(PKEY_Device_FriendlyName, &var);
    if (SUCCEEDED(hr) && (var.vt == VT_LPWSTR))
    {
        *ppNameOut = var.pwszVal;
    }
    else
    {
        PropVariantClear(&var);
    }

Exit:
    return(hr);
}


// ----------------------------------------------------------------------------
// Function:
//      CSwapPropPage::RetrieveSysFXState
//
// Description:
//      Get the current state (enabled or disabled) of system effects
//
// Return values:
//      S_OK if successful
// ----------------------------------------------------------------------------
HRESULT CSwapPropPage::RetrieveSysFXState()
{
    HRESULT hr = E_POINTER;

    if ((m_pAudioFXExtParams != NULL) && (m_pAudioFXExtParams->pFxProperties != NULL))
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Get the state of whether system effects are enabled or not
        hr = m_pAudioFXExtParams->pFxProperties->GetValue(PKEY_AudioEndpoint_Disable_SysFx, &var);
        if (SUCCEEDED(hr) && (var.vt == VT_UI4))
        {
            if (var.ulVal == 0L)
            {
                m_fDisableSysFX = FALSE;
            }
            else
            {
                m_fDisableSysFX = TRUE;
            }
        }

        PropVariantClear(&var);
    }

    return(hr);
}


// ----------------------------------------------------------------------------
// Function:
//      CSwapPropPage::SetSysFXState
//
// Description:
//      Enable or disable system effects
//
// Return values:
//      S_OK if successful
// ----------------------------------------------------------------------------
HRESULT CSwapPropPage::SetSysFXState()
{
    HRESULT hr = E_POINTER;

    if ((m_pAudioFXExtParams != NULL) && (m_pAudioFXExtParams->pFxProperties != NULL))
    {
        PROPVARIANT var;
        var.vt = VT_UI4;

        if (m_fDisableSysFX)
        {
            var.ulVal = 1L;
        }
        else
        {
            var.ulVal = 0L;
        }

        // Enable or disable SysFX
        hr = m_pAudioFXExtParams->pFxProperties->SetValue(PKEY_AudioEndpoint_Disable_SysFx, var);
    }

    return(hr);
}


// ----------------------------------------------------------------------------
// Function:
//      CSwapPropPage::RetrieveSwapLFXState
//
// Description:
//      Get the current state (enabled or disabled) of channel swap LFX
//
// Return values:
//      S_OK if successful
// ----------------------------------------------------------------------------
HRESULT CSwapPropPage::RetrieveSwapLFXState()
{
    HRESULT hr = E_POINTER;

    if ((m_pAudioFXExtParams != NULL) && (m_pAudioFXExtParams->pFxProperties != NULL))
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Get the state of whether channel swap LFX is enabled or not
        hr = m_pAudioFXExtParams->pFxProperties->GetValue(PKEY_Endpoint_Enable_Channel_Swap_LFX, &var);
        if (SUCCEEDED(hr) && (var.vt == VT_UI4))
        {
            if (var.ulVal == 0L)
            {
                m_fEnableSwapLFX = FALSE;
            }
            else
            {
                m_fEnableSwapLFX = TRUE;
            }
        }

        PropVariantClear(&var);
    }

    return(hr);
}


// ----------------------------------------------------------------------------
// Function:
//      CSwapPropPage::SetSwapLFXState
//
// Description:
//      Enable or disable channel swap LFX
//
// Return values:
//      S_OK if successful
// ----------------------------------------------------------------------------
HRESULT CSwapPropPage::SetSwapLFXState()
{
    HRESULT                 hr = S_OK;
    PROPVARIANT             var;

    IF_TRUE_ACTION_JUMP((m_pAudioFXExtParams == NULL), hr = E_POINTER, Exit);

    PropVariantInit(&var);

    if (m_pAudioFXExtParams->pFxProperties != NULL)
    {
        var.vt = VT_UI4;

        if (m_fEnableSwapLFX)
        {
            var.ulVal = 1L;
        }
        else
        {
            var.ulVal = 0L;
        }

        // Enable or disable channel swap LFX
        hr = m_pAudioFXExtParams->pFxProperties->SetValue(PKEY_Endpoint_Enable_Channel_Swap_LFX, var);
    }

Exit:
    return(hr);
}


// ----------------------------------------------------------------------------
// Function:
//      CSwapPropPage::RetrieveSwapGFXState
//
// Description:
//      Get the current state (enabled or disabled) of channel swap GFX
//
// Return values:
//      S_OK if successful
// ----------------------------------------------------------------------------
HRESULT CSwapPropPage::RetrieveSwapGFXState()
{
    HRESULT hr = E_POINTER;

    if ((m_pAudioFXExtParams != NULL) && (m_pAudioFXExtParams->pFxProperties != NULL))
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Get the state of whether channel swap GFX is enabled or not
        hr = m_pAudioFXExtParams->pFxProperties->GetValue(PKEY_Endpoint_Enable_Channel_Swap_GFX, &var);
        if (SUCCEEDED(hr) && (var.vt == VT_UI4))
        {
            if (var.ulVal == 0L)
            {
                m_fEnableSwapGFX = FALSE;
            }
            else
            {
                m_fEnableSwapGFX = TRUE;
            }
        }

        PropVariantClear(&var);
    }

    return(hr);
}


// ----------------------------------------------------------------------------
// Function:
//      CSwapPropPage::SetSwapGFXState
//
// Description:
//      Enable or disable channel swap GFX
//
// Return values:
//      S_OK if successful
// ----------------------------------------------------------------------------
HRESULT CSwapPropPage::SetSwapGFXState()
{
    HRESULT                 hr = S_OK;
    PROPVARIANT             var;

    IF_TRUE_ACTION_JUMP((m_pAudioFXExtParams == NULL), hr = E_POINTER, Exit);

    PropVariantInit(&var);

    if (m_pAudioFXExtParams->pFxProperties != NULL)
    {
        var.vt = VT_UI4;

        if (m_fEnableSwapGFX)
        {
            var.ulVal = 1L;
        }
        else
        {
            var.ulVal = 0L;
        }

        // Enable or disable channel swap GFX
        hr = m_pAudioFXExtParams->pFxProperties->SetValue(PKEY_Endpoint_Enable_Channel_Swap_GFX, var);
    }

Exit:
    return(hr);
}


// ----------------------------------------------------------------------------
// Function:
//      CSwapPropPage::OnInitDialog
//
// Description:
//      Dialog initialization routine
//
// Parameters:
//      hwndDlg - [in] Handle to dialog box
//      wParam - [in] Handle to control to receive the default keyboard focus
//      lParam - [in] Specifies additional message-specific information
//
// Return values:
//      TRUE to direct the system to set the keyboard focus to the control
//      specified by wParam. Otherwise, it should return FALSE to prevent the
//      system from setting the default keyboard focus.
// ----------------------------------------------------------------------------
BOOL CSwapPropPage::OnInitDialog
(
    HWND hwndDlg,
    WPARAM wParam,
    LPARAM lParam
)
{
    UNREFERENCED_PARAMETER(wParam);
    UNREFERENCED_PARAMETER(lParam);

    HRESULT hr = S_OK;
    LPWSTR pwstrEndpointName = NULL;

    // Retrieve the endpoint's friendly name, system effects, and swap LFX and GFX states
    hr = GetDeviceFriendlyName(&pwstrEndpointName);
    IF_FAILED_JUMP(hr, Exit);

    hr = RetrieveSysFXState();
    IF_FAILED_JUMP(hr, Exit);

    hr = RetrieveSwapLFXState();
    IF_FAILED_JUMP(hr, Exit);

    hr = RetrieveSwapGFXState();
    IF_FAILED_JUMP(hr, Exit);

    // Update the property page with retrieved information
    SetWindowText(GetDlgItem(hwndDlg, IDC_SPP_ENDPOINT_NAME), pwstrEndpointName);

    // Based on the retrieved states, toggle the checkboxes to reflect them
    if (m_fDisableSysFX)
    {
        CheckDlgButton(hwndDlg, IDC_DISABLE_SYSFX, BST_CHECKED);

        // Disable APO toggling controls on the page
        EnableWindow(GetDlgItem(hwndDlg, IDC_ENABLE_SWAP_LFX), FALSE);
        EnableWindow(GetDlgItem(hwndDlg, IDC_ENABLE_SWAP_GFX), FALSE);
    }
    else
    {
        CheckDlgButton(hwndDlg, IDC_DISABLE_SYSFX, BST_UNCHECKED);

        // Enable APO toggling controls on the page
        EnableWindow(GetDlgItem(hwndDlg, IDC_ENABLE_SWAP_LFX), TRUE);
        EnableWindow(GetDlgItem(hwndDlg, IDC_ENABLE_SWAP_GFX), TRUE);
    }

    if (m_fEnableSwapLFX)
    {
        CheckDlgButton(hwndDlg, IDC_ENABLE_SWAP_LFX, BST_CHECKED);
    }
    else
    {
        CheckDlgButton(hwndDlg, IDC_ENABLE_SWAP_LFX, BST_UNCHECKED);
    }

    if (m_fEnableSwapGFX)
    {
        CheckDlgButton(hwndDlg, IDC_ENABLE_SWAP_GFX, BST_CHECKED);
    }
    else
    {
        CheckDlgButton(hwndDlg, IDC_ENABLE_SWAP_GFX, BST_UNCHECKED);
    }

Exit:
    SAFE_COTASKMEMFREE(pwstrEndpointName);
    return(FALSE);
}


// ----------------------------------------------------------------------------
// Function:
//      CSwapPropPage::OnApply
//
// Description:
//      Handle the pressing of the apply button
//
// Parameters:
//      hwndDlg - [in] Handle to the dialog box
//
// Return values:
//      TRUE to set keyboard focus on control
// ----------------------------------------------------------------------------
BOOL CSwapPropPage::OnApply
(
    HWND hwndDlg
)
{
    HRESULT hr = S_OK;

    // Commit the settings
    hr = SetSysFXState();
    IF_FAILED_JUMP(hr, Exit);

    hr = SetSwapLFXState();
    IF_FAILED_JUMP(hr, Exit);

    hr = SetSwapGFXState();
    IF_FAILED_JUMP(hr, Exit);

Exit:
    if (SUCCEEDED(hr))
    {
        SetWindowLongPtr(hwndDlg, DWLP_MSGRESULT, PSNRET_NOERROR);
    }
    else
    {
        SetWindowLongPtr(hwndDlg, DWLP_MSGRESULT, PSNRET_INVALID);
    }

    return(TRUE);
}


// ----------------------------------------------------------------------------
// Function:
//      CSwapPropPage::OnCheckBoxClickedDisableSysFX
//
// Description:
//      Handle the clicking of the Disable System Effects check box
//
// Parameters:
//      hwndDlg - [in] Handle to the dialog box
//
// Return values:
//      FALSE to not set default keyboard focus
// ----------------------------------------------------------------------------
BOOL CSwapPropPage::OnCheckBoxClickedDisableSysFX
(
    HWND hwndDlg
)
{
    // Check the state of the check box and update associated data member
    if (BST_CHECKED == IsDlgButtonChecked(hwndDlg, IDC_DISABLE_SYSFX))
    {
        m_fDisableSysFX = TRUE;

        // Disable APO toggling controls on the page
        EnableWindow(GetDlgItem(hwndDlg, IDC_ENABLE_SWAP_LFX), FALSE);
        EnableWindow(GetDlgItem(hwndDlg, IDC_ENABLE_SWAP_GFX), FALSE);
    }
    else
    {
        m_fDisableSysFX = FALSE;

        // Enable APO toggling controls on the page
        EnableWindow(GetDlgItem(hwndDlg, IDC_ENABLE_SWAP_LFX), TRUE);
        EnableWindow(GetDlgItem(hwndDlg, IDC_ENABLE_SWAP_GFX), TRUE);
    }

    // If the user changes the check box, enable the Apply button
    SendMessage(GetParent(hwndDlg), PSM_CHANGED, (WPARAM)hwndDlg, 0);

    return(FALSE);
}


// ----------------------------------------------------------------------------
// Function:
//      CSwapPropPage::OnCheckBoxClickedEnableSwapLFX
//
// Description:
//      Handle the clicking of the Enable Channel Swap LFX check box
//
// Parameters:
//      hwndDlg - [in] Handle to the dialog box
//
// Return values:
//      FALSE to not set default keyboard focus
// ----------------------------------------------------------------------------
BOOL CSwapPropPage::OnCheckBoxClickedEnableSwapLFX
(
    HWND hwndDlg
)
{
    // Check the state of the check box and update associated data member
    if (BST_CHECKED == IsDlgButtonChecked(hwndDlg, IDC_ENABLE_SWAP_LFX))
    {
        m_fEnableSwapLFX = TRUE;
    }
    else
    {
        m_fEnableSwapLFX = FALSE;
    }

    // If the user changes the check box, enable the Apply button
    SendMessage(GetParent(hwndDlg), PSM_CHANGED, (WPARAM)hwndDlg, 0);

    return(FALSE);
}


// ----------------------------------------------------------------------------
// Function:
//      CSwapPropPage::OnCheckBoxClickedEnableSwapGFX
//
// Description:
//      Handle the clicking of the Enable Channel Swap GFX check box
//
// Parameters:
//      hwndDlg - [in] Handle to the dialog box
//
// Return values:
//      FALSE to not set default keyboard focus
// ----------------------------------------------------------------------------
BOOL CSwapPropPage::OnCheckBoxClickedEnableSwapGFX
(
    HWND hwndDlg
)
{
    // Check the state of the check box and update associated data member
    if (BST_CHECKED == IsDlgButtonChecked(hwndDlg, IDC_ENABLE_SWAP_GFX))
    {
        m_fEnableSwapGFX = TRUE;
    }
    else
    {
        m_fEnableSwapGFX = FALSE;
    }

    // If the user changes the check box, enable the Apply button
    SendMessage(GetParent(hwndDlg), PSM_CHANGED, (WPARAM)hwndDlg, 0);

    return(FALSE);
}


// ----------------------------------------------------------------------------
// Function:
//      CSwapPropPage::DialogProcPage1
//
// Description:
//      Callback for property page
//
// Parameters:
//      hwndDlg - [in] Handle to the dialog box
//      uMsg - [in] Specifies the message
//      wParam - [in] Specifies additional message-specific information
//      lParam - [in] Specifies additional message-specific information
//
// Return values:
//      TRUE if it processed the message, FALSE if not
// ----------------------------------------------------------------------------
INT_PTR CALLBACK CSwapPropPage::DialogProcPage1
(
    HWND    hwndDlg,
    UINT    uMsg,
    WPARAM  wParam,
    LPARAM  lParam
)
{
    CSwapPropPage* pthis = (CSwapPropPage*)(LONG_PTR)GetWindowLongPtr(
                                hwndDlg, GWLP_USERDATA);
    BOOL fRet = FALSE;

    switch (uMsg)
    {
        case WM_INITDIALOG:
        {
            // Extract the context data from PROPSHEETPAGE::lParam
            PROPSHEETPAGE*  pSheetDesc = (PROPSHEETPAGE*)lParam;

            // Create the property page factory class
#pragma warning(push)
#pragma warning(disable: 28197)
            pthis = new CComObject<CSwapPropPage>();
#pragma warning(pop)
            if (pthis == NULL)
            {
                return(FALSE);
            }

            // Save this object in lParam
            SetWindowLongPtr(hwndDlg, GWLP_USERDATA, (LONG_PTR)pthis);

            // Keep audio FX extension parameters passed by the control panel
            pthis->m_pAudioFXExtParams = (AudioFXExtensionParams*)pSheetDesc->lParam;

            fRet = pthis->OnInitDialog(hwndDlg, wParam, lParam);
            break;
        }

        case WM_NOTIFY:
        {
            switch (((NMHDR FAR*)lParam)->code)
            {
                case PSN_APPLY:
                    if (pthis)
                    {
                        // Apply button pressed
                        fRet = pthis->OnApply(hwndDlg);
                    }
                    break;
            }
            break;
        }

        case WM_COMMAND:
        {
            switch (LOWORD(wParam))
            {
                // Handle the clicking of the check boxes
                case IDC_DISABLE_SYSFX:
                    if (pthis)
                    {
                        fRet = pthis->OnCheckBoxClickedDisableSysFX(hwndDlg);
                    }
                    break;

                case IDC_ENABLE_SWAP_LFX:
                    if (pthis)
                    {
                        fRet = pthis->OnCheckBoxClickedEnableSwapLFX(hwndDlg);
                    }
                    break;

                case IDC_ENABLE_SWAP_GFX:
                    if (pthis)
                    {
                        fRet = pthis->OnCheckBoxClickedEnableSwapGFX(hwndDlg);
                    }
                    break;
            }
            break;
        }

        case WM_DESTROY:
        {
            SAFE_DELETE(pthis);
            SetWindowLongPtr(hwndDlg, GWLP_USERDATA, NULL);
            fRet = TRUE;
            break;
        }
    }

    return(fRet);
}


// ----------------------------------------------------------------------------
// Function:
//      CSwapPropPage::PropSheetPageProc
//
// Description:
//      Callback that gets invoked right after page creation or right before
//      before page destruction
//
// Parameters:
//      hwnd - Reserved; must be NULL
//      uMsg - [in] Action flag. PSPCB_ADDREF, PSPCB_CREATE, or PSPCB_RELEASE
//      ppsp - [in, out] Pointer to a PROPSHEETPAGE structure that defines
//             the page being created or destroyed.
//
// Return values:
//      Depends on the value of the uMsg parameter
// ----------------------------------------------------------------------------
UINT CALLBACK CSwapPropPage::PropSheetPageProc
(
    HWND            hwnd,
    UINT            uMsg,
    LPPROPSHEETPAGE ppsp
)
{
    UNREFERENCED_PARAMETER(hwnd);
    UNREFERENCED_PARAMETER(uMsg);
    UNREFERENCED_PARAMETER(ppsp);

    // if (uMsg == PSPCB_CREATE) ...
    return(1);
}


// ----------------------------------------------------------------------------
// Function:
//      CSwapPropPage::Initialize
//
// Description:
//      Implementation of IShellExtInit::Initialize. Initializes a property
//      sheet extension, shortcut menu extension, or drag-and-drop handler.
//
// Parameters:
//      pidlFolder - [in] Address of an ITEMIDLIST structure that uniquely
//                   identifies a folder. For property sheet extensions,
//                   this parameter is NULL.
//      pdtobj - [out] Address of an IDataObject interface object that can be
//               used to retrieve the objects being acted upon. 
//      hkeyProgID - [in] Registry key for the file object or folder type.
//
// Return values:
//      Returns NOERROR if successful, or an OLE-defined error value otherwise
// ----------------------------------------------------------------------------
_Use_decl_annotations_
HRESULT CSwapPropPage::Initialize
(
    LPCITEMIDLIST   pidlFolder,
    IDataObject*    pdtobj,
    HKEY            hkeyProgID
)
{
    UNREFERENCED_PARAMETER(pidlFolder);
    UNREFERENCED_PARAMETER(pdtobj);
    UNREFERENCED_PARAMETER(hkeyProgID);

    return(S_OK);
}


// ----------------------------------------------------------------------------
// Function:
//      CSwapPropPage::AddPages
//
// Description:
//      Implementation of IShellPropSheetExt::AddPages. Adds one or more pages
//      to a property sheet that the Shell displays for a file object.
//
// Parameters:
//      lpfnAddPage - [in] Address of a function that the property sheet
//                    handler calls to add a page to the property sheet. The
//                    function takes a property sheet handle returned by the
//                    CreatePropertySheetPage function and the lParam parameter
//                    passed to the AddPages method. 
//      lParam - [in] Parameter to pass to the function specified by the
//               lpfnAddPage method.
//
// Return values:
//      Returns S_OK if successful. If the method fails, an OLE-defined error
//      code is returned
// ----------------------------------------------------------------------------
_Use_decl_annotations_
HRESULT STDMETHODCALLTYPE CSwapPropPage::AddPages
(
    LPFNADDPROPSHEETPAGE    lpfnAddPage,    // See PrSht.h
    LPARAM                  lParam          // Used by caller, don't modify
)
{
    HRESULT                 hr = S_OK;
    PROPSHEETPAGE           psp;
    HPROPSHEETPAGE          hPage1 = NULL;
    AudioFXExtensionParams* pAudioFXParams = (AudioFXExtensionParams*)lParam;
#pragma warning(push)
#pragma warning(disable: 28197)
    AudioFXExtensionParams* pAudioFXParamsCopy = new AudioFXExtensionParams;
#pragma warning(pop)

    if (pAudioFXParamsCopy == NULL)
    {
        return E_OUTOFMEMORY;
    }

    // Make a copy of the params
    CopyMemory(pAudioFXParamsCopy, pAudioFXParams, sizeof(AudioFXExtensionParams));
    SAFE_ADDREF(pAudioFXParamsCopy->pFxProperties);

    // Initialize property page params and create page
    psp.dwSize        = sizeof(psp);
    psp.dwFlags       = PSP_USEREFPARENT | PSP_USECALLBACK;
    psp.hInstance     = _AtlBaseModule.GetModuleInstance();
    psp.hIcon         = 0;
    psp.pcRefParent   = (UINT*)&m_dwRef;
    psp.lParam        = (LPARAM)pAudioFXParamsCopy;
    psp.pszTemplate   = MAKEINTRESOURCE(IDD_SWAP_PROP_PAGE);
    psp.pfnDlgProc    = (DLGPROC)DialogProcPage1;
    psp.pfnCallback   = PropSheetPageProc;

    // Create the property sheet page and add the page
    hPage1 = CreatePropertySheetPage(&psp);
    if (hPage1)
    {
        if (!lpfnAddPage(hPage1, pAudioFXParams->AddPageParam))
        {
            hr = E_FAIL;
            delete pAudioFXParamsCopy;
            DestroyPropertySheetPage(hPage1);
        }
        else
        {
            // Add ref for page
            this->AddRef();
        }
    }
    else
    {
        delete pAudioFXParamsCopy;
        hr = E_OUTOFMEMORY;
    }

    return(hr);
}


// ----------------------------------------------------------------------------
// Function:
//      CSwapPropPage::ReplacePage
//
// Description:
//      Implementation of IShellPropSheetExt::ReplacePage. Replaces a page in
//      a property sheet for a Control Panel object.
//
// Parameters:
//      uPageID - [in] Identifier of the page to replace 
//      lpfnReplacePage - [in] Address of a function that the property sheet
//                        handler calls to replace a page to the property
//                        sheet. The function takes a property sheet handle
//                        returned by the CreatePropertySheetPage function and
//                        the lParam parameter passed to the ReplacePage
//                        method.
//      lParam - [in] Parameter to pass to the function specified by the
//               lpfnReplacePage parameter. 
//
// Return values:
//      Returns NOERROR if successful, or an OLE-defined error value otherwise
// ----------------------------------------------------------------------------
_Use_decl_annotations_
HRESULT STDMETHODCALLTYPE CSwapPropPage::ReplacePage
(
    UINT                    uPageID,
    LPFNSVADDPROPSHEETPAGE  lpfnReplaceWith,
    LPARAM                  lParam
)
{
    UNREFERENCED_PARAMETER(uPageID);
    UNREFERENCED_PARAMETER(lpfnReplaceWith);
    UNREFERENCED_PARAMETER(lParam);

    return(S_FALSE);
}
