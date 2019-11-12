//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once

namespace SDKSample { namespace MediaExtensions {
	ref class SampleUtilities sealed
	{
	public:
		static void PickSingleFileAndSet(Windows::Foundation::Collections::IVector<Platform::String^>^ fileTypeFilters, Windows::Foundation::Collections::IVector<Windows::UI::Xaml::Controls::MediaElement^>^ mediaElements);
	};
} }