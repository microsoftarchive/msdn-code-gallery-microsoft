//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace WRLOutOfProcessWinRTComponent;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    { "Using Custom Components", "WRLOutOfProcessWinRTComponent.OvenClient" },
    { "Using Custom Components (Implemented using WRL)", "WRLOutOfProcessWinRTComponent.OvenClientWRL" },
    { "Handling Windows Runtime Exceptions", "WRLOutOfProcessWinRTComponent.CustomException" },
    { "Handling Windows Runtime Exceptions (Implemented using WRL)", "WRLOutOfProcessWinRTComponent.CustomExceptionWRL" }
}; 
