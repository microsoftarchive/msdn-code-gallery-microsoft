//
// Scenario3ItemViewer.xaml.h
// Declaration of the Scenario3ItemViewer class
//

#pragma once

#include "Scenario3ItemViewer.g.h"
#include "SampleDataSource.h"


namespace SDKSample
{
    namespace ListViewInteraction
    {
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario3ItemViewer sealed
        {
        public:
            Scenario3ItemViewer();
            void ShowTitle(SDKSample::ListViewInteractionSampleDataSource::Item^ item);
            void ShowContent();
            void ClearData();

        private:
            // reference to the data item that will be visualized by this ItemViewer
            SDKSample::ListViewInteractionSampleDataSource::Item^ _item;
        };
    }
}

