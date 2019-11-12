
// CppWindowsHookDlg.h : header file
//

#pragma once

// CCppWindowsHookDlg dialog
class CCppWindowsHookDlg : public CDialog
{
// Construction
public:
	CCppWindowsHookDlg(CWnd* pParent = NULL);	// standard constructor

// Dialog Data
	enum { IDD = IDD_CPPWINDOWSHOOK_DIALOG };

	protected:
	afx_msg long OnHookKeyboard(WPARAM wParam, LPARAM lParam);
	afx_msg long OnHookLowKeyboard(WPARAM wParam, LPARAM lParam);

// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	virtual BOOL OnInitDialog();
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedSethook();
	afx_msg void OnBnClickedSethookthread();
	afx_msg void OnBnClickedSethookinput();
	afx_msg void OnBnClickedResettext();
};
