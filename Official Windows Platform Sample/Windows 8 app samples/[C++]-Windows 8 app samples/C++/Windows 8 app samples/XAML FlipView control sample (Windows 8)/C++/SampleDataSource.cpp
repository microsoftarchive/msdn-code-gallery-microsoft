//*********************************************************

//
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"
#include "SampleDataSource.h"

using namespace SDKSample::FlipViewData;

SampleDataItem::SampleDataItem(Platform::String^ titile, Platform::String^ type, Platform::String^ picture):_title(titile),_type(type),_picture(picture)
{
}

