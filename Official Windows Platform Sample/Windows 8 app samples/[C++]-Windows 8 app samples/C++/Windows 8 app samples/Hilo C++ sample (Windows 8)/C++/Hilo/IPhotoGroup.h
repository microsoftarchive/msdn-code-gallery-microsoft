// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

namespace Hilo
{
    interface class IPhoto;

    // The IPhotoGroup class defines the interface of the app's photo containers
    // (the HubPhotoGroup, MonthGroup, NullPhotoGroup classes). XAML controls
    // bind to objects of this type.
    [Windows::Foundation::Metadata::WebHostHidden]
    public interface class IPhotoGroup
    {
        property Platform::String^ Title 
        { 
            Platform::String^ get();
        }

        property Windows::Foundation::Collections::IObservableVector<IPhoto^>^ Items
        {
            Windows::Foundation::Collections::IObservableVector<IPhoto^>^ get();
        }
    };
}
