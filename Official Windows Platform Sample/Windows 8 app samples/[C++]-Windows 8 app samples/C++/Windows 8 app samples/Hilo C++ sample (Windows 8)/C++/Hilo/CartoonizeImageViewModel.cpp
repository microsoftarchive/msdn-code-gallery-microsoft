// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"

#include <amp.h>

#include "CartoonizeImageViewModel.h"
#include "DelegateCommand.h"
#include "IPhoto.h"
#include "TaskExceptionsExtensions.h"
#include "ImageNavigationData.h"
#include "ImageUtilities.h"
#include "Repository.h"

#include "PPLCartoon.h"
#include "AmpPixel.h"
#include "AmpCartoon.h"

using namespace concurrency;
using namespace Hilo;
using namespace Platform;
using namespace std;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Imaging;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::UI::Xaml::Navigation;

Platform::String^ const NeighborWindowKey = "neighbourWindow";
Platform::String^ const PhasesKey = "phases";

const unsigned int MaxWidth = 1024;
const unsigned int MaxHeight = 768;

CartoonizeImageViewModel::CartoonizeImageViewModel(shared_ptr<Repository> repository, shared_ptr<ExceptionPolicy> exceptionPolicy) :
    ImageBase(exceptionPolicy), m_repository(repository), m_inProgress(false), m_isSaving(false), 
    m_isAmpPixelArrayPopulated(false), m_isSourcePixelsPopulated(false), m_checkedForHardwareAcceleration(false), m_useHardwareAcceleration(false),
    m_neighborWindow(3.0), m_phases(3.0), m_pOriginalPixels(nullptr), m_pSourcePixels(nullptr),
    m_scaledWidth(0u), m_scaledHeight(0u)
{
    m_cartoonizeCommand = ref new DelegateCommand(
        ref new ExecuteDelegate(this, &CartoonizeImageViewModel::CartoonizeImage),
        ref new CanExecuteDelegate(this, &CartoonizeImageViewModel::IsCartoonizing));
    m_resumeCartoonizeCommand = ref new DelegateCommand(
        ref new ExecuteDelegate(this, &CartoonizeImageViewModel::Unsnap), nullptr);
    m_saveCommand = ref new DelegateCommand(
        ref new ExecuteDelegate(this, &CartoonizeImageViewModel::SaveImage),
        ref new CanExecuteDelegate(this, &CartoonizeImageViewModel::IsCartoonizing));
    m_cancelCommand = ref new DelegateCommand(
        ref new ExecuteDelegate(this, &CartoonizeImageViewModel::CancelCartoonize), nullptr);

    m_initializationTask = create_empty_task();
    ViewModelBase::m_isAppBarSticky = true;
}

CartoonizeImageViewModel::~CartoonizeImageViewModel()
{
    if (m_pOriginalPixels != nullptr)
    {
        delete[] m_pOriginalPixels;
        m_pOriginalPixels = nullptr;
    }
    if (m_pSourcePixels != nullptr)
    {
        delete[] m_pSourcePixels;
        m_pSourcePixels = nullptr;
    }
}

ICommand^ CartoonizeImageViewModel::CartoonizeCommand::get()
{
    return m_cartoonizeCommand;
}

ICommand^ CartoonizeImageViewModel::ResumeCartoonizeCommand::get()
{
    return m_resumeCartoonizeCommand;
}

ICommand^ CartoonizeImageViewModel::SaveCommand::get()
{
    return m_saveCommand;
}

ICommand^ CartoonizeImageViewModel::CancelCommand::get()
{
    return m_cancelCommand;
}

bool CartoonizeImageViewModel::InProgress::get()
{
    return m_inProgress;
}

ImageSource^ CartoonizeImageViewModel::Image::get()
{
    return m_image;
}

float64 CartoonizeImageViewModel::NeighborWindow::get()
{
    return m_neighborWindow;
}

void CartoonizeImageViewModel::NeighborWindow::set(float64 value)
{
    if (value != m_neighborWindow)
    {
        m_neighborWindow = value;
    }
}

float64 CartoonizeImageViewModel::Phases::get()
{
    return m_phases;
}

