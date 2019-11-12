//
// ItemViewer.xaml.h
// Declaration of the ItemViewer class
//

#pragma once

#include "ItemViewer.g.h"
#include "SampleDataSource.h"

using namespace SDKSample::ListViewSampleDataSource;

namespace SDKSample
{
	namespace ListViewSimple
	{
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class ItemViewer sealed
		{
		public:
			ItemViewer();
			void ShowPlaceholder(Item^ item);
			void ShowTitle();
			void ShowCategory();
			void ShowImage();
			void ClearData();

		private:
            // reference to the data item that will be visualized by this ItemViewer
			Item^ _item;

		};
	}
}
