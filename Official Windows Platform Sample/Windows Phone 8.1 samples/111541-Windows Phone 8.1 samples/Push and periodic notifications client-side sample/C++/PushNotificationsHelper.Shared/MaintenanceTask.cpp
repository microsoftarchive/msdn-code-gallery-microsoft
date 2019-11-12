//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "MaintenanceTask.h"
#include "Helper.h"

using namespace PushNotificationsHelper;

using namespace Concurrency;
using namespace Windows::ApplicationModel::Background;

void MaintenanceTask::Run(IBackgroundTaskInstance^ taskInstance)
{
	auto notifier = ref new Notifier();

	// It's important not to block UI threads. Since this is a background task, we do need
    // to block on the channel operations completing
	create_task(notifier->RenewAllAsync(false)).wait();
}
