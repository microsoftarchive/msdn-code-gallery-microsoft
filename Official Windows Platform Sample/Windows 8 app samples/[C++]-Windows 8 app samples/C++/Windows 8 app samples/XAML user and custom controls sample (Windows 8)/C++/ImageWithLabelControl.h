//
// ImageWithLabelControl.h
// Declaration of the ImageWithLabelControl class.
//

using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Media;
using namespace Platform;
#pragma once

#include "pch.h"

namespace SDKSample
{
    namespace UserAndCustomControls
    {
    	[Windows::Foundation::Metadata::WebHostHidden]
    	public ref class ImageWithLabelControl sealed : public Control
    	{
    	private:
    		static DependencyProperty^ _LabelProperty;
    		static DependencyProperty^ _ImagePathProperty;
    
    	public:
    		ImageWithLabelControl();
    		static property DependencyProperty^ LabelProperty
    		{
    			DependencyProperty^ get() { return _LabelProperty; }
    		}
    
    		static property DependencyProperty^ ImagePathProperty
    		{
    			DependencyProperty^ get() { return _ImagePathProperty; }
    		}
    
    		property String^ Label
    		{
    			String^ get() { return (String^)GetValue(LabelProperty); }
    			void set(String^ value) { SetValue(LabelProperty, value); }
    		}
    		property ImageSource^ ImagePath
    		{
    			ImageSource^ get() { return (ImageSource^)GetValue(ImagePathProperty); }
    			void set(ImageSource^ value) { SetValue(ImagePathProperty, value); }
    		}
    	};
    }
}
