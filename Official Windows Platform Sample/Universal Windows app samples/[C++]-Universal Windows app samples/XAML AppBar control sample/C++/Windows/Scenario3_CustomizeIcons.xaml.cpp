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
	leftItems = ref new Vector<UIElement^>();
	rightItems = ref new Vector<UIElement^>();
}

void Scenario3::OnNavigatedTo(NavigationEventArgs^ e)
{

	// Find the stack panels that host our AppBarButtons within the AppBar
	leftPanel = safe_cast<StackPanel^>(rootPage->FindName("LeftPanel"));
	rightPanel = safe_cast<StackPanel^>(rootPage->FindName("RightPanel"));

	CopyButtons(leftPanel, leftItems);
	CopyButtons(rightPanel, rightItems);

	// Remove existing AppBarButtons
	leftPanel->Children->Clear();
	rightPanel->Children->Clear();

	// Create the AppBarToggle button for the 'Shuffle' command
	AppBarToggleButton^ shuffle = ref new AppBarToggleButton();
	shuffle->Label = "Shuffle";
	shuffle->Icon = ref new SymbolIcon(Symbol::Shuffle);

	rightPanel->Children->Append(shuffle);

	// Create the AppBarButton for the 'Sun' command
	AppBarButton^ sun = ref new AppBarButton();
	sun->Label = "Sun";

	// This button will use the FontIcon class for its icon which allows us to choose
	// any glyph from any FontFamily
	FontIcon^ sunIcon = ref new FontIcon();
	sunIcon->FontFamily = ref new Windows::UI::Xaml::Media::FontFamily("Wingdings");
	sunIcon->FontSize = 30.0;
	sunIcon->Glyph = "\x52";
	sun->Icon = sunIcon;

	rightPanel->Children->Append(sun);

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

	rightPanel->Children->Append(triangle);

	// Create the AppBarButton for the 'Smiley' command
	AppBarButton^ smiley = ref new AppBarButton();
	smiley->Label = "Smiley";
	BitmapIcon^ smileyIcon = ref new BitmapIcon();
	smileyIcon->UriSource = ref new Windows::Foundation::Uri("ms-appx://Assets/smiley.png");
	smiley->Icon = smileyIcon;

	rightPanel->Children->Append(smiley);
}

void Scenario3::OnNavigatedFrom(NavigationEventArgs^ e)
{
	leftPanel->Children->Clear();
	rightPanel->Children->Clear();

	RestoreButtons(leftPanel, leftItems);
	RestoreButtons(rightPanel, rightItems);
}

void Scenario3::CopyButtons(StackPanel^ panel, Vector<UIElement^>^ list)
{
	for each(UIElement^ element in panel->Children)
	{
		list->Append(element);
	}
}

void Scenario3::RestoreButtons(StackPanel^ panel, Vector<UIElement^>^ list)
{
	for each(UIElement^ element in list)
	{
		panel->Children->Append(element);
	}
}
