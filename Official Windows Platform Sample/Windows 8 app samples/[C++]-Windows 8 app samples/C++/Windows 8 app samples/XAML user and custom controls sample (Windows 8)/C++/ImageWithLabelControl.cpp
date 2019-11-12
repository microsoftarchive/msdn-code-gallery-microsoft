//
// ImageWithLabelControl.cpp
// Implementation of the ImageWithLabelControl class.
//

#include "pch.h"
#include "ImageWithLabelControl.h"

using namespace SDKSample::UserAndCustomControls;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Documents;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

ImageWithLabelControl::ImageWithLabelControl()
{
	DefaultStyleKey = "SDKSample.UserAndCustomControls.ImageWithLabelControl";
}

DependencyProperty^ ImageWithLabelControl::_LabelProperty = DependencyProperty::Register("Label", Platform::String::typeid, ImageWithLabelControl::typeid, nullptr);
DependencyProperty^ ImageWithLabelControl::_ImagePathProperty = DependencyProperty::Register("ImagePath", ImageSource::typeid, ImageWithLabelControl::typeid, nullptr);
