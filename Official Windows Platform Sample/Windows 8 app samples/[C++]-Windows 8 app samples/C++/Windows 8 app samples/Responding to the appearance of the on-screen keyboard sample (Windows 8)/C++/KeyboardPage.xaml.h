//
// KeyboardPage.xaml.h
// Declaration of the KeyboardPage class
//

#pragma once

#include "pch.h"
#include "KeyboardPage.g.h"
#include "InputPaneHelper.h"

namespace KeyboardEventsSampleCPP
{
	enum ResizeType
	{
		NoResize = 0,
		ResizeFromShow = 1,
		ResizeFromHide = 2
	};
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public ref class KeyboardPage sealed
	{
    public:
        KeyboardPage();
	protected:
		virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    private:
        InputPaneHelper^ _inputPaneHelper;  
        double _displacement, _viewSize, _bottomOfList; 
		bool _resized;
		ResizeType _shouldResize;

		void CloseView_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void CustomKeyboardHandler(Object^ sender, Windows::UI::ViewManagement::InputPaneVisibilityEventArgs^ e);
        void ShowAnimationComplete(Object^ sender, Object^ e);
        void InputPaneHiding(Windows::UI::ViewManagement::InputPane^ sender, Windows::UI::ViewManagement::InputPaneVisibilityEventArgs^ e);
        void MiddleScroller_SizeChanged(Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e);
	};
}
