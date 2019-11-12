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
// StartStopTethering.xaml.cpp
// Implementation of the StartStopTethering class
//

#include "pch.h"
#include "StartStopTethering.xaml.h"

using namespace SDKSample::MobileBroadband;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Networking::NetworkOperators;
using namespace Windows::Networking::Connectivity;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Storage;
using namespace Windows::Foundation;
using namespace Platform;
using namespace concurrency;

StartStopTethering::StartStopTethering()
{
    InitializeComponent();
    sampleDispatcher = Window::Current->Dispatcher;
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void StartStopTethering::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    PrepareScenario();
}

/// <summary>
/// This function will make tethering manager available. If not, shows error messages
/// </summary>
void StartStopTethering::MakeTetheringManagerAvailable()
{
    if (tetheringManager == nullptr)
    {
        auto allAccounts = MobileBroadbandAccount::AvailableNetworkAccountIds;

        if (allAccounts->Size == 0)
        {
            rootPage->NotifyUser("No Mobile Broadband accounts found", NotifyType::ErrorMessage);
        }
        else
        {
            // verify tethering capabilities
            TetheringCapability capabilities = NetworkOperatorTetheringManager::GetTetheringCapability(allAccounts->GetAt(0));
            if (capabilities != TetheringCapability::Enabled)
            {
                // on Capability Error
                OnCapabilityError(capabilities);
            }
            else
            {
                tetheringManager = NetworkOperatorTetheringManager::CreateFromNetworkAccountId(allAccounts->GetAt(0));
            }
        }
    }
}

/// <summary>
/// Shows capability error messages
/// </summary>
void StartStopTethering::OnCapabilityError(TetheringCapability capabilities)
{

    switch (capabilities)
    {
    case TetheringCapability::DisabledByGroupPolicy:
        rootPage->NotifyUser("Your network administrator has disabled tethering on your machine.", NotifyType::ErrorMessage);
        break;
    case TetheringCapability::DisabledByHardwareLimitation:
        rootPage->NotifyUser("Your device hardware doesn't support tethering.", NotifyType::ErrorMessage);
        break;
    case TetheringCapability::DisabledByOperator:
        rootPage->NotifyUser("Your Mobile Broadband Operator has disabled tethering on your device.", NotifyType::ErrorMessage);
        break;
    case TetheringCapability::DisabledBySku:
        rootPage->NotifyUser("This version of Windows doesn't support tethering.", NotifyType::ErrorMessage);
        break;
    case TetheringCapability::DisabledByRequiredAppNotInstalled:
        rootPage->NotifyUser("Required app is not installed.", NotifyType::ErrorMessage);
        break;
    case TetheringCapability::DisabledDueToUnknownCause:
        rootPage->NotifyUser("Unknown issue.", NotifyType::ErrorMessage);
        break;
    }
}

/// <summary>
/// This will keep UI components updated with current tethering status
/// </summary>
void StartStopTethering::UpdateUI()
{
    if (tetheringManager != nullptr)
    {
        switch (tetheringManager->TetheringOperationalState)
        {
        case TetheringOperationalState::On:
            StartTetheringButton->IsEnabled = false;
            StopTetheringButton->IsEnabled = true;
            rootPage->NotifyUser(tetheringManager->ClientCount.ToString() +
                " of " +
                tetheringManager->MaxClientCount.ToString() +
                " are connected to your tethering network",
                NotifyType::StatusMessage);
            break;
        case TetheringOperationalState::Off:
            StartTetheringButton->IsEnabled = true;
            StopTetheringButton->IsEnabled = false;
            break;
        case TetheringOperationalState::InTransition:
            StartTetheringButton->IsEnabled = false;
            StopTetheringButton->IsEnabled = false;
            break;
        case TetheringOperationalState::Unknown:
            StartTetheringButton->IsEnabled = false;
            StopTetheringButton->IsEnabled = false;
            break;
        }
        Apply->IsEnabled = true;
        NetworkOperatorTetheringAccessPointConfiguration ^conf = tetheringManager->GetCurrentAccessPointConfiguration();
        if (conf != nullptr)
        {
            Passphrase->Text = conf->Passphrase;
            NetworkName->Text = conf->Ssid;
        }

    }
    else
    {
        StartTetheringButton->IsEnabled = false;
        StopTetheringButton->IsEnabled = false;
        Apply->IsEnabled = false;
    }
}

void StartStopTethering::PrepareScenario()
{
    rootPage->NotifyUser("", NotifyType::StatusMessage);

    try
    {
        // Register mobile operator notification completion handler
        auto iter = BackgroundTaskRegistration::AllTasks->First();
        auto hasCur = iter->HasCurrent;
        while (hasCur)
        {
            auto cur = iter->Current;
            if (cur->Value->Name == "MobileOperatorNotificationHandler")
            {
                cur->Value->Completed += ref new BackgroundTaskCompletedEventHandler(this, &StartStopTethering::OnCompleted);
            }
            hasCur = iter->MoveNext();
        }

        MakeTetheringManagerAvailable();
        UpdateUI();
    }
    catch (Platform::Exception^ ex)
    {
        rootPage->NotifyUser("Error:" + ex->Message, NotifyType::ErrorMessage);
    }
}

