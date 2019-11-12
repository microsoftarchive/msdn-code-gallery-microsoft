// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"

#include "CropImageViewModel.h"
#include "DelegateCommand.h"
#include "TaskExceptionsExtensions.h"
#include "IPhoto.h"
#include "ImageNavigationData.h"
#include "NullPhotoGroup.h"
#include "Repository.h"
#include "ExifExtensions.h"
#include "ImageUtilities.h"

#define MINIMUMBMPSIZE 10
#define EXIFOrientationPropertyName "System.Photo.Orientation"

using namespace concurrency;
using namespace Hilo;
using namespace Platform;
using namespace Platform::Collections;
using namespace std;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Imaging;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;

CropImageViewModel::CropImageViewModel(shared_ptr<Repository> repository, shared_ptr<ExceptionPolicy> exceptionPolicy) : ImageBase(exceptionPolicy), m_repository(repository),
    m_inProgress(false), m_isCropOverlayVisible(false), m_isSaving(false), m_cropX(0u), m_cropY(0u)
{
    m_saveCommand = ref new DelegateCommand(ref new ExecuteDelegate(this, &CropImageViewModel::SaveImage), nullptr);
    m_cancelCommand = ref new DelegateCommand(ref new ExecuteDelegate(this, &CropImageViewModel::CancelCrop), nullptr);
    m_resumeCropCommand = ref new DelegateCommand(ref new ExecuteDelegate(this, &CropImageViewModel::Unsnap), nullptr);

    ViewModelBase::m_isAppBarSticky = true;
}

ICommand^ CropImageViewModel::ResumeCropCommand::get()
{
    return m_resumeCropCommand;
}

ICommand^ CropImageViewModel::SaveCommand::get()
{
    return m_saveCommand;
}

ICommand^ CropImageViewModel::CancelCommand::get()
{
    return m_cancelCommand;
}

bool CropImageViewModel::InProgress::get()
{
    return m_inProgress;
}

float64 CropImageViewModel::CropOverlayLeft::get()
{
    return m_cropOverlayLeft;
}

float64 CropImageViewModel::CropOverlayTop::get()
{
    return m_cropOverlayTop;
}

float64 CropImageViewModel::CropOverlayHeight::get()
{
    return m_cropOverlayHeight;
}

float64 CropImageViewModel::CropOverlayWidth::get()
{
    return m_cropOverlayWidth;
}

bool CropImageViewModel::IsCropOverlayVisible::get()
{
    return m_isCropOverlayVisible;
}

ImageSource^ CropImageViewModel::Image::get()
{
    return m_image;
}

void CropImageViewModel::OnNavigatedTo(NavigationEventArgs^ e)
{
    String^ data = dynamic_cast<String^>(e->Parameter); 
    ImageNavigationData navigationData(data);

    Initialize(navigationData.GetFilePath());
}

void CropImageViewModel::Initialize(String^ photoPath)
{
    assert(IsMainThread());
    m_photo = nullptr;
    m_image = nullptr;
    m_photoPath = photoPath;
    auto photoStream = make_shared<IRandomAccessStreamWithContentType^>(nullptr);

    GetImagePhotoAsync().then([this](IPhoto^ photo)
    {
        assert(IsMainThread());
        // Return to the hub page if the photo is no longer present
        if (photo == nullptr)
        {
            GoHome();
            cancel_current_task();
        }
        m_photo = photo;
        return m_photo->OpenReadAsync();

    }).then([this, photoStream](IRandomAccessStreamWithContentType^ imageData)
    {
        assert(IsMainThread());
        (*photoStream) = imageData;
        return BitmapDecoder::CreateAsync(imageData);

    }).then([this, photoStream](BitmapDecoder^ decoder)
    {
        assert(IsMainThread());
        m_image = ref new WriteableBitmap(decoder->PixelWidth, decoder->PixelHeight);
        (*photoStream)->Seek(0);
        return m_image->SetSourceAsync(*photoStream);

    }).then([this]()
    {
        assert(IsMainThread());
        OnPropertyChanged("Image");

    }).then(ObserveException<void>(m_exceptionPolicy));
}

task<IPhoto^> CropImageViewModel::GetImagePhotoAsync()
{
    assert(IsMainThread());
    return m_repository->GetSinglePhotoAsync(m_photoPath);
}

