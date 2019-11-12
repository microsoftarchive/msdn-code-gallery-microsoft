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
// CachedFileUpdaterPage.xaml.cpp
// Implementation of the CachedFileUpdaterPage.xaml class.
//

#include "pch.h"
#include "CachedFileUpdaterPage.xaml.h"
#include "Constants.h"
#include "App.xaml.h"

#include <collection.h>

using namespace SDKSample;
using namespace SDKSample::Common;
using namespace SDKSample::FilePickerContracts;
using namespace Platform;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage::Provider;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Navigation;

CachedFileUpdaterPage^ CachedFileUpdaterPage::Current = nullptr;

CachedFileUpdaterPage::CachedFileUpdaterPage()
{
    InitializeComponent();

    // This frame is hidden, meaning it is never shown.  It is simply used to load
    // each scenario page and then pluck out the input and output sections and
    // place them into the UserControls on the main page.
    HiddenFrame = ref new Windows::UI::Xaml::Controls::Frame();
    HiddenFrame->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    LayoutRoot->Children->Append(HiddenFrame);

    FeatureName->Text = FEATURE_NAME;

    this->SizeChanged += ref new SizeChangedEventHandler(this, &CachedFileUpdaterPage::CachedFileUpdaterPage_SizeChanged);
    Scenarios->SelectionChanged += ref new SelectionChangedEventHandler(this, &CachedFileUpdaterPage::Scenarios_SelectionChanged);

    CachedFileUpdaterPage::Current = this;
    autoSizeInputSectionWhenSnapped = true;
}

/// <summary>
/// We need to handle SizeChanged so that we can make the sample layout property
/// in the various layouts.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void CachedFileUpdaterPage::CachedFileUpdaterPage_SizeChanged(Object^ sender, SizeChangedEventArgs^ e)
{
    InvalidateSize();
    PageSizeChangedEventArgs^ args = ref new PageSizeChangedEventArgs();
    args->ViewState = ApplicationView::Value;
    PageResized(this, args);
}

void CachedFileUpdaterPage::InvalidateSize()
{
    // Get the window width
    double windowWidth = this->ActualWidth;

    if (windowWidth != 0.0)
    {
        // Get the width of the ListBox.
        double listBoxWidth = Scenarios->ActualWidth;

        // Is the ListBox using any margins that we need to consider?
        double listBoxMarginLeft = Scenarios->Margin.Left;
        double listBoxMarginRight = Scenarios->Margin.Right;

        // Figure out how much room is left after considering the list box width
        double availableWidth = windowWidth - listBoxWidth;

        // Is the top most child using margins?
        double layoutRootMarginLeft = ContentRoot->Margin.Left;
        double layoutRootMarginRight = ContentRoot->Margin.Right;

        // We have different widths to use depending on the view state
        if (ApplicationView::Value != ApplicationViewState::Snapped)
        {
            // Make us as big as the the left over space, factoring in the ListBox width, the ListBox margins.
            // and the LayoutRoot's margins
            InputSection->Width = availableWidth - (layoutRootMarginLeft + layoutRootMarginRight + listBoxMarginLeft + listBoxMarginRight);
        }
        else
        {
            // Make us as big as the left over space, factoring in just the LayoutRoot's margins.
            if (autoSizeInputSectionWhenSnapped)
            {
                InputSection->Width = windowWidth - (layoutRootMarginLeft + layoutRootMarginRight);
            }
        }
    }
    InvalidateViewState();
}

void CachedFileUpdaterPage::InvalidateViewState()
{
    // Are we going to snapped mode?
    if (ApplicationView::Value == ApplicationViewState::Snapped)
    {
        Grid::SetRow(DescriptionText, 3);
        Grid::SetColumn(DescriptionText, 0);

        Grid::SetRow(InputSection, 4);
        Grid::SetColumn(InputSection, 0);

        Grid::SetRow(FooterPanel, 2);
        Grid::SetColumn(FooterPanel, 0);
    }
    else
    {
        Grid::SetRow(DescriptionText, 1);
        Grid::SetColumn(DescriptionText, 1);

        Grid::SetRow(InputSection, 2);
        Grid::SetColumn(InputSection, 1);

        Grid::SetRow(FooterPanel, 1);
        Grid::SetColumn(FooterPanel, 1);
    }

    //  Since we don't load the scenario page in the traditional manner (we just pluck out the
    // input and output sections from the page) we need to ensure that any VSM code used
    // by the scenario's input and output sections is fired.
    VisualStateManager::GoToState(InputSection, "Input" + LayoutAwarePage::DetermineVisualState(ApplicationView::Value), false);
    VisualStateManager::GoToState(OutputSection, "Output" + LayoutAwarePage::DetermineVisualState(ApplicationView::Value), false);
}

