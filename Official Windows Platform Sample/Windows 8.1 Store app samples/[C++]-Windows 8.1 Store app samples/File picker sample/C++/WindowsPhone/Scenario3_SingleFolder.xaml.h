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
// Scenario3.xaml.h
// Declaration of the Scenario3 class
//

#pragma once

#include "pch.h"
#include "Scenario3_SingleFolder.g.h"
#include "MainPage.xaml.h"

#include "ContinuationManager.h"

namespace SDKSample
{
    namespace FilePicker
    {
        /// <summary>
        /// Implement IFolderPickerContinuable interface, in order that Continuation Manager can automatically
        /// trigger the method to process returned folder.
        /// </summary>
        public ref class Scenario3 sealed : IFolderPickerContinuable
        {
        public:
            Scenario3();
            virtual void ContinueFolderPicker(FolderPickerContinuationEventArgs^ args);

        private:
            MainPage^ rootPage;

            void PickFolderButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
