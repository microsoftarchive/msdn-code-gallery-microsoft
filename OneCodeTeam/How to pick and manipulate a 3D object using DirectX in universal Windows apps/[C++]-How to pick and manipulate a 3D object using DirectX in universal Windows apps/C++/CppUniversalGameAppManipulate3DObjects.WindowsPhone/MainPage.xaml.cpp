//
// MainPage.xaml.cpp
// Implementation of the MainPage class.
//

#include "pch.h"
#include "MainPage.xaml.h"

using namespace CppWindowsStoreAppManipulate3DObjects;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

using namespace Windows::Graphics::Display;
using namespace Windows::UI::Core;
using namespace concurrency;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

MainPage::MainPage()
{
	InitializeComponent();

	// Hide the status bar
	create_task(Windows::UI::ViewManagement::StatusBar::GetForCurrentView()->HideAsync());

	CompositionTarget::Rendering::add(ref new EventHandler<Object^>(this, &MainPage::OnRendering));

	m_cubeRenderer = ref new CubeRenderer();
	DisplayInformation^ currentDisplayInformation = DisplayInformation::GetForCurrentView();
	m_cubeRenderer->Initialize(
		Window::Current->CoreWindow,
		SwapChainPanel,
		currentDisplayInformation->LogicalDpi
		);

	Window::Current->CoreWindow->SizeChanged +=
		ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &MainPage::OnWindowSizeChanged);

	Window::Current->CoreWindow->PointerPressed +=
		ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &MainPage::OnPointerPressed);

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

void MainPage::OnRendering(Object^ sender, Object^ args)
{
	if (m_isNeedUpdate)
	{
		m_cubeRenderer->Update(m_transform);
		m_isNeedUpdate = false;
	}
	float color[] = { 1.0f, 1.0f, 1.0f };
	m_cubeRenderer->Render(m_renderTargetColor);
	m_cubeRenderer->Present();
}

void MainPage::OnWindowSizeChanged(CoreWindow^ sender, WindowSizeChangedEventArgs^ args)
{
	m_cubeRenderer->UpdateForWindowSizeChange();
	/*if (args->Size.Width <= 600)
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
	}*/
}

void MainPage::OnPointerPressed(CoreWindow^ sender, PointerEventArgs^ args)
{
	m_currentPoint = args->CurrentPoint->Position;
	Point pt = m_cubeRenderer->TransformToOrientation(m_currentPoint, true);
	bool isIntersects = m_cubeRenderer->IsIntersectsTriangle(
		pt.X,
		pt.Y);
	if (isIntersects)
	{
		m_pointerMoveToken = Window::Current->CoreWindow->PointerMoved +=
			ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &MainPage::OnPointerMoved);
		//sender->PointerCursor = ref new CoreCursor(CoreCursorType::SizeAll, 0);
	}
	m_lastPoint = m_currentPoint;
}

void MainPage::OnPointerMoved(CoreWindow^ sender, PointerEventArgs^ args)
{
	m_currentPoint = args->CurrentPoint->Position;

	m_transform = m_cubeRenderer->TransformWithMouse(m_transType, m_lastPoint.X, m_lastPoint.Y, 0.0f, m_currentPoint.X, m_currentPoint.Y, 0.0f);

	m_isNeedUpdate = true;

	Window::Current->CoreWindow->PointerReleased +=
		ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &MainPage::OnPointerReleased);
	m_lastPoint = m_currentPoint;
}
void MainPage::OnPointerReleased(CoreWindow^ sender, PointerEventArgs^ args)
{
	m_isNeedUpdate = false;
	//sender->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);
	sender->PointerMoved -= m_pointerMoveToken;
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.
/// This parameter is typically used to configure the page.</param>
void MainPage::OnNavigatedTo(NavigationEventArgs^ e)
{
	(void) e;	// Unused parameter

	// TODO: Prepare page for display here.

	// TODO: If your application contains multiple pages, ensure that you are
	// handling the hardware Back button by registering for the
	// Windows::Phone::UI::Input::HardwareButtons.BackPressed event.
	// If you are using the NavigationHelper provided by some templates,
	// this event is handled for you.
}


void CppWindowsStoreAppManipulate3DObjects::MainPage::RotateRB_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	m_transType = TransformTypeEnum::Rotate;
}


void CppWindowsStoreAppManipulate3DObjects::MainPage::RotateRB_Unchecked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	m_transType = TransformTypeEnum::None;
}


void CppWindowsStoreAppManipulate3DObjects::MainPage::TranslateRB_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	m_transType = TransformTypeEnum::Translate;
}


void CppWindowsStoreAppManipulate3DObjects::MainPage::TranslateRB_Unchecked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	m_transType = TransformTypeEnum::None;
}


void CppWindowsStoreAppManipulate3DObjects::MainPage::ScaleRB_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	m_transType = TransformTypeEnum::Scale;
}


void CppWindowsStoreAppManipulate3DObjects::MainPage::ScaleRB_Unchecked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	m_transType = TransformTypeEnum::None;
}


void CppWindowsStoreAppManipulate3DObjects::MainPage::Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{

}