void CachedFileUpdaterPage::PopulateScenarios()
{
    ScenarioList = ref new Platform::Collections::Vector<Object^>();

    // Populate the ListBox with the list of scenarios as defined in Constants.cpp.
    for (unsigned int i = 0; i < scenarios->Length; ++i)
    {
        Scenario s = scenarios[i];
        ListBoxItem^ item = ref new ListBoxItem();
        item->Name = s.ClassName;
        item->Content = (i + 1).ToString() + ") " + s.Title;
        ScenarioList->Append(item);
    }

    // Bind the ListBox to the scenario list.
    Scenarios->ItemsSource = ScenarioList;
}

/// <summary>
/// This method is responsible for loading the individual input and output sections for each scenario.  This
/// is based on navigating a hidden Frame to the ScenarioX.xaml page and then extracting out the input
/// and output sections into the respective UserControl on the main page.
/// </summary>
/// <param name="scenarioName"></param>
void CachedFileUpdaterPage::LoadScenario(String^ scenarioName)
{
    autoSizeInputSectionWhenSnapped = true;

    // Load the ScenarioX.xaml file into the Frame.
    TypeName scenarioType = {scenarioName, TypeKind::Custom};
    HiddenFrame->Navigate(scenarioType, this);

    // Get the top element, the Page, so we can look up the elements
    // that represent the input and output sections of the ScenarioX file.
    Page^ hiddenPage = safe_cast<Page^>(HiddenFrame->Content);

    // Get each element.
    UIElement^ input = safe_cast<UIElement^>(hiddenPage->FindName("Input"));
    UIElement^ output = safe_cast<UIElement^>(hiddenPage->FindName("Output"));

    if (input == nullptr)
    {
        // Malformed input section.
        NotifyUser("Cannot load scenario input section for " + scenarioName +
            "  Make sure root of input section markup has x:Name of 'Input'", NotifyType::ErrorMessage);
        return;
    }

    if (output == nullptr)
    {
        // Malformed output section.
        NotifyUser("Cannot load scenario output section for " + scenarioName +
            "  Make sure root of output section markup has x:Name of 'Output'", NotifyType::ErrorMessage);
        return;
    }

    // Find the LayoutRoot which parents the input and output sections in the main page.
    Panel^ panel = safe_cast<Panel^>(hiddenPage->FindName("LayoutRoot"));

    if (panel != nullptr)
    {
        unsigned int index = 0;
        UIElementCollection^ collection = panel->Children;

        // Get rid of the content that is currently in the intput and output sections.
        collection->IndexOf(input, &index);
        collection->RemoveAt(index);

        collection->IndexOf(output, &index);
        collection->RemoveAt(index);

        // Populate the input and output sections with the newly loaded content.
        InputSection->Content = input;
        OutputSection->Content = output;

        ScenarioLoaded(this, nullptr);
    }
    else
    {
        // Malformed Scenario file.
        NotifyUser("Cannot load scenario: " + scenarioName + ".  Make sure root tag in the '" +
            scenarioName + "' file has an x:Name of 'LayoutRoot'", NotifyType::ErrorMessage);
    }
}

void CachedFileUpdaterPage::Scenarios_SelectionChanged(Object^ sender, SelectionChangedEventArgs^ e)
{
    if (Scenarios->SelectedItem != nullptr)
    {
        NotifyUser("", NotifyType::StatusMessage);

        LoadScenario((safe_cast<ListBoxItem^>(Scenarios->SelectedItem))->Name);
        InvalidateSize();
    }
}

void CachedFileUpdaterPage::NotifyUser(String^ strMessage, NotifyType type)
{
    switch (type)
    {
    case NotifyType::StatusMessage:
        // Use the status message style.
        StatusBlock->Style = safe_cast<Windows::UI::Xaml::Style^>(this->Resources->Lookup("StatusStyle"));
        break;
    case NotifyType::ErrorMessage:
        // Use the error message style.
        StatusBlock->Style = safe_cast<Windows::UI::Xaml::Style^>(this->Resources->Lookup("ErrorStyle"));
        break;
    default:
        break;
    }
    StatusBlock->Text = strMessage;

    // Collapsed the StatusBlock if it has no text to conserve real estate.
    if (StatusBlock->Text != "")
    {
        StatusBlock->Visibility = Windows::UI::Xaml::Visibility::Visible;
    }
    else
    {
        StatusBlock->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    }
}

