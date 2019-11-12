// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

namespace Hilo
{
    interface class IYearGroup;

    // The IMonthBlock class defines the signature of per-month data used by the 
    // image browser's zoomed-out view of the user's picture library. XAML controls
    // bind to objects of this type.
    public interface class IMonthBlock
    {
        property Platform::String^ Name 
        { 
            Platform::String^ get(); 
        }

        property bool HasPhotos
        { 
            bool get();
        }

        property unsigned int Month
        {
            unsigned int get();
        }

        property IYearGroup^ Group
        {
            IYearGroup^ get();
        }
    };
}