task<IRandomAccessStream^> CropImageViewModel::EncodeImageAsync(IRandomAccessStream^ sourceStream)
{
    assert(IsMainThread());
    auto backgroundContext = task_continuation_context::use_arbitrary();
    auto ras = ref new InMemoryRandomAccessStream();
    auto bitmapEncoder = make_shared<BitmapEncoder^>(nullptr);
    auto bitmapDecoder = make_shared<BitmapDecoder^>(nullptr);
    auto cropRect = make_shared<Rect>(
        static_cast<float>(m_cropX), 
        static_cast<float>(m_cropY),
        static_cast<float>(m_image->PixelWidth),
        static_cast<float>(m_image->PixelHeight));

    return create_task(BitmapDecoder::CreateAsync(sourceStream))
        .then([bitmapDecoder](BitmapDecoder^ decoder)
    {
        // Fetch the System.Photo.Orientation property to find out if the image is rotated.
        assert(IsBackgroundThread());
        auto requestedProperty = ref new Vector<String^>();
        requestedProperty->Append(EXIFOrientationPropertyName);
        (*bitmapDecoder) = decoder;
        return decoder->BitmapProperties->GetPropertiesAsync(requestedProperty);
    }, backgroundContext).then([cropRect, bitmapDecoder](task<BitmapPropertySet^> propertiesTask)
    {
        // Determine if the image is rotated. This may require handling exceptions.
        assert(IsBackgroundThread());
        BitmapPropertySet^ properties;
        ExifRotations exifOrientation = ExifRotations::NotRotated;
        try {
            properties = propertiesTask.get();
            if (properties->HasKey(EXIFOrientationPropertyName))
            {
                exifOrientation = ExifRotations(safe_cast<unsigned short>(properties->Lookup(EXIFOrientationPropertyName)->Value));
            }
        }
        catch(Exception^ ex)
        {
            // If the file format doesn't support properties, continue without
            // using Exif orientation.
            switch (ex->HResult)
            {
            case WINCODEC_ERR_UNSUPPORTEDOPERATION:
            case WINCODEC_ERR_PROPERTYNOTSUPPORTED:
            case E_INVALIDARG:
                exifOrientation = ExifRotations::NotRotated;
                break;

            default:
                throw;
            }
        }

        // If rotated flag has been set, rotate the crop region to match the image's rotation.
        Rect originalDimensions(0.f, 0.f, static_cast<float>((*bitmapDecoder)->PixelWidth), static_cast<float>((*bitmapDecoder)->PixelHeight));
        Rect unrotatedDimensions = originalDimensions;
        if (exifOrientation == ExifRotations::RotatedLeft || exifOrientation == ExifRotations::RotatedRight)
        {
            unrotatedDimensions.Width = static_cast<float>((*bitmapDecoder)->PixelHeight);
            unrotatedDimensions.Height = static_cast<float>((*bitmapDecoder)->PixelWidth);
        }

        int exifAppliedRotation = static_cast<int>(ExifExtensions::ConvertExifOrientationToDegreesRotation(exifOrientation));

        if (exifAppliedRotation != 0)
        {
            assert(exifAppliedRotation > 0);

            // Adjust crop back to original.
            auto adjustedCrop = ExifExtensions::RotateClockwise((*cropRect), unrotatedDimensions, -exifAppliedRotation);
            (*cropRect) = adjustedCrop;
        }
    }, backgroundContext).then([ras, bitmapDecoder]
    {
        // Create the transcoder that will save the image to disk.
        return BitmapEncoder::CreateForTranscodingAsync(ras, (*bitmapDecoder));
    }, backgroundContext).then([this, cropRect, bitmapEncoder](BitmapEncoder^ encoder)
    {
        assert(IsBackgroundThread());
        BitmapBounds bounds;
        bounds.X = static_cast<unsigned int>((*cropRect).X);
        bounds.Y = static_cast<unsigned int>((*cropRect).Y);
        bounds.Width = static_cast<unsigned int>((*cropRect).Width);
        bounds.Height = static_cast<unsigned int>((*cropRect).Height);
        encoder->BitmapTransform->Bounds = bounds;

        // Force a new thumbnail to reflect any rotation operation.
        encoder->IsThumbnailGenerated = true;
        (*bitmapEncoder) = encoder;
        // Start the encoding.
        return encoder->FlushAsync();
    }, backgroundContext).then([this, ras, bitmapEncoder](task<void> encodeTask)
    {
        // When the encoding has finished, check for any errors in creating the thumbnail.
        assert(IsBackgroundThread());
        try
        {
            encodeTask.get();
        }
        catch (Exception^ ex)
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
        // Return the result.
        assert(IsBackgroundThread());
        return dynamic_cast<IRandomAccessStream^>(ras);
    }, backgroundContext);
}

