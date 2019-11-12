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
// Utilities.cpp
// Declaration of the Utilities class
//

#pragma once

namespace SDKSample
{
    namespace GlobalizationPreferencesSample
    {
        /// <summary>
        /// Contains common code used by several scenarios of the sample.
        /// </summary>
         class Utilities sealed
        {
            public:
                static Platform::String^ VectorViewToString(Windows::Foundation::Collections::IVectorView<Platform::String^>^ data);
        };
    };
    }
