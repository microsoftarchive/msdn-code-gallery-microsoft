// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "RotateImageViewModel.h"
#include "DelegateCommand.h"
#include "IPhoto.h"
#include "ImageNavigationData.h"
#include "TaskExceptionsExtensions.h"
#include "Repository.h"
#include "ExifExtensions.h"

#define EXIFOrientationPropertyName "System.Photo.Orientation"

using namespace concurrency;
using namespace Hilo;
using namespace Platform;
using namespace Platform::Collections;
using namespace std;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Imaging;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Navigation;

Platform::String^ const RotationStateKey = "rotation";
Platform::String^ const MarginLeftStateKey = "margin-left";
Platform::String^ const MarginTopStateKey = "margin-top";
Platform::String^ const MarginRightStateKey = "margin-right";
Platform::String^ const MarginBottomStateKey = "margin-bottom";

RotateImageViewModel::RotateImageViewModel(shared_ptr<Repository> repository, shared_ptr<ExceptionPolicy> exceptionPolicy) : 
    ImageBase(exceptionPolicy), m_repository(repository), m_imageMargin(Thickness(0.0)), m_getPhotoAsyncIsRunning(false),
    m_inProgress(false), m_isSaving(false), m_rotationAngle(0.0)
{
    m_rotateCommand = ref new DelegateCommand(ref new ExecuteDelegate(this, &RotateImageViewModel::Rotate90), nullptr);
    m_resumeRotateCommand = ref new DelegateCommand(ref new ExecuteDelegate(this, &RotateImageViewModel::Unsnap), nullptr);
    m_saveCommand = ref new DelegateCommand(ref new ExecuteDelegate(this, &RotateImageViewModel::SaveImage), nullptr);
    m_cancelCommand = ref new DelegateCommand(ref new ExecuteDelegate(this, &RotateImageViewModel::CancelRotate), nullptr);

    ViewModelBase::m_isAppBarSticky = true;
}

ICommand^ RotateImageViewModel::RotateCommand::get()
{
    return m_rotateCommand;
}

ICommand^ RotateImageViewModel::ResumeRotateCommand::get()
{
    return m_resumeRotateCommand;
}

ICommand^ RotateImageViewModel::SaveCommand::get()
{
    return m_saveCommand;
}

ICommand^ RotateImageViewModel::CancelCommand::get()
{
    return m_cancelCommand;
}

Thickness RotateImageViewModel::ImageMargin::get()
{
    return m_imageMargin;
}

bool RotateImageViewModel::InProgress::get()
{
    return m_inProgress;
}

float64 RotateImageViewModel::RotationAngle::get()
{
    return m_rotationAngle;
}

void RotateImageViewModel::RotationAngle::set(float64 value)
{
    m_rotationAngle = value;

    // Derive margin so that rotated image is always fully shown on screen.
    Thickness margin(0.0);
    switch (safe_cast<unsigned int>(m_rotationAngle))
    {
    case 90:
    case 270:
        margin.Top = 110.0;
        margin.Bottom = 110.0;
        break;
    }
    m_imageMargin = margin;
    OnPropertyChanged("ImageMargin");
    OnPropertyChanged("RotationAngle");
}

IPhoto^ RotateImageViewModel::Photo::get()
{
    if (nullptr == m_photo && !m_getPhotoAsyncIsRunning)
    {
        m_getPhotoAsyncIsRunning = true;
        run_async_non_interactive([this]()
        {
            GetImagePhotoAsync().then([this] (task<IPhoto^> photoTask)
            {
                assert(IsMainThread());
                m_getPhotoAsyncIsRunning = false;
                auto photo = photoTask.get();
                if (photo != nullptr)
                {
                    m_photo = photo;
                    OnPropertyChanged("Photo");
                }
            }).then(ObserveException<void>(m_exceptionPolicy));
        });
    }

    return m_photo;
}

void RotateImageViewModel::Rotate90(Object^ parameter)
{   
    RotationAngle += 90;
    EndRotation();
}

