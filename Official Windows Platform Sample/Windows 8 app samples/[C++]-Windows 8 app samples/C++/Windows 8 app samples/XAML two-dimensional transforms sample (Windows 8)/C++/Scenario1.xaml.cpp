//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"

using namespace SDKSample::Transforms;

using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Media;


Scenario1::Scenario1()
{
    InitializeComponent();
}

// Invoked when this page is about to be displayed in a Frame.
void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
	// A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
	rootPage = MainPage::Current;
    rootPage->AutoSizeInputSectionWhenSnapped = false;
	
	// Set up the bindings
	SetUpBinding(ScaleXSlider, CompositeTransform::ScaleXProperty);
	SetUpBinding(ScaleYSlider, CompositeTransform::ScaleYProperty);
	SetUpBinding(SkewXSlider, CompositeTransform::SkewXProperty);
	SetUpBinding(SkewYSlider, CompositeTransform::SkewYProperty);
	SetUpBinding(TranslateXSlider, CompositeTransform::TranslateXProperty);
	SetUpBinding(TranslateYSlider, CompositeTransform::TranslateYProperty);
	SetUpBinding(RotationSlider, CompositeTransform::RotationProperty);
}

// Handlder for resizing of MainPage
void SDKSample::Transforms::Scenario1::rootPage_MainPageResized(Platform::Object^ sender, MainPageSizeChangedEventArgs^ e)
{
	if(e->ViewState == Windows::UI::ViewManagement::ApplicationViewState::Snapped)
	{
		InputTextBlock1->MaxWidth = 300;
	}
}

// Sets up a binding
void SDKSample::Transforms::Scenario1::SetUpBinding(Windows::UI::Xaml::Controls::Slider^ slider, Windows::UI::Xaml::DependencyProperty^ dp)
{
	auto binding = ref new Binding();
	binding->Source = slider;
	binding->Path = ref new PropertyPath("Value");
	BindingOperations::SetBinding(ImageTransform, dp, binding);
}