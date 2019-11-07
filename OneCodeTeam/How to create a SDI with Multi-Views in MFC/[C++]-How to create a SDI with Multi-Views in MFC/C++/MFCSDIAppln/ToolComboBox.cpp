/****************************** Module Header ******************************\
* Module Name:  ToolComboBox.cpp
* Project:      MFCSDIAppln
* Copyright (c) Microsoft Corporation.
*
* This is a custom ComboBox.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#include "stdafx.h"
#include "MFCSDIAppln.h"
#include "ToolComboBox.h"
#include "MainFrm.h"
#include "MFCSDIApplnDoc.h"
#include "MFCSDIApplnView.h"
#include "MyEditView.h"
#include "MyFrmView.h"


// CToolComboBox

IMPLEMENT_DYNAMIC(CToolComboBox, CComboBox)

CToolComboBox::CToolComboBox()
{

}

CToolComboBox::~CToolComboBox()
{
}


BEGIN_MESSAGE_MAP(CToolComboBox, CComboBox)
	ON_CONTROL_REFLECT(CBN_SELCHANGE, &CToolComboBox::OnCbnSelchange)
END_MESSAGE_MAP()

// CToolComboBox message handlers

void CToolComboBox::OnCbnSelchange()
{
	int iCurSel = GetCurSel();

	switch (iCurSel)
	{
	case 0:
		((CMainFrame*)AfxGetMainWnd())->SwitchView(RUNTIME_CLASS(CMFCSDIApplnView));
		break;
	case 1:
		((CMainFrame*)AfxGetMainWnd())->SwitchView(RUNTIME_CLASS(CMyFrmView));
		break;
	case 2:
		((CMainFrame*)AfxGetMainWnd())->SwitchView(RUNTIME_CLASS(CMyEditView));
		break;
	default:
		break;
	}
}
