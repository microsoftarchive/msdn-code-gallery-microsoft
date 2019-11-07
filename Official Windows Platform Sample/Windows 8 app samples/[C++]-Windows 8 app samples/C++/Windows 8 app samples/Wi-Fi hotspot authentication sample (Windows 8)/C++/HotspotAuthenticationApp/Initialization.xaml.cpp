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
// Initialization.xaml.cpp
// Implementation of the Initialization class
//

#include "pch.h"
#include "Initialization.xaml.h"
#include "ScenarioCommon.h"

using namespace concurrency;
using namespace SDKSample::HotspotAuthenticationApp;
using namespace HotspotAuthenticationTask;
using namespace Platform;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Data::Xml::Dom;
using namespace Windows::Networking::NetworkOperators;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Initialization::Initialization()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Initialization::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    // Configure background task handler to perform authentication as default
    ConfigStore::AuthenticateThroughBackgroundTask = true;

    // Setup completion handler
    auto isTaskRegistered = ScenarioCommon::Instance->RegisteredCompletionHandlerForBackgroundTask();

    // Initialize button state
    UpdateButtonState(isTaskRegistered);
}

// This is the click handler for the 'Provision' button to provision the embedded XML file
void Initialization::ProvisionButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ProvisionButton->IsEnabled = false;

    // Open the installation folder
    auto installLocation = Windows::ApplicationModel::Package::Current->InstalledLocation;

    // Get the provisioning file
    task<StorageFile^> getFileTask(installLocation->GetFileAsync("ProvisioningData.xml"));
    getFileTask.then([this](StorageFile^ provisioningFile)
    {
        // Load with XML parser
        return XmlDocument::LoadFromFileAsync(provisioningFile);
    }).then([this](XmlDocument^ xmlDocument)
    {
        // Get raw XML
        auto provisioningXml = xmlDocument->GetXml();

        // Create ProvisionFromXmlDocumentResults Object
        auto provisioningAgent = ref new ProvisioningAgent();
        return provisioningAgent->ProvisionFromXmlDocumentAsync(provisioningXml);
    }).then([this](task<ProvisionFromXmlDocumentResults^> resultTask)
    {
        try
        {
            // Try getting all exceptions from the continuation chain above this point.
            // Get will throw an exception if the task failed with an error.
            auto result = resultTask.get();

            if (result->AllElementsProvisioned)
            {
                // Provisioning is done successfully
                rootPage->NotifyUser("Provisioning was successful", NotifyType::StatusMessage);
            }
            else
            {
                // Error has occured during provisioning
                rootPage->NotifyUser("Provisioning result: " + result->ProvisionResultsXml, NotifyType::ErrorMessage);
            }
        }
        catch (Exception^ ex)
        {
            // Handle errors
            rootPage->NotifyUser("Provisioning failed: " + ex->Message, NotifyType::ErrorMessage);
        }
        ProvisionButton->IsEnabled = true;
    });
}

// This is the click handler for the 'Register' button to registers a background task for
// the NetworkOperatorHotspotAuthentication event
void Initialization::RegisterButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        // Create a new background task builder.
        auto taskBuilder = ref new BackgroundTaskBuilder();

        // Create a new NetworkOperatorHotspotAuthentication trigger.
        auto trigger = ref new NetworkOperatorHotspotAuthenticationTrigger();

        // Associate the NetworkOperatorHotspotAuthentication trigger with the background task builder.
        taskBuilder->SetTrigger(trigger);

        // Specify the background task to run when the trigger fires.
        taskBuilder->TaskEntryPoint = ScenarioCommon::Instance->BackgroundTaskEntryPoint;

        // Name the background task.
        taskBuilder->Name = ScenarioCommon::Instance->BackgroundTaskName;

        // Register the background task.
        auto task = taskBuilder->Register();

        // Associate progress and completed event handlers with the new background task.
        task->Completed += ref new BackgroundTaskCompletedEventHandler(ScenarioCommon::Instance, &ScenarioCommon::OnBackgroundTaskCompleted);

        UpdateButtonState(true);
    }
    catch (Exception^ ex)
    {
        rootPage->NotifyUser(ex->ToString(), NotifyType::ErrorMessage);
    }
}

// This is the click handler for the 'Unregister' button
void Initialization::UnregisterButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    UnregisterBackgroundTask();
    UpdateButtonState(false);
}

// Unregister background task
void Initialization::UnregisterBackgroundTask()
{
    // Loop through all background tasks and unregister any.
    auto iter = BackgroundTaskRegistration::AllTasks->First();
    while(iter->HasCurrent)
    {
        auto task = iter->Current;
        if (task->Value->Name == ScenarioCommon::Instance->BackgroundTaskName)
        {
            task->Value->Unregister(true);
        }
        iter->MoveNext();
    }
}

// Update button state
void Initialization::UpdateButtonState(bool registered)
{
    if (registered)
    {
        RegisterButton->IsEnabled = false;
        UnregisterButton->IsEnabled = true;
    }
    else
    {
        RegisterButton->IsEnabled = true;
        UnregisterButton->IsEnabled = false;
    }
}
