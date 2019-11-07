#include "pch.h"
#include "PageWithParametersConfiguration.h"

using namespace SDKSample::Navigation;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization::NumberFormatting;
using namespace Windows::UI::Xaml::Media;

PageWithParametersConfiguration::PageWithParametersConfiguration()
{
	this->ID = _numberPages;
	_numberPages = _numberPages + 1;
}