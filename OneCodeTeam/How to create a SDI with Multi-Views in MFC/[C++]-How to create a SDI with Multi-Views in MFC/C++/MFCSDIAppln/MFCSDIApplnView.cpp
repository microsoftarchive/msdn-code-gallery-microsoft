/****************************** Module Header ******************************\
* Module Name:  MFCSDIApplnView.cpp
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

#include "stdafx.h"
// SHARED_HANDLERS can be defined in an ATL project implementing preview, thumbnail
// and search filter handlers and allows sharing of document code with that project.
#ifndef SHARED_HANDLERS
#include "MFCSDIAppln.h"
#endif

#include "MFCSDIApplnDoc.h"
#include "MFCSDIApplnView.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CMFCSDIApplnView

IMPLEMENT_DYNCREATE(CMFCSDIApplnView, CView)

BEGIN_MESSAGE_MAP(CMFCSDIApplnView, CView)
	// Standard printing commands
	ON_COMMAND(ID_FILE_PRINT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_DIRECT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_PREVIEW, &CView::OnFilePrintPreview)
END_MESSAGE_MAP()

// CMFCSDIApplnView construction/destruction

CMFCSDIApplnView::CMFCSDIApplnView()
{
	// TODO: add construction code here

}

CMFCSDIApplnView::~CMFCSDIApplnView()
{
}

BOOL CMFCSDIApplnView::PreCreateWindow(CREATESTRUCT& cs)
{
	// TODO: Modify the Window class or styles here by modifying
	//  the CREATESTRUCT cs

	return CView::PreCreateWindow(cs);
}

// CMFCSDIApplnView drawing

void CMFCSDIApplnView::OnDraw(CDC* pDC)
{
	CMFCSDIApplnDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
	if (!pDoc)
		return;

	CString str = _T("Displaying MainView");

	pDC->DrawText(str, CRect(100, 100, 500, 500), DT_CENTER);
}


// CMFCSDIApplnView printing

BOOL CMFCSDIApplnView::OnPreparePrinting(CPrintInfo* pInfo)
{
	// default preparation
	return DoPreparePrinting(pInfo);
}

void CMFCSDIApplnView::OnBeginPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: add extra initialization before printing
}

void CMFCSDIApplnView::OnEndPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: add cleanup after printing
}


// CMFCSDIApplnView diagnostics

#ifdef _DEBUG
void CMFCSDIApplnView::AssertValid() const
{
	CView::AssertValid();
}

void CMFCSDIApplnView::Dump(CDumpContext& dc) const
{
	CView::Dump(dc);
}

CMFCSDIApplnDoc* CMFCSDIApplnView::GetDocument() const // non-debug version is inline
{
	ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(CMFCSDIApplnDoc)));
	return (CMFCSDIApplnDoc*)m_pDocument;
}
#endif //_DEBUG


// CMFCSDIApplnView message handlers
