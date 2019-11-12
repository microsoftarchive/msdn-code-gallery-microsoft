//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// CachedFileUpdaterPage.xaml.h
// Declaration of the CachedFileUpdaterPage.xaml class.
//

#pragma once

#include "pch.h"
#include "CachedFileUpdaterPage.g.h"
#include "Common\LayoutAwarePage.h"
#include "Constants.h"

namespace SDKSample
{
    namespace FilePickerContracts
    {
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class CachedFileUpdaterPage sealed
        {
        public:
            CachedFileUpdaterPage();
            void Activate(Windows::ApplicationModel::Activation::CachedFileUpdaterActivatedEventArgs^ e);

            static property Platform::String^ FEATURE_NAME
            {
                Platform::String^ get()
                {
                    return ref new Platform::String(L"Cached File Updater Page");
                }
            }

            static property Platform::Array<Scenario>^ scenarios
            {
                Platform::Array<Scenario>^ get()
                {
                    if (CachedFileUpdaterPage::Current->cachedFileUpdaterUI->UpdateTarget == Windows::Storage::Provider::CachedFileTarget::Local)
                    {
                        return localScenariosInner;
                    }
                    else
                    {
                        return remoteScenariosInner;
                    }
                }
            }

        internal:
            property bool AutoSizeInputSectionWhenSnapped
            {
                bool get()
                {
                    return autoSizeInputSectionWhenSnapped;
                }

                void set(bool value)
                {
                    autoSizeInputSectionWhenSnapped = value;
                }
            }

            property Windows::ApplicationModel::Activation::LaunchActivatedEventArgs^ LaunchArgs
            {
                Windows::ApplicationModel::Activation::LaunchActivatedEventArgs^ get()
                {
                    return safe_cast<App^>(App::Current)->LaunchArgs;
                }
            }

            bool EnsureUnsnapped();
            void ResetScenarioOutput(Windows::UI::Xaml::Controls::TextBlock^ textblock);
            void NotifyUser(Platform::String^ strMessage, NotifyType type);
            void LoadScenario(Platform::String^ scenarioName);
            event Windows::Foundation::EventHandler<Platform::Object^>^ ScenarioLoaded;
            event Windows::Foundation::EventHandler<PageSizeChangedEventArgs^>^ PageResized;

        private:
            void PopulateScenarios();
            void InvalidateSize();
            void InvalidateViewState();
            void Footer_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            Platform::Collections::Vector<Object^>^ ScenarioList;
            Windows::UI::Xaml::Controls::Frame^ HiddenFrame;
            bool autoSizeInputSectionWhenSnapped;
            static Platform::Array<Scenario>^ localScenariosInner;
            static Platform::Array<Scenario>^ remoteScenariosInner;

        protected:
            void CachedFileUpdaterPage_SizeChanged(Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e);
            void Scenarios_SelectionChanged(Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);
            virtual void LoadState(Platform::Object^ navigationParameter,
                Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState) override;
            virtual void SaveState(Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState) override;

        private:
            void OnFileUpdateRequested(Windows::Storage::Provider::CachedFileUpdaterUI^ sender, Windows::Storage::Provider::FileUpdateRequestedEventArgs^ e);
            void OnUIRequested(Windows::Storage::Provider::CachedFileUpdaterUI^ sender, Platform::Object^ e);

        internal:
            static CachedFileUpdaterPage^ Current;
            Windows::Storage::Provider::FileUpdateRequest^ fileUpdateRequest;
            Windows::Storage::Provider::FileUpdateRequestDeferral^ fileUpdateRequestDeferral;
            Windows::Storage::Provider::CachedFileUpdaterUI^ cachedFileUpdaterUI;
        };
    }
}