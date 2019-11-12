//
// Scenario2ItemViewer.xaml.cpp
// Implementation of the Scenario2ItemViewer class
//

#include "pch.h"
#include "Scenario2ItemViewer.xaml.h"

using namespace SDKSample;
using namespace SDKSample::ListViewInteractionSampleDataSource;
using namespace Windows::UI::Xaml::Controls;


SDKSample::ListViewInteraction::Scenario2ItemViewer::Scenario2ItemViewer()
{
    InitializeComponent();
}

// Visualize the Title by updating the TextBlock for Title and setting Opacity
// to 1.
void SDKSample::ListViewInteraction::Scenario2ItemViewer::ShowTitle(Item^ item)
{
    _item = item;
    subtitleTextBlock->Opacity = 0;
    image->Opacity = 0;

    if (_item != nullptr)
    {
        titleTextBlock->Text = _item->Title;
    }
    titleTextBlock->Opacity = 1;
}

// Visualize subtitle information by updating the correct TextBlock and 
// setting Opacity to 1.
void SDKSample::ListViewInteraction::Scenario2ItemViewer::ShowSubtitle()
{
    if (_item != nullptr)
    {
        subtitleTextBlock->Text = _item->Subtitle;
    }
    subtitleTextBlock->Opacity = 1;
}

// Visualize the Image associated with the data item by updating the Image 
// object and setting Opacity to 1.
void SDKSample::ListViewInteraction::Scenario2ItemViewer::ShowImage()
{
    if (_item != nullptr)
    {
        image->Source = _item->Image;
    }
    image->Opacity = 1;
}

// Drop all refrences to the data item
void SDKSample::ListViewInteraction::Scenario2ItemViewer::ClearData()
{
    _item = nullptr;
    titleTextBlock->ClearValue(TextBlock::TextProperty);
    subtitleTextBlock->ClearValue(TextBlock::TextProperty);
    image->ClearValue(Image::SourceProperty);
}