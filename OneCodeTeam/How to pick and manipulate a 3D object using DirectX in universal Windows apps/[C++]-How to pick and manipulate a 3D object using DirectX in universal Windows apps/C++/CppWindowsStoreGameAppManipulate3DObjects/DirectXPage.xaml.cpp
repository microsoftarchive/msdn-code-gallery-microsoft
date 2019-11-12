/****************************** Module Header ******************************\
* Module Name:  DirectXPage.xaml.cpp
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


#include "pch.h"
#include "DirectXPage.xaml.h"

using namespace CppWindowsStoreAppManipulate3DObjects;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Input;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

DirectXPage::DirectXPage() : m_isNeedUpdate(false)
{
	InitializeComponent();

	DirectX::XMStoreFloat4x4(&m_transform, DirectX::XMMatrixIdentity());

	m_cubeRenderer = ref new CubeRenderer();
	DisplayInformation^ currentDisplayInformation = DisplayInformation::GetForCurrentView();
	m_cubeRenderer->Initialize(
		Window::Current->CoreWindow,
		SwapChainPanel,
		currentDisplayInformation->LogicalDpi
		);

	m_accessibilitySettings =
		ref new Windows::UI::ViewManagement::AccessibilitySettings;

	m_accessibilitySettings->HighContrastChanged +=
		ref new TypedEventHandler<Windows::UI::ViewManagement::AccessibilitySettings^, Object^>(this, &DirectXPage::OnHighContrastChanged);

	if (m_accessibilitySettings->HighContrast)
	{
		LogoImage->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	}

	Window::Current->CoreWindow->SizeChanged += 
		ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &DirectXPage::OnWindowSizeChanged);

	Window::Current->CoreWindow->PointerPressed +=
		ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &DirectXPage::OnPointerPressed);
	
	currentDisplayInformation->DpiChanged +=
		ref new TypedEventHandler<DisplayInformation^, Object^>(this, &DirectXPage::OnDpiChanged);

	currentDisplayInformation->OrientationChanged +=
		ref new TypedEventHandler<DisplayInformation^, Object^>(this, &DirectXPage::OnOrientationChanged);

	DisplayInformation::DisplayContentsInvalidated +=
		ref new TypedEventHandler<DisplayInformation^, Object^>(this, &DirectXPage::OnDisplayContentsInvalidated);

	m_eventToken = CompositionTarget::Rendering::add(ref new EventHandler<Object^>(this, &DirectXPage::OnRendering));

	m_timer = ref new BasicTimer();

	Windows::UI::Xaml::Media::SolidColorBrush^ br =
		(Windows::UI::Xaml::Media::SolidColorBrush^)Windows::UI::Xaml::Application::Current->Resources->
		Lookup(
		"ApplicationPageBackgroundThemeBrush"
		);
	Windows::UI::Color Color = br->Color;
	m_renderTargetColor[0] = Color.R;
	m_renderTargetColor[1] = Color.G;
	m_renderTargetColor[2] = Color.B;
	m_renderTargetColor[3] = Color.A;
}
void DirectXPage::OnPointerPressed(CoreWindow^ sender, PointerEventArgs^ args)
{
	m_currentPoint = args->CurrentPoint->Position;
	Point pt = m_cubeRenderer->TransformToOrientation(m_currentPoint, true);
	bool isIntersects = m_cubeRenderer->IsIntersectsTriangle(
		pt.X,
		pt.Y);
	if (isIntersects)
	{
		m_pointerMoveToken = Window::Current->CoreWindow->PointerMoved +=
			ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &DirectXPage::OnPointerMoved);
		sender->PointerCursor = ref new CoreCursor(CoreCursorType::SizeAll, 0);
	}
	m_lastPoint = m_currentPoint;
}
void DirectXPage::OnPointerMoved(CoreWindow^ sender, PointerEventArgs^ args)
{
	m_currentPoint = args->CurrentPoint->Position;	
	
	m_transform = m_cubeRenderer->TransformWithMouse(m_transType, m_lastPoint.X, m_lastPoint.Y, 0.0f, m_currentPoint.X, m_currentPoint.Y, 0.0f);
	
	m_isNeedUpdate = true;
	
	Window::Current->CoreWindow->PointerReleased +=
		ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &DirectXPage::OnPointerReleased);
	m_lastPoint = m_currentPoint;
}
void DirectXPage::OnPointerReleased(CoreWindow^ sender, PointerEventArgs^ args)
{
	m_isNeedUpdate = false;
	sender->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);
	sender->PointerMoved -= m_pointerMoveToken;
}

void DirectXPage::OnHighContrastChanged(Windows::UI::ViewManagement::AccessibilitySettings^ sender, Object^ args)
{
	if (sender->HighContrast)
	{
		LogoImage->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	}
	else
	{
		LogoImage->Visibility = Windows::UI::Xaml::Visibility::Visible;
	}

	// Update render target color.
	Windows::UI::Xaml::Media::SolidColorBrush^ br =
		(Windows::UI::Xaml::Media::SolidColorBrush^)Windows::UI::Xaml::Application::Current->Resources->
		Lookup(
		"ApplicationPageBackgroundThemeBrush"
		);
	Windows::UI::Color Color = br->Color;
	m_renderTargetColor[0] = Color.R;
	m_renderTargetColor[1] = Color.G;
	m_renderTargetColor[2] = Color.B;
	m_renderTargetColor[3] = Color.A;
}

void DirectXPage::OnWindowSizeChanged(CoreWindow^ sender, WindowSizeChangedEventArgs^ args)
{	
	m_cubeRenderer->UpdateForWindowSizeChange();
	if (args->Size.Width <= 600)
	{
		VisualStateManager::GoToState(this, "MinimalLayout", true);
	}
	else if (args->Size.Width < args->Size.Height)
	{
		VisualStateManager::GoToState(this, "PortraitLayout", true);
	}
	else
	{
		VisualStateManager::GoToState(this, "DefaultLayout", true);
	}
}

void DirectXPage::OnDpiChanged(DisplayInformation^ sender, Object^ args)
{
	m_cubeRenderer->SetDpi(sender->LogicalDpi);
}

void DirectXPage::OnOrientationChanged(DisplayInformation^ sender, Object^ args)
{
	m_cubeRenderer->UpdateForWindowSizeChange();
}


void DirectXPage::OnDisplayContentsInvalidated(DisplayInformation^ sender, Object^ args)
{
	m_cubeRenderer->ValidateDevice();
}

void DirectXPage::OnRendering(Object^ sender, Object^ args)
{	
	if (m_isNeedUpdate)
	{
		m_cubeRenderer->Update(m_transform);
		m_isNeedUpdate = false;
	}
	
	m_cubeRenderer->Render(m_renderTargetColor);
	m_cubeRenderer->Present();
}

void CppWindowsStoreAppManipulate3DObjects::DirectXPage::RotateRB_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	m_transType = TransformTypeEnum::Rotate;	
}


void CppWindowsStoreAppManipulate3DObjects::DirectXPage::RotateRB_Unchecked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	m_transType = TransformTypeEnum::None;	
}


void CppWindowsStoreAppManipulate3DObjects::DirectXPage::TranslateRB_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	m_transType = TransformTypeEnum::Translate;
}


void CppWindowsStoreAppManipulate3DObjects::DirectXPage::TranslateRB_Unchecked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	m_transType = TransformTypeEnum::None;
}


void CppWindowsStoreAppManipulate3DObjects::DirectXPage::ScaleRB_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	m_transType = TransformTypeEnum::Scale;
}


void CppWindowsStoreAppManipulate3DObjects::DirectXPage::ScaleRB_Unchecked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	m_transType = TransformTypeEnum::None;
}


void CppWindowsStoreAppManipulate3DObjects::DirectXPage::Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{	
	Windows::System::Launcher::LaunchUriAsync(ref new Uri((String^)((HyperlinkButton^)sender)->Tag));
}
