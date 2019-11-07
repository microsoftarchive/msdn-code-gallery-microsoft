//
// ProjectionViewPage.xaml.h
// Declaration of the ProjectionViewPage class
//

#pragma once

#include "ProjectionViewPage.g.h"

namespace SDKSample
{
    namespace Projection
    {
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class ProjectionViewPage sealed
        {
        public:
            ProjectionViewPage();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            void SwapViews_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void StopProjecting_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void thisViewControl_Released(Platform::Object^ sender, Platform::Object^ e);
            SecondaryViewsHelpers::ViewLifetimeControl^ thisViewControl;
            Windows::UI::Core::CoreDispatcher^ mainDispatcher;
            int mainViewId;
            Windows::Foundation::EventRegistrationToken releasedToken;
        };
    }
}