// Copyright (c) Microsoft. All rights reserved.

#pragma once

#include <map>
#include "Scenario1_ExtractText.g.h"
#include "MainPage.xaml.h"

#if WINAPI_FAMILY==WINAPI_FAMILY_PHONE_APP
#include "ContinuationManager.h"
#endif

namespace OCR
{
    /// <summary>
    /// On Windows Phone, the page implements IFileOpenPickerContinuable to support
    /// Windows Phone-style continuable file pickers. This interface is invoked when
    /// the app is re-activated after the file picker process returns.
    /// For more information about continuable file pickers, see:
    /// http://go.microsoft.com/fwlink/?LinkId=393345
    /// </summary>
	[Windows::Foundation::Metadata::WebHostHidden]
    
#if WINAPI_FAMILY==WINAPI_FAMILY_PHONE_APP
    public ref class ExtractText sealed : IFileOpenPickerContinuable
#else
    public ref class ExtractText sealed
#endif
	{
	public:
		ExtractText();

#if WINAPI_FAMILY==WINAPI_FAMILY_PHONE_APP
        virtual void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs^ args);
#endif

	private:
        void Load_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void Sample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void ExtractText_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void Overlay_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void LanguageList_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);

        void ClearResults();
        void LoadImage(Windows::Storage::StorageFile^ file);
        byte* GetPointerToPixelData(Windows::Storage::Streams::IBuffer^ buffer);

        // A pointer back to main page.
        MainPage^ rootPage;

        // Bitmap holder of currently loaded image.
        Windows::UI::Xaml::Media::Imaging::WriteableBitmap^ bitmap;

        // OCR engine instance used to extract text from images.
        WindowsPreview::Media::Ocr::OcrEngine^ ocrEngine;

        // Mapping languages names and OcrLanguage enum.
        std::map<Platform::String^, WindowsPreview::Media::Ocr::OcrLanguage> languages;
    };
}
