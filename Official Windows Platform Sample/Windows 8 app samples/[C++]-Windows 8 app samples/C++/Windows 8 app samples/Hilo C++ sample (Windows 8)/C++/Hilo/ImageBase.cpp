// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "ImageBase.h"
#include "IPhoto.h"
#include "HiloCommonDefinitions.h"

using namespace concurrency;
using namespace Hilo;
using namespace Platform;
using namespace Platform::Collections;
using namespace std;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace Windows::Storage::Streams;

ImageBase::ImageBase(shared_ptr<ExceptionPolicy> exceptionPolicy) : ViewModelBase(exceptionPolicy)
{
}

concurrency::task<IRandomAccessStreamWithContentType^> ImageBase::GetStreamWithFailCheck()
{
    return create_task(m_photo->OpenReadAsync())
        .then([this](task<IRandomAccessStreamWithContentType^> openTask)
    {
        assert(IsMainThread());
        try
        {
            auto stream = openTask.get();
            return stream;
        }
        catch(Platform::Exception^ ex)
        {
            switch(ex->HResult)
            {
            case HILO_PHOTO_FILE_NOT_FOUND:
                // Image has been removed, take the user home.
                GoHome();
                cancel_current_task();
                break;

            default:
                throw;
            }
        }
    }, task_continuation_context::use_current());
}


task<StorageFile^> ImageBase::GetFileNameFromFileSavePickerAsync(String^ fileType)
{
    assert(IsMainThread());
    auto fileExtension = ref new Vector<String^>();
    fileExtension->Append(fileType);

    auto savePicker = ref new FileSavePicker();
    savePicker->SuggestedStartLocation = PickerLocationId::PicturesLibrary;    
    savePicker->FileTypeChoices->Insert("Image", fileExtension);
    savePicker->DefaultFileExtension = fileType;

    auto filePickerTask = create_task(savePicker->PickSaveFileAsync());
    return filePickerTask.then([](StorageFile^ file)
    {
        assert(IsMainThread());
        if (file == nullptr)
        {
            cancel_current_task();
        }
        return file;
    });
}

task<void> ImageBase::SaveImageAsync(StorageFile^ file, IRandomAccessStream^ ras)
{
    assert(IsMainThread());
    std::shared_ptr<IBuffer^> buffer = std::make_shared<IBuffer^>(nullptr);

    IInputStream^ inputStream = ras->GetInputStreamAt(0);
    unsigned int size = static_cast<unsigned int>(ras->Size);
    auto streamReader = ref new DataReader(inputStream);

    auto loadStreamTask = create_task(streamReader->LoadAsync(size));
    return loadStreamTask.then([streamReader, buffer, size, file](unsigned int loadedBytes)
    {
        assert(IsBackgroundThread());
        (void)loadedBytes; // Unused parameter
        *buffer = streamReader->ReadBuffer(size);
        return FileIO::WriteBufferAsync(file, *buffer);
    }, concurrency::task_continuation_context::use_arbitrary()).then([this]() 
    {
        assert(IsMainThread());
        ViewModelBase::GoBack();
    });
}