void CartoonizeImageViewModel::Phases::set(float64 value)
{
    if (value != m_phases)
    {
        m_phases = value;
    }
}

concurrency::task<IPhoto^> CartoonizeImageViewModel::GetImagePhotoAsync()
{
    assert(IsMainThread());
    return m_repository->GetSinglePhotoAsync(m_photoPath);
}

void CartoonizeImageViewModel::CheckForCancellation()
{
    if (is_task_cancellation_requested())
    {
        cancel_current_task();
    }
}

void CartoonizeImageViewModel::ChangeInProgress(bool value)
{
    m_inProgress = value;
    OnPropertyChanged("InProgress");
}

void CartoonizeImageViewModel::EvaluateCommands()
{
    m_cartoonizeCommand->CanExecute(IsCartoonizing(nullptr));
    m_saveCommand->CanExecute(IsCartoonizing(nullptr));
}

void CartoonizeImageViewModel::CartoonizeImage(Object^ parameter)
{
    assert(IsMainThread());
    m_cts = cancellation_token_source();
    auto token = m_cts.get_token();

    // Check for hardware acceleration if we haven't already.
    if (!m_checkedForHardwareAcceleration)
    {
        m_checkedForHardwareAcceleration = true;
        accelerator acc;
        m_useHardwareAcceleration = !acc.is_emulated;
    }

    ChangeInProgress(true);
    EvaluateCommands();
    m_initializationTask = m_initializationTask.then([this, token]() -> task<void> 
    {
        // Use the C++ AMP algorithim if the default accelerator is not an emulator (WARP or reference device).
        if (m_useHardwareAcceleration)
        {
            return CartoonizeImageAmpAsync(token);
        }
        // Otherwise, use the PPL to leverage all available CPU cores.
        else
        {
            return CartoonizeImagePPLAsync(token);
        }
    }, task_continuation_context::use_current()).then([this](task<void> priorTask)
    {
        m_initializationTask = create_empty_task();
        ChangeInProgress(false);
        EvaluateCommands();
        priorTask.get();
    }, task_continuation_context::use_current()).then(ObserveException<void>(m_exceptionPolicy));
}

void CartoonizeImageViewModel::Unsnap(Object^ parameter)
{
    if (ApplicationView::Value == ApplicationViewState::Snapped)
    {
        ApplicationView::TryUnsnap();
    }
}

void CartoonizeImageViewModel::SaveImage(Object^ parameter)
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
        assert(IsMainThread());
        (*encodedStream) = stream;
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
        catch (const task_canceled&)
        {
        }
    }).then(ObserveException<void>(m_exceptionPolicy));
}

void CartoonizeImageViewModel::CancelCartoonize(Object^ parameter)
{
    m_cts.cancel();
    ViewModelBase::GoBack();
}

bool CartoonizeImageViewModel::IsCartoonizing(Object^ parameter)
{
    return !m_inProgress;
}

// See http://go.microsoft.com/fwlink/?LinkId=267280 for more info on Hilo's implementation of suspend/resume.
void CartoonizeImageViewModel::LoadState(IMap<String^, Object^>^ stateMap)
{
    if (stateMap != nullptr)
    {
        if (stateMap->HasKey(NeighborWindowKey))
            m_neighborWindow = static_cast<float64>(stateMap->Lookup(NeighborWindowKey));

        if (stateMap->HasKey(PhasesKey))
            m_phases = static_cast<float64>(stateMap->Lookup(PhasesKey));
    }
}

void CartoonizeImageViewModel::SaveState(IMap<String^, Object^>^ stateMap)
{
    stateMap->Insert(NeighborWindowKey, m_neighborWindow);
    stateMap->Insert(PhasesKey, m_phases);
}

void CartoonizeImageViewModel::OnNavigatedTo(NavigationEventArgs^ e)
{
    String^ data = dynamic_cast<String^>(e->Parameter);
    ImageNavigationData navigationData(data);
    Initialize(navigationData.GetFilePath());
}

