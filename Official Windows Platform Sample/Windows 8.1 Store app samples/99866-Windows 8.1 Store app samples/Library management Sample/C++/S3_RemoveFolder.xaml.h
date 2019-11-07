//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario3.xaml.h
// Declaration of the Scenario3 class
//

#pragma once
#include "S3_RemoveFolder.g.h"

namespace SDKSample
{
    namespace LibraryManagement
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class Scenario3 sealed
        {
        public:
            Scenario3();

        private:
            void RemoveFolderButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            void GetPicturesLibrary();
            void FillComboBox();
            void UpdateControls();

            // The dispatcher for this page.  Because it is obtained from Window.Current, which is thread affine,
            // the dispatcher must be obtained from the UI thread this page will run.  It can then be referenced
            // from a background thread to run code that will update the UI.
            Windows::UI::Core::CoreDispatcher^ dispatcher;
            Windows::Storage::StorageLibrary^ picturesLibrary;
        };
    }
}
