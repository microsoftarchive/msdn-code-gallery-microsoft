/****************************** Module Header ******************************\
* Module Name:  ATLExeCOMServer.cpp
* Project:      ATLExeCOMServer
* Copyright (c) Microsoft Corporation.
* 
* Acitve Template Library (ATL) is designed to simplify the process of 
* creating efficient, flexible, lightweight COM components. ATLExeCOMServer 
* provides the our-of-process server objects that are implemented in an EXE 
* and are run in a separate process space.
* 
* ATLExeCOMServer.cpp implements WinMain and defines the server module.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma region Includes
#include "stdafx.h"
#include "resource.h"
#include "ATLExeCOMServer_i.h"
#include "dlldatax.h"
#pragma endregion


class CATLExeCOMServerModule : public CAtlExeModuleT< CATLExeCOMServerModule >
{
public :
	DECLARE_LIBID(LIBID_ATLExeCOMServerLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_ATLEXECOMSERVER, "{B711EE75-FDA3-4B0E-BFAA-67CB305D62AE}")
};

CATLExeCOMServerModule _AtlModule;



//
extern "C" int WINAPI _tWinMain(HINSTANCE /*hInstance*/, HINSTANCE /*hPrevInstance*/, 
                                LPTSTR /*lpCmdLine*/, int nShowCmd)
{
    return _AtlModule.WinMain(nShowCmd);
}

