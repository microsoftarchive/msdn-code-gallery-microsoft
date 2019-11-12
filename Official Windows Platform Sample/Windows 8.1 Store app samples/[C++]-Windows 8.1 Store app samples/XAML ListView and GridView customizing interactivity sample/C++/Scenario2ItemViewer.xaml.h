//
// Scenario2ItemViewer.xaml.h
// Declaration of the Scenario2ItemViewer class
//

#pragma once

#include "Scenario2ItemViewer.g.h"
#include "SampleDataSource.h"

namespace SDKSample
{
    namespace ListViewInteraction
    {

        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario2ItemViewer sealed
        {
        public:
            Scenario2ItemViewer();
            void ShowTitle(SDKSample::ListViewInteractionSampleDataSource::Item^ item);
            void ShowSubtitle();
            void ShowImage();
            void ClearData();

        private:
            // reference to the data item that will be visualized by this ItemViewer
            SDKSample::ListViewInteractionSampleDataSource::Item^ _item;
        };
    }
}