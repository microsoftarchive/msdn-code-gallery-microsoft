//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2.xaml.h
// Declaration of the Scenario2 class
//

#pragma once
#include "S2_ListFolders.g.h"

namespace SDKSample
{
    namespace LibraryManagement
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class Scenario2 sealed
        {
        public:
            Scenario2();

        private:
            void ListFoldersButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void UpdateHeaderText();

            Windows::Storage::StorageLibrary^ picturesLibrary;
        };
    }
}