concurrency::task<RotateImageViewModel::ImageEncodingInformation> RotateImageViewModel::GetDecoderInfo(
    IRandomAccessStream^ source, 
    concurrency::task_continuation_context backgroundContext)
{
    assert(IsMainThread());
    auto  decoderInfo = std::make_shared<RotateImageViewModel::ImageEncodingInformation>();
    (*decoderInfo).usesExifOrientation = false;

    return create_task(m_photo->OpenReadAsync()).then([](IRandomAccessStream^ stream)
    {
        assert(IsMainThread());
        return BitmapDecoder::CreateAsync(stream);
    }).then([decoderInfo](BitmapDecoder^ decoder)
    {        
        assert(IsBackgroundThread());
        (*decoderInfo).decoder = decoder;

        auto requestedProperty = ref new Vector<String^>();
        requestedProperty->Append(EXIFOrientationPropertyName);
        return (*decoderInfo).decoder->BitmapProperties->GetPropertiesAsync(requestedProperty);
    }, backgroundContext).then([decoderInfo](task<BitmapPropertySet^> propertiesTask)
    {
        assert(IsBackgroundThread());
        BitmapPropertySet^ properties;

        try {
            properties = propertiesTask.get();
            if (properties->HasKey(EXIFOrientationPropertyName))
            {
                (*decoderInfo).usesExifOrientation = true;
                (*decoderInfo).exifOrientation = safe_cast<unsigned short>(properties->Lookup(EXIFOrientationPropertyName)->Value);
            }
        }
        catch(Exception^ ex)
        {
            // If the file format doesn't support properties, continue without using Exif orientation.
            switch (ex->HResult)
            {
            case WINCODEC_ERR_UNSUPPORTEDOPERATION:
            case WINCODEC_ERR_PROPERTYNOTSUPPORTED:
            case E_INVALIDARG:
                (*decoderInfo).usesExifOrientation = false;
                break;

            default:
                throw;
            }
        }

        return (*decoderInfo);
    }, backgroundContext);
}

unsigned int RotateImageViewModel::CheckRotationAngle(unsigned int angle)
{
    return (angle > 360) ? angle - 360 : angle;
}

concurrency::task<BitmapEncoder^> RotateImageViewModel::SetEncodingRotation(BitmapEncoder^ encoder, shared_ptr<ImageEncodingInformation> encodingInfo, float64 rotationAngle, concurrency::task_continuation_context backgroundContext)
{
    // If the file format supports Exif orientation then update the orientation flag
    // to reflect any user-specified rotation. Otherwise, perform a hard rotate 
    // using the BitmapTransform class.
    auto encodingTask = create_empty_task();
    if (encodingInfo->usesExifOrientation)
    {
        // Try encoding with Exif with updated values.
        auto currentExifOrientationDegrees = ExifExtensions::ConvertExifOrientationToDegreesRotation(ExifRotations(encodingInfo->exifOrientation));
        auto newRotationAngleToApply = CheckRotationAngle(safe_cast<unsigned int>(rotationAngle + currentExifOrientationDegrees));
        auto exifOrientationToApply = ExifExtensions::ConvertDegreesRotationToExifOrientation(newRotationAngleToApply);
        auto orientedTypedValue = ref new BitmapTypedValue(static_cast<unsigned short>(exifOrientationToApply), PropertyType::UInt16);

        auto properties = ref new Map<String^, BitmapTypedValue^>();
        properties->Insert(EXIFOrientationPropertyName, orientedTypedValue);

        encodingTask = encodingTask.then([encoder, properties]
        { 
            assert(IsBackgroundThread());
            return encoder->BitmapProperties->SetPropertiesAsync(properties);
        }, backgroundContext).then([encoder, encodingInfo] (task<void> setPropertiesTask)
        {
            assert(IsBackgroundThread());

            try 
            {
                setPropertiesTask.get();
            }
            catch(Exception^ ex)
            {
                switch(ex->HResult)
                {
                case WINCODEC_ERR_UNSUPPORTEDOPERATION:
                case WINCODEC_ERR_PROPERTYNOTSUPPORTED:
                case E_INVALIDARG:
                    encodingInfo->usesExifOrientation = false;
                    break;
                default:
                    throw;
                }
            }

        }, backgroundContext);
    }

    return encodingTask.then([encoder, encodingInfo, rotationAngle]
    {
        assert(IsBackgroundThread());
        if (!encodingInfo->usesExifOrientation)
        {
            BitmapRotation rotation = static_cast<BitmapRotation>((int)floor(rotationAngle / 90));
            encoder->BitmapTransform->Rotation = rotation;
        }
        return encoder;
    });
}

