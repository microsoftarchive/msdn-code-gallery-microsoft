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
#pragma once



// CMyFrmView form view

class CMyFrmView : public CFormView
{
	DECLARE_DYNCREATE(CMyFrmView)

protected:
	CMyFrmView();           // protected constructor used by dynamic creation
	virtual ~CMyFrmView();

public:
	enum { IDD = IDD_MYFRMVIEW };
#ifdef _DEBUG
	virtual void AssertValid() const;
#ifndef _WIN32_WCE
	virtual void Dump(CDumpContext& dc) const;
#endif
#endif

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()
};