void CartoonizeImageViewModel::Initialize(String^ photoPath)
{
    assert(IsMainThread());
    m_photo = nullptr;
    m_image = nullptr;
    m_photoPath = photoPath;
    auto photoStream = make_shared<IRandomAccessStreamWithContentType^>(nullptr);

    m_initializationTask = GetImagePhotoAsync().then([this](IPhoto^ photo)
    {
        assert(IsMainThread());
        // Return to the hub page if the photo is no longer present.
        if (photo == nullptr)
        {
            GoHome();
            cancel_current_task();
        }
        m_photo = photo;
        // Read the image data.
        return m_photo->OpenReadAsync();
    }).then([photoStream](IRandomAccessStreamWithContentType^ imageData)
    {
        // Create a BitmapDecoder object to read the image's width and height.
        assert(IsMainThread());
        (*photoStream) = imageData;
        return BitmapDecoder::CreateAsync(imageData);
    }).then([this, photoStream](BitmapDecoder^ decoder) 
    {
        // Scale the image if its width or height exceed the maximum.
        assert(IsMainThread());
        auto ras = ref new InMemoryRandomAccessStream();
        auto origWidth = decoder->PixelWidth;
        auto origHeight = decoder->PixelHeight;

        return create_task(BitmapEncoder::CreateForTranscodingAsync(ras, decoder)).then([this, origWidth, origHeight](BitmapEncoder^ encoder)
        {
            assert(IsBackgroundThread());
            // Scale the image if either dimension is larger than the maximum.
            if (ScaleImageDimensions(origWidth, origHeight))
            {
                encoder->BitmapTransform->ScaledWidth = m_scaledWidth;
                encoder->BitmapTransform->ScaledHeight = m_scaledHeight;
                // Because we will apply color simplification, we care more about
                // speed than image quality.
                encoder->BitmapTransform->InterpolationMode = BitmapInterpolationMode::NearestNeighbor;
            }
            return encoder->FlushAsync();
        }, task_continuation_context::use_arbitrary()).then([ras]
        {
            assert(IsBackgroundThread());
            return dynamic_cast<IRandomAccessStream^>(ras);
        }, task_continuation_context::use_arbitrary());

    }).then([this, photoStream](IRandomAccessStream^ resizedStream)
    {
        assert(IsMainThread());
        m_image = ref new WriteableBitmap(m_scaledWidth, m_scaledHeight);
        resizedStream->Seek(0);
        return m_image->SetSourceAsync(resizedStream);
    }).then([this](task<void> priorTask)
    {
        assert(IsMainThread());
        OnPropertyChanged("Image");
        m_initializationTask = create_empty_task();
        priorTask.get();
    }).then(ObserveException<void>(m_exceptionPolicy));
}

bool CartoonizeImageViewModel::ScaleImageDimensions(unsigned int origWidth, unsigned int origHeight)
{
    auto& scaledWidth = m_scaledWidth;
    auto& scaledHeight = m_scaledHeight;

    scaledWidth = origWidth;
    scaledHeight = origHeight;

    auto scale_width = [](unsigned int& w, unsigned int& h)
    {
        float64 p = MaxWidth / static_cast<float64>(w);
        w = static_cast<unsigned int>(w * p);
        h = static_cast<unsigned int>(h * p);
    };
    auto scale_height = [](unsigned int& w, unsigned int& h)
    {
        float64 p = MaxHeight / static_cast<float64>(h);
        w = static_cast<unsigned int>(w * p);
        h = static_cast<unsigned int>(h * p);
    };

    if (origWidth <= MaxWidth && origHeight <= MaxHeight)
    {
        // The image does not need to be scaled.
        return false;
    }
    else if (origWidth > MaxWidth && origHeight > MaxHeight)
    {
        // Both dimensions exceed the maximum.
        while (scaledWidth > MaxWidth || scaledHeight > MaxHeight)
        {
            if (scaledWidth > MaxWidth)
            {
                scale_width(scaledWidth, scaledHeight);
            }
            else
            {
                scale_height(scaledWidth, scaledHeight);
            }
        }
    }
    else if (origWidth > MaxWidth)
    {
        // Width exceeds maximum.
        scale_width(scaledWidth, scaledHeight);
    }
    else
    {
        // Height exceeds maximum.
        assert(origHeight > MaxHeight);
        scale_height(scaledWidth, scaledHeight);
    }

    assert(scaledWidth <= MaxWidth);
    assert(scaledHeight <= MaxHeight);
    return true;
}

