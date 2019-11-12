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

class SetupCmdLineInfo : public CCommandLineInfo
{
private:
    BOOL _bInstall;  //for /install
    BOOL _bQuiet;    //for /quiet
    BOOL _bPassive;  //for /passive
 
public:
    SetupCmdLineInfo();
    BOOL IsInstall()     { return _bInstall; };
    BOOL IsQuiet()       { return _bQuiet; };
    BOOL IsPassive()     { return _bPassive; };
   
protected:
    virtual void ParseParam(const char* pszParam, BOOL bFlag, BOOL bLast);
    virtual void ParseParamFlag(const char* pszParam);
};