void CachedFileUpdaterPage::Footer_Click(Object^ sender, RoutedEventArgs^ e)
{
    auto uri = ref new Uri((String^)((HyperlinkButton^)sender)->Tag);
    Windows::System::Launcher::LaunchUriAsync(uri);
}

/// <summary>
/// Populates the page with content passed during navigation.  Any saved state is also
/// provided when recreating a page from a prior session.
/// </summary>
/// <param name="navigationParameter">The parameter value passed to
/// <see cref="Frame::Navigate(Type, Object)"/> when this page was initially requested.
/// </param>
/// <param name="pageState">A map of state preserved by this page during an earlier
/// session.  This will be null the first time a page is visited.</param>
void CachedFileUpdaterPage::LoadState(Object^ navigationParameter, IMap<String^, Object^>^ pageState)
{
    (void) navigationParameter; // Unused parameter

    PopulateScenarios();

    // Starting scenario is the first or based upon a previous state.
    ListBoxItem^ startingScenario = nullptr;
    int startingScenarioIndex = -1;

    if (pageState != nullptr && pageState->HasKey("SelectedScenarioIndex"))
    {
        startingScenarioIndex = safe_cast<int>(pageState->Lookup("SelectedScenarioIndex"));
    }

    Scenarios->SelectedIndex = startingScenarioIndex != -1 ? startingScenarioIndex : 0;

    InvalidateViewState();
}

/// <summary>
/// Preserves state associated with this page in case the application is suspended or the
/// page is discarded from the navigation cache.  Values must conform to the serialization
/// requirements of <see cref="SuspensionManager::SessionState"/>.
/// </summary>
/// <param name="pageState">An empty map to be populated with serializable state.</param>
void CachedFileUpdaterPage::SaveState(IMap<String^, Object^>^ pageState)
{
    int selectedListBoxItemIndex = Scenarios->SelectedIndex;
    pageState->Insert("SelectedScenarioIndex", selectedListBoxItemIndex);
}

bool CachedFileUpdaterPage::EnsureUnsnapped()
{
    // FilePicker APIs will not work if the application is in a snapped state.
    // If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
    bool unsnapped = ((ApplicationView::Value != ApplicationViewState::Snapped) || ApplicationView::TryUnsnap());
    if (!unsnapped)
    {
        NotifyUser("Cannot unsnap the sample application", NotifyType::StatusMessage);
    }

    return unsnapped;
}

void CachedFileUpdaterPage::Activate(CachedFileUpdaterActivatedEventArgs^ args)
{
    // cache CachedFileUpdaterUI
    cachedFileUpdaterUI = args->CachedFileUpdaterUI;

    cachedFileUpdaterUI->FileUpdateRequested += ref new TypedEventHandler<CachedFileUpdaterUI^, FileUpdateRequestedEventArgs^>(this, &CachedFileUpdaterPage::OnFileUpdateRequested, CallbackContext::Same);
    cachedFileUpdaterUI->UIRequested += ref new TypedEventHandler<CachedFileUpdaterUI^, Platform::Object^>(this, &CachedFileUpdaterPage::OnUIRequested, CallbackContext::Same);

    Window::Current->Activate();
}

void CachedFileUpdaterPage::OnFileUpdateRequested(CachedFileUpdaterUI^ sender, FileUpdateRequestedEventArgs^ e)
{
    fileUpdateRequest = e->Request;
    fileUpdateRequestDeferral = fileUpdateRequest->GetDeferral();
    switch (cachedFileUpdaterUI->UIStatus)
    {
    case UIStatus::Hidden:
        fileUpdateRequest->Status = FileUpdateStatus::UserInputNeeded;
        fileUpdateRequestDeferral->Complete();
        break;
    case UIStatus::Visible:
        break;
    case UIStatus::Unavailable:
        fileUpdateRequest->Status = FileUpdateStatus::Failed;
        fileUpdateRequestDeferral->Complete();
        break;
    }
}

void CachedFileUpdaterPage::OnUIRequested(Windows::Storage::Provider::CachedFileUpdaterUI^ sender, Platform::Object^ e)
{
    Window::Current->Content = this;
    LoadState(nullptr, nullptr);
}
