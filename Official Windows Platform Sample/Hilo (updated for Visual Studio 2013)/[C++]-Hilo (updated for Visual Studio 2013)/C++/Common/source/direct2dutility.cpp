//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "Direct2DUtility.h"

using namespace Hilo::Direct2DHelpers;

Direct2DUtility::Direct2DUtility()
{
}

Direct2DUtility::~Direct2DUtility()
{
}

//
// Creates a Direct2D bitmap from the specified
// file name.
//
HRESULT Direct2DUtility::LoadBitmapFromFile(
    ID2D1RenderTarget *renderTarget,
    const wchar_t *uri,
    unsigned int destinationWidth, 
    unsigned int destinationHeight,
    ID2D1Bitmap ** bitmap)
{
    HRESULT hr = S_OK;

    ComPtr<IWICBitmapDecoder> decoder;
    ComPtr<IWICBitmapFrameDecode> bitmapSource;
    ComPtr<IWICStream> stream;
    ComPtr<IWICFormatConverter> converter;
    ComPtr<IWICBitmapScaler> scaler;
    ComPtr<IWICImagingFactory> wicFactory;

    hr = GetWICFactory(&wicFactory);
    if (SUCCEEDED(hr))
    {
        hr = wicFactory->CreateDecoderFromFilename(
            uri,
            nullptr,
            GENERIC_READ,
            WICDecodeMetadataCacheOnLoad,
            &decoder);
    }

    if (SUCCEEDED(hr))
    {
        // Create the initial frame.
        hr = decoder->GetFrame(0, &bitmapSource);
    }

    if (SUCCEEDED(hr))
    {
        // Convert the image format to 32bppPBGRA
        // (DXGI_FORMAT_B8G8R8A8_UNORM + D2D1_ALPHA_MODE_PREMULTIPLIED).
        hr = wicFactory->CreateFormatConverter(&converter);
    }

    if (SUCCEEDED(hr))
    {
        // If a new width or height was specified, create an
        // IWICBitmapScaler and use it to resize the image.
        if (destinationWidth != 0 || destinationHeight != 0)
        {
            unsigned int originalWidth, originalHeight;
            hr = bitmapSource->GetSize(&originalWidth, &originalHeight);
            if (SUCCEEDED(hr))
            {
                if (destinationWidth == 0)
                {
                    float scalar = static_cast<float>(destinationHeight) / static_cast<float>(originalHeight);
                    destinationWidth = static_cast<unsigned int>(scalar * static_cast<float>(originalWidth));
                }
                else if (destinationHeight == 0)
                {
                    float scalar = static_cast<float>(destinationWidth) / static_cast<float>(originalWidth);
                    destinationHeight = static_cast<unsigned int>(scalar * static_cast<float>(originalHeight));
                }

                hr = wicFactory->CreateBitmapScaler(&scaler);
                if (SUCCEEDED(hr))
                {
                    hr = scaler->Initialize(
                        bitmapSource,
                        destinationWidth,
                        destinationHeight,
                        WICBitmapInterpolationModeCubic);
                }
                if (SUCCEEDED(hr))
                {
                    hr = converter->Initialize(
                        scaler,
                        GUID_WICPixelFormat32bppPBGRA,
                        WICBitmapDitherTypeNone,
                        nullptr,
                        0.f,
                        WICBitmapPaletteTypeMedianCut);
                }
            }
        }
        else // Don't scale the image.
        {
            hr = converter->Initialize(
                bitmapSource,
                GUID_WICPixelFormat32bppPBGRA,
                WICBitmapDitherTypeNone,
                nullptr,
                0.f,
                WICBitmapPaletteTypeMedianCut);
        }
    }

    if (SUCCEEDED(hr))
    {
        // Create a Direct2D bitmap from the WIC bitmap.
        hr = renderTarget->CreateBitmapFromWicBitmap(
            converter,
            nullptr,
            bitmap);
    }

    return hr;
}