concurrency::task<IRandomAccessStream^> RotateImageViewModel::EncodeRotateImageToStream(
    ImageEncodingInformation encodingInformation, 
    float64 rotationAngle, 
    concurrency::task_continuation_context backgroundContext )
{
    assert(IsMainThread());
    auto ras = ref new InMemoryRandomAccessStream();
    auto bitmapEncoder = make_shared<BitmapEncoder^>(nullptr);
    auto bitmapDecoderInfo = make_shared<ImageEncodingInformation>(encodingInformation);

    return create_task(BitmapEncoder::CreateForTranscodingAsync(ras, (*bitmapDecoderInfo).decoder))
        .then([this, rotationAngle, backgroundContext, bitmapDecoderInfo](BitmapEncoder^ encoder)
    {
        assert(IsBackgroundThread());
        return SetEncodingRotation(encoder, bitmapDecoderInfo, rotationAngle, backgroundContext);
    }, backgroundContext).then([bitmapEncoder](BitmapEncoder^ encoder)
    {        
        assert(IsBackgroundThread());
        // Force a new thumbnail to reflect any rotation operation
        encoder->IsThumbnailGenerated = true;
        (*bitmapEncoder) = encoder;
        return encoder->FlushAsync();
    }, backgroundContext).then([bitmapEncoder](task<void> encodeTask)
    {
        assert(IsBackgroundThread());
        try
        {
            encodeTask.get();
        }
        catch(Exception^ ex)
        {
            switch (ex->HResult)
            {
            case WINCODEC_ERR_UNSUPPORTEDOPERATION:
                // If the encoder does not support writing a thumbnail, then
                // disable thumbnail generation before trying again.
                (*bitmapEncoder)->IsThumbnailGenerated = false;
                break;

            default:
                throw ex;
            }
        }
        return ((*bitmapEncoder)->IsThumbnailGenerated == false) ? (*bitmapEncoder)->FlushAsync() : create_async([]{});
    }, backgroundContext).then([ras]
    {
        assert(IsMainThread());
        return safe_cast<IRandomAccessStream^>(ras); 
    });
}

concurrency::task<IRandomAccessStream^> RotateImageViewModel::RotateImageAsync(IRandomAccessStream^ source, float64 angle)
{
    assert(IsMainThread());
    assert(angle < 360);
    while (angle < 0)
    {
        angle += 360;
    }

    auto backgroundContext = task_continuation_context::use_arbitrary();
    return GetDecoderInfo(source, backgroundContext)
        .then([this, angle, backgroundContext](ImageEncodingInformation encodingInformation)
    {
        assert(IsMainThread());
        return EncodeRotateImageToStream(encodingInformation, angle, backgroundContext);
    });
}

void RotateImageViewModel::ChangeInProgress(bool value)
{
    m_inProgress = value;
    OnPropertyChanged("InProgress");
}

