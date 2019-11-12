/****************************** Module Header ******************************\
* Module Name:  DirectXPage.xaml.h
* Project:      CppWindowsStoreAppManipulate3DObjects
* Copyright (c) Microsoft Corporation.
*
* This sample shows how to pick and manipulate 3D object in Windows Store DirectX game app.
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

#include "DirectXPage.g.h"
#include "CubeRenderer.h"
#include "BasicTimer.h"

namespace CppWindowsStoreAppManipulate3DObjects
{
	/// <summary>
	/// A DirectX page that can be used on its own.  Note that it may not be used within a Frame.
	/// </summary>
	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class DirectXPage sealed
	{
	public:
		DirectXPage();



	private:
		void OnPointerPressed(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::PointerEventArgs^ args);
		void OnPointerMoved(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::PointerEventArgs^ args);
		void OnPointerReleased(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::PointerEventArgs^ args);		
		
		void OnWindowSizeChanged(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ args);
		void OnDpiChanged(Windows::Graphics::Display::DisplayInformation^ sender, Platform::Object^ args);
		void OnOrientationChanged(Windows::Graphics::Display::DisplayInformation^ sender, Platform::Object^ args);
		void OnDisplayContentsInvalidated(Windows::Graphics::Display::DisplayInformation^ sender, Platform::Object^ args);
		void OnRendering(Platform::Object^ sender, Platform::Object^ args);
		void OnHighContrastChanged(Windows::UI::ViewManagement::AccessibilitySettings^ sender, Platform::Object^ args);

		void RotateRB_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void RotateRB_Unchecked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void TranslateRB_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void TranslateRB_Unchecked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ScaleRB_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ScaleRB_Unchecked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

		Windows::Foundation::EventRegistrationToken m_eventToken;
		
		CubeRenderer^ m_cubeRenderer;

		Windows::Foundation::Point m_lastPoint;
		Windows::Foundation::Point m_currentPoint;
		bool m_isNeedUpdate;			//If the scene need to update.
		DirectX::XMFLOAT4X4 m_transform;//Transform matrix for the object
		TransformTypeEnum m_transType;	//Scale, rotate or translation
		Windows::Foundation::EventRegistrationToken m_pointerMoveToken;
		
		BasicTimer^ m_timer;

		// Get the system high contrast settings.
		Windows::UI::ViewManagement::AccessibilitySettings^ m_accessibilitySettings;

		float m_renderTargetColor[4];
		
	};
}
