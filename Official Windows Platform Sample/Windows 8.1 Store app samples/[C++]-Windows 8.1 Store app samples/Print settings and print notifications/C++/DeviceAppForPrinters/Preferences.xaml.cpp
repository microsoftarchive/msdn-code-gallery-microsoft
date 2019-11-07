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
// Preferences.xaml.cpp
// Implementation of the Preferences class
//

#include "pch.h"
#include "Preferences.xaml.h"

using namespace DeviceAppForPrinters;

using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Devices::Printers::Extensions;

Preferences::Preferences()
{
    InitializeComponent();

    // Get a copy of the dispatcher for later usage
    _dispatcher = Window::Current->Dispatcher;

    // Initialize feature list
    _features = ref new Platform::Collections::Vector<Platform::String^>;
    _features->Append("PageOrientation");
    _features->Append("PageOutputColor");
    _features->Append("PageMediaSize");
    _features->Append("PageMediaType");
    // Initialize _selection to unselected state (-1)
    _selections = ref new Platform::Collections::Vector<int>;
    _selections->Append(-1);
    _selections->Append(-1);
    _selections->Append(-1);
    _selections->Append(-1);
    _featureLabels = ref new Platform::Collections::Vector<TextBlock^>;
    _featureLabels->Append(PageOrientationLabel);
    _featureLabels->Append(PageOutputColorLabel);
    _featureLabels->Append(PageMediaSizeLabel);
    _featureLabels->Append(PageMediaTypeLabel);
    _featureBoxes = ref new Platform::Collections::Vector<ComboBox^>;
    _featureBoxes->Append(PageOrientationBox);
    _featureBoxes->Append(PageOutputColorBox);
    _featureBoxes->Append(PageMediaSizeBox);
    _featureBoxes->Append(PageMediaTypeBox);
    _featureConstraints = ref new Platform::Collections::Vector<TextBlock^>;
    _featureConstraints->Append(PageOrientationConstraint);
    _featureConstraints->Append(PageOutputColorConstraint);
    _featureConstraints->Append(PageMediaSizeConstraint);
    _featureConstraints->Append(PageMediaTypeConstraint);
}

void Preferences::Initialize()
{
    rootPage->AddMessage("Initialize Preference");
    Windows::Devices::Printers::Extensions::PrintTaskConfiguration^ configuration = rootPage->Configuration;
    DeviceAppForPrinters::PrintHelper^ printHelper = rootPage->PrintHelper;
    if ((configuration == nullptr) || (printHelper == nullptr))
    {
        rootPage->AddMessage("PrintTaskConfiguration is nullptr");
        return;
    }
    _saveRequestedToken = configuration->SaveRequested += 
        ref new Windows::Foundation::TypedEventHandler<PrintTaskConfiguration^, PrintTaskConfigurationSaveRequestedEventArgs^>(this, &Preferences::OnSaveRequested);

    // Fill in the drop-down select controls for some common printing features.
    for (unsigned int i = 0; i < _features->Size; i++)
    {
        String^ feature = _features->GetAt(i);

        // Only show features that exists
        TextBlock^ featureName = _featureLabels->GetAt(i);
        ComboBox^ featureBox = _featureBoxes->GetAt(i);
        featureName->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
        featureBox->Visibility = Windows::UI::Xaml::Visibility::Collapsed;

        // Check whether the currently selected printer's capabilities include this feature.
        if (!printHelper->FeatureExists(feature))
        {
            rootPage->AddMessage(feature + ": feature does not exist");
            continue;
        }

        // Fill in the labels so that they display the display name of each feature.
        featureName->Text = printHelper->GetFeatureDisplayName(feature);

        // Fill in the items for each feature
        featureBox->Items->Clear();
        printHelper->GetOptionInfo(feature, featureBox);
        rootPage->AddMessage(feature + ": DisplayName is " + featureName->Text);

        // Everytime the selection for a feature changes, we update our local cached set of selections.
        featureBox->SelectionChanged += ref new Windows::UI::Xaml::Controls::SelectionChangedEventHandler(this, &Preferences::OnFeatureOptionsChanged);

        // Show existing features
        featureName->Visibility = Windows::UI::Xaml::Visibility::Visible;
        featureBox->Visibility = Windows::UI::Xaml::Visibility::Visible;

        // By default there is no constraint
        TextBlock^ block = _featureConstraints->GetAt(i);
        block->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    }
}

// Notify user on UI thread
void Preferences::NotifyUser(Platform::String^ strMessage, NotifyType type)
{
    _dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this, strMessage, type] () {
        rootPage->NotifyUser(strMessage, type);
    }, Window::Current->CallbackContext::Any));
}

// Add debug message on UI thread
void Preferences::AddMessage(Platform::String^ strMessage)
{
    _dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this, strMessage] () {
        rootPage->AddMessage(strMessage);
    }, Window::Current->CallbackContext::Any));
}

// Hide or show the settings
void Preferences::AllowSettingsChange(bool bAllow)
{
    _dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this, bAllow] () {
        if (bAllow)
        {
            Input->Visibility = Windows::UI::Xaml::Visibility::Visible;
        }
        else
        {
            Input->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
        }
    }, Window::Current->CallbackContext::Any));
}

