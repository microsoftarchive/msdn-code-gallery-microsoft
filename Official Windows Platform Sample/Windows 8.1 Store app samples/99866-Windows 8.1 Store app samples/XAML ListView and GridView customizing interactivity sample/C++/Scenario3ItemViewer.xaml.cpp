//
// Scenario3ItemViewer.xaml.cpp
// Implementation of the Scenario3ItemViewer class
//

#include "pch.h"
#include "Scenario3ItemViewer.xaml.h"
#include "Scenario3.xaml.h"

using namespace SDKSample;
using namespace SDKSample::ListViewInteractionSampleDataSource;
using namespace Platform;
using namespace Windows::UI::Xaml::Controls;


SDKSample::ListViewInteraction::Scenario3ItemViewer::Scenario3ItemViewer()
{
    InitializeComponent();
}

// Visualize the Title by updating the TextBlock for Title and setting Opacity
// to 1.
void SDKSample::ListViewInteraction::Scenario3ItemViewer::ShowTitle(Item^ item)
{
    _item = item;
    contentTextBlock->Opacity = 0;

    if (_item != nullptr)
    {
        titleTextBlock->Text = _item->Title;
    }
    titleTextBlock->Opacity = 1;
}

// Visualize content information by updating the correct TextBlock and 
// setting Opacity to 1.
void SDKSample::ListViewInteraction::Scenario3ItemViewer::ShowContent()
{
    if (_item != nullptr)
    {
        contentTextBlock->Text = _item->Content;
    }
    contentTextBlock->Opacity = 1;
}

// Drop all refrences to the data item
void SDKSample::ListViewInteraction::Scenario3ItemViewer::ClearData()
{
    _item = nullptr;
    titleTextBlock->ClearValue(TextBlock::TextProperty);
    contentTextBlock->ClearValue(TextBlock::TextProperty);
}
