// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// TouchableTextBlock.xaml.cpp
// Implementation of the TouchableTextBlock class
//

#include "pch.h"
#include "TouchableTextBlock.xaml.h"

using namespace SDKSample::TouchKeyboard;

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
using namespace Windows::UI::Xaml::Automation;            // needed for enum SupportedTextSelection in ITextProvider
using namespace Windows::UI::Xaml::Automation::Text;      // needed for TextPatternRangeEndpoint
using namespace Windows::UI::Xaml::Automation::Peers;     // needed for FrameworkElementAutomationPeer class
using namespace Windows::UI::Xaml::Automation::Provider;  // needed for ITextProvider and IValueProvider

namespace SDKSample
{
    namespace TouchKeyboard 
    {
    	/// <summary>
    	/// Automation Peer class for TouchableTextBlock.  
    	/// 
    	/// Note: The difference between this and NonTouchableTextBlockAutomationPeer is that this one implements
    	/// Text Pattern (ITextProvider) and Value Pattern (IValuePattern) interfaces.  So Touch keyboard shows 
    	/// automatically when user taps on the control with Touch or Pen.
    	/// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
    	public ref class TouchableTextBlockAutomationPeer sealed : FrameworkElementAutomationPeer, ITextProvider, IValueProvider
    	{
    	public:
    
    		TouchableTextBlockAutomationPeer(TouchableTextBlock^ owner)
    			: FrameworkElementAutomationPeer(owner)
    		{
    			this->textBlock = owner;
    			this->accClass = "TouchableTextBlock";
    		}
    	protected:
    		/// <summary>
    		/// Override GetPatternCore to return the object that supports the specified pattern.  In this case the Value pattern, Text
    		/// patter and any base class patterns.
    		/// </summary>
    		/// <param name="patternInterface"></param>
    		/// <returns>the object that supports the specified pattern</returns>
    		virtual Object^ GetPatternCore(PatternInterface patternInterface) override
    		{
    			if (patternInterface == PatternInterface::Value)
    			{
    				return this;
    			}
    			else if (patternInterface == PatternInterface::Text)
    			{
    				return this;
    			}
    			return FrameworkElementAutomationPeer::GetPatternCore(patternInterface);
    		}
    
    		/// <summary>
    		/// Override GetClassNameCore and set the name of the class that defines the type associated with this control.
    		/// </summary>
    		/// <returns>The name of the control class</returns>
    		virtual String^ GetClassNameCore() override
    		{
    			return this->accClass;
    		}
    	public:
    #pragma region ITextProvider_Implementation
    		// Complete implementation of the ITextPattern is beyond the scope of this sample.  The implementation provided
    		// is specific to this sample's custom control, so it is unlikely that they are directly transferable to other 
    		// custom control.
    
    		property ITextRangeProvider^ DocumentRange
    		{
    			// A real implementation of this method is beyond the scope of this sample.
    			// If your custom control has complex text involving both readonly and non-readonly ranges, 
    			// it will need a smarter implementation than just returning a fixed range
    			virtual ITextRangeProvider^ get() { return ref new TouchableTextBlockRangeProvider(textBlock->ContentText, this); }
    		}
    
    		virtual Array<ITextRangeProvider^>^ GetSelection()
    		{
    			return ref new Array<ITextRangeProvider^>(0);
    		}
    
    		virtual Array<ITextRangeProvider^>^ GetVisibleRanges()
    		{
    			auto ret = ref new Array<ITextRangeProvider^>(1);
    			ret[0] = ref new TouchableTextBlockRangeProvider(textBlock->ContentText, this);
    			return ret;
    		}
    
    		virtual ITextRangeProvider^ RangeFromChild(IRawElementProviderSimple^ childElement)
    		{
    			return ref new TouchableTextBlockRangeProvider(textBlock->ContentText, this);
    		}
    
    		virtual ITextRangeProvider^ RangeFromPoint(Point screenLocation)
    		{
    			return ref new TouchableTextBlockRangeProvider(textBlock->ContentText, this);
    		}
    
    		property SupportedTextSelection SupportedTextSelection
    		{
    			virtual Windows::UI::Xaml::Automation::SupportedTextSelection get() { return Windows::UI::Xaml::Automation::SupportedTextSelection::Single; }
    		}
    
    #pragma endregion ITextProvider_Implementation
    
    #pragma region IValueProvider_Implementation
    		// Complete implementation of the IValueProvider is beyond the scope of this sample.  The implementation provided
    		// is specific to this sample's custom control, so it is unlikely that they are directly transferable to other 
    		// custom control.
    
    		/// <summary>
    		/// The value needs to be false for the Touch keyboard to be launched automatically because Touch keyboard
    		/// does not appear when the input focus is in a readonly UI control.
    		/// </summary>
    		property bool IsReadOnly
    		{
    			virtual bool get() { return false; }
    		}
    
    		virtual void SetValue(String^ value)
    		{
    			textBlock->ContentText = value;
    		}
    
    		property String^ Value
    		{
    			virtual String^ get() { return textBlock->ContentText; }
    		}
    
    #pragma endregion IValueProvider_Implementation
    
    		IRawElementProviderSimple^ GetRawElementProviderSimple()
    		{
    			return ProviderFromPeer(this);
    		}
    
    	private:
    		TouchableTextBlock^ textBlock;
    		String^ accClass;
    	};
    
    }

TouchableTextBlockRangeProvider::TouchableTextBlockRangeProvider(String^ text, TouchableTextBlockAutomationPeer^ peer) : _text(text), _peer(peer)
{

}

void TouchableTextBlockRangeProvider::AddToSelection()
{

}

ITextRangeProvider^ TouchableTextBlockRangeProvider::Clone()
{
	return ref new TouchableTextBlockRangeProvider(_text, _peer);
}

bool TouchableTextBlockRangeProvider::Compare(ITextRangeProvider^ other)
{
	return true;
}

int TouchableTextBlockRangeProvider::CompareEndpoints(TextPatternRangeEndpoint endpoint, ITextRangeProvider^ targetRange, TextPatternRangeEndpoint targetEndpoint)
{
	return 0;
}

void TouchableTextBlockRangeProvider::ExpandToEnclosingUnit(TextUnit unit)
{

}

ITextRangeProvider^ TouchableTextBlockRangeProvider::FindAttribute(int attribute, Object^ value, bool backward)
{
	return this;
}

ITextRangeProvider^ TouchableTextBlockRangeProvider::FindText(String^ text, bool backward, bool ignoreCase)
{
	return this;
}

Object^ TouchableTextBlockRangeProvider::GetAttributeValue(int attribute)
{
	return this;
}

void TouchableTextBlockRangeProvider::GetBoundingRectangles(Array<double>^* rectangles)
{
	*rectangles = ref new Array<double>(0);
}

Array<IRawElementProviderSimple^>^ TouchableTextBlockRangeProvider::GetChildren()
{
	return ref new Array<IRawElementProviderSimple^>(0);
}

IRawElementProviderSimple^ TouchableTextBlockRangeProvider::GetEnclosingElement()
{
	return _peer->GetRawElementProviderSimple();
}

String^ TouchableTextBlockRangeProvider::GetText(int maxLength)
{
	auto textLength = _text->End() - _text->Begin();
	auto retLength = (maxLength < 0) ? textLength : min(maxLength, textLength);
	return ref new String(_text->Data(), retLength);
}

int TouchableTextBlockRangeProvider::Move(TextUnit unit, int count)
{
	return 0;
}

void TouchableTextBlockRangeProvider::MoveEndpointByRange(TextPatternRangeEndpoint endpoint, ITextRangeProvider^ targetRange, TextPatternRangeEndpoint targetEndpoint)
{

}

int TouchableTextBlockRangeProvider::MoveEndpointByUnit(TextPatternRangeEndpoint endpoint, TextUnit unit, int count)
{
	return 0;
}

void TouchableTextBlockRangeProvider::RemoveFromSelection()
{

}

void TouchableTextBlockRangeProvider::ScrollIntoView(bool alignToTop)
{

}

void TouchableTextBlockRangeProvider::Select()
{

}

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

/// <summary>
/// Loads the XAML UI contents and set properties required for this custom control.
/// </summary>
TouchableTextBlock::TouchableTextBlock()
{
	InitializeComponent();
	this->IsTabStop = true;
	this->IsTapEnabled = true;
	this->contentText = "";
}

/// <summary>
/// Create the Automation peer implementations for custom control (CustomInputBox2) to provide the accessibility support.
/// </summary>
/// <returns>Automation Peer implementation for this control</returns>
AutomationPeer^ TouchableTextBlock::OnCreateAutomationPeer()
{
	return ref new TouchableTextBlockAutomationPeer(this);
}

/// <summary>
/// Override the default event handler for GotFocus.
/// When the control got focus, indicate it has focus by highlighting the control by changing the background color to yellow.
/// </summary>
/// <param name="e">State information and event data associated with GotFocus event.</param>
void TouchableTextBlock::OnGotFocus(RoutedEventArgs^ e)
{
	this->myBorder->Background = ref new SolidColorBrush(Windows::UI::Colors::Yellow);
}

/// <summary>
/// Override the default event handler for LostFocus.
/// When the control lost focus, indicate it does not have focus by changing the background color to gray.
/// And the content is cleared.
/// </summary>
/// <param name="e">State information and event data associated with LostFocus event.</param>
void TouchableTextBlock::OnLostFocus(RoutedEventArgs^ e)
{
	this->myBorder->Background = ref new SolidColorBrush(Windows::UI::Colors::Gray);
	contentText = "";
	this->myTextBlock->Text = contentText;
}

/// <summary>
/// Override the default event handler for Tapped.
/// Set input focus to the control when tapped on.
/// </summary>
/// <param name="e">State information and event data associated with Tapped event.</param>
void TouchableTextBlock::OnTapped(TappedRoutedEventArgs^ e)
{
	this->Focus(Windows::UI::Xaml::FocusState::Pointer);
}

/// <summary>
/// Override the default event handler for KeyDown.
/// Displays the text "A key is pressed" and the approximate time when the key is pressed.
/// </summary>
/// <param name="e">State information and event data associated with KeyDown event.</param>
void TouchableTextBlock::OnKeyDown(KeyRoutedEventArgs^ e)
{
	auto currentCalendar = ref new Windows::Globalization::Calendar();
	String^ timeOfDay = currentCalendar->HourAsPaddedString(2) + ":" + 
		currentCalendar->MinuteAsPaddedString(2) + ":" + 
		currentCalendar->SecondAsPaddedString(2);
	contentText = "A key was pressed @ " + timeOfDay + " ";
	this->myTextBlock->Text = contentText;
}
}