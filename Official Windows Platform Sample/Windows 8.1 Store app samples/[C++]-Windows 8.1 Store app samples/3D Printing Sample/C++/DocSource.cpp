//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "pch.h"
#include "docsource.h"
#include <wrl.h>
#include <shcore.h>
#include <ppltasks.h>
#include <DocumentTarget.h>
#include <xpsobjectmodel_2.h>

using namespace Microsoft::WRL;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::Graphics::Printing;
using namespace Windows::Graphics::Printing::OptionDetails;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Concurrency;

#pragma region IDocumentPageSource Methods

String^ printTicketString =
"<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\" ?>\r\n\
<psf:PrintTicket xmlns:psf=\"http://schemas.microsoft.com/windows/2003/08/printing/printschemaframework\" \
xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" \
xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" \
xmlns:psk=\"http://schemas.microsoft.com/windows/2003/08/printing/printschemakeywords\" \
version=\"1\" \
xmlns:psk3d=\"http://schemas.microsoft.com/3dmanufacturing/2013/01/pskeywords3d\">";

IFACEMETHODIMP
CDocumentSource::GetPreviewPageCollection(
    _In_  IPrintDocumentPackageTarget*  docPackageTarget,
    _Out_ IPrintPreviewPageCollection** docPageCollection
    )
{
    HRESULT hr = (docPackageTarget != nullptr) ? S_OK : E_INVALIDARG;

    // Get for IPrintPreviewDXGIPackageTarget interface.
    if (SUCCEEDED(hr))
    {
        hr = docPackageTarget->GetPackageTarget(
            __uuidof(IPrintPreviewDxgiPackageTarget),
            IID_PPV_ARGS(&m_dxgiPreviewTarget)
            );
    }

    ComPtr<IPrintPreviewPageCollection> pageCollection;
    if (SUCCEEDED(hr))
    {
        ComPtr<CDocumentSource> docSource(this);
        hr = docSource.As<IPrintPreviewPageCollection>(&pageCollection);
    }

    if (SUCCEEDED(hr))
    {
        hr = pageCollection.CopyTo(docPageCollection);
    }

    return hr;
}

