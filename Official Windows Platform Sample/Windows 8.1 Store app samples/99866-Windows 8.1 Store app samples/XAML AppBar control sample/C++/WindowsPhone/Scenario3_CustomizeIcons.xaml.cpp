//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "Scenario3_CustomizeIcons.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace Platform::Collections;
using namespace SDKSample::AppBarControl;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Controls;


Scenario3::Scenario3()
{
    InitializeComponent();
	rootPage = MainPage::Current;
}

void Scenario3::OnNavigatedTo(NavigationEventArgs^ e)
{
	// Create the AppBarButton for the 'Triangle' command
	AppBarButton^ triangle = ref new AppBarButton();
	triangle->Label = "Triangle";

	// This button will use the PathIcon class for its icon which allows us to 
	// use vector data to represent the icon
	PathIcon^ trianglePathIcon = ref new PathIcon();
	PathGeometry^ g = ref new PathGeometry();
	g->FillRule = FillRule::Nonzero;

	// Just create a simple triange shape
	PathFigure^ f = ref new PathFigure();
	f->IsFilled = true;
	f->IsClosed = true;
	f->StartPoint = Windows::Foundation::Point(20.0, 5.0);
	LineSegment^ s1 = ref new LineSegment();
	s1->Point = Windows::Foundation::Point(30.0, 30.0);
	LineSegment^ s2 = ref new LineSegment();
	s2->Point = Windows::Foundation::Point(10.0, 30.0);
	LineSegment^ s3 = ref new LineSegment();
	s3->Point = Windows::Foundation::Point(20.0, 5.0);
	f->Segments->Append(s1);
	f->Segments->Append(s2);
	f->Segments->Append(s3);
	g->Figures->Append(f);

	trianglePathIcon->Data = g;

	triangle->Icon = trianglePathIcon;

	CommandBar^ commandBar = safe_cast<CommandBar^>(this->BottomAppBar);
	commandBar->PrimaryCommands->InsertAt(2, triangle);
}

