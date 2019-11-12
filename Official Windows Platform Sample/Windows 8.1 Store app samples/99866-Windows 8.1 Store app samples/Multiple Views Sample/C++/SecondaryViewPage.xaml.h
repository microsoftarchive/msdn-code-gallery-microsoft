//
// SecondaryViewPage.xaml.h
// Declaration of the SecondaryViewPage class
// This page is shown in secondary views created by this app.
// See Scenario 1 for details on how to create a secondary view
//

#pragma once

#include "SecondaryViewPage.g.h"

namespace SDKSample
{
    namespace MultipleViews
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class SecondaryViewPage sealed
        {
        public:
            SecondaryViewPage();
            void HandleProtocolLaunch(Windows::Foundation::Uri^ uri);
            void SwitchAndAnimate(int fromViewId);
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            void SetTitle_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ClearTitle_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ProtocolLaunch_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void GoToMain_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void HideView_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void SetTitle(Platform::String^ newTitle);
            void View_Released(Platform::Object^ sender, Platform::Object^ e);
            Windows::UI::Xaml::Media::Animation::Storyboard^ CreateEnterAnimation();


            Windows::UI::Xaml::Media::Animation::Storyboard^ enterAnimation;
            Windows::UI::Core::CoreDispatcher^ mainDispatcher;
            int mainViewId;
            SecondaryViewsHelpers::ViewLifetimeControl^ thisViewControl;
            Windows::Foundation::EventRegistrationToken releasedToken;
        };
    }
}