// Check if the selected option has constraint
void Preferences::CheckConstraint(void)
{
    _dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this] () {
        // Retrieve the existing print helper class
        PrintHelper^ printHelper = rootPage->PrintHelper;
        if (printHelper == nullptr)
        {
            rootPage->NotifyUser("CheckConstraint: printHelper cannot be null", NotifyType::ErrorMessage);
        }
        else
        {
            // Go through all the feature select elements and check their constraint
            for (unsigned int i = 0; i < _features->Size; i++)
            {
                ULONG index = _selections->GetAt(i);
                bool bResult = printHelper->IsOptionConstrained(_features->GetAt(i), index);
                AddMessage("Constraint: feature " + _features->GetAt(i) + " is " + bResult.ToString());
                TextBlock^ block = _featureConstraints->GetAt(i);
                if (nullptr != block && bResult)
                {		
                    block->Visibility = Windows::UI::Xaml::Visibility::Visible;
                }
            }
        }
    }, Window::Current->CallbackContext::Any));
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Preferences::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
    rootPage->AddMessage("Navigate to Preference");
    Initialize();

}

void Preferences::OnFeatureOptionsChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ args)
{
    ComboBox^ featureBox = safe_cast<ComboBox^>(sender);

    for (unsigned int i = 0; i < _features->Size; i++)
    {
        // Find the feature associated with the selected combo box using name matching
        if (_features->GetAt(i) + "Box" == featureBox->Name)
        {
            ComboBoxItem^ item = safe_cast<ComboBoxItem^>(featureBox->SelectedItem);
            // Save the new setting
            // Validation is delayed to when user press BACK button to avoid multiple validations
            _selections->SetAt(i, featureBox->SelectedIndex);
            rootPage->AddMessage(featureBox->Name + " changed to " + featureBox->SelectedIndex);
            // Remove the constrint error
            TextBlock^ block = _featureConstraints->GetAt(i);
            block->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
            break;
        }
    }
}

void Preferences::OnSaveRequested(Windows::Devices::Printers::Extensions::PrintTaskConfiguration^ sender, Windows::Devices::Printers::Extensions::PrintTaskConfigurationSaveRequestedEventArgs^ args)
{
    NotifyUser("Saving print settings", NotifyType::WaitMessage);
    AllowSettingsChange(false);

    // Retrieve the existing print helper class
    PrintHelper^ printHelper = rootPage->PrintHelper;
    Object^ context = rootPage->PrinterExtensionContext;
    if (printHelper == nullptr || context == nullptr || args == nullptr)
    {
        NotifyUser("onSaveRequested: args, printHelper, and context cannot be null", NotifyType::ErrorMessage);
        return;
    }

    // Get the request object, which has the save method that allows saving updated print settings
    _request = args->Request;

    if (_request == nullptr)
    {
        NotifyUser("onSaveRequested: request cannot be null", NotifyType::ErrorMessage);
        return;
    }    

    // Request for deferral to do asynchronous operation to save the ticket
    _deferral = _request->GetDeferral();

    // Go through all the feature select elements, look up the selected option name, and update the context
    for (unsigned int i = 0; i < _features->Size; i++)
    {
        // Set the feature's selected option in the context's print ticket
        ULONG value = _selections->GetAt(i);
        if (value >= 0)
        {
            printHelper->SetFeatureOption(_features->GetAt(i), value);
            AddMessage("update feature " + _features->GetAt(i) + " to " + value.ToString());
        }
    }

    try
    {
        // Save the print ticket through printer extension context
        AddMessage("Calling Save()...");
        if (_request != nullptr)
        {
            // This save request will throw an exception if ticket validation fails.
            // When the exception is thrown, the app flyout will remain.
            // If you want the flyout to remain regardless of outcome, you can call
            // _request->Cancel(). This should be used sparingly, however, as it could
            // disrupt the entire the print flow and will force the user to 
            // light dismiss to restart the entire experience.
            _request->Save(context);
        }
        AddMessage("Save() returned");

        // Remove the SaveRequested event handler
        Windows::Devices::Printers::Extensions::PrintTaskConfiguration^ configuration = rootPage->Configuration;
        if (configuration != nullptr)
        {
            configuration->SaveRequested -= _saveRequestedToken;
            AddMessage("SaveRequested event removed");
        }

        NotifyUser("Print settings saved", NotifyType::StatusMessage);
    }
    catch (Platform::Exception^ exception)
    {
        // Check the result to see if the exception is because of invalid print ticket
        if (exception->HResult == static_cast<int>(HRESULT_FROM_WIN32(ERROR_INVALID_DATA)))
        {
            NotifyUser("Failed to save the print ticket", NotifyType::ErrorMessage);
            CheckConstraint();
        }
        else
        {
            NotifyUser(exception->Message, NotifyType::ErrorMessage);
            throw;
        }
    }

    // The operation is complete, we complete the deferral
    if (_deferral != nullptr)
    {
        _deferral->Complete();
    }

    AllowSettingsChange(true);
}
