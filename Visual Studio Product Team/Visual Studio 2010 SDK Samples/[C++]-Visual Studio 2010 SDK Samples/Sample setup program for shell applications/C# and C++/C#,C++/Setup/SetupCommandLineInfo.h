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
#pragma once
#include "afxwin.h"
class SetupCommandLineInfo : public CCommandLineInfo
{
public:
    SetupCommandLineInfo(void);
    ~SetupCommandLineInfo(void);

private:
      BOOL _bInstall;    //for /install
      BOOL _bRestart;    //for /restart
      BOOL _bPassive;    //for /passive

    void Parse(CString param);
 
public:
    BOOL IsInstall()     { return _bInstall; };
      BOOL IsRestart()   { return _bRestart; };
      BOOL IsPassive()   { return _bPassive; };
   
protected:
    virtual void ParseParam(const char* pszParam, BOOL bFlag, BOOL bLast);
    virtual void ParseParam(const TCHAR* pszParam, BOOL bFlag, BOOL bLast);
    virtual void ParseParamFlag(const char* pszParam);
};

