//
// ItemViewer.xaml.h
// Declaration of the ItemViewer class
//

#pragma once

#include "ItemViewer.g.h"
#include "SampleDataSource.h"

namespace SDKSample
{
    namespace ListViewInteraction
    {
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class ItemViewer sealed
        {
        public:
            ItemViewer();
            void ShowPlaceholder(SDKSample::ListViewInteractionSampleDataSource::Item^ item);
            void ShowTitle();
            void ShowCategory();
            void ShowImage();
            void ClearData();

        private:
            // reference to the data item that will be visualized by this ItemViewer
            SDKSample::ListViewInteractionSampleDataSource::Item^ _item;

        };
    }
}
