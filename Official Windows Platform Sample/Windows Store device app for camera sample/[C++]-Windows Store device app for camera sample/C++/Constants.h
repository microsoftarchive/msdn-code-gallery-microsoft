//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#pragma once

#include <collection.h>

namespace DeviceAppForWebcam
{
    partial ref class MainPage
    {
    public:
        static property Platform::String^ FEATURE_NAME 
        {
            Platform::String^ get() 
            {  
                return ref new Platform::String(L"Windows Store device app for webcam sample"); 
            }
        }
    };


}
