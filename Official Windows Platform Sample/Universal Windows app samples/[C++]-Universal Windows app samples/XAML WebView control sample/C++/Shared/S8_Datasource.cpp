//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#pragma once

#include "pch.h"
#include "S8_Datasource.h"

using namespace Windows::UI::Xaml::Media;
using namespace SDKSample::WebViewControl;

BookmarkItem::BookmarkItem(Platform::String^ title, Windows::UI::Xaml::Media::Imaging::BitmapSource^ preview, Windows::Foundation::Uri^ url)
{
	_title = title;
	_preview=preview;
	_pageUrl=url;
}