/// <summary>
/// Build string error messages from tethering operation error code
/// </summary>
String^ StartStopTethering::GetTetheringErrorString(TetheringOperationStatus errorCode)
{
    String^ errorString;
    switch (errorCode)
    {
    case TetheringOperationStatus::Success:
        errorString = "No error";
        break;
    case TetheringOperationStatus::Unknown:
        errorString = "Unknown error has occurred.";
        break;
    case TetheringOperationStatus::MobileBroadbandDeviceOff:
        errorString = "Please make sure your MB device is turned on.";
        break;
    case TetheringOperationStatus::WiFiDeviceOff:
        errorString = "Please make sure your WiFi device is turned on.";
        break;
    case TetheringOperationStatus::EntitlementCheckTimeout:
        errorString = "We coudn't contact your Mobile Broadband operator to verify your ability to enable tethering, please contact your Mobile Operator.";
        break;
    case TetheringOperationStatus::EntitlementCheckFailure:
        errorString = "You Mobile Broadband operator does not allow tethering on this device.";
        break;
    case TetheringOperationStatus::OperationInProgress:
        errorString = "The system is busy, please try again later.";
        break;
    }
    return errorString;

}

void StartStopTethering::StartTetheringButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    IAsyncOperation<NetworkOperatorTetheringOperationResult^> ^op = tetheringManager->StartTetheringAsync();
    auto startTetheringTask = create_task(op);
    startTetheringTask.then([this](task<NetworkOperatorTetheringOperationResult^> result)
    {
        try
        {
            auto opResult = result.get();
            if (opResult->Status != TetheringOperationStatus::Success)
            {
                String^ errorString;
                if (opResult->AdditionalErrorMessage != nullptr) {
                    errorString = opResult->AdditionalErrorMessage;
                }
                else
                {
                    errorString = GetTetheringErrorString(opResult->Status);
                }
                rootPage->NotifyUser("Error:" + errorString, NotifyType::ErrorMessage);
            }
            else
            {
                rootPage->NotifyUser("Operation succeeded", NotifyType::StatusMessage);
            }
        }
        catch (Exception^ e)
        {
            rootPage->NotifyUser("Unexpected exception:" + e->ToString(), NotifyType::ErrorMessage);
        }
    });
}


void StartStopTethering::StopTetheringButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    IAsyncOperation<NetworkOperatorTetheringOperationResult^> ^op = tetheringManager->StopTetheringAsync();
    auto startTetheringTask = create_task(op);
    startTetheringTask.then([this](task<NetworkOperatorTetheringOperationResult^> result)
    {
        try
        {
            auto opResult = result.get();
            if (opResult->Status != TetheringOperationStatus::Success)
            {
                String^ errorString;
                if (opResult->AdditionalErrorMessage != nullptr) {
                    errorString = opResult->AdditionalErrorMessage;
                }
                else
                {
                    errorString = GetTetheringErrorString(opResult->Status);
                }
                rootPage->NotifyUser("Error:" + errorString, NotifyType::ErrorMessage);
            }
            else
            {
                rootPage->NotifyUser("Operation succeeded", NotifyType::StatusMessage);
            }
        }
        catch (Exception^ e)
        {
            rootPage->NotifyUser("Unexpected exception:" + e->ToString(), NotifyType::ErrorMessage);
        }
    });
}


void StartStopTethering::Apply_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (tetheringManager != nullptr)
    {
        rootPage->NotifyUser("", NotifyType::StatusMessage);

        NetworkOperatorTetheringAccessPointConfiguration ^conf = ref new NetworkOperatorTetheringAccessPointConfiguration();
        conf->Passphrase = Passphrase->Text;
        conf->Ssid = NetworkName->Text;
        auto asyncOp = tetheringManager->ConfigureAccessPointAsync(conf);
        asyncOp->Completed = ref new AsyncActionCompletedHandler([=](IAsyncAction ^asyncAct, AsyncStatus status)
        {
            if (status == AsyncStatus::Completed)
            {
                sampleDispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler
                    ([this, asyncAct, status]()
                {
                    rootPage->NotifyUser("Operation succeeded", NotifyType::StatusMessage);
                }));
            }
            else if (status == AsyncStatus::Error)
            {
                sampleDispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler
                    ([this, asyncAct, status]()
                {
                    rootPage->NotifyUser("Operation failed. Error code: " + asyncAct->Status.ToString(), NotifyType::StatusMessage);
                }));
            }
            
        });
    }

}

//
// Handle background task completion event
//
void StartStopTethering::OnCompleted(BackgroundTaskRegistration^ sender, BackgroundTaskCompletedEventArgs^ e)
{
    //
    // Update the UI with progress reported by the background task
    //
    sampleDispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler
        ([this, sender, e]() {
            try
            {
                if ((sender != nullptr) && (e != nullptr))
                {
                    //
                    // This method throws if the event is reporting an error
                    //
                    e->CheckResult();

                    //
                    // Update the UI if the notification is for tethering
                    //
                    auto key = sender->TaskId.ToString() + "_type";
                    auto settings = ApplicationData::Current->LocalSettings;
                    String^ msgType = (String^)settings->Values->Lookup(key);

                    if ((msgType == NetworkOperatorEventMessageType::TetheringNumberOfClientsChanged.ToString()) ||
                        (msgType == NetworkOperatorEventMessageType::TetheringOperationalStateChanged.ToString()))
                    {
                        UpdateUI();
                    }
                }
            }
            catch (Platform::Exception^ ex)
            {
                rootPage->NotifyUser("Error: " + ex->ToString(), NotifyType::ErrorMessage);
            }
    }));
}
