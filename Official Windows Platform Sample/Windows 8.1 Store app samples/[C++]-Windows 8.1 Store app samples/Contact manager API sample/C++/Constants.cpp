//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace SDKSample;
using namespace SDKSample::ContactManager;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    { "Show contact card", "SDKSample.ContactManager.ScenarioShowContactCard" },
    { "Show contact card with delay loaded data", "SDKSample.ContactManager.ScenarioShowContactCardDelayLoad" }
}; 

Rect SDKSample::ContactManager::GetElementRect(FrameworkElement^ element)
{
    Windows::UI::Xaml::Media::GeneralTransform^ buttonTransform = element->TransformToVisual(nullptr);
    const Point pointOrig(0, 0);
    const Point pointTransformed = buttonTransform->TransformPoint(pointOrig);
    const Rect rect(pointTransformed.X,
              pointTransformed.Y,
              safe_cast<float>(element->ActualWidth),
              safe_cast<float>(element->ActualHeight));
    return rect;
}
