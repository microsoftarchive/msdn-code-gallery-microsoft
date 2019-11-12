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
//  And then, call media.SetSource(picked file).
//  If the file is successfully opened, VideoMediaOpened() will be called and call media.Play().
//
void SampleUtilities::PickSingleFileAndSet(IVector<String^>^ fileTypeFilters, IVector<MediaElement^>^ mediaElements)
{
    auto picker = ref new Pickers::FileOpenPicker();
    auto dispatcher = Window::Current->Dispatcher;
    picker->SuggestedStartLocation = Pickers::PickerLocationId::VideosLibrary;
    for (unsigned int index = 0; index < fileTypeFilters->Size; ++index)
    {
        picker->FileTypeFilter->Append(fileTypeFilters->GetAt(index));
    }

    create_task(picker->PickSingleFileAsync()).then(
        [mediaElements](StorageFile^ file)
    {
        if (file)
        {
            auto contentType = file->ContentType;
            create_task(file->OpenAsync(FileAccessMode::Read)).then(
                [contentType, mediaElements](Streams::IRandomAccessStream^ strm)
            {
                {
                    for (unsigned int i = 0; i < mediaElements->Size; ++i)
                    {
                        MediaElement^ media = mediaElements->GetAt(i);
                        media->Stop();

                        if (i + 1 < mediaElements->Size)
                        {
                            media->SetSource(strm->CloneStream(), contentType);
                        }
                        else
                        {
                            media->SetSource(strm, contentType);
                        }
                    }
                }
            });
        }
    });
}


