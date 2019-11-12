// Copyright (c) Microsoft. All rights reserved.

#include "pch.h"
#include "Scenario2_GetTheParentFolderOfAFile.xaml.h"

using namespace SDKSample::FileAccess;

Scenario2::Scenario2() : rootPage(MainPage::Current)
{
    InitializeComponent();
    rootPage->Initialize();
    rootPage->NotifyUser("Windows Phone doesn’t currently support this function.", NotifyType::ErrorMessage);
}