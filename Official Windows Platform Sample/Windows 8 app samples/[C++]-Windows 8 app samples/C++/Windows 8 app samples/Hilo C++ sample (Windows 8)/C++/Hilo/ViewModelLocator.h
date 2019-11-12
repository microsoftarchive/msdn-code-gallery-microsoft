// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "MainHubViewModel.h" // Required by generated header
#include "ImageBrowserViewModel.h" // Required by generated header
#include "ImageViewModel.h" // Required by generated header
#include "CropImageViewModel.h" // Required by generated header
#include "RotateImageViewModel.h" // Required by generated header
#include "CartoonizeImageViewModel.h" // Required by generated header
#include "HubPhotoGroup.h" // Required by generated header
#include "Photo.h" // Required by generated header
#include "MonthGroup.h" // Required by generated header
#include "YearGroup.h" // Required by generated header
#include "MonthBlock.h" // Required by generated header

namespace Hilo
{
    class ExceptionPolicy;
    class Repository;

    // See http://go.microsoft.com/fwlink/?LinkId=267276 for info on how the ViewModelLocator 
    // connects views (pages) with their corresponding view models (presentation logic).

    // The ViewModelLocator class gives pages access to the app's presentation logic
    // following the Model-View-ViewModel (MVVM) pattern.
    [Windows::UI::Xaml::Data::Bindable]
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class ViewModelLocator sealed
    {
    public:
        ViewModelLocator();

        property ImageBrowserViewModel^ ImageBrowserVM { ImageBrowserViewModel^ get(); }
        property MainHubViewModel^ MainHubVM { MainHubViewModel^ get(); }
        property ImageViewModel^ ImageVM { ImageViewModel^ get(); }
        property CropImageViewModel^ CropImageVM { CropImageViewModel^ get(); }
        property RotateImageViewModel^ RotateImageVM { RotateImageViewModel^ get(); }
        property CartoonizeImageViewModel^ CartoonizeImageVM { CartoonizeImageViewModel^ get(); }

    private:
        ImageBrowserViewModel^ m_imageBrowswerViewModel;
        std::shared_ptr<ExceptionPolicy> m_exceptionPolicy;
        std::shared_ptr<Repository> m_repository;
    };
}