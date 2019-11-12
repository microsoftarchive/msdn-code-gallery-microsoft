// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "ViewModelLocator.h"
#include "DebugLoggingExceptionPolicy.h"

using namespace Hilo;
using namespace Platform::Collections;
using namespace std;
using namespace Windows::ApplicationModel::Resources;

ViewModelLocator::ViewModelLocator()
{
    m_exceptionPolicy = make_shared<DebugLoggingExceptionPolicy>();
    m_repository = safe_cast<Hilo::App^>(Windows::UI::Xaml::Application::Current)->GetRepository();
}

MainHubViewModel^ ViewModelLocator::MainHubVM::get()
{
    auto vector = ref new Vector<HubPhotoGroup^>();
    // Pictures Group
    auto loader = ref new ResourceLoader();
    auto title = loader->GetString("PicturesTitle");
    auto emptyTitle = loader->GetString("EmptyPicturesTitle");
    auto picturesGroup = ref new HubPhotoGroup(title, emptyTitle, m_repository, m_exceptionPolicy);
    vector->Append(picturesGroup);
    return ref new MainHubViewModel(vector, m_exceptionPolicy);
}

ImageBrowserViewModel^ ViewModelLocator::ImageBrowserVM::get()
{
    if (nullptr == m_imageBrowswerViewModel)
    {
        m_imageBrowswerViewModel = ref new ImageBrowserViewModel(m_repository, m_exceptionPolicy);
    }

    return m_imageBrowswerViewModel;
}

ImageViewModel^ ViewModelLocator::ImageVM::get()
{
    return ref new ImageViewModel(m_repository, m_exceptionPolicy);
}

CropImageViewModel^ ViewModelLocator::CropImageVM::get()
{
    return ref new CropImageViewModel(m_repository, m_exceptionPolicy);
}

RotateImageViewModel^ ViewModelLocator::RotateImageVM::get()
{
    return ref new RotateImageViewModel(m_repository, m_exceptionPolicy);
}

CartoonizeImageViewModel^ ViewModelLocator::CartoonizeImageVM::get()
{
    return ref new CartoonizeImageViewModel(m_repository, m_exceptionPolicy);
}