//
// Creates a Direct2D bitmap from the specified
// resource name and type.
//
HRESULT Direct2DUtility::LoadBitmapFromResource(
    ID2D1RenderTarget *renderTarget,
    const wchar_t *resourceName,
    const wchar_t *resourceType,
    unsigned int destinationWidth,
    unsigned int destinationHeight,
    ID2D1Bitmap **bitmap)
{
    ComPtr<IWICImagingFactory> wicFactory;
    ComPtr<IWICBitmapDecoder> decoder;
    ComPtr<IWICBitmapFrameDecode> bitmapSource;
    ComPtr<IWICStream> stream;
    ComPtr<IWICFormatConverter> formatConverter;
    ComPtr<IWICBitmapScaler> scaler;

    HRSRC imageResHandle = nullptr;
    HGLOBAL imageResDataHandle = nullptr;
    void *imageFile = nullptr;
    unsigned long imageFileSize = 0;

    // Locate the resource.
    imageResHandle = ::FindResourceW(HINST_THISCOMPONENT, resourceName, resourceType);

    HRESULT hr = imageResHandle ? S_OK : E_FAIL;
    if (SUCCEEDED(hr))
    {
        // Load the resource.
        imageResDataHandle = ::LoadResource(HINST_THISCOMPONENT, imageResHandle);

        hr = imageResDataHandle ? S_OK : E_FAIL;
    }
    if (SUCCEEDED(hr))
    {
        // Lock it to get a system memory pointer.
        imageFile = ::LockResource(imageResDataHandle);

        hr = imageFile ? S_OK : E_FAIL;
    }
    if (SUCCEEDED(hr))
    {
        // Calculate the size.
        imageFileSize = SizeofResource(HINST_THISCOMPONENT, imageResHandle);

        hr = imageFileSize ? S_OK : E_FAIL;
    }

    if (SUCCEEDED(hr))
    {
        hr = GetWICFactory(&wicFactory);
    }

    if (SUCCEEDED(hr))
    {
        // Create a WIC stream to map onto the memory.
        hr = wicFactory->CreateStream(&stream);
    }

    if (SUCCEEDED(hr))
    {
        // Initialize the stream with the memory pointer and size.
        hr = stream->InitializeFromMemory(
            reinterpret_cast<unsigned char*>(imageFile),
            imageFileSize);
    }

    if (SUCCEEDED(hr))
    {
        // Create a decoder for the stream.
        hr = wicFactory->CreateDecoderFromStream(
            stream,
            nullptr,
            WICDecodeMetadataCacheOnLoad,
            &decoder);
    }

    if (SUCCEEDED(hr))
    {
        // Create the initial frame.
        hr = decoder->GetFrame(0, &bitmapSource);
    }

    if (SUCCEEDED(hr))
    {
        // Convert the image format to 32bppPBGRA
        // (DXGI_FORMAT_B8G8R8A8_UNORM + D2D1_ALPHA_MODE_PREMULTIPLIED).
        hr = wicFactory->CreateFormatConverter(&formatConverter);
    }

    if (SUCCEEDED(hr))
    {
        // If a new width or height was specified, create an
        // IWICBitmapScaler and use it to resize the image.
        if (destinationWidth != 0 || destinationHeight != 0)
        {
            unsigned int originalWidth, originalHeight;
            hr = bitmapSource->GetSize(&originalWidth, &originalHeight);
            if (SUCCEEDED(hr))
            {
                if (destinationWidth == 0)
                {
                    float scalar = static_cast<float>(destinationHeight) / static_cast<float>(originalHeight);
                    destinationWidth = static_cast<unsigned int>(scalar * static_cast<float>(originalWidth));
                }
                else if (destinationHeight == 0)
                {
                    float scalar = static_cast<float>(destinationWidth) / static_cast<float>(originalWidth);
                    destinationHeight = static_cast<unsigned int>(scalar * static_cast<float>(originalHeight));
                }

                hr = wicFactory->CreateBitmapScaler(&scaler);
                if (SUCCEEDED(hr))
                {
                    hr = scaler->Initialize(
                        bitmapSource,
                        destinationWidth,
                        destinationHeight,
                        WICBitmapInterpolationModeCubic);
                    if (SUCCEEDED(hr))
                    {
                        hr = formatConverter->Initialize(
                            scaler,
                            GUID_WICPixelFormat32bppPBGRA,
                            WICBitmapDitherTypeNone,
                            nullptr,
                            0.f,
                            WICBitmapPaletteTypeMedianCut);
                    }
                }
            }
        }
        else
        {
            hr = formatConverter->Initialize(
                bitmapSource,
                GUID_WICPixelFormat32bppPBGRA,
                WICBitmapDitherTypeNone,
                nullptr,
                0.f,
                WICBitmapPaletteTypeMedianCut);
        }
    }

    if (SUCCEEDED(hr))
    {
        //create a Direct2D bitmap from the WIC bitmap.
        hr = renderTarget->CreateBitmapFromWicBitmap(
            formatConverter,
            nullptr,
            bitmap);
    }

    return hr;
}

