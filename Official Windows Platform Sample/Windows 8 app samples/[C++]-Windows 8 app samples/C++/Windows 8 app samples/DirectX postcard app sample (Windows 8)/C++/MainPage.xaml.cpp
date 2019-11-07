//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "MainPage.xaml.h"
#include "EffectsFlyout.xaml.h"
#include "PaperFlyout.xaml.h"
#include "TextFlyout.xaml.h"
#include "SignatureFlyout.xaml.h"

using namespace Postcard;

using namespace concurrency;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::DataTransfer;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace Windows::Storage::Streams;
using namespace Windows::System;
using namespace Windows::UI::Core;
using namespace Windows::UI::Popups;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Microsoft::WRL;

MainPage::MainPage()
{
    InitializeComponent();

    m_renderer = ref new PostcardRenderer();
    m_renderer->Initialize(Window::Current->CoreWindow, this->DXSwapChainPanel, DisplayProperties::LogicalDpi);

    m_atStartScreen = true;

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &MainPage::OnLogicalDpiChanged);

    Window::Current->CoreWindow->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &MainPage::OnWindowSizeChanged);

    this->PointerPressed +=
        ref new PointerEventHandler(this, &MainPage::OnPointerPressed);
    this->PointerReleased +=
        ref new PointerEventHandler(this, &MainPage::OnPointerReleased);
    this->PointerMoved +=
        ref new PointerEventHandler(this, &MainPage::OnPointerMoved);

    this->ManipulationMode =
        ManipulationModes::TranslateX |
        ManipulationModes::TranslateY |
        ManipulationModes::Scale;

    this->ManipulationStarted +=
        ref new ManipulationStartedEventHandler(m_renderer, &PostcardRenderer::OnManipulationStarted);
    this->ManipulationCompleted +=
        ref new ManipulationCompletedEventHandler(m_renderer, &PostcardRenderer::OnManipulationCompleted);
    this->ManipulationDelta +=
        ref new ManipulationDeltaEventHandler(m_renderer, &PostcardRenderer::OnManipulationDelta);

    this->Tapped +=
        ref new TappedEventHandler(m_renderer, &PostcardRenderer::OnTapped);

    DataTransferManager::GetForCurrentView()->DataRequested +=
        ref new TypedEventHandler<DataTransferManager^, DataRequestedEventArgs^>(m_renderer, &PostcardRenderer::OnDataRequested);

    m_renderer->Render();
    m_renderer->Present();
}

void MainPage::OnLogicalDpiChanged(Object^ /* sender */)
{
    m_renderer->SetDpi(DisplayProperties::LogicalDpi);
    m_renderer->Render();
    m_renderer->Present();
}

void MainPage::OnWindowSizeChanged(CoreWindow^ /* sender */, WindowSizeChangedEventArgs^ args)
{
    if (ApplicationView::Value == ApplicationViewState::Snapped)
    {
        VisualStateManager::GoToState(this, "SnappedState", true);
    }
    else
    {
        if (m_atStartScreen)
        {
            VisualStateManager::GoToState(this, "StartingState", true);
        }
        else
        {
            VisualStateManager::GoToState(this, "ClearState", true);
        }
    }

    m_renderer->UpdateForViewStateChanged(ApplicationView::Value);
    m_renderer->UpdateForWindowSizeChange();
    m_renderer->Render();
    m_renderer->Present();
}

void MainPage::OnPointerPressed(Object^ sender, PointerRoutedEventArgs^ args)
{
    if (m_atStartScreen && ApplicationView::Value != ApplicationViewState::Snapped)
    {
        AddImage();
    }
    else
    {
        m_renderer->OnPointerPressed(sender, args);
    }
}

void MainPage::OnPointerReleased(Object^ sender, PointerRoutedEventArgs^ args)
{
    m_renderer->OnPointerReleased(sender, args);
}

void MainPage::OnPointerMoved(Object^ sender, PointerRoutedEventArgs^ args)
{
    m_renderer->OnPointerMoved(sender, args);
}

void MainPage::AddImage()
{
    FileOpenPicker^ picker = ref new FileOpenPicker();
    picker->SuggestedStartLocation = PickerLocationId::PicturesLibrary;
    picker->ViewMode = PickerViewMode::Thumbnail;
    picker->FileTypeFilter->Append(".png");
    picker->FileTypeFilter->Append(".jpg");
    picker->FileTypeFilter->Append(".bmp");

    create_task(picker->PickSingleFileAsync()).then([=](StorageFile^ file)
    {
        if (file == nullptr)
        {
            cancel_current_task();
        }
        return file->OpenAsync(FileAccessMode::Read);
    }).then([=](IRandomAccessStream^ randomAccessStream)
    {
        m_renderer->SetSampleMode(SampleMode::AddImage);
        m_renderer->LoadImage(randomAccessStream);
        VisualStateManager::GoToState(this, "ClearState", true);
        m_atStartScreen = false;
    });
}

void MainPage::AddEffectsClicked(Object^ sender, RoutedEventArgs^ e)
{
    GeneralTransform^ t = AddEffectsButtonBorder->TransformToVisual(this);
    auto point = t->TransformPoint(Point(0, 0));

    EffectsPopup->VerticalOffset = point.Y - EffectsFlyout->Height - 10;
    EffectsPopup->HorizontalOffset = point.X;
    EffectsPopup->IsOpen = true;

    m_renderer->SetSampleMode(SampleMode::AddEffect);
}