void RotateImageViewModel::SaveImage(Object^ parameter)
{
    if (m_isSaving) return;

    m_isSaving = true;
    auto file = make_shared<StorageFile^>(nullptr);
    auto stream = make_shared<IRandomAccessStreamWithContentType^>(nullptr);
    auto rotatedStream = make_shared<IRandomAccessStream^>(nullptr);

    GetStreamWithFailCheck().then([this, stream](IRandomAccessStreamWithContentType^ sourceStream)
    {
        assert(IsMainThread());
        *stream = sourceStream;
        ChangeInProgress(true);
    }).then([this, stream]
    {
        assert(IsMainThread());
        return RotateImageAsync((*stream), RotationAngle);
    }).then([this, rotatedStream](IRandomAccessStream^ pictureStream)
    {
        (*rotatedStream) = pictureStream;
        // Cancel the task if the user has switched to snapped view before the FileSavePicker has appeared.
        if (ApplicationView::Value == ApplicationViewState::Snapped)
        {
            cancel_current_task();
        }
        return ImageBase::GetFileNameFromFileSavePickerAsync(m_photo->FileType);
    }).then([this, file, rotatedStream](StorageFile^ f)
    {
        assert(IsMainThread());
        (*file) = f;
        return ImageBase::SaveImageAsync((*file), (*rotatedStream));
    }).then([this](task<void> priorTask)
    {
        assert(IsMainThread());
        m_isSaving = false;
        ChangeInProgress(false);
        try
        {
            priorTask.get();
        }
        catch(const concurrency::task_canceled&)
        {
        }
    }).then(ObserveException<void>(m_exceptionPolicy));
}

void RotateImageViewModel::Unsnap(Object^ parameter)
{
    if (ApplicationView::Value == ApplicationViewState::Snapped)
    {
        ApplicationView::TryUnsnap();
    }
}

void RotateImageViewModel::CancelRotate(Object^ parameter)
{
    ViewModelBase::GoBack();
}

// See http://go.microsoft.com/fwlink/?LinkId=267280 for more info on Hilo's implementation of suspend/resume.
void RotateImageViewModel::LoadState(IMap<String^, Object^>^ stateMap) 
{
    if (stateMap != nullptr)
    {
        if (stateMap->HasKey(RotationStateKey))
            m_rotationAngle = static_cast<float64>(stateMap->Lookup(RotationStateKey));

        Thickness margin(0.0);
        if (stateMap->HasKey(MarginRightStateKey))
            margin.Left = static_cast<float64>(stateMap->Lookup(MarginRightStateKey));

        if (stateMap->HasKey(MarginTopStateKey))
            margin.Top = static_cast<float64>(stateMap->Lookup(MarginTopStateKey));

        if (stateMap->HasKey(MarginRightStateKey))
            margin.Right = static_cast<float64>(stateMap->Lookup(MarginRightStateKey));

        if (stateMap->HasKey(MarginBottomStateKey))
            margin.Bottom = static_cast<float64>(stateMap->Lookup(MarginBottomStateKey));

        m_imageMargin = margin;
    }
}

void RotateImageViewModel::SaveState(IMap<String^, Object^>^ stateMap) 
{
    stateMap->Insert(RotationStateKey, m_rotationAngle);
    stateMap->Insert(MarginLeftStateKey, m_imageMargin.Left);
    stateMap->Insert(MarginTopStateKey, m_imageMargin.Top);
    stateMap->Insert(MarginRightStateKey, m_imageMargin.Right);
    stateMap->Insert(MarginBottomStateKey, m_imageMargin.Bottom);
}

void RotateImageViewModel::OnNavigatedTo(NavigationEventArgs^ e)
{
    String^ data = dynamic_cast<String^>(e->Parameter); 
    ImageNavigationData navigationData(data);

    Initialize(navigationData.GetFilePath());
}


void RotateImageViewModel::Initialize(String^ photoPath)
{
    assert(IsMainThread());
    m_photo = nullptr;
    m_photoPath = photoPath;

    GetImagePhotoAsync().then([this](IPhoto^ photo)
    {
        assert(IsMainThread());
        // Return to the hub page if the photo is no longer present
        if (photo == nullptr)
        {
            GoHome();
        }
    });
}

concurrency::task<IPhoto^> RotateImageViewModel::GetImagePhotoAsync()
{
    assert(IsMainThread());
    return m_repository->GetSinglePhotoAsync(m_photoPath);
}

void RotateImageViewModel::EndRotation()
{
    auto quarterTurns = (RotationAngle / 90);
    auto nearestQuarter = (int)floor(quarterTurns + 0.5) % 4;
    RotationAngle = (float64)nearestQuarter * 90;
}