//
// Save a WICBitmap to a file 
// The original file name is required to get original metadata and encoding information
//
HRESULT Direct2DUtility::SaveBitmapToFile(IWICBitmap* updatedBitmap,
    const wchar_t *uriOriginalFile,
    const wchar_t *uriUpdatedFile)
{
    unsigned int frameCount = 0;
    unsigned int width = 0;
    unsigned int height = 0;

    bool isUsingTempFile = false;

    std::wstring outputFilePath;
    GUID containerFormat = GUID_ContainerFormatBmp;
    WICPixelFormatGUID pixelFormat = GUID_WICPixelFormatDontCare;

    ComPtr<IWICImagingFactory> factory;
    ComPtr<IWICStream> stream;
    ComPtr<IStream> sharedStream;
    ComPtr<IWICBitmapDecoder> decoder;
    ComPtr<IWICBitmapEncoder> encoder;
    ComPtr<IWICMetadataBlockWriter> blockWriter;
    ComPtr<IWICMetadataBlockReader> blockReader;

    HRESULT hr = Direct2DUtility::GetWICFactory(&factory);
    if (SUCCEEDED(hr))
    {
        factory->CreateDecoderFromFilename(uriOriginalFile, nullptr, GENERIC_READ, WICDecodeMetadataCacheOnDemand, &decoder);
    }

    if (SUCCEEDED(hr))
    {
        hr = decoder->GetFrameCount(&frameCount);
    }

    // Calculate temporary file name if an updated file was not specified
    if (nullptr == uriUpdatedFile)
    {
        // Indicate that we're using a tempoary file for saving
        isUsingTempFile = true;

        // Get location of temporary folder
        wchar_t tempFilePathBuffer[MAX_PATH];
        unsigned long count = GetTempPath(MAX_PATH, tempFilePathBuffer);
        
        if (count > MAX_PATH || count == 0)
        {
            // Unable to get temporary path (use current directory)
            outputFilePath.append(uriOriginalFile);
        }
        else
        {
            // Get file name by itself
            outputFilePath.append(uriOriginalFile);
            outputFilePath = outputFilePath.substr(outputFilePath.find_last_of('\\') + 1);

            // Insert temporary folder before the file name
            outputFilePath.insert(0, tempFilePathBuffer);
        }
    }
    else
    {
        outputFilePath.append(uriUpdatedFile);
    }

    // File extension to determine which container format to use for the output file
    std::wstring fileExtension(outputFilePath.substr(outputFilePath.find_last_of('.')));

    // Convert all characters to lower case
    std::transform(fileExtension.begin(), fileExtension.end(), fileExtension.begin (), tolower);

    if (SUCCEEDED(hr))
    {
        // Default value is bitmap encoding
        if (fileExtension.compare(L".jpg") == 0 ||
            fileExtension.compare(L".jpeg") == 0 ||
            fileExtension.compare(L".jpe") == 0 ||
            fileExtension.compare(L".jfif") == 0)
        {
            containerFormat = GUID_ContainerFormatJpeg;
        }
        else if (fileExtension.compare(L".tif") == 0 ||
            fileExtension.compare(L".tiff") == 0)
        {
            containerFormat = GUID_ContainerFormatTiff;
        }
        else if (fileExtension.compare(L".gif") == 0)
        {
            containerFormat = GUID_ContainerFormatGif;
        }
        else if (fileExtension.compare(L".png") == 0)
        {
            containerFormat = GUID_ContainerFormatPng;
        }
        else if (fileExtension.compare(L".wmp") == 0)
        {
            containerFormat = GUID_ContainerFormatWmp;
        }

        hr = factory->CreateEncoder(containerFormat, nullptr, &encoder);
    }

    if (SUCCEEDED(hr))
    {
        // Create a stream for the encoder
        hr = factory->CreateStream(&stream);
    }

    if (SUCCEEDED(hr))
    {
        // Update temporary file name if needed
        if (isUsingTempFile)
        {
            outputFilePath.append(L".tmp");
        }
        
        // Initialize the stream using the output file path
        hr = stream->InitializeFromFilename(outputFilePath.c_str(), GENERIC_WRITE);
    }

    if (SUCCEEDED(hr))
    {
        // Create encoder to write to image file
        hr = encoder->Initialize(stream, WICBitmapEncoderNoCache);
    }

    // Process each frame
    for (unsigned int i = 0; i < frameCount && SUCCEEDED(hr); i++)
    {
        //Frame variables
        ComPtr<IWICBitmapFrameDecode> frameDecode;
        ComPtr<IWICBitmapFrameEncode> frameEncode;
        ComPtr<IWICMetadataQueryReader> frameQueryReader;
        ComPtr<IWICMetadataQueryWriter> frameQueryWriter;

        //Get and create image frame
        if (SUCCEEDED(hr))
        {
            hr = decoder->GetFrame(i, &frameDecode);
        }

        if (SUCCEEDED(hr))
        {
            hr = encoder->CreateNewFrame(&frameEncode, nullptr);
        }

        //Initialize the encoder
        if (SUCCEEDED(hr))
        {
            hr = frameEncode->Initialize(nullptr);
        }

        //Get and set size
        if (SUCCEEDED(hr))
        {
            if (i == 0)
            {
                hr = updatedBitmap->GetSize(&width, &height);
            }
            else
            {
                hr = frameDecode->GetSize(&width, &height);
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = frameEncode->SetSize(width, height);
        }

        //Set pixel format
        if (SUCCEEDED(hr))
        {
            frameDecode->GetPixelFormat(&pixelFormat);
        }

        if (SUCCEEDED(hr))
        {
            hr = frameEncode->SetPixelFormat(&pixelFormat);
        }

        //Check that the destination format and source formats are the same
        bool formatsEqual = false;

        if (SUCCEEDED(hr))
        {
            GUID srcFormat;
            GUID destFormat;

            hr = decoder->GetContainerFormat(&srcFormat);
            
            if (SUCCEEDED(hr))
            {
                hr = encoder->GetContainerFormat(&destFormat);
            }
            
            if (SUCCEEDED(hr))
            {
                formatsEqual = (srcFormat == destFormat) ? true : false;
            }
        }

        if (SUCCEEDED(hr) && formatsEqual)
        {
            //Copy metadata using metadata block reader/writer
            frameDecode->QueryInterface(&blockReader);
            frameEncode->QueryInterface(&blockWriter);

            if (nullptr != blockReader && nullptr != blockWriter)
            {
                blockWriter->InitializeFromBlockReader(blockReader);
            }
        }

        if (SUCCEEDED(hr))
        {
            if (i == 0)
            {
                // Copy updated bitmap to output
                hr = frameEncode->WriteSource(updatedBitmap, nullptr);
            }
            else
            {
                // Copy existing image to output
                hr = frameEncode->WriteSource(static_cast<IWICBitmapSource*> (frameDecode), nullptr);
            }
        }

        //Commit the frame
        if (SUCCEEDED(hr))
        {
            hr = frameEncode->Commit();
        }
    }

    if (SUCCEEDED(hr))
    {
        encoder->Commit();
    }

    // Ensure that the input and output files are not locked by releasing corresponding objects
    if (stream)
    {
        stream = nullptr;
    }

    if (decoder)
    {
        decoder = nullptr;
    }

    if (encoder)
    {
        encoder = nullptr;
    }

    if (blockWriter)
    {
        blockWriter = nullptr;
    }

    if (blockReader)
    {
        blockReader = nullptr;
    }

    if (SUCCEEDED(hr) && isUsingTempFile)
    {
        // Move temporary file to current file
        if (! ::CopyFileW(outputFilePath.c_str(), uriOriginalFile, false))
        {
            hr = E_FAIL;
        }

        // Delete the temporary file
        ::DeleteFileW(outputFilePath.c_str());
    }

    return hr;
}

//
// Create a Direct2D Bitmap from a Shell Thumbnail cache
//
HRESULT Direct2DUtility::DecodeImageFromThumbCache(
    IShellItem *shellItem,
    ID2D1RenderTarget* renderTarget,
    unsigned int thumbnailSize,
    ID2D1Bitmap **bitmap)
{
    ComPtr<IShellItemImageFactory> imageFactory;
    ComPtr<IWICFormatConverter> converter;

    HRESULT hr = shellItem->QueryInterface(IID_PPV_ARGS(&imageFactory));

    HBITMAP hBitmap = nullptr;
    if (SUCCEEDED(hr))
    {
        SIZE size = {thumbnailSize, thumbnailSize};
        hr = imageFactory->GetImage(
            size,
            SIIGBF_BIGGERSIZEOK, // improves performance, since we'll need to resize anyway
            &hBitmap);
    }

    hr = nullptr == hBitmap ? E_FAIL : hr;

    ComPtr<IWICImagingFactory> wicFactory;
    if (SUCCEEDED(hr))
    {
        hr = GetWICFactory(&wicFactory);
    }

    ComPtr<IWICBitmap> wicBitmap;
    if (SUCCEEDED(hr))
    {
        hr = wicFactory->CreateBitmapFromHBITMAP(
            hBitmap,
            nullptr,
            WICBitmapUseAlpha,
            &wicBitmap);
    }

    // Make sure to free the resource as soon as it's not needed.
    if (nullptr != hBitmap)
    {
        ::DeleteObject(hBitmap);
        hBitmap = nullptr;
    }

    if (SUCCEEDED(hr))
    {
        // Convert the image format to 32bppPBGRA
        // (DXGI_FORMAT_B8G8R8A8_UNORM + D2D1_ALPHA_MODE_PREMULTIPLIED).
        hr = wicFactory->CreateFormatConverter(&converter);
    }

    if (SUCCEEDED(hr))
    {
        hr = converter->Initialize(
            wicBitmap,
            GUID_WICPixelFormat32bppPBGRA,
            WICBitmapDitherTypeNone,
            nullptr,
            0.f,
            WICBitmapPaletteTypeMedianCut);
    }

    if (SUCCEEDED(hr))
    {
        // Create a D2D bitmap from the WIC bitmap
        hr = renderTarget->CreateBitmapFromWicBitmap(
            converter,
            nullptr,
            bitmap);
    }

    return hr;
}

//
// Get a Direct2D factory
//
HRESULT Direct2DUtility::GetD2DFactory(ID2D1Factory** factory)
{
    static ComPtr<ID2D1Factory> m_pD2DFactory;
    HRESULT hr = S_OK;

    if (nullptr == m_pD2DFactory)
    {
#if defined(DEBUG) || defined(_DEBUG)
        D2D1_FACTORY_OPTIONS options;
        options.debugLevel = D2D1_DEBUG_LEVEL_INFORMATION;

        hr = D2D1CreateFactory(
            D2D1_FACTORY_TYPE_MULTI_THREADED,
            options,
            &m_pD2DFactory);
#else
        hr = D2D1CreateFactory(
            D2D1_FACTORY_TYPE_MULTI_THREADED,
            &m_pD2DFactory);
#endif
    }

    if (SUCCEEDED(hr))
    {
        hr = AssignToOutputPointer(factory, m_pD2DFactory);
    }

    return hr;
}

//
// Get a WIC Imaging factory
//
HRESULT Direct2DUtility::GetWICFactory(IWICImagingFactory** factory)
{
    static ComPtr<IWICImagingFactory> m_pWICFactory;
    HRESULT hr = S_OK;

    if (nullptr == m_pWICFactory)
    {
        hr = CoCreateInstance(
            CLSID_WICImagingFactory,
            nullptr,
            CLSCTX_INPROC_SERVER,
            IID_PPV_ARGS(&m_pWICFactory));
    }

    if (SUCCEEDED(hr))
    {
        hr = AssignToOutputPointer(factory, m_pWICFactory);
    }

    return hr;
}

//
// Get a DirectWrite factory
//
HRESULT Direct2DUtility::GetDWriteFactory(IDWriteFactory** factory )
{
    static ComPtr<IDWriteFactory> m_pDWriteFactory;
    HRESULT hr = S_OK;

    if (nullptr == m_pDWriteFactory)
    {
        hr = DWriteCreateFactory(DWRITE_FACTORY_TYPE_SHARED, __uuidof(m_pDWriteFactory), reinterpret_cast<IUnknown**>(&m_pDWriteFactory));
    }

    if (SUCCEEDED(hr))
    {
        hr = AssignToOutputPointer(factory, m_pDWriteFactory);
    }

    return hr;
}