// MFCClipboardDlg.h : header file
//

#pragma once
#include "afxwin.h"


// CMFCClipboardDlg dialog
class CMFCClipboardDlg : public CDialog
{
// Construction
public:
	CMFCClipboardDlg(CWnd* pParent = NULL);	// standard constructor

// Dialog Data
	enum { IDD = IDD_MFCCLIPBOARD_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support


// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedCopyButton();
	afx_msg void OnBnClickedCutButton();
	afx_msg void OnBnClickedPasteButton();
private:
	CEdit m_editTarget;
	CEdit m_editSource;
};
