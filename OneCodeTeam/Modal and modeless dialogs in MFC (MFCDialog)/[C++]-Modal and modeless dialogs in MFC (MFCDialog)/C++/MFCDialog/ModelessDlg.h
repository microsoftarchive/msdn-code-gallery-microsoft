#pragma once


// CModelessDlg dialog

class CModelessDlg : public CDialog
{
	DECLARE_DYNAMIC(CModelessDlg)

public:
	CModelessDlg(CWnd* pParent = NULL);   // standard constructor
	virtual ~CModelessDlg();

// Dialog Data
	enum { IDD = IDD_MODELESSDIALOG };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()
};
