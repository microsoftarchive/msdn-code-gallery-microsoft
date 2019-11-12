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
#include "PictureWriter.h"
namespace CppUniversalAppImageToVideo
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public ref class MainPage sealed
	{
	public:
		MainPage();

	private:
		void ImageBtn_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ImageGV_ItemClick(Platform::Object^ sender, Windows::UI::Xaml::Controls::ItemClickEventArgs^ e);
		void Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void EncodeBtn_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

		EncodeImage::PictureWriter^ m_picture;
		const int m_videoWidth;
		const int m_videoHeight;
		Platform::Collections::Vector<Windows::UI::Xaml::Controls::Image^>^ m_images;
		Platform::Collections::Vector<Windows::Storage::StorageFile^>^ m_files;
		Windows::Storage::StorageFile^ m_videoFile;

		void Page_SizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e);
		
	};
}