IFACEMETHODIMP
CDocumentSource::MakeDocument(
    _In_ IInspectable*                docOptions,
    _In_ IPrintDocumentPackageTarget* docPackageTarget
    )
{
    if (docOptions == nullptr || docPackageTarget == nullptr)
    {
        return E_INVALIDARG;
    }

    // Set the model data on the doc source
    ComPtr<IStream> pModelStream;
    m_renderer->Generate3dModelXml(pModelStream);
    Set3dModelXml(pModelStream);
    float32 width = 0;
    float32 height = 0;
    InMemoryRandomAccessStream^ imageStream = m_renderer->GetPageImage();
    m_renderer->GetPageImageSize(&width, &height);
    SetPageImage(imageStream, width, height);

    // Get print settings from PrintTaskOptions for printing, such as page description, which contains page size, imageable area, DPI.
    // User can obtain other print settings in the same way, such as NumberOfCopies, which are not shown in this sample.
    PrintTaskOptions^       option      = reinterpret_cast<PrintTaskOptions^>(docOptions);
    PrintPageDescription    pageDesc    = option->GetPageDescription(1); // Get the description of the first page.
    XPS_SIZE                pageSize    = {pageDesc.PageSize.Width, pageDesc.PageSize.Height};

    //Get the package target types that are supported by the IPrintDocumentManagementTarget provided by the Print system.
    UINT32 targetCount = 0;
    GUID* pGuids = nullptr;

    HRESULT hr = docPackageTarget->GetPackageTargetTypes(&targetCount, &pGuids);

    if (SUCCEEDED(hr) &&
        (targetCount == 0 ||
        pGuids == nullptr))
    {
        // If no valid target type GUID is returned, treat that as failure.
        hr = E_UNEXPECTED;
    }

    // See if package target supports 3D and get the right kind of package target interface (2D or 3D)
    ComPtr<IXpsDocumentPackageTarget>   packageTarget2D;
    ComPtr<IXpsDocumentPackageTarget3D> packageTarget3D;
    bool is3D = false;

    if (SUCCEEDED(hr))
    {
        for (auto it = pGuids; !is3D && it != pGuids + targetCount; ++it)
        {
            if (IsEqualGUID(*it, ID_DOCUMENTPACKAGETARGET_OPENXPS_WITH_3D))
            {
                is3D = true;
            }
        }

        if (is3D)
        {
            hr = docPackageTarget->GetPackageTarget(
                        ID_DOCUMENTPACKAGETARGET_OPENXPS_WITH_3D,
                        __uuidof(IXpsDocumentPackageTarget3D),
                        &packageTarget3D);
        }
        else
        {
            hr = docPackageTarget->GetPackageTarget(
                        pGuids[0],  // Either XPS or OXPS package target
                        __uuidof(IXpsDocumentPackageTarget),
                        &packageTarget2D);
        }

        CoTaskMemFree(pGuids);
    }

    // Get the XPS factory to generate 2D payload
    ComPtr<IXpsOMObjectFactory>  xpsFactory;
    ComPtr<IXpsOMObjectFactory1> xpsFactory1;

    if (SUCCEEDED(hr))
    {
        if (is3D)
        {
            hr = packageTarget3D->GetXpsOMFactory( &xpsFactory );
        }
        else
        {
            hr = packageTarget2D->GetXpsOMFactory( &xpsFactory );
        }
    }

    // Get the OXPS factory
    if (SUCCEEDED(hr))
    {
        hr = (xpsFactory.Get())->QueryInterface( __uuidof(IXpsOMObjectFactory1), &xpsFactory1 );
    }

    // Create a part URI for the 2D payload root
    ComPtr<IOpcPartUri> partUriFDS;

    if (SUCCEEDED(hr))
    {
        hr = xpsFactory1->CreatePartUri( L"/FixedDocumentSequence.fdseq", &partUriFDS );
    }

    // Create a package writer for our content
    ComPtr<IXpsOMPackageWriter>   packageWriter2D;
    ComPtr<IXpsOMPackageWriter3D> packageWriter3D;
    ComPtr<IOpcPartUri> partUriModel;

    if (SUCCEEDED(hr))
    {
        if (is3D)
        {
            hr = xpsFactory->CreatePartUri( L"/3D/3dModel.model", &partUriModel );

            if (SUCCEEDED(hr))
            {
                const LARGE_INTEGER liZero = {0};
                hr = m_modelStream->Seek(liZero, STREAM_SEEK_SET, NULL);

                if (SUCCEEDED(hr))
                {
                    hr = packageTarget3D->GetXpsOMPackageWriter3D(
                         partUriFDS.Get(),
                         NULL,
                         partUriModel.Get(),
                         m_modelStream.Get(),
                         &packageWriter3D
                         );
                }
            }

            if (SUCCEEDED(hr))
            {
                hr = (packageWriter3D.Get())->QueryInterface( __uuidof(IXpsOMPackageWriter), &packageWriter2D );
            }
        }
        else
        {
            hr = packageTarget2D->GetXpsOMPackageWriter(
                 partUriFDS.Get(),
                 NULL,
                 &packageWriter2D
                 );
        }
    }

    // Add the remainder of the 3D payload
    if (SUCCEEDED(hr) && is3D)
    {
        ComPtr<IStream> modelStream;

        try
        {
            // Create a Model PrintTicket stream
            InMemoryRandomAccessStream^ memStream = ref new InMemoryRandomAccessStream();

            // Create the DataWriter object backed by the in-memory stream.
            DataWriter^ modelPTWriter = ref new DataWriter(memStream);
            modelPTWriter->UnicodeEncoding = UnicodeEncoding::Utf8;
            modelPTWriter->ByteOrder = ByteOrder::LittleEndian;

            // Write the data
            modelPTWriter->WriteString( MakePrintTicket(docOptions) );

            // Flush it and wait for completion
            auto writeTask = create_task(modelPTWriter->StoreAsync()).then([this, modelPTWriter] (unsigned int bytesStored)
            {
                // Detach the memStream so that modelWriter does not close it when it goes out of scope
                modelPTWriter->DetachStream();
            });
            writeTask.wait();

            // Convert the RandomAccessStream memStream into the IStream modelStream
            memStream->Seek(0);
            hr = CreateStreamOverRandomAccessStream( reinterpret_cast<IUnknown*>(memStream), IID_PPV_ARGS(&modelStream) );
        }
        catch (Platform::Exception^ e)
        {
            hr = e->HResult;
        }

        // Generate Model PrintTicket URI
        ComPtr<IOpcPartUri> partUriPT;
        if (SUCCEEDED(hr))
        {
            hr = xpsFactory1->CreatePartUri( L"/3D/Metadata/Model_PT.xml", &partUriPT );
        }

        // Add the PrintTicket to the 3D payload
        if (SUCCEEDED(hr))
        {
            hr = packageWriter3D->SetModelPrintTicket( partUriPT.Get(), modelStream.Get() );
        }

        // Generate the texture URI - this must match the name that appears in the 3D Model part markup
        ComPtr<IOpcPartUri> partUriTexture;

        if (SUCCEEDED(hr))
        {
            hr = xpsFactory1->CreatePartUri( L"/3D/Textures/tex0.texture", &partUriTexture );
        }

        // Load our texture as a stream
        ComPtr<IStream> textureStream;

        if (SUCCEEDED(hr))
        {
            StorageFolder^ packageLocation = Package::Current->InstalledLocation;
            auto openTask = create_task(packageLocation->GetFileAsync("reftexture.png")).then([&] (StorageFile^ file)
            {
                return file->OpenReadAsync();
            }).then([&] (IRandomAccessStreamWithContentType^ stream)
            {
                CreateStreamOverRandomAccessStream( reinterpret_cast<IUnknown*>(stream), IID_PPV_ARGS(&textureStream) );
            });
            openTask.wait();
        }

        // Add our texture to the 3D payload
        if (SUCCEEDED(hr))
        {
            hr = packageWriter3D->AddModelTexture( partUriTexture.Get(), textureStream.Get() );
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = Add2DPayload(xpsFactory, xpsFactory1, packageWriter2D, pageSize);
    }

    if(SUCCEEDED(hr))
    {
        hr = packageWriter2D->Close();
    }

    return hr;
}

HRESULT
CDocumentSource::Add2DPayload(
    ComPtr<IXpsOMObjectFactory>& xpsFactory,
    ComPtr<IXpsOMObjectFactory1>& xpsFactory1,
    ComPtr<IXpsOMPackageWriter>& packageWriter2D,
    XPS_SIZE& pageSize
    )
{
    // Define some variables for building the 2D XPS page
    ComPtr<IXpsOMVisualCollection>          xpsVisualColl;
    ComPtr<IXpsOMPath>                      xpsPath;
    ComPtr<IXpsOMGeometry>                  xpsGeometry;
    ComPtr<IXpsOMGeometryFigureCollection>  xpsFigureColl;
    ComPtr<IXpsOMGeometryFigure>            xpsFigure;
    ComPtr<IXpsOMImageBrush>                xpsBrush;
    ComPtr<IXpsOMSolidColorBrush>           xpsColorBrush;

    // Start creating the 2D payload
    ComPtr<IOpcPartUri> partUriFD;
    HRESULT hr = xpsFactory1->CreatePartUri( L"/Documents/1/FixedDocument.fdoc", &partUriFD );

    ComPtr<IOpcPartUri> partUriPage;
    if (SUCCEEDED(hr))
    {
        hr = xpsFactory1->CreatePartUri(L"/Documents/1/Pages/1.fpage", &partUriPage );
    }

    if (SUCCEEDED(hr))
    {
        hr = packageWriter2D->StartNewDocument(partUriFD.Get(), NULL, NULL, NULL, NULL);
    }

    ComPtr<IXpsOMPage1> xpsPage;

    if (SUCCEEDED(hr))
    {
        hr = xpsFactory1->CreatePage1(
            &pageSize,                  // page size
            L"en-US",                   // page language
            partUriPage.Get(),          // page part name
            &xpsPage);
    }

    // Create a box path to hold our 2D view (image) of the model
    if (SUCCEEDED(hr))
    {
        hr = xpsFactory1->CreatePath( &xpsPath );
    }

    // Create a geometry
    if (SUCCEEDED(hr))
    {
        hr = xpsFactory1->CreateGeometry( &xpsGeometry );
    }

    // Connect the geometry to our path
    if (SUCCEEDED(hr))
    {
        hr = xpsPath->SetGeometryLocal( xpsGeometry.Get() );
    }

    // Get a pointer to the figure collection
    if (SUCCEEDED(hr))
    {
        hr = xpsGeometry->GetFigures( &xpsFigureColl );
    }

    // Define the corner points of the box
    D2D_RECT_F drawRect = D2D1::RectF((pageSize.width / 2.0f) - (m_pageImageWidth / 2.0f),
                                      (pageSize.height / 2.0f) - (m_pageImageHeight / 2.0f),
                                      (pageSize.width / 2.0f) - (m_pageImageWidth / 2.0f) + m_pageImageWidth,
                                      (pageSize.height / 2.0f) - (m_pageImageHeight / 2.0f) + m_pageImageHeight);
    XPS_POINT startPoint = { drawRect.left, drawRect.top };
    FLOAT sidebarSegmentData[6] = {
        drawRect.right,    // x2
        drawRect.top,      // y2
        drawRect.right,    // x3
        drawRect.bottom,   // y3
        drawRect.left,     // x4
        drawRect.bottom    // y4
    };

    // Define the segment types to be straight lines
    XPS_SEGMENT_TYPE sidebarSegmentTypes[4] = {
        XPS_SEGMENT_TYPE_LINE,
        XPS_SEGMENT_TYPE_LINE,
        XPS_SEGMENT_TYPE_LINE,
        XPS_SEGMENT_TYPE_LINE
    };

    // Define the lines to not stroked
    BOOL sidebarSegmentStrokes[4] = {
        FALSE, FALSE, FALSE, FALSE
    };

    // create the geometry figure interface
    if (SUCCEEDED(hr))
    {
        hr = xpsFactory->CreateGeometryFigure( &startPoint, &xpsFigure );
    }

    // Set the segments using the information defined above
    if (SUCCEEDED(hr))
    {
        hr = xpsFigure->SetSegments(
            3,
            6,
            sidebarSegmentTypes,
            sidebarSegmentData,
            sidebarSegmentStrokes);
    }

    // Close the figure so that the last segment point is connected to the start point
    if (SUCCEEDED(hr))
    {
        hr = xpsFigure->SetIsClosed( FALSE );
    }

    // Add the figure of the rectangle to the geometry
    if (SUCCEEDED(hr))
    {
        hr = xpsFigureColl->Append( xpsFigure.Get() );
    }

    // set the shape to be filled by the fill brush
    if (SUCCEEDED(hr))
    {
        hr = xpsFigure->SetIsFilled( TRUE );
    }

    // Create an image part URI
    ComPtr<IOpcPartUri> partUriImage;

    if (SUCCEEDED(hr))
    {
        hr = xpsFactory1->CreatePartUri( L"/Documents/1/Pages/Images/modelImage.png", &partUriImage );
    }

    // Create an image resource
    ComPtr<IStream> imageStream;

    if (SUCCEEDED(hr))
    {
        hr = CreateStreamOverRandomAccessStream( reinterpret_cast<IUnknown*>(m_pageImageStream), IID_PPV_ARGS(&imageStream) );
    }

    ComPtr<IXpsOMImageResource> imageResource;
    if (SUCCEEDED(hr))
    {
        hr = xpsFactory1->CreateImageResource( imageStream.Get(), XPS_IMAGE_TYPE_PNG, partUriImage.Get(), &imageResource );
    }

    // Create an image brush
    const XPS_RECT viewBox = { 0.0f, 0.0f, m_pageImageWidth, m_pageImageHeight };
    const XPS_RECT viewPort = { drawRect.left, drawRect.top, drawRect.right - drawRect.left, drawRect.bottom - drawRect.top };
    if (SUCCEEDED(hr))
    {
        hr = xpsFactory1->CreateImageBrush( imageResource.Get(), &viewBox, &viewPort, &xpsBrush );
    }

    if (SUCCEEDED(hr))
    {
        hr = xpsPath->SetFillBrushLocal( xpsBrush.Get() );
    }

    // Add it to the page
    if (SUCCEEDED(hr))
    {
        hr = xpsPage->GetVisuals(&xpsVisualColl);
    }

    if (SUCCEEDED(hr))
    {
        hr = xpsVisualColl->Append(xpsPath.Get());
    }

    if(SUCCEEDED(hr))
    {
        hr = packageWriter2D->AddPage (
            xpsPage.Get(),
            &pageSize,
            NULL,    // no discardable resource part
            NULL,    // no story fragments part
            NULL,    // no print ticket
            NULL);   // no thumbnail image
    }

    return hr;
}

#pragma endregion IDocumentPageSource Methods

#pragma region IPrintPreviewPageCollection Methods

IFACEMETHODIMP
CDocumentSource::Paginate(
    _In_   uint32           currentJobPage,
    _In_   IInspectable*    docOptions
    )
{
    HRESULT hr = (docOptions != nullptr) ? S_OK : E_INVALIDARG;

    if (SUCCEEDED(hr))
    {
        // Get print settings from PrintTaskOptions for preview, such as page description, which contains page size, DPI, etc.
        PrintTaskOptions^ option = reinterpret_cast<PrintTaskOptions^>(docOptions);
        PrintPageDescription pageDesc = option->GetPageDescription(currentJobPage);

        hr = m_dxgiPreviewTarget->InvalidatePreview();

        // Set the total page number.
        if (SUCCEEDED(hr))
        {
            hr = m_dxgiPreviewTarget->SetJobPageCount(PageCountType::FinalPageCount, m_totalPages);
        }

        if (SUCCEEDED(hr))
        {
            m_width = pageDesc.PageSize.Width;
            m_height = pageDesc.PageSize.Height;

            m_imageableRect = D2D1::RectF(
                pageDesc.ImageableRect.X,
                pageDesc.ImageableRect.Y,
                pageDesc.ImageableRect.X + pageDesc.ImageableRect.Width,
                pageDesc.ImageableRect.Y + pageDesc.ImageableRect.Height
                );

            // Now we are ready to let MakePage to be called.
            m_paginateCalled = true;
        }
    }

    return hr;
}

// Here, desiredWidth/desiredHeight is the desired size of preview surface by print mananger in system.
// The final size of the preview surface must have the same proportion as that of the desired width/height.
// In this sample, we just use it as preview size and return the scale variant for surface drawing.
// The size here is in DIPs.
float
CDocumentSource::TransformedPageSize(
    _In_  float                         desiredWidth,
    _In_  float                         desiredHeight,
    _Out_ Windows::Foundation::Size*    previewSize
    )
{
    float scale = 1.0f;

    if (desiredWidth > 0 && desiredHeight > 0)
    {
        previewSize->Width  = desiredWidth;
        previewSize->Height = desiredHeight;

        scale = m_width / desiredWidth;
    }
    else
    {
        previewSize->Width = 0;
        previewSize->Height = 0;
    }

    return scale;
}

// This sample only acts upon orientation setting for an example. The orientation is read from the user selection
// in the Print Experience and is then used to reflow the content in a different way.
IFACEMETHODIMP
CDocumentSource::MakePage(
    _In_ uint32 desiredJobPage,
    _In_ float  width,
    _In_ float  height
    )
{
    HRESULT hr = (width > 0 && height > 0) ? S_OK : E_INVALIDARG;

    // When desiredJobPage is JOB_PAGE_APPLICATION_DEFINED, it means a new preview begins. If the implementation here is by an async way,
    // for example, queue MakePage calls for preview, app needs to clean resources for previous preview before next.
    // In this sample, we will reset page number if Paginate() has been called.
    if (desiredJobPage == JOB_PAGE_APPLICATION_DEFINED && m_paginateCalled)
    {
        desiredJobPage = 1;
    }

    if (SUCCEEDED(hr) && m_paginateCalled)
    {
        // Calculate the size of preview surface, according to desired width and height.
        Windows::Foundation::Size previewSize;
        float scale = TransformedPageSize(width, height, &previewSize);

        try
        {
            m_renderer->DrawPreviewSurface(
                previewSize.Width,
                previewSize.Height,
                scale,
                m_imageableRect,
                desiredJobPage,
                m_dxgiPreviewTarget.Get()
                );
        }
        catch (Platform::Exception^ e)
        {
            hr = e->HResult;

            if (hr == D2DERR_RECREATE_TARGET)
            {
                // In case of device lost, we should recover so that the device is ready to render
                // the next preview page when requested. At the same time, we should propagate this
                // error to the Windows Store app Print Dialog.
                m_renderer->HandleDeviceLost();
            }
        }
    }

    return hr;
}

#pragma region IPrintPreviewPageCollection Methods

#pragma region Document Methods
void CDocumentSource::InvalidatePreview()
{
    m_dxgiPreviewTarget->InvalidatePreview();
}

void CDocumentSource::Set3dModelXml(
    _In_ ComPtr<IStream>& modelStream
    )
{
    m_modelStream = modelStream;
}

void CDocumentSource::SetPageImage(
    _In_ InMemoryRandomAccessStream^ pageImageStream,
    _In_ float32 pageImageWidth,
    _In_ float32 pageImageHeight
    )
{
    m_pageImageStream = pageImageStream;
    m_pageImageWidth = pageImageWidth;
    m_pageImageHeight = pageImageHeight;
}

String^ CDocumentSource::MakePrintTicket(
    _In_ IInspectable* docOptions
    )
{
    PrintTaskOptions^       options = reinterpret_cast<PrintTaskOptions^>(docOptions);
    PrintTaskOptionDetails^ printDetailedOptions = PrintTaskOptionDetails::GetFromPrintTaskOptions(options);
    IPrintOptionDetails^    printOptionDetails;
    String^                 pageContent;
    String^                 output;

    // Prepare the PT output
    output = printTicketString;

    // Set the quality setting
    output += "<psf:Feature name=\"psk3d:Job3DQuality\"><psf:Option name=\"psk3d:";
    printOptionDetails = printDetailedOptions->Options->Lookup(ref new String(L"Job3DQuality"));
    pageContent = safe_cast<String^>(printOptionDetails->Value);
    if (0 == wcscmp(pageContent->Data(), L"Job3DQualityHigh"))
    {
        output += "High";
    }
    else if (0 == wcscmp(pageContent->Data(), L"Job3DQualityMedium"))
    {
        output += "Medium";
    }
    else if (0 == wcscmp(pageContent->Data(), L"Job3DQualityDraft"))
    {
        output += "Draft";
    }
    output += "\"/></psf:Feature>";

    // Set the density setting
    output += "<psf:Feature name=\"psk3d:Job3DDensity\"><psf:Option name=\"psk3d:";
    printOptionDetails = printDetailedOptions->Options->Lookup(ref new String(L"Job3DDensity"));
    pageContent = safe_cast<String^>(printOptionDetails->Value);

    if (0 == wcscmp(pageContent->Data(), L"Job3DDensityHollow"))
    {
        output += "Hollow\"/>";
    }
    else if (0 == wcscmp(pageContent->Data(), L"Job3DDensityLow"))
    {
        output += "Low\"/>";
    }
    else if (0 == wcscmp(pageContent->Data(), L"Job3DDensityMedium"))
    {
        output += "Medium\"/>";
    }
    else if (0 == wcscmp(pageContent->Data(), L"Job3DDensityHigh"))
    {
        output += "High\"/>";
    }
    else if (0 == wcscmp(pageContent->Data(), L"Job3DDensitySolid"))
    {
        output += "Solid\"/>";
    }
    output += "</psf:Feature>";

    // Set the supports setting
    output += "<psf:Feature name=\"psk3d:Job3DSupports\"><psf:Option name=\"psk3d:";
    printOptionDetails = printDetailedOptions->Options->Lookup(ref new String(L"Job3DAddSupports"));
    pageContent = safe_cast<String^>(printOptionDetails->Value);
    if (0 == wcscmp(pageContent->Data(), L"Job3DSupportsExcluded"))
    {
        output += "SupportsExcluded";
    }
    else if (0 == wcscmp(pageContent->Data(), L"Job3DSupportsIncluded"))
    {
        output += "SupportsIncluded";
    }
    output += "\"/></psf:Feature>";

    // Set the raft setting
    output += "<psf:Feature name=\"psk3d:Job3DRaft\"><psf:Option name=\"psk3d:";
    printOptionDetails = printDetailedOptions->Options->Lookup(ref new String(L"Job3DAddRaft"));
    pageContent = safe_cast<String^>(printOptionDetails->Value);
    if (0 == wcscmp(pageContent->Data(), L"Job3DRaftExcluded"))
    {
        output += "RaftExcluded";
    }
    else if (0 == wcscmp(pageContent->Data(), L"Job3DRaftIncluded"))
    {
        output += "RaftIncluded";
    }
    output += "\"/></psf:Feature></psf:PrintTicket>";

    // Return the PT
    return output;
}