void CartoonizeImageViewModel::CopyPixelDataToPlatformArray(IBuffer^ buffer, Array<unsigned char, 1>^ pixels, unsigned int width, unsigned int height)
{
    assert(IsMainThread());

    // Get a pointer to the image pixel data.
    byte* pPixels = GetPointerToPixelData(buffer);

    // Copy pixel data from C style array to Platform::Array,
    // re-arranging from BGRA to RGBA.
    parallel_for (0u, height, [width, pixels, pPixels](unsigned int y)
    {
        for (unsigned int x = 0; x < width; x++)
        {
            pixels[(x + y * width) * 4] = 
                pPixels[(x + y * width) * 4 + 2]; // R
            pixels[(x + y * width) * 4 + 1] = 
                pPixels[(x + y * width) * 4 + 1]; // G
            pixels[(x + y * width) * 4 + 2] = 
                pPixels[(x + y * width) * 4];     // B
            pixels[(x + y * width) * 4 + 3] = 
                pPixels[(x + y * width) * 4 + 3]; // A
        }
    });
}

task<IRandomAccessStream^> CartoonizeImageViewModel::EncodeImageAsync(IRandomAccessStream^ sourceStream)
{
    assert(IsMainThread());
    auto bitmapEncoder = make_shared<BitmapEncoder^>(nullptr);
    auto backgroundContext = task_continuation_context::use_arbitrary();
    auto ras = ref new InMemoryRandomAccessStream();
    auto dpiX = make_shared<float64>(0.0);
    auto dpiY = make_shared<float64>(0.0);
    auto width = m_image->PixelWidth;
    auto height = m_image->PixelHeight;
    auto pixels = ref new Array<unsigned char>((width * height) * 4);

    CopyPixelDataToPlatformArray(m_image->PixelBuffer, pixels, width, height);

    return create_task(BitmapDecoder::CreateAsync(sourceStream))
        .then([ras, dpiX, dpiY](BitmapDecoder^ decoder)
    {
        assert(IsBackgroundThread());
        (*dpiX) = decoder->DpiX;
        (*dpiY) = decoder->DpiY;
        return BitmapEncoder::CreateForTranscodingAsync(ras, decoder);
    }, backgroundContext).then([this, width, height, dpiX, dpiY, pixels, bitmapEncoder](BitmapEncoder^ encoder)
    {
        assert(IsBackgroundThread());
        encoder->SetPixelData(BitmapPixelFormat::Rgba8,
            BitmapAlphaMode::Straight,
            width,
            height,
            (*dpiX),
            (*dpiY),
            pixels);
        // Force a new thumbnail to be generated.
        encoder->IsThumbnailGenerated = true;
        (*bitmapEncoder) = encoder;
        return encoder->FlushAsync();
    }, backgroundContext).then([this, ras, bitmapEncoder](task<void> encodeTask)
    {
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
        assert(IsBackgroundThread());
        return dynamic_cast<IRandomAccessStream^>(ras);
    }, backgroundContext);
}

AmpPixel* CartoonizeImageViewModel::CopyPixelDataToAmpPixelArray(byte* sourcePixels, unsigned int width, unsigned int height, const unsigned int size)
{
    (void) size;
    unsigned int nPixels = width * height;
    assert(nPixels <= size / 4);

    AmpPixel* pixels = new AmpPixel[(nPixels)];
    float r;
    float g;
    float b;

    // Copy data from sourcePixels to originalPixels, converting the type, ignoring alpha.
    for (unsigned int i = 0; i < nPixels; i++)
    {
        AmpPixel p;
        r = sourcePixels[4 * i] / 255.0F;
        g = sourcePixels[4 * i + 1] / 255.0F;
        b = sourcePixels[4 * i + 2] / 255.0F;

        p.SetFromRGB(r, g, b);
        pixels[i] = p; 
    }
    return pixels;
}