void MainPage::EffectIntensitySliderChanged(RangeBaseValueChangedEventArgs^ e)
{
    // Check for the renderer's existence, since this event can be fired before
    // m_renderer is initialized.
    if (m_renderer != nullptr)
    {
        m_renderer->SetEffectIntensity(static_cast<float>(e->NewValue));
    }
}

void MainPage::AddConstructionPaperClicked(Object^ sender, RoutedEventArgs^ e)
{
    GeneralTransform^ t = AddConstructionPaperButtonBorder->TransformToVisual(this);
    auto point = t->TransformPoint(Point(0, 0));

    PaperPopup->VerticalOffset = point.Y - PaperFlyout->Height - 10;
    PaperPopup->HorizontalOffset = point.X;
    PaperPopup->IsOpen = true;

    m_renderer->SetSampleMode(SampleMode::AddConstructionPaper);
}

void MainPage::PaperFlyoutAddPaperClicked()
{
    m_renderer->SetSampleMode(SampleMode::AddConstructionPaper);
    m_renderer->AddPaper();
}

void MainPage::PaperFlyoutDeletePaperClicked()
{
    m_renderer->SetSampleMode(SampleMode::AddConstructionPaper);
    m_renderer->RemovePaper();
}

void MainPage::PaperFlyoutMoveClicked()
{
    m_renderer->SetSampleMode(SampleMode::AddConstructionPaper);
    m_renderer->MovePaper();
}

void MainPage::PaperFlyoutStampClicked()
{
    m_renderer->SetSampleMode(SampleMode::AddConstructionPaper);
    m_renderer->StampPaper();
}

void MainPage::AddTextClicked(Object^ sender, RoutedEventArgs^ e)
{
    GeneralTransform^ t = AddTextButton->TransformToVisual(this);
    auto point = t->TransformPoint(Point(0, 0));

    TextPopup->VerticalOffset = point.Y - TextFlyout->Height - 10;
    TextPopup->HorizontalOffset = point.X;
    TextPopup->IsOpen = true;

    TextFlyout->Focus(::FocusState::Programmatic);

    m_renderer->SetSampleMode(SampleMode::AddText);
}

void MainPage::TextSubmitted(String^ text)
{
    m_renderer->SetExtrudedText(text);
}

void MainPage::AddSignatureClicked(Object^ sender, RoutedEventArgs^ e)
{
    GeneralTransform^ t = AddSignatureButtonBorder->TransformToVisual(this);
    auto point = t->TransformPoint(Point(0, 0));

    SignaturePopup->VerticalOffset = point.Y - SignatureFlyout->Height - 10;
    SignaturePopup->HorizontalOffset = point.X;
    SignaturePopup->IsOpen = true;
}

void MainPage::SignClicked()
{
    m_renderer->SetSampleMode(SampleMode::AddSignature);
    m_renderer->StartSignature();
}

void MainPage::EraseClicked()
{
    m_renderer->ResetSignature();
}

void MainPage::SaveClicked(Object^ sender, RoutedEventArgs^ e)
{
    FileSavePicker^ savePicker = ref new FileSavePicker();
    auto pngExtensions = ref new Vector<String^>();
    pngExtensions->Append(".png");
    savePicker->FileTypeChoices->Insert("PNG file", pngExtensions);
    auto jpgExtensions = ref new Vector<String^>();
    jpgExtensions->Append(".jpg");
    savePicker->FileTypeChoices->Insert("JPEG file", jpgExtensions);
    auto bmpExtensions = ref new Vector<String^>();
    bmpExtensions->Append(".bmp");
    savePicker->FileTypeChoices->Insert("BMP file", bmpExtensions);
    savePicker->DefaultFileExtension = ".png";
    savePicker->SuggestedFileName = "MyPostcard";
    savePicker->SuggestedStartLocation = Pickers::PickerLocationId::PicturesLibrary;

    std::shared_ptr<GUID> wicFormat = std::make_shared<GUID>(GUID_ContainerFormatPng);

    create_task(savePicker->PickSaveFileAsync()).then([=](StorageFile^ file)
    {
        if (file == nullptr)
        {
            cancel_current_task();
        }

        // Select the appropriate format based on the user-selected file extension.
        if (file->FileType == ".bmp")
        {
            *wicFormat = GUID_ContainerFormatBmp;
        }
        else if (file->FileType == ".jpg")
        {
            *wicFormat = GUID_ContainerFormatJpeg;
        }
        return file->OpenAsync(FileAccessMode::ReadWrite);
    }).then([=](Streams::IRandomAccessStream^ randomAccessStream)
    {
        m_renderer->SavePostcard(randomAccessStream, *wicFormat);
    });
}

void MainPage::ShareClicked(Object^ sender, RoutedEventArgs^ e)
{
    DataTransferManager::ShowShareUI();
}

void MainPage::ResetClicked(Object^ sender, RoutedEventArgs^ e)
{
    VisualStateManager::GoToState(this, "StartingState", true);

    m_renderer->SetSampleMode(SampleMode::Ready);
    m_renderer->ResetPostcard();
    m_atStartScreen = true;
}
