// Copyright (c) Microsoft. All rights reserved.

#include "pch.h"
#include "MainPage.xaml.h"
#include "SampleConfiguration.h"

using namespace SDKSample;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    { "Use PeerFinder to connect to peers", "SDKSample.PeerFinderScenario" }, 
    { "Use PeerWatcher to scan for peers", "SDKSample.PeerWatcherScenario" },
    { "Use ProximityDevice to publish and subscribe for messages", "SDKSample.ProximityDeviceScenario" },
    { "Display ProximityDevice events", "SDKSample.ProximityDeviceEvents" }
}; 
