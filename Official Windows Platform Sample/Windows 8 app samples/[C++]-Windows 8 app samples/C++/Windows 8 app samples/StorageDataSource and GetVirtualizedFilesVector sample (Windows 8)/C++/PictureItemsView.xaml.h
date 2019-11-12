//
// PictureItemsView.xaml.h
// Declaration of the PictureItemsView class
//

#pragma once

#include "pch.h"
#include "Common\LayoutAwarePage.h" // Required by generated header
#include "PictureItemsView.g.h"
#include "FileFolderTemplateSelector.h"
#include "ThumbnailConverter.h"

namespace SDKSample
{
    namespace DataSourceAdapter
    {
        /// <summary>
        /// A page that displays a collection of item previews.  In the Split Application this page
        /// is used to display and select one of the available groups.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class PictureItemsView sealed
        {
        public:
            PictureItemsView();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        };
    }
}
