// Copyright (c) Microsoft. All rights reserved.

#include "pch.h"
#include "Scenario11_TryToGetAFileWithoutGettingAnError.xaml.h"

using namespace SDKSample::FileAccess;

Scenario11::Scenario11() : rootPage(MainPage::Current)
{
    InitializeComponent();
    rootPage->Initialize();
    rootPage->NotifyUser("Windows Phone doesn’t currently support this function.", NotifyType::ErrorMessage);
}