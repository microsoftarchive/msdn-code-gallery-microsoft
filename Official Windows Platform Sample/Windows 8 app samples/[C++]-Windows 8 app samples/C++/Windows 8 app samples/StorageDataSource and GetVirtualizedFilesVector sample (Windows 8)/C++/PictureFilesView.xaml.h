//
// PictureFilesView.xaml.h
// Declaration of the PictureFilesView class
//

#pragma once

#include "pch.h"
#include "Common\LayoutAwarePage.h" // Required by generated header
#include "PictureFilesView.g.h"
#include "ThumbnailConverter.h"

namespace SDKSample
{
    namespace DataSourceAdapter
    {
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class PictureFilesView sealed
        {
        public:
            PictureFilesView();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        };
    }
}
