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
#include "stdafx.h"
#include "SetupCmdLineInfo.h"

SetupCmdLineInfo::SetupCmdLineInfo()
{
          _bInstall = _bQuiet = _bPassive = FALSE;
}

void SetupCmdLineInfo::ParseParamFlag(const char* pszParam)
{
    CString param(pszParam);

    if(0 == strcmp(pszParam, "install"))
    {
        _bInstall = TRUE;
    } 
    else if(0 == strcmp(pszParam, "quiet"))
    {
        _bQuiet = TRUE;
    }
    else if(0 == strcmp(pszParam, "passive"))
    {
        _bPassive = TRUE;
    }
}

void SetupCmdLineInfo::ParseParam(const char* pszParam, BOOL bFlag, BOOL bLast)
{    
    CString param(pszParam);

    if(0 == strcmp(pszParam, "/install"))
    {
        _bInstall = TRUE;
    } 
    else if(0 == strcmp(pszParam, "/quiet"))
    {
        _bQuiet = TRUE;
    }
    else if(0 == strcmp(pszParam, "/passive"))
    {
        _bPassive = TRUE;
    }
}


