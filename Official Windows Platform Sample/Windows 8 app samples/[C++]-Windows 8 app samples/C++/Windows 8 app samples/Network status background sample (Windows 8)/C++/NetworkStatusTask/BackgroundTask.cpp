// SampleBackgroundTask.cpp
#include "pch.h"
#include "BackgroundTask.h"
#include <Windows.h>

using namespace NetworkStatusTask;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::Networking::Connectivity;
using namespace Windows::ApplicationModel::Background;
using namespace Platform;

BackgroundTask::BackgroundTask()
{
}

BackgroundTask::~BackgroundTask()
{
}

//
//Helper for writing debug output to the connected debugger
//
void Debug(String^ message)
{
	String^ finalMessage = ref new String(message->Data());
	finalMessage += "\r\n";
	OutputDebugString(finalMessage->Data());
}

void BackgroundTask::Run(IBackgroundTaskInstance^ taskInstance)
{
	ApplicationDataContainer^ localSettings = ApplicationData::Current->LocalSettings;

	//
    // Associate a cancellation handler with the background task.
    //
    taskInstance->Canceled += ref new BackgroundTaskCanceledEventHandler(this, &BackgroundTask::OnCanceled);
	try
    {
		ConnectionProfile^ profile = NetworkInformation::GetInternetConnectionProfile();
        if (profile == nullptr)
        {
			localSettings->Values->Insert("InternetProfile", "Not connected to Internet");
			localSettings->Values->Insert("NetworkAdapterId", "Not connected to Internet");
        }
        else
        {
			localSettings->Values->Insert("InternetProfile", profile->ProfileName);

            if (profile->NetworkAdapter == nullptr)
            {
				localSettings->Values->Insert("NetworkAdapterId", "Not connected to Internet");
            }
            else
            {
				localSettings->Values->Insert("NetworkAdapterId", profile->NetworkAdapter->NetworkAdapterId.ToString());
            }
        }
    }
    catch (Exception^ ex)
    {
		Debug("Unhandled exception: " + ex->ToString());
    }
}

//
// Handles background task cancellation.
//
void BackgroundTask::OnCanceled(IBackgroundTaskInstance^ taskInstance, BackgroundTaskCancellationReason reason)
{
    //
    // Indicate that the background task is canceled.
    //
	Debug("Background" + taskInstance->Task->Name + "cancel requested...");
}
