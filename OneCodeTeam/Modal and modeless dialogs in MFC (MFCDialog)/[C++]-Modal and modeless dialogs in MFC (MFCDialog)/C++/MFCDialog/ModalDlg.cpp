// ModalDlg.cpp : implementation file
//

#include "stdafx.h"
#include "MFCDialog.h"
#include "ModalDlg.h"


// CModalDlg dialog

IMPLEMENT_DYNAMIC(CModalDlg, CDialog)

CModalDlg::CModalDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CModalDlg::IDD, pParent)
{

}

CModalDlg::~CModalDlg()
{
}

void CModalDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}


BEGIN_MESSAGE_MAP(CModalDlg, CDialog)
END_MESSAGE_MAP()


// CModalDlg message handlers
