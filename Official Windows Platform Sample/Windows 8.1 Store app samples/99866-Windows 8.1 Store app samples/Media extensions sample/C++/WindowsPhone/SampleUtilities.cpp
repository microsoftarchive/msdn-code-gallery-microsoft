//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "SampleUtilities.h"

using namespace SDKSample::MediaExtensions;

using namespace Platform;

using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

using namespace concurrency;

//
//  Open a single file picker [with fileTypeFilter].
//  On the phone we just set up media elements with preconfigured urls.
//
void SampleUtilities::PickSingleFileAndSet(IVector<String^>^ fileTypeFilters, IVector<MediaElement^>^ mediaElements)
{
    String ^firstFilter = fileTypeFilters->GetAt(0);
    Uri ^uri;
    if (wcscmp(firstFilter->Data(), L".mpg") == 0)
    {
        uri = ref new Uri("ms-appx:///", "video.mpg");
    }
    else if (wcscmp(firstFilter->Data(), L".mp4") == 0)
    {
        uri = ref new Uri("http://ie.microsoft.com/testdrive/Graphics/VideoFormatSupport/big_buck_bunny_trailer_480p_high.mp4");
    }
    else
    {
        throw ref new InvalidArgumentException();
    }

    for (unsigned int i = 0; i < mediaElements->Size; ++i)
    {
        MediaElement^ media = mediaElements->GetAt(i);
        media->Stop();
        media->Source = uri;
    }
}

