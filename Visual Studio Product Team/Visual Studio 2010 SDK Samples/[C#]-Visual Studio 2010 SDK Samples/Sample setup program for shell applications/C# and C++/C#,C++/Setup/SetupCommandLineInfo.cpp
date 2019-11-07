//******************************************************************
//
// Copyright (c) Microsoft Corporation. All rights reserved.
// This code is licensed under the Visual Studio SDK license terms.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//******************************************************************
#include "StdAfx.h"
#include "SetupCommandLineInfo.h"


SetupCommandLineInfo::SetupCommandLineInfo(void)
{
    _bInstall = _bRestart = _bPassive = FALSE;
}


SetupCommandLineInfo::~SetupCommandLineInfo(void)
{

}

void SetupCommandLineInfo::Parse(CString param)
{
    if(0 == param.CompareNoCase(_T("install")))
    {
        _bInstall = TRUE;
    } 
    else if(0 == param.CompareNoCase(_T("restart")))
    {
        _bRestart = TRUE;
    }
    else if(0 == param.CompareNoCase(_T("passive")))
    {
        _bPassive = TRUE;
    }
}

void SetupCommandLineInfo::ParseParam(const TCHAR* pszParam, BOOL bFlag, BOOL bLast)
{
    Parse(pszParam);
}

void SetupCommandLineInfo::ParseParamFlag(const char* pszParam)
{
    CString param(pszParam);
    Parse(param);
}

void SetupCommandLineInfo::ParseParam(const char* pszParam, BOOL bFlag, BOOL bLast)
{    
    CString param(pszParam);
    Parse(param);
}


