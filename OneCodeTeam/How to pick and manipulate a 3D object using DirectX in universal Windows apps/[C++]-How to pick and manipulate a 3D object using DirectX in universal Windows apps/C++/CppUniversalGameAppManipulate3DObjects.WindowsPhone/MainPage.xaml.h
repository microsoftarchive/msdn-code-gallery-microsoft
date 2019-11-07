//
// MainPage.xaml.h
// Declaration of the MainPage class.
//

#pragma once

#include "MainPage.g.h"
#include "CubeRenderer.h"

namespace CppWindowsStoreAppManipulate3DObjects
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public ref class MainPage sealed
	{
	public:
		MainPage();

	protected:
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
		
	private:		
		CubeRenderer^ m_cubeRenderer;

		Windows::Foundation::Point m_lastPoint;
		Windows::Foundation::Point m_currentPoint;
		Windows::Foundation::EventRegistrationToken m_pointerMoveToken;
		bool m_isNeedUpdate;			//If the scene need to update.
		DirectX::XMFLOAT4X4 m_transform;//Transform matrix for the object
		TransformTypeEnum m_transType;	//Scale, rotate or translation
		float m_renderTargetColor[4];

		void OnWindowSizeChanged(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ args);
		void OnRendering(Platform::Object^ sender, Platform::Object^ args);
		void OnPointerPressed(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::PointerEventArgs^ args);
		void OnPointerMoved(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::PointerEventArgs^ args);
		void OnPointerReleased(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::PointerEventArgs^ args);
		void RotateRB_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void RotateRB_Unchecked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void TranslateRB_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void TranslateRB_Unchecked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ScaleRB_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ScaleRB_Unchecked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
	};
}
