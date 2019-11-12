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
// UserPreferences.xaml.cpp
// Implementation of the UserPreferences class
//

#include "pch.h"
#include "Utilities.h"

using namespace SDKSample::GlobalizationPreferencesSample;

using namespace Platform;
using namespace Windows::Foundation;

/// <summary>
/// Converts vector view of strings into a single string.
/// </summary>
Platform::String^ Utilities::VectorViewToString(Windows::Foundation::Collections::IVectorView<Platform::String^>^ data)
{
    // Initial value is empty
    String^ value = "";

    // Iterate throgh the elements in the vector view and add them to the string
    for (unsigned int i = 0; i < data->Size; i++)
        value = value + data->GetAt(i) + " ";

    // Return the string
    return value;
}
 
