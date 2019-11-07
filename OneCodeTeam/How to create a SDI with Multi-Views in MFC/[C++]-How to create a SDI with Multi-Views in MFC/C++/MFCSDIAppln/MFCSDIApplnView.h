/****************************** Module Header ******************************\
* Module Name:  MFCSDIApplnView.h
* Project:      MFCSDIAppln
* Copyright (c) Microsoft Corporation.
*
* This is Main View.
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


class CMFCSDIApplnView : public CView
{
protected: // create from serialization only
	CMFCSDIApplnView();
	DECLARE_DYNCREATE(CMFCSDIApplnView)

// Attributes
public:
	CMFCSDIApplnDoc* GetDocument() const;

// Operations
public:

// Overrides
public:
	virtual void OnDraw(CDC* pDC);  // overridden to draw this view
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
protected:
	virtual BOOL OnPreparePrinting(CPrintInfo* pInfo);
	virtual void OnBeginPrinting(CDC* pDC, CPrintInfo* pInfo);
	virtual void OnEndPrinting(CDC* pDC, CPrintInfo* pInfo);

// Implementation
public:
	virtual ~CMFCSDIApplnView();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

// Generated message map functions
protected:
	DECLARE_MESSAGE_MAP()
};

#ifndef _DEBUG  // debug version in MFCSDIApplnView.cpp
inline CMFCSDIApplnDoc* CMFCSDIApplnView::GetDocument() const
   { return reinterpret_cast<CMFCSDIApplnDoc*>(m_pDocument); }
#endif

