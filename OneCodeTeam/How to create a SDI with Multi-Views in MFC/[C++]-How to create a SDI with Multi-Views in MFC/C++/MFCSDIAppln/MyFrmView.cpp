/****************************** Module Header ******************************\
* Module Name:  MyFrmView.h
* Project:      MFCSDIAppln
* Copyright (c) Microsoft Corporation.
*
* This is a FormView.
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
#include "MyFrmView.h"


// CMyFrmView

IMPLEMENT_DYNCREATE(CMyFrmView, CFormView)

CMyFrmView::CMyFrmView()
	: CFormView(CMyFrmView::IDD)
{

}

CMyFrmView::~CMyFrmView()
{
}

void CMyFrmView::DoDataExchange(CDataExchange* pDX)
{
	CFormView::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CMyFrmView, CFormView)
END_MESSAGE_MAP()


// CMyFrmView diagnostics

#ifdef _DEBUG
void CMyFrmView::AssertValid() const
{
	CFormView::AssertValid();
}

#ifndef _WIN32_WCE
void CMyFrmView::Dump(CDumpContext& dc) const
{
	CFormView::Dump(dc);
}
#endif
#endif //_DEBUG


// CMyFrmView message handlers
