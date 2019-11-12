/****************************** Module Header ******************************\
* Module Name:  MainPage.xaml.h
* Project:      CppUniversalAppImageToVideo
* Copyright (c) Microsoft Corporation.
*
* This sample shows how to encode several images to a video using Media Foundation.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
#pragma once

#include "MainPage.g.h"
#include "ContinuationManager.h"
#include "PictureWriter.h"
namespace CppUniversalAppImageToVideo
{
	/// <summary>
	/// Implement IFileSavePickerContinuable interface, in order that Continuation Manager can automatically
	/// trigger the method to process returned file.
	/// </summary>
	public ref class MainPage sealed : IFileSavePickerContinuable, IFileOpenPickerContinuable
	{
	public:
		MainPage();
		/// <summary>
		/// Gets the view model for this <see cref="Page"/>. 
		/// This can be changed to a strongly typed view model.
		/// </summary>
		property Windows::Foundation::Collections::IObservableMap<Platform::String^, Platform::Object^>^ DefaultViewModel
		{
			Windows::Foundation::Collections::IObservableMap<Platform::String^, Platform::Object^>^  get();
		}

		/// <summary>
		/// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
		/// </summary>
		property Common::NavigationHelper^ NavigationHelper
		{
			Common::NavigationHelper^ get();
		}

		virtual void ContinueFileSavePicker(FileSavePickerContinuationEventArgs^ args);
		virtual void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs^ args);
	protected:
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
		
	private:
		void LoadState(Platform::Object^ sender, Common::LoadStateEventArgs^ e);
		static Windows::UI::Xaml::DependencyProperty^ _defaultViewModelProperty;
		static Windows::UI::Xaml::DependencyProperty^ _navigationHelperProperty;
		EncodeImage::PictureWriter^ m_picture;
		const int m_videoWidth;
		const int m_videoHeight;
		Platform::Collections::Vector<Windows::Storage::StorageFile^>^ m_files;
		
		void ImageBtn_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ImageGV_ItemClick(Platform::Object^ sender, Windows::UI::Xaml::Controls::ItemClickEventArgs^ e);
		void EncodeBtn_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		
		void Page_SizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e);
	};
}
