// Copyright (c) Microsoft. All rights reserved.

#include "pch.h"
#include "Scenario9_CompareTwoFilesToSeeIfTheyAreTheSame.xaml.h"

using namespace SDKSample::FileAccess;

Scenario9::Scenario9() : rootPage(MainPage::Current)
{
    InitializeComponent();
    rootPage->Initialize();
    rootPage->NotifyUser("Windows Phone doesn’t currently support this function.", NotifyType::ErrorMessage);
}