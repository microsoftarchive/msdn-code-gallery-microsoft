//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// pch.h
// Header for standard system include files.
//

#pragma once

#include <Windows.h>
#include <wrl.h>
#include <wrl\implements.h>
#include <Shcore.h>
#include <MsXml6.h>
#include <collection.h>
#include <ppltasks.h>
#include <sstream>

#define CHKHR(stmt)  do { hr = (stmt); if (FAILED(hr)) goto Exit; } while(0)  

#include "Common\LayoutAwarePage.h"
#include "Common\SuspensionManager.h"
#include "App.xaml.h"
