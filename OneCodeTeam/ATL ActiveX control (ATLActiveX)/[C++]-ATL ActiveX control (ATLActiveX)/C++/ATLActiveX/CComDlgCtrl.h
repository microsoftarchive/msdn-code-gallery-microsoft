/****************************** Module Header ******************************\
* Module Name:  CComDlgCtrl.h
* Project:      ATLActiveX
* Copyright (c) Microsoft Corporation.
* 
* CComDlgCtrl is designed for writing dialog-based based ActiveX control 
* using ATL. The class is based on CComControl, and is derived from 
* CDialogImpl instead of CWindowImpl. 
* See http://support.microsoft.com/kb/175503 for more details.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


template <class T>
class ATL_NO_VTABLE CComDlgCtrl : public CComControlBase,
    public CDialogImpl<T>
{
public:
    CComDlgCtrl() : CComControlBase(m_hWnd) {}

    HRESULT FireOnRequestEdit(DISPID dispID)
    {
        T* pT = static_cast<T*>(this);
        return T::__ATL_PROP_NOTIFY_EVENT_CLASS::FireOnRequestEdit
            (pT->GetUnknown(), dispID);
    }

    HRESULT FireOnChanged(DISPID dispID)
    {
        T* pT = static_cast<T*>(this);
        return T::__ATL_PROP_NOTIFY_EVENT_CLASS::FireOnChanged
            (pT->GetUnknown(), dispID);
    }

    virtual HRESULT ControlQueryInterface(const IID& iid, void** ppv)
    {
        T* pT = static_cast<T*>(this);
        return pT->_InternalQueryInterface(iid, ppv);
    }

    virtual HWND CreateControlWindow(HWND hWndParent, RECT& rcPos)
    {
        T* pT = static_cast<T*>(this);
        return pT->Create(hWndParent);
        // CDialogImpl::Create differs from CWindowImpl
    }
};