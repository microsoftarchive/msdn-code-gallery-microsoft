/****************************** Module Header ******************************\
* Module Name:  MFCSDIAppln.h
* Project:      MFCSDIAppln
* Copyright (c) Microsoft Corporation.
*
* Defines the class behaviors for the application.
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

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"       // main symbols


// CMFCSDIApplnApp:
// See MFCSDIAppln.cpp for the implementation of this class
//

class CMFCSDIApplnApp : public CWinApp
{
public:
	CMFCSDIApplnApp();


// Overrides
public:
	virtual BOOL InitInstance();
	virtual int ExitInstance();

// Implementation
	afx_msg void OnAppAbout();
	DECLARE_MESSAGE_MAP()
};

extern CMFCSDIApplnApp theApp;
