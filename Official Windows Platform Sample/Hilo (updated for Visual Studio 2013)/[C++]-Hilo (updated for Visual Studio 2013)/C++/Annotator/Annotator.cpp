//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

// Annotator.cpp : Defines the entry point for the application

#include "stdafx.h"
#include "AnnotatorApplication.h"

// The window application
INT APIENTRY _tWinMain(
    HINSTANCE /*hInstance*/,
    HINSTANCE /*hPrevInstance*/,
    LPTSTR    /*lpCmdLine*/,
    INT       /*nCmdShow*/)
{
    ComPtr<IWindowApplication> mainApp;

    // Create and initialize the main application object
    HRESULT hr = SharedObject<AnnotatorApplication>::Create(&mainApp);

    if (SUCCEEDED(hr))
    {
        // Start the message loop for the application
        hr = mainApp->RunMessageLoop();
    }

    return 0;
}