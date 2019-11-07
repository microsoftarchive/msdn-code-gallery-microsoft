//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include <math.h>
#include "Scenario1.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::PDFAPICPP;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace concurrency;
using namespace Windows::Data::Pdf;
using namespace Platform;


unsigned int PDF_PAGE_INDEX = 0; //first page
unsigned int ZOOM_FACTOR = 3; //300% zoom
Rect PDF_PORTION_RECT =  Rect(100, 100, 300, 400); //portion of a page
Platform::String^ PDFFILENAME = "Assets\\Windows_7_Product_Guide.pdf"; //Pdf File

PdfDocument^ _pdfDocument;

Scenario1::Scenario1()
{
    InitializeComponent();
}

IAsyncAction^ PDFAPICPP::Scenario1::PDFRenderToStreamAsync(PdfPage^ pdfPage,IRandomAccessStream^ randomStream,RENDEROPTIONS renderOptions)
{
    IAsyncAction^ pdfRenderToStreamOp;
    PdfPageRenderOptions^ pdfPageRenderOptions = ref new PdfPageRenderOptions();
    Size pdfPageSize;
    switch (renderOptions)
    {
        case RENDEROPTIONS::NORMAL:
            pdfRenderToStreamOp = pdfPage->RenderToStreamAsync(randomStream);
            break;
        case RENDEROPTIONS::ZOOM:
            pdfPageSize = pdfPage->Size;    
            //set PDFPageRenderOptions.DestinationWidth or DestinationHeight with expected zoom value
            pdfPageRenderOptions->DestinationHeight = (unsigned int) pdfPageSize.Height * ZOOM_FACTOR;                             
            pdfRenderToStreamOp = pdfPage->RenderToStreamAsync(randomStream,pdfPageRenderOptions);
            break;        
        case RENDEROPTIONS::PORTION:
            //Set PDFPageRenderOptions.SourceRect to render portion of a page
            pdfPageRenderOptions->SourceRect = PDF_PORTION_RECT;
            pdfRenderToStreamOp = pdfPage->RenderToStreamAsync(randomStream,pdfPageRenderOptions);
            break;
    }

     return pdfRenderToStreamOp;  
}

/// <summary>
/// This renders PDF page with render options 
/// Rendering a pdf page requires following 3 steps
///     1. PdfDocument::LoadFromFileAsync(pdfFile) which returns pdfDocument
///     2. pdfDocument->GetPage(pageIndex) 
///     3. pdfPage->RenderToStreamAsync(stream) or pdfPage->RenderToStreamAsync(stream,pdfPageRenderOptions)
/// </summary>
void PDFAPICPP::Scenario1::RenderPDFPage(Platform::String^ pdfFileName,RENDEROPTIONS renderOptions)
{
    create_task(Windows::ApplicationModel::Package::Current->InstalledLocation->GetFileAsync(PDFFILENAME)).then([this, renderOptions](StorageFile^ pdfFile)
    {
        //Load Pdf File
        create_task(PdfDocument::LoadFromFileAsync(pdfFile)).then([this, renderOptions](PdfDocument^ _pdfDocument)
        {    
            if (_pdfDocument != nullptr  && _pdfDocument->PageCount > 0)
            {
                //Get PDF Page
                PdfPage^ pdfPage = _pdfDocument->GetPage(PDF_PAGE_INDEX);

                if (pdfPage!= nullptr)
                {
                    StorageFolder^ tempFolder = ApplicationData::Current->TemporaryFolder;                    
                    create_task(tempFolder->CreateFileAsync("test1.png", CreationCollisionOption::ReplaceExisting)).then([this, pdfPage, _pdfDocument, renderOptions](StorageFile^ pngFile)
                    {
                        if (pngFile !=nullptr)
                        {
                            create_task(pngFile->OpenAsync(FileAccessMode::ReadWrite)).then([this, pdfPage, pngFile, _pdfDocument, renderOptions](IRandomAccessStream^ randomStream)
                            {
                                //Render PDF page to stream
                                create_task(PDFRenderToStreamAsync(pdfPage, randomStream, renderOptions)).then([this, randomStream, pngFile, renderOptions]()
                                {
                                    create_task(randomStream->FlushAsync()).then([this, randomStream, pngFile, renderOptions](bool success)
                                    {        
                                        delete randomStream;
                                        DisplayImageFileAsync(pngFile, renderOptions);
                                    });
                                });
                            });
                        }
                    });
                 }
              }
        });
    });
}

void PDFAPICPP::Scenario1::EnableRenderingButtons()
{
    RenderPageAtZoom->IsEnabled = true;
    RenderPortionOfPage->IsEnabled = true;
    RenderPage->IsEnabled = true;
}

void PDFAPICPP::Scenario1::DisableRenderingButtons()
{
    RenderPageAtZoom->IsEnabled = false;
    RenderPortionOfPage->IsEnabled = false;
    RenderPage->IsEnabled = false;
}

void PDFAPICPP::Scenario1::DisplayImageFileAsync(StorageFile^ file, RENDEROPTIONS ropt)
{  
    create_task(file->OpenAsync(FileAccessMode::Read)).then([this, ropt](IRandomAccessStream^ randomStreamNew)
    {
        BitmapImage^ src = ref new BitmapImage();
        src->SetSource(randomStreamNew);
        Image1->Source = src;
    }).then([this, ropt]() {
        switch (ropt)
        {
            case RENDEROPTIONS::NORMAL:
                MainPage::Current->NotifyUser("Rendered page with default options...", NotifyType::StatusMessage);
                break;
            case RENDEROPTIONS::ZOOM:
                MainPage::Current->NotifyUser("Rendered page at zoom level", NotifyType::StatusMessage);
                break;
            case RENDEROPTIONS::PORTION:
                MainPage::Current->NotifyUser("Rendered portion of page", NotifyType::StatusMessage);
                break;
        }
        EnableRenderingButtons();
    });
}

void PDFAPICPP::Scenario1::RenderDocument(RENDEROPTIONS renderOptions)
{
    DisableRenderingButtons();
    try
    {
        MainPage::Current->NotifyUser("Rendering page...", NotifyType::StatusMessage);
        RenderPDFPage(PDFFILENAME, renderOptions);
    }
    catch (int e)
    {
        MainPage::Current->NotifyUser("Exception occured" + e, NotifyType::StatusMessage);
        EnableRenderingButtons();
    }
}

void PDFAPICPP::Scenario1::RenderPage_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    RenderDocument(RENDEROPTIONS::NORMAL);
}

void PDFAPICPP::Scenario1::RenderPageZoom_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    RenderDocument(RENDEROPTIONS::ZOOM);
}

void PDFAPICPP::Scenario1::RenderPagePortion_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    RenderDocument(RENDEROPTIONS::PORTION);
}