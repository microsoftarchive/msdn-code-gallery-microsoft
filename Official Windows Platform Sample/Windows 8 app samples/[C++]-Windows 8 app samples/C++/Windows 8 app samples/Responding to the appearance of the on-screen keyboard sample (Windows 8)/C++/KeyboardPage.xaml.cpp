//
// KeyboardPage.xaml.cpp
// Implementation of the KeyboardPage class
//

#include "pch.h"
#include "KeyboardPage.xaml.h"

using namespace KeyboardEventsSampleCPP;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

KeyboardPage::KeyboardPage() : _displacement(0), _viewSize(0), _bottomOfList(0), _resized(false), _shouldResize(ResizeType::NoResize)
{
    InitializeComponent();

    // Each scrollable area should be large enough to demonstrate scrolling
    double listHeight = Window::Current->Bounds.Height * 2;
    LeftList->Height = listHeight;
    MiddleList->Height = listHeight;
    
    // InputPaneHelper is a custom class that allows keyboard event listeners to              
    // be attached to individual elements
    
    _inputPaneHelper = ref new InputPaneHelper();
    _inputPaneHelper->SubscribeToKeyboard(true);
    _inputPaneHelper->AddShowingHandler(CustomHandlingBox, ref new InputPaneShowingHandler(this, &KeyboardPage::CustomKeyboardHandler));  
    _inputPaneHelper->SetHidingHandler(ref new InputPaneHidingHandler(this, &KeyboardPage::InputPaneHiding));
}

void KeyboardPage::CloseView_Click(Object^ sender, RoutedEventArgs^ e) 
{ 
    this->Frame->GoBack();
}

void KeyboardPage::OnNavigatedFrom(NavigationEventArgs^ e)
{
	_inputPaneHelper->SubscribeToKeyboard(false);
	_inputPaneHelper->RemoveShowingHandler(CustomHandlingBox);
	_inputPaneHelper->SetHidingHandler(nullptr);
}

void KeyboardPage::CustomKeyboardHandler(Object^ sender, InputPaneVisibilityEventArgs^ e)  
{  
    // This function animates the middle scroll area up, then resizes the rest of              
    // the viewport. The order of operations is important to ensure that the user              
    // doesn't see a blank spot appear behind the keyboard              
    _viewSize = e->OccludedRect.Y;  

    // Keep in mind that other elements could be shifting out of your control. The sticky app bar, for example
    // will move on its own. You should make sure the input element doesn't get occluded by the bar
    _displacement = -e->OccludedRect.Height;  
    _bottomOfList = MiddleScroller->VerticalOffset + MiddleScroller->ActualHeight;  
  
    // Be careful with this property. Once it has been set, the framework will
    // do nothing to help you keep the focused element in view.
    e->EnsuredFocusedElementInView = true;  
  
    ShowingMoveSpline->Value = _displacement;  
    MoveMiddleOnShowing->Begin();
}

void KeyboardPage::ShowAnimationComplete(Object^ sender, Object^ e)  
{  
	// Once the animation completes, the app is resized 
	_shouldResize = ResizeType::ResizeFromShow;
	Container->SetValue(Grid::HeightProperty, _viewSize);
    MiddleTranslate->Y = 0; 
}  
  
void KeyboardPage::InputPaneHiding(InputPane^ sender, InputPaneVisibilityEventArgs^ e)  
{  
    if (_displacement != 0.0)  
    {  
		MoveMiddleOnShowing->Stop();

        // Keep in mind that other elements could be shifting out of your control. The sticky app bar, for example
        // will move on its own. You should make sure the input element doesn't get occluded by the bar
        _bottomOfList = MiddleScroller->VerticalOffset + MiddleScroller->ActualHeight; 
		
        // If the middle area has actually completed resize, then we want to ignore
		// the default system behavior
		if (_resized)
		{
			// Be careful with this property. Once it has been set, the framework will not change
			// any layouts in response to the keyboard coming up
			e->EnsuredFocusedElementInView = true;
		}

		// If the container has already been resized, it should be sized back to the right size
        // Otherwise, there's no need to change the height
		//
		// This piece of code checks if the height is NaN
		double computedHeight = dynamic_cast<IPropertyValue^>(Container->GetValue(Grid::HeightProperty))->GetDouble();
		if (computedHeight != computedHeight)
		{
			MoveMiddleOnHiding->Begin();		
		}
		else
		{
			_shouldResize = ResizeType::ResizeFromHide;
			// Clear the height property in order to return it to the default height defined in XAML
			Container->ClearValue(Grid::HeightProperty);  
		}
    }  
}  
  
void KeyboardPage::MiddleScroller_SizeChanged(Object^ sender, SizeChangedEventArgs^ e)  
{  
	// Scrolling should occur after the scrollable element has been resized to ensure              
    // that the items the user was looking at remain in view  
	if (_shouldResize == ResizeType::ResizeFromShow)
    {
		_resized = true;
		_shouldResize = ResizeType::NoResize;
		MiddleScroller->ScrollToVerticalOffset(_bottomOfList - MiddleScroller->ActualHeight);
	}
	else if (_shouldResize == ResizeType::ResizeFromHide)
    {
		_shouldResize = ResizeType::NoResize;
		MiddleTranslate->Y = _displacement;  
		MiddleScroller->ScrollToVerticalOffset(_bottomOfList - MiddleScroller->ActualHeight);  
  
		_displacement = 0;  
		_resized = false;
  
		MoveMiddleOnHiding->Begin();  
	}
}  
