/****************************** Module Header ******************************\
* Module Name:  ToolComboBox.h
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
#pragma once


// CToolComboBox

class CToolComboBox : public CComboBox
{
	DECLARE_DYNAMIC(CToolComboBox)

public:
	CToolComboBox();
	virtual ~CToolComboBox();

protected:
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnCbnSelchange();
};


