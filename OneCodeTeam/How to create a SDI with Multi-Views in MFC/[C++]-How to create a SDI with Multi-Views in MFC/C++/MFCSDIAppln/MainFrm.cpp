
// MainFrm.cpp : implementation of the CMainFrame class
//

#include "stdafx.h"
#include "MFCSDIAppln.h"

#include "MainFrm.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

// CMainFrame

IMPLEMENT_DYNCREATE(CMainFrame, CFrameWnd)

BEGIN_MESSAGE_MAP(CMainFrame, CFrameWnd)
	ON_WM_CREATE()
END_MESSAGE_MAP()

static UINT indicators[] =
{
	ID_SEPARATOR,           // status line indicator
	ID_INDICATOR_CAPS,
	ID_INDICATOR_NUM,
	ID_INDICATOR_SCRL,
};

// CMainFrame construction/destruction

CMainFrame::CMainFrame()
{
	// TODO: add member initialization code here
}

CMainFrame::~CMainFrame()
{
}

int CMainFrame::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
	if (CFrameWnd::OnCreate(lpCreateStruct) == -1)
		return -1;

	if (!m_wndToolBar.CreateEx(this, TBSTYLE_FLAT, WS_CHILD | WS_VISIBLE | CBRS_TOP | CBRS_GRIPPER | CBRS_TOOLTIPS | CBRS_FLYBY | CBRS_SIZE_DYNAMIC) ||
		!m_wndToolBar.LoadToolBar(IDR_MAINFRAME))
	{
		TRACE0("Failed to create toolbar\n");
		return -1;      // fail to create
	}

	if (!m_wndStatusBar.Create(this))
	{
		TRACE0("Failed to create status bar\n");
		return -1;      // fail to create
	}
	m_wndStatusBar.SetIndicators(indicators, sizeof(indicators)/sizeof(UINT));

	// TODO: Delete these three lines if you don't want the toolbar to be dockable
	CRect rect;
	int nIndex = m_wndToolBar.GetToolBarCtrl().CommandToIndex(ID_COMBO);
	m_wndToolBar.SetButtonInfo(nIndex, ID_COMBO, TBBS_SEPARATOR, 205);
	m_wndToolBar.GetToolBarCtrl().GetItemRect(nIndex, &rect);
	rect.top = 1;
	rect.bottom = rect.top + 30 /*drop height*/;
	if (!m_comboBox.Create(CBS_DROPDOWNLIST | WS_VISIBLE |
		WS_TABSTOP | WS_VSCROLL, rect, &m_wndToolBar, ID_COMBO))
	{
		TRACE(_T("Failed to create combo-box\n"));
		return FALSE;
	}

	m_comboBox.AddString(_T("Main View"));
	m_comboBox.AddString(_T("Form View"));
	m_comboBox.AddString(_T("Edit View"));

	m_comboBox.SetCurSel(0);

	m_wndToolBar.EnableDocking(CBRS_ALIGN_ANY);
	EnableDocking(CBRS_ALIGN_ANY);
	DockControlBar(&m_wndToolBar);


	return 0;
}

BOOL CMainFrame::PreCreateWindow(CREATESTRUCT& cs)
{
	if( !CFrameWnd::PreCreateWindow(cs) )
		return FALSE;
	// TODO: Modify the Window class or styles here by modifying
	//  the CREATESTRUCT cs

	return TRUE;
}

BOOL CMainFrame::SwitchView(CRuntimeClass* pNewView)
{
	CView* pOldView = GetActiveView();

	// If no active view for the frame, return FALSE because
	// this function retrieves the current document from the active
	// view.
	if (pOldView == NULL)
		return FALSE;

	// If we're already displaying this kind of view, no need
	// to go further.
	if ((pOldView->IsKindOf(pNewView)) == TRUE)
		return TRUE;

	// Get pointer to CDocument object so that it can be used
	// in the creation process of the new view.
	CDocument * pDoc = pOldView->GetDocument();

	// Set flag so that document will not be deleted when
	// view is destroyed.
	BOOL bAutoDelete = pDoc->m_bAutoDelete;
	pDoc->m_bAutoDelete = FALSE;
	// hide existing view
	pOldView->ShowWindow(SW_HIDE);

	// restore flag
	pDoc->m_bAutoDelete = bAutoDelete;
	
	CCreateContext context;

	// Create new view and redraw.
	context.m_pNewViewClass = pNewView;
	context.m_pCurrentDoc = pDoc;

	CWnd *pWnd = CreateView(&context);


	CView* pNewActiveView = STATIC_DOWNCAST(CView, CreateView(&context));  // Now we can create a new view and get rid of the previous one  

	if (pNewActiveView == NULL)
	{
		TRACE1("Warning: Dynamic create of view type %Fs failed\n", pNewView->m_lpszClassName);
		return FALSE;
	}

	if (pNewActiveView != NULL)
	{
		pNewActiveView->ShowWindow(SW_SHOW);
		pNewActiveView->OnInitialUpdate();
		SetActiveView(pNewActiveView);
		RecalcLayout();
	}

	// Delete existing view
	pOldView->DestroyWindow();

	return TRUE;
}

// CMainFrame diagnostics

#ifdef _DEBUG
void CMainFrame::AssertValid() const
{
	CFrameWnd::AssertValid();
}

void CMainFrame::Dump(CDumpContext& dc) const
{
	CFrameWnd::Dump(dc);
}
#endif //_DEBUG


// CMainFrame message handlers

