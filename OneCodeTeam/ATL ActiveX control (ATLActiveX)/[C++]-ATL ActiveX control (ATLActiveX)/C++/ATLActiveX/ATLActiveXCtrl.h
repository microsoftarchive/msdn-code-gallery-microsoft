/****************************** Module Header ******************************\
* Module Name:  ATLActiveXCtrl.h
* Project:      ATLActiveX
* Copyright (c) Microsoft Corporation.
* 
* Declare the component's implementation class CATLActiveXCtrl.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma once

#pragma region Includes
#include "resource.h"       // main symbols
#include <atlctl.h>
#include "ATLActiveX_i.h"
#include "_IATLActiveXCtrlEvents_CP.h"

#include "CComDlgCtrl.h"
#pragma endregion


#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "Single-threaded COM objects are not properly supported on Windows CE platform, such as the Windows Mobile platforms that do not include full DCOM support. Define _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA to force ATL to support creating single-thread COM object's and allow use of it's single-threaded COM object implementations. The threading model in your rgs file was set to 'Free' as that is the only threading model supported in non DCOM Windows CE platforms."
#endif

class CATLActiveXCtrlLic
{
protected:
    static BOOL VerifyLicenseKey(BSTR bstr)
    {
        return !lstrcmpW(bstr, L"ATLActiveXCtrl license");
    }

    static BOOL GetLicenseKey(DWORD dwReserved, BSTR* pBstr)
    {
        if( pBstr == NULL )
            return FALSE;
        *pBstr = SysAllocString(L"ATLActiveXCtrl license");
        return TRUE;
    }

    static BOOL IsLicenseValid()
    {
        return TRUE;
    }
};


// CATLActiveXCtrl
class ATL_NO_VTABLE CATLActiveXCtrl :
    public CComObjectRootEx<CComSingleThreadModel>,
    public CStockPropImpl<CATLActiveXCtrl, IATLActiveXCtrl>,
    public IPersistStreamInitImpl<CATLActiveXCtrl>,
    public IOleControlImpl<CATLActiveXCtrl>,
    public IOleObjectImpl<CATLActiveXCtrl>,
    public IOleInPlaceActiveObjectImpl<CATLActiveXCtrl>,
    public IViewObjectExImpl<CATLActiveXCtrl>,
    public IOleInPlaceObjectWindowlessImpl<CATLActiveXCtrl>,
    public ISupportErrorInfo,
    public IConnectionPointContainerImpl<CATLActiveXCtrl>,
    public CProxy_IATLActiveXCtrlEvents<CATLActiveXCtrl>,
    public IPersistStorageImpl<CATLActiveXCtrl>,
    public ISpecifyPropertyPagesImpl<CATLActiveXCtrl>,
    public IQuickActivateImpl<CATLActiveXCtrl>,
#ifndef _WIN32_WCE
    public IDataObjectImpl<CATLActiveXCtrl>,
#endif
    public IProvideClassInfo2Impl<&CLSID_ATLActiveXCtrl, &__uuidof(_IATLActiveXCtrlEvents), &LIBID_ATLActiveXLib>,
#ifdef _WIN32_WCE // IObjectSafety is required on Windows CE for the control to be loaded correctly
    public IObjectSafetyImpl<CATLActiveXCtrl, INTERFACESAFE_FOR_UNTRUSTED_CALLER>,
#endif
    public CComCoClass<CATLActiveXCtrl, &CLSID_ATLActiveXCtrl>,
    public CComDlgCtrl<CATLActiveXCtrl>	// CComDlgCtrl replaced CComControl
{
public:

    // Update class definition to identify the dialog resource.
    enum { IDD = IDD_MAINDIALOG };


    CATLActiveXCtrl() : m_fField(0.0f)
    {
        // Force the control to be non-Windowless
        m_bWindowOnly = 1;
    }

    DECLARE_CLASSFACTORY2(CATLActiveXCtrlLic)

    DECLARE_OLEMISC_STATUS(OLEMISC_RECOMPOSEONRESIZE |
    OLEMISC_CANTLINKINSIDE |
        OLEMISC_INSIDEOUT |
        OLEMISC_ACTIVATEWHENVISIBLE |
        OLEMISC_SETCLIENTSITEFIRST
        )

        DECLARE_REGISTRY_RESOURCEID(IDR_ATLACTIVEXCTRL)


    BEGIN_COM_MAP(CATLActiveXCtrl)
        COM_INTERFACE_ENTRY(IATLActiveXCtrl)
        COM_INTERFACE_ENTRY(IDispatch)
        COM_INTERFACE_ENTRY(IViewObjectEx)
        COM_INTERFACE_ENTRY(IViewObject2)
        COM_INTERFACE_ENTRY(IViewObject)
        COM_INTERFACE_ENTRY(IOleInPlaceObjectWindowless)
        COM_INTERFACE_ENTRY(IOleInPlaceObject)
        COM_INTERFACE_ENTRY2(IOleWindow, IOleInPlaceObjectWindowless)
        COM_INTERFACE_ENTRY(IOleInPlaceActiveObject)
        COM_INTERFACE_ENTRY(IOleControl)
        COM_INTERFACE_ENTRY(IOleObject)
        COM_INTERFACE_ENTRY(IPersistStreamInit)
        COM_INTERFACE_ENTRY2(IPersist, IPersistStreamInit)
        COM_INTERFACE_ENTRY(ISupportErrorInfo)
        COM_INTERFACE_ENTRY(IConnectionPointContainer)
        COM_INTERFACE_ENTRY(ISpecifyPropertyPages)
        COM_INTERFACE_ENTRY(IQuickActivate)
        COM_INTERFACE_ENTRY(IPersistStorage)
#ifndef _WIN32_WCE
        COM_INTERFACE_ENTRY(IDataObject)
#endif
        COM_INTERFACE_ENTRY(IProvideClassInfo)
        COM_INTERFACE_ENTRY(IProvideClassInfo2)
#ifdef _WIN32_WCE // IObjectSafety is required on Windows CE for the control to be loaded correctly
        COM_INTERFACE_ENTRY_IID(IID_IObjectSafety, IObjectSafety)
#endif
    END_COM_MAP()

    BEGIN_PROP_MAP(CATLActiveXCtrl)
        PROP_DATA_ENTRY("_cx", m_sizeExtent.cx, VT_UI4)
        PROP_DATA_ENTRY("_cy", m_sizeExtent.cy, VT_UI4)
#ifndef _WIN32_WCE
        PROP_ENTRY_TYPE("BackColor", DISPID_BACKCOLOR, CLSID_StockColorPage, VT_DISPATCH)
#endif
        PROP_ENTRY_TYPE("Enabled", DISPID_ENABLED, CLSID_NULL, VT_BOOL)
        // Example entries
        // PROP_ENTRY_TYPE("Property Name", dispid, clsid, vtType)
        // PROP_PAGE(CLSID_StockColorPage)
    END_PROP_MAP()

    BEGIN_CONNECTION_POINT_MAP(CATLActiveXCtrl)
        CONNECTION_POINT_ENTRY(__uuidof(_IATLActiveXCtrlEvents))
    END_CONNECTION_POINT_MAP()

    BEGIN_MSG_MAP(CATLActiveXCtrl)
        MESSAGE_HANDLER(WM_INITDIALOG, OnInitDialog)
        // CHAIN_MSG_MAP(CComControl<CATLActiveXCtrl>)
        DEFAULT_REFLECTION_HANDLER()
        COMMAND_HANDLER(IDC_MSGBOX_BN, BN_CLICKED, OnBnClickedMsgboxBn)
    END_MSG_MAP()
    // Handler prototypes:
    //  LRESULT MessageHandler(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled);
    //  LRESULT CommandHandler(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled);
    //  LRESULT NotifyHandler(int idCtrl, LPNMHDR pnmh, BOOL& bHandled);

    // ISupportsErrorInfo
    IFACEMETHOD(InterfaceSupportsErrorInfo)(REFIID riid)
    {
        static const IID* arr[] =
        {
            &IID_IATLActiveXCtrl,
        };

        for (int i=0; i<sizeof(arr)/sizeof(arr[0]); i++)
        {
            if (InlineIsEqualGUID(*arr[i], riid))
                return S_OK;
        }
        return S_FALSE;
    }

    // IViewObjectEx
    DECLARE_VIEW_STATUS(VIEWSTATUS_SOLIDBKGND | VIEWSTATUS_OPAQUE)

    // IATLActiveXCtrl
public:
    //	HRESULT OnDraw(ATL_DRAWINFO& di)
    //	{
    //		// 
    //		// Change the UI of the control - Method1. Draw the UI.
    //		// 
    //
    //		HDC hdc = di.hdcDraw;
    //		RECT& rc = *(RECT*)di.prcBounds;
    //
    //		// Translate m_clrBackColor into a COLORREF type
    //		COLORREF clrBack;
    //		OleTranslateColor(this->m_clrBackColor, NULL, &clrBack);
    //
    //		HBRUSH hBrush = CreateSolidBrush(clrBack);
    //		HBRUSH hOldBrush = (HBRUSH)SelectObject(hdc, hBrush);
    //		
    //		Rectangle(hdc, rc.left, rc.top, rc.right, rc.bottom);
    //
    //		// Set the background color
    //		SetBkColor(hdc, clrBack);
    //
    //		SetTextAlign(hdc, TA_CENTER | TA_BASELINE);
    //		LPCTSTR pszText = _T("ATL 8.0 : ATLActiveXCtrl");
    //#ifndef _WIN32_WCE
    //		TextOut(hdc,
    //			(rc.left + rc.right) / 2,
    //			(rc.top + rc.bottom) / 2,
    //			pszText,
    //			lstrlen(pszText));
    //#else
    //		ExtTextOut(hdc,
    //			(rc.left + rc.right) / 2,
    //			(rc.top + rc.bottom) / 2,
    //			ETO_OPAQUE,
    //			NULL,
    //			pszText,
    //			ATL::lstrlen(pszText),
    //			NULL);
    //#endif
    //
    //		// Select back the old brush and delete the brush we created
    //		SelectObject(hdc, hOldBrush);
    //		DeleteObject(hBrush);
    //
    //		return S_OK;
    //	}

    // Override IOleInPlaceActiveObjectImpl::TranslateAccelerator to handle 
    // tabbing and other navigation keys correctly.
    IFACEMETHOD(TranslateAccelerator)(MSG *pMsg)
    {
        if ((pMsg->message < WM_KEYFIRST || pMsg->message > WM_KEYLAST) &&
            (pMsg->message < WM_MOUSEFIRST || pMsg->message > WM_MOUSELAST))
        {
            return S_FALSE;
        }
        return (IsDialogMessage(pMsg)) ? S_OK : S_FALSE;
    }

    OLE_COLOR m_clrBackColor;
    void OnBackColorChanged()
    {
        ATLTRACE(_T("OnBackColorChanged\n"));
    }
    BOOL m_bEnabled;
    void OnEnabledChanged()
    {
        ATLTRACE(_T("OnEnabledChanged\n"));
    }

    DECLARE_PROTECT_FINAL_CONSTRUCT()

    HRESULT FinalConstruct()
    {
        return S_OK;
    }

    void FinalRelease()
    {
    }

protected:
    // Used by FloatProperty
    float m_fField;

public:

    IFACEMETHOD(get_FloatProperty)(FLOAT *pVal);
    IFACEMETHOD(put_FloatProperty)(FLOAT newVal);
    IFACEMETHOD(HelloWorld)(BSTR *pRet);
    IFACEMETHOD(GetProcessThreadID)(LONG *pdwProcessId, LONG *pdwThreadId);
    LRESULT OnInitDialog(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled);
    LRESULT OnBnClickedMsgboxBn(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled);
};

OBJECT_ENTRY_AUTO(__uuidof(ATLActiveXCtrl), CATLActiveXCtrl)