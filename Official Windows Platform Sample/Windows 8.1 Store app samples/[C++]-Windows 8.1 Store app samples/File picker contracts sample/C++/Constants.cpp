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
#include "Constants.h"
#include "MainPage.xaml.h"
#include "FileOpenPickerPage.xaml.h"
#include "FileSavePickerPage.xaml.h"
#include "CachedFileUpdaterPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::Common;
using namespace SDKSample::FilePickerContracts;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Storage::Pickers::Provider;
using namespace Windows::Storage::Provider;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::ViewManagement;

String^ Status::FileAdded      = "File added to the basket.";
String^ Status::FileRemoved    = "File removed from the basket.";
String^ Status::FileAddFailed  = "Couldn't add file to the basket.";

Array<Scenario>^ MainPage::scenariosInner = ref new Array<Scenario>
{
    { "File Open Picker contract",    "SDKSample.FilePickerContracts.MainPage_PickFile" },
    { "File Save Picker contract",    "SDKSample.FilePickerContracts.MainPage_SaveFile" },
    { "Cached File Updater contract", "SDKSample.FilePickerContracts.MainPage_CachedFile" },
};

Array<Scenario>^ FileOpenPickerPage::scenariosInner = ref new Array<Scenario>
{
    { "Pick a file from the application package", "SDKSample.FilePickerContracts.FileOpenPicker_PickAppFile" },
    { "Pick a file from a URI",                   "SDKSample.FilePickerContracts.FileOpenPicker_PickURLFile" },
    { "Pick cached file",                         "SDKSample.FilePickerContracts.FileOpenPicker_PickCachedFile" },
};

Array<Scenario>^ FileSavePickerPage::scenariosInner = ref new Array<Scenario>
{
    { "Save a file to app's storage",    "SDKSample.FilePickerContracts.FileSavePicker_SaveToAppStorage" },
    { "Fail to save a file",             "SDKSample.FilePickerContracts.FileSavePicker_FailToSave" },
    { "Save to cached file",             "SDKSample.FilePickerContracts.FileSavePicker_SaveToCachedFile" },
};

Array<Scenario>^ CachedFileUpdaterPage::localScenariosInner = ref new Array<Scenario>
{
    { "Get latest version", "SDKSample.FilePickerContracts.CachedFileUpdater_Local" },
};

Array<Scenario>^ CachedFileUpdaterPage::remoteScenariosInner = ref new Array<Scenario>
{
    { "Remote file update", "SDKSample.FilePickerContracts.CachedFileUpdater_Remote" },
};
