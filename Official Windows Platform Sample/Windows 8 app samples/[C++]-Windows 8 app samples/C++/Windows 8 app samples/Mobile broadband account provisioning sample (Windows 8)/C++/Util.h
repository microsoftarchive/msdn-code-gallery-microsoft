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
// Declaration of the Util class
//

#pragma once

#include "pch.h"
#include "MainPage.xaml.h"

using namespace Platform;

namespace SDKSample
{
    namespace ProvisioningAgent
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class Util sealed
        {
        public:
            Util();
    
            String^ ParseErrorCode(String^ errorCode);
            String^ ParseResultXML(String^ resultsXml);
            String^ PrintExceptionCode(Exception^ error);
        };
    }
}
