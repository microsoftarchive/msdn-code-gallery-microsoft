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
// HomeGroupPicker.xaml.cpp
// Implementation of the HomeGroupPicker class
//

#include "pch.h"
#include "HomeGroupPickerScenario.xaml.h"

using namespace SDKSample::HomeGroup;

using namespace concurrency;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Storage::Pickers;
using namespace Windows::Storage;
using namespace Platform;

HomeGroupPicker::HomeGroupPicker()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void HomeGroupPicker::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

// Suggest that the picker opens in homegroup.  Show how the UNC path is returned.
void SDKSample::HomeGroup::HomeGroupPicker::Default_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ button = safe_cast<Button^>(sender);
    if (button != nullptr)
    {
        if (rootPage->EnsureUnsnapped())
        {
            auto picker = ref new FileOpenPicker();
            picker->ViewMode = PickerViewMode::Thumbnail;
            picker->SuggestedStartLocation = PickerLocationId::HomeGroup;
            picker->FileTypeFilter->Append("*");
            create_task(picker->PickSingleFileAsync()).then([this](StorageFile^ file)
            {
                if (file)
                {
                    // Application now has read/write access to the picked file
                    rootPage->NotifyUser("1 file selected: " + file->Path, NotifyType::StatusMessage);
                }
                else
                {
                    rootPage->NotifyUser("File was not returned", NotifyType::ErrorMessage);
                }
            });
        }
    }
}
