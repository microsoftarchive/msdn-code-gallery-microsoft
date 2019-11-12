// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// ThumbnailConverter.cpp
// Implementation of the ThumbnailConverter class
//

#include "pch.h"
#include "ThumbnailConverter.h"

using namespace SDKSample::DataSourceAdapter;

using namespace Platform;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media::Imaging;

ThumbnailConverter::ThumbnailConverter()
{
}

ThumbnailConverter::~ThumbnailConverter()
{
}

Object^ ThumbnailConverter::Convert(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
    if (value != nullptr)
    {
        IRandomAccessStream^ thumbnailStream = dynamic_cast<IRandomAccessStream^>(value);
        BitmapImage^ bitmapImage = ref new BitmapImage();
        bitmapImage->SetSource(thumbnailStream);
        return bitmapImage;
    }
    else
    {
        return DependencyProperty::UnsetValue;
    }
}

Object^ ThumbnailConverter::ConvertBack(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
    return nullptr;
}