void CropImageViewModel::ChangeInProgress(bool value)
{
    m_inProgress = value;
    OnPropertyChanged("InProgress");
}

void CropImageViewModel::SaveImage(Object^ parameter)
{
    if (m_isSaving) return;

    m_isSaving = true;
    auto sourceStream = make_shared<IRandomAccessStreamWithContentType^>(nullptr);
    auto encodedStream = make_shared<IRandomAccessStream^>(nullptr);

    GetStreamWithFailCheck().then([this, sourceStream](IRandomAccessStreamWithContentType^ stream)
    {
        assert(IsMainThread());
        (*sourceStream) = stream;
        ChangeInProgress(true);
        return EncodeImageAsync(stream);
    }).then([this, encodedStream](IRandomAccessStream^ stream)
    {
        (*encodedStream) = stream;
        assert(IsMainThread());
        // Cancel the task if the user has switched to snapped view before the FileSavePicker has appeared.
        if (ApplicationView::Value == ApplicationViewState::Snapped)
        {
            cancel_current_task();
        }
        return ImageBase::GetFileNameFromFileSavePickerAsync(m_photo->FileType);
    }).then([this, encodedStream](StorageFile^ f)
    {
        assert(IsMainThread());
        return ImageBase::SaveImageAsync(f, (*encodedStream));
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

void CropImageViewModel::CancelCrop(Object^ parameter)
{
    ViewModelBase::GoBack();
}

void CropImageViewModel::Unsnap(Object^ parameter)
{
    if (ApplicationView::Value == ApplicationViewState::Snapped)
    {
        ApplicationView::TryUnsnap();
    }
}

void CropImageViewModel::CalculateInitialCropOverlayPosition(GeneralTransform^ transform, float width, float height)
{
    // Find the onscreen coordinates of the top left corner of the photo.
    Point topLeft;
    topLeft.X = 0;
    topLeft.Y = 0;
    if (transform != nullptr)
    {
        topLeft = transform->TransformPoint(topLeft);
    }

    m_left = topLeft.X;
    m_top = topLeft.Y;
    m_bottom = m_top + height;
    m_right = m_left + width;
    m_cropOverlayTop = m_top;
    m_cropOverlayLeft = m_left;
    m_cropOverlayHeight = height;
    m_cropOverlayWidth = width;
    m_actualHeight = height;
    m_actualWidth = width;
    m_isCropOverlayVisible = true;

    OnPropertyChanged("CropOverlayLeft");
    OnPropertyChanged("CropOverlayTop");
    OnPropertyChanged("CropOverlayHeight");
    OnPropertyChanged("CropOverlayWidth");
    OnPropertyChanged("IsCropOverlayVisible");
}

void CropImageViewModel::UpdateCropOverlayPostion(Thumb ^thumb, float64 verticalChange, float64 horizontalChange, float64 minWidth, float64 minHeight)
{
    if (thumb != nullptr)
    {
        float64 deltaH;
        float64 deltaV;
        float64 left;
        float64 top;

        switch (thumb->VerticalAlignment)
        {
        case VerticalAlignment::Bottom:
            deltaV = min(-verticalChange, m_cropOverlayHeight - minHeight);
            m_cropOverlayHeight -= deltaV;
            if (m_cropOverlayHeight + m_cropOverlayTop > m_bottom)
            {
                m_cropOverlayHeight = m_bottom - m_cropOverlayTop;
            }
            OnPropertyChanged("CropOverlayHeight");
            break;
        case VerticalAlignment::Top:
            deltaV = min(verticalChange, m_cropOverlayHeight - minHeight);
            top = m_cropOverlayTop + deltaV;
            if (m_top < top)
            {
                m_cropOverlayTop = top;
                m_cropOverlayHeight -= deltaV;
                if (m_cropOverlayHeight > m_actualHeight)
                {
                    m_cropOverlayHeight = m_actualHeight;
                }
                OnPropertyChanged("CropOverlayTop");
                OnPropertyChanged("CropOverlayHeight");
            }
            break;
        default:
            break;
        }

        switch (thumb->HorizontalAlignment)
        {
        case HorizontalAlignment::Left:
            deltaH = min(horizontalChange, m_cropOverlayWidth - minWidth);
            left = m_cropOverlayLeft + deltaH;
            if (m_left < left)
            {
                m_cropOverlayLeft = left;
                m_cropOverlayWidth -= deltaH;
                if (m_cropOverlayWidth > m_actualWidth)
                {
                    m_cropOverlayWidth = m_actualWidth;
                }
                OnPropertyChanged("CropOverlayLeft");
                OnPropertyChanged("CropOverlayWidth");
            }
            break;
        case HorizontalAlignment::Right:
            deltaH = min(-horizontalChange, m_cropOverlayWidth - minWidth);
            m_cropOverlayWidth -= deltaH;
            if (m_cropOverlayWidth + m_cropOverlayLeft > m_right)
            {
                m_cropOverlayWidth = m_right - m_cropOverlayLeft;
            }
            OnPropertyChanged("CropOverlayWidth");
            break;

        default:
            break;
        }
    }
}

void CropImageViewModel::DoCrop(uint32_t xOffset, uint32_t yOffset, uint32_t newHeight, uint32_t newWidth, uint32_t oldWidth, byte* pSrcPixels, byte* pDestPixels)
{    
    assert(IsBackgroundThread());
    parallel_for (0u, newHeight, [xOffset, yOffset, newHeight, newWidth, oldWidth, pDestPixels, pSrcPixels](unsigned int y)
    {
        for (unsigned int x = 0; x < newWidth; x++)
        {
            pDestPixels[(x + y * newWidth) * 4] = 
                pSrcPixels[(x +  xOffset + (y + yOffset) * oldWidth) * 4];     // B
            pDestPixels[(x + y * newWidth) * 4 + 1] = 
                pSrcPixels[(x +  xOffset + (y + yOffset) * oldWidth) * 4 + 1]; // G
            pDestPixels[(x + y * newWidth) * 4 + 2] = 
                pSrcPixels[(x +  xOffset + (y + yOffset) * oldWidth) * 4 + 2]; // R
            pDestPixels[(x + y * newWidth) * 4 + 3] =
                pSrcPixels[(x +  xOffset + (y + yOffset) * oldWidth) * 4 + 3]; // A
        }
    });        
}

task<void> CropImageViewModel::CropImageAsync(float64 actualWidth)
{
    assert(IsMainThread());
    ChangeInProgress(true);

    // Calculate crop values
    float64 scaleFactor = m_image->PixelWidth / actualWidth;
    unsigned int xOffset = safe_cast<unsigned int>((m_cropOverlayLeft - m_left) * scaleFactor);
    unsigned int yOffset = safe_cast<unsigned int>((m_cropOverlayTop - m_top) * scaleFactor);
    unsigned int newWidth = safe_cast<unsigned int>(m_cropOverlayWidth * scaleFactor); 
    unsigned int newHeight = safe_cast<unsigned int>(m_cropOverlayHeight * scaleFactor);

    if (newHeight < MINIMUMBMPSIZE || newWidth < MINIMUMBMPSIZE)
    {
        ChangeInProgress(false);
        m_isCropOverlayVisible = false;
        OnPropertyChanged("IsCropOverlayVisible");
        return create_empty_task();
    }

    m_cropX += xOffset;
    m_cropY += yOffset;

    // Create destination bitmap
    WriteableBitmap^ destImage = ref new WriteableBitmap(newWidth, newHeight);

    // Get pointers to the source and destination pixel data
    byte* pSrcPixels = GetPointerToPixelData(m_image->PixelBuffer);
    byte* pDestPixels = GetPointerToPixelData(destImage->PixelBuffer);   
    auto oldWidth = m_image->PixelWidth;

    return create_task([this, xOffset, yOffset, newHeight, newWidth, oldWidth, pSrcPixels, pDestPixels] () {
        assert(IsBackgroundThread());
        DoCrop(xOffset, yOffset, newHeight, newWidth, oldWidth, pSrcPixels, pDestPixels);
    }).then([this, destImage](){
        assert(IsMainThread());

        // Update image on screen
        m_image = destImage;
        OnPropertyChanged("Image");
        ChangeInProgress(false);
    }, task_continuation_context::use_current()).then(ObserveException<void>(m_exceptionPolicy));
}

