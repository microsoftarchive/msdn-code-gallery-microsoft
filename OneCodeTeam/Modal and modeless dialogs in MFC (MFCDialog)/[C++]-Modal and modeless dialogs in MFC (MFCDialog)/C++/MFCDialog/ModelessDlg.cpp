// ModelessDlg.cpp : implementation file
//

#include "stdafx.h"
#include "MFCDialog.h"
#include "ModelessDlg.h"


// CModelessDlg dialog

IMPLEMENT_DYNAMIC(CModelessDlg, CDialog)

CModelessDlg::CModelessDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CModelessDlg::IDD, pParent)
{

}

CModelessDlg::~CModelessDlg()
{
}

void CModelessDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}


BEGIN_MESSAGE_MAP(CModelessDlg, CDialog)
END_MESSAGE_MAP()


// CModelessDlg message handlers
