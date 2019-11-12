/****************************** Module Header ******************************\
* Module Name:  MFCActiveXCtrlPage.cpp
* Project:      MFCCOMClient
* Copyright (c) Microsoft Corporation.
* 
* 
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
#include "MFCCOMClient.h"
#include "MFCActiveXCtrlPage.h"
#pragma endregion


// CMFCActiveXCtrlPage dialog

IMPLEMENT_DYNAMIC(CMFCActiveXCtrlPage, CDialog)

CMFCActiveXCtrlPage::CMFCActiveXCtrlPage(CWnd* pParent /*=NULL*/)
	: CDialog(CMFCActiveXCtrlPage::IDD, pParent)
	, m_fEditFloatProperty(0)
{
}

CMFCActiveXCtrlPage::~CMFCActiveXCtrlPage()
{
}

void CMFCActiveXCtrlPage::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_MFCACTIVEXCTRL, m_ocxActiveXCtrl);
	DDX_Text(pDX, IDC_FLOATPROP_EDIT, m_fEditFloatProperty);
	DDV_MinMaxFloat(pDX, m_fEditFloatProperty, -99999999.0F, 99999999.0F);
}


BEGIN_MESSAGE_MAP(CMFCActiveXCtrlPage, CDialog)
	ON_BN_CLICKED(IDC_SETFLOATPROP_BN, &CMFCActiveXCtrlPage::OnBnClickedSetFloatPropBn)
	ON_BN_CLICKED(IDC_GETFLOATPROP_BN, &CMFCActiveXCtrlPage::OnBnClickedGetFloatPropBn)
END_MESSAGE_MAP()


// CMFCActiveXCtrlPage message handlers
BEGIN_EVENTSINK_MAP(CMFCActiveXCtrlPage, CDialog)
	ON_EVENT(CMFCActiveXCtrlPage, IDC_MFCACTIVEXCTRL, 1, CMFCActiveXCtrlPage::FloatPropertyChangingMFCActiveXCtrl, VTS_R4 VTS_PBOOL)
END_EVENTSINK_MAP()


void CMFCActiveXCtrlPage::FloatPropertyChangingMFCActiveXCtrl(
	float NewValue, BOOL* Cancel)
{
	CString strMessage;
	strMessage.Format(_T("FloatProperty is being changed to %f"), NewValue);

	// OK or cancel the change of FloatProperty
	*Cancel = (IDCANCEL == MessageBox(strMessage,
		_T("MFCActiveX!FloatPropertyChanging"), MB_OKCANCEL));
}

void CMFCActiveXCtrlPage::OnBnClickedSetFloatPropBn()
{
	// Verify the value in the FloatProperty Edit control
	if (UpdateData())
	{
		// Set FloatProperty to the ActiveX control
		m_ocxActiveXCtrl.SetFloatProperty(m_fEditFloatProperty);
	}
}

void CMFCActiveXCtrlPage::OnBnClickedGetFloatPropBn()
{
	// Get FloatProperty from the ActiveX control
	FLOAT fProp = m_ocxActiveXCtrl.GetFloatProperty();

	CString strMessage;
	strMessage.Format(_T("FloatProperty equals %f"), fProp);

	MessageBox(strMessage, _T("MFCActiveX!FloatProperty"));
}
