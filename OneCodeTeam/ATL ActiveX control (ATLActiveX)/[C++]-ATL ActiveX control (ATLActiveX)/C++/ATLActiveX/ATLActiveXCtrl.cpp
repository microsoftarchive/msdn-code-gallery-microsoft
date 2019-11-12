/****************************** Module Header ******************************\
* Module Name:  ATLActiveXCtrl.cpp
* Project:      ATLActiveX
* Copyright (c) Microsoft Corporation.
* 
* Define the component's implementation class CATLActiveXCtrl
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma region Includes
#include "stdafx.h"
#include <atlstr.h>

#include "ATLActiveXCtrl.h"
#pragma endregion


IFACEMETHODIMP CATLActiveXCtrl::get_FloatProperty(FLOAT* pVal)
{
    *pVal = m_fField;
    return S_OK;
}

IFACEMETHODIMP CATLActiveXCtrl::put_FloatProperty(FLOAT newVal)
{
    // Fire the FloatPropertyChanging event.
    VARIANT_BOOL cancel = VARIANT_FALSE; 
    Fire_FloatPropertyChanging(newVal, &cancel);

    if (cancel == VARIANT_FALSE)
    {
        m_fField = newVal;	// Save the new value

        // Display the new value in the control UI
        CString strFloatProp;
        strFloatProp.Format(_T("%f"), m_fField);
        SetDlgItemText(IDC_FLOATPROP_STATIC, strFloatProp);
    }

    return S_OK;
}

IFACEMETHODIMP CATLActiveXCtrl::HelloWorld(BSTR* pRet)
{
    // Allocate memory for the string.
    *pRet = ::SysAllocString(L"HelloWorld");
    return pRet ? S_OK : E_OUTOFMEMORY;
}

IFACEMETHODIMP CATLActiveXCtrl::GetProcessThreadID(LONG* pdwProcessId, LONG* pdwThreadId)
{
    *pdwProcessId = GetCurrentProcessId();
    *pdwThreadId = GetCurrentThreadId();
    return S_OK;
}

LRESULT CATLActiveXCtrl::OnInitDialog(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
    InPlaceActivate(OLEIVERB_UIACTIVATE);
    // Perform any dialog initialization
    // ...
    return 0;
}

LRESULT CATLActiveXCtrl::OnBnClickedMsgboxBn(WORD /*wNotifyCode*/, 
    WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/)
{
    wchar_t szMessage[256];
    GetDlgItemText(IDC_MSGBOX_EDIT, szMessage, 256);
    MessageBox(szMessage, L"HelloWorld", MB_ICONINFORMATION | MB_OK);

    return 0;
}