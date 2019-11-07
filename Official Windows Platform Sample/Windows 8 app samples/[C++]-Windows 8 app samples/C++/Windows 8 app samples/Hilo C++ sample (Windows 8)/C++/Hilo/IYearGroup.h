// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

namespace Hilo
{
    interface class IMonthBlock;

    // The IYearGroup class defines the signature of data used by the image 
    // browser's zoomed-out view of the user's picture library. XAML controls 
    // bind to objects of this type.
    public interface class IYearGroup
    {
        property Platform::String^ Title
        { 
            Platform::String^ get();
        }

        property int Year
        {
            int get();
        }

        property Windows::Foundation::Collections::IObservableVector<IMonthBlock^>^ Items
        {
            Windows::Foundation::Collections::IObservableVector<IMonthBlock^>^ get();
        }
    };
}