void CartoonizeImageViewModel::CopyAmpPixelArrayToPixelData(byte* destPixels, AmpPixel* newPixels, const unsigned int width, const unsigned int height)
{
    for (unsigned int pixel = 0; pixel < (width * height); pixel++)
    {
        AmpPixel p = newPixels[pixel];
        destPixels[4 * pixel] = static_cast<byte>(p.r * 255.0F); // R
        destPixels[4 * pixel + 1] = static_cast<byte>(p.g * 255.0F); // G
        destPixels[4 * pixel + 2] = static_cast<byte>(p.b * 255.0F); // B
        destPixels[4 * pixel + 3] = 255; // A
    }
}

task<void> CartoonizeImageViewModel::CartoonizeImageAmpAsync(cancellation_token token)
{
    assert(IsMainThread());

    const unsigned int width = m_image->PixelWidth;
    const unsigned int height = m_image->PixelHeight;
    const unsigned int size = m_image->PixelBuffer->Length;

    byte* sourcePixels = nullptr;
    if (!m_isAmpPixelArrayPopulated)
    {
        sourcePixels = GetPointerToPixelData(m_image->PixelBuffer);
    }

    auto destImage = ref new WriteableBitmap(m_image->PixelWidth, m_image->PixelHeight);
    byte* destPixels = GetPointerToPixelData(destImage->PixelBuffer);

    // Cartoonize source pixels into destination pixels.
    return create_task([this, sourcePixels, destPixels, width, height, size, token]
    {
        assert(IsBackgroundThread());

        const unsigned int neighborWindow = static_cast<unsigned int>(m_neighborWindow);
        const unsigned int phases = static_cast<unsigned int>(m_phases);

        if (!m_isAmpPixelArrayPopulated)
        {
            m_pOriginalPixels = CopyPixelDataToAmpPixelArray(sourcePixels, width, height, size);
            m_isAmpPixelArrayPopulated = true;
        }
        CheckForCancellation();

        auto newPixels = shared_ptr<AmpPixel>(new AmpPixel[width * height]);
        ApplyCartoonEffectAmp(m_pOriginalPixels, newPixels.get(), width, height, neighborWindow, phases, token);
        CheckForCancellation();

        CopyAmpPixelArrayToPixelData(destPixels, newPixels.get(), width, height);
    }, token).then([this, destImage](task<void> priorTask)
    {
        assert(IsMainThread());
        try
        {
            priorTask.get();

            // Update image on screen.
            m_image = destImage;
            OnPropertyChanged("Image");
        }
        catch (const task_canceled&)
        {
        }
        catch (const bad_alloc&)
        {
        }
    }, task_continuation_context::use_current());
}

task<void> CartoonizeImageViewModel::CartoonizeImagePPLAsync(cancellation_token token)
{
    assert(IsMainThread());

    const unsigned int width = m_image->PixelWidth;
    const unsigned int height = m_image->PixelHeight;
    const unsigned int size = m_image->PixelBuffer->Length;

    byte* sourcePixels = nullptr;
    if (!m_isSourcePixelsPopulated)
    {
        sourcePixels = GetPointerToPixelData(m_image->PixelBuffer);
        // Make a copy of the PixelBuffer data.
        if (m_pSourcePixels == nullptr)
        {
            m_pSourcePixels = new byte[size];
            memcpy(m_pSourcePixels, sourcePixels, size);
            m_isSourcePixelsPopulated = true;
        }
    }
    else
    {
        // Copy original data back to the PixelBuffer.
        if (sourcePixels == nullptr)
        {
            sourcePixels = GetPointerToPixelData(m_image->PixelBuffer);
            memcpy(sourcePixels,  m_pSourcePixels, size); 
        }
    }
      
    // Cartoonize source pixels into destination pixels.
    return create_task([this, sourcePixels, width, height, size, token]
    {
        assert(IsBackgroundThread());
        const unsigned int neighborWindow = static_cast<unsigned int>(m_neighborWindow);
        const unsigned int phases = static_cast<unsigned int>(m_phases);
        ApplyCartoonEffectPPL(sourcePixels, width, height, neighborWindow, phases, size, token); 
    }, token).then([this](task<void> priorTask)
    {
        assert(IsMainThread());
        try
        {
            priorTask.get();

            // Update image on screen.
            m_image->Invalidate();
        }
        catch (const task_canceled&)
        {
        }
        catch (const bad_alloc&)
        {
        }
    }, task_continuation_context::use_